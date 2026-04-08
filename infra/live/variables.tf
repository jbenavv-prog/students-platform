variable "aws_region" {
  type    = string
  default = "us-east-1"
}

variable "project_name" {
  type    = string
  default = "students-platform"
}

variable "environment" {
  type = string
}

variable "container_image" {
  type    = string
  default = "public.ecr.aws/docker/library/nginx:stable-alpine"
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

variable "db_name" {
  type    = string
  default = "students_platform"
}

variable "db_username" {
  type    = string
  default = "postgres"
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

variable "frontend_allowed_origins" {
  type = list(string)
}
