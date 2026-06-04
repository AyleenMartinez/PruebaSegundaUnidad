using System.ComponentModel.DataAnnotations;

namespace PruebaSegundaUnidad.Models
{
    // ViewModel usado por la vista de inicio de sesión.
    // Contiene solo los datos que necesita el formulario de login.
    public class LoginViewModel
    {
        // Permite ingresar correo o nombre de usuario en el mismo campo.
        [Required(ErrorMessage = "Debe ingresar correo o nombre de usuario")]
        public string UsuarioOCorreo { get; set; }

        // Contraseña escrita por el usuario en el formulario.
        [Required(ErrorMessage = "Debe ingresar la contraseña")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
    }
}