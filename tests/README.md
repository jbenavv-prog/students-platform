# Tests

Este directorio agrupa la validacion automatizada de la solucion.

## Estructura

- `backend/StudentsPlatform.Domain.Tests`: pruebas unitarias de reglas del dominio.
- `backend/StudentsPlatform.Api.IntegrationTests`: pruebas de integracion sobre endpoints REST.

## Que valida la suite

- exactamente 3 materias por estudiante
- maximo de 9 creditos
- no duplicidad de materias
- no repeticion de profesor
- seed de catalogo
- CRUD basico de estudiantes
- detalle de estudiante con companeros por materia

## Como ejecutar

Desde la raiz del repositorio:

```powershell
dotnet test StudentsPlatform.sln
```

## Nota de integracion

Las pruebas de integracion usan un entorno de base de datos aislado para validar la API sin depender del PostgreSQL local.

