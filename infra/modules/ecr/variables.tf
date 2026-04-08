variable "repository_name" {
  description = "Nombre del repositorio ECR compartido por los ambientes."
  type        = string
}

variable "tags" {
  description = "Tags comunes."
  type        = map(string)
  default     = {}
}
