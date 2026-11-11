namespace Biblioteca.Entidades
{
    public class Autor
    {
        public int IdAutor { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Nacionalidad { get; set; }

        public Autor() { }

        public Autor(int idAutor, string nombre, string apellido, string nacionalidad)
        {
            IdAutor = idAutor;
            Nombre = nombre;
            Apellido = apellido;
            Nacionalidad = nacionalidad;
        }
    }
}
