using System.Data.SqlClient;

namespace Biblioteca.Datos
{
    public class Conexion
    {
        // Si tu instancia tiene nombre específico (ej. SQLEXPRESS), usa: Server=.\\SQLEXPRESS;...
        // Si usas la instancia por defecto o LocalDB, 'localhost' o '.' funciona directo.
        private static readonly string CadenaConexion =
            @"Server=.;Database=BibliotecaDB;Integrated Security=True;TrustServerCertificate=True;";
        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(CadenaConexion);
        }
    }
}