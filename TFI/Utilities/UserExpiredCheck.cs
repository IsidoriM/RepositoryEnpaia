using System.Web;
using System.Web.Mvc;

namespace TFI.Utilities
{
    public class UserExpiredCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session != null)
                if (filterContext.HttpContext.Session["utente"] is null)
                    HttpContext.Current.Response.Redirect("/Login/Accesso");
        }
    }
}