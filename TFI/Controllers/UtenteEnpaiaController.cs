using System.Collections.Generic;
using System.Web.Mvc;
using BLL.UtenteEnpaia;
using log4net;
using TFI.OCM.AziendaConsulente;
using TFI.OCM.Utente;
using TFI.OCM.Utilities;
using TFI.Utilities;

namespace TFI.Controllers
{
    [UserExpiredCheck]
    public class UtenteEnpaiaController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger("RollingFile");

        private static readonly ILog TrackLog = LogManager.GetLogger("Track");

        public ActionResult Index()
        {
            Utente utente = base.Session["utente"] as Utente;
            List<Azienda> aziende = new List<Azienda>();
            return View(aziende);
        }

        [HttpPost]
        public ActionResult Index(string posizione, string ragioneSociale, string codiceFiscale, string partitaIVA, string submit_button)
        {
            UtenteEnpaiaBLL utente = new UtenteEnpaiaBLL();
            List<Azienda> aziende = utente.GetAziendeEnpaia(posizione, ragioneSociale, codiceFiscale, partitaIVA);
            return View(aziende);
        }

        [HttpPost]
        public ActionResult SelezionaAzienda(string posizione, string ragioneSociale)
        {
            Utente utente = base.Session["utente"] as Utente;
            AssociamentoEnpaia associamento = base.Session["associamento"] as AssociamentoEnpaia;
            utente.CodPosizione = posizione.ToString();
            utente.NomeAzienda = ragioneSociale;
            return RedirectToAction(associamento._actionMethod, "AziendaConsulente");
        }

        public ActionResult DissociaAzienda()
        {
            Utente utente = base.Session["utente"] as Utente;
            AssociamentoEnpaia associamento = base.Session["associamento"] as AssociamentoEnpaia;
            base.Session["associamento"] = null;
            if (utente.Tipo == "E" && associamento != null)
            {
                utente.CodPosizione = string.Empty;
                utente.NomeAzienda = string.Empty;
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
