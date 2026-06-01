using System.Web.Mvc;
using System.Linq;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    public class HomeController : Controller
    {
        // Instanciamos los repositorios para leer los datos
        private readonly SolicitudRepository _solicitudRepo = new SolicitudRepository();
        private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();

        [HttpGet]
        public ActionResult Index()
        {
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");

            // Solo hacemos las consultas si el usuario es Admin o Soporte
            if (Session["Rol"]?.ToString() == "Administrador" || Session["Rol"]?.ToString() == "Soporte")
            {
                var todasLasSolicitudes = _solicitudRepo.ObtenerTodas();

                // Filtramos por el nombre del estado (o por el ID del estado)
                ViewBag.Pendientes = todasLasSolicitudes.Count(s => s.NombreEstado == "Pendiente");
                ViewBag.EnProceso = todasLasSolicitudes.Count(s => s.NombreEstado == "En proceso");
                ViewBag.Resueltas = todasLasSolicitudes.Count(s => s.NombreEstado == "Resuelto");

                ViewBag.TotalUsuarios = _usuarioRepo.ObtenerTodos().Count(u => u.Estado == true);
            }

            return View();
        }
    }
}