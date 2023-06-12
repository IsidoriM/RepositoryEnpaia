// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Iscritto.FondoContoIndBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System;
using System.Collections.Generic;
using System.Web;
using TFI.BLL.Utilities;
using TFI.DAL.Iscritto;
using TFI.OCM.Iscritto;

namespace TFI.BLL.Iscritto
{
    public class FondoContoIndBLL
    {
        private Anagrafica FondoAnag;
        private FondoContoIndDAL FondoDAL = new FondoContoIndDAL();

        public bool IsDocumentUploaded(ref string ErrorMsg)
        {
            string matricolaIscritto = this.FondoDAL.GetMatricolaIscritto((HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente).Username);
            try
            {
                return this.FondoDAL.IsDocumentUploaded(matricolaIscritto);
            }
            catch (Exception ex)
            {
                ErrorMsg = "Errore in fase di controllo documenti";
                return false;
            }
        }

        public bool SalvaDatiDocumento(
          string tipoDocumento,
          string numeroDocumento,
          string scadenzaDocumento)
        {
            string matricolaIscritto = this.FondoDAL.GetMatricolaIscritto((HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente).Username);
            return this.FondoDAL.SalvaDatiDocumento(tipoDocumento, numeroDocumento, scadenzaDocumento, matricolaIscritto);
        }

        public Anagrafica GetFondoContoInd(string cf)
        {
            this.FondoAnag = new Anagrafica();
            this.FondoAnag = this.FondoDAL.GetFondoContoInd(cf);
            return this.FondoAnag;
        }

        public Anagrafica RicFondo(Anagrafica FondoAnag)
        {
            string str = HttpContext.Current.Request.Form["divPagamento"].ToString();
            if (!Utils.CheckIban(FondoAnag.Iban) && str == "1")
            {
                HttpContext.Current.Items[(object)"ErrorMessage"] = (object)"Iban Errato";
                return this.FondoAnag;
            }
            return FondoDAL.RicFondo(FondoAnag);
        }

        public List<IscrittoRicTFROCM.RichiestaLiquidazione> GetListaRichiestaLiquidazione(
          int mat)
        {
            return new FondoContoIndDAL().GetListaRichiestaLiquidazione(mat);
        }

        public bool HasFondoConto(string username)
        {
            return FondoDAL.HasFondoContoIndividuale(username);
        }
    }
}
