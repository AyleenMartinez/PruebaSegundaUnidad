using System.Web.Mvc;

namespace PruebaSegundaUnidad.Controllers
{
    /// Controlador principal que sirve como punto de entrada al sistema 
    /// una vez que el usuario ha superado la barrera de autenticación.
    public class HomeController : Controller
    {
        #region Inicio

        /// Petición GET: Carga el panel de control (Dashboard) o página de bienvenida.
        /// <returns>Vista principal o redirección al login si se intenta entrar sin permiso.</returns>
        public ActionResult Index()
        {
            // Control de acceso manual: 
            // Si alguien intenta escribir "/Home/Index" en la URL sin haber pasado
            // por el AuthController, la variable Session["UsuarioId"] estará vacía,
            // por lo que será expulsado a la pantalla de inicio de sesión.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Si la sesión es válida, se muestra la interfaz del sistema.
            return View();
        }

        #endregion
    }
}