# Infraestructura y Deploy

Esta carpeta deja una base pragmatica para desplegar el backend monolitico modular en AWS usando:

- `Terraform` para IaC
- `ECR` para la imagen del backend
- `ECS Fargate` para ejecutar la API
- `ALB` para exponer HTTP
- `RDS PostgreSQL` para persistencia
- `GitHub Actions` para CI/CD

## Estructura

```text
infra/
  bootstrap/
    state/
  shared/
    ecr/
  live/
    envs/
      dev.tfvars
      qa.tfvars
      prod.tfvars
  modules/
    ecr/
    network/
    database/
    ecs_app/
    platform/
```

## Como convergen IaC y CI/CD

- `Terraform` define la plataforma deseada.
- `GitHub Actions` autentica con AWS via `OIDC`, construye la imagen y la publica en `ECR`.
- El deploy invoca `terraform apply` con el `image tag` nuevo para que la tarea de `ECS` quede alineada con la infraestructura versionada.

En otras palabras:

- `IaC` controla el estado de la plataforma.
- `CI/CD` mueve una nueva version de la aplicacion sobre esa plataforma.

## Que es OIDC

`OIDC` significa `OpenID Connect`.

En esta configuracion GitHub Actions le pide a GitHub un token firmado de corta duracion, AWS confia en ese proveedor de identidad y entrega credenciales temporales al workflow al asumir un rol IAM.

Ventajas:

- no guardas `AWS_ACCESS_KEY_ID` ni `AWS_SECRET_ACCESS_KEY` fijas en GitHub
- credenciales efimeras por ejecucion
- permisos acotados por repositorio, rama o environment

## Bootstrap Recomendado

### 1. Crear bucket de remote state

Ejecuta una vez, con estado local:

```powershell
terraform -chdir=infra/bootstrap/state init
terraform -chdir=infra/bootstrap/state apply -var "bucket_name=students-platform-tf-state"
```

### 2. Configurar variables en GitHub

Variables de repositorio o environment recomendadas:

- `AWS_REGION`
- `AWS_ROLE_TO_ASSUME`
- `TF_STATE_BUCKET`

### 3. Crear el rol IAM para OIDC

El rol debe confiar en el proveedor OIDC de GitHub y permitir:

- `ecr:*` sobre el repositorio necesario
- `ecs:*` sobre el cluster y servicio del proyecto
- `elasticloadbalancing:*` cuando aplique IaC
- `rds:*`, `ec2:*`, `logs:*`, `iam:*`, `secretsmanager:*`, `application-autoscaling:*` para Terraform
- `s3:*` sobre el bucket de state

En produccion conviene separar roles:

- uno para `plan`
- otro para `apply`

## Ambitos de esta base

Esta implementacion despliega el backend completo y sus dependencias.

El frontend Angular todavia no se publica desde esta base; una siguiente iteracion natural es `S3 + CloudFront` o `Amplify Hosting`.
