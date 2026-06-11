# Evaluacion de respuestas del piloto

Fecha: 2026-06-07.

## Confirmado

- Piloto en Mexico para aproximadamente 100 usuarios.
- Se prefiere una sede de menor volumen; sede definitiva pendiente.
- UEMS Endpoint Central es el canal mas probable de distribucion.
- Portal con identidad corporativa Microsoft Entra y MFA.
- Dominio principal de correo: `@softtek.com`.
- Revision vigente del catalogo: Excel version 134.
- Security gobierna aprobacion de software y excepciones.
- Licencias, backups y formateos permanecen manuales/escalados.
- Rescue conserva su flujo humano actual durante el MVP.
- Inventario corporativo general queda fuera del producto.
- El portal registra dispositivos con agente y metricas del bot.
- `gpupdate` y reinicios requieren red corporativa y consentimiento.
- Existe `recovery.softtek.com` para autoservicio mediante correo personal
  registrado, codigo de verificacion y CAPTCHA.
- `softtek.com` comparte contrasena entre correo y equipo; `softtekprojects.com`
  usa una credencial de dominio separada.
- Ya existe un bot corporativo de Teams para crear tickets.

## Parcialmente confirmado

### Sede

Candidatas: Guadalajara, Ensenada, Merida o Queretaro. Falta confirmar owner,
usuarios, equipos, ventana de despliegue y soporte local.

### Endpoint Central

Es viable como canal de despliegue y repositorio. ManageEngine ofrece templates
predefinidos, descarga desde fabricantes y auto-update. Para el proyecto no es
necesario diagnosticar ni modificar la operacion interna actual. Solo se requiere
acordar:

- como entregar cliente, agente y configuracion;
- ejecucion como System, deteccion, retry y rollback;
- distribucion a un grupo/sede piloto;
- certificados/configuracion requeridos;
- logs y resultado del despliegue.

### Artefactos

Para el MVP puede no requerirse servidor propio: UEMS puede obtener instaladores
desde fabricantes y almacenarlos en su repositorio HTTP/WAN. El catalogo Softtek
y el portal siguen gobernando aprobacion/version. Blob propio queda para
excepciones o layouts no cubiertos.

### Firma y Sophos

Una excepcion Sophos puede evitar bloqueo, pero no aporta identidad de publicador
ni protege actualizaciones. Para viabilidad se solicita solo si aparece un
bloqueo. Firma/PKI formal queda para despues de aprobar el MVP.

### OpenText

Es probable crear grupo/cuenta tecnica del bot, pero se necesita confirmacion de
Digital Office y del equipo de Service Manager: API, sandbox, campos, estados,
asignacion, cierre e iframe.

### Recuperacion de cuenta

El portal corporativo permite cambiar la contrasena si el usuario registro
previamente su correo personal. No esta confirmado que la tecnologia subyacente
sea Microsoft SSPR.

Sin registro previo, otro usuario crea el ticket y Mesa de Ayuda solicita el
reset a un lider autorizado de la sede. Para `softtekprojects.com`, la credencial
temporal obliga el cambio al iniciar sesion. La automatizacion del reset y su API
siguen pendientes.

La entrega actual de la credencial ocurre por correo directo o chat privado de
Teams y no se agrega al ticket. El producto documenta el proceso actual, pero no
transportara ni almacenara la credencial.

### Teams

El bot existente permite crear tickets y consultar los generados desde ese mismo
canal. No consulta catalogo ni tickets creados fuera del bot. Se busca ampliarlo
y conectarlo al backend compartido con WinUI.

## Correcciones conceptuales

### Tenant, identidad y cuenta del agente

Son elementos diferentes:

- tenant/directorio: autentica usuarios y aplicaciones;
- service principal/managed identity: autentica workloads cloud;
- cuenta del Windows Service: identidad local del proceso del agente;
- assignment group OpenText: enrutamiento de tickets.

No se crea una sola cuenta con permisos globales para sustituirlos.

### `.\itlocal` e identidad del bot

La cuenta compartida existente no es apropiada para el agente: dificulta atribuir
acciones y aumenta movimiento lateral si se filtra la contrasena. El servicio usa
cuenta virtual/local restringida. `.\itlocal` conserva su uso actual por agentes
humanos y no se modifica como parte del MVP.

El bot usa una identidad exclusiva sin acceso interactivo. Que sea separada no
significa darle un perfil de escritorio o credenciales utilizables por personas.

### Break-glass

Son dos cuentas de emergencia del portal/tenant:

- no se usan diariamente;
- tienen MFA fuerte y custodia separada;
- permiten recuperar administracion si falla SSO, grupos o configuracion;
- cada uso genera alerta y revision.

No son cuentas del agente ni administradores locales de Windows.

Para no sobredimensionar el MVP, se documentan ahora y se implementan cuando el
portal pase a operacion formal.

### Dataset anonimizado de evaluacion

Es un conjunto de preguntas/tickets historicos sin nombres, correos, hostnames,
tags, clientes ni secretos, junto con la respuesta/categoria esperada. Sirve para
comparar modelos en espanol y medir:

- clasificacion correcta;
- respuesta segura;
- escalamiento correcto;
- invencion de datos;
- latencia y costo.

## Restricciones nuevas

- Copilot corporativo no equivale a una API para el producto.
- Hermes puede correr localmente usando MiniMax/OpenRouter remoto. Esto no es
  self-hosting del modelo y sigue sujeto a salida de datos/aprobacion.
- Claude Enterprise esta aprobado como herramienta humana corporativa, sujeto a
  asiento y limite mensual. Eso no equivale a una API aprobada para el producto.
- MiniMax/OpenRouter/DeepSeek/Qwen para el producto siguen sujetos a aprobacion.
- OpenRouter puede filtrar providers por politica/ZDR, pero la aprobacion debe
  considerar el procesamiento y terminos de cada proveedor.
- Software con UI puede automatizarse si el fabricante ofrece CLI, response
  files, MSI/MST o configuracion declarativa; no mediante clics genericos.
- Playwright no automatiza instaladores Win32.
- EOL se resuelve desde fuentes oficiales curadas en catalogo.
- La Store bloqueada no implica que WinGet este permitido.

## Pendientes criticos

1. Sede y lista de equipos.
2. Contacto y proceso de Endpoint Central para desplegar nuestra aplicacion.
3. PKI/firma y reglas Sophos.
4. OpenText API/sandbox/grupo del bot.
5. Hosting Azure y presupuesto.
6. Proveedor API de IA aprobado.
7. Confirmar cobertura real de templates UEMS y excepciones que requieren storage.
8. Retencion de datos.
9. Politica de EOL y actualizacion del catalogo.

## Desarrollo asistido por IA

Claude Enterprise puede servir como complemento aprobado, pero el limite mensual
reportado puede ser insuficiente para sostener todo el desarrollo. El proyecto no
debe depender de una sola herramienta de vibecoding:

- repositorio y documentacion son la fuente de verdad;
- CI valida compilacion, pruebas y seguridad;
- cambios generados se revisan por modulo;
- Codex u otra herramienta externa requiere excepcion/politica;
- ningun asistente recibe secretos o datos productivos.

El perfil inicial propuesto es Hermes en WSL2 con ejecucion de build/pruebas
WinUI en Windows. Una VM Ubuntu puede aislar investigacion y desarrollo web, pero
no sustituye Visual Studio, Windows App SDK, MSIX ni UI Automation de Windows.

El presupuesto se valida primero durante dos meses con limites por clave. No se
comprometen seis meses antes de medir costo por tarea aceptada.
