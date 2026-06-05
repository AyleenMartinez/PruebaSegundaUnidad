using System.Web.Http;

namespace PruebaSegundaUnidad
{
    // Configura las rutas de Web API.
    // En este proyecto se usa principalmente para el módulo Solicitudes.
    public static class WebApiConfig
    {
        // Este método se ejecuta al iniciar la aplicación desde Global.asax.
        public static void Register(HttpConfiguration config)
        {
            #region Rutas por atributos

            // Permite usar atributos como:
            // [RoutePrefix("api/solicitudes")]
            // [Route("{id:int}")]
            config.MapHttpAttributeRoutes();

            #endregion

            #region Ruta API por defecto

            // Ruta general para controladores API.
            // Ejemplo:
            // /api/solicitudes
            // /api/solicitudes/5
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional
                }
            );

            #endregion
        }
    }
}