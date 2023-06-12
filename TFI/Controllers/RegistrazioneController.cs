using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using log4net;
using OCM.TFI.OCM.Registrazione;
using TFI.BLL.Dropdown;
using TFI.BLL.Registrazione;
using TFI.BLL.Utilities;
using TFI.OCM.Amministrativo;
using TFI.Utilities;

namespace TFI.Controllers
{
    public class RegistrazioneController : Controller
    {
        private readonly RegistrazioneAziendaBLL _registrazioneAziendaBLL = new RegistrazioneAziendaBLL();
        private readonly RegistrazioneConsulenteBLL _registrazioneConsulenteBll = new RegistrazioneConsulenteBLL();
        private readonly DropdownBLL _dropdownBll = new DropdownBLL();
        private static readonly ILog log = LogManager.GetLogger("RollingFile");

        public ActionResult IntroduzioneRegistrazioneAzienda()
        {
            return View();
        }

        public ActionResult RegistrazioneAzienda()
        {
            SetViewData();
            return View(new RegistrazioneAziendaModel());
        }

        private void SetViewData()
        {
            var province = _dropdownBll.GetProvince().ToSelectListItem();
            var statiEsteri = _dropdownBll.GetStatiEsteri().ToSelectListItem();
            var tipiVia = _dropdownBll.GetTipiVia().ToSelectListItem();
            ViewBag.NatureGiuridiche = _dropdownBll.GetNatureGiuridiche().ToSelectListItem();
            ViewBag.TipiIndirizzo = tipiVia;
            ViewBag.StatiEsteri = statiEsteri;
            ViewBag.Province = province;
            ViewBag.Generi = _dropdownBll.GetGeneri().ToSelectListItem();
            ViewBag.Incarichi = _dropdownBll.GetIncarichiRappresentante().ToSelectListItem();
            ViewBag.ProvincieDiNascita = province;
            ViewBag.CategorieAttivita = _dropdownBll.GetCategorieAttivita().ToSelectListItem();
            ViewBag.CodiciStatistici = _dropdownBll.GetCodiciStatistici().ToSelectListItem();
            ViewBag.Comuni = new List<SelectListItem>();
            ViewBag.LocalitaSelezionabili = new List<SelectListItem>();
            ViewBag.ComuniDiNascita = new List<SelectListItem>();
        }

