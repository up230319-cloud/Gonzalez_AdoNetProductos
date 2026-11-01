using AdoNetProductos.Models;
using AdoNetProductos.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Swagger permite probar las rutas desde el navegador.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cuando se pide la interfaz, se utiliza ProductoRepository.
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Consultar todos los productos.
app.MapGet("/productos", (IProductoRepository repo) =>
{
    var productos = repo.ObtenerTodos();
    return Results.Ok(productos);
}).WithName("ObtenerTodos");

// Buscar un producto por su Id.
app.MapGet("/productos/{id:int:min(1)}", (int id, IProductoRepository repo) =>
{
    var producto = repo.ObtenerPorId(id);

    if (producto == null)
        return Results.NotFound(new { mensaje = "No existe un producto con ese Id." });

    return Results.Ok(producto);
}).WithName("ObtenerPorId");

// Insertar un producto. El Id lo genera SQL Server.
app.MapPost("/productos", (Producto producto, IProductoRepository repo) =>
{
    if (!EsValido(producto))
        return Results.BadRequest(new { mensaje = "Revisa nombre, descripción, precio y existencia." });

    producto.Id = repo.Insertar(producto);
    return Results.Created($"/productos/{producto.Id}", producto);
}).WithName("Insertar");

// Actualizar usando el Id de la URL.
app.MapPut("/productos/{id:int:min(1)}", (int id, Producto producto, IProductoRepository repo) =>
{
    if (!EsValido(producto))
        return Results.BadRequest(new { mensaje = "Revisa nombre, descripción, precio y existencia." });

    producto.Id = id;
    bool actualizado = repo.Actualizar(producto);

    if (!actualizado)
        return Results.NotFound(new { mensaje = "No existe el producto que quieres actualizar." });

    return Results.Ok(new { mensaje = "Producto actualizado." });
}).WithName("Actualizar");

// Eliminar un producto.
app.MapDelete("/productos/{id:int:min(1)}", (int id, IProductoRepository repo) =>
{
    bool eliminado = repo.Eliminar(id);

    if (!eliminado)
        return Results.NotFound(new { mensaje = "No existe el producto que quieres eliminar." });

    return Results.Ok(new { mensaje = "Producto eliminado." });
}).WithName("Eliminar");

app.Run();

// Esta revisión se comparte entre insertar y actualizar.
static bool EsValido(Producto producto)
{
    return !string.IsNullOrWhiteSpace(producto.Nombre)
        && producto.Nombre.Length <= 100
        && producto.Descripcion != null
        && producto.Descripcion.Length <= 255
        && producto.Precio > 0
        && producto.Precio <= 99999999.99m
        && decimal.Round(producto.Precio, 2) == producto.Precio
        && producto.Existencia >= 0;
}

