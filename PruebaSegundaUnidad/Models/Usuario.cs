using System;

namespace PruebaSegundaUnidad.Models
{
    /// Representa la entidad principal de un usuario dentro del sistema.
    /// Contiene los datos persistentes de la base de datos y se utiliza para 
    /// gestionar la sesión, la autorización por roles y la visualización del perfil.
    public class Usuario
    {
        /// Identificador único del usuario (Llave Primaria).
        public int Id { get; set; }

        /// Identificador del rol asignado (Llave Foránea).
        public int RolId { get; set; }

        /// Propiedad auxiliar (no existe como columna directa en la tabla Usuarios).
        /// Almacena el nombre descriptivo del rol resultante de un JOIN con la tabla Roles
        /// para facilitar la visualización en la interfaz.
        public string NombreRol { get; set; }

        /// Nombre real de la persona. Se utiliza para el autocompletado en 
        /// las solicitudes de soporte (Fase 4).
        public string NombreCompleto { get; set; }

        /// Credencial de acceso (Username) única en el sistema.
        public string NombreUsuario { get; set; }

        /// Correo electrónico corporativo o personal, único por usuario.
        public string Correo { get; set; }

        /// Contraseña encriptada. Nunca debe viajar hacia la vista (frontend) 
        /// por motivos de seguridad; solo se utiliza en procesos de backend.
        public string ClaveHash { get; set; }

        /// Indicador de acceso (Activo/Inactivo). Si es 'false', el usuario 
        /// no podrá iniciar sesión en el sistema.
        public bool Estado { get; set; }

        /// Fecha y hora exacta en la que se creó la cuenta en la base de datos.
        public DateTime FechaRegistro { get; set; }
    }
}