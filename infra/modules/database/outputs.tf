output "connection_secret_arn" {
  value = aws_secretsmanager_secret.connection.arn
}

output "db_endpoint" {
  value = aws_db_instance.this.address
}

output "db_security_group_id" {
  value = aws_security_group.db.id
}
