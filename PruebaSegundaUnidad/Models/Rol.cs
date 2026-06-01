namespace PruebaSegundaUnidad.Models
{
    /// Modelo de catálogo que representa los niveles de acceso dentro del sistema.
    /// Mapea directamente a la tabla 'Roles' de la base de datos.
    public class Rol
    {
        /// Identificador único del rol (Ejemplo: 1, 2, 3).
        /// Corresponde a la llave primaria de la base de datos.
        public int Id { get; set; }

        /// Nombre descriptivo del nivel de acceso.
        /// (Ejemplo: Administrador, Soporte, Usuario).
        public string NombreRol { get; set; }
    }
}