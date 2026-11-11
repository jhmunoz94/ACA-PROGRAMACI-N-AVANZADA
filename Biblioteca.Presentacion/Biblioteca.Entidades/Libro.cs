namespace Biblioteca.Entidades
{
    public class Libro
    {
        public int IdLibro { get; set; }
        public string CodigoISBN { get; set; }
        public string Titulo { get; set; }
        public int AnioPublicacion { get; set; }
        public int Stock { get; set; }
        public int IdAutor { get; set; }
        public int IdEditorial { get; set; }

        // Propiedades de navegación visual para la tabla
        public string NombreAutor { get; set; }
        public string NombreEditorial { get; set; }

        public Libro() { }
    }
}
