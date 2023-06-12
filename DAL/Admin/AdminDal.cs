// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AdminDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.OCM;

namespace TFI.DAL
{
  public class AdminDAL
  {
    public List<Admin> GetDropDpownList1()
    {
      DataLayer dataLayer = new DataLayer();
      string strSQL = "SELECT CODSIS, SISTEMA FROM SISTEMI WHERE VISIBILE = 'S'  ORDER BY SISTEMA";
      DataTable dataTable = new DataTable();
      List<Admin> dropDpownList1 = new List<Admin>();
      string Err = "";
      DataSet dataSet = new DataSet();
      foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataSet(strSQL, ref Err).Tables[0].Rows)
      {
        Admin admin = new Admin()
        {
          codSis = row["CODSIS"].ToString(),
          sistema = row["SISTEMA"].ToString()
        };
        dropDpownList1.Add(admin);
      }
      return dropDpownList1;
    }

    public List<Admin> GetDropDpownList2()
    {
      DataLayer dataLayer = new DataLayer();
      string strSQL = "SELECT CODGRU, DENGRUSIS FROM GRUSIS ORDER BY DENGRUSIS";
      DataTable dataTable = new DataTable();
      List<Admin> dropDpownList2 = new List<Admin>();
      string Err = "";
      DataSet dataSet = new DataSet();
      foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataSet(strSQL, ref Err).Tables[0].Rows)
      {
        Admin admin = new Admin()
        {
          codGru = row["CODGRU"].ToString(),
          denGru = row["DENGRUSIS"].ToString()
        };
        dropDpownList2.Add(admin);
      }
      return dropDpownList2;
    }

    public List<Admin> GetDropDpownList3()
    {
      DataLayer dataLayer = new DataLayer();
      string strSQL = "SELECT CODFUNSIS, DENFUNSIS FROM FUNSIS ORDER BY DENFUNSIS";
      DataTable dataTable = new DataTable();
      List<Admin> dropDpownList3 = new List<Admin>();
      string Err = "";
      DataSet dataSet = new DataSet();
      foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataSet(strSQL, ref Err).Tables[0].Rows)
      {
        Admin admin = new Admin()
        {
          codFun = row["CODFUNSIS"].ToString(),
          denFun = row["DENFUNSIS"].ToString()
        };
        dropDpownList3.Add(admin);
      }
      return dropDpownList3;
    }

    public List<Admin> GetAnagrafica(
      string codiceUtente,
      string selezionaDenominazione,
      string dropStato,
      string selezionaSistema,
      string selezionaGruppo,
      string selezionaFunzione)
    {
      if (selezionaDenominazione == "")
        selezionaDenominazione = (string) null;
      if (codiceUtente == "")
        codiceUtente = (string) null;
      if (dropStato == "null")
        dropStato = (string) null;
      if (selezionaSistema == "null")
        selezionaSistema = (string) null;
      if (selezionaGruppo == "null")
        selezionaGruppo = (string) null;
      if (selezionaFunzione == "null")
        selezionaFunzione = (string) null;
      string str = (string) null;
      if (!string.IsNullOrEmpty(codiceUtente))
        str = " AND UTENTI.CODUTE like '%" + codiceUtente.ToUpper() + "%'";
      if (!string.IsNullOrEmpty(selezionaDenominazione))
        str = str + " AND UTENTI.DENUTE LIKE '%" + selezionaDenominazione.ToUpper() + "%'";
      if (!string.IsNullOrEmpty(dropStato))
        str = !(dropStato == "D") ? str + " AND CODUTE IN (SELECT CODUTE FROM UTEPIN WHERE STAPIN = 'A' AND DATINI = (SELECT MAX(DATINI) FROM UTEPIN WHERE CODUTE = UTENTI.CODUTE))" : str + " AND CODUTE IN (SELECT CODUTE FROM UTEPIN WHERE STAPIN = 'D' AND DATINI = (SELECT MAX(DATINI) FROM UTEPIN WHERE CODUTE = UTENTI.CODUTE))";
      if (!string.IsNullOrEmpty(selezionaSistema))
        str = str + " AND UTENTI.CODUTE IN (SELECT CODUTE FROM SISUTE WHERE CODSIS = '" + selezionaSistema + "')";
      if (!string.IsNullOrEmpty(selezionaGruppo))
        str = str + " AND UTENTI.CODUTE IN (SELECT CODUTE FROM GRUUTE WHERE CODGRU =  '" + selezionaGruppo + "')";
      if (!string.IsNullOrEmpty(selezionaFunzione))
        str = str + " AND (UTENTI.CODUTE IN (SELECT CODUTE FROM PROUTE WHERE CODFUNSIS = '" + selezionaFunzione + "') OR UTENTI.CODUTE IN (SELECT GRUUTE.CODUTE FROM GRUUTE, FUNGRU  WHERE GRUUTE.CODGRU =  FUNGRU.CODGRU AND FUNGRU.CODFUNSIS= '" + selezionaFunzione + "') OR UTENTI.CODUTE IN (SELECT ENPUTE.CODUTE FROM ENPUTE, PROENPF  WHERE ENPUTE.CODFUNENP =  PROENPF.CODFUNENP AND PROENPF.CODFUNSIS= '" + selezionaFunzione + "'))";
      if (selezionaDenominazione == null && dropStato == null && codiceUtente == null && selezionaSistema == null && selezionaGruppo == null && selezionaFunzione == null)
        return (List<Admin>) null;
      iDB2DataReader dataReaderFromQuery = new DataLayer().GetDataReaderFromQuery("SELECT DISTINCT TIPUTE.DENTIPUTE, UTENTI.CODFIS, UTENTI.CODUTE, UTENTI.DENUTE, UTENTI.EMAIL, UTENTI.UTEWINDOWS,  (SELECT DISTINCT CASE STAPIN WHEN 'A' THEN 'ATTIVO' WHEN 'D' THEN 'NON ATTIVO' END CASE  FROM UTEPIN WHERE UTEPIN.CODUTE = UTENTI.CODUTE AND UTEPIN.DATFIN = (SELECT MAX(DATFIN) FROM UTEPIN WHERE UTEPIN.CODUTE=UTENTI.CODUTE)) AS STAPIN,  UTENTI.UTEAGG, UTENTI.ULTAGG  FROM UTENTI, TIPUTE  WHERE UTENTI.CODTIPUTE = TIPUTE.CODTIPUTE AND UTENTI.CODTIPUTE = 'E'" + str + " ORDER BY UTENTI.DENUTE ", CommandType.Text);
      List<Admin> anagrafica = new List<Admin>();
      Admin admin1 = new Admin();
      while (dataReaderFromQuery.Read())
      {
        Admin admin2 = new Admin()
        {
          denTipute = dataReaderFromQuery["DENTIPUTE"].ToString().Trim(),
          codFis = dataReaderFromQuery["codfis"].ToString().Trim(),
          codUtente = dataReaderFromQuery["codute"].ToString().Trim(),
          nome = dataReaderFromQuery["denute"].ToString().Trim(),
          email = dataReaderFromQuery["EMAIL"].ToString().Trim(),
          winUt = dataReaderFromQuery["UTEWINDOWS"].ToString().Trim()
        };
        anagrafica.Add(admin2);
      }
      return anagrafica;
    }

    public List<Admin> GetInserimento(
      string codiceUtenteIns,
      string selezionaDenominazioneIns,
      string email,
      string codFis,
      string uteWin)
    {
      List<Admin> inserimento = new List<Admin>();
      Admin admin = new Admin();
      string Err = "";
      DataLayer dataLayer = new DataLayer();
      string strSQL = "INSERT INTO UTENTI(CODUTE,DENUTE,CODTIPUTE,ULTAGG,UTEAGG,EMAIL,CODFIS,UTEWINDOWS)  VALUES('" + codiceUtenteIns.ToUpper() + "', '" + selezionaDenominazioneIns.ToUpper() + "',  'E', CURRENT TIMESTAMP,'ADMIN', '" + email + "', '" + codFis.ToUpper() + "', '" + uteWin.ToUpper() + "')";
      DataSet dataSet = new DataSet();
      dataSet = dataLayer.GetDataSet(strSQL, ref Err);
      int num = 1;
      admin.recordAffected = num;
      if (admin.recordAffected == 0)
        return (List<Admin>) null;
      inserimento.Add(admin);
      return inserimento;
    }

    public List<Admin> GetGruppiWeb(string codUte)
    {
      new Admin().codUtente = codUte;
      iDB2DataReader dataReaderFromQuery = new DataLayer().GetDataReaderFromQuery("SELECT DISTINCT g2.CODGRU, g2.DENGRUSIS FROM GRUUTE g, GRUSIS g2 \tWHEre  TIPGRU  = 'W' ORDER BY g2.CODGRU", CommandType.Text);
      List<Admin> gruppiWeb = new List<Admin>();
      while (dataReaderFromQuery.Read())
      {
        Admin admin = new Admin()
        {
          idCheckbox = dataReaderFromQuery["CODGRU"].ToString(),
          denGrusis = dataReaderFromQuery["DENGRUSIS"].ToString().Trim()
        };
        gruppiWeb.Add(admin);
      }
      return gruppiWeb;
    }

    public List<Admin> GetFunzioniWeb(string codUte)
    {
      new Admin().codUtente = codUte;
      iDB2DataReader dataReaderFromQuery = new DataLayer().GetDataReaderFromQuery("SELECT DISTINCT f.CODFUNSIS,f.DENFUNSIS FROM proute g, FUNSIS f,FUNGRU f2  WHERE TIPFUN = 'W' ORDER BY f.CODFUNSIS", CommandType.Text);
      List<Admin> funzioniWeb = new List<Admin>();
      int num = 0;
      while (dataReaderFromQuery.Read())
      {
        Admin admin = new Admin()
        {
          idCheckbox = dataReaderFromQuery["CODFUNSIS"].ToString().Trim(),
          denFunsis = dataReaderFromQuery["DENFUNSIS"].ToString().Trim()
        };
        funzioniWeb.Add(admin);
        ++num;
      }
      return funzioniWeb;
    }

    public List<Admin> GetCheckBoxGruppi(string codUte)
    {
      new Admin().codUtente = codUte;
      iDB2DataReader dataReaderFromQuery = new DataLayer().GetDataReaderFromQuery("SELECT DISTINCT  g2.CODGRU,g2.DENGRUSIS FROM GRUUTE g, GRUSIS g2 \tWHERE CODUTE  = '" + codUte + "'AND TIPGRU  = 'W' AND g.CODGRU = g2.CODGRU ", CommandType.Text);
      List<Admin> checkBoxGruppi = new List<Admin>();
      Admin admin1 = new Admin();
      while (dataReaderFromQuery.Read())
      {
        Admin admin2 = new Admin()
        {
          denGrusis = dataReaderFromQuery["DENGRUSIS"].ToString().Trim(),
          idCheckbox = dataReaderFromQuery["CODGRU"].ToString().Trim()
        };
        checkBoxGruppi.Add(admin2);
      }
      return checkBoxGruppi;
    }

    public List<Admin> GetCheckBoxFunzioni(string codUte)
    {
      new Admin().codUtente = codUte;
      iDB2DataReader dataReaderFromQuery = new DataLayer().GetDataReaderFromQuery("SELECT DISTINCT f.CODFUNSIS,f.DENFUNSIS FROM proute g, FUNSIS f,FUNGRU f2  WHERE TIPFUN = 'W' AND CODUTE = '" + codUte + "'", CommandType.Text);
      List<Admin> checkBoxFunzioni = new List<Admin>();
      Admin admin1 = new Admin();
      while (dataReaderFromQuery.Read())
      {
        Admin admin2 = new Admin()
        {
          idCheckbox = dataReaderFromQuery["CODFUNSIS"].ToString().Trim(),
          denFunsis = dataReaderFromQuery["DENFUNSIS"].ToString().Trim()
        };
        checkBoxFunzioni.Add(admin2);
      }
      return checkBoxFunzioni;
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
      List<Admin> dati = new List<Admin>();
      Admin admin = new Admin();
      string Err = "";
      DataLayer dataLayer = new DataLayer();
      string strSQL = "UPDATE UTENTI SET CODFIS = '" + codFis + "', DENUTE =  '" + nome + "', EMAIL =  '" + email + "', UTEWINDOWS =  '" + winUt + "' WHERE CODUTE =  '" + codUtente + "'";
      DataSet dataSet = new DataSet();
      dataSet = dataLayer.GetDataSet(strSQL, ref Err);
      int num = 1;
      admin.recordAffected = num;
      if (admin.recordAffected == 0)
        return (List<Admin>) null;
      dati.Add(admin);
      return dati;
    }

    public List<Admin> DeleteSaveGruppi(string codUte, List<Admin> listaGruppi)
    {
      Admin admin1 = new Admin();
      admin1.codUtente = codUte;
      List<Admin> adminList = new List<Admin>();
      DataLayer dataLayer = new DataLayer();
      string Err = "";
      int num = 0;
      foreach (Admin admin2 in listaGruppi)
      {
        string strSQL = "SELECT codgru FROM grusis WHERE DENGRUSIS = '" + admin2.denGrusis + "' AND TIPGRU = 'W'";
        admin2.codGru = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        adminList.Add(admin2);
      }
      DataSet dataSet = new DataSet();
      string strSQL1 = "DELETE FROM GRUUTE WHERE CODUTE = '" + codUte + "' AND CODGRU IN(SELECT CODGRU FROM  GRUSIS WHEre tipgru = 'W') ";
      dataSet = dataLayer.GetDataSet(strSQL1, ref Err);
      foreach (Admin admin3 in adminList)
      {
        string strSQL2 = "INSERT INTO GRUUTE VALUES('" + codUte + "','" + admin3.codGru + "', CURRENT TIMESTAMP,'ADMIN')";
        dataSet = dataLayer.GetDataSet(strSQL2, ref Err);
        ++num;
      }
      admin1.recordAffected = num;
      if (admin1.recordAffected == 0)
        return (List<Admin>) null;
      adminList.Add(admin1);
      return adminList;
    }

    public List<Admin> DeleteSaveFunzioni(string codUte, List<Admin> listaFunzioni)
    {
      Admin admin1 = new Admin();
      admin1.codUtente = codUte;
      List<Admin> adminList = new List<Admin>();
      DataLayer dataLayer = new DataLayer();
      string Err = "";
      int num = 0;
      foreach (Admin admin2 in listaFunzioni)
      {
        string strSQL = "SELECT CODFUNSIS FROM FUNSIS WHERE TIPFUN = 'W' AND  DENFUNSIS  = '" + admin2.denFunsis + "'";
        admin2.codFun = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        adminList.Add(admin2);
      }
      DataSet dataSet = new DataSet();
      string strSQL1 = "DELETE FROM PROUTE \tWHERE CODUTE  = '" + codUte + "'AND CODFUNSIS IN (SELECT CODFUNSIS FROM FUNSIS f WHEre TIPFUN = 'W')";
      dataSet = dataLayer.GetDataSet(strSQL1, ref Err);
      foreach (Admin admin3 in adminList)
      {
        string strSQL2 = "INSERT INTO PROUTE VALUES('" + codUte + "','" + admin3.codFun + "', CURRENT TIMESTAMP,'ADMIN')";
        dataSet = dataLayer.GetDataSet(strSQL2, ref Err);
        ++num;
      }
      admin1.recordAffected = num;
      if (admin1.recordAffected == 0)
        return (List<Admin>) null;
      adminList.Add(admin1);
      return adminList;
    }

    public List<Admin> GetcheckboxSelected(string id)
    {
      DataLayer dataLayer = new DataLayer();
      string strSQL = "SELECT DISTINCT  f2.CODFUNSIS, f2.DENFUNSIS FROM FUNSIS f2 LEFT JOIN FUNGRU f ON f2.CODFUNSIS = f.CODFUNSIS WHERE f.CODGRU = '" + id + "' AND f2.TIPFUN = 'W'";
      DataTable dataTable = new DataTable();
      List<Admin> adminList = new List<Admin>();
      string Err = "";
      DataSet dataSet = new DataSet();
      foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataSet(strSQL, ref Err).Tables[0].Rows)
      {
        Admin admin = new Admin()
        {
          codFun = row["CODFUNSIS"].ToString()
        };
        adminList.Add(admin);
      }
      return adminList;
    }

    public List<Admin> GetTabella(string selNome)
    {
      if (selNome == "")
        selNome = (string) null;
      if (selNome == null)
        return (List<Admin>) null;
      DataLayer dataLayer = new DataLayer();
      string strSQL = "SELECT COUNT(g.CODGRU) as CONTEGGIO , g2.DENGRUSIS FROM GRUUTE g FULL JOIN GRUSIS g2 ON g.CODGRU = g2.CODGRU WHERE g2.DENGRUSIS LIKE'%" + selNome.ToUpper() + "%' GROUP BY g2.dengrusis";
      DataTable dataTable = new DataTable();
      List<Admin> tabella = new List<Admin>();
      string Err = "";
      DataSet dataSet = new DataSet();
      foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataSet(strSQL, ref Err).Tables[0].Rows)
      {
        Admin admin = new Admin()
        {
          codGru = row["CONTEGGIO"].ToString(),
          denGru = row["DENGRUSIS"].ToString()
        };
        tabella.Add(admin);
      }
      return tabella;
    }

    public string GetSalvataggio(string newGrup, string oldGrup)
    {
      try
      {
        DataLayer dataLayer = new DataLayer();
        string strSQL1 = "SELECT g.DENGRUSIS FROM GRUSIS g WHERE g.DENGRUSIS = '" + newGrup + "'";
        DataSet dataSet = new DataSet();
        if (!string.IsNullOrEmpty(dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text).ToString()))
          return (string) null;
        string strSQL2 = "UPDATE GRUSIS g SET g.DENGRUSIS ='" + newGrup.ToUpper() + "' , g.ULTAGG = CURRENT TIMESTAMP WHERE g.DENGRUSIS ='" + oldGrup + "'";
        return dataLayer.Get1ValueFromSQL(strSQL2, CommandType.Text).ToString();
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    public string GetNuovoGruppo(string NomeGruppo)
    {
      try
      {
        DataLayer dataLayer = new DataLayer();
        string strSQL1 = "SELECT g.DENGRUSIS FROM GRUSIS g WHERE g.DENGRUSIS = '" + NomeGruppo + "'";
        DataSet dataSet1 = new DataSet();
        if (!string.IsNullOrEmpty(dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text).ToString()))
          return (string) null;
        string strSQL2 = "SELECT MAX(g.CODGRU)+1 FROM GRUSIS g ";
        DataSet dataSet2 = new DataSet();
        string strSQL3 = "INSERT INTO GRUSIS VALUES (" + Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL2, CommandType.Text)).ToString() + ",'" + NomeGruppo.ToUpper() + "','T',CURRENT TIMESTAMP,'ADMIN')";
        return dataLayer.Get1ValueFromSQL(strSQL3, CommandType.Text).ToString();
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    public List<Admin> TabellaFunzionalita(string selezioneNomeFunz)
    {
      if (selezioneNomeFunz == "")
        selezioneNomeFunz = (string) null;
      if (selezioneNomeFunz == null)
        return (List<Admin>) null;
      DataLayer dataLayer = new DataLayer();
      string strSQL = "SELECT COUNT(p.CODFUNSIS ) AS CONTEGGIO , f.DENFUNSIS FROM PROUTE p FULL JOIN FUNSIS f ON p.CODFUNSIS = f.CODFUNSIS WHERE f.UTEAGG = 'ADMIN'AND f.DENFUNSIS LIKE'%" + selezioneNomeFunz + "%'GROUP BY f.DENFUNSIS";
      DataTable dataTable = new DataTable();
      List<Admin> adminList = new List<Admin>();
      string Err = "";
      DataSet dataSet = new DataSet();
      foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataSet(strSQL, ref Err).Tables[0].Rows)
      {
        Admin admin = new Admin()
        {
          codFun = row["CONTEGGIO"].ToString(),
          denFun = row["DENFUNSIS"].ToString()
        };
        adminList.Add(admin);
      }
      return adminList;
    }

    public string SalvataggioFunzionalita(string newFun, string oldFun)
    {
      try
      {
        if (string.IsNullOrEmpty(newFun))
          return (string) null;
        DataLayer dataLayer = new DataLayer();
        string strSQL1 = "SELECT f.DENFUNSIS FROM FUNSIS f  WHERE f.DENFUNSIS= '" + newFun + "'";
        DataSet dataSet = new DataSet();
        if (!string.IsNullOrEmpty(dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text).ToString()))
          return (string) null;
        string strSQL2 = "UPDATE FUNSIS f SET f.DENFUNSIS ='" + newFun.ToUpper() + "' , f.ULTAGG = CURRENT TIMESTAMP WHERE f.DENFUNSIS ='" + oldFun + "'";
        return dataLayer.Get1ValueFromSQL(strSQL2, CommandType.Text).ToString();
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    public string NewFunzionalita(string nuovaFun)
    {
      try
      {
        DataLayer dataLayer = new DataLayer();
        string strSQL1 = "SELECT f.DENFUNSIS FROM FUNSIS f WHERE f.DENFUNSIS = '" + nuovaFun + "'";
        DataSet dataSet1 = new DataSet();
        if (!string.IsNullOrEmpty(dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text).ToString()))
          return (string) null;
        string strSQL2 = "SELECT MAX(f.CODFUNSIS)+1 AS MAX FROM FUNSIS f";
        DataSet dataSet2 = new DataSet();
        string strSQL3 = "INSERT INTO FUNSIS VALUES ('" + Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL2, CommandType.Text)).ToString() + "' ,'" + nuovaFun + "','I',CURRENT TIMESTAMP,'ADMIN'";
        return dataLayer.Get1ValueFromSQL(strSQL3, CommandType.Text).ToString();
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }

    public List<Admin> TabellaGruppoFunzionalita(string oldGrup)
    {
      try
      {
        DataLayer dataLayer = new DataLayer();
        string strSQL = "SELECT f.DENFUNSIS FROM FUNSIS f,FUNGRU f1,GRUSIS g WHERE f.CODFUNSIS =f1.CODFUNSIS AND f1.CODGRU = g.CODGRU AND g.DENGRUSIS ='" + oldGrup + "'";
        DataTable dataTable = new DataTable();
        Admin admin1 = new Admin();
        List<Admin> adminList = new List<Admin>();
        string Err = "";
        DataSet dataSet = new DataSet();
        foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataSet(strSQL, ref Err).Tables[0].Rows)
        {
          Admin admin2 = new Admin()
          {
            denGru = row["DENFUNSIS"].ToString()
          };
          adminList.Add(admin2);
        }
        return adminList;
      }
      catch (Exception ex)
      {
        return (List<Admin>) null;
      }
    }
  }
}
