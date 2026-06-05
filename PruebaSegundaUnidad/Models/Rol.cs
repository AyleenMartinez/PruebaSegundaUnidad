namespace PruebaSegundaUnidad.Models
{
    // Representa un rol del sistema.
    // Se usa para controlar permisos y mostrar opciones en formularios.
    public class Rol
    {
        // Id del rol en la base de datos.
        // Ejemplo: 1, 2 o 3.
        public int Id { get; set; }

        // Nombre del rol.
        // Ejemplo: Administrador, Soporte o Usuario.
        public string NombreRol { get; set; }
    }
}