# Kill switch local del DeviceAgent

## Estado

Primera unidad tecnica local del Bloque 10, implementada y validada localmente
el 2026-06-14. Este incremento no es evidencia de piloto corporativo y no
sustituye UEMS, identidad de dispositivo, Security, Sophos, PKI ni owner
operativo.

## Brecha

El DeviceAgent tiene allowlist, perfiles de ejecucion y configuracion de
sincronizacion, pero no existe un control unico que impida de forma verificable:

- aceptar un trabajo nuevo por IPC;
- reclamar un trabajo nuevo del control plane;
- iniciar o reanudar un trabajo local pendiente.

Detener solo la sincronizacion no protege el IPC. Deshabilitar solo el perfil de
7-Zip tampoco cubre la accion simulada ni evita que un trabajo quede reclamado.

## Alcance tecnico

Se agrega un gate inmutable de proceso controlado por
`DeviceAgent:JobExecutionEnabled`.

Con el valor ausente o `false`:

- `StartAgentJobRequest` falla cerrado antes de crear estado local;
- la sincronizacion no reclama trabajos del control plane;
- trabajos locales `queued`, incluidos los recuperados tras reinicio, no
  avanzan;
- consultas de estado, diagnosticos y cancelacion permanecen disponibles;
- una operacion que ya entro al adaptador no se interrumpe de forma insegura.

Con el valor `true`, las validaciones existentes siguen siendo obligatorias:
contrato versionado, identificadores acotados, allowlist exacta, perfil,
idempotencia, artefacto e integridad. El switch no autoriza ninguna accion.

El valor se carga al iniciar el proceso. Cambiarlo exige reiniciar el servicio.
La configuracion predeterminada del repositorio permanece deshabilitada.

## Alternativas

- Archivo sentinel observado en caliente: descartado para esta unidad porque
  agrega vigilancia de archivos, decisiones de ACL y condiciones de carrera.
- Endpoint remoto de administracion: descartado porque requiere identidad,
  autorizacion, auditoria y owner corporativos no disponibles.
- Usar solo `ControlPlaneSyncEnabled`: insuficiente porque no protege IPC ni
  trabajos locales ya persistidos.
- Cancelar automaticamente trabajos en curso: descartado porque algunos
  adaptadores declaran que la cancelacion en ejecucion no es segura.

## Evidencia esperada

- prueba de que la configuracion predeterminada rechaza una accion allowlisted
  sin crear un trabajo;
- prueba de que un trabajo en cola no avanza mientras el gate esta deshabilitado;
- prueba de que no se reclama un trabajo remoto mientras el gate esta
  deshabilitado;
- regresion de IPC, idempotencia, recuperacion, adaptador, contratos y E2E;
- runbook local de deshabilitacion, rollback y retiro;
- gate completo, auditoria de dependencias y escaneo de secretos.

## Implementacion

- `DeviceAgentOptions.JobExecutionEnabled` tiene valor predeterminado `false`.
- `AgentJobExecutionGate` es inmutable durante la vida del proceso.
- `AgentJobService` rechaza admision y no avanza trabajos pendientes cuando el
  gate esta deshabilitado.
- `ControlPlaneAgentSyncService` no realiza el claim remoto cuando el gate esta
  deshabilitado.
- `ControlPlaneAgentWorker` no inicia su loop si el gate, el perfil o la
  sincronizacion no estan habilitados.
- Las pruebas habilitan el gate de forma explicita para demostrar que el switch
  no sustituye allowlist, perfil, idempotencia ni validacion de artefactos.

El runbook relacionado vive en
`../runbooks/device-agent-disable-rollback-retirement.md`.

## Criterio de aceptacion

1. El valor predeterminado es `false`.
2. Habilitar el switch no amplia la allowlist ni evita otras validaciones.
3. Deshabilitarlo no crea, reclama, inicia ni reanuda trabajos.
4. Diagnosticos, consulta y cancelacion siguen disponibles.
5. No se agregan comandos, argumentos, rutas de entrada, datos ni privilegios.
6. No se cambia ningun contrato IPC o HTTP publico.
7. El punto de sustitucion para piloto queda documentado como configuracion
   administrada y reinicio del servicio mediante el mecanismo aprobado.

## Limites y riesgos residuales

- No existe owner corporativo que opere el switch ni canal UEMS aprobado.
- El valor local no esta firmado ni protegido por una politica corporativa.
- Una operacion ya iniciada se deja terminar para evitar una interrupcion
  insegura.
- Los trabajos que permanezcan en el control plane requieren una decision
  operativa sobre cancelacion, expiracion o reanudacion antes del piloto.
- La eficacia ante compromiso de una cuenta privilegiada depende de ACL,
  despliegue, monitoreo y respuesta externos.

## Validacion

Validado el 2026-06-14:

- `scripts/Validate.ps1`: correcto;
- .NET: 129 pruebas, incluidas cuatro nuevas de este control;
- Node: 20 pruebas unitarias/de contrato;
- PostgreSQL: 11 integraciones AdminWeb y 4 del Worker;
- cuatro migraciones aplicadas y E2E WinUI/DeviceAgent correcto;
- `corepack pnpm@11.5.3 audit --prod --audit-level high`: sin vulnerabilidades
  conocidas;
- `scripts/Test-Secrets.ps1`: sin hallazgos.
