using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    /// Controlador MVC encargado de la gestión de usuarios (Panel de Administración).
    /// Funciona exclusivamente con vistas Razor y peticiones síncronas para el CRUD.
    public class UsuariosController : Controller
    {
        // Instancias de los repositorios necesarios para consultar y guardar datos
        private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();
        private readonly CatalogoRepository _catalogoRepo = new CatalogoRepository();

        /// Petición GET: Carga la vista principal con la tabla de todos los usuarios registrados.
        /// <returns>Vista Index con la lista de objetos Usuario.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            // Validar seguridad: Solo usuarios autenticados pueden ver la lista
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");

            var listaUsuarios = _usuarioRepo.ObtenerTodos();
            return View(listaUsuarios);
        }

        /// Petición GET: Carga el formulario en blanco para registrar un nuevo usuario.
        /// <returns>Vista Crear con el ViewBag cargado con los roles disponibles.</returns>
        [HttpGet]
        public ActionResult Crear()
        {
            // Validar seguridad: Solo usuarios autenticados pueden acceder al formulario
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");

            // Se envían los roles desde la BD a la vista para poblar el <select> de Roles
            ViewBag.Roles = _catalogoRepo.ObtenerRoles();
            return View();
        }

        /// Petición POST: Recibe los datos del formulario, los valida y registra el usuario en SQL.
        /// <param name="usuario">Modelo de Usuario poblado con los datos del formulario web.</param>
        /// <returns>Redirección al Index en caso de éxito, o recarga del formulario mostrando errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Usuario usuario)
        {
            // Validar que la sesión no haya expirado justo antes de guardar
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                // Si faltan datos, debemos recargar los roles en el ViewBag antes de devolver la vista,
                // de lo contrario, el <select> de roles aparecerá vacío y arrojará error.
                ViewBag.Roles = _catalogoRepo.ObtenerRoles();
                return View(usuario);
            }

            // Para fines prácticos de esta entrega, el valor se pasa directo. 
            // En un entorno de producción, aquí se aplicaría un algoritmo Hash (ej. SHA-256)
            // ej: usuario.ClaveHash = Encriptar(usuario.ClaveHash);
            usuario.ClaveHash = usuario.ClaveHash;

            // Se envía el objeto validado al repositorio para ejecutar el INSERT en la base de datos
            _usuarioRepo.Insertar(usuario);

            // Retorna a la tabla de usuarios para ver el nuevo registro reflejado
            return RedirectToAction("Index");
        }
    }
}