using System.Web.Mvc;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    // Controlador MVC del módulo Solicitudes.
    // Este controlador solo carga la vista Razor.
    // El registro, listado, edición y eliminación se hacen en la API.
    public class SolicitudesController : Controller
    {
        // Repositorio usado para llenar los select del formulario.
        private readonly CatalogoRepository _catalogoRepo = new CatalogoRepository();

        #region Vista principal

        [HttpGet]
        public ActionResult Index()
        {
            // Antes: si alguien escribía la URL directa, podía intentar entrar a la vista.
            // Ahora: primero se revisa si existe sesión activa.
            if (Session["UsuarioId"] == null)
            {
                // Si no hay sesión, se devuelve al login.
                return RedirectToAction("Login", "Auth");
            }

            // Antes: los select podían quedar fijos o escritos manualmente en la vista.
            // Ahora: las áreas se cargan desde la base usando CatalogoRepository.
            ViewBag.Areas = _catalogoRepo.ObtenerAreas();

            // Antes: los tipos de problema también podían quedar escritos a mano.
            // Ahora: se cargan desde la base para mantenerlos dinámicos.
            ViewBag.TiposProblema = _catalogoRepo.ObtenerTiposProblema();

            // Muestra la vista Views/Solicitudes/Index.cshtml.
            return View();
        }

        #endregion
    }
}