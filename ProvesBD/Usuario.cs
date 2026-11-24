
namespace Modelo
{
    public class Usuario
    {
        private int id;
        private string email;
        private string nombre;
        private string password;
        private char tipo;
        private DateTime fechaRegistro;

        public Usuario(int id, string email, string nombre, string password, char tipo, DateTime fechaRegistro)
        {
            Id = id;
            Email = email;
            Nombre = nombre;
            Password = password;
            Tipo = tipo;
            FechaRegistro = fechaRegistro;
        }

        public int Id { get => id; set => id = value; }
        public string Email { get => email; set => email = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Password { get => password; set => password = value; }
        public char Tipo { get => tipo; set => tipo = value; }
        public DateTime FechaRegistro { get => fechaRegistro; set => fechaRegistro = value; }
    }

}
