using System.Web.Http;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    // API RESTful del módulo Solicitudes.
    // Gestiona listar, buscar, crear, actualizar estado y eliminar solicitudes.
    [RoutePrefix("api/solicitudes")]
    public class SolicitudesApiController : ApiController
    {
        // Repositorio que contiene las consultas SQL de solicitudes.
        private readonly SolicitudRepository _repo = new SolicitudRepository();

        #region GET: listar solicitudes

        [HttpGet]
        [Route("")]
        public IHttpActionResult ObtenerTodas()
        {
            // Obtiene todas las solicitudes desde la base de datos.
            var solicitudes = _repo.ObtenerTodas();

            // Devuelve la lista en formato JSON.
            return Ok(solicitudes);
        }

        #endregion

        #region GET: buscar solicitud por Id

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult ObtenerPorId(int id)
        {
            // Busca una solicitud específica por su Id.
            var solicitud = _repo.ObtenerPorId(id);

            // Si no existe, devuelve error 404.
            if (solicitud == null)
            {
                return NotFound();
            }

            // Devuelve la solicitud encontrada en formato JSON.
            return Ok(solicitud);
        }

        #endregion

        #region POST: crear solicitud

        [HttpPost]
        [Route("")]
        public IHttpActionResult Crear(SolicitudSoporte solicitud)
        {
            // Revisa que JavaScript haya enviado datos.
            if (solicitud == null)
            {
                return BadRequest("No se recibieron datos de la solicitud.");
            }

            // Revisa las validaciones del modelo.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Inserta la solicitud en la base de datos.
            bool creada = _repo.Insertar(solicitud);

            // Si no se pudo guardar, devuelve error.
            if (!creada)
            {
                return BadRequest("No se pudo registrar la solicitud.");
            }

            // Devuelve mensaje de éxito para JavaScript.
            return Ok(new { mensaje = "Solicitud creada exitosamente." });
        }

        #endregion

        #region PUT: actualizar estado

        [HttpPut]
        [Route("{id:int}/estado")]
        public IHttpActionResult ActualizarEstado(int id, [FromBody] ActualizarEstadoRequest request)
        {
            // Revisa que el cuerpo de la petición tenga datos.
            if (request == null)
            {
                return BadRequest("No se recibió el nuevo estado.");
            }

            // Revisa las validaciones del modelo.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Actualiza solo el estado de la solicitud.
            bool actualizado = _repo.ActualizarEstado(id, request.IdEstado);

            // Si no encontró la solicitud, devuelve error 404.
            if (!actualizado)
            {
                return NotFound();
            }

            // Devuelve mensaje de éxito para JavaScript.
            return Ok(new { mensaje = "Estado actualizado correctamente." });
        }

        #endregion

        #region DELETE: eliminar solicitud

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Eliminar(int id)
        {
            // Elimina la solicitud según el Id recibido.
            bool eliminado = _repo.Eliminar(id);

            // Si no encontró la solicitud, devuelve error 404.
            if (!eliminado)
            {
                return NotFound();
            }

            // Devuelve mensaje de éxito para JavaScript.
            return Ok(new { mensaje = "Solicitud eliminada correctamente." });
        }

        #endregion
    }
}