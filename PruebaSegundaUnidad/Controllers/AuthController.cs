using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    /// Controlador responsable de la seguridad y el acceso al sistema.
    /// Gestiona el login, las variables de sesión y el cierre de sesión.
    public class AuthController : Controller
    {
        // Instancia del repositorio para consultar usuarios en la base de datos.
        private readonly AuthRepository authRepository = new AuthRepository();

        #region Login

        /// Petición GET: muestra la pantalla de inicio de sesión.
        /// <returns>Vista de login o redirección al Home si ya existe sesión.</returns>
        [HttpGet]
        public ActionResult Login()
        {
            // Se revisa si ya existe un usuario autenticado.
            if (Session["UsuarioId"] != null)
            {
                // Si ya inició sesión, no necesita ver el login otra vez.
                return RedirectToAction("Index", "Home");
            }

            // Si no hay sesión, se muestra la vista Login.
            return View();
        }

        /// Petición POST: procesa el usuario/correo y contraseña del formulario.
        /// <param name="modelo">Datos ingresados por el usuario en el login.</param>
        /// <returns>Redirección al sistema o vista con error.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel modelo)
        {
            // Revisa las validaciones del LoginViewModel.
            if (!ModelState.IsValid)
            {
                // Si faltan datos, vuelve a mostrar el login con errores.
                return View(modelo);
            }

            // Se consulta la base de datos para validar las credenciales.
            var usuario = authRepository.ValidarLogin(modelo.UsuarioOCorreo, modelo.Clave);

            // Si usuario es null, significa que no se encontró coincidencia.
            if (usuario == null)
            {
                // Mensaje visible para credenciales incorrectas.
                ViewBag.Error = "Usuario o contraseña incorrectos";

                // Vuelve al login mostrando el mensaje.
                return View(modelo);
            }

            // Se guarda el Id del usuario en sesión.
            Session["UsuarioId"] = usuario.Id;

            // Se guarda el nombre completo para mostrarlo en otras vistas.
            Session["NombreCompleto"] = usuario.NombreCompleto;

            // Se guarda el correo para usarlo en el perfil.
            Session["Correo"] = usuario.Correo;

            // Se guarda el rol para controlar permisos o mostrar información.
            Session["Rol"] = usuario.NombreRol;

            // Si el login fue correcto, entra al panel principal.
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Cierre de sesión

        /// Cierra la sesión del usuario actual.
        /// <returns>Redirección a la pantalla de login.</returns>
        public ActionResult Logout()
        {
            // Elimina todas las variables guardadas en Session.
            Session.Clear();

            // Finaliza la sesión actual en el servidor.
            Session.Abandon();

            // Después de cerrar sesión, vuelve al login.
            return RedirectToAction("Login", "Auth");
        }

        #endregion
    }
}