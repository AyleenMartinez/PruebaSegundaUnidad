using System.Web.Mvc;
using PruebaSegundaUnidad.Models;
using PruebaSegundaUnidad.Repositories;

namespace PruebaSegundaUnidad.Controllers
{
    public class PerfilController : Controller
    {
        private readonly PerfilRepository _perfilRepo = new PerfilRepository();

        [HttpGet]
        public ActionResult Index()
        {
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");
            var perfil = new PerfilViewModel
            {
                UsuarioId = (int)Session["UsuarioId"],
                NombreCompleto = Session["NombreCompleto"].ToString(),
                Correo = Session["Correo"].ToString()
            };
            return View(perfil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarClave(PerfilViewModel modelo)
        {
            if (Session["UsuarioId"] == null) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View("Index", modelo);

            bool exito = _perfilRepo.ActualizarContrasena((int)Session["UsuarioId"], modelo.NuevaClave);
            ViewBag.MensajeExito = exito ? "Contraseña actualizada." : "Error al actualizar.";

            modelo.NombreCompleto = Session["NombreCompleto"].ToString();
            modelo.Correo = Session["Correo"].ToString();
            return View("Index", modelo);
        }
    }
}