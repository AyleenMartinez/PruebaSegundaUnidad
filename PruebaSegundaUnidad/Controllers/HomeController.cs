using System.Web.Mvc;

namespace PruebaSegundaUnidad.Controllers
{
    // Controlador de entrada al sistema después del login.
    // Por ahora solo usamos Index para mantener simple el flujo.
    public class HomeController : Controller
    {
        #region Inicio

        // Página principal después de iniciar sesión.
        public ActionResult Index()
        {
            // Si no hay usuario en sesión, se devuelve al login.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Si hay sesión activa, carga la vista de inicio.
            return View();
        }

        #endregion
    }
}