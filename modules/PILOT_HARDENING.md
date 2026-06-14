# Endurecimiento para piloto

## Estado

Documento propietario del Bloque 10 `in_progress`.

Los Bloques 0 a 8 estan `completed`. El Bloque 9 esta `blocked` por la
integracion corporativa del bot existente y el Bloque 11 permanece `pending`.
El trabajo de este bloque no convierte `local-demo` en un piloto corporativo.

## Objetivo

Reducir y documentar los riesgos necesarios para probar el producto en dos
endpoints autorizados, sin inventar infraestructura, identidades, permisos ni
controles corporativos.

El bloque cubre:

- threat model basado en el codigo y las fronteras reales;
- identidad restringida del agente y menor privilegio;
- deshabilitacion de emergencia, recuperacion y retiro;
- logs y evidencia saneados;
- empaquetado, integridad y confianza de publicador;
- procedimiento de despliegue, actualizacion, rollback y desinstalacion;
- evidencia requerida de UEMS, Security/Sophos, Identity y owners operativos.

## Alcance local permitido

Antes de recibir accesos corporativos se puede:

- auditar controles existentes contra `core/SECURITY.md`;
- probar fallos cerrados, replay, idempotencia, downgrade y perdida de red;
- implementar configuracion o proveedores locales reemplazables;
- definir y probar un kill switch local que niegue trabajos nuevos sin aceptar
  comandos de texto;
- verificar redaccion, correlacion, retencion local y contenido de evidencia;
- generar runbooks y ensayar despliegue/retiro solo en VM o laboratorio
  autorizado;
- producir SBOM, auditorias de dependencias y escaneo de secretos con las
  herramientas ya adoptadas.

No se puede:

- conectar UEMS, Entra, Sophos, PKI, OpenText o Teams reales;
- usar cuentas, endpoints, tenant, certificados o datos corporativos;
- crear una exclusion antivirus general;
- distribuir una clave privada o confiar en un publicador ficticio;
- declarar aprobacion de Security, identidad corporativa o despliegue real;
- ampliar el portal administrativo del Bloque 11.

## Fronteras a proteger

- WinUI no privilegiado frente al DeviceAgent.
- Named Pipe versionado y con ACL explicita.
- Control plane frente a dispositivos, worker e integraciones.
- PostgreSQL, auditoria append-only y outbox.
- Artefactos, manifiestos y proveedor de paquetes.
- Identidad del usuario, identidad del dispositivo e identidad del servicio.
- Canales WinUI/Teams frente a autorizacion y ejecucion.
- Configuracion local frente a promocion accidental a piloto.

El inventario inicial de amenazas y evidencia vive en
`../docs/threat-model/README.md`.

## Primera unidad local validada

El 2026-06-14 se completo un kill switch local del DeviceAgent:

- `DeviceAgent:JobExecutionEnabled` es `false` por defecto;
- deshabilitado, rechaza nuevos trabajos IPC sin persistirlos;
- no reclama trabajos del control plane;
- no inicia ni reanuda trabajos locales pendientes;
- conserva consulta, diagnosticos y cancelacion;
- no interrumpe a la fuerza un adaptador que ya inicio;
- no agrega acciones, privilegios, datos, dependencias ni contratos publicos.

La especificacion y evidencia viven en
`../docs/modules/pilot-hardening-local-kill-switch.md`. El procedimiento local
de deshabilitacion, rollback y retiro vive en
`../docs/runbooks/device-agent-disable-rollback-retirement.md`.

Este control reduce la brecha local de deshabilitacion, pero no cierra el gate
corporativo: faltan owner, configuracion administrada, revocacion de identidad,
retencion, Security/Sophos, UEMS y ensayo en dos endpoints.

## Gates externos para dos endpoints

Se requiere evidencia verificable de:

- owner y procedimiento UEMS para desplegar, actualizar, hacer rollback y
  retirar;
- cuenta virtual o de servicio restringida, sin logon interactivo;
- revision minima de Security y tratamiento de Sophos;
- identidad de usuario y dispositivo aprobada;
- kill switch operable por un owner definido;
- logs, retencion, soporte y respuesta ante compromiso;
- paquete aprobado y mecanismo de confianza de publicador;
- dos endpoints, ventana, responsables y criterio de restauracion.

Las solicitudes concretas viven en
`../project-management/INFORMATION_REQUESTS.md`.

## Gate del Bloque 10

El bloque solo puede declararse `completed` cuando:

- el threat model fue contrastado con la implementacion y revisado por los
  owners aplicables;
- no quedan amenazas criticas sin control, rechazo explicito o stopper;
- despliegue, actualizacion, deshabilitacion, rollback y retiro fueron ensayados
  en dos endpoints autorizados;
- la identidad restringida y el comportamiento de Sophos fueron validados;
- logs y evidencia no contienen secretos, PII innecesaria ni salida operativa
  completa;
- el paquete y la confianza del publicador tienen mecanismo aprobado;
- `scripts/Validate.ps1`, auditoria de dependencias y escaneo de secretos pasan.

Hasta entonces el Bloque 10 permanece `in_progress` o pasa a `blocked` si solo
resta evidencia externa.

## Referencias

- `../core/SECURITY.md`
- `../core/ARCHITECTURE.md`
- `../core/DECISIONS.md`
- `OPERATIONS.md`
- `../docs/modules/local-mvp-lab.md`
- `../docs/threat-model/README.md`
- `../project-management/PILOT_ASSESSMENT.md`
- `../project-management/INFORMATION_REQUESTS.md`
