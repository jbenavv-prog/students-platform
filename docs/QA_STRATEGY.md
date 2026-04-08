# QA Strategy

## Objetivo

Definir una estrategia de validacion que proteja las reglas criticas del dominio sin sobredimensionar la prueba tecnica. La prioridad es asegurar cumplimiento exacto del enunciado y evitar estados inconsistentes.

## Alcance

La cobertura QA se centra en cuatro niveles:

1. Reglas de dominio: exactamente 3 materias, maximo 9 creditos, no duplicidad de materias y no repeticion de profesor.
2. Integracion API: CRUD de estudiantes, catalogo semilla, flujo de inscripcion y detalle con companeros por materia.
3. Verificacion manual: navegacion basica, mensajes de error y consistencia visual.
4. Trazabilidad documental: cada regla del enunciado queda asociada a una prueba o evidencia concreta.

## Enfoque

- Las reglas criticas se validan primero en backend.
- El frontend replica validaciones para mejorar la experiencia, pero no reemplaza al servidor.
- Las pruebas de dominio cubren comportamiento puro y rapido.
- Las pruebas de integracion validan la API con persistencia real de prueba.

## Criterios de salida

- La solucion compila y la suite automatizada pasa.
- El seed contiene exactamente 10 materias, 5 profesores y 3 creditos por materia.
- El backend exige exactamente 3 materias por estudiante.
- Los casos invalidos devuelven respuestas claras y consistentes.
- El detalle de estudiante muestra solo los nombres de los companeros por materia.

## Riesgos cubiertos

- Inconsistencias entre frontend y backend.
- Regresiones en reglas de inscripcion.
- Errores en la consulta de companeros por clase.
- Duplicidad o corrupcion del catalogo semilla.
