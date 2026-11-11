namespace Biblioteca.Entidades
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Documento { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }

        public Usuario() { }

        public Usuario(int idUsuario, string documento, string nombre, string apellido, string telefono, string correo)
        {
            IdUsuario = idUsuario;
            Documento = documento;
            Nombre = nombre;
            Apellido = apellido;
            Telefono = telefono;
            Correo = correo;
        }
    }
}
