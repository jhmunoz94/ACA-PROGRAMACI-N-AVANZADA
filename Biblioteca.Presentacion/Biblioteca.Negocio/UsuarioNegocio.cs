using Biblioteca.Datos;
using Biblioteca.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Biblioteca.Negocio
{
    public class UsuarioNegocio
    {
        private UsuarioDatos datos = new UsuarioDatos();

        public List<Usuario> Listar()
        {
            return datos.Listar();
        }

        public string Registrar(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Documento))
                return "El número de documento es obligatorio.";

            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                return "El nombre del usuario es obligatorio.";

            if (string.IsNullOrWhiteSpace(usuario.Apellido))
                return "El apellido del usuario es obligatorio.";

            try
            {
                bool exito = datos.Insertar(usuario);
                return exito ? "OK" : "No se pudo registrar el usuario.";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601) // Error de clave única duplicada en SQL Server
                    return "Ya existe un usuario registrado con ese número de documento.";

                return "Error al guardar el usuario: " + ex.Message;
            }
        }

        public string Modificar(Usuario usuario)
        {
            if (usuario.IdUsuario <= 0)
                return "Seleccione un usuario válido de la lista para modificar.";

            if (string.IsNullOrWhiteSpace(usuario.Documento))
                return "El número de documento es obligatorio.";

            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                return "El nombre del usuario es obligatorio.";

            if (string.IsNullOrWhiteSpace(usuario.Apellido))
                return "El apellido del usuario es obligatorio.";

            try
            {
                bool exito = datos.Modificar(usuario);
                return exito ? "OK" : "No se pudo actualizar el usuario.";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    return "El número de documento ya pertenece a otro usuario registrado.";

                return "Error al modificar el usuario: " + ex.Message;
            }
        }

        public string Eliminar(int idUsuario)
        {
            try
            {
                bool exito = datos.Eliminar(idUsuario);
                return exito ? "OK" : "No se pudo eliminar el usuario.";
            }
            catch (Exception)
            {
                return "No es posible eliminar el usuario porque registra préstamos asociados en el sistema.";
            }
        }
    }
}
