
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TFI.BLL;
using TFI.OCM;
using TFI.Utilities;

namespace TFI.Controllers
{
    [UserExpiredCheck]
    public class AdminController : Controller
    {
        public ActionResult Search()
        {
            return null;
        }

        public ActionResult Index()
        {
            AdminBLL utente = new AdminBLL();
            List<Admin> admins = new List<Admin>();
            List<Admin> admins2 = new List<Admin>();
            List<Admin> admins3 = new List<Admin>();
            List<Admin> adminsFinal = new List<Admin>();
            Admin adm = null;
            admins = utente.GetDropDpownList1();
            admins2 = utente.GetDropDpownList2();
            admins3 = utente.GetDropDpownList3();
            foreach (Admin itm3 in admins)
            {
                adm = new Admin();
                adm.codSis = itm3.codSis;
                adm.sistema = itm3.sistema;
                adm.NumCmb = 1;
                adminsFinal.Add(adm);
            }
            foreach (Admin itm2 in admins2)
            {
                adm = new Admin();
                adm.NumCmb = 2;
                adm.codGru = itm2.codGru;
                adm.denGru = itm2.denGru;
                adminsFinal.Add(adm);
            }
            foreach (Admin itm in admins3)
            {
                adm = new Admin();
                adm.NumCmb = 3;
                adm.denFun = itm.denFun;
                adm.codFun = itm.codFun;
                adminsFinal.Add(adm);
            }
            return View(adminsFinal);
        }

