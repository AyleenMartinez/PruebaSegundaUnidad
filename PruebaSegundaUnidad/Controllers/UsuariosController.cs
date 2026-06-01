using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();
        private readonly CatalogoRepository _catalogoRepo = new CatalogoRepository();

        [HttpGet]
        public ActionResult Index()
        {
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");
            return View(_usuarioRepo.ObtenerTodos());
        }

        [HttpGet]
        public ActionResult Crear()
        {
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");
            ViewBag.Roles = _catalogoRepo.ObtenerRoles();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Usuario usuario)
        {
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _catalogoRepo.ObtenerRoles();
                return View(usuario);
            }
            _usuarioRepo.Insertar(usuario);
            return RedirectToAction("Index");
        }
    }
}