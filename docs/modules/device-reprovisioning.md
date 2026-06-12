# Borrado y reprovisionamiento de dispositivos

## Estado

Capacidad final del roadmap posterior al MVP. No forma parte de los Bloques
5-11 y no debe implementarse antes de validar diagnostico, reparaciones,
mantenimiento, enrolamiento, retiro y seguridad.

## Principio

El agente no sobrevive fisicamente al formateo. La continuidad funcional depende
de una autoridad externa que vuelva a instalar un bootstrap firmado:

- ISO/UEMS corporativa;
- Autopilot/Intune;
- provisioning package;
- otra herramienta RMM aprobada;
- portal autenticado y descarga manual del bootstrap.

No se implementa persistencia propia en UEFI, firmware o kernel. Un driver tipo
anticheat tampoco sobreviviria a un borrado completo y agregaria una superficie
de ataque injustificada.

## Proveedores

El dominio depende de interfaces, no de una empresa:

- `IEnrollmentProvider`;
- `IOperatingSystemDeploymentProvider`;
- `IDeviceIdentityProvider`;
- `ISecurityAgentProvider`;
- `INetworkAccessProvider`;
- `IDomainJoinProvider`;
- `ISoftwareProvisioningProvider`.

## Rama empresarial

1. El backend conserva hostname, asset tag, perfil, aplicaciones elegidas y
   correlacion.
2. El usuario confirma que completo su respaldo empresarial en OneDrive. El
   producto no realiza ni verifica copias de archivos.
3. El mecanismo corporativo inicia borrado e instalacion de Windows.
4. La ISO/UEMS instala Windows, UEMS, Sophos y bootstrap firmado.
5. El bootstrap valida dispositivo y canjea un token de un solo uso.
6. El agente verifica seguridad, actualizaciones y conectividad.
7. El proveedor corporativo restaura hostname y realiza enrolamiento de dominio.
8. Las configuraciones que requieren VPN siguen el flujo aprobado.
9. Tras iniciar sesion, se preparan las aplicaciones seleccionadas.
10. El agente verifica resultados y elimina el token.

Siempre que sea posible, la preparacion tecnica corre como servicio durante el
despliegue. No usa autologon con credenciales humanas.

## Rama independiente

1. El usuario instala Windows 11 limpio.
2. Accede al portal y se autentica.
3. Selecciona el dispositivo/perfil autorizado.
4. Descarga un bootstrap firmado y personalizado mediante token de un solo uso.
5. El bootstrap valida usuario, dispositivo, expiracion y revocacion.
6. Descarga el perfil y las politicas; el instalador no los lleva embebidos.
7. El agente instala aplicaciones y aplica configuracion permitida.
8. El portal muestra progreso, evidencia y acciones que requieren al usuario.

Sin Autopilot, MDM, imagen personalizada o instalacion manual del bootstrap, una
instalacion publica limpia de Windows no puede recuperar el agente por si sola.

## Estado conservado

Solo datos tecnicos y permitidos:

- identidad del registro del dispositivo;
- hostname;
- asset tag;
- perfil versionado;
- aplicaciones seleccionadas;
- correlacion y estado del proceso.

No se conservan archivos personales, credenciales, PIN de BitLocker, recovery
password, secretos VPN ni tokens reutilizables.

## BitLocker

Asset tag, PIN de inicio y recovery password son valores distintos. Aunque una
organizacion use una convencion derivable del tag, el PIN sigue siendo una
credencial y este producto no la calcula ni la muestra.

Para borrado y enrolamiento:

- el mecanismo de despliegue aprobado administra particiones/cifrado;
- si BitLocker requiere accion humana, se crea o escala un ticket;
- la recovery password sigue el canal telefonico/humano aprobado;
- el agente registra solo que se requirio intervencion, no la clave.

## Confirmaciones y seguridad

Antes de borrar:

- autenticar y autorizar usuario, dispositivo y politica;
- mostrar impacto irreversible;
- exigir confirmacion de respaldo completado;
- comprobar energia y conectividad;
- verificar que existe un mecanismo de reinstalacion y retiro;
- usar doble confirmacion y ventana de cancelacion;
- emitir kill switch y correlacion auditable.

Despues de borrar:

- token corto, de un solo uso y ligado al dispositivo;
- identidad nueva o rotada;
- comprobacion de agentes de seguridad;
- recuperacion idempotente;
- revocacion del registro anterior.

## Desenrolamiento

Desde el portal administrativo:

1. revocar certificados, tokens y campanas;
2. impedir nuevos trabajos;
3. retirar configuracion y agente por proveedor aprobado;
4. conservar auditoria segun retencion;
5. marcar el dispositivo como retirado;
6. permitir posteriormente una instalacion limpia.

Desenrolar no borra remotamente el equipo por defecto. Un borrado remoto exige
una capacidad separada, autorizacion reforzada y pruebas de equipo objetivo.

## Gates pendientes

- mecanismo actual para iniciar la ISO corporativa sin USB;
- capacidad de UEMS/ISO para incluir bootstrap firmado;
- tipo de union AD, hybrid Entra o Entra;
- procedimiento de hostname, dominio, VPN y seguridad;
- provider de borrado/reinstalacion y rollback;
- identidad de hardware aceptada;
- portal y bootstrap independiente;
- retiro completo del registro externo;
- aprobacion de Security, Endpoint Management y Legal/licenciamiento.

## Referencias

- Windows Autopilot para dispositivos existentes:
  https://learn.microsoft.com/autopilot/existing-devices
- Windows Autopilot Reset:
  https://learn.microsoft.com/autopilot/windows-autopilot-reset
- Paquetes de aprovisionamiento:
  https://learn.microsoft.com/windows/configuration/provisioning-packages/provisioning-packages