        public JsonResult GetComuniFromProvincia(string provincia)
        {
            var comuniFiltratiPerProvincia = _dropdownBll.GetComuniFrom(provincia).ToSelectListItem();
            return Json(comuniFiltratiPerProvincia, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocalitaFromComune(string codiceComune)
        {
            var localitaFiltratePerComune = _dropdownBll.GetLocalitaFrom(codiceComune).ToSelectListItem();
            return Json(localitaFiltratePerComune, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDenominazioneComuneAndSiglaProvincia(string codiceComune)
        {
            var comuneFromCodiceComune = _registrazioneAziendaBLL.GetDenominazioneComuneAndSiglaProvinciaFrom(codiceComune);
            return Json(comuneFromCodiceComune, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTipiAttivita(string categoria)
        {
            var tipiAttivita = _dropdownBll.GetTipiAttivitaFrom(categoria).ToSelectListItem();
            return Json(tipiAttivita, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DoesCodiceFiscaleExist(string codiceFiscale)
        {
            if (string.IsNullOrEmpty(codiceFiscale))
                return Json(false, JsonRequestBehavior.AllowGet);

            var isCodiceFiscalePresent = _registrazioneAziendaBLL.IsCodiceFiscalePresent(codiceFiscale);
            return Json(isCodiceFiscalePresent, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DoesPartitaIvaExist(string partitaIva)
        {
            if(string.IsNullOrEmpty(partitaIva))
                return Json(false, JsonRequestBehavior.AllowGet);

            var isPartitaIvaPresent = _registrazioneAziendaBLL.IsPartitaIvaPresent(partitaIva);
            return Json(isPartitaIvaPresent, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RegistrazioneAzienda(RegistrazioneAziendaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    SetDatiRegistrazione(model);
                    return View(model);
                }

                var errorMsg = string.Empty;
                var successMsg = string.Empty;

                var result = _registrazioneAziendaBLL.SalvaAzienda(model, ref errorMsg, ref successMsg);

                if (!string.IsNullOrWhiteSpace(errorMsg))
                {
                    SetDatiRegistrazione(model);
                    TempData["errorMessage"] = errorMsg;
                }
                if (!string.IsNullOrWhiteSpace(successMsg))
                    TempData["successMessage"] = successMsg;

                return result.StatoRegistrazione == StatoRegistrazioneAzienda.Fallita ? View(model) : View("EsitoRegistrazioneAzienda", result);
            }
            catch (Exception e)
            {
                log.Info("err", e);
                log.Error("err", e);
                log.Debug("err", e);
                log.Fatal("err", e);
                SetDatiRegistrazione(model);
                return View(model);
            }
            
            void SetDatiRegistrazione(RegistrazioneAziendaModel modelToSet)
            {
                SetViewData();
    
                if (modelToSet.SedeLegale.SelectedProvincia != default) SetIndirizzoModel(modelToSet.SedeLegale);
                if (modelToSet.Corrispondenza.SelectedProvincia != default) SetIndirizzoModel(modelToSet.Corrispondenza);
    
                SetRappresentanteLegaleData();
    
                if (modelToSet.AltriDati.SelectedCategorieAttivita != default)
                    modelToSet.AltriDati.TipiAttivia =
                        _dropdownBll.GetTipiAttivitaFrom(modelToSet.AltriDati.SelectedCategorieAttivita)
                            .ToSelectListItem();
    
                void SetRappresentanteLegaleData()
                {
                    if (modelToSet.RappresentanteLegale.SelectedProvincia != default) SetIndirizzoModel(modelToSet.RappresentanteLegale);
                    if (modelToSet.RappresentanteLegale.SelectedProvinciaDiNascita != default)
                    {
                        modelToSet.RappresentanteLegale.ComuniDiNascita = _dropdownBll
                            .GetComuniFrom(modelToSet.RappresentanteLegale.SelectedProvinciaDiNascita).ToSelectListItem();
    
                        var provinceDiNascita = (IEnumerable<SelectListItem>)ViewBag.ProvincieDiNascita;
                        ViewBag.ProvinciaDiNascitaPlaceHolder = provinceDiNascita.FirstOrDefault(provincia =>
                            provincia.Value == modelToSet.RappresentanteLegale.SelectedProvinciaDiNascita)?.Text;
                    }
    
                    if (modelToSet.RappresentanteLegale.SelectedComuneDiNascita != default)
                    {
                        var selectedComune = modelToSet.RappresentanteLegale?.ComuniDiNascita?.FirstOrDefault(comune =>
                            comune.Value == modelToSet.RappresentanteLegale.SelectedComuneDiNascita);
                        if (selectedComune != null) selectedComune.Selected = true;
                    }
    
                    if (modelToSet.RappresentanteLegale?.SelectedStatoEsteroDiNascita != null)
                    {
                        var statiEsteri = (IEnumerable<SelectListItem>)ViewBag.StatiEsteri;
                        ViewBag.StatoEsteroDiNascitaPlaceHolder = statiEsteri.FirstOrDefault(statoEstero =>
                            statoEstero.Value == modelToSet.RappresentanteLegale.SelectedStatoEsteroDiNascita)?.Text;
                    }
    
                    if (modelToSet.RappresentanteLegale == null ||
                        modelToSet.RappresentanteLegale.SelectedGenere == default) return;
                    var generi = (IEnumerable<SelectListItem>)ViewBag.Generi;
                    ViewBag.GenerePlaceHolder = generi
                        .FirstOrDefault(genere => genere.Value == modelToSet.RappresentanteLegale.SelectedGenere.ToString())?.Text;
                }
    
                void SetIndirizzoModel(GestioneAziendeWebOCM.IndirizzoModel indirizzoModel)
                {
                    indirizzoModel.Comuni = _dropdownBll.GetComuniFrom(indirizzoModel.SelectedProvincia)
                        .ToSelectListItem();
                    ;
                    if (indirizzoModel.SelectedComune != default)
                        indirizzoModel.LocalitaSelezionabili = _dropdownBll
                            .GetLocalitaFrom(indirizzoModel.SelectedComune).ToSelectListItem();
                }
            }
        }

        public ActionResult AccessoPraticaRegistrazioneAzienda()
        {
            return View();
        }
        public ActionResult ConsultaPraticaRegistrazioneAzienda(string identificativoAzienda = null, string numProtocollo = default, string token = null)
        {
            var errorMsg = string.Empty;

            if (!string.IsNullOrWhiteSpace(token))
            {
                (identificativoAzienda, numProtocollo) = GetDataFromToken(token);
                if (identificativoAzienda == default || numProtocollo == default) return View("AccessoPraticaRegistrazioneAzienda");
            }
            if(string.IsNullOrEmpty(identificativoAzienda) || string.IsNullOrEmpty(numProtocollo)) return View("AccessoPraticaRegistrazioneAzienda");

            var result = _registrazioneAziendaBLL.GetDettaglioPratica(identificativoAzienda, numProtocollo, ref errorMsg);

            if (!string.IsNullOrWhiteSpace(errorMsg))
                TempData["errorMessage"] = errorMsg;

            if (result == null)
                return View("AccessoPraticaRegistrazioneAzienda");

            return View(result);

            (string partitaIva, string numProtocollo) GetDataFromToken(string s)
            {
                var resultToken = TokenHelper.ValidateToken(token, "identificativoAzienda", "numeroProtocollo");
                var flag1 = resultToken.TryGetValue("identificativoAzienda", out var identificativoAziendaToken);
                var flag2 = resultToken.TryGetValue("numeroProtocollo", out var numProtocolloToken);
                if (flag1 && flag2)
                {
                    return (identificativoAziendaToken, numProtocolloToken);
                }
                return (string.Empty, string.Empty);
            }
        }

        public ActionResult CaricaDocumentiRegistrazioneAzienda(DettaglioPraticaRegistrazioneAzienda praticaRegistrazione)
        {
            var errorMsg = string.Empty;
            if (!ModelState.IsValid)
            {
                var model = _registrazioneAziendaBLL.GetDettaglioPratica(praticaRegistrazione.PartitaIva,
                    praticaRegistrazione.Protocollo.NumeroProtocollo, ref errorMsg);
                return View("ConsultaPraticaRegistrazioneAzienda", model);
            }

            var result = _registrazioneAziendaBLL.SalvaDocumentiRegistrazioneAzienda(praticaRegistrazione, ref errorMsg);
            if (!string.IsNullOrWhiteSpace(errorMsg))
                TempData["errorMessage"] = errorMsg;
            if (result == null)
                return RedirectToAction("Accesso", "Login");
            return View("EsitoRegistrazioneAzienda", result);
        }

        public ActionResult Consulente()
        {
            SetViewbagConsulente();
            
            return View(new RegistrazioneConsulenteModel());
        }
        [HttpPost]
        public ActionResult Consulente(RegistrazioneConsulenteModel model)
        {
            if (ModelState.IsValid)
            {
                var resultDto = _registrazioneConsulenteBll.Registra(model);
                
                if (resultDto.IsSuccessfull) return RedirectToAction("ConsulenteRegistrazioneEseguita");
                
                TempData["errorMessage"] = resultDto.Message;
                SetViewbagConsulente();
                SetIndirizzoModelData(model);
                return View(model);
            }
            SetIndirizzoModelData(model);
            SetViewbagConsulente();
            return View(model);
        }

        public ActionResult ConsulenteRegistrazioneEseguita() => View();

        private void SetViewbagConsulente()
        {
            ViewBag.Province = _dropdownBll.GetProvince().ToSelectListItem();
            ViewBag.TipiIndirizzo = _dropdownBll.GetTipiVia().ToSelectListItem();
            ViewBag.Associazioni = _dropdownBll.GetAssociazioni().ToSelectListItem();
        }

        private void SetIndirizzoModelData(GestioneAziendeWebOCM.IndirizzoModel model)
        {
            if (model.SelectedProvincia != default)
                model.Comuni = _dropdownBll.GetComuniFrom(model.SelectedProvincia).ToSelectListItem();
            if (model.SelectedComune != default)
                model.LocalitaSelezionabili = _dropdownBll.GetLocalitaFrom(model.SelectedComune).ToSelectListItem();
        }
    }
}