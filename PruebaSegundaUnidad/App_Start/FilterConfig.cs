using System.Web;
using System.Web.Mvc;

// Seguridad global para la aplicación, se pueden agregar filtros personalizados aquí

namespace PruebaSegundaUnidad
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
