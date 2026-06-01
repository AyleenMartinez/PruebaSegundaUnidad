using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    /// Controlador encargado de mostrar el perfil del usuario
    /// y permitir el cambio de contraseña desde la cuenta activa.
    public class PerfilController : Controller
    {
        // Instancia del repositorio para trabajar con datos del perfil en la base de datos.
        private readonly PerfilRepository _perfilRepo = new PerfilRepository();

        #region Vista del perfil

        /// Petición GET: muestra la pantalla Mi Perfil.
        /// <returns>Vista del perfil o redirección al login si no hay sesión.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            // Verifica si existe un usuario con sesión iniciada.
            if (Session["UsuarioId"] == null)
            {
                // Si no hay sesión, se devuelve al login.
                return RedirectToAction("Login", "Auth");
            }

            // Se crea el ViewModel que se enviará a la vista.
            var perfil = new PerfilViewModel
            {
                // Id del usuario guardado al iniciar sesión.
                UsuarioId = (int)Session["UsuarioId"],

                // Nombre completo del usuario conectado.
                NombreCompleto = Session["NombreCompleto"].ToString(),

                // Correo del usuario conectado.
                Correo = Session["Correo"].ToString()
            };

            // Se muestra la vista Index con los datos del perfil.
            return View(perfil);
        }

        #endregion

        #region Cambio de contraseña

        /// Petición POST: procesa el formulario de cambio de contraseña.
        /// <param name="modelo">Datos ingresados en el formulario de perfil.</param>
        /// <returns>Vista con errores o redirección al perfil con mensaje de éxito.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarClave(PerfilViewModel modelo)
        {
            // Se valida nuevamente que exista una sesión activa.
            if (Session["UsuarioId"] == null)
            {
                // Si no hay sesión, no se permite modificar la contraseña.
                return RedirectToAction("Login", "Auth");
            }

            // Se recupera el Id del usuario desde la sesión.
            modelo.UsuarioId = (int)Session["UsuarioId"];

            // Se vuelve a cargar el nombre porque no viene completo desde el formulario.
            modelo.NombreCompleto = Session["NombreCompleto"].ToString();

            // Se vuelve a cargar el correo para mostrarlo de nuevo en pantalla.
            modelo.Correo = Session["Correo"].ToString();

            // Revisa las validaciones del ViewModel:
            // campos obligatorios, largo mínimo y confirmación de contraseña.
            if (!ModelState.IsValid)
            {
                // Si hay errores, vuelve a la misma vista mostrando los mensajes.
                return View("Index", modelo);
            }

            // Se consulta en la base de datos si la contraseña actual es correcta.
            bool claveActualCorrecta = _perfilRepo.ValidarClaveActual(
                modelo.UsuarioId,
                modelo.ClaveActual
            );

            // Si la contraseña actual no coincide, no se permite actualizar.
            if (!claveActualCorrecta)
            {
                // El error se asocia directamente al campo ClaveActual.
                ModelState.AddModelError("ClaveActual", "La contraseña actual no es correcta.");

                // Vuelve a la vista con el error visible.
                return View("Index", modelo);
            }

            // Si la contraseña actual está correcta, se guarda la nueva contraseña.
            bool exito = _perfilRepo.ActualizarContrasena(
                modelo.UsuarioId,
                modelo.NuevaClave
            );

            // Si por alguna razón no se actualizó ninguna fila, se muestra error.
            if (!exito)
            {
                // Error general del formulario.
                ModelState.AddModelError("", "No se pudo actualizar la contraseña.");

                // Vuelve a la vista mostrando el error.
                return View("Index", modelo);
            }

            // TempData permite mostrar el mensaje después de redirigir.
            TempData["MensajeExito"] = "Contraseña actualizada correctamente.";

            // Redirige al GET de Index para no quedar en /Perfil/ActualizarClave.
            return RedirectToAction("Index");
        }

        #endregion
    }
}