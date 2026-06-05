using System.Linq;
using System.Web.Mvc;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    // Controlador del dashboard principal.
    // Muestra el inicio del sistema y prepara métricas para Administrador y Soporte.
    public class HomeController : Controller
    {
        // Repositorio para consultar solicitudes de soporte.
        private readonly SolicitudRepository _solicitudRepo = new SolicitudRepository();

        // Repositorio para consultar usuarios.
        private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();

        #region Dashboard principal

        [HttpGet]
        public ActionResult Index()
        {
            // Si no hay sesión activa, se devuelve al login.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Solo Administrador y Soporte ven métricas generales del sistema.
            if (Session["Rol"] != null &&
                (Session["Rol"].ToString() == "Administrador" || Session["Rol"].ToString() == "Soporte"))
            {
                // Se obtienen todas las solicitudes para calcular los indicadores del dashboard.
                var todasLasSolicitudes = _solicitudRepo.ObtenerTodas();

                // Se cuentan solicitudes según su estado.
                ViewBag.Pendientes = todasLasSolicitudes.Count(s => s.NombreEstado == "Pendiente");
                ViewBag.EnProceso = todasLasSolicitudes.Count(s => s.NombreEstado == "En proceso");
                ViewBag.Resueltas = todasLasSolicitudes.Count(s => s.NombreEstado == "Resuelto");

                // Se cuentan solo usuarios activos.
                ViewBag.TotalUsuarios = _usuarioRepo.ObtenerTodos().Count(u => u.Estado == true);
            }

            // Se muestra la vista principal.
            return View();
        }

        #endregion
    }
}