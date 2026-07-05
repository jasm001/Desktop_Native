# Decisiones

## Cerradas

| ID | Decision |
| --- | --- |
| D-001 | El mockup React/Vite no evoluciona a producto real. |
| D-002 | La implementacion vivira en un repositorio independiente. |
| D-003 | Cliente nativo con C#, .NET 10 LTS, WinUI 3 y Windows App SDK. |
| D-004 | UI y servicio privilegiado son procesos separados. |
| D-005 | Plano web Next.js modular, Prisma y PostgreSQL; worker Node separado. |
| D-006 | Portal administrativo web separado, con RBAC por sede y area. |
| D-007 | Identidad desacoplada: desarrollo local y Microsoft Entra workforce con MFA en piloto/produccion. |
| D-008 | La IA no autoriza ni ejecuta acciones. |
| D-009 | Integraciones externas se implementan mediante adaptadores. |
| D-010 | El Excel corporativo es fuente inicial, no allowlist ejecutable directa. |
| D-011 | El piloto requiere distribucion corporativa y confianza verificable del publicador; mecanismo pendiente. |
| D-012 | OpenText Service Manager es la mesa de ayuda objetivo. |
| D-013 | GoTo/LogMeIn Rescue es el soporte remoto actual; no se clonara en el MVP. |
| D-014 | El piloto inicia en Mexico, limitado por sede y grupos de equipos. |
| D-015 | x64 y x86 se soportan si Windows 11, licencia y EOL son validos. |
| D-016 | Backend y datos viven preferentemente en Azure; Vercel no es backend principal. |
| D-017 | Hermes + API externa y una API Microsoft aprobada se evaluan por costo, seguridad y calidad. |
| D-018 | Los casos resueltos por el bot viven primero en metricas internas; OpenText se usa obligatoriamente para escalaciones. |
| D-019 | Next.js se prueba en Vercel/Supabase con datos sinteticos y puede migrar a Azure sin cambiar el dominio. |
| D-021 | Piloto objetivo de aproximadamente 100 usuarios en una sede pequena de Mexico. |
| D-023 | El portal usara identidad corporativa Microsoft Entra con MFA; no autenticacion local propia. |
| D-024 | Integracion API de Rescue queda fuera del MVP inicial; se conserva el flujo humano actual. |
| D-025 | El inventario del producto cubre solo dispositivos con agente y evidencia operativa; no reemplaza el inventario corporativo. |
| D-026 | La revision 134 del Excel es la fuente vigente inicial del catalogo. |
| D-027 | Solo se automatiza software con ejecucion determinista y probada: unattended oficial o paquete reempaquetado aprobado. |
| D-028 | El agente no usa `.\itlocal`, contrasenas compartidas ni credenciales VPN de usuarios. |
| D-029 | `gpupdate` solo se intenta con red corporativa/VPN ya activa y consentimiento para reinicio/logoff. |
| D-030 | Proveedores externos de IA requieren aprobacion; acceso a Copilot no implica acceso API. |
| D-031 | La identidad del Windows Service sera exclusiva, restringida y sin inicio de sesion interactivo. |
| D-032 | UEMS distribuye y puede obtener paquetes desde templates del fabricante, pero el catalogo Softtek gobierna cuales se publican. |
| D-033 | Instaladores configurables se automatizan con CLI, response files, MST, DSC o adaptadores propios; no con control visual generico. |
| D-034 | Hermes puede ejecutarse localmente consumiendo MiniMax/OpenRouter remoto; eso no significa que el modelo se aloje localmente. |
| D-035 | Fuera de LAN/VPN corporativa, acciones de dominio como `gpupdate` se escalan en el MVP. |
| D-036 | Solicitudes corporativas se dividen en minimo MVP y controles posteriores. |
| D-037 | Roles del portal se administran manualmente en MVP; Entra provee login/MFA. |
| D-038 | No se solicita storage propio para instaladores en MVP; UEMS/URLs oficiales son la ruta inicial. |
| D-039 | Semgrep CE es el SAST inicial; CodeQL CI depende de licencia corporativa. |
| D-040 | Reempaquetado MSIX por captura precede a automatizacion visual de installers. |
| D-041 | Desarrollo asistido usa roles logicos y modelos escalonados; no seis agentes residentes. |
| D-042 | Hermes se evalua primero en WSL2 y las validaciones WinUI/MSIX se ejecutan en Windows. |
| D-043 | OpenRouter inicia con USD 50/mes por dos meses, claves y limites separados. |
| D-044 | IDE, IA de desarrollo e IA del producto se presupuestan por separado. |
| D-045 | WinUI y Teams son canales del mismo asistente y comparten backend, catalogo y tickets. |
| D-046 | Power Automate complementa aprobaciones y procesos; no sustituye el agente Windows privilegiado. |
| D-047 | Recuperacion usa `recovery.softtek.com` y el flujo humano; SSPR/password writeback quedan por confirmar. |
| D-048 | No se implementa un Credential Provider propio ni chatbot sobre el login de Windows en el MVP. |
| D-049 | La sesion abierta de Windows o Teams no autoriza un reset; se exige MFA/step-up oficial. |
| D-050 | El primer MVP automatiza ticket y orientacion de recuperacion, no la generacion/entrega de contrasenas temporales. |
| D-051 | Una futura API de reset requiere aprobador independiente y nunca persiste la credencial temporal. |
| D-052 | El cambio dentro de 48-72 horas es un procedimiento operativo, no una expiracion tecnica automatica. |
| D-053 | El producto informa la recomendacion de cambio, pero no persigue ni bloquea al usuario si conserva la credencial temporal. |
| D-054 | `recovery.softtek.com` es el portal actual de autoservicio; no se afirma que sea SSPR hasta confirmar su tecnologia. |
| D-055 | El bot existente de Teams se ampliara como canal del backend compartido; no se creara un segundo bot independiente. |
| D-056 | Los flujos de identidad distinguen `softtek.com` de `softtekprojects.com`. |
| D-057 | La prueba inicia con agente nativo y API compartida; despues se amplia el bot de Teams existente. |
| D-058 | Una consulta nunca crea instalacion ni ticket sin confirmacion explicita posterior. |
| D-059 | WinUI y Teams usan los mismos estados de consulta, propuesta, confirmacion y accion. |
| D-060 | Una futura actualizacion de politicas se activa mediante campana administrativa explicita; el agente no detecta cambios de GPO mediante ping. |
| D-061 | La conexion automatica a red corporativa solo puede usar un mecanismo de maquina o pre-logon aprobado; nunca credenciales humanas almacenadas. |
| D-062 | El roadmap posterior prioriza evidencia, reparaciones cerradas, almacenamiento, perifericos, mantenimiento, perfiles y modo local antes del borrado/reprovisionamiento. |
| D-063 | El agente no persiste mediante firmware o kernel; despues de reinstalar Windows debe ser desplegado nuevamente por ISO/UEMS, Autopilot, provisioning package o bootstrap autenticado. |
| D-064 | La rama empresarial y la independiente comparten contratos y proveedores; ninguna dependencia corporativa se codifica dentro del dominio. |
| D-065 | El producto conserva asset tag y configuracion permitida, pero nunca deriva, almacena o muestra PIN o recovery password de BitLocker. |
| D-066 | Hermes/RAG puede explicar o seleccionar acciones conocidas; el agente nunca ejecuta scripts o argumentos generados por IA. |
| D-067 | La direccion corporativa o producto independiente se decide despues sin detener el MVP; los niveles local, personal/pro y equipos pequenos son opciones posteriores. |
| D-068 | Mientras se decide la aprobacion corporativa, puede construirse un perfil `local-demo` completo en una VM Windows 11 personal, usando solo datos publicos/sinteticos y proveedores locales o fake. |
| D-069 | El mirror local de laboratorio es un proveedor de desarrollo para software libre con redistribucion permitida, manifiesto e integridad obligatoria; no sustituye ni contradice la decision productiva de usar UEMS/URLs oficiales. |
| D-070 | Hermes puede ejecutarse localmente con RAG e indice locales y consumir una API externa para inferencia. Sin conexion, el producto conserva busqueda/conocimiento local y flujos deterministas; no se presenta esa configuracion como LLM offline. |
| D-071 | El agente puede operar sin conexion solo con politicas, autorizaciones, acciones y artefactos ya disponibles. En `local-demo`, una politica de desarrollo instalada y confirmacion local pueden autorizar acciones allowlisted; esa politica no se promueve a piloto. |
| D-072 | Los Bloques 6-11 pueden preparar un MVP local mediante proveedores reemplazables. Los gates corporativos de UEMS, Entra, OpenText, Teams, PKI, Security y conectividad permanecen pendientes y separados. |
| D-073 | El perfil `local-demo` puede persistir y mostrar datos reales de laboratorio saneados bajo la categoria `lab-real-sanitized`, siempre que sean generados por componentes locales controlados, no contengan datos corporativos ni secretos, esten etiquetados como laboratorio y no cierren los Bloques 9, 10 u 11. |

