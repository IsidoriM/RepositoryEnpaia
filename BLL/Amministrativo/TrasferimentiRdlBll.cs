// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.TrasferimentiRdlBll
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using TFI.DAL.Amministrativo;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Amministrativo
{
  public class TrasferimentiRdlBll
  {
    private readonly TrasferimentiRdlDal trasferimenti = new TrasferimentiRdlDal();

    public void SalvaTrasferimentiBll(
      TrasferimentiRdlOCM trasferimentiRdlOCM,
      TFI.OCM.Utente.Utente u,
      string CodPos,
      string DatDec,
      ref string MsgErrore,
      ref string MsgSuccess)
    {
      this.trasferimenti.Salva_EseguiTrasferimento(trasferimentiRdlOCM, u, CodPos, DatDec, ref MsgErrore, ref MsgSuccess);
    }

    public bool Aggiungi_click(
      TrasferimentiRdlOCM tra,
      string CodPos,
      string CodPosNew,
      string DatDec,
      ref string MsgErrore)
    {
      return this.trasferimenti.BtnAggiungi_Click(tra, CodPos, CodPosNew, DatDec, ref MsgErrore);
    }

    public void CaricaDati_Trasferimento(
      TrasferimentiRdlOCM trasferimentiRdlOCM,
      string DatDec,
      string CodPos,
      ref string MsgErrore)
    {
      this.trasferimenti.CaricaDati_ListaIscrittiXTrasferimenti(trasferimentiRdlOCM, DatDec, CodPos, ref MsgErrore);
    }

    public TrasferimentiRdlOCM CaricaDatiRicercaTrasferimenti(
      TrasferimentiRdlOCM.DatiTrasferimento rdl,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      return this.trasferimenti.CaricaDatiRicercaTrasferimenti(rdl, ref ErroreMSG, ref SuccessMSG);
    }

    public void EseguiAnnullamento(
      TFI.OCM.Utente.Utente u,
      string CodPos,
      string CodPosTra,
      string RagSoc,
      string RagSocTra,
      string Matricola,
      string Nome,
      string Cognome,
      string ProRap,
      string ProRapTra,
      string ProTraRap,
      string Gruppo,
      string ProSos,
      string DatAnn,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      this.trasferimenti.EseguiAnnullamentoTrasferimento(u, CodPos, CodPosTra, RagSoc, RagSocTra, Matricola, Nome, Cognome, ProRap, ProRapTra, ProTraRap, Gruppo, ProSos, DatAnn, ref ErroreMSG, ref SuccessMSG);
    }
  }
}
