using AdoNetProductos.Models;
using Microsoft.Data.SqlClient;

namespace AdoNetProductos.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly string _connectionString;

        public ProductoRepository(IConfiguration configuration)
        {
            // La conexión se lee de appsettings.json.
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Falta la conexión DefaultConnection.");
        }

        public List<Producto> ObtenerTodos()
        {
            var productos = new List<Producto>();

            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            string sql = "SELECT Id, Nombre, Descripcion, Precio, Existencia FROM dbo.Productos ORDER BY Id";
            using var comando = new SqlCommand(sql, conexion);
            using var lector = comando.ExecuteReader();

            // Leemos una fila a la vez y la convertimos en un producto.
            while (lector.Read())
            {
                var producto = new Producto
                {
                    Id = lector.GetInt32(0),
                    Nombre = lector.GetString(1),
                    Descripcion = lector.GetString(2),
                    Precio = lector.GetDecimal(3),
                    Existencia = lector.GetInt32(4)
                };

                productos.Add(producto);
            }

            return productos;
        }

        public Producto? ObtenerPorId(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            string sql = "SELECT Id, Nombre, Descripcion, Precio, Existencia FROM dbo.Productos WHERE Id = @Id";
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.AddWithValue("@Id", id);

            using var lector = comando.ExecuteReader();

            if (lector.Read())
            {
                return new Producto
                {
                    Id = lector.GetInt32(0),
                    Nombre = lector.GetString(1),
                    Descripcion = lector.GetString(2),
                    Precio = lector.GetDecimal(3),
                    Existencia = lector.GetInt32(4)
                };
            }

            return null;
        }

        public int Insertar(Producto producto)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            string sql = @"
                INSERT INTO dbo.Productos (Nombre, Descripcion, Precio, Existencia)
                OUTPUT INSERTED.Id
                VALUES (@Nombre, @Descripcion, @Precio, @Existencia)";

            using var comando = new SqlCommand(sql, conexion);

            // Los valores se envían como parámetros, sin concatenarlos con el SQL.
            comando.Parameters.AddWithValue("@Nombre", producto.Nombre);
            comando.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
            comando.Parameters.AddWithValue("@Precio", producto.Precio);
            comando.Parameters.AddWithValue("@Existencia", producto.Existencia);

            // SQL Server genera el Id y ExecuteScalar lo recupera.
            int nuevoId = Convert.ToInt32(comando.ExecuteScalar());
            return nuevoId;
        }

        public bool Actualizar(Producto producto)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            string sql = @"
                UPDATE dbo.Productos
                SET Nombre = @Nombre,
                    Descripcion = @Descripcion,
                    Precio = @Precio,
                    Existencia = @Existencia
                WHERE Id = @Id";

            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.AddWithValue("@Id", producto.Id);
            comando.Parameters.AddWithValue("@Nombre", producto.Nombre);
            comando.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
            comando.Parameters.AddWithValue("@Precio", producto.Precio);
            comando.Parameters.AddWithValue("@Existencia", producto.Existencia);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }

        public bool Eliminar(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            conexion.Open();

            string sql = "DELETE FROM dbo.Productos WHERE Id = @Id";
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.AddWithValue("@Id", id);

            int filasAfectadas = comando.ExecuteNonQuery();
            return filasAfectadas > 0;
        }
    }
}
