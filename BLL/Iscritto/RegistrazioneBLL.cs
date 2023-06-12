using OCM.TFI.OCM.Iscritto;
using System;
using System.Collections.Generic;
using System.Web;
using TFI.BLL.Utilities;
using TFI.DAL.Iscritto;
using TFI.DAL.Utilities;
using TFI.OCM.Iscritto;

namespace TFI.BLL.Iscritto
{
    public class RegistrazioneBLL
    {
        private readonly RegistrazioneDAL registrazioneDAL = new RegistrazioneDAL();
        private readonly AnagraficaDAL anagraficaDAL = new AnagraficaDAL();

        public bool CheckSignUp(string matricola,
          string cf,
          ref string ErrorMsg)
        {
            return registrazioneDAL.CheckSignUp(matricola, cf, ref ErrorMsg);
        }

        public List<string> GetTipoDocList() => registrazioneDAL.GetTipoDocumentoList();
        public bool ConsolidaRegistrazione(AnagraficaConPwd anagrafica, ref string errorMSG, ref string successMSG)
        {
            if (!anagrafica.Privacy)
            {
                errorMSG = "Il consenso della privacy è obbligatorio.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(anagrafica.Password)
                || (anagrafica.Password ?? "") != (anagrafica.ConfermaPassword ?? ""))
            {
                errorMSG = "Password non valida o non coincidente";
                return false;
            }

            if (anagrafica.EmailCert is not null)
            {
                if (anagrafica.EmailCert.Contains("@pec") == false && (anagrafica.EmailCert.Contains("@legalmail") == false))
                {
                    errorMSG = "Errore Pec non valida";
                    return false;
                }
            }

            if (anagrafica.StatoEsteroResidenza == "0")
            {
                anagrafica.StatoEsteroResidenza = null;
            }

            anagrafica.CodTitstu = SetTitoloStudio(anagrafica.TitoloStudio);

            var oldAnagrafica = anagraficaDAL.GetAnagrafica(anagrafica.CodiceFiscale);
            if (AnagraficaIsEdited(oldAnagrafica, anagrafica))
            {
                errorMSG = "Non è consentito modificare dati anagrafici";
                return false;
            }
            if (DataIsEdited(oldAnagrafica, anagrafica))
            {
                if(registrazioneDAL.ConsolidaRegistrazioneModificaDati(anagrafica, ref errorMSG, ref successMSG))
                {
                    var result = SmtpEmailService.SendRegistrazioneEmail(anagrafica.Email, $"{anagrafica.Nome} {anagrafica.Cognome}");
                    return result.Succeded;
                }
            }
            if (registrazioneDAL.ConsolidaRegistrazione(anagrafica, ref errorMSG, ref successMSG))
            {
                var result = SmtpEmailService.SendRegistrazioneEmail(anagrafica.Email, $"{anagrafica.Nome} {anagrafica.Cognome}");
                return result.Succeded;
            }
            return false;
        }

        private decimal SetTitoloStudio(string titoloStudio)
        {
            var num = 1M;
            if (!(titoloStudio == "LICENZA ELEMENTARE"))
            {
                if (!(titoloStudio == "LICENZA MEDIA"))
                {
                    if (!(titoloStudio == "LAUREA"))
                    {
                        if (titoloStudio == "DIPLOMA")
                            num = 4M;
                    }
                    else
                        num = 3M;
                }
                else
                    num = 2M;
            }
            else
                num = 1M;
            return num;
        }

        private bool AnagraficaIsEdited (Anagrafica oldAnagrafica, AnagraficaConPwd anagrafica)
        {
            return oldAnagrafica.Nome != anagrafica.Nome
                    || oldAnagrafica.Cognome != anagrafica.Cognome
                    || oldAnagrafica.CodiceFiscale != anagrafica.CodiceFiscale
                    || oldAnagrafica.Sesso != anagrafica.Sesso
                    || oldAnagrafica.DataNascita != anagrafica.DataNascita
                    || ((!string.IsNullOrWhiteSpace(oldAnagrafica.ComuneNascita) && !string.IsNullOrWhiteSpace(anagrafica.ComuneNascita))
                            && oldAnagrafica.ComuneNascita != anagrafica.ComuneNascita)
                    || ((!string.IsNullOrWhiteSpace(oldAnagrafica.SigproNascita) && !string.IsNullOrWhiteSpace(anagrafica.SigproNascita))
                            && oldAnagrafica.SigproNascita != anagrafica.SigproNascita)
                    || ((!string.IsNullOrWhiteSpace(oldAnagrafica.StatoEsteroNascita) && !string.IsNullOrWhiteSpace(anagrafica.StatoEsteroNascita))
                            && oldAnagrafica.StatoEsteroNascita != anagrafica.StatoEsteroNascita);
        }

        private bool DataIsEdited(Anagrafica oldAnagrafica, AnagraficaConPwd anagrafica)
        {
            return  oldAnagrafica.TitoloStudio != anagrafica.TitoloStudio
                    || oldAnagrafica.Localita != anagrafica.Localita
                    || oldAnagrafica.ComuneResidenza != anagrafica.ComuneResidenza
                    || ((!string.IsNullOrWhiteSpace(oldAnagrafica.StatoEsteroResidenza) && !string.IsNullOrWhiteSpace(anagrafica.StatoEsteroResidenza))
                            && oldAnagrafica.StatoEsteroResidenza != anagrafica.StatoEsteroResidenza)
                    || oldAnagrafica.Indirizzo != anagrafica.Indirizzo
                    || oldAnagrafica.NumeroCivico != anagrafica.NumeroCivico
                    || oldAnagrafica.Cap != anagrafica.Cap
                    || oldAnagrafica.Cellulare != anagrafica.Cellulare
                    || oldAnagrafica.Email != anagrafica.Email
                    || ((!string.IsNullOrWhiteSpace(oldAnagrafica.EmailCert) && !string.IsNullOrWhiteSpace(anagrafica.EmailCert)) 
                            && oldAnagrafica.EmailCert != anagrafica.EmailCert);
        }
    }
}
