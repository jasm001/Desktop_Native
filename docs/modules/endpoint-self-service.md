# Autoservicio local y mantenimiento del endpoint

## Estado

Roadmap posterior al MVP. No cambia el Bloque 5 ni autoriza remediaciones durante
el diagnostico de solo lectura.

## Objetivo

Reducir tareas repetitivas de soporte mediante acciones pequenas, tipadas,
reversibles cuando sea posible y visibles para el usuario.

Orden previsto:

1. paquete de evidencia para tickets;
2. reparaciones locales firmadas;
3. limpieza segura de almacenamiento;
4. diagnostico guiado de perifericos;
5. coordinador de mantenimiento y reinicios;
6. perfiles declarativos de ambientacion;
7. modo local/offline;
8. campanas de actualizacion del producto;
9. preparacion de aplicaciones para reprovisionamiento.

El borrado de Windows no pertenece a este documento y se implementa al final,
segun `device-reprovisioning.md`.

## Evidencia para soporte

El usuario puede revisar antes de compartir:

- version de Windows y del agente;
- arquitectura, memoria y almacenamiento agregado;
- estado saneado de red y prerrequisitos;
- pruebas realizadas y codigos de resultado;
- eventos expresamente allowlisted y redactados.

No incluye archivos personales, nombres internos, IP, tokens, historial de
navegacion, prompts completos ni logs sin filtrar.

## Reparaciones locales

Cada reparacion implementa `Detect`, `Preflight`, `Remediate`, `Verify` y, cuando
sea seguro, `Rollback`. Ejemplos candidatos:

- recuperar un servicio conocido;
- reparar una cola de impresion;
- reparar componentes de Windows Update;
- limpiar cache de una aplicacion soportada;
- ejecutar limpieza sobre ubicaciones temporales allowlisted.

Las acciones estan firmadas, versionadas, auditadas y probadas. Hermes/RAG puede
explicar o seleccionar una accion conocida, pero no genera scripts, comandos ni
argumentos para el agente.

## Almacenamiento

El agente clasifica capacidad y ubicaciones tecnicas conocidas sin recorrer
documentos personales. Puede proponer Storage Sense y limpieza de temporales
allowlisted.

Descargas, Escritorio, Documentos, OneDrive y Papelera no se eliminan
automaticamente. Cualquier operacion sobre contenido del usuario requiere
politica y consentimiento especificos.

## Perifericos

Pruebas guiadas y no destructivas:

- camara;
- microfono y audio;
- Bluetooth;
- pantalla externa;
- impresora;
- docking station.

El agente reporta presencia, estado y resultado de una prueba conocida. No
captura audio, video o documentos sin una accion explicita y visible.

## Mantenimiento y reinicios

El coordinador agrupa trabajos compatibles y busca una ventana aprobada. Antes
de reiniciar muestra:

- motivo y criticidad;
- tareas que se completaran;
- tiempo estimado;
- fecha limite y posposiciones disponibles;
- cuenta regresiva;
- aviso para guardar archivos;
- necesidad de introducir PIN de BitLocker al arrancar, si aplica.

Una actualizacion critica puede llegar a una fecha limite no posponible, pero no
simula urgencia ni reinicia inmediatamente mientras el usuario guarda trabajo.
El PIN no se almacena, calcula ni muestra.

## Perfiles de ambientacion

Un perfil declara estado deseado, por ejemplo:

```text
office-standard.v1
developer-windows.v1
personal-clean-install.v1
```

Puede incluir aplicaciones, preferencias permitidas, impresoras, comprobaciones
y orden de instalacion. No contiene secretos ni archivos personales.

Antes de una reinstalacion, el usuario selecciona aplicaciones disponibles. Tras
iniciar sesion nuevamente, el agente prepara el equipo y muestra progreso,
resultados y pasos que requieren autenticacion humana.

## Modo local

Sin backend o IA siguen disponibles:

- diagnostico;
- catalogo cacheado;
- evidencia local;
- acciones fijas incluidas en bundles firmados;
- guias de conocimiento local versionadas.

No se inicia una accion que requiera autorizacion remota nueva. Al reconectar,
la evidencia se sincroniza de forma idempotente.

## Perfiles comerciales futuros

- Local: salud, evidencia, mantenimiento y perfiles basicos.
- Personal/Pro: reparaciones, ambientacion y reprovisionamiento asistido.
- Equipos pequenos/MSP: campanas, catalogo, auditoria e inventario minimo.

Estos niveles son opciones de producto, no compromisos del MVP.
