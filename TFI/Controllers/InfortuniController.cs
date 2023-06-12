using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using OCM.TFI.OCM.Infortuni;
using TFI.BLL.Dropdown;
using TFI.BLL.Infortuni;
using TFI.OCM.Utente;
using TFI.Utilities;

namespace TFI.Controllers
{
    [UserExpiredCheck]
    public class InfortuniController : Controller
    {
        private readonly InfortuniBll _infortuniBll;
        private readonly DropdownBLL _dropdownBll;

        private Utente Utente => (Utente)Session["utente"];

        public InfortuniController()
        {
            _infortuniBll = new InfortuniBll();
            _dropdownBll = new DropdownBLL();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Nuovo()
        {
            if (Utente.Tipo == "C")
                return RedirectToAction("Index", "Consulente");
            if (Utente.Tipo != "A")
                return RedirectToAction("Index", "Infortuni");
            ViewBag.TipiInfortunio = _dropdownBll.GetTipiInfortunio().ToSelectListItem();
            return View(new InserimentoInfortunio
            {
                DataDenuncia = DateTime.Now
            });
        }

        public ActionResult RicercaIscritto(RicercaIscrittoModel model)
        {
            var resultRicercaIscritto = _infortuniBll.RicercaIscritto(new RicercaIscritto
            {
                Matricola = model.MatricolaFornita,
                Nome = model.NomeFornito,
                Cognome = model.CognomeFornito,
                CodiceFiscale = model.CodiceFiscaleFornito,
                CodicePosizione = Utente.CodPosizione
            });

            if (!resultRicercaIscritto.IsSuccessfull)
            {
                ViewBag["errorMsg"] = resultRicercaIscritto.Message;
                return PartialView("_RicercaIscritto", model);
            }

            model.IscrittiTrovati = resultRicercaIscritto.Content;

            return PartialView("_RicercaIscritto", model);
        }
    }
}