        [HttpPost]
        public ActionResult Index(string codiceUtente, string selezionaDenominazione, string dropStato, string selezionaSistema, string selezionaGruppo, string selezionaFunzione, string codiceUtenteIns, string selezionaDenominazioneIns, string email, string codFis, string uteWin, string submit_button)
        {
            base.Session["ciao"] = codiceUtente + "," + selezionaDenominazione + "," + dropStato + "," + selezionaSistema + "," + selezionaGruppo + "," + selezionaFunzione;
            AdminBLL utente = new AdminBLL();
            if (submit_button == "Inserisci")
            {
                base.Session["ListaInserimento"] = utente.GetInserimento(codiceUtenteIns, selezionaDenominazioneIns, email, codFis, uteWin);
            }
            else
            {
                base.Session["ListaAdmin"] = utente.GetAnagrafica(codiceUtente, selezionaDenominazione, dropStato, selezionaSistema, selezionaGruppo, selezionaFunzione);
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public ActionResult DettagliUtenti(string customerJSON)
        {
            AdminBLL utente = new AdminBLL();
            List<Admin> customer = new List<Admin>();
            Admin customer2 = new JavaScriptSerializer().Deserialize<Admin>(customerJSON);
            base.Session["GruppiWeb"] = utente.GetGruppiWeb(customer2.codUtente);
            base.Session["FunzioniWeb"] = utente.GetFunzioniWeb(customer2.codUtente);
            customer.Add(customer2);
            return View(customer);
        }

        [HttpPost]
        public ActionResult _Salva(string codUtente, string nome, string codFis, string email, string winUt, bool checkGruppo = false, bool checkFunzione = false)
        {
            AdminBLL utente = new AdminBLL();
            Admin funzioni = null;
            Admin gruppi = null;
            string result = base.Request.Form["Gruppi"];
            string result2 = base.Request.Form["Funzioni"];
            List<string> gruppoSelezionato = new List<string>();
            List<string> funzioneSelezionata = new List<string>();
            List<Admin> listaGruppi = new List<Admin>();
            List<Admin> listaFunzioni = new List<Admin>();
            List<Admin> a = utente.GetDati(codUtente, nome, codFis, email, winUt, checkGruppo, checkFunzione);
            if (result != null)
            {
                result = base.Request.Form["Gruppi"].ToString();
                gruppoSelezionato = result.Split(',').ToList();
                foreach (string sel2 in gruppoSelezionato)
                {
                    if (sel2 != "")
                    {
                        gruppi = new Admin();
                        gruppi.denGrusis = sel2;
                        listaGruppi.Add(gruppi);
                    }
                }
                List<Admin> b2 = utente.DeleteSaveGruppi(codUtente, listaGruppi);
            }
            else
            {
                List<Admin> b = utente.DeleteSaveGruppi(codUtente, listaGruppi);
            }
            if (result2 != null)
            {
                result2 = base.Request.Form["Funzioni"].ToString();
                funzioneSelezionata = result2.Split(',').ToList();
                foreach (string sel in funzioneSelezionata)
                {
                    if (sel != "")
                    {
                        funzioni = new Admin();
                        funzioni.denFunsis = sel;
                        listaFunzioni.Add(funzioni);
                    }
                }
                List<Admin> c2 = utente.DeleteSaveFunzioni(codUtente, listaFunzioni);
            }
            else
            {
                List<Admin> c = utente.DeleteSaveFunzioni(codUtente, listaFunzioni);
            }
            base.Session["ListaAdmin"] = null;
            base.Session["GruppiWeb"] = null;
            base.Session["FunzioniWeb"] = null;
            return RedirectToAction("Index", "Admin");
        }

        public JsonResult CatturaCheck(string id)
        {
            AdminBLL utente = new AdminBLL();
            List<Admin> listacheck = new List<Admin>();
            Admin checkbox = new Admin();
            checkbox.codGru = id.ToString();
            listacheck = utente.GetcheckboxSelected(id);
            return new JsonResult
            {
                Data = listacheck,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult AnagraficaGruppoUtente()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AnagraficaGruppoUtente(string selezionaNomeGruppo, string newGrup, string nuovoNomeGruppo, string oldGrup)
        {
            AdminBLL utente = new AdminBLL();
            if (!string.IsNullOrEmpty(selezionaNomeGruppo))
            {
                return View(utente.GetTabella(selezionaNomeGruppo));
            }
            if (!string.IsNullOrEmpty(newGrup))
            {
                string s2 = utente.GetSalvataggio(newGrup, oldGrup);
                if (s2 == null)
                {
                    base.TempData["errorMessage"] = AdminBLL.Message;
                }
                else
                {
                    base.TempData["successMessage"] = AdminBLL.Message;
                }
                return View(s2);
            }
            if (!string.IsNullOrEmpty(nuovoNomeGruppo))
            {
                string s = utente.GetNuovoGruppo(nuovoNomeGruppo);
                if (s == null)
                {
                    base.TempData["errorMessage"] = AdminBLL.Message;
                }
                else
                {
                    base.TempData["successMessage"] = AdminBLL.Message;
                }
                return View(s);
            }
            return View();
        }

        [HttpGet]
        public ActionResult AnagraficaFunzionalita(string selezioneNomeFunz, string newFun, string nuovaFun, string oldFun)
        {
            AdminBLL utente = new AdminBLL();
            if (!string.IsNullOrEmpty(selezioneNomeFunz))
            {
                return View(utente.AnagraficaFunzionalitaBLL(selezioneNomeFunz));
            }
            if (!string.IsNullOrEmpty(newFun))
            {
                string s2 = utente.SalvataggioFunBLL(newFun, oldFun);
                if (s2 == null)
                {
                    base.TempData["errorMessage"] = AdminBLL.Message;
                }
                else
                {
                    base.TempData["successMessage"] = AdminBLL.Message;
                }
                return View(s2);
            }
            if (!string.IsNullOrEmpty(nuovaFun))
            {
                string s = utente.NewFunBLL(nuovaFun);
                if (s == null)
                {
                    base.TempData["errorMessage"] = AdminBLL.Message;
                }
                else
                {
                    base.TempData["successMessage"] = AdminBLL.Message;
                }
                return View(s);
            }
            return View();
        }

        [HttpGet]
        public ActionResult GruppoFunzionalita(string oldGrup)
        {
            AdminBLL utente = new AdminBLL();
            return View(utente.GruppoFunzionalitaBLL(oldGrup));
        }
    }
}
