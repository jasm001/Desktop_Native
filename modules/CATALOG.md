# Gobierno del catalogo

## Fuente inicial

Archivo:

`Open Source and Freeware Exceptions.xlsx`

La copia revisada contiene:

- hoja de lineamientos y clasificacion `INTERNAL`;
- `Coreload` con software base;
- `OSS & Freeware (Everyone)` con 211 filas y varias tablas paralelas;
- software publicado por Softtek;
- software prohibido;
- 79 entradas en tipos de licencia.

Se detectan aproximadamente 224 nombres candidatos en la hoja principal. Esa
cifra no significa 224 paquetes instalables: hay encabezados internos,
aplicaciones de otras plataformas, versiones antiguas y entradas propietarias.

## Implementacion local del Bloque 2

`src/Catalog` contiene la primera frontera ejecutable del dominio, sin
dependencias de WinUI, Prisma, IA, persistencia o integraciones. Incluye:

- producto, version, licencia, estado, aliases y alternativas;
- estados `Approved`, `Unlisted`, `EndOfLife` y `Prohibited`;
- licencia `Commercial` como condicion independiente de autorizacion;
- busqueda determinista por nombre, alias, fabricante, categoria y version;
- filtros combinables por estado y tipo de licencia;
- decisiones tipadas `Inform`, `Propose`, `Escalate` y `Reject`;
- fixtures pequenos y completamente sinteticos.

Las reglas vigentes del incremento son:

| Consulta | Resultado |
| --- | --- |
| Informacion sobre cualquier referencia | `Inform`; nunca crea una accion |
| Adquisicion de producto aprobado no comercial | `Propose` |
| Adquisicion de producto comercial | `Escalate` |
| Adquisicion de producto no listado o desconocido | `Escalate` |
| Adquisicion de producto EOL | `Reject` |
| Adquisicion de producto prohibido | `Reject` |

Una propuesta sigue siendo solo una decision de catalogo. No crea solicitud,
ticket, trabajo ni instalacion. El Bloque 7 implemento la solicitud y el trabajo
sinteticos confirmados; el siguiente Bloque 8 modelara `BotCase`, escalamiento y
ticketing fake sin cambiar estas reglas de catalogo.

## Regla

El Excel prueba que un producto puede estar permitido, prohibido o sujeto a
aprobacion. No prueba que exista un instalador seguro, silencioso y compatible.
Por eso no se importa directamente como allowlist del agente.

## Modelo normalizado

Cada producto requiere:

- nombre canonico, aliases y fabricante;
- categoria del Excel y referencia de fila/origen;
- uso y tipo de licencia;
- sistemas y arquitecturas compatibles;
- version minima, recomendada y versiones bloqueadas;
- estado EOL/EOS y fecha de ultima revision;
- regla por pais, sede, proyecto o cliente;
- requiere licencia, compra o aprobacion de seguridad;
- fuente oficial y repositorio interno;
- tipo de paquete: MSIX, MSI, EXE, ZIP u otro;
- hash SHA-256, firma/editor y tamano;
- comandos cerrados de install/uninstall/detect/repair;
- codigos de salida, reinicio y timeout;
- diagnostico previo y verificacion posterior;
- owner, reviewer, aprobaciones y fechas;
- estado: draft, review, approved, suspended, retired, prohibited.

En MVP, hash/firma de terceros puede quedar sin dato si UEMS descarga
directamente del fabricante y no lo expone. Se registra URL oficial, version,
template/package ID y fecha. La verificacion estricta por hash se vuelve
obligatoria al introducir un mirror propio.

## Pipeline de importacion

1. Cargar una revision del Excel y conservar hash/evidencia.
2. Extraer entradas a staging.
3. Resolver duplicados, aliases y secciones paralelas.
4. Separar Windows de macOS, herramientas web y componentes no instalables.
5. Revisar licencia, version, compatibilidad Windows 11 y EOL.
6. Asociar un artefacto interno y adaptador probado.
7. Ejecutar analisis antivirus y pruebas en VM/Sandbox.
8. Aprobar por segregacion de funciones.
9. Publicar una version firmada del catalogo.

Cambiar el Excel no publica software automaticamente.

## Repositorio de artefactos

Orden preferente:

1. Template oficial UEMS que descarga desde el fabricante y crea el paquete en
   Software Repository HTTP/WAN.
2. Paquete manual UEMS descargado desde el fabricante y validado por el proyecto.
3. Azure Blob privado o repositorio nuevo solo para excepciones que UEMS no cubra.
4. File share interno solo para piloto y con ACL, hash y backups.

Sus paquetes historicos locales se consideran no confiables hasta revalidar
version, EOL, origen, switches, hash y deteccion.

### Origen local de laboratorio

El perfil `local-demo` puede usar `LocalDevelopmentArtifactSource` para simular
un mirror sin solicitar infraestructura corporativa. Puede ser un servidor HTTP
local o una carpeta controlada montada en la VM.

