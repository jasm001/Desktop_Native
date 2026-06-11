# Preguntas para cerrar el piloto

> Registro bruto de respuestas del 2026-06-07. La evaluacion y estado vigente
> estan en `../project-management/PILOT_ASSESSMENT.md`; las solicitudes
> accionables estan en `../project-management/INFORMATION_REQUESTS.md`.

## Bloqueantes

1. Que sede de Mexico, cuantos equipos y cuantos usuarios participan?
R: tratare de que sea por volumen de usuario, hasta donde se, la mayoria se concentra en las sedes de MTY y las 3 de CDMX, entonces, voy a tratar de que sea una sede con menor volumen como Guadalajara, Ensenada, Mérida o Queretaro, le estimo que sea un volumen de 100 usuarios.
2. Que herramienta distribuira cliente, agente, certificado y configuracion:
   Intune, Configuration Manager o Endpoint Central?
R: no ando seguro, pero es probale que la herramienta que distribuya el proyecto sera UEMS Endpoint Central, creo que veces anteriores fue por ese medio para otros programas que actualizaron de manera global en la empresa.
3. Existe PKI interna y el area puede confiar un certificado de publicador en
   `Trusted People/Trusted Publishers` de los equipos piloto?
R:No tengo esta informacion, asi que queda pendiente, aunque certificados de publicador, no estoy seguro, pero para aplicaciones y .exe si se puede solicitar una excepcion para que el antivirus Sophos no lo elimine o marque como virus.
4. ¿Dónde se almacenarán los instaladores aprobados de terceros que usará el agente: Azure Blob privado, file share o repositorio corporativo existente?
R: Se va a solicitar un servidor, pero en MVP no estoy seguro si nos den uno, no se que tan probable sea que el bot pueda descargarlo de internet y usar los de la tienda de microsoft en la web para descargar, ya que nativamente la store esta bloqueada por seguridad, aunque por comando de powershell me parece que si se pueden instalar aun, pero casi nadie sabe eso
5. ¿Dónde se publicarán las versiones firmadas del cliente y agente para que el sistema corporativo de distribución las instale y actualice?
R: Lo mas probable es que se almacenen en azure o yo comparta el instalador y solo lo distribuyan
6. Quienes son owners de catalogo, seguridad, licencias/compras y antivirus?
R: del catalogo de apps permitidas el área de seguridad: security@softtek.com y a IT.SecandComp@softtek.com ; los mismos correos son los que estan a cargo de seguridad, osea igual para aprobar los programas que no estan en la lista de permitidos y dar excepciones de uso para esos programas, de licencias si hay un owner de una distribution list, pero estos de momento, esos tickets los debe abrir el usuario de manera manual pues son las sedes a las que pertnecen los usuarios quienes llevan el proceso de compra y asignacion junto con el area de compras, de antivirus es un area interna, no hay un correo como tal, pero para antivirus se manejan como IM fuera de los tickets normales y nosotros como agentes solo levantamos solicitudes por los usuarios, somos como el puente, asi que cualquier cosa de seguridad puedo preguntarles directamente
7. Que producto/version exacta de OpenText usa `help.softtek.com` y existe un
   ambiente/API de prueba?
No sabria decirte, pero si es ambiente de produccion, supongo que el equipo que lleva ese portal tendra un ambiente de pruebas, seria que solicitara informacion sobre eso
8. La licencia de Rescue permite crear sesiones/API y devolver resultados?
R: no lo se, pero de momento esa integracion podemos verla despues y enfocarnos en el MVP, los agentes de momento para escalaciones seguiran el flujo actual, ven el ticket en opentext, contactan a los usuarios por teams, y le proporcionan la url de conexion a los usuarios usando logmein
9. Que retencion se requiere para inventario, auditoria, tickets, IA y sesiones?
R: de inventario de los equipos lo lleva otra area con su propio portal, no nos inmiscuiremos con ellos, que de su lado de momento sigan trabajando como andan, pero si te refieres a otra cosa, se puede llevar un inventario con los equipos que tienen instalado el agente cuando se despliegue a una sede, por auditoria en el portal lo mas recomendable es que ahi tenga acceso el area de seguridad para auditar las acciones que haga el bot de instalaciones o cosas asi, de tickets se haran las metricas desde opentext aunque la web tendrá igual un reporte de los tickets que ha abierto internamente para si mismo y cuantos ha escalado, cuantos cerrados y cuantos con calificacion, de ia no estoy seguro aun, de sesiones supongo que las veces que han consultado algo al bot
10. Puede crearse un assignment group/usuario tecnico para tickets resueltos por
   el bot y asignarlos directamente por API?
