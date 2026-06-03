using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    /// Controlador encargado de mostrar el perfil del usuario,
    /// actualizar datos personales y cambiar contraseña sin usar API.
    public class PerfilController : Controller
    {
        // Repositorio para trabajar con datos del perfil en la base de datos.
        private readonly PerfilRepository _perfilRepo = new PerfilRepository();

        #region Vista del perfil

        /// Petición GET: muestra la pantalla Mi Perfil.
        [HttpGet]
        public ActionResult Index()
        {
            // Verifica si existe un usuario con sesión iniciada.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int usuarioId = (int)Session["UsuarioId"];

            // Antes: el perfil se armaba solo con datos guardados en Session.
            // Ahora: se consulta la base para mostrar datos actualizados.
            PerfilViewModel perfil = _perfilRepo.ObtenerDatosPerfil(usuarioId);

            if (perfil == null)
            {
                // Si por algún motivo no existe el usuario, se cierra la sesión.
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Auth");
            }

            // Se actualiza Session por si el usuario modificó nombre o correo antes.
            Session["NombreCompleto"] = perfil.NombreCompleto;
            Session["Correo"] = perfil.Correo;

            return View(perfil);
        }

        #endregion

        #region Actualizar datos personales

        /// Petición POST: actualiza nombre completo y correo del usuario autenticado.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarDatos(PerfilViewModel modelo)
        {
            // Si no hay sesión activa, no se permite guardar cambios.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Se asegura que el Id usado sea el de la sesión, no uno alterado desde el formulario.
            modelo.UsuarioId = (int)Session["UsuarioId"];

            // Antes: Perfil solo permitía cambiar contraseña.
            // Ahora: también valida y guarda datos básicos como nombre y correo.
            if (!ModelState.IsValid)
            {
                return View("Index", modelo);
            }

            // Se revisa si el correo ya pertenece a otro usuario.
            bool correoDuplicado = _perfilRepo.ExisteCorreoEnOtroUsuario(modelo.UsuarioId, modelo.Correo);

            if (correoDuplicado)
            {
                ModelState.AddModelError("Correo", "Este correo ya está registrado por otro usuario.");
                return View("Index", modelo);
            }

            // Se guardan los datos personales en la base de datos.
            bool exito = _perfilRepo.ActualizarDatosPerfil(
                modelo.UsuarioId,
                modelo.NombreCompleto,
                modelo.Correo
            );

            if (!exito)
            {
                ModelState.AddModelError("", "No se pudieron actualizar los datos del perfil.");
                return View("Index", modelo);
            }

            // Se actualiza Session para que el menú superior muestre el nuevo nombre.
            Session["NombreCompleto"] = modelo.NombreCompleto;
            Session["Correo"] = modelo.Correo;

            TempData["MensajePerfil"] = "Datos del perfil actualizados correctamente.";

            return RedirectToAction("Index");
        }

        #endregion

        #region Cambio de contraseña

        /// Petición POST: procesa el formulario de cambio de contraseña.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarClave(PerfilViewModel modelo)
        {
            // Se valida nuevamente que exista una sesión activa.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            modelo.UsuarioId = (int)Session["UsuarioId"];

            // Antes: los datos del perfil se recargaban desde Session.
            // Ahora: se recargan desde la base para que la vista conserve datos actualizados.
            PerfilViewModel datosPerfil = _perfilRepo.ObtenerDatosPerfil(modelo.UsuarioId);

            if (datosPerfil != null)
            {
                modelo.NombreCompleto = datosPerfil.NombreCompleto;
                modelo.Correo = datosPerfil.Correo;
            }

            // Antes: la contraseña usaba Required en el ViewModel.
            // Ahora: se valida manualmente para que el formulario de datos personales funcione separado.
            if (string.IsNullOrWhiteSpace(modelo.ClaveActual))
            {
                ModelState.AddModelError("ClaveActual", "La contraseña actual es obligatoria.");
            }

            if (string.IsNullOrWhiteSpace(modelo.NuevaClave))
            {
                ModelState.AddModelError("NuevaClave", "La nueva contraseña es obligatoria.");
            }
            else if (modelo.NuevaClave.Length < 6)
            {
                ModelState.AddModelError("NuevaClave", "La contraseña debe tener al menos 6 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(modelo.ConfirmarClave))
            {
                ModelState.AddModelError("ConfirmarClave", "Debe confirmar la nueva contraseña.");
            }

            if (!string.IsNullOrWhiteSpace(modelo.NuevaClave) &&
                !string.IsNullOrWhiteSpace(modelo.ConfirmarClave) &&
                modelo.NuevaClave != modelo.ConfirmarClave)
            {
                ModelState.AddModelError("ConfirmarClave", "Las contraseñas no coinciden.");
            }

            if (!ModelState.IsValid)
            {
                return View("Index", modelo);
            }

            // Se consulta en la base de datos si la contraseña actual es correcta.
            bool claveActualCorrecta = _perfilRepo.ValidarClaveActual(
                modelo.UsuarioId,
                modelo.ClaveActual
            );

            if (!claveActualCorrecta)
            {
                ModelState.AddModelError("ClaveActual", "La contraseña actual no es correcta.");
                return View("Index", modelo);
            }

            // Si la contraseña actual está correcta, se guarda la nueva contraseña.
            bool exito = _perfilRepo.ActualizarContrasena(
                modelo.UsuarioId,
                modelo.NuevaClave
            );

            if (!exito)
            {
                ModelState.AddModelError("", "No se pudo actualizar la contraseña.");
                return View("Index", modelo);
            }

            TempData["MensajeClave"] = "Contraseña actualizada correctamente.";

            return RedirectToAction("Index");
        }

        #endregion
    }
}