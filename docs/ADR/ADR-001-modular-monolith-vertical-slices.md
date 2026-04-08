# ADR-001: Modular Monolith + Vertical Slices

## Estado

Aceptado

## Contexto

La prueba técnica requiere una solución sólida, clara y fácil de explicar. El dominio no justifica microservicios, mensajería ni despliegue distribuido.

## Decisión

Implementar el backend como `modular monolith` organizado por `vertical slices`.

## Justificación

- reduce complejidad operativa
- mantiene límites lógicos por feature
- facilita pruebas por caso de uso
- evita capas artificiales

## Consecuencias

- un único despliegue backend
- menor costo cognitivo
- escalabilidad suficiente para el alcance del reto
