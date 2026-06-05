using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PruebaSegundaUnidad
{
    // Clase principal de arranque de la aplicación ASP.NET MVC.
    // Aquí se registra la configuración general del proyecto.
    public class MvcApplication : System.Web.HttpApplication
    {
        // Este método se ejecuta una vez cuando inicia la aplicación.
        protected void Application_Start()
        {
            // Registra áreas MVC si existieran en el proyecto.
            AreaRegistration.RegisterAllAreas();

            // Registra la configuración de Web API.
            // Esto permite usar rutas como /api/solicitudes.
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Registra filtros globales, como manejo general de errores.
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // Registra rutas MVC.
            // Esto permite URLs como /Home/Index o /Usuarios/Editar/5.
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Registra bundles de CSS y JavaScript.
            // Se cargan desde el Layout con @Styles.Render y @Scripts.Render.
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}