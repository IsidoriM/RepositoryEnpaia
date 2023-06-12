// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.RettificaPA_CA_BLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System;
using TFI.DAL.Amministrativo;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Amministrativo
{
  public class RettificaPA_CA_BLL
  {
    private RettifichePA_CA_DAL rettifichePA_CA_DAL = new RettifichePA_CA_DAL();

    public RettifichePA_CA_OCM SearchRett(
      RettifichePA_CA_OCM rettifichePA_CA_OCM1,
      string cerca,
      ref string MSGErorre,
      ref string MSGSuccess)
    {
      if (!string.IsNullOrEmpty(rettifichePA_CA_OCM1.search_rett.AnnoA) && !string.IsNullOrEmpty(rettifichePA_CA_OCM1.search_rett.MeseA))
      {
        rettifichePA_CA_OCM1 = this.rettifichePA_CA_DAL.SearchRett(rettifichePA_CA_OCM1, cerca, ref MSGErorre, ref MSGSuccess);
        return rettifichePA_CA_OCM1;
      }
      if (!string.IsNullOrEmpty(rettifichePA_CA_OCM1.search_rett.AnnoA) && string.IsNullOrEmpty(rettifichePA_CA_OCM1.search_rett.MeseA))
      {
        rettifichePA_CA_OCM1 = this.rettifichePA_CA_DAL.SearchRett(rettifichePA_CA_OCM1, cerca, ref MSGErorre, ref MSGSuccess);
        return rettifichePA_CA_OCM1;
      }
      if (string.IsNullOrEmpty(rettifichePA_CA_OCM1.search_rett.AnnoA) && !string.IsNullOrEmpty(rettifichePA_CA_OCM1.search_rett.MeseA))
      {
        MSGErorre = "Inserire anno finale";
        return rettifichePA_CA_OCM1;
      }
      if (!string.IsNullOrEmpty(rettifichePA_CA_OCM1.search_rett.AnnoA) || !string.IsNullOrEmpty(rettifichePA_CA_OCM1.search_rett.MeseA))
        return rettifichePA_CA_OCM1;
      rettifichePA_CA_OCM1 = this.rettifichePA_CA_DAL.SearchRett(rettifichePA_CA_OCM1, cerca, ref MSGErorre, ref MSGSuccess);
      return rettifichePA_CA_OCM1;
    }

    public bool SalvaRett(
      RettifichePA_CA_OCM rettifichePA_CA_OCM1,
      ref string MSGErorre,
      ref string MSGSuccess,
      TFI.OCM.Utente.Utente u)
    {
      return this.rettifichePA_CA_DAL.SalvaRett(rettifichePA_CA_OCM1, ref MSGErorre, ref MSGSuccess, u);
    }

    public string GetRagioneSociale(string codPos)
    {
      try
      {
        return this.rettifichePA_CA_DAL.GetRagioneSociale(codPos);
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }
  }
}
