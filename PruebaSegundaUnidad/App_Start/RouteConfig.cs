using System.Web.Mvc;
using System.Web.Routing;

namespace PruebaSegundaUnidad
{
    // Configura las rutas MVC del proyecto.
    // Define cómo una URL llega a un controlador y a una acción.
    public class RouteConfig
    {
        // Este método se ejecuta al iniciar la aplicación desde Global.asax.
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Ignora archivos .axd usados internamente por ASP.NET.
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Ruta MVC principal del sistema.
            // Ejemplo:
            // /Usuarios/Editar/5
            // controller = Usuarios
            // action = Editar
            // id = 5
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );
        }
    }
}