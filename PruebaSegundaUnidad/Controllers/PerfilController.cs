using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    // Controlador encargado del perfil del usuario conectado.
    // Permite ver datos, editar datos personales y cambiar contraseña.
    public class PerfilController : Controller
    {
        // Repositorio que trabaja con la tabla Usuarios para datos de perfil.
        private readonly PerfilRepository _perfilRepo = new PerfilRepository();

        #region Vista perfil

        [HttpGet]
        public ActionResult Index()
        {
            // Se valida si hay sesión activa.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Se obtiene el Id del usuario conectado.
            int usuarioId = (int)Session["UsuarioId"];

            // Se consultan los datos actualizados del usuario en la base.
            PerfilViewModel perfil = _perfilRepo.ObtenerDatosPerfil(usuarioId);

            // Si no se encuentra el usuario, se cierra la sesión.
            if (perfil == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Auth");
            }

            // Se actualiza la sesión para que el menú muestre datos recientes.
            Session["NombreCompleto"] = perfil.NombreCompleto;
            Session["Correo"] = perfil.Correo;

            return View(perfil);
        }

        #endregion

        #region Actualizar datos personales

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarDatos(PerfilViewModel modelo)
        {
            // Se valida si hay sesión activa.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Se usa siempre el Id de la sesión para evitar cambios manuales desde el formulario.
            modelo.UsuarioId = (int)Session["UsuarioId"];

            // Se limpian espacios innecesarios.
            if (modelo.NombreCompleto != null)
            {
                modelo.NombreCompleto = modelo.NombreCompleto.Trim();
            }

            if (modelo.Correo != null)
            {
                modelo.Correo = modelo.Correo.Trim();
            }

            // Se revisan las validaciones del modelo.
            if (!ModelState.IsValid)
            {
                return View("Index", modelo);
            }

            // Se revisa que el correo no esté usado por otro usuario.
            bool correoDuplicado = _perfilRepo.ExisteCorreoEnOtroUsuario(modelo.UsuarioId, modelo.Correo);

            if (correoDuplicado)
            {
                ModelState.AddModelError("Correo", "Este correo ya está registrado por otro usuario.");
                return View("Index", modelo);
            }

            // Se guardan los datos personales en la base.
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

            // Se actualiza la sesión con los datos nuevos.
            Session["NombreCompleto"] = modelo.NombreCompleto;
            Session["Correo"] = modelo.Correo;

            TempData["MensajePerfil"] = "Datos del perfil actualizados correctamente.";

            return RedirectToAction("Index");
        }

        #endregion

        #region Cambio de contraseña

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarClave(PerfilViewModel modelo)
        {
            // Se valida si hay sesión activa.
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Se usa el Id de la sesión.
            modelo.UsuarioId = (int)Session["UsuarioId"];

            // Se recargan los datos del perfil para mantenerlos visibles en la vista.
            PerfilViewModel datosPerfil = _perfilRepo.ObtenerDatosPerfil(modelo.UsuarioId);

            if (datosPerfil != null)
            {
                modelo.NombreCompleto = datosPerfil.NombreCompleto;
                modelo.Correo = datosPerfil.Correo;
            }

            // Se valida que la contraseña actual venga escrita.
            if (string.IsNullOrWhiteSpace(modelo.ClaveActual))
            {
                ModelState.AddModelError("ClaveActual", "La contraseña actual es obligatoria.");
            }

            // Se valida que la nueva contraseña venga escrita.
            if (string.IsNullOrWhiteSpace(modelo.NuevaClave))
            {
                ModelState.AddModelError("NuevaClave", "La nueva contraseña es obligatoria.");
            }

            // Se valida que la confirmación venga escrita.
            if (string.IsNullOrWhiteSpace(modelo.ConfirmarClave))
            {
                ModelState.AddModelError("ConfirmarClave", "Debe confirmar la nueva contraseña.");
            }

            // Se revisan las validaciones del modelo, incluyendo mínimo 4 y confirmación.
            if (!ModelState.IsValid)
            {
                return View("Index", modelo);
            }

            // Se revisa si la contraseña actual coincide con la base.
            bool claveActualCorrecta = _perfilRepo.ValidarClaveActual(
                modelo.UsuarioId,
                modelo.ClaveActual
            );

            if (!claveActualCorrecta)
            {
                ModelState.AddModelError("ClaveActual", "La contraseña actual no es correcta.");
                return View("Index", modelo);
            }

            // Se guarda la nueva contraseña.
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