// Decompiled with JetBrains decompiler
// Type: TFI.BLL.AziendaConsulente.CessazioniRdlBll
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using Newtonsoft.Json;
using System;
using System.Web;
using TFI.BLL.Utilities;
using TFI.DAL.AziendaConsulente;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;
using TFI.OCM.Iscritto;
using System.Data;

namespace TFI.BLL.AziendaConsulente
{
    public class CessazioniRdlBll
    {
        public string errorMessage;
        private TFI.OCM.Utente.Utente u = HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente;
        private readonly CessazioniRdlDal cessazioni = new CessazioniRdlDal();

        public CessazioniRdl Liste(string CodPos) => new CessazioniRdl()
        {
            causas = this.cessazioni.Causales(CodPos),
            listas = this.cessazioni.CessazioniRdlGet(CodPos),
            matricolas = this.cessazioni.Matricolas(CodPos)
        };

        public bool Controllare(
          string Matricola,
          string Nome,
          string Cognome,
          string DataIscrizione,
          string DataCessazione,
          string Causale,
          string ProRap,
          string CodPos,
          TFI.OCM.Utente.Utente u,
          string iban,
          string bicSwift,
          int? giorniPreav,
          double? importoPreav,
          ref string ErroreMSG,
          ref string SuccessMSG,
          ref string WarningMSG)
        {
            try
            {
                if (!Utile.CheckIban(iban))
                {
                    ErroreMSG = "Iban Errato";
                    return false;
                }
                if (!Utile.CheckAbiAndCab(iban))
                {
                    WarningMSG = "Abi e cab non censiti.";
                }

                if (!cessazioni.Controllo(Matricola, Nome, Cognome, DataIscrizione, DataCessazione, Causale, ProRap, CodPos, u, ref ErroreMSG, ref SuccessMSG))
                    return false;
                var risultatoSalvataggio = cessazioni.CessazioneRDL(iban, Matricola, DataCessazione, Causale, ProRap, CodPos, giorniPreav, importoPreav, u, ref ErroreMSG, ref SuccessMSG);
                if (risultatoSalvataggio)
                {
                    DataRow emailCodiceFiscale = cessazioni.GetEmailCodFisIscritto(Matricola);

                    string bodyIscritto = $"<p> Salve {Nome} {Cognome}, \n La informiamo che in data {Utile.FromStringToDateTimeToFormatString(DataCessazione, "dd-MM-yyyy")} il suo rapporto lavorativo con {u.Denominazione} cessa. \n A presto, Fondazione Enpaia.</p>";
                    var result = SmtpEmailService.InviaEmailCessazioneRapporto("Registrazione Cessazione rapporto di lavoro", bodyIscritto, emailCodiceFiscale["EMAIL"].ToString(), emailCodiceFiscale["CODFIS"].ToString());

                    string bodyAzienda = $"<p> Salve {u.Denominazione}, \n La informiamo che in data {Utile.FromStringToDateTimeToFormatString(DataCessazione, "dd-MM-yyyy")} il rapporto lavorativo di  {Matricola} - {Nome} {Cognome} cessa.</p>";
                    string emailAzienda = cessazioni.GetEmailAzienda(CodPos);
                    result = SmtpEmailService.InviaEmailCessazioneRapporto("Registrazione Cessazione rapporto di lavoro", bodyAzienda, emailAzienda, CodPos);

                    string emailDelegato = cessazioni.GetEmailConsulente(CodPos);
                    if (!string.IsNullOrWhiteSpace(emailDelegato))
                        result = SmtpEmailService.InviaEmailCessazioneRapporto("Registrazione Cessazione rapporto di lavoro", bodyAzienda, emailDelegato, u.Username);
                }
                return risultatoSalvataggio;
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)u), JsonConvert.SerializeObject((object)cessazioni));
                return false;
            }
        }

        public Anagrafica GetDatiAnagrafici(string matricola, out int? outcome)
        {
            try
            {
                Anagrafica datiAnagrafici = new CessazioniRdlDal().GetDatiAnagrafici(matricola);
                if (datiAnagrafici == null)
                {
                    outcome = new int?(2);
                    return (Anagrafica)null;
                }
                outcome = new int?(0);
                return datiAnagrafici;
            }
            catch (Exception ex)
            {
                outcome = new int?(1);
                this.errorMessage = ex.Message;
                return (Anagrafica)null;
            }
        }
    }
}
