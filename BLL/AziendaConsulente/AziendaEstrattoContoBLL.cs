// Decompiled with JetBrains decompiler
// Type: TFI.BLL.AziendaConsulente.AziendaEstrattoContoBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using Newtonsoft.Json;
using System;
using System.Web;
using TFI.DAL.AziendaConsulente;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;

namespace TFI.BLL.AziendaConsulente
{
  public class AziendaEstrattoContoBLL
  {
    private TFI.OCM.Utente.Utente u = HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente;
    private readonly AziendaEstattoContoDAL aziEster = new AziendaEstattoContoDAL();

    public AnagraficaEstrattiContoAzienda GetEstrattoContoAzienda(
      string CodPos)
    {
      AnagraficaEstrattiContoAzienda estrattiContoAzienda = new AnagraficaEstrattiContoAzienda();
      try
      {
        return this.aziEster.GetEstrattoContoAzienda(CodPos);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) estrattiContoAzienda));
        return (AnagraficaEstrattiContoAzienda) null;
      }
    }
  }
}
