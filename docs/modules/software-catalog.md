# Dominio de catalogo de software

## Responsabilidad

`src/Catalog` es la frontera pura que modela productos de software y decide como
responder a una consulta o intencion de adquisicion. No ejecuta acciones y no
conoce WinUI, Prisma, IA, agentes, tickets, IPC ni servicios externos.

## Modelo

- `SoftwareProduct`: identidad, nombre, fabricante, categoria, aliases, version,
  licencia, estado y alternativas.
- `SoftwareVersion`: etiqueta de version normalizada.
- `SoftwareLicense`: tipo y regla de uso; `Commercial` requiere entitlement.
- `SoftwareProductStatus`: `Approved`, `Unlisted`, `EndOfLife` o `Prohibited`.
- `ProductAlternative`: producto alternativo por identificador y motivo.

Los constructores rechazan valores obligatorios vacios y las colecciones
publicas son de solo lectura.

## Casos de uso

`CatalogSearchService` aplica filtros por estado y licencia, normaliza
mayusculas y diacriticos, busca en campos permitidos y ordena siempre por nombre
e identificador.

`CatalogDecisionService` separa dos propositos:

- `Information`: devuelve `Inform` y no representa una accion.
- `Acquisition`: devuelve `Propose`, `Escalate` o `Reject` segun estado y
  licencia.

Una referencia desconocida se trata como no listada. Productos EOL o prohibidos
se rechazan y pueden devolver alternativas del mismo catalogo.

## Datos

`SyntheticCatalog` contiene siete fixtures ficticios para cubrir las reglas. No
provienen del Excel corporativo y no representan autorizaciones reales.

El importador, staging, persistencia y publicacion del Excel revision 134 siguen
fuera de este bloque.

## Adaptadores

`src/Desktop` registra los casos de uso mediante DI. `CatalogViewModel` consulta
el listado inicial y solo transforma los resultados a textos e iconos de
presentacion. No reimplementa decisiones de catalogo.

## Pruebas

`tests/Unit` cubre:

- validacion y normalizacion del modelo;
- licencias comerciales;
- busqueda por campos y aliases;
- busqueda sin distincion de mayusculas o diacriticos;
- filtros combinados y resultados vacios;
- cada rama de decision;
- referencias desconocidas;
- alternativas para EOL y prohibido.

`tests/Architecture` verifica que el assembly de catalogo no referencia
Desktop, DeviceAgent ni WinUI.
