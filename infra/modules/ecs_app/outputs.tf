output "alb_dns_name" {
  value = aws_lb.this.dns_name
}

output "cluster_name" {
  value = aws_ecs_cluster.this.name
}

output "service_name" {
  value = aws_ecs_service.this.name
}

output "task_definition_family" {
  value = aws_ecs_task_definition.this.family
}

output "alarm_names" {
  value = {
    target_5xx      = aws_cloudwatch_metric_alarm.target_5xx.alarm_name
    unhealthy_hosts = aws_cloudwatch_metric_alarm.unhealthy_hosts.alarm_name
    ecs_cpu_high    = aws_cloudwatch_metric_alarm.ecs_cpu_high.alarm_name
  }
}
