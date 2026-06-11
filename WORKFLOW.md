# Workflow de desarrollo

## Estado

- Fase actual: inicio de la experiencia nativa Windows 11.
- Bloque activo: ninguno; Bloque 1 preparado como siguiente unidad.
- Ultimo resultado: fundacion compilable y validada en el repositorio
  independiente.
- Siguiente resultado: shell WinUI 3 con navegacion y accesibilidad base.

## Ultima validacion

- Fecha: 2026-06-11.
- Remoto `Desktop_Native.git` verificado sin refs antes de inicializar.
- `dotnet restore ITSupportNative.slnx --force-evaluate`: correcto.
- `dotnet format ITSupportNative.slnx --verify-no-changes --no-restore`:
  correcto.
- `dotnet build ITSupportNative.slnx --configuration Release --no-restore`:
  correcto, 0 warnings y 0 errores.
- `dotnet test ITSupportNative.slnx --configuration Release --no-build`:
  correcto, 4 pruebas.
- `corepack pnpm@11.5.3 run check`: correcto.
- `gitleaks dir . --redact`: correcto, sin hallazgos.

## Regla por tarea

1. Elegir un solo modulo y criterio de aceptacion.
2. Leer contexto base y documentos del modulo.
3. Inspeccionar el codigo existente antes de proponer cambios.
4. Implementar el cambio minimo completo.
5. Ejecutar formato, build, pruebas y escaneos aplicables.
6. Revisar seguridad, arquitectura, accesibilidad y regresiones.
7. Actualizar documento del modulo y este workflow.
8. Proponer un mensaje de commit para la unidad de cambio validada.
9. Reportar archivos, validaciones, riesgos, documentos actualizados, commit
   sugerido y siguiente bloque desbloqueado.

## Regla de commits

- Proponer un commit al terminar una seccion coherente y validada; no esperar
  necesariamente al cierre de un bloque grande.
- Usar Conventional Commits, por ejemplo:
  `feat(desktop): add initial WinUI navigation shell`.
- No mezclar cambios no relacionados en el mismo commit sugerido.
- No sugerir commit como terminado si faltan build o pruebas aplicables.
- Al completar una seccion o modulo, actualizar su documentacion y la tabla de
  bloques antes de proponer el commit.
- El push no sustituye la validacion. Registrar en evidencia el commit o PR
  cuando realmente exista.

## Estados permitidos

- `pending`
- `in_progress`
- `blocked`
- `completed`

Solo un bloque principal puede estar `in_progress`.

## Registro de bloques

| Bloque | Estado | Evidencia |
| --- | --- | --- |
| 0. Fundacion del repositorio | completed | Solucion/workspace, lockfiles, CI y gates locales validados; commit raiz en `main`. |
| 1. Shell nativo WinUI | pending | |
| 2. Dominio y catalogo sintetico | pending | |
| 3. Conversacion determinista | pending | |
| 4. Agente simulado e IPC | pending | |
| 5. Diagnostico de solo lectura | pending | |
| 6. Primer adaptador en VM | pending | |
| 7. API compartida y persistencia | pending | |
| 8. Casos, tickets y OpenText fake | pending | |
| 9. Canal Teams existente | pending | |
| 10. Endurecimiento para piloto | pending | |
| 11. Portal administrativo web | pending | |

## Formato de stopper

```text
Fecha:
Modulo:
Decision requerida:
Evidencia:
Alternativas:
Impacto:
Recomendacion:
Owner:
```

## Regla de documentacion

No usar este archivo como diario detallado. Conservar solo estado actual,
resultados verificables, bloqueos y siguiente paso. El conocimiento estable vive
en el documento propietario del modulo o en un ADR del repositorio definitivo.
