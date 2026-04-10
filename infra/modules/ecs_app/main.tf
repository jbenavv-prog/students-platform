data "aws_iam_policy_document" "ecs_task_assume_role" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["ecs-tasks.amazonaws.com"]
    }
  }
}

resource "aws_cloudwatch_log_group" "this" {
  name              = "/ecs/${var.project_name}-${var.environment}"
  retention_in_days = var.log_retention_days

  tags = var.tags
}

resource "aws_ecs_cluster" "this" {
  name = "${var.project_name}-${var.environment}"

  setting {
    name  = "containerInsights"
    value = var.enable_container_insights ? "enabled" : "disabled"
  }

  tags = var.tags
}

resource "aws_iam_role" "execution" {
  name               = "${var.project_name}-${var.environment}-ecs-execution"
  assume_role_policy = data.aws_iam_policy_document.ecs_task_assume_role.json

  tags = var.tags
}

resource "aws_iam_role" "task" {
  name               = "${var.project_name}-${var.environment}-ecs-task"
  assume_role_policy = data.aws_iam_policy_document.ecs_task_assume_role.json

  tags = var.tags
}

resource "aws_iam_role_policy_attachment" "execution_managed" {
  role       = aws_iam_role.execution.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

data "aws_iam_policy_document" "execution_secret_access" {
  statement {
    actions   = ["secretsmanager:GetSecretValue"]
    resources = [var.db_connection_secret_arn]
  }
}

resource "aws_iam_role_policy" "execution_secret_access" {
  name   = "${var.project_name}-${var.environment}-execution-secret-access"
  role   = aws_iam_role.execution.id
  policy = data.aws_iam_policy_document.execution_secret_access.json
}

resource "aws_lb" "this" {
  name               = substr("${var.project_name}-${var.environment}-alb", 0, 32)
  load_balancer_type = "application"
  internal           = false
  subnets            = var.public_subnet_ids
  security_groups    = [var.alb_security_group_id]

  tags = var.tags
}

resource "aws_lb_target_group" "this" {
  name        = substr("${var.project_name}-${var.environment}-tg", 0, 32)
  port        = var.container_port
  protocol    = "HTTP"
  target_type = "ip"
  vpc_id      = var.vpc_id

  health_check {
    enabled             = true
    path                = var.health_check_path
    matcher             = "200-399"
    interval            = 30
    timeout             = 5
    healthy_threshold   = 2
    unhealthy_threshold = 3
  }

  tags = var.tags
}

resource "aws_lb_listener" "http" {
  load_balancer_arn = aws_lb.this.arn
  port              = 80
  protocol          = "HTTP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.this.arn
  }
}

locals {
  effective_desired_count = var.app_enabled ? var.desired_count : 0
  effective_min_capacity  = var.app_enabled ? var.min_capacity : 0
  effective_max_capacity  = var.app_enabled ? var.max_capacity : 0

  container_environment = concat(
    [
      {
        name  = "ASPNETCORE_ENVIRONMENT"
        value = var.aspnetcore_environment
      }
    ],
    [
      for idx, origin in var.allowed_origins : {
        name  = "Cors__AllowedOrigins__${idx}"
        value = origin
      }
    ]
  )
}

resource "aws_ecs_task_definition" "this" {
  family                   = "${var.project_name}-${var.environment}"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = tostring(var.cpu)
  memory                   = tostring(var.memory)
  execution_role_arn       = aws_iam_role.execution.arn
  task_role_arn            = aws_iam_role.task.arn

  container_definitions = jsonencode([
    {
      name      = "api"
      image     = var.container_image
      essential = true
      portMappings = [
        {
          containerPort = var.container_port
          hostPort      = var.container_port
          protocol      = "tcp"
          appProtocol   = "http"
        }
      ]
      environment = local.container_environment
      secrets = [
        {
          name      = "ConnectionStrings__DefaultConnection"
          valueFrom = var.db_connection_secret_arn
        }
      ]
      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-group         = aws_cloudwatch_log_group.this.name
          awslogs-region        = var.aws_region
          awslogs-stream-prefix = "ecs"
        }
      }
    }
  ])

  tags = var.tags
}

