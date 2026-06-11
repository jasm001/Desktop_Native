# Licencias y presupuesto de desarrollo

Fecha de referencia: 2026-06-07. Los precios externos cambian y deben
recotizarse antes de compra.

## Tres centros de costo

| Centro | Incluye | No confundir con |
| --- | --- | --- |
| Herramientas de desarrollo | Visual Studio, GitHub, IDE y seguridad | Inferencia del bot |
| Asistencia IA al desarrollo | Copilot, Claude, Codex o Hermes/OpenRouter | API productiva |
| IA del producto | Clasificacion y asistencia a usuarios finales | Vibecoding |

Cada centro usa cuenta, owner, limite y evidencia propios.

## Licencias base a solicitar

- Visual Studio Professional para C#/WinUI, perfiles de empaquetado y
  diagnostico Windows.
- VS Code para Next.js, documentacion y herramientas auxiliares.
- repositorio privado GitHub Enterprise proporcionado por la empresa;
- GitHub Actions o runner Windows corporativo;
- Semgrep CE, Gitleaks, Trivy y ZAP sin add-on inicial;
- GitHub Code Security solo si ya esta incluido o se aprueba su costo.

La cotizacion corporativa de GitHub/Visual Studio indicada vence en junio de
2026. Debe solicitarse una cotizacion nueva antes de compararla.

## Claude Enterprise

La informacion interna reporta:

- asiento anual de USD 239.36 con vigencia hasta abril de 2027;
- limite mensual inicial de USD 50;
- ampliacion mediante ticket.

No es absurdo que exista asiento mas consumo: el asiento puede cubrir gobierno,
identidad, administracion y acceso, mientras el consumo limita inferencia. Sin
embargo, no resulta automaticamente rentable para desarrollo intensivo. Antes
de elegirlo se necesita confirmar:

- si USD 239.36 es total del periodo o precio anual prorrateado;
- que funciones/modelos incluye;
- como se mide el limite de USD 50;
- proteccion de datos y uso de codigo para entrenamiento;
- auditoria, retencion y posibilidad de API;
- costo real de excedentes.

Se mantiene como herramienta corporativa complementaria, no como unico motor.

## GitHub Copilot

La cotizacion interna de USD 19 debe recotizarse. Desde el 1 de junio de 2026,
GitHub esta migrando Copilot a cobro basado en uso con creditos de IA. El plan
base y el consumo agentico ya no deben interpretarse como uso ilimitado.

Copilot aporta integracion directa en Visual Studio/VS Code, sugerencias,
revision y controles empresariales. Puede ser util aun con Hermes, pero no
reemplaza presupuesto de tareas autonomas largas.

Decision propuesta:

- aceptar Copilot si viene incluido en el paquete corporativo y su costo
  incremental es razonable;
- no depender de su asignacion incluida para todo el proyecto;
- medir durante un mes antes de comprar consumo adicional.

## Piloto Hermes/OpenRouter

Solicitar USD 50 mensuales durante dos meses, no seis meses inicialmente.
Configurar claves separadas:

| Clave | Limite mensual |
| --- | ---: |
| Builder | USD 25 |
| Reviewer | USD 8 |
| Research/documentacion | USD 5 |
| Vision/QA | USD 7 |
| Reserva para stopper | USD 5 |

OpenRouter permite limites por clave y consulta de consumo. Desactivar auto
top-up o limitarlo al presupuesto aprobado.

### Referencia con el consumo observado

La captura compartida muestra aproximadamente:

- 36.8 M tokens de entrada;
- 0.661 M de salida;
- 108.4 M de lectura de cache;
- 74.7 % de cache hit.

Aplicando precios publicados el 2026-06-07, como aproximacion y antes de cargos,
promociones o diferencias de proveedor:

| Modelo | Costo aproximado de ese patron |
| --- | ---: |
| Step 3.7 Flash | USD 12.46 |
| DeepSeek V4 Flash | desde USD 3.81 mas cache |
| DeepSeek V4 Pro | cerca de USD 16.98 con cache directo publicado |
| MiniMax M3 promocional | cerca de USD 18.34 |
| Qwen3.7 Plus | desde USD 15.78 mas cache |
| Kimi K2.6 con proveedor ZDR fijado | cerca de USD 54.95 |

Estas cifras no predicen el costo multiagente: cada delegacion puede volver a
enviar contexto. Explican por que V4 Flash/Step sirven para volumen, V4 Pro para
trabajo dificil y Kimi solo para escalaciones visuales.

## Escenarios

### Recomendado para viabilidad

- licencias corporativas Visual Studio/GitHub;
- USD 50/mes OpenRouter por dos meses;
- herramientas de seguridad open source;
- Codex/Claude/Copilot solo si estan formalmente aprobados;
- renovacion despues de medir costo por tarea aceptada.

Costo OpenRouter autorizado inicial: USD 100 mas la comision vigente de compra
de creditos.

### Presupuesto de USD 200 para todo el proyecto

Reservar:

- USD 100 para los primeros dos meses;
- USD 50 para integracion/QA;
- USD 50 para contingencias o evaluacion del modelo del producto.

No distribuir USD 200 desde el primer dia ni ejecutar agentes sin limites.

### Horizonte hasta diciembre de 2026

De julio a diciembre hay seis meses completos: USD 50 mensuales equivalen a USD
300, no USD 270. Junio parcial debe presupuestarse aparte si se consume.

## Restricciones

- no pagar personalmente servicios que procesen codigo corporativo sin permiso;
- no compartir una API key entre desarrollo y producto;
- no usar modelos gratuitos como dependencia: promociones y rate limits cambian;
- no enviar datos reales para aprovechar una promocion;
- no presentar cache hit como ahorro hasta verificar que el proveedor factura
  cache a una tarifa menor;
- revisar consumo semanal y cortar automaticamente al alcanzar 80 %.

## Informacion necesaria para compra

Solicitar una tabla vigente con:

- Visual Studio Professional/MSDN y fecha de vencimiento;
- GitHub Enterprise requerido o ya disponible;
- Copilot plan, creditos incluidos y costo por excedente;
- condiciones de Claude Enterprise y limite mensual;
- permiso para Hermes, OpenRouter y proveedores seleccionados;
- tarjeta/cuenta corporativa, owner y centro de costo;
- tratamiento de codigo, retencion, regiones y proveedores permitidos.
