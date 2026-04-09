variable "project_name" {
  type = string
}

variable "environment" {
  type = string
}

variable "aws_region" {
  type = string
}

variable "vpc_id" {
  type = string
}

variable "public_subnet_ids" {
  type = list(string)
}

variable "app_subnet_ids" {
  type = list(string)
}

variable "alb_security_group_id" {
  type = string
}

variable "app_security_group_id" {
  type = string
}

variable "db_connection_secret_arn" {
  type = string
}

variable "container_image" {
  type = string
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

variable "allowed_origins" {
  type = list(string)
}

variable "aspnetcore_environment" {
  type    = string
  default = "Production"
}

variable "alarm_actions" {
  type    = list(string)
  default = []
}

variable "tags" {
  type    = map(string)
  default = {}
}