Solo admite software libre cuando la licencia permita redistribucion y, si
aplica, modificacion o reempaquetado. Cada artefacto declara:

- producto, version y arquitectura;
- URL oficial y fecha de obtencion;
- licencia y evidencia de redistribucion permitida;
- nombre logico, tamano y SHA-256;
- firma/editor cuando exista;
- adaptador y version compatibles;
- estado `development-only`.

Los binarios no se guardan en Git. El manifiesto puede versionarse, pero el
almacen local permanece ignorado y reemplazable. Corrupcion, hash incorrecto,
version no esperada o ausencia del mirror fallan cerrado.

Este proveedor no altera el orden productivo anterior ni convierte un paquete de
laboratorio en software aprobado para endpoints corporativos.

## Interseccion de catalogos

Un template UEMS no significa que Softtek autorice el programa. Una entrada del
Excel tampoco significa que UEMS tenga un template correcto.

Solo se publica la interseccion:

```text
Softtek approved
AND Windows 11 compatible
AND supported/LTS/Current
AND UEMS template or reviewed manual package
AND silent install verified
AND security checks passed
```

El portal mantiene el mapeo entre `SoftwareProduct` aprobado y `UemsTemplateId` /
`UemsPackageId`. No muestra el catalogo completo de ManageEngine al usuario.

## Actualizacion con UEMS

- Sincronizar templates del proveedor.
- Habilitar auto-update solo en productos permitidos.
- Crear nueva version de paquete, no desplegarla automaticamente.
- Ejecutar validacion EOL, licencia, hash/editor y smoke test.
- Promover a `approved` y despues a anillos internal/pilot.
- Conservar rollback a la ultima version aprobada cuando sea viable.

No habilitar auto-update global para todos los templates: podria publicar software
no permitido o una version aun no validada.

No depender de Microsoft Store: puede estar deshabilitada. WinGet solo se usa si
la fuente y el paquete quedan fijados, validados y permitidos.

## Licencias y software no listado

- Comercial: crear ticket y aprobacion de compras/licencias.
- Proporcionado por cliente: registrar aprobacion formal y periodo.
- No listado: solicitar a seguridad business case, periodo, aprobaciones y URL.
- EOL: rechazar por defecto y solicitar excepcion a Security si existe una
  necesidad obligatoria.
- Prohibido: ofrecer alternativas aprobadas. Si no cubren el caso y el uso es
  obligatorio, solicitar excepcion a Security.
- VPN: aplicar la excepcion operativa vigente de Mesa de Ayuda, usando fuente
  oficial y documentando la solicitud.

El agente instala solo despues de recibir autorizacion y paquete tecnico, no solo
por aparecer el nombre en el Excel.

## Automatizacion real

`Aprobado` significa permitido por politica. `Automatizable` exige ademas:

- instalador silencioso o mecanismo oficial no interactivo;
- origen reproducible y permitido;
- version compatible y no EOL;
- origen oficial verificable; hash/firma cuando este disponible;
- parametros, deteccion y rollback probados;
- ausencia de decisiones manuales durante el setup.

SQL Server Express, SSMS u otros instaladores con opciones complejas deben
empaquetarse con configuracion declarativa, response file o switches oficiales.
No se automatizan clics de instaladores de escritorio con Playwright. Si no hay
mecanismo soportado, se escala o queda manual.

Patrones admitidos:

- argumentos silenciosos documentados por fabricante;
- archivos de respuesta/configuracion;
- propiedades MSI y transforms MST;
- layouts offline versionados;
- WinGet Configuration/DSC revisado;
- adaptador C# o script firmado con entradas cerradas;
- post-configuracion declarativa e idempotente.

Ejemplos:

- SQL Server: `/CONFIGURATIONFILE`, `/Q` o `/QS` y parametros oficiales.
- SSMS: `--quiet`/`--passive`, `--config`, layout y `--norestart`.
- Git for Windows: flags oficiales de instalacion silenciosa.

WinGet solo se usa con source/package ID fijado y politica corporativa validada.

El agente no busca instaladores libremente en la web durante una solicitud. Que
Microsoft Store este bloqueada visualmente no garantiza que `winget` o `msstore`
esten autorizados por politica.

## Vigencia y alternativas

- La revision inicial es la 134 del Excel.
- Para runtimes se publican solo ramas LTS o Current soportadas.
- EOL se determina mediante fuentes oficiales/versionadas del fabricante, no por
  una respuesta libre del modelo en tiempo de ejecucion.
- El catalogo guarda alternativas aprobadas, por ejemplo Postman hacia
  Insomnia/SoapUI o FileZilla hacia WinSCP cuando corresponda.
- Si no existe alternativa, el bot recopila contexto y escala; no abandona al
  usuario ni propone software fuera del catalogo.
- La lista interna por pais/cliente/proyecto debe incorporarse como fuente de
  solo lectura versionada. Hasta disponer de ella, cualquier ausencia se escala.
- Los mensajes para no listado, EOL y prohibido usan
  `../reference/templates/NO_LISTED.md`.
