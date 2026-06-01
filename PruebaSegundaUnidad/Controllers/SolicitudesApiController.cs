using System.Web.Http;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    /// Controlador API RESTful para la gestión integral de solicitudes de soporte.
    /// Consume exclusivamente formato JSON y es invocado mediante peticiones AJAX/Fetch
    /// desde las vistas del cliente.
    [RoutePrefix("api/solicitudes")]
    public class SolicitudesApiController : ApiController
    {
        // Instancia del repositorio que contiene la lógica de conexión a la base de datos
        private readonly SolicitudRepository _repo = new SolicitudRepository();

        /// ENDPOINT: GET /api/solicitudes
        /// Obtiene el listado completo de todas las solicitudes de soporte registradas,
        /// incluyendo los nombres de las áreas, prioridades y estados vinculados.
        /// <returns>Retorna un código 200 (OK) junto con un array JSON de solicitudes.</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult ObtenerTodas()
        {
            var solicitudes = _repo.ObtenerTodas();
            return Ok(solicitudes);
        }

        /// ENDPOINT: POST /api/solicitudes
        /// Registra una nueva solicitud de soporte en el sistema.
        /// <param name="solicitud">Objeto JSON que debe contener todos los campos obligatorios del modelo SolicitudSoporte.</param>
        /// Retorna 200 (OK) con un mensaje de éxito, o 400 (Bad Request) si faltan datos 
        /// obligatorios (ej. Descripción muy corta o sin Área seleccionada).
        [HttpPost]
        [Route("")]
        public IHttpActionResult Crear(SolicitudSoporte solicitud)
        {
            // Evalúa automáticamente las reglas [Required] y [MinLength] del modelo
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Al insertar, el repositorio fuerza automáticamente el estado a "1" (Pendiente)
            _repo.Insertar(solicitud);

            // Retorna un objeto anónimo estructurado para que el frontend pueda leer 'response.mensaje'
            return Ok(new { mensaje = "Solicitud creada exitosamente." });
        }

        /// ENDPOINT: PUT /api/solicitudes/{id}/estado
        /// Actualiza de forma parcial una solicitud, modificando exclusivamente su estado actual.
        /// <param name="id">El identificador único de la solicitud en la URL.</param>
        /// <param name="request">Objeto JSON (DTO) que contiene únicamente la propiedad IdEstado.</param>
        /// <returns>Retorna 200 (OK) si se actualizó, 400 si el modelo es inválido, o 404 (Not Found) si no existe el ID.</returns>
        [HttpPut]
        [Route("{id}/estado")]
        public IHttpActionResult ActualizarEstado(int id, [FromBody] ActualizarEstadoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ejecuta el UPDATE en la base de datos
            bool actualizado = _repo.ActualizarEstado(id, request.IdEstado);

            if (actualizado)
            {
                return Ok(new { mensaje = "Estado actualizado." });
            }

            // Si el repositorio devuelve false, significa que el ID enviado no existe en la BD
            return NotFound();
        }

        /// ENDPOINT: DELETE /api/solicitudes/{id}
        /// Elimina permanentemente una solicitud de soporte del sistema.
        /// <param name="id">El identificador único de la solicitud a eliminar en la URL.</param>
        /// <returns>Retorna 200 (OK) si se eliminó correctamente, o 404 (Not Found) si el ID no existe.</returns>
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Eliminar(int id)
        {
            // Ejecuta el DELETE en la base de datos
            bool eliminado = _repo.Eliminar(id);

            if (eliminado)
            {
                return Ok(new { mensaje = "Solicitud eliminada." });
            }

            return NotFound();
        }
    }
}