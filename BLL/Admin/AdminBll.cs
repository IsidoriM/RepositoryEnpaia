// Decompiled with JetBrains decompiler
// Type: TFI.BLL.AdminBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System.Collections.Generic;
using System.Linq;
using TFI.DAL;
using TFI.OCM;

namespace TFI.BLL
{
  public class AdminBLL
  {
    public static string Message = string.Empty;

    public List<Admin> GetAnagrafica(
      string codiceUtente,
      string dropDenominazione,
      string dropStato,
      string selezionaSistema,
      string selezionaGruppo,
      string selezionaFunzione)
    {
      if (dropDenominazione == "")
        dropDenominazione = (string) null;
      if (dropStato == "Selezionare uno stato")
        dropStato = (string) null;
      return new AdminDAL().GetAnagrafica(codiceUtente, dropDenominazione, dropStato, selezionaSistema, selezionaGruppo, selezionaFunzione);
    }

    public List<Admin> GetInserimento(
      string codiceUtenteIns,
      string selezionaDenominazioneIns,
      string email,
      string codFis,
      string uteWin)
    {
      return new AdminDAL().GetInserimento(codiceUtenteIns, selezionaDenominazioneIns, email, codFis, uteWin);
    }

    public List<Admin> GetDati(
      string codUtente,
      string nome,
      string codFis,
      string email,
      string winUt,
      bool checkGruppo,
      bool checkFunzione)
    {
      return new AdminDAL().GetDati(codUtente, nome, codFis, email, winUt, checkGruppo, checkFunzione);
    }

    public List<Admin> GetGruppiWeb(string codUte)
    {
      AdminDAL adminDal = new AdminDAL();
      List<Admin> gruppiWeb = adminDal.GetGruppiWeb(codUte);
      List<Admin> checkBoxGruppi = adminDal.GetCheckBoxGruppi(codUte);
      foreach (Admin admin1 in gruppiWeb.ToList<Admin>())
      {
        foreach (Admin admin2 in checkBoxGruppi)
        {
          if (admin1.denGrusis == admin2.denGrusis)
            admin1.checkGruppo = true;
        }
      }
      return gruppiWeb;
    }

    public List<Admin> GetFunzioniWeb(string codUte)
    {
      AdminDAL adminDal = new AdminDAL();
      List<Admin> funzioniWeb = adminDal.GetFunzioniWeb(codUte);
      List<Admin> checkBoxFunzioni = adminDal.GetCheckBoxFunzioni(codUte);
      foreach (Admin admin1 in funzioniWeb.ToList<Admin>())
      {
        foreach (Admin admin2 in checkBoxFunzioni)
        {
          if (admin1.denFunsis == admin2.denFunsis)
            admin1.checkFunzione = true;
        }
      }
      return funzioniWeb;
    }

    public List<Admin> GetDropDpownList1() => new AdminDAL().GetDropDpownList1();

    public List<Admin> GetDropDpownList2() => new AdminDAL().GetDropDpownList2();

    public List<Admin> GetDropDpownList3() => new AdminDAL().GetDropDpownList3();

    public List<Admin> GetCheckBoxFunzioni(string codUte) => new AdminDAL().GetCheckBoxFunzioni(codUte);

    public List<Admin> DeleteSaveGruppi(string codUtente, List<Admin> listaGruppi) => new AdminDAL().DeleteSaveGruppi(codUtente, listaGruppi);

    public List<Admin> DeleteSaveFunzioni(string codUtente, List<Admin> listaFunzioni) => new AdminDAL().DeleteSaveFunzioni(codUtente, listaFunzioni);

    public List<Admin> GetcheckboxSelected(string id) => new AdminDAL().GetcheckboxSelected(id);

    public List<Admin> GetTabella(string selNome) => new AdminDAL().GetTabella(selNome);

    public string GetSalvataggio(string newGrup, string oldGrup)
    {
      string salvataggio = new AdminDAL().GetSalvataggio(newGrup, oldGrup);
      if (salvataggio != null)
      {
        AdminBLL.Message = "Salvataggio avvenuto con Successo";
        return salvataggio;
      }
      AdminBLL.Message = "Gruppo vuoto o già inserito";
      return (string) null;
    }

    public string GetNuovoGruppo(string NomeGruppo)
    {
      string nuovoGruppo = new AdminDAL().GetNuovoGruppo(NomeGruppo);
      if (nuovoGruppo != null)
      {
        AdminBLL.Message = "Inserimento nuovo gruppo avvenuto con Successo";
        return nuovoGruppo;
      }
      AdminBLL.Message = "Gruppo già inserito";
      return (string) null;
    }

    public List<Admin> AnagraficaFunzionalitaBLL(string selezioneNomeFunz) => new AdminDAL().TabellaFunzionalita(selezioneNomeFunz);

    public string SalvataggioFunBLL(string newFun, string oldFun)
    {
      string str = new AdminDAL().SalvataggioFunzionalita(newFun, oldFun);
      if (str != null)
      {
        AdminBLL.Message = "Salvataggio avvenuto con Successo";
        return str;
      }
      AdminBLL.Message = "Funzionalità vuoto o già inserito";
      return (string) null;
    }

    public string NewFunBLL(string nuovaFun)
    {
      string str = new AdminDAL().NewFunzionalita(nuovaFun);
      if (str != null)
      {
        AdminBLL.Message = "Inserimento nuova funzionalità avvenuto con Successo";
        return str;
      }
      AdminBLL.Message = "Funzionalità già inserita";
      return (string) null;
    }

    public List<Admin> GruppoFunzionalitaBLL(string oldGrup) => new AdminDAL().TabellaGruppoFunzionalita(oldGrup);
  }
}
