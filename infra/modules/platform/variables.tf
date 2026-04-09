variable "project_name" {
  type = string
}

variable "environment" {
  type = string
}

variable "aws_region" {
  type = string
}

variable "container_image" {
  type = string
}

variable "vpc_cidr" {
  type = string
}

variable "public_subnet_cidrs" {
  type = list(string)
}

variable "app_subnet_cidrs" {
  type = list(string)
}

variable "db_subnet_cidrs" {
  type = list(string)
}

variable "use_nat_gateway" {
  type    = bool
  default = true
}

variable "db_name" {
  type = string
}

variable "db_username" {
  type = string
}

variable "db_instance_class" {
  type = string
}

variable "db_allocated_storage" {
  type = number
}

variable "backup_retention_period" {
  type = number
}

variable "deletion_protection" {
  type = bool
}

variable "skip_final_snapshot" {
  type = bool
}

variable "multi_az" {
  type = bool
}

variable "container_port" {
  type    = number
  default = 8080
}

variable "desired_count" {
  type = number
}

variable "app_enabled" {
  type    = bool
  default = true
}

variable "min_capacity" {
  type = number
}

variable "max_capacity" {
  type = number
}

variable "cpu" {
  type = number
}

variable "memory" {
  type = number
}

variable "health_check_path" {
  type    = string
  default = "/health"
}

variable "assign_public_ip" {
  type    = bool
  default = false
}

variable "enable_container_insights" {
  type    = bool
  default = false
}

variable "log_retention_days" {
  type    = number
  default = 7
}

variable "frontend_allowed_origins" {
  type = list(string)
}

variable "alarm_actions" {
  type    = list(string)
  default = []
}

variable "create_alarm_topic" {
  type    = bool
  default = false
}

variable "alarm_email_subscriptions" {
  type    = list(string)
  default = []
}

variable "tags" {
  type    = map(string)
  default = {}
}
