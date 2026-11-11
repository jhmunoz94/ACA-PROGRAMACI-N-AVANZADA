using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Biblioteca.Datos;
using Biblioteca.Entidades;

namespace Biblioteca.Negocio
{
    public class LibroNegocio
    {
        private LibroDatos datos = new LibroDatos();

        public List<Libro> Listar()
        {
            return datos.Listar();
        }

        public string Registrar(Libro libro)
        {
            if (string.IsNullOrWhiteSpace(libro.CodigoISBN))
                return "El código ISBN es obligatorio.";

            if (string.IsNullOrWhiteSpace(libro.Titulo))
                return "El título del libro es obligatorio.";

            if (libro.IdAutor <= 0)
                return "Debe seleccionar un autor para el libro.";

            if (libro.IdEditorial <= 0)
                return "Debe seleccionar una editorial para el libro.";

            if (libro.Stock < 0)
                return "El stock no puede ser un número negativo.";

            try
            {
                bool exito = datos.Insertar(libro);
                return exito ? "OK" : "No se pudo registrar el libro.";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    return "Ya existe un libro con ese código ISBN.";

                return "Error al guardar el libro: " + ex.Message;
            }
        }

        public string Modificar(Libro libro)
        {
            if (libro.IdLibro <= 0)
                return "Seleccione un libro válido de la tabla para modificar.";

            if (string.IsNullOrWhiteSpace(libro.CodigoISBN))
                return "El código ISBN es obligatorio.";

            if (string.IsNullOrWhiteSpace(libro.Titulo))
                return "El título del libro es obligatorio.";

            if (libro.IdAutor <= 0)
                return "Debe seleccionar un autor.";

            if (libro.IdEditorial <= 0)
                return "Debe seleccionar una editorial.";

            if (libro.Stock < 0)
                return "El stock no puede ser un número negativo.";

            try
            {
                bool exito = datos.Modificar(libro);
                return exito ? "OK" : "No se pudo actualizar el libro.";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    return "El código ISBN ya pertenece a otro libro registrado.";

                return "Error al actualizar: " + ex.Message;
            }
        }

        public string Eliminar(int idLibro)
        {
            try
            {
                bool exito = datos.Eliminar(idLibro);
                return exito ? "OK" : "No se pudo eliminar el libro.";
            }
            catch (Exception)
            {
                return "No se puede eliminar el libro porque está registrado en préstamos históricos o activos.";
            }
        }
    }
}
