// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Iscritto.RicPagTFR_BLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System;
using System.Web;
using TFI.BLL.Utilities;
using TFI.DAL.Iscritto;
using TFI.DAL.Login;
using TFI.OCM.Iscritto;
using TFI.OCM.Utente;

namespace TFI.BLL.Iscritto
{
    public class RicPagTFR_BLL
    {
        private readonly RicPagTFR_DAL tfrDAL = new RicPagTFR_DAL();

        public bool SalvaDatiDocumento(
            string tipoDocumento,
            string numeroDocumento,
            string scadenzaDocumento)
        {
            string matricolaIscritto =
                this.tfrDAL.GetMatricolaIscritto((HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)
                    .Username);
            return this.tfrDAL.SalvaDatiDocumento(tipoDocumento, numeroDocumento, scadenzaDocumento, matricolaIscritto);
        }

        public bool IsDocumentUploaded(ref string ErrorMsg)
        {
            string matricolaIscritto =
                this.tfrDAL.GetMatricolaIscritto((HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)
                    .Username);
            try
            {
                return this.tfrDAL.IsDocumentUploaded(matricolaIscritto);
            }
            catch (Exception ex)
            {
                ErrorMsg = "Errore in fase di controllo documenti";
                return false;
            }
        }

        public IscrittoRicTFROCM CaricaDatiTFR(
            ref string ErrorMsg,
            ref string SuccesMsg,
            ref string InfoMSG)
        {
            return this.tfrDAL.CaricaDatiTFR(ref ErrorMsg, ref SuccesMsg, ref InfoMSG);
        }

        public bool CheckTfr(Utente utente)
        {
            var matricola = LoginDAL.GetMatricola(utente);
            return this.tfrDAL.CheckTfr(matricola);
        }

        public void InvioRichiestaTFR(IscrittoRicTFROCM modelloForm, ref string ErrorMsg, ref string SuccesMsg)
        {
            tfrDAL.InvioRichiestaTFR(modelloForm, ref ErrorMsg, ref SuccesMsg);
        }
    }
}