## Supuestos de trabajo por validar

| ID | Supuesto |
| --- | --- |
| W-001 | UEMS Endpoint Central distribuira cliente, agente y configuracion inicial. |
| W-002 | Los templates oficiales y auto-update de UEMS evitaran un servidor propio de instaladores durante el MVP. |
| W-003 | Una sede entre Guadalajara, Ensenada, Merida o Queretaro alojara aproximadamente 100 usuarios. |
| W-004 | Digital Office puede crear grupo/cuenta tecnica del bot en OpenText. |
| W-005 | La empresa proporcionara recursos Azure para el piloto. |
| W-006 | Network/Endpoint Management puede entregar un perfil FortiClient EMS de maquina, pre-logon o equivalente para el equipo de prueba. |
| W-007 | Axis Atmos puede proporcionar al equipo de prueba las rutas de dominio necesarias para DNS, controlador de dominio, `SYSVOL` y `NETLOGON`. |
| W-008 | La ISO/UEMS corporativa puede incluir un bootstrap firmado del agente y volver a instalarlo despues del borrado. |
| W-009 | Un portal independiente puede autenticar al usuario y entregar un bootstrap de un solo uso sin incluir secretos ni configuracion sensible en el instalador. |

## Requieren decision de negocio

1. Nombre comercial y propietario legal del producto.
2. Uso interno exclusivo o futuro producto multiempresa.
3. API, autenticacion tecnica y ambiente de prueba de OpenText.
4. Confirmar con Endpoint Central el proceso y requisitos para desplegar nuestra
   aplicacion al grupo piloto.
