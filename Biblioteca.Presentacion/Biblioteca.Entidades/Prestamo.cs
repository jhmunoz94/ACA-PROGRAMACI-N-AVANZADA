using System;

namespace Biblioteca.Entidades
{
    public class Prestamo
    {
        public int IdPrestamo { get; set; }
        public int IdUsuario { get; set; }
        public int IdLibro { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string Estado { get; set; }

        // Propiedades de soporte para mostrar nombres en la grilla
        public string NombreUsuario { get; set; }
        public string TituloLibro { get; set; }

        public Prestamo() { }
    }
}
