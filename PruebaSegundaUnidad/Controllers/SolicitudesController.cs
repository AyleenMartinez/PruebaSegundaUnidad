using System.Web.Mvc;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    /// Controlador MVC exclusivo para renderizar la interfaz gráfica (Vistas Razor) 
    /// del módulo de soporte. Todo el CRUD transaccional (Crear, Editar, Eliminar) 
    /// lo manejará la API RESTful de forma asíncrona mediante JavaScript.
    public class SolicitudesController : Controller
    {
        // Instancia del repositorio de lectura para obtener los datos de las tablas maestras.
        private readonly CatalogoRepository _catalogoRepo = new CatalogoRepository();

        /// Petición GET: Carga la página principal (dashboard) de la gestión de solicitudes.
        /// <returns>Vista HTML de la página de solicitudes o redirección al Login si no hay sesión activa.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            // Validación de seguridad: Impide el acceso a usuarios no autenticados por URL directa.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Pasamos los catálogos a la vista a través del diccionario ViewBag.
            // Esto permite armar las opciones de los <select> en el formulario de registro 
            // de manera dinámica justo en el momento en que se procesa la vista Razor.
            ViewBag.Areas = _catalogoRepo.ObtenerAreas();
            ViewBag.TiposProblema = _catalogoRepo.ObtenerTiposProblema();

            return View();
        }
    }
}