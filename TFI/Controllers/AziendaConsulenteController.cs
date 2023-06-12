using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using OCM.TFI.OCM.AziendaConsulente;
using OCM.TFI.OCM.Utilities;
using PagedList;
using TFI.BLL.Amministrativo;
using TFI.BLL.AziendaConsulente;
using TFI.BLL.Login;
using TFI.BLL.Utilities;
using TFI.BLL.Utilities.PagoPa;
using TFI.CRYPTO.Crypto;
using TFI.OCM.Amministrativo;
using TFI.OCM.AziendaConsulente;
using TFI.OCM.Iscritto;
using TFI.OCM.Utente;
using TFI.OCM.Utilities;
using TFI.Utilities;
using TFI.BLL.Utilities.Pdf;
using TFI.Utilities.Validators;
using DelegheOCM = TFI.OCM.AziendaConsulente.DelegheOCM;
using JsonSerializer = System.Text.Json.JsonSerializer;
using TFI.BLL.Infortuni;
using ProtocolloDll_OPENKM;
using System.Configuration;
using TFI.Utilities.Validators.FileValidatorDenuncia;
using TFI.BLL.Dropdown;
using OCM.TFI.OCM.Registrazione;
using System.Drawing.Printing;

namespace TFI.Controllers
{
    //[CheckLogin]
    [UserExpiredCheck]
    public class AziendaConsulenteController : Controller
    {
        private readonly CessazioniRdlBll rdlBll = new CessazioniRdlBll();

        private static readonly ILog log = LogManager.GetLogger("RollingFile");

        private static readonly ILog TrackLog = LogManager.GetLogger("Track");

        private readonly AziendaBLL aziendaBLL = new AziendaBLL();
        private readonly DropdownBLL _dropdownBll = new DropdownBLL();

        public ActionResult Index()
        {
            Utente utente = base.Session["utente"] as Utente;
            if (utente.Tipo == "C")
            {
                base.Session["aziendaConDelegheAttive"] = false;
            }
            return View();
        }

        public ActionResult EstrattoContoAzienda()
        {
            Utente u = (Utente)base.HttpContext.Session["utente"];
            string codPos = u.CodPosizione;
            AziendaEstrattoContoBLL aziendaEstrattoContoBLL = new AziendaEstrattoContoBLL();
            AnagraficaEstrattiContoAzienda es = aziendaEstrattoContoBLL.GetEstrattoContoAzienda(codPos);
            es.estrattiContos = es.estrattiContos.OrderByDescending(x => x.Anno).ToList();
            return View(es);
        }



        public ActionResult RapportiLavoro()
        {
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            string codpos = base.HttpContext.Session["Posizione"].ToString();
            var rapportiDiLavoro = rapLavBll.RapportiBLLFiltrato(codpos);
            return View(rapportiDiLavoro);
        }

        public ActionResult RapportiLavoroPartialFiltered(FilterRDLModel model)
        {
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            string codpos = base.HttpContext.Session["Posizione"].ToString();
            var rapportiDiLavoro = rapLavBll.RapportiBLLFiltrato(codpos, model.PageNumber, model.PageSize, model.Filter, model.TipoRDL, model.OrderBy, model.OrderDesc);
            return PartialView("PartialViewRDL", rapportiDiLavoro);
        }

        public ActionResult NuovoRapportiLavoro(NuovoIscritto nuovoIscritto)
        {
            //if (!nuovoIscritto.pec.EndsWith("@pec") || !nuovoIscritto.pec.EndsWith("@legalmail"))
            //{
            //    base.TempData["errorMessage"] = "Errore indirizzo Pec non valido";
            //    return View();
            //}
            string codpos = base.HttpContext.Session["Posizione"].ToString();
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            RapportiLavoro rl = new RapportiLavoro();

            AziendaBLL aziendaBLL = new AziendaBLL();
            var statiEsteri = aziendaBLL.GetStatiEsteri().OrderBy(stato => stato.DENCOMSE);
            var generi = aziendaBLL.GetGeneri();

            ViewBag.ListaGeneri = generi;
            ViewBag.ListaStatiEsteri = statiEsteri;

            rl = rapLavBll.NuvoRapportiLavoroBLL(codpos);
            TempData["isAziSomm"] = System.Web.HttpContext.Current.Items["IsAziSomm"];
            rl.posizione = codpos;
            TempData["og"] = rl;

            var gestioneAziendeBLL = new GestioneAziendeWebBLL();
            ViewBag.ListaComuni = gestioneAziendeBLL.GetComunes().OrderBy(comune => comune.DENCOM);
            ViewBag.ListaLocalita = gestioneAziendeBLL.GetLocalita().OrderBy(loc => loc.DENLOC);
            ViewBag.ListaProvince = gestioneAziendeBLL.GetProvinces().OrderBy(provincia => provincia.SIGPRO);
            ViewBag.ListaDug = gestioneAziendeBLL.GetVias();

            return View(rl);
        }

