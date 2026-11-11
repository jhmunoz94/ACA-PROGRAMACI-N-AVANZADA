using System;
using System.Collections.Generic;
using Biblioteca.Datos;
using Biblioteca.Entidades;

namespace Biblioteca.Negocio
{
    public class AutorNegocio
    {
        private AutorDatos datos = new AutorDatos();

        public List<Autor> Listar()
        {
            return datos.Listar();
        }

        public string Registrar(Autor autor)
        {
            if (string.IsNullOrWhiteSpace(autor.Nombre))
                return "El nombre del autor es obligatorio.";

            if (string.IsNullOrWhiteSpace(autor.Apellido))
                return "El apellido del autor es obligatorio.";

            bool exito = datos.Insertar(autor);
            return exito ? "OK" : "No se pudo registrar el autor.";
        }

        public string Modificar(Autor autor)
        {
            if (autor.IdAutor <= 0)
                return "Seleccione un autor válido para modificar.";

            if (string.IsNullOrWhiteSpace(autor.Nombre))
                return "El nombre del autor es obligatorio.";

            if (string.IsNullOrWhiteSpace(autor.Apellido))
                return "El apellido del autor es obligatorio.";

            bool exito = datos.Modificar(autor);
            return exito ? "OK" : "No se pudo actualizar el autor.";
        }

        public string Eliminar(int idAutor)
        {
            try
            {
                bool exito = datos.Eliminar(idAutor);
                return exito ? "OK" : "No se pudo eliminar el autor.";
            }
            catch (Exception)
            {
                // Previene caídas si la clave foránea en Libros restringe el borrado
                return "No es posible eliminar el autor porque tiene libros asociados en el sistema.";
            }
        }
    }
}
