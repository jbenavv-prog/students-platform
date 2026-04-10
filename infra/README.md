# Infraestructura y Deploy

Esta carpeta deja una base pragmatica para desplegar el backend monolitico modular en AWS usando:

- `Terraform` para IaC
- `ECR` para la imagen del backend
- `ECS Fargate` para ejecutar la API
- `ALB` para exponer HTTP
- `RDS PostgreSQL` para persistencia
- `S3 + CloudFront` para publicar el frontend Angular
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
  modules/
    ecr/
    network/
    database/
    ecs_app/
    frontend_static/
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
- `deploy-dev`: tambien construye el frontend Angular, publica a `S3` e invalida `CloudFront`
- `promote-qa`: separa `plan` y `apply` para promover una imagen ya publicada hacia `qa`, empaqueta el build del frontend y lo publica a `S3`

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
- `TF_STATE_BUCKET_REGION`
- `AWS_ROLE_TO_ASSUME`
- `AWS_ROLE_TO_ASSUME_PLAN`
- `AWS_ROLE_TO_ASSUME_APPLY`
- `TF_STATE_BUCKET`

Si solo defines `AWS_ROLE_TO_ASSUME`, los workflows siguen funcionando. Si defines `AWS_ROLE_TO_ASSUME_PLAN` y `AWS_ROLE_TO_ASSUME_APPLY`, los workflows usan esos roles separados segun el paso.

`TF_STATE_BUCKET_REGION` permite separar la region del bucket de state de la region donde despliegas la infraestructura. Si no lo defines, los workflows usan `AWS_REGION`.

Para una guia mas concreta de `OIDC`, trust policies y matriz exacta de variables entre repositorio, `dev` y `qa`, revisa [bootstrap/github_oidc/README.md](/c:/intp/students-platform/infra/bootstrap/github_oidc/README.md).

### 3. Crear el rol IAM para OIDC

El rol debe confiar en el proveedor OIDC de GitHub y permitir:

- `ecr:*` sobre el repositorio necesario
- `ecs:*` sobre el cluster y servicio del proyecto
- `elasticloadbalancing:*` cuando aplique IaC
- `cloudfront:*` para crear la distribucion y publicar invalidaciones
- `rds:*`, `ec2:*`, `logs:*`, `cloudwatch:*`, `sns:*`, `iam:*`, `secretsmanager:*`, `application-autoscaling:*` para Terraform
- `s3:*` sobre el bucket de state
- `s3:ListBucket`, `s3:GetObject`, `s3:PutObject`, `s3:DeleteObject` sobre el bucket del frontend

En ambientes con aprobacion conviene separar roles:

- uno para `plan`
- otro para `apply`

El workflow ya soporta esa separacion usando `AWS_ROLE_TO_ASSUME_PLAN` y `AWS_ROLE_TO_ASSUME_APPLY`, con fallback a `AWS_ROLE_TO_ASSUME`.

Si quieres una aprobacion humana explicita antes del `apply` en `qa`, configura `required reviewers` en el environment `qa` de GitHub. Con el workflow actual, el `plan` corre primero y el gate del environment se aplica justo antes del job de `apply`.

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

## Perfil de Costo Bajo

La configuracion activa de `dev` y `qa` esta optimizada para entrevista tecnica y uso liviano:

- `NAT Gateway` deshabilitado en ambos ambientes
- tareas `ECS` con `assign_public_ip = true` para salir a Internet sin costo fijo de NAT
- `Container Insights` deshabilitado
- retencion de logs reducida a `3` dias
- `desired_count = 1` y `max_capacity = 1` para evitar scale-out inesperado
- `RDS` en `db.t3.micro`, `Single-AZ` y con retencion de backup corta
- `CloudFront` en `PriceClass_100`
- `SNS` para alarmas deshabilitado por defecto

La base de datos sigue en subredes privadas; solo las tareas de aplicacion reciben IP publica controlada y mantienen entrada restringida al `ALB` mediante `security groups`.

Ademas, `qa` queda hibernado por defecto con `app_enabled = false`. Eso conserva la arquitectura provisionada por Terraform, pero deja el servicio `ECS` en `desired_count = 0` hasta que quieras activarlo en una promocion o demo.

Para reducir todavia mas el costo de `qa`, puedes detener manualmente la instancia `RDS` cuando no la uses. Terraform no modela ese estado transitorio, asi que es una decision operativa fuera del `apply`.

## Hibernar y Activar Dev

El workflow `manage-dev-power` permite cambiar el ambiente `dev` entre:

- `hibernated`: aplica `app_enabled = false`, deja el servicio `ECS` en `desired_count = 0` y opcionalmente detiene `RDS`
- `active`: inicia `RDS` si estaba detenido, aplica `app_enabled = true` y valida `/health` contra el `ALB`

Esto conserva `S3`, `CloudFront`, `ALB`, `ECR`, logs y state remoto. Sirve para bajar el costo variable de `ECS` y el costo horario de la instancia `RDS`, aunque se siguen cobrando recursos persistentes como storage de `RDS`, backups, `S3`, `ECR`, `CloudWatch` y el `ALB`.

AWS puede mantener una instancia `RDS` detenida por un maximo de 7 dias consecutivos; despues la inicia automaticamente para mantenimiento. Si necesitas cero costo de plataforma por mas tiempo, usa `terraform destroy` sabiendo que al recrear cambiaran URLs como CloudFront y ALB.

## Ambitos de esta base

Esta implementacion despliega el backend completo, el frontend estatico y sus dependencias con dos ambientes activos:

- `dev`
- `qa`

El archivo `infra/live/envs/prod.tfvars` queda marcado como deprecado para evitar un tercer ambiente mientras esta base siga optimizada a costo de entrevista tecnica.

El frontend Angular queda servido por `S3 + CloudFront` y consume la API por la ruta relativa `/api`, que `CloudFront` reenvia al `ALB` del backend. Eso evita exponer otra URL para la API en el navegador y reduce la necesidad de `CORS` en el flujo desplegado.

En `qa`, esta base permite demostrar multiambiente aunque el backend quede hibernado:

- Terraform sigue creando la misma topologia base que en `dev`
- `CloudFront` y el bucket del frontend quedan creados
- el frontend se puede publicar desde `promote-qa`
- `app_enabled = false` deja el servicio `ECS` en `desired_count = 0` hasta que quieras activarlo
