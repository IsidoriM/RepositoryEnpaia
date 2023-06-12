// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.RappLegBLL
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
  public class RappLegBLL
  {
    public Rappresentante_legaleOCM RicRap(
      string posizione,
      string ragioneSociale,
      string partitaiva,
      string codiceFiscale,
      string cognome,
      string nome,
      string codfis,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      TFI.OCM.Utente.Utente utente = HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente;
      RappLegDAL rappLegDal = new RappLegDAL();
      Rappresentante_legaleOCM rappresentanteLegaleOcm = new Rappresentante_legaleOCM();
      try
      {
        if (!string.IsNullOrEmpty(posizione) || !string.IsNullOrEmpty(ragioneSociale) || !string.IsNullOrEmpty(partitaiva) || !string.IsNullOrEmpty(codiceFiscale) || !string.IsNullOrEmpty(cognome) || !string.IsNullOrEmpty(nome) || !string.IsNullOrEmpty(codfis))
          return rappLegDal.RicRappLeg(posizione, ragioneSociale, partitaiva, codiceFiscale, cognome, nome, codfis, ref ErroreMSG, ref SuccessMSG);
        ErroreMSG = "Inserire campi ricerca";
        return (Rappresentante_legaleOCM) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) utente), JsonConvert.SerializeObject((object) rappresentanteLegaleOcm));
        ErroreMSG = "Errore durante la ricerca";
        return (Rappresentante_legaleOCM) null;
      }
    }

    public Rappresentante_legaleOCM DetRap(
      string codpos,
      string datini,
      string rappri,
      string denfunrap,
      string datcom,
      string cog,
      string nom,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      TFI.OCM.Utente.Utente utente = HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente;
      RappLegDAL rappLegDal = new RappLegDAL();
      Rappresentante_legaleOCM rappresentanteLegaleOcm = new Rappresentante_legaleOCM();
      try
      {
        return rappLegDal.DetRap(codpos, datini, rappri, denfunrap, datcom, cog, nom, ref ErroreMSG, ref SuccessMSG);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) utente), JsonConvert.SerializeObject((object) rappresentanteLegaleOcm));
        ErroreMSG = "Errore durante l'operazione ";
        return rappresentanteLegaleOcm;
      }
    }

    public Rappresentante_legaleOCM InsRap(
      Rappresentante_legaleOCM.DettRap detrap,
      Rappresentante_legaleOCM rap,
      ref string ErroreMSG,
      ref string SuccessMSG,
      TFI.OCM.Utente.Utente u)
    {
      RappLegDAL rappLegDal = new RappLegDAL();
      try
      {
        if (detrap.datcom != rap.dettRap.datcom.Substring(0, 10))
          rap.dettRap.datcom = detrap.datcom;
        if (detrap.datini != rap.dettRap.datini.Substring(0, 10))
          rap.dettRap.datini = detrap.datini;
        if (detrap.ind != rap.dettRap.ind)
          rap.dettRap.ind = detrap.ind;
        if (detrap.numciv != rap.dettRap.numciv)
          rap.dettRap.numciv = detrap.numciv;
        if (detrap.dencom != rap.dettRap.dencom)
          rap.dettRap.dencom = detrap.dencom;
        if (detrap.sigpro != rap.dettRap.sigpro)
          rap.dettRap.sigpro = detrap.sigpro;
        if (detrap.cap != rap.dettRap.cap)
          rap.dettRap.cap = detrap.cap;
        if (detrap.denloc != rap.dettRap.denloc)
          rap.dettRap.denloc = detrap.denloc;
        if (detrap.denstaest == null)
        {
          detrap.denstaest = "";
          if (detrap.denstaest != rap.dettRap.denstaest)
            rap.dettRap.denstaest = detrap.denstaest;
        }
        else if (detrap.denstaest != rap.dettRap.denstaest)
          rap.dettRap.denstaest = detrap.denstaest;
        if (detrap.tel1 == null)
        {
          detrap.tel1 = "";
          if (detrap.tel1 != rap.dettRap.tel1)
            rap.dettRap.tel1 = detrap.tel1;
        }
        else if (detrap.tel1 != rap.dettRap.tel1)
          rap.dettRap.tel1 = detrap.tel1;
        if (detrap.tel2 == null)
        {
          detrap.tel2 = "";
          if (detrap.tel2 != rap.dettRap.tel2)
            rap.dettRap.tel2 = detrap.tel2;
        }
        else if (detrap.tel2 != rap.dettRap.tel2)
          rap.dettRap.tel2 = detrap.tel2;
        if (detrap.cell == null)
        {
          detrap.cell = "";
          if (detrap.cell != rap.dettRap.cell)
            rap.dettRap.cell = detrap.cell;
        }
        else if (detrap.cell != rap.dettRap.cell)
          rap.dettRap.cell = detrap.cell;
        if (detrap.fax == null)
        {
          detrap.fax = "";
          if (detrap.fax != rap.dettRap.fax)
            rap.dettRap.fax = detrap.fax;
        }
        else if (detrap.fax != rap.dettRap.fax)
          rap.dettRap.fax = detrap.fax;
        if (detrap.email == null)
        {
          detrap.email = "";
          if (detrap.email != rap.dettRap.email)
            rap.dettRap.email = detrap.email;
        }
        else if (detrap.email != rap.dettRap.email)
          rap.dettRap.email = detrap.email;
        if (detrap.emailcert == null)
        {
          detrap.emailcert = "";
          if (detrap.emailcert != rap.dettRap.emailcert)
            rap.dettRap.emailcert = detrap.emailcert;
        }
        else if (detrap.emailcert != rap.dettRap.emailcert)
          rap.dettRap.emailcert = detrap.emailcert;
        return rappLegDal.InsRap(rap, ref ErroreMSG, ref SuccessMSG, u);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) u), JsonConvert.SerializeObject((object) rap));
        ErroreMSG = "Errore durante l'operazione di Inserimento";
        return rap;
      }
    }

    public Rappresentante_legaleOCM Delete(
      Rappresentante_legaleOCM rap,
      string codpos,
      string prorec,
      string datconf,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      TFI.OCM.Utente.Utente utente = HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente;
      RappLegDAL rappLegDal = new RappLegDAL();
      try
      {
        return rappLegDal.Delete(rap, codpos, prorec, datconf, ref ErroreMSG, ref SuccessMSG);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) utente), JsonConvert.SerializeObject((object) rap));
        ErroreMSG = "Errore durante l'operazione di eliminazione";
        return rap;
      }
    }
  }
}
