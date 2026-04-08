output "vpc_id" {
  value = aws_vpc.this.id
}

output "public_subnet_ids" {
  value = [for key in sort(keys(aws_subnet.public)) : aws_subnet.public[key].id]
}

output "app_subnet_ids" {
  value = [for key in sort(keys(aws_subnet.app)) : aws_subnet.app[key].id]
}

output "db_subnet_ids" {
  value = [for key in sort(keys(aws_subnet.db)) : aws_subnet.db[key].id]
}
