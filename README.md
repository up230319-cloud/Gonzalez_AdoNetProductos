# CRUD sencillo con ADO.NET y SQL Server

Esta versión se hizo tomando como guía [AdoNetPractica](https://github.com/UP230612/AdoNetPractica), revisión `55c3425af04eeb9f7e845797382db89dfe5f9969`.

Se conserva su estructura: rutas en Program.cs, modelo Producto, interfaz y repositorio. Se agregaron validaciones básicas y un script que se puede ejecutar de nuevo sin duplicar productos. No se usa Entity Framework.

## Archivos que debes conocer

| Archivo | Para qué sirve |
|---|---|
| Program.cs | Configura Swagger y contiene las cinco rutas |
| Models/Producto.cs | Define los datos de un producto |
| Repositories/IProductoRepository.cs | Declara los cinco métodos solicitados |
| Repositories/ProductoRepository.cs | Abre la conexión y ejecuta las consultas SQL |
| appsettings.json | Guarda la cadena de conexión |
| Scripts/CrearTabla.sql | Crea la base y la tabla |
| AdoNetProductos.http | Contiene ejemplos de peticiones |

## Cómo ejecutarlo

Necesitas .NET SDK 10 y SQL Server. La configuración incluida usa SQL Server LocalDB.

1. Abre SQL Server Management Studio y conéctate a `(localdb)\MSSQLLocalDB`.
2. Abre y ejecuta `Scripts/CrearTabla.sql`. Se creará la base `ProductosDB` con una tabla vacía.
3. Abre una terminal dentro de esta carpeta y ejecuta:

```powershell
dotnet restore
dotnet run
```

4. Abre [Swagger](http://localhost:5081/swagger/index.html).

## Probar las cinco operaciones en Swagger

En cada operación pulsa **Try it out**, completa los datos y pulsa **Execute**.

1. **POST /productos**: inserta este producto. El Id se genera en SQL Server.

```json
{
  "nombre": "Mouse",
  "descripcion": "Mouse USB",
  "precio": 150.50,
  "existencia": 10
}
```

La respuesta debe ser **201**, incluir el producto y mostrar su Id.

2. **GET /productos**: consulta todos; debe responder **200** con una lista.
3. **GET /productos/{id}**: usa el Id que recibiste en el POST. Debe responder **200** con ese producto.
4. **PUT /productos/{id}**: usa el mismo Id en la URL y este cuerpo:

```json
{
  "nombre": "Mouse",
  "descripcion": "Mouse USB actualizado",
  "precio": 175.00,
  "existencia": 8
}
```

Debe responder **200**. Vuelve a buscarlo para comprobar el cambio. En esta versión se utiliza el Id de la URL.

5. **DELETE /productos/{id}**: elimina el producto y responde **200**. Si vuelves a buscarlo, debe responder **404**.

Buscar, actualizar o eliminar un Id inexistente devuelve **404**. Un nombre vacío, un precio inválido o una existencia negativa devuelve **400**. El nombre admite hasta 100 caracteres, la descripción hasta 255 y el precio debe ser positivo, con máximo dos decimales y valor máximo de 99999999.99.

## Explicación del flujo

El recorrido es: **Swagger → ruta de Program.cs → repositorio → SQL Server → respuesta HTTP**.

Swagger es la pantalla donde hacemos las pruebas. Al pulsar Execute, envía una petición a una ruta. Por ejemplo, GET /productos pide consultar la lista y POST /productos pide guardar un producto.

Program.cs recibe esa petición y llama al método correspondiente de IProductoRepository. La interfaz es el contrato: indica los métodos que debe tener el repositorio. La línea AddScoped conecta ese contrato con la clase ProductoRepository.

El constructor del repositorio recibe IConfiguration y lee DefaultConnection desde appsettings.json. Cada método crea una SqlConnection y llama a Open para comunicarse con SQL Server. Las instrucciones using liberan los recursos al terminar el método.

**ObtenerTodos** ejecuta SELECT mediante ExecuteReader. El lector avanza fila por fila con Read y cada fila se convierte en un Producto que se agrega a la lista.

**ObtenerPorId** también ejecuta SELECT, pero agrega WHERE Id = @Id para buscar un solo registro. Devuelve un Producto si lo encuentra; si no, devuelve null.

**Insertar** ejecuta INSERT. OUTPUT INSERTED.Id le pide a SQL Server el identificador que acaba de generar. ExecuteScalar obtiene ese único valor y el método lo devuelve como int.

**Actualizar** ejecuta UPDATE para cambiar nombre, descripción, precio y existencia del producto indicado. ExecuteNonQuery devuelve cuántas filas cambió. Si cambió una, el método devuelve true; si no encontró el Id, devuelve false.

**Eliminar** ejecuta DELETE usando el Id recibido. También usa ExecuteNonQuery para saber si eliminó una fila.

Los valores se pasan mediante parámetros como @Id y @Nombre. AddWithValue asigna el valor a cada parámetro. Así no mezclamos el texto que envía el usuario con la instrucción SQL.

Antes de insertar o actualizar, EsValido revisa los datos básicos. Finalmente, cada ruta transforma el resultado en una respuesta: 200 para operaciones correctas, 201 para creación, 400 para datos inválidos y 404 cuando no existe el producto.



