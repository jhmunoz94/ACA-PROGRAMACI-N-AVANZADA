using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Biblioteca.Entidades;

namespace Biblioteca.Datos
{
    public class PrestamoDatos
    {
        public List<Prestamo> Listar()
        {
            List<Prestamo> lista = new List<Prestamo>();

            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                try
                {
                    string query = @"
                        SELECT 
                            p.IdPrestamo,
                            p.IdUsuario,
                            p.IdLibro,
                            p.FechaPrestamo,
                            p.FechaDevolucion,
                            p.Estado,
                            u.Nombre AS NombreUsuario,
                            l.Titulo AS TituloLibro
                        FROM Prestamos p
                        INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
                        INNER JOIN Libros l ON p.IdLibro = l.IdLibro;";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    conexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Prestamo
                            {
                                IdPrestamo = Convert.ToInt32(dr["IdPrestamo"]),
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                IdLibro = Convert.ToInt32(dr["IdLibro"]),
                                FechaPrestamo = Convert.ToDateTime(dr["FechaPrestamo"]),
                                FechaDevolucion = dr["FechaDevolucion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaDevolucion"]) : (DateTime?)null,
                                Estado = dr["Estado"].ToString(),
                                NombreUsuario = dr["NombreUsuario"].ToString(),
                                TituloLibro = dr["TituloLibro"].ToString()
                            });
                        }
                    }
                }
                catch (Exception)
                {
                    lista = new List<Prestamo>();
                }
            }

            return lista;
        }

        public bool RegistrarPrestamo(Prestamo obj)
        {
            bool respuesta = false;

            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                try
                {
                    conexion.Open();

                    // 1. Validar existencias del libro
                    string queryStock = "SELECT Stock FROM Libros WHERE IdLibro = @IdLibro";
                    using (SqlCommand cmdStock = new SqlCommand(queryStock, conexion))
                    {
                        cmdStock.Parameters.AddWithValue("@IdLibro", obj.IdLibro);
                        object resultadoStock = cmdStock.ExecuteScalar();

                        if (resultadoStock == null || Convert.ToInt32(resultadoStock) <= 0)
                        {
                            return false; // Sin stock
                        }
                    }

                    // 2. Registrar préstamo y descontar 1 unidad al stock
                    string queryInsertar = @"
                        INSERT INTO Prestamos (IdUsuario, IdLibro, FechaPrestamo, FechaDevolucion, Estado) 
                        VALUES (@IdUsuario, @IdLibro, @FechaPrestamo, @FechaDevolucion, 'Prestado');
                        
                        UPDATE Libros 
                        SET Stock = Stock - 1 
                        WHERE IdLibro = @IdLibro;";

                    using (SqlCommand cmd = new SqlCommand(queryInsertar, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdUsuario", obj.IdUsuario);
                        cmd.Parameters.AddWithValue("@IdLibro", obj.IdLibro);
                        cmd.Parameters.AddWithValue("@FechaPrestamo", obj.FechaPrestamo);
                        cmd.Parameters.AddWithValue("@FechaDevolucion", (object)obj.FechaDevolucion ?? DBNull.Value);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        respuesta = filasAfectadas > 0;
                    }
                }
                catch (Exception)
                {
                    respuesta = false;
                }
            }

            return respuesta;
        }

        public bool RegistrarDevolucion(int idPrestamo, DateTime fechaDevolucion)
        {
            bool respuesta = false;

            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                try
                {
                    conexion.Open();

                    // Marcar préstamo como Devuelto y sumar 1 unidad al stock del libro correspondiente
                    string query = @"
                        UPDATE Prestamos 
                        SET FechaDevolucion = @FechaDevolucion, Estado = 'Devuelto' 
                        WHERE IdPrestamo = @IdPrestamo;

                        UPDATE Libros 
                        SET Stock = Stock + 1 
                        WHERE IdLibro = (SELECT IdLibro FROM Prestamos WHERE IdPrestamo = @IdPrestamo);";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@FechaDevolucion", fechaDevolucion);
                        cmd.Parameters.AddWithValue("@IdPrestamo", idPrestamo);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        respuesta = filasAfectadas > 0;
                    }
                }
                catch (Exception)
                {
                    respuesta = false;
                }
            }

            return respuesta;
        }

        // Métodos de compatibilidad por si se llaman con los nombres cortos
        public bool Registrar(Prestamo obj) => RegistrarPrestamo(obj);
        public bool Devolver(int idPrestamo, DateTime fechaDevolucion) => RegistrarDevolucion(idPrestamo, fechaDevolucion);
    }
}