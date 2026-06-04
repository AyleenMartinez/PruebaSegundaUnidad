using System;
using System.ComponentModel.DataAnnotations;

namespace PruebaSegundaUnidad.Models
{
    // Representa un usuario del sistema.
    // Se usa en login, gestión de usuarios, roles y perfil.
    public class Usuario
    {
        // Id principal del usuario en la base de datos.
        public int Id { get; set; }

        // Rol asociado al usuario.
        // Debe venir seleccionado desde el formulario.
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rol")]
        public int RolId { get; set; }

        // Nombre del rol que viene desde la tabla Roles.
        // No es una columna directa de Usuarios; se llena cuando se hace JOIN con Roles.
        public string NombreRol { get; set; }

        // Nombre completo de la persona.
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre completo no puede superar los 100 caracteres")]
        public string NombreCompleto { get; set; }

        // Nombre de usuario usado para iniciar sesión.
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede superar los 50 caracteres")]
        public string NombreUsuario { get; set; }

        // Correo del usuario.
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
        [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres")]
        public string Correo { get; set; }

        // Contraseña para acceder al sistema.
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(4, ErrorMessage = "La contraseña debe tener al menos 4 caracteres")]
        [DataType(DataType.Password)]
        public string ClaveHash { get; set; }

        // Indica si el usuario puede iniciar sesión.
        // true = Activo / false = Inactivo.
        public bool Estado { get; set; }

        // Fecha en que se creó el usuario.
        public DateTime FechaRegistro { get; set; }
    }
}