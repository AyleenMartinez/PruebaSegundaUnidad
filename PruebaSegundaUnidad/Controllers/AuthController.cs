using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    // Controlador encargado del inicio y cierre de sesión.
    // Este módulo funciona con MVC y Razor, sin usar API.
    public class AuthController : Controller
    {
        // Repositorio que consulta los usuarios y roles en la base de datos.
        private readonly AuthRepository authRepository = new AuthRepository();

        #region Login

        [HttpGet]
        public ActionResult Login()
        {
            // Si ya existe una sesión activa, no se muestra el login otra vez.
            if (Session["UsuarioId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Muestra la vista del formulario de inicio de sesión.
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel modelo)
        {
            // Revisa si el usuario/correo viene vacío.
            if (string.IsNullOrWhiteSpace(modelo.UsuarioOCorreo))
            {
                return View(modelo);
            }

            // Limpia espacios al inicio o final para evitar errores al buscar.
            modelo.UsuarioOCorreo = modelo.UsuarioOCorreo.Trim();

            // Busca la cuenta usando nombre de usuario o correo.
            var usuario = authRepository.ObtenerPorUsuarioOCorreo(modelo.UsuarioOCorreo);

            // Si no encuentra la cuenta, muestra un mensaje general.
            if (usuario == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View(modelo);
            }

            // Si la cuenta existe pero está inactiva, no permite el ingreso.
            if (!usuario.Estado)
            {
                ViewBag.Error = "La cuenta se encuentra inactiva. Comuníquese con un administrador.";
                return View(modelo);
            }

            // Revisa las validaciones del modelo, por ejemplo contraseña obligatoria.
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // En esta versión académica, ClaveHash se compara como texto simple.
            // En un sistema real debería compararse usando hash seguro.
            if (usuario.ClaveHash != modelo.Clave)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View(modelo);
            }

            // Guarda datos del usuario conectado para usarlos en las demás vistas.
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
            // Limpia todos los datos guardados en Session.
            Session.Clear();

            // Finaliza la sesión actual del usuario.
            Session.Abandon();

            // Redirige nuevamente al login.
            return RedirectToAction("Login", "Auth");
        }

        #endregion
    }
}