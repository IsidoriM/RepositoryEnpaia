using System.Web.Mvc;
using log4net;
using TFI.BLL.AziendaConsulente;
using TFI.OCM.AziendaConsulente;
using TFI.OCM.Utente;

namespace TFI.Controllers
{
    // [CheckLogin]
    public class HomeController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger("RollingFile");

        private static readonly ILog TrackLog = LogManager.GetLogger("Track");

        public ActionResult Index()
        {
            Utente utente = base.Session["utente"] as Utente;
            AziDelegheBLL delegheBLL = new AziDelegheBLL();
            string errorMessage = null;
            string successMessage = null;
            
            if (utente.Tipo == "A")
            {
                DelegheOCM deleghe = delegheBLL.SearchDeleghe(utente.CodPosizione, ref errorMessage, ref successMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    base.TempData["errorMessage"] = errorMessage;
                }
                else
                {
                    base.Session["aziendaConDelegheAttive"] = deleghe.delegheatt.Count > 0;
                }
            }
            base.ViewBag.NomeUtente = base.Session["NomeUtente"];
           // HttpContext.Items[(object)"VisibilityShaker"] = (object)" style = 'visibility:hidden;' ";
            return View();
        }
    }
}