resource "aws_ecs_service" "this" {
  name                              = "${var.project_name}-${var.environment}"
  cluster                           = aws_ecs_cluster.this.id
  task_definition                   = aws_ecs_task_definition.this.arn
  desired_count                     = local.effective_desired_count
  launch_type                       = "FARGATE"
  wait_for_steady_state             = true
  health_check_grace_period_seconds = 60

  deployment_circuit_breaker {
    enable   = true
    rollback = true
  }

  network_configuration {
    subnets          = var.app_subnet_ids
    security_groups  = [var.app_security_group_id]
    assign_public_ip = var.assign_public_ip
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.this.arn
    container_name   = "api"
    container_port   = var.container_port
  }

  depends_on = [aws_lb_listener.http]

  tags = var.tags
}

resource "aws_appautoscaling_target" "this" {
  count = var.app_enabled ? 1 : 0

  max_capacity       = local.effective_max_capacity
  min_capacity       = local.effective_min_capacity
  resource_id        = "service/${aws_ecs_cluster.this.name}/${aws_ecs_service.this.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"
}

resource "aws_appautoscaling_policy" "cpu" {
  count = var.app_enabled ? 1 : 0

  name               = "${var.project_name}-${var.environment}-cpu-scaling"
  policy_type        = "TargetTrackingScaling"
  resource_id        = aws_appautoscaling_target.this[0].resource_id
  scalable_dimension = aws_appautoscaling_target.this[0].scalable_dimension
  service_namespace  = aws_appautoscaling_target.this[0].service_namespace

  target_tracking_scaling_policy_configuration {
    predefined_metric_specification {
      predefined_metric_type = "ECSServiceAverageCPUUtilization"
    }

    target_value = 70
  }
}

resource "aws_cloudwatch_metric_alarm" "target_5xx" {
  alarm_name                = "${var.project_name}-${var.environment}-target-5xx"
  alarm_description         = "Triggers when the application behind the ALB returns 5xx responses."
  comparison_operator       = "GreaterThanOrEqualToThreshold"
  evaluation_periods        = 2
  metric_name               = "HTTPCode_Target_5XX_Count"
  namespace                 = "AWS/ApplicationELB"
  period                    = 60
  statistic                 = "Sum"
  threshold                 = 1
  treat_missing_data        = "notBreaching"
  alarm_actions             = var.alarm_actions
  ok_actions                = var.alarm_actions
  insufficient_data_actions = []

  dimensions = {
    LoadBalancer = aws_lb.this.arn_suffix
    TargetGroup  = aws_lb_target_group.this.arn_suffix
  }

  tags = var.tags
}

resource "aws_cloudwatch_metric_alarm" "unhealthy_hosts" {
  alarm_name                = "${var.project_name}-${var.environment}-unhealthy-hosts"
  alarm_description         = "Triggers when the ALB target group reports unhealthy hosts."
  comparison_operator       = "GreaterThanOrEqualToThreshold"
  evaluation_periods        = 2
  metric_name               = "UnHealthyHostCount"
  namespace                 = "AWS/ApplicationELB"
  period                    = 60
  statistic                 = "Average"
  threshold                 = 1
  treat_missing_data        = "notBreaching"
  alarm_actions             = var.alarm_actions
  ok_actions                = var.alarm_actions
  insufficient_data_actions = []

  dimensions = {
    LoadBalancer = aws_lb.this.arn_suffix
    TargetGroup  = aws_lb_target_group.this.arn_suffix
  }

  tags = var.tags
}

resource "aws_cloudwatch_metric_alarm" "ecs_cpu_high" {
  alarm_name                = "${var.project_name}-${var.environment}-ecs-cpu-high"
  alarm_description         = "Triggers when ECS service CPU utilization stays above 80%."
  comparison_operator       = "GreaterThanOrEqualToThreshold"
  evaluation_periods        = 3
  metric_name               = "CPUUtilization"
  namespace                 = "AWS/ECS"
  period                    = 60
  statistic                 = "Average"
  threshold                 = 80
  treat_missing_data        = "notBreaching"
  alarm_actions             = var.alarm_actions
  ok_actions                = var.alarm_actions
  insufficient_data_actions = []

  dimensions = {
    ClusterName = aws_ecs_cluster.this.name
    ServiceName = aws_ecs_service.this.name
  }

  tags = var.tags
}
