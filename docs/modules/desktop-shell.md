# Shell nativo WinUI

## Responsabilidad

`src/Desktop` contiene el cliente Windows 11 sin privilegios. Presenta la shell
navegable y consume los dominios sinteticos de catalogo y conversacion como
adaptador de presentacion. No ejecuta comandos, no crea solicitudes reales y no
se conecta al agente, backend o integraciones corporativas.

## Experiencia

La shell usa `NavigationView`, Mica con fallback nativo y recursos de tema.
Expone cinco destinos:

- Inicio;
- Catalogo;
- Asistente;
- Solicitudes;
- Salud del equipo.

El diseno toma como referencia la jerarquia visual del mockup local: encabezado,
panel lateral, tarjetas, estado del entorno y densidad de informacion. No replica
el escritorio, taskbar o chrome simulado del prototipo web.

Las vistas muestran fixtures pequenos y deterministas. Los controles que podrian
implicar una accion futura estan deshabilitados o se presentan como consultas de
demostracion.

## Arquitectura

- `App` crea el contenedor de `Microsoft.Extensions.DependencyInjection`.
- `App` registra los fixtures y casos de uso puros de `src/Catalog`.
- `App` registra la maquina de estados pura de `src/Conversation`.
- `MainWindow` posee solo navegacion, tema y comportamiento adaptable de shell.
- Los ViewModels exponen contenido de presentacion inmutable.
- `CatalogViewModel` transforma resultados del dominio a tarjetas; no decide
  autorizacion, escalamiento o rechazo.
- `AssistantViewModel` traduce estados y codigos a texto y comandos MVVM; no
  decide transiciones ni reglas de catalogo.
- `ShellRouteCatalog` mantiene las rutas permitidas del bloque.
- Los recursos visuales viven en `Themes`.
- Cada destino mantiene su XAML y code-behind de enlace por separado.

El ejecutable de desarrollo es x64, autocontenido para .NET y Windows App SDK, y
se ejecuta con `asInvoker`.

La edicion y depuracion interactiva requieren Visual Studio 2026 version 18.0 o
posterior. Visual Studio 2022 no admite el target `net10.0`; la presencia del SDK
.NET 10 global no cambia esa limitacion del IDE.

## Accesibilidad

- destinos y controles principales tienen nombres de automatizacion;
- la navegacion funciona con foco, flechas y Enter;
- el tema claro y oscuro conserva contraste mediante recursos de tema;
- `NavigationView` cambia a modo compacto;
- el contenido usa scroll, wrapping y grids adaptables;
- el manifiesto declara DPI `PerMonitorV2`.

La validacion del bloque cubre 1280x820 y 760x700 como viewports efectivo amplio
y compacto. El escalado corporativo a 200 % debe repetirse en el gate de piloto
con la matriz real de monitores.

## Seguridad

- no hay elevacion ni `runas`;
- no existen shell, PowerShell, procesos hijos o argumentos dinamicos;
- no se almacenan secretos, credenciales o datos corporativos;
- las referencias de solicitud son sinteticas; instalaciones, tickets,
  diagnosticos reales e IA quedan fuera de este bloque;
- el contenido identifica explicitamente el modo local y sintetico.

## Pruebas

- pruebas de rutas en `tests/WindowsUi`;
- build Release sin warnings;
- smoke test del ejecutable autocontenido;
- UI Automation para las cinco rutas, tema y teclado;
- revision visual en tema claro, oscuro y layout compacto.

## Siguiente bloque

El Bloque 4 agregara un agente simulado e IPC tipado. La shell seguira sin
ejecutar acciones privilegiadas.
