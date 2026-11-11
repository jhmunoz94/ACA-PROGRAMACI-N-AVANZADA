using System;
using System.Collections.Generic;
using Biblioteca.Datos;
using Biblioteca.Entidades;

namespace Biblioteca.Negocio
{
    public class EditorialNegocio
    {
        private EditorialDatos datos = new EditorialDatos();

        public List<Editorial> Listar()
        {
            return datos.Listar();
        }

        public string Registrar(Editorial editorial)
        {
            if (string.IsNullOrWhiteSpace(editorial.Nombre))
                return "El nombre de la editorial es obligatorio.";

            bool exito = datos.Insertar(editorial);
            return exito ? "OK" : "No se pudo registrar la editorial.";
        }

        public string Modificar(Editorial editorial)
        {
            if (editorial.IdEditorial <= 0)
                return "Seleccione una editorial válida para modificar.";

            if (string.IsNullOrWhiteSpace(editorial.Nombre))
                return "El nombre de la editorial es obligatorio.";

            bool exito = datos.Modificar(editorial);
            return exito ? "OK" : "No se pudo actualizar la editorial.";
        }

        public string Eliminar(int idEditorial)
        {
            try
            {
                bool exito = datos.Eliminar(idEditorial);
                return exito ? "OK" : "No se pudo eliminar la editorial.";
            }
            catch (Exception)
            {
                return "No es posible eliminar la editorial porque tiene libros asociados en el sistema.";
            }
        }
    }
}
