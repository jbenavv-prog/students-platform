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
