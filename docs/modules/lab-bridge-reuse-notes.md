# Reutilizacion del laboratorio Bunny Bridge

## Estado

Nota modular de contexto para el MVP local. No cambia el estado de ningun
bloque, no introduce una integracion nueva y no habilita UEMS, Copilot Studio,
Power Automate, Entra, Teams, OpenText ni servicios productivos dentro de este
repositorio.

El laboratorio externo
`C:\Users\Ruruu\Documents\IT Support studio\bunnysoftware-endpointcentral-lab`
demuestra un patron controlado que puede reutilizarse como referencia de
arquitectura para cerrar el ciclo de laboratorio de IT Support Native.

## Lectura del laboratorio externo

El laboratorio externo ya contiene:

- red Hyper-V aislada con salida NAT;
- dominio de laboratorio `bunnysoftware.local`;
- servidor `AdmBunny-DC01`;
- endpoint administrado `DevBunny-01`;
- usuario de dominio no administrador local;
- Endpoint Central/UEMS trial con agente instalado;
- despliegue manual validado desde la consola UEMS;
- Copilot Studio, Power Automate, on-premises data gateway y Bunny Bridge API;
- bridge local en modo `validate-only`;
- evidencia de bloqueo externo para despliegue automatizado por API publica de
  Software Deployment.

Los dominios, hostnames y usuarios del laboratorio fueron elegidos para simular
un entorno empresarial realista. Pueden ayudar a explicar la demo y el recorrido
de prueba, pero siguen siendo valores de laboratorio. No deben tratarse como
tenants, dominios, hostnames, owners, permisos, ambientes o asignaciones
productivas confirmadas. En una aprobacion real pueden requerir ajustes de
identidad, naming, politicas, red, permisos y gobierno.

## Patron reutilizable

El valor reutilizable es el contrato y la frontera, no la dependencia concreta
de UEMS.

El bridge expone:

- `GET /health`;
- `POST /software/install`.

El contrato de solicitud usa:

```json
{
  "hostname": "DevBunny-01",
  "software": "notepadplusplus",
  "requestedBy": "ruruu.demo@bunnysoftware.local",
  "confirmed": true
}
```

La respuesta positiva incluye:

- `accepted`;
- `requestId`;
- `hostname`;
- `resourceId`;
- `software`;
- `softwareKey`;
- `packageId`;
- `deploymentSubmitted`;
- `initialStatus`;
- `message`.

Para este repositorio, el mismo patron puede informar un futuro adaptador de
laboratorio:

1. validar identidad/requester;
2. validar confirmacion explicita;
3. validar hostname contra una allowlist o inventario controlado;
4. validar software contra catalogo aprobado;
5. resolver el endpoint contra un inventario/proveedor;
6. crear folio/correlacion;
7. devolver estado inicial sin prometer ejecucion si el proveedor no acepto la
   orden;
8. mantener el proveedor reemplazable.

## Revision estatica de scripts

`scripts/Start-BunnySoftwareBridgeApi.ps1`:

- carga configuracion desde `.env`;
- no imprime secretos;
- exige `UEMS_BASE_URL` y `UEMS_API_KEY` o `UEMS_AUTH_TOKEN`;
- exige `BRIDGE_ALLOWED_REQUESTER_UPN`;
- usa `BRIDGE_ALLOWED_HOSTNAME`, con `DevBunny-01` como default de laboratorio;
- exige `BRIDGE_API_KEY` salvo modo local explicito
  `-AllowUnauthenticatedLocal`;
- acepta autenticacion por header `X-BunnyLab-Api-Key` o Basic Auth con usuario
  `BRIDGE_BASIC_USERNAME`/`bunnylab` y password `BRIDGE_API_KEY`;
- valida `confirmed=true`;
- rechaza requester, hostname, software y catalogo desconocidos;
- resuelve el equipo con `GET /api/1.4/som/computers`;
- contiene un catalogo local acotado donde solo `notepadplusplus` esta
  `DeploymentReady=true`;
- mantiene `BRIDGE_UEMS_DEPLOYMENT_MODE=validate-only` como unico modo
  soportado;
- rechaza cualquier otro modo de despliegue;
- no llama rutas internas `/dcapi/...`;
- responde `deploymentSubmitted=false` y
  `validated_pending_supported_uems_deployment_endpoint`.

`scripts/Test-BunnySoftwareBridgeApi.ps1`:

- lee `BRIDGE_BASE_URL`, `BRIDGE_API_KEY`, `BRIDGE_BASIC_USERNAME` y
  `BRIDGE_ALLOWED_REQUESTER_UPN` desde `.env`;
