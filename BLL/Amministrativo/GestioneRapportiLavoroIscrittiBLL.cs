// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.GestioneRapportiLavoroIscrittiBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using TFI.DAL.Amministrativo;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Amministrativo
{
    public class GestioneRapportiLavoroIscrittiBLL
    {
        private readonly GestioneRapportiLavoroIscrittiDAL rapportiLavoro = new GestioneRapportiLavoroIscrittiDAL();

        public GestioneRapportiLavoroIscrittiOCM RicercaIscrittiBLL(
          GestioneRapportiLavoroIscrittiOCM.Iscritti isc,
          ref string ErroreMSG,
          ref string SuccessMSG)
        {
            return this.rapportiLavoro.RicercaIscrittiDAL(isc, ref ErroreMSG, ref SuccessMSG);
        }

        public GestioneRapportiLavoroIscrittiOCM DettaglioModificaIscrittiBLL(
          string mat)
        {
            return this.rapportiLavoro.DettaglioModificaIscrittiDAL(mat);
        }

        public bool SalvaModifica(
          GestioneRapportiLavoroIscrittiOCM.Iscritti rdl,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG)
        {
            return this.rapportiLavoro.BtnSalvaMod_Click(rdl, u, ref ErroreMSG, ref SuccessMSG);
        }

        public GestioneRapportiLavoroIscrittiOCM GetMatricolaBLL(
          string mat,
          string nom,
          string cog,
          string codfis)
        {
            return this.rapportiLavoro.GetMatricolaDAL(mat, nom, cog, codfis);
        }

        public GestioneRapportiLavoroIscrittiOCM GetDatiAzienda(
          GestioneRapportiLavoroIscrittiOCM gestrdl,
          string codposAz,
          ref string ErroreMSG)
        {
            return this.rapportiLavoro.Carica_AziendaSomministrazione(gestrdl, codposAz, ref ErroreMSG);
        }

        public GestioneRapportiLavoroIscrittiOCM GetDatiAziendaConPiva(
            GestioneRapportiLavoroIscrittiOCM gestrdl,
            string piva,
            ref string ErroreMSG)
        {
            return this.rapportiLavoro.Carica_AziendaSomministrazioneConPiva(gestrdl, piva, ref ErroreMSG);
        }

        public GestioneRapportiLavoroIscrittiOCM VariazioniRDLBLL(
        string ErroreMSG,
        GestioneRapportiLavoroIscrittiOCM rdl,
        TFI.OCM.Utente.Utente u,
        string mat,
        string datNas)
        {
            return this.rapportiLavoro.LoadVariazione(ErroreMSG, rdl, u, mat, datNas);
        }

        public void VariazioniBLL(
          GestioneRapportiLavoroIscrittiOCM rdl,
          TFI.OCM.Utente.Utente u,
          ref string MsgErrore,
          ref string SuccessMSG)
        {
            this.rapportiLavoro.SalvaVariazioni(rdl, u, ref MsgErrore, ref SuccessMSG);
        }

        public GestioneRapportiLavoroIscrittiOCM CaricaInserimentoBLL(
          GestioneRapportiLavoroIscrittiOCM gestrdl,
          string codpos,
          ref string ErroreMSG,
          TFI.OCM.Utente.Utente u)
        {
            return this.rapportiLavoro.CaricaInserimentoDAL(gestrdl, codpos, ref ErroreMSG, u);
        }

        public GestioneRapportiLavoroIscrittiOCM DdlContrattoBLL(
          GestioneRapportiLavoroIscrittiOCM gestrdl,
          string datIni,
          string strAzione)
        {
            this.rapportiLavoro.Module_Carica_Contratti(gestrdl, datIni, strAzione);
            return gestrdl;
        }

        public GestioneRapportiLavoroIscrittiOCM GetdllLivello(
          GestioneRapportiLavoroIscrittiOCM gestrdl,
          string codcon,
          string dencon)
        {
            this.rapportiLavoro.Carica_Livelli(gestrdl, codcon, dencon);
            return gestrdl;
        }

        public GestioneRapportiLavoroIscrittiOCM MensilitaBLL(
          GestioneRapportiLavoroIscrittiOCM gestrdl,
          string dencon)
        {
            this.rapportiLavoro.Carica_Mensilità(gestrdl, dencon);
            return gestrdl;
        }

        public GestioneRapportiLavoroIscrittiOCM GetdllAliquota(
          GestioneRapportiLavoroIscrittiOCM gestrdl,
          string datnas,
          string codcon,
          string dencon,
          string codpos)
        {
            this.rapportiLavoro.Carica_Aliquote(gestrdl, datnas, codcon, dencon, codpos);
            return gestrdl;
        }

        public GestioneRapportiLavoroIscrittiOCM GetdllAliquotaSomma(
          GestioneRapportiLavoroIscrittiOCM gestrdl,
          int CODQUACON,
          int CODGRUASS,
          string STRDATA,
          string FAP)
        {
            this.rapportiLavoro.Module_GetListaAliquoteContributive(gestrdl, CODQUACON, CODGRUASS, STRDATA, FAP);
            return gestrdl;
        }

        public void btn_Salva(
          TFI.OCM.Utente.Utente u,
          GestioneRapportiLavoroIscrittiOCM rdl,
          string codpos,
          ref string ErroreMSG,
          ref string SuccessMSG)
        {
            this.rapportiLavoro.btnSalva_Click(u, rdl, rdl.aziendaUtilizzatrice.codposAz, ref ErroreMSG, ref SuccessMSG);
        }

        public void ModIns_Erede(
          GestioneRapportiLavoroIscrittiOCM.Eredi erediOCM,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            this.rapportiLavoro.ModInsEredi(erediOCM, u, ref ErroreMSG, ref SuccessMSG, mat);
        }

        public void ModIns_Blocchi(
          GestioneRapportiLavoroIscrittiOCM.BlocchiIscritto blocchiOCM,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            this.rapportiLavoro.ModInsBlocchi(blocchiOCM, u, ref ErroreMSG, ref SuccessMSG, mat);
        }

        public void ModIns_Bancari(
          GestioneRapportiLavoroIscrittiOCM.DatiBancariIscritto bancariOCM,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            this.rapportiLavoro.ModInsBnacari(bancariOCM, u, ref ErroreMSG, ref SuccessMSG, mat);
        }

        public void ModIns_Note(
          GestioneRapportiLavoroIscrittiOCM.NoteIscritto noteOCM,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            this.rapportiLavoro.ModInsNote(noteOCM, u, ref ErroreMSG, ref SuccessMSG, mat);
        }

        public void ModIns_Debiti(
          GestioneRapportiLavoroIscrittiOCM.DebitiIscritto debitiOCM,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            this.rapportiLavoro.ModInsDebito(debitiOCM, u, ref ErroreMSG, ref SuccessMSG, mat);
        }

        public void Elimina_Erede(
          string proereE,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            this.rapportiLavoro.mnuEliminaErede_Click(proereE, u, ref ErroreMSG, ref SuccessMSG, mat);
        }

        public void Elimina_Blocchi(
          string bloccoID,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            this.rapportiLavoro.mnuEliminaBlocchi_Click(bloccoID, u, ref ErroreMSG, ref SuccessMSG, mat);
        }

        public void Elimina_Bancari(
          string proiscban,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            this.rapportiLavoro.mnuEliminaDatBanc_Click(proiscban, u, ref ErroreMSG, ref SuccessMSG, mat);
        }

        public void Elimina_Note(
          string NoteID,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            this.rapportiLavoro.mnuEliminaNote_Click(NoteID, u, ref ErroreMSG, ref SuccessMSG, mat);
        }

        public void Elimina_Debito(
          string codpos,
          string prorap,
          string protratt,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            this.rapportiLavoro.mnuEliminaDebito_Click(codpos, prorap, protratt, u, ref ErroreMSG, ref SuccessMSG, mat);
        }

        public GestioneRapportiLavoroIscrittiOCM GetValAliquota(
          GestioneRapportiLavoroIscrittiOCM gestrdl,
          string CODGRUASS,
          string dencon,
          string datIsc,
          string datMod,
          string messaggio,
          string dataNas,
          bool fap)
        {
            this.rapportiLavoro.GetPercentuali(gestrdl, CODGRUASS, dencon, datIsc, datMod, ref messaggio, dataNas, fap);
            return gestrdl;
        }

        public GestioneRapportiLavoroIscrittiOCM GetSelLivello(
          GestioneRapportiLavoroIscrittiOCM gestrdl,
          string numScatti,
          string dencon,
          string codliv,
          string datMod,
          string datIsc,
          string prorap,
          string tiprap,
          string perPar)
        {
            this.rapportiLavoro.AggiornaScatti(gestrdl, numScatti, dencon, codliv, datMod, datIsc, prorap, tiprap, perPar);
            return gestrdl;
        }

        public GestioneRapportiLavoroIscrittiOCM GetEmolumenti(
          GestioneRapportiLavoroIscrittiOCM gestrdl,
          string codliv,
          string dencon,
          string datMod)
        {
            this.rapportiLavoro.AggiornaEmolumenti(gestrdl, codliv, dencon, datMod);
            return gestrdl;
        }

        public GestioneRapportiLavoroIscrittiOCM GetCercaBanca(
          GestioneRapportiLavoroIscrittiOCM gestrdl,
          string iban)
        {
            this.rapportiLavoro.CercaBanca(gestrdl, iban);
            return gestrdl;
        }

        public GestioneRapportiLavoroIscrittiOCM CaricaModErediBLL(
          GestioneRapportiLavoroIscrittiOCM gestrdl,
          string mat,
          string proere)
        {
            this.rapportiLavoro.CarciaErediMod(gestrdl, mat, proere);
            return gestrdl;
        }
    }
}
