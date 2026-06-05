using System.Web.Mvc;

namespace PruebaSegundaUnidad
{
    // Configura filtros globales de la aplicación.
    // Los filtros permiten aplicar reglas generales a los controladores.
    public class FilterConfig
    {
        // Este método se ejecuta al iniciar la aplicación desde Global.asax.
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Maneja errores no controlados y permite mostrar una vista de error.
            filters.Add(new HandleErrorAttribute());
        }
    }
}