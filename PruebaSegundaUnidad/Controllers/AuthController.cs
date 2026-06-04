using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    // Controlador encargado del inicio y cierre de sesión.
    // Este módulo funciona con MVC y Razor, sin API.
    public class AuthController : Controller
    {
        // Repositorio que consulta usuarios y roles en la base de datos.
        private readonly AuthRepository authRepository = new AuthRepository();

        #region Login

        [HttpGet]
        public ActionResult Login()
        {
            // Si ya hay sesión activa, se envía al dashboard principal.
            if (Session["UsuarioId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Muestra la vista de login.
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel modelo)
        {
            // Si no escribió usuario/correo, se deja que el modelo muestre sus validaciones.
            if (string.IsNullOrWhiteSpace(modelo.UsuarioOCorreo))
            {
                return View(modelo);
            }

            // Limpia espacios para evitar errores por escribir con espacio al inicio o final.
            modelo.UsuarioOCorreo = modelo.UsuarioOCorreo.Trim();

            // Primero se busca la cuenta por usuario o correo.
            var usuario = authRepository.ObtenerPorUsuarioOCorreo(modelo.UsuarioOCorreo);

            // Si no existe la cuenta, se muestra error general.
            if (usuario == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View(modelo);
            }

            // Si la cuenta existe pero está inactiva, se muestra mensaje específico.
            // Esta validación va antes de revisar la contraseña para informar claramente el estado de la cuenta.
            if (!usuario.Estado)
            {
                ViewBag.Error = "La cuenta se encuentra inactiva. Comuníquese con un administrador.";
                return View(modelo);
            }

            // Revisa las validaciones del formulario.
            // Esto valida, por ejemplo, que la contraseña no venga vacía.
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // Si la cuenta está activa, se revisa la contraseña.
            // En esta versión académica el campo ClaveHash guarda texto simple.
            if (usuario.ClaveHash != modelo.Clave)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View(modelo);
            }

            // Se guardan datos importantes del usuario conectado en Session.
            Session["UsuarioId"] = usuario.Id;
            Session["NombreCompleto"] = usuario.NombreCompleto;
            Session["Correo"] = usuario.Correo;
            Session["Rol"] = usuario.NombreRol;

            // Si todo está correcto, entra al dashboard principal.
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Cierre de sesión

        public ActionResult Logout()
        {
            // Limpia los datos guardados en sesión.
            Session.Clear();

            // Finaliza la sesión actual.
            Session.Abandon();

            // Vuelve al login.
            return RedirectToAction("Login", "Auth");
        }

        #endregion
    }
}