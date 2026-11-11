using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Biblioteca.Entidades;

namespace Biblioteca.Datos
{
    public class AutorDatos
    {
        public List<Autor> Listar()
        {
            List<Autor> lista = new List<Autor>();
            string query = "SELECT IdAutor, Nombre, Apellido, Nacionalidad FROM Autores";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Autor
                            {
                                IdAutor = Convert.ToInt32(dr["IdAutor"]),
                                Nombre = dr["Nombre"].ToString(),
                                Apellido = dr["Apellido"].ToString(),
                                Nacionalidad = dr["Nacionalidad"] != DBNull.Value ? dr["Nacionalidad"].ToString() : ""
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool Insertar(Autor autor)
        {
            string query = "INSERT INTO Autores (Nombre, Apellido, Nacionalidad) VALUES (@Nombre, @Apellido, @Nacionalidad)";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Nombre", autor.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", autor.Apellido);
                    cmd.Parameters.AddWithValue("@Nacionalidad", string.IsNullOrEmpty(autor.Nacionalidad) ? (object)DBNull.Value : autor.Nacionalidad);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Modificar(Autor autor)
        {
            string query = "UPDATE Autores SET Nombre = @Nombre, Apellido = @Apellido, Nacionalidad = @Nacionalidad WHERE IdAutor = @IdAutor";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdAutor", autor.IdAutor);
                    cmd.Parameters.AddWithValue("@Nombre", autor.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", autor.Apellido);
                    cmd.Parameters.AddWithValue("@Nacionalidad", string.IsNullOrEmpty(autor.Nacionalidad) ? (object)DBNull.Value : autor.Nacionalidad);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Eliminar(int idAutor)
        {
            string query = "DELETE FROM Autores WHERE IdAutor = @IdAutor";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdAutor", idAutor);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
