# MVP local de laboratorio

## Objetivo

Construir una demostracion funcional de extremo a extremo mientras se resuelve
la aprobacion corporativa. El entorno es una VM Windows 11 personal, sin datos,
credenciales, paquetes ni endpoints corporativos.

El laboratorio demuestra arquitectura y comportamiento. No acredita despliegue,
seguridad, licenciamiento ni operacion corporativa.

## Estado actual

Los incrementos de adaptador y control plane del laboratorio estan completados:

- `seven-zip.msi.v1` para 7-Zip 26.01 x64;
- mirror filesystem temporal con manifiesto y SHA-256;
- instalacion, verificacion, idempotencia y desinstalacion en VM Windows 11;
- fallos cerrados por mirror ausente y hash corrupto;
- restauracion del checkpoint y confirmacion del estado inicial.
- API Next.js, PostgreSQL y worker locales;
- recorrido WinUI -> API -> agente simulado -> evidencia validado.
- casos internos, evento de escalamiento y ticketing fake idempotente.

El Windows Service instalado, la conexion IPC de Salud desde WinUI, Hermes/RAG y
el portal siguen siendo componentes previstos. No forman parte de los gates
cerrados de los Bloques 6, 7 y 8.

## Perfil `local-demo`

Componentes previstos:

- WinUI sin privilegios;
- DeviceAgent instalado como Windows Service restringido en la VM;
- IPC con ACL explicita entre el usuario de WinUI y la identidad del servicio;
- diagnosticos reales mostrados en Salud del equipo;
- API, PostgreSQL y worker locales;
- portal e identidad de desarrollo;
- ticketing, Teams, UEMS y demas integraciones mediante fakes;
- mirror local de artefactos;
- Hermes local con API externa opcional;
- documentos e indice RAG locales;
- modo degradado sin conexion.

La configuracion `local-demo` debe ser identificable y fallar al intentar usarse
como perfil `pilot` o `production`.

## Mirror local

El mirror simula un servidor de artefactos mediante HTTP local o filesystem
controlado. Los instaladores no se versionan en Git.

Solo se usan paquetes:

- libres y de bajo riesgo;
- con licencia que permita redistribucion;
- sin credenciales, drivers ni dependencia corporativa;
- con version y arquitectura fijadas;
- con modo unattended documentado;
- con SHA-256 obligatorio;
- probados en snapshot de VM.

El manifiesto versionado relaciona `artifactId`, producto, version, origen,
licencia, hash, firma disponible y adaptador compatible.

El reempaquetado se prueba solo cuando la licencia permita modificacion y
redistribucion. Un MSIX o wrapper de laboratorio no se considera aprobado para
usuarios reales.

## Hermes y RAG

Hermes se ejecuta localmente como proveedor del chat. La inferencia puede usar
una API externa de prueba, por lo que requiere Internet y no se considera IA
offline.

Permanecen locales:

- documentos Markdown curados;
- chunks y metadatos;
- indice de recuperacion;
- catalogo sintetico;
- fuentes y versiones;
- reglas e intenciones deterministas.

No se indexan ni envian tickets, correos, hostnames, diagnosticos reales,
credenciales, logs completos o documentos corporativos. La clave API vive fuera
del repositorio.

Si la API no esta disponible, el producto usa busqueda local, respuestas
curadas, catalogo y la maquina de estados determinista.

## Operacion sin conexion

El DeviceAgent puede continuar sin red cuando la accion y sus dependencias ya
estan disponibles localmente.

Permitido:

- diagnosticos locales;
- consulta de catalogo y conocimiento cacheados;
- trabajos previamente autorizados con artefacto validado;
- acciones de la politica `local-demo` confirmadas localmente dentro de la VM;
- persistencia de progreso y evidencia;
- sincronizacion posterior idempotente.

No permitido:

- descargar artefactos ausentes;
- crear autorizaciones nuevas fuera de la politica instalada;
- ampliar la allowlist;
- ejecutar texto, scripts o argumentos producidos por Hermes;
- ocultar que el backend, mirror o proveedor de IA no estan disponibles.

## Recorrido demostrable

1. Iniciar WinUI y el Windows Service.
2. Mostrar diagnosticos reales de la VM.
3. Consultar catalogo y conocimiento local.
4. Usar Hermes cuando exista conexion y fallback determinista cuando no.
5. Proponer una accion conocida y solicitar confirmacion.
6. Resolver un artefacto del mirror local y verificar SHA-256.
7. Ejecutar `Detect`, `Preflight`, `Install` y `Verify`.
8. Repetir la instalacion para validar idempotencia.
9. Ejecutar `Uninstall` y verificar ausencia.
10. Conservar evidencia local y simular sincronizacion al recuperar conexion.

## Limite de avance

Los Bloques 6, 7 y 8 ya cerraron sus gates locales. El Bloque 9 es el siguiente y
los Bloques 9-11 pueden preparar sus dominios, UI, persistencia y adaptadores
locales/fake. Ninguno puede declararse corporativamente validado mientras
falten:

- UEMS y proceso de distribucion/retiro;
- cuenta e identidad restringida del servicio;
- Security/Sophos y threat model aprobado;
- Entra y tenant corporativo;
- OpenText de prueba;
- bot de Teams existente;
- PKI/firma y confianza de publicador;
- hosting, retencion y conectividad aprobados.

La decision corporativa cambia proveedores y gates de despliegue, no los
contratos centrales ni las fronteras de seguridad.
