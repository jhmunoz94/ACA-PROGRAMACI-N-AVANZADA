using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Biblioteca.Entidades;

namespace Biblioteca.Datos
{
    public class UsuarioDatos
    {
        public List<Usuario> Listar()
        {
            List<Usuario> lista = new List<Usuario>();
            string query = "SELECT IdUsuario, Documento, Nombre, Apellido, Telefono, Correo FROM Usuarios";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Usuario
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                Documento = dr["Documento"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Apellido = dr["Apellido"].ToString(),
                                Telefono = dr["Telefono"] != DBNull.Value ? dr["Telefono"].ToString() : "",
                                Correo = dr["Correo"] != DBNull.Value ? dr["Correo"].ToString() : ""
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool Insertar(Usuario usuario)
        {
            string query = "INSERT INTO Usuarios (Documento, Nombre, Apellido, Telefono, Correo) " +
                           "VALUES (@Documento, @Nombre, @Apellido, @Telefono, @Correo)";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Documento", usuario.Documento);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(usuario.Telefono) ? (object)DBNull.Value : usuario.Telefono);
                    cmd.Parameters.AddWithValue("@Correo", string.IsNullOrEmpty(usuario.Correo) ? (object)DBNull.Value : usuario.Correo);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Modificar(Usuario usuario)
        {
            string query = "UPDATE Usuarios SET Documento = @Documento, Nombre = @Nombre, Apellido = @Apellido, " +
                           "Telefono = @Telefono, Correo = @Correo WHERE IdUsuario = @IdUsuario";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@Documento", usuario.Documento);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(usuario.Telefono) ? (object)DBNull.Value : usuario.Telefono);
                    cmd.Parameters.AddWithValue("@Correo", string.IsNullOrEmpty(usuario.Correo) ? (object)DBNull.Value : usuario.Correo);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Eliminar(int idUsuario)
        {
            string query = "DELETE FROM Usuarios WHERE IdUsuario = @IdUsuario";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
