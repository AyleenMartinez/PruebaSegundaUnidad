namespace PruebaSegundaUnidad.Models
{
    // Estas clases representan las entidades de solo lectura de la base de datos.
    // Se utilizarán principalmente para cargar las opciones en los formularios.

    /// Representa los departamentos o áreas de la institución que pueden generar una solicitud.
    /// (Ejemplo: Administración, Docencia, Laboratorio).
    public class AreaSolicitante
    {
        /// Identificador único del área (Llave Primaria).
        public int Id { get; set; }

        /// Nombre descriptivo del área solicitante.
        public string NombreArea { get; set; }
    }
    /// Establece los niveles de urgencia o SLA para la atención del ticket.
    /// (Ejemplo: Baja, Media, Alta).
    public class Prioridad
    {
        public int Id { get; set; }
        public string NivelPrioridad { get; set; }
    }

    /// Indica la fase actual en la que se encuentra la solicitud de soporte.
    /// (Ejemplo: Pendiente, En proceso, Resuelto).
    public class EstadoSolicitud
    {
        public int Id { get; set; }
        public string NombreEstado { get; set; }
    }
}