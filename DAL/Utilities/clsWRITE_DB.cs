// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Utilities.clsWRITE_DB
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Data;
using TFI.DAL.ConnectorDB;

namespace TFI.DAL.Utilities
{
  public class clsWRITE_DB
  {
    public string TAB_CONTABILE_MOVIMSAP = "MOVIMSAP";
    public string TAB_CONTABILE_SPLIMPOSAP = "SPLIMPOSAP";
    public string TAB_CONTABILE_MOVASSAP = "MOVASSAP";
    public string TAB_CONTABILE_MOVRETSAP = "MOVRETSAP";
    public string TAB_CONTABILE_PARTSAP = "PARTSAP";
    public string TAB_CONTABILE_DTPARTSAP = "DTPARTSAP";
    public string TAB_CONTABILE_ABBINSAP = "ABBINSAP";
    public string TAB_CONTABILE_SPLABSAP = "SPLABSAP";

    public void WRITE_UPDATE_FPCESSATI(
      DataLayer objDataAccess,
      int intMat,
      string strflaglett,
      TFI.OCM.Utente.Utente u)
    {
      string username = u.Username;
      string str = "UPDATE FPCESSATI SET ";
      string strSQL = (!(strflaglett == "N") ? str + " FLGLETT = 'S' , " : str + " FLGLETT = NULL , ") + " UTEAGG = " + DBMethods.DoublePeakForSql(username) + ", " + " ULTAGG = CURRENT_TIMESTAMP " + " WHERE MAT = " + intMat.ToString();
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public string GET_PARTITA(DataLayer objDataAccess, int CODPOS, int ANNO, int MESE, TFI.OCM.Utente.Utente u)
    {
      DataTable dataTable1 = new DataTable();
      string username = u.Username;
      MESE.ToString();
      string strData = objDataAccess.Get1ValueFromSQL("SELECT CURRENT_DATE AS DATASISTEMA FROM DBUNICONET.TIPIND", CommandType.Text);
      string strSQL1;
      if (ANNO < 2003)
        strSQL1 = "SELECT * FROM " + this.TAB_CONTABILE_PARTSAP + " WHERE" + " PARTITA = " + DBMethods.DoublePeakForSql((CODPOS.ToString().Trim().PadLeft(6, '0') + "000000").Trim());
      else
        strSQL1 = "SELECT * FROM " + this.TAB_CONTABILE_PARTSAP + " WHERE" + " CODPOSIZ = " + CODPOS.ToString() + " AND ANNODIST = " + ANNO.ToString() + " AND MESEDIST = " + MESE.ToString().Trim().PadLeft(0, '2');
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL1);
      string partita;
      if (dataTable2.Rows.Count > 0)
      {
        if (dataTable2.Rows[0]["STATO"].ToString() == "C")
        {
          string strSQL2 = " UPDATE " + this.TAB_CONTABILE_PARTSAP + " SET STATO = 'A' WHERE " + " CODPOSIZ = " + CODPOS.ToString() + " AND ANNODIST = " + ANNO.ToString() + " AND MESEDIST = " + DBMethods.DoublePeakForSql(MESE.ToString().Trim().PadLeft(0, '2')) + " AND PARTITA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[0]["PARTITA"].ToString().Trim());
          objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
          partita = dataTable2.Rows[0]["PARTITA"].ToString().Trim();
          string strSQL3 = " UPDATE " + this.TAB_CONTABILE_DTPARTSAP + " SET DATACL = 0 WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[0]["PARTITA"].ToString().Trim());
          objDataAccess.WriteTransactionData(strSQL3, CommandType.Text);
        }
        else
          partita = dataTable2.Rows[0]["PARTITA"].ToString().Trim();
      }
      else
      {
        partita = ANNO >= 2003 ? CODPOS.ToString().Trim().PadLeft(6, '0') + ANNO.ToString().Trim().Substring(2) + MESE.ToString().PadLeft(2, '0') + "01" : CODPOS.ToString().Trim().PadLeft(6, '0') + "000000";
        string strSQL4 = " INSERT INTO " + this.TAB_CONTABILE_PARTSAP + " (CODPOSIZ, ANNODIST, MESEDIST, PARTITA, STATO, USERID)" + " VALUES ( " + CODPOS.ToString() + ", " + ANNO.ToString() + ", " + DBMethods.DoublePeakForSql(MESE.ToString().PadLeft(2, '0')) + ", " + DBMethods.DoublePeakForSql(partita.Trim()) + ", " + " 'A', " + DBMethods.DoublePeakForSql(username) + ")";
        objDataAccess.WriteTransactionData(strSQL4, CommandType.Text);
        string strSQL5 = " INSERT INTO " + this.TAB_CONTABILE_DTPARTSAP + "(PARTITA, DATAAP, DATACL, USERID)" + " VALUES ( " + DBMethods.DoublePeakForSql(partita.Trim()) + ", " + DBMethods.Db2Date(strData).Replace("-", "") + ", " + " 0, " + DBMethods.DoublePeakForSql(username) + ")";
        objDataAccess.WriteTransactionData(strSQL5, CommandType.Text);
      }
      return partita;
    }

    public void WRITE_NOTE(
      DataLayer objDataAccess,
      int CODPOS,
      int MAT,
      int PRORAP,
      int CODSTAPRE,
      string NOTE,
      TFI.OCM.Utente.Utente u)
    {
      string username = u.Username;
      string strData = objDataAccess.Get1ValueFromSQL("SELECT CURRENT_DATE AS DATASISTEMA FROM DBUNICONET.TIPIND", CommandType.Text);
      string strSQL1 = "SELECT VALUE(MAX(PRONOT),0) + 1 FROM MODPRENOT " + " WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString();
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string strSQL2 = "INSERT INTO MODPRENOT (CODPOS, MAT, PRORAP, PRONOT, DATA, NOTE, " + " CODSTAPRE, ULTAGG, UTEAGG) VALUES (" + CODPOS.ToString() + ", " + MAT.ToString() + ", " + PRORAP.ToString() + ", " + int32.ToString() + ", " + DBMethods.Db2Date(strData) + ", " + DBMethods.DoublePeakForSql(NOTE) + ", " + CODSTAPRE.ToString() + " , " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
    }

