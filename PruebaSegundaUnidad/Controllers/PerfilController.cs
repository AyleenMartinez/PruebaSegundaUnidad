using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    /// Controlador MVC para visualizar los datos del usuario en sesión 
    /// y gestionar la actualización de su contraseña de forma segura.
    public class PerfilController : Controller
    {
        private readonly PerfilRepository _perfilRepo = new PerfilRepository();

        /// Petición GET: Carga la vista del perfil con los datos de solo lectura del usuario.
        /// <returns>Vista Index con el PerfilViewModel cargado.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            // Validar que la sesión exista antes de cargar la página
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");

            // Poblamos el ViewModel directamente con los datos seguros guardados en la sesión actual
            var perfil = new PerfilViewModel
            {
                UsuarioId = (int)Session["UsuarioId"],
                NombreCompleto = Session["NombreCompleto"].ToString(),
                Correo = Session["Correo"].ToString()
            };

            return View(perfil);
        }

        /// Petición POST: Procesa el formulario de cambio de contraseña validando reglas de negocio.
        /// <param name="modelo">Datos capturados desde los campos de texto de la vista.</param>
        /// <returns>Recarga la vista mostrando el mensaje de éxito o los errores de validación.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarClave(PerfilViewModel modelo)
        {
            // Validar nuevamente la sesión para evitar peticiones directas maliciosas por URL
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                // Si las validaciones fallan (ej. contraseñas no coinciden), recargamos la vista
                return View("Index", modelo);
            }

            // Nunca confiamos en el ID que viene del HTML (modelo.UsuarioId), ya que puede ser alterado.
            // Siempre usamos el ID de la sesión en el servidor para garantizar que modifica su propia clave.
            int idUsuarioAutenticado = (int)Session["UsuarioId"];

            // Se envía la nueva clave al repositorio para actualizarla en SQL
            bool exito = _perfilRepo.ActualizarContrasena(idUsuarioAutenticado, modelo.NuevaClave);

            if (exito)
            {
                ViewBag.MensajeExito = "Contraseña actualizada correctamente.";
            }
            else
            {
                ViewBag.Error = "Ocurrió un error al actualizar la contraseña.";
            }

            // Para que la vista no se rompa al recargar, debemos re-asignar los datos de lectura
            modelo.NombreCompleto = Session["NombreCompleto"].ToString();
            modelo.Correo = Session["Correo"].ToString();

            return View("Index", modelo);
        }
    }
}