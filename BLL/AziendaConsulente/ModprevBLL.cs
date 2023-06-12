// Decompiled with JetBrains decompiler
// Type: TFI.BLL.AziendaConsulente.ModprevBLL
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
  public class ModprevBLL
  {
    private TFI.OCM.Utente.Utente u = HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente;

    public ModPrevOCM GetModPre(
      int codPos,
      string mat,
      string nom,
      string cog,
      string prorap,
      string promod,
      ref string errorMessage)
    {
      ModPrevOCM modPrevOcm = new ModPrevOCM();
      ModprevDAL modprevDal = new ModprevDAL();
      try
      {
        return modprevDal.GetModPre(codPos, mat, nom, cog, prorap, promod, ref errorMessage);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) modPrevOcm));
        return (ModPrevOCM) null;
      }
    }

    public ModPrevOCM arretrati(
      string anncomp,
      string aliq,
      ModPrevOCM og,
      string elimina)
    {
      ModprevDAL modprevDal = new ModprevDAL();
      try
      {
        string date1 = DateTime.Now.ToString().Substring(6, 4);
        string date2 = DateTime.Now.AddYears(-1).ToString().Substring(6, 4);
        if (string.IsNullOrEmpty(elimina))
        {
          if (og.Arretrati.Count >= 2)
            return og;
          if (anncomp == date1)
          {
            DateTime dateTime = new DateTime(Convert.ToInt32(date1), 1, 1);
            og.Arretrati.Add(new ModPrevOCM.arretrati()
            {
              annoComp = date1,
              annoDen = anncomp,
              Aliq = aliq,
              occ = "0,00",
              retrimp = "0,00",
              iniperiodo = dateTime.ToString().Substring(0, 10),
              finperiodo = DateTime.Now.ToString().Substring(0, 10),
              id = date1
            });
            return og;
          }
          if (!(anncomp == date2))
            return og;
          DateTime dateTime1 = new DateTime(Convert.ToInt32(date2), 1, 1);
          DateTime dateTime2 = new DateTime(Convert.ToInt32(date2), 12, 31);
          og.Arretrati.Add(new ModPrevOCM.arretrati()
          {
            annoComp = date1,
            annoDen = anncomp,
            Aliq = aliq,
            occ = "0,00",
            retrimp = "0,00",
            iniperiodo = dateTime1.ToString().Substring(0, 10),
            finperiodo = dateTime2.ToString().Substring(0, 10),
            id = date2
          });
          return og;
        }
        Convert.ToInt32(date1);
        Convert.ToInt32(date2);
        ModPrevOCM.arretrati arretrati1 = og.Arretrati.Find((Predicate<ModPrevOCM.arretrati>) (x => x.id.Contains(date1)));
        ModPrevOCM.arretrati arretrati2 = og.Arretrati.Find((Predicate<ModPrevOCM.arretrati>) (x => x.id.Contains(date2)));
        if (arretrati1 != null)
        {
          int index = og.Arretrati.IndexOf(arretrati1);
          og.Arretrati.RemoveAt(index);
        }
        else if (arretrati2 != null)
        {
          int index = og.Arretrati.IndexOf(arretrati2);
          og.Arretrati.RemoveAt(index);
        }
        return og;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) og));
        return og;
      }
    }

    public ModPrevOCM sospensioni(
      string dal,
      string al,
      string sospensione,
      ModPrevOCM og)
    {
      try
      {
        if (DateTime.Compare(Convert.ToDateTime(dal), Convert.ToDateTime(al)) < 0)
        {
          og.sosp.Add(new ModPrevOCM.sospensioni()
          {
            dal = dal,
            al = al,
            motsosp = sospensione
          });
          return og;
        }
        return og;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) og));
        return og;
      }
    }

    public ModPrevOCM Save_sosp(ModPrevOCM og)
    {
      try
      {
        og = new ModprevDAL().Save_sosp(og);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) og));
      }
      return og;
    }

    public ModPrevOCM Save_arr(ModPrevOCM og)
    {
      try
      {
        og = new ModprevDAL().Save_arr(og);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) og));
      }
      return og;
    }

    public ModPrevOCM Save_den(ModPrevOCM og)
    {
      try
      {
        og = new ModprevDAL().Save_den(og);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) this.u), JsonConvert.SerializeObject((object) og));
      }
      return og;
    }
  }
}