    public int WRITE_INSERT_DENTES(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      string TIPMOV,
      string TIPDEN,
      string STADEN,
      string DATAPE,
      string UTEAPE,
      string DATCHI,
      string UTECHI,
      string IMPABB,
      string DATSCA,
      string RICSANUTE)
    {
      string username = u.Username;
      string strSQL1 = " SELECT VALUE(MAX(PRODEN), 0) + 1  FROM DENTES " + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString();
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string str1 = " INSERT INTO DENTES (" + "CODPOS , " + "ANNDEN , " + "MESDEN , " + "PRODEN , " + "TIPMOV , " + "TIPDEN , " + "STADEN , " + "DATAPE , " + "UTEAPE , " + "DATCHI , " + "UTECHI , " + "IMPABB , " + "NUMRIGDET , " + "DATSCA , " + "RICSANUTE , " + "FLGAPP," + "ULTAGG, " + "UTEAGG) " + "VALUES (" + CODPOS.ToString() + ", " + ANNDEN.ToString() + ", " + MESDEN.ToString() + ", " + int32.ToString() + ", " + DBMethods.DoublePeakForSql(TIPMOV) + ", " + DBMethods.DoublePeakForSql(TIPDEN) + ", " + DBMethods.DoublePeakForSql(STADEN) + ", ";
      string str2 = (!(DATAPE == "") ? str1 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATAPE.Replace("'", ""))) + ", " : str1 + "NULL, ") + DBMethods.DoublePeakForSql(UTEAPE) + ", ";
      string str3 = (!(DATCHI == "") ? str2 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCHI.Replace("'", ""))) + ", " : str2 + "NULL, ") + DBMethods.DoublePeakForSql(UTECHI) + ", " + IMPABB + ", " + "0, ";
      string strSQL2 = (!(DATSCA == "") ? str3 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATSCA.Replace("'", ""))) + ", " : str3 + "NULL, ") + DBMethods.DoublePeakForSql(RICSANUTE) + ", 'I', " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
      return int32;
    }

    public int WRITE_INSERT_MODPRE(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int CODPOS,
      int MAT,
      int PRORAP,
      int CODSTAPRE,
      string DATAPE,
      string UTEAPE,
      string DATCHI,
      string UTECHI,
      string PREVUFF,
      string PREAVVISO,
      string DATINIPRE,
      string DATSCAPRE,
      int GIOINDSOS,
      Decimal IMPINDSOS,
      string RIFNOME,
      string RIFTEL,
      string NOTE,
      string UTEASS,
      string PRESTITO,
      string CARTAENP)
    {
      string username = u.Username;
      string strSQL1 = " SELECT VALUE(MAX(PROMOD), 0) + 1  FROM MODPRE WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString();
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string str1 = " INSERT INTO MODPRE (" + "CODPOS , " + "MAT , " + "PRORAP , " + "PROMOD , " + "CODSTAPRE , " + "DATAPE , " + "UTEAPE , " + "DATCHI , " + "UTECHI , " + "PREVUFF , " + "PREAVVISO , " + "DATINIPRE , " + "DATSCAPRE , " + "GIOINDSOS , " + "IMPINDSOS , " + "RIFNOME , " + "RIFTEL , " + "NOTE , " + "UTEASS , " + "PRESTITO , " + "CARTAENP , " + "ULTAGG, " + "UTEAGG) " + "VALUES (" + CODPOS.ToString() + ", " + MAT.ToString() + ", " + PRORAP.ToString() + ", " + int32.ToString() + ", " + CODSTAPRE.ToString() + ", ";
      string str2 = !(DATAPE == "") ? str1 + DBMethods.Db2Date(DATAPE.Replace("'", "")) + ", " : str1 + "NULL, ";
      string str3 = !(UTEAPE == "") ? str2 + DBMethods.DoublePeakForSql(UTEAPE) + ", " : str2 + "NULL, ";
      string str4 = !(DATCHI == "") ? str3 + DBMethods.Db2Date(DATCHI.Replace("'", "")) + ", " : str3 + "NULL, ";
      string str5 = (!(UTECHI == "") ? str4 + DBMethods.DoublePeakForSql(UTECHI) + ", " : str4 + "NULL, ") + DBMethods.DoublePeakForSql(PREVUFF) + ", " + DBMethods.DoublePeakForSql(PREAVVISO) + ", ";
      string str6 = !(DATINIPRE == "") ? str5 + DBMethods.Db2Date(DATINIPRE.Replace("'", "")) + ", " : str5 + "NULL, ";
      string str7 = !(DATSCAPRE == "") ? str6 + DBMethods.Db2Date(DATSCAPRE.Replace("'", "")) + ", " : str6 + "NULL, ";
      string str8 = GIOINDSOS != 0 ? str7 + GIOINDSOS.ToString() + ", " : str7 + "NULL, ";
      string str9 = Convert.ToDouble(IMPINDSOS) != 0.0 ? str8 + IMPINDSOS.ToString().Replace(",", ".") + ", " : str8 + "NULL, ";
      string str10 = !(RIFNOME == "") ? str9 + DBMethods.DoublePeakForSql(RIFNOME) + ", " : str9 + "NULL, ";
      string str11 = !(RIFTEL == "") ? str10 + DBMethods.DoublePeakForSql(RIFTEL) + ", " : str10 + "NULL, ";
      string str12 = !(NOTE == "") ? str11 + DBMethods.DoublePeakForSql(NOTE) + ", " : str11 + "NULL, ";
      string str13 = !(UTEASS == "") ? str12 + DBMethods.DoublePeakForSql(UTEASS) + ", " : str12 + "NULL, ";
      string str14 = !(PRESTITO == "") ? str13 + DBMethods.Db2Date(PRESTITO) + ", " : str13 + "NULL, ";
      string strSQL2 = (!(CARTAENP == "") ? str14 + DBMethods.Db2Date(CARTAENP) + ", " : str14 + "NULL, ") + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
      return int32;
    }

    public int WRITE_INSERT_RETTES(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int ANNRET,
      int PRORET,
      string NUMGRURET,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      string TIPANN,
      string TIPIMP,
      string CODCAUSAN,
      int ANNO_BILANCIO)
    {
      string username = u.Username;
      string str = "SELECT PRORETTES  FROM RETTES " + " WHERE ANNRET = " + ANNRET.ToString() + " AND PRORET = " + PRORET.ToString() + " AND NUMGRURET = " + DBMethods.DoublePeakForSql(NUMGRURET) + " AND CODPOS = " + CODPOS.ToString() + " AND TIPANN = " + DBMethods.DoublePeakForSql(TIPANN) + " AND TIPIMP = " + DBMethods.DoublePeakForSql(TIPIMP) + " AND ANNBILMOV = " + ANNO_BILANCIO.ToString();
      if (TIPIMP == "+")
        str = !(CODCAUSAN.Trim() == "") ? str + " AND CODCAUSAN = '" + CODCAUSAN + "'" : str + " AND CODCAUSAN IS NULL";
      string strSQL1 = str + " AND NUMMOV IS NULL";
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      if (int32 == 0)
      {
        string strSQL2 = "SELECT VALUE(MAX(PRORETTES), 0) + 1 FROM RETTES WHERE ANNRET=" + ANNRET.ToString() + " AND PRORET=" + PRORET.ToString();
        int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text));
        string strSQL3 = " INSERT INTO RETTES (" + "ANNRET , " + "PRORET , " + "PRORETTES , " + "NUMGRURET , " + "CODPOS , " + "TIPANN , " + "TIPIMP , " + "CODCAUSAN , " + "ANNBILMOV , " + "ULTAGG , " + "UTEAGG) " + "VALUES (" + ANNRET.ToString() + ", " + PRORET.ToString() + ", " + int32.ToString() + ", " + DBMethods.DoublePeakForSql(NUMGRURET) + ", " + CODPOS.ToString() + ", " + DBMethods.DoublePeakForSql(TIPANN) + ", " + DBMethods.DoublePeakForSql(TIPIMP) + ", " + DBMethods.DoublePeakForSql(CODCAUSAN) + ", " + ANNO_BILANCIO.ToString() + ", " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
        objDataAccess.WriteTransactionData(strSQL3, CommandType.Text);
      }
      return int32;
    }

    public int WRITE_INSERT_DENDET(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int MAT,
      string DAL,
      string AL,
      string FAP,
      string PERFAP,
      Decimal IMPRET,
      Decimal IMPOCC,
      Decimal IMPFIG,
      Decimal IMPABB,
      Decimal IMPASSCON,
      Decimal IMPCON,
      Decimal IMPMIN,
      string PREV,
      int PRORAP,
      int CODCON,
      int PROCON,
      int CODLOC,
      int PROLOC,
      int CODLIV,
      int CODQUACON,
      string DATNAS,
      string ETA65,
      string TIPMOV,
      string DATDEC,
      string DATCES,
      Decimal NUMGGAZI,
      Decimal NUMGGFIG,
      Decimal NUMGGPER,
      Decimal NUMGGDOM,
      Decimal NUMGGSOS,
      Decimal NUMGGCONAZI,
      Decimal IMPSCA,
      Decimal IMPTRAECO,
      string TIPRAP,
      Decimal PERPAR,
      Decimal PERAPP,
      string TIPSPE,
      int CODGRUASS,
      Decimal ALIQUOTA,
      string TIPDEN,
      string DATINISAN,
      string DATFINSAN,
      Decimal TASSO,
      Decimal PRIORITA = 0M,
      Decimal IMPRETDEL = 0M,
      Decimal IMPOCCDEL = 0M,
      Decimal IMPFIGDEL = 0M,
      Decimal IMPCONDEL = 0M,
      Decimal IMPRETPRE = 0M,
      Decimal IMPOCCPRE = 0M,
      Decimal IMPFIGPRE = 0M,
      Decimal IMPSANDETPRE = 0M,
      Decimal IMPCONPRE = 0M,
      Decimal IMPSANDET = 0M,
      string CODCAUSAN = "",
      Decimal IMPABBPRE = 0M,
      Decimal IMPASSCONPRE = 0M,
      Decimal IMPABBDEL = 0M,
      Decimal IMPASSCONDEL = 0M,
      string ANNCOM = "",
      string NUMMOV = "")
    {
      string username = u.Username;
      string strSQL1 = " SELECT VALUE(MAX(PRODENDET), 0) + 1  FROM DENDET " + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND MAT = " + MAT.ToString();
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string str1 = "INSERT INTO DENDET (" + "CODPOS ," + "ANNDEN ," + "MESDEN ," + "PRODEN ," + "MAT , " + "PRODENDET ," + "DAL ," + "AL ," + "FAP ," + "PERFAP ," + "IMPRET ," + "IMPOCC ," + "IMPFIG ," + "IMPABB ," + "IMPASSCON ," + "IMPCON ," + "IMPMIN ," + "PREV ," + "PRORAP ," + "CODCON ," + "PROCON ," + "CODLOC ," + "PROLOC ," + "CODLIV ," + "CODQUACON ," + "DATNAS ," + "ETA65 ," + " TIPMOV, " + " DATDEC, " + " DATCES, " + " NUMGGAZI, " + " NUMGGFIG, " + " NUMGGPER, " + " NUMGGDOM, " + " NUMGGSOS, " + " NUMGGCONAZI, " + " IMPSCA, " + " IMPTRAECO, " + " TIPRAP, " + " PERPAR, " + " PERAPP, " + " TIPSPE, " + " CODGRUASS, " + " ALIQUOTA, " + " TIPDEN, " + " DATINISAN, " + " DATFINSAN, " + " TASSAN, " + " PRIORITA, " + " IMPRETDEL, " + " IMPOCCDEL, " + " IMPFIGDEL, " + " IMPCONDEL, " + " IMPRETPRE, " + " IMPOCCPRE, " + " IMPFIGPRE, " + " IMPSANDETPRE, " + " IMPCONPRE, " + " IMPSANDET, " + " CODCAUSAN, " + " IMPABBPRE, " + " IMPASSCONPRE, " + " IMPABBDEL, " + " IMPASSCONDEL, " + " ANNCOM, FLGAPP, " + "ULTAGG, " + "UTEAGG) " + " VALUES (" + CODPOS.ToString() + ", " + ANNDEN.ToString() + ", " + MESDEN.ToString() + ", " + PRODEN.ToString() + ", " + MAT.ToString() + ", " + int32.ToString() + ", " + DBMethods.Db2Date(DAL.Replace("'", "")) + ", " + DBMethods.Db2Date(AL.Replace("'", "")) + ", " + DBMethods.DoublePeakForSql(FAP) + ", ";
      string str2 = (!(PERFAP.ToString().Trim() != "") ? str1 + " NULL, " : str1 + PERFAP.Replace(",", ".") + ", ") + IMPRET.ToString().Replace(",", ".") + ", " + IMPOCC.ToString().Replace(",", ".") + ", " + IMPFIG.ToString().Replace(",", ".") + ", " + IMPABB.ToString().Replace(",", ".") + ", " + IMPASSCON.ToString().Replace(",", ".") + ", " + IMPCON.ToString().Replace(",", ".") + ", " + IMPMIN.ToString().Replace(",", ".") + ", ";
      string str3 = (!(PREV == "X") ? str2 + DBMethods.DoublePeakForSql(PREV) + ", " : str2 + " 'N', ") + PRORAP.ToString() + ", " + CODCON.ToString() + ", " + PROCON.ToString() + ", " + CODLOC.ToString() + ", " + PROLOC.ToString() + ", " + CODLIV.ToString() + ", " + CODQUACON.ToString() + ", " + DBMethods.Db2Date(DATNAS.Replace("'", "")) + ", " + DBMethods.DoublePeakForSql(ETA65.Substring(0, 1)) + ", " + DBMethods.DoublePeakForSql(TIPMOV.Trim()) + ", ";
      string str4 = !(DATDEC != "") ? str3 + "NULL, " : str3 + DBMethods.Db2Date(DATDEC) + ", ";
      string str5 = (!(DATCES != "") ? str4 + "NULL, " : str4 + DBMethods.Db2Date(DATCES) + ", ") + NUMGGAZI.ToString().Replace(",", ".") + ", " + NUMGGFIG.ToString().Replace(",", ".") + ", " + NUMGGPER.ToString().Replace(",", ".") + ", " + NUMGGDOM.ToString().Replace(",", ".") + ", " + NUMGGSOS.ToString().Replace(",", ".") + ", " + NUMGGCONAZI.ToString().Replace(",", ".") + ", " + IMPSCA.ToString().Replace(",", ".") + ", " + IMPTRAECO.ToString().Replace(",", ".") + ", ";
      string str6 = (!(TIPRAP.ToString().Trim() != "") ? str5 + " NULL, " : str5 + TIPRAP + ", ") + PERPAR.ToString().Replace(",", ".") + ", " + PERAPP.ToString().Replace(",", ".") + ", ";
      string str7 = (!(TIPSPE.ToString().Trim() != "") ? str6 + " NULL, " : str6 + DBMethods.DoublePeakForSql(TIPSPE) + ", ") + CODGRUASS.ToString() + ", " + ALIQUOTA.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(TIPDEN) + ", ";
      string str8 = !(DATINISAN != "") ? str7 + "NULL, " : str7 + DBMethods.Db2Date(DATINISAN) + ", ";
      string str9 = (!(DATFINSAN != "") ? str8 + "NULL, " : str8 + DBMethods.Db2Date(DATFINSAN) + ", ") + TASSO.ToString().Replace(",", ".") + ", " + PRIORITA.ToString().Replace(",", ".") + ", " + IMPRETDEL.ToString().Replace(",", ".") + ", " + IMPOCCDEL.ToString().Replace(",", ".") + ", " + IMPFIGDEL.ToString().Replace(",", ".") + ", " + IMPCONDEL.ToString().Replace(",", ".") + ", " + IMPRETPRE.ToString().Replace(",", ".") + ", " + IMPOCCPRE.ToString().Replace(",", ".") + ", " + IMPFIGPRE.ToString().Replace(",", ".") + ", " + IMPSANDETPRE.ToString().Replace(",", ".") + ", " + IMPCONPRE.ToString().Replace(",", ".") + ", " + IMPSANDET.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(CODCAUSAN) + ", " + IMPABBPRE.ToString().Replace(",", ".") + ", " + IMPASSCONPRE.ToString().Replace(",", ".") + ", " + IMPABBDEL.ToString().Replace(",", ".") + ", " + IMPASSCONDEL.ToString().Replace(",", ".") + ", ";
      string strSQL2 = (!(ANNCOM != "") ? str9 + "NULL, " : str9 + Convert.ToInt32(ANNCOM).ToString() + ", ") + " 'I', CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
      return int32;
    }

    public int WRITE_INSERT_RETTESGRU(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int ANNO,
      ref string NUMGRURET)
    {
      string username = u.Username;
      string strSQL1 = " SELECT VALUE(MAX(PRORET), 0) + 1  FROM RETTESGRU " + " WHERE ANNRET = " + ANNO.ToString();
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      NUMGRURET = ANNO.ToString().Trim() + "/" + int32.ToString().Trim().PadLeft(4, '0');
      string strSQL2 = "INSERT INTO RETTESGRU (" + "ANNRET ," + "PRORET ," + "NUMGRURET, " + "DATGRU , " + "ULTAGG, " + "UTEAGG ) " + "VALUES (" + ANNO.ToString() + ", " + int32.ToString() + ", " + DBMethods.DoublePeakForSql(NUMGRURET) + ", " + "CURRENT_DATE, " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
      return int32;
    }

    public string WRITE_INSERT_MOVIMSAP(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      ref string MSGErrore,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      string CODCAU,
      string DATA,
      int ANNO_BILANCIO,
      string DATSCA,
      Decimal IMPORTO,
      Decimal IMPABB,
      Decimal IMPADD,
      Decimal IMPASSCON,
      string SANZIONE,
      string ANNULLAMENTO,
      string TIPANN,
      ref string PARTITA_MOVIMENTO,
      ref Decimal PROGMOV_MOVIMENTO,
      ref string PARTITA_SANZIONE,
      ref Decimal PROGMOV_SANZIONE,
      DataTable DTRETTIFICHE = null,
      string TIPO_OPERAZIONE = "",
      string TIPO_IMPORTO = "",
      string NUMMOV_ORIGINE = "",
      Decimal IMPADD_RET = 0M)
    {
      iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
      string TIPMOVSAN = "";
      DataTable dataTable = new DataTable();
      string username = u.Username;
      string strData = objDataAccess.Get1ValueFromSQL("SELECT CURRENT_DATE AS DATASISTEMA FROM DBUNICONET.TIPIND", CommandType.Text);
      string str1 = DBMethods.Db2Date(strData).Replace("-", "");
      string str2 = !(TIPO_OPERAZIONE == "DIPA") ? DBMethods.Db2Date(strData).Replace("-", "") : DBMethods.Db2Date(DATSCA).Replace("-", "");
      string str3 = !(ANNULLAMENTO == "S" | TIPO_IMPORTO == "-") ? (!(TIPO_IMPORTO == "+") ? DBMethods.Db2Date(strData).Replace("-", "") : str2) : ANNDEN.ToString() + MESDEN.ToString().Trim().PadLeft(2, '0') + "01";
      string str4;
      DateTime dateTime;
      if (SANZIONE == "S")
        str4 = DBMethods.Db2Date(strData).Replace("-", "");
      else if (TIPO_OPERAZIONE == "DIPA")
        str4 = DBMethods.Db2Date(DATSCA).Replace("-", "");
      else if (ANNULLAMENTO == "S" | TIPO_IMPORTO == "-")
      {
        str4 = "0";
      }
      else
      {
        dateTime = Convert.ToDateTime(strData);
        dateTime = dateTime.AddDays(30.0);
        str4 = DBMethods.Db2Date(dateTime.ToString()).Replace("-", "");
      }
      string str5 = !(TIPANN == "AC") ? "S" : "N";
      if (DTRETTIFICHE != null)
      {
        ANNDEN = Convert.ToInt32(DTRETTIFICHE.Rows[DTRETTIFICHE.Rows.Count - 1][nameof (ANNDEN)]);
        MESDEN = Convert.ToInt32(DTRETTIFICHE.Rows[DTRETTIFICHE.Rows.Count - 1][nameof (MESDEN)]);
      }
      string partita = this.GET_PARTITA(objDataAccess, CODPOS, ANNDEN, MESDEN, u);
      string strSQL1 = " SELECT VALUE(MAX(PROGMOV), 0) + 1  FROM " + this.TAB_CONTABILE_MOVIMSAP + " WHERE PARTITA = '" + partita + "'";
      int int32_1 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string str6 = ANNO_BILANCIO < ANNDEN ? ANNDEN.ToString() : ANNO_BILANCIO.ToString();
      dateTime = Convert.ToDateTime(strData);
      string str7 = dateTime.Year.ToString();
      string strSQL2 = " SELECT VALUE(MAX(NUMERORIF), 0) + 1  FROM " + this.TAB_CONTABILE_MOVIMSAP + " WHERE CODPOSIZ = " + CODPOS.ToString() + " AND ANNORIF = " + str7 + " AND CODCAUS = " + DBMethods.DoublePeakForSql(CODCAU);
      int int32_2 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text));
      string numeroSapMovimento = this.Module_Get_NumeroSapMovimento(int32_2, CODCAU, Convert.ToInt32(str7));
      string str8 = CODPOS.ToString().PadLeft(6, '0') + numeroSapMovimento.Substring(5, 2) + numeroSapMovimento.Substring(0, 2) + numeroSapMovimento.Substring(8).PadLeft(3, '0');
      if (SANZIONE == "S")
      {
        PARTITA_SANZIONE = partita;
        PROGMOV_SANZIONE = (Decimal) int32_1;
      }
      else
      {
        PARTITA_MOVIMENTO = partita;
        PROGMOV_MOVIMENTO = (Decimal) int32_1;
      }
      string str9 = "INSERT INTO " + this.TAB_CONTABILE_MOVIMSAP + "(" + " PARTITA, " + " PROGMOV, " + " CODCAUS, " + " DATAMOV, " + " DATAREG, " + " IMPORTO, " + " DATACOM, " + " DATASAP, " + " DATASCAD, " + " ANNOESER, " + " NOTE, " + " DECODMOV, " + " CODPOSIZ, " + " ANNORIF, " + " NUMERORIF, " + " PROGRIF, " + " TIPORIF, " + " POSTRASF, " + " VERSTRASF, " + " STATOVAL, " + " STATOSAP, " + " IMPRIDUZ, " + " IMPABB, " + " IMPRESID, " + " CODELMAV, " + " DTHHMAV, " + " FLAGGRP, " + " ANNOGRP, " + " NUMGRP, " + " USERID, " + " DTDECOD, " + " STATOSED, " + " STATOABB ) " + " VALUES (" + "'" + partita + "', " + int32_1.ToString() + ", " + DBMethods.DoublePeakForSql(CODCAU.Trim().PadLeft(2, '0')) + ", ";
      Decimal num = Convert.ToDecimal(str2.Replace("'", ""));
      string str10 = num.ToString();
      string str11 = str9 + str10 + ", ";
      num = Convert.ToDecimal(str1.Replace("'", ""));
      string str12 = num.ToString();
      string str13 = str11 + str12 + ", " + IMPORTO.ToString().Replace(",", ".") + ", ";
      num = Convert.ToDecimal(str3.Replace("'", ""));
      string str14 = num.ToString();
      string str15 = str13 + str14 + ", " + " 0, ";
      num = Convert.ToDecimal(str4.Replace("'", ""));
      string str16 = num.ToString();
      string str17 = str15 + str16 + ", " + str6 + ", " + " '', ";
      string str18 = objDataAccess.Get1ValueFromSQL("SELECT DESCAU FROM TIPMOVCAU WHERE CODCAU = '" + CODCAU.Trim().PadLeft(2, '0') + "'", CommandType.Text).ToString().Trim() + " (" + DBMethods.GetMesi()[MESDEN] + " " + ANNDEN.ToString() + ")";
      if (str18.Length > 50)
        str18 = str18.Substring(0, 50);
      string str19 = str17 + DBMethods.DoublePeakForSql(str18) + ", " + CODPOS.ToString() + ", " + str7 + ", " + int32_2.ToString() + ", " + " 0, " + DBMethods.DoublePeakForSql(CODCAU.Trim().PadLeft(2, '0')) + ", " + " 0, " + " '', " + " 'V'," + " ''," + " 0, " + " 0, " + IMPORTO.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(str8) + ", " + " 0," + " 'N'," + " 0, " + " 0, " + DBMethods.DoublePeakForSql(username) + ", ";
      string str20 = DBMethods.Db2Date(strData).Replace("-", "");
      string str21 = str19;
      num = Convert.ToDecimal(str20.Replace("'", ""));
      string str22 = num.ToString();
      string strSQL3 = str21 + str22 + ", " + " 'V'," + "''" + ")";
      objDataAccess.WriteTransactionData(strSQL3, CommandType.Text);
      switch (TIPO_OPERAZIONE)
      {
        case "ANNULLAMENTO ARRETRATI":
          this.SPLITTA_ANNULLAMENTO_ARRETRATI(objDataAccess, u, numeroSapMovimento, partita, int32_1, CODPOS, ANNDEN, MESDEN, PRODEN, DATA, ANNO_BILANCIO, IMPORTO, IMPADD);
          return numeroSapMovimento;
        case "ANNULLAMENTO DIPA":
        case "ANNULLAMENTO NOTIFICHE":
          this.SPLITTA_NOTIFICHE(objDataAccess, u, numeroSapMovimento, partita, int32_1, CODPOS, ANNDEN, MESDEN, PRODEN, DATA, ANNO_BILANCIO, IMPORTO, IMPADD, IMPABB, IMPASSCON, ANNULLAMENTO, NUMMOV_ORIGINE.Trim(), IMPADD_RET);
          return numeroSapMovimento;
        case "ANNULLAMENTO RETTIFICHE":
          this.SPLITTA_RETTIFICHE(objDataAccess, u, numeroSapMovimento, partita, int32_1, CODPOS, ANNDEN, MESDEN, PRODEN, DATA, ANNO_BILANCIO, IMPORTO, IMPADD, IMPABB, IMPASSCON, "S", ref DTRETTIFICHE, TIPANN);
          return numeroSapMovimento;
        case "ANNULLAMENTO SANZIONI NOTIFICHE DIPA":
          this.SPLITTA_SANZIONI_NOTIFICHE_DIPA(objDataAccess, u, numeroSapMovimento, partita, int32_1, ANNDEN, DATA, ANNO_BILANCIO, IMPORTO, TIPMOVSAN, CODCAU, "S");
          return numeroSapMovimento;
        case "ANNULLAMENTO SANZIONI RETTIFICHE":
          this.SPLITTA_SANZIONI_RETTIFICHE(objDataAccess, u, numeroSapMovimento, partita, int32_1, ANNDEN, DATA, ANNO_BILANCIO, IMPORTO, TIPMOVSAN, CODCAU, TIPANN, "S");
          return numeroSapMovimento;
        case "DIPA":
        case "NOTIFICHE":
          this.SPLITTA_NOTIFICHE(objDataAccess, u, numeroSapMovimento, partita, int32_1, CODPOS, ANNDEN, MESDEN, PRODEN, DATA, ANNO_BILANCIO, IMPORTO, IMPADD, IMPABB, IMPASSCON, ANNULLAMENTO);
          return numeroSapMovimento;
        case "RETTIFICHE":
          this.SPLITTA_RETTIFICHE(objDataAccess, u, numeroSapMovimento, partita, int32_1, CODPOS, ANNDEN, MESDEN, PRODEN, DATA, ANNO_BILANCIO, IMPORTO, IMPADD, IMPABB, IMPASSCON, "N", ref DTRETTIFICHE, TIPANN);
          return numeroSapMovimento;
        case "SANZIONI NOTIFICHE":
          this.SPLITTA_SANZIONI_NOTIFICHE_DIPA(objDataAccess, u, numeroSapMovimento, partita, int32_1, ANNDEN, DATA, ANNO_BILANCIO, IMPORTO, TIPMOVSAN, CODCAU);
          return numeroSapMovimento;
        case "SANZIONI RETTIFICHE":
          this.SPLITTA_SANZIONI_RETTIFICHE(objDataAccess, u, numeroSapMovimento, partita, int32_1, ANNDEN, DATA, ANNO_BILANCIO, IMPORTO, TIPMOVSAN, CODCAU, TIPANN);
          return numeroSapMovimento;
        default:
          MSGErrore = "NON GESTITO";
          return numeroSapMovimento;
      }
    }

    public void SPLITTA_ANNULLAMENTO_ARRETRATI(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      string NUMERO_MOVIMENTO_SAP,
      string PARTITA,
      int PROGMOV,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      string DATA,
      int ANNO_BILANCIO,
      Decimal IMPORTO,
      Decimal IMPADD)
    {
      DataTable dataTable1 = new DataTable();
      string username = u.Username;
      Convert.ToDecimal(this.Module_GetValorePargen(objDataAccess, 5, this.Module_GetDataSistema(objDataAccess), CODPOS));
      int num1 = !(IMPORTO * -1M < 0M) ? -1 : 1;
      Decimal num2 = IMPORTO * -1M;
      this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "TOTDIS", "", IMPORTO * -1M, "", "");
      string strSQL1 = "SELECT ROUND(SUM(DENDETALI.IMPCON), 2) AS IMPORTO, ANNCOM, " + " (SELECT CATFORASS FROM FORASS WHERE CODFORASS = DENDETALI.CODFORASS) AS CATFORASS " + " FROM DENDETALI, DENDET " + " WHERE DENDETALI.CODPOS = DENDET.CODPOS " + " AND DENDETALI.ANNDEN = DENDET.ANNDEN" + " AND DENDETALI.MESDEN = DENDET.MESDEN" + " AND DENDETALI.PRODEN = DENDET.PRODEN" + " AND DENDETALI.MAT = DENDET.MAT" + " AND DENDETALI.PRODENDET = DENDET.PRODENDET" + " AND DENDETALI.CODPOS=" + CODPOS.ToString() + " AND DENDETALI.ANNDEN=" + ANNDEN.ToString() + " AND DENDETALI.MESDEN=" + MESDEN.ToString() + " AND DENDETALI.PRODEN=" + PRODEN.ToString() + " AND DENDET.NUMMOV IS NOT NULL" + " AND DENDET.NUMMOVANN IS NULL" + " GROUP BY ANNCOM, (SELECT CATFORASS FROM FORASS WHERE CODFORASS = DENDETALI.CODFORASS) " + " HAVING SUM(DENDETALI.IMPCON) <> 0";
      dataTable1.Clear();
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL1);
      for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
      {
        string ANNO_PRECEDENTE = !(this.Module_GetTipoAnno(Convert.ToInt32(dataTable2.Rows[index]["ANNCOM"]), ANNO_BILANCIO) == "AC") ? "S" : "N";
        num2 += Convert.ToDecimal(Convert.ToInt32(dataTable2.Rows[index][nameof (IMPORTO)]) * num1);
        this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "", dataTable2.Rows[index]["CATFORASS"].ToString(), (Decimal) (Convert.ToInt32(dataTable2.Rows[index][nameof (IMPORTO)]) * num1), "S", ANNO_PRECEDENTE);
      }
      string strSQL2 = " SELECT ANNCOM,  ROUND(SUM(IMPORTO),2) AS IMPCONTRIBUTO FROM" + " (SELECT ANNCOM, CASE TIPMOV" + " WHEN 'RT' THEN (IMPCONDEL) * 4 / 100" + " ELSE (IMPCON * 4) / 100 END AS IMPORTO FROM DENDET" + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND TIPMOV IN ('AR', 'RT')" + " AND NUMMOV IS NOT NULL" + " AND NUMMOVANN IS NULL" + " ) AS TABELLA" + " GROUP BY ANNCOM";
      DataTable dataTable3 = objDataAccess.GetDataTable(strSQL2);
      for (int index = 0; index <= dataTable3.Rows.Count - 1; ++index)
      {
        string tipoAnno = this.Module_GetTipoAnno(Convert.ToInt32(dataTable3.Rows[index]["ANNCOM"]), ANNO_BILANCIO);
        if (IMPADD != 0M)
        {
          string ANNO_PRECEDENTE = !(tipoAnno == "AC") ? "S" : "N";
          IMPADD = (Decimal) Convert.ToInt32(dataTable3.Rows[index]["IMPCONTRIBUTO"]);
          num2 += IMPADD * (Decimal) num1;
          this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "ADDIZIONALE", "", IMPADD * (Decimal) num1, "S", ANNO_PRECEDENTE);
        }
        string strSQL3 = "UPDATE DENDET SET " + " DATMOVANN = CURRENT_DATE, " + " NUMMOVANN = " + DBMethods.DoublePeakForSql(NUMERO_MOVIMENTO_SAP) + ", " + " BILMOVANN = " + ANNO_BILANCIO.ToString() + ", " + " TIPANNMOVARRANN = " + DBMethods.DoublePeakForSql(tipoAnno) + ", " + " UTEAGG = " + DBMethods.DoublePeakForSql(username) + ", " + " ULTAGG = CURRENT_TIMESTAMP" + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND ANNCOM = " + dataTable3.Rows[index]["ANNCOM"]?.ToString() + " AND TIPMOV = 'AR' ";
        objDataAccess.WriteTransactionData(strSQL3, CommandType.Text);
      }
      if (num2 != 0M)
        throw new Exception("Attenzione... per la Posizione = " + CODPOS.ToString() + ", Anno = " + ANNDEN.ToString() + ", Mese = " + MESDEN.ToString() + " e PRODEN = " + PRODEN.ToString() + " l'importo totale non corrisponde per € " + num2.ToString() + " allo splittamento in contabilità! La transazione sarà annullata");
    }

    public void SPLITTA_NOTIFICHE(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      string NUMERO_MOVIMENTO_SAP,
      string PARTITA,
      int PROGMOV,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      string DATA,
      int ANNO_BILANCIO,
      Decimal IMPORTO,
      Decimal IMPADD,
      Decimal IMPABB,
      Decimal IMPASSCON,
      string ANNULLAMENTO = "N",
      string NUMERO_MOVIMENTO_ORIGINE = "",
      Decimal IMPADD_RET = 0M)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      int num1 = !(IMPORTO * -1M < 0M) ? -1 : 1;
      if (ANNULLAMENTO != "S")
        ANNULLAMENTO = "N";
      string ANNO_PRECEDENTE = !(this.Module_GetTipoAnno(ANNDEN, ANNO_BILANCIO) == "AC") ? "S" : "N";
      Decimal num2 = IMPORTO * -1M;
      this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "TOTDIS", "", IMPORTO * -1M, "", "");
      if (IMPABB != 0M)
      {
        num2 += IMPABB * (Decimal) num1;
        this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "ABBPRE", "", IMPABB * (Decimal) num1, ANNULLAMENTO, ANNO_PRECEDENTE);
      }
      if (IMPADD != 0M)
      {
        num2 += IMPADD * (Decimal) num1;
        this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "ADDIZIONALE", "", IMPADD * (Decimal) num1, ANNULLAMENTO, ANNO_PRECEDENTE);
      }
      if (IMPASSCON != 0M)
      {
        num2 += IMPASSCON * (Decimal) num1;
        this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "ASSCON", "", IMPASSCON * (Decimal) num1, ANNULLAMENTO, ANNO_PRECEDENTE);
      }
      string str = "SELECT ROUND(SUM(DENDETALI.IMPCON), 2) AS IMPORTO, " + " (SELECT CATFORASS FROM FORASS WHERE CODFORASS = DENDETALI.CODFORASS) AS CATFORASS FROM DENDETALI, DENDET " + " WHERE DENDETALI.CODPOS = DENDET.CODPOS " + " AND DENDETALI.ANNDEN = DENDET.ANNDEN" + " AND DENDETALI.MESDEN = DENDET.MESDEN" + " AND DENDETALI.PRODEN = DENDET.PRODEN" + " AND DENDETALI.MAT = DENDET.MAT" + " AND DENDETALI.PRODENDET = DENDET.PRODENDET" + " AND DENDETALI.CODPOS=" + CODPOS.ToString() + " AND DENDETALI.ANNDEN=" + ANNDEN.ToString() + " AND DENDETALI.MESDEN=" + MESDEN.ToString();
      string strSQL1 = (!(NUMERO_MOVIMENTO_ORIGINE == "") ? str + " AND DENDET.NUMMOV = " + DBMethods.DoublePeakForSql(NUMERO_MOVIMENTO_ORIGINE) : str + " AND DENDETALI.PRODEN=" + PRODEN.ToString()) + " GROUP BY (SELECT CATFORASS FROM FORASS WHERE CODFORASS = DENDETALI.CODFORASS)";
      DataTable dataTable3 = objDataAccess.GetDataTable(strSQL1);
      for (int index1 = 0; index1 <= dataTable3.Rows.Count - 1; ++index1)
      {
        if (NUMERO_MOVIMENTO_ORIGINE != "")
        {
          string strSQL2 = "SELECT ROUND(SUM(DENDETALI.IMPCON), 2) AS IMPORTO, " + " (SELECT CATFORASS FROM FORASS WHERE CODFORASS = DENDETALI.CODFORASS) AS CATFORASS FROM DENDETALI, DENDET " + " WHERE DENDETALI.CODPOS = DENDET.CODPOS " + " AND DENDETALI.ANNDEN = DENDET.ANNDEN" + " AND DENDETALI.MESDEN = DENDET.MESDEN" + " AND DENDETALI.PRODEN = DENDET.PRODEN" + " AND DENDET.NUMMOV IS NOT NULL AND NUMMOVANN IS NULL" + " AND DENDETALI.MAT = DENDET.MAT" + " AND DENDETALI.PRODENDET = DENDET.PRODENDET" + " AND DENDET.TIPMOV = 'RT'" + " AND DENDETALI.CODPOS=" + CODPOS.ToString() + " AND DENDETALI.ANNDEN=" + ANNDEN.ToString() + " AND DENDETALI.MESDEN=" + MESDEN.ToString() + " GROUP BY (SELECT CATFORASS FROM FORASS WHERE CODFORASS = DENDETALI.CODFORASS)";
          DataTable dataTable4 = objDataAccess.GetDataTable(strSQL2);
          if (dataTable4.Rows.Count == 0)
          {
            if (Convert.ToDecimal(dataTable3.Rows[index1][nameof (IMPORTO)]) != 0M)
            {
              num2 += Convert.ToDecimal(Convert.ToDecimal(dataTable3.Rows[index1][nameof (IMPORTO)]) * (Decimal) num1);
              this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "", dataTable3.Rows[index1]["CATFORASS"].ToString(), Convert.ToDecimal(dataTable3.Rows[index1][nameof (IMPORTO)]) * (Decimal) num1, ANNULLAMENTO, ANNO_PRECEDENTE);
            }
          }
          else
          {
            for (int index2 = 0; index2 <= dataTable4.Rows.Count - 1; ++index2)
            {
              if (dataTable3.Rows[index1]["CATFORASS"].ToString() == dataTable4.Rows[index2]["CATFORASS"].ToString() && Convert.ToDecimal(dataTable3.Rows[index1][nameof (IMPORTO)]) != 0M)
              {
                dataTable3.Rows[index1][nameof (IMPORTO)] = (object) (Convert.ToDecimal(dataTable3.Rows[index1][nameof (IMPORTO)]) + Convert.ToDecimal(dataTable4.Rows[index2][nameof (IMPORTO)]));
                num2 += Convert.ToDecimal(Convert.ToDecimal(dataTable3.Rows[index1][nameof (IMPORTO)]) * (Decimal) num1);
                this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "", dataTable3.Rows[index1]["CATFORASS"].ToString(), Convert.ToDecimal(dataTable3.Rows[index1][nameof (IMPORTO)]) * (Decimal) num1, ANNULLAMENTO, ANNO_PRECEDENTE);
              }
            }
          }
        }
        else if (Convert.ToInt32(dataTable3.Rows[index1][nameof (IMPORTO)]) != 0)
        {
          num2 += Convert.ToDecimal(dataTable3.Rows[index1][nameof (IMPORTO)]) * (Decimal) num1;
          this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "", dataTable3.Rows[index1]["CATFORASS"].ToString(), Convert.ToDecimal(dataTable3.Rows[index1][nameof (IMPORTO)]) * (Decimal) num1, ANNULLAMENTO, ANNO_PRECEDENTE);
        }
      }
      if (NUMERO_MOVIMENTO_ORIGINE != "" && IMPADD_RET != 0M)
        num2 += IMPADD_RET * (Decimal) num1;
      if (num2 != 0M)
        throw new Exception("Attenzione... per la Posizione = " + CODPOS.ToString() + ", Anno = " + ANNDEN.ToString() + ", Mese = " + MESDEN.ToString() + " e PRODEN = " + PRODEN.ToString() + " l'importo totale non corrisponde per € " + num2.ToString() + " allo splittamento in contabilità! La transazione sarà annullata");
    }

    public void SPLITTA_RETTIFICHE(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      string NUMERO_MOVIMENTO_SAP,
      string PARTITA,
      int PROGMOV,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      string DATA,
      int ANNO_BILANCIO,
      Decimal IMPORTO,
      Decimal IMPADD,
      Decimal IMPABB,
      Decimal IMPASSCON,
      string ANNULLAMENTO,
      ref DataTable DTRETTIFICHE,
      string TIPANN)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataColumn column = new DataColumn();
      if (ANNULLAMENTO != "S")
        ANNULLAMENTO = "N";
      int num1 = !(IMPORTO > 0M) ? (!(ANNULLAMENTO != "S") ? -1 : 1) : (!(ANNULLAMENTO != "S") ? -1 : 1);
      string ANNO_PRECEDENTE = !(TIPANN == "AC") ? "S" : "N";
      Decimal num2 = IMPORTO * -1M;
      this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "TOTDIS", "", IMPORTO * -1M, "", "");
      if (IMPABB != 0M)
      {
        num2 += IMPABB * (Decimal) num1;
        this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "ABBPRE", "", IMPABB * (Decimal) num1, ANNULLAMENTO, ANNO_PRECEDENTE);
      }
      if (IMPADD != 0M)
      {
        num2 += IMPADD * (Decimal) num1;
        this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "ADDIZIONALE", "", IMPADD * (Decimal) num1, ANNULLAMENTO, ANNO_PRECEDENTE);
      }
      if (IMPASSCON != 0M)
      {
        num2 += IMPASSCON * (Decimal) num1;
        this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "ASSCON", "", IMPASSCON * (Decimal) num1, ANNULLAMENTO, ANNO_PRECEDENTE);
      }
      column.ColumnName = nameof (IMPORTO);
      column.DataType = Type.GetType("System.Decimal");
      dataTable2.Columns.Add(column);
      dataTable2.Columns.Add(new DataColumn()
      {
        ColumnName = "CATFORASS",
        DataType = Type.GetType("System.String")
      });
      for (int index1 = 0; index1 <= DTRETTIFICHE.Rows.Count - 1; ++index1)
      {
        string strSQL = "SELECT SUM(IMPCON) AS IMPORTO, (SELECT CATFORASS FROM FORASS WHERE CODFORASS = DENDETALI.CODFORASS) AS CATFORASS FROM DENDETALI " + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + DTRETTIFICHE.Rows[index1][nameof (ANNDEN)]?.ToString() + " AND MESDEN = " + DTRETTIFICHE.Rows[index1][nameof (MESDEN)]?.ToString() + " AND PRODEN = " + DTRETTIFICHE.Rows[index1][nameof (PRODEN)]?.ToString() + " AND MAT = " + DTRETTIFICHE.Rows[index1]["MAT"]?.ToString() + " AND PRODENDET = " + DTRETTIFICHE.Rows[index1]["PRODENDET"]?.ToString() + " GROUP BY (SELECT CATFORASS FROM FORASS WHERE CODFORASS = DENDETALI.CODFORASS)";
        dataTable1.Rows.Clear();
        dataTable1 = objDataAccess.GetDataTable(strSQL);
        for (int index2 = 0; index2 <= dataTable1.Rows.Count - 1; ++index2)
        {
          int index3 = 0;
          while (index3 <= dataTable2.Rows.Count - 1 && !(dataTable2.Rows[index3]["CATFORASS"].ToString().Trim() == dataTable1.Rows[index2]["CATFORASS"].ToString().Trim()))
            ++index3;
          if (index3 > dataTable2.Rows.Count - 1)
          {
            DataRow row = dataTable2.NewRow();
            row["CATFORASS"] = dataTable1.Rows[index2]["CATFORASS"];
            row[nameof (IMPORTO)] = dataTable1.Rows[index2][nameof (IMPORTO)];
            dataTable2.Rows.Add(row);
          }
          else
            dataTable2.Rows[index3][nameof (IMPORTO)] = (object) (Convert.ToDecimal(dataTable2.Rows[index3][nameof (IMPORTO)]) + Convert.ToDecimal(dataTable1.Rows[index2][nameof (IMPORTO)]));
        }
      }
      for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
      {
        if (Convert.ToInt32(dataTable2.Rows[index][nameof (IMPORTO)]) != 0)
        {
          num2 += Convert.ToDecimal(Convert.ToInt32(dataTable2.Rows[index][nameof (IMPORTO)]) * num1);
          this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "", dataTable2.Rows[index]["CATFORASS"].ToString(), (Decimal) (Convert.ToInt32(dataTable2.Rows[index][nameof (IMPORTO)]) * num1), ANNULLAMENTO, ANNO_PRECEDENTE);
        }
      }
      if (num2 != 0M)
        throw new Exception("Attenzione... per la Posizione = " + CODPOS.ToString() + ", Anno = " + ANNDEN.ToString() + ", Mese = " + MESDEN.ToString() + " e PRODEN = " + PRODEN.ToString() + " l'importo totale non corrisponde per € " + num2.ToString() + " allo splittamento in contabilità! La transazione sarà annullata");
    }

    public void SPLITTA_SANZIONI_RETTIFICHE(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      string NUMERO_MOVIMENTO_SAP,
      string PARTITA,
      int PROGMOV,
      int ANNDEN,
      string DATA,
      int ANNO_BILANCIO,
      Decimal IMPORTO,
      string TIPMOVSAN,
      string CODCAU,
      string TIPANN,
      string ANNULLAMENTO = "N")
    {
      int num = !(IMPORTO * -1M < 0M) ? -1 : 1;
      if (ANNULLAMENTO != "S")
        ANNULLAMENTO = "N";
      string ANNO_PRECEDENTE = !(TIPANN == "AC") ? "S" : "N";
      this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "TOTSAN", "", IMPORTO * -1M, "", "");
      TIPMOVSAN = objDataAccess.Get1ValueFromSQL("SELECT TIPMOV FROM TIPMOVCAU WHERE CODCAU = " + DBMethods.DoublePeakForSql(CODCAU), CommandType.Text).ToString().Trim();
      this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, TIPMOVSAN, "", IMPORTO, ANNULLAMENTO, ANNO_PRECEDENTE);
    }

    public void SPLITTA_SANZIONI_NOTIFICHE_DIPA(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      string NUMERO_MOVIMENTO_SAP,
      string PARTITA,
      int PROGMOV,
      int ANNDEN,
      string DATA,
      int ANNO_BILANCIO,
      Decimal IMPORTO,
      string TIPMOVSAN,
      string CODCAU,
      string ANNULLAMENTO = "N")
    {
      int num = !(IMPORTO * -1M < 0M) ? -1 : 1;
      if (ANNULLAMENTO != "S")
        ANNULLAMENTO = "N";
      string ANNO_PRECEDENTE = !(this.Module_GetTipoAnno(ANNDEN, ANNO_BILANCIO) == "AC") ? "S" : "N";
      this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, "TOTSAN", "", IMPORTO * -1M, "", "");
      TIPMOVSAN = objDataAccess.Get1ValueFromSQL("SELECT TIPMOV FROM TIPMOVCAU WHERE CODCAU = " + DBMethods.DoublePeakForSql(CODCAU), CommandType.Text).ToString().Trim();
      this.WRITE_INSERT_SPLIMPOSAP(objDataAccess, u, PARTITA, PROGMOV, DATA, TIPMOVSAN, "", IMPORTO, ANNULLAMENTO, ANNO_PRECEDENTE);
    }

    public void WRITE_INSERT_SPLIMPOSAP(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      string PARTITA,
      int PROGMOV,
      string DATA,
      string CATIMP,
      string CATFORASS,
      Decimal IMPORTO,
      string ANNULLAMENTO,
      string ANNO_PRECEDENTE)
    {
      DataTable dataTable1 = new DataTable();
      iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
      string username = u.Username;
      string strSQL1 = "SELECT VALUE(MAX(PROGSPLI), 0) + 1 AS PROGSPLI " + "FROM " + this.TAB_CONTABILE_SPLIMPOSAP + " WHERE PARTITA = '" + PARTITA + "'" + " AND PROGMOV = " + PROGMOV.ToString();
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string strSQL2 = "SELECT CODCON, GESCON, GESCON1 FROM CONTI WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATA)) + " BETWEEN DATINI AND DATFIN";
      if (ANNULLAMENTO != "")
        strSQL2 = !(IMPORTO < 0M) ? strSQL2 + " AND MOVANN = " + DBMethods.DoublePeakForSql(ANNULLAMENTO) : strSQL2 + " AND MOVANN = 'S' ";
      if (CATIMP != "")
        strSQL2 = strSQL2 + " AND CATIMP = " + DBMethods.DoublePeakForSql(CATIMP);
      if (CATFORASS != "")
        strSQL2 = strSQL2 + " AND CATFORASS = " + DBMethods.DoublePeakForSql(CATFORASS);
      if (ANNO_PRECEDENTE != "")
        strSQL2 = strSQL2 + " AND ANNPRE = " + DBMethods.DoublePeakForSql(ANNO_PRECEDENTE);
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL2);
      if (dataTable2.Rows.Count == 0)
        throw new Exception("Attenzione... Impossibile trovare il conto, per lo splittamento, nella tabella CONTI! La transazione sarà annullata. Analizzare la query: " + strSQL2);
      string str1 = dataTable2.Rows[0]["CODCON"].ToString().Trim();
      string str2 = dataTable2.Rows[0]["GESCON"].ToString().Trim();
      string str3 = dataTable2.Rows[0]["GESCON1"].ToString().Trim();
      string str4 = "INSERT INTO " + this.TAB_CONTABILE_SPLIMPOSAP + " (PARTITA, PROGMOV, PROGSPLI, " + " CODCONTO, GESTCON, " + " IMPORTO, IMPRIDU, IMPABB, IMPRES, STATOSAP, USERID, GESTCON1, FLSAP) " + " VALUES (" + "'" + PARTITA + "', " + PROGMOV.ToString() + ", " + int32.ToString() + ", " + DBMethods.DoublePeakForSql(str1) + ", " + DBMethods.DoublePeakForSql(str2) + ", " + IMPORTO.ToString().Trim().Replace(",", ".") + ", " + "0, " + "0, " + IMPORTO.ToString().Trim().Replace(",", ".") + ", " + " '', " + DBMethods.DoublePeakForSql(username) + ", ";
      string str5 = !(str3 == "") ? str4 + DBMethods.DoublePeakForSql(str3) + ", " : str4 + "'', ";
      string strSQL3 = !(str2 == "CAZA" | str2 == "CSAA") ? str5 + "'N')" : str5 + "'S')";
      objDataAccess.WriteTransactionData(strSQL3, CommandType.Text);
    }

    public void WRITE_INSERT_IMPAGG(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int intCodPos,
      int intMat,
      int intProRap,
      string strDatInizio,
      string strMensilita,
      Decimal decExtra)
    {
      string username = u.Username;
      if (decExtra == 0M)
        return;
      string strSQL1 = " SELECT VALUE(MAX(PROIMP),0) + 1  FROM IMPAGG WHERE" + " CODPOS = " + intCodPos.ToString() + " AND MAT = " + intMat.ToString() + " AND PRORAP = " + intProRap.ToString() + " AND DATINI = '" + DBMethods.Db2Date(strDatInizio.Replace("'", "")) + "' ";
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string strSQL2 = " INSERT INTO IMPAGG (CODPOS,MAT,PRORAP,PROIMP,DATINI," + " MENAGG,IMPAGG,ULTAGG,UTEAGG)" + " VALUES (" + intCodPos.ToString() + "," + intMat.ToString() + "," + intProRap.ToString() + "," + int32.ToString() + ",'" + DBMethods.Db2Date(strDatInizio.Replace("'", "")) + "'," + strMensilita + ",'" + decExtra.ToString().Replace(",", ".") + "'," + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
    }

    public void WRITE_INSERT_MODRDL(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int intCodPos,
      int intMat,
      int intProRap,
      int intMese,
      int intAnno,
      int TIPPRIRET,
      string DATA_DEC)
    {
      DataTable dataTable1 = new DataTable();
      bool flag = false;
      string username = u.Username;
      string strSQL1 = "SELECT ANNDA, MESDA FROM MODRDL" + " WHERE CODPOS = " + intCodPos.ToString() + " AND MAT = " + intMat.ToString() + " AND PRORAP = " + intProRap.ToString() + " AND DATELA IS NULL AND DATANN IS NULL";
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL1);
      if (dataTable2.Rows.Count > 0)
      {
        if (Convert.ToInt32(dataTable2.Rows[0]["ANNDA"].ToString().Trim()) + Convert.ToInt32(dataTable2.Rows[0]["MESDA"].ToString().Trim().PadLeft(2, '0')) > Convert.ToInt32(intAnno.ToString().Trim()) + Convert.ToInt32(intMese.ToString().Trim().PadLeft(2, '0')))
        {
          string strSQL2 = "UPDATE MODRDL SET ANNDA = " + intAnno.ToString() + ", " + " MESDA = " + intMese.ToString() + ", " + " DATDEC = " + DBMethods.Db2Date(DATA_DEC) + ", " + " TIPPRIRET = " + TIPPRIRET.ToString() + " WHERE CODPOS = " + intCodPos.ToString() + " AND MAT = " + intMat.ToString() + " AND PRORAP = " + intProRap.ToString() + " AND DATELA IS NULL ";
          objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
          flag = false;
        }
      }
      else
        flag = true;
      if (!flag)
        return;
      string strSQL3 = "SELECT VALUE(MAX(PROREC),0) + 1  FROM MODRDL WHERE" + " CODPOS = " + intCodPos.ToString() + " AND MAT = " + intMat.ToString() + " AND PRORAP = " + intProRap.ToString();
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL3, CommandType.Text));
      string strSQL4 = "INSERT INTO MODRDL (CODPOS,MAT,PRORAP,PROREC," + " ANNDA, MESDA, DATELA, DATDEC, TIPPRIRET, ULTAGG, UTEAGG) " + " VALUES (" + intCodPos.ToString() + "," + intMat.ToString() + "," + intProRap.ToString() + "," + int32.ToString() + "," + intAnno.ToString() + "," + intMese.ToString() + "," + "NULL," + DBMethods.Db2Date(DATA_DEC) + ", " + TIPPRIRET.ToString() + "," + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL4, CommandType.Text);
    }

    public void WRITE_INSERT_DIPAPREV(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int CODPOS,
      int MAT,
      int PRORAP,
      int PROMOD,
      int MESDEN,
      int PRODEN,
      int PRODENDET,
      int ANNDEN,
      string TIPMOV,
      string AL = "",
      string ANNO = "",
      string TIPMOV_QRY = "")
    {
      string username = u.Username;
      string strSQL1 = "SELECT VALUE(MAX(PROMODDET),0) + 1 FROM MODPREDET " + " WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND PROMOD = " + PROMOD.ToString();
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string str1 = "INSERT INTO MODPREDET " + " SELECT " + CODPOS.ToString() + ", " + MAT.ToString() + ", " + PRORAP.ToString() + ", " + PROMOD.ToString() + ", " + int32.ToString() + ", ";
      string str2 = (!(ANNO != "") ? str1 + ANNDEN.ToString() + ", " : str1 + ANNO + ", ") + MESDEN.ToString() + ", " + DBMethods.DoublePeakForSql(TIPMOV) + ", " + "DAL, ";
      string str3 = (!(AL == "") ? str2 + DBMethods.Db2Date(AL) + ", " : str2 + "AL, ") + " DATERO, IMPRET, " + "IMPOCC, IMPFIG, IMPABB, " + "IMPASSCON, IMPCON, IMPMIN, DATDEC, DATCES, NUMGGAZI, " + "NUMGGFIG, NUMGGPER, NUMGGDOM, NUMGGSOS, " + "NUMGGCONAZI, IMPSCA, IMPTRAECO, ETA65, TIPRAP, FAP, " + "PERFAP, IMPFAP, PERPAR, PERAPP, CODCON, " + "PROCON, TIPSPE, CODLOC, PROLOC, CODLIV, CODGRUASS, " + "CODQUACON, ALIQUOTA, DATNAS, DATINPS, " + "DATSAP, ANNCOM, NOTE, IMPSANDET, TIPDEN, NUMMOV, " + "DATCONMOV, NUMMOVANN, DATMOVANN, NUMGRURET, " + "NUMGRURETRIF, ESIRET, IMPRETDEL, IMPOCCDEL, " + "IMPFIGDEL, IMPABBDEL, IMPASSCONDEL, " + "PRIORITA, PRORETTES, ANNRET, PRORET, IMPCONDEL, " + "DATINISAN, DATFINSAN, IMPRETPRE, IMPOCCPRE, " + "IMPFIGPRE, IMPSANDETPRE, " + "IMPCONPRE, CODCAUSAN, TASSAN, IMPABBPRE, IMPASSCONPRE, " + "NUMSANANN, DATSANANN, CODSANANN, NUMRETTES, " + "NUMRETTESRIF, BILMOVANN, BILSANANN, NUMSAN, " + "TIPANNMOVARR, TIPANNMOVARRANN, 0, 0, 0, 0, 0, 0, CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ", " + "PRODENDET, " + "PRODEN " + " FROM DENDET WHERE " + " CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString();
      string strSQL2 = (!(ANNO != "") ? str3 + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND TIPMOV <> 'AR'" : str3 + " AND ANNCOM = " + ANNDEN.ToString() + " AND ANNDEN = " + ANNO + " AND MESDEN = " + MESDEN.ToString() + " AND TIPMOV = " + DBMethods.DoublePeakForSql(TIPMOV_QRY.Trim())) + " AND PRODEN = " + PRODEN.ToString() + " AND PRODENDET = " + PRODENDET.ToString() + " AND VALUE(ESIRET,'') <> 'S' ";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
      string strSQL3 = "UPDATE MODPREDET SET IMPRETPRV = NULL, IMPOCCPRV = NULL, IMPFIGPRV = NULL, " + " IMPABBPRV = NULL, " + " IMPASSCONPRV = NULL, IMPCONPRV = NULL " + " WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND PROMOD = " + PROMOD.ToString() + " AND PROMODDET = " + int32.ToString();
      objDataAccess.WriteTransactionData(strSQL3, CommandType.Text);
    }

    public void WRITE_INSERT_PARPREV(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      ref iDB2DataAdapter DA,
      int CODPOS,
      int MAT,
      int PRORAP,
      int PROMOD,
      string ANNO_CES_PREC,
      string DATCES)
    {
      DataTable dataTable1 = new DataTable();
      int index1 = 0;
      Decimal num = 0M;
      string s = "";
      bool flag = false;
      string username = u.Username;
      string strSQL1 = "SELECT CODPOS, MAT, PRORAP, DATINI, DATFIN, TIPRAP, PERAPP, " + " PERPAR FROM STORDL " + " WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND VALUE(PERPAR, 0) > 0 " + " AND (DATINI >= " + DBMethods.Db2Date(ANNO_CES_PREC) + " OR " + " " + DBMethods.Db2Date(ANNO_CES_PREC) + " BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31') OR " + "  " + DBMethods.Db2Date(DATCES) + " BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')) " + " AND DATINI <= " + DBMethods.Db2Date(DATCES) + " " + " ORDER BY DATINI";
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL1);
      if (dataTable2.Rows.Count > 0)
      {
        while (!flag)
        {
          if (Convert.ToDecimal("0" + dataTable2.Rows[index1]["PERPAR"]?.ToString()) != num)
          {
            dataTable2.Rows[index1]["DATINI"].ToString();
            s = dataTable2.Rows[index1]["DATFIN"].ToString();
            num = Convert.ToDecimal("0" + dataTable2.Rows[index1]["PERPAR"]?.ToString());
            ++index1;
          }
          else if (DateTime.Compare(DateTime.Parse(s).AddDays(1.0), DateTime.Parse(dataTable2.Rows[index1]["DATINI"].ToString())) == 0)
          {
            s = dataTable2.Rows[index1]["DATFIN"].ToString();
            dataTable2.Rows[index1 - 1]["DATFIN"] = (object) s;
            dataTable2.Rows.RemoveAt(index1);
          }
          else
            ++index1;
          if (index1 == dataTable2.Rows.Count)
            break;
        }
        for (int index2 = 0; index2 <= dataTable2.Rows.Count - 1; ++index2)
        {
          string str1 = "INSERT INTO MODPREPAR (CODPOS, MAT, PRORAP, PROMOD, DATINI, " + " DATFIN, TIPRAP, PERAPP, PERPAR, " + "ULTAGG, UTEAGG) VALUES (" + CODPOS.ToString() + ", " + MAT.ToString() + ", " + PRORAP.ToString() + ", " + PROMOD.ToString() + ", " + DBMethods.Db2Date(dataTable2.Rows[index2]["DATINI"].ToString()) + ", " + DBMethods.Db2Date(dataTable2.Rows[index2]["DATFIN"].ToString()) + ", ";
          string str2 = string.IsNullOrEmpty(dataTable2.Rows[index2]["TIPRAP"].ToString()) ? str1 + "Null, " : str1 + dataTable2.Rows[index2]["TIPRAP"]?.ToString() + ", ";
          string str3 = string.IsNullOrEmpty(dataTable2.Rows[index2]["PERAPP"].ToString()) ? str2 + "Null, " : str2 + dataTable2.Rows[index2]["PERAPP"].ToString().Replace(",", ".") + ", ";
          string strSQL2 = (string.IsNullOrEmpty(dataTable2.Rows[index2]["PERPAR"].ToString()) ? str3 + "Null, " : str3 + dataTable2.Rows[index2]["PERPAR"].ToString().Replace(",", ".") + ", ") + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ") ";
          objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
        }
      }
      dataTable2.Dispose();
    }

    public void WRITE_INSERT_SOSPREV(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int CODPOS,
      int MAT,
      int PRORAP,
      int PROMOD,
      string ANNO_CES_PREC,
      string DATCES)
    {
      string username = u.Username;
      string strSQL = "INSERT INTO MODPRESOS " + " SELECT CODPOS, MAT, PRORAP, PROSOS, CODSOS, " + PROMOD.ToString() + ", " + " DATINISOS, " + " DATFINSOS, PERAZI, PERFIG, STASOS, CURRENT_TIMESTAMP," + DBMethods.DoublePeakForSql(username) + ", " + " CODPOSDA, CODPOSA FROM SOSRAP WHERE " + " CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND STASOS = 0" + " AND (DATINISOS >= " + DBMethods.Db2Date(ANNO_CES_PREC) + " OR " + " " + DBMethods.Db2Date(ANNO_CES_PREC) + " BETWEEN DATINISOS AND VALUE(DATFINSOS, '9999-12-31') OR " + "  " + DBMethods.Db2Date(DATCES) + " BETWEEN DATINISOS AND VALUE(DATFINSOS, '9999-12-31'))" + " AND DATINISOS <= " + DBMethods.Db2Date(DATCES) + " " + " ORDER BY DATINISOS";
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public void WRITE_DELETE_IMPAGG(
      DataLayer objDataAccess,
      int intCodPos,
      int intMat,
      int intProRap,
      string strDatInizio)
    {
      string strSQL = " DELETE FROM IMPAGG WHERE" + " CODPOS=" + intCodPos.ToString() + " AND MAT=" + intMat.ToString() + " AND PRORAP=" + intProRap.ToString() + " AND DATINI='" + DBMethods.Db2Date(strDatInizio.Replace("'", "")) + "'";
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public void WRITE_DELETE_STORDL(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int intCodPos,
      int intMat,
      int intProRap,
      string strDatInizio)
    {
      string username = u.Username;
      string strSQL = " DELETE FROM STORDL WHERE" + " CODPOS=" + intCodPos.ToString() + " AND MAT=" + intMat.ToString() + " AND PRORAP=" + intProRap.ToString() + " AND DATINI=" + DBMethods.Db2Date(strDatInizio.Replace("'", ""));
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public void WRITE_DELETE_PARTIMM(
      DataLayer objDataAccess,
      int intCodPos,
      int intMat,
      int intProRap,
      string strDatInizio)
    {
      string strSQL = " DELETE FROM PARTIMM WHERE" + " CODPOS=" + intCodPos.ToString() + " AND MAT=" + intMat.ToString() + " AND PRORAP=" + intProRap.ToString() + " AND DATINI='" + DBMethods.Db2Date(strDatInizio.Replace("'", "")) + "' ";
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public void WRITE_INSERT_ISCD(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int intMat,
      string strDatIni,
      int intDug,
      string strIndirizzoResidenza,
      string strCivicoResidenza,
      string strStatoEsteroResidenza,
      string strLocalitaResidenza,
      string strCapResidenza,
      string strProvinciaResidenza,
      string strTel1,
      string strTel2,
      string strFax,
      string strEmail,
      string strUrl,
      string strComune,
      string PEC,
      string CELL,
      string strCO = "")
    {
      string str1 = "S";
      string username = u.Username;
      string strSQL1 = "DELETE FROM ISCD WHERE MAT = " + intMat.ToString() + " AND DATINI = '" + DBMethods.Db2Date(strDatIni) + "'";
      objDataAccess.WriteTransactionData(strSQL1, CommandType.Text);
      string str2 = "INSERT INTO ISCD(MAT, DATINI, CODDUG, IND, NUMCIV, DENSTAEST," + "DENLOC, CAP, SIGPRO, AGGMAN, TEL1, TEL2, FAX, EMAIL, URL, CELL, EMAILCERT, ";
      string str3 = (!(strCO != "") ? str2 + "ULTAGG, UTEAGG, CODCOM)" : str2 + "ULTAGG, UTEAGG, CODCOM, CO)") + "VALUES( " + intMat.ToString() + ", '" + DBMethods.Db2Date(strDatIni.Replace("'", "")) + "', " + intDug.ToString() + ", " + DBMethods.DoublePeakForSql(strIndirizzoResidenza) + ", " + DBMethods.DoublePeakForSql(strCivicoResidenza) + ", '" + strStatoEsteroResidenza + "', " + DBMethods.DoublePeakForSql(strLocalitaResidenza) + ", " + DBMethods.DoublePeakForSql(strCapResidenza) + ", " + DBMethods.DoublePeakForSql(strProvinciaResidenza) + ", " + DBMethods.DoublePeakForSql(str1) + ", '" + strTel1 + "', '" + strTel2 + "', '" + strFax + "', " + DBMethods.DoublePeakForSql(strEmail) + ", '" + strUrl + "', '" + CELL.Replace(" ", "") + "', '" + PEC + "', " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ", ";
      string strSQL2 = !(strCO != "") ? (strComune == null ? str3 + " NULL) " : str3 + DBMethods.DoublePeakForSql(strComune) + ") ") : str3 + DBMethods.DoublePeakForSql(strComune) + ", '" + strCO + "') ";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
    }

    public void WRITE_INSERT_ISCD_P(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      string strCodFis,
      string strDatIni,
      int intDug,
      string strIndirizzoResidenza,
      string strCivicoResidenza,
      string strStatoEsteroResidenza,
      string strLocalitaResidenza,
      string strCapResidenza,
      string strProvinciaResidenza,
      string strTel1,
      string strTel2,
      string strFax,
      string strEmail,
      string strUrl,
      string strComune,
      string PEC,
      string CELL,
      string strCO = "")
    {
      string str1 = "S";
      string username = u.Username;
      string strSQL1 = "DELETE FROM ISCD_P WHERE CODFIS = " + DBMethods.DoublePeakForSql(strCodFis) + " AND DATINI = " + DBMethods.Db2Date(strDatIni);
      objDataAccess.WriteTransactionData(strSQL1, CommandType.Text);
      string str2 = "INSERT INTO ISCD_P(CODFIS, DATINI, CODDUG, IND, NUMCIV, DENSTAEST," + "DENLOC, CAP, SIGPRO, AGGMAN, TEL1, TEL2, FAX, EMAIL, URL, CELL, EMAILCERT, ";
      string str3 = (!(strCO != "") ? str2 + "ULTAGG, UTEAGG, CODCOM)" : str2 + "ULTAGG, UTEAGG, CODCOM, CO)") + "VALUES( " + DBMethods.DoublePeakForSql(strCodFis) + ", " + DBMethods.Db2Date(strDatIni.Replace("'", "")) + ", " + intDug.ToString() + ", " + DBMethods.DoublePeakForSql(strIndirizzoResidenza) + ", " + DBMethods.DoublePeakForSql(strCivicoResidenza) + ", " + DBMethods.DoublePeakForSql(strStatoEsteroResidenza) + ", " + DBMethods.DoublePeakForSql(strLocalitaResidenza) + ", " + DBMethods.DoublePeakForSql(strCapResidenza) + ", " + DBMethods.DoublePeakForSql(strProvinciaResidenza) + ", " + DBMethods.DoublePeakForSql(str1) + ", " + DBMethods.DoublePeakForSql(strTel1) + ", " + DBMethods.DoublePeakForSql(strTel2) + ", " + DBMethods.DoublePeakForSql(strFax) + ", " + DBMethods.DoublePeakForSql(strEmail) + ", " + DBMethods.DoublePeakForSql(strUrl) + ", " + DBMethods.DoublePeakForSql(CELL.Replace(" ", "")) + ", " + DBMethods.DoublePeakForSql(PEC) + ", " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ", ";
      string strSQL2 = !(strCO != "") ? (strComune == null ? str3 + " NULL) " : str3 + DBMethods.DoublePeakForSql(strComune) + ") ") : str3 + DBMethods.DoublePeakForSql(strComune) + ", " + DBMethods.DoublePeakForSql(strCO) + ") ";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
    }

    public void WRITE_UPDATE_ISCT(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int intMat,
      string strCognome,
      string strNome,
      string strCodiceFiscale,
      string strDataNascita,
      string strCODCOM,
      string strSesso,
      string TitStu)
    {
      string username = u.Username;
      string str = "UPDATE ISCT SET " + " COG = " + DBMethods.DoublePeakForSql(strCognome) + ", " + " NOM = " + DBMethods.DoublePeakForSql(strNome) + ", " + " CODFIS = " + DBMethods.DoublePeakForSql(strCodiceFiscale) + ", " + " DATNAS = '" + DBMethods.Db2Date(strDataNascita.Replace("'", "")) + "', " + " CODCOM = " + DBMethods.DoublePeakForSql(strCODCOM) + ", " + " SES = " + DBMethods.DoublePeakForSql(strSesso) + ", ";
      if (TitStu != "")
        str = str + " TITSTU = '" + TitStu + "', ";
      string strSQL = str + " FLGAPP = 'M', UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " ULTAGG = CURRENT_TIMESTAMP " + " WHERE MAT = '" + intMat.ToString() + "'";
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public void WRITE_UPDATE_RAPLAV(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int intMat,
      int intCodPos,
      int intProRap,
      string strDatDen,
      string strDatAss,
      string strIsc)
    {
      string username = u.Username;
      string strSQL = "UPDATE RAPLAV SET " + " FLGAPP = 'M', DATDEN = " + DBMethods.Db2Date(strDatDen.Replace("'", "")) + ", " + " DATLIQTFR = " + DBMethods.Db2Date(strIsc.Replace("'", "")) + ", " + " DATASS = " + DBMethods.Db2Date(strDatAss.Replace("'", "")) + ", " + " DATDEC = " + DBMethods.Db2Date(strIsc.Replace("'", "")) + ", " + " DATSAP = NULL, " + " DATINPS = NULL, " + " UTEAGG = " + DBMethods.DoublePeakForSql(username) + ", " + " ULTAGG = CURRENT_TIMESTAMP " + " WHERE CODPOS = " + intCodPos.ToString() + " AND MAT = " + intMat.ToString() + " AND PRORAP = " + intProRap.ToString();
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public void WRITE_INSERT_STORDL(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int intCodPos,
      int intMat,
      int intProRap,
      string strDatIni,
      string strDatFin,
      Decimal decTraEco,
      string strPerApp,
      int intTipRap,
      int intCodConRif,
      int intCodConLoc,
      int intLivello,
      int intMensilita,
      string strCorresponsione14,
      string strCorresponsione15,
      string strCorresponsione16,
      int intAliquota,
      string strAssCon,
      string strAbbPre,
      string strPercentualePartTime,
      string strDataScadenzaTermine,
      int intNumScatti,
      Decimal intImpScatti,
      string strDataUltScatto,
      string strDataNuovoScatto,
      string FAP)
    {
      string username = u.Username;
      string str1 = " INSERT INTO STORDL (CODPOS,MAT,PRORAP,DATINI," + "DATFIN,TIPRAP,CODCON,CODLOC,CODLIV,TRAECO," + "FAP, NUMMEN, PERAPP, MESMEN14, MESMEN15, MESMEN16, INDANN, CODGRUASS, ASSCON," + "ABBPRE, PERPAR, DATSCATER, NUMSCAMAT, IMPSCAMAT, DATULTSCA, DATNEWSCA, " + "ULTAGG, UTEAGG) " + "VALUES ('" + intCodPos.ToString() + "', '" + intMat.ToString() + "', '" + intProRap.ToString() + "', '" + DBMethods.Db2Date(strDatIni.Replace("'", "")) + "', '" + DBMethods.Db2Date(strDatFin.Replace("'", "")) + "', '" + intTipRap.ToString() + "', '" + intCodConRif.ToString() + "', ";
      string str2 = (intCodConLoc != 0 ? str1 + intCodConLoc.ToString() + "', '" : str1 + "NULL, '") + intLivello.ToString() + "', '" + decTraEco.ToString().Replace(",", ".") + "', '" + FAP + "', '" + intMensilita.ToString() + "', ";
      string str3 = (!(strPerApp == "") ? str2 + DBMethods.DoublePeakForSql(strPerApp) + ", '" : str2 + "Null,'") + strCorresponsione14 + "', '" + strCorresponsione15 + "', '" + strCorresponsione16 + "', " + "'N','" + intAliquota.ToString() + "', '" + strAssCon + "', '" + strAbbPre + "', '" + strPercentualePartTime + "', ";
      string str4 = (!(strDataScadenzaTermine != "") ? str3 + "NULL, '" : str3 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strDataScadenzaTermine.Replace("'", ""))) + ", ") + intNumScatti.ToString() + "', '" + intImpScatti.ToString().Replace(",", ".") + "', ";
      string str5 = !(strDataUltScatto != "") ? str4 + "NULL, " : str4 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strDataUltScatto.Replace("'", ""))) + ", ";
      string strSQL = (!(strDataNuovoScatto != "") ? str5 + "NULL, " : str5 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strDataNuovoScatto.Replace("'", ""))) + ", ") + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public void WRITE_UPDATE_STORDL(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int intCodPos,
      int intMat,
      int intProRap,
      string strDatIni,
      string strDatFin,
      Decimal decTraEco,
      string strPercentualeApp,
      int intTipRap,
      int intCodConRif,
      int intCodConLoc,
      int intLivello,
      int intMensilita,
      string strCorresponsione14,
      string strCorresponsione15,
      string strCorresponsione16,
      int intAliquota,
      string strAssCon,
      string strAbbPre,
      string strPercentualePartTime,
      string strDataScadenzaTermine,
      int intNumScatti,
      Decimal intImpScatti,
      string strDataUltScatto,
      string strDataNuovoScatto,
      string FAP)
    {
      string username = u.Username;
      string str1 = "UPDATE STORDL SET " + " DATFIN = '" + strDatFin + "', " + " TIPRAP = " + intTipRap.ToString() + ", " + " CODCON = " + intCodConRif.ToString() + ", ";
      string str2 = (intCodConLoc != 0 ? str1 + " CODLOC = " + intCodConLoc.ToString() + ", " : str1 + " CODLOC =  NULL, ") + " CODLIV = " + intLivello.ToString() + ", " + " TRAECO = " + decTraEco.ToString().Replace(",", ".") + ", " + " FAP = " + DBMethods.DoublePeakForSql(FAP) + ", " + " NUMMEN = " + intMensilita.ToString() + ", ";
      string str3 = (!(strPercentualeApp == "N") ? str2 + " PERAPP = " + DBMethods.DoublePeakForSql(strPercentualeApp) + ", " : str2 + " PERAPP =  NULL, ") + " MESMEN14 = " + DBMethods.DoublePeakForSql(strCorresponsione14) + ", " + " MESMEN15 = " + DBMethods.DoublePeakForSql(strCorresponsione15) + ", " + " MESMEN16 = " + DBMethods.DoublePeakForSql(strCorresponsione16) + ", " + " INDANN = 'N', " + " CODGRUASS = " + intAliquota.ToString() + ", " + " ASSCON = " + DBMethods.DoublePeakForSql(strAssCon) + ", " + " ABBPRE = " + DBMethods.DoublePeakForSql(strAbbPre) + ", " + " PERPAR = " + DBMethods.DoublePeakForSql(strPercentualePartTime) + ", ";
      string str4 = (!(strDataScadenzaTermine != "") ? str3 + " DATSCATER = NULL, " : str3 + " DATSCATER = " + DBMethods.Db2Date(strDataScadenzaTermine.Replace("'", "")) + ",") + " NUMSCAMAT = " + intNumScatti.ToString() + ", " + " IMPSCAMAT = " + intImpScatti.ToString().Replace(",", ".") + ", ";
      string str5 = !(strDataUltScatto != "") ? str4 + " DATULTSCA = NULL, " : str4 + " DATULTSCA = '" + DBMethods.Db2Date(strDataUltScatto.Replace("'", "")) + "',";
      string strSQL = (!(strDataNuovoScatto != "") ? str5 + " DATNEWSCA = NULL, " : str5 + " DATNEWSCA = '" + DBMethods.Db2Date(strDataNuovoScatto.Replace("'", "")) + "',") + " ULTAGG = CURRENT_TIMESTAMP ," + " UTEAGG = " + DBMethods.DoublePeakForSql(username) + " WHERE CODPOS = " + intCodPos.ToString() + " AND MAT = " + intMat.ToString() + " AND PRORAP = " + intProRap.ToString() + " AND DATINI = '" + strDatIni + "' ";
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public void WRITE_INSERT_ISCT_P(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      string strCognome,
      string strNome,
      string strDataNascita,
      string strCodCom,
      string strCodiceFiscale,
      string strSesso,
      int intTitStu)
    {
      string username = u.Username;
      string strSQL = "INSERT INTO ISCT_P(COG,NOM,DATNAS,CODCOM,CODFIS,SES,TITSTU,ULTAGG,UTEAGG)" + " VALUES(" + DBMethods.DoublePeakForSql(strCognome) + ", " + DBMethods.DoublePeakForSql(strNome) + ", " + DBMethods.Db2Date(strDataNascita.Replace("'", "")) + ", " + DBMethods.DoublePeakForSql(strCodCom) + ", " + DBMethods.DoublePeakForSql(strCodiceFiscale) + ", " + DBMethods.DoublePeakForSql(strSesso) + ", " + intTitStu.ToString() + ", " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public int WRITE_INSERT_ISCT(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      string strCognome,
      string strNome,
      string strDataNascita,
      string strComuneNascita,
      string strCodComComuneNascita,
      string strCodComStatoEsteroNascita,
      string strCodiceFiscale,
      string strSesso,
      int intTitStu)
    {
      string username = u.Username;
      string strSQL1 = "SELECT VALUE(MAX(MAT),0) + 1 FROM ISCT";
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string str = "INSERT INTO ISCT(MAT,COG,NOM,DATNAS,CODCOM,CODFIS,SES,TITSTU, DATINSMAT, FLGAPP, ULTAGG,UTEAGG)" + " VALUES(" + int32.ToString() + ", " + DBMethods.DoublePeakForSql(strCognome) + ", " + DBMethods.DoublePeakForSql(strNome) + ", " + DBMethods.Db2Date(strDataNascita.Replace("'", "")) + ", ";
      string strSQL2 = (!(strComuneNascita != "") ? str + DBMethods.DoublePeakForSql(strCodComStatoEsteroNascita) + ", " : str + DBMethods.DoublePeakForSql(strCodComComuneNascita) + ", ") + DBMethods.DoublePeakForSql(strCodiceFiscale) + ", " + DBMethods.DoublePeakForSql(strSesso) + ", " + intTitStu.ToString() + ", " + " CURRENT_DATE, 'I', " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
      return int32;
    }

    public int WRITE_INSERT_RAPLAV(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int intCodPos,
      int intMat,
      string strDataIscrizione,
      string strDataAssunzione,
      string strDataDenuncia,
      string strNumeroProtocollo,
      string strDataProtocollo,
      string strCodPosSom,
      string strRagSocSom,
      string strPIvaSom,
      string strCodFiscSom,
      string strIndirizzoSom,
      object strDugSom,
      string strnumCivSom,
      string strLocalitaSom,
      string strCapSom,
      string strProvSom,
      string strComuneSom,
      string strTelFor)
    {
      string username = u.Username;
      Decimal num = Convert.ToDecimal(strNumeroProtocollo);
      string strSQL1 = "SELECT VALUE(MAX(PRORAP), 0) + 1 FROM RAPLAV WHERE " + " MAT=" + intMat.ToString();
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string str1 = "INSERT INTO RAPLAV(CODPOS,MAT,PRORAP,DATDEC,DATCES,CODCAUCES," + " DATASS,DATDEN,DATDENCES,NUMPRO,DATPRO," + " CODPOSFOR,RAGSOCFOR,PARIVAFOR,CODFISFOR,INDFOR," + " CODDUGFOR,NUMCIVFOR,DENLOCFOR,CAPFOR,SIGPROFOR," + " TELFOR, FLGAPP, ULTAGG,UTEAGG, CODCOMFOR)" + " VALUES ( " + intCodPos.ToString() + ", " + intMat.ToString() + ", " + int32.ToString() + ", '" + DBMethods.Db2Date(strDataIscrizione) + "', " + " NULL, " + " NULL, '" + DBMethods.Db2Date(strDataAssunzione) + "', '" + DBMethods.Db2Date(strDataDenuncia) + "', " + "NULL, '" + num.ToString() + "', '";
      string str2 = !(strDataProtocollo == "") ? str1 + DBMethods.Db2Date(strDataProtocollo) + "', '" : str1 + " NULL, ";
      string str3 = (!(strCodPosSom != "") ? str2 + "NULL, '" : str2 + strCodPosSom + "', '") + strRagSocSom + "', '" + strPIvaSom + "', '" + strCodFiscSom + "', '" + strIndirizzoSom + "', ";
      string strSQL2 = (strDugSom == null ? str3 + "NULL, '" : str3 + strDugSom?.ToString() + "', ") + strnumCivSom + "', '" + strLocalitaSom + "', '" + strCapSom + "', '" + strProvSom + "', '" + strTelFor + "', " + " 'I', CURRENT_TIMESTAMP, '" + username + "', '" + strComuneSom + "') ";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
      return int32;
    }

    public void WRITE_INSERT_PARTIMM(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int intCodPos,
      int intMat,
      int intProRap,
      string strDatIni,
      int intMese)
    {
      string username = u.Username;
      string strSQL = " INSERT INTO PARTIMM (CODPOS,MAT,PRORAP,DATINI,PARMES, " + " ULTAGG,UTEAGG)" + "VALUES (" + intCodPos.ToString() + ", " + intMat.ToString() + ", " + intProRap.ToString() + ", '" + DBMethods.Db2Date(strDatIni.Replace("'", "")) + "', " + intMese.ToString() + ", " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public void WRITE_INSERT_SCADEN(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      string CODPOS,
      string RAGSOC,
      string MAT,
      string COG,
      string NOM,
      string PRORAP,
      string CODDOC,
      string NOTE,
      string DATSCA)
    {
      Convert.ToDateTime(this.Module_GetDataSistema(objDataAccess));
      string username = u.Username;
      string strSQL1 = "SELECT VALUE(MAX(PROSCA), 0) + 1  FROM SCADEN ";
      string str1 = "INSERT INTO SCADEN (PROSCA, CODPOS, RAGSOC, MAT, COG, NOM, PRORAP, DATINS, CODUTEINS, CODDOC, NOTE, DATSCA, ULTAGG, UTEAGG)" + " VALUES (" + Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text)).ToString() + ", ";
      string str2 = (!(CODPOS != "") ? str1 + "Null, " : str1 + CODPOS + ", ") + DBMethods.DoublePeakForSql(RAGSOC) + ", ";
      string str3 = (!(MAT != "") ? str2 + "Null, " : str2 + MAT + ", ") + DBMethods.DoublePeakForSql(COG) + ", " + DBMethods.DoublePeakForSql(NOM) + ", ";
      string str4 = (!(PRORAP != "") ? str3 + "Null, " : str3 + PRORAP + ", ") + "CURRENT_DATE, " + DBMethods.DoublePeakForSql(username) + ", ";
      string str5 = (!(CODDOC != "") ? str4 + "Null, " : str4 + CODDOC + ", ") + DBMethods.DoublePeakForSql(NOTE) + ", ";
      string strSQL2 = (!(DATSCA != "") ? str5 + DBMethods.Db2Date("31/12/9999") + ", " : str5 + DBMethods.Db2Date(DATSCA.Replace("'", "")) + ", ") + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
    }

    public void WRITE_UPDATE_SCADEN(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int PROSCA,
      string CODPOS,
      string RAGSOC,
      string MAT,
      string COG,
      string NOM,
      string PRORAP,
      string CODDOC,
      string NOTE,
      string DATSCA)
    {
      Convert.ToDateTime(this.Module_GetDataSistema(objDataAccess));
      string username = u.Username;
      string str1 = "UPDATE SCADEN SET ";
      string str2 = (!(CODPOS != "") ? str1 + " CODPOS = NULL, " : str1 + " CODPOS = " + CODPOS + ", ") + " RAGSOC = " + DBMethods.DoublePeakForSql(RAGSOC) + ", ";
      string str3 = !(CODDOC != "") ? str2 + " CODDOC = NULL, " : str2 + " CODDOC = " + CODDOC + ", ";
      string str4 = (!(MAT != "") ? str3 + "MAT = NULL, " : str3 + "MAT = " + MAT + ", ") + "COG = " + DBMethods.DoublePeakForSql(COG) + ", " + "NOM = " + DBMethods.DoublePeakForSql(NOM) + ", " + "NOTE = " + DBMethods.DoublePeakForSql(NOTE) + ", ";
      string strSQL = (!(DATSCA != "") ? str4 + "DATSCA = " + DBMethods.Db2Date("31/12/9999") + ", " : str4 + "DATSCA = " + DBMethods.Db2Date(DATSCA.Replace("'", "")) + ", ") + "ULTAGG = CURRENT_TIMESTAMP, " + "UTEAGG = " + DBMethods.DoublePeakForSql(username) + " WHERE PROSCA = " + PROSCA.ToString();
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public void WRITE_RISOLVI_SCADEN(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int lngPROSCA,
      int lngCODDOC,
      string strNote = "")
    {
      DateTime dateTime = Convert.ToDateTime(this.Module_GetDataSistema(objDataAccess));
      string username = u.Username;
      string str = "UPDATE SCADEN SET" + " CODDOC = " + lngCODDOC.ToString();
      string strSQL = (!(strNote != "") ? str + ", NOTE = Null" : str + ", NOTE = " + DBMethods.DoublePeakForSql(strNote)) + ", DATRIS = " + DBMethods.Db2Date(dateTime.ToString()) + ", CODUTERIS = " + DBMethods.DoublePeakForSql(username) + ", ULTAGG = CURRENT_TIMESTAMP" + ", UTEAGG = " + DBMethods.DoublePeakForSql(username) + " WHERE PROSCA = " + lngPROSCA.ToString();
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public void WRITE_DELETE_SCADEN(DataLayer objDataAccess, TFI.OCM.Utente.Utente u, int lngPROSCA)
    {
      DateTime dateTime = Convert.ToDateTime(this.Module_GetDataSistema(objDataAccess));
      string username = u.Username;
      string strSQL = "UPDATE SCADEN SET" + " DATANN = " + DBMethods.Db2Date(dateTime.ToString()) + ", CODUTEANN = " + DBMethods.DoublePeakForSql(username) + ", ULTAGG = CURRENT_TIMESTAMP" + ", UTEAGG = " + DBMethods.DoublePeakForSql(username) + " WHERE PROSCA = " + lngPROSCA.ToString();
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public int WRITE_INSERT_MODPREDET(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int CODPOS,
      int MAT,
      int PRORAP,
      int PROMOD,
      int ANNDEN,
      int MESDEN,
      string TIPMOV,
      string DAL,
      string AL,
      string DATERO,
      string IMPRET,
      string IMPOCC,
      string IMPFIG,
      Decimal IMPABB,
      Decimal IMPASSCON,
      Decimal IMPADDREC,
      string IMPRETPRV,
      string IMPOCCPRV,
      string IMPFIGPRV,
      string IMPABBPRV,
      string IMPASSCONPRV,
      string IMPADDRECPRV,
      Decimal IMPMIN,
      string DATDEC,
      string DATCES,
      Decimal NUMGGAZI,
      Decimal NUMGGFIG,
      Decimal NUMGGPER,
      Decimal NUMGGDOM,
      Decimal NUMGGSOS,
      Decimal NUMGGCONAZI,
      Decimal IMPSCA,
      Decimal IMPTRAECO,
      string ETA65,
      int TIPRAP,
      string FAP,
      Decimal PERFAP,
      Decimal IMPFAP,
      Decimal PERPAR,
      Decimal PERAPP,
      int CODCON,
      int PROCON,
      string TIPSPE,
      int CODLOC,
      int PROLOC,
      int CODLIV,
      int CODGRUASS,
      int CODQUACON,
      Decimal ALIQUOTA,
      string DATNAS,
      int ANNCOM,
      int PRODEN = 0,
      int PRODENDET = 0)
    {
      int[,] numArray = new int[1, 2];
      Decimal num = 0.0M;
      string username = u.Username;
      num = Convert.ToDecimal(this.Module_GetValorePargen(objDataAccess, 5, DAL, CODPOS));
      string strSQL1 = " SELECT VALUE(MAX(PROMODDET), 0) + 1  FROM MODPREDET " + " WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND PROMOD = " + PROMOD.ToString();
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string str1 = "INSERT INTO MODPREDET (" + "CODPOS, " + "MAT, " + "PRORAP, " + "PROMOD, " + "PROMODDET, " + "ANNDEN, " + "MESDEN, " + "TIPMOV, " + "DAL, " + "AL, " + "DATERO, " + "IMPRET, " + "IMPOCC, " + "IMPFIG, " + "IMPABB, " + "IMPASSCON, " + "IMPCON, " + "IMPRETPRV, " + "IMPOCCPRV, " + "IMPFIGPRV, " + "IMPABBPRV, " + "IMPASSCONPRV, " + "IMPCONPRV, " + "IMPMIN, " + "DATDEC, " + "DATCES, " + "NUMGGAZI, " + "NUMGGFIG, " + "NUMGGPER, " + "NUMGGDOM, " + "NUMGGSOS, " + "NUMGGCONAZI, " + "IMPSCA, " + "IMPTRAECO, " + "ETA65 ," + "TIPRAP, " + "FAP, " + "PERFAP ," + "IMPFAP ," + "PERPAR, " + "PERAPP, " + "CODCON, " + "PROCON, " + "TIPSPE, " + "CODLOC, " + "PROLOC, " + "CODLIV, " + "CODGRUASS, " + "CODQUACON, " + "ALIQUOTA, " + "DATNAS, " + "ANNCOM, ";
      if (PRODEN != 0)
        str1 = str1 + "PRODEN, " + "PRODENDET, ";
      string str2 = str1 + "ULTAGG, " + "UTEAGG) " + " VALUES (" + CODPOS.ToString() + ", " + MAT.ToString() + ", " + PRORAP.ToString() + ", " + PROMOD.ToString() + ", " + int32.ToString() + ", " + ANNDEN.ToString() + ", " + MESDEN.ToString() + ", " + DBMethods.DoublePeakForSql(TIPMOV) + ", " + DBMethods.Db2Date(DAL) + ", " + DBMethods.Db2Date(AL) + ", ";
      string str3 = (!(DATERO != "") ? str2 + "Null, " : str2 + DBMethods.Db2Date(DATERO) + ", ") + IMPRET.ToString().Replace(",", ".") + ", " + IMPOCC.ToString().Replace(",", ".") + ", " + IMPFIG.ToString().Replace(",", ".") + ", " + IMPABB.ToString().Replace(",", ".") + ", " + IMPASSCON.ToString().Replace(",", ".") + ", " + "ROUND(" + IMPRET.ToString().Replace(",", ".") + " * " + ALIQUOTA.ToString().Replace(",", ".") + " / 100, 2), ";
      string str4 = !(IMPRETPRV.Trim() == "") ? str3 + IMPRETPRV.Replace(",", ".") + ", " : str3 + "Null, ";
      string str5 = !(IMPOCCPRV.Trim() == "") ? str4 + IMPOCCPRV.Replace(",", ".") + ", " : str4 + "Null, ";
      string str6 = !(IMPFIGPRV.Trim() == "") ? str5 + IMPFIGPRV.ToString().Replace(",", ".") + ", " : str5 + "Null, ";
      string str7 = !(IMPABBPRV.Trim() == "") ? str6 + IMPABB.ToString().Replace(",", ".") + ", " : str6 + "Null, ";
      string str8;
      if (IMPRETPRV.Trim() == "")
        str8 = str7 + "Null, " + "Null, ";
      else
        str8 = (!(IMPASSCONPRV.Trim() == "") ? str7 + IMPASSCONPRV.ToString().Replace(",", ".") + ", " : str7 + "Null, ") + "ROUND(" + IMPRETPRV.ToString().Replace(",", ".") + " * " + ALIQUOTA.ToString().Replace(",", ".") + " / 100, 2), ";
      string str9 = str8 + IMPMIN.ToString().Replace(",", ".") + ", ";
      string str10 = !(DATDEC != "") ? str9 + "NULL, " : str9 + DBMethods.Db2Date(DATDEC) + ", ";
      string str11 = (!(DATCES != "") ? str10 + "NULL, " : str10 + DBMethods.Db2Date(DATCES) + ", ") + NUMGGAZI.ToString().Replace(",", ".") + ", " + NUMGGFIG.ToString().Replace(",", ".") + ", " + NUMGGPER.ToString().Replace(",", ".") + ", " + NUMGGDOM.ToString().Replace(",", ".") + ", " + NUMGGSOS.ToString().Replace(",", ".") + ", " + NUMGGCONAZI.ToString().Replace(",", ".") + ", " + IMPSCA.ToString().Replace(",", ".") + ", " + IMPTRAECO.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(ETA65) + ", ";
      string str12 = (TIPRAP == 0 ? str11 + "NULL, " : str11 + TIPRAP.ToString() + ", ") + DBMethods.DoublePeakForSql(FAP) + ", ";
      if (FAP == "S")
        IMPFAP = !(IMPRETPRV.Trim() == "") ? (Decimal) (Convert.ToInt32(IMPRETPRV) / 100) * PERFAP : 0M * PERFAP;
      string str13 = str12 + PERFAP.ToString().Replace(",", ".") + ", " + IMPFAP.ToString().Replace(",", ".") + ", " + PERPAR.ToString().Replace(",", ".") + ", " + PERAPP.ToString().Replace(",", ".") + ", " + CODCON.ToString() + ", " + PROCON.ToString() + ", " + DBMethods.DoublePeakForSql(TIPSPE) + ", " + CODLOC.ToString() + ", " + PROLOC.ToString() + ", " + CODLIV.ToString() + ", " + CODGRUASS.ToString() + ", " + CODQUACON.ToString() + ", " + ALIQUOTA.ToString().Replace(",", ".") + ", " + DBMethods.Db2Date(DATNAS) + ", ";
      string str14 = ANNCOM == 0 ? str13 + "NULL, " : str13 + ANNCOM.ToString() + ", ";
      if (PRODEN != 0)
        str14 = str14 + PRODEN.ToString() + ", " + PRODENDET.ToString() + ", ";
      string strSQL2 = str14 + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
      return int32;
    }

    public bool WRITE_INSERT_MOVASSAP(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      ref string PARTITA,
      ref int PROGMOV,
      ref string PARTITA_ASS,
      ref int PROGMOV_ASS,
      ref string CODCAU_ASS,
      Decimal IMP_ASS)
    {
      string username = u.Username;
      string dataSistema = this.Module_GetDataSistema(objDataAccess);
      string str1 = Convert.ToDateTime(dataSistema).Year.ToString();
      string str2 = Convert.ToDateTime(dataSistema).Month.ToString();
      string str3 = Convert.ToDateTime(dataSistema).Day.ToString();
      string str4 = str1 + str2.ToString().PadLeft(2, '0') + str3.ToString().PadLeft(2, '0');
      if (IMP_ASS < 0M)
        IMP_ASS = Convert.ToDecimal(IMP_ASS * -1M);
      string strSQL = "INSERT INTO " + this.TAB_CONTABILE_MOVASSAP + " " + " ( PARTITA, PROGMOV, PARTASS, PROGRASS, CODCAUASS, " + " STATOSAP, IMPASS, DTASSOC, USERID) VALUES( " + DBMethods.DoublePeakForSql(PARTITA) + ", " + PROGMOV.ToString() + ", " + DBMethods.DoublePeakForSql(PARTITA_ASS) + ", " + PROGMOV_ASS.ToString() + ", " + DBMethods.DoublePeakForSql(CODCAU_ASS) + ", " + " '', " + IMP_ASS.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(str4) + ", " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
      return true;
    }

    public bool WRITE_INSERT_MOVRETSAP(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      ref string PARTITA,
      ref int PROGMOV,
      ref string PARTITARET,
      ref int PROGMOVRET,
      ref string CODCAURET,
      Decimal IMP_RETTIFICA,
      Decimal IMPTFR,
      Decimal IMPFP,
      Decimal IMPINF,
      Decimal IMPFAP,
      Decimal IMPAD,
      Decimal IMPAC,
      Decimal IMPAB,
      Decimal IMPALTRO)
    {
      string username = u.Username;
      string dataSistema = this.Module_GetDataSistema(objDataAccess);
      string str1 = Convert.ToDateTime(dataSistema).Year.ToString();
      string str2 = Convert.ToDateTime(dataSistema).Month.ToString();
      string str3 = Convert.ToDateTime(dataSistema).Day.ToString();
      string str4 = str1 + str2.ToString().PadLeft(2, '0') + str3.ToString().PadLeft(2, '0');
      string strSQL = "INSERT INTO " + this.TAB_CONTABILE_MOVRETSAP + " " + " (PARTITA, PROGMOV, PARTRET, PROGRRET, CODCAURET, " + " FLGSTORNO, IMPRET, IMPTFR, IMPFP, IMPINF, IMPFAP, IMPAD, IMPAC, IMPAB, " + " IMPALTRO, DTRETTIF, STATOSAP, TIPORETT, USERID) VALUES( " + DBMethods.DoublePeakForSql(PARTITA) + ", " + PROGMOV.ToString() + ", " + DBMethods.DoublePeakForSql(PARTITARET) + ", " + PROGMOVRET.ToString() + ", " + DBMethods.DoublePeakForSql(CODCAURET) + ", " + " 'N', " + IMP_RETTIFICA.ToString().Replace(",", ".") + ", " + IMPTFR.ToString().Replace(",", ".") + ", " + IMPFP.ToString().Replace(",", ".") + ", " + IMPINF.ToString().Replace(",", ".") + ", " + IMPFAP.ToString().Replace(",", ".") + ", " + IMPAD.ToString().Replace(",", ".") + ", " + IMPAC.ToString().Replace(",", ".") + ", " + IMPAB.ToString().Replace(",", ".") + ", " + IMPALTRO.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(str4) + ", " + " '', " + " '', " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
      return true;
    }

    public void WRITE_INSERT_SPLABSAP(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      string PARTITA_DA,
      int PROGMOV_DA,
      string PARTITA_A,
      int PROGMOV_A,
      int PROGABBIN,
      int PROGSPLID,
      int PROGSPLIA,
      string GESCON,
      Decimal IMPORTO)
    {
      string username = u.Username;
      string strSQL = "INSERT INTO " + this.TAB_CONTABILE_SPLABSAP + " " + " ( PARTITAD, PROGMOVD, PARTITAA, PROGMOVA, PROGABBIN, " + " PROGSPLID, PROGSPLIA, GESTCON, IMPORTO, STATOSAP, USERID) VALUES( " + DBMethods.DoublePeakForSql(PARTITA_DA) + ", " + PROGMOV_DA.ToString() + ", " + DBMethods.DoublePeakForSql(PARTITA_A) + ", " + PROGMOV_A.ToString() + ", " + PROGABBIN.ToString() + ", " + PROGSPLID.ToString() + ", " + PROGSPLIA.ToString() + ", " + DBMethods.DoublePeakForSql(GESCON) + ", " + " ( " + IMPORTO.ToString().Replace(",", ".") + " ), " + " '', " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
    }

    public int WRITE_INSERT_ABBINSAP(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      ref string PARTITA_DA,
      ref int PROGMOV_DA,
      ref string PARTITA_A,
      ref int PROGMOV_A,
      Decimal IMPORTO)
    {
      string username = u.Username;
      string dataSistema = this.Module_GetDataSistema(objDataAccess);
      string str1 = Convert.ToDateTime(dataSistema).Year.ToString();
      string str2 = Convert.ToDateTime(dataSistema).Month.ToString();
      string str3 = Convert.ToDateTime(dataSistema).Day.ToString();
      string str4 = str1 + str2.ToString().PadLeft(2, '0') + str3.ToString().PadLeft(2, '0');
      string strSQL1 = " SELECT VALUE(MAX(PROGABBIN ), 0) + 1  FROM " + this.TAB_CONTABILE_ABBINSAP + " " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITA_DA) + " AND PROGMOVD = " + DBMethods.DoublePeakForSql(PROGMOV_DA.ToString()) + " AND PARTITAA = " + DBMethods.DoublePeakForSql(PARTITA_A) + " AND PROGMOVA = " + DBMethods.DoublePeakForSql(PROGMOV_A.ToString());
      int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string strSQL2 = "INSERT INTO " + this.TAB_CONTABILE_ABBINSAP + " " + " ( PARTITAD, PROGMOVD, PARTITAA, PROGMOVA, PROGABBIN, " + " IMPORTO, STATOVAL, STATOSAP, STATOSED, DTABBIN, DTANNULL, USERID) VALUES( " + DBMethods.DoublePeakForSql(PARTITA_DA) + ", " + PROGMOV_DA.ToString() + ", " + DBMethods.DoublePeakForSql(PARTITA_A) + ", " + PROGMOV_A.ToString() + ", " + int32.ToString() + ", " + IMPORTO.ToString().Replace(",", ".") + ", " + " 'V', " + " '', " + " 'V', " + DBMethods.DoublePeakForSql(str4) + ", " + " 0, " + DBMethods.DoublePeakForSql(username) + ")";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
      return int32;
    }

    public object Module_GetValorePargen(
      DataLayer objDataAccess,
      int lngCODPAR,
      string strDataDecorrenza = "DATA SISTEMA",
      int CODPOS = 0)
    {
      if (strDataDecorrenza == "DATA SISTEMA")
        strDataDecorrenza = objDataAccess.Get1ValueFromSQL("SELECT CURRENT_DATE AS DATASISTEMA FROM DBUNICONET.TIPIND", CommandType.Text);
      object valorePargen;
      if (CODPOS != 0)
      {
        string strSQL1 = "SELECT VALORE FROM PARGENPOS WHERE CODPOS = " + CODPOS.ToString() + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strDataDecorrenza)) + " BETWEEN DATINI AND DATFIN";
        valorePargen = (object) objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text);
        if (string.IsNullOrEmpty(valorePargen.ToString()))
        {
          string strSQL2 = "SELECT VALORE FROM PARGENDET WHERE CODPAR = " + lngCODPAR.ToString() + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strDataDecorrenza)) + " BETWEEN DATINI AND DATFIN";
          valorePargen = (object) objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text);
        }
      }
      else
      {
        string strSQL = "SELECT VALORE FROM PARGENDET WHERE CODPAR = " + lngCODPAR.ToString() + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strDataDecorrenza)) + " BETWEEN DATINI AND DATFIN";
        valorePargen = (object) objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text);
      }
      return valorePargen;
    }

    public string Module_GetDataSistema(DataLayer objDataAccess) => objDataAccess.Get1ValueFromSQL("SELECT CURRENT_DATE AS DATASISTEMA FROM DBUNICONET.TIPIND", CommandType.Text);

    public string Module_GetTipoAnno(int ANNDEN, int ANNBIL)
    {
      if (ANNDEN == ANNBIL)
        return "AC";
      if (ANNBIL < ANNDEN)
        throw new Exception("Contattare l'Amministratore del Sistema. Anno di bilancio inferiore all'anno della distinta");
      return "AP";
    }

    public string Module_Get_NumeroSapMovimento(int NUMERO_RIF, string CODCAU, int ANNO)
    {
      if (ANNO < 2003)
        return CODCAU.PadLeft(2, '0') + "/0000/0" + NUMERO_RIF.ToString().Trim();
      return CODCAU.PadLeft(2, '0') + "/" + ANNO.ToString() + "/" + NUMERO_RIF.ToString().Trim();
    }

    public bool AGGIORNA_RETANN(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      string CODPOS,
      string MAT,
      int PRORAP,
      int ANNO,
      Decimal RETIMP,
      Decimal RETOCC,
      Decimal RETFIG,
      string OPERAZIONE,
      bool FLGARR = false)
    {
      string strData1 = "";
      string strData2 = "";
      DataTable dataTable1 = new DataTable();
      try
      {
        if (FLGARR)
        {
          string strSQL1 = "SELECT COUNT(*) FROM RETANN WHERE CODPOS = " + CODPOS + " AND MAT = " + MAT + " AND PRORAP = " + PRORAP.ToString() + " AND ANNRET = " + ANNO.ToString();
          if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text)) == 0)
          {
            string strSQL2 = "SELECT DATDEC, DATCES FROM RAPLAV WHERE CODPOS = " + CODPOS + " AND MAT = " + MAT + " AND PRORAP = " + PRORAP.ToString();
            DataTable dataTable2 = objDataAccess.GetDataTable(strSQL2);
            if (dataTable2.Rows.Count > 0)
            {
              DateTime dateTime = Convert.ToDateTime(dataTable2.Rows[0]["DATDEC"]);
              strData1 = dateTime.Year != ANNO ? ANNO.ToString() + "-01-01" : (string) dataTable2.Rows[0]["DATDEC"];
              if (dataTable2.Rows[0]["DATCES"].ToString().Trim() != "")
              {
                dateTime = Convert.ToDateTime(dataTable2.Rows[0]["DATCES"]);
                strData2 = dateTime.Year != ANNO ? ANNO.ToString() + "-12-31" : (string) dataTable2.Rows[0]["DATCES"];
              }
              else
                strData2 = ANNO.ToString() + "-12-31";
            }
            string strSQL3 = "INSERT INTO RETANN (CODPOS, MAT, PRORAP, ANNRET, DATINI, DATFIN, RETIMP, RETOCC, RETFIG, RETUTILE, RETARRIMP, RETARROCC, FLAGRIC, ULTAGG, UTEAGG)" + " VALUES (" + CODPOS + ", " + MAT + ", " + PRORAP.ToString() + ", " + ANNO.ToString() + ", " + DBMethods.Db2Date(strData1) + ", " + DBMethods.Db2Date(strData2) + ", " + " 0.0, " + " 0.0, " + " 0.0, " + " 0.0, " + RETIMP.ToString().Replace(",", ".") + ", " + RETOCC.ToString().Replace(",", ".") + ", 'S', " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")";
            objDataAccess.WriteTransactionData(strSQL3, CommandType.Text);
          }
          else
          {
            string strSQL4 = " UPDATE RETANN SET FLAGRIC = 'S', RETARRIMP = RETARRIMP " + OPERAZIONE + RETIMP.ToString().Replace(",", ".") + ", RETARROCC = RETARROCC " + OPERAZIONE + RETOCC.ToString().Replace(",", ".") + ", " + " ULTAGG = CURRENT_TIMESTAMP, UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + " WHERE CODPOS = " + CODPOS + " AND MAT = " + MAT + " AND PRORAP = " + PRORAP.ToString() + " AND ANNRET = " + ANNO.ToString();
            objDataAccess.WriteTransactionData(strSQL4, CommandType.Text);
          }
        }
        else
        {
          string strSQL5 = "SELECT COUNT(*) FROM RETANN WHERE CODPOS = " + CODPOS + " AND MAT = " + MAT + " AND PRORAP = " + PRORAP.ToString() + " AND ANNRET = " + ANNO.ToString();
          if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL5, CommandType.Text)) == 0)
          {
            string strSQL6 = "SELECT DATDEC, DATCES FROM RAPLAV WHERE CODPOS = " + CODPOS + " AND MAT = " + MAT + " AND PRORAP = " + PRORAP.ToString();
            DataTable dataTable3 = objDataAccess.GetDataTable(strSQL6);
            if (dataTable3.Rows.Count > 0)
            {
              DateTime dateTime = Convert.ToDateTime(dataTable3.Rows[0]["DATDEC"]);
              strData1 = dateTime.Year != ANNO ? ANNO.ToString() + "-01-01" : (string) dataTable3.Rows[0]["DATDEC"];
              if (dataTable3.Rows[0]["DATCES"].ToString().Trim() != "")
              {
                dateTime = Convert.ToDateTime(dataTable3.Rows[0]["DATCES"]);
                strData2 = dateTime.Year != ANNO ? ANNO.ToString() + "-12-31" : (string) dataTable3.Rows[0]["DATCES"];
              }
              else
                strData2 = ANNO.ToString() + "-12-31";
            }
            string strSQL7 = "INSERT INTO RETANN (CODPOS, MAT, PRORAP, ANNRET, DATINI, DATFIN, RETIMP, RETOCC, RETFIG, RETUTILE, RETARRIMP, RETARROCC, FLAGRIC, ULTAGG, UTEAGG)" + " VALUES (" + CODPOS + ", " + MAT + ", " + PRORAP.ToString() + ", " + ANNO.ToString() + ", " + DBMethods.Db2Date(strData1) + ", " + DBMethods.Db2Date(strData2) + ", " + RETIMP.ToString().Replace(",", ".") + ", " + RETOCC.ToString().Replace(",", ".") + ", " + RETFIG.ToString().Replace(",", ".") + ", " + (RETIMP - RETOCC + RETFIG).ToString().Replace(",", ".") + ", " + " 0.0, " + " 0.0, 'S', " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")";
            objDataAccess.WriteTransactionData(strSQL7, CommandType.Text);
          }
          else
          {
            string strSQL8 = " UPDATE RETANN SET FLAGRIC = 'S', RETIMP = RETIMP " + OPERAZIONE + RETIMP.ToString().Replace(",", ".") + ", RETOCC = RETOCC " + OPERAZIONE + RETOCC.ToString().Replace(",", ".") + ", " + " RETFIG = RETFIG " + OPERAZIONE + RETFIG.ToString().Replace(",", ".") + ", RETUTILE = RETUTILE " + OPERAZIONE + (RETIMP - RETOCC + RETFIG).ToString().Replace(",", ".") + ", " + " ULTAGG = CURRENT_TIMESTAMP, UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + " WHERE CODPOS = " + CODPOS + " AND MAT = " + MAT + " AND PRORAP = " + PRORAP.ToString() + " AND ANNRET = " + ANNO.ToString();
            objDataAccess.WriteTransactionData(strSQL8, CommandType.Text);
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }
  }
}
