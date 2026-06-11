# Entrega

## Repositorio definitivo

```text
IT-Support-Native/
  src/
    Desktop/                 # WinUI 3
    AdminWeb/                # Next.js + Fluent UI + Prisma
    DeviceAgent/             # Windows Service
    Worker/                  # Node.js durable worker
    Contracts/
    BuildingBlocks/
  tests/
    Unit/
    Integration/
    Architecture/
    Contract/
    WindowsUi/
    AdminWeb/
  deploy/
    bicep/
    intune/
    endpoint-central/
    msix/
  docs/
    adr/
    threat-model/
    runbooks/
  package.json
  pnpm-workspace.yaml
  Directory.Build.props
  Directory.Packages.props
  global.json
  ITSupportNative.slnx
```

Una solucion y un repositorio son suficientes al inicio. Las fronteras se
aplican con proyectos, pruebas de arquitectura y ownership.

Repositorio remoto inicial:

`https://github.com/jasm001/Desktop_Native.git`

Antes del primer push se revisan ramas e historial remoto. No se usa force push
ni se reemplaza contenido existente sin una decision explicita. La rama principal
es `main`.

## Calidad desde el primer commit

- README inicial con objetivo, requisitos, estructura y comandos de validacion.
- `.gitignore` para Visual Studio, .NET, WinUI, Node.js, Next.js, Prisma,
  secretos, configuracion local y artefactos.
- Compilacion con warnings como errores.
- Formato y analizadores .NET.
- Dependencias centralizadas y fijadas.
- Unit tests para reglas.
- Integration tests con PostgreSQL real efimero.
- Contract tests para agente e integraciones.
- Architecture tests para evitar dependencias prohibidas.
- UI smoke tests para los recorridos criticos.
- Escaneo de secretos, dependencias, SBOM y firma.
- Nullable Reference Types, analyzers y reglas de arquitectura obligatorios.
- Revisiones de codigo y ownership para seguridad, agente y catalogo.
- CodeQL/Semgrep, Gitleaks, Trivy y ZAP segun `CODING_STANDARDS.md`.

## Toolchain de desarrollo Windows

- El repositorio fija .NET SDK `10.0.301` mediante `global.json`.
- El build por CLI usa el SDK instalado en `C:\Program Files\dotnet` o la copia
  portable ignorada bajo `.dotnet`.
- El desarrollo y debugging WinUI de `net10.0` requieren Visual Studio 2026
  version 18.0 o posterior.
- Visual Studio 2022 version 17.x puede permanecer instalado en paralelo, pero
  no debe usarse para cargar o depurar los proyectos .NET 10 de esta solucion.
- La configuracion Desktop usa `x64`; el proyecto es WinUI sin empaquetar y no
  requiere marcar `Implementar` en el Administrador de configuracion.

## Pipeline

1. Restore determinista.
2. Format/analyzers.
3. Build Release.
4. Unit and architecture tests.
5. Integration and contract tests.
6. Construir portal, MSIX/instalador y agente.
7. Generar SBOM y escanear.
8. Firmar con PKI interna o servicio aprobado en ambiente protegido.
9. Desplegar portal/control plane y worker al ambiente.
10. Publicar en anillo interno.
11. Promocion manual a piloto y produccion.

## Ambientes y anillos

- Local: emuladores/adaptadores fake, nunca credenciales productivas.
- Development: integraciones sandbox.
- Test: datos sinteticos y pruebas end-to-end.
- Pilot: usuarios/equipos controlados.
- Production: despliegue gradual con rollback.

Anillos del cliente/agente:

- internal;
- pilot;
- broad.

## Definicion de terminado

- Criterios funcionales y de accesibilidad cumplidos.
- Threat model actualizado.
- Telemetria, alertas y runbook incluidos.
- Migraciones probadas hacia delante y rollback operativo documentado.
- Paquetes firmados.
- Sin secretos ni datos reales en repositorio o fixtures.
- Evidencia de pruebas adjunta al release.

## Primeros incrementos

La secuencia detallada y sus gates viven en `../DEVELOPMENT_PLAN.md`.

1. Fundacion del repositorio, CI local y contratos base.
2. Shell WinUI con paridad visual y accesibilidad.
3. Dominio, catalogo sintetico y conversacion determinista.
4. Agente simulado, IPC y maquina de estados.
5. Diagnostico de solo lectura.
6. Primer adaptador real validado en VM Windows 11.
7. API compartida, PostgreSQL, worker y auditoria.
8. Casos internos y adaptador OpenText fake.
9. Integracion del bot de Teams existente.
10. Endurecimiento y prueba controlada en dos equipos.
11. Portal administrativo web.

Rescue conserva el flujo humano durante el MVP. Su integracion API no forma
parte de estos primeros incrementos.

## Para que sirven GitHub Actions y Azure DevOps

Son la fabrica automatizada del producto. Ante cada cambio:

- compilan cliente, agente, portal y backend;
- ejecutan pruebas y escaneos;
- producen artefactos repetibles;
- generan SBOM;
- firman desde un entorno protegido;
- despliegan a development/test/pilot con aprobaciones;
- conservan quien produjo y promovio cada version.

No son componentes que ejecuten en los equipos de usuario. Azure DevOps integra
repositorios, Boards, pipelines, artefactos y aprobaciones en el ecosistema
Microsoft. GitHub Actions hace CI/CD desde repositorios GitHub. Para Softtek se
elige el que ya este aprobado y conectado a identidades corporativas.