R:Deberia poderse, se solicita con el area interna de Digital Office como un grupo dedicado al bot, de la APO para asignarselos, pues como se comenta, se tiene que consultar con el equipo que lleva el portal actual de OpenText de help.softtek.com
11. La aplicacion OpenText permite iframe desde el dominio del portal?
como usa credenciales OAUTH de Microsoft con nuestros correos empresariales para acceder, supongo que deberia siempre y cuando esten logeados con su correo y puedan ver la aplicacion con iframe

## Identidad

1. Se puede crear un tenant/directorio separado para el piloto?
R: Si, si se refiere como servicio o cuenta aparte para instalar y tenga sus propios accesos admin y poder limitarle lo que haga para rollback como aqui en codex, si se puede crear, tambien el grupo para dentro de opentext de los tickets
2. Se exige MFA desde el primer dia?
R: para el web? si, toda cuenta corporativa cuando accedemos requiere MFA vinculado a nuestro correo empresarial, de hecho, el de opentext usa nuestro correo y debemos iniciar sesion con el MFA
3. Se permiten cuentas locales invite-only o debe usarse Entra corporativo?
R: se puede solicitar que se creen cuentas locales, por ejemplo, en los equipos hay 2 dominios, softtekprojects para los usuarios que estan dando o prestando servicio a clientes y usen el equipo para programar y ahi dentro hay un usuario local pero en red: .\itlocal
que esta disponible en todos los equipos y que solo los agentes saben la contraseña global para acceder y hacer configuraciones o instalaciones que requieran de permisos admin, el otro dominio es softtek.com donde estoy yo y lo usan los de staff (directivos, los de soporte como help desk, red, security, RH, las de reclutamiento, etc)
4. Que dominios de correo, sedes y areas se pueden invitar?
R. dominios de correo, pues todos los que provea la empresa, normalmente tenemos el domonio de correo: @softtek.com asi que seria el principal, para el MVP controlado, como ya lo discutimos, seria una sede pequeña como Mérida, Guadalajara, Ensenada o Queretaro
5. Quienes custodiaran las dos cuentas break-glass?
R: Tengo dudas a que te refieres

## Red y dominio

1. Como detectan hoy la VPN activa y el dominio alcanzable?
R: para la vpn depende del dominio, digamos que si mandan la politica a un usuario cuyo dominio esta en softtekprojects, el agente con su cuenta vpn para ese dominio, la levanta en forticlient y hace las actualizaciones para las politicas que le asignen, mismo caso para dominios softtek.com, hay otra vpn que se puede dejar para todos los equipos y es la de Axis Atmos pero no todos lo usan o lo tienen, se puede lanzar o solicitar que si un equipo no lo tiene para la sede de prueba, se les instale el axis atmos, que hace que solo esten conectados a nuestras redes como si estuvieran en oficinas
2. Puede el cliente VPN abrirse con deep link o CLI sin guardar credenciales?
R: no, al menos para forticlient es necesario cuenta vpn, el de Axis Atmos solo basta con que el usuario inicie sesion con su cuenta de correo normal, el problema es que para la cuenta de servicio o perfil para el bot tendría que iniciar igual sesion por su cuenta
3. Que acciones requieren LAN/VPN: `gpupdate`, cambio de password, certificados,
   unidades, impresoras u otras?
R: De momento para vpn solo el gpupdate, el cambio de pass en los equipos cuando expiren (en softtekprojects usan pass diferente el correo, el dominio de softtek.com usa la pass de correo), en softtekprojects para extender cuentas, el de softtek.com no requiere vpn pues esta vinculada a la cuenta de correo y nunca expira la cuenta a menos que lo den de baja al usuario o expire su pass, pero ahi puede renovarla en el portal interno para restablecer pass, certificados creo que no, impresoras las configuran en sede asi que no aplica y es todo creo de momento
4. Que acciones permiten reboot/logoff y como se solicita consentimiento?
R: te refieres a como lo solicitara el bot? para gpupdate si se le solicitara al usuario que guarde sus archivos para reiniciar y aplicar cambios, para algunos programas igual requieren de reinicio o al desinstalar y reinstalar, el bot puede reiniciar siempre y cuando el usuario le confirme que puede proceder

## IA

