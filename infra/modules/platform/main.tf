module "network" {
  source = "../network"

  project_name        = var.project_name
  environment         = var.environment
  vpc_cidr            = var.vpc_cidr
  public_subnet_cidrs = var.public_subnet_cidrs
  app_subnet_cidrs    = var.app_subnet_cidrs
  db_subnet_cidrs     = var.db_subnet_cidrs
  tags                = var.tags
}

resource "aws_security_group" "alb" {
  name        = "${var.project_name}-${var.environment}-alb-sg"
  description = "Public access to the application load balancer"
  vpc_id      = module.network.vpc_id

  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = merge(var.tags, {
    Name = "${var.project_name}-${var.environment}-alb-sg"
  })
}

resource "aws_security_group" "app" {
  name        = "${var.project_name}-${var.environment}-app-sg"
  description = "Application tasks access"
  vpc_id      = module.network.vpc_id

  ingress {
    from_port       = var.container_port
    to_port         = var.container_port
    protocol        = "tcp"
    security_groups = [aws_security_group.alb.id]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = merge(var.tags, {
    Name = "${var.project_name}-${var.environment}-app-sg"
  })
}

module "database" {
  source = "../database"

  project_name            = var.project_name
  environment             = var.environment
  vpc_id                  = module.network.vpc_id
  db_subnet_ids           = module.network.db_subnet_ids
  app_security_group_id   = aws_security_group.app.id
  db_name                 = var.db_name
  db_username             = var.db_username
  db_instance_class       = var.db_instance_class
  db_allocated_storage    = var.db_allocated_storage
  backup_retention_period = var.backup_retention_period
  deletion_protection     = var.deletion_protection
  skip_final_snapshot     = var.skip_final_snapshot
  multi_az                = var.multi_az
  tags                    = var.tags
}

resource "aws_sns_topic" "alarms" {
  count = var.create_alarm_topic ? 1 : 0

  name = "${var.project_name}-${var.environment}-alarms"

  tags = merge(var.tags, {
    Name = "${var.project_name}-${var.environment}-alarms"
  })
}

resource "aws_sns_topic_subscription" "alarm_email" {
  for_each = var.create_alarm_topic ? toset(var.alarm_email_subscriptions) : toset([])

  topic_arn = aws_sns_topic.alarms[0].arn
  protocol  = "email"
  endpoint  = each.value
}

locals {
  effective_alarm_actions = concat(
    var.alarm_actions,
    aws_sns_topic.alarms[*].arn)
}

module "ecs_app" {
  source = "../ecs_app"

  project_name             = var.project_name
  environment              = var.environment
  aws_region               = var.aws_region
  vpc_id                   = module.network.vpc_id
  public_subnet_ids        = module.network.public_subnet_ids
  app_subnet_ids           = module.network.app_subnet_ids
  alb_security_group_id    = aws_security_group.alb.id
  app_security_group_id    = aws_security_group.app.id
  db_connection_secret_arn = module.database.connection_secret_arn
  container_image          = var.container_image
  container_port           = var.container_port
  desired_count            = var.desired_count
  min_capacity             = var.min_capacity
  max_capacity             = var.max_capacity
  cpu                      = var.cpu
  memory                   = var.memory
  health_check_path        = var.health_check_path
  allowed_origins          = var.frontend_allowed_origins
  alarm_actions            = local.effective_alarm_actions
  tags                     = var.tags
}
