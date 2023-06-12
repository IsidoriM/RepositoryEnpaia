// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Login.LoginBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using DocumentFormat.OpenXml.Drawing.Charts;
using log4net;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using TFI.BLL.Utilities;
using TFI.CRYPTO.Crypto;
using TFI.DAL.Login;
using TFI.OCM.Login;

namespace TFI.BLL.Login
{
    public static class LoginBLL
    {
        private static readonly ILog log = LogManager.GetLogger("RollingFile");
        private static readonly ILog TrackLog = LogManager.GetLogger("Track");
        public static string errorMessage = string.Empty;

        public static TFI.OCM.Utente.Utente Login(Credenziali credenziali)
        {
            string empty = string.Empty;
            bool flag = true;
            if (string.IsNullOrEmpty(credenziali.Password))
            {
                LoginBLL.errorMessage = "Inserire Password";
                return (TFI.OCM.Utente.Utente)null;
            }
            try
            {
                string password = Cypher.CryptPassword(credenziali.Password.Trim());
                TFI.OCM.Utente.Utente userNoRetired;
                if ((userNoRetired = LoginDAL.GetUserNoRetired(credenziali.Username.Trim(), credenziali.TipoUtente)) != null)
                {
                    if (userNoRetired.Password == password)
                    {
                        if (LoginDAL.VerifyPassword(password))
                        {
                            if (userNoRetired.Tipo == "A" || userNoRetired.Tipo == "I")
                            {
                                if (userNoRetired.Tipo == "A")
                                {
                                    string ragioneSociale = LoginDAL.GetRagioneSociale(userNoRetired.Username);
                                    userNoRetired.DescrizioneAzienda = userNoRetired.CodPosizione + "-" + ragioneSociale;
                                }
                                else
                                {
                                    int matricola;
                                    if (userNoRetired.Tipo == "I" && (matricola = LoginDAL.GetMatricola(userNoRetired)) != 0)
                                        userNoRetired.DescrizioneAzienda = string.Format("{0}-{1}", (object)matricola, (object)userNoRetired.Denominazione);
                                }
                                while (flag)
                                {
                                    if (userNoRetired.DescrizioneAzienda.Substring(0, 1) == "0")
                                        userNoRetired.DescrizioneAzienda.Substring(1);
                                    else
                                        flag = false;
                                }
                            }
                            return userNoRetired;
                        }
                        LoginBLL.errorMessage = "Password scaduta";
                        return (TFI.OCM.Utente.Utente)null;
                    }
                    LoginBLL.errorMessage = "Username o Password errati";
                    return (TFI.OCM.Utente.Utente)null;
                }
                LoginBLL.errorMessage = "Username o Password errati";
                return (TFI.OCM.Utente.Utente)null;
            }
            catch (Exception ex)
            {
                LoginBLL.log.Info((object)string.Format("[TFI.BLL] : LoginBLL - E' stata generata un'eccezione in data: {0}. Messaggio: {1}", (object)DateTime.Now, (object)ex.Message));
                LoginBLL.errorMessage = "Eccezione generata in fase di Login " + ex.Message;
                return (TFI.OCM.Utente.Utente)null;
            }
        }

        public static int? SendOTP()
        {
            int num = new Random().Next(100000, 999999);
            try
            {
                return new int?(num);
            }
            catch (Exception ex)
            {
                LoginBLL.errorMessage = ex.Message;
                return new int?();
            }
        }

        public static bool ControllaOTPDeleghe(string otp, string username)
        {
            try
            {
                if (LoginDAL.ControllaOTPDeleghe(otp, username))
                    return true;
                LoginBLL.errorMessage = "Il codice OTP fornito non è valido";
                return false;
            }
            catch (Exception ex)
            {
                LoginBLL.log.Info((object)string.Format("[TFI.BLL] : LoginBLL - E' stata generata un'eccezione in data: {0}. Messaggio: {1}", (object)DateTime.Now, (object)ex.Message));
                LoginBLL.errorMessage = "Eccezione generata in fase di controllo OTP Deleghe";
                return false;
            }
        }

