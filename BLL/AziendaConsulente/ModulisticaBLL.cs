// Decompiled with JetBrains decompiler
// Type: TFI.BLL.AziendaConsulente.ModulisticaBLL
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
  public class ModulisticaBLL
  {
    private readonly ModulisticaDAL ModDAL = new ModulisticaDAL();
    private TFI.OCM.Utente.Utente u = HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente;

    public List<Modulistica> ListMod()
    {
      List<Modulistica> modulisticaList = new List<Modulistica>();
      try
      {
        modulisticaList = this.ModDAL.ModuliDAL();
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) modulisticaList));
      }
      return modulisticaList;
    }
  }
}
