namespace PruebaSegundaUnidad.Models
{
    // Representa un área o departamento que puede generar una solicitud.
    // Ejemplo: Administración, Docencia o Laboratorio.
    public class AreaSolicitante
    {
        // Id del área en la base de datos.
        public int Id { get; set; }

        // Nombre descriptivo del área.
        public string NombreArea { get; set; }
    }

    // Representa el nivel de urgencia de una solicitud.
    // Ejemplo: Baja, Media o Alta.
    public class Prioridad
    {
        // Id de la prioridad en la base de datos.
        public int Id { get; set; }

        // Nombre o nivel de la prioridad.
        public string NivelPrioridad { get; set; }
    }

    // Representa el estado actual de una solicitud.
    // Ejemplo: Pendiente, En proceso o Resuelto.
    public class EstadoSolicitud
    {
        // Id del estado en la base de datos.
        public int Id { get; set; }

        // Nombre descriptivo del estado.
        public string NombreEstado { get; set; }
    }
}