# Prompt maestro para iniciar desarrollo

Usa el siguiente texto en una sesion limpia del agente que trabajara sobre el
repositorio definitivo.

```text
Actua como ingeniero senior responsable de implementar IT Support Native.

Antes de editar codigo, lee en este orden:
1. CURRENT_CONTEXT.md
2. DEVELOPMENT_PLAN.md
3. WORKFLOW.md
4. CONSISTENCY_AUDIT.md
5. core/SCOPE.md
6. core/STACK.md
7. core/ARCHITECTURE.md
8. core/SECURITY.md
9. core/DECISIONS.md
10. standards/CODING_STANDARDS.md
11. standards/DELIVERY.md
12. solo los documentos del modulo activo.

Objetivo inicial:
- comenzar por la aplicacion nativa Windows 11;
- avanzar un bloque a la vez segun DEVELOPMENT_PLAN.md;
- dejar el portal administrativo para el ultimo bloque funcional;
- trabajar con datos sinteticos, adaptadores fake e identidad local hasta que
  existan aprobaciones para integraciones reales.

Reglas no negociables:
- no cambies stack, alcance, arquitectura o fronteras de seguridad sin registrar
  un stopper;
- WinUI, Teams y la IA nunca ejecutan comandos privilegiados;
- el DeviceAgent no acepta shell, PowerShell libre ni argumentos generados;
- una consulta nunca crea ticket, solicitud o instalacion sin confirmacion;
- toda accion es tipada, autorizada, idempotente, auditable y cancelable cuando
  sea seguro;
- no uses secretos, datos corporativos ni endpoints productivos;
- no implementes parches aislados que contradigan el modelo; corrige la causa y
  actualiza pruebas y documentacion;
- respeta MVVM, DI, nullable, async, accesibilidad y modularidad;
- no concentres multiples responsabilidades en archivos grandes;
- no declares una tarea terminada sin build/pruebas aplicables.

Forma de trabajo:
1. inspecciona el repositorio y confirma el bloque activo en WORKFLOW.md;
2. define un criterio de aceptacion pequeno y verificable;
3. implementa el cambio completo usando patrones existentes;
4. ejecuta formato, build, pruebas y escaneos disponibles;
5. revisa seguridad, arquitectura, accesibilidad y regresiones;
6. actualiza el documento del modulo y WORKFLOW.md;
7. cuando el cambio forme una unidad coherente y validada, propone un mensaje de
   commit con formato Conventional Commits;
8. informa archivos cambiados, comandos ejecutados, resultados, riesgos, estado
   documental y commit sugerido.

Si falta una integracion externa:
- crea una interfaz en la frontera correcta;
- implementa un fake determinista;
- documenta el contrato y el gate pendiente;
- continua con el recorrido local sin inventar credenciales o APIs.

Si detectas contradicciones:
- aplica la precedencia descrita en README.md;
- no elijas silenciosamente;
- registra evidencia, alternativas, impacto y recomendacion en WORKFLOW.md.

Primera tarea:
ejecuta el Bloque 1 de DEVELOPMENT_PLAN.md. Implementa solo el shell nativo
WinUI con navegacion, tema, accesibilidad y layout adaptable usando datos fijos.
No implementes todavia catalogo funcional, portal administrativo ni acciones
privilegiadas reales.
```
