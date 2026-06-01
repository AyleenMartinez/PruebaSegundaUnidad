using System.ComponentModel.DataAnnotations;

namespace PruebaSegundaUnidad.Models
{
    /// ViewModel encargado de gestionar la vista del perfil del usuario autenticado.
    /// Contiene los datos visibles del perfil y las validaciones para cambiar contraseña.
    public class PerfilViewModel
    {
        #region Datos visibles del perfil

        /// Id interno del usuario que inició sesión.
        public int UsuarioId { get; set; }

        /// Nombre completo del usuario, se muestra en la vista de perfil.
        public string NombreCompleto { get; set; }

        /// Correo del usuario, se muestra en la vista de perfil.
        public string Correo { get; set; }

        #endregion

        #region Datos para cambio de contraseña

        /// Contraseña actual del usuario.
        /// Se pide para confirmar que quien cambia la clave es el dueño de la cuenta.
        [Required(ErrorMessage = "La contraseña actual es obligatoria")]
        [DataType(DataType.Password)]
        public string ClaveActual { get; set; }

        /// Nueva contraseña que el usuario quiere guardar.
        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]

        // Se exige un mínimo de 6 caracteres para evitar claves demasiado simples
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]

        // Indica que este campo se debe tratar como contraseña en la vista
        [DataType(DataType.Password)]
        public string NuevaClave { get; set; }

        /// Confirmación de la nueva contraseña.
        [Required(ErrorMessage = "Debe confirmar la nueva contraseña")]

        // Indica que este campo también se debe mostrar como contraseña
        [DataType(DataType.Password)]

        // Compara este campo con NuevaClave y valida que sean iguales
        [Compare("NuevaClave", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarClave { get; set; }

        #endregion
    }
}