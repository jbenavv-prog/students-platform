variable "aws_region" {
  type    = string
  default = "us-east-1"
}

variable "bucket_name" {
  description = "Bucket S3 para remote state de Terraform."
  type        = string
}

variable "tags" {
  type    = map(string)
  default = {}
}
