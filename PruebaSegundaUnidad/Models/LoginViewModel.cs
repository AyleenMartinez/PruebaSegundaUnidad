using System.ComponentModel.DataAnnotations;

namespace PruebaSegundaUnidad.Models
{
    /// ViewModel exclusivo para gestionar el flujo de autenticación.
    public class LoginViewModel
    {
        /// Credencial de acceso ingresada por el usuario. 
        /// Su diseño permite flexibilidad en el backend para buscar coincidencias 
        /// tanto en el campo 'Correo' como en el campo 'NombreUsuario'.
        [Required(ErrorMessage = "Debe ingresar correo o nombre de usuario")]
        public string UsuarioOCorreo { get; set; }

        /// Contraseña en texto plano capturada directamente desde el formulario.
        [Required(ErrorMessage = "Debe ingresar la contraseña")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
    }
}