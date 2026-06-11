# Auditoria de consistencia documental

Fecha: 2026-06-10.

## Cobertura

- 25 Markdown fuera de `executive` revisados.
- 25 Markdown conservados dentro de la estructura clasificada.
- 6 documentos nuevos de arranque, coordinacion y auditoria.
- Ningun archivo de `executive` fue modificado o copiado como dependencia.

## Inconsistencias resueltas

### Orden de desarrollo

El delivery anterior proponia portal administrativo antes del shell WinUI. La
secuencia vigente inicia con aplicacion nativa, catalogo, agente y API; el portal
queda como ultimo bloque funcional.

### Bot de Teams

No se construye otro prototipo independiente. Primero se valida cliente/agente y
API compartida. Despues se conecta el bot existente.

### Consultas y acciones

Una consulta informativa no crea solicitud, ticket ni instalacion. Toda accion
pasa por propuesta y confirmacion explicita.

### Integraciones pendientes

La falta de OpenText, UEMS, Entra, Azure, IA o PKI no bloquea el repositorio ni
los recorridos locales. Se usan interfaces, fakes y datos sinteticos hasta
cumplir el gate correspondiente.

### Contexto ejecutivo

Los mapas técnicos ya no dependen de archivos de `executive`. Esa carpeta
permanece separada para comunicacion con liderazgo.

## Diferencias intencionales

- Los originales se conservaron como blueprint y trazabilidad.
- `development` es la fuente de trabajo para la implementacion.
- `reference` puede contener respuestas antiguas o lenguaje operativo que no
  tiene precedencia sobre `core`, `standards` o `modules`.
- El portal sigue siendo parte del producto y del MVP, aunque se implemente al
  final del plan inicial.

## Incertidumbres externas, no contradicciones

- sede y endpoints del piloto;
- API y modelo operativo de OpenText;
- App Registration y detalles de identidad;
- hosting y presupuesto;
- proceso final UEMS;
- firma/PKI y Sophos;
- proveedor de IA;
- retencion de datos.

Estas incertidumbres deben resolverse en
`project-management/INFORMATION_REQUESTS.md`. No autorizan al desarrollo a
inventar APIs, permisos o credenciales.