        public JsonResult CercaIscritto(string codfis)
        {
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            RapportiLavoro rl = new RapportiLavoro();
            rl = rapLavBll.GetDatiMatricola(codfis);
            string codFis = rl.modIsc.codFis;
            string matricola = rl.modIsc.matricola;
            string cognome = rl.modIsc.cognome;
            string nome = rl.modIsc.nome;
            string dataNas = Convert.ToDateTime(rl.modIsc.dataNas).ToString("yyyy-MM-dd");
            string sesso = rl.modIsc.sesso;
            string comuneN = rl.modIsc.comuneN;
            string comuneNCod = rl.modIsc.comuneNCod;
            string provinciaN = rl.modIsc.provinciaN;
            string titoloStudio = rl.modIsc.titoloStudio;
            string titoloStudioCod = rl.modIsc.titoloStudioCod;
            string indirizzoCod = rl.modIsc.indirizzoCod;
            string indirizzo = rl.modIsc.indirizzo;
            string residenza = rl.modIsc.residenza;
            string civico = rl.modIsc.civico;
            string comune = rl.modIsc.comune;
            string provincia = rl.modIsc.provincia;
            string cap = rl.modIsc.cap;
            string localita = rl.modIsc.localita;
            string statoEs = rl.modIsc.statoEs;
            string co = rl.modIsc.co;
            string tel = rl.modIsc.tel;
            string tel2 = rl.modIsc.tel2;
            string cell = rl.modIsc.cell;
            string fax = rl.modIsc.fax;
            string email = rl.modIsc.email;
            string pec = rl.modIsc.pec;
            return Json(new
            {
                codFis,
                matricola,
                cognome,
                nome,
                dataNas,
                sesso,
                comuneN,
                comuneNCod,
                provinciaN,
                titoloStudio,
                titoloStudioCod,
                indirizzoCod,
                indirizzo,
                residenza,
                civico,
                comune,
                provincia,
                cap,
                localita,
                statoEs,
                co,
                tel,
                tel2,
                cell,
                fax,
                email,
                pec
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ModificaPWD(Anagrafica a)
        {
            Utente utente = Session["utente"] as Utente;
            string VecchiaPassword = base.HttpContext.Request.Form["VecchiaPassword"];
            string NuovaPassword = base.HttpContext.Request.Form["NuovaPassword"];

            var isPasswordValid = PasswordHelper.CheckPassword(NuovaPassword, out var validationError);
            if (!isPasswordValid)
            {
                TempData["errorMessage"] = validationError;
                return ModificaPWD();
            }

            LoginBLL.ModificaPWD(VecchiaPassword, NuovaPassword, utente.Tipo);
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

        public ActionResult SchedaIscritto(string matricola, string prorap)
        {
            var codpos = HttpContext.Session["Posizione"].ToString();
            var rapLavBll = new RapportiLavoroBLL();
            var rapportoLavoro = rapLavBll.SchedaIscrittoBLL(matricola, prorap, codpos);
            rapportoLavoro.posizione = codpos;

            TempData["isAziSomm"] = System.Web.HttpContext.Current.Items["IsAziSomm"];
            TempData["og"] = rapportoLavoro;

            SetupViewBag();

            return View(rapportoLavoro);

            void SetupViewBag()
            {
                var aziendaBLL = new AziendaBLL();
                var gestioneAziendeBLL = new GestioneAziendeWebBLL();

                ViewBag.ListaComuni = _dropdownBll.GetComuniFrom(rapportoLavoro.modIsc.provincia);
                ViewBag.ListaLocalita = _dropdownBll.GetLocalitaFrom(rapportoLavoro.modIsc.comuneCod)
                    .Select(localita => GetDropDownWithCleanValue(localita)).OrderBy(localita => localita.Value);

                ViewBag.ListaGeneri = aziendaBLL.GetGeneri();
                ViewBag.ListaStatiEsteri = aziendaBLL.GetStatiEsteri().OrderBy(stato => stato.DENCOMSE);
                ViewBag.ListaProvince = gestioneAziendeBLL.GetProvinces().OrderBy(provincia => provincia.SIGPRO);
                ViewBag.ListaDug = gestioneAziendeBLL.GetVias();
                ViewBag.ListaTitStu = gestioneAziendeBLL.GetTitStu();
                ViewBag.ListaTipRap = gestioneAziendeBLL.GetTipRap();

                if (!(bool)TempData["isAziSomm"])
                    return;

                ViewBag.ListaComuniAziSomm = _dropdownBll.GetComuniFrom(rapportoLavoro.modIsc.provinciaAZ);
                ViewBag.ListaLocalitaAziSomm = _dropdownBll.GetLocalitaFrom(rapportoLavoro.modIsc.comuneAZCod)
                    .Select(localita => GetDropDownWithCleanValue(localita)).OrderBy(localita => localita.Value);

                DropdownModel GetDropDownWithCleanValue(DropdownModel localita)
                {
                    return new DropdownModel
                    {
                        Key = localita.Key,
                        Value = localita.Value?.Split('-')[0].Trim()
                    };
                }
            }
        }

        [HttpPost]
        public JsonResult UpdateRapportoLavoro(int codPos, int proRap, NuovoIscritto updatedIscritto)
        {
            string successMsg = string.Empty;
            string errorMsg = string.Empty;
            Utente utente = base.Session["utente"] as Utente;
            RapportiLavoroBLL rapportiLavoroBLL = new RapportiLavoroBLL();

            bool isRapportoUpdated = rapportiLavoroBLL.ModRapportiLavoroBLL(updatedIscritto, codPos, utente, proRap, ref errorMsg, ref successMsg);
            string resultMsg = !string.IsNullOrWhiteSpace(successMsg) ? successMsg : errorMsg;

            return Json(new { isRapportoUpdated, resultMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Sospensioni(string matricola, string nome, string cognome, string prorap, string messaggio, string SuccessMSG)
        {
            RapportiLavoro rl = new RapportiLavoro();
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            string codpos = base.HttpContext.Session["Posizione"].ToString();
            rl = rapLavBll.SospensioniBLL(matricola, nome, cognome, prorap, codpos, ref messaggio, ref SuccessMSG);
            base.TempData["Ogg"] = rl;
            if (messaggio != "")
            {
                base.TempData["errorMessage"] = messaggio;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return View(rl);
        }

        public ActionResult SalvaSospensioni(string dataDa, string dataAl, string matricola, string prorap, string motsosp, string prosos, string nominativo)
        {
            Utente u = base.Session["utente"] as Utente;
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            string codpos = base.HttpContext.Session["Posizione"].ToString();
            string[] arr = nominativo.Split(' ');
            string cognome = arr[0];
            string nome = arr[1];
            string messaggio = "";
            string SuccessMSG = "";
            rapLavBll.SalvaSospensioniBLL(dataDa, dataAl, motsosp, matricola, prorap, codpos, u, prosos, ref messaggio, ref SuccessMSG);
            if (messaggio != "")
            {
                base.TempData["errorMessage"] = messaggio;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return RedirectToAction("Sospensioni", new { matricola, prorap, cognome, nome, messaggio });
        }

        public ActionResult EliminaSospensioni(string dataDa, string dataAl, string matricola, string prorap, string prosos, string motsosp, string nominativo)
        {
            Utente u = base.Session["utente"] as Utente;
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            RapportiLavoro ca = new RapportiLavoro();
            string codpos = base.HttpContext.Session["Posizione"].ToString();
            string messaggio = "";
            string SuccessMSG = "";
            rapLavBll.EliminaSospensioniBLL(dataDa, dataAl, matricola, prorap, prosos, codpos, u, ref messaggio, ref SuccessMSG);
            RapportiLavoro rl = (RapportiLavoro)base.TempData["Ogg"];
            base.TempData.Keep("Ogg");
            string[] arr = nominativo.Split(' ');
            string cognome = arr[0];
            string nome = arr[1];
            if (messaggio != "")
            {
                base.TempData["errorMessage"] = messaggio;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return RedirectToAction("Sospensioni", new { matricola, prorap, cognome, nome, messaggio });
        }

        public ActionResult CarrieraRapportiLavoro(string matricola, string prorap)
        {
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            string codpos = base.HttpContext.Session["Posizione"].ToString();
            return View(rapLavBll.CarrieraRapportiLavoroBLL(matricola, prorap, codpos));
        }

        public ActionResult ModRapportiLavoro(NuovoIscritto modIsc)
        {
            Utente u = base.Session["utente"] as Utente;
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            int codpos = Convert.ToInt32(base.HttpContext.Session["Posizione"]);
            RapportiLavoro rl = new RapportiLavoro();
            rl = (RapportiLavoro)base.TempData["og"];
            string messaggio = "";
            string SuccessMSG = "";
            NuovoIscritto nuovoIscritto = new NuovoIscritto();
            nuovoIscritto = modIsc;
            if (nuovoIscritto.checkIscritto == 0)
            {
                base.TempData.Keep("og");
                int intProRap = Convert.ToInt32(nuovoIscritto.prorap);
                rapLavBll.ModRapportiLavoroBLL(nuovoIscritto, Convert.ToInt32(codpos), u, intProRap, ref messaggio, ref SuccessMSG);
                if (messaggio != "")
                {
                    base.TempData["errorMessage"] = messaggio;
                }
                else if (SuccessMSG != "")
                {
                    base.TempData["successMessage"] = SuccessMSG;
                }
                return RedirectToAction("RapportiLavoro", new { codpos, messaggio });
            }
            base.TempData["og"] = rl;
            rapLavBll.ModRapportiLavoroBLL(nuovoIscritto, Convert.ToInt32(codpos), u, 0, ref messaggio, ref SuccessMSG);
            if (messaggio != "")
            {
                base.TempData["errorMessage"] = messaggio;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return RedirectToAction("RapportiLavoro", new { codpos, messaggio });
        }

        public JsonResult DdlContratto(string datIni)
        {
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            RapportiLavoro ni = new RapportiLavoro();
            ni = rapLavBll.GetdllContratto(datIni);
            return Json(ni.listCon.Select((Contratti x) => new { x.CODCON, x.DENCON, x.DENQUA, x.NUMMEN, x.M14, x.M15, x.M16, x.PROCON, x.CODQUACON, x.ASSCON, x.CODLOC }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DdlLivello(string codcon, string dencon)
        {
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            RapportiLavoro ni = new RapportiLavoro();
            RapportiLavoro.DatiRapporto a = new RapportiLavoro.DatiRapporto();
            ni = rapLavBll.GetdllLivello(codcon, dencon);
            var listliv = ni.listLiv.Select((Livello x) => new { x.codliv, x.denliv }).ToList();
            return Json(listliv, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Mensilita(string dencon)
        {
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            RapportiLavoro ni = new RapportiLavoro();
            RapportiLavoro.DatiRapporto a = new RapportiLavoro.DatiRapporto();
            ni = rapLavBll.GetMensilita(dencon);
            string m14 = ni.datRap.m14;
            string m15 = ni.datRap.m15;
            string m16 = ni.datRap.m16;
            string qualifica = ni.datRap.qualifica;
            string mensilita = ni.datRap.mensilita;
            string tipspe = ni.datRap.tipspe;
            string codloc = ni.datRap.codloc;
            return Json(new { m14, m15, m16, qualifica, mensilita, tipspe, codloc }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DdlAliquota(string dataNas, string codcon, string dencon)
        {
            string codpos = base.HttpContext.Session["Posizione"].ToString();
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            RapportiLavoro ni = new RapportiLavoro();
            ni = rapLavBll.GetdllAliquota(dataNas, codcon, dencon, codpos);
            return Json(ni.aliq.Select((Aliquota x) => new { x.DENGRUASS, x.CODGRUASS }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValAliquota(string CODGRUASS, string dencon, string datIsc, string datMod, string messaggio, string dataNas, bool fap)
        {
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            RapportiLavoro ni = new RapportiLavoro();
            ni = rapLavBll.GetValAliquota(CODGRUASS, dencon, datIsc, datMod, messaggio, dataNas, fap);
            string aliquota = ni.aliquota;
            return Json(aliquota, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelLivello(string numScatti, string dencon, string codliv, string datMod, string datIsc, string prorap, string tiprap)
        {
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            RapportiLavoro ni = new RapportiLavoro();
            ni = rapLavBll.GetSelLivello(numScatti, dencon, codliv, datMod, datIsc, prorap, tiprap);
            if (ni == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            string importoSc = ni.modIsc.importoSc;
            return Json(importoSc, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Emolumenti(string codliv, string dencon, string datMod)
        {
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            RapportiLavoro ni = new RapportiLavoro();
            ni = rapLavBll.GetEmolumenti(codliv, dencon, datMod);
            string emolumenti = Convert.ToDecimal(ni.modIsc.emolumenti.Trim()).ToString("C", CultureInfo.GetCultureInfo("it-IT"));
            return Json(emolumenti, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Stampa(string prot)
        {
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            string messaggio = "";
            rapLavBll.StampaBLL(prot, ref messaggio);
            base.TempData["errorMessage"] = messaggio;
            RapportiLavoro rl = new RapportiLavoro();
            string matricola = (string)base.TempData["rap"];
            string uteagg = (string)base.TempData["rap2"];
            string datini = (string)base.TempData["rap3"];
            string prorap = (string)base.TempData["rap4"];
            base.TempData.Peek("rap");
            base.TempData.Peek("rap2");
            base.TempData.Peek("rap3");
            base.TempData.Peek("rap4");
            return RedirectToAction("DettagliCarriera", "AziendaConsulente", new { matricola, uteagg, datini, prorap });
        }

        public ActionResult DettagliCarriera(string matricola, string uteagg, string datini, string prorap)
        {
            var rapLavBll = new RapportiLavoroBLL();
            var codpos = base.HttpContext.Session["Posizione"].ToString();
            var rapportiLavoro = rapLavBll.DettagliCarrieraBLL(matricola, uteagg, datini, prorap, codpos);
            TempData["rap"] = matricola;
            TempData["rap2"] = uteagg;
            TempData["rap3"] = datini;
            TempData["rap4"] = prorap;
            ViewBag.IsAziSomm = System.Web.HttpContext.Current.Items["IsAziSomm"];
            return View(rapportiLavoro);
        }



        public ActionResult DatiTFR_PREV()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DatiTFR_PREV(string mat, string cog, string nom, string periodo1, string periodo2, string stato)
        {
            DatiPREV_TFR_BLL ListPrev = new DatiPREV_TFR_BLL();
            DatiPREV_TFR_OCM datiPREV_TFR_OCM = new DatiPREV_TFR_OCM();
            string codpos = base.HttpContext.Session["Posizione"].ToString();
            datiPREV_TFR_OCM.prevdaConfs = ListPrev.SearchPrevBLL(mat, cog, nom, periodo1, periodo2, stato, codpos);
            if (string.IsNullOrEmpty(mat))
            {
                base.ViewBag.Mat = "";
            }
            else
            {
                base.ViewBag.Mat = mat;
            }
            if (string.IsNullOrEmpty(nom))
            {
                base.ViewBag.Nom = "";
            }
            else
            {
                base.ViewBag.Nom = nom;
            }
            if (string.IsNullOrEmpty(cog))
            {
                base.ViewBag.Cog = "";
            }
            else
            {
                base.ViewBag.Cog = cog;
            }
            if (string.IsNullOrEmpty(periodo1))
            {
                base.ViewBag.periodo1 = "";
            }
            else
            {
                base.ViewBag.periodo1 = periodo1;
            }
            if (string.IsNullOrEmpty(periodo2))
            {
                base.ViewBag.periodo2 = "";
            }
            else
            {
                base.ViewBag.periodo2 = periodo2;
            }

            return View(datiPREV_TFR_OCM.prevdaConfs);
        }

        public ActionResult Conferma_Prev()
        {
            DatiPREV_TFR_BLL ListPrevConf = new DatiPREV_TFR_BLL();
            DatiPREV_TFR_OCM datiPREV_TFR_OCM = new DatiPREV_TFR_OCM();
            string codpos = base.HttpContext.Session["Posizione"].ToString();
            datiPREV_TFR_OCM.prevdaConfs = ListPrevConf.SearchPrevConfBLL(codpos);
            return View(datiPREV_TFR_OCM.prevdaConfs);
        }

        public ActionResult ProspettiLiquidaTFR()
        {
            Utente utente = base.Session["utente"] as Utente;
            DatiPREV_TFR_BLL datiPREV_TFR_BLL = new DatiPREV_TFR_BLL();
            DatiPREV_TFR_OCM datiPREV_TFR_OCM = new DatiPREV_TFR_OCM();
            datiPREV_TFR_OCM = datiPREV_TFR_BLL.ProspTFR(utente.CodPosizione);

            //datiPREV_TFR_OCM.listProspLiqTfrs = datiPREV_TFR_OCM.listProspLiqTfrs.OrderByDescending(x => x.anno);
            return View(datiPREV_TFR_OCM);
        }

        public ActionResult Modprev01(string mat, string nom, string cog, string prorap, string promod)
        {
            Utente utente = base.Session["utente"] as Utente;
            string errorMessage = null;
            ModprevBLL modPrevBLL = new ModprevBLL();
            ModPrevOCM modPrevOCM = modPrevBLL.GetModPre(int.Parse(utente.CodPosizione), mat, nom, cog, prorap, promod, ref errorMessage);
            base.TempData["oggetto"] = modPrevOCM;
            base.ViewBag.Active1 = "show active";
            if (errorMessage != null)
            {
                base.TempData["errorMessage"] = errorMessage;
            }
            return View(modPrevOCM);
        }

        [HttpPost]
        public ActionResult Modprev01(string arretrato, string anncomp, string Nuovasosp, string dal, string al, string motsosp, string elimina, string btnsave_sosp, string btnsave_den, string btnsave_arr, string btnsave_Tot)
        {
            ModprevBLL modprevBLL = new ModprevBLL();
            string dal2 = Convert.ToDateTime(dal).ToString("d");
            string al2 = Convert.ToDateTime(al).ToString("d");
            ModPrevOCM og = (ModPrevOCM)base.TempData["oggetto"];
            base.TempData.Keep("oggetto");
            string aliq = og.datGen.Aliq;
            string date1 = DateTime.Now.ToString().Substring(6, 4);
            string date2 = DateTime.Now.AddYears(-1).ToString().Substring(6, 4);
            if (!string.IsNullOrEmpty(arretrato))
            {
                if (anncomp == date1 || anncomp == date2)
                {
                    og = modprevBLL.arretrati(anncomp, aliq, og, elimina);
                    base.TempData["oggetto"] = og;
                }
            }
            else if (!string.IsNullOrEmpty(Nuovasosp))
            {
                og = modprevBLL.sospensioni(dal2, al2, motsosp, og);
                base.TempData["oggetto"] = og;
            }
            else if (!string.IsNullOrEmpty(elimina))
            {
                og = modprevBLL.arretrati(anncomp, aliq, og, elimina);
                base.TempData["oggetto"] = og;
            }
            else if (!string.IsNullOrEmpty(btnsave_sosp))
            {
                og = modprevBLL.Save_sosp(og);
                base.TempData["oggetto"] = og;
            }
            else if (!string.IsNullOrEmpty(btnsave_den))
            {
                og = modprevBLL.Save_den(og);
                base.TempData["oggetto"] = og;
            }
            else if (!string.IsNullOrEmpty(btnsave_arr))
            {
                og = modprevBLL.Save_arr(og);
                base.TempData["oggetto"] = og;
            }
            return View(og);
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

        public ActionResult DatiAzienda(int? id, string name = "")
        {
            Azienda azienda = new Azienda();

            Utente u = (Utente)base.HttpContext.Session["utente"];

            if (u == default) return RedirectToAction("Login", "Login", new { tipoUtente = "A" });

            string codPos = u.CodPosizione;

            azienda.rapLeg = aziendaBLL.GetRapLeg(codPos);
            azienda.posAss = aziendaBLL.GetPosAss(codPos);
            azienda.consulente = aziendaBLL.GetConsulente(codPos);
            azienda.sedi = aziendaBLL.GetListaSedi(codPos);

            if (id.HasValue)
            {
                azienda.sedi.appoggioS = id.Value;
            }

            var gestioneAziendeBLL = new GestioneAziendeWebBLL();
            var province = gestioneAziendeBLL.GetProvinces();
            ViewBag.ListaProvince = province;
            ViewBag.Provincie = province;

            var comuni = gestioneAziendeBLL.GetComunes();
            ViewBag.ListaComuni = comuni;
            ViewBag.Comuni = comuni;

            var localita = gestioneAziendeBLL.GetLocalita();
            ViewBag.ListaLocalita = localita;
            ViewBag.Localitas = new List<GestioneAziendeWebOCM.Localita>();

            ViewBag.ListaStatiEsteri = HttpContext.Items["ListaStati"];

            azienda.vias = aziendaBLL.GetVias();
            azienda.TypeSedi = aziendaBLL.ListaTipiSedi();


            var tipoResidenzaList = gestioneAziendeBLL.GetVias();
            ViewBag.TipoResidenza = tipoResidenzaList.Select(tr => tr.DENDUG);


            if (!string.IsNullOrEmpty(azienda.rapLeg.ProvinciaRP) && (azienda.rapLeg.ProvinciaRP != "EE"))
            {
                ViewBag.Comuni = comuni.Where(c => c.SIGPRO == azienda.rapLeg.ProvinciaRP).ToList();
            }

            if (!string.IsNullOrEmpty(azienda.rapLeg.CodiceComuneResidenzaRP))
            {
                ViewBag.Localitas = localita.Where(com => com.CODCOM == azienda.rapLeg.CodiceComuneResidenzaRP).ToList();
            }

            if (!string.IsNullOrEmpty(azienda.sedi.ProvinciaS) && (azienda.sedi.ProvinciaS != "EE"))
            {
                ViewBag.ListaComuni = comuni.Where(c => c.SIGPRO == azienda.sedi.ProvinciaS);
            }

            ViewBag.ListaLocalita = new List<GestioneAziendeWebOCM.Localita>();
            if (!string.IsNullOrEmpty(azienda.sedi.CODCOMS))
            {
                ViewBag.ListaLocalita = localita.Where(com => com.CODCOM == azienda.sedi.CODCOMS).ToList();
            }

            return View(azienda);
        }

        [HttpPost]
        public ActionResult DatiAzienda(Azienda form, Azienda.PosAss pa, Azienda.RapLeg rl, Azienda.Sedi azs, string tutti, string blocco)
        {
            string ErroreMSG = "";
            string SuccessMSG = "";
            string codpos = base.Session["posizione"].ToString();
            Utente u = base.Session["utente"] as Utente;
            form.posAss = pa;
            form.rapLeg = rl;
            form.sedi = azs;

            if (azs.ViaS.Length > 80 || azs.CivicoS.Length > 20 || azs.Telefono1S.Length > 13)
                ErroreMSG = "Indirizzo, Civico o Telefono inseriti per sede corrispondenza non validi";
            else if (rl.IndirizzoRP.Length > 80 || rl.CivicoRP.Length > 20)
                ErroreMSG = "Indirizzo o Civico inseriti per rappresentante legale non validi";
            else
                aziendaBLL.UpdateAzienda(form, codpos, ref ErroreMSG, ref SuccessMSG);

            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return RedirectToAction("DatiAzienda", "AziendaConsulente");
        }

        /*public JsonResult CercaSedeInd(string id)
        {
            Azienda azienda = new Azienda();
            Utente u = (Utente)base.HttpContext.Session["utente"];
            string codPos = u.CodPosizione;
            azienda = aziendaBLL.GetSedeInd(id, codPos);
            if (azienda == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            string TipoIndirizzoS = azienda.sedi.TipoIndirizzoS;
            int coddugS = azienda.sedi.CODDUGS;
            string IndirizzoS = azienda.sedi.IndirizzoS;
            string ViaS = azienda.sedi.ViaS;
            string CivicoS = azienda.sedi.CivicoS;
            string CODCOMS = azienda.sedi.CODCOMS;
            string ComuneS = azienda.sedi.ComuneS.Trim();
            string CAPS = azienda.sedi.CAPS;
            string ProvinciaS = azienda.sedi.ProvinciaS;
            string LocalitaS = azienda.sedi.LocalitaS;
            string StatoEsteroS = azienda.sedi.StatoEsteroS;
            string Telefono1S = azienda.sedi.Telefono1S;
            string Telefono2S = azienda.sedi.Telefono2S;
            string FaxS = azienda.sedi.FaxS;
            string EmailS = azienda.sedi.EmailS;
            string EmailCertificataS = azienda.sedi.EmailCertificataS;
            string DATINIS = azienda.sedi.DATINIS;
            string AGGMANS = azienda.sedi.AGGMANS;
            return Json(new
            {
                IdS = id,
                TipoIndirizzoS = TipoIndirizzoS,
                coddugS = coddugS,
                IndirizzoS = IndirizzoS,
                ViaS = ViaS,
                CivicoS = CivicoS,
                CODCOMS = CODCOMS,
                ComuneS = ComuneS,
                CAPS = CAPS,
                ProvinciaS = ProvinciaS,
                LocalitaS = LocalitaS,
                StatoEsteroS = StatoEsteroS,
                Telefono1S = Telefono1S,
                Telefono2S = Telefono2S,
                FaxS = FaxS,
                EmailS = EmailS,
                EmailCertificataS = EmailCertificataS
            }, JsonRequestBehavior.AllowGet);
        }*/

        public ActionResult CessazioniRdl(string CodPos)
        {
            CessazioniRdlBll bll = new CessazioniRdlBll();
            CessazioniRdl c = new CessazioniRdl();
            string Posizione = base.HttpContext.Session["Posizione"].ToString();
            c = bll.Liste(Posizione);
            base.Session["ListeCessazioni"] = c;
            return View(c);
        }

        [HttpPost]
        public ActionResult CessazioniRdl(string Matricola, string Nome, string Cognome, string DataIscrizione,
            string DataCessazione, string Causale, string Iban, string BIC, int? Giorni, double? Importi)
        {
            Utente u = base.Session["utente"] as Utente;
            CessazioniRdl ListeCessazioni = base.Session["ListeCessazioni"] as CessazioniRdl;
            string ErroreMSG = "";
            string SuccessMSG = "";
            string WarningMSG = "";
            string Posizione = base.HttpContext.Session["Posizione"].ToString();
            string ProRap = string.Empty;
            string[] ProRap2 = Matricola.Split('-');
            Matricola = ProRap2[0];
            ProRap = ProRap2[1];
            CessazioniRdlBll aaa = new CessazioniRdlBll();
            bool Checked = aaa.Controllare(Matricola, Nome.Trim(), Cognome.Trim(), DataIscrizione, DataCessazione, Causale, ProRap, Posizione, u, Iban,
                BIC, Giorni, Importi, ref ErroreMSG, ref SuccessMSG, ref WarningMSG);
            TempData["errorMessage"] = ErroreMSG;
            TempData["warningMessage"] = WarningMSG;
            TempData["successMessage"] = SuccessMSG;
            return RedirectToAction("CessazioniRdl", "AziendaConsulente");
        }

        public JsonResult GetDatiAnagraficiIscritto(string matricola)
        {
            matricola = matricola.Split('-')[0];
            CessazioniRdlBll cessazioniRdlBll = new CessazioniRdlBll();
            int? outcome;
            Anagrafica anagrafica = cessazioniRdlBll.GetDatiAnagrafici(matricola, out outcome);

            string json = (outcome == 0) ? JsonConvert.SerializeObject(anagrafica) : string.Empty;



            return new JsonResult
            {
                Data = json,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult Modulistica()
        {
            ModulisticaBLL ModList = new ModulisticaBLL();
            return View(ModList.ListMod());
        }

        public ActionResult DenunciaMensile_Index(string annoSelezionato, bool isFirstLoading = true, string proDen = "", bool btnStampaIsVisible = false, bool btnConfermaIsVisible = true)
        {
            Utente utente = base.Session["utente"] as Utente;
            base.Session["annoSelezionato"] = 0;
            base.Session["meseSelezionato"] = 0;
            base.Session["progressivoDenuncia"] = 0;
            bool ciSonoNonConfermate = false;
            string hdnPrimoAnno = string.Empty;
            int? outcome = null;
            int annoCorrente = DateTime.Now.Year;
            int meseCorrente = DateTime.Now.Month;
            Dictionary<int, string> mesi = DenunciaMensileBLL.GetListaMesi();
            List<SelectListItem> listaMesi = new List<SelectListItem>();
            AssociamentoEnpaia associamento = new AssociamentoEnpaia(utente, 1);
            base.Session["associamento"] = associamento;
            if (!associamento._isAssociated && utente.Tipo == "E")
            {
                return RedirectToAction("Index", "UtenteEnpaia");
            }
            // RicercaArretrato arretrato = DenunciaMensileBLL.Ricerca(utente.CodPosizione, out outcome);
            // string lblArretrato;
            // if (outcome == 0)
            // {
            //     base.TempData["arretratoSospeso"] = arretrato;
            //     lblArretrato = "Denuncia Arretrati (sospesa)";
            //     outcome = null;
            // }
            // else
            // {
            //     if (outcome != 2)
            //     {
            //         base.TempData["errorMessage"] = DenunciaMensileBLL.ErrorMessage;
            //         return RedirectToAction("Index", "Home");
            //     }
            //     base.TempData["arretratoSospeso"] = null;
            //     lblArretrato = "Inserimento Denuncia Arretrati";
            //     outcome = null;
            // }
            foreach (var key in mesi.Keys)
            {
                SelectListItem mese = new SelectListItem();
                mese.Text = mesi[key];
                mese.Value = key.ToString();
                listaMesi.Add(mese);
            }
            int meseDenunciabile;
            int annoDenunciabile;
            if (meseCorrente - 1 != 0)
            {
                meseDenunciabile = meseCorrente - 1;
                annoDenunciabile = annoCorrente;
            }
            else
            {
                meseDenunciabile = 12;
                annoDenunciabile = annoCorrente - 1;
            }
            List<int> listaAnni = DenunciaMensileBLL.CaricaDDLAnni(Convert.ToInt32(utente.CodPosizione), ref hdnPrimoAnno, ref ciSonoNonConfermate);
            int anno = (isFirstLoading ? annoCorrente : Convert.ToInt32(annoSelezionato));
            List<DenunciaMensileSalvata> listaDenunceMensili = DenunciaMensileBLL.GetDenunceMensili(anno, Convert.ToInt32(utente.CodPosizione), utente.Tipo);
            int numeroDenunceSospese = DenunciaMensileBLL.GetNumeroSospesi(utente.CodPosizione);
            int numeroDenunceAnnoPrecedente = DenunciaMensileBLL.GetNumeroDenunceDellAnno(utente.CodPosizione, anno - 1);
            //if (listaDenunceMensili.Count == 0)
            //{
            //    base.TempData["errorMessage"] = "Non sono presenti Rapporti di Lavoro attivi per l'anno selezionato.";
            //}
            // base.ViewBag.lblArretrato = lblArretrato;
            base.ViewBag.listaMesi = listaMesi;
            base.ViewBag.proDen = proDen;
            base.ViewBag.btnStampaIsVisible = btnStampaIsVisible;
            base.ViewBag.btnConfermaIsVisible = btnConfermaIsVisible;
            base.ViewBag.numeroDenunceAnnoPrecedente = numeroDenunceAnnoPrecedente;
            base.ViewBag.numeroDenunceSospese = numeroDenunceSospese;
            base.ViewBag.hdnPrimoAnno = hdnPrimoAnno;
            base.ViewBag.annoDenunciabile = annoDenunciabile;
            base.ViewBag.meseDenunciabile = meseDenunciabile;
            base.ViewBag.annoSelezionato = anno;
            ViewBag.PrimoAnnoNonPrescritto = annoCorrente - 5;
            if (listaAnni == null)
            {
                listaAnni = new List<int>();
                listaAnni.Add(anno);
            }
            base.ViewBag.listaAnni = listaAnni;
            ViewBag.ErrorsInFile = TempData["errorsInFile"];

            return View(listaDenunceMensili);
        }

        [HttpPost]
        public ActionResult DenunciaMensile_SceltaAnno(string anno)
        {
            return RedirectToAction("DenunciaMensile_Index", new
            {
                annoSelezionato = anno,
                isFirstLoading = false
            });
        }

        public ActionResult DenunciaMensile_Nuova(string anno = "", string mese = "", int proDen = 0, int idDIPA = 0, bool isFirstLoading = false)
        {
            string currentTimeStamp_session = null;
            Utente utente = base.Session["utente"] as Utente;
            string sanit = null;
            DatiNuovaDenuncia datiDenuncia;
            if (isFirstLoading)
            {
                if ((datiDenuncia = DenunciaMensileBLL.CaricaNuovaDenunciaMensile(utente, anno, mese, proDen, idDIPA, sanit, out currentTimeStamp_session)) == null)
                {
                    base.TempData["errorMessage"] = DenunciaMensileBLL.ErrorMessage;
                    return RedirectToAction("DenunciaMensile_Index");
                }
                base.Session["timeStampDipa"] = currentTimeStamp_session;
                base.Session["datiDenuncia"] = datiDenuncia;
                base.Session["listaReport"] = datiDenuncia.ListaReport;
            }
            else
            {
                datiDenuncia = base.Session["datiDenuncia"] as DatiNuovaDenuncia;
                datiDenuncia?.ListaReport.ForEach(report => report.ImpCon = Math.Round(report.ImpRet * report.Aliquota / 100, 2));
            }
            return View(datiDenuncia);
        }

        [HttpPost]
        public ActionResult DenunciaMensile_Inserimento(string anno, string mese, int proDen, int idDipa)
        {
            return RedirectToAction("DenunciaMensile_Nuova", new
            {
                anno = anno,
                mese = mese,
                proDen = proDen,
                idDIPA = idDipa,
                isFirstLoading = true
            });
        }

        public JsonResult CaricaListaSospensioni(string matricola, string proRap, string dataIni, string dataFin)
        {
            Utente utente = Session["utente"] as Utente;
            List<SospensioneRapporto> listaSospensioni = DenunciaMensileBLL.GetSospensioni(matricola, proRap, utente.CodPosizione, dataIni, dataFin);
            return Json(listaSospensioni, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Paginazione(int? paginaProvenienza, int? paginaSelezionata, string datiModificati_json)
        {
            int recordPerPagina = 10;
            int ultimoRecordDaAggiornare = recordPerPagina * paginaProvenienza.Value;
            int primoRecordDaAggiornare = ultimoRecordDaAggiornare - recordPerPagina;
            DatiNuovaDenuncia datiDenuncia = base.Session["datiDenuncia"] as DatiNuovaDenuncia;
            List<DatiRicerca> datiModificati = JsonConvert.DeserializeObject<List<DatiRicerca>>(datiModificati_json);
            DenunciaMensileBLL.Aggiornamento(ref datiDenuncia, datiModificati);
            base.Session["datiDenuncia"] = datiDenuncia;
            int ultimoRecordDaVisualizzare = recordPerPagina * paginaSelezionata.Value;
            int primoRecordDaVisualizzare = ultimoRecordDaVisualizzare - recordPerPagina;
            List<RetribuzioneRDL> reportDaVisualizzare = datiDenuncia.ListaReport.Skip(primoRecordDaVisualizzare).ToList();
            reportDaVisualizzare = reportDaVisualizzare.Take(recordPerPagina).ToList();
            string jsonData = JsonSerializer.Serialize(reportDaVisualizzare);
            return new JsonResult
            {
                Data = jsonData,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult SalvaParzialeDIPA(List<RetribuzioneRDL> listaReport, double imponibile, double occasionale, double figurativo, double contributi, int proDen = 0)
        {
            Utente utente = base.Session["utente"] as Utente;
            DatiNuovaDenuncia datiDenuncia = base.Session["datiDenuncia"] as DatiNuovaDenuncia;
            string currentTimeStamp_session = base.Session["timeStampDipa"].ToString();

            MergeDataFromModelBindingWithSessionData();
            int idDipa = 0;
            if (DenunciaMensileBLL.SalvaParzialeDIPA(utente, datiDenuncia.ListaReport, datiDenuncia, ref currentTimeStamp_session, out proDen, out idDipa))
            {
                datiDenuncia.ProDen = proDen;
                datiDenuncia.IdDIPA = idDipa;
                base.Session["timeStampDipa"] = currentTimeStamp_session;
                base.Session["datiDenuncia"] = datiDenuncia;
                base.TempData["successMessage"] = "Operazione effettuata con successo!";
            }
            else
            {
                base.TempData["errorMessage"] = DenunciaMensileBLL.ErrorMessage;
            }
            return RedirectToAction("DenunciaMensile_Nuova", new { mese = datiDenuncia.IntMese.ToString(), anno = datiDenuncia.Anno, proDen = datiDenuncia.ProDen });

            void MergeDataFromModelBindingWithSessionData()
            {
                var reportModificatiFromModelBinding = listaReport.Where(report => IsReportModificato(report));
                reportModificatiFromModelBinding.ForEach(report => datiDenuncia.ListaReport.ForEach(rep => AssignValuesFromModelBinding(rep, report)));
                CalcolaImpostaImporti(datiDenuncia);

                bool IsReportModificato(RetribuzioneRDL report) => report.ImpOcc != 0 || report.ImpRet != 0 || report.ImpFig != 0;

                void AssignValuesFromModelBinding(RetribuzioneRDL rep, RetribuzioneRDL report)
                {
                    if (!(rep.Mat == report.Mat && rep.ProDenDet == report.ProDenDet && report.ProRap == rep.ProRap))
                        return;

                    rep.ImpCon = report.ImpCon;
                    rep.ImpRet = report.ImpRet;
                    rep.ImpOcc = report.ImpOcc;
                    rep.ImpFig = report.ImpFig;
                }
            }
        }

        public ActionResult DenunciaMensile_Lettura(int anno, int mese, int proDen, int idDipa, bool isFirstLoading = true)
        {
            Utente utente = base.Session["utente"] as Utente;
            DatiDenuncia datiDenuncia = DenunciaMensileBLL.CaricaDati(utente.CodPosizione, anno, mese, proDen, idDipa, isFirstLoading);
            if (datiDenuncia == null)
            {
                TempData["errorMessage"] = DenunciaMensileBLL.ErrorMessage;
                return RedirectToAction("DenunciaMensile_Index", new
                {
                    annoSelezionato = anno,
                    isFirstLoading = false
                });
            }
            bool btnRettificheIsVisible = DenunciaMensileBLL.CiSonoRettificheDIPA(utente.CodPosizione, anno, mese, out var outcome);
            if (btnRettificheIsVisible)
            {
                ViewBag.btnRettificheIsVisible = btnRettificheIsVisible;
                return View(datiDenuncia);
            }
            if (outcome == 1)
            {
                TempData["errorMessage"] = DenunciaMensileBLL.ErrorMessage;
                return RedirectToAction("DenunciaMensile_Index", new
                {
                    annoSelezionato = anno,
                    isFirstLoading = false
                });
            }
            ViewBag.btnRettificheIsVisible = btnRettificheIsVisible;
            return View(datiDenuncia);
        }

        [HttpPost]
        public ActionResult VisualizzaDIPA(int anno, int mese, int proDen, int idDipa)
        {
            return RedirectToAction("DenunciaMensile_Lettura", new { anno, mese, proDen, idDipa });
        }

        public JsonResult GetStatoDenuncia(int anno, int mese, string tipMov, int proDen)
        {
            Utente utente = base.Session["utente"] as Utente;
            string statoDenuncia = DenunciaMensileBLL.GetStaDen(Convert.ToInt32(utente.CodPosizione), anno, mese, tipMov, proDen);
            return Json(statoDenuncia, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminaDipa(int anno, int mese, int proDen, int idDipa)
        {
            Utente utente = Session["utente"] as Utente;
            var isDeleted = DenunciaMensileBLL.EliminaDipa(utente, anno, mese, proDen, idDipa);
            List<DenunciaMensileSalvata> listaDenunceMensili = DenunciaMensileBLL.GetDenunceMensili(anno, Convert.ToInt32(utente.CodPosizione), utente.Tipo);
            return Json(new { isDeleted, listaDenunceMensili }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DenunciaMensile_Totali(string ret = "", string staDen = "", int anno = 0, int mese = 0, int proDen = 0, int idDipa = 0, bool isFirstLoading = false)
        {
            Utente utente = base.Session["utente"] as Utente;

            DatiTotaliDenuncia totaliDenuncia = new DatiTotaliDenuncia();
            if (isFirstLoading)
            {
                string timeStampDipa = string.Empty;
                DenunciaMensileBLL.CaricaTotaliDenuncia(totaliDenuncia, utente.CodPosizione, anno, mese, proDen, idDipa, "", ref timeStampDipa);
                DenunciaMensileBLL.InitPage(totaliDenuncia);
                base.Session["timeStampDipa"] = timeStampDipa;
                base.Session["totaliDenuncia"] = totaliDenuncia;
            }
            else
            {
                totaliDenuncia = base.Session["totaliDenuncia"] as DatiTotaliDenuncia;
                totaliDenuncia.StaDen = staDen;
                if (totaliDenuncia.HdnMAV == "ok")
                {
                    totaliDenuncia.BtnStampaMAVIsVisible = true;
                    totaliDenuncia.BtnMAVIsVisible = false;
                    totaliDenuncia.TxtDataVersamento = DateTime.Now.ToString("d");
                    totaliDenuncia.TxtImportoVersato = totaliDenuncia.LblTotPagare;
                }
                else
                {
                    totaliDenuncia.BtnStampaMAVIsVisible = false;
                    totaliDenuncia.BtnMAVIsVisible = true;
                }
                if (totaliDenuncia.HdnMavSanit == "ok")
                {
                    totaliDenuncia.BtnStampaMAVSanitIsVisible = true;
                    totaliDenuncia.BtnMAVSanitIsVisible = false;
                    totaliDenuncia.TxtDataVersamentoSanit = DateTime.Now.ToString("d");
                    totaliDenuncia.TxtImportoVersatoSanit = totaliDenuncia.LblTotFondo;
                }
                else
                {
                    totaliDenuncia.BtnStampaMAVSanitIsVisible = false;
                    totaliDenuncia.BtnMAVSanitIsVisible = true;
                }
            }

            totaliDenuncia.BtnCreditiIsVisible = totaliDenuncia.TxtCreditiIsVisible = totaliDenuncia.LblCreditiIsVisible = totaliDenuncia.StaDen != "A";

            ViewBag.PrimoAnnoNonPrescritto = DateTime.Now.Year - 5;

            SetDettaglioPagoPa();

            return View(totaliDenuncia);

            void SetDettaglioPagoPa()
            {
                var result = DenunciaMensileBLL.GetDettagliPagoPa(anno, mese, proDen, utente.CodPosizione);

                if (string.IsNullOrWhiteSpace(result.EsitoIuvTransId.IuvCode) || string.IsNullOrWhiteSpace(result.EsitoIuvTransId.TransActionId))
                {
                    totaliDenuncia.StatoPagamentoPagoPa = StatoPagoPa.DACREARE;
                    return;
                }

                if (result.EsitoPagoPa.Esito != "OK")
                {
                    TempData["errorMsg"] = "Errore nel prelievo dei dati PagoPa";
                    return;
                }

                totaliDenuncia.IuvCode = result.EsitoPagoPa.Data.Iuv;
                totaliDenuncia.TransActionId = result.EsitoPagoPa.Data.TransActionId;
                if (Enum.TryParse<StatoPagoPa>(result.EsitoPagoPa.Data.Esito, true, out var statoEnum))
                    totaliDenuncia.StatoPagamentoPagoPa = statoEnum;
            }
        }

        [HttpGet]
        public ActionResult TotaliDIPA(int anno, int mese, int proDen, int idDipa, string ret = "")
        {
            return RedirectToAction("DenunciaMensile_Totali", new
            {
                ret = ret,
                anno = anno,
                mese = mese,
                proDen = proDen,
                idDipa = idDipa,
                isFirstLoading = true
            });
        }

        [HttpPost]
        public ActionResult FiltroBtnTotali(FiltroBtnTotali datiFiltro)
        {
            switch (datiFiltro.btnType)
            {
                case "SALVA_TOTALE":
                    return RedirectToAction("SalvaTotaleDIPA", new { datiFiltro.anno, datiFiltro.mese, datiFiltro.proDen, datiFiltro.idDipa, datiFiltro.txtCrediti, datiFiltro.lblTotFondo, datiFiltro.annFia, datiFiltro.staDen });
                case "DETTAGLIO":
                    if (datiFiltro.staDen == "A" || datiFiltro.staDen == "AP")
                    {
                        return RedirectToAction("DenunciaMensile_Lettura", new { datiFiltro.anno, datiFiltro.mese, datiFiltro.proDen, datiFiltro.idDipa });
                    }
                    return RedirectToAction("DenunciaMensile_Nuova");
                case "CHIUDI":
                    return RedirectToAction("DenunciaMensile_Index", new
                    {
                        annoSelezionato = datiFiltro.anno,
                        isFirstLoading = false
                    });
                default:
                    return RedirectToAction("DenunciaMensile_Index", new
                    {
                        annoSelezionato = datiFiltro.anno,
                        isFirstLoading = false
                    });
            }
        }

        public ActionResult SalvaTotaleDIPA(int anno, int mese, int proDen, decimal txtCrediti, decimal lblTotFondo, int annFia, string staDen)
        {
            Utente utente = Session["utente"] as Utente;
            string sessionTimeStampDipa = Session["timeStampDipa"].ToString();
            List<RetribuzioneRDL> listaReport = Session["listaReport"] as List<RetribuzioneRDL>;
            if (DenunciaMensileBLL.SalvaTotaleDipa(utente, listaReport, anno, mese, proDen, sessionTimeStampDipa, txtCrediti, lblTotFondo, annFia, ref staDen))
            {
                TempData["successMessage"] = DenunciaMensileBLL.SuccessMessage;
            }
            else
            {
                TempData["errorMessage"] = DenunciaMensileBLL.ErrorMessage;
            }
            return RedirectToAction("DenunciaMensile_Totali", new
            {
                anno = anno,
                mese = mese,
                proDen = proDen,
                isFirstLoading = true
            });
        }

        public string AggiornaCreditoDecurtato(int anno, int mese, int proDen, decimal txtCrediti)
        {
            Utente utente = base.Session["utente"] as Utente;
            if (DenunciaMensileBLL.AggiornaCreditoDecurtato(utente.CodPosizione, anno, mese, proDen, txtCrediti))
            {
                return "S";
            }
            return DenunciaMensileBLL.ErrorMessage;
        }

        public string RipristinaImporto(int anno, int mese, int proDen)
        {
            Utente utente = base.Session["utente"] as Utente;
            if (DenunciaMensileBLL.RipristinaImporto(utente.CodPosizione, anno, mese, proDen))
            {
                return "S";
            }
            return DenunciaMensileBLL.ErrorMessage;
        }

        [HttpGet]
        public ActionResult Filtro_totaliNuovaDenuncia(FiltroBtnTotali datiFiltro)
        {
            List<RetribuzioneRDL> listaReport = base.Session["listaReport"] as List<RetribuzioneRDL>;
            if (!DenunciaMensileBLL.VerificaImponibile(listaReport))
            {
                return RedirectToAction("TotaliDIPA", new { datiFiltro.anno, datiFiltro.mese, datiFiltro.proDen, datiFiltro.idDipa });
            }
            base.TempData["errorMessage"] = "Errore: tutte le retribuzioni hanno importo zero.";
            return RedirectToAction("DenunciaMensile_Nuova");
        }

        [HttpPost]
        public ActionResult UploadDIPA(int anno, int mese, HttpPostedFileBase fileUpload)
        {
            Utente utente = Session["utente"] as Utente;

            if (DenunciaMensileBLL.CheckDenunciaEsistente(anno, mese, utente.CodPosizione))
            {
                TempData["errorMessage"] = "Impossibile caricare la denuncia per questo mese! La denuncia risulta gia' presente oppure sono presenti delle notifiche di ufficio.";
                return RedirectToAction("DenunciaMensile_Index");
            }

            DatiNuovaDenuncia datiDenuncia = DenunciaMensileBLL.CaricaNuovaDenunciaMensile(utente, anno.ToString(), mese.ToString(), 0, 0,
                    null, out var currentTimeStamp_session);
            if (datiDenuncia == null)
            {
                TempData["errorMessage"] = DenunciaMensileBLL.ErrorMessage;
                return RedirectToAction("DenunciaMensile_Index");
            }

            var fileContent = fileUpload.ReadFileUploaded();
            var validationResult = DipaAziendaFileValidator.ReadAndValidateDipaUploadFile(fileContent, anno, datiDenuncia);
            if (!validationResult.IsSucceeded)
            {
                validationResult.Errors.Sort(StringComparer.InvariantCultureIgnoreCase);
                TempData["errorsInFile"] = validationResult.Errors;
                return RedirectToAction("DenunciaMensile_Index");
            }

            DenunciaMensileBLL.ParseAndFillDenunciaFromFile(fileContent, datiDenuncia, anno, mese);
            CalcolaImpostaImporti(datiDenuncia);

            if (DenunciaMensileBLL.SalvaParzialeDIPA(utente, datiDenuncia.ListaReport, datiDenuncia, ref currentTimeStamp_session, out var proDen, out _))
            {
                datiDenuncia.ProDen = proDen;
                Session["timeStampDipa"] = currentTimeStamp_session;
                Session["datiDenuncia"] = datiDenuncia;
                TempData["successMessage"] = "Operazione effettuata con successo!";
                return RedirectToAction("DenunciaMensile_Nuova", new
                {
                    anno = anno,
                    mese = mese,
                    proDen = proDen,
                    isFirstLoading = true
                });
            }

            TempData["errorMessage"] = DenunciaMensileBLL.ErrorMessage;
            return RedirectToAction("DenunciaMensile_Index");
        }

        //[HttpPost]
        //public ActionResult UploadDIPA(int anno, int mese, HttpPostedFileBase fileUpload, string proDen)
        //{
        //    string logErrori = "";
        //    Utente utente = base.Session["utente"] as Utente;
        //    bool btnStampaIsVisible = false;
        //    bool btnConfermaIsVisible = true;
        //    try
        //    {
        //        if (fileUpload != null)
        //        {
        //            if (fileUpload.ContentLength > 0)
        //            {
        //                string path = base.Server.MapPath("~/Upload/Dipa/" + utente.CodPosizione + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Year + ".txt");
        //                if (DenunciaMensileBLL.SalvaDIPA_upload(utente, anno, mese, fileUpload, path, ref proDen, ref btnStampaIsVisible, ref btnConfermaIsVisible))
        //                {
        //                    base.TempData["successMessage"] = DenunciaMensileBLL.SuccessMessage;
        //                    return RedirectToAction("DenunciaMensile_Index", new
        //                    {
        //                        annoSelezionato = anno,
        //                        isFirstLoading = false,
        //                        proDen = proDen,
        //                        btnStampaIsVisible = btnStampaIsVisible,
        //                        btnConfermaIsVisible = btnConfermaIsVisible
        //                    });
        //                }
        //                base.TempData["errorMessage"] = DenunciaMensileBLL.ErrorMessage;
        //                return RedirectToAction("DenunciaMensile_Index", new
        //                {
        //                    annoSelezionato = anno,
        //                    isFirstLoading = false
        //                });
        //            }
        //            base.TempData["errorMessage"] = "Non è stato possibile effettuare l'upload del file!";
        //            return RedirectToAction("DenunciaMensile_Index", new
        //            {
        //                annoSelezionato = anno,
        //                isFirstLoading = false
        //            });
        //        }
        //        base.TempData["errorMessage"] = "Non è stato possibile effettuare l'upload del file!";
        //        return RedirectToAction("DenunciaMensile_Index", new
        //        {
        //            annoSelezionato = anno,
        //            isFirstLoading = false
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Info($"[TFI.BLL] : SalvaDIPA_upload() - Durante la fase di salvataggio è stata generata un'eccezione in data: {DateTime.Now}");
        //        base.TempData["errorMessage"] = "Caricamento della denuncia mensile non riuscito! " + Environment.NewLine + " " + ex.Message;
        //        return RedirectToAction("DenunciaMensile_Index", new
        //        {
        //            annoSelezionato = anno,
        //            isFirstLoading = false
        //        });
        //    }
        //}

        public ActionResult DenunciaMensile_Rettifiche(int anno, int mese, int proDen, int idDipa)
        {
            Utente utente = base.Session["utente"] as Utente;
            RettificheDIPA rettifiche = DenunciaMensileBLL.CaricaDatiRettifiche(utente.CodPosizione, anno, mese, proDen);
            if (rettifiche != null)
            {
                rettifiche.Anno = anno;
                rettifiche.Mese = mese;
                rettifiche.ProDen = proDen;
                rettifiche.IdDipa = idDipa;
                List<DatiRettificheDIPA> listaRettifiche;
                if ((listaRettifiche = DenunciaMensileBLL.CaricaRettifiche(utente.CodPosizione, anno, mese, proDen)) != null)
                {
                    rettifiche.ListaRettifiche = listaRettifiche;
                    return View(rettifiche);
                }
                base.TempData["errorMessage"] = DenunciaMensileBLL.ErrorMessage;
                return RedirectToAction("DenunciaMensile_Index", new
                {
                    annoSelezionato = anno,
                    isFirstLoading = false
                });
            }
            base.TempData["errorMessage"] = DenunciaMensileBLL.ErrorMessage;
            return RedirectToAction("DenunciaMensile_Index", new
            {
                annoSelezionato = anno,
                isFirstLoading = false
            });
        }

        public ActionResult InsDenunciaArretrati(string anno, string mese, string proDen)
        {
            Utente ute = (Utente)base.HttpContext.Session["utente"];
            ViewBag.IsUtenteAmministrativo = ute.Tipo == "E";
            DenunciaArretratiBLL utente = new DenunciaArretratiBLL();
            DenunciaArretrati arretrato = new DenunciaArretrati();
            string lblPeriodo = (arretrato.lblPeriodo = ((!string.IsNullOrEmpty(proDen) && !(proDen == "0")) ? "Denuncia Arretrati" : "Inserimento Denuncia Arretrati"));
            arretrato.hdnAnno = ((!string.IsNullOrEmpty(anno)) ? int.Parse(anno) : 0);
            arretrato.hdnMese = ((!string.IsNullOrEmpty(mese)) ? int.Parse(mese) : 0);
            arretrato.hdnProden = ((!string.IsNullOrEmpty(proDen)) ? proDen : string.Empty);
            base.ViewBag.annoSelez = false;
            ViewBag.ListaAnniCompetenzaNonPrescritti =
                _dropdownBll.GetAnniCompetenzaNonPrescrittiFrom(ute.CodPosizione).ToSelectListItem();
            ViewBag.IsDataDenunciaFromModifica = false;
            return View(arretrato);
        }

        [HttpPost]
        public ActionResult InsDenunciaArretrati(
            string txtAnno, string hdnSalva, string hdnAnnComp, string txtDataDenuncia, string radio, List<DenunciaArretrati> ListaDenunce, string arrayNascosto, string btnIndietro, string lblPeriodo, string datApe)
        {
            Utente ute = (Utente)base.HttpContext.Session["utente"];
            ViewBag.IsUtenteAmministrativo = ute.Tipo == "E";
            DenunciaArretratiBLL utente = new DenunciaArretratiBLL();
            List<DenunciaArretrati> listaarretrati = new List<DenunciaArretrati>();
            DenunciaArretrati arretrato = new DenunciaArretrati();
            arretrato.lblPeriodo = lblPeriodo;
            string ErrorMSG = "";
            string SuccessMSG = "";
            if (btnIndietro != null)
            {
                if (arretrato.hdnSalva == "S")
                {
                    utente.SalvaArretrati(radio, txtDataDenuncia, listaarretrati, ref ErrorMSG, ref SuccessMSG);
                }
                HttpContext.Session["tbArretrati"] = null;
                return RedirectToAction("RicercaArretrati");
            }
            if (hdnSalva == "S")
            {
                if (arrayNascosto != null)
                {
                    string[] arrayArretrati = arrayNascosto.Split(',');
                    for (int x = 0; x < arrayArretrati.Length; x++)
                    {
                        DenunciaArretrati salvaArretrati = new DenunciaArretrati();
                        string[] arrayListaDenunce = arrayArretrati[x].Split('-');
                        salvaArretrati.mat = Convert.ToInt32(arrayListaDenunce[0]);
                        salvaArretrati.impret = arrayListaDenunce[1].ToString().Replace(".", ",");
                        salvaArretrati.impocc = arrayListaDenunce[2].ToString().Replace(".", ",");
                        salvaArretrati.impcon = arrayListaDenunce[3].ToString().Replace(".", ",");
                        listaarretrati.Add(salvaArretrati);
                    }
                    arretrato = utente.SalvaArretrati(radio, txtDataDenuncia, listaarretrati, ref ErrorMSG, ref SuccessMSG);
                }
            }
            else
            {
                listaarretrati = utente.CaricaArretrati(txtAnno, hdnSalva, hdnAnnComp, radio, txtDataDenuncia, ListaDenunce, ref ErrorMSG, ref SuccessMSG);
            }
            arretrato.anno = int.Parse(txtAnno);
            base.ViewBag.annoSelez = true;
            if (arretrato.blnCommit)
            {
                base.TempData["successMessage"] = SuccessMSG;
                return RedirectToAction("RiepilogoArretrati", arretrato);
            }

            if (string.IsNullOrEmpty(arretrato.txtDataDenuncia))
            {
                if (!string.IsNullOrEmpty(datApe))
                    fillTxtDataDenuncia(datApe);
                if (!string.IsNullOrEmpty(txtDataDenuncia))
                    fillTxtDataDenuncia(txtDataDenuncia);

                void fillTxtDataDenuncia(string dataDenunciaDaConvertire)
                {
                    var dataDenunciaParseResult = DateTime.TryParse(dataDenunciaDaConvertire, CultureInfo.GetCultureInfo("it-IT"), DateTimeStyles.AssumeLocal, out var dataDenuncia);
                    arretrato.txtDataDenuncia = dataDenunciaParseResult ? dataDenuncia.ToString("dd/MM/yyyy") : DateTime.Today.ToString("dd/MM/yyyy");
                    ViewBag.IsDataDenunciaFromModifica = dataDenunciaParseResult;
                }
            }
            else
            {
                ViewBag.IsDataDenunciaFromModifica = false;
            }

            base.ViewBag.ListaDenunciaArretrati = listaarretrati;
            ViewBag.ListaAnniCompetenzaNonPrescritti =
                _dropdownBll.GetAnniCompetenzaNonPrescrittiFrom(ute.CodPosizione).ToSelectListItem();
            base.TempData["errorMessage"] = ErrorMSG;
            return View(arretrato);
        }

        public ActionResult RiepilogoArretrati(string codPos, int hdnAnno, int hdnMese, int hdnProden, string lblPeriodo)
        {
            DenunciaArretratiBLL utente = new DenunciaArretratiBLL();
            DenunciaArretrati arretrato = new DenunciaArretrati();
            List<DenunciaArretrati> listaarretrati = new List<DenunciaArretrati>();
            listaarretrati = utente.VediArretrati(codPos, hdnAnno, hdnMese, hdnProden);
            base.ViewBag.ListaDenunciaArretrati = listaarretrati;
            arretrato.DatApe = listaarretrati.First().DatApe;
            arretrato.anncom = listaarretrati.First().anno;
            arretrato.codpos = Convert.ToInt32(codPos);
            arretrato.hdnAnno = hdnAnno;
            arretrato.hdnMese = hdnMese;
            arretrato.hdnProden = hdnProden.ToString();
            arretrato.lblPeriodo = listaarretrati[0].lblPeriodo;
            return View(arretrato);
        }

        [HttpPost]
        public ActionResult CambioPagina(int anno, int mese, int proden, string btnIndietro, int anncom)
        {
            Utente ute = (Utente)base.HttpContext.Session["utente"];
            DenunciaArretratiBLL utente = new DenunciaArretratiBLL();
            DenunciaArretrati arretrato = new DenunciaArretrati();
            List<DenunciaArretrati> listaarretrati = new List<DenunciaArretrati>();
            if (btnIndietro != null)
            {
                base.HttpContext.Session["tbArretrati"] = null;
                return RedirectToAction("InsDenunciaArretrati");
            }
            return RedirectToAction("TotaleArretrati", new
            {
                hdnAnno = anno,
                hdnMese = mese,
                hdnProden = proden,
                parm = "",
                anncom = anncom,
                ricerca = false
            });
        }

        public ActionResult TotaleArretrati(int hdnAnno, int hdnMese, int hdnProden, string parm, string staDen, string mat, string anncom, bool ricerca, string ret = "")
        {
            string errorMsg = "";
            Utente utente = Session["utente"] as Utente;
            TotaleArretratiBLL totaleArretratiBll = new TotaleArretratiBLL();
            TotaleArretrati arretrato = totaleArretratiBll.TotaleArretrati(utente.CodPosizione, hdnAnno, hdnMese, hdnProden, ret, ref errorMsg);
            arretrato.isRicerca = ricerca;
            arretrato.parm = parm;
            arretrato.staDen = staDen;
            arretrato.mat = mat;
            arretrato.anncom = anncom;

            if (!string.IsNullOrWhiteSpace(errorMsg)) TempData["errorMsg"] = errorMsg;

            return View(arretrato);
        }

        [HttpPost]
        public ActionResult TotaleArretrati(int hdnAnno, int hdnMese, int hdnProden, string txtCrediti, string TEXTAREA1, bool ricerca, string parm, string staDen, string btnType, bool btnConfermaTotaliVisible, string mat, string anncom, string ret = "")
        {
            string ErrorMSG = "";
            string SuccessMSG = "";
            Utente ute = base.Session["utente"] as Utente;
            TotaleArretratiBLL totaleArretratiBll = new TotaleArretratiBLL();
            TEXTAREA1 = string.IsNullOrEmpty(TEXTAREA1) ? "" : TEXTAREA1.Trim();
            if (btnType == "SALVA_TOTALI")
            {
                totaleArretratiBll.SalvaTotaleArretrati(ute.CodPosizione, hdnAnno, hdnMese, hdnProden, txtCrediti, TEXTAREA1, ref ErrorMSG, ref SuccessMSG);
                TotaleArretrati arretrato = totaleArretratiBll.TotaleArretrati(ute.CodPosizione, hdnAnno, hdnMese, hdnProden, ret, ref ErrorMSG);
                base.TempData["successMessage"] = SuccessMSG;
                base.TempData["errorMessage"] = ErrorMSG;
                base.TempData["refresh"] = true;
                arretrato.anncom = anncom;
                if (!string.IsNullOrEmpty(SuccessMSG))
                {
                    arretrato.isRicerca = false;
                    arretrato.btnConfermaTotaliVisible = false;
                }
                else
                {
                    arretrato.isRicerca = ricerca;
                }
                return View(arretrato);
            }
            if (btnType == "DETTAGLIO")
            {
                if (!btnConfermaTotaliVisible)
                {
                    if (ricerca)
                    {
                        return RedirectToAction("ConsultazioneArretrati", new
                        {
                            anno = hdnAnno,
                            mese = hdnMese,
                            parm = parm,
                            staDen = staDen,
                            mat = mat,
                            proDen = hdnProden,
                            anncom = anncom,
                            ricerca = ricerca
                        });
                    }
                    return RedirectToAction("ConsultazioneArretrati", new
                    {
                        anno = hdnAnno,
                        mese = hdnMese,
                        parm = parm,
                        staDen = staDen,
                        mat = mat,
                        proDen = hdnProden,
                        anncom = anncom,
                        ricerca = false
                    });
                }
                return RedirectToAction("InsDenunciaArretrati");
            }
            // if (ricerca)
            // {
            //     return RedirectToAction("RicercaArretrati");
            // }
            // if (!ricerca)
            // {
            //     return RedirectToAction("Index", "Home");
            // }
            return RedirectToAction("RicercaArretrati");
        }

        public ActionResult RicercaArretrati(ParametriRicerca parametri)
        {
            try
            {
                Utente ute = (Utente)base.Session["utente"];
                AssociamentoEnpaia associamento = new AssociamentoEnpaia(ute, 2);
                base.Session["associamento"] = associamento;
                if (!associamento._isAssociated && ute.Tipo == "E")
                {
                    return RedirectToAction("Index", "UtenteEnpaia");
                }

                RicercaArretrati arretrati =
                    RicercaArretratiBLL.GetArretrati(1, ute.CodPosizione, null, "0", null, null, null, null, null);

                PagingAndFormattingArretrati(arretrati, 1, null, "0", "-1", "First Loading", false, true, false);
                SetupViewElementsForRicercaArretratiView(ute, arretrati);

                if (TempData["arretratoDeletingMessage"] != null)
                    ViewBag.arretratoDeletingMessage = TempData["arretratoDeletingMessage"];

                if (TempData["errorsInFile"] != null)
                {
                    ViewBag.ErrorsInFile = TempData["errorsInFile"];
                }

                return View(arretrati);

            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                return View();
            }
        }

        [HttpPost]
        public ActionResult RicercaArretrati(RicercaArretrati modelFromView,
            string ddlAnno, string ddlAnnCom, string txtMatricola, string parm, string proDen, string anno, string mat)
        {
            try
            {
                Utente utente = Session["utente"] as Utente;

                RicercaArretrati arretrati = RicercaArretratiBLL.GetArretrati(
                    modelFromView.selectedRadioButton, utente.CodPosizione, ddlAnno, ddlAnnCom, txtMatricola, parm, proDen, anno, mat);

                PagingAndFormattingArretrati(
                    arretrati, modelFromView.selectedRadioButton, txtMatricola, ddlAnnCom, ddlAnno, modelFromView.SelectedColumnForSorting, modelFromView.IsAnnoDenSelected,
                    modelFromView.IsAnnoComSelected, modelFromView.IsMatricolaSelected, modelFromView.IsSortingAscending, modelFromView.PageNumber, modelFromView.PageSize);

                SetupViewElementsForRicercaArretratiView(utente, arretrati);

                return View(arretrati);
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                return View();
            }
        }

        private void PagingAndFormattingArretrati(
            RicercaArretrati arretrati, int radioButtonVal, string txtMatricola, string ddlAnnCom, string ddlAnno, string selectedColumnForSorting, bool isAnnoDenSelected,
            bool isAnnoComSelected, bool isMatricolaSelected, bool isSortingAscending = false, int pageNumber = 1, double pageSize = 10)
        {
            if (arretrati.listaDatiRicerca != null)
            {
                SortArretrati();
                FormatDateOfArretrati();
                arretrati.listaDatiRicerca = GetPagedArretrati();

                void SortArretrati()
                {
                    switch (selectedColumnForSorting)
                    {
                        case "Anno Competenza":
                            {
                                arretrati.listaDatiRicerca = isSortingAscending
                                    ? arretrati.listaDatiRicerca.OrderBy(arretrato => arretrato.StaDen).ThenBy(arretrato => arretrato.AnnCom).ThenBy(arretrato => arretrato.MesDen).ToList()
                                    : arretrati.listaDatiRicerca.OrderByDescending(arretrato => arretrato.StaDen).ThenByDescending(arretrato => arretrato.AnnCom).ThenByDescending(arretrato => arretrato.MesDen).ToList();
                                break;
                            }
                        case "Anno Denuncia":
                            {
                                arretrati.listaDatiRicerca = isSortingAscending
                                    ? arretrati.listaDatiRicerca.OrderBy(arretrato => arretrato.AnnDen).ToList()
                                    : arretrati.listaDatiRicerca.OrderByDescending(arretrato => arretrato.AnnDen).ToList();
                                break;
                            }
                        case "Mese Denuncia":
                            {
                                arretrati.listaDatiRicerca = isSortingAscending
                                    ? arretrati.listaDatiRicerca.OrderBy(arretrato => arretrato.MesDen).ToList()
                                    : arretrati.listaDatiRicerca.OrderByDescending(arretrato => arretrato.MesDen).ToList();
                                break;
                            }
                        case "Retribuzioni":
                            {
                                arretrati.listaDatiRicerca = isSortingAscending
                                    ? arretrati.listaDatiRicerca.OrderBy(arretrato => arretrato.ImpRet).ToList()
                                    : arretrati.listaDatiRicerca.OrderByDescending(arretrato => arretrato.ImpRet).ToList();
                                break;
                            }
                        case "Occasionali":
                            {
                                arretrati.listaDatiRicerca = isSortingAscending
                                    ? arretrati.listaDatiRicerca.OrderBy(arretrato => arretrato.ImpOcc).ToList()
                                    : arretrati.listaDatiRicerca.OrderByDescending(arretrato => arretrato.ImpOcc).ToList();
                                break;
                            }
                        case "Contributi":
                            {
                                arretrati.listaDatiRicerca = isSortingAscending
                                    ? arretrati.listaDatiRicerca.OrderBy(arretrato => arretrato.ImpCon).ToList()
                                    : arretrati.listaDatiRicerca.OrderByDescending(arretrato => arretrato.ImpCon).ToList();
                                break;
                            }
                        case "Stato Denuncia":
                            {
                                arretrati.listaDatiRicerca = isSortingAscending
                                    ? arretrati.listaDatiRicerca.OrderBy(arretrato => arretrato.StaDen).ToList()
                                    : arretrati.listaDatiRicerca.OrderByDescending(arretrato => arretrato.StaDen).ToList();
                                break;
                            }
                        default:
                            {
                                arretrati.listaDatiRicerca = isSortingAscending
                                    ? arretrati.listaDatiRicerca.OrderBy(arretrato => arretrato.StaDen).ThenBy(arretrato => arretrato.AnnCom).ThenBy(arretrato => arretrato.MesDen).ToList()
                                    : arretrati.listaDatiRicerca.OrderByDescending(arretrato => arretrato.StaDen).ThenByDescending(arretrato => arretrato.AnnCom).ThenByDescending(arretrato => arretrato.MesDen).ToList();
                                selectedColumnForSorting = "Anno Competenza";
                                break;
                            }
                    }
                }

                void FormatDateOfArretrati()
                {
                    foreach (RicercaArretrati_Data arretrato in arretrati.listaDatiRicerca)
                    {
                        if (!string.IsNullOrEmpty(arretrato.Al))
                            arretrato.Al = DateTime.Parse(arretrato.Al).ToString("d");
                        if (!string.IsNullOrEmpty(arretrato.Dal))
                            arretrato.Dal = DateTime.Parse(arretrato.Dal).ToString("d");
                        if (!string.IsNullOrEmpty(arretrato.DatChi))
                            arretrato.DatChi = DateTime.Parse(arretrato.DatChi).ToString("d");
                        if (!string.IsNullOrEmpty(arretrato.DatApe))
                            arretrato.DatApe = DateTime.Parse(arretrato.DatApe).ToString("d");
                    }
                }

                List<RicercaArretrati_Data> GetPagedArretrati()
                {
                    arretrati.TotalPages = (int)Math.Ceiling(arretrati.listaDatiRicerca.Count / pageSize);
                    if (arretrati.TotalPages < pageNumber)
                        pageNumber = 1;
                    return arretrati.listaDatiRicerca.ToPagedList(pageNumber, (int)pageSize).ToList();
                }
            }
            else
            {
                arretrati.IsArretratiListEmpty = true;
                arretrati.ModVisualizzazione = string.Empty;
            }

            SetArretratiPropertiesForSortingPagingAndRicerca();

            void SetArretratiPropertiesForSortingPagingAndRicerca()
            {
                arretrati.SelectedColumnForSorting = selectedColumnForSorting;
                arretrati.IsSortingAscending = isSortingAscending;
                arretrati.PageNumber = pageNumber;
                arretrati.PageSize = (int)pageSize;
                arretrati.IsAnnoDenSelected = isAnnoDenSelected;
                arretrati.IsAnnoComSelected = isAnnoComSelected;
                arretrati.IsMatricolaSelected = isMatricolaSelected;
                arretrati.selectedRadioButton = radioButtonVal;
                arretrati.SelectedAnnoDen = int.TryParse(ddlAnno, out int selectedAnnoDen) ? selectedAnnoDen : -1;
                arretrati.SelectedAnnoComp = int.TryParse(ddlAnnCom, out int selectedAnnoComp) ? selectedAnnoComp : -1;
                arretrati.txtMatricola = int.TryParse(txtMatricola, out int textMatricola) ? textMatricola : 0;
            }
        }

        private void SetupViewElementsForRicercaArretratiView(Utente utente, RicercaArretrati arretrati)
        {
            RicercaArretrati ddl = RicercaArretratiBLL.CaricaDDL(utente.CodPosizione);
            List<SelectListItem> listaAnniComp = new List<SelectListItem>();
            List<SelectListItem> listaAnniDen = new List<SelectListItem>();
            SetupViewBagForRicercaArretrati(arretrati.SelectedAnnoComp.ToString());
            arretrati.IsAnniCompetenzaListEmpty = ddl.IsAnniCompetenzaListEmpty;

            void SetupViewBagForRicercaArretrati(string ddlAnnCom)
            {
                ValorizeSelectLists();
                RemoveEmptyItemsFromSelectLists();

                ViewBag.ListaAnniComp = listaAnniComp;
                ViewBag.ListaAnniDen = listaAnniDen;
                ViewBag.ListaAnniCompetenzaNonPrescritti =
                    _dropdownBll.GetAnniCompetenzaNonPrescrittiFrom(utente.CodPosizione).ToSelectListItem();

                void ValorizeSelectLists()
                {
                    listaAnniComp.Add(new SelectListItem
                    {
                        Value = "-1",
                        Selected = false,
                        Text = "Seleziona anno competenza..."
                    });
                    listaAnniDen.Add(new SelectListItem
                    {
                        Value = "-1",
                        Selected = true,
                        Text = "Seleziona anno denuncia..."
                    });

                    if (ddl.listaAnniCompetenza != null && ddl.listaAnniCompetenza.Count > 0)
                    {
                        foreach (CaricamentoAnniDenuncia item2 in ddl.listaAnniCompetenza)
                        {
                            SelectListItem AnniComp = new SelectListItem();
                            AnniComp.Value = item2.valore.ToString();
                            AnniComp.Text = item2.testo;
                            listaAnniComp.Add(AnniComp);
                        }
                    }
                    else
                    {
                        ddl.IsAnniCompetenzaListEmpty = true;
                    }
                    if (ddl.listaAnniDenuncia != null && ddl.listaAnniDenuncia.Count > 0)
                    {
                        foreach (CaricamentoAnniDenuncia item in ddl.listaAnniDenuncia)
                        {
                            SelectListItem anniDenuncia = new SelectListItem();
                            anniDenuncia.Value = item.valore.ToString();
                            anniDenuncia.Text = item.testo;
                            listaAnniDen.Add(anniDenuncia);
                        }
                    }

                    if (listaAnniComp.Count() > 1)
                        listaAnniComp.Find(anno => anno.Value == ddlAnnCom).Selected = true;
                    else
                        listaAnniComp.First(anno => anno.Value == "-1").Selected = true;
                }

                void RemoveEmptyItemsFromSelectLists()
                {
                    List<SelectListItem> remove = listaAnniDen.Where((SelectListItem l) => l.Text == " " || l.Text == string.Empty).ToList();
                    if (remove != null && remove.Count > 0)
                    {
                        remove.ForEach(delegate (SelectListItem r)
                        {
                            listaAnniDen.Remove(r);
                        });
                    }
                    remove = listaAnniComp.Where((SelectListItem l) => l.Text == " " || l.Text == string.Empty).ToList();
                    if (remove != null && remove.Count > 0)
                    {
                        remove.ForEach(delegate (SelectListItem r)
                        {
                            listaAnniComp.Remove(r);
                        });
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult VerificaArretrati(string selectedYear, string selectedMonth, string selectedProgressivo, string selectedDescrizione, string selectedParametro, string selectedMatricola, string selectedAnnoCompetenza, string btnChiudi, string btnRettifiche)
        {
            if (btnChiudi == "CHIUDI")
            {
                return RedirectToAction("DenunciaMensile_Index");
            }
            if (btnRettifiche == "RETTIFICHE")
            {
                return RedirectToAction("RettificheArretrati", new
                {
                    anno = selectedYear,
                    mese = selectedMonth,
                    parm = selectedParametro,
                    staDen = selectedDescrizione,
                    mat = selectedMatricola,
                    proDen = selectedProgressivo,
                    anncom = selectedAnnoCompetenza,
                    ricerca = true
                });
            }
            return RedirectToAction("ConsultazioneArretrati", new
            {
                anno = selectedYear,
                mese = selectedMonth,
                parm = selectedParametro,
                staDen = selectedDescrizione,
                mat = selectedMatricola,
                proDen = selectedProgressivo,
                anncom = selectedAnnoCompetenza,
                ricerca = true
            });
        }

        public ActionResult ConsultazioneArretrati(string anno, string mese, string parm, string staDen, string mat, string proDen, string anncom, bool ricerca)
        {
            Utente ute = (Utente)base.Session["utente"];
            string CodPos = ute.CodPosizione;
            ConsultazioneArretrati arretrati = new ConsultazioneArretrati();
            arretrati = ConsultazioneArretratiBLL.LoadData(CodPos, anno, mese, parm, staDen, proDen, mat, anncom);
            arretrati.btnTotaliVisible = ricerca;
            arretrati.anno = anno;
            arretrati.mese = mese;
            arretrati.proDen = proDen;
            arretrati.parm = parm;
            arretrati.matricola = mat;
            arretrati.staDen = staDen;
            arretrati.isRicerca = ricerca;
            return View(arretrati);
        }

        [HttpPost]
        public ActionResult ConsultazioneArretrati(string anno, string mese, string parm, string staDen, string mat, string proDen, string anncom, bool ricerca, string btnChiudi, string btnRettifiche)
        {
            Utente ute = (Utente)base.Session["utente"];
            if (btnChiudi == "CHIUDI")
            {
                if (!ricerca)
                {
                    return RedirectToAction("DenunciaMensile_Index");
                }
                return RedirectToAction("RicercaArretrati");
            }
            if (btnRettifiche == "RETTIFICHE")
            {
                return RedirectToAction("RettificheArretrati");
            }
            return RedirectToAction("TotaleArretrati", new
            {
                hdnAnno = anno,
                hdnMese = mese,
                hdnProden = proDen,
                parm = parm,
                staDen = staDen,
                mat = mat,
                anncom = anncom,
                ricerca = true
            });
        }

        public ActionResult RettificheArretrati(string anno, string mese, string proDen, string anncom, string mat)
        {
            Utente ute = (Utente)base.Session["utente"];
            string CodPos = ute.CodPosizione;
            RettificheArretrati arretrati = new RettificheArretrati();
            arretrati = RicercaArretratiBLL.LoadDataRettifiche(CodPos, anno, mese, proDen, anncom, mat);
            arretrati.hdnAnno = anno;
            arretrati.hdnMese = mese;
            arretrati.hdnProDen = proDen;
            arretrati.hdnMat = mat;
            arretrati.hdnAnncom = anncom;
            return View(arretrati);
        }

        [HttpPost]
        public ActionResult RettificheArretrati(string anno, string mese, string proDen, string anncom, string mat, string btnScelta)
        {
            Utente ute = (Utente)base.Session["utente"];
            string CodPos = ute.CodPosizione;
            RettificheArretrati arretrati = new RettificheArretrati();
            if (btnScelta == "Totali")
            {
                return RedirectToAction("TotaleArretrati", new
                {
                    hdnAnno = anno,
                    hdnMese = mese,
                    hdnProden = proDen,
                    mat = mat,
                    anncom = anncom,
                    ricerca = true,
                    ret = "S"
                });
            }
            if (btnScelta == "Arretrato Originale")
            {
                return RedirectToAction("ConsultazioneArretrati", new
                {
                    anno = anno,
                    mese = mese,
                    mat = mat,
                    proDen = proDen,
                    anncom = anncom,
                    ricerca = true
                });
            }
            return RedirectToAction("RicercaArretrati");
        }

        public ActionResult UploadArretrati()
        {
            Utente ute = Session["utente"] as Utente;

            AssociamentoEnpaia associamento = new AssociamentoEnpaia(ute, 3);
            Session["associamento"] = associamento;
            if (!associamento._isAssociated && ute.Tipo == "E")
            {
                return RedirectToAction("Index", "UtenteEnpaia");
            }

            if (TempData["esitoUpload"] == null || !(bool)base.TempData["esitoUpload"])
                ViewBag.showModal = 1;
            else
                ViewBag.showModal = 0;

            if (TempData["errorsInFile"] != null)
            {
                ViewBag.showModal = false;
                ViewBag.ErrorsInFile = TempData["errorsInFile"];
            }

            return View();
        }

        [HttpPost]
        public ActionResult UploadArretrati(string anno, HttpPostedFileBase fileUpload, string proDen)
        {
            string logErrori = "";
            Utente utente = base.Session["utente"] as Utente;
            bool btnStampaIsVisible = false;
            bool btnConfermaIsVisible = true;
            try
            {
                if (fileUpload != null)
                {
                    if (fileUpload.ContentLength > 0)
                    {
                        string path = base.Server.MapPath("~/Upload/" + utente.CodPosizione + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Year + ".txt");
                        if (RicercaArretratiBLL.UploadArretrati(utente, Convert.ToInt32(anno), fileUpload, path, ref proDen, ref btnStampaIsVisible, ref btnConfermaIsVisible))
                        {
                            base.TempData["successMessage"] = DenunciaMensileBLL.SuccessMessage;
                            return RedirectToAction("UploadArretrati");
                        }
                        base.TempData["errorMessage"] = DenunciaMensileBLL.ErrorMessage;
                        return RedirectToAction("UploadArretrati", new
                        {
                            annoSelezionato = anno,
                            isFirstLoading = false
                        });
                    }
                    base.TempData["errorMessage"] = "Non è stato possibile effettuare l'upload del file!";
                    return RedirectToAction("UploadArretrati", new
                    {
                        annoSelezionato = anno,
                        isFirstLoading = false
                    });
                }
                base.TempData["errorMessage"] = "Non è stato possibile effettuare l'upload del file!";
                return RedirectToAction("UploadArretrati", new
                {
                    annoSelezionato = anno,
                    isFirstLoading = false
                });
            }
            catch (Exception ex)
            {
                log.Info($"[TFI.BLL] : SalvaDIPA_upload() - Durante la fase di salvataggio è stata generata un'eccezione in data: {DateTime.Now}");
                base.TempData["errorMessage"] = "Caricamento della denuncia ddl non riuscito! " + Environment.NewLine + " " + ex.Message;
                return RedirectToAction("UploadArretrati", new
                {
                    annoSelezionato = anno,
                    isFirstLoading = false
                });
            }
        }

        public ActionResult EliminaArretrato(int aDen, int mDen, int pDen)
        {
            Utente utente = Session["utente"] as Utente;
            try
            {
                if (DenunciaMensileBLL.EliminaArretrato(utente, aDen, mDen, pDen))
                {
                    TempData["arretratoDeletingMessage"] = "Arretrato eliminato con successo";
                }
                else
                {
                    TempData["arretratoDeletingMessage"] = "Si sono verificati dei problemi nella concellazione dell'arretrato";
                }
                return RedirectToAction("RicercaArretrati");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult UploadArretrato(int anno, HttpPostedFileBase fileUpload)
        {
            Utente user = Session["utente"] as Utente;
            DenunciaArretratiBLL denArrBll = new DenunciaArretratiBLL();
            bool result = false;
            try
            {
                string strDataDal = $"01/01/{anno}";
                string strDataAl = $"31/12/{anno}";
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    var validationResult = ArretratoFileValidator.CheckFileExtension(fileUpload);
                    if (!validationResult.IsSucceeded)
                    {
                        TempData["errorsInFile"] = validationResult.Errors;
                        ViewBag.ErrorsInFile = validationResult.Errors;
                        return RedirectToAction("RicercaArretrati");
                    }

                    var fileContent = fileUpload.ReadFileUploaded();
                    List<DenunciaArretrati> listaDenunciaArretrati = denArrBll.GeneraDenunciaArr(strDataDal, strDataAl, user.CodPosizione, string.Empty, true);

                    validationResult = ArretratoFileValidator.ReadAndValidateArretratoUploadFile(fileContent, anno, listaDenunciaArretrati);
                    if (!validationResult.IsSucceeded)
                    {
                        validationResult.Errors.Sort(StringComparer.InvariantCultureIgnoreCase);
                        TempData["errorsInFile"] = validationResult.Errors;
                        ViewBag.ErrorsInFile = validationResult.Errors;
                        return RedirectToAction("RicercaArretrati");
                    }
                    result = denArrBll.UploadArretrato(user, fileContent, anno, listaDenunciaArretrati);
                }

                if (result)
                {
                    TempData["successMessage"] = "Operazione completata con successo!";
                }
                else
                {
                    TempData["errorMessage"] = "Si è verificato un'errore durante il caricamento dell'arretrato.";
                }
                return RedirectToAction("RicercaArretrati");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult ModificaDeleghe(string datini, string asster, string codter, string assnaz, string codnaz, string stato, string codpos, string ragsoc)
        {
            AziDelegheBLL delegheBLL = new AziDelegheBLL();
            DelegheOCM delegheOCM = new DelegheOCM();
            delegheOCM = delegheBLL.GetAssociazione(delegheOCM, codter, codnaz, codpos, assnaz, asster);
            delegheOCM.datidelega.codNaz = codnaz;
            delegheOCM.datidelega.codTer = codter;
            return View(delegheOCM);
        }

        [HttpPost]
        public ActionResult ModificaDeleghe(DelegheOCM.DatiDelega Deleghe)
        {
            Utente u = base.Session["utente"] as Utente;
            AziDelegheBLL delegheBLL = new AziDelegheBLL();
            DelegheOCM delegheOCM = new DelegheOCM();
            delegheOCM.datidelega = Deleghe;
            string MSGErrore = "";
            string MSGSuccess = "";
            string strModo = Deleghe.strModo;
            string codTer = Deleghe.codTer;
            string codpos = u.CodPosizione;
            delegheBLL.salvdelegheBll(ref MSGErrore, ref MSGSuccess, strModo, codTer, codpos, u, Deleghe);
            if (MSGErrore != "")
            {
                base.TempData["errorMessage"] = MSGErrore;
            }
            else if (MSGSuccess != "")
            {
                base.TempData["successMessage"] = MSGSuccess;
            }
            return View(delegheOCM);
        }

        public ActionResult InserimentoDeleghe()
        {
            AziDelegheBLL delegheBLL = new AziDelegheBLL();
            DelegheOCM delegheOCM = new DelegheOCM();
            delegheOCM = delegheBLL.AssNaz();
            base.Session["AssociazioneNaz"] = delegheOCM;
            base.Session["ListDen"] = delegheOCM;
            base.Session["ListaIndirizzi"] = delegheOCM;
            return View(delegheOCM);
        }

        [HttpPost]
        public ActionResult InserimentoDeleghe(DelegheOCM.DatiDelega Deleghe, string strModo, string codTer)
        {
            AziDelegheBLL delegheBLL = new AziDelegheBLL();
            DelegheOCM delegheOCM = new DelegheOCM();
            Utente u = base.Session["utente"] as Utente;
            DelegheOCM Liste = base.Session["AssociazioneNaz"] as DelegheOCM;
            DelegheOCM Liste2 = base.Session["ListDen"] as DelegheOCM;
            DelegheOCM Liste3 = base.Session["ListaIndirizzi"] as DelegheOCM;
            string codpos = u.CodPosizione;
            string MSGErrore = "";
            string MSGSuccess = "";
            delegheBLL.salvdelegheBll(ref MSGErrore, ref MSGSuccess, strModo, codTer, codpos, u, Deleghe);
            if (MSGErrore != "")
            {
                base.TempData["errorMessage"] = MSGErrore;
            }
            else if (MSGSuccess != "")
            {
                base.TempData["successMessage"] = MSGSuccess;
            }
            base.TempData["OTPDeleghe"] = Deleghe.OtpDeleghe;
            return RedirectToAction("InserimentoDeleghe", "AziendaConsulente");
        }

        public JsonResult CaricaIva(string codFis, string ParIva)
        {
            string MSGErrore = "";
            AziDelegheBLL delegheBLL = new AziDelegheBLL();
            DelegheOCM delegheOCM = new DelegheOCM();
            delegheOCM = delegheBLL.GetCaricaIva(ref MSGErrore, codFis, ParIva);
            string ragSoc = delegheOCM.datidelega.RagSocDe;
            string ragSocBr = delegheOCM.datidelega.RagSocBrDe;
            string pariva = delegheOCM.datidelega.ParIvaDe;
            string codfis = delegheOCM.datidelega.CodFisDe;
            string ind = delegheOCM.datidelega.IndDe;
            string ViaDe = delegheOCM.datidelega.ViaDe;
            string NumCivDe = delegheOCM.datidelega.NumCivDe;
            string citta = delegheOCM.datidelega.LocalitaDe;
            string comune = delegheOCM.datidelega.ComuneDe;
            string CodCom = delegheOCM.datidelega.CODCOM;
            string prov = delegheOCM.datidelega.PrDe;
            string cap = delegheOCM.datidelega.CapDe;
            string tel = delegheOCM.datidelega.TelDe;
            string fax = delegheOCM.datidelega.FaxDe;
            string cell = delegheOCM.datidelega.CelDe;
            string email = delegheOCM.datidelega.EmailDe;
            string emailcert = delegheOCM.datidelega.EmailCertDe;
            return Json(new
            {
                ragSocBr,
                ragSoc,
                pariva,
                codfis,
                ind,
                ViaDe,
                NumCivDe,
                citta,
                prov,
                cap,
                comune,
                CodCom,
                tel,
                fax,
                email,
                cell,
                emailcert
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DatiAssModifica(string codTer, string codNaz, string strModo, string posizione)
        {
            string assnaz = "";
            string asster = "";
            string MSGErrore = "";
            AziDelegheBLL delegheBLL = new AziDelegheBLL();
            DelegheOCM delegheOCM = new DelegheOCM();
            DelegheOCM delegheOCM2 = new DelegheOCM();
            delegheOCM = delegheBLL.GetAssociazione(delegheOCM2, codTer, codNaz, posizione, assnaz, asster);
            string ragSoc = delegheOCM.datidelega.RagSocDe;
            string ragSocBr = delegheOCM.datidelega.RagSocBrDe;
            string pariva = delegheOCM.datidelega.ParIvaDe;
            string codfis = delegheOCM.datidelega.CodFisDe;
            string ind = delegheOCM.datidelega.IndDe;
            string ViaDe = delegheOCM.datidelega.ViaDe;
            string NumCivDe = delegheOCM.datidelega.NumCivDe;
            string citta = delegheOCM.datidelega.LocalitaDe;
            string comune = delegheOCM.datidelega.ComuneDe;
            string CodCom = delegheOCM.datidelega.CODCOM;
            string prov = delegheOCM.datidelega.PrDe;
            string cap = delegheOCM.datidelega.CapDe;
            string tel = delegheOCM.datidelega.TelDe;
            string fax = delegheOCM.datidelega.FaxDe;
            string cell = delegheOCM.datidelega.CelDe;
            string email = delegheOCM.datidelega.EmailDe;
            string emailcert = delegheOCM.datidelega.EmailCertDe;
            return Json(new
            {
                ragSocBr,
                ragSoc,
                pariva,
                codfis,
                ind,
                ViaDe,
                NumCivDe,
                citta,
                prov,
                cap,
                comune,
                CodCom,
                tel,
                fax,
                email,
                cell,
                emailcert
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DatiAss(string codTer, string codNaz, string strModo)
        {
            string codpos = (string)base.Session["Posizione"];
            string MSGErrore = "";
            string assnaz = "";
            string asster = "";
            AziDelegheBLL delegheBLL = new AziDelegheBLL();
            DelegheOCM delegheOCM = new DelegheOCM();
            DelegheOCM delegheOCM2 = new DelegheOCM();
            delegheOCM = delegheBLL.GetAssociazione(delegheOCM2, codTer, codNaz, codpos, assnaz, asster);
            string ragSoc = delegheOCM.datidelega.RagSocDe;
            string ragSocBr = delegheOCM.datidelega.RagSocBrDe;
            string pariva = delegheOCM.datidelega.ParIvaDe;
            string codfis = delegheOCM.datidelega.CodFisDe;
            string ind = delegheOCM.datidelega.IndDe;
            string citta = delegheOCM.datidelega.LocalitaDe;
            string comune = delegheOCM.datidelega.ComuneDe;
            string CodCom = delegheOCM.datidelega.CODCOM;
            string prov = delegheOCM.datidelega.PrDe;
            string cap = delegheOCM.datidelega.CapDe;
            string tel = delegheOCM.datidelega.TelDe;
            string fax = delegheOCM.datidelega.FaxDe;
            string cell = delegheOCM.datidelega.CelDe;
            string email = delegheOCM.datidelega.EmailDe;
            string emailcert = delegheOCM.datidelega.EmailCertDe;
            return Json(new
            {
                ragSocBr,
                ragSoc,
                pariva,
                codfis,
                ind,
                citta,
                prov,
                cap,
                comune,
                CodCom,
                tel,
                fax,
                email,
                cell,
                emailcert
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Denominazione(string codNaz)
        {
            string MSGErrore = "";
            AziDelegheBLL delegheBLL = new AziDelegheBLL();
            DelegheOCM delegheOCM = new DelegheOCM();
            delegheOCM = delegheBLL.GetDenominazione(codNaz, ref MSGErrore);
            return Json(delegheOCM.ListDenominazione.Select((DelegheOCM.ListDen x) => new { x.RAGSOCASS, x.CODTER }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delega(string nuovaDel)
        {
            string ErroreMSG = "";
            string SuccessMSG = "";

            Utente ute = (Utente)Session["utente"];
            AziDelegheBLL delbll = new AziDelegheBLL();

            if (!string.IsNullOrEmpty(nuovaDel))
                return RedirectToAction("InserimentoDeleghe", "AziendaConsulente");

            var del = delbll.SearchDeleghe(ute.CodPosizione, ref ErroreMSG, ref SuccessMSG);

            if (ErroreMSG != "")
                TempData["errorMessage"] = ErroreMSG;
            else if (SuccessMSG != "")
                TempData["successMessage"] = SuccessMSG;
            Session["aziendaConDelegheAttive"] = delbll.HaDelegaAttiva(ute.CodPosizione);

            return View(del);
        }

        public ActionResult EliminaDelega(string codter)
        {
            try
            {
                AziDelegheBLL deleghe = new AziDelegheBLL();
                Utente u = base.Session["utente"] as Utente;
                string codpos = u.CodPosizione;

                var result = deleghe.EliminaDelega(codter, codpos, u.Username);

                if (!result)
                {
                    return RedirectToDelegaOperazioneNonRiuscita();
                }

                var delegaPerEsito = deleghe.GetDettaglioDelega(codter, codpos);
                TempData["successMessage"] = "Operazione avvenuta con successo";

                var sendEmailResult = deleghe.InviaEmailPerCambioStatoDelega(codter, codpos, "rifiutata");
                if (sendEmailResult.Succeded)
                {
                    delegaPerEsito.datidelega.IsFromLink = false;
                    return View("EsitoDelega", delegaPerEsito);
                }

                return RedirectToEsitoDelegaInvioEmailNonRiuscito(false, delegaPerEsito, sendEmailResult.Message);
            }
            catch (Exception e)
            {
                log.Error($"[TFI.BLL] : AttivaDelega() - è stata generata un'eccezione in data: {DateTime.Now} {e.Message}");
                return RedirectToDelegaOperazioneNonRiuscita();
            }
        }

        public ActionResult AttivaDelega(string codter)
        {
            try
            {
                AziDelegheBLL deleghe = new AziDelegheBLL();
                Utente u = base.Session["utente"] as Utente;
                string codpos = u.CodPosizione;

                var result = deleghe.AttivaDelega(codter, codpos, u.Username);

                if (!result)
                {
                    return RedirectToDelegaOperazioneNonRiuscita();
                }

                var delegaPerEsito = deleghe.GetDettaglioDelega(codter, codpos);
                TempData["successMessage"] = "Operazione avvenuta con successo";

                var sendEmailResult = deleghe.InviaEmailPerCambioStatoDelega(codter, codpos, "accettata");
                if (sendEmailResult.Succeded)
                {
                    delegaPerEsito.datidelega.IsFromLink = false;
                    return View("EsitoDelega", delegaPerEsito);
                }
                return RedirectToEsitoDelegaInvioEmailNonRiuscito(false, delegaPerEsito, sendEmailResult.Message);

            }
            catch (Exception e)
            {
                log.Error($"[TFI.BLL] : AttivaDelega() - è stata generata un'eccezione in data: {DateTime.Now} {e.Message}");
                return RedirectToDelegaOperazioneNonRiuscita();
            }
        }

        public bool CheckUnioneSospensioniIntersecanti(string dataDa, string dataAl, string matricola, string prorap, string motsosp, string prosos, string nominativo)
        {
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            string codpos = base.HttpContext.Session["Posizione"].ToString();
            return rapLavBll.CheckUnioneSospensioniIntersecanti(prosos, prorap, matricola, dataDa, dataAl, codpos, motsosp);
        }

        public JsonResult SalvaSospensioniAjax(string dataDa, string dataAl, string matricola, string prorap, string motsosp, string prosos, string nominativo)
        {
            Utente u = base.Session["utente"] as Utente;
            RapportiLavoroBLL rapLavBll = new RapportiLavoroBLL();
            string codpos = base.HttpContext.Session["Posizione"].ToString();
            string[] arr = nominativo.Split(' ');
            string cognome = arr[0];
            string nome = arr[1];
            string messaggio = "";
            string SuccessMSG = "";
            bool isError = false;
            rapLavBll.SalvaSospensioniBLL(dataDa, dataAl, motsosp, matricola, prorap, codpos, u, prosos, ref messaggio, ref SuccessMSG);
            if (messaggio != "")
            {
                isError = true;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
                isError = false;
            }
            return Json(new { matricola, prorap, cognome, nome, messaggio, isError }, JsonRequestBehavior.AllowGet);
        }

        [OverrideActionFilters]
        public ActionResult DettaglioDelega(string codTer, string token)
        {
            string posAz = string.Empty;
            DelegheOCM delegaToShow;
            AziDelegheBLL aziDelegheBLL = new AziDelegheBLL();

            if (!string.IsNullOrWhiteSpace(token))
            {
                var tokenValues = TokenHelper.ValidateToken(token, "codTer", "codPos");
                var isCodTerInToken = tokenValues.TryGetValue("codTer", out codTer);
                var isCodPosInToken = tokenValues.TryGetValue("codPos", out posAz);

                if (!isCodTerInToken || !isCodPosInToken)
                    return RedirectToLoginLinkNonValido();

                delegaToShow = new DelegheOCM();
                delegaToShow = aziDelegheBLL.GetDettaglioDelega(codTer, posAz);

                if (!delegaToShow.datidelega.IsInAttesa)
                {
                    TempData["errorMessage"] = "Delega già gestita, per vedere nel dettaglio effettuare il login";
                    return RedirectToAction("Login", "Login", new { tipoUtente = "A" });
                }

                //TempData["token"] = token;
                delegaToShow.datidelega.Token = token;
                return View("DettaglioDelegaLink", delegaToShow);
            }
            else
            {
                Utente ute = (Utente)base.Session["utente"];
                posAz = ute.CodPosizione;
                delegaToShow = aziDelegheBLL.GetDettaglioDelega(codTer, posAz);
                return View(delegaToShow);
            }
        }

        [OverrideActionFilters]
        public ActionResult AccettaDelegaOtp(string token)
        {
            try
            {
                AziDelegheBLL deleghe = new AziDelegheBLL();
                return AlteraStatoDelegaOtp(token, deleghe.AttivaDelega, "accettata");
            }
            catch (Exception e)
            {
                return RedirectToLoginOperazioneNonRiuscitaWithLogErrorException(e.Message);
            }

        }

        [OverrideActionFilters]
        public ActionResult RifiutaDelegaOtp(string token)
        {
            try
            {
                AziDelegheBLL deleghe = new AziDelegheBLL();
                return AlteraStatoDelegaOtp(token, deleghe.EliminaDelega, "rifiutata");
            }
            catch (Exception e)
            {
                return RedirectToLoginOperazioneNonRiuscitaWithLogErrorException(e.Message);
            }
        }

        private ActionResult AlteraStatoDelegaOtp(string token, Func<string, string, string, bool> alteraStatoDelega, string operazione)
        {
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToLoginLinkNonValido();

            var tokenValues = TokenHelper.ValidateToken(token, "codTer", "codPos");
            var isCodTerInToken = tokenValues.TryGetValue("codTer", out var codTer);
            var isCodPosInToken = tokenValues.TryGetValue("codPos", out var codPos);

            if (!isCodTerInToken || !isCodPosInToken)
                return RedirectToLoginLinkNonValido();

            var alteraStatoDelegaResult = alteraStatoDelega(codTer, codPos, codPos);

            if (alteraStatoDelegaResult)
                TempData["successMessage"] = "Operazione avvenuta con successo";
            else
                TempData["errorMessage"] = "Operazione non riuscita";

            AziDelegheBLL delegheBLL = new AziDelegheBLL();
            var delegaPerEsito = delegheBLL.GetDettaglioDelega(codTer, codPos);

            if (!alteraStatoDelegaResult)
                return RedirectToAction("Login", "Login", new { tipoUtente = "A" });

            var sendEmailResult = delegheBLL.InviaEmailPerCambioStatoDelega(codTer, codPos, operazione);
            if (sendEmailResult.Succeded)
            {
                delegaPerEsito.datidelega.IsFromLink = true;
                return View("EsitoDelega", delegaPerEsito);
            }

            return RedirectToEsitoDelegaInvioEmailNonRiuscito(true, delegaPerEsito, sendEmailResult.Message);
        }

        private RedirectToRouteResult RedirectToLoginLinkNonValido()
        {
            TempData["errorMessage"] = "Link non più valido";
            return RedirectToAction("Login", "Login", new { tipoUtente = "A" });
        }

        private ViewResult RedirectToEsitoDelegaInvioEmailNonRiuscito(bool isFromLink, DelegheOCM delegaPerEsito, string emailResultMessage)
        {
            delegaPerEsito.datidelega.IsFromLink = isFromLink;
            TempData["errorMessage"] = "Invio email non riuscito";
            log.Info($"[TFI.BLL] : AttivaDelega() - è stata generata un'eccezione in data: {DateTime.Now} {emailResultMessage}");
            return View("EsitoDelega", delegaPerEsito);
        }

        private RedirectToRouteResult RedirectToDelegaOperazioneNonRiuscita()
        {
            TempData["errorMessage"] = "Operazione non riuscita";
            return RedirectToAction("Delega");
        }

        private RedirectToRouteResult RedirectToLoginOperazioneNonRiuscitaWithLogErrorException(string exceptionMessage)
        {
            TempData["errorMessage"] = "Operazione non riuscita";
            log.Error($"[TFI.BLL] : AttivaDelega() - è stata generata un'eccezione in data: {DateTime.Now} {exceptionMessage}");
            return RedirectToAction("Login", "Login", new { tipoUtente = "A" });
        }

        private void CalcolaImpostaImporti(DatiNuovaDenuncia datiDenuncia)
        {
            datiDenuncia.Imponibile = Math.Round(datiDenuncia.ListaReport.Sum(report => report.ImpRet), 2).ToString();
            datiDenuncia.Occasionali = Math.Round(datiDenuncia.ListaReport.Sum(report => report.ImpOcc), 2).ToString();
            datiDenuncia.Figurative = Math.Round(datiDenuncia.ListaReport.Sum(report => report.ImpFig), 2).ToString();
            datiDenuncia.Contributi = Math.Round(datiDenuncia.ListaReport.Sum(report => report.ImpRet * report.Aliquota / 100), 2).ToString();
        }

        public ActionResult CreaPagamentoPa(int anno = 0, int mese = 0, int progressivo = 0, string tipMov = default)
        {
            Utente utente = Session["utente"] as Utente;
            var codPos = utente.CodPosizione;
            try
            {
                var flussoPagoPa = new PagoPa(anno, mese, progressivo, string.Empty, string.Empty, codPos, string.Empty, codPos);
                var urlCallBack = CreaUrl();

                var resp = flussoPagoPa.CreaPagamento(urlCallBack, tipMov);
                if (resp.Esito == "OK")
                {
                    string redirectUrlPagoPa = $"{flussoPagoPa.PagoPaSection["PathRedirectPagamento"]}?iuvCode={resp.Data.IuvCode}&transActionId={resp.Data.TransActionId}";
                    return Redirect(redirectUrlPagoPa);
                }

                TempData["errorMessage"] = resp.Message;
                return RedirectToAction("DenunciaMensile_Totali", new { anno, mese, proDen = progressivo, isFirstLoading = true });

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = $"Attenzione si e' verificato il seguente errore:{ex.Message.Replace("'", "\'")}";
                log.Error($"AziendaConsulenteController - CreaPagamentoPa():  stata generata un'eccezione in data: {DateTime.Now} {ex.Message}");
                return RedirectToAction("DenunciaMensile_Totali", new { anno, mese, proDen = progressivo, isFirstLoading = true });
            }

            string CreaUrl()
            {
                var baseUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}";
                if (tipMov == "DP")
                    return $"{baseUrl}/AziendaConsulente/DenunciaMensile_Totali?anno={anno}&mese={mese}&proDen={progressivo}&isFirstLoading=True";
                if (tipMov == "AR")
                    return $"{baseUrl}/AziendaConsulente/TotaleArretrati?hdnAnno={anno}&hdnMese={mese}&hdnProden={progressivo}&ricerca=True";
                return baseUrl;
            }
        }

        public FileResult StampaBollettino(string iuvCode, string tranActionId, string anno, string mese, string progressivo)
        {
            Utente utente = Session["utente"] as Utente;
            var codPos = utente.CodPosizione;

            var flussoPagoPa = new PagoPa(int.Parse(anno), int.Parse(mese), int.Parse(progressivo), iuvCode, tranActionId,
                codPos, string.Empty, codPos);
            try
            {
                var resp = flussoPagoPa.GetBollettino();
                if (resp.Esito == "OK")
                {
                    string nomeFile = $"AvvisoPagamentoEnpaia_{codPos}{anno}{mese}{progressivo}.pdf";
                    var byteInfo = Convert.FromBase64String(resp.Data.base64PDF);
                    MemoryStream workStream = new MemoryStream();
                    workStream.Write(byteInfo, 0, byteInfo.Length);
                    workStream.Position = 0;

                    return new FileStreamResult(workStream, "application/pdf");
                }
                log.Error($"AziendaConsulenteController - StampaBollettino(): si e\\' verificato un errore durante la stampa dell'avviso PagoPA\"");
                return null;
            }
            catch (Exception ex)
            {
                log.Error($"AziendaConsulenteController - StampaBollettino(): si e\\' verificato un errore durante la stampa dell'avviso PagoPA\"");
                return null;
            }
        }

        public ActionResult PagaOnLinePagoPA(string iuvCode, string tranActionId)
        {
            var pagoPa = new PagoPa();
            return Redirect($"{pagoPa.PagoPaSection["PathRedirectPagamento"]}?iuvCode={iuvCode}&transActionId={tranActionId}");
        }
      
        public FileResult StampaDipa(string anno, string mese, int proDen)
        {
            Utente utente = Session["utente"] as Utente;
            var timestamp = "";
            var datiDenuncia = DenunciaMensileBLL.CaricaNuovaDenunciaMensile(utente, anno, mese, proDen, 0, string.Empty,
                out timestamp);
            var pdfService = new PdfService();
            var byteInfo = pdfService.StampaDipa(datiDenuncia, utente.CodPosizione);
            MemoryStream workStream = new MemoryStream();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            return new FileStreamResult(workStream, "application/pdf");
        }
        public ActionResult StampaRicevutaDipa(int mese, int anno, int proDen)
        {
            var utente = Session["utente"] as Utente;
            var codPos = utente.CodPosizione;
            var ricevutaDipa = DenunciaMensileBLL.GetRicevutaDipa(proDen, mese, anno, codPos);

            if (!ricevutaDipa.IsSuccessfull)
            {
                TempData["errorMessage"] = ricevutaDipa.Message;
                return RedirectToAction("DenunciaMensile_Totali", new
                {
                    anno = anno,
                    mese = mese,
                    proDen = proDen,
                    isFirstLoading = true
                });
            }

            MemoryStream workStream = new MemoryStream();
            workStream.Write(ricevutaDipa.Content, 0, ricevutaDipa.Content.Length);
            workStream.Position = 0;
            return new FileStreamResult(workStream, "application/pdf");
        }

        public FileResult StampaArretrati(string anno, string mese, string parm, string staDen, string proDen, string mat, string annCom)
        {
            try
            {
                var utente = Session["utente"] as Utente;
                var codPos = utente.CodPosizione;
                var denunciaArretrati = ConsultazioneArretratiBLL.LoadData(codPos, anno, mese, parm, staDen, proDen, mat, annCom).listaSelezione;
                var posizioneAssicurativaAzienda = aziendaBLL.GetPosAss(codPos);
                var pdfArretratoService = new PdfArretratoService();
                var byteInfo = pdfArretratoService.CreaStampaArretrati(denunciaArretrati, posizioneAssicurativaAzienda, int.Parse(anno), int.Parse(mese), int.Parse(proDen));
                var workStream = new MemoryStream();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;
                return new FileStreamResult(workStream, "application/pdf");
            }
            catch (Exception e)
            {
                log.Info($"[TFI] Stampa Arretrati ha generato un'eccezione il {DateTime.Now} con messaggio {e.Message} e StackTrace {e.StackTrace}");
                return new FileContentResult(new byte[] { }, "application/pdf");
            }
        }

        public ActionResult StampaRicevutaArretrato(int anno, int mese, int proDen, int annCom)
        {
            var utente = Session["utente"] as Utente;
            var codPos = utente.CodPosizione;
            var denominazioneAzienda = utente.DescrizioneAzienda;
            var result = TotaleArretratiBLL.GetRicevutaArretrato(anno, mese, proDen, annCom, codPos, denominazioneAzienda);

            if (!result.IsSuccessfull)
            {
                TempData["errorMessage"] = result.Message;
                return RedirectToAction("TotaleArretrati", new
                {
                    hdnAnno = anno,
                    hdnMese = mese,
                    hdnProden = proDen,
                    parm = "",
                    anncom = annCom,
                    ricerca = false
                });
            }

            MemoryStream workStream = new MemoryStream();
            workStream.Write(result.Content, 0, result.Content.Length);
            workStream.Position = 0;
            return new FileStreamResult(workStream, "application/pdf");
        }

    }
}