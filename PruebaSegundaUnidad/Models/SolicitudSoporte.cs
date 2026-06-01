using System;
using System.ComponentModel.DataAnnotations;

namespace PruebaSegundaUnidad.Models
{
    /// Modelo principal que representa un ticket de soporte en el sistema.
    /// Combina los identificadores (Llaves foráneas) necesarios para la base de datos
    /// junto con los campos de texto descriptivos (Ej: NombreArea, NombreEstado) 
    /// para facilitar la renderización en las vistas JSON de la API.
    public class SolicitudSoporte
    {
        /// Identificador único de la solicitud (Llave Primaria).
        public int Id { get; set; }

        // --- Datos del Usuario Solicitante ---
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string CorreoUsuario { get; set; }

        // --- Datos de Clasificación ---
        [Required(ErrorMessage = "El área es obligatoria")]
        public int IdArea { get; set; }
        public string NombreArea { get; set; }

        [Required(ErrorMessage = "El tipo de problema es obligatorio")]
        public int IdTipoProblema { get; set; }
        public string DescripcionProblema { get; set; }

        public int IdPrioridad { get; set; }
        public string NivelPrioridad { get; set; }

        public int IdEstado { get; set; }
        public string NombreEstado { get; set; }

        /// Explicación detallada del problema. Validado desde el servidor para
        /// asegurar que el usuario ingrese información útil (mínimo 10 caracteres).
        [Required(ErrorMessage = "La descripción es obligatoria")]
        [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres")]
        public string Descripcion { get; set; }

        public DateTime FechaRegistro { get; set; }
    }

    /// DTO (Data Transfer Object) utilizado exclusivamente por la API RESTful (Fase 4).
    /// Su única función es recibir de forma segura el nuevo ID del estado durante 
    /// una petición PUT (Actualización parcial), evitando sobrecargar la red.
    public class ActualizarEstadoRequest
    {
        /// Nuevo estado que se asignará a la solicitud existente.
        [Required(ErrorMessage = "El ID del estado es obligatorio")]
        public int IdEstado { get; set; }
    }
}