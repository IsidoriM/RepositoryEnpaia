// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.Rettifiche_BLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System;
using TFI.DAL.Amministrativo;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Amministrativo
{
  public class Rettifiche_BLL
  {
    private Rettifiche_DAL rettifiche_DAL = new Rettifiche_DAL();

    public Rettifiche_OCM SearchRett(
      Rettifiche_OCM rettifiche_OCM,
      ref string MSGErorre,
      ref string MSGSuccess,
      TFI.OCM.Utente.Utente u)
    {
      if (!string.IsNullOrEmpty(rettifiche_OCM.searchRett.Codpos) && string.IsNullOrEmpty(rettifiche_OCM.searchRett.AnnoA) && string.IsNullOrEmpty(rettifiche_OCM.searchRett.MeseA))
      {
        rettifiche_OCM = this.rettifiche_DAL.SearchRett(rettifiche_OCM, ref MSGErorre, ref MSGSuccess, u);
        return rettifiche_OCM;
      }
      if (!string.IsNullOrEmpty(rettifiche_OCM.searchRett.Codpos) && !string.IsNullOrEmpty(rettifiche_OCM.searchRett.AnnoA) && !string.IsNullOrEmpty(rettifiche_OCM.searchRett.MeseA))
      {
        rettifiche_OCM = this.rettifiche_DAL.SearchRett(rettifiche_OCM, ref MSGErorre, ref MSGSuccess, u);
        return rettifiche_OCM;
      }
      if (!string.IsNullOrEmpty(rettifiche_OCM.searchRett.Codpos) && !string.IsNullOrEmpty(rettifiche_OCM.searchRett.AnnoA) && string.IsNullOrEmpty(rettifiche_OCM.searchRett.MeseA))
      {
        rettifiche_OCM = this.rettifiche_DAL.SearchRett(rettifiche_OCM, ref MSGErorre, ref MSGSuccess, u);
        return rettifiche_OCM;
      }
      if (!string.IsNullOrEmpty(rettifiche_OCM.searchRett.Codpos) && string.IsNullOrEmpty(rettifiche_OCM.searchRett.AnnoA) && !string.IsNullOrEmpty(rettifiche_OCM.searchRett.MeseA))
      {
        MSGErorre = "Inserire anno finale";
        return rettifiche_OCM;
      }
      if (!string.IsNullOrEmpty(rettifiche_OCM.searchRett.Codpos) && string.IsNullOrEmpty(rettifiche_OCM.searchRett.AnnoA) && string.IsNullOrEmpty(rettifiche_OCM.searchRett.MeseA))
      {
        rettifiche_OCM = this.rettifiche_DAL.SearchRett(rettifiche_OCM, ref MSGErorre, ref MSGSuccess, u);
        return rettifiche_OCM;
      }
      if (!string.IsNullOrEmpty(rettifiche_OCM.searchRett.Codpos))
        return rettifiche_OCM;
      MSGErorre = "Inserire il codice posizione";
      return rettifiche_OCM;
    }

    public void SalvaRett(
      TFI.OCM.Utente.Utente u,
      Rettifiche_OCM rettifiche_OCM,
      ref string MSGErorre,
      ref string MSGSuccess)
    {
      this.rettifiche_DAL.SalvaRett(u, rettifiche_OCM, ref MSGErorre, ref MSGSuccess);
    }

    public string GetNominativo(string mat)
    {
      try
      {
        return this.rettifiche_DAL.GetNominativo(mat);
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }
  }
}
