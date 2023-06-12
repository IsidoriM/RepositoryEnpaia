using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.Iscritto;
using DocumentFormat.OpenXml.Drawing;
using log4net;
using Newtonsoft.Json;
using OCM.TFI.OCM.Iscritto;
using TFI.BLL.Amministrativo;
using TFI.BLL.Iscritto;
using TFI.BLL.Login;
using TFI.BLL.Utilities;
using TFI.CRYPTO.Crypto;
using TFI.OCM;
using TFI.OCM.Iscritto;
using TFI.OCM.Utente;
using TFI.Utilities;
using TFI.Utilities.Validators;
using static TFI.OCM.Amministrativo.GestioneAziendeWebOCM;
using static TFI.OCM.Amministrativo.GestioneRapportiLavoroIscrittiOCM;

namespace TFI.Controllers
{
    //[CheckLogin]
    [UserExpiredCheck]
    public class IscrittoController : Controller
    {
        private readonly ProsPagBLL prospagBLL = new ProsPagBLL();

        private static readonly ILog log = LogManager.GetLogger("RollingFile");

        private static readonly ILog TrackLog = LogManager.GetLogger("Track");

        private RicPagTFR_BLL tfr_bll = new RicPagTFR_BLL();

        private AnticipazioniTFRBLL ant_tfr_bll = new AnticipazioniTFRBLL();

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Retribuzioni()
        {
            Utente u = (Utente)base.HttpContext.Session["utente"];
            RetribuzioneBLL retribuzioneBLL = new RetribuzioneBLL();
            var retribuzioneAnnuale = retribuzioneBLL.GetRetribuzioneAnnuales(u);
            return View(retribuzioneAnnuale);
        }

        public ActionResult RetribuzioniMensile(int posizione, int matricola, int progressivo, int anno)
        {
            RetribuzioneBLL retribuzioneBLL = new RetribuzioneBLL();
            var retribuzioneMensile = retribuzioneBLL.GetRetribuzioneMensile(posizione, matricola, progressivo, anno);
            return View(retribuzioneMensile);
        }

        public ActionResult Anagrafica()
        {
            AnagraficaBLL Anagrafica = new AnagraficaBLL();
            Anagrafica anagrafica = new Anagrafica();
            GestioneAziendeWebBLL d = new GestioneAziendeWebBLL();
            Utente u = (Utente)base.HttpContext.Session["utente"];
            var ts = Utils.GetTitoliStudio();
            var titoloStudioList = new List<TitoliStudio>();
            foreach (var titoloStudio in ts)
            {
                titoloStudioList.Add(new TitoliStudio
                {
                    codtitstu = titoloStudio.codtitstu,
                    dentistu = titoloStudio.dentistu
                });
            }
            base.ViewBag.TitoloStudio = titoloStudioList;
            string codfis = u.Username;
            anagrafica = Anagrafica.GetAnagrafica(codfis.ToUpper());
            base.ViewBag.TipoResidenza = HttpContext.Items[(object)"ListaTipoInd"];
            base.ViewBag.ListaStati = HttpContext.Items[(object)"ListaStati"];

            base.ViewBag.Provincie = d.GetProvinces();
            var comuni = d.GetComunes();
            if (!string.IsNullOrEmpty(anagrafica.SigproResidenza) && (anagrafica.SigproResidenza != "EE"))
            {
                comuni = comuni.Where(c => c.SIGPRO == anagrafica.SigproResidenza).ToList();
            }
            base.ViewBag.Comuni = comuni;
            if (!string.IsNullOrEmpty(anagrafica.CodComuneResidenza))
            {
                var localita = d.GetLocalita();
                localita = localita.Where(com => com.CODCOM == anagrafica.CodComuneResidenza).ToList();
                base.ViewBag.Localitas = localita;
            }
            anagrafica.Localita.Trim();
            ViewBag.ListaGeneri = d.GetGeneri();
            /*  if (string.IsNullOrEmpty(anagrafica.StatoEsteroResidenza))
              {
                  base.ViewBag.Provincia = "Inserire la Provincia ";
                  base.ViewBag.Cap = "Inserire il Cap ";
                  base.ViewBag.Localita = "Inserire la Località ";
              }
              else
              {
                  base.ViewBag.Nome = "readonly";
                  base.ViewBag.Cap = " ";
                  base.ViewBag.Localita = " ";
                  base.ViewBag.Provincia = " ";
              }*/
            return View(anagrafica);
        }

        [HttpPost]
        public ActionResult ModificaPWD(Anagrafica a)
        {
            string VecchiaPassword = base.HttpContext.Request.Form["VecchiaPassword"];
            string NuovaPassword = base.HttpContext.Request.Form["NuovaPassword"];

            var isPasswordValid = PasswordHelper.CheckPassword(NuovaPassword, out var validationError);
            if (!isPasswordValid)
            {
                TempData["errorMessage"] = validationError;
                return ModificaPWD();
            }

            LoginBLL.ModificaPWD(VecchiaPassword, NuovaPassword, "I");


            if (base.HttpContext.Items["errorMessage"] != null)
            {
                base.ViewBag.ViewToast = true;
                base.TempData["errorMessage"] = base.HttpContext.Items["errorMessage"].ToString();
                return ModificaPWD();
            }
            base.ViewBag.ViewToast = true;
            base.TempData["successMessage"] = "Password modificata correttamente!";
            return View();
        }

