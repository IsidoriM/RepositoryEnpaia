using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using OCM.TFI.OCM.Utilities;
using TFI.BLL.AziendaConsulente;
using TFI.BLL.Consulente;
using TFI.BLL.Utilities;
using TFI.OCM;
using TFI.OCM.AziendaConsulente;
using TFI.OCM.Utente;
using TFI.Utilities;
using TFI.Utilities.Validators;
using TFI.Utilities.Validators.FileValidatorDenuncia;

namespace TFI.Controllers
{
    [UserExpiredCheck]
    public class ConsulenteController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger("RollingFile");
        private static readonly ILog TrackLog = LogManager.GetLogger("Track");
        private static readonly ConsulenteBLL _consulenteBll = new ConsulenteBLL();

        public ActionResult Index()
        {            
            Session["layout"] = "PartialViewConsulente";
            Session["NomeAzienda"] = null;
            Utente utente = Session["utente"] as Utente;
            var aziende = _consulenteBll.GetAziendeConDelegaAttivaOrInAttesa(utente.CodTer);
            return View(aziende);
        }

        public ActionResult SelezionaAzienda(int id, string nome)
        {
            Utente utente = Session["utente"] as Utente;
            utente.NomeAzienda = nome;
            utente.CodPosizione = id.ToString();
            string nomeAzi = utente.NomeAzienda;
            Session["NomeAzienda"] = nomeAzi;
            Session["Posizione"] = utente.CodPosizione;
            Session["layout"] = "PartialViewAzienda";
            return RedirectToAction("Index", "AziendaConsulente");
        }
        
