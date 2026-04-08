# Test Matrix

| Regla / escenario | Tipo de prueba | Evidencia |
| --- | --- | --- |
| Exigir exactamente 3 materias | Unitaria de dominio + integracion API | `EnrollmentSelectionPolicyTests` y `StudentsEndpointsTests` |
| No permitir mas de 9 creditos | Unitaria de dominio | `EnrollmentSelectionPolicyTests` |
| No permitir repetir una materia | Unitaria de dominio + integracion API | `EnrollmentSelectionPolicyTests` y validacion de request |
| No permitir dos materias del mismo profesor | Unitaria de dominio + integracion API | `EnrollmentSelectionPolicyTests` y `CreateStudent` / `UpdateStudent` |
| Mostrar 10 materias semilla | Integracion API | `CatalogEndpointsTests` |
| Mostrar 5 profesores semilla | Integracion API | `CatalogEndpointsTests` |
| Cada materia vale 3 creditos | Integracion API | `CatalogEndpointsTests` |
| CRUD basico de estudiante | Integracion API | `StudentsEndpointsTests` |
| Consultar otros estudiantes registrados | Integracion API | `GET /api/students` y `GET /api/students/{id}` |
| Mostrar companeros por materia | Integracion API | `StudentDetail_ShouldIncludeClassmatesGroupedBySubject` |
| Impedir estados inconsistentes en backend | Integracion API | Respuestas `400`, `404` y `409` |
| Validacion visual de la UI | Manual | `docs/MANUAL_CHECKLIST.md` |

## Cobertura minima implementada

- 6 pruebas unitarias de dominio.
- 6 pruebas de integracion API.
- 1 checklist manual para validacion visual y funcional.

## Criterio de interpretacion

La matriz se usa como trazabilidad entre enunciado, implementacion y evidencia. Si una regla cambia, se actualiza primero el test y luego el codigo.
