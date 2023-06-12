// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.WriteDIPA
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;

namespace TFI.DAL.AziendaConsulente
{
  public class WriteDIPA
  {
    private static readonly ILog log = LogManager.GetLogger("RollingFile");
    private static readonly ILog TrackLog = LogManager.GetLogger("Track");
    public static string ErrorMessage = string.Empty;

    public static int WRITE_INSERT_DENTES(
      DataLayer db,
      string USERNAME,
      string CODPOS,
      int ANNDEN,
      int MESDEN,
      string DATAPE,
      string UTEAPE,
      string DATCHI,
      string UTECHI,
      string TIPMOV,
      string TIPDEN,
      string STADEN,
      Decimal IMPDIS,
      Decimal IMPRET,
      Decimal IMPOCC,
      Decimal IMPFIG,
      Decimal IMPCON,
      Decimal IMPADDREC,
      Decimal IMPABB,
      Decimal IMPASSCON,
      Decimal IMPDEC,
      string DATCONMOV,
      string CODCAUMOV,
      string NUMMOV,
      int NUMRIGDET,
      string DATSCA,
      string ANNBILMOV,
      int CODMODPAG,
      Decimal IMPVER,
      string DATVER,
      string UFFPOS,
      string CITDIC,
      string PRODIC,
      Decimal ABIDIC,
      Decimal CABDIC,
      string NUMCC,
      int NUMMAV,
      int CODLINE,
      string ESIRET,
      string RICSANUTE,
      string DATERO)
    {
      try
      {
        string strSQL1 = " SELECT VALUE(MAX(PRODEN), 0) + 1  FROM DENTES " + " WHERE CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString();
        int int32 = Convert.ToInt32(db.Get1ValueFromSQL(strSQL1, CommandType.Text));
        string str1 = " INSERT INTO DENTES (" + "CODPOS, " + "ANNDEN, " + "MESDEN, " + "PRODEN, " + "DATAPE, " + "UTEAPE, " + "DATCHI, " + "UTECHI, " + "TIPMOV, " + "TIPDEN, " + "STADEN, " + "IMPDIS, " + "IMPRET, " + "IMPOCC, " + "IMPFIG, " + "IMPCON, " + "IMPADDREC, " + "IMPABB, " + "IMPASSCON, " + "IMPDEC, " + "DATCONMOV, " + "CODCAUMOV, " + "NUMMOV, " + "NUMRIGDET, " + "DATSCA, " + "ANNBILMOV, " + "CODMODPAG, " + "IMPVER, " + "DATVER, " + "UFFPOS, " + "CITDIC, " + "PRODIC, " + "ABIDIC, " + "CABDIC, " + "NUMCC, " + "NUMMAV, " + "CODLINE, " + "ESIRET, " + "RICSANUTE, " + "DATEROARR, " + "ULTAGG, " + "UTEAGG) " + "VALUES ( " + CODPOS + ", " + ANNDEN.ToString() + ", " + MESDEN.ToString() + ", " + int32.ToString() + ", ";
        string str2 = (!string.IsNullOrEmpty(DATAPE) ? str1 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATAPE)) + ", " : str1 + "NULL, ") + DBMethods.DoublePeakForSql(UTEAPE) + ", ";
        string str3 = (!string.IsNullOrEmpty(DATCHI) ? str2 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCHI)) + ", " + DBMethods.DoublePeakForSql(UTECHI) + ", " : str2 + "Null, Null, ") + DBMethods.DoublePeakForSql(TIPMOV) + ", " + DBMethods.DoublePeakForSql(TIPDEN) + ", " + DBMethods.DoublePeakForSql(STADEN) + ", " + IMPDIS.ToString().Replace(",", ".") + ", " + IMPRET.ToString().Replace(",", ".") + ", " + IMPOCC.ToString().Replace(",", ".") + ", " + IMPFIG.ToString().Replace(",", ".") + ", " + IMPCON.ToString().Replace(",", ".") + ", " + IMPADDREC.ToString().Replace(",", ".") + ", " + IMPABB.ToString().Replace(",", ".") + ", " + IMPASSCON.ToString().Replace(",", ".") + ", " + IMPDEC.ToString().Replace(",", ".") + ", ";
        string str4 = !string.IsNullOrEmpty(DATCONMOV) ? str3 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCONMOV)) + ", " : str3 + "NULL, ";
        string str5 = !string.IsNullOrEmpty(CODCAUMOV) ? str4 + DBMethods.DoublePeakForSql(CODCAUMOV) + ", " : str4 + "NULL, ";
        string str6 = (!string.IsNullOrEmpty(NUMMOV) ? str5 + DBMethods.DoublePeakForSql(NUMMOV) + ", " : str5 + "NULL, ") + NUMRIGDET.ToString() + ", ";
        string str7 = !string.IsNullOrEmpty(DATSCA) ? str6 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATSCA)) + ", " : str6 + "NULL, ";
        string str8 = ANNBILMOV != null ? str7 + ANNBILMOV + ", " : str7 + "NULL, ";
        string str9 = (CODMODPAG != 0 ? str8 + CODMODPAG.ToString() + ", " : str8 + "NULL, ") + IMPVER.ToString().Replace(",", ".") + ", ";
        string str10 = (!string.IsNullOrEmpty(DATVER) ? str9 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATVER)) + ", " : str9 + "NULL, ") + DBMethods.DoublePeakForSql(UFFPOS) + ", " + DBMethods.DoublePeakForSql(CITDIC) + ", " + DBMethods.DoublePeakForSql(PRODIC) + ", ";
        string str11 = !(ABIDIC == 0M) ? str10 + ABIDIC.ToString() + ", " : str10 + "NULL, ";
        string str12 = (!(CABDIC == 0M) ? str11 + CABDIC.ToString() + ", " : str11 + "NULL, ") + DBMethods.DoublePeakForSql(NUMCC) + ", ";
        string str13 = NUMMAV != 0 ? str12 + NUMMAV.ToString() + ", " : str12 + "NULL, ";
        string str14 = (CODLINE != 0 ? str13 + CODLINE.ToString() + ", " : str13 + "NULL, ") + DBMethods.DoublePeakForSql(ESIRET) + ", ";
        string str15 = !string.IsNullOrEmpty(RICSANUTE) ? str14 + DBMethods.DoublePeakForSql(RICSANUTE) + ", " : str14 + "NULL, ";
        string strSQL2 = (!string.IsNullOrEmpty(DATERO) ? str15 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATERO)) + ", " : str15 + "NULL, ") + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(USERNAME) + ")";
        if (db.WriteTransactionData(strSQL2, CommandType.Text))
          return int32;
        throw new Exception();
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static int WRITE_INSERT_DIPATESTMP(
      DataLayer db,
      string USERNAME,
      string CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int ANNFIA,
      int MESFIA,
      string VERSANTE,
      int IDAZI,
      Decimal IMPDISSAN,
      string DATSCA)
    {
      try
      {
        string strSQL1 = " SELECT VALUE(MAX(IDDIPA), 1499) + 1  FROM FIACONSIP.DIPATESTMP";
        int int32 = Convert.ToInt32(db.Get1ValueFromSQL(strSQL1, CommandType.Text));
        string strSQL2 = "SELECT VALUE(MAX(DECIMAL(VALUE(SUBSTR(NUMRIC, 6), '0'))), 0) AS NUMRIC " + " FROM FIACONSIP.DIPATESTMP WHERE SUBSTR(NUMRIC, 0, 5) = " + DateTime.Now.Year.ToString();
        string str1 = db.Get1ValueFromSQL(strSQL2, CommandType.Text);
        string str2 = Convert.ToInt32("0" + str1) <= 0 ? "1" : Convert.ToString(Convert.ToInt32(str1) + 1);
        int num = DateTime.Now.Year;
        string str3 = num.ToString() + "/" + str2.ToString();
        string str4 = "01/" + ANNFIA.ToString() + "/" + int32.ToString();
        string str5 = " INSERT INTO FIACONSIP.DIPATESTMP (IDDIPA, CODPOS, ANNDEN, MESDEN, PRODEN, ANNFIA, MESFIA, VERSANTE," + " IDAZI, IDISC, IMPDISSAN, DATSCA, DATCHI, NUMMOV, DATCONMOV, NUMRIC, " + " ULTAGG, UTEAGG)" + " VALUES ( " + int32.ToString() + ", ";
        string str6 = (IDAZI != 0 ? str5 + CODPOS + ", " : str5 + "NULL, ") + ANNDEN.ToString() + ", " + MESDEN.ToString() + ", " + PRODEN.ToString() + ", " + ANNFIA.ToString() + ", " + MESFIA.ToString() + ", " + " 'A', ";
        string str7 = (IDAZI != 0 ? str6 + IDAZI.ToString() + ", " : str6 + "NULL, ") + "NULL, " + IMPDISSAN.ToString().Replace(",", ".") + ", ";
        string strSQL3 = (!string.IsNullOrEmpty(DATSCA) ? str7 + "'" + DBMethods.Db2Date(DATSCA) + "', " : str7 + "NULL, ") + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(str4) + ", " + "CURRENT_DATE, " + DBMethods.DoublePeakForSql(str3) + ", CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(USERNAME) + ")";
        if (!db.WriteTransactionData(strSQL3, CommandType.Text))
          throw new Exception();
        num = int32;
        return num;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool WRITE_DELETE_DENDET(
      DataLayer db,
      string CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int ANNCOM,
      string TIPMOV)
    {
      bool flag;
      try
      {
        string strSQL1 = "DELETE FROM DENDETALI WHERE CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString();
        flag = db.WriteTransactionData(strSQL1, CommandType.Text);
        if (flag)
        {
          string strSQL2 = "DELETE FROM DENDETSOS WHERE CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString();
          flag = db.WriteTransactionData(strSQL2, CommandType.Text);
          if (flag)
          {
            string strSQL3 = "DELETE FROM DENDET WHERE CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString();
            if (TIPMOV == "AR" & ANNCOM != 0)
              strSQL3 = strSQL3 + " AND ANNCOM = " + ANNCOM.ToString();
            if (db.WriteTransactionData(strSQL3, CommandType.Text))
              return true;
            throw new Exception();
          }
        }
      }
      catch (Exception ex)
      {
        throw;
      }
      return flag;
    }

    public static bool WRITE_DELETE_DIPADETTMP(
      DataLayer db,
      int IDDIPA,
      string CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN)
    {
      try
      {
        string strSQL = "DELETE FROM FIACONSIP.DIPADETTMP WHERE IDDIPA = " + IDDIPA.ToString() + " AND CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString();
        if (db.WriteTransactionData(strSQL, CommandType.Text))
          return true;
        throw new Exception();
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool WRITE_INSERT_DENDET(
      DataLayer db,
      List<RetribuzioneRDL> listaReport,
      List<ParametriGenerali> listaParametriGenerali,
      TFI.OCM.Utente.Utente utente,
      string CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int MAT,
      string TIPMOV,
      string DAL,
      string AL,
      string DATERO,
      Decimal IMPRET,
      Decimal IMPOCC,
      Decimal IMPFIG,
      Decimal IMPABB,
      Decimal IMPASSCON,
      Decimal IMPCON,
      Decimal IMPMIN,
      string PREV,
      int PROMOD,
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
      int PRORAP,
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
      string TIPDEN,
      ref int intProDenDet)
    {
      int? outcome = new int?();
      int[,] numArray = new int[1, 2];
      Decimal num1 = 0.0M;
      try
      {
        if (listaParametriGenerali == null)
          listaParametriGenerali = DenunciaMensileDAL.GetParametriGenerali(CODPOS.ToString(), out outcome);
        int? nullable = outcome;
        int num2 = 1;
        if (nullable.GetValueOrDefault() == num2 & nullable.HasValue)
          throw new Exception();
        num1 = WriteDIPA.GetImportoParametro(listaParametriGenerali, 5, DAL);
        string strSQL1 = " SELECT VALUE(MAX(PRODENDET), 0) + 1  FROM DENDET " + " WHERE CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND MAT = " + MAT.ToString();
        int int32 = Convert.ToInt32(db.Get1ValueFromSQL(strSQL1, CommandType.Text));
        intProDenDet = int32;
        if (PREV == "S")
        {
          string strSQL2 = "UPDATE MODPREDET SET PRODEN = " + PRODEN.ToString() + ", PRODENDET = " + int32.ToString() + ", IMPRET = " + IMPRET.ToString().Replace(",", ".") + ", IMPOCC = " + IMPOCC.ToString().Replace(",", ".") + ", " + "IMPFIG = " + IMPFIG.ToString().Replace(",", ".") + " WHERE CODPOS = " + CODPOS + " AND " + "ANNDEN = " + ANNDEN.ToString() + " And MESDEN = " + MESDEN.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND TIPMOV = 'DP' AND PROMOD = (SELECT MAX(PROMOD) FROM MODPREDET WHERE CODPOS = " + CODPOS + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + ") AND PRODEN IS NULL AND " + "PRODENDET IS NULL";
          if (!db.WriteTransactionData(strSQL2, CommandType.Text))
            throw new Exception();
        }
        string str1 = "INSERT INTO DENDET (" + "CODPOS, " + "ANNDEN, " + "MESDEN, " + "PRODEN, " + "MAT, " + "PRODENDET, " + "TIPMOV, " + "DAL, " + "AL, " + "DATERO, " + "IMPRET, " + "IMPOCC, " + "IMPFIG, " + "IMPABB, " + "IMPASSCON, " + "IMPCON, " + "IMPMIN, " + "PREV, " + "DATDEC, " + "DATCES, " + "NUMGGAZI, " + "NUMGGFIG, " + "NUMGGPER, " + "NUMGGDOM, " + "NUMGGSOS, " + "NUMGGCONAZI, " + "IMPSCA, " + "IMPTRAECO, " + "ETA65 ," + "TIPRAP, " + "FAP ," + "PERFAP ," + "IMPFAP ," + "PERPAR, " + "PERAPP, " + "PRORAP, " + "CODCON, " + "PROCON, " + "TIPSPE, " + "CODLOC, " + "PROLOC, " + "CODLIV, " + "CODGRUASS, " + "CODQUACON, " + "ALIQUOTA, " + "DATNAS, " + "ANNCOM, " + "TIPDEN, " + "ULTAGG, " + "UTEAGG) " + " VALUES (" + CODPOS + ", " + ANNDEN.ToString() + ", " + MESDEN.ToString() + ", " + PRODEN.ToString() + ", " + MAT.ToString() + ", " + int32.ToString() + ", " + DBMethods.DoublePeakForSql(TIPMOV) + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DAL)) + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(AL)) + ", ";
        string str2 = (string.IsNullOrEmpty(DATERO) ? str1 + "Null, " : str1 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATERO)) + ", ") + IMPRET.ToString().Replace(",", ".") + ", " + IMPOCC.ToString().Replace(",", ".") + ", " + IMPFIG.ToString().Replace(",", ".") + ", " + IMPABB.ToString().Replace(",", ".") + ", " + IMPASSCON.ToString().Replace(",", ".") + ", " + "ROUND(" + IMPRET.ToString().Replace(",", ".") + " * " + ALIQUOTA.ToString().Replace(",", ".") + " / 100, 2), " + IMPMIN.ToString().Replace(",", ".") + ", ";
        string str3 = !(PREV != "N") ? str2 + "'N', " : str2 + "'S', ";
        string str4 = string.IsNullOrEmpty(DATDEC) ? str3 + "NULL, " : str3 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATDEC)) + ", ";
        string str5 = (string.IsNullOrEmpty(DATCES) ? str4 + "NULL, " : str4 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCES)) + ", ") + NUMGGAZI.ToString().Replace(",", ".") + ", " + NUMGGFIG.ToString().Replace(",", ".") + ", " + NUMGGPER.ToString().Replace(",", ".") + ", " + NUMGGDOM.ToString().Replace(",", ".") + ", " + NUMGGSOS.ToString().Replace(",", ".") + ", " + NUMGGCONAZI.ToString().Replace(",", ".") + ", " + IMPSCA.ToString().Replace(",", ".") + ", " + IMPTRAECO.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(ETA65) + ", " + TIPRAP.ToString() + ", " + DBMethods.DoublePeakForSql(FAP) + ", ";
        if (FAP == "S")
          IMPFAP = IMPRET / 100M * PERFAP;
        string str6 = str5 + PERFAP.ToString().Replace(",", ".") + ", " + IMPFAP.ToString().Replace(",", ".") + ", " + PERPAR.ToString().Replace(",", ".") + ", " + PERAPP.ToString().Replace(",", ".") + ", " + PRORAP.ToString() + ", " + CODCON.ToString() + ", " + PROCON.ToString() + ", " + DBMethods.DoublePeakForSql(TIPSPE) + ", " + CODLOC.ToString() + ", " + PROLOC.ToString() + ", " + CODLIV.ToString() + ", " + CODGRUASS.ToString() + ", " + CODQUACON.ToString() + ", " + ALIQUOTA.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATNAS)) + ", ";
        string str7 = ANNCOM != 0 ? str6 + ANNCOM.ToString() + ", " : str6 + "NULL, ";
        string strSQL3 = (!string.IsNullOrEmpty(TIPDEN) ? str7 + DBMethods.DoublePeakForSql(TIPDEN) + ", " : str7 + "NULL, ") + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(utente.Username) + ")";
        if (!db.WriteTransactionData(strSQL3, CommandType.Text))
          throw new Exception();
        if (PREV == "S" & PROMOD > 0 & TIPMOV == "DP")
        {
          string strSQL4 = "SELECT CODSTAPRE FROM MODPRE WHERE CODPOS = " + CODPOS + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND PROMOD = " + PROMOD.ToString();
          if (Convert.ToInt16(db.Get1ValueFromSQL(strSQL4, CommandType.Text)) == (short) 0)
          {
            string str8 = "UPDATE MODPREDET SET IMPRET = " + IMPRET.ToString().Replace(",", ".") + ", " + "IMPOCC = " + IMPOCC.ToString().Replace(",", ".") + ", " + "IMPFIG = " + IMPFIG.ToString().Replace(",", ".") + ", " + "IMPABB = " + IMPABB.ToString().Replace(",", ".") + ", " + "IMPASSCON = " + IMPASSCON.ToString().Replace(",", ".") + ", " + "IMPCON = ROUND(" + IMPRET.ToString().Replace(",", ".") + " * " + ALIQUOTA.ToString().Replace(",", ".") + " / 100, 2), " + "IMPMIN = " + IMPMIN.ToString().Replace(",", ".") + ", " + "IMPRETPRV = NULL, IMPOCCPRV = NULL, IMPFIGPRV = NULL, " + "PRODEN = " + PRODEN.ToString() + ", " + "PRODENDET = " + int32.ToString() + " WHERE CODPOS = " + CODPOS + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND PROMOD = " + PROMOD.ToString() + " AND DAL = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DAL)) + " AND AL = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(AL)) + " AND TIPMOV = " + DBMethods.DoublePeakForSql(TIPMOV);
            string strSQL5 = !(TIPMOV == "AR") ? str8 + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() : str8 + " AND ANNCOM = " + ANNCOM.ToString();
            if (!db.WriteTransactionData(strSQL5, CommandType.Text))
              throw new Exception();
          }
        }

        if(listaReport != null)
            listaReport.First(report => report.Mat == MAT && report.Dal == DAL && report.Al == AL).ProDenDet = intProDenDet;
        return Convert.ToBoolean(int32);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static int WRITE_INSERT_DIPADETTMP(
      DataLayer db,
      TFI.OCM.Utente.Utente utente,
      string CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int PRODENDET,
      int MAT,
      int IDDIPA,
      int ANNFIA,
      int MESFIA,
      int IDAZI,
      int IDISC,
      int IDADE,
      int IDPOL,
      Decimal IMPSAN)
    {
      try
      {
        string strSQL1 = " SELECT VALUE(MAX(IDDIPADET), 0) + 1  FROM FIACONSIP.DIPADETTMP";
        int int32 = Convert.ToInt32(db.Get1ValueFromSQL(strSQL1, CommandType.Text));
        string str1 = "01/" + ANNFIA.ToString() + "/" + IDDIPA.ToString();
        string str2 = "INSERT INTO FIACONSIP.DIPADETTMP (" + " IDDIPADET, IDDIPA,\tCODPOS,\tANNDEN,\tMESDEN,\tPRODEN, " + " PRODENDET, MAT, ANNFIA, MESFIA, VERSANTE, IDAZI, IDISC, IDADE, " + " IDPOL, IMPSAN, NUMMOV, DATCONMOV, ULTAGG, UTEAGG, IMPABB, IMPDAABB)" + " VALUES (" + int32.ToString() + ", " + IDDIPA.ToString() + ", ";
        string str3 = (!(Convert.ToDecimal(IDAZI) == 0M) ? str2 + CODPOS + ", " : str2 + "NULL, ") + ANNDEN.ToString() + ", " + MESDEN.ToString() + ", " + PRODEN.ToString() + ", " + PRODENDET.ToString() + ", ";
        string str4 = (MAT != 0 ? str3 + MAT.ToString() + ", " : str3 + "NULL, ") + ANNFIA.ToString() + ", " + MESFIA.ToString() + ", " + " 'A', ";
        string str5 = !(Convert.ToDecimal(IDAZI) == 0M) ? str4 + IDAZI.ToString() + ", " : str4 + "NULL, ";
        string strSQL2 = (!(Convert.ToDecimal(IDISC) == 0M) ? str5 + IDISC.ToString() + ", " : str5 + "NULL, ") + IDADE.ToString() + ", " + IDPOL.ToString() + ", " + IMPSAN.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(str1) + ", " + "CURRENT_DATE, " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(utente.Username) + ", " + " 0, " + IMPSAN.ToString().Replace(",", ".") + ")";
        if (db.WriteTransactionData(strSQL2, CommandType.Text))
          return int32;
        throw new Exception();
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static Decimal GetImportoParametro(
      List<ParametriGenerali> listaParametriGenerali,
      int codPar,
      string dal,
      string flag = "S")
    {
      DateTime data = DateTime.Parse(dal);
      Decimal importoParametro;
      if (flag == "S")
      {
        List<ParametriGenerali> list = listaParametriGenerali.Where<ParametriGenerali>((Func<ParametriGenerali, bool>) (p => p.CodPar == (Decimal) codPar && DateTime.Parse(p.DataInizio) <= data)).ToList<ParametriGenerali>();
        importoParametro = list[0].Valore != null ? Convert.ToDecimal(list[0].Valore.Trim()) : 0M;
      }
      else
        importoParametro = !string.IsNullOrEmpty(listaParametriGenerali[0].Valore) ? Convert.ToDecimal(listaParametriGenerali[0].Valore.Trim()) : 0M;
      return importoParametro;
    }

    public static Decimal FIA_GET_IMP_FORMULE_E_DIPA(
      DataLayer db,
      TFI.OCM.Utente.Utente utente,
      string codPos,
      string IDADE,
      string IDISC,
      int ANNFIA,
      int MESFIA,
      string VERSANTE,
      ref int IDPOLIZZA,
      string CODFIS,
      int CODQUACON,
      int MATRICOLA,
      int IDTIPPAG_ADE,
      string DATINIRDL,
      bool GET_IMPORTO_DIPA = false,
      int IDDIPA_CORRENTE = 0,
      bool SALVA_DATI_POLIZZA = false,
      DataTable DT = null,
      bool blnErr = false,
      int IDDIPADET_CORRENTE = 0)
    {
      string strData1 = "01/01/" + ANNFIA.ToString();
      string strData2 = "31/12/" + ANNFIA.ToString();
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      int num5 = 0;
      Decimal num6 = 0M;
      int num7 = 0;
      Decimal num8 = 0M;
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      try
      {
        if (IDADE == "5863" & ANNFIA == 2019 & MESFIA == 12 | IDADE == "5872" & ANNFIA == 2019 & MESFIA == 12 || ANNFIA < 2015)
          return 0M;
        string strSQL1 = string.Format("SELECT COUNT(*) FROM FIACONSIP.RINUNCEFIANEW WHERE CODPOS = {0} AND MAT = {1} AND {2} BETWEEN ANNODA AND ANNOA", (object) codPos, (object) MATRICOLA, (object) ANNFIA);
        if (Convert.ToInt32(db.Get1ValueFromSQL(strSQL1, CommandType.Text)) > 0)
          return 0M;
        if (string.IsNullOrEmpty(IDADE))
        {
          if (!string.IsNullOrEmpty(DATINIRDL.ToString().Trim()))
          {
            if ((Convert.ToDateTime(DATINIRDL) - Convert.ToDateTime("01/" + MESFIA.ToString() + "/" + ANNFIA.ToString())).Days <= 180)
              return 0M;
            if (!WriteDIPA.WRITE_ADESIONI(db, codPos, MATRICOLA, CODFIS, ANNFIA.ToString() + "-" + MESFIA.ToString().PadLeft(2, '0') + "-01", CODQUACON, ref IDADE, IDTIPPAG_ADE.ToString(), ANNFIA.ToString(), IDISC))
              throw new Exception("Errore inserimento adesione Fondo Sanitario");
          }
        }
        else if (!WriteDIPA.WRITE_ADESIONI(db, codPos, MATRICOLA, CODFIS, ANNFIA.ToString() + "-" + MESFIA.ToString().PadLeft(2, '0') + "-01", CODQUACON, ref IDADE, IDTIPPAG_ADE.ToString(), ANNFIA.ToString(), IDISC))
          throw new Exception("Errore inserimento adesione Fondo Sanitario");
        if (DT == null)
        {
          DT = new DataTable();
        }
        else
        {
          DT.Columns.Clear();
          DT.Rows.Clear();
        }
        string strSQL2 = "SELECT A.*, " + " (SELECT CONVENZIONE FROM FIACONSIP.TIPISC WHERE IDTIPISC=A.IDTIPISC) AS CONVENZIONE " + " FROM FIACONSIP.ADESIONI A " + " WHERE A.IDADE = " + IDADE;
        DT = db.GetDataTable(strSQL2);
        int int32_1 = Convert.ToInt32(DT.Rows[0]["IDTIPISC"]);
        string str1 = DT.Rows[0]["CONVENZIONE"].ToString();
        IDISC = DT.Rows[0][nameof (IDISC)].ToString();
        if (DT.Rows[0]["CODPOS"].ToString().Trim() != "")
          num4 = Convert.ToInt32(DT.Rows[0]["CODPOS"]);
        if (DT.Rows[0]["MAT"].ToString().Trim() != "")
          num5 = Convert.ToInt32(DT.Rows[0]["MAT"]);
        if (DT.Rows[0]["IDAZI"].ToString().Trim() != "")
          num1 = Convert.ToInt32(DT.Rows[0]["IDAZI"]);
        string strSQL3 = "SELECT IDTIPPAG FROM FIACONSIP.AZI WHERE IDAZI = " + num1.ToString();
        int num9 = (int) Math.Round(Convert.ToDecimal("0" + db.Get1ValueFromSQL(strSQL3, CommandType.Text)));
        string strSQL4 = "SELECT IDTIPPAG FROM FIACONSIP.ADESIONITIPPAG WHERE IDADE=" + IDADE + " ORDER BY ANNODA DESC";
        int num10 = (int) Math.Round(Convert.ToDecimal("0" + db.Get1ValueFromSQL(strSQL4, CommandType.Text)));
        string strSQL5 = "SELECT MAXETAFIGCAR, MAXETAFIGSTU FROM FIACONSIP.FORMULEIMP WHERE ANNO=" + ANNFIA.ToString() + " AND IDTIPISC=" + int32_1.ToString() + " AND CONVENZIONE=" + DBMethods.DoublePeakForSql(str1);
        DT = db.GetDataTable(strSQL5);
        if (DT.Rows.Count == 0)
          throw new Exception("Per l'anno " + ANNFIA.ToString() + " non sono presenti i parametri relativi alle formule");
        int int32_2 = Convert.ToInt32(DT.Rows[0]["MAXETAFIGSTU"]);
        int int32_3 = Convert.ToInt32(DT.Rows[0]["MAXETAFIGCAR"]);
        string strSQL6 = "SELECT IDPOL FROM FIACONSIP.POLIZZE WHERE IDADE = " + IDADE + " AND ANNPOL = " + ANNFIA.ToString();
        int IDPOL = (int) Math.Round(Convert.ToDecimal("0" + db.Get1ValueFromSQL(strSQL6, CommandType.Text)));
        DT.Clear();
        string strSQL7 = "SELECT F.FORMULA, A.*, 0 AS NUMFIGLI, 0 AS MESI, 0.0 AS IMPAZI, 0.0 AS IMPISC, 0.0 AS IMPORTO, AZIPAGQUOISC, 0.0 AS IMPADE, 0.0 AS IMPCAS " + " FROM FIACONSIP.ADESIONIFOR A, FIACONSIP.FORMULE F WHERE F.IDFORMULA = A.IDFORMULA AND A.IDADE=" + IDADE + " AND A.DATINI <= '" + DBMethods.Db2Date(strData2) + "' AND A.DATFIN >= '" + DBMethods.Db2Date(strData1) + "'" + " ORDER BY F.ORDINE, VERSANTE, TIPQUO DESC";
        DT = db.GetDataTable(strSQL7);
        int num11 = DT.Rows.Count - 1;
        for (int index1 = 0; index1 <= num11; ++index1)
        {
          string strSQL8 = "";
          string strSQL9 = "";
          string strSQL10 = "";
          if (string.Compare(WriteDIPA.AggiungiApici(DBMethods.Db2Date(DT.Rows[index1]["DATINI"].ToString())), WriteDIPA.AggiungiApici(DBMethods.Db2Date(strData1)), false) < 0)
            DT.Rows[index1]["DATINI"] = (object) DBMethods.Db2Date(strData1);
          if (string.Compare(WriteDIPA.AggiungiApici(DBMethods.Db2Date(DT.Rows[index1]["DATFIN"].ToString())), WriteDIPA.AggiungiApici(DBMethods.Db2Date(strData2)), false) > 0)
            DT.Rows[index1]["DATFIN"] = (object) DBMethods.Db2Date(strData2);
          num3 = Convert.ToDateTime(DT.Rows[index1]["DATFIN"]).Month - Convert.ToDateTime(DT.Rows[index1]["DATINI"]).Month + 1;
          num2 = 0;
          if (DT.Rows[index1]["TIPQUO"] is string str2)
          {
            string strSQL11;
            string strSQL12;
            if (!(str2 == "I"))
            {
              if (str2 == "F")
              {
                string strSQL13 = "SELECT YEAR(" + WriteDIPA.AggiungiApici(DBMethods.Db2Date(DT.Rows[index1]["DATINI"].ToString())) + " - DATNAS) AS ETA, " + " STUDENTE FROM FIACONSIP.ADESIONIASS WHERE IDADE=" + IDADE + " AND IDGRAPAR = 2 " + " AND " + WriteDIPA.AggiungiApici(DBMethods.Db2Date(DT.Rows[index1]["DATINI"].ToString())) + " BETWEEN DATINI AND DATFIN";
                DataTable dataTable3 = db.GetDataTable(strSQL13);
                int num12 = dataTable3.Rows.Count - 1;
                for (int index2 = 0; index2 <= num12; ++index2)
                {
                  if (dataTable3.Rows[index2]["STUDENTE"].ToString().Trim().ToUpper() == "S")
                  {
                    if (Convert.ToInt32(dataTable3.Rows[index2]["ETA"]) > int32_2)
                      ++num2;
                  }
                  else if (Convert.ToInt32(dataTable3.Rows[index2]["ETA"]) > int32_3)
                    ++num2;
                }
                strSQL11 = "SELECT ROUND(((((IMPFIG * " + num2.ToString() + "))/12)*" + num3.ToString() + "), 2) AS IMPFIG FROM FIACONSIP.FORMULEIMP " + " WHERE IDFORMULA = " + DT.Rows[index1]["IDFORMULA"]?.ToString() + " AND ANNO = " + ANNFIA.ToString() + " AND IDTIPISC = " + int32_1.ToString() + " AND CONVENZIONE = " + DBMethods.DoublePeakForSql(str1);
                strSQL12 = "SELECT ROUND(((((IMPCASFIG * " + num2.ToString() + "))/12)*" + num3.ToString() + "), 2) AS IMPCASFIG FROM FORMULEIMP " + " WHERE IDFORMULA = " + DT.Rows[index1]["IDFORMULA"]?.ToString() + " AND ANNO = " + ANNFIA.ToString() + " AND IDTIPISC = " + int32_1.ToString() + " AND CONVENZIONE = " + DBMethods.DoublePeakForSql(str1);
              }
              else
                goto label_45;
            }
            else
            {
              string strSQL14 = "SELECT COUNT(*) FROM FIACONSIP.FORMULEIMP " + " WHERE IDFORMULA = " + DT.Rows[index1]["IDFORMULA"]?.ToString() + " AND ANNO = " + ANNFIA.ToString() + " AND IDTIPISC = " + int32_1.ToString() + " AND CONVENZIONE = " + DBMethods.DoublePeakForSql(str1) + " AND IMPAZI > 0";
              if (Convert.ToDecimal("0" + db.Get1ValueFromSQL(strSQL14, CommandType.Text)) > 0M)
              {
                strSQL9 = "SELECT ROUND((((IMPISC)/12)*" + num3.ToString() + "), 2) AS IMPISC FROM FIACONSIP.FORMULEIMP " + " WHERE IDFORMULA = " + DT.Rows[index1]["IDFORMULA"]?.ToString() + " AND ANNO = " + ANNFIA.ToString() + " AND IDTIPISC = " + int32_1.ToString() + " AND CONVENZIONE = " + DBMethods.DoublePeakForSql(str1);
                strSQL8 = "SELECT ROUND((((IMPAZI-IMPADE)/12)*" + num3.ToString() + "), 2) + IMPADE AS IMPAZI FROM FIACONSIP.FORMULEIMP " + " WHERE IDFORMULA = " + DT.Rows[index1]["IDFORMULA"]?.ToString() + " AND ANNO = " + ANNFIA.ToString() + " AND IDTIPISC = " + int32_1.ToString() + " AND CONVENZIONE = " + DBMethods.DoublePeakForSql(str1);
              }
              else
              {
                strSQL9 = "SELECT ROUND((((IMPISC-IMPADE)/12)*" + num3.ToString() + "), 2) + IMPADE AS IMPISC FROM FIACONSIP.FORMULEIMP " + " WHERE IDFORMULA = " + DT.Rows[index1]["IDFORMULA"]?.ToString() + " AND ANNO = " + ANNFIA.ToString() + " AND IDTIPISC = " + int32_1.ToString() + " AND CONVENZIONE = " + DBMethods.DoublePeakForSql(str1);
                strSQL8 = "SELECT ROUND((((IMPAZI)/12)*" + num3.ToString() + "), 2) AS IMPAZI FROM FIACONSIP.FORMULEIMP " + " WHERE IDFORMULA = " + DT.Rows[index1]["IDFORMULA"]?.ToString() + " AND ANNO = " + ANNFIA.ToString() + " AND IDTIPISC = " + int32_1.ToString() + " AND CONVENZIONE = " + DBMethods.DoublePeakForSql(str1);
              }
              strSQL11 = "SELECT ROUND((((IMPAZI+IMPISC-IMPADE)/12)*" + num3.ToString() + "), 2) + IMPADE AS IMPTOTAZIISC FROM FIACONSIP.FORMULEIMP " + " WHERE IDFORMULA = " + DT.Rows[index1]["IDFORMULA"]?.ToString() + " AND ANNO = " + ANNFIA.ToString() + " AND IDTIPISC = " + int32_1.ToString() + " AND CONVENZIONE = " + DBMethods.DoublePeakForSql(str1);
              strSQL10 = "SELECT IMPADE FROM FIACONSIP.FORMULEIMP " + " WHERE IDFORMULA = " + DT.Rows[index1]["IDFORMULA"]?.ToString() + " AND ANNO = " + ANNFIA.ToString() + " AND IDTIPISC = " + int32_1.ToString() + " AND CONVENZIONE = " + DBMethods.DoublePeakForSql(str1);
              strSQL12 = "SELECT ROUND((((IMPCASANN)/12)*" + num3.ToString() + "), 2) AS IMPCASANN FROM FIACONSIP.FORMULEIMP " + " WHERE IDFORMULA = " + DT.Rows[index1]["IDFORMULA"]?.ToString() + " AND ANNO = " + ANNFIA.ToString() + " AND IDTIPISC = " + int32_1.ToString() + " AND CONVENZIONE = " + DBMethods.DoublePeakForSql(str1);
            }
            DT.Rows[index1]["MESI"] = (object) num3;
            DT.Rows[index1]["NUMFIGLI"] = (object) num2;
            DT.Rows[index1]["IMPORTO"] = (object) Convert.ToDecimal("0" + db.Get1ValueFromSQL(strSQL11, CommandType.Text));
            DT.Rows[index1]["IMPAZI"] = string.IsNullOrEmpty(strSQL8) ? (object) 0 : (object) Convert.ToDecimal("0" + db.Get1ValueFromSQL(strSQL8, CommandType.Text));
            DT.Rows[index1]["IMPISC"] = string.IsNullOrEmpty(strSQL9) ? (object) 0 : (object) Convert.ToDecimal("0" + db.Get1ValueFromSQL(strSQL9, CommandType.Text));
            num6 = Convert.ToDecimal(DT.Rows[index1]["IMPAZI"]);
            Decimal num13 = Convert.ToDecimal(DT.Rows[index1]["IMPORTO"]) - (Convert.ToDecimal(DT.Rows[index1]["IMPAZI"]) + Convert.ToDecimal(DT.Rows[index1]["IMPISC"]));
            if (num13 < -1M)
              throw new Exception("Importo azienda + importo iscritto diverso da importo totale");
            if (!(num13 < 0M))
            {
              if (!(num13 == 0M))
              {
                if (num13 < 1M)
                {
                  if (num6 != 0M)
                    num6 += Convert.ToDecimal(DT.Rows[index1]["IMPORTO"]) - num6 + Convert.ToDecimal(DT.Rows[index1]["IMPISC"]);
                  else
                    DT.Rows[index1]["IMPISC"] = DT.Rows[index1]["IMPORTO"];
                }
                else
                {
                  if (!(num13 == Convert.ToDecimal(DT.Rows[index1]["IMPORTO"])))
                    throw new Exception("Importo azienda + importo iscritto diverso da importo totale");
                  DT.Rows[index1]["IMPISC"] = (object) Convert.ToDecimal(DT.Rows[index1]["IMPORTO"]);
                  num6 = 0M;
                }
              }
            }
            else if (num6 != 0M)
              num6 += Convert.ToDecimal(DT.Rows[index1]["IMPORTO"]) - (Convert.ToDecimal(DT.Rows[index1]["IMPAZI"]) + Convert.ToDecimal(DT.Rows[index1]["IMPISC"]));
            else
              DT.Rows[index1]["IMPISC"] = DT.Rows[index1]["IMPORTO"];
            DT.Rows[index1]["IMPADE"] = string.IsNullOrEmpty(strSQL10) ? (object) 0 : (object) Convert.ToDecimal("0" + db.Get1ValueFromSQL(strSQL10, CommandType.Text));
            DT.Rows[index1]["IMPCAS"] = string.IsNullOrEmpty(strSQL12) ? (object) 0 : (object) Convert.ToDecimal("0" + db.Get1ValueFromSQL(strSQL12, CommandType.Text));
            continue;
          }
label_45:
          throw new Exception("Tipo quota non definito");
        }
        if (SALVA_DATI_POLIZZA)
        {
          if (IDPOL == 0)
          {
            string strSQL15 = "SELECT VALUE(MAX(IDPOL),0)+1 FROM FIACONSIP.POLIZZE";
            IDPOL = (int) Math.Round(Convert.ToDecimal("0" + db.Get1ValueFromSQL(strSQL15, CommandType.Text)));
            string str3 = "INSERT INTO FIACONSIP.POLIZZE (IDPOL, IDADE, ANNPOL, DATPOL, DATINI, DATFIN, IDISC, IDAZI, MAT, CODPOS, MESI, NUMFIGLI, UTEAGG, ULTAGG) " + " VALUES (" + IDPOL.ToString() + ", " + IDADE + ", " + ANNFIA.ToString() + ", " + " CURRENT_DATE, " + WriteDIPA.AggiungiApici(DBMethods.Db2Date(strData1)) + ", " + WriteDIPA.AggiungiApici(DBMethods.Db2Date(strData2)) + ", " + IDISC + ", ";
            string str4 = num1 == 0 ? str3 + "NULL, " : str3 + num1.ToString() + ", ";
            string str5 = num5 == 0 ? str4 + "NULL, " : str4 + num5.ToString() + ", ";
            string strSQL16 = (num4 == 0 ? str5 + "NULL, " : str5 + num4.ToString() + ", ") + num3.ToString() + ", " + num2.ToString() + ", " + DBMethods.DoublePeakForSql(utente.Username) + ", " + "CURRENT_TIMESTAMP)";
            if (!db.WriteTransactionData(strSQL16, CommandType.Text))
              throw new Exception("Attenzione...errore durante il calcolo dell\\' importo del Fondo Sanitario");
            IDPOLIZZA = IDPOL;
          }
          else
            IDPOLIZZA = IDPOL;
          if (IDPOL != -1)
          {
            string strSQL17 = "DELETE FROM FIACONSIP.POLIZZEFOR WHERE IDPOL = " + IDPOL.ToString();
            if (!db.WriteTransactionData(strSQL17, CommandType.Text))
              throw new Exception("Attenzione...errore durante il calcolo dell\\' importo del Fondo Sanitario");
            int num14 = DT.Rows.Count - 1;
            for (int index = 0; index <= num14; ++index)
            {
              if (Convert.ToDecimal(DT.Rows[index]["IMPORTO"]) > 0M)
              {
                string strSQL18 = "SELECT VALUE(MAX(IDPOLFOR),0)+1 FROM FIACONSIP.POLIZZEFOR";
                string str6 = "INSERT INTO FIACONSIP.POLIZZEFOR(IDPOLFOR, IDPOL, ANNPOL, IDISC, IDAZI, VERSANTE, IDTIPPAG, TIPQUO, MESI, NUMFIGLI, IDFORMULA, IMPORTO, IMPAZI, IMPISC, AZIPAGQUOISC, IMPADE, IMPCAS," + "UTEAGG,ULTAGG) VALUES(" + ((int) Math.Round(Convert.ToDecimal("0" + db.Get1ValueFromSQL(strSQL18, CommandType.Text)))).ToString() + ", " + IDPOL.ToString() + ", " + ANNFIA.ToString() + ", " + IDISC + ",";
                string str7 = (num1 == 0 ? str6 + "NULL," : str6 + num1.ToString() + ",") + DBMethods.DoublePeakForSql(DT.Rows[index][nameof (VERSANTE)].ToString()) + ", ";
                if (DT.Rows[index][nameof (VERSANTE)] is string str8)
                {
                  if (!(str8 == "A"))
                  {
                    if (str8 == "I")
                      str7 = str7 + num10.ToString() + ",";
                  }
                  else
                    str7 = str7 + num9.ToString() + ",";
                }
                string str9 = str7 + DBMethods.DoublePeakForSql(DT.Rows[index]["TIPQUO"].ToString()) + ", " + num3.ToString() + ", " + num2.ToString() + ", " + DT.Rows[index]["IDFORMULA"]?.ToString() + ", ";
                Decimal num15 = Convert.ToDecimal(DT.Rows[index]["IMPORTO"]);
                string str10 = num15.ToString().Trim().Replace(",", ".");
                string str11 = str9 + str10 + ", ";
                num15 = Convert.ToDecimal(num6);
                string str12 = num15.ToString().Trim().Replace(",", ".");
                string str13 = str11 + str12 + ", ";
                num15 = Convert.ToDecimal(DT.Rows[index]["IMPISC"]);
                string str14 = num15.ToString().Trim().Replace(",", ".");
                string str15 = str13 + str14 + ", " + DBMethods.DoublePeakForSql(DT.Rows[index]["AZIPAGQUOISC"].ToString().Trim()) + ", ";
                num15 = Convert.ToDecimal(DT.Rows[index]["IMPADE"]);
                string str16 = num15.ToString().Trim().Replace(",", ".");
                string str17 = str15 + str16 + ", ";
                num15 = Convert.ToDecimal(DT.Rows[index]["IMPCAS"]);
                string str18 = num15.ToString().Trim().Replace(",", ".");
                string strSQL19 = str17 + str18 + ", " + DBMethods.DoublePeakForSql(utente.Username) + ", CURRENT_TIMESTAMP)";
                if (!db.WriteTransactionData(strSQL19, CommandType.Text))
                  throw new Exception("Attenzione...errore durante il calcolo dell\\' importo del Fondo Sanitario");
              }
            }
            if (Convert.ToDecimal("0" + db.Get1ValueFromSQL("SELECT COUNT(*) FROM FIACONSIP.POLIZZEFOR WHERE IDPOL = " + IDPOL.ToString() + " AND IMPORTO<>(IMPAZI+IMPISC)", CommandType.Text)) > 0M)
              throw new Exception("Attenzione... Totale importo diverso da importo iscritto ed azienda");
            string strSQL20 = "UPDATE FIACONSIP.POLIZZE SET " + " IMPISCDOV=(SELECT SUM(IMPORTO) FROM FIACONSIP.POLIZZEFOR WHERE IDPOL=" + IDPOL.ToString() + " AND TIPQUO='I'), " + " IMPFIGDOV=(SELECT VALUE(SUM(IMPORTO), 0) FROM FIACONSIP.POLIZZEFOR WHERE IDPOL=" + IDPOL.ToString() + " AND TIPQUO='F'), " + " IMPTOTDOV=(SELECT SUM(IMPORTO) FROM FIACONSIP.POLIZZEFOR WHERE IDPOL=" + IDPOL.ToString() + "), " + " IMPADEDOV=(SELECT SUM(IMPADE) FROM FIACONSIP.POLIZZEFOR WHERE IDPOL=" + IDPOL.ToString() + "), " + " IMPCASDOV=(SELECT SUM(IMPCAS) FROM FIACONSIP.POLIZZEFOR WHERE IDPOL=" + IDPOL.ToString() + "), " + " NUMFIGLI=(SELECT MAX(NUMFIGLI) FROM FIACONSIP.POLIZZEFOR WHERE IDPOL=" + IDPOL.ToString() + "), " + " MESI=(SELECT MAX(MESI) FROM FIACONSIP.POLIZZEFOR WHERE IDPOL=" + IDPOL.ToString() + "),  " + " ULTAGG=CURRENT_TIMESTAMP, UTEAGG=" + DBMethods.DoublePeakForSql(utente.Username) + " WHERE IDPOL=" + IDPOL.ToString();
            if (!db.WriteTransactionData(strSQL20, CommandType.Text))
              throw new Exception("Attenzione...errore durante il calcolo dell\\' importo del Fondo Sanitario");
            WriteDIPA.FIA_AGGIORNA_TOTALI_POLIZZA(db, IDPOL);
            if (GET_IMPORTO_DIPA)
              num8 = WriteDIPA.CALCOLA_IMPPOL(db, IDPOL, VERSANTE);
          }
        }
        else if (GET_IMPORTO_DIPA)
        {
          if (IDPOL == 0)
          {
            int num16 = DT.Rows.Count - 1;
            for (int index = 0; index <= num16; ++index)
            {
              if (DT.Rows[index][nameof (VERSANTE)].ToString() == "A")
              {
                if (DT.Rows[index]["AZIPAGQUOISC"].ToString().Trim() == "N")
                  num8 += num6;
                else
                  num8 += Convert.ToDecimal(DT.Rows[index]["IMPORTO"]);
              }
            }
          }
          else
            num8 = WriteDIPA.CALCOLA_IMPPOL(db, IDPOL, VERSANTE);
        }
        if (!GET_IMPORTO_DIPA)
          return -1M;
        string strSQL21 = "SELECT VALUE(SUM(IMPSAN), 0) AS IMPPOL_DIPA, COUNT(*) AS DIPA_FATTI " + " FROM FIACONSIP.DIPADET WHERE IDPOL=" + IDPOL.ToString() + " AND IDADE = " + IDADE + " AND IDDIPA <> " + IDDIPA_CORRENTE.ToString() + " AND VERSANTE=" + DBMethods.DoublePeakForSql(VERSANTE) + " AND IMPSAN > 0" + " AND NUMMOV IS NOT NULL" + " AND DATMOVANN IS NULL";
        DataTable dataTable4 = db.GetDataTable(strSQL21);
        Decimal num17 = Convert.ToDecimal(dataTable4.Rows[0]["IMPPOL_DIPA"]);
        int int32_4 = Convert.ToInt32(dataTable4.Rows[0]["DIPA_FATTI"]);
        Decimal d = num8 - num17;
        string str19 = VERSANTE ?? "";
        if (!(str19 == "A"))
        {
          if (str19 == "I")
            num7 = num10;
        }
        else
          num7 = num9;
        if (d > 0M)
        {
          switch (num7)
          {
            case 1:
              d = Math.Round(d, 2);
              break;
            case 2:
              d = int32_4 != 0 ? (MESFIA >= 6 ? Math.Round(d, 2) : 0M) : (MESFIA >= 12 ? Math.Round(d, 2) : Math.Round(d / 2M, 2));
              break;
            case 12:
              int num18 = 12 - MESFIA + 1 + int32_4 - int32_4;
              d = num18 <= 1 ? Math.Round(d, 2) : Math.Round(d / (Decimal) num18, 2);
              break;
            default:
              throw new Exception("Attenzione... tipo di pagamento non gestito");
          }
        }
        return !(d < 0M) ? d : throw new Exception("Attenzione... importo non valido");
      }
      catch (Exception ex)
      {
        return -1M;
      }
    }

    public static bool WRITE_ADESIONI(
      DataLayer db,
      string codPos,
      int MAT,
      string CODFIS,
      string PRIMO_GIORNO_MESE,
      int CODQUACON,
      ref string IDADE,
      string IDTIPPAG,
      string ANNFIA,
      string IDISC)
    {
      bool flag1 = true;
      int num = 0;
      DataTable dataTable1 = new DataTable();
      if (db == null)
        db = new DataLayer();
      try
      {
        string strSQL1 = "SELECT COUNT(*) FROM FIACONSIP.AZI WHERE CODPOS=" + codPos;
        int int32_1;
        if (Convert.ToInt32("0" + db.Get1ValueFromSQL(strSQL1, CommandType.Text)) == 0)
        {
          string strSQL2 = "SELECT VALUE(MAX(IDAZI),0)+1 FROM FIACONSIP.AZI";
          int32_1 = Convert.ToInt32(db.Get1ValueFromSQL(strSQL2, CommandType.Text));
          IDTIPPAG = "12";
          string strSQL3 = "INSERT INTO FIACONSIP.AZI" + " (IDAZI,  IDTIPAZI,       RAGSOC, PARIVA, CODFIS, CODDUG, IND,    NUMCIV, CAP,    DENSTAEST,      " + " DENLOC, CODCOM, DENCOM, SIGPRO, TEL,    CELL,   FAX,    EMAIL,  PEC," + " CONVENZIONE,    FONTEDATI, IDTIPPAG," + " CODPOS, ULTAGG, UTEAGG, DATREG, CODUTEREG, CODUTECONF, DATCONF, INSDADIPA)" + " SELECT" + " " + int32_1.ToString() + ",  1 AS TIPAZI," + " RAGSOC, PARIVA, CODFIS," + " CODDUG, IND,    NUMCIV, CAP,    DENSTAEST,      DENLOC, CODCOM, DENCOM, SIGPRO, TEL1,    CELL,   " + " FAX,    EMAIL,  EMAILCERT," + " 'S' AS CONVENZIONE,    'CONTRIBUTI' AS FONTEDATI, " + IDTIPPAG + "," + " AZI.CODPOS, CURRENT_TIMESTAMP, AZI.CODPOS, CURRENT_DATE AS DATREG," + " 'AZIENDA' AS CODUTEREG, 'DIPA WEB' AS CODUTECONF, CURRENT_DATE AS DATCONF, 'S' " + " FROM AZI LEFT JOIN INDSED ON AZI.CODPOS = INDSED.CODPOS" + " AND INDSED.TIPIND = 2 AND CURRENT_DATE BETWEEN INDSED.DATINI AND INDSED.DATFIN" + " WHERE AZI.CODPOS=" + codPos;
          flag1 = db.WriteTransactionData(strSQL3, CommandType.Text);
        }
        else
        {
          string strSQL4 = "SELECT IDAZI FROM FIACONSIP.AZI WHERE CODPOS=" + codPos;
          int32_1 = Convert.ToInt32(db.Get1ValueFromSQL(strSQL4, CommandType.Text));
        }
        if (flag1)
        {
          string strSQL5 = "SELECT COUNT(*) FROM FIACONSIP.ADESIONI WHERE MAT=" + MAT.ToString() + " AND IDAZI = " + int32_1.ToString() + " AND '" + PRIMO_GIORNO_MESE + "' BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31') AND DATANN IS NULL";
          if (Convert.ToInt32("0" + db.Get1ValueFromSQL(strSQL5, CommandType.Text)) == 0)
          {
            string strSQL6 = "SELECT COUNT(*) FROM FIACONSIP.ADESIONI WHERE MAT=" + MAT.ToString() + " AND '" + PRIMO_GIORNO_MESE + "' BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31') AND DATANN IS NULL";
            if (Convert.ToInt32("0" + db.Get1ValueFromSQL(strSQL6, CommandType.Text)) > 0)
            {
              string strSQL7 = "SELECT IDADE, CODPOS FROM FIACONSIP.ADESIONI WHERE MAT=" + MAT.ToString() + " AND '" + PRIMO_GIORNO_MESE + "' BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31') AND DATANN IS NULL";
              DataTable dataTable2 = db.GetDataTable(strSQL7);
              bool flag2;
              if (dataTable2.Rows.Count > 0)
              {
                num = Convert.ToInt32(dataTable2.Rows[0][nameof (IDADE)].ToString().Trim());
                int int32_2 = Convert.ToInt32(dataTable2.Rows[0]["CODPOS"].ToString().Trim());
                string strSQL8 = "INSERT INTO FIACONSIP.ADESIONISTO (IDADE, MAT, CODPOS_NEW, CODPOS_OLD, UTEAGG, ULTAGG)" + " VALUES (" + num.ToString() + ", " + MAT.ToString() + ", " + codPos + ", " + int32_2.ToString() + ", " + codPos + ", CURRENT_TIMESTAMP)";
                flag2 = db.WriteTransactionData(strSQL8, CommandType.Text);
              }
              else
                flag2 = false;
              string strSQL9 = "UPDATE FIACONSIP.ADESIONI SET IDAZI = " + int32_1.ToString() + ", CODPOS = " + codPos + " WHERE IDADE = " + num.ToString();
              flag1 = db.WriteTransactionData(strSQL9, CommandType.Text);
              if (flag1)
              {
                string strSQL10 = "UPDATE FIACONSIP.ADESIONIFOR SET IDAZI = " + int32_1.ToString() + " WHERE IDADE = " + num.ToString();
                flag1 = db.WriteTransactionData(strSQL10, CommandType.Text);
              }
              if (flag1)
              {
                string strSQL11 = "UPDATE FIACONSIP.POLIZZE SET IDAZI = " + int32_1.ToString() + ", CODPOS = " + codPos + " WHERE IDADE = " + num.ToString() + " AND ANNPOL = " + ANNFIA;
                flag1 = db.WriteTransactionData(strSQL11, CommandType.Text);
              }
              if (flag1)
              {
                string strSQL12 = "UPDATE FIACONSIP.POLIZZEFOR SET IDAZI = " + int32_1.ToString() + " WHERE IDPOL = (SELECT IDPOL FROM FIACONSIP.POLIZZE WHERE IDADE = " + num.ToString() + " AND ANNPOL = " + ANNFIA + ")";
                flag1 = db.WriteTransactionData(strSQL12, CommandType.Text);
              }
            }
            else
            {
              string strSQL13 = "SELECT IDISC FROM FIACONSIP.ISCT WHERE CODFIS= '" + CODFIS + "'";
              int int32_3;
              if (Convert.ToInt32("0" + db.Get1ValueFromSQL(strSQL13, CommandType.Text)) == 0)
              {
                string strSQL14 = "SELECT VALUE(MAX(IDISC),0)+1 FROM FIACONSIP.ISCT";
                int32_3 = Convert.ToInt32(db.Get1ValueFromSQL(strSQL14, CommandType.Text));
                string strSQL15 = "INSERT INTO FIACONSIP.ISCT (" + " IDISC,   COG,     NOM,     CODFIS,  SES,     DATNAS,  CODCOMNAS,       SIGPRONAS," + " CODDUG,  IND,     NUMCIV,  DENSTAEST,       DENLOC," + " CODCOM,  CAP,     SIGPRO,  TEL,     CELL,    FAX,     EMAIL,   PEC," + " ULTAGG,  UTEAGG )" + " SELECT " + int32_3.ToString() + ",   COG,     NOM,     CODFIS,  SES,     DATNAS,  ISCT.CODCOM AS CODCOMNAS," + " (SELECT SIGPRO FROM CODCOM WHERE CODCOM.CODCOM=ISCT.CODCOM) AS SIGPRONAS," + " CODDUG,  IND,     NUMCIV,  DENSTAEST,       DENLOC," + " ISCD.CODCOM,  CAP,     SIGPRO,  TEL1,     CELL,    FAX,     EMAIL,   EMAILCERT," + " CURRENT_TIMESTAMP AS ULTAGG,  'AZIENDA' AS UTEAGG FROM ISCT LEFT JOIN ISCD ON ISCT.MAT = ISCD.MAT" + " AND ISCD.DATINI = (SELECT MAX(DATINI) FROM ISCD WHERE MAT=" + MAT.ToString() + ")" + " WHERE ISCT.MAT=" + MAT.ToString();
                flag1 = db.WriteTransactionData(strSQL15, CommandType.Text);
              }
              else
                int32_3 = Convert.ToInt32(db.Get1ValueFromSQL(strSQL13, CommandType.Text));
              if (flag1)
              {
                string strSQL16 = "SELECT VALUE(MAX(IDADE),0)+1 FROM FIACONSIP.ADESIONI";
                num = Convert.ToInt32(db.Get1ValueFromSQL(strSQL16, CommandType.Text));
                string strSQL17 = " INSERT INTO FIACONSIP.ADESIONI (" + " IDADE,   IDISC,   IDAZI,   DATINI,  DATFIN,  DATDOM,  CODUTEDOM," + " IDTIPISC,   IDTIPAZI,    MAT,     CODPOS, ULTAGG,  UTEAGG, INSDADIPA)" + " VALUES" + " (" + num.ToString() + ", " + int32_3.ToString() + ", " + int32_1.ToString() + ",'" + PRIMO_GIORNO_MESE + "', '9999-12-31', CURRENT_DATE, 'AZIENDA', " + CODQUACON.ToString() + "," + " 1, " + MAT.ToString() + ", " + codPos + ", CURRENT_TIMESTAMP, 'AZIENDA', 'S')";
                flag1 = db.WriteTransactionData(strSQL17, CommandType.Text);
                if (flag1)
                {
                  string strSQL18 = "INSERT INTO FIACONSIP.ADESIONITIPPAG (IDADETIPPAG, IDADE, IDISC, IDTIPPAG, ANNODA, ULTAGG, UTEAGG) VALUES ( " + Convert.ToInt32(db.Get1ValueFromSQL("SELECT VALUE(MAX(IDADETIPPAG),0) + 1 FROM FIACONSIP.ADESIONITIPPAG", CommandType.Text)).ToString() + ", " + num.ToString() + ", " + int32_3.ToString() + ", 1, " + ANNFIA + ", " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(codPos) + ") ";
                  flag1 = db.WriteTransactionData(strSQL18, CommandType.Text);
                }
              }
              if (flag1)
              {
                string strSQL19 = " INSERT INTO FIACONSIP.ADESIONIFOR" + " (IDADEFOR, IDADE,   IDISC,   IDAZI,   DATINI,  DATFIN,  IDFORMULA, VERSANTE, TIPQUO, " + " ULTAGG, UTEAGG, AZIPAGQUOISC)" + " SELECT" + " (SELECT VALUE(MAX(IDADEFOR),0)+1 FROM FIACONSIP.ADESIONIFOR), IDADE, IDISC, IDAZI, DATINI, DATFIN, " + " 1, 'A','I', CURRENT_TIMESTAMP, 'AZIENDA', 'S' " + " FROM FIACONSIP.ADESIONI WHERE IDADE=" + num.ToString();
                flag1 = db.WriteTransactionData(strSQL19, CommandType.Text);
              }
            }
          }
          else
          {
            string strSQL20 = "SELECT IDADE FROM FIACONSIP.ADESIONI WHERE MAT=" + MAT.ToString() + " AND IDAZI = " + int32_1.ToString() + " AND '" + PRIMO_GIORNO_MESE + "' BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')";
            num = Convert.ToInt32(db.Get1ValueFromSQL(strSQL20, CommandType.Text));
          }
        }
        if (string.IsNullOrEmpty(IDADE))
          IDADE = num.ToString();
        return Convert.ToInt32(IDADE) != 0 && flag1;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool FIA_AGGIORNA_TOTALI_POLIZZA(DataLayer db, int IDPOL)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      Decimal num1 = 0M;
      Decimal num2 = 0M;
      try
      {
        Convert.ToInt32(db.Get1ValueFromSQL("SELECT ANNPOL FROM FIACONSIP.POLIZZE WHERE IDPOL=" + IDPOL.ToString(), CommandType.Text));
        string strSQL1 = "SELECT IMPFIGDOV, IMPISCDOV FROM FIACONSIP.POLIZZE WHERE IDPOL=" + IDPOL.ToString();
        DataTable dataTable3 = db.GetDataTable(strSQL1);
        Decimal num3 = Convert.ToDecimal(dataTable3.Rows[0]["IMPISCDOV"]);
        Decimal num4 = Convert.ToDecimal(dataTable3.Rows[0]["IMPFIGDOV"]);
        string strSQL2 = "SELECT VALUE(SUM(IMPCAS), 0) AS IMPCAS " + "FROM FIACONSIP.POLIZZEFOR " + "WHERE IDPOL=" + IDPOL.ToString() + " AND TIPQUO='I'";
        Decimal num5 = Convert.ToDecimal(db.Get1ValueFromSQL(strSQL2, CommandType.Text));
        string strSQL3 = "SELECT VALUE(SUM(IMPCAS), 0) AS IMPCAS " + "FROM FIACONSIP.POLIZZEFOR " + "WHERE IDPOL=" + IDPOL.ToString() + " AND TIPQUO='F'";
        Decimal num6 = Convert.ToDecimal(db.Get1ValueFromSQL(strSQL3, CommandType.Text));
        Decimal num7 = num6 + num5;
        string strSQL4 = "SELECT VALUE(SUM(IMPADE), 0) AS IMPADE " + "FROM FIACONSIP.POLIZZEFOR " + "WHERE IDPOL=" + IDPOL.ToString();
        Decimal num8 = Convert.ToDecimal(db.Get1ValueFromSQL(strSQL4, CommandType.Text));
        Decimal num9 = num3 - num8 - num5;
        Decimal num10 = num4 - num6;
        Decimal num11 = num9 + num10;
        if (num8 < 0M | num7 < 0M | num10 < 0M | num9 < 0M)
          throw new Exception("Attenzione gli importi del dovuto non possono essere inferiori a zero euro");
        string strSQL5 = "SELECT VALUE(SUM(IMPABB), 0) AS IMPPAG, TIPQUO FROM FIACONSIP.VERSAMENTIABBDET WHERE IDPOL=" + IDPOL.ToString() + " GROUP BY TIPQUO";
        DataTable dataTable4 = db.GetDataTable(strSQL5);
        int num12 = dataTable4.Rows.Count - 1;
        for (int index = 0; index <= num12; ++index)
        {
          if (dataTable4.Rows[index]["TIPQUO"] is string str)
          {
            if (!(str == "I"))
            {
              if (str == "F")
              {
                num2 = Convert.ToDecimal(dataTable4.Rows[index]["IMPPAG"]);
                continue;
              }
            }
            else
            {
              num1 = Convert.ToDecimal(dataTable4.Rows[index]["IMPPAG"]);
              continue;
            }
          }
          throw new Exception("Il campo TIPQUO deve valere I o F");
        }
        Decimal num13 = num1 + num2;
        Decimal num14;
        Decimal num15;
        if (num13 > num8)
        {
          num14 = num8;
          num15 = num13 - num8;
        }
        else
        {
          num14 = num13;
          num15 = 0M;
        }
        Decimal num16 = num4 + num3 - num8;
        Decimal num17 = Math.Round(num7 * num15 / num16, 2);
        if (num4 + num3 - num8 - num7 != num11)
          throw new Exception("Attenzione incongruenza con l'importo assicurazione da pagare. Contattare il CED");
        string strSQL6 = "SELECT VALUE(SUM(IMPASSPAG), 0) FROM FIACONSIP.LOTCOMASSDET WHERE IDPOL=" + IDPOL.ToString() + " AND TIPQUO='I' AND DATANN IS NULL";
        Decimal num18 = Convert.ToDecimal(db.Get1ValueFromSQL(strSQL6, CommandType.Text));
        string strSQL7 = "SELECT VALUE(SUM(IMPASSPAG), 0) FROM FIACONSIP.LOTCOMASSDET WHERE IDPOL=" + IDPOL.ToString() + " AND TIPQUO='F' AND DATANN IS NULL";
        Decimal num19 = Convert.ToDecimal(db.Get1ValueFromSQL(strSQL7, CommandType.Text));
        Decimal num20 = num18 + num19;
        Decimal num21 = Convert.ToDecimal(num8);
        string str1 = "UPDATE FIACONSIP.POLIZZE SET " + " IMPADEDOV=" + num21.ToString().Trim().Replace(",", ".") + ", ";
        num21 = Convert.ToDecimal(num7);
        string str2 = num21.ToString().Trim().Replace(",", ".");
        string str3 = str1 + " IMPCASDOV=" + str2 + ", " + " IMPTOTDOV= (IMPISCDOV+IMPFIGDOV) , ";
        num21 = Convert.ToDecimal(num9);
        string str4 = num21.ToString().Trim().Replace(",", ".");
        string str5 = str3 + " IMPASSDOV=" + str4 + ", ";
        num21 = Convert.ToDecimal(num10);
        string str6 = num21.ToString().Trim().Replace(",", ".");
        string str7 = str5 + " IMPASSFIGDOV=" + str6 + ", ";
        num21 = Convert.ToDecimal(num14);
        string str8 = num21.ToString().Trim().Replace(",", ".");
        string str9 = str7 + " IMPADEPAG=" + str8 + ", ";
        num21 = Convert.ToDecimal(num17);
        string str10 = num21.ToString().Trim().Replace(",", ".");
        string str11 = str9 + " IMPCASPAG=" + str10 + ", ";
        num21 = Convert.ToDecimal(num1);
        string str12 = num21.ToString().Trim().Replace(",", ".");
        string str13 = str11 + " IMPISCPAG=" + str12 + ", ";
        num21 = Convert.ToDecimal(num2);
        string str14 = num21.ToString().Trim().Replace(",", ".");
        string str15 = str13 + " IMPFIGPAG=" + str14 + ", ";
        num21 = Convert.ToDecimal(num1 + num2);
        string str16 = num21.ToString().Trim().Replace(",", ".");
        string str17 = str15 + " IMPTOTPAG=" + str16 + ", ";
        num21 = Convert.ToDecimal(num18);
        string str18 = num21.ToString().Trim().Replace(",", ".");
        string str19 = str17 + " IMPASSPAG=" + str18 + ", ";
        num21 = Convert.ToDecimal(num19);
        string str20 = num21.ToString().Trim().Replace(",", ".");
        string strSQL8 = str19 + " IMPASSFIGPAG=" + str20 + " WHERE IDPOL=" + IDPOL.ToString();
        bool flag = db.WriteTransactionData(strSQL8, CommandType.Text);
        if (flag)
        {
          string strSQL9 = "UPDATE FIACONSIP.POLIZZE SET " + " IMPADEDIF=IMPADEDOV-IMPADEPAG, " + " IMPCASDIF=IMPCASDOV-IMPCASPAG, " + " IMPISCDIF=IMPISCDOV-IMPISCPAG, " + " IMPFIGDIF=IMPFIGDOV-IMPFIGPAG, " + " IMPTOTDIF=IMPTOTDOV-IMPTOTPAG, " + " IMPASSDIF=IMPASSDOV-IMPASSPAG, " + " IMPASSFIGDIF=IMPASSFIGDOV-IMPASSFIGPAG " + " WHERE IDPOL=" + IDPOL.ToString();
          flag = db.WriteTransactionData(strSQL9, CommandType.Text);
        }
        return flag;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private static Decimal CALCOLA_IMPPOL(DataLayer db, int IDPOL, string VERSANTE)
    {
      DataTable dataTable1 = new DataTable();
      Decimal num1 = 0M;
      try
      {
        if (VERSANTE == "A")
        {
          string strSQL = "SELECT IMPORTO, IMPAZI, IMPISC, AZIPAGQUOISC FROM FIACONSIP.POLIZZEFOR WHERE IDPOL=" + IDPOL.ToString() + " AND VERSANTE='A'";
          DataTable dataTable2 = db.GetDataTable(strSQL);
          int num2 = dataTable2.Rows.Count - 1;
          for (int index = 0; index <= num2; ++index)
          {
            if (dataTable2.Rows[index]["AZIPAGQUOISC"].ToString().Trim() == "N")
              num1 += Convert.ToDecimal(dataTable2.Rows[index]["IMPAZI"]);
            else
              num1 += Convert.ToDecimal(dataTable2.Rows[index]["IMPORTO"]);
          }
        }
        else
        {
          string strSQL1 = "SELECT IMPORTO, IMPAZI, IMPISC, AZIPAGQUOISC FROM FIACONSIP.POLIZZEFOR WHERE IDPOL=" + IDPOL.ToString() + " AND VERSANTE='I'";
          DataTable dataTable3 = db.GetDataTable(strSQL1);
          int num3 = dataTable3.Rows.Count - 1;
          for (int index = 0; index <= num3; ++index)
            num1 += Convert.ToDecimal(dataTable3.Rows[index]["IMPORTO"]);
          string strSQL2 = "SELECT IMPORTO, IMPAZI, IMPISC FROM FIACONSIP.POLIZZEFOR WHERE IDPOL=" + IDPOL.ToString() + " AND VERSANTE = 'A' AND AZIPAGQUOISC='N'";
          DataTable dataTable4 = db.GetDataTable(strSQL2);
          int num4 = dataTable4.Rows.Count - 1;
          for (int index = 0; index <= num4; ++index)
            num1 += Convert.ToDecimal(dataTable4.Rows[index]["IMPISC"]);
        }
        return num1;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    private static string AggiungiApici(string s) => "'" + s + "'";
  }
}
