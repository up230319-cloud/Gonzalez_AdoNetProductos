namespace AdoNetProductos.Models
{
    // Cada propiedad corresponde a una columna de la tabla Productos.
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Existencia { get; set; }
    }
}
