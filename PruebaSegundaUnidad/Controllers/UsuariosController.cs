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

        [HttpGet]
        public ActionResult CambiarEstado(int id)
        {
            // Validar seguridad
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");

            // Llama al método de tu repositorio para invertir el estado actual del usuario
            _usuarioRepo.CambiarEstado(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Editar(int? id) // <-- Agregamos el signo de interrogación (int?)
        {
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");

            // Si por algún motivo la URL no tiene ID (ej. /Usuarios/Editar/), lo devolvemos a la tabla
            if (id == null) return RedirectToAction("Index");

            // Buscamos usando id.Value
            var usuario = _usuarioRepo.ObtenerPorId(id.Value);
            if (usuario == null) return HttpNotFound();

            ViewBag.Roles = _catalogoRepo.ObtenerRoles();
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Usuario usuario)
        {
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");

            ModelState.Remove("ClaveHash");

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _catalogoRepo.ObtenerRoles();
                return View(usuario);
            }

            _usuarioRepo.Actualizar(usuario);
            return RedirectToAction("Index");
        }

        // De paso, protegemos el método CambiarEstado con la misma lógica
        [HttpGet]
        public ActionResult CambiarEstado(int? id)
        {
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");

            // Protegemos contra nulos
            if (id == null) return RedirectToAction("Index");

            _usuarioRepo.CambiarEstado(id.Value);

            return RedirectToAction("Index");
        }
    }
}