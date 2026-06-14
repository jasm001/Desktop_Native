# Refresco administrado de politicas de dominio

## Estado y alcance

Capacidad planeada para despues del MVP. No forma parte del Bloque 5 y no debe
implementarse hasta cerrar los gates de FortiClient EMS o Axis Atmos.

Su objetivo es reemplazar el procedimiento manual de abrir una sesion
administrativa, conectar una VPN con credenciales humanas, ejecutar `gpupdate` y
reiniciar. El producto nunca almacenara esas credenciales ni creara una sesion
oculta de tecnico.

El rechazo de ambos mecanismos de conectividad no bloquea el proyecto. En ese
caso se conserva el comportamiento del MVP: detectar falta de red corporativa,
orientar al usuario y escalar.

## Flujo propuesto

1. El owner operativo de AD/UEMS publica una campana explicita en el control
   plane.
2. El DeviceAgent consulta por HTTPS saliente cada hora con jitter configurable.
3. El agente valida identidad de dispositivo, vigencia, alcance e idempotencia.
4. WinUI muestra impacto estimado y permite aceptar o posponer.
5. Un mecanismo corporativo aprobado establece conectividad de maquina.
6. El agente valida DNS, controlador de dominio, `SYSVOL` y `NETLOGON`.
7. Ejecuta la accion fija `group-policy.refresh.computer.v1`.
8. Reporta resultado, logoff requerido o reinicio requerido.
9. Logoff/reinicio necesita confirmacion separada y tiempo para guardar trabajo.
10. Despues del reinicio verifica evidencia saneada y cierra la campana.

No se usa ping como detector de cambios. AD no envia una notificacion generica
al endpoint cuando cambia una GPO; la campana administrativa es la senal
determinista y auditable.

## Contratos previstos

La campana debera incluir solo datos tipados:

- `campaignId`;
- version de esquema y politica;
- alcance del dispositivo;
- accion allowlisted;
- fecha de inicio y expiracion;
- ventana de posposicion;
- requerimiento conocido de logoff/reboot;
- idempotency key y correlacion.

El backend no envia ejecutables, comandos, switches, rutas o scripts. El
adaptador local fija los parametros, timeout y codigos aceptados.

## Consumo y programacion

El agente permanece dormido entre consultas. El intervalo inicial propuesto es
una hora con jitter, backoff ante fallos y suspension cuando el equipo no tiene
red. No mantiene ping continuo ni una sesion VPN abierta solo para descubrir
campanas.

UEMS puede ser una fuente alternativa de la campana si ofrece correlacion,
vigencia, auditoria e idempotencia equivalentes. La eleccion final se realiza
despues del MVP y solo cuando se conozca la capacidad corporativa. El backend
existente no autoriza campanas, UEMS ni conectividad de dominio.

## Seguridad

- Prohibido almacenar usuario, password, OTP, token interactivo o secreto VPN.
- Prohibido automatizar la UI de FortiClient o Axis mediante clicks.
- Preferir certificado de maquina, device tunnel, pre-logon o API/CLI aprobada.
- La cuenta del servicio no permite inicio interactivo.
- El consentimiento para refrescar no autoriza automaticamente el reinicio.
- El usuario puede posponer dentro de una ventana definida por politica.
- La evidencia no incluye nombres de dominio, servidores, IP ni salida completa
  de herramientas.
- El agente falla cerrado si no valida conectividad, vigencia o allowlist.
- El flujo debe tener kill switch y retiro mediante UEMS.

## Gates pendientes

### Gate A: FortiClient EMS

Se necesita confirmar:

- version de FortiClient y EMS del piloto;
- si existe perfil de maquina, pre-logon, auto-connect o equivalente;
- metodo de autenticacion sin credenciales humanas almacenadas;
- posibilidad de limitar el perfil al equipo/OU de prueba;
- rutas, split tunnel, DNS y servicios de dominio publicados;
- comportamiento tras suspension, cambio de red y reinicio;
- API/CLI soportada, si existe, y owner operativo;
- logs permitidos, rollback y procedimiento de retiro;
- aprobacion de Network, Endpoint Management y Security.

### Gate B: Axis Atmos

En el equipo fisico de prueba se necesita comprobar:

- version y estado administrado del agente Axis;
- resolucion de DNS corporativo;
- localizacion de controlador de dominio;
- alcance requerido a Kerberos/LDAP/SMB segun politica corporativa;
- acceso a `SYSVOL` y `NETLOGON`;
- aplicacion de una GPO inocua, reversible y aprobada;
- diferencia entre politica de equipo y usuario;
- resultado con logoff y reinicio;
- recuperacion cuando Axis no esta disponible;
- aprobacion de Network, Active Directory y Security.

## Evidencia que debe entregar el usuario

No incluir secretos, nombres reales de dominio, IP internas ni capturas con
identidades. Entregar una version saneada de:

1. respuesta de los owners sobre FortiClient EMS y mecanismo disponible;
2. versiones de FortiClient, EMS y Axis usadas en la prueba;
3. matriz simple de conectividad: DNS, DC, `SYSVOL`, `NETLOGON`;
4. resultado antes/despues de una GPO de laboratorio;
5. si requirio conexion manual, logoff o reboot;
6. tiempos aproximados y comportamiento ante posposicion;
7. owner que autoriza la prueba y restricciones indicadas;
8. procedimiento de rollback o retiro del perfil de prueba.

Con esa evidencia se actualizan los supuestos `W-006` y `W-007`, se cierran los
stoppers correspondientes y se decide si la implementacion usa FortiClient,
Axis Atmos o conserva el escalamiento del MVP.

## Prueba fisica segura

- Usar laptop desasignada autorizada y una OU/grupo de laboratorio.
- Confirmar backup o mecanismo de recuperacion antes de aplicar GPO.
- Empezar con comprobaciones de solo lectura.
- Usar una GPO inocua y facilmente reversible.
- No probar primero sobre un endpoint productivo ni la PC principal.
- Registrar solo evidencia saneada y retirar perfiles al terminar.

## Referencias

- Microsoft `gpupdate`:
  https://learn.microsoft.com/windows-server/administration/windows-commands/gpupdate
- Microsoft Always On VPN:
  https://learn.microsoft.com/windows-server/remote/remote-access/overview-always-on-vpn
- FortiClient Administration Guide:
  https://docs.fortinet.com/product/forticlient
