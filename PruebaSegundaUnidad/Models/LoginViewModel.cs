using System.ComponentModel.DataAnnotations;

namespace PruebaSegundaUnidad.Models
{
    // Modelo usado solo para recibir los datos del formulario de login.
    // No es una tabla completa de la base de datos.
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Debe ingresar correo o nombre de usuario")]
        public string UsuarioOCorreo { get; set; }

        [Required(ErrorMessage = "Debe ingresar la contraseña")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
    }
}