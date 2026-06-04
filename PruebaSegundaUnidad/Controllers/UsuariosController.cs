using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    // Controlador MVC encargado de administrar usuarios.
    // Permite listar, crear, editar y activar/desactivar usuarios.
    public class UsuariosController : Controller
    {
        // Repositorio que contiene las consultas SQL para usuarios.
        private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();

        // Repositorio que carga catálogos, como los roles del sistema.
        private readonly CatalogoRepository _catalogoRepo = new CatalogoRepository();

        #region Seguridad del módulo

        // Revisa si existe una sesión activa.
        private bool HaySesionActiva()
        {
            return Session["UsuarioId"] != null;
        }

        // Revisa si el usuario conectado tiene rol Administrador.
        private bool UsuarioEsAdministrador()
        {
            return Session["Rol"] != null && Session["Rol"].ToString() == "Administrador";
        }

        // Protege el módulo de usuarios.
        // Si no hay sesión, vuelve al login.
        // Si hay sesión, pero no es administrador, vuelve al dashboard.
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
            // Se valida que solo un administrador pueda entrar al listado de usuarios.
            ActionResult acceso = ValidarAcceso();

            if (acceso != null)
            {
                return acceso;
            }

            // Se obtiene la lista completa de usuarios desde el repositorio.
            var usuarios = _usuarioRepo.ObtenerTodos();

            // Se envía la lista a la vista Index.
            return View(usuarios);
        }

        #endregion

        #region Crear usuario

        [HttpGet]
        public ActionResult Crear()
        {
            // Se valida que solo un administrador pueda abrir el formulario de creación.
            ActionResult acceso = ValidarAcceso();

            if (acceso != null)
            {
                return acceso;
            }

            // Se cargan los roles para llenar el select del formulario.
            ViewBag.Roles = _catalogoRepo.ObtenerRoles();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Usuario usuario)
        {
            // Se valida que solo un administrador pueda guardar usuarios.
            ActionResult acceso = ValidarAcceso();

            if (acceso != null)
            {
                return acceso;
            }

            // Se limpian espacios innecesarios antes de validar o guardar.
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

            // Se revisa si el correo ya existe para evitar error por restricción UNIQUE.
            if (!string.IsNullOrWhiteSpace(usuario.Correo) && _usuarioRepo.ExisteCorreo(usuario.Correo))
            {
                ModelState.AddModelError("Correo", "Este correo ya está registrado por otro usuario.");
            }

            // Se revisa si el nombre de usuario ya existe.
            if (!string.IsNullOrWhiteSpace(usuario.NombreUsuario) && _usuarioRepo.ExisteNombreUsuario(usuario.NombreUsuario))
            {
                ModelState.AddModelError("NombreUsuario", "Este nombre de usuario ya está registrado.");
            }

            // Se revisan las validaciones del modelo Usuario.
            // Ejemplo: campos obligatorios, correo válido y contraseña mínima.
            if (!ModelState.IsValid)
            {
                // Se vuelven a cargar los roles porque la vista los necesita para el select.
                ViewBag.Roles = _catalogoRepo.ObtenerRoles();

                return View(usuario);
            }

            // Si todo está correcto, se inserta el usuario en la base de datos.
            _usuarioRepo.Insertar(usuario);

            // Vuelve al listado principal de usuarios.
            return RedirectToAction("Index");
        }

        #endregion

        #region Editar usuario

        [HttpGet]
        public ActionResult Editar(int? id)
        {
            // Se valida que solo un administrador pueda editar usuarios.
            ActionResult acceso = ValidarAcceso();

            if (acceso != null)
            {
                return acceso;
            }

            // Si la URL no trae Id, se vuelve al listado.
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            // Se busca el usuario por Id.
            var usuario = _usuarioRepo.ObtenerPorId(id.Value);

            // Si no existe, se devuelve error 404.
            if (usuario == null)
            {
                return HttpNotFound();
            }

            // Se cargan los roles para mostrar el select en la vista.
            ViewBag.Roles = _catalogoRepo.ObtenerRoles();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Usuario usuario)
        {
            // Se valida que solo un administrador pueda guardar cambios.
            ActionResult acceso = ValidarAcceso();

            if (acceso != null)
            {
                return acceso;
            }

            // Este formulario no cambia contraseña.
            // Por eso se quita ClaveHash del ModelState, ya que Usuario.cs la exige al crear.
            ModelState.Remove("ClaveHash");

            // Se limpian espacios innecesarios antes de validar o guardar.
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

            // Se revisa si el correo ya está usado por otro usuario distinto al editado.
            if (!string.IsNullOrWhiteSpace(usuario.Correo) && _usuarioRepo.ExisteCorreo(usuario.Correo, usuario.Id))
            {
                ModelState.AddModelError("Correo", "Este correo ya está registrado por otro usuario.");
            }

            // Se revisa si el nombre de usuario ya está usado por otro usuario distinto al editado.
            if (!string.IsNullOrWhiteSpace(usuario.NombreUsuario) && _usuarioRepo.ExisteNombreUsuario(usuario.NombreUsuario, usuario.Id))
            {
                ModelState.AddModelError("NombreUsuario", "Este nombre de usuario ya está registrado.");
            }

            // Se revisan las validaciones del modelo.
            if (!ModelState.IsValid)
            {
                // Se vuelven a cargar los roles para que la vista se pueda mostrar correctamente.
                ViewBag.Roles = _catalogoRepo.ObtenerRoles();

                return View(usuario);
            }

            // Se actualizan los datos del usuario.
            _usuarioRepo.Actualizar(usuario);

            // Vuelve al listado principal.
            return RedirectToAction("Index");
        }

        #endregion

        #region Cambiar estado

        [HttpGet]
        public ActionResult CambiarEstado(int? id)
        {
            // Se valida que solo un administrador pueda activar o desactivar usuarios.
            ActionResult acceso = ValidarAcceso();

            if (acceso != null)
            {
                return acceso;
            }

            // Si la URL no trae Id, se vuelve al listado.
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            // Se cambia el estado del usuario.
            // Si está activo pasa a inactivo; si está inactivo pasa a activo.
            _usuarioRepo.CambiarEstado(id.Value);

            return RedirectToAction("Index");
        }

        #endregion
    }
}