5. Rescue queda para fase posterior; confirmar solo el flujo manual del piloto.
6. Proveedor/API de IA aprobado dentro del ecosistema corporativo.
7. Region Azure y requisitos de residencia/replicacion internacional.
8. Retencion de tickets, inventario, auditoria, sesiones y prompts.
9. Owner operativo que normaliza paquetes; Security conserva aprobacion de uso.
10. Matriz de aprobacion por categoria, licencia, sede y proyecto.
11. PKI interna y mecanismo para confiar el certificado en endpoints.
12. Azure DevOps o GitHub como plataforma corporativa.
13. App registration Entra para login; grupos/provisionamiento quedan para fase posterior.
14. Sede definitiva y lista de aproximadamente 100 equipos/usuarios.
15. Confirmar que UEMS puede sincronizar templates y descargar fabricantes desde
    la red corporativa sin repositorio adicional.
16. Si OpenText permite grupo/assignee tecnico propio para tickets del bot.
17. Si `help.softtek.com` autoriza iframe desde el dominio del portal.
18. Presupuesto mensual para Vercel/Supabase o recursos Azure.
19. Politica aprobada para validar EOL y fuentes web de fabricantes.
20. Retencion exacta por clase de dato.
21. Licencias disponibles de Power Automate Process, Copilot Studio y Entra
    SSPR/password writeback.
22. Tipo de union de equipos: Entra joined, hybrid joined o dominio local.
23. Canal alterno para usuarios sin acceso simultaneo a Windows y Teams.
24. API/procedimiento y grupo autorizado para restablecimiento administrativo.
25. Texto oficial que debe mostrarse sobre la ventana de 48-72 horas y la espera
    recomendada de 24 horas entre cambios.
26. Si el producto sigue como iniciativa corporativa, producto independiente o
    ambas variantes con proveedores distintos.
27. Mecanismo corporativo actual para iniciar una reinstalacion remota sin USB y
    si permite agregar un bootstrap firmado a la ISO/UEMS.

## Gate para crear el repositorio

Se puede crear ya el repositorio, portal, cliente, agente simulado y adaptadores
fake.

Para probar en dos equipos con acciones reales solo se necesita:

1. aprobacion del lider;
2. proceso UEMS para distribuir/retirar;
3. login Microsoft o identidad temporal aprobada;
4. revision minima de Security;
5. cuenta restringida del agente;
6. paquetes UEMS/URLs oficiales aprobados;
7. kill switch, logs y consentimiento de reinicio.

OpenText, IA externa, PKI formal, Azure productivo, webhooks, mirror propio,
break-glass y retencion corporativa completa pueden desarrollarse con fakes y
cerrarse antes del piloto de aproximadamente 100 usuarios.
