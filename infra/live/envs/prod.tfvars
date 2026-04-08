environment = "prod"

vpc_cidr = "10.50.0.0/16"
public_subnet_cidrs = [
  "10.50.0.0/24",
  "10.50.1.0/24"
]
app_subnet_cidrs = [
  "10.50.10.0/24",
  "10.50.11.0/24"
]
db_subnet_cidrs = [
  "10.50.20.0/24",
  "10.50.21.0/24"
]

db_instance_class       = "db.t3.small"
db_allocated_storage    = 50
backup_retention_period = 7
deletion_protection     = true
skip_final_snapshot     = false
multi_az                = true

desired_count = 2
min_capacity  = 2
max_capacity  = 4
cpu           = 1024
memory        = 2048

frontend_allowed_origins = [
  "https://students.example.com"
]
