using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using log4net;
using Newtonsoft.Json;
using TFI.BLL.Amministrativo;
using TFI.BLL.Crypto;
using TFI.BLL.Utilities;
using TFI.OCM.Amministrativo;
using TFI.OCM.Utente;
using TFI.Utilities;
using ClosedXML.Excel;
using static TFI.OCM.Amministrativo.GestioneRapportiLavoroIscrittiOCM;

namespace TFI.Controllers
{
    // [CheckLogin]
    [UserExpiredCheck]
    public class AmministrativoController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger("RollingFile");

        private static readonly ILog TrackLog = LogManager.GetLogger("Track");

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Scadenzario()
        {
            return View();
        }

        public ActionResult Scadenzario_Infortuni()
        {
            return View();
        }

        public ActionResult ScadenzarioFondo()
        {
            return View();
        }

        public ActionResult Liquidazione()
        {
            return View();
        }

        public ActionResult ListaUtenti()
        {
            return View();
        }

        public ActionResult ProspettoCartaEnpaia()
        {
            return View();
        }

        public ActionResult ProspettoPREV()
        {
            return View();
        }

        public ActionResult ProspettoFondo()
        {
            return View();
        }

        public ActionResult Anagrafica()
        {
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
            return View();
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

        public ActionResult RicercaDiffide()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RicercaDiffide(DiffideOCM.Diffide Diff)
        {
            DiffideBLL diffideBLL = new DiffideBLL();
            DiffideOCM oCM = new DiffideOCM();
            string MsgErrore = " ";
            string CodPos = Diff.CodPos;
            string Anno = Diff.Anno;
            diffideBLL.CercaDiffide(oCM, CodPos, Anno, ref MsgErrore);
            if (MsgErrore != " ")
            {
                base.TempData["errorMessage"] = MsgErrore;
            }
            return View(oCM);
        }

        public ActionResult RicercaTrasferimentiRDL()
        {
            TrasferimentiRdlOCM trasferimentiRdlOCM = new TrasferimentiRdlOCM();
            if (base.TempData["Ricerca"] != null)
            {
                trasferimentiRdlOCM = (TrasferimentiRdlOCM)base.TempData["Ricerca"];
                base.TempData.Keep("Ricerca");
                return View(trasferimentiRdlOCM);
            }
            return View();
        }

        [HttpPost]
        public ActionResult RicercaTrasferimentiRDL(TrasferimentiRdlOCM.DatiTrasferimento rdl)
        {
            string ErroreMSG = "";
            string SuccessMSG = "";
            TrasferimentiRdlOCM trasferimentiRdlocm = new TrasferimentiRdlOCM();
            TrasferimentiRdlBll trasferimentiRdlBll = new TrasferimentiRdlBll();
            trasferimentiRdlocm = trasferimentiRdlBll.CaricaDatiRicercaTrasferimenti(rdl, ref ErroreMSG, ref SuccessMSG);
            base.TempData["Ricerca"] = trasferimentiRdlocm;
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return View(trasferimentiRdlocm);
        }

        public ActionResult AnnullamentoTrasferimenti(string CodPos, string CodPosTra, string RagSoc, string RagSocTra, string Matricola, string Nome, string Cognome, string ProRap, string ProRapTra, string ProTraRap, string Gruppo, string ProSos, string DatAnn)
        {
            Utente u = base.Session["utente"] as Utente;
            string ErroreMSG = "";
            string SuccessMSG = "";
            TrasferimentiRdlBll trasferimentiRdlBll = new TrasferimentiRdlBll();
            trasferimentiRdlBll.EseguiAnnullamento(u, CodPos, CodPosTra, RagSoc, RagSocTra, Matricola, Nome, Cognome, ProRap, ProRapTra, ProTraRap, Gruppo, ProSos, DatAnn, ref ErroreMSG, ref SuccessMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return RedirectToAction("RicercaTrasferimentiRDL", "Amministrativo");
        }

        public ActionResult ListaIscrittiX_Trasferimenti()
        {
            TrasferimentiRdlOCM trasferimentiRdlOCM = new TrasferimentiRdlOCM();
            TrasferimentiRdlBll trasferimentiRdlBll = new TrasferimentiRdlBll();
            string MsgErrore = " ";
            string CodPos = "";
            string DatDec = " ";
            if (base.TempData["CodPos"] != null && base.TempData["DatDec"] != null)
            {
                CodPos = base.TempData["CodPos"].ToString();
                DatDec = base.TempData["DatDec"].ToString();
            }
            else
            {
                CodPos = null;
                DatDec = null;
            }
            if (!string.IsNullOrEmpty(CodPos) && !string.IsNullOrEmpty(DatDec))
            {
                trasferimentiRdlBll.CaricaDati_Trasferimento(trasferimentiRdlOCM, DatDec, CodPos, ref MsgErrore);
                return View(trasferimentiRdlOCM);
            }
            return View();
        }

        [HttpPost]
        public ActionResult ListaIscrittiX_Trasferimenti(TrasferimentiRdlOCM.DatiTrasferimento rdlOcm)
        {
            string MsgErrore = " ";
            string CodPos = "";
            string DatDec = " ";
            string RagSoc = " ";
            TrasferimentiRdlBll trasferimentiRdlBll = new TrasferimentiRdlBll();
            TrasferimentiRdlOCM trasferimentiRdlOCM = new TrasferimentiRdlOCM();
            DatDec = rdlOcm.DatDec;
            CodPos = rdlOcm.CodPos;
            if (!string.IsNullOrEmpty(CodPos) && !string.IsNullOrEmpty(DatDec))
            {
                trasferimentiRdlBll.CaricaDati_Trasferimento(trasferimentiRdlOCM, DatDec, CodPos, ref MsgErrore);
                base.TempData["CodPos"] = CodPos;
                base.TempData.Keep("CodPos");
                base.TempData["DatDec"] = DatDec;
                base.TempData.Keep("DatDec");
                RagSoc = trasferimentiRdlOCM.datitrasferimento.RagSoc;
                base.TempData["RagSoc"] = RagSoc;
                base.TempData.Keep("RagSoc");
                if (MsgErrore != " ")
                {
                    base.TempData["errorMessage"] = MsgErrore;
                }
                return View(trasferimentiRdlOCM);
            }
            MsgErrore = "Impossibile eseguire la ricerca senza aver inserito la Posizione e la Data Decorrenza.";
            if (MsgErrore != " ")
            {
                base.TempData["errorMessage"] = MsgErrore;
            }
            return View();
        }

        public JsonResult Aggiungi_ClickTrasferimentiRdl(string cod, string codpos, string datden, string objList)
        {
            TrasferimentiRdlBll trasferimentiRdlBll = new TrasferimentiRdlBll();
            TrasferimentiRdlOCM tra = new TrasferimentiRdlOCM();
            string MsgErrore = "";
            List<TrasferimentiRdlOCM.ListaAziende> listaAziendes = (tra.ListaAziendes = JsonConvert.DeserializeObject<List<TrasferimentiRdlOCM.ListaAziende>>(objList));
            bool controllo = trasferimentiRdlBll.Aggiungi_click(tra, cod, codpos, datden, ref MsgErrore);
            return Json(new { cod, codpos, datden, controllo, MsgErrore }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TrasferimentoRdl(string objList)
        {
            List<TrasferimentiRdlOCM.DatiTrasferimento> ListaIscrittis = JsonConvert.DeserializeObject<List<TrasferimentiRdlOCM.DatiTrasferimento>>(objList);
            base.Session["Lista"] = ListaIscrittis;
            return new JsonResult
            {
                Data = JsonConvert.SerializeObject(ListaIscrittis),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult TabAziendaTrasferimenti(string objList)
        {
            List<TrasferimentiRdlOCM.ListaAziende> listaAziendes = JsonConvert.DeserializeObject<List<TrasferimentiRdlOCM.ListaAziende>>(objList);
            base.Session["ListaAziende"] = listaAziendes;
            return new JsonResult
            {
                Data = JsonConvert.SerializeObject(listaAziendes),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult EseguiTrasferimentiRdl(TrasferimentiRdlOCM trasferimentiRdlOCM)
        {
            string codpos = base.TempData["CodPos"].ToString();
            base.TempData.Keep("CodPos");
            string datDec = base.TempData["DatDec"].ToString();
            base.TempData.Keep("DatDec");
            string RagSoc = base.TempData["RagSoc"].ToString();
            base.TempData.Keep("RagSoc");
            List<TrasferimentiRdlOCM.DatiTrasferimento> ListaIscrittis = (trasferimentiRdlOCM.ListaIscrittis = base.Session["Lista"] as List<TrasferimentiRdlOCM.DatiTrasferimento>);
            trasferimentiRdlOCM.datitrasferimento.CodPos = codpos;
            trasferimentiRdlOCM.datitrasferimento.DatDec = datDec;
            trasferimentiRdlOCM.datitrasferimento.RagSoc = RagSoc;
            return View(trasferimentiRdlOCM);
        }

        public ActionResult SalvaTrasferimentiRdl()
        {
            Utente u = base.Session["utente"] as Utente;
            string MsgErrore = "";
            string MsgSuccess = " ";
            string CodPos = "";
            string DatDec = "";
            TrasferimentiRdlOCM trasferimentiRdlOCM = new TrasferimentiRdlOCM();
            CodPos = base.TempData["CodPos"].ToString();
            DatDec = base.TempData["DatDec"].ToString();
            List<TrasferimentiRdlOCM.DatiTrasferimento> ListaIscrittis = base.Session["Lista"] as List<TrasferimentiRdlOCM.DatiTrasferimento>;
            List<TrasferimentiRdlOCM.ListaAziende> listaAziendes = base.Session["ListaAziende"] as List<TrasferimentiRdlOCM.ListaAziende>;
            trasferimentiRdlOCM.ListaIscrittis = ListaIscrittis;
            trasferimentiRdlOCM.ListaAziendes = listaAziendes;
            TrasferimentiRdlBll trasferimentiRdlBll = new TrasferimentiRdlBll();
            trasferimentiRdlBll.SalvaTrasferimentiBll(trasferimentiRdlOCM, u, CodPos, DatDec, ref MsgErrore, ref MsgSuccess);
            if (MsgErrore != "")
            {
                base.TempData["errorMessage"] = MsgErrore;
            }
            else if (MsgSuccess != "")
            {
                base.TempData["successMessage"] = MsgSuccess;
            }
            return RedirectToAction("ListaIscrittiX_Trasferimenti", "Amministrativo");
        }

        public ActionResult EstrattoContoAzienda()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EstrattoContoAzienda(string codpos, string ragsoc)
        {
            EstrattoContoBLL estrattoConto = new EstrattoContoBLL();
            EstrattoContoOCM eca = new EstrattoContoOCM();
            if (string.IsNullOrEmpty(codpos) && string.IsNullOrEmpty(ragsoc))
            {
                string ErroreMSG = "Inserire almeno un campo per la ricerca";
                base.TempData["errorMessage"] = ErroreMSG;
                return View();
            }
            eca = estrattoConto.GetEstrattoConto(codpos, ragsoc);
            return View(eca);
        }

        public ActionResult EstrattoContoIscritto()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EstrattoContoIscritto(string mat, string nom, string cog, string codfis)
        {
            EstrattoContoBLL estrattoConto = new EstrattoContoBLL();
            EstrattoContoOCM eca = new EstrattoContoOCM();
            if (string.IsNullOrEmpty(mat) && string.IsNullOrEmpty(nom) && string.IsNullOrEmpty(cog) && string.IsNullOrEmpty(codfis))
            {
                string ErroreMSG = "Inserire almeno un campo per la ricerca";
                base.TempData["errorMessage"] = ErroreMSG;
                return View();
            }
            eca = estrattoConto.GetEstrattoContoIsc(mat, nom, cog, codfis);
            return View(eca);
        }

        public ActionResult ListaIscritti()
        {
            base.ViewBag.src = base.Request.QueryString["src"];
            return View();
        }

        public ActionResult ListaAziende()
        {
            base.ViewBag.src = base.Request.QueryString["src"];
            return View();
        }

        public ActionResult AnagraficaAzienda()
        {
            base.ViewBag.src = base.Request.QueryString["src"];
            return View();
        }

        public ActionResult RicRappLeg()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RicRappLeg(string posizione, string ragioneSociale, string partitaiva, string codiceFiscale, string cognome, string nome, string codfis, string cerca)
        {
            RappLegBLL rapp = new RappLegBLL();
            Rappresentante_legaleOCM rappleg = new Rappresentante_legaleOCM();
            string ErroreMSG = "";
            string SuccessMSG = "";
            if (!string.IsNullOrEmpty(cerca))
            {
                rappleg = rapp.RicRap(posizione, ragioneSociale, partitaiva, codiceFiscale, cognome, nome, codfis, ref ErroreMSG, ref SuccessMSG);
                base.TempData["rapleg"] = rappleg;
                if (ErroreMSG != "")
                {
                    base.TempData["errorMessage"] = ErroreMSG;
                }
                else if (SuccessMSG != "")
                {
                    base.TempData["successMessage"] = SuccessMSG;
                }
                return View(rappleg);
            }
            return View();
        }

        public ActionResult Delete(string codpos, string prorec, string datconf)
        {
            Rappresentante_legaleOCM rap = (Rappresentante_legaleOCM)base.TempData["rapleg"];
            base.TempData.Peek("rapleg");
            RappLegBLL delete = new RappLegBLL();
            string ErroreMSG = "";
            string SuccessMSG = "";
            rap = delete.Delete(rap, codpos, prorec, datconf, ref ErroreMSG, ref SuccessMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return RedirectToAction("RicRappLeg");
        }

        public ActionResult DettRapp(string codpos, string datini, string rappri, string denfunrap, string datcom, string cog, string nom)
        {
            RappLegBLL rapp = new RappLegBLL();
            Rappresentante_legaleOCM dettrap = new Rappresentante_legaleOCM();
            string ErroreMSG = "";
            string SuccessMSG = "";
            dettrap = rapp.DetRap(codpos, datini, rappri, denfunrap, datcom, cog, nom, ref ErroreMSG, ref SuccessMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            base.TempData["rapp"] = dettrap;
            return View(dettrap);
        }

        [HttpPost]
        public ActionResult DettRapp(Rappresentante_legaleOCM.DettRap detrap)
        {
            Utente u = base.Session["utente"] as Utente;
            RappLegBLL insrap = new RappLegBLL();
            Rappresentante_legaleOCM rap = (Rappresentante_legaleOCM)base.TempData["rapp"];
            base.TempData.Keep("rapp");
            string ErroreMSG = "";
            string SuccessMSG = "";
            rap = insrap.InsRap(detrap, rap, ref ErroreMSG, ref SuccessMSG, u);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return View(rap);
        }

        public ActionResult GestioneAziendeWeb()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GestioneAziendeWeb(string Codpos, string Ragsoc, string Partitaiva, string codfis, string cerca, string nuovaAz, string Cessato)
        {
            GestioneAziendeWebBLL ListaAziende = new GestioneAziendeWebBLL();
            GestioneAziendeWebOCM list = new GestioneAziendeWebOCM();
            string ErroreMSG = "";
            string SuccessMSG = "";
            if (!string.IsNullOrEmpty(cerca))
            {
                list.Ricerca = ListaAziende.Lista(Codpos, Ragsoc, Partitaiva, codfis, ref ErroreMSG, ref SuccessMSG, Cessato);
                if (ErroreMSG != "")
                {
                    base.TempData["errorMessage"] = ErroreMSG;
                }
                else if (SuccessMSG != "")
                {
                    base.TempData["successMessage"] = SuccessMSG;
                }
                return View(list);
            }
            if (!string.IsNullOrEmpty(nuovaAz))
            {
                return RedirectToAction("NuovaAzienda", "Amministrativo");
            }
            return View();
        }

        public ActionResult DettaglioAziendaWeb(int id, string codpos)
        {
            if (!string.IsNullOrEmpty(id.ToString()))
            {
                base.ViewBag.codpos = codpos;
                GestioneAziendeWebOCM list = new GestioneAziendeWebOCM();
                GestioneAziendeWebBLL d = new GestioneAziendeWebBLL();
                Utente u = base.Session["utente"] as Utente;
                base.TempData["id"] = id;
                list.datiAzienda = d.a(id);
                list.rapleg = d.b(id);
                list.altridati = d.c(id);
                list.documenti = d.d(id);
                list.sedeLegale = d.e(id);
                list.sedeAmministrativa = d.f(id);
                list.indirizzoCorrispondenza = d.g(id);
                list.natgiu = d.natg();
                list.GetMezzs = d.mezzComms();
                list.tipoRapLegs = d.tipoRapLegs();
                list.vias = d.GetVias();
                list.Comunes = d.GetComunes();
                list.GetCategoriaAttivitas = d.GetCategorias();
                list.tipoAttivitas = d.GetTipoAttivitas();
                list.CodiceStatisticos = d.GetStatisticos();
                list.ListaLocalita = d.GetLocalita();
                string ErroreMSG = "";
                string SuccessMSG = "";
                if (ErroreMSG != "")
                {
                    base.TempData["errorMessage"] = ErroreMSG;
                }
                else if (SuccessMSG != "")
                {
                    base.TempData["successMessage"] = SuccessMSG;
                }
                return View(list);
            }
            return View();
        }

        [HttpPost]
        public ActionResult DettaglioAziendaWeb(GestioneAziendeWebOCM gestione, GestioneAziendeWebOCM.DatiAzienda a, GestioneAziendeWebOCM.IndirizzoCorrispondenza f, GestioneAziendeWebOCM.SedeAmministrativa e, GestioneAziendeWebOCM.SedeLegale d, GestioneAziendeWebOCM.AltriDati c, GestioneAziendeWebOCM.Documenti b, GestioneAziendeWebOCM.RapLeg g)
        {
            gestione.datiAzienda = a;
            gestione.indirizzoCorrispondenza = f;
            gestione.sedeAmministrativa = e;
            gestione.sedeLegale = d;
            gestione.altridati = c;
            gestione.documenti = b;
            gestione.rapleg = g;
            Utente u = base.Session["utente"] as Utente;
            GestioneAziendeWebBLL aziBLL = new GestioneAziendeWebBLL();
            GestioneAziendeWebOCM carica = new GestioneAziendeWebOCM();
            string ErroreMSG = "";
            string SuccessMSG = "";
            carica = aziBLL.Update(gestione, u, ref ErroreMSG, ref SuccessMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return RedirectToAction("DettaglioAziendaWeb", "Amministrativo", new
            {
                id = Convert.ToInt32(base.TempData["id"])
            });
        }

        public ActionResult NuovaAzienda()
        {
            GestioneAziendeWebOCM list = new GestioneAziendeWebOCM();
            GestioneAziendeWebBLL d = new GestioneAziendeWebBLL();
            Utente u = base.Session["utente"] as Utente;
            list.natgiu = d.natg();
            list.GetMezzs = d.mezzComms();
            list.tipoRapLegs = d.tipoRapLegs();
            list.vias = d.GetVias();
            list.Comunes = d.GetComunes();
            list.GetCategoriaAttivitas = d.GetCategorias();
            list.tipoAttivitas = d.GetTipoAttivitas();
            list.CodiceStatisticos = d.GetStatisticos();
            list.ListaLocalita = d.GetLocalita();
            return View(list);
        }

        [HttpPost]
        public ActionResult NuovaAzienda(GestioneAziendeWebOCM gestione, GestioneAziendeWebOCM.DatiAzienda a, GestioneAziendeWebOCM.IndirizzoCorrispondenza f, GestioneAziendeWebOCM.SedeAmministrativa e, GestioneAziendeWebOCM.SedeLegale d, GestioneAziendeWebOCM.AltriDati c, GestioneAziendeWebOCM.Documenti b, GestioneAziendeWebOCM.RapLeg g)
        {
            string ErroreMSG = "";
            string SuccessMSG = "";
            gestione.datiAzienda = a;
            gestione.indirizzoCorrispondenza = f;
            gestione.sedeAmministrativa = e;
            gestione.sedeLegale = d;
            gestione.altridati = c;
            gestione.documenti = b;
            gestione.rapleg = g;
            Utente u = base.Session["utente"] as Utente;
            GestioneAziendeWebBLL newazi = new GestioneAziendeWebBLL();
            gestione.natgiu = newazi.natg();
            gestione.GetMezzs = newazi.mezzComms();
            gestione.tipoRapLegs = newazi.tipoRapLegs();
            gestione.vias = newazi.GetVias();
            gestione.Comunes = newazi.GetComunes();
            gestione.GetCategoriaAttivitas = newazi.GetCategorias();
            gestione.tipoAttivitas = newazi.GetTipoAttivitas();
            gestione.CodiceStatisticos = newazi.GetStatisticos();
            gestione.ListaLocalita = newazi.GetLocalita();
            gestione = newazi.Update(gestione, u, ref ErroreMSG, ref SuccessMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return View(gestione);
        }

        public ActionResult DettaglioDeleghe(string datini, string asster, string codter, string assnaz, string codnaz, string stato, string codpos, string ragsoc)
        {
            DelegheBLL delegheBLL = new DelegheBLL();
            DelegheOCM delegheOCM = new DelegheOCM();
            delegheOCM = delegheBLL.GetAssociazione(delegheOCM, codter, codnaz, codpos, assnaz, asster, ragsoc);
            return View(delegheOCM);
        }

        public JsonResult CaricaIva(string codFis, string ParIva)
        {
            string MSGErrore = "";
            DelegheBLL delegheBLL = new DelegheBLL();
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
                ragSocBr, ragSoc, pariva, codfis, ind, ViaDe, NumCivDe, citta, prov, cap,
                comune, CodCom, tel, fax, email, cell, emailcert
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchDelega()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchDelega(string PosAz, string Partitaiva, string tipass, string cerca, string nuovaDel)
        {
            string ErroreMSG = "";
            string SuccessMSG = "";
            DelegheOCM del = new DelegheOCM();
            DelegheBLL delbll = new DelegheBLL();
            if (!string.IsNullOrEmpty(cerca))
            {
                del = delbll.SearchDeleghe(PosAz, Partitaiva, tipass, ref ErroreMSG, ref SuccessMSG);
                if (ErroreMSG != "")
                {
                    base.TempData["errorMessage"] = ErroreMSG;
                }
                else if (SuccessMSG != "")
                {
                    base.TempData["successMessage"] = SuccessMSG;
                }
                base.TempData["del"] = del;
                return View(del);
            }
            if (!string.IsNullOrEmpty(nuovaDel))
            {
                return RedirectToAction("InserimentoDeleghe", "Amministrativo");
            }
            return View();
        }

        public ActionResult GestioneRapportiLavoroIscritti()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GestioneRapportiLavoroIscritti(GestioneRapportiLavoroIscrittiOCM.Iscritti isc, string cerca, string inserimento, string codpos)
        {
            Utente u = base.Session["utente"] as Utente;
            string ErroreMSG = "";
            string SuccessMSG = "";
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            if (!string.IsNullOrEmpty(cerca))
            {
                gestrdl = gestrap.RicercaIscrittiBLL(isc, ref ErroreMSG, ref SuccessMSG);
            }
            if (!string.IsNullOrEmpty(inserimento))
            {
                return RedirectToAction("InserimentoRapportiLavoro", "Amministrativo", new
                {
                    Codpos = codpos
                });
            }
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return View(gestrdl);
        }

        public ActionResult InserimentoRapportiLavoro(string codpos)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            Utente u = base.Session["utente"] as Utente;
            string ErroreMSG = "";
            string SuccessMSG = "";
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            gestrdl = gestrap.CaricaInserimentoBLL(gestrdl, codpos, ref ErroreMSG, u);
            return View(gestrdl);
        }

        [HttpPost]
        public ActionResult InserimentoRapportiLavoro(GestioneRapportiLavoroIscrittiOCM.Iscritti rdl_Iscritti, GestioneRapportiLavoroIscrittiOCM.AziendaUtilizzatrice azi, GestioneRapportiLavoroIscrittiOCM.DatiContrattuali daticontr, GestioneRapportiLavoroIscrittiOCM.DatiRetributivi datret, GestioneRapportiLavoroIscrittiOCM.AltriDati altridati, GestioneRapportiLavoroIscrittiOCM.Mesi mesi)
        {
            GestioneRapportiLavoroIscrittiBLL rdlBLL = new GestioneRapportiLavoroIscrittiBLL();
            Utente u = base.Session["utente"] as Utente;
            string ErroreMSG = "";
            string SuccessMSG = "";
            GestioneRapportiLavoroIscrittiOCM rdl = new GestioneRapportiLavoroIscrittiOCM();
            if (!rdl_Iscritti.pec.EndsWith("@pec") || !rdl_Iscritti.pec.EndsWith("@legalmail"))
            {
                ErroreMSG = "Errore Pec non valida";
                return RedirectToAction("GestioneRapportiLavoroIscritti", "Amministrativo");
            }
            rdl.iscritti = rdl_Iscritti;
            rdl.aziendaUtilizzatrice = azi;
            rdl.datiContrattuali = daticontr;
            rdl.datiRetributivi = datret;
            rdl.altriDati = altridati;
            rdlBLL.btn_Salva(u, rdl, rdl.aziendaUtilizzatrice.codposAz, ref ErroreMSG, ref SuccessMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return RedirectToAction("GestioneRapportiLavoroIscritti", "Amministrativo");
        }

        public ActionResult ModificaAnagraficaRapportiLavoroIscritti(string mat)
        {
            Utente u = base.Session["utente"] as Utente;
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            gestrdl = gestrap.DettaglioModificaIscrittiBLL(mat);
            return View(gestrdl);
        }

        [HttpPost]
        public ActionResult ModificaAnagraficaRapportiLavoroIscritti(GestioneRapportiLavoroIscrittiOCM.Iscritti rdl)
        {
            Utente u = base.Session["utente"] as Utente;
            string ErroreMSG = "";
            string SuccessMSG = "";
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            gestrap.SalvaModifica(rdl, u, ref ErroreMSG, ref SuccessMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return RedirectToAction("GestioneRapportiLavoroIscritti");
        }

        public JsonResult CercaMatricola(string mat, string nom, string cog, string codfisr)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            gestrdl = gestrap.GetMatricolaBLL(mat, nom, cog, codfisr);
            if (gestrdl != null)
            {
                string ultagg = gestrdl.iscritti.ultagg;
                string strdatnasold = gestrdl.iscritti.strdatnasold;
                string matricola = gestrdl.iscritti.mat;
                string nominativo = gestrdl.iscritti.nominativo;
                string cognome = gestrdl.iscritti.cognome;
                string nome = gestrdl.iscritti.nome;
                string codfis = gestrdl.iscritti.codfis;
                string datnas = gestrdl.iscritti.datnas;
                string sesso = gestrdl.iscritti.sesso;
                string dug = gestrdl.iscritti.dug;
                string coddug = gestrdl.iscritti.coddug;
                string co = gestrdl.iscritti.co;
                string indirizzo = gestrdl.iscritti.indirizzo;
                string numciv = gestrdl.iscritti.numciv;
                string comuneN = gestrdl.iscritti.comuneN;
                string comuneCodN = gestrdl.iscritti.comuneCodN;
                string provN = gestrdl.iscritti.provN;
                string statoEsN = gestrdl.iscritti.statoEsN;
                string statoEsCodN = gestrdl.iscritti.statoEsCodN;
                string statoEs = gestrdl.iscritti.statoEs;
                string cap = gestrdl.iscritti.cap;
                string localita = gestrdl.iscritti.localita;
                string comuneCod = gestrdl.iscritti.comuneCod;
                string comune = gestrdl.iscritti.comune;
                string prov = gestrdl.iscritti.prov;
                string statocivile = gestrdl.iscritti.statocivile;
                string titoloStudio = gestrdl.iscritti.titoloStudio;
                string titoloStudioCod = gestrdl.iscritti.titoloStudioCod;
                string pec = gestrdl.iscritti.pec;
                string email = gestrdl.iscritti.email;
                string cellulare = gestrdl.iscritti.cellulare;
                string tel = gestrdl.iscritti.tel;
                string tel2 = gestrdl.iscritti.tel2;
                string fax = gestrdl.iscritti.fax;
                string url = gestrdl.iscritti.url;
                string datiNONModificati = gestrdl.iscritti.datiNONModificati;
                string datiRecapitiOld = gestrdl.iscritti.datiRecapitiOld;
                string datiAnagraficiOld = gestrdl.iscritti.datiAnagraficiOld;
                string datisc = gestrdl.datiContrattuali.datisc;
                string datAssunzione = gestrdl.altriDati.datAssunzione;
                return Json(new
                {
                    ultagg, strdatnasold, mat, nominativo, cognome, nome, codfis, datnas, sesso, dug,
                    coddug, co, indirizzo, numciv, comuneN, comuneCodN, provN, statoEsN, statoEsCodN, statoEs,
                    cap, localita, comuneCod, comune, prov, statocivile, titoloStudio, titoloStudioCod, pec, email,
                    cellulare, tel, tel2, fax, url, datiNONModificati, datiRecapitiOld, datiAnagraficiOld, datisc, datAssunzione
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }

        public JsonResult CercaAziendaConPiva(string piva)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            string ErroreMSG = "";
            gestrdl = gestrap.GetDatiAziendaConPiva(gestrdl, piva, ref ErroreMSG);
            if (gestrdl == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            string ragsocAz = gestrdl.aziendaUtilizzatrice.ragsocAz;
            string codfisAz = gestrdl.aziendaUtilizzatrice.codfisAz;
            string codpos = gestrdl.aziendaUtilizzatrice.codposAz;
            string localitaAz = gestrdl.aziendaUtilizzatrice.localitaAz;
            string dugAz = gestrdl.aziendaUtilizzatrice.dugAz;
            string dugCodAz = gestrdl.aziendaUtilizzatrice.dugCodAz;
            string indirizzoAz = gestrdl.aziendaUtilizzatrice.indirizzoAz;
            string civicoAz = gestrdl.aziendaUtilizzatrice.civicoAz;
            string provAz = gestrdl.aziendaUtilizzatrice.provAz;
            string capAz = gestrdl.aziendaUtilizzatrice.capAz;
            string telefonoAz = gestrdl.aziendaUtilizzatrice.telefonoAz;
            string comuneAz = gestrdl.aziendaUtilizzatrice.comuneAz;
            string comuneCodAz = gestrdl.aziendaUtilizzatrice.comuneCodAz;
            string emailAz = gestrdl.aziendaUtilizzatrice.emailAz;
            string pecAz = gestrdl.aziendaUtilizzatrice.pecAz;
            return Json(new
            {
                ragsocAz,
                codfisAz,
                codpos,
                localitaAz,
                dugAz,
                dugCodAz,
                indirizzoAz,
                civicoAz,
                provAz,
                capAz,
                telefonoAz,
                comuneAz,
                comuneCodAz,
                emailAz,
                pecAz
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CercaAzienda(string codposAz)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            string ErroreMSG = "";
            gestrdl = gestrap.GetDatiAzienda(gestrdl, codposAz, ref ErroreMSG);
            if (gestrdl == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            string ragsocAz = gestrdl.aziendaUtilizzatrice.ragsocAz;
            string codfisAz = gestrdl.aziendaUtilizzatrice.codfisAz;
            string parivaAz = gestrdl.aziendaUtilizzatrice.parivaAz;
            string localitaAz = gestrdl.aziendaUtilizzatrice.localitaAz;
            string dugAz = gestrdl.aziendaUtilizzatrice.dugAz;
            string indirizzoAz = gestrdl.aziendaUtilizzatrice.indirizzoAz;
            string civicoAz = gestrdl.aziendaUtilizzatrice.civicoAz;
            string provAz = gestrdl.aziendaUtilizzatrice.provAz;
            string capAz = gestrdl.aziendaUtilizzatrice.capAz;
            string telefonoAz = gestrdl.aziendaUtilizzatrice.telefonoAz;
            string comuneAz = gestrdl.aziendaUtilizzatrice.comuneAz;
            string comuneCodAz = gestrdl.aziendaUtilizzatrice.comuneCodAz;
            string emailAz = gestrdl.aziendaUtilizzatrice.emailAz;
            string pecAz = gestrdl.aziendaUtilizzatrice.pecAz;
            return Json(new
            {
                ragsocAz, codfisAz, parivaAz, localitaAz, dugAz, indirizzoAz, civicoAz, provAz, capAz, telefonoAz,
                comuneAz, comuneCodAz, emailAz, pecAz
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VariazioniRDL(string mat, string datNas, string Nome, string Cognome)
        {
            Utente u = base.Session["utente"] as Utente;
            string ErroreMSG = "";
            GestioneRapportiLavoroIscrittiOCM rdl = new GestioneRapportiLavoroIscrittiOCM();
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            rdl = gestrap.VariazioniRDLBLL(ErroreMSG, rdl, u, mat, datNas);
            rdl.iscritti.nominativo = Nome + " " + Cognome;
            base.TempData["rdl"] = rdl;
            return View(rdl);
        }

        [HttpPost]
        public ActionResult VariazioniRDL(GestioneRapportiLavoroIscrittiOCM.DatiContrattuali rdl_daticont, GestioneRapportiLavoroIscrittiOCM.Iscritti rdl_iscritti, GestioneRapportiLavoroIscrittiOCM.DatiRetributivi rdl_datiRet, GestioneRapportiLavoroIscrittiOCM.AziendaUtilizzatrice rdl_aziUte, GestioneRapportiLavoroIscrittiOCM.AltriDati rdl_altriDati)
        {
            GestioneRapportiLavoroIscrittiOCM rdl = new GestioneRapportiLavoroIscrittiOCM();
            rdl.datiContrattuali = rdl_daticont;
            rdl.iscritti = rdl_iscritti;
            rdl.datiRetributivi = rdl_datiRet;
            rdl.aziendaUtilizzatrice = rdl_aziUte;
            rdl.altriDati = rdl_altriDati;
            string MsgErrore = "";
            string SuccessMSG = "";
            if (!rdl_iscritti.pec.EndsWith("@pec") || !rdl_iscritti.pec.EndsWith("@legalmail"))
            {
                MsgErrore = "Errore Pec non valida";
                return RedirectToAction("VariazioniRDL", "Amministrativo", new { rdl_iscritti.mat, rdl_iscritti.datnas });
            }
            if (!rdl_aziUte.pecAz.EndsWith("@pec") || !rdl_aziUte.pecAz.EndsWith("@legalmail"))
            {
                MsgErrore = "Errore Pec non valida";
                return RedirectToAction("VariazioniRDL", "Amministrativo", new { rdl_iscritti.mat, rdl_iscritti.datnas });
            }
            Utente u = base.Session["utente"] as Utente;
            GestioneRapportiLavoroIscrittiBLL var = new GestioneRapportiLavoroIscrittiBLL();
            var.VariazioniBLL(rdl, u, ref MsgErrore, ref SuccessMSG);
            if (MsgErrore != "")
            {
                base.TempData["errorMessage"] = MsgErrore;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            string mat = rdl_iscritti.mat;
            string datnas = rdl_iscritti.datnas;
            return RedirectToAction("VariazioniRDL", "Amministrativo", new { mat, datnas });
        }

        public JsonResult DdlContratto(string datIni, string strAzione)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            gestrdl = gestrap.DdlContrattoBLL(gestrdl, datIni, strAzione);
            return Json(gestrdl.listContratti.Select((GestioneRapportiLavoroIscrittiOCM.Contratti x) => new { x.CODCON, x.DENCON, x.DENQUA, x.NUMMEN, x.M14, x.M15, x.M16, x.PROCON, x.CODQUACON }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DdlLivello(string codcon, string dencon)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            gestrdl = gestrap.GetdllLivello(gestrdl, codcon, dencon);
            var listliv = gestrdl.listLivello.Select((GestioneRapportiLavoroIscrittiOCM.Livello x) => new
            {
                codliv = x.CODLIV,
                denliv = x.DENLIV
            }).ToList();
            return Json(listliv, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Mensilita(string dencon)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            gestrdl = gestrap.MensilitaBLL(gestrdl, dencon);
            string m14 = gestrdl.datiContrattuali.mens14;
            string m15 = gestrdl.datiContrattuali.mens15;
            string m16 = gestrdl.datiContrattuali.mens16;
            string qualifica = gestrdl.datiContrattuali.qualifica;
            string mensilita = gestrdl.datiContrattuali.mensilita;
            string tipspe = gestrdl.datiContrattuali.TIPSPE;
            string codloc = gestrdl.datiContrattuali.CODLOC;
            return Json(new { m14, m15, m16, qualifica, mensilita, tipspe, codloc }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DdlAliquota(string datnas, string codcon, string dencon, string codpos)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            gestrdl = gestrap.GetdllAliquota(gestrdl, datnas, codcon, dencon, codpos);
            return Json(gestrdl.ListAliq.Select((GestioneRapportiLavoroIscrittiOCM.ListAliquota x) => new
            {
                DENGRUASS = x.DENFORASS,
                CODGRUASS = x.CODFORASS
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DdlAliquotaSomma(int CODQUACON, int CODGRUASS, string STRDATA, string FAP)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            gestrdl = gestrap.GetdllAliquotaSomma(gestrdl, CODQUACON, CODGRUASS, STRDATA, FAP);
            return Json(gestrdl.ListAliq.Select((GestioneRapportiLavoroIscrittiOCM.ListAliquota x) => new
            {
                DENGRUASS = x.DENFORASS,
                CODGRUASS = x.CODFORASS,
                ALIQUOTA = x.ALIQUOTA
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModificaRighe(string eredi, string note, string debito, string blocco, string bancari, string mat, GestioneRapportiLavoroIscrittiOCM.Eredi erediOCM, GestioneRapportiLavoroIscrittiOCM.NoteIscritto noteOCM, GestioneRapportiLavoroIscrittiOCM.DebitiIscritto debitoOCM, GestioneRapportiLavoroIscrittiOCM.BlocchiIscritto bloccoOCM, GestioneRapportiLavoroIscrittiOCM.DatiBancariIscritto bancariOCM)
        {
            string ErroreMSG = "";
            string SuccessMSG = "";
            Utente u = base.Session["utente"] as Utente;
            GestioneRapportiLavoroIscrittiBLL rdlBLL = new GestioneRapportiLavoroIscrittiBLL();
            if (!string.IsNullOrEmpty(eredi))
            {
                rdlBLL.ModIns_Erede(erediOCM, u, ref ErroreMSG, ref SuccessMSG, mat);
            }
            if (!string.IsNullOrEmpty(note))
            {
                rdlBLL.ModIns_Note(noteOCM, u, ref ErroreMSG, ref SuccessMSG, mat);
            }
            if (!string.IsNullOrEmpty(debito))
            {
                rdlBLL.ModIns_Debiti(debitoOCM, u, ref ErroreMSG, ref SuccessMSG, mat);
            }
            if (!string.IsNullOrEmpty(blocco))
            {
                rdlBLL.ModIns_Blocchi(bloccoOCM, u, ref ErroreMSG, ref SuccessMSG, mat);
            }
            if (!string.IsNullOrEmpty(bancari))
            {
                rdlBLL.ModIns_Bancari(bancariOCM, u, ref ErroreMSG, ref SuccessMSG, mat);
            }
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return RedirectToAction("ModificaAnagraficaRapportiLavoroIscritti", "Amministrativo", new { mat });
        }

        public ActionResult EliminaRighe(string mat, string blocco, string bancari, string debiti, string note, string eredi, string proiscban, string proereE, string codpos, string prorap, string protratt, string bloccoID, string NoteID)
        {
            string ErroreMSG = "";
            string SuccessMSG = "";
            Utente u = base.Session["utente"] as Utente;
            GestioneRapportiLavoroIscrittiBLL rdlBLL = new GestioneRapportiLavoroIscrittiBLL();
            if (!string.IsNullOrEmpty(eredi))
            {
                rdlBLL.Elimina_Erede(proereE, u, ref ErroreMSG, ref SuccessMSG, mat);
            }
            if (!string.IsNullOrEmpty(note))
            {
                rdlBLL.Elimina_Note(NoteID, u, ref ErroreMSG, ref SuccessMSG, mat);
            }
            if (!string.IsNullOrEmpty(debiti))
            {
                rdlBLL.Elimina_Debito(codpos, prorap, protratt, u, ref ErroreMSG, ref SuccessMSG, mat);
            }
            if (!string.IsNullOrEmpty(blocco))
            {
                rdlBLL.Elimina_Blocchi(bloccoID, u, ref ErroreMSG, ref SuccessMSG, mat);
            }
            if (!string.IsNullOrEmpty(bancari))
            {
                rdlBLL.Elimina_Bancari(proiscban, u, ref ErroreMSG, ref SuccessMSG, mat);
            }
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return RedirectToAction("ModificaAnagraficaRapportiLavoroIscritti", "Amministrativo", new { mat });
        }

        public JsonResult ValAliquota(string CODGRUASS, string dencon, string datIsc, string datMod, string messaggio, string dataNas, bool fap)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            gestrdl = gestrap.GetValAliquota(gestrdl, CODGRUASS, dencon, datIsc, datMod, messaggio, dataNas, fap);
            string aliquota = gestrdl.datiRetributivi.TOTALIQ;
            string CODQUACON = gestrdl.datiContrattuali.CODQUA;
            return Json(new { aliquota, CODQUACON }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelLivello(string numScatti, string dencon, string codliv, string datMod, string datIsc, string prorap, string tiprap, string perPar)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            gestrdl = gestrap.GetSelLivello(gestrdl, numScatti, dencon, codliv, datMod, datIsc, prorap, tiprap, perPar);
            string importoSc = gestrdl.datiRetributivi.importoSc;
            return Json(importoSc, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Emolumenti(string codliv, string dencon, string datMod)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            gestrdl = gestrap.GetEmolumenti(gestrdl, codliv, dencon, datMod);
            string emolumenti = gestrdl.datiRetributivi.emolumenti;
            return Json(emolumenti, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CercaBanca(string iban)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            gestrdl = gestrap.GetCercaBanca(gestrdl, iban);
            string istCre = gestrdl.datiBancariIscritto.istCre;
            string age = gestrdl.datiBancariIscritto.age;
            string indAge = gestrdl.datiBancariIscritto.indAge;
            string proiscban = gestrdl.datiBancariIscritto.proiscban;
            return Json(new { istCre, age, indAge, proiscban }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CaricaModificaEredi(string mat, string proere)
        {
            GestioneRapportiLavoroIscrittiBLL gestrap = new GestioneRapportiLavoroIscrittiBLL();
            GestioneRapportiLavoroIscrittiOCM gestrdl = new GestioneRapportiLavoroIscrittiOCM();
            gestrdl = gestrap.CaricaModErediBLL(gestrdl, mat, proere);
            string nomeE = gestrdl.eredi.nomeE;
            string cognomeE = gestrdl.eredi.cognomeE;
            string sessoE = gestrdl.eredi.sessoE;
            string codfisE = gestrdl.eredi.codfisE;
            string datnasE = gestrdl.eredi.datnasE;
            string parentelaE = gestrdl.eredi.parentelaE;
            string datiniE = gestrdl.eredi.datiniE;
            string coddugE = gestrdl.eredi.coddugE;
            string comuneCodNE = gestrdl.eredi.comuneCodNE;
            string provNE = gestrdl.eredi.provNE;
            string indirizzoE = gestrdl.eredi.indirizzoE;
            string statoEs_E = gestrdl.eredi.statoEs_E;
            string comuneCodE = gestrdl.eredi.comuneCodE;
            string capE = gestrdl.eredi.capE;
            string localitaE = gestrdl.eredi.localitaE;
            string provE = gestrdl.eredi.provE;
            string perTFRE = gestrdl.eredi.perTFRE;
            string perFPE = gestrdl.eredi.perFPE;
            string maggE = gestrdl.eredi.maggE;
            string maggPerE = gestrdl.eredi.maggPerE;
            string emailE = gestrdl.eredi.emailE;
            string telE = gestrdl.eredi.telE;
            string coE = gestrdl.eredi.coE;
            string IBANE = gestrdl.eredi.IBANE;
            string probancE = gestrdl.eredi.probancE;
            string SWIFTE = gestrdl.eredi.SWIFTE;
            string istCre = gestrdl.datiBancariIscritto.istCre;
            string age = gestrdl.datiBancariIscritto.age;
            string indAge = gestrdl.datiBancariIscritto.indAge;
            string proiscban = gestrdl.datiBancariIscritto.proiscban;
            string comuneE = gestrdl.eredi.comuneE;
            string comuneNE = gestrdl.eredi.comuneNE;
            return Json(new
            {
                nomeE, cognomeE, sessoE, codfisE, datnasE, parentelaE, datiniE, coddugE, comuneCodNE, provNE,
                indirizzoE, statoEs_E, comuneCodE, capE, localitaE, provE, perTFRE, perFPE, maggE, maggPerE,
                emailE, telE, coE, IBANE, probancE, SWIFTE, istCre, age, indAge, proiscban,
                comuneE, comuneNE
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ScadenzaDipa()
        {
            ScadenzeDipaBLL scadipaBLL = new ScadenzeDipaBLL();
            ScadenzaDipaOCM scadipaOCM = new ScadenzaDipaOCM();
            scadipaOCM = scadipaBLL.AnnoScaDipa();
            base.TempData["ScadenzaDipaAnni"] = scadipaOCM.listanni;
            return View(scadipaOCM);
        }

        [HttpPost]
        public ActionResult ScadenzaDipa(string newSosp, string SearchSosp, string anno, string ModSosp, ScadenzaDipaOCM.DatiScadenzeDipa listScadenzaDipa)
        {
            ScadenzeDipaBLL scadipaBLL = new ScadenzeDipaBLL();
            ScadenzaDipaOCM scadipaOCM = new ScadenzaDipaOCM();
            string ErroreMSG = "";
            string SuccessMSG = "";
            Utente u = base.Session["utente"] as Utente;
            scadipaOCM.listanni = (List<ScadenzaDipaOCM.Anno>)base.TempData["ScadenzaDipaAnni"];
            base.TempData.Keep("ScadenzaDipaAnni");
            if (!string.IsNullOrEmpty(newSosp))
            {
                scadipaOCM = scadipaBLL.GetNewScadipa(scadipaOCM);
            }
            if (!string.IsNullOrEmpty(SearchSosp))
            {
                scadipaOCM = scadipaBLL.CercaDataBLL(anno, scadipaOCM, ref ErroreMSG);
            }
            if (!string.IsNullOrEmpty(ModSosp))
            {
                scadipaOCM.datiScadenzeDipa = listScadenzaDipa;
                scadipaOCM = scadipaBLL.ModDipaBLL(scadipaOCM, u, ref ErroreMSG, ref SuccessMSG);
            }
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return View(scadipaOCM);
        }

        public ActionResult ModificaContratto(string Qualifica, string ASSCON, string Denominazione, string DATINI, string DATFIN, string NUMMEN, string MAXSCA, string PERSCA, string RIVAUT, string CODCON, string PROCON, string CODTIPCON)
        {
            GestioneContrattiOCM GCOCM = new GestioneContrattiOCM();
            GestioneContrattiBLL GCBLL = new GestioneContrattiBLL();
            GCOCM.contratti.Qualifica = Qualifica;
            GCOCM.contratti.ASSCON = ASSCON;
            GCOCM.contratti.Denominazione = Denominazione;
            GCOCM.contratti.DATINI = Convert.ToDateTime(DATINI).ToString("yyyy-MM-dd");
            GCOCM.contratti.DATFIN = Convert.ToDateTime(DATFIN).ToString("yyyy-MM-dd");
            GCOCM.contratti.NUMMEN = NUMMEN;
            GCOCM.contratti.MAXSCA = MAXSCA;
            GCOCM.contratti.PERSCA = PERSCA;
            GCOCM.contratti.RIVAUT = RIVAUT;
            GCOCM.contratti.CODCON = CODCON;
            GCOCM.contratti.PROCON = PROCON;
            GCOCM.contratti.CODTIPCON = CODTIPCON;
            GCBLL.Carica(GCOCM);
            GCOCM.contratti.checkInput = CODTIPCON + RIVAUT + PERSCA + MAXSCA + NUMMEN + GCOCM.contratti.DATFIN + GCOCM.contratti.DATINI + Denominazione + ASSCON + GCOCM.contratti.CODQUACON;
            return View(GCOCM);
        }

        [HttpPost]
        public ActionResult ModificaContratto(GestioneContrattiOCM.Contratti gc)
        {
            GestioneContrattiBLL GCBLL = new GestioneContrattiBLL();
            Utente u = base.Session["utente"] as Utente;
            string MSGErorre = "";
            string MSGSuccess = "";
            GCBLL.ModificaContrattoBLL(gc, u, ref MSGErorre, ref MSGSuccess);
            if (MSGErorre != "")
            {
                base.TempData["errorMessage"] = MSGErorre;
            }
            else if (MSGSuccess != "")
            {
                base.TempData["successMessage"] = MSGSuccess;
            }
            return RedirectToAction("ContrattiRicerca", "Amministrativo");
        }

        public ActionResult EliminaContratti(GestioneContrattiOCM gc, string CODCON, string PROCON, string DATINI, string DATFIN)
        {
            GestioneContrattiBLL GCBLL = new GestioneContrattiBLL();
            Utente u = base.Session["utente"] as Utente;
            string ErroreMSG = "";
            string SuccessMSG = "";
            GCBLL.EliminaContrattiBLL(gc, u, CODCON, PROCON, DATINI, DATFIN, ref ErroreMSG, ref SuccessMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            else if (SuccessMSG != "")
            {
                base.TempData["successMessage"] = SuccessMSG;
            }
            return RedirectToAction("ContrattiRicerca", "Amministrativo");
        }

        public ActionResult ContrattiRicerca()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ContrattiRicerca(string Denominazione, string livello)
        {
            GestioneContrattiBLL GCBLL = new GestioneContrattiBLL();
            string ErroreMSG = "";
            GestioneContrattiOCM gc = new GestioneContrattiOCM();
            gc = GCBLL.ContrattiRicercaTabellaBLL(Denominazione, livello, ref ErroreMSG);
            if (ErroreMSG != "")
            {
                base.TempData["errorMessage"] = ErroreMSG;
            }
            return View(gc);
        }

        public ActionResult ContrattoInserimento()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ContrattoInserimento(GestioneContrattiOCM.Contratti gc)
        {
            Utente u = base.Session["utente"] as Utente;
            string MSGErorre = "";
            string MSGSuccess = "";
            GestioneContrattiBLL GCBLL = new GestioneContrattiBLL();
            GCBLL.ContrattiInserimentoBLL(gc, u, ref MSGErorre, ref MSGSuccess);
            if (MSGErorre != "")
            {
                base.TempData["errorMessage"] = MSGErorre;
            }
            else if (MSGSuccess != "")
            {
                base.TempData["successMessage"] = MSGSuccess;
            }
            return RedirectToAction("ContrattiRicerca", "Amministrativo");
        }

        public ActionResult AnnullamentoDenunce()
        {
            int codPos = 0;
            List<SelectListItem> listaMesi = new List<SelectListItem>();
            Dictionary<int, string> dictMesi = AnnullamentiBLL.PopolaDDLMesi();
            List<TipiMovimento> listaTipiMovimento = AnnullamentiBLL.PopolaDDLTipiMovimento();
            if (listaTipiMovimento.Count == 0)
            {
                base.TempData["errorMessage"] = AnnullamentiBLL.ErrorMessage;
            }
            else
            {
                listaTipiMovimento.Insert(0, new TipiMovimento
                {
                    Codice = "",
                    Denominazione = ""
                });
                listaTipiMovimento.Insert(1, new TipiMovimento
                {
                    Codice = "SAN",
                    Denominazione = "SANZIONI"
                });
            }
            for (int i = 0; i < dictMesi.Count; i++)
            {
                listaMesi.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = dictMesi[i]
                });
            }
            if (AnnullamentiBLL.AutorizzazioniSpecialiSoloEnpaia(ref codPos, "CONTRIBUTI"))
            {
                base.ViewBag.codPos = codPos;
            }
            base.ViewBag.listaTipiMovimento = listaTipiMovimento;
            base.ViewBag.listaMesi = listaMesi;
            return View();
        }

        [HttpPost]
        public ActionResult AnnullamentoDenunce(string codPos, string anno_da, string mese_da, string anno_a, string mese_a, string tipMov, string ragSoc)
        {
            int codicePos = 0;
            List<SelectListItem> listaMesi = new List<SelectListItem>();
            Dictionary<int, string> dictMesi = AnnullamentiBLL.PopolaDDLMesi();
            List<TipiMovimento> listaTipiMovimento = AnnullamentiBLL.PopolaDDLTipiMovimento();
            if (listaTipiMovimento.Count == 0)
            {
                base.TempData["errorMessage"] = AnnullamentiBLL.ErrorMessage;
            }
            else
            {
                listaTipiMovimento.Insert(0, new TipiMovimento
                {
                    Codice = "",
                    Denominazione = ""
                });
                listaTipiMovimento.Insert(1, new TipiMovimento
                {
                    Codice = "SAN",
                    Denominazione = "SANZIONI"
                });
            }
            for (int i = 0; i < dictMesi.Count; i++)
            {
                listaMesi.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = dictMesi[i]
                });
            }
            if (AnnullamentiBLL.AutorizzazioniSpecialiSoloEnpaia(ref codicePos, "CONTRIBUTI"))
            {
                base.ViewBag.codPos = codicePos;
            }
            base.ViewBag.meseDa = mese_da;
            base.ViewBag.meseA = mese_a;
            base.ViewBag.ragioneSociale = ragSoc;
            base.ViewBag.tipMov = tipMov;
            base.ViewBag.listaTipiMovimento = listaTipiMovimento;
            base.ViewBag.listaMesi = listaMesi;
            if (!AnnullamentiBLL.AutorizzazioniSpeciali(codPos, "CONTRIBUTI"))
            {
                base.TempData["errorMessage"] = "Utente non abilitato";
                return View();
            }
            List<AnnullamentoDenunceOCM> listaAnnullamentoDenunce;
            if ((listaAnnullamentoDenunce = AnnullamentiBLL.CaricaDati(codPos, anno_da, mese_da, anno_a, mese_a, tipMov)) != null)
            {
                base.ViewBag.numRecord = listaAnnullamentoDenunce.Count;
                return View(listaAnnullamentoDenunce);
            }
            base.TempData["errorMessage"] = AnnullamentiBLL.ErrorMessage;
            return View();
        }

        public JsonResult AnnullaDenuncia_Step1(string objList)
        {
            List<AnnullamentoDenunceOCM> listaDenunceDaAnnullare = JsonConvert.DeserializeObject<List<AnnullamentoDenunceOCM>>(objList);
            base.Session["denunceDaAnnullare"] = listaDenunceDaAnnullare;
            List<string> listaConferme = AnnullamentiBLL.AnnullaDenuncia_Step1(listaDenunceDaAnnullare);
            if (listaConferme == null)
            {
                base.TempData["errorMessage"] = AnnullamentiBLL.ErrorMessage;
                listaConferme.Insert(0, 1.ToString());
                return new JsonResult
                {
                    Data = JsonConvert.SerializeObject(listaConferme),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            if (listaConferme.Count > 0)
            {
                listaConferme.Insert(0, "CONFERMA");
                return new JsonResult
                {
                    Data = JsonConvert.SerializeObject(listaConferme),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            listaConferme.Insert(0, "PROSEGUI");
            return new JsonResult
            {
                Data = JsonConvert.SerializeObject(listaConferme),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public string AnnullaDenuncia_Step2_Ajax()
        {
            decimal totale = default(decimal);
            string messaggio = string.Empty;
            string mess_iniziale = string.Empty;
            string mess_finale = string.Empty;
            List<AnnullamentoDenunceOCM> listaDenunceDaAnnullare = base.Session["denunceDaAnnullare"] as List<AnnullamentoDenunceOCM>;
            ImportiAnnullamentoDenunce objSalvataggioImporti = base.Session["objImporti_annullDen"] as ImportiAnnullamentoDenunce;
            string tipMov = listaDenunceDaAnnullare[0].OldTipMov.Trim();
            int numRecord = listaDenunceDaAnnullare[0].NumRecord;
            switch (tipMov)
            {
                case "NU":
                    mess_iniziale = $"Notifiche di Ufficio contabilizzate in lista: {numRecord} - Selezionate: {listaDenunceDaAnnullare.Count} |";
                    mess_finale = "Procedere con l'annullamento delle Notifiche selezionate contabilizzate?";
                    break;
                case "DP":
                    mess_iniziale = $"DIPA contabilizzati in lista: {numRecord} - Selezionati: {listaDenunceDaAnnullare.Count} |";
                    mess_finale = "Procedere con l'annullamento dei DIPA selezionati contabilizzati?";
                    break;
                case "AR":
                    mess_iniziale = $"Arretrati contabilizzati in lista: {numRecord} - Selezionati: {listaDenunceDaAnnullare.Count} |";
                    mess_finale = "Procedere con l'annullamento degli Arretrati selezionati contabilizzati?";
                    break;
                default:
                    messaggio = $"Sanzioni contabilizzate in lista: {numRecord} - Selezionate: {listaDenunceDaAnnullare.Count} |";
                    messaggio += $"Importo Sanzioni: € {objSalvataggioImporti.Importo_sanzione} |";
                    messaggio += "Procedere con l'annullamento delle Sanzioni selezionate contabilizzate?";
                    break;
            }
            if (tipMov != "SAN")
            {
                totale += objSalvataggioImporti.Importo_sanzione + objSalvataggioImporti.Importo_contributo + objSalvataggioImporti.Importo_abb + objSalvataggioImporti.Importo_addizionale + objSalvataggioImporti.Importo_assistenza + objSalvataggioImporti.Importo_rett_contrib + objSalvataggioImporti.Importo_rett_addiz + objSalvataggioImporti.Importo_rett_sanzione;
                messaggio = "Importo Contributo: € " + objSalvataggioImporti.Importo_contributo.ToString().PadLeft(totale.ToString().Length) + " |";
                messaggio = messaggio + "Importo Addizionale: € " + objSalvataggioImporti.Importo_addizionale.ToString().PadLeft(totale.ToString().Length) + " |";
                messaggio = messaggio + "Importo Assistenza contrattuale: € " + objSalvataggioImporti.Importo_assistenza.ToString().PadLeft(totale.ToString().Length) + " |";
                messaggio = messaggio + "Importo Abbonamenti: € " + objSalvataggioImporti.Importo_abb.ToString().PadLeft(totale.ToString().Length) + " |";
                messaggio = messaggio + "Importo Sanzioni: € " + objSalvataggioImporti.Importo_sanzione.ToString().PadLeft(totale.ToString().Length) + " |";
                messaggio = messaggio + "Importo Rettifiche contributo: € " + objSalvataggioImporti.Importo_rett_contrib.ToString().PadLeft(totale.ToString().Length) + " |";
                messaggio = messaggio + "Importo Rettifiche addizionale: € " + objSalvataggioImporti.Importo_rett_addiz.ToString().PadLeft(totale.ToString().Length) + " |";
                messaggio = messaggio + "Importo Rettifiche sanzione: € " + objSalvataggioImporti.Importo_rett_sanzione.ToString().PadLeft(totale.ToString().Length) + " |";
                messaggio = messaggio + "Importo Totale: € " + totale.ToString().PadLeft(totale.ToString().Length) + " |";
            }
            return mess_iniziale + " \n " + messaggio + " \n " + mess_finale;
        }

        public string VisualizzaRagioneSociale(string codPos)
        {
            return AnnullamentiBLL.GetRagioneSociale(codPos);
        }

        public ActionResult AnnullaDenuncia_Step3()
        {
            Utente utente = base.Session["utente"] as Utente;
            ImportiAnnullamentoDenunce objSalvataggioImporti = base.Session["objImporti_annullDen"] as ImportiAnnullamentoDenunce;
            List<ImportiFromDENTES> listaImporti = base.Session["listaImporti_annullDen"] as List<ImportiFromDENTES>;
            bool flgArr = objSalvataggioImporti.FlgArr;
            List<AnnullamentoDenunceOCM> listaDenunceDaAnnullare = base.Session["denunceDaAnnullare"] as List<AnnullamentoDenunceOCM>;
            if (AnnullamentiBLL.AnnullaDenuncia_Step3(listaDenunceDaAnnullare, flgArr, listaImporti, utente))
            {
                base.TempData["successMessage"] = "Operazione completata";
                return RedirectToAction("AnnullamentoDenunce");
            }
            base.TempData["errorMessage"] = AnnullamentiBLL.ErrorMessage;
            return RedirectToAction("AnnullamentoDenunce");
        }

        public ActionResult RettifichePA_AC()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RettifichePA_AC(RettifichePA_CA_OCM.Search_Rett rettifichePA_CA_OCM, string cerca)
        {
            RettificaPA_CA_BLL rettificaPA_CA_BLL = new RettificaPA_CA_BLL();
            RettifichePA_CA_OCM rettifichePA_CA_OCM2 = new RettifichePA_CA_OCM();
            rettifichePA_CA_OCM2.search_rett = rettifichePA_CA_OCM;
            Utente u = base.Session["utente"] as Utente;
            string MSGErorre = "";
            string MSGSuccess = "";
            if (!string.IsNullOrEmpty(cerca))
            {
                base.TempData["Rett"] = rettifichePA_CA_OCM;
                rettifichePA_CA_OCM2 = rettificaPA_CA_BLL.SearchRett(rettifichePA_CA_OCM2, cerca, ref MSGErorre, ref MSGSuccess);
            }
            if (MSGErorre != "")
            {
                base.TempData["errorMessage"] = MSGErorre;
            }
            else if (MSGSuccess != "")
            {
                base.TempData["successMessage"] = MSGSuccess;
            }
            return View(rettifichePA_CA_OCM2);
        }

        public JsonResult RettifichePA(string objList)
        {
            string MSGErorre = "";
            string MSGSuccess = "";
            RettificaPA_CA_BLL rettificaPA_CA_BLL = new RettificaPA_CA_BLL();
            RettifichePA_CA_OCM rettifichePA_CA_OCM1 = new RettifichePA_CA_OCM();
            Utente u = base.Session["utente"] as Utente;
            List<RettifichePA_CA_OCM.List_Rett> listrett = (rettifichePA_CA_OCM1.list_rett = JsonConvert.DeserializeObject<List<RettifichePA_CA_OCM.List_Rett>>(objList));
            rettifichePA_CA_OCM1.search_rett = (RettifichePA_CA_OCM.Search_Rett)base.TempData["Rett"];
            base.TempData.Peek("Rett");
            rettificaPA_CA_BLL.SalvaRett(rettifichePA_CA_OCM1, ref MSGErorre, ref MSGSuccess, u);
            if (MSGErorre != "")
            {
                base.TempData["errorMessage"] = MSGErorre;
            }
            else if (MSGSuccess != "")
            {
                base.TempData["successMessage"] = MSGSuccess;
            }
            return null;
        }

        public string VisualizzaRagioneSociale1(string codPos)
        {
            RettificaPA_CA_BLL rettificaPA_CA_BLL = new RettificaPA_CA_BLL();
            return rettificaPA_CA_BLL.GetRagioneSociale(codPos);
        }

        public ActionResult CaricaDatiModelloPrev()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CaricaDatiModelloPrev(ModelloPrevOCM.DatiModello dati)
        {
            string MSGErorre = "";
            string MSGSuccess = "";
            ModelloPrevOCM model = new ModelloPrevOCM();
            ModelloPrevBLL modelloBLL = new ModelloPrevBLL();
            model = modelloBLL.CaricaDatiModelloPrev(dati, ref MSGErorre, ref MSGSuccess);
            if (MSGErorre != "")
            {
                base.TempData["errorMessage"] = MSGErorre;
            }
            else if (MSGSuccess != "")
            {
                base.TempData["successMessage"] = MSGSuccess;
            }
            return View(model);
        }

        public JsonResult Rett(string objList)
        {
            List<Rettifiche_OCM.ListRett> ListaRettifiche = JsonConvert.DeserializeObject<List<Rettifiche_OCM.ListRett>>(objList);
            base.Session["ListaRet"] = ListaRettifiche;
            return new JsonResult
            {
                Data = JsonConvert.SerializeObject(ListaRettifiche),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult SalvataggioRettifiche()
        {
            string MSGErorre = "";
            string MSGSuccess = "";
            Utente u = base.Session["utente"] as Utente;
            Rettifiche_OCM Rett = new Rettifiche_OCM();
            Rettifiche_BLL rettifiche_BLL = new Rettifiche_BLL();
            List<Rettifiche_OCM.ListRett> ListaRettifiche = (Rett.listRett = base.Session["ListaRet"] as List<Rettifiche_OCM.ListRett>);
            rettifiche_BLL.SalvaRett(u, Rett, ref MSGErorre, ref MSGSuccess);
            if (MSGErorre != "")
            {
                base.TempData["errorMessage"] = MSGErorre;
            }
            else if (MSGSuccess != "")
            {
                base.TempData["successMessage"] = MSGSuccess;
            }
            return RedirectToAction("Rettifiche", "Amministrativo");
        }

        public ActionResult Rettifiche()
        {
            return View();
        }

        public ActionResult GeneraNotifiche(bool downloadLink = false)
        {
            int codPos = 0;
            List<SelectListItem> listaMesi = new List<SelectListItem>();
            Dictionary<int, string> dictMesi = NotificheUfficioBLL.PopolaDDLMesi();
            string dataSistema = NotificheUfficioBLL.GetDataSistema();
            for (int i = 0; i < dictMesi.Count; i++)
            {
                listaMesi.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = dictMesi[i]
                });
            }
            base.ViewBag.listaMesi = listaMesi;
            base.ViewBag.hd_meseDa = "";
            base.ViewBag.hd_meseA = "";
            if (base.TempData["ExcelPronto"] == null)
            {
                base.ViewBag.downloadLink = false;
            }
            else if (!(bool)base.TempData["ExcelPronto"])
            {
                base.ViewBag.downloadLink = false;
            }
            else
            {
                base.ViewBag.downloadLink = downloadLink;
            }
            if (dataSistema == null)
            {
                base.TempData["errorMessage"] = NotificheUfficioBLL.errorMessage;
                return View();
            }
            base.ViewBag.dataSistema = dataSistema;
            if (NotificheUfficioBLL.AutorizzazioniSpecialiSoloEnpaia(ref codPos, "CONTRIBUTI"))
            {
                base.ViewBag.codPos = codPos;
            }
            return View();
        }

        [HttpPost]
        public ActionResult GeneraNotifiche(string codPos_da, string codPos_a, string anno, string mese_da, string mese_a, bool downloadLink = false)
        {
            int codPos = 0;
            List<SelectListItem> listaMesi = new List<SelectListItem>();
            Dictionary<int, string> dictMesi = NotificheUfficioBLL.PopolaDDLMesi();
            NotificheDatiRicerca datiRicerca = new NotificheDatiRicerca
            {
                CodPos_da = codPos_da,
                CodPos_a = codPos_a,
                Anno = anno,
                Mese_da = mese_da,
                Mese_a = mese_a
            };
            string dataSistema = Convert.ToDateTime(NotificheUfficioBLL.GetDataSistema()).ToString("d");
            base.TempData["datiRicerca"] = datiRicerca;
            for (int i = 0; i < dictMesi.Count; i++)
            {
                listaMesi.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = dictMesi[i]
                });
            }
            base.ViewBag.listaMesi = listaMesi;
            base.ViewBag.hd_meseDa = mese_da;
            base.ViewBag.hd_meseA = mese_a;
            base.ViewBag.downloadLink = downloadLink;
            if (dataSistema == null)
            {
                base.TempData["errorMessage"] = NotificheUfficioBLL.errorMessage;
                return View();
            }
            base.ViewBag.dataSistema = dataSistema;
            if (NotificheUfficioBLL.AutorizzazioniSpecialiSoloEnpaia(ref codPos, "CONTRIBUTI"))
            {
                base.ViewBag.codPos = codPos;
            }
            if (!string.IsNullOrEmpty(codPos_da) && !NotificheUfficioBLL.AutorizzazioniSpeciali(codPos_da, "CONTRIBUTI"))
            {
                base.TempData["errorMessage"] = "Utente non abilitato";
                return View();
            }
            if (!string.IsNullOrEmpty(codPos_a) && !NotificheUfficioBLL.AutorizzazioniSpeciali(codPos_a, "CONTRIBUTI"))
            {
                base.TempData["errorMessage"] = "Utente non abilitato";
                return View();
            }
            List<NotificheUfficioOCM> listaNotifiche = NotificheUfficioBLL.GetData(codPos_da, codPos_a, anno, int.Parse(mese_da), int.Parse(mese_a));
            if (listaNotifiche == null)
            {
                base.TempData["errorMessage"] = NotificheUfficioBLL.errorMessage;
                base.ViewBag.numOccorrenze = 0;
                return View();
            }
            base.ViewBag.numOccorrenze = listaNotifiche.Count;
            return View(listaNotifiche);
        }

        public string CalcolaNotifiche(string jsonString)
        {
            DataTable dtLog = new DataTable();
            DataTable dtAziende = new DataTable();
            Utente utente = base.Session["utente"] as Utente;
            NotificheDatiRicerca datiRicerca = base.TempData["datiRicerca"] as NotificheDatiRicerca;
            List<NotificheUfficioOCM> listaNotifiche = JsonConvert.DeserializeObject<List<NotificheUfficioOCM>>(jsonString);
            string dataRiferimento = listaNotifiche[0].DataRiferimento;
            string causale = listaNotifiche[0].Causale;
            string strRicPrev = string.Empty;
            string pattern = "^([0-2][0-9]|(3)[0-1])(\\/)(((0)[0-9])|((1)[0-2]))(\\/)\\d{4}$";
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(dataRiferimento))
            {
                dataRiferimento = DateTime.Parse(dataRiferimento).ToString("d");
                base.TempData["dataRiferimento"] = dataRiferimento;
                DataColumn column = new DataColumn();
                column.ColumnName = "CODPOS";
                dtLog.Columns.Add(column);
                column = new DataColumn();
                column.ColumnName = "ANNDEN";
                dtLog.Columns.Add(column);
                column = new DataColumn();
                column.ColumnName = "MESDEN";
                dtLog.Columns.Add(column);
                column = new DataColumn();
                column.ColumnName = "MAT";
                dtLog.Columns.Add(column);
                column = new DataColumn();
                column.ColumnName = "DESERR";
                dtLog.Columns.Add(column);
                dtAziende.Columns.Add(new DataColumn("CODPOS", Type.GetType("System.Int32")));
                dtAziende.Columns.Add(new DataColumn("ANNO", Type.GetType("System.Int32")));
                dtAziende.Columns.Add(new DataColumn("MESE", Type.GetType("System.Int32")));
                dtAziende.Columns.Add(new DataColumn("DATSCA", Type.GetType("System.String")));
                dtAziende.Columns.Add(new DataColumn("TIPISC", Type.GetType("System.String")));
                dtAziende.Columns.Add(new DataColumn("RIMUOVI", Type.GetType("System.String")));
                foreach (NotificheUfficioOCM notifica in listaNotifiche)
                {
                    DataRow row = dtAziende.NewRow();
                    row["CODPOS"] = notifica.CodPos;
                    row["ANNO"] = notifica.Anno;
                    row["MESE"] = notifica.NumMese;
                    row["TIPISC"] = notifica.TipIsc;
                    row["DATSCA"] = string.Empty;
                    row["RIMUOVI"] = string.Empty;
                    dtAziende.Rows.Add(row);
                }
                string tipMovSan = ((causale == "rbEvasione") ? "SAN_MD" : ((!(causale == "rbRitardo")) ? string.Empty : "SAN_RD"));
                if (datiRicerca.CodPos_da != string.Empty)
                {
                    strRicPrev = strRicPrev + " AND MODPRE.CODPOS>= " + datiRicerca.CodPos_da;
                }
                if (datiRicerca.CodPos_a != string.Empty)
                {
                    strRicPrev = strRicPrev + " AND MODPRE.CODPOS<= " + datiRicerca.CodPos_a;
                }
                if (datiRicerca.Anno != string.Empty)
                {
                    strRicPrev = strRicPrev + " AND MODPREDET.ANNDEN= " + datiRicerca.Anno;
                }
                if (datiRicerca.Mese_da != string.Empty)
                {
                    strRicPrev = strRicPrev + " AND MODPREDET.MESDEN>= " + datiRicerca.Mese_da;
                }
                if (datiRicerca.Mese_a != string.Empty)
                {
                    strRicPrev = strRicPrev + " AND MODPREDET.MESDEN<= " + datiRicerca.Mese_a;
                }
                if (NotificheUfficioBLL.GeneraSalvaNotifiche(utente, dtAziende, dtLog, tipMovSan, dataRiferimento, strRicPrev))
                {
                    return "OK";
                }
                base.TempData["errorMessage"] = "Operazione fallita";
                return "NO";
            }
            base.TempData["errorMessage"] = "Data di emissione della notifica non valida";
            return null;
        }

        public ActionResult ContabilizzazioneNotifiche()
        {
            decimal tempoMassimoPrevisto = default(decimal);
            string tempoInizio = null;
            string codCauNot = NotificheUfficioBLL.GetCausaleMovimento("NU");
            List<CercaNotifiche> listaNotifiche = NotificheUfficioBLL.CercaNotifiche(ref tempoMassimoPrevisto, ref tempoInizio);
            base.ViewBag.lblCausale = codCauNot;
            base.TempData["tempo_massimo"] = tempoMassimoPrevisto;
            base.TempData["tempo_inizio"] = Convert.ToDateTime(tempoInizio);
            if (listaNotifiche != null)
            {
                base.ViewBag.occorrenze = listaNotifiche.Count;
                return View(listaNotifiche);
            }
            base.TempData["errorMessage"] = NotificheUfficioBLL.errorMessage;
            return View();
        }

        public string Contabilizza(string jsonString)
        {
            string messaggio = string.Empty;
            decimal importoAddizionale = default(decimal);
            decimal importoAbb = default(decimal);
            decimal importoAssistenza = default(decimal);
            decimal importoSanzione = default(decimal);
            decimal importoContributo = default(decimal);
            DateTime dataSistema = Convert.ToDateTime(NotificheUfficioBLL.GetDataSistema());
            DateTime tempoInizio = Convert.ToDateTime(base.TempData["tempo_inizio"]);
            decimal tempoMassimoPrevisto = Convert.ToDecimal(base.TempData["tempo_massimo"]);
            string avvisoTempoScaduto = $"Il tempo per la contabilizzazione previsto in {tempoMassimoPrevisto} secondi è scaduto. Chiudere la maschera e procedere di nuovo al calcolo delle notifiche.";
            TimeSpan ts_start = FromDataToTimeSpan(dataSistema);
            TimeSpan ts_end = FromDataToTimeSpan(tempoInizio);
            if ((decimal)(ts_start.Subtract(ts_end).Ticks / 10000000) > tempoMassimoPrevisto)
            {
                return avvisoTempoScaduto;
            }
            List<CercaNotifiche> listaNotifiche = JsonConvert.DeserializeObject<List<CercaNotifiche>>(jsonString);
            List<CercaNotifiche> listaNotificheSelezionate = listaNotifiche.Where((CercaNotifiche n) => n.Checked == "S").ToList();
            if (listaNotificheSelezionate != null)
            {
                foreach (CercaNotifiche notifica in listaNotificheSelezionate)
                {
                    importoAddizionale += notifica.ImpAddRec;
                    importoAbb += notifica.ImpAbb;
                    importoContributo += notifica.ImpCon;
                    importoAssistenza += notifica.ImpAssCon;
                    importoSanzione += notifica.ImpSanDet;
                }
            }
            decimal totale = importoSanzione + importoContributo + importoAbb + importoAddizionale + importoAssistenza;
            messaggio = $"Righe in lista: {listaNotifiche.Count} - Righe selezionate: {listaNotificheSelezionate.Count} |";
            messaggio += $"Importo Contributo: € {importoContributo} |";
            messaggio += $"Importo Addizionale: € {importoAddizionale} |";
            messaggio += $"Importo Assistenza contrattuale:  € {importoAssistenza} |";
            messaggio += $"Importo Abbonamenti:  € {importoAbb} |";
            messaggio += $"Importo Sanzioni:  € {importoSanzione} |";
            messaggio += $"Importo Totale:  € {totale} |";
            messaggio += "Procedere con la contabilizzazione delle notifiche selezionate?";
            base.Session["notifiche_da_contabilizzare"] = listaNotifiche;
            return messaggio;
        }

        public ActionResult Contabilizza_Step2()
        {
            ExcelExport ExcelExporter = new ExcelExport();
            string filePath1 = "";
            bool cont_effettuata = false;
            Utente utente = base.Session["utente"] as Utente;
            string dataRiferimento = base.TempData["dataRiferimento"].ToString();
            List<CercaNotifiche> listaNotifiche = base.Session["notifiche_da_contabilizzare"] as List<CercaNotifiche>;
            if (!Directory.Exists(base.Server.MapPath("/Resources") + "\\Excels"))
            {
                Directory.CreateDirectory(base.Server.MapPath("/Resources") + "\\Excels");
            }
            DirectoryInfo info = new DirectoryInfo(base.Server.MapPath("/Resources") + "\\Excels");
            int count = info.GetFiles().Length;
            filePath1 = base.Server.MapPath("/Resources") + "\\Excels" + $"/ExportNotifiche{count + 1}.xlsx";
            List<NotificheBase> lista = new List<NotificheBase>();
            if ((lista = NotificheUfficioBLL.Contabilizza_Step2(listaNotifiche, dataRiferimento, utente, ref cont_effettuata, filePath1)) != null)
            {
                base.TempData["cont_effettuata"] = cont_effettuata;
                base.TempData["infoMessage"] = NotificheUfficioBLL.infoMessage;
                base.TempData["successMessage"] = "Operazione completata";
                base.TempData["ExcelPronto"] = true;
                base.TempData.Keep("ExcelPronto");
                if (lista.Count > 0)
                {
                    TFI.BLL.Utilities.ExcelExport.excelExporter.ExportExcelDataTable(lista, filePath1);
                }
                return RedirectToAction("GeneraNotifiche", new
                {
                    downloadLink = (lista.Count > 0)
                });
            }
            base.TempData["errorMessage"] = NotificheUfficioBLL.errorMessage;
            return RedirectToAction("ContabilizzazioneNotifiche");
        }

        [HttpGet]
        public FileResult DownloadFileExcel()
        {
            string ErrorMSG = "";
            try
            {
                DirectoryInfo info = new DirectoryInfo(base.Server.MapPath("/Resources") + "\\Excels");
                int count = info.GetFiles().Length;
                string filePath2 = base.Server.MapPath("/Resources") + "\\Excels" + $"/ExportNotifiche{count}.xlsx";
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath2);
                string fileName = $"ExportNotifiche{count}.xlsx";
                FileContentResult file = File(fileBytes, "application/octet-stream", fileName);
                if (Directory.Exists(base.Server.MapPath("/Resources") + "\\Excels"))
                {
                    if (count == 0)
                    {
                        Directory.Delete(base.Server.MapPath("/Resources") + "\\Excels");
                    }
                    else
                    {
                        FileInfo[] files = info.GetFiles();
                        foreach (FileInfo fileDaEliminare in files)
                        {
                            fileDaEliminare.Delete();
                        }
                        Directory.Delete(base.Server.MapPath("/Resources") + "\\Excels");
                    }
                }
                base.TempData["ExcelPronto"] = false;
                return file;
            }
            catch (Exception)
            {
                ErrorMSG = "Download Fallito";
                if (ErrorMSG != "")
                {
                    base.TempData["errorMessage"] = ErrorMSG;
                }
                return null;
            }
        }

        private TimeSpan FromDataToTimeSpan(DateTime _dt)
        {
            return TimeSpan.FromTicks(_dt.Ticks);
        }

        [HttpPost]
        public ActionResult Rettifiche(string cerca, Rettifiche_OCM.SearchRett searchRett)
        {
            string MSGErorre = "";
            string MSGSuccess = "";
            Utente u = base.Session["utente"] as Utente;
            Rettifiche_BLL rettifiche_BLL = new Rettifiche_BLL();
            Rettifiche_OCM rettifiche_OCM = new Rettifiche_OCM();
            Rettifiche_OCM rettifiche_OCM2 = new Rettifiche_OCM();
            if (!string.IsNullOrEmpty(cerca))
            {
                rettifiche_OCM.searchRett = searchRett;
                rettifiche_OCM2 = rettifiche_BLL.SearchRett(rettifiche_OCM, ref MSGErorre, ref MSGSuccess, u);
            }
            if (MSGErorre != "")
            {
                base.TempData["errorMessage"] = MSGErorre;
            }
            else if (MSGSuccess != "")
            {
                base.TempData["successMessage"] = MSGSuccess;
            }
            return View(rettifiche_OCM2);
        }

        public string VisualizzaNominativo(string mat)
        {
            Rettifiche_BLL rettifica_BLL = new Rettifiche_BLL();
            return rettifica_BLL.GetNominativo(mat);
        }

        public string ContabilizzazioneNotifiche_exit(string jsonString)
        {
            List<CercaNotifiche> listaNotifiche = JsonConvert.DeserializeObject<List<CercaNotifiche>>(jsonString);
            NotificheUfficioBLL.ContabilizzazioneNotifiche_exit(listaNotifiche);
            return "OK";
        }

        public ActionResult DettaglioImporti(string CodPos, string AnnDen, string MesDen, string ProDen, string Matricola, string ProDenDet, string Periodo, string TipMov, string RagSoc, string Nominativo, string MovRet)
        {
            NotificheUfficioOCM notifiche = new NotificheUfficioOCM();
            base.ViewBag.CodPos = CodPos;
            base.ViewBag.Matricola = Matricola;
            base.ViewBag.Periodo = Periodo;
            base.ViewBag.TipMov = TipMov;
            base.ViewBag.RagSoc = RagSoc;
            base.ViewBag.Nominativo = Nominativo;
            notifiche.CodPos = Convert.ToInt32(CodPos);
            notifiche.AnnDen = AnnDen;
            notifiche.MesDen = MesDen;
            notifiche.ProDen = ProDen;
            notifiche.Matricola = Matricola;
            notifiche.ProDenDet = ProDenDet;
            notifiche.Periodo = Periodo;
            List<NotificheUfficioOCM> ListaDettaglioImporti = NotificheUfficioBLL.CaricaDatiDettaglioImporti(notifiche, MovRet);
            return View(ListaDettaglioImporti);
        }

        public ActionResult DettaglioDenuncia(string Codpos, string AnnDen, string MesDen, string ProDen)
        {
            NotificheUfficioOCM notificheUfficioOCM = new NotificheUfficioOCM();
            notificheUfficioOCM.CodPos = Convert.ToInt32(Codpos);
            notificheUfficioOCM.AnnDen = AnnDen;
            notificheUfficioOCM.MesDen = MesDen;
            notificheUfficioOCM.ProDen = ProDen;
            string RagSoc = "";
            string tipomovimento = "";
            List<NotificheUfficioOCM> listDenuce = NotificheUfficioBLL.DettaglioDenuncia(notificheUfficioOCM, ref RagSoc, ref tipomovimento);
            if (listDenuce != null)
            {
                string nomMes = "";
                switch (Convert.ToInt32(MesDen))
                {
                    case 1:
                        nomMes = "Gennaio";
                        break;
                    case 2:
                        nomMes = "Febbraio";
                        break;
                    case 3:
                        nomMes = "Marzo";
                        break;
                    case 4:
                        nomMes = "Aprile";
                        break;
                    case 5:
                        nomMes = "Maggio";
                        break;
                    case 6:
                        nomMes = "Giugno";
                        break;
                    case 7:
                        nomMes = "Luglio";
                        break;
                    case 8:
                        nomMes = "Agosto";
                        break;
                    case 9:
                        nomMes = "Settembre";
                        break;
                    case 10:
                        nomMes = "Ottobre";
                        break;
                    case 11:
                        nomMes = "Novembre";
                        break;
                    case 12:
                        nomMes = "Dicembre";
                        break;
                }
                base.ViewBag.AnnDen = AnnDen;
                base.ViewBag.MesDen = MesDen;
                base.ViewBag.Periodo = AnnDen + " " + nomMes;
                base.ViewBag.CodPos = Codpos;
                base.ViewBag.ProDen = ProDen;
                base.ViewBag.RagSoc = RagSoc;
                base.ViewBag.Tipmov = tipomovimento;
                base.ViewBag.Occorrenze = listDenuce.Count;
                return View(listDenuce);
            }
            return View();
        }

        public string StampaDocumento_step1(string codPos, string annDen, string mesDen, string proDen, string tipoMovimento)
        {
            StampaDocumentoNotifiche stampaDocumento = NotificheUfficioBLL.GetStampaObj(codPos, annDen, mesDen, proDen);
            if (stampaDocumento != null)
            {
                if (stampaDocumento.NumMov.Trim() != string.Empty)
                {
                    if (stampaDocumento.EsiRet.Trim() == "S")
                    {
                        base.TempData["codPos"] = codPos;
                        base.TempData["annDen"] = annDen;
                        base.TempData["mesDen"] = mesDen;
                        base.TempData["proDen"] = proDen;
                        return tipoMovimento + " con RETTIFICA. Procedere con la stampa?";
                    }
                    return "NO";
                }
                return "Movimento non contabilizzato. Impossibile stampare";
            }
            return "NO";
        }

        public ActionResult StampaDocumento_step2()
        {
            string codPos = base.TempData["codPos"].ToString();
            string annDen = base.TempData["annDen"].ToString();
            string mesDen = base.TempData["mesDen"].ToString();
            string proDen = base.TempData["proDen"].ToString();
            List<StampaDocumentoNotifiche> listaStampaDocumento = NotificheUfficioBLL.GetStampaList(codPos, annDen, mesDen, proDen);
            return RedirectToAction("DettaglioDenuncia", new
            {
                Codpos = codPos,
                AnnDen = annDen,
                MesDen = mesDen,
                ProDen = proDen
            });
        }
    }
}
