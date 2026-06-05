using System.Linq;
using System.Web.Mvc;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    // Controlador del dashboard principal.
    // Prepara métricas diferentes según el rol del usuario conectado.
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

            // Datos de sesión usados para adaptar el dashboard.
            int usuarioId = (int)Session["UsuarioId"];
            string rol = Session["Rol"] != null ? Session["Rol"].ToString() : "";

            // Se obtienen todas las solicitudes desde la base.
            var todasLasSolicitudes = _solicitudRepo.ObtenerTodas();

            // Lista que se usará para calcular las métricas del dashboard.
            var solicitudesDashboard = todasLasSolicitudes;

            // Administrador y Soporte ven todas las solicitudes.
            // Usuario normal solo ve sus propias solicitudes.
            if (rol != "Administrador" && rol != "Soporte")
            {
                solicitudesDashboard = todasLasSolicitudes
                    .Where(s => s.UsuarioId == usuarioId)
                    .ToList();
            }

            // Métricas comunes para todos los roles.
            ViewBag.TotalSolicitudes = solicitudesDashboard.Count;
            ViewBag.Pendientes = solicitudesDashboard.Count(s => s.IdEstado == 1 || s.NombreEstado == "Pendiente");
            ViewBag.EnProceso = solicitudesDashboard.Count(s => s.IdEstado == 2 || s.NombreEstado == "En proceso");
            ViewBag.Resueltas = solicitudesDashboard.Count(s => s.IdEstado == 3 || s.NombreEstado == "Resuelto");

            // Solo el administrador ve cantidad de usuarios activos.
            if (rol == "Administrador")
            {
                ViewBag.TotalUsuarios = _usuarioRepo.ObtenerTodos().Count(u => u.Estado);
            }

            // Variables para adaptar el texto y los accesos rápidos.
            ViewBag.EsAdministrador = rol == "Administrador";
            ViewBag.EsSoporte = rol == "Soporte";
            ViewBag.EsUsuario = rol == "Usuario";

            // Muestra la vista principal.
            return View();
        }

        #endregion
    }
}