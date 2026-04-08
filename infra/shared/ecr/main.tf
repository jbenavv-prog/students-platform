locals {
  common_tags = {
    Project   = var.project_name
    ManagedBy = "terraform"
    Scope     = "shared"
  }
}

module "ecr" {
  source = "../../modules/ecr"

  repository_name = var.repository_name
  tags            = local.common_tags
}
