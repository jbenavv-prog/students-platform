output "alb_dns_name" {
  value = module.platform.alb_dns_name
}

output "cluster_name" {
  value = module.platform.cluster_name
}

output "service_name" {
  value = module.platform.service_name
}

output "task_definition_family" {
  value = module.platform.task_definition_family
}

output "db_endpoint" {
  value = module.platform.db_endpoint
}

output "alarm_names" {
  value = module.platform.alarm_names
}

output "alarm_topic_arn" {
  value = module.platform.alarm_topic_arn
}

output "frontend_bucket_name" {
  value = module.platform.frontend_bucket_name
}

output "frontend_distribution_id" {
  value = module.platform.frontend_distribution_id
}

output "frontend_distribution_domain_name" {
  value = module.platform.frontend_distribution_domain_name
}

output "frontend_url" {
  value = module.platform.frontend_url
}
