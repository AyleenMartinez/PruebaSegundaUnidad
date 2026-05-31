using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    // Controlador del acceso al sistema.
    // Maneja login, datos de sesión y cierre de sesión.
    public class AuthController : Controller
    {
        private readonly AuthRepository authRepository = new AuthRepository();

        #region Login

        // Carga la pantalla de inicio de sesión.
        [HttpGet]
        public ActionResult Login()
        {
            // Si ya hay sesión activa, no tiene sentido mostrar otra vez el login.
            if (Session["UsuarioId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Recibe los datos escritos en el formulario de login.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel modelo)
        {
            // Si faltó usuario o contraseña, vuelve al login con los mensajes de validación.
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // Se consulta la BD usando correo o nombre de usuario.
            var usuario = authRepository.ValidarLogin(modelo.UsuarioOCorreo, modelo.Clave);

            // Si no encontró usuario válido, se muestra error en la misma pantalla.
            if (usuario == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View(modelo);
            }

            // Datos que quedan guardados para saber quién está usando el sistema.
            Session["UsuarioId"] = usuario.Id;
            Session["NombreCompleto"] = usuario.NombreCompleto;
            Session["Correo"] = usuario.Correo;
            Session["Rol"] = usuario.NombreRol;

            // Cuando el login es correcto, se entra al inicio del sistema.
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Cierre de sesion

        // Cierra la sesión y devuelve al login.
        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Login", "Auth");
        }

        #endregion
    }
}