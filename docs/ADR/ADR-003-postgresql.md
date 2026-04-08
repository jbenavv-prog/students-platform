# ADR-003: PostgreSQL

## Estado

Aceptado

## Contexto

El dominio es relacional: estudiantes, materias, profesores e inscripciones tienen relaciones claras y restricciones útiles.

## Decisión

Usar `PostgreSQL` como base de datos principal.

## Justificación

- madurez y confiabilidad
- excelente ajuste para modelos relacionales
- soporte sólido para constraints e índices
- ecosistema amplio y conocido

## Consecuencias

- persistencia consistente
- modelo fácil de explicar
- soporte adecuado para validaciones y consultas de detalle
