// Decompiled with JetBrains decompiler
// Type: TFI.BLL.AziendaConsulente.RapportiLavoroBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using Newtonsoft.Json;
using OCM.TFI.OCM.AziendaConsulente;
using System;
using System.Web;
using log4net;
using TFI.BLL.Utilities;
using TFI.DAL.AziendaConsulente;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;
using TFI.OCM.Utente;

namespace TFI.BLL.AziendaConsulente
{
    public class RapportiLavoroBLL
    {
        private RapportiLavoro raplav = new RapportiLavoro();
        private RapportiLavoroDAL rapLavDal = new RapportiLavoroDAL();
        private TFI.OCM.Utente.Utente u = HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente;
        private static readonly ILog _log = LogManager.GetLogger("RollingFile");


    public RapportiLavoro NuvoRapportiLavoroBLL(string codpos)
    {
      try
      {
        this.raplav = this.rapLavDal.GetNuvoRapportiLavoro(codpos);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) this.raplav));
      }
      return this.raplav;
    }

        public RapportiLavoro SchedaIscrittoBLL(
          string matricola,
          string prorap,
          string codpos)
        {
            try
            {
                this.raplav = this.rapLavDal.GetSchedaIscritto(matricola, prorap, codpos);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
            return this.raplav;
        }

        public RapportiLavoro SospensioniBLL(
          string matricola,
          string nome,
          string cognome,
          string prorap,
          string codpos,
          ref string messaggio,
          ref string SuccessMSG)
        {
            try
            {
                this.raplav = this.rapLavDal.GetSospensioni(matricola, nome, cognome, prorap, codpos, ref messaggio, ref SuccessMSG);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
            return this.raplav;
        }

        public RDLPaginatiModel RapportiBLLFiltrato(string codpos, int? page = 1, int? pageSize = 10, string filtro = "",
            TipoRDL tipoRdl = TipoRDL.RDLAttivo, OrderByType orderBy = OrderByType.MatricolaDataIscr, bool orderDesc = false)
        {
            RDLPaginatiModel rdl = new RDLPaginatiModel();
            try
            {
                rdl = rapLavDal.GetRapportiFiltrato(codpos, filtro, tipoRdl, page, pageSize, orderBy, orderDesc);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
            return rdl;
        }

    public bool SalvaSospensioniBLL(
      string dataDa,
      string dataAl,
      string codsos,
      string matricola,
      string prorap,
      string codpos,
      TFI.OCM.Utente.Utente u,
      string prosos,
      ref string messaggio,
      ref string SuccessMSG)
    {
      bool flag = false;
      try
      {
        return this.rapLavDal.GetSalvaSospensioni(dataDa, dataAl, codsos, matricola, prorap, codpos, u, prosos, ref messaggio, ref SuccessMSG);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) u), JsonConvert.SerializeObject((object) this.raplav));
        return flag;
      }
    }

        public bool EliminaSospensioniBLL(
          string dataDa,
          string dataAl,
          string matricola,
          string prorap,
          string prosos,
          string codpos,
          TFI.OCM.Utente.Utente u,
          ref string messaggio,
          ref string SuccessMSG)
        {
            bool flag = false;
            try
            {
                return this.rapLavDal.GetEliminaSospensioni(dataDa, dataAl, matricola, prorap, prosos, codpos, u, ref messaggio, ref SuccessMSG);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)u), JsonConvert.SerializeObject((object)this.raplav));
                return flag;
            }
        }

        public RapportiLavoro CarrieraRapportiLavoroBLL(
          string matricola,
          string prorap,
          string codpos)
        {
            try
            {
                this.raplav = this.rapLavDal.GetCarrieraRapportiLavoro(matricola, prorap, codpos);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
            return this.raplav;
        }

        public bool ModRapportiLavoroBLL(
          NuovoIscritto nuovoIscritto,
          int codpos,
          TFI.OCM.Utente.Utente u,
          int intProRap,
          ref string messaggio,
          ref string SuccessMSG)
        {
            bool flag = false;
            try
            {
                bool isNewIscritto = string.IsNullOrWhiteSpace(nuovoIscritto.matricola) || nuovoIscritto.matricola == "0";
                bool isNewRdl = intProRap == 0;

                var (matricola, result) = this.rapLavDal.WriteData(u, codpos, intProRap, nuovoIscritto, ref messaggio, ref SuccessMSG);

                if (result)
                    SendEmail(isNewIscritto, isNewRdl, nuovoIscritto, u);

                return result;
            }
            catch (Exception ex)
            {
                _log.Info($"[BLL - RapportiLavoroBLL - ModRapportiLAvoro] - Eccezione generata: {ex.Message} - stackTrace: {ex.StackTrace}");
                return flag;
            }
        }

        private void SendEmail(bool isNewIscritto, bool isNewRdl, NuovoIscritto nuovoIscritto, Utente u)
        {
            string emailAzienda = rapLavDal.GetEmailAzienda(u.CodPosizione);

            string bodyIscritto = $"Salve {nuovoIscritto.nome} {nuovoIscritto.cognome},\n";
            string bodyAzienda = "";
            if (isNewIscritto)
            {
                //Nuovo iscritto, inviare la matricola 
                bodyIscritto += $"sei stato censito nel portale Enpaia. Da oggi potrai effettuare la registrazione a questo link, " +
                    $"inserendo la seguente matricola {nuovoIscritto.matricola} ed il tuo codice fiscale {nuovoIscritto.codFis}.\n";
                bodyAzienda += $"hai censito {nuovoIscritto.nome} {nuovoIscritto.cognome} - {nuovoIscritto.codFis} - {nuovoIscritto.matricola} all'interno del portale Enpaia. \n";
            }
            if (isNewRdl)
            {
                //Nuovo rapporto di lavoro
                bodyIscritto += $"Ti informiamo inoltre che l'azienda {u.Denominazione} ha registrato un nuovo rapporto di lavoro a tuo nome.";
                bodyAzienda += "Ti informiamo inoltre che hai correttamente inserito un nuovo contratto di lavoro a nome dell'iscritto";
            }
            else
            {
                //Modifica rapporto di lavoro
                bodyIscritto += $"Ti informiamo inoltre che l'azienda {u.Denominazione} ha modificato un nuovo rapporto di lavoro a nome dell'iscritto.";
                bodyAzienda += "Ti informiamo inoltre che hai correttamente modificato il rapporto di lavoro dell'iscritto.";
            }

            var resultInvioIscritto = SmtpEmailService.InviaEmailRDL("Registrazione Rapporto di Lavoro", bodyIscritto, nuovoIscritto.email, nuovoIscritto.codFis);
            var resultInvioAzienda = SmtpEmailService.InviaEmailRDL("Registrazione Rapporto di Lavoro", bodyAzienda, emailAzienda, u.CodPosizione);
        }

        public RapportiLavoro DettagliCarrieraBLL(
          string matricola,
          string uteagg,
          string datini,
          string prorap,
          string codpos)
        {
            try
            {
                this.raplav = this.rapLavDal.GetDettagliCarriera(matricola, uteagg, datini, prorap, codpos);

            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
            return this.raplav;
        }

        public RapportiLavoro GetdllContratto(string datIni)
        {
            try
            {
                this.raplav = this.rapLavDal.CaricaContratti(string.Empty);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
            return this.raplav;
        }

        public RapportiLavoro GetDatiMatricola(string codfis)
        {
            try
            {
                this.raplav = this.rapLavDal.CaricaDatiMatricola(codfis);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
            return this.raplav;
        }

        public RapportiLavoro GetdllLivello(string codcon, string dencon)
        {
            try
            {
                this.raplav = this.rapLavDal.CaricaLivello(codcon, dencon);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
            return this.raplav;
        }

        public RapportiLavoro GetdllAliquota(
          string dataNas,
          string codcon,
          string dencon,
          string codpos)
        {
            try
            {
                this.raplav = this.rapLavDal.GetCorrectAliquota(dataNas, codcon, dencon, codpos);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
            return this.raplav;
        }

        public RapportiLavoro GetValAliquota(
          string CODGRUASS,
          string dencon,
          string datIsc,
          string datMod,
          string messaggio,
          string dataNas,
          bool fap)
        {
            try
            {
                this.raplav = this.rapLavDal.GetPercentuali(CODGRUASS, dencon, datIsc, datMod, ref messaggio, dataNas, fap);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
            return this.raplav;
        }

        public RapportiLavoro GetSelLivello(
          string numScatti,
          string dencon,
          string codliv,
          string datMod,
          string datIsc,
          string prorap,
          string tiprap)
        {
            try
            {
                this.raplav = this.rapLavDal.AggiornaScatti(numScatti, dencon, codliv, datMod, datIsc, prorap, tiprap);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
            return this.raplav;
        }

        public RapportiLavoro GetEmolumenti(string codliv, string dencon, string datMod)
        {
            try
            {
                this.raplav = this.rapLavDal.AggiornaEmolumenti(codliv, dencon, datMod);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
            return this.raplav;
        }

        public RapportiLavoro GetMensilita(string dencon)
        {
            try
            {
                this.raplav = this.rapLavDal.GetMensilitames(dencon);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
            return this.raplav;
        }

        public void StampaBLL(string prot, ref string messaggio)
        {
            try
            {
                this.rapLavDal.btnStampa(prot, ref messaggio);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)this.raplav));
            }
        }

        public bool CheckUnioneSospensioniIntersecanti(string prosos, string prorap, string matricola, string dataDa, string dataAl, string codpos, string codsos)
        {
            return rapLavDal.CheckStessaSospensione(prosos, prorap, matricola, dataDa, dataAl, codpos, codsos);
        }        
    }
}
