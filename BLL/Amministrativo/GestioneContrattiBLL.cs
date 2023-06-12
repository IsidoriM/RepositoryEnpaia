// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.GestioneContrattiBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using TFI.DAL.Amministrativo;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Amministrativo
{
  public class GestioneContrattiBLL
  {
    private readonly GestioneContrattiDAl contrattiDAL = new GestioneContrattiDAl();

    public void ModificaContrattoBLL(
      GestioneContrattiOCM.Contratti gc,
      TFI.OCM.Utente.Utente u,
      ref string MSGErorre,
      ref string MSGSuccess)
    {
      this.contrattiDAL.ModificaContratto(gc, u, ref MSGErorre, ref MSGSuccess);
    }

    public void Carica(GestioneContrattiOCM gc) => this.contrattiDAL.CaricaContratto(gc);

    public void ContrattiInserimentoBLL(
      GestioneContrattiOCM.Contratti gc,
      TFI.OCM.Utente.Utente u,
      ref string MSGErorre,
      ref string MSGSuccess)
    {
      this.contrattiDAL.SalvaNuovoContratto(gc, u, ref MSGErorre, ref MSGSuccess);
    }

    public void EliminaContrattiBLL(
      GestioneContrattiOCM gc,
      TFI.OCM.Utente.Utente u,
      string CODCON,
      string PROCON,
      string DATINI,
      string DATFIN,
      ref string MSGErorre,
      ref string MSGSuccess)
    {
      this.contrattiDAL.Elimina(gc, u, CODCON, PROCON, DATINI, DATFIN, ref MSGErorre, ref MSGSuccess);
    }

    public GestioneContrattiOCM ContrattiRicercaTabellaBLL(
      string Denominazione,
      string livello,
      ref string ErroreMSG)
    {
      GestioneContrattiOCM gestioneContrattiOcm = new GestioneContrattiOCM();
      if (!string.IsNullOrEmpty(Denominazione) || !string.IsNullOrEmpty(livello))
        return this.contrattiDAL.CaricaDati(Denominazione, livello, ref ErroreMSG);
      ErroreMSG = "Inserire almeno un campo di ricerca";
      return (GestioneContrattiOCM) null;
    }
  }
}
