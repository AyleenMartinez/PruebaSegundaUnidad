using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    // Controlador MVC encargado de administrar usuarios del sistema.
    // Permite listar, crear, editar y cambiar el estado de un usuario.
    public class UsuariosController : Controller
    {
        // Repositorio que contiene las consultas relacionadas con la tabla Usuarios.
        private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();

        // Repositorio usado para cargar datos de apoyo, como los roles.
        private readonly CatalogoRepository _catalogoRepo = new CatalogoRepository();

        #region Listado de usuarios

        [HttpGet]
        public ActionResult Index()
        {
            // Se valida que exista una sesión activa antes de mostrar la administración de usuarios.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Se obtiene la lista completa de usuarios registrados.
            var usuarios = _usuarioRepo.ObtenerTodos();

            // Se envía la lista de usuarios a la vista Index.
            return View(usuarios);
        }

        #endregion

        #region Crear usuario

        [HttpGet]
        public ActionResult Crear()
        {
            // Se valida que exista una sesión activa antes de permitir crear usuarios.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Se cargan los roles para mostrarlos en el formulario de creación.
            ViewBag.Roles = _catalogoRepo.ObtenerRoles();

            // Se muestra la vista Crear.
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Usuario usuario)
        {
            // Se valida que exista una sesión activa antes de guardar un nuevo usuario.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Se revisa si los datos ingresados cumplen las validaciones del modelo.
            if (!ModelState.IsValid)
            {
                // Se cargan nuevamente los roles para que el select no quede vacío al volver a la vista.
                ViewBag.Roles = _catalogoRepo.ObtenerRoles();

                // Se devuelve la vista con los datos ingresados y los mensajes de error.
                return View(usuario);
            }

            // Se inserta el nuevo usuario en la base de datos.
            _usuarioRepo.Insertar(usuario);

            // Se vuelve al listado principal de usuarios.
            return RedirectToAction("Index");
        }

        #endregion

        #region Editar usuario

        [HttpGet]
        public ActionResult Editar(int? id)
        {
            // Se valida que exista una sesión activa antes de editar usuarios.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Se valida que la URL tenga un Id de usuario.
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            // Se busca el usuario en la base de datos usando el Id recibido.
            var usuario = _usuarioRepo.ObtenerPorId(id.Value);

            // Si el usuario no existe, se devuelve error 404.
            if (usuario == null)
            {
                return HttpNotFound();
            }

            // Se cargan los roles para mostrarlos en el formulario de edición.
            ViewBag.Roles = _catalogoRepo.ObtenerRoles();

            // Se muestra la vista Editar con los datos del usuario encontrado.
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Usuario usuario)
        {
            // Se valida que exista una sesión activa antes de guardar cambios.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Este formulario no modifica contraseña, por eso se excluye ClaveHash de la validación.
            ModelState.Remove("ClaveHash");

            // Se revisa si los datos editados cumplen las validaciones del modelo.
            if (!ModelState.IsValid)
            {
                // Se cargan nuevamente los roles para que el formulario pueda mostrarse correctamente.
                ViewBag.Roles = _catalogoRepo.ObtenerRoles();

                // Se devuelve la vista con los datos ingresados y los errores.
                return View(usuario);
            }

            // Se actualizan los datos del usuario en la base de datos.
            _usuarioRepo.Actualizar(usuario);

            // Se vuelve al listado principal de usuarios.
            return RedirectToAction("Index");
        }

        #endregion

        #region Cambiar estado

        [HttpGet]
        public ActionResult CambiarEstado(int? id)
        {
            // Se valida que exista una sesión activa antes de cambiar el estado de un usuario.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Se valida que la URL tenga un Id de usuario.
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            // Se cambia el estado del usuario seleccionado.
            // Si está activo, pasa a inactivo; si está inactivo, pasa a activo.
            _usuarioRepo.CambiarEstado(id.Value);

            // Se vuelve al listado para ver el estado actualizado.
            return RedirectToAction("Index");
        }

        #endregion
    }
}