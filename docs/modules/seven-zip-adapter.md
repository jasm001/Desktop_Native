# Adaptador 7-Zip 26.01

## Estado

Incremento del Bloque 6 seleccionado el 2026-06-13. El adaptador y sus pruebas
automatizadas pueden construirse en el host, pero la ejecucion del paquete queda
prohibida hasta disponer de una VM Windows 11 desechable con checkpoint
verificable.

## Paquete fijado

| Campo | Valor |
| --- | --- |
| Producto | 7-Zip |
| Version | 26.01, publicada el 2026-04-27 |
| Paquete | MSI alternativo oficial |
| Arquitectura del incremento | x64 |
| Target | `seven-zip` / `26.01` |
| Adaptador | `seven-zip.msi.v1` |
| Acciones | `software.install.7zip.v1`, `software.uninstall.7zip.v1` |
| Credenciales | Ninguna |
| Drivers | Ninguno esperado; confirmar en VM |
| Reinicio | No iniciado por el adaptador; `msiexec /norestart` |
| Archivo | `7z2601-x64.msi`, 2,002,432 bytes |
| SHA-256 | `A47EA8DCF8BC08E6DE474CAE77C828E031FA22CB528F6095DEFFFEBF11CD02F2` |
| ProductCode | `{23170F69-40C1-2702-2601-000001000000}` |
| Version MSI | `26.01.00.0` |
| Firma | Sin firma Authenticode |

La pagina oficial publica instaladores x64, x86 y ARM64, pero este incremento
implementa unicamente el MSI x64. x86 y ARM64 requieren manifiestos, hashes,
pruebas y autorizaciones separados.

## Evidencia oficial previa a descarga

- Fuente y version:
  `https://www.7-zip.org/download.html` publica 7-Zip 26.01 y enlaza el MSI x64
  alternativo desde la release oficial `ip7z/7zip`.
- Release:
  `https://github.com/ip7z/7zip/releases/tag/26.01`.
- Licencia:
  `https://www.7-zip.org/license.txt` declara GNU LGPL para los archivos
  generales, componentes BSD y la restriccion unRAR. Permite redistribucion
  binaria si se reproducen los avisos de licencia relacionados.
- Modo unattended:
  `https://learn.microsoft.com/windows-server/administration/windows-commands/msiexec`
  documenta `/i`, `/x`, `/qn` y `/norestart` para Windows 11.
- Codigos MSI:
  `https://learn.microsoft.com/windows/win32/msi/error-codes` define `0`,
  `1641` y `3010` como resultados de exito, con reinicio iniciado o requerido
  para los dos ultimos.

No se reempaqueta ni modifica el MSI. El mirror de laboratorio debe conservar
el archivo oficial y una copia de `license.txt`. El manifiesto versionado es
`deploy/local-demo/manifests/seven-zip-26.01-x64.json`.

La inspeccion de solo lectura se realizo el 2026-06-13 sin ejecutar el paquete.
`license.txt` descargado desde el origen oficial mide 6,031 bytes y tiene
SHA-256
`3A184AA13DC8AD30734E28FF901B478CCBCB5B41BE52427A5E7609C8FD9E5DDB`.

## Contrato cerrado implementado

El IPC solo envia accion, target y version. El adaptador resuelve internamente:

- el `artifactId` fijo;
- el nombre exacto del MSI;
- el SHA-256 esperado;
- `msiexec.exe` desde el directorio de sistema de Windows;
- argumentos de instalacion `/i <artefacto> /qn /norestart`;
- argumentos de desinstalacion `/x <ProductCode> /qn /norestart`;
- timeout, codigos de salida y politica de retry.

No se aceptan URL, ruta, ejecutable, ProductCode, switches ni argumentos desde
IPC, WinUI, backend, Hermes o IA.

## Politica operativa implementada

- `Detect`: consulta la identidad MSI fijada y comprueba version exacta.
- `Preflight`: exige perfil `local-demo`, Windows x64, artefacto disponible y
  longitud/hash exactos. Una transaccion MSI concurrente devuelve `1618` y
  falla sin retry interno.
- `Install`: no ejecuta si `Detect` ya confirma 26.01.
- `Verify`: repite deteccion despues de una salida aceptada.
- `Uninstall`: no ejecuta si el producto ya esta ausente.
- Timeout: fijo por operacion; un timeout falla cerrado y no se reintenta
  automaticamente.
- Retry: solo una nueva solicitud explicita e idempotente despues de
  diagnosticar el estado. `1618` no se reintenta dentro del adaptador.
- Reinicio: el adaptador nunca reinicia. `3010` se devuelve como exito con
  reinicio pendiente. `1641` falla cerrado porque indica que el instalador
  inicio un reinicio pese a `/norestart`.
- Cancelacion: segura antes de iniciar `msiexec`; durante una transaccion MSI se
  rechaza con estado tipado y la operacion continua hasta salida o timeout.
- Evidencia: solo codigos y textos fijos de fase/resultado. No se conservan
  stdout, stderr, exit code, rutas, nombres internos, hash, ProductCode,
  payloads ni excepciones.

## Matriz VM pendiente

1. Confirmar Windows 11 x64 limpio y checkpoint inicial.
2. Confirmar ausencia mediante `Detect`.
3. Servir MSI y licencia desde el mirror local.
4. Ejecutar `Preflight`, `Install` y `Verify`.
5. Repetir `Install` y confirmar idempotencia sin nueva ejecucion.
6. Ejecutar `Uninstall` y verificar ausencia.
7. Repetir `Uninstall` y confirmar idempotencia.
8. Probar mirror ausente.
9. Probar copia con SHA-256 incorrecto.
10. Probar timeout/fallo controlado sin cambiar argumentos.
11. Restaurar el checkpoint y confirmar el estado inicial.

Los resultados, tiempos, codigos de salida y evidencia saneada se agregaran a
este documento. Hasta entonces el Bloque 6 permanece `in_progress`.

## Evidencia automatizada

Validacion local del 2026-06-13:

- build Release completo: correcto, 0 warnings y 0 errores;
- 110 pruebas: correctas;
- cobertura agregada: manifiesto, mirror, longitud, SHA-256, perfil,
  plataforma, argumentos fijos, idempotencia, timeout, `1603`, `1641`, `3010`,
  fallo de inicio, verificacion, desinstalacion, seleccion, autorizacion y
  cancelacion segura/no segura;
- las pruebas usan dobles y no ejecutan el MSI;
- `dotnet format --verify-no-changes`: correcto;
- Named Pipe: correcto fuera del sandbox.
- `scripts/Validate.ps1`: correcto, incluidos workspace pnpm y escaneo de
  secretos.

La matriz VM sigue pendiente por permisos Hyper-V.
