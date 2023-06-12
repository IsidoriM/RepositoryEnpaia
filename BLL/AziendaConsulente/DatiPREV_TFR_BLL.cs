// Decompiled with JetBrains decompiler
// Type: TFI.BLL.AziendaConsulente.DatiPREV_TFR_BLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using TFI.DAL.AziendaConsulente;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;

namespace TFI.BLL.AziendaConsulente
{
  public class DatiPREV_TFR_BLL
  {
    private TFI.OCM.Utente.Utente u = HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente;

    public List<DatiPREV_TFR_OCM.PrevdaConf> SearchPrevBLL(
      string mat,
      string cog,
      string nom,
      string periodo1,
      string periodo2,
      string stato,
      string codpos)
    {
      List<DatiPREV_TFR_OCM.PrevdaConf> prevdaConfList = new List<DatiPREV_TFR_OCM.PrevdaConf>();
      try
      {
        return new DatiPREV_TFR_DAL().SearchPrev(mat, cog, nom, periodo1, periodo2, stato, codpos);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) prevdaConfList));
        return (List<DatiPREV_TFR_OCM.PrevdaConf>) null;
      }
    }

    public List<DatiPREV_TFR_OCM.PrevdaConf> SearchPrevConfBLL(string codpos)
    {
      List<DatiPREV_TFR_OCM.PrevdaConf> prevdaConfList = new List<DatiPREV_TFR_OCM.PrevdaConf>();
      try
      {
        return new DatiPREV_TFR_DAL().SearchPrevConfDAL(codpos);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) prevdaConfList));
        return (List<DatiPREV_TFR_OCM.PrevdaConf>) null;
      }
    }

    public DatiPREV_TFR_OCM ProspTFR(string codPos)
    {
      DatiPREV_TFR_OCM datiPrevTfrOcm = new DatiPREV_TFR_OCM();
      try
      {
        return new DatiPREV_TFR_DAL().ProspTFR(codPos);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) datiPrevTfrOcm));
        return (DatiPREV_TFR_OCM) null;
      }
    }
  }
}
