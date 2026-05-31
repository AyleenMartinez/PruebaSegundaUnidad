using System.Web.Mvc;

namespace PruebaSegundaUnidad.Controllers
{
    // Controlador de las paginas base del sistema.
    // Se deja separado del login y de solicitudes para mantener el orden MVC.
    public class HomeController : Controller
    {
        #region Inicio

        // Pagina principal despues de iniciar sesion.
        public ActionResult Index()
        {
            // Validacion simple para evitar entrar al sistema sin login.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Si hay sesion, carga la vista de inicio.
            return View();
        }

        #endregion

        #region Paginas informativas

        // Pagina informativa del sistema.
        public ActionResult About()
        {
            // Misma validacion para no dejar paginas internas abiertas sin sesion.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Message = "Sistema de solicitudes de soporte tecnico TechHelpWeb.";
            return View();
        }

        // Pagina de contacto o informacion adicional.
        public ActionResult Contact()
        {
            // Si alguien entra directo por URL, se manda primero al login.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Message = "Modulo de contacto del sistema.";
            return View();
        }

        #endregion
    }
}