        public static string SelectLayout(string tipoUtente)
        {
            if (tipoUtente == "AD")
                return "PartialViewAdmin";
            if (tipoUtente == "E")
                return "PartialViewAmministrativo";
            if (tipoUtente == "A")
                return "PartialViewAzienda";
            if (tipoUtente == "C")
                return "PartialViewConsulente";
            if (tipoUtente == "I")
                return "PartialViewIscritto";
            return tipoUtente == "M" ? "" : (string)null;
        }

        public static void ModificaPWD(string VecchiaPassword, string NuovaPassword, string tipoUtente)
        {
            string str1 = HttpContext.Current.Request.Form["ConfermaPassword"];

            if (NuovaPassword != str1)
            {
                HttpContext.Current.Items[(object)"ErrorMessage"] = (object)"Le Password non coincidono";
                VecchiaPassword = "";
                NuovaPassword = "";
            }
            if (NuovaPassword == VecchiaPassword)
            {
                HttpContext.Current.Items[(object)"ErrorMessage"] = (object)"La nuova password non deve coincidere con quella vecchia";
                VecchiaPassword = "";
                NuovaPassword = "";
            }
            string strKey = "";
            string strIV = "";
            VecchiaPassword = Cypher.CryptPassword(VecchiaPassword, strKey, strIV);
            NuovaPassword = Cypher.CryptPassword(NuovaPassword, strKey, strIV);
            LoginDAL.ModificaPWD(VecchiaPassword, NuovaPassword, tipoUtente);
        }

        public static bool CiSonoDeleghePerConsulente(TFI.OCM.Utente.Utente utente)
        {
            try
            {
                return LoginDAL.CiSonoDeleghePerConsulente(utente.CodTer);
            }
            catch (Exception ex)
            {
                LoginBLL.log.Info((object)string.Format("[TFI.BLL] : LoginBLL - E' stata generata un'eccezione in data: {0}. Messaggio: {1}", (object)DateTime.Now, (object)ex.Message));
                LoginBLL.errorMessage = "Eccezione generata in fase di Login";
                return false;
            }
        }

        public static void ResetPassword(string identifier, string tipoUtente, string password)
        {
            var encryptedPassword = Cypher.CryptPassword(password, "", "");
            if (tipoUtente == "I")
                LoginDAL.ResetPasswordIscritto(identifier, encryptedPassword);
            else if (tipoUtente == "A")
                LoginDAL.ResetPasswordAziendaConsulente(identifier, encryptedPassword);
            else if(tipoUtente == "C")
                LoginDAL.ResetPasswordAziendaConsulente(LoginDAL.GetCodiceFiscaleConsulente(identifier), encryptedPassword);
            
        }

        public static void PasswordRecovery(string username, string tipoUtente)
        {
            username = username.Trim().Replace(" ", "").ToUpper();
            var email = string.Empty;

            if (tipoUtente == "I")
                email = LoginDAL.GetEmailIscritto(username);
            else if (tipoUtente == "A")
                (email, username) = LoginDAL.GetDettaglioAzienda(username);
            else if(tipoUtente == "C")
                email = LoginDAL.GetEmailConsulente(username);
            
            if (string.IsNullOrEmpty(email))
            {
                log.Info(string.Format("[TFI.BLL] : LoginBLL - E' fallito il reset della password in data: {0}. Per username: {1}. Tipo utente: {2}. Email non trovata", DateTime.Now, username, tipoUtente));
                return;
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, username),
                new("tipoUtente", tipoUtente)
            };
            var result = SmtpEmailService.SendPasswordRecoveryEmail(email, username, TokenHelper.CreateToken(claims));

            if (!result.Succeeded)
                log.Info(string.Format("[TFI.BLL] : LoginBLL - E' fallito l'invio di email per il reset della password in data: {0}. Messaggio: {1}", DateTime.Now, result.Message));
        }
    }
}
