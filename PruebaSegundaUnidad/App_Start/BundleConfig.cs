using System.Web.Optimization;

namespace PruebaSegundaUnidad
{
    // Configura los bundles del proyecto.
    // Un bundle agrupa archivos JavaScript o CSS para cargarlos de forma más ordenada desde el Layout.
    public class BundleConfig
    {
        // Este método se ejecuta al iniciar la aplicación desde Global.asax.
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Scripts principales

            // Bundle de jQuery.
            // Se usa para funcionalidades JavaScript y también por Bootstrap.
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Bundle de validaciones del lado cliente.
            // Se usa con DataAnnotations y @Html.ValidationMessageFor.
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Bundle de Modernizr.
            // Ayuda a detectar características del navegador.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Bundle de Bootstrap JavaScript.
            // Se usa para componentes como navbar, modal y botones responsivos.
            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            #endregion

            #region Estilos principales

            // Bundle principal de CSS.
            // Se carga desde _Layout.cshtml con @Styles.Render("~/Content/css").
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap/bootstrap.css",
                      "~/Content/Site.css",
                      "~/Content/index.css"));

            #endregion
        }
    }
}