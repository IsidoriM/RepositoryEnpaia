// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.EstrattoContoBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using Newtonsoft.Json;
using System;
using System.Web;
using TFI.DAL.Amministrativo;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Amministrativo
{
  public class EstrattoContoBLL
  {
    private TFI.OCM.Utente.Utente u = HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente;
    private readonly EstrattoContoDAL aziEster = new EstrattoContoDAL();

    public EstrattoContoOCM GetEstrattoConto(string CodPos, string ragsoc)
    {
      EstrattoContoOCM estrattoContoOcm = new EstrattoContoOCM();
      try
      {
        return this.aziEster.GetEstrattoContoAzienda(CodPos, ragsoc);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) estrattoContoOcm));
        return (EstrattoContoOCM) null;
      }
    }

    public EstrattoContoOCM GetEstrattoContoIsc(
      string mat,
      string nom,
      string cog,
      string codfis)
    {
      EstrattoContoOCM estrattoContoOcm = new EstrattoContoOCM();
      try
      {
        return this.aziEster.GetEstrattoContoIscritto(mat, nom, cog, codfis);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) estrattoContoOcm));
        return (EstrattoContoOCM) null;
      }
    }
  }
}
