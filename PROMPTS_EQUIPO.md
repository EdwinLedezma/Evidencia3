# Prompts de desarrollo por integrante

Copiar el bloque correspondiente en el asistente del integrante. El asistente necesita acceso al repositorio y a `AGENTS.md`. Estos prompts autorizan desarrollar, probar, hacer commits y push únicamente a la rama propia; no autorizan merges ni PR en esta etapa.

La base compartida se está desarrollando en `feature/edwin-modificar-eliminar`. Mientras no esté disponible en la rama del compañero mediante una integración revisada por el equipo, no iniciar una aplicación paralela ni copiar o integrar otras ramas automáticamente. Compartir este documento no publica ni integra la base.

## Joaquin Lopez

```text
Lee AGENTS.md completo antes de editar. Soy Joaquin Lopez. Desarrolla código funcional para prioridades y filtros de tareas en feature/joaquin-prioridades-filtros.

Usa C#, .NET 10 y la aplicación DDD compartida con Presentation, Application, Domain e Infrastructure. Implementa el establecimiento de prioridades Baja, Media y Alta, y consultas filtradas por estado o responsable; permite combinar filtros y define el comportamiento cuando no hay resultados. Reutiliza Tarea y el repositorio compartido, conservando título, descripción, responsable y estado al cambiar una prioridad. Coordina los contratos con las consultas de Angel y los estados de Agustin.

Verifica la rama activa y la identidad Git. No cambies ni suplantes mi identidad si no coincide: informa el problema. Conserva cambios locales ajenos. Si falta AGENTS.md o la base compartida, informa qué falta antes de implementar o integrar otra rama. No crees otra aplicación ni adoptes otras tecnologías.

Desarrolla y ejecuta verificaciones de prioridades válidas e inválidas, tareas inexistentes, filtros individuales y combinados, resultados vacíos y conservación de datos. Actualiza las instrucciones específicas fuera del README si hace falta. Revisa el diff y crea commits coherentes y descriptivos con mi identidad. Haz push exclusivamente a origin/feature/joaquin-prioridades-filtros, sin force push.

No crees, edites, reemplaces ni elimines ningún README: es exclusivo de Edwin. No hagas ningún merge, no abras PR todavía y no hagas push a develop, main ni a ramas de compañeros. Si necesitas cambios del README, entrega la sugerencia en tu respuesta. Esta restricción de no integrar ramas prevalece sobre los ejemplos generales de sincronización de AGENTS.md.

Al terminar informa la rama, funcionalidades desarrolladas, comandos y resultados reales de verificación, commits y confirmación del push. El revisor del futuro PR a develop será Angel Espinoza.
```

## Angel Espinoza

```text
Lee AGENTS.md completo antes de editar. Soy Angel Espinoza. Desarrolla código funcional de registro y consulta general de tareas en feature/angel-registro-consulta.

Usa C#, .NET 10 y la aplicación DDD compartida con Presentation, Application, Domain e Infrastructure. Implementa registro, listado general y consulta por identificador. Reutiliza Tarea, sus invariantes y el repositorio compartido. Genera identificadores únicos y conserva los campos de responsable, estado y prioridad acordados. Asegura persistencia compatible con modificación, eliminación y los módulos de los compañeros; no implementes sus funcionalidades.

Verifica la rama activa y la identidad Git. No cambies ni suplantes mi identidad si no coincide: informa el problema. Conserva cambios locales ajenos. Si falta AGENTS.md o la base compartida, informa qué falta antes de implementar o integrar otra rama. No crees otra aplicación ni adoptes otras tecnologías.

Desarrolla y ejecuta verificaciones de registro válido, título obligatorio, límites de longitud, valores predeterminados, identificadores únicos, listado vacío, consulta inexistente y recuperación de datos persistidos. Actualiza las instrucciones específicas fuera del README si hace falta. Revisa el diff y crea commits coherentes y descriptivos con mi identidad. Haz push exclusivamente a origin/feature/angel-registro-consulta, sin force push.

No crees, edites, reemplaces ni elimines ningún README: es exclusivo de Edwin. No hagas ningún merge, no abras PR todavía y no hagas push a develop, main ni a ramas de compañeros. Si necesitas cambios del README, entrega la sugerencia en tu respuesta. Esta restricción de no integrar ramas prevalece sobre los ejemplos generales de sincronización de AGENTS.md.

Al terminar informa la rama, funcionalidades desarrolladas, comandos y resultados reales de verificación, commits y confirmación del push. El revisor del futuro PR a develop será Edwin Ledezma.
```

## Agustin Montoya

```text
Lee AGENTS.md completo antes de editar. Soy Agustin Montoya. Desarrolla código funcional de asignación y estados de tareas en feature/agustin-asignacion-estados.

Usa C#, .NET 10 y la aplicación DDD compartida con Presentation, Application, Domain e Infrastructure. Implementa la asignación de tareas a un responsable y el cambio de estado entre Pendiente, EnProceso y Terminada. Expresa las validaciones y comportamiento en Domain y los casos de uso en Application. Reutiliza Tarea y el repositorio compartido; conserva título, descripción y prioridad en las actualizaciones. No inventes restricciones de transición que el equipo no haya acordado. Si no hay un catálogo de integrantes, usa el responsable textual opcional definido en AGENTS.md, sin crear un sistema de usuarios independiente.

Verifica la rama activa y la identidad Git. No cambies ni suplantes mi identidad si no coincide: informa el problema. Conserva cambios locales ajenos. Si falta AGENTS.md o la base compartida, informa qué falta antes de implementar o integrar otra rama. No crees otra aplicación ni adoptes otras tecnologías.

Desarrolla y ejecuta verificaciones de asignación, cambio de responsable, estados válidos e inválidos, tarea inexistente, persistencia y conservación de los demás campos. Actualiza las instrucciones específicas fuera del README si hace falta. Revisa el diff y crea commits coherentes y descriptivos con mi identidad. Haz push exclusivamente a origin/feature/agustin-asignacion-estados, sin force push.

No crees, edites, reemplaces ni elimines ningún README: es exclusivo de Edwin. No hagas ningún merge, no abras PR todavía y no hagas push a develop, main ni a ramas de compañeros. Si necesitas cambios del README, entrega la sugerencia en tu respuesta. Esta restricción de no integrar ramas prevalece sobre los ejemplos generales de sincronización de AGENTS.md.

Al terminar informa la rama, funcionalidades desarrolladas, comandos y resultados reales de verificación, commits y confirmación del push. El revisor del futuro PR a develop será Joaquin Lopez.
```
