using System.ComponentModel.DataAnnotations;

namespace PruebaSegundaUnidad.Models
{
    /// ViewModel encargado de gestionar la vista del perfil del usuario autenticado.
    /// Contiene datos personales editables y datos para cambio de contraseña.
    public class PerfilViewModel
    {
        #region Datos editables del perfil

        /// Id interno del usuario que inició sesión.
        public int UsuarioId { get; set; }

        /// Antes: el nombre solo se mostraba como texto.
        /// Ahora: también se puede editar y validar desde la vista de perfil.
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre completo no puede superar los 100 caracteres")]
        public string NombreCompleto { get; set; }

        /// Antes: el correo solo se mostraba como texto.
        /// Ahora: también se puede editar y validar con formato de correo.
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
        [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres")]
        public string Correo { get; set; }

        #endregion

        #region Datos para cambio de contraseña

        /// Contraseña actual del usuario.
        /// Antes: se validaba con [Required].
        /// Ahora: se valida manualmente en el controlador para permitir que el formulario de datos personales funcione aparte.
        [DataType(DataType.Password)]
        public string ClaveActual { get; set; }

        /// Nueva contraseña que el usuario quiere guardar.
        /// Se mantiene MinLength para apoyar la validación, pero también se revisa en el controlador.
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        public string NuevaClave { get; set; }

        /// Confirmación de la nueva contraseña.
        /// Se compara con NuevaClave cuando se intenta cambiar la contraseña.
        [DataType(DataType.Password)]
        [Compare("NuevaClave", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarClave { get; set; }

        #endregion
    }
}