        public ActionResult ModificaPWD()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ConvenzioniBPM()
        {
            return View();
        }

        [HttpGet]
        [OverrideActionFilters]
        public JsonResult GetComune(string provincia)
        {
            GestioneAziendeWebBLL d = new GestioneAziendeWebBLL();
            var comuni = d.GetComunes();
            comuni = comuni.Where(p => p.SIGPRO == provincia).ToList();
            return Json(comuni, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OverrideActionFilters]
        public JsonResult GetLocalita(string comune)
        {
            GestioneAziendeWebBLL d = new GestioneAziendeWebBLL();
            var localita = d.GetLocalita();
            localita = localita.Where(com => com.CODCOM == comune).ToList();
            return Json(localita, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OverrideActionFilters]
        public JsonResult GetCap(string loc)
        {
            GestioneAziendeWebBLL d = new GestioneAziendeWebBLL();
            var localita = d.GetLocalita();
            localita = localita.Where(com => com.DENLOC == loc).ToList();
            return Json(localita, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Anagrafica(Anagrafica a)
        {
            AnagraficaBLL AnagBLL = new AnagraficaBLL();
            base.ViewBag.ViewToast = true;
            var ts = Utils.GetTitoliStudio();
            var titoloStudioList = new List<TitoliStudio>();
            foreach (var titoloStudio in ts)
            {
                titoloStudioList.Add(new TitoliStudio
                {
                    codtitstu = titoloStudio.codtitstu,
                    dentistu = titoloStudio.dentistu
                });
            }
            base.ViewBag.TitoloStudio = titoloStudioList;
            GestioneAziendeWebBLL d = new GestioneAziendeWebBLL();
            var comuni = d.GetComunes();
            a.CodComuneResidenza = a.ComuneResidenza;
            if (!string.IsNullOrWhiteSpace(a.Iban))
            {
                if (!Utils.CheckIban(a.Iban))
                {
                    base.TempData["errorMessage"] = "Iban Errato";
                    /*  base.ViewBag.Provincie = d.GetProvinces();

                      if (!string.IsNullOrEmpty(a.SigproResidenza))
                      {
                          comuni = comuni.Where(c => c.SIGPRO == a.SigproResidenza).ToList();
                      }
                      base.ViewBag.Comuni = comuni;
                      if (!string.IsNullOrEmpty(a.CodComuneResidenza))
                      {
                          var localitaList = d.GetLocalita();
                          localitaList = localitaList.Where(com => com.CODCOM == a.CodComuneResidenza).ToList();
                          base.ViewBag.Localitas = localitaList;
                      }*/
                    return Anagrafica();
                }

            }
            if (a.StatoEsteroResidenza == "0")
            {
                var comuneToSave = comuni.Where(p => p.CODCOM == a.ComuneResidenza).FirstOrDefault().DENCOM;
                a.ComuneResidenza = comuneToSave.Trim();

            }

            var tipoResidenzaList = d.GetVias();
            base.ViewBag.TipoResidenza = tipoResidenzaList.Select(tr => tr.DENDUG);
            var tipoResidenza = tipoResidenzaList.FirstOrDefault(tr => tr.DENDUG.Trim() == a.TipoResidenza.Trim()).CODDUG;
            a.CodTipoResidenza = tipoResidenza;

            string ErroreMSG = "";
            string SuccessMSG = "";

            a = AnagBLL.ModificaAnagrafica(a, ref ErroreMSG, ref SuccessMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }

            return Anagrafica();
        }

        public ActionResult EstrattoConto()
        {
            IscrittoEstrattoContoBLL f = new IscrittoEstrattoContoBLL();
            return View(f.EstrattoConto());
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Convenzioni()
        {
            ListaConvenzioniBLL f = new ListaConvenzioniBLL();
            return View(f.Convenzioni());
        }

        public ActionResult CertificazioniUniche()
        {
            CertUnicBLL lista = new CertUnicBLL();
            return View(lista.Dati());
        }

        public ActionResult ProspettiPagamento()
        {
            string ErroreMSG = "";
            List<Prospetti> lp = prospagBLL.GeneraProspettiPagamento(ref ErroreMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            return View(lp);
        }

        public ActionResult Privacy()
        {
            PrivacyBLL PriBLL = new PrivacyBLL();
            PriBLL.GestionePrivacy();
            return View();
        }

        [HttpPost]
        public ActionResult Privacy(Anagrafica a)
        {
            PrivacyBLL PriBLL = new PrivacyBLL();
            string ErroreMSG = "";
            string SuccessMSG = "";
            PriBLL.GestionePrivacy(a, ref ErroreMSG, ref SuccessMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return View();
        }

        public ActionResult RichiestaContoIndividuale()
        {
            FondoContoIndBLL Fondo = new FondoContoIndBLL();
            Anagrafica FondoAnag = new Anagrafica();
            Utente ute = (Utente)base.HttpContext.Session["utente"];
            if (!Fondo.HasFondoConto(ute.CodFiscale))
                return RedirectToAction("Index", "Home");
            string codfis = ute.Username;
            string ErroreMSG2 = "";
            bool result = Fondo.IsDocumentUploaded(ref ErroreMSG2);
            if (!(ErroreMSG2 != ""))
            {
                base.ViewBag.IsDocumentUploaded = result;
            }
            else
            {
                base.TempData["errorMessage"] = ErroreMSG2;
            }
            ViewBag.TipoDocumenti = UtilsGetFromDB.GetTipoDocumentoList();
            ViewBag.IBAN = Utils.GetIban(ute.CodFiscale).Trim();
            FondoAnag = Fondo.GetFondoContoInd(codfis);
            if (base.HttpContext.Items["ErrorMessage"] != null || base.HttpContext.Items["WarningMessage"] != null)
            {
                if (base.HttpContext.Items["ErrorMessage"] != null)
                {
                    base.TempData["errorMessage"] = base.HttpContext.Items["ErrorMessage"];
                }
                if (base.HttpContext.Items["WarningMessage"] != null)
                {
                    base.TempData["infoMessage"] = base.HttpContext.Items["WarningMessage"];
                }
            }
            var hasFondo = Fondo.HasFondoConto(ute.CodFiscale);
            base.Session["HasFondoConto"] = hasFondo;
            if (!hasFondo)
                return GestionePratiche();
            return View(FondoAnag);
        }

        [HttpPost]
        public ActionResult RichiestaContoIndividuale(Anagrafica a)
        {
            FondoContoIndBLL Fondo = new FondoContoIndBLL();
            string ModPagamento = base.HttpContext.Request.Form["divPagamento"].ToString();
            string Iban;
            string Bic_swift;
            if (ModPagamento == "2")
            {
                Iban = "";
                Bic_swift = "";
            }
            else
            {
                Iban = base.HttpContext.Request.Form["Iban"].ToString();
                Bic_swift = base.HttpContext.Request.Form["BICSWIFT"].ToString();
            }
            Utente utente = Session.Contents["utente"] as Utente;
            string DataFineRapp = base.HttpContext.Request.Form["DataFineRapp"].ToString();
            string Numero_Doc = base.HttpContext.Request.Form["numeroDocumento"].ToString();
            string Tipo_Doc = base.HttpContext.Request.Form["tipoDocumento"].ToString();
            string Scadenza_Doc = base.HttpContext.Request.Form["scadenzaDocumento"].ToString();
            Anagrafica FondoAnag = new Anagrafica
            {
                CodiceFiscale = utente.CodFiscale,
                DataFineRapp = DataFineRapp,
                Iban = Iban,
                Bic_swift = Bic_swift,
                Numero_Documento = Numero_Doc,
                Tipo_Documento = Tipo_Doc,
                Scadenza_Documento = Scadenza_Doc
            };
            FondoAnag = Fondo.RicFondo(FondoAnag);
            var hasFondo = Fondo.HasFondoConto(utente.CodFiscale);
            base.Session["HasFondoConto"] = hasFondo;
            GestionePraticheBLL gestionePraticheBLL = new GestionePraticheBLL();
            var hasPratiche = gestionePraticheBLL.CheckHasPratiche(utente);
            Session["HasPratiche"] = hasPratiche;
            if (hasFondo)
                return RichiestaContoIndividuale();
            return GestionePratiche();

        }

        public ActionResult RichiestaTFR(IscrittoRicTFROCM modelloForm)
        {
            string ErroreMSG = "";
            string ErroreMSG2 = "";
            string SuccessMSG = "";
            bool result = tfr_bll.IsDocumentUploaded(ref ErroreMSG2);
            if (!(ErroreMSG2 != ""))
            {
                base.ViewBag.IsDocumentUploaded = result;
            }
            else
            {
                base.TempData["errorMessage"] = ErroreMSG2;
            }
            string InfoMSG = "";
            IscrittoRicTFROCM listaRicTFR = tfr_bll.CaricaDatiTFR(ref ErroreMSG, ref SuccessMSG, ref InfoMSG);
            if (ErroreMSG != "")
            {
                if (ErroreMSG2 != "")
                {
                    base.TempData["errorMessage"] = ErroreMSG + " \n " + ErroreMSG2;
                }
                base.TempData["errorMessage"] = ErroreMSG;
            }
            if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            if (InfoMSG != "")
            {
                base.TempData["infoMessage"] = InfoMSG;
            }

            Utente utente = Session.Contents["utente"] as Utente;
            ViewBag.IBAN = UtilsGetFromDB.GetIban(utente.CodFiscale).Trim();

            ViewBag.TipoDocumenti = UtilsGetFromDB.GetTipoDocumentoList();
            modelloForm.listTFR = listaRicTFR.listTFR;
            modelloForm.ricTFR = listaRicTFR.ricTFR;
            modelloForm.AbilitaBtn = listaRicTFR.AbilitaBtn;
            return View(modelloForm);
        }

        [HttpPost]
        public ActionResult RichiestaTFR(IscrittoRicTFROCM modelloFormTFR, HttpPostedFileBase fronteDocumento, HttpPostedFileBase retroDocumento)
        {
            if (!(Utils.FileExtensionsIsValid(fronteDocumento.FileName) && Utils.FileExtensionsIsValid(retroDocumento.FileName)))
            {
                TempData["errorMessage"] = "Formato del file non accettato";
                return GestionePratiche();
            }

            string ErroreMSG = "";
            string SuccessMSG = "";
            string path = "";
            Utente utente = base.Session["utente"] as Utente;
            //TODO: decommentare e rivedere il salvataggio
            //if (fronteDocumento != null && fronteDocumento.ContentLength > 0)
            //{
            //    string tmp = "~/Upload/" + utente.Username + "_fronte." + fronteDocumento.FileName.Split('.').Last();
            //    path = base.Server.MapPath(tmp);
            //    fronteDocumento.SaveAs(path);
            //}
            //if (retroFile != null && retroFile.ContentLength > 0)
            //{
            //    string tmp = "~/Upload/" + utente.Username + "_retro." + retroFile.FileName.Split('.').Last();
            //    path = base.Server.MapPath(tmp);
            //    retroFile.SaveAs(path);
            //}
            tfr_bll.InvioRichiestaTFR(modelloFormTFR, ref ErroreMSG, ref SuccessMSG);
            var isTfr = tfr_bll.CheckTfr(utente);
            Session["IsTfr"] = isTfr;
            GestionePraticheBLL gestionePraticheBLL = new GestionePraticheBLL();
            var hasPratiche = gestionePraticheBLL.CheckHasPratiche(utente);
            Session["HasPratiche"] = hasPratiche;
            if (isTfr)
            {
                return RichiestaTFR(modelloFormTFR);
            }
            return GestionePratiche();
        }

        public ActionResult AnticipazioneTFR()
        {
            string ErroreMSG = "";
            string ErroreMSG2 = "";
            string SuccessMSG = "";
            base.ViewBag.TipoDocumenti = ant_tfr_bll.GetTipoDocList();
            bool result = ant_tfr_bll.IsDocumentUploaded(ref ErroreMSG2);
            if (!(ErroreMSG2 != ""))
            {
                base.ViewBag.IsDocumentUploaded = result;
            }
            else
            {
                base.TempData["errorMessage"] = ErroreMSG2;
            }
            string InfoMSG = "";
            ant_tfr_bll.CaricamentoDati(ref ErroreMSG, ref SuccessMSG, ref InfoMSG);
            Utente utente = base.Session.Contents["utente"] as Utente;
            ViewBag.IBAN = Utils.GetIban(utente.CodFiscale).Trim();
            if (ErroreMSG != "")
            {
                if (ErroreMSG2 != "")
                {
                    base.TempData["errorMessage"] = ErroreMSG + " \n " + ErroreMSG2;
                }
                base.TempData["errorMessage"] = ErroreMSG;
            }
            if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            if (InfoMSG != "")
            {
                base.TempData["infoMessage"] = InfoMSG;
            }
            return View();
        }

        [HttpPost]
        public ActionResult AnticipazioneTFR(Anagrafica a)
        {
            string ErroreMSG = "";
            string SuccessMSG = "";
            string WarningMSG = "";

            ant_tfr_bll.InvioDati(a, ref ErroreMSG, ref SuccessMSG, ref WarningMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
                if (WarningMSG != "")
                    base.TempData["warningMessage"] = WarningMSG;
            }
            Utente u = (Utente)base.HttpContext.Session["utente"];
            if (u == default) return RedirectToAction("Index", "Home");
            var checkAnticipo = ant_tfr_bll.CheckAnticipoTFR(u);
            base.Session["IsAnticipo"] = checkAnticipo;
            GestionePraticheBLL gestionePraticheBLL = new GestionePraticheBLL();
            var hasPratiche = gestionePraticheBLL.CheckHasPratiche(u);
            Session["HasPratiche"] = hasPratiche;
            if (checkAnticipo)
                return RedirectToAction("AnticipazioneTFR");
            return GestionePratiche();
        }

        public ActionResult Infortuni()
        {
            return View();
        }

        public ActionResult DettaglioInfortuni()
        {
            return View();
        }

        public ActionResult CartaEnpaia()
        {
            Anagrafica CartaAnag = new Anagrafica();
            CartaEnpaiaBLL getCarta = new CartaEnpaiaBLL();
            Utente ute = (Utente)base.HttpContext.Session["utente"];
            string codfis = ute.Username;
            CartaAnag = getCarta.getCartaEnapia(codfis);
            return View(CartaAnag);
        }

        [HttpPost]
        public ActionResult CartaEnpaia(Anagrafica a)
        {
            CartaEnpaiaBLL RicCarta = new CartaEnpaiaBLL();
            string mat = base.HttpContext.Request.Form["Mat"].ToString();
            string Cognome = base.HttpContext.Request.Form["Cognome"].ToString();
            string Nome = base.HttpContext.Request.Form["Nome"].ToString();
            string CodiceFiscale = base.HttpContext.Request.Form["CodiceFiscale"].ToString();
            string Indirizzo = base.HttpContext.Request.Form["Indirizzo"].ToString();
            string NumeroCivico = base.HttpContext.Request.Form["NumeroCivico"].ToString();
            string TipoResidenza = base.HttpContext.Request.Form["TipoResidenza"].ToString();
            string ComuneResidenza = base.HttpContext.Request.Form["ComuneResidenza"].ToString();
            string SigproResidenza = base.HttpContext.Request.Form["SigproResidenza"].ToString();
            string Cap = base.HttpContext.Request.Form["Cap"].ToString();
            string Telefono1 = base.HttpContext.Request.Form["Telefono1"].ToString();
            string Cellulare = base.HttpContext.Request.Form["Cellulare"].ToString();
            string Email = base.HttpContext.Request.Form["Email"].ToString();
            string EmailCert = base.HttpContext.Request.Form["EmailCert"].ToString();
            Anagrafica AnagCarta = new Anagrafica
            {
                Mat = Convert.ToDecimal(mat),
                Cognome = Cognome,
                Nome = Nome,
                CodiceFiscale = CodiceFiscale,
                Indirizzo = Indirizzo,
                NumeroCivico = NumeroCivico,
                TipoResidenza = TipoResidenza,
                ComuneResidenza = ComuneResidenza,
                SigproResidenza = SigproResidenza,
                Cap = Cap,
                Telefono1 = Telefono1,
                Cellulare = Cellulare,
                Email = Email,
                EmailCert = EmailCert
            };
            string ErroreMSG = "";
            string SuccessMSG = "";
            AnagCarta = RicCarta.RicCartaEnpaia(AnagCarta, ref ErroreMSG, ref SuccessMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return View(AnagCarta);
        }

        //public ActionResult Assegni()
        //{
        //    FondoContoAssegniBLL Fondo = new FondoContoAssegniBLL();
        //    Anagrafica FondoAnag = new Anagrafica();
        //    Utente ute = (Utente)base.HttpContext.Session["utente"];
        //    string codfis = ute.Username;
        //    string ErroreMSG = "";
        //    string SuccessMSG = "";
        //    string InfoMSG = "";
        //    FondoAnag = Fondo.GetFondoContoAssegni(codfis, ref ErroreMSG, ref SuccessMSG, ref InfoMSG);
        //    if (ErroreMSG != "")
        //    {
        //        base.TempData["errorMessage"] = ErroreMSG;
        //    }
        //    if (SuccessMSG != "")
        //    {
        //        base.TempData["successMessage"] = SuccessMSG;
        //    }
        //    if (InfoMSG != "")
        //    {
        //        base.TempData["infoMessage"] = InfoMSG;
        //    }
        //    return View(FondoAnag);
        //}

        //[HttpPost]
        //public ActionResult Assegni(Anagrafica a)
        //{
        //    FondoContoAssegniBLL Fondo = new FondoContoAssegniBLL();
        //    string ModPagamento = base.HttpContext.Request.Form["divPagamento"].ToString();
        //    string Iban;
        //    string Bic_swift;
        //    if (ModPagamento == "2")
        //    {
        //        Iban = "";
        //        Bic_swift = "";
        //    }
        //    else
        //    {
        //        Iban = base.HttpContext.Request.Form["Iban"].ToString();
        //        Bic_swift = base.HttpContext.Request.Form["Bic_swift"].ToString();
        //    }
        //    string mat = base.HttpContext.Request.Form["Mat"].ToString();
        //    string Cognome = base.HttpContext.Request.Form["Cognome"].ToString();
        //    string Nome = base.HttpContext.Request.Form["Nome"].ToString();
        //    string CodiceFiscale = base.HttpContext.Request.Form["CodiceFiscale"].ToString();
        //    DateTime DataNascita = Convert.ToDateTime(base.HttpContext.Request.Form["DataNascita"].ToString());
        //    string Sesso = base.HttpContext.Request.Form["Sesso"].ToString();
        //    string Indirizzo = base.HttpContext.Request.Form["Indirizzo"].ToString();
        //    string NumeroCivico = base.HttpContext.Request.Form["NumeroCivico"].ToString();
        //    string TipoResidenza = base.HttpContext.Request.Form["TipoResidenza"].ToString();
        //    string ComuneResidenza = base.HttpContext.Request.Form["ComuneResidenza"].ToString();
        //    string ComuneNascita = base.HttpContext.Request.Form["ComuneNascita"].ToString();
        //    string SigproNascita = base.HttpContext.Request.Form["SigproNascita"].ToString();
        //    string SigproResidenza = base.HttpContext.Request.Form["SigproResidenza"].ToString();
        //    string StatoEsteroNascita = base.HttpContext.Request.Form["StatoEsteroNascita"].ToString();
        //    string StatoEsteroResidenza = base.HttpContext.Request.Form["StatoEsteroResidenza"].ToString();
        //    string Cap = base.HttpContext.Request.Form["Cap"].ToString();
        //    string Localita = base.HttpContext.Request.Form["Localita"].ToString();
        //    string Telefono1 = base.HttpContext.Request.Form["Telefono1"].ToString();
        //    string Telefono2 = base.HttpContext.Request.Form["Telefono2"].ToString();
        //    string Cellulare = base.HttpContext.Request.Form["Cellulare"].ToString();
        //    string Email = base.HttpContext.Request.Form["Email"].ToString();
        //    string EmailCert = base.HttpContext.Request.Form["EmailCert"].ToString();
        //    string Fax = base.HttpContext.Request.Form["Fax"].ToString();
        //    string DataFineRapp = base.HttpContext.Request.Form["DataFineRapp"].ToString();
        //    Anagrafica FondoAnag = new Anagrafica
        //    {
        //        Mat = Convert.ToDecimal(mat),
        //        Cognome = Cognome,
        //        Nome = Nome,
        //        CodiceFiscale = CodiceFiscale,
        //        DataNascita = DataNascita,
        //        Sesso = Sesso,
        //        Indirizzo = Indirizzo,
        //        NumeroCivico = NumeroCivico,
        //        TipoResidenza = TipoResidenza,
        //        ComuneResidenza = ComuneResidenza,
        //        ComuneNascita = ComuneNascita,
        //        SigproNascita = SigproNascita,
        //        SigproResidenza = SigproResidenza,
        //        StatoEsteroNascita = StatoEsteroNascita,
        //        StatoEsteroResidenza = StatoEsteroResidenza,
        //        Cap = Cap,
        //        Localita = Localita,
        //        Telefono1 = Telefono1,
        //        Telefono2 = Telefono2,
        //        Cellulare = Cellulare,
        //        Email = Email,
        //        EmailCert = EmailCert,
        //        Fax = Fax,
        //        DataFineRapp = DataFineRapp,
        //        Iban = Iban,
        //        Bic_swift = Bic_swift
        //    };
        //    string ErroreMSG = "";
        //    string SuccessMSG = "";
        //    FondoAnag = Fondo.RicFondo(FondoAnag, ref ErroreMSG, ref SuccessMSG);
        //    if (ErroreMSG != "")
        //    {
        //        base.TempData["errorMessage"] = ErroreMSG;
        //    }
        //    else if (SuccessMSG != "")
        //    {
        //        base.TempData["successMessage"] = SuccessMSG;
        //    }
        //    return View(FondoAnag);
        //}

        [HttpPost]
        public ActionResult UploadDocumentoTFR(string tipoDocumento, string numeroDocumento, string scadenzaDocumento, HttpPostedFileBase fronteFile, HttpPostedFileBase retroFile)
        {
            if (!(Utils.FileExtensionsIsValid(fronteFile.FileName) && Utils.FileExtensionsIsValid(retroFile.FileName)))
            {
                TempData["errorMessage"] = "Formato del file non accettato";
                return RedirectToAction("RichiestaTFR");
            }

            RicPagTFR_BLL tfr_bll = new RicPagTFR_BLL();
            try
            {
                Utente utente = base.Session["utente"] as Utente;
                string path = "";
                if (tfr_bll.SalvaDatiDocumento(tipoDocumento, numeroDocumento, scadenzaDocumento))
                {
                    if (fronteFile != null && fronteFile.ContentLength > 0)
                    {
                        var tmp = "~/Upload/" + utente.Username + "_fronte." + fronteFile.FileName.Split('.').Last();
                        path = base.Server.MapPath(tmp);
                        fronteFile.SaveAs(path);
                    }
                    if (retroFile != null && retroFile.ContentLength > 0)
                    {
                        var tmp = "~/Upload/" + utente.Username + "_retro." + retroFile.FileName.Split('.').Last();
                        path = base.Server.MapPath(tmp);
                        retroFile.SaveAs(path);
                    }
                    base.TempData["successMessage"] = "Documenti caricati con successo";
                }
                else
                {
                    base.TempData["errorMessage"] = "Errore in fase di caricamento documenti";
                }
                return RedirectToAction("RichiestaTFR");
            }
            catch (Exception)
            {
                base.TempData["errorMessage"] = "Errore in fase di caricamento documenti";
                return RedirectToAction("RichiestaTFR");
            }
        }

        [HttpPost]
        public ActionResult UploadDocumentoAntTFR(string tipoDocumento, string numeroDocumento, string scadenzaDocumento, HttpPostedFileBase fronteFile, HttpPostedFileBase retroFile)
        {
            if (!(Utils.FileExtensionsIsValid(fronteFile.FileName) && Utils.FileExtensionsIsValid(retroFile.FileName)))
            {
                TempData["errorMessage"] = "Formato del file non accettato";
                return RedirectToAction("AnticipazioneTFR");
            }

            AnticipazioniTFRBLL antTfr_bll = new AnticipazioniTFRBLL();
            try
            {
                Utente utente = base.Session["utente"] as Utente;
                string path = "";
                if (antTfr_bll.SalvaDatiDocumento(tipoDocumento, numeroDocumento, scadenzaDocumento))
                {
                    if (fronteFile != null && fronteFile.ContentLength > 0)
                    {
                        var tmp = "~/Upload/" + utente.Username + "_fronte." + fronteFile.FileName.Split('.').Last();
                        path = base.Server.MapPath(tmp);
                        fronteFile.SaveAs(path);
                    }
                    if (retroFile != null && retroFile.ContentLength > 0)
                    {
                        var tmp = "~/Upload/" + utente.Username + "_retro." + retroFile.FileName.Split('.').Last();
                        path = base.Server.MapPath(tmp);
                        retroFile.SaveAs(path);
                    }
                    base.TempData["successMessage"] = "Documenti caricati con successo";
                }
                else
                {
                    base.TempData["errorMessage"] = "Qualcosa è andato storto";
                }
                return RedirectToAction("AnticipazioneTFR");
            }
            catch (Exception)
            {
                base.TempData["errorMessage"] = "Qualcosa è andato storto";
                return RedirectToAction("AnticipazioneTFR");
            }
        }

        [HttpPost]
        public ActionResult UploadDocumentoContoIndividuale(string tipoDocumento, string numeroDocumento, string scadenzaDocumento, HttpPostedFileBase fronteFile, HttpPostedFileBase retroFile)
        {
            if (!(Utils.FileExtensionsIsValid(fronteFile.FileName) && Utils.FileExtensionsIsValid(retroFile.FileName)))
            {
                TempData["errorMessage"] = "Formato del file non accettato";
                return RedirectToAction("RichiestaContoIndividuale");
            }

            FondoContoIndBLL fondoContoIndividuale_bll = new FondoContoIndBLL();
            try
            {
                Utente utente = base.Session["utente"] as Utente;
                string path = "";
                if (fondoContoIndividuale_bll.SalvaDatiDocumento(tipoDocumento, numeroDocumento, scadenzaDocumento))
                {
                    if (fronteFile != null && fronteFile.ContentLength > 0)
                    {
                        var tmp = "~/Upload/" + utente.Username + "_fronte." + fronteFile.FileName.Split('.').Last();
                        path = base.Server.MapPath(tmp);
                        fronteFile.SaveAs(path);
                    }
                    if (retroFile != null && retroFile.ContentLength > 0)
                    {
                        var tmp = "~/Upload/" + utente.Username + "_retro." + retroFile.FileName.Split('.').Last();
                        path = base.Server.MapPath(tmp);
                        retroFile.SaveAs(path);
                    }
                    base.TempData["successMessage"] = "Documenti caricati con successo";
                }
                else
                {
                    base.TempData["errorMessage"] = "Qualcosa è andato storto";
                }
                return RedirectToAction("RichiestaContoIndividuale");
            }
            catch (Exception)
            {
                base.TempData["errorMessage"] = "Qualcosa è andato storto";
                return RedirectToAction("RichiestaContoIndividuale");
            }
        }

        public FileResult FTP_DownloadFile(string path)
        {
            try
            {
                string u = Cypher.DeCryptPassword(path);
                string mimeType = MimeMapping.GetMimeMapping(u);
                FTP fTP = new FTP();
                return File(fTP.DownloadFile(u), mimeType);
            }
            catch (Exception ex)
            {
                base.TempData["errorMessage"] = ex.Message;
                return null;
            }
        }
        
        [OverrideActionFilters]
        public ActionResult TryToSignUp()
        {
            return View();
        }

        [OverrideActionFilters]
        public ActionResult SignUp(string matricola, string cf)
        {
            string ErroreMSG = "";
            RegistrazioneBLL registrazioneBll = new RegistrazioneBLL();
            if (!registrazioneBll.CheckSignUp(matricola, cf.ToUpper(), ref ErroreMSG))
            {
                TempData["errorMessage"] = ErroreMSG;
                return View("TryToSignUp");
            }

            var anagrafica = SetupViewBag(cf);

            var serializedAnagrafica = JsonConvert.SerializeObject(anagrafica);
            var anagraficaConPwd = JsonConvert.DeserializeObject<AnagraficaConPwd>(serializedAnagrafica);

            return View(anagraficaConPwd);
        }

        [HttpPost]
        [OverrideActionFilters]
        public ActionResult SignUp(AnagraficaConPwd a)
        {
            if (!(Utils.FileExtensionsIsValid(a.FronteFile.FileName) && Utils.FileExtensionsIsValid(a.RetroFile.FileName)))
            {
                SetupViewBag(a.CodiceFiscale);
                TempData["errorMessage"] = "Formato del file non accettato";
                return SignUp(a.Mat.ToString(), a.CodiceFiscale);
            }

            var isPasswordValid = PasswordHelper.CheckPassword(a.Password, out var validationError);
            if (!isPasswordValid)
            {
                TempData["errorMessage"] = validationError;
                return SignUp(a.Mat.ToString(), a.CodiceFiscale);
            }

            RegistrazioneBLL registrazioneBll = new RegistrazioneBLL();

            AnagraficaBLL AnagBLL = new AnagraficaBLL();
            ViewBag.ViewToast = true;
            var ts = Utils.GetTitoliStudio();
            var titoloStudioList = new List<TitoliStudio>();
            foreach (var titoloStudio in ts)
            {
                titoloStudioList.Add(new TitoliStudio
                {
                    codtitstu = titoloStudio.codtitstu,
                    dentistu = titoloStudio.dentistu
                });
            }
            ViewBag.TitoloStudio = titoloStudioList;
            GestioneAziendeWebBLL d = new GestioneAziendeWebBLL();
            var comuni = d.GetComunes();
            a.CodComuneResidenza = a.ComuneResidenza;

            if (a.StatoEsteroResidenza == "0")
            {
                var comuneToSave = comuni.Where(p => p.CODCOM == a.ComuneResidenza).FirstOrDefault().DENCOM;
                a.ComuneResidenza = comuneToSave.Trim();
            }

            if (a.StatoEsteroNascita == null)
            {
                a.CodComuneNascita = comuni.Where(p => p.DENCOM.Trim() == a.ComuneNascita).FirstOrDefault().CODCOM.Trim();
            }
            else
            {
                a.CodComuneNascita = comuni.Where(p => p.DENCOM.Trim() == a.StatoEsteroNascita).FirstOrDefault().CODCOM.Trim();
            }
            var tipoResidenzaList = d.GetVias();
            ViewBag.TipoResidenza = tipoResidenzaList.Select(tr => tr.DENDUG);
            var tipoResidenza = tipoResidenzaList.FirstOrDefault(tr => tr.DENDUG.Trim() == a.TipoResidenza.Trim()).CODDUG;
            a.CodTipoResidenza = tipoResidenza;

            string ErroreMSG = "";
            string SuccessMSG = "";

            var result = registrazioneBll.ConsolidaRegistrazione(a, ref ErroreMSG, ref SuccessMSG);
            if (ErroreMSG != "")
            {
                TempData["errorMessage"] = ErroreMSG;
                SetupViewBag(a.CodiceFiscale);
                return View(a);
            }
            if (SuccessMSG != "")
            {
                TempData["successMessage"] = SuccessMSG;
            }

            return RedirectToAction("Login", "Login", new { tipoUtente = "I" });
        }

        public ActionResult GestionePratiche()
        {
            string ErroreMSG = "";
            string SuccessMSG = "";
            Utente u = (Utente)base.HttpContext.Session["utente"];
            if (u == default) return RedirectToAction("Index", "Home");
            GestionePraticheBLL gestionePraticheBLL = new GestionePraticheBLL();
            var pratiche = gestionePraticheBLL.GetPratiche(u, ref ErroreMSG, ref SuccessMSG);
            if (pratiche.Count == 0)
                return RedirectToAction("Index", "Home");
            return View("GestionePratiche", pratiche);
        }

        public ActionResult DettaglioPratica(int? IdPratica)
        {
            Utente u = (Utente)base.HttpContext.Session["utente"];
            if (u == default || IdPratica == null) return RedirectToAction("Index", "Home");
            GestionePraticheBLL gestionePraticheBLL = new GestionePraticheBLL();
            DettaglioPratica result = gestionePraticheBLL.GetPraticaById(IdPratica.Value, u.Denominazione, u.CodFiscale);
            if(result == default)
                return RedirectToAction("Index", "Home");
            return View(result);
        }

        private Anagrafica SetupViewBag(string cf)
        {
            RegistrazioneBLL registrazioneBll = new RegistrazioneBLL();
            AnagraficaBLL Anagrafica = new AnagraficaBLL();
            GestioneAziendeWebBLL d = new GestioneAziendeWebBLL();

            var anagrafica = Anagrafica.GetAnagrafica(cf.ToUpper());

            var ts = Utils.GetTitoliStudio();
            var titoloStudioList = new List<TitoliStudio>();
            foreach (var titoloStudio in ts)
            {
                titoloStudioList.Add(new TitoliStudio
                {
                    codtitstu = titoloStudio.codtitstu,
                    dentistu = titoloStudio.dentistu
                });
            }
            ViewBag.TitoloStudio = titoloStudioList;

            ViewBag.TipoResidenza = HttpContext.Items[(object)"ListaTipoInd"];
            ViewBag.ListaStati = HttpContext.Items[(object)"ListaStati"];
            ViewBag.TipoDocumenti = registrazioneBll.GetTipoDocList();

            ViewBag.Provincie = d.GetProvinces();
            var comuni = d.GetComunes();
            if (!string.IsNullOrEmpty(anagrafica.SigproResidenza) && (anagrafica.SigproResidenza != "EE"))
            {
                comuni = comuni.Where(c => c.SIGPRO == anagrafica.SigproResidenza).ToList();
            }
            ViewBag.Comuni = comuni;
            if (!string.IsNullOrEmpty(anagrafica.CodComuneResidenza))
            {
                var localita = d.GetLocalita();
                localita = localita.Where(com => com.CODCOM == anagrafica.CodComuneResidenza).ToList();
                base.ViewBag.Localitas = localita;
            }
            ViewBag.ListaGeneri = d.GetGeneri();

            anagrafica.Localita.Trim();

            return anagrafica;
        }
    }
}
