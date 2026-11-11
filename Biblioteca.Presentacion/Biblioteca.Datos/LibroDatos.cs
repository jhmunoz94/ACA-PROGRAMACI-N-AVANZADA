using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Biblioteca.Entidades;

namespace Biblioteca.Datos
{
    public class LibroDatos
    {
        public List<Libro> Listar()
        {
            List<Libro> lista = new List<Libro>();
            string query = @"SELECT L.IdLibro, L.CodigoISBN, L.Titulo, L.AnioPublicacion, L.Stock, 
                                    L.IdAutor, L.IdEditorial,
                                    (A.Nombre + ' ' + A.Apellido) AS Autor,
                                    E.Nombre AS Editorial
                             FROM Libros L
                             INNER JOIN Autores A ON L.IdAutor = A.IdAutor
                             INNER JOIN Editoriales E ON L.IdEditorial = E.IdEditorial";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Libro
                            {
                                IdLibro = Convert.ToInt32(dr["IdLibro"]),
                                CodigoISBN = dr["CodigoISBN"].ToString(),
                                Titulo = dr["Titulo"].ToString(),
                                AnioPublicacion = dr["AnioPublicacion"] != DBNull.Value ? Convert.ToInt32(dr["AnioPublicacion"]) : 0,
                                Stock = Convert.ToInt32(dr["Stock"]),
                                IdAutor = Convert.ToInt32(dr["IdAutor"]),
                                IdEditorial = Convert.ToInt32(dr["IdEditorial"]),
                                NombreAutor = dr["Autor"].ToString(),
                                NombreEditorial = dr["Editorial"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool Insertar(Libro libro)
        {
            string query = @"INSERT INTO Libros (CodigoISBN, Titulo, AnioPublicacion, Stock, IdAutor, IdEditorial) 
                             VALUES (@CodigoISBN, @Titulo, @AnioPublicacion, @Stock, @IdAutor, @IdEditorial)";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CodigoISBN", libro.CodigoISBN);
                    cmd.Parameters.AddWithValue("@Titulo", libro.Titulo);
                    cmd.Parameters.AddWithValue("@AnioPublicacion", libro.AnioPublicacion);
                    cmd.Parameters.AddWithValue("@Stock", libro.Stock);
                    cmd.Parameters.AddWithValue("@IdAutor", libro.IdAutor);
                    cmd.Parameters.AddWithValue("@IdEditorial", libro.IdEditorial);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Modificar(Libro libro)
        {
            string query = @"UPDATE Libros 
                             SET CodigoISBN = @CodigoISBN, Titulo = @Titulo, AnioPublicacion = @AnioPublicacion, 
                                 Stock = @Stock, IdAutor = @IdAutor, IdEditorial = @IdEditorial 
                             WHERE IdLibro = @IdLibro";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdLibro", libro.IdLibro);
                    cmd.Parameters.AddWithValue("@CodigoISBN", libro.CodigoISBN);
                    cmd.Parameters.AddWithValue("@Titulo", libro.Titulo);
                    cmd.Parameters.AddWithValue("@AnioPublicacion", libro.AnioPublicacion);
                    cmd.Parameters.AddWithValue("@Stock", libro.Stock);
                    cmd.Parameters.AddWithValue("@IdAutor", libro.IdAutor);
                    cmd.Parameters.AddWithValue("@IdEditorial", libro.IdEditorial);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Eliminar(int idLibro)
        {
            string query = "DELETE FROM Libros WHERE IdLibro = @IdLibro";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdLibro", idLibro);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
