using System;
using System.Configuration;
using System.Web.Mvc;
using log4net;
using Newtonsoft.Json;
using TFI.BLL.Crypto;
using TFI.BLL.Stampa;
using TFI.OCM.Stampa;
using TFI.OCM.Utente;
using TFI.Utilities;

namespace TFI.Controllers
{
    //[CheckLogin]
    [UserExpiredCheck]
    public class StampaController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger("RollingFile");

        private static readonly ILog TrackLog = LogManager.GetLogger("Track");

        public ActionResult Stampa(string fileName = "", string fileEC = "", string tipMov = "", string datiDiStampa = null)
        {
           
            DatiDiStampa dati =JsonConvert.DeserializeObject<DatiDiStampa>(datiDiStampa);
            if (!string.IsNullOrEmpty(fileName))
            {
                base.Response.AddHeader("Content-disposition", "filename = " + fileName);
                base.Response.TransmitFile(fileName);
            }
            else if (string.IsNullOrEmpty(fileEC))
            {
             
                string dateTime = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_")
                    .Replace(".", "_") + "_" + DateTime.Now.Millisecond;
                string basePath = base.Request.ServerVariables["APPL_PHYSICAL_PATH"] + "bin\\";
                if (!string.IsNullOrEmpty(tipMov))
                {
                    string @case = tipMov.Trim().ToUpper();
                    switch (@case)
                    {
                        case "MAVDIFF":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("MAVDIFF", dati).GetBuffer());
                            break;
                        case "PREV_MOD":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("PREV_MOD", dati).GetBuffer());
                            break;
                        case "NU":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("NU", dati).GetBuffer());
                            break;
                        case "AR":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("AR", dati).GetBuffer());
                            break;
                        case "DP":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("DP", dati).GetBuffer());
                            break;
                        case "SA":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("SA", dati).GetBuffer());
                            break;
                        case "SD":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("SD", dati).GetBuffer());
                            break;
                        case "RV":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("RV", dati).GetBuffer());
                            break;
                        case "PREV":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("PREV", dati).GetBuffer());
                            break;
                        case "RDL":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("RDL", dati).GetBuffer());
                            break;
                        case "MAV":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("MAV", dati).GetBuffer());
                            break;
                        case "MAVSANIT":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("MAVSANIT", dati).GetBuffer());
                            break;
                        case "DENSANIT":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("DENSANIT", dati).GetBuffer());
                            break;
                        case "RICSANIT":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("RICSANIT", dati).GetBuffer());
                            break;
                        case "ADE":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("ADE", dati).GetBuffer());
                            break;
                        case "PROT_RIC_RDL":
                            base.Response.BinaryWrite(StampaBLL.GetPdfMethods("PROT_RIC_RDL", dati).GetBuffer());
                            break;
                        default:
                            if (@case == "")
                            {
                                StampaBLL.GetPdfMethods(string.Empty, dati);
                            }
                            break;
                    }
                    fileName = tipMov + "_" + dateTime + "_.pdf";
                    base.Response.AddHeader("Content-disposition", "inline; filename=" + fileName);
                }
                else
                {
                    base.Response.AddHeader("Content-disposition", "inline; filename=" + fileName);
                    base.Response.TransmitFile(fileName);
                }
            }
            else
            {
                base.Response.AddHeader("Content-disposition", "filename = " + fileName);
                base.Response.TransmitFile(fileEC);
            }
            base.Response.End();
            return View();
        }

        public ActionResult StampaRicevuta_Denunce(int anno, int mese, int proDen, string tipMov, string prot)
        {
            Utente utente = base.Session["utente"] as Utente;
            string connessioneProtocollo = Cypher.DeCryptPassword(ConfigurationManager.AppSettings["ConnProtocollo"]);
            string fileName = string.Empty;
            string numPro = string.Empty;
            string datPro = string.Empty;
            string ragioneSociale = utente.DescrizioneAzienda.Substring(utente.DescrizioneAzienda.IndexOf('-') + 1);
            string ricevutaDIPA = $"RICEVUTA DIPA - POS : {utente.CodPosizione} - PERIODO : {anno}/{mese}";
            string basePath = base.Request.ServerVariables["APPL_PHYSICAL_PATH"] + "bin\\";
            if (StampaBLL.StampaRicevuta_Denunce(utente, connessioneProtocollo, ref fileName, ragioneSociale, ricevutaDIPA, prot, numPro, datPro, anno, mese, proDen, basePath, tipMov))
            {
                return RedirectToAction("Stampa", new { fileName });
            }
            base.TempData["errorMessage"] = StampaBLL.ErrorMessage;
            return RedirectToAction("DenunciaMensile_Totali", "AziendaConsulente");
        }
    }
}
