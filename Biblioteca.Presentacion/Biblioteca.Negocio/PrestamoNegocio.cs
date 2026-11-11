using System;
using System.Collections.Generic;
using Biblioteca.Datos;
using Biblioteca.Entidades;

namespace Biblioteca.Negocio
{
    public class PrestamoNegocio
    {
        private PrestamoDatos datos = new PrestamoDatos();

        public List<Prestamo> Listar()
        {
            return datos.Listar();
        }

        // Métodos directos que llama FormPrestamos.cs
        public bool Registrar(Prestamo prestamo)
        {
            if (prestamo == null || prestamo.IdUsuario <= 0 || prestamo.IdLibro <= 0)
                return false;

            return datos.RegistrarPrestamo(prestamo);
        }

        public bool Devolver(int idPrestamo, DateTime fechaDevolucion)
        {
            if (idPrestamo <= 0)
                return false;

            return datos.RegistrarDevolucion(idPrestamo, fechaDevolucion);
        }

        // Alias por si alguna otra parte los invoca con el nombre largo
        public bool RegistrarPrestamo(Prestamo prestamo) => Registrar(prestamo);
        public bool RegistrarDevolucion(int idPrestamo, DateTime fechaDevolucion) => Devolver(idPrestamo, fechaDevolucion);
    }
}