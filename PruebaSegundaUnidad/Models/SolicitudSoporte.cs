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

        // Nombre del usuario, se muestra en la tabla.
        public string NombreUsuario { get; set; }

        // Correo del usuario, queda disponible si después se quiere mostrar o revisar.
        public string CorreoUsuario { get; set; }

        #endregion

        #region Clasificación de la solicitud

        // Antes: el área solo estaba como int y podía llegar en 0 si no se seleccionaba nada.
        // Ahora: Range obliga a que el usuario seleccione un área válida desde el formulario.
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un área")]
        public int IdArea { get; set; }

        // Nombre del área, viene desde la tabla AreasSolicitantes.
        public string NombreArea { get; set; }

        // Antes: el tipo de problema también podía llegar como 0.
        // Ahora: se valida que venga un Id válido antes de registrar la solicitud.
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de problema")]
        public int IdTipoProblema { get; set; }

        // Nombre del tipo de problema, viene desde la tabla TiposProblema.
        public string DescripcionProblema { get; set; }

        // Antes: la prioridad dependía solo del select de la vista.
        // Ahora: también se valida desde el modelo por seguridad.
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

        // Antes: la descripción se recibía desde el formulario.
        // Ahora: además se valida que no esté vacía y que tenga mínimo 10 caracteres.
        [Required(ErrorMessage = "La descripción es obligatoria")]
        [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres")]
        public string Descripcion { get; set; }

        // Fecha y hora en que se registró la solicitud.
        public DateTime FechaRegistro { get; set; }

        #endregion
    }

    // Modelo pequeño usado solo para actualizar el estado desde la API.
    public class ActualizarEstadoRequest
    {
        // Antes: el estado podía llegar con cualquier número.
        // Ahora: solo acepta 1, 2 o 3, que corresponden a los estados del sistema.
        [Range(1, 3, ErrorMessage = "Debe seleccionar un estado válido")]
        public int IdEstado { get; set; }
    }
}