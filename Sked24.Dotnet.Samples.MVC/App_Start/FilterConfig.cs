using System.Web;
using System.Web.Mvc;

namespace Sked24.Dotnet.Samples.MVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
