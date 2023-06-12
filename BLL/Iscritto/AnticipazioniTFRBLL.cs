// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Iscritto.AnticipazioniTFRBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using DAL.Iscritto;
using System;
using System.Collections.Generic;
using System.Web;
using TFI.BLL.Utilities;
using TFI.DAL.Login;
using TFI.OCM.Iscritto;
using TFI.OCM.Utente;

namespace TFI.BLL.Iscritto
{
    public class AnticipazioniTFRBLL
    {
        private readonly AnticipazioniTFRDAL tfrDAL = new AnticipazioniTFRDAL();

        public void CaricamentoDati(ref string ErroreMSG, ref string SuccessMSG, ref string InfoMSG) => this.tfrDAL.ATFR(ref ErroreMSG, ref SuccessMSG, ref InfoMSG);

        public bool CheckAnticipoTFR(Utente utente) {
            var matricola = LoginDAL.GetMatricola(utente);
            return this.tfrDAL.CheckAnticipoTFR(matricola); 
        }

        public void InvioDati(Anagrafica a, ref string ErroreMSG, ref string SuccessMSG, ref string WarningMSG)
        {
            if (!Utils.CheckIban(a.Iban))
            {
                ErroreMSG = "Iban Errato";
                return;
            }

            if (!Utils.CheckAbiAndCab(a.Iban))
                WarningMSG = "Abi e Cab non riconosciuti";

            this.tfrDAL.ATFR(a, ref ErroreMSG, ref SuccessMSG);
        }

        public bool SalvaDatiDocumento(
          string tipoDocumento,
          string numeroDocumento,
          string scadenzaDocumento)
        {
            string matricolaIscritto = this.tfrDAL.GetMatricolaIscritto((HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente).Username);
            return this.tfrDAL.SalvaDatiDocumento(tipoDocumento, numeroDocumento, scadenzaDocumento, matricolaIscritto);
        }

        public List<string> GetTipoDocList() => tfrDAL.GetTipoDocumentoList();
       


        public bool IsDocumentUploaded(ref string ErrorMsg)
        {
            string matricolaIscritto = this.tfrDAL.GetMatricolaIscritto((HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente).Username);
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
    }
}
