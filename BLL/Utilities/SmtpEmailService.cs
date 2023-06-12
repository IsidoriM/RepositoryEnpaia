using System;
using System.Configuration;
using System.IO;
using System.Net;
using log4net;
using MailKit.Security;
using MimeKit;
using TFI.OCM.AziendaConsulente;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace TFI.BLL.Utilities
{
    public static class SmtpEmailService
    {
        private static readonly ILog _log = LogManager.GetLogger("RollingFile");
        public static (bool Succeeded, string Message) SendPasswordRecoveryEmail(string emailTo, string username, string token)
        {
            var body = $"<p>Ciao <b>{username}</b>,\n puoi resettare la tua password cliccando su questo " +
                $"<a href=\"{ConfigurationManager.AppSettings["baseUrl"]}Login/PasswordReset?token={token}\">link</a>\n</p>" +
                $"<hr>\n <p>Se non dovesse funzionare puoi accedere a questo indirizzo dal browser: \n " +
                $"{ConfigurationManager.AppSettings["baseUrl"]}Login/PasswordReset?token={token}</p>";
            
            var subject = "Reset password enpaia";
            var to = new MailboxAddress(username, emailTo);
            var sendEmailResult = SendEmail(body, subject, to);
            //var response = sendEmailResult.Succeded ? "Email inviata con successo" :
            //    "Operazione non riuscita";

            return sendEmailResult;
        }

        public static (bool Succeded, string Message) SendRegistrazioneEmail(string emailTo, string username)
        {
            var body = $"<p>Ciao <b>{username}</b>,\n benvenuto sul portale Enpaia. Da ora potrai effettuare il login direttamente con le credenziali da te scelte al seguente  " +
               $"<a href=\"{ConfigurationManager.AppSettings["baseUrl"]}Login/Login?tipoUtente=I\">link</a>\n</p>" +
               $"<hr>\n <p>Se non dovesse funzionare puoi accedere a questo indirizzo dal browser: \n " +
               $"{ConfigurationManager.AppSettings["baseUrl"]}Login/Login?tipoUtente=I</p>";

            var subject = "Benvenuto sul portale Enpaia";
            return SendEmailAndReturnOperationResult(emailTo, username, body, subject);
        }
        
        public static (bool Succeded, string Message) InviaEmailSalvataggioAzienda(string emailTo, string partitaIva, string ragioneSociale, string token, byte[] pdf)
        {
            try
            {
                var body =
                    $"<p>Ciao <b>{ragioneSociale}</b>,\n puoi consultare e aggiungere documenti alla tua pratica di registrazione cliccando su questo " +
                    $"<a href=\"{ConfigurationManager.AppSettings["baseUrl"]}Registrazione/ConsultaPraticaRegistrazioneAzienda?token={token}\">link</a>\n</p>" +
                    $"<hr>\n <p>Se non dovesse funzionare puoi accedere a questo indirizzo dal browser: \n " +
                    $"{ConfigurationManager.AppSettings["baseUrl"]}Registrazione/ConsultaPraticaRegistrazioneAzienda?token={token}</p>";
                var subject = "Registrazione Azienda";
                var allegato = CreateAttachment(pdf, $"RicevutaPraticaRegistrazione_{partitaIva}.pdf");

                return SendEmailAndReturnOperationResult(emailTo, partitaIva, body, subject, allegato);
            }
            catch (Exception ex)
            {
                _log.Info($"Invio email non riuscito per {emailTo} in data {DateTime.Now}\n messaggio: {ex.Message}\n stacktrace {ex.StackTrace}");
                return (false,
                    $"Invio email non riuscito per {emailTo} in data {DateTime.Now}\n messaggio: {ex.Message}\n stacktrace {ex.StackTrace}");
            }
        }
        
        private static (bool Succeded, string Message) SendEmailAndReturnOperationResult(string emailTo, string username, string body, string subject, MimePart allegato = null)
        {
            var to = new MailboxAddress(username, emailTo);
            var sendEmailResult = SendEmail(body, subject, to, allegato);
            var response = sendEmailResult.Succeded ? "Email inviata con successo" : $"Invio email non riuscito! Info:\n {sendEmailResult.Message}";

            return (sendEmailResult.Succeded, response);
        }

        public static (bool Succeded, string Message) SendRisultatoOperazioneDelegheEmail(DelegheOCM.DatiDelega dati, string operazioneEffettuata)
        {
            var result1 = SendRisultatoOperazioneDelegheForAziendaEmail(dati.EmailAz, dati.RagSocAz, dati.RagSocDe, operazioneEffettuata);
            var result2 = SendRisultatoOperazioneDelegheForAziendaEmail(dati.EmailCertAz, dati.RagSocAz, dati.RagSocDe, operazioneEffettuata);
            var result3 = SendRisultatoOperazioneDelegheForConsulenteEmail(dati.EmailDe, dati.RagSocDe, dati.RagSocAz, operazioneEffettuata);
            var result4 = SendRisultatoOperazioneDelegheForConsulenteEmail(dati.EmailCertDe, dati.RagSocDe, dati.RagSocAz, operazioneEffettuata);
            
            var finalSucceeded = result1.Succeded && result2.Succeded && result3.Succeded && result4.Succeded;
            var finalMessage = $"Invio email azienda: {result1.Message}\n" +
                               $"Invio email cert azienda: {result2.Message}\n" +
                               $"Invio email delegato: {result3.Message}\n" +
                               $"Invio email cert delegato: {result4.Message}";

            return (finalSucceeded, finalMessage);

            (bool Succeded, string Message) SendRisultatoOperazioneDelegheForAziendaEmail(string emailTo, string username, string ragsoc, string operazioneEffettuata)
            {
                var body = $"<p>Ciao <b>{username}</b>,\n la richiesta delega del delegato con ragione sociale: {ragsoc} &egrave; stata {operazioneEffettuata}</p>";
                var subject = "Esito richiesta delega";
                return SendEmailAndReturnOperationResult(emailTo, username, body, subject);
            }

            (bool Succeded, string Message) SendRisultatoOperazioneDelegheForConsulenteEmail(string emailTo, string username, string ragsoc, string operazioneEffettuata)
            {
                var body = $"<p>Ciao <b>{username}</b>,\n la richiesta delega inviata all'azienda con ragione sociale: {ragsoc} &egrave; stata {operazioneEffettuata}</p>";
                var subject = "Esito richiesta delega";
                return SendEmailAndReturnOperationResult(emailTo, username, body, subject);
            }
        }


        public static (bool Succeded, string Message) InviaEmailCessazioneRapporto(string subject, string body, string emailTo, string username)
        {
            return SendEmailAndReturnOperationResult(emailTo, username, body, subject);
        }

        public static (bool Succeded, string Message) InviaEmailRDL(string subject, string body, string emailTo, string username)
        {
            return SendEmailAndReturnOperationResult(emailTo, username, body, subject);
        }
        
        public static (bool Succeded, string Message) InviaEmailConfermaDipa(string subject, string body, string emailTo, string username)
        {
            return SendEmailAndReturnOperationResult(emailTo, username, body, subject);
        }

        public static (bool Succeded, string Message) InviaMailRichiestaDelegaAzienda(string emailTo, string username, string consulente, string token)
        {
            var body = $"<p>Salve <b>{username}</b>,\n hai ricevuto una richiesta di delega da parte di {consulente}. Per poterla visionare e gestire," +
                       $" puoi effettuare l'accesso sul portale e visitare la sezione deleghe oppure cliccare sul seguente  " +
                       $"<a href=\"{ConfigurationManager.AppSettings["baseUrl"]}AziendaConsulente/DettaglioDelega?token={token}&codTer={consulente}\">link</a>\n</p>" +
                       $"<hr>\n <p>Se non dovesse funzionare puoi accedere a questo indirizzo dal browser: \n " +
                       $"{ConfigurationManager.AppSettings["baseUrl"]}AziendaConsulente/DettaglioDelega?token={token}&codTer={consulente}</p>";
            
            var subject = "Richiesta Delega";
            var result = SendEmailAndReturnOperationResult(emailTo, username, body, subject);
            if (!result.Succeded)
                _log.Info($"[InvioMail] Richiesta Delega - Invio Mail Azienda non riuscita. Data: {DateTime.Now}. Messaggio: {result.Message}");
            return result;
        }
        public static (bool Succeded, string Message) InviaMailRichiestaDelegaConsulente(string emailTo, string username, string azienda)
        {
            var subject = "Richiesta Delega";
            var body = $"<p>Salve <b>{username}</b>, la tua richiesta di delega per {azienda} risulta correttamente inviata.</p>";
            var result = SendEmailAndReturnOperationResult(emailTo, username, body, subject);
            if (!result.Succeded)
                _log.Info($"[InvioMail] Richiesta Delega - Invio Mail Consulente non riuscita. Data: {DateTime.Now}. Messaggio: {result.Message}");
            return result;
        }
        
        public static (bool Succeded, string Message) InviaRegistrazioneConsulente(string emailTo, string username, string password, string identificativo)
        {
            var body = EmailTemplates.GetRegistrazioneConsulenteTemplate(username,password, identificativo);
            var subject = "Benvenuto sul portale Enpaia";
            return SendEmailAndReturnOperationResult(emailTo, username, body, subject);
        }

        private static MimePart CreateAttachment(byte[] file, string fileName)
        {
            var attach = new MimePart(MimeTypes.GetMimeType(fileName))
            {
                Content = new MimeContent(new MemoryStream(file)),
                ContentDisposition = new ContentDisposition (ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = fileName
            };

            return attach;
        }
        
        private static (bool Succeded, string Message) SendEmail(string body, string subject, MailboxAddress to, MimePart allegato = null)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(ConfigurationManager.AppSettings["email_user"],
                ConfigurationManager.AppSettings["email_usermail"]));
            email.To.Add(to);
            email.Subject = subject;
            var builder = new BodyBuilder
            {
                HtmlBody = body
            };

            if(allegato != null) builder.Attachments.Add(allegato);
            
            email.Body = builder.ToMessageBody();
            
            try
            {
                using (var smtp = new SmtpClient())
                {
                    smtp.Connect(ConfigurationManager.AppSettings["email_host"], 
                        Convert.ToInt32(ConfigurationManager.AppSettings["email_port"]), 
                        SecureSocketOptions.StartTls);
                    smtp.Authenticate(new NetworkCredential(ConfigurationManager.AppSettings["email_username"],
                        ConfigurationManager.AppSettings["email_password"]));
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }

                return (true, $"Invio email riuscito in data {DateTime.Now}");
            }
            catch (Exception ex)
            {
                _log.Info($"Invio email non riuscito per {subject} in data {DateTime.Now}\n messaggio: {ex.Message}\n stacktrace {ex.StackTrace}");
                return (false, $"Invio email non riuscito per {subject} in data {DateTime.Now}\n messaggio: {ex.Message}\n stacktrace {ex.StackTrace}");
            }
        }
    }
}
