using System.ComponentModel.DataAnnotations;

namespace PruebaSegundaUnidad.Models
{
    /// ViewModel encargado de gestionar la vista del perfil del usuario autenticado.
    /// Transporta los datos básicos para visualización (solo lectura) y contiene las validaciones 
    /// estrictas necesarias para procesar el cambio de contraseña de forma segura.
    public class PerfilViewModel
    {
        /// Identificador interno del usuario. Generalmente se mantiene oculto en la vista 
        /// mediante un campo @Html.HiddenFor() para saber qué registro actualizar.
        public int UsuarioId { get; set; }

        /// Nombre completo del usuario, destinado a ser mostrado como texto informativo en la interfaz.
        public string NombreCompleto { get; set; }

        /// Correo electrónico del usuario, destinado a visualización en el perfil.
        public string Correo { get; set; }

        /// Contraseña vigente del usuario. Es requerida por el backend para verificar 
        /// la identidad antes de autorizar la modificación del hash en la base de datos.
        [Required(ErrorMessage = "La contraseña actual es obligatoria")]
        [DataType(DataType.Password)]
        public string ClaveActual { get; set; }

        /// Nueva contraseña propuesta por el usuario. Incluye validación de longitud mínima
        /// para cumplir con los estándares básicos de seguridad exigidos en la rúbrica.
        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        public string NuevaClave { get; set; }

        /// Campo de seguridad auxiliar. El atributo [Compare] instruye a MVC para que evalúe 
        /// automáticamente (tanto en frontend como en backend) que este valor sea idéntico a 'NuevaClave'.
        [Required(ErrorMessage = "Debe confirmar la nueva contraseña")]
        [DataType(DataType.Password)]
        [Compare("NuevaClave", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarClave { get; set; }
    }
}