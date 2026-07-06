# Catalogo local curado de laboratorio

## Alcance

La Unidad 4 agrega una proyeccion administrativa de solo lectura para
artefactos libres permitidos en `local-demo`. No cambia el catalogo corporativo,
no publica instaladores, no agrega migraciones y no habilita nuevas solicitudes
desde el portal.

La fuente curada vive en `deploy/local-demo/manifests/`. Cada manifiesto debe
declarar:

- producto, version fija, arquitectura y version de paquete;
- licencia redistribuible allowlisted y URL publica de licencia;
- origen publico del artefacto y release fija;
- SHA-256, longitud esperada y adaptador compatible;
- `environment: development-only`.

## Gobierno

El portal muestra tres categorias separadas:

- catalogo sintetico: decisiones demo ya existentes;
- catalogo de laboratorio: artefactos libres con manifiesto, hash y origen
  publico bajo `lab-real-sanitized`;
- catalogo corporativo futuro: no conectado en esta unidad.

El catalogo de laboratorio solo se lista cuando `IT_SUPPORT_ENVIRONMENT` es
`development` y el perfil efectivo es `local-demo`. Cualquier otro ambiente o
perfil devuelve una lista vacia.

## Estados de artefacto

La lectura del portal nunca ejecuta comandos ni llama al DeviceAgent. Solo
consulta el mirror configurado mediante `IT_SUPPORT_LAB_MIRROR_PATH`:

- `available`: el archivo existe fuera del repositorio Git, su longitud coincide
  y el SHA-256 calculado coincide con el manifiesto;
- `absent`: el mirror no esta configurado, apunta dentro de Git, falta el
  archivo o no se puede leer;
- `hash_mismatch`: la longitud o el SHA-256 no coinciden.

El instalador no se versiona en Git. El manifiesto es metadata de desarrollo y
el fallo de artefacto ocurre antes de iniciar ejecucion en el adaptador local.

## Validaciones cubiertas

- licencia redistribuible allowlisted;
- version fija, sin `latest` ni rangos;
- origen publico permitido;
- SHA-256 hexadecimal de 64 caracteres;
- confinamiento a `Development`/`local-demo`;
- render administrativo sin formularios, botones ni mutaciones.
