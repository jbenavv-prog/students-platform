output "alb_dns_name" {
  value = module.ecs_app.alb_dns_name
}

output "cluster_name" {
  value = module.ecs_app.cluster_name
}

output "service_name" {
  value = module.ecs_app.service_name
}

output "task_definition_family" {
  value = module.ecs_app.task_definition_family
}

output "db_endpoint" {
  value = module.database.db_endpoint
}

output "alarm_names" {
  value = module.ecs_app.alarm_names
}

output "alarm_topic_arn" {
  value = try(aws_sns_topic.alarms[0].arn, null)
}

output "frontend_bucket_name" {
  value = module.frontend_static.bucket_name
}

output "frontend_distribution_id" {
  value = module.frontend_static.distribution_id
}

output "frontend_distribution_domain_name" {
  value = module.frontend_static.distribution_domain_name
}

output "frontend_url" {
  value = module.frontend_static.frontend_url
}
