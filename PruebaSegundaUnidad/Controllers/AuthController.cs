using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    /// Controlador responsable de la seguridad y el acceso al sistema.
    /// Gestiona el flujo de autenticación, la creación de variables de sesión 
    /// y la destrucción de las mismas al cerrar sesión.
    public class AuthController : Controller
    {
        // Instancia del repositorio para separar la lógica de base de datos del controlador.
        private readonly AuthRepository authRepository = new AuthRepository();

        #region Login

        /// Petición GET: Renderiza la vista de inicio de sesión.
        /// <returns>Vista HTML del login o redirección al Home si ya existe sesión.</returns>
        [HttpGet]
        public ActionResult Login()
        {
            // Verificación preventiva: Si el usuario ya está autenticado, 
            // se salta la pantalla de login y va directo al sistema.
            if (Session["UsuarioId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// Petición POST: Procesa las credenciales enviadas desde el formulario web.
        /// <param name="modelo">Objeto que contiene el usuario/correo y la contraseña.</param>
        /// <returns>Redirección al sistema en caso de éxito, o recarga de la vista con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Medida de seguridad obligatoria contra ataques de falsificación de peticiones
        public ActionResult Login(LoginViewModel modelo)
        {
            // Verifica que los atributos [Required] del modelo se hayan cumplido.
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // Ejecuta la consulta a la base de datos a través del repositorio.
            var usuario = authRepository.ValidarLogin(modelo.UsuarioOCorreo, modelo.Clave);

            // Si la consulta devuelve null, las credenciales no coinciden o el usuario está inactivo.
            if (usuario == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View(modelo);
            }

            // Se almacenan datos clave en memoria para evitar consultar la BD 
            // constantemente en cada página que el usuario visite.
            // ====================================================================
            Session["UsuarioId"] = usuario.Id;
            Session["NombreCompleto"] = usuario.NombreCompleto;
            Session["Correo"] = usuario.Correo;
            Session["Rol"] = usuario.NombreRol;

            // Autenticación exitosa: Redirige al panel principal.
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Cierre de sesion

        /// Petición GET/POST: Destruye los datos del usuario autenticado y bloquea el acceso.
        /// <returns>Redirección a la pantalla de login.</returns>
        public ActionResult Logout()
        {
            // Elimina todas las variables guardadas en Session["..."]
            Session.Clear();

            // Alternativa extra segura (opcional): Session.Abandon();

            return RedirectToAction("Login", "Auth");
        }

        #endregion
    }
}