1. Hay VM/GPU disponible y quien la opera/parcha?
R: VM se puede solicitar alguna, pero creo que son sencillas como un equipo normal y son de azure, GPU desconozco, las asignaciones las hace el area de servidores, pero como genera costo debe ser previamente aprobado asi que esta complicado de hacer una VM con hermes y GPU potente para que se almacene en local la ia
2. Que modelo exacto significa M3 y cual es su licencia de uso empresarial?
R: M3 me refiero a la ia de Minimax M3: https://platform.minimax.io/docs/guides/pricing-paygo
para usarlo de manera local con Hermes, para abaratar costos para el MVP veo como opciones este, Deepseek V4 pro/flash o Qwen 3.7 plus
3. Se permite que prompts/tickets salgan de la red hacia Azure OpenAI?
R: Actualmente se nos pide no usar modelos externos como directamente chatgpt o claude, en su lugar usar el de Copilot y que veo que tiene de modelos los de Open Ai de GPT 5.5 High o 5.5 Low, solo se pueden usar los internos de Microsoft que nos dan en nuestra cuenta, asi que creo que si siempre y cuando sea de nuestro ecosistema del dominio de la empresa
4. Que conjunto anonimizado se usara para evaluar exactitud en espanol?
R: no entiendo la pregunta
5. Que presupuesto mensual maximo se acepta para IA?
R: aun no lo tengo esos datos, aun no esta como tal aprovado el proyecto pero estoy avanzando lo mas que pueda para tener avanzado este tipo de cosas

## Presupuesto y hosting

1. La empresa entregara suscripcion Azure o el MVP se financiara externamente?
R: para la web, y cosas internas, si, deberian proveer licencia y lo que se necesite
2. Se permite Vercel/Supabase solo con datos sinteticos durante desarrollo?
R: Voy a solicitar aprobacion para usarlo de manera interna con el correo de softtek o en su defecto me provean los ambientes de desarrollo y testeo
3. GitHub Enterprise incluye GitHub Code Security/CodeQL para repos privados?
R: es de licencia el github, pero hasta donde se, creo que si o no, si te refieres al uso de copilot, personalmente preferiria solicitar la cuenta normal y repo, para usar externamente codex
4. Que gasto mensual maximo cubre web, base de datos, storage, IA y monitoreo?
R: sin datos aun

## Catalogo

1. Que revision del Excel es vigente y quien publica actualizaciones?
R: el compartido es el vigente y actualizado, que seria la version 134 del excel compartido
2. Se automatiza todo lo aprobado o solo lo que tenga instalador silencioso?
R: deberia ser todo lo aprobado del excel y ver la manera que tenga instalador silencioso, si no ver la manera de agregar algo parecido a playwright para configuracion (Ejemplo, SQL Server express ocupa configuraciones, el SSMS igual ocupa, aunque como tal se puede instalar la version nueva asi como esta sin aditamientos)
3. Que software comercial debe aparecer como solicitable pero no instalable?
R: de manera interna nosotros sabemos cuales son, asi que si no aparece, escalar ticket, si es uno no autorizado, dar alternativas, por ejemplo, postman requiere si o si de licencia, pero las alternativas que sabemos y estan en el excel, es insomnia o Soapui, por ejemplo si filezilla esta como no permitido, en lugar de cerrar asistencia del bot, dar la alternativa que sale en el excel como winscp si no hay alternativas, cerrar asistencia pero no directamente, si no sugerirle que busque alternativas y digamos nos da una, decirle que se escalara la peticion y se abre el ticket para no dejar en el limbo al usuario
4. Que entradas son exclusivas por pais, cliente o proyecto?
R: se maneja otra lista, pero es de manera interna, si no sale es mejor escalar ticket, la compartiria pero no es un excel asi que es un poco complicado, igual la intencion seria centralizar toda esa informacion para poder hacerlo mas automatizable este tipo de cosas y el bot tenga acceso a esa informacino sin modificar nada
5. Que criterio define EOL y quien suspende paquetes vulnerables?
R los programas EOL, digamos, serian los permitidos, solo se pueden versiones LTS o Current, por ejemplo de microsoft no se pueden los .net sdk EOL, ejemplo el 6, no se permite, de node mismo caso, solo los LTS o Current, nadie lo suspende pero de eso nos encargamos nosotros de evaluar las peticiones asi que el bot tendria que buscar en la web si le piden el net sdk 6 y ve que esta EOL, decir que no se puede y la alternativa es la ultima version o la que esta aun con actualizaciones
