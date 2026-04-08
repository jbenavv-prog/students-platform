# ADR-004: Terraform como Base Opcional

## Estado

Propuesto solo si el alcance lo justifica

## Contexto

La prueba técnica prioriza producto, arquitectura y reglas de negocio. Infraestructura compleja puede distraer del objetivo principal.

## Decisión

No introducir Terraform en el MVP salvo que se requiera una base mínima de despliegue repetible.

## Justificación

- evita sobredimensionar el reto
- concentra el esfuerzo en funcionalidad y calidad
- puede agregarse luego sin rediseño

## Consecuencias

- menor tiempo de implementación
- documentación de despliegue simple y clara
- IaC reservado para una evolución futura
