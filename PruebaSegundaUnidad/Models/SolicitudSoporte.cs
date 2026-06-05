using System;
using System.ComponentModel.DataAnnotations;

namespace PruebaSegundaUnidad.Models
{
    // Modelo principal de una solicitud de soporte.
    // Se usa para mover datos entre la API, el repositorio y la vista.
    public class SolicitudSoporte
    {
        #region Datos principales

        // Id único de la solicitud.
        public int Id { get; set; }

        // Id del usuario que creó la solicitud.
        public int UsuarioId { get; set; }

        // Nombre del usuario que se muestra en la tabla.
        public string NombreUsuario { get; set; }

        // Correo del usuario asociado a la solicitud.
        public string CorreoUsuario { get; set; }

        #endregion

        #region Clasificación de la solicitud

        // Área seleccionada en el formulario.
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un área")]
        public int IdArea { get; set; }

        // Nombre del área, viene desde la tabla AreasSolicitantes.
        public string NombreArea { get; set; }

        // Tipo de problema seleccionado en el formulario.
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de problema")]
        public int IdTipoProblema { get; set; }

        // Nombre del tipo de problema, viene desde la tabla TiposProblema.
        public string DescripcionProblema { get; set; }

        // Prioridad seleccionada en el formulario.
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una prioridad")]
        public int IdPrioridad { get; set; }

        // Nombre de la prioridad, viene desde la tabla Prioridades.
        public string NivelPrioridad { get; set; }

        // Estado actual de la solicitud.
        public int IdEstado { get; set; }

        // Nombre del estado, viene desde la tabla EstadosSolicitud.
        public string NombreEstado { get; set; }

        #endregion

        #region Descripción y fecha

        // Descripción del problema escrito por el usuario.
        [Required(ErrorMessage = "La descripción es obligatoria")]
        [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres")]
        public string Descripcion { get; set; }

        // Fecha y hora en que se registró la solicitud.
        public DateTime FechaRegistro { get; set; }

        #endregion
    }

    // Modelo pequeño usado solo para actualizar el estado desde la API.
    // Evita enviar toda la solicitud cuando solo cambia el estado.
    public class ActualizarEstadoRequest
    {
        // Estado nuevo de la solicitud.
        // En el sistema se usan 1 = Pendiente, 2 = En proceso, 3 = Resuelto.
        [Range(1, 3, ErrorMessage = "Debe seleccionar un estado válido")]
        public int IdEstado { get; set; }
    }
}