- envia una solicitud fija o parametrizada a `/software/install`;
- permite probar header propio o Basic Auth;
- muestra URL, hostname, software y requester, pero no imprime la clave.

`scripts/Validate-BunnyGatewayHost.ps1`:

- busca servicios locales del on-premises data gateway;
- consulta `GET /health` del bridge;
- valida conectividad TCP desde el host hacia UEMS en puertos de laboratorio;
- no lee `.env` ni imprime secretos.

`scripts/Invoke-EndpointCentralApi.ps1` y
`scripts/Get-EndpointCentralComputer.ps1`:

- encapsulan llamadas de lectura/escritura a `/api/...`;
- cargan autorizacion desde `.env`;
- no imprimen la autorizacion;
- advierten cuando se intenta usar rutas que no parecen API REST publicas;
- sirven como diagnostico de laboratorio, no como contrato de este repo.

`scripts/Invoke-UEMSInternalDcApiLabOnly.ps1`:

- esta marcado explicitamente como investigacion de laboratorio;
- requiere cookies, CSRF o headers de sesion exportados de la consola web;
- usa rutas internas `/dcapi/...`;
- no debe heredarse como integracion, contrato productivo ni mecanismo de este
  repositorio.

## Que se puede reutilizar aqui

- El contrato estable entre canal conversacional, orquestador y bridge.
- El modelo `validate-only` como estado honesto cuando falta proveedor real.
- La separacion entre conocimiento del agente, confirmacion, validacion y
  ejecucion.
- El uso de un gateway o bridge para no exponer consolas administrativas a
  Internet.
- El uso de allowlists para requester, hostname y software.
- La respuesta con folio, correlacion, estado inicial y bandera explicita
  `deploymentSubmitted`.
- La documentacion de bloqueo externo cuando el proveedor no ofrece endpoint
  soportado.

## Que no se reutiliza

- Rutas internas `/dcapi/...`.
- Cookies, CSRF, headers de sesion o replay de consola web.
- Credenciales humanas o claves en prompts, Markdown, capturas o logs.
- IDs concretos de UEMS como supuestos productivos.
- El dominio o hostname de laboratorio como dato corporativo confirmado.
- La afirmacion de instalacion real cuando `deploymentSubmitted=false`.
- Power Automate como frontera de autorizacion o ejecutor privilegiado.
- Copilot Studio como fuente de autorizacion.

## Relacion con los bloques

Bloque 9:

- el laboratorio refuerza que Copilot Studio/Power Automate pueden ser un canal
  u orquestador;
- no desbloquea la integracion corporativa real del bot existente;
- no reemplaza el stopper de owner, tenant, plataforma, DLP, permisos y
  despliegue.

Bloque 10:

- el laboratorio aporta evidencia de un flujo aislado y controlado;
- no valida UEMS corporativo, Security/Sophos, cuenta restringida, firma,
  retiro ni dos endpoints autorizados;
- no convierte `local-demo` en piloto.

Bloque 11:

- el documento puede guiar superficies de solo lectura del portal, por ejemplo
  estado de conectores, adaptadores o laboratorio;
- no obliga a mostrar UEMS en el esqueleto `/admin/*`;
- no autoriza mutaciones administrativas ni integraciones reales;
- no cambia que el portal local debe permanecer sintetico, server-side,
  fail-closed y de solo lectura.

## Criterio para una futura integracion de laboratorio

Si este repositorio adopta un adaptador inspirado en Bunny Bridge, debe cumplir:

- perfil de laboratorio explicito y fallar cerrado fuera de `Development`;
- secretos solo en variables de entorno o vault aprobado;
- requester, hostname, software, payload y versiones conocidos;
- confirmacion explicita;
- idempotencia, correlacion y auditoria antes de cualquier accion;
- proveedor reemplazable detras de interfaz;
- ausencia de comandos, scripts, rutas, argumentos libres o texto operativo;
- ningun acceso directo desde portal, Teams, Copilot o IA al DeviceAgent;
- pruebas de acceso permitido/denegado, no mutacion y ausencia de secretos;
- documentacion clara cuando el estado sea `validate-only`.

## Pendientes externos separados

- respuesta oficial de ManageEngine sobre endpoint soportado de Software
  Deployment;
- mecanismo productivo de identidad y autorizacion;
- owners y permisos de Power Platform/Copilot Studio;
- DLP, gateway, secretos, retencion y auditoria;
- decision de proveedor para despliegue real;
- revision Security antes de usar datos, tenants o endpoints corporativos.

Estos pendientes no bloquean el cierre del esqueleto local del portal si el
trabajo permanece sintetico, protegido en servidor, no mutante y sin conexiones
externas.
