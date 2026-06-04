using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    // Controlador MVC encargado de administrar usuarios.
    // Permite listar, crear, editar y cambiar estado.
    public class UsuariosController : Controller
    {
        // Repositorio de consultas SQL para usuarios.
        private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();

        // Repositorio para cargar catálogos, como roles.
        private readonly CatalogoRepository _catalogoRepo = new CatalogoRepository();

        #region Seguridad del módulo

        // Revisa si hay una sesión activa.
        private bool HaySesionActiva()
        {
            return Session["UsuarioId"] != null;
        }

        // Revisa si el usuario conectado es Administrador.
        private bool UsuarioEsAdministrador()
        {
            return Session["Rol"] != null && Session["Rol"].ToString() == "Administrador";
        }

        // Protege el módulo de usuarios.
        private ActionResult ValidarAcceso()
        {
            if (!HaySesionActiva())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!UsuarioEsAdministrador())
            {
                return RedirectToAction("Index", "Home");
            }

            return null;
        }

        #endregion

        #region Listado de usuarios

        [HttpGet]
        public ActionResult Index()
        {
            // Se valida acceso al módulo.
            ActionResult acceso = ValidarAcceso();

            if (acceso != null)
            {
                return acceso;
            }

            // Se obtiene la lista completa de usuarios.
            var usuarios = _usuarioRepo.ObtenerTodos();

            return View(usuarios);
        }

        #endregion

        #region Crear usuario

        [HttpGet]
        public ActionResult Crear()
        {
            // Se valida acceso al módulo.
            ActionResult acceso = ValidarAcceso();

            if (acceso != null)
            {
                return acceso;
            }

            // Se cargan roles para el formulario.
            ViewBag.Roles = _catalogoRepo.ObtenerRoles();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Usuario usuario)
        {
            // Se valida acceso al módulo.
            ActionResult acceso = ValidarAcceso();

            if (acceso != null)
            {
                return acceso;
            }

            // Se limpian espacios innecesarios.
            if (usuario.NombreCompleto != null)
            {
                usuario.NombreCompleto = usuario.NombreCompleto.Trim();
            }

            if (usuario.NombreUsuario != null)
            {
                usuario.NombreUsuario = usuario.NombreUsuario.Trim();
            }

            if (usuario.Correo != null)
            {
                usuario.Correo = usuario.Correo.Trim();
            }

            // Se valida si el correo ya existe.
            if (!string.IsNullOrWhiteSpace(usuario.Correo) && _usuarioRepo.ExisteCorreo(usuario.Correo))
            {
                ModelState.AddModelError("Correo", "Este correo ya está registrado por otro usuario.");
            }

            // Se valida si el nombre de usuario ya existe.
            if (!string.IsNullOrWhiteSpace(usuario.NombreUsuario) && _usuarioRepo.ExisteNombreUsuario(usuario.NombreUsuario))
            {
                ModelState.AddModelError("NombreUsuario", "Este nombre de usuario ya está registrado.");
            }

            // Se revisan las validaciones del modelo.
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _catalogoRepo.ObtenerRoles();
                return View(usuario);
            }

            // Se inserta el usuario en la base.
            _usuarioRepo.Insertar(usuario);

            return RedirectToAction("Index");
        }

        #endregion

        #region Editar usuario

        [HttpGet]
        public ActionResult Editar(int? id)
        {
            // Se valida acceso al módulo.
            ActionResult acceso = ValidarAcceso();

            if (acceso != null)
            {
                return acceso;
            }

            // Se valida que venga un Id en la URL.
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            // Se busca el usuario.
            var usuario = _usuarioRepo.ObtenerPorId(id.Value);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            // Se cargan roles para el formulario.
            ViewBag.Roles = _catalogoRepo.ObtenerRoles();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Usuario usuario)
        {
            // Se valida acceso al módulo.
            ActionResult acceso = ValidarAcceso();

            if (acceso != null)
            {
                return acceso;
            }

            // Este formulario no cambia contraseña.
            ModelState.Remove("ClaveHash");

            // Se limpian espacios innecesarios.
            if (usuario.NombreCompleto != null)
            {
                usuario.NombreCompleto = usuario.NombreCompleto.Trim();
            }

            if (usuario.NombreUsuario != null)
            {
                usuario.NombreUsuario = usuario.NombreUsuario.Trim();
            }

            if (usuario.Correo != null)
            {
                usuario.Correo = usuario.Correo.Trim();
            }

            // Se revisa si el correo pertenece a otro usuario.
            if (!string.IsNullOrWhiteSpace(usuario.Correo) && _usuarioRepo.ExisteCorreo(usuario.Correo, usuario.Id))
            {
                ModelState.AddModelError("Correo", "Este correo ya está registrado por otro usuario.");
            }

            // Se revisa si el nombre de usuario pertenece a otro usuario.
            if (!string.IsNullOrWhiteSpace(usuario.NombreUsuario) && _usuarioRepo.ExisteNombreUsuario(usuario.NombreUsuario, usuario.Id))
            {
                ModelState.AddModelError("NombreUsuario", "Este nombre de usuario ya está registrado.");
            }

            // Se revisan las validaciones del modelo.
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _catalogoRepo.ObtenerRoles();
                return View(usuario);
            }

            // Se actualizan los datos del usuario.
            _usuarioRepo.Actualizar(usuario);

            return RedirectToAction("Index");
        }

        #endregion

        #region Cambiar estado

        [HttpGet]
        public ActionResult CambiarEstado(int? id)
        {
            // Se valida acceso al módulo.
            ActionResult acceso = ValidarAcceso();

            if (acceso != null)
            {
                return acceso;
            }

            // Se valida que venga un Id en la URL.
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            // Se cambia activo/inactivo.
            _usuarioRepo.CambiarEstado(id.Value);

            return RedirectToAction("Index");
        }

        #endregion
    }
}