environment = "dev"

vpc_cidr = "10.30.0.0/16"
public_subnet_cidrs = [
  "10.30.0.0/24",
  "10.30.1.0/24"
]
app_subnet_cidrs = [
  "10.30.10.0/24",
  "10.30.11.0/24"
]
db_subnet_cidrs = [
  "10.30.20.0/24",
  "10.30.21.0/24"
]

db_instance_class       = "db.t3.micro"
db_allocated_storage    = 20
backup_retention_period = 1
deletion_protection     = false
skip_final_snapshot     = true
multi_az                = false

desired_count = 1
min_capacity  = 1
max_capacity  = 2
cpu           = 512
memory        = 1024

frontend_allowed_origins = [
  "http://localhost:4200"
]

create_alarm_topic = true
alarm_email_subscriptions = []
