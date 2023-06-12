using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Claims;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Web.Mvc;
using BLL.Iscritto;
using log4net;
using TFI.BLL.Iscritto;
using TFI.BLL.Login;
using TFI.BLL.Utilities;
using TFI.Models;
using TFI.OCM.Iscritto;
using TFI.OCM.Login;
using TFI.OCM.Utente;
using TFI.Utilities;
using TFI.Utilities.Validators;

namespace TFI.Controllers
{
    public class LoginController : Controller
    {
        private static readonly string STATIC_PATH_TMP = System.Web.HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings.Get("TMPCryptoFile").ToString());

        private static readonly ILog log = LogManager.GetLogger("RollingFile");

        private static readonly ILog TrackLog = LogManager.GetLogger("Track");


        public ActionResult Accesso()
        {
            return View();
        }
        public ActionResult Login(string tipoUtente)
        {
            return View();
        }


        public ActionResult Amministrativo(string type)
        {
            UtilsLog uti = new UtilsLog();
            uti.CheckLog();
            string ddl_type = base.Request.QueryString["type"];
            List<SelectListItem> options = null;
            options = ((!(ddl_type == "ext")) ? new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "SELEZIONA",
                    Value = string.Empty,
                    Selected = true
                },
                new SelectListItem
                {
                    Text = "ADMIN",
                    Value = "AD"
                },
                new SelectListItem
                {
                    Text = "ENPAIA",
                    Value = "E"
                }
            } : new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "SELEZIONA",
                    Value = string.Empty,
                    Selected = true
                },
                new SelectListItem
                {
                    Text = "AZIENDA",
                    Value = "A"
                },
                new SelectListItem
                {
                    Text = "CONSULENTE",
                    Value = "C"
                },
                new SelectListItem
                {
                    Text = "ISCRITTO",
                    Value = "I"
                },
                new SelectListItem
                {
                    Text = "MEDICO LEGALE",
                    Value = "M"
                }
            });
            if ((string)base.TempData["modal"] == "si")
            {
                if (isOTPSent())
                {
                    base.ViewBag.ViewToast = true;
                    base.ViewData["modal"] = "si";
                    base.TempData["infoMessage"] = string.Format(" OTP: {0}", base.TempData["OTP"]);
                }
                else
                {
                    base.ViewBag.ViewToast = true;
                    base.TempData["errorMessage"] = "OTP non inviato: " + LoginBLL.errorMessage;
                }
            }
            base.ViewData["tipoAccesso"] = ddl_type;
            return View("Index", options);
        }

        [HttpPost]
        public ActionResult TryToLogin(Credenziali credenziali)
        {
            Utente utente = LoginBLL.Login(credenziali);
            if (utente != null)
            {
                base.Session["utente"] = utente;
                base.Session["NomeUtente"] = utente.Denominazione;
                base.Session["NomeAzienda"] = utente.NomeAzienda;
                base.Session["Posizione"] = utente.CodPosizione;
                base.Session["TipoUtenza"] = utente.Tipo;
                base.Session["layout"] = LoginBLL.SelectLayout(credenziali.TipoUtente);
                if (credenziali.TipoUtente == "I")
                {
                    string ErroreMSG = string.Empty;
                    string SuccessMSG = string.Empty;
                    GestionePraticheBLL gestionePraticeBLL = new GestionePraticheBLL();
                    Session["HasPratiche"] = gestionePraticeBLL.CheckHasPratiche(utente);
                    RicPagTFR_BLL tfr_bll = new RicPagTFR_BLL();
                    base.Session["IsTfr"] = tfr_bll.CheckTfr(utente);
                    AnticipazioniTFRBLL ant_tft_bll = new AnticipazioniTFRBLL();
                    base.Session["IsAnticipo"] = ant_tft_bll.CheckAnticipoTFR(utente);
                    //FondoContoAssegniBLL Fondo = new FondoContoAssegniBLL();
                    FondoContoIndBLL fondoContoIndBLL = new FondoContoIndBLL();
                    base.Session["HasFondoConto"] = fondoContoIndBLL.HasFondoConto(utente.CodFiscale);
                    //string ErroreMSG = string.Empty;
                    //string SuccessMSG = string.Empty;
                    //string InfoMSG = string.Empty;
                    //var fondoAnag = Fondo.GetFondoContoAssegni(utente.Username, ref ErroreMSG, ref SuccessMSG, ref InfoMSG);
                    //base.Session["IsAssegno"] = fondoAnag.ShowBtn;
                }
                if (credenziali.TipoUtente == "E")
                {
                    return RedirectToAction("Index", "Amministrativo");
                }
                return RedirectToAction("CheckOTP");
            }
            if (LoginBLL.errorMessage == string.Empty)
            {
                base.TempData["errorMessage"] = "Inserire tutti i dati richiesti";
                return RedirectToAction("Login", new
                {
                    tipoUtente = credenziali.TipoAccesso
                });
            }
            base.TempData["errorMessage"] = LoginBLL.errorMessage;
            return RedirectToAction("Login", new
            {
                tipoUtente = credenziali.TipoAccesso
            });
        }

        public ActionResult CheckOTP()
        {
            Utente utente = base.TempData["UtenteTMP"] as Utente;
            if (base.Session["TipoUtenza"] != null)
            {
                if (base.Session["TipoUtenza"].ToString() == "C")
                {
                    return RedirectToAction("Index", "Consulente");
                }
                if (base.Session["TipoUtenza"].ToString() == "E")
                {
                    return RedirectToAction("Index", "Amministrativo");
                }
                return RedirectToAction("Index", "Home");
            }
            base.Session["utente"] = utente;
            base.Session["NomeUtente"] = utente.Denominazione;
            base.Session["NomeAzienda"] = utente.NomeAzienda;
            base.Session["Posizione"] = utente.CodPosizione;
            base.Session["TipoUtenza"] = utente.Tipo;
            base.Session["layout"] = LoginBLL.SelectLayout(utente.Tipo);
            return RedirectToAction("Index", "Consulente");
        }

        [HttpPost]
        public ActionResult CheckOTPDeleghe(string otp, string tipoAccesso)
        {
            Utente utente = base.TempData["UtenteTMP"] as Utente;
            if (LoginBLL.ControllaOTPDeleghe(otp, utente.Username))
            {
                return RedirectToAction("CheckOTP");
            }
            base.TempData["errorMessage"] = LoginBLL.errorMessage;
            return RedirectToAction("Index", new
            {
                type = tipoAccesso
            });
        }

        private bool isOTPSent()
        {
            int? otp = null;
            int? num = (otp = LoginBLL.SendOTP());
            if (num.HasValue)
            {
                base.TempData["OTP"] = otp;
                base.TempData.Keep("OTP");
                return true;
            }
            return false;
        }

        public ActionResult LogOut()
        {
            base.Session.Clear();
            base.Session.Abandon();
            return RedirectToAction("Accesso");
        }

        public ActionResult PasswordRecovery(string tipoUtente)
        {
            return View(new PasswordRecoveryViewModel() { TipoUtente = tipoUtente });
        }

        [HttpPost]
        public ActionResult PasswordRecovery(PasswordRecoveryViewModel data)
        {
            if (!ModelState.IsValid)
            {
                base.TempData["errorMessage"] = "Errore nell'elaborazione della richiesta";
                return View(data.TipoUtente);
            }

            LoginBLL.PasswordRecovery(data.Username, data.TipoUtente);

            return View("PasswordRecoverySuccess");
        }

        public ActionResult PasswordRecoverySuccess()
        {
            return View();
        }

        public ActionResult PasswordReset(string token)
        {
            return View(new PasswordResetViewModel { Token = token });
        }

        [HttpPost]
        public ActionResult PasswordReset(PasswordResetViewModel data)
        {
            try
            {
                if (!ModelState.IsValid || string.IsNullOrWhiteSpace(data.Password) || string.IsNullOrWhiteSpace(data.RepeatPassword))
                {
                    TempData["errorMessage"] = "Inserire tutti i dati richiesti";
                    return View(data);
                }

                if (data.Password != data.RepeatPassword)
                {
                    TempData["errorMessage"] = "Le password devono coincidere";
                    return View(data);
                }

                var isPasswordValid = PasswordHelper.CheckPassword(data.Password, out var validationError);
                if (!isPasswordValid)
                {
                    TempData["errorMessage"] = validationError;
                    return View(data);
                }

                var result = TokenHelper.ValidateToken(data.Token, ClaimTypes.NameIdentifier, "tipoUtente");
                var flag1 = result.TryGetValue(ClaimTypes.NameIdentifier, out var identifier);
                var flag2 = result.TryGetValue("tipoUtente", out var tipoUtente);
                if (!flag1 || !flag2)
                {
                    TempData["errorMessage"] = "Errore durante il recupero dei dati";
                    return View(data);
                }

                LoginBLL.ResetPassword(identifier, tipoUtente, data.Password);

                if (base.HttpContext.Items["errorMessage"] != null)
                {
                    base.ViewBag.ViewToast = true;
                    base.TempData["errorMessage"] = base.HttpContext.Items["errorMessage"].ToString();
                    return View("Accesso");
                }

                base.TempData["successMessage"] = "Password modificata correttamente!";
                return View("Accesso");
            }
            catch (SecurityTokenExpiredException ex)
            {
                TempData["errorMessage"] = "Token scaduto";
                return View(data);
            }
            catch
            {
                TempData["errorMessage"] = "Errore durante il reset della password";
                return View(data);
            }
        }
    }
}
