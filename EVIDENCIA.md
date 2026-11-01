# Evidencia de la versión sencilla

Fecha: 1 de noviembre de 2026.

## Verificado en este entorno

- Se descargó y revisó https://github.com/UP230612/AdoNetPractica.git.
- Referencia consultada: commit 55c3425af04eeb9f7e845797382db89dfe5f9969.
- Se creó una versión separada en AdoNetProductos con rutas en Program.cs y constructores normales.
- Se implementa exactamente el contrato IProductoRepository solicitado.
- dotnet build: compilación correcta, 0 advertencias y 0 errores.
- dotnet run --no-build: inició la API en http://localhost:5081.
- GET /swagger/index.html: HTTP 200.
- GET /swagger/v1/swagger.json: aparecen GET y POST /productos, y GET, PUT y DELETE /productos/{id}.
- POST /productos con nombre vacío y precio negativo: HTTP 400.
- PUT /productos/1 con precio de tres decimales: HTTP 400.

## Pendiente de comprobar con una base disponible

La conexión a (localdb)\MSSQLLocalDB mediante sqlcmd agotó el tiempo de espera. Por ello no se verificó la ejecución del script SQL ni el ciclo CRUD contra SQL Server. Las pruebas de Swagger y validación anteriores no demuestran persistencia.

El README contiene la secuencia para probar inserción, consulta, búsqueda, actualización y eliminación cuando SQL Server esté disponible.

Se entrega un repositorio Git local, un bundle con historial y un ZIP del código. No se ha publicado un repositorio remoto.

