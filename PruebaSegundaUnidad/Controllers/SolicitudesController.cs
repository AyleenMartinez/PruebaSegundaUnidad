using System.Web.Mvc;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    // Controlador MVC del módulo Solicitudes.
    // Este controlador solo carga la vista Razor.
    // El registro, listado, actualización y eliminación se trabajan desde la API.
    public class SolicitudesController : Controller
    {
        // Repositorio usado para cargar catálogos del formulario.
        private readonly CatalogoRepository _catalogoRepo = new CatalogoRepository();

        #region Vista principal

        [HttpGet]
        public ActionResult Index()
        {
            // Se valida que exista una sesión activa antes de mostrar la vista.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Se cargan las áreas desde la base para llenar el select del formulario.
            ViewBag.Areas = _catalogoRepo.ObtenerAreas();

            // Se cargan los tipos de problema desde la base para llenar el select del formulario.
            ViewBag.TiposProblema = _catalogoRepo.ObtenerTiposProblema();

            // Muestra la vista Views/Solicitudes/Index.cshtml.
            return View();
        }

        #endregion
    }
}