        public JsonResult CercaAziendaSenzaDelegaAttivaAjax(string identificativo)
        {
            var errorMsg = string.Empty;
            var azienda = _consulenteBll.CercaAziendaSenzaDelegaAttiva(identificativo, ref errorMsg);

            return Json(new { azienda,  errorMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RichiediDelega(string codPos)
        {
            Utente utente = (Utente)HttpContext.Session["utente"];

            var resultRichiestaDelega = _consulenteBll.RichiediDelega(codPos, utente.CodTer);

            SetTempDataMessage(resultRichiestaDelega);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UploadDipa(int annoDenuncia, int meseDenuncia, HttpPostedFileBase fileUpload)
        {
            if(annoDenuncia >= 10000)
                return ReturnToUploadDIPAWithErrorMsg("Anno inserito non valido");
            if (meseDenuncia > 12)
                return ReturnToUploadDIPAWithErrorMsg("Mese inserito non valido");

            Utente utente = Session["utente"] as Utente;
            var idAziendeConDelegaAttiva = 
                _consulenteBll.GetAziendeConDelegaAttivaOrInAttesa(utente.CodTer)
                .Where(azienda => azienda.IsDelegaConfermata)
                .Select(azienda => azienda.CodiceIdentificativo).ToList();
            var validationResults = new List<ValidationResult>();
            var fileContent = fileUpload.ReadFileRowsUploaded();

            SalvaDIPAFromFileIfValidationsAreSuccessful();
            
            if(!validationResults.Any(validResult => !validResult.IsSucceeded))
                TempData["successMessage"] = "Operazione effettuata con successo";

            ViewBag.listaMesi = GetMesiSelectList();
            return View("UploadDIPA");

            ActionResult ReturnToUploadDIPAWithErrorMsg(string errorMsg)
            {
                TempData["errorMessage"] = errorMsg;
                ViewBag.listaMesi = GetMesiSelectList();
                return View("UploadDIPA");
            }

            void SalvaDIPAFromFileIfValidationsAreSuccessful()
            {
                var gruppiAziende = fileContent.GroupBy(row => row.Content.Substring(0, 8));
                foreach (var azienda in gruppiAziende)
                {
                    ValidationResult validationResult;

                    var isCodPosParsable = int.TryParse(azienda.Key, out int codPosFromFile);
                    if (!isCodPosParsable)
                    {
                        AddNewErrorToValidationResults($"Il seguente CodPos inserito è invalido: {azienda.Key}");
                        continue;
                    }
                    var codPosAzienda = codPosFromFile.ToString();
                    utente.CodPosizione = codPosAzienda;
                    if (!idAziendeConDelegaAttiva.Contains(codPosAzienda))
                    {
                        AddNewErrorToValidationResults($"Non risulta una delega attiva per l'azienda con posizione: {codPosAzienda}");
                        continue;
                    }
                    if (!DenunciaMensileBLL.CheckDIPAMesePrecedente(annoDenuncia, meseDenuncia, codPosFromFile))
                    {
                        AddNewErrorToValidationResults(
                            $"Impossibile caricare la denuncia di questo mese per l'azienda {codPosAzienda}! La denuncia del mese precedente non risulta inserita o confermata.");
                        continue;
                    }

                    if (DenunciaMensileBLL.CheckDenunciaEsistente(annoDenuncia, meseDenuncia, codPosAzienda))
                    {
                        AddNewErrorToValidationResults(
                            $"Impossibile caricare la denuncia di questo mese per l'azienda {codPosAzienda}! La denuncia risulta gia' presente oppure sono presenti delle notifiche di ufficio.");
                        continue;
                    }
                    if (!DenunciaMensileBLL.IsDenunciaPresentabile(annoDenuncia, meseDenuncia, codPosFromFile))
                    {
                        AddNewErrorToValidationResults(
                            $"Impossibile caricare la denuncia di questo mese per l'azienda {codPosAzienda}! Non risultano esserci rapporti di lavoro attivi nel mese.");
                        continue;
                    }

                    DatiNuovaDenuncia datiDenuncia = DenunciaMensileBLL.CaricaNuovaDenunciaMensile(utente, annoDenuncia.ToString(),
                        meseDenuncia.ToString(), 0, 0,
                        null, out var currentTimeStamp_session);
                    if (datiDenuncia == null)
                    {
                        AddNewErrorToValidationResults(DenunciaMensileBLL.ErrorMessage);
                        continue;
                    }

                    var rowsDenunciaAzienda = azienda.Select(line =>
                                                                    new FileLine()
                                                                    {
                                                                        Content = line.Content.Substring(8),
                                                                        Index = line.Index
                                                                    }).ToList();

                    validationResult =
                        DipaConsulenteFileValidator.ReadAndValidateDipaConsulenteUploadFile(rowsDenunciaAzienda, annoDenuncia, datiDenuncia);
                    if (!validationResult.IsSucceeded)
                    {
                        validationResults.Add(validationResult);
                        validationResult.Errors.Sort(StringComparer.InvariantCultureIgnoreCase);
                        continue;
                    }

                    DenunciaMensileBLL.ParseAndFillDenunciaFromFile(rowsDenunciaAzienda.Select(row => row.Content).ToList(), datiDenuncia, annoDenuncia, meseDenuncia);
                    CalcolaImpostaImporti(datiDenuncia);
                    
                    if (!DenunciaMensileBLL.SalvaParzialeDIPAConsulente(annoDenuncia, meseDenuncia, utente, datiDenuncia.ListaReport, datiDenuncia,
                            ref currentTimeStamp_session, out var proDen, out _))
                    {
                        validationResult = new ValidationResult(DenunciaMensileBLL.ErrorMessage);//$"Errore nel salvataggio della denuncia per l'azienda {codPosAzienda}");
                    }

                    validationResults.Add(validationResult);

                    void AddNewErrorToValidationResults(string errorMsg)
                    {
                        validationResults.Add(new ValidationResult(errorMsg));
                    }
                }

                TempData["errorsInFile"] = ViewBag.ErrorsInFile = validationResults.SelectMany(record => record.Errors).ToList();
            }
        }

        private void SetTempDataMessage(ResultDto result)
        {
            if (result.Warnings.Any())
                TempData["warningHtmlRawMessage"] = result.GetBulletListOfWarningsForHtmlRaw();            
            if(result.IsSuccessfull)
            {
                TempData["successMessage"] = result.Message;
                return;
            }
            TempData["errorMessage"] = result.Message;
        }

        public ActionResult UploadDIPA()
        {
            ViewBag.listaMesi = GetMesiSelectList();
            ViewBag.ErrorsInFile = TempData["errorsInFile"];
            return View();
        }

        private List<SelectListItem> GetMesiSelectList()
        {
            var mesi = Utils.GetMesi();
            return mesi.Keys.Select(key => new SelectListItem { Text = mesi[key], Value = key.ToString() }).ToList();
        }

        private void CalcolaImpostaImporti(DatiNuovaDenuncia datiDenuncia)
        {
            datiDenuncia.Imponibile = Math.Round(datiDenuncia.ListaReport.Sum(report => report.ImpRet), 2).ToString();
            datiDenuncia.Occasionali = Math.Round(datiDenuncia.ListaReport.Sum(report => report.ImpOcc), 2).ToString();
            datiDenuncia.Figurative = Math.Round(datiDenuncia.ListaReport.Sum(report => report.ImpFig), 2).ToString();
            datiDenuncia.Contributi = Math.Round(datiDenuncia.ListaReport.Sum(report => report.ImpRet * report.Aliquota / 100), 2).ToString();
        }
    }
}        