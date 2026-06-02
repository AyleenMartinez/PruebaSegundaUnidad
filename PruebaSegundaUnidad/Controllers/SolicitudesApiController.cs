using System.Web.Http;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    // API RESTful del módulo Solicitudes.
    // Aquí van las acciones GET, POST, PUT y DELETE.
    // Esta API se usa solo para solicitudes de soporte, no para login ni perfil.
    [RoutePrefix("api/solicitudes")]
    public class SolicitudesApiController : ApiController
    {
        // Repositorio que se comunica con la base de datos.
        private readonly SolicitudRepository _repo = new SolicitudRepository();

        #region GET: listar solicitudes

        [HttpGet]
        [Route("")]
        public IHttpActionResult ObtenerTodas()
        {
            // Antes: la vista podía necesitar datos cargados manualmente.
            // Ahora: la API entrega todas las solicitudes en formato JSON.
            var solicitudes = _repo.ObtenerTodas();

            // Devuelve la lista como JSON.
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

            // Antes: si no existía el Id, podía no quedar claro qué pasó.
            // Ahora: si no existe, la API responde 404 NotFound.
            if (solicitud == null)
            {
                return NotFound();
            }

            // Si existe, devuelve la solicitud como JSON.
            return Ok(solicitud);
        }

        #endregion

        #region POST: crear solicitud

        [HttpPost]
        [Route("")]
        public IHttpActionResult Crear(SolicitudSoporte solicitud)
        {
            // Antes: se asumía que el JSON llegaba bien desde JavaScript.
            // Ahora: primero se revisa si llegó vacío para evitar errores raros.
            if (solicitud == null)
            {
                return BadRequest("No se recibieron datos de la solicitud.");
            }

            // Antes: se podía intentar guardar aunque faltaran datos.
            // Ahora: se revisan las validaciones del modelo antes de insertar.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Antes: Insertar podía ser void y la API no sabía si realmente guardó.
            // Ahora: Insertar devuelve true o false según si se insertó una fila.
            bool creada = _repo.Insertar(solicitud);

            // Si no se insertó ninguna fila, se informa error.
            if (!creada)
            {
                return BadRequest("No se pudo registrar la solicitud.");
            }

            // Mensaje que puede leer el JavaScript.
            return Ok(new { mensaje = "Solicitud creada exitosamente." });
        }

        #endregion

        #region PUT: actualizar estado

        [HttpPut]
        [Route("{id:int}/estado")]
        public IHttpActionResult ActualizarEstado(int id, [FromBody] ActualizarEstadoRequest request)
        {
            // Antes: se asumía que el cuerpo del PUT venía con datos.
            // Ahora: si el JSON viene vacío, se devuelve error claro.
            if (request == null)
            {
                return BadRequest("No se recibió el nuevo estado.");
            }

            // Antes: podía llegar cualquier número de estado.
            // Ahora: el modelo valida que el estado sea válido.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Actualiza solo el estado de la solicitud.
            bool actualizado = _repo.ActualizarEstado(id, request.IdEstado);

            // Antes: si el Id no existía, podía parecer que no pasaba nada.
            // Ahora: si no encontró la solicitud, devuelve 404.
            if (!actualizado)
            {
                return NotFound();
            }

            // Mensaje que puede leer el JavaScript.
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

            // Antes: si el Id no existía, no siempre quedaba claro.
            // Ahora: si no eliminó nada, responde 404.
            if (!eliminado)
            {
                return NotFound();
            }

            // Mensaje que puede leer el JavaScript.
            return Ok(new { mensaje = "Solicitud eliminada correctamente." });
        }

        #endregion
    }
}