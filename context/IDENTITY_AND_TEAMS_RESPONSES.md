# Registro interno de respuestas: identidad y Teams

Fecha de consolidacion: 2026-06-08.

Este archivo conserva respuestas operativas aportadas durante el levantamiento.
No esta preparado para enviarse como cuestionario. Las decisiones vigentes se
reflejan en `../core/ARCHITECTURE.md`, `../modules/OPERATIONS.md`,
`../project-management/PILOT_ASSESSMENT.md` y `../core/DECISIONS.md`.

## Recuperacion desde el portal

**Respuesta aportada:** al pulsar "Olvide mi contrasena", el usuario es
redirigido a `recovery.softtek.com`. Si registro previamente su correo personal,
recibe un codigo, resuelve un CAPTCHA y puede crear una nueva contrasena. El
mismo flujo puede ayudar cuando la cuenta esta bloqueada. Si no registro ese
correo, Mesa de Ayuda solicita un reset manual.

**Validacion:** confirmado como comportamiento operativo. Sigue pendiente
confirmar si la tecnologia interna es Microsoft SSPR u otra plataforma.

## Usuarios con autoservicio

**Respuesta aportada:** esta disponible para usuarios con cuenta o correo
corporativo.

**Validacion:** parcialmente confirmado. La cobertura real esta condicionada a
que el usuario haya registrado previamente su correo personal.

## Relacion entre correo y dominio

**Respuesta aportada:** en `softtek.com`, cambiar la contrasena del correo
tambien actualiza la utilizada en el equipo. En `softtekprojects.com`, la cuenta
de dominio usa una contrasena diferente. Tras un reset administrativo, el usuario
entra con una credencial temporal y Windows le exige definir una nueva.

**Validacion:** confirmado como comportamiento observado. Queda pendiente saber
si `softtek.com` utiliza tecnicamente password writeback y cual es el tipo de
union de los equipos.

## Tipo de union de los equipos

**Respuesta aportada:** no hubo respuesta.

**Estado:** pendiente confirmar si son Entra joined, hybrid joined o domain
joined. Esta respuesta afecta la recuperacion desde login y la sincronizacion
con Active Directory.

## Funcion del portal

**Respuesta aportada:** el portal cambia la contrasena. Si el usuario no puede
acceder al portal ni a OpenText, otro colaborador levanta el ticket y Mesa de
Ayuda realiza el cambio directo.

**Validacion:** confirmado.

## Reset administrativo

**Respuesta aportada:** lideres autorizados de las sedes realizan el reset. Mesa
de Ayuda lo solicita por Teams, recibe la credencial temporal y la entrega
directamente al usuario.

**Validacion:** confirmado como proceso humano actual. No se confirmo una API
oficial para automatizarlo.

## Recomendacion de 48-72 horas

**Respuesta aportada inicialmente:** mostrar una alerta, solicitar confirmacion
con un boton "Entendido" y enviar otro recordatorio despues de 48 horas con el
portal y la guia de MFA.

**Decision posterior:** no implementar seguimiento automatico. El producto solo
muestra la recomendacion inicial de cambiar la credencial dentro de 48-72 horas
y evitar cambios consecutivos durante las primeras 24 horas.

## Entrega actual de la credencial temporal

**Respuesta aportada:** se envia directamente al correo personal y corporativo
del usuario, sin copiar al area, y tambien puede compartirse por su chat privado
de Teams. No se agrega al ticket.

**Validacion:** confirmado como practica actual. No se adopta como mecanismo
tecnico del producto: el chatbot, Power Automate, backend y logs no almacenaran
ni transportaran la credencial.

## Sesion abierta de Teams

**Respuesta aportada:** Teams normalmente permanece abierto mientras la cuenta
esta bloqueada y cierra la sesion cuando se cambia la contrasena directamente o
desde el portal.

**Validacion:** comportamiento observado, no garantia de seguridad ni de
duracion. El diseno no utilizara una sesion abierta de Teams como unica prueba
para autorizar un reset.

## Bot existente de Teams

**Respuesta aportada:** ya existe un bot personalizado. La intencion es que el
area de automatizacion lo amplie y lo conecte con el asistente nativo, en lugar
de crear otro bot.

**Validacion:** confirmado como direccion deseada. Falta identificar owner,
tecnologia, permisos y mecanismo de integracion.

## Capacidades actuales del bot

**Respuesta aportada:** permite crear tickets y consultar los generados desde el
propio bot. No consulta el catalogo, tickets especificos externos ni tickets
creados desde el portal.

**Validacion:** confirmado como alcance actual. Que pueda consumir un backend
compartido sigue siendo un supuesto por validar.

## Orden de la prueba de viabilidad

**Aclaracion posterior:** no se necesita crear otro prototipo de Teams. Ya existe
uno que abre tickets, aunque no entiende catalogo, licencias ni alternativas.

**Decision:** probar primero el agente nativo y la API compartida. Despues,
ampliar el bot existente para consultar esa API. Tanto WinUI como Teams deben
responder consultas sin acciones y pedir confirmacion antes de instalar o
escalar.

## Usuario sin Windows, correo ni Teams

**Respuesta aportada:** actualmente contacta a un agente por mensaje directo,
correo/cuenta personal o medios personales como WhatsApp si tiene el contacto.

**Validacion:** confirmado como practica informal, pero no como canal corporativo
oficial. El proyecto necesita que el area defina la ruta aprobada.

## Sin respuesta

- licencias disponibles de Power Automate, Copilot Studio y Entra;
- aprobacion de Power Automate Process desatendido;
- API o conector de OpenText;
- API de reset administrativo;
- posibilidad tecnica de que el bot existente use el backend compartido.
