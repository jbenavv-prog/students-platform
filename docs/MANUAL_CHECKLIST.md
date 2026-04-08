# Manual Checklist

## Preparacion

- Confirmar que la API esta levantada y que la base de datos de PostgreSQL responde.
- Confirmar que el frontend Angular carga sin errores.
- Verificar que el seed muestra 10 materias y 5 profesores.

## Flujo principal

- Crear un estudiante con exactamente 3 materias validas.
- Editar un estudiante existente y cambiar su perfil sin romper la seleccion requerida.
- Actualizar la seleccion de materias manteniendo exactamente 3 y sin profesor repetido.
- Consultar el listado de estudiantes.
- Abrir el detalle de un estudiante y revisar materias, profesor y companeros.

## Validaciones criticas

- Intentar guardar 2 materias.
- Intentar guardar 4 materias.
- Intentar repetir la misma materia.
- Intentar combinar dos materias del mismo profesor.
- Intentar usar una materia inexistente.
- Intentar consultar un estudiante inexistente.

## Verificacion visual

- El formulario muestra errores legibles.
- El detalle separa claramente datos del estudiante y detalle de inscripcion.
- La lista de companeros muestra solo nombres, sin datos adicionales.
- La interfaz mantiene una lectura limpia en desktop y en vista reducida.

## Criterio de aceptacion

- Ningun escenario invalido debe persistir.
- Las respuestas de error deben ser consistentes con la API.
- La UI debe anticipar errores, pero el backend siempre debe seguir protegiendo la regla.
