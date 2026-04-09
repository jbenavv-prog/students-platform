# Infraestructura y Deploy

Esta carpeta deja una base pragmatica para desplegar el backend monolitico modular en AWS usando:

- `Terraform` para IaC
- `ECR` para la imagen del backend
- `ECS Fargate` para ejecutar la API
- `ALB` para exponer HTTP
- `RDS PostgreSQL` para persistencia
- `CloudWatch` para logs y alarmas operativas basicas
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

Workflows incluidos en esta base:

- `infra-validate`: formato y validacion estatica de Terraform en PRs
- `infra-plan`: genera evidencia de `terraform plan` para `shared/ecr` y `live/dev` en PRs, la sube como artifact y actualiza un comentario en el PR
- `deploy-dev`: construye, publica imagen y despliega `dev`
- `promote-environment`: separa `plan` y `apply` para promover una imagen ya publicada hacia `qa` o `prod`

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
- `AWS_ROLE_TO_ASSUME_PLAN`
- `AWS_ROLE_TO_ASSUME_APPLY`
- `TF_STATE_BUCKET`

Si solo defines `AWS_ROLE_TO_ASSUME`, los workflows siguen funcionando. Si defines `AWS_ROLE_TO_ASSUME_PLAN` y `AWS_ROLE_TO_ASSUME_APPLY`, los workflows usan esos roles separados segun el paso.

### 3. Crear el rol IAM para OIDC

El rol debe confiar en el proveedor OIDC de GitHub y permitir:

- `ecr:*` sobre el repositorio necesario
- `ecs:*` sobre el cluster y servicio del proyecto
- `elasticloadbalancing:*` cuando aplique IaC
- `rds:*`, `ec2:*`, `logs:*`, `cloudwatch:*`, `sns:*`, `iam:*`, `secretsmanager:*`, `application-autoscaling:*` para Terraform
- `s3:*` sobre el bucket de state

En produccion conviene separar roles:

- uno para `plan`
- otro para `apply`

El workflow ya soporta esa separacion usando `AWS_ROLE_TO_ASSUME_PLAN` y `AWS_ROLE_TO_ASSUME_APPLY`, con fallback a `AWS_ROLE_TO_ASSUME`.

Si quieres una aprobacion humana explicita antes del `apply` en `prod`, configura `required reviewers` en el environment `prod` de GitHub. Con el workflow actual, el `plan` corre primero y el gate del environment se aplica justo antes del job de `apply`.

## Observabilidad Minima Incluida

La base de infraestructura deja alarmas de `CloudWatch` para:

- `HTTPCode_Target_5XX_Count` del `ALB`
- `UnHealthyHostCount` del target group
- `CPUUtilization` del servicio `ECS`

Puedes conectar acciones de alarma de dos maneras:

- `alarm_actions`
- `create_alarm_topic`
- `alarm_email_subscriptions`

Si `create_alarm_topic = true`, el entorno crea un topic `SNS` propio y su ARN se agrega automaticamente a `alarm_actions`. Si ademas cargas valores en `alarm_email_subscriptions`, Terraform deja creadas las suscripciones por email pendientes de confirmacion.

Los nombres de las alarmas quedan expuestos en el output `alarm_names` y el topic generado en `alarm_topic_arn`.

## Ambitos de esta base

Esta implementacion despliega el backend completo y sus dependencias.

El frontend Angular todavia no se publica desde esta base; una siguiente iteracion natural es `S3 + CloudFront` o `Amplify Hosting`.
