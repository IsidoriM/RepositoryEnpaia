// Decompiled with JetBrains decompiler
// Type: TFI.BLL.AziendaConsulente.AziDelegheBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using TFI.DAL.AziendaConsulente;
using TFI.DAL.Login;
using TFI.OCM.AziendaConsulente;
using Utilities;
using static TFI.BLL.Utilities.SmtpEmailService;

namespace TFI.BLL.AziendaConsulente
{
    public class AziDelegheBLL
    {
        private readonly DelegheDAL delegheDal = new DelegheDAL();

        public DelegheOCM AssNaz() => this.delegheDal.AssNaz();

        public DelegheOCM GetCodNazAssTerAssNaz(string codTer)
        {
            return delegheDal.GetCodNazAssTerAssNaz(codTer);
        }

        public DelegheOCM SearchDeleghe(
          string PosAz,
          ref string ErroreMSG,
          ref string SuccessMSG)
        {
            var tmp = this.delegheDal.SearcheDeleghe(PosAz, ref ErroreMSG, ref SuccessMSG);
            return tmp;
        }

        public bool salvdelegheBll(
          ref string MSGErrore,
          ref string MSGSuccess,
          string strModo,
          string codTer,
          string codpos,
          TFI.OCM.Utente.Utente u,
          DelegheOCM.DatiDelega Deleghe)
        {
            bool flag = false;
            if (this.delegheDal.CheckInput(ref MSGErrore, strModo, codTer, codpos, Deleghe))
                flag = this.delegheDal.btnSalva_Click(u, ref MSGErrore, ref MSGSuccess, Deleghe);
            if (flag)
            {
                string codUteDe = Deleghe.CodUteDe;
                if (!LoginDAL.CiSonoDeleghePerConsulente(codUteDe))
                {
                    string otp = this.delegheDal.GeneraOTPDeleghe();
                    this.delegheDal.SalvaOTPDeleghe(codUteDe, otp);
                    Deleghe.OtpDeleghe = otp;
                }
            }
            return flag;
        }

        public (bool Succeded, string Message) InviaEmailPerCambioStatoDelega(string codter, string codpos, string operazione)
        {
            var datiDelega = delegheDal.GetInfoForDelega(codter, codpos);
            return SendRisultatoOperazioneDelegheEmail(datiDelega, operazione);
        }

        public bool EliminaDelega(
          string codter,
          string codpos,
          string u)
        {
            return delegheDal.EliminaDeleghe(codter, codpos, u);
        }

        public bool AttivaDelega(
          string codter,
          string codpos,
          string u)
        {
            return delegheDal.AttivaDeleghe(codter, codpos, u);
        }

        public DelegheOCM GetCaricaIva(ref string MSGErrore, string codFis, string ParIva) => this.delegheDal.CARICA_IVA(ref MSGErrore, codFis, ParIva);

        public DelegheOCM GetDatiAzienda(
          ref string MSGErrore,
          string posizione,
          string strModo)
        {
            return this.delegheDal.carica_Dati_Azienda(ref MSGErrore, posizione, strModo);
        }

        public DelegheOCM GetDenominazione(string codNaz, ref string MSGErrore) => this.delegheDal.caricaDenominazione(codNaz, ref MSGErrore);

        public DelegheOCM GetAssociazione(
          DelegheOCM delegheOCM,
          string codTer,
          string codNaz,
          string codpos,
          string assnaz,
          string asster)
        {
            return this.delegheDal.carica_Dati_Associazione(delegheOCM, codTer, codNaz, codpos, assnaz, asster);
        }

        public DelegheOCM GetDettaglioDelega(string codTer, string codPos)
        {
            string ErroreMSG = "";
            DelegheOCM delegaToShow = new DelegheOCM();

            delegaToShow = delegheDal.GetCodNazAssTerAssNaz(codTer);
            string codNaz = delegaToShow.datidelega.codNaz;
            string assNaz = delegaToShow.datidelega.AssStudio;
            string assTer = delegaToShow.datidelega.RagSocDe;

            delegaToShow = delegheDal.carica_Dati_Azienda(ref ErroreMSG, codPos, "VISUALIZZA");
            delegaToShow = delegheDal.carica_Dati_Associazione(delegaToShow, codTer, codNaz, codPos, assNaz, assTer);
            delegaToShow.datidelega.PosizioneAz = codPos;
            delegaToShow.datidelega.codTer = codTer;
            delegaToShow.datidelega.datini = delegheDal.GetDatiniDelega(codTer, codPos);

            delegheDal.GetFlagAttDelega(codTer, codPos, delegaToShow.datidelega.datini, delegaToShow);
            delegaToShow.datidelega.datini = delegaToShow.datidelega.datini.ToDomainDateFormat().ToString("dd/MM/yyyy");
            return delegaToShow;
        }

        public bool HaDelegaAttiva(string codpos)
        {
            return delegheDal.HaDelgaAttiva(codpos);
        }
    }
}
