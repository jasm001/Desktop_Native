# Deshabilitacion, rollback y retiro del DeviceAgent

## Alcance

Procedimiento local del Bloque 10 para VM o laboratorio autorizado. No define
el mecanismo corporativo, nombre del servicio, owner, UEMS, retencion, firma ni
revocacion de identidad del piloto.

## Configuracion

`DeviceAgent:JobExecutionEnabled` controla la admision y el inicio de trabajos.
El valor predeterminado es `false`.

Para una ejecucion local autorizada deben coincidir tres controles:

- ambiente .NET `Development`;
- `JobExecutionEnabled=true`;
- `ExecutionProfile=local-demo`;
- `ControlPlaneSyncEnabled=true` solo cuando se necesite sincronizacion local.

La configuracion puede suministrarse por el proveedor normal de configuracion
.NET, por ejemplo `DeviceAgent__JobExecutionEnabled`. El repositorio no incluye
un perfil habilitado para piloto o produccion. Un perfil desconocido,
`local-demo` fuera de `Development` o una capacidad habilitada con perfil
`disabled` impiden iniciar el host.

## Deshabilitacion local

1. Registrar motivo, hora, endpoint de laboratorio y correlacion disponible sin
   incluir logs completos, secretos ni PII.
2. Consultar el estado local. Si un adaptador ya esta ejecutandose, esperar su
   estado terminal o usar solo la cancelacion que el adaptador declare segura.
3. Establecer `DeviceAgent:JobExecutionEnabled=false` o retirar una habilitacion
   local existente.
4. Reiniciar el proceso o servicio mediante el mecanismo autorizado del
   laboratorio. No forzar el reinicio durante una operacion no cancelable.
5. Verificar que una accion allowlisted nueva sea rechazada sin crear estado.
6. Verificar que no se reclame ningun trabajo del control plane.
7. Verificar que un trabajo local en cola permanezca en cola.
8. Confirmar que consulta, diagnosticos y cancelacion siguen disponibles.

Una operacion que ya entro a un adaptador no se termina a la fuerza. Se espera
su resultado tipado o se aplica la cancelacion segura declarada por el adaptador.

## Rollback y reactivacion

Antes de reactivar:

1. identificar la causa y confirmar que no existe sospecha de compromiso;
2. revisar los trabajos pendientes, su vigencia, dispositivo, accion, version,
   idempotency key y artefacto;
3. cancelar los trabajos que ya no deban ejecutarse cuando la cancelacion sea
   segura;
4. confirmar que el ambiente es `Development`, el perfil sigue siendo
   `local-demo` y el artefacto conserva longitud y SHA-256 esperados;
5. establecer `JobExecutionEnabled=true`, reiniciar y observar un solo trabajo
   sintetico o de laboratorio;
6. volver a `false` si la verificacion falla.

Para piloto, la reactivacion necesita owner operativo, autorizacion, auditoria y
configuracion administrada. Este runbook no concede esas capacidades.

## Retiro local

1. Deshabilitar trabajos y reiniciar.
2. Confirmar que no hay adaptadores en ejecucion.
3. Cancelar trabajos en cola cuando sea seguro.
4. Detener y retirar el proceso o servicio mediante el instalador o mecanismo
   de laboratorio que lo creo.
5. Conservar o eliminar SQLite y evidencia solo conforme a la retencion
   aprobada; la politica corporativa sigue pendiente.
6. Verificar que el Named Pipe ya no este disponible y que no existan procesos
   del agente.
7. Restaurar la VM o endpoint de laboratorio al estado inicial autorizado.

## Sustitucion para piloto

Los puntos de sustitucion son la fuente de configuracion y
`DeviceAgentConfigurationPolicy`. UEMS o el mecanismo aprobado debera:

- entregar una configuracion protegida y vinculada al ambiente;
- registrar quien deshabilita o reactiva y por que;
- reiniciar o retirar el servicio;
- impedir que `local-demo` sea promovido;
- introducir un perfil empresarial explicito con validaciones propias;
- coordinar revocacion de identidad y tratamiento de trabajos remotos;
- aportar evidencia de despliegue y retiro en dos endpoints.

## Evidencia externa pendiente

- owner y procedimiento UEMS;
- identidad restringida y revocacion;
- revision Security/Sophos;
- mecanismo de confianza del publicador;
- retencion y respuesta ante compromiso;
- dos endpoints, ventana, responsables y criterio de restauracion.
