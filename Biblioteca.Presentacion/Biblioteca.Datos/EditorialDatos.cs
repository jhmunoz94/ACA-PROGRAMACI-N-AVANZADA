using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Biblioteca.Entidades;

namespace Biblioteca.Datos
{
    public class EditorialDatos
    {
        public List<Editorial> Listar()
        {
            List<Editorial> lista = new List<Editorial>();
            string query = "SELECT IdEditorial, Nombre, Pais FROM Editoriales";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Editorial
                            {
                                IdEditorial = Convert.ToInt32(dr["IdEditorial"]),
                                Nombre = dr["Nombre"].ToString(),
                                Pais = dr["Pais"] != DBNull.Value ? dr["Pais"].ToString() : ""
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool Insertar(Editorial editorial)
        {
            string query = "INSERT INTO Editoriales (Nombre, Pais) VALUES (@Nombre, @Pais)";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Nombre", editorial.Nombre);
                    cmd.Parameters.AddWithValue("@Pais", string.IsNullOrEmpty(editorial.Pais) ? (object)DBNull.Value : editorial.Pais);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Modificar(Editorial editorial)
        {
            string query = "UPDATE Editoriales SET Nombre = @Nombre, Pais = @Pais WHERE IdEditorial = @IdEditorial";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdEditorial", editorial.IdEditorial);
                    cmd.Parameters.AddWithValue("@Nombre", editorial.Nombre);
                    cmd.Parameters.AddWithValue("@Pais", string.IsNullOrEmpty(editorial.Pais) ? (object)DBNull.Value : editorial.Pais);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Eliminar(int idEditorial)
        {
            string query = "DELETE FROM Editoriales WHERE IdEditorial = @IdEditorial";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdEditorial", idEditorial);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}