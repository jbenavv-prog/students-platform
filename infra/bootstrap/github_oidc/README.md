# GitHub Actions OIDC Setup

Esta carpeta deja una base concreta para conectar `GitHub Actions` con `AWS` usando `OIDC`, sin guardar llaves largas en GitHub.

La configuracion actual del repo asume:

- un bucket de state remoto ya creado
- un rol de `plan`
- un rol de `apply` para `dev`
- un rol de `apply` para `qa`

## 1. Crear el bucket de Terraform state

Usa primero el bootstrap ya existente:

```powershell
terraform -chdir=infra/bootstrap/state init
terraform -chdir=infra/bootstrap/state apply -var "bucket_name=students-platform-tf-state"
```

## 2. Crear el proveedor OIDC de GitHub en AWS

En IAM crea un proveedor OpenID Connect con estos valores:

- Provider URL: `https://token.actions.githubusercontent.com`
- Audience: `sts.amazonaws.com`

## 3. Crear los roles IAM

La base recomendada es:

- `students-platform-gh-plan`
- `students-platform-gh-apply-dev`
- `students-platform-gh-apply-qa`

Usa estas plantillas:

- `trust-plan.json`
- `trust-apply-dev.json`
- `trust-apply-qa.json`
- `terraform-state-access-policy.json`
- `terraform-apply-policy.json`

Si prefieres un solo rol de `apply` compartido entre `dev` y `qa`, tambien puedes reutilizar `trust-apply.json`, pero la recomendacion de esta base es separar ambos ambientes.

## 4. Que permisos asignar

### Rol de plan

Adjunta:

- la politica administrada `ReadOnlyAccess`
- la politica personalizada `terraform-state-access-policy.json`

La trust policy de `plan` ya contempla estos casos del repo actual:

- jobs con environment `dev`
- `pull_request` para `infra-plan`
- ejecuciones manuales o directas sobre `master`

### Roles de apply

Adjunta a `students-platform-gh-apply-dev` y `students-platform-gh-apply-qa`:

- la politica personalizada `terraform-apply-policy.json`

La politica de `apply` es pragmatica y esta pensada para entrevista tecnica y bootstrap rapido. No intenta ser minimo privilegio estricto.

De todas formas, ya viene endurecida en dos puntos sensibles:

- `iam:PassRole` solo aplica a los roles `ecs-execution` y `ecs-task` creados por este proyecto
- `iam:CreateServiceLinkedRole` queda limitado a `ECS`, `Elastic Load Balancing` y `Application Auto Scaling` para `ECS`
- `iam:CreateServiceLinkedRole` tambien contempla `rds.amazonaws.com` para `AWSServiceRoleForRDS`
- el bloque `FrontendBucketAccess` incluye tagging del bucket S3 del frontend

## 5. Variables exactas en GitHub

### Variables de repositorio

Estas deben vivir a nivel `Repository -> Settings -> Secrets and variables -> Actions -> Variables`:

- `AWS_REGION`
- `TF_STATE_BUCKET_REGION`
- `TF_STATE_BUCKET`
- `AWS_ROLE_TO_ASSUME_PLAN`

Valores esperados:

```text
AWS_REGION=us-east-1
TF_STATE_BUCKET_REGION=us-east-2
TF_STATE_BUCKET=students-platform-tf-state
AWS_ROLE_TO_ASSUME_PLAN=arn:aws:iam::YOUR_AWS_ACCOUNT_ID:role/students-platform-gh-plan
```

### Variables del environment `dev`

Estas deben vivir en `Settings -> Environments -> dev -> Variables`:

- `AWS_ROLE_TO_ASSUME_APPLY`

Valor esperado:

```text
AWS_ROLE_TO_ASSUME_APPLY=arn:aws:iam::YOUR_AWS_ACCOUNT_ID:role/students-platform-gh-apply-dev
```

### Variables del environment `qa`

Estas deben vivir en `Settings -> Environments -> qa -> Variables`:

- `AWS_ROLE_TO_ASSUME_APPLY`

Valor esperado:

```text
AWS_ROLE_TO_ASSUME_APPLY=arn:aws:iam::YOUR_AWS_ACCOUNT_ID:role/students-platform-gh-apply-qa
```

## 6. Por que el rol de plan va a nivel repositorio

El workflow `promote-qa` tiene un job `plan` sin `environment`. Por eso ese job no puede depender de variables definidas solo dentro de `qa`.

En cambio:

- `AWS_REGION`
- `TF_STATE_BUCKET_REGION`
- `TF_STATE_BUCKET`
- `AWS_ROLE_TO_ASSUME_PLAN`

deben quedar a nivel repositorio.

Los roles de `apply` si pueden vivir por ambiente porque:

- `deploy-dev` corre en environment `dev`
- `promote-qa` aplica en environment `qa`

## 7. Environments de GitHub

Crea estos dos environments:

- `dev`
- `qa`

Opcionalmente, agrega `required reviewers` en `qa` para que el `apply` necesite aprobacion humana.

## 8. Primer flujo recomendado

Una vez creados bucket, provider, roles y variables:

1. Haz push o merge a `master` para disparar `deploy-dev`.
2. Verifica que `dev` cree `ECR`, backend, frontend, `CloudFront` y base de datos.
3. Ejecuta `promote-qa` con `activate_service = false` para dejar `qa` provisionado pero hibernado.
4. Cuando quieras demostrar la promocion completa, vuelve a correr `promote-qa` con `activate_service = true`.

## 9. Como queda alineado con este repo

Con esta configuracion:

- `deploy-dev` usa el rol de `plan` del repositorio y el rol de `apply` del environment `dev`
- `promote-qa` usa el rol de `plan` del repositorio y el rol de `apply` del environment `qa`
- `infra-plan` usa el rol de `plan` del repositorio

Eso conversa bien con el diseno actual de `dev + qa`.

