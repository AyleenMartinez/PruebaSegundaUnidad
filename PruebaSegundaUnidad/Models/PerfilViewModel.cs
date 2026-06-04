using System.ComponentModel.DataAnnotations;

namespace PruebaSegundaUnidad.Models
{
    // ViewModel usado en la vista Mi Perfil.
    // Junta datos personales editables y campos para cambiar contraseña.
    public class PerfilViewModel
    {
        #region Datos personales

        // Id del usuario que tiene la sesión iniciada.
        public int UsuarioId { get; set; }

        // Nombre completo editable desde el perfil.
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre completo no puede superar los 100 caracteres")]
        public string NombreCompleto { get; set; }

        // Correo editable desde el perfil.
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
        [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres")]
        public string Correo { get; set; }

        #endregion

        #region Cambio de contraseña

        // Contraseña actual escrita por el usuario.
        [DataType(DataType.Password)]
        public string ClaveActual { get; set; }

        // Nueva contraseña que se quiere guardar.
        [MinLength(4, ErrorMessage = "La contraseña debe tener al menos 4 caracteres")]
        [DataType(DataType.Password)]
        public string NuevaClave { get; set; }

        // Confirmación de la nueva contraseña.
        [DataType(DataType.Password)]
        [Compare("NuevaClave", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarClave { get; set; }

        #endregion
    }
}