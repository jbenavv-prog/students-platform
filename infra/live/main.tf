locals {
  common_tags = {
    Project     = var.project_name
    Environment = var.environment
    ManagedBy   = "terraform"
    Repository  = "students-platform"
  }
}

module "platform" {
  source = "../modules/platform"

  project_name             = var.project_name
  environment              = var.environment
  aws_region               = var.aws_region
  container_image          = var.container_image
  vpc_cidr                 = var.vpc_cidr
  public_subnet_cidrs      = var.public_subnet_cidrs
  app_subnet_cidrs         = var.app_subnet_cidrs
  db_subnet_cidrs          = var.db_subnet_cidrs
  use_nat_gateway          = var.use_nat_gateway
  db_name                  = var.db_name
  db_username              = var.db_username
  db_instance_class        = var.db_instance_class
  db_allocated_storage     = var.db_allocated_storage
  backup_retention_period  = var.backup_retention_period
  deletion_protection      = var.deletion_protection
  skip_final_snapshot      = var.skip_final_snapshot
  multi_az                 = var.multi_az
  container_port           = var.container_port
  desired_count            = var.desired_count
  app_enabled              = var.app_enabled
  min_capacity             = var.min_capacity
  max_capacity             = var.max_capacity
  cpu                      = var.cpu
  memory                   = var.memory
  health_check_path        = var.health_check_path
  assign_public_ip         = var.assign_public_ip
  enable_container_insights = var.enable_container_insights
  log_retention_days       = var.log_retention_days
  frontend_allowed_origins = var.frontend_allowed_origins
  alarm_actions            = var.alarm_actions
  create_alarm_topic       = var.create_alarm_topic
  alarm_email_subscriptions = var.alarm_email_subscriptions
  tags                     = local.common_tags
}
