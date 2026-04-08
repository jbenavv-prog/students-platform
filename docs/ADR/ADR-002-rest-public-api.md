# ADR-002: REST como API Pública

## Estado

Aceptado

## Contexto

El frontend es una aplicación web Angular. La API pública debe ser simple, estándar y amigable para el navegador.

## Decisión

Exponer la funcionalidad del backend mediante `REST`.

## Justificación

- es el estándar natural para frontends web
- simplifica integración y debugging
- permite pruebas manuales con facilidad
- evita introducir complejidad innecesaria en el MVP

## Consecuencias

- contratos HTTP explícitos
- serialización simple
- mejor trazabilidad en entrevista y en mantenimiento
