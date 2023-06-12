using System.Web.Mvc;
using TFI.BLL.Convenzioni;
using TFI.Utilities;

namespace TFI.Controllers
{
    [UserExpiredCheck]
    [RoutePrefix("Convenzioni")]
    public class ConvenzioniController : Controller
    {
        [Route("")]
        public ActionResult Convenzioni()
        {
            ConvenzioniBLL convenzioniBLL = new ConvenzioniBLL();
            return View(convenzioniBLL.GetListaConvenzioni);
        }

        [Route("BPM")]
        public ActionResult ConvenzioniBPM()
        {
            return View();
        }
    }
}