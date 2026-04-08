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
