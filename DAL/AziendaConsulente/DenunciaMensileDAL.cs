// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.DenunciaMensileDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using OCM.TFI.OCM.Protocollo;
using OCM.TFI.OCM.Utilities;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;
using Utilities;
using static Utilities.DbParameters;

namespace TFI.DAL.AziendaConsulente
{
  public static class DenunciaMensileDAL
  {
    private static readonly ILog log = LogManager.GetLogger("RollingFile");
    private static readonly ILog TrackLog = LogManager.GetLogger("Track");
    public static List<ParametriGenerali> listaParametriGenerali;
    public static string ErrorMessage = "";

    public static bool CiSonoRettificheDIPA(string codPos, int anno, int mese)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT (VALUE(IMPCONDEL,0) + VALUE(IMPADDRECDEL,0)) AS RETTIF FROM DENTES A WHERE CODPOS = " + codPos + " " + string.Format("AND ANNDEN = {0} AND MESDEN = {1} AND ((TIPMOV = 'NU' AND DATCONMOV IS NOT NULL) OR (TIPMOV = 'DP')) AND NUMMOVANN IS NULL", (object) anno, (object) mese);
        return Convert.ToDecimal(dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text)) != 0M;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static List<DatiRettificheDIPA> GetRettificheDIPA_3(
      string codPos,
      int anno,
      int mese,
      int proDen)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT B.MAT, (TRIM(A.COG) || ' ' || TRIM(A.NOM)) AS NOME, TRANSLATE(CHAR(DAL,EUR),'/','.') " + "AS DAL, TRANSLATE(CHAR(AL,EUR),'/','.') AS AL, LEFT((SELECT DISTINCT TRIM(DENQUA) FROM " + "QUACON WHERE QUACON.CODQUACON = B.CODQUACON), 1) AS DENQUA, CODLIV, PERAPP, IMPRET, IMPOCC, " + "IMPFIG, IMPRETDEL, IMPRETPRE, IMPOCCDEL, IMPFIGDEL, ALIQUOTA, IMPCONDEL, IMPABBDEL, " + "IMPASSCONDEL, CASE WHEN NUMSAN IS NULL THEN 0 ELSE IMPSANDET END AS IMPSANDET, PRODENDET " + "FROM DENDET B INNER JOIN ISCT A ON B.MAT = A.MAT WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + proDen.ToString() + " AND B.TIPMOV = 'RT' AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL " + "AND VALUE(ESIRET, '') <> 'S' " + "ORDER BY MAT, PRODENDET";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (List<DatiRettificheDIPA>) null;
        List<DatiRettificheDIPA> rettificheDipa3 = new List<DatiRettificheDIPA>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          rettificheDipa3.Add(new DatiRettificheDIPA()
          {
            Mat = !DBNull.Value.Equals(row["MAT"]) ? Convert.ToInt32(row["MAT"]) : 0,
            Nome = !DBNull.Value.Equals(row["NOME"]) ? row["NOME"].ToString() : string.Empty,
            Dal = !DBNull.Value.Equals(row["DAL"]) ? row["DAL"].ToString() : string.Empty,
            Al = !DBNull.Value.Equals(row["AL"]) ? row["AL"].ToString() : string.Empty,
            DenQua = !DBNull.Value.Equals(row["DENQUA"]) ? row["DENQUA"].ToString() : string.Empty,
            CodLiv = !DBNull.Value.Equals(row["CODLIV"]) ? Convert.ToInt32(row["CODLIV"]) : 0,
            PerApp = !DBNull.Value.Equals(row["PERAPP"]) ? Convert.ToDecimal(row["PERAPP"]) : 0M,
            ImpRet = !DBNull.Value.Equals(row["IMPRET"]) ? Convert.ToDecimal(row["IMPRET"]) : 0M,
            ImpOcc = !DBNull.Value.Equals(row["IMPOCC"]) ? Convert.ToDecimal(row["IMPOCC"]) : 0M,
            ImpFig = !DBNull.Value.Equals(row["IMPFIG"]) ? Convert.ToDecimal(row["IMPFIG"]) : 0M,
            ImpRetDel = !DBNull.Value.Equals(row["IMPRETDEL"]) ? Convert.ToDecimal(row["IMPRETDEL"]) : 0M,
            ImpRetPre = !DBNull.Value.Equals(row["IMPRETPRE"]) ? Convert.ToDecimal(row["IMPRETPRE"]) : 0M,
            ImpOccDel = !DBNull.Value.Equals(row["IMPOCCDEL"]) ? Convert.ToDecimal(row["IMPOCCDEL"]) : 0M,
            ImpFigDel = !DBNull.Value.Equals(row["IMPFIGDEL"]) ? Convert.ToDecimal(row["IMPFIGDEL"]) : 0M,
            Aliquota = !DBNull.Value.Equals(row["ALIQUOTA"]) ? Convert.ToDecimal(row["ALIQUOTA"]) : 0M,
            ImpConDel = !DBNull.Value.Equals(row["IMPCONDEL"]) ? Convert.ToDecimal(row["IMPCONDEL"]) : 0M,
            ImpAbbDel = !DBNull.Value.Equals(row["IMPABBDEL"]) ? Convert.ToDecimal(row["IMPABBDEL"]) : 0M,
            ImpAssConDel = !DBNull.Value.Equals(row["IMPASSCONDEL"]) ? Convert.ToDecimal(row["IMPASSCONDEL"]) : 0M,
            ImpSanDet = !DBNull.Value.Equals(row["IMPSANDET"]) ? Convert.ToDecimal(row["IMPSANDET"]) : 0M,
            ProDenDet = !DBNull.Value.Equals(row["PRODENDET"]) ? Convert.ToInt32(row["PRODENDET"]) : 0
          });
        return rettificheDipa3;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static void GetRettificheDIPA_2(RettificheDIPA rettifiche, string codPos)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT DISTINCT A.INSAZI, A.AZINOTC, A.CODPOS, A.RAGSOC, A.RAGSOCBRE, A.CODFIS AS CODFISAZI, A.PARIVA, A.NATGIU, " + "CHAR(A.ULTAGG) AS ULTAGG, NATGIU.DENNATGIU, TRANSLATE(CHAR(A.DATAPE, EUR), '/', '.') AS DATAPE, TRANSLATE(CHAR" + "(A.DATCHI, EUR), '/', '.') AS DATCHI, C.CATATTCAM, D.DENATTCAM, C.CODATTCAM, TRANSLATE(CHAR(C.DATINI, EUR), " + "'/', '.') AS DATINIATT, F.CODSTACON, F.DENCODSTA FROM AZI A INNER JOIN NATGIU ON A.NATGIU = NATGIU.NATGIU LEFT " + "JOIN AZIATT AS C ON A.CODPOS = C.CODPOS AND C.DATFIN = '9999-12-31' LEFT JOIN TIPATT AS D ON C.CATATTCAM = " + "D.CATATTCAM AND C.CODATTCAM = D.CODATTCAM LEFT JOIN AZISTO AS E ON A.CODPOS = E.CODPOS AND current_date BETWEEN " + "E.DATINI AND VALUE(E.DATFIN, '9999-12-31') LEFT JOIN CODSTA AS F ON E.CODSTACON = F.CODSTACON WHERE A.CODPOS = " + codPos;
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return;
        DataRow row = dataTable.Rows[0];
        rettifiche.CodFis = !DBNull.Value.Equals(row["CODFISAZI"]) ? row["CODFISAZI"].ToString() : string.Empty;
        rettifiche.PartitaIVA = !DBNull.Value.Equals(row["PARIVA"]) ? row["PARIVA"].ToString() : string.Empty;
        rettifiche.NatGiu = !DBNull.Value.Equals(row["NATGIU"]) ? row["NATGIU"].ToString() : string.Empty;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static RettificheDIPA GetRettificheDIPA_1(
      string codPos,
      int anno,
      int mese,
      int proDen)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT DATAPE, VALUE(IMPCONDEL, 0) AS IMPCONDEL, VALUE(IMPADDRECDEL, 0) AS IMPADDRECDEL, VALUE(IMPABBDEL,  0) AS " + "IMPABBDEL, VALUE(IMPASSCONDEL, 0) AS IMPASSCONDEL, VALUE(IMPSANRET, 0) AS IMPSANRET, TIPMOV, SANSOTSOG, NUMSAN, " + "NUMSANANN FROM DENTES WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + proDen.ToString();
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (RettificheDIPA) null;
        DataRow row = dataTable.Rows[0];
        RettificheDIPA rettificheDipa1 = new RettificheDIPA()
        {
          DatApe = !DBNull.Value.Equals(row["DATAPE"]) ? row["DATAPE"].ToString() : string.Empty,
          ImpConDel = !DBNull.Value.Equals(row["IMPCONDEL"]) ? Convert.ToDecimal(row["IMPCONDEL"]) : 0M,
          ImpAddRecDel = !DBNull.Value.Equals(row["IMPADDRECDEL"]) ? Convert.ToDecimal(row["IMPADDRECDEL"]) : 0M
        };
        rettificheDipa1.ImpTotale = rettificheDipa1.ImpConDel + rettificheDipa1.ImpAddRecDel;
        rettificheDipa1.ImpSanRet = !DBNull.Value.Equals(row["IMPSANRET"]) ? Convert.ToDecimal(row["IMPSANRET"]) : 0M;
        rettificheDipa1.TipMov = !DBNull.Value.Equals(row["TIPMOV"]) ? row["TIPMOV"].ToString() : string.Empty;
        return rettificheDipa1;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static RicercaArretrato RicercaArretrato(
      string codPos,
      out int? outcome,
      int anno = 0,
      int mese = 0,
      int progressivo = 0)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string str = $"SELECT ANNDEN, MESDEN, PRODEN, STADEN, TIPMOV FROM DENTES WHERE CODPOS = {codPos}";
        if (anno != 0)
          str += $" AND ANNDEN = {anno}";
        if (mese != 0)
          str += $" AND MESDEN = {mese}";
        if (progressivo != 0)
          str += $" AND PRODEN = {progressivo}";
        string strSql = $"{str} AND STADEN = 'N' AND TIPMOV ='AR' AND DATMOVANN IS NULL";
        DataTable dataTable = dataLayer.GetDataTable(strSql);
        if (dataTable.Rows.Count > 0)
        {
          RicercaArretrato ricercaArretrato = new RicercaArretrato
          {
            anno = !DBNull.Value.Equals(dataTable.Rows[0]["ANNDEN"]) ? Convert.ToInt32(dataTable.Rows[0]["ANNDEN"]) : 0,
            mese = !DBNull.Value.Equals(dataTable.Rows[0]["MESDEN"]) ? Convert.ToInt32(dataTable.Rows[0]["MESDEN"]) : 0,
            proDen = !DBNull.Value.Equals(dataTable.Rows[0]["PRODEN"]) ? Convert.ToInt32(dataTable.Rows[0]["PRODEN"]) : 0,
            staDen = !DBNull.Value.Equals(dataTable.Rows[0]["STADEN"]) ? dataTable.Rows[0]["STADEN"].ToString() : string.Empty,
            tipMov = !DBNull.Value.Equals(dataTable.Rows[0]["TIPMOV"]) ? dataTable.Rows[0]["TIPMOV"].ToString() : string.Empty
          };
          outcome = 0;
          return ricercaArretrato;
        }
        outcome = 2;
        return null;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool EliminaArretrato(
      DataLayer db,
      string codPos,
      int annoDenuncia,
      int meseDenuncia,
      int progDenuncia)
    {
      try
      {
        string strSql1 = $"DELETE FROM DENDETALI WHERE CODPOS = {codPos} AND ANNDEN = {annoDenuncia} AND MESDEN = {meseDenuncia} AND PRODEN = {progDenuncia}";
        if (db.WriteTransactionData(strSql1, CommandType.Text))
        {
          string strSql2 = $"DELETE FROM DENDETSOS WHERE CODPOS = {codPos} AND ANNDEN = {annoDenuncia} AND MESDEN = {meseDenuncia} AND PRODEN = {progDenuncia}";
          if (db.WriteTransactionData(strSql2, CommandType.Text))
          {
            string strSql3 = $"DELETE FROM DENDET WHERE CODPOS = {codPos} AND ANNDEN = {(object)annoDenuncia} AND MESDEN = {meseDenuncia} AND PRODEN = {progDenuncia}";
            if (db.WriteTransactionData(strSql3, CommandType.Text))
            {
              string strSql4 = $"DELETE FROM DENTES WHERE CODPOS = {codPos} AND ANNDEN = {annoDenuncia} AND MESDEN = {meseDenuncia} AND PRODEN = {progDenuncia} AND STADEN = 'N' AND TIPMOV ='AR' AND DATMOVANN IS NULL";
              return db.WriteTransactionData(strSql4, CommandType.Text);
            }
          }
        }
        return false;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject(HttpContext.Current.Session["utente"] as OCM.Utente.Utente), "");
        throw;
      }
    }

    public static bool SalvaDIPA_upload(
      TFI.OCM.Utente.Utente utente,
      string codPos,
      int anno,
      int mese,
      HttpPostedFileBase dipa,
      string strPath,
      ref string PRODEN,
      ref bool btnStampaIsVisible,
      ref bool btnConfermaIsVisible)
    {
      DataLayer db1 = new DataLayer();
      int intProDenDet = 0;
      string newLine = Environment.NewLine;
      string[] strArray = new string[10];
      bool blnCommit = false;
      bool flag1 = false;
      int PRODEN1 = 0;
      int num1 = 0;
      string TIPDEN = (string) null;
      bool flag2 = false;
      bool flag3 = false;
      int IDDIPA = 0;
      DataTable dataTable1 = new DataTable();
      int IDAZI = 0;
      bool flag4 = false;
      try
      {
        string strSQL1 = "SELECT IDAZI FROM FIACONSIP.AZI WHERE CODPOS = " + codPos;
        DataTable dataTable2 = db1.GetDataTable(strSQL1);
        if (dataTable2.Rows.Count > 0)
          IDAZI = Convert.ToInt32(dataTable2.Rows[0]["IDAZI"]);
        string strSQL2 = "SELECT COUNT(*) FROM DENTES" + " WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND ((TIPMOV = 'DP' AND NUMMOV IS NOT NULL AND DATMOVANN IS NULL)" + " or" + " (TIPMOV = 'NU' AND NUMMOV IS NOT NULL AND DATMOVANN IS NULL))";
        if (Convert.ToInt32("0" + db1.Get1ValueFromSQL(strSQL2, CommandType.Text)) == 0)
        {
          if (File.Exists(strPath))
            File.Delete(strPath);
          Stream inputStream = dipa.InputStream;
          FileStream fileStream = File.OpenWrite(strPath);
          byte[] instance = (byte[]) Array.CreateInstance(Type.GetType("System.Byte"), inputStream.Length);
          inputStream.Read(instance, 0, (int) inputStream.Length);
          fileStream.Write(instance, 0, (int) inputStream.Length);
          fileStream.Close();
          inputStream.Close();
          inputStream.Dispose();
          btnStampaIsVisible = false;
          db1.StartTransaction();
          flag1 = true;
          string strSQL3 = "SELECT PRODEN FROM DENTES WHERE " + " CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND " + " MESDEN = " + mese.ToString() + " AND TIPMOV = 'DP' AND STADEN = 'N' AND " + "DATMOVANN IS NULL";
          string str1 = db1.Get1ValueFromSQL(strSQL3, CommandType.Text) ?? "";
          if (!string.IsNullOrEmpty(str1))
          {
            string strSQL4 = "DELETE FROM DENDETALI WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + str1;
            if (db1.WriteTransactionData(strSQL4, CommandType.Text))
            {
              string strSQL5 = "DELETE FROM DENDETSOS WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + str1;
              if (db1.WriteTransactionData(strSQL5, CommandType.Text))
              {
                string strSQL6 = "DELETE FROM DENDET WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + str1;
                if (db1.WriteTransactionData(strSQL6, CommandType.Text))
                {
                  string strSQL7 = "DELETE FROM DENTES WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + str1;
                  flag2 = db1.WriteTransactionData(strSQL7, CommandType.Text);
                }
              }
            }
            if (!flag2)
              flag3 = true;
          }
          string strSQL8 = "SELECT TIPISC, ABB FROM AZISTO WHERE CODPOS = " + codPos + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date("01/" + mese.ToString() + "/" + anno.ToString())) + " <= VALUE(DATFIN, '9999-12-31') ORDER BY DATFIN DESC";
          DataTable dataTable3 = db1.GetDataTable(strSQL8);
          if (dataTable3.Rows.Count > 0)
            TIPDEN = dataTable3.Rows[0]["TIPISC"].ToString();
          string DAL = "01/" + mese.ToString().PadLeft(2, '0') + "/" + anno.ToString();
          string AL = DateTime.DaysInMonth(anno, mese).ToString() + "/" + mese.ToString().PadLeft(2, '0') + "/" + anno.ToString();
          List<RetribuzioneRDL> retribuzioneRdlList = DenunciaMensileDAL.PreparaDenunciaMensile(utente, int.Parse(PRODEN), DAL, AL, codPos, annFia: anno, mesFia: mese);
          StreamReader streamReader = File.OpenText(strPath);
          DateTime dateTime1;
          while (streamReader.Peek() >= 0)
          {
            if (PRODEN1 == 0)
            {
              DataLayer db2 = db1;
              string username = utente.Username;
              string CODPOS = codPos;
              int ANNDEN = anno;
              int MESDEN = mese;
              dateTime1 = DateTime.Now;
              string DATAPE = dateTime1.ToString();
              string UTEAPE = utente.Tipo.Trim();
              PRODEN1 = WriteDIPA.WRITE_INSERT_DENTES(db2, username, CODPOS, ANNDEN, MESDEN, DATAPE, UTEAPE, "", "", "DP", "O", "N", 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, "", "", "", 0, "", "0", 0, 0.0M, "", "", "", "", 0M, 0M, "", 0, 0, "N", "", "");
              if (PRODEN1 > 0)
                PRODEN = PRODEN1.ToString();
              string strSQL9 = "SELECT COUNT(*) FROM FIACONSIP.DIPATES WHERE CODPOS = " + codPos + " AND ANNFIA = " + anno.ToString() + " AND " + " MESFIA = " + mese.ToString() + " AND NUMMOV IS NOT NULL AND DATMOVANN IS NULL";
              if (Convert.ToInt32("0" + db1.Get1ValueFromSQL(strSQL9, CommandType.Text)) == 0)
              {
                dateTime1 = Convert.ToDateTime(DateTime.DaysInMonth(anno, mese).ToString() + "/" + mese.ToString() + "/" + anno.ToString());
                dateTime1 = dateTime1.AddDays(30.0);
                string DATSCA = dateTime1.ToString().Substring(0, 10);
                IDDIPA = WriteDIPA.WRITE_INSERT_DIPATESTMP(db1, utente.Username, codPos, anno, mese, PRODEN1, anno, mese, "A", IDAZI, 0M, DATSCA);
                if (IDDIPA == 0)
                  PRODEN = "";
              }
              else
                flag4 = true;
            }
            if (!string.IsNullOrEmpty(PRODEN))
            {
              bool flag5 = true;
              string str2 = streamReader.ReadLine();
              if (!string.IsNullOrEmpty(str2))
              {
                strArray[0] = str2.Substring(0, 16);
                strArray[1] = str2.Substring(16, 2);
                strArray[2] = str2.Substring(18, 1);
                strArray[3] = str2.Substring(19, 4);
                strArray[4] = str2.Substring(23, 4);
                strArray[5] = str2.Substring(27, 5);
                strArray[5] = strArray[5].Substring(0, 2) + "," + strArray[5].Substring(2, 3);
                Decimal num2 = Convert.ToDecimal(strArray[5]);
                strArray[6] = str2.Substring(32, 9);
                strArray[6] = strArray[6].Substring(0, 7) + "," + strArray[6].Substring(7, 2);
                Decimal d1 = Math.Round(Convert.ToDecimal(strArray[6]), 2);
                strArray[7] = str2.Substring(41, 9);
                strArray[7] = strArray[7].Substring(0, 7) + "," + strArray[7].Substring(7, 2);
                Decimal d2 = Math.Round(Convert.ToDecimal(strArray[7]), 2);
                strArray[8] = str2.Substring(50, 9);
                strArray[8] = strArray[8].Substring(0, 7) + "," + strArray[8].Substring(7, 2);
                Decimal d3 = Math.Round(Convert.ToDecimal(strArray[8]), 2);
                Decimal IMPCON = (Convert.ToDecimal(strArray[6]) + Convert.ToDecimal(strArray[8]) / 100M) * num2;
                DAL = anno.ToString() + "-" + strArray[3].Substring(0, 2) + "-" + strArray[3].Substring(2);
                AL = anno.ToString() + "-" + strArray[4].Substring(0, 2) + "-" + strArray[4].Substring(2);
                string strSQL10 = "SELECT MAT FROM ISCT WHERE CODFIS = '" + strArray[0] + "'";
                string MAT = db1.Get1ValueFromSQL(strSQL10, CommandType.Text) ?? "";
                if (string.IsNullOrEmpty(MAT))
                {
                  flag3 = true;
                  break;
                }
                List<RetribuzioneRDL> list = retribuzioneRdlList.Where<RetribuzioneRDL>((Func<RetribuzioneRDL, bool>) (r =>
                {
                  if (r.Mat == Convert.ToInt32(MAT))
                  {
                    string dal = r.Dal;
                    DateTime dateTime2 = DateTime.Parse(DAL);
                    string str3 = dateTime2.ToString("d");
                    if (dal == str3)
                    {
                      string al = r.Al;
                      dateTime2 = DateTime.Parse(AL);
                      string str4 = dateTime2.ToString("d");
                      return al == str4;
                    }
                  }
                  return false;
                })).ToList<RetribuzioneRDL>();
                if (list.Count == 0)
                {
                  flag5 = false;
                  flag3 = true;
                }
                else
                {
                  if (Convert.ToDecimal("0" + list[0].Aliquota.ToString()) == 0M)
                  {
                    flag5 = false;
                    flag3 = true;
                  }
                  if (d3 > 0M & Convert.ToInt32("0" + list[0].NumGGSos.ToString()) == 0)
                    flag3 = true;
                }
                if (flag5)
                {
                  flag2 = WriteDIPA.WRITE_INSERT_DENDET(db1, retribuzioneRdlList, DenunciaMensileDAL.listaParametriGenerali, utente, codPos, anno, mese, PRODEN1, Convert.ToInt32(MAT), "DP", DAL, AL, "", Math.Round(d1, 0), Math.Round(d2, 0), Math.Round(d3, 0), Convert.ToDecimal(list[0].ImpAbb), Convert.ToDecimal(list[0].ImpAssCon), IMPCON, Convert.ToDecimal(list[0].ImpMin), list[0].Prev.ToString(), Convert.ToInt32(list[0].ProMod), list[0].DatDec.ToString(), list[0].DatCes.ToString(), Convert.ToDecimal(list[0].NumGGAzi), Convert.ToDecimal(list[0].NumGGFig), Convert.ToDecimal(list[0].NumGGPer), Convert.ToDecimal(list[0].NumGGDom), Convert.ToDecimal(list[0].NumGGSos), Convert.ToDecimal(list[0].NumGGConAzi), Convert.ToDecimal(list[0].ImpSca), Convert.ToDecimal(list[0].ImpTraEco), list[0].Eta65.ToString(), Convert.ToInt32(list[0].TipRap), list[0].Fap.ToString(), Convert.ToDecimal(list[0].PerFap), Convert.ToDecimal(list[0].ImpFap), Convert.ToDecimal(list[0].PerPar), Convert.ToDecimal(list[0].PerApp), Convert.ToInt32(list[0].ProRap), Convert.ToInt32(list[0].CodCon), Convert.ToInt32(list[0].ProCon), list[0].TipSpe.ToString(), Convert.ToInt32(list[0].CodLoc), Convert.ToInt32(list[0].ProLoc), Convert.ToInt32(list[0].CodLiv), Convert.ToInt32(list[0].CodGruAss), Convert.ToInt32(list[0].CodQuaCon), Convert.ToDecimal(list[0].Aliquota), list[0].DatNas.ToString(), 0, TIPDEN, ref intProDenDet);
                  if (flag2)
                  {
                    if (list[0].Sanitario.ToString().Trim() == "S")
                    {
                      if (flag4)
                        ++num1;
                      else if (WriteDIPA.WRITE_INSERT_DIPADETTMP(db1, utente, codPos, anno, mese, PRODEN1, intProDenDet, Convert.ToInt32(list[0].Mat), IDDIPA, anno, mese, IDAZI, Convert.ToInt32(list[0].IdIsc), Convert.ToInt32(list[0].IdAde), Convert.ToInt32(list[0].IdPolizza), Convert.ToDecimal(list[0].ImpSanit)) == 0)
                        flag3 = true;
                      else
                        ++num1;
                    }
                    else
                      ++num1;
                  }
                  else
                    flag3 = true;
                }
              }
            }
            else
            {
              flag3 = true;
              break;
            }
          }
          streamReader.Close();
          streamReader.Dispose();
          if (flag2 & !flag3 & num1 > 0)
          {
            string strSQL11 = "SELECT SUM(IMPRET) AS IMPRET, SUM(IMPOCC) AS IMPOCC, SUM(IMPFIG) AS IMPFIG, SUM(IMPCON) AS IMPCON, " + "SUM(IMPABB) AS IMPABB, SUM(IMPASSCON) AS IMPASSCON FROM DENDET WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + PRODEN1.ToString() + " AND TIPMOV = 'DP'";
            dataTable3?.Rows.Clear();
            DataTable dataTable4 = db1.GetDataTable(strSQL11);
            if (dataTable4.Rows.Count > 0)
            {
              string strSQL12 = "UPDATE DENTES SET IMPRET = " + dataTable4.Rows[0]["IMPRET"].ToString().Replace(",", ".") + ", IMPOCC = " + dataTable4.Rows[0]["IMPOCC"].ToString().Replace(",", ".") + ", IMPFIG = " + dataTable4.Rows[0]["IMPFIG"].ToString().Replace(",", ".") + ", IMPCON = " + dataTable4.Rows[0]["IMPCON"].ToString().Replace(",", ".") + ", IMPABB = " + dataTable4.Rows[0]["IMPABB"].ToString().Replace(",", ".") + ", IMPASSCON = " + dataTable4.Rows[0]["IMPASSCON"].ToString().Replace(",", ".") + ", NUMRIGDET = " + num1.ToString() + " WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + PRODEN1.ToString() + " AND TIPMOV = 'DP'";
              blnCommit = db1.WriteTransactionData(strSQL12, CommandType.Text);
              if (blnCommit)
              {
                dateTime1 = DateTime.Now;
                string strSQL13 = "UPDATE DENTES SET IMPADDREC = ROUND(((IMPCON / 100) * " + DenunciaMensileDAL.GetImportoParametro(5, dateTime1.ToString()).ToString().Replace(",", ".") + "), 2) " + "WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + PRODEN1.ToString() + " AND TIPMOV = 'DP'";
                blnCommit = db1.WriteTransactionData(strSQL13, CommandType.Text);
                if (blnCommit)
                {
                  string strSQL14 = "UPDATE DENTES SET IMPDIS = IMPCON + IMPASSCON + IMPADDREC + IMPABB" + " WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + PRODEN1.ToString() + " AND TIPMOV = 'DP'";
                  blnCommit = db1.WriteTransactionData(strSQL14, CommandType.Text);
                }
                string strSQL15 = "SELECT VALUE(SUM(IMPSAN), 0) AS IMPSAN FROM FIACONSIP.DIPADETTMP WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + PRODEN1.ToString() + " AND IDDIPA = " + IDDIPA.ToString();
                dataTable4?.Rows.Clear();
                DataTable dataTable5 = db1.GetDataTable(strSQL15);
                if (dataTable5.Rows.Count > 0)
                {
                  string strSQL16 = "UPDATE FIACONSIP.DIPATESTMP SET IMPDISSAN = " + dataTable5.Rows[0]["IMPSAN"].ToString().Replace(",", ".") + " WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + PRODEN1.ToString() + " AND IDDIPA = " + IDDIPA.ToString();
                  blnCommit = db1.WriteTransactionData(strSQL16, CommandType.Text);
                }
              }
            }
            if (!blnCommit)
              flag3 = true;
          }
          db1.EndTransaction(blnCommit);
          flag1 = false;
          btnConfermaIsVisible = false;
          return blnCommit;
        }
        DenunciaMensileDAL.ErrorMessage = "Impossibile caricare la denuncia per questo mese! La denuncia risulta già presente oppure sono presenti delle notifiche di ufficio.";
        return false;
      }
      catch (Exception ex)
      {
        if (flag1)
          db1.EndTransaction(false);
        throw;
      }
      finally
      {
        if (flag3)
          btnStampaIsVisible = true;
      }
    }

    public static int GetNumeroDenunceDellAnno(string codPos, int anno)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = string.Format("SELECT COUNT(*) FROM DENTES WHERE CODPOS = {0} AND ANNDEN = {1} AND TIPMOV = 'DP'", (object) codPos, (object) anno);
        return Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text));
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static int GetNumSospesi(string codPos)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
          //Non c'è check su anno o altre cose?
        string strSQL = "SELECT COUNT(*) FROM DENTES WHERE CODPOS = " + codPos + " AND TIPMOV = 'DP' AND STADEN = 'N'";
        return Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text));
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool RipristinaImporto(string codPos, int anno, int mese, int proDen)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = string.Format("UPDATE DENTES SET IMPDEC = 0 WHERE CODPOS = {0} AND ANNDEN = {1}", (object) codPos, (object) anno) + string.Format(" AND MESDEN = {0} AND PRODEN = {1}", (object) mese, (object) proDen);
        return dataLayer.WriteData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static bool AggiornaCreditoDecurtato(
      string codPos,
      int anno,
      int mese,
      int proDen,
      string txtCrediti)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "UPDATE DENTES SET IMPDEC = IMPDEC + " + txtCrediti + " WHERE CODPOS = " + codPos + string.Format(" AND ANNDEN = {0} AND MESDEN = {1} AND PRODEN = {2}", (object) anno, (object) mese, (object) proDen);
        return dataLayer.WriteData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static int CountRecordsFromDIPATES(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen)
    {
      try
      {
        string strSQL = "SELECT COUNT(*) FROM FIACONSIP.DIPATES" + " WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + proDen.ToString();
        return Convert.ToInt32(db.Get1ValueFromSQL(strSQL, CommandType.Text));
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static int CountRecordsFromDIPATESTMP(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen)
    {
      try
      {
        string strSQL = "SELECT COUNT(*) FROM FIACONSIP.DIPATESTMP" + " WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + proDen.ToString() + " AND IMPDISSAN > 0";
        return Convert.ToInt32(db.Get1ValueFromSQL(strSQL, CommandType.Text));
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static List<RetribuzioneRDL> GetDataFromDENDET_2(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen)
    {
      try
      {
        string strSQL = $"SELECT * FROM DENDET " +
                        $"WHERE CODPOS = {codPos} " +
                        $"AND ANNDEN = {anno} " +
                        $"AND MESDEN = {mese} " +
                        $"AND PRODEN = {proDen}";

        DataTable dataTable = db.GetDataTable(strSQL);
        
        if (dataTable.Rows.Count <= 0)
          return null;

        List<RetribuzioneRDL> dataFromDendet2 = new List<RetribuzioneRDL>();

        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          dataFromDendet2.Add(new RetribuzioneRDL()
          {
            CodPos = DBNull.Value.Equals(row["CODPOS"]) ? string.Empty : row["CODPOS"].ToString(),
            AnnDen = DBNull.Value.Equals(row["ANNDEN"]) ? string.Empty : row["ANNDEN"].ToString(),
            Mat = !DBNull.Value.Equals(row["MAT"]) ? Convert.ToInt32(row["MAT"]) : 0,
            ProRap = !DBNull.Value.Equals(row["PRORAP"]) ? Convert.ToInt32(row["PRORAP"]) : 0,
            ImpRet = !DBNull.Value.Equals(row["IMPRET"]) ? Convert.ToDecimal(row["IMPRET"]) : 0M,
            ImpOcc = !DBNull.Value.Equals(row["IMPOCC"]) ? Convert.ToDecimal(row["IMPOCC"]) : 0M,
            ImpFig = !DBNull.Value.Equals(row["IMPFIG"]) ? Convert.ToDecimal(row["IMPFIG"]) : 0M
          });
        return dataFromDendet2;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool AggiornaDIPADET(
      DataLayer db,
      Decimal IMPORTO_SANITARIO,
      int IDDIPANEW,
      int IDPOL,
      int IDDIPADET)
    {
      try
      {
        string strSQL = " UPDATE FIACONSIP.DIPADET SET IDPOL = " + IDPOL.ToString() + ", " + " IMPDAABB = " + IMPORTO_SANITARIO.ToString().Replace(",", ".") + ", " + " IMPSAN = " + IMPORTO_SANITARIO.ToString().Replace(",", ".") + " WHERE IDDIPA = " + IDDIPANEW.ToString() + " AND IDDIPADET = " + IDDIPADET.ToString();
        return db.WriteTransactionData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static List<DIPADET_Data> GetDataFromDIPADET(DataLayer db, int idDipaNew)
    {
      try
      {
        string strSQL = string.Format("SELECT * FROM FIACONSIP.DIPADET WHERE IDDIPA = {0}", (object) idDipaNew);
        DataTable dataTable = db.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (List<DIPADET_Data>) null;
        List<DIPADET_Data> dataFromDipadet = new List<DIPADET_Data>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          dataFromDipadet.Add(new DIPADET_Data()
          {
            IdAde = !DBNull.Value.Equals(row["IDADE"]) ? Convert.ToInt32(row["IDADE"]) : 0,
            IdIsc = !DBNull.Value.Equals(row["IDISC"]) ? Convert.ToInt32(row["IDISC"]) : 0,
            AnnFia = !DBNull.Value.Equals(row["ANNFIA"]) ? Convert.ToInt32(row["ANNFIA"]) : 0,
            MesFia = !DBNull.Value.Equals(row["MESFIA"]) ? Convert.ToInt32(row["MESFIA"]) : 0,
            Mat = !DBNull.Value.Equals(row["MAT"]) ? Convert.ToInt32(row["MAT"]) : 0,
            IdDipaDet = !DBNull.Value.Equals(row["IDDIPADET"]) ? Convert.ToInt32(row["IDDIPADET"]) : 0
          });
        return dataFromDipadet;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static int CountRecordFromDIPATES(DataLayer db, string codPos, int anno, int mese)
    {
      try
      {
        string strSQL = "SELECT COUNT(*) FROM FIACONSIP.DIPATES WHERE CODPOS = " + codPos + " AND ANNFIA = " + anno.ToString() + " AND MESFIA = " + mese.ToString() + " AND PRODEN IS NOT NULL AND DATMOVANN IS NULL";
        return Convert.ToInt32(db.Get1ValueFromSQL(strSQL, CommandType.Text));
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static void VerificaContabilizzazione2(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen,
      string partitaMov,
      int progMov,
      string codCaus,
      ref bool blnRet)
    {
      try
      {
        string strSQL1 = $"SELECT COUNT(*) FROM MOVIMSAP " +
                         $"WHERE PARTITA = {DBMethods.DoublePeakForSql(partitaMov)} " +
                         $"AND PROGMOV = {progMov} " +
                         $"AND CODCAUS = {DBMethods.DoublePeakForSql(codCaus.Trim().PadLeft(2, '0'))}";
        
        if (Convert.ToInt32(db.Get1ValueFromSQL(strSQL1, CommandType.Text)) == 0)
          blnRet = false;
        
        string strSQL2 = $"UPDATE DENTES SET FLGRIC = 'N' " +
                         $"WHERE CODPOS = {codPos} " +
                         $"AND ANNDEN = {anno} " +
                         $"AND MESDEN = {mese} " +
                         $"AND PRODEN = {proDen}";

        blnRet = db.WriteTransactionData(strSQL2, CommandType.Text);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static int VerificaContabilizzazione(
      DataLayer db,
      string partitaMov,
      int progMov,
      string codCaus)
    {
      try
      {
        string strSQL = $"SELECT COUNT(*) FROM MOVIMSAP " +
                        $"WHERE PARTITA = {DBMethods.DoublePeakForSql(partitaMov)} " +
                        $"AND PROGMOV = {progMov} " +
                        $"AND CODCAUS = {DBMethods.DoublePeakForSql(codCaus.Trim().PadLeft(2, '0'))}";

        return Convert.ToInt32(db.Get1ValueFromSQL(strSQL, CommandType.Text));
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static void Aggiorna_IDOC(DataLayer db, clsIDOC iDoc)
    {
      iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
      try
      {
        iDoc.Aggiorna_IDOC(ref db);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static void WRITE_IDOC_E1PITYP(DataLayer db, DataTable dt_mat, clsIDOC iDoc)
    {
      iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
      try
      {
        iDoc.WRITE_IDOC_E1PITYP(db, "9004", dt_mat, false);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static void WriteIDocTestata(DataLayer db, DataRow dr_iDoc, clsIDOC iDoc)
    {
      iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
      try
      {
        iDoc.WRITE_IDOC_TESTATA(db, dr_iDoc);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static DataTable GetIDocDataTable(
      DataLayer db,
      TFI.OCM.Utente.Utente utente,
      int anno,
      int mese,
      int proDen,
      out clsIDOC iDoc)
    {
      iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
      try
      {
        iDoc = new clsIDOC();
        iDoc.strUserCode = utente.Username;
        return iDoc.GET_IDOC_DATI_E1PITYPE(db, "9004", Convert.ToInt32(utente.CodPosizione), 0, 0, 0, "", "", "9999-12-31", "", "", "", anno, mese, proDen, "", "", "DP", "");
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool AggiornaMODPREDET(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen,
      string numMov)
    {
      bool flag = true;
      try
      {
        string strSQL1 = $"SELECT MAT, PRODENDET FROM DENDET WHERE CODPOS = {codPos} AND ANNDEN = {anno} AND MESDEN = {mese} AND PRODEN = {proDen} AND PREV = 'S'";
        foreach (DataRow row in (InternalDataCollectionBase) db.GetDataTable(strSQL1).Rows)
        {
          string strSQL2 = $"UPDATE MODPREDET SET " +
                           $"NUMMOV = {DBMethods.DoublePeakForSql(numMov)} " +
                           $"WHERE CODPOS = {codPos} AND ANNDEN = {anno} AND MESDEN = {mese} AND PRODEN = {proDen} AND PRODENDET = {row["PRODENDET"]} AND MAT = {row["MAT"]}";
          flag = db.WriteTransactionData(strSQL2, CommandType.Text);
          if (!flag)
            break;
        }
        return flag;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool AggiornaTestataEDettaglio(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen,
      string numSan,
      string tipMov,
      int progMov,
      string partita,
      int annBilSan)
    {
      bool flag = false;
      try
      {
        string str = $"UPDATE DENTES SET " +
                     $"DATSAN = current_date, " +
                     $"NUMSAN = {DBMethods.DoublePeakForSql(numSan)}, " +
                     $"ANNBILSAN = {annBilSan}";
        
        if (tipMov == "DP")
          str = $"{str}, TIPANNSAN = TIPANNMOV";

        string strSQL1 = $"{str} WHERE CODPOS = {codPos} AND ANNDEN = {anno} AND MESDEN = {mese} AND PRODEN = {proDen}";

        if (db.WriteTransactionData(strSQL1, CommandType.Text))
        {
          string strSQL2 = $"UPDATE DENDET SET " +
                           $"NUMSAN = {DBMethods.DoublePeakForSql(numSan)}, " +
                           $"PARTITASAN = {DBMethods.DoublePeakForSql(partita)}, " +
                           $"PROGMOVSAN = {progMov} " +
                           $"WHERE CODPOS = {codPos} AND ANNDEN = {anno} AND MESDEN = {mese} AND PRODEN = {proDen}";
          flag = db.WriteTransactionData(strSQL2, CommandType.Text);
        }
        return flag;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string GetCausaleSanzione(DataLayer db, string tipMovValue)
    {
      try
      {
        string strSQL = $"SELECT CODCAU FROM TIPMOVCAU WHERE TIPMOV = '{tipMovValue}' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
        return db.Get1ValueFromSQL(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static Tuple<decimal, string, string> ControllaSanzioni(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen,
      string tipMov)
    {
      try
      {
        string strSQL = $"SELECT VALUE(IMPSANDET, 0) AS IMPSANDET, VALUE(SANSOTSOG, '') AS SANSOTSOG, VALUE(RICSANUTE, '') AS RICSANUTE FROM DENTES " +
                        $"WHERE CODPOS = {codPos} AND ANNDEN = {anno} AND MESDEN = {mese} AND PRODEN = {proDen} AND TIPMOV = {DBMethods.DoublePeakForSql(tipMov)}";
        DataTable dataTable = db.GetDataTable(strSQL);
        return dataTable.Rows.Count > 0 
            ? Tuple.Create<decimal, string, string>(
                !DBNull.Value.Equals(dataTable.Rows[0]["IMPSANDET"]) 
                    ? Convert.ToDecimal(dataTable.Rows[0]["IMPSANDET"]) 
                    : 0.00M, 
                !DBNull.Value.Equals(dataTable.Rows[0]["SANSOTSOG"]) 
                    ? dataTable.Rows[0]["SANSOTSOG"].ToString() 
                    : string.Empty,
                !DBNull.Value.Equals(dataTable.Rows[0]["RICSANUTE"]) 
                    ? dataTable.Rows[0]["RICSANUTE"].ToString() 
                    : string.Empty) 
            : null;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool AggiornaContabilitaDENDET(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen,
      string numMov,
      string tipMov,
      int progMov,
      string partita)
    {
      try
      {
        string strSQL1 = $"UPDATE DENDET SET " +
                         $"FLGAPP = 'I', " +
                         $"DATCONMOV = {DBMethods.DoublePeakForSql(DBMethods.Db2Date(DateTime.Now.ToString("d")))}, " +
                         $"NUMMOV = {DBMethods.DoublePeakForSql(numMov)}, " +
                         $"PARTITA = {DBMethods.DoublePeakForSql(partita)}, " +
                         $"PROGMOV = {progMov} " +
                         $"WHERE CODPOS = {codPos} " +
                         $"AND ANNDEN = {anno} " +
                         $"AND MESDEN = {mese} " +
                         $"AND PRODEN = {proDen} " +
                         $"AND TIPMOV = {DBMethods.DoublePeakForSql(tipMov)}";
        
        bool flag = db.WriteTransactionData(strSQL1, CommandType.Text);
        
        return flag;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool AggiornaContabilitaDENTES(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen,
      string codCaus,
      string numMov,
      string dataScadenza,
      int annoBilancio,
      string tipMov)
    {
      try
      {
        string strSQL1 = $"UPDATE DENTES SET " +
                         $"FLGAPP = 'I', " +
                         $"DATCONMOV = {DBMethods.DoublePeakForSql(DBMethods.Db2Date(DateTime.Now.ToString("d")))}, " +
                         $"CODCAUMOV = {DBMethods.DoublePeakForSql(codCaus)}, " +
                         $"NUMMOV = {DBMethods.DoublePeakForSql(numMov)}, " +
                         $"DATSCA = {DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataScadenza))}, " +
                         $"ANNBILMOV = {annoBilancio}, " +
                         $"STADEN = 'A', " +
                         $"DATORARIC = CURRENT_TIMESTAMP " +
                         $"WHERE CODPOS = {codPos} " +
                         $"AND ANNDEN = {anno} " +
                         $"AND MESDEN = {mese} " +
                         $"AND PRODEN = {proDen} " +
                         $"AND TIPMOV = '{tipMov}'";
        
        bool flag = db.WriteTransactionData(strSQL1, CommandType.Text);
                
        return flag;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string GetDataScadenzaDenuncia2(
      DataLayer db,
      int mese,
      int anno,
      string codPos,
      int proDen)
    {
        string strSQL1 = $"SELECT CHAR(DATAPE) FROM DENTES WHERE CODPOS = {codPos} AND ANNDEN = {anno} AND MESDEN = {mese} AND PRODEN = {proDen}";
        string strDatSca = db.Get1ValueFromSQL(strSQL1, CommandType.Text).Substring(0, 10);
        string strSQL2 = $"SELECT VALORE FROM PARGENDET WHERE CODPAR = 13 AND {DBMethods.DoublePeakForSql(DBMethods.Db2Date($"01/{mese}/{anno}"))} BETWEEN DATINI AND DATFIN";
        int ggSanzione = Convert.ToInt32(db.Get1ValueFromSQL(strSQL2, CommandType.Text));
        strDatSca = DateTime.Parse(strDatSca).AddDays(ggSanzione).ToString("d");
        return strDatSca;
    }

    public static string GetDataScadenzaDenuncia(DataLayer db, int mese, int anno)
    {
        string strSQL = $"SELECT VALORE FROM PARGENDET WHERE CODPAR = 3 AND {DBMethods.DoublePeakForSql(DBMethods.Db2Date($"01/{mese}/{anno}"))} BETWEEN DATINI AND DATFIN";
        return db.Get1ValueFromSQL(strSQL, CommandType.Text);
    }

    public static string GetCodCau(DataLayer db, string tipMov)
    {
      try
      {
        string strSQL = $"SELECT CODCAU FROM TIPMOVCAU WHERE TIPMOV = {DBMethods.DoublePeakForSql(tipMov)} AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
        return db.Get1ValueFromSQL(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static List<DatiContabilita> GetDatiContabilita2(
      int annoBilancio,
      string codPos,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();

      string strSQL = $"SELECT TIPMOV, 0 AS ANNCOM, IMPADDREC, IMPABB, IMPASSCON, {annoBilancio} AS ANNBIL FROM DENTES " +
                      $"WHERE CODPOS = {codPos} AND ANNDEN = {anno} AND MESDEN = {mese} AND PRODEN = {proDen}";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          List<DatiContabilita> datiContabilita2 = new List<DatiContabilita>();
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            datiContabilita2.Add(new DatiContabilita()
            {
              TipMov = !DBNull.Value.Equals(row["TIPMOV"]) ? row["TIPMOV"].ToString() : string.Empty,
              AnnCom = !DBNull.Value.Equals(row["ANNCOM"]) ? Convert.ToInt32(row["ANNCOM"]) : 0,
              ImpAddRec = !DBNull.Value.Equals(row["IMPADDREC"]) ? Convert.ToDecimal(row["IMPADDREC"]) : 0.00M,
              ImpAbb = !DBNull.Value.Equals(row["IMPABB"]) ? Convert.ToDecimal(row["IMPABB"]) : 0.00M,
              ImpAssCon = !DBNull.Value.Equals(row["IMPASSCON"]) ? Convert.ToDecimal(row["IMPASSCON"]) : 0.00M,
              AnnBil = !DBNull.Value.Equals(row["ANNBIL"]) ? Convert.ToInt32(row["ANNBIL"]) : 0
            });
          outcome = 0;
          return datiContabilita2;
        }
        outcome = 2;
        return null;
      }

    public static List<DatiContabilita> GetDatiContabilita(
      decimal addRec,
      int annoBilancio,
      string codPos,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
        DataLayer dataLayer = new DataLayer();
      
        string strSQL = $"SELECT TIPMOV, ANNCOM, ROUND(((IMPCON / 100) * {addRec}), 2) AS IMPADDREC, IMPABB, IMPASSCON, {annoBilancio} AS ANNBIL FROM DENDET " +
                        $"WHERE CODPOS = {codPos} AND ANNDEN = {anno} AND MESDEN = {mese} AND PRODEN = {proDen} ORDER BY ANNCOM";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          List<DatiContabilita> datiContabilita = new List<DatiContabilita>();
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            datiContabilita.Add(new DatiContabilita
            {
              TipMov = !DBNull.Value.Equals(row["TIPMOV"]) ? row["TIPMOV"].ToString() : string.Empty,
              AnnCom = !DBNull.Value.Equals(row["ANNCOM"]) ? Convert.ToInt32(row["ANNCOM"]) : 0,
              ImpAddRec = !DBNull.Value.Equals(row["IMPADDREC"]) ? Convert.ToDecimal(row["IMPADDREC"]) : 0.00M,
              ImpAbb = !DBNull.Value.Equals(row["IMPABB"]) ? Convert.ToDecimal(row["IMPABB"]) : 0.00M,
              ImpAssCon = !DBNull.Value.Equals(row["IMPASSCON"]) ? Convert.ToDecimal(row["IMPASSCON"]) : 0.00M,
              AnnBil = !DBNull.Value.Equals(row["ANNBIL"]) ? Convert.ToInt32(row["ANNBIL"]) : 0
            });
          outcome = 0;
          return datiContabilita;
        }
        outcome = 2;
        return null;
    }

    public static int GetAnnoBilancio(int anno, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      if (anno == DateTime.Now.Year)
      {
        outcome = 0;
        return anno;
      }
      try
      {
        string strSQL = "SELECT VALORE FROM PARGENDET WHERE CODPAR = 2 AND current_date BETWEEN DATINI AND DATFIN";
        string annBil = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        if (string.IsNullOrEmpty(annBil))
        {
          outcome = 2;
          return 0;
        }
        outcome = 0;
        return Convert.ToInt32(annBil);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string GetContabilizAutomatica()
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT VALORE FROM PARGENDET WHERE CODPAR = 11 AND current_date BETWEEN DATINI AND DATFIN";
        return dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string GetTimeStampDipa2()
    {
      DataLayer dataLayer = new DataLayer();
      string strSQL = "SELECT CHAR(CURRENT_TIMESTAMP) FROM DENTES FETCH FIRST 1 ROWS ONLY"; 
      return dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
    }

    public static List<RetribuzioneRDL> GetDataFromDENDET(
      string codPos,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      var codPosParam = dataLayer.CreateParameter("@codPos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
      var annoParam = dataLayer.CreateParameter("@anno", iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input, anno.ToString());
      var meseParam = dataLayer.CreateParameter("@mese", iDB2DbType.iDB2Decimal, 2, ParameterDirection.Input, mese.ToString());
      var proDenParam = dataLayer.CreateParameter("@proDen", iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, proDen.ToString());
      try
      {
        string strSQL = "SELECT A.*, (B.COG || ' ' || B.NOM) AS NOME FROM DENDET A " +
                        "INNER JOIN ISCT B ON A.MAT = B.MAT " +
                        "WHERE CODPOS = @codPos AND ANNDEN = @anno AND MESDEN = @mese AND PRODEN = @proDen AND TIPMOV = 'DP' " +
                        "ORDER BY NOME, MAT, DAL, AL";

        DataTable dataTable = dataLayer.GetDataTableWithParameters(strSQL, codPosParam, annoParam, meseParam, proDenParam);
        
        if (dataTable.Rows.Count > 0)
        {
          var dataFromDendet = new List<RetribuzioneRDL>();

          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          {
            var retribuzioneRdl = new RetribuzioneRDL
            {
                Dal = !DBNull.Value.Equals(row["DAL"]) ? Convert.ToDateTime(row["DAL"]).ToString("d") : string.Empty,
                Al = !DBNull.Value.Equals(row["AL"]) ? Convert.ToDateTime(row["AL"]).ToString("d") : string.Empty,
                Eta65 = Convert.ToChar(row["ETA65"]),
                TipRap = !DBNull.Value.Equals(row["TIPRAP"]) ? Convert.ToInt32(row["TIPRAP"]) : 0,
                CodCon = !DBNull.Value.Equals(row["CODCON"]) ? Convert.ToInt32(row["CODCON"]) : 0,
                ProCon = !DBNull.Value.Equals(row["PROCON"]) ? Convert.ToDecimal(row["PROCON"]) : 0M,
                CodLoc = !DBNull.Value.Equals(row["CODLOC"]) ? Convert.ToInt32(row["CODLOC"]) : 0,
                ProLoc = !DBNull.Value.Equals(row["PROLOC"]) ? Convert.ToDecimal(row["PROLOC"]) : 0M,
                CodLiv = !DBNull.Value.Equals(row["CODLIV"]) ? Convert.ToInt32(row["CODLIV"]) : 0,
                CodQuaCon = !DBNull.Value.Equals(row["CODQUACON"]) ? Convert.ToInt32(row["CODQUACON"]) : 0,
                PerPar = !DBNull.Value.Equals(row["PERPAR"]) ? Convert.ToDecimal(row["PERPAR"]) : 0M,
                PerApp = !DBNull.Value.Equals(row["PERAPP"]) ? Convert.ToDecimal(row["PERAPP"]) : 0M,
                ImpMin = !DBNull.Value.Equals(row["IMPMIN"]) ? Convert.ToDecimal(row["IMPMIN"]) : 0M,
                ImpTraEco = !DBNull.Value.Equals(row["IMPTRAECO"]) ? Convert.ToDecimal(row["IMPTRAECO"]) : 0M,
                ImpSca = !DBNull.Value.Equals(row["IMPSCA"]) ? Convert.ToDecimal(row["IMPSCA"]) : 0M,
                Fap = DBNull.Value.Equals(row["FAP"]) ? string.Empty : row["FAP"].ToString(),
                ImpAbb = !DBNull.Value.Equals(row["IMPABB"]) ? Convert.ToDecimal(row["IMPABB"]) : 0M,
                ImpAssCon = !DBNull.Value.Equals(row["IMPASSCON"]) ? Convert.ToDecimal(row["IMPASSCON"]) : 0M,
                TipSpe = DBNull.Value.Equals(row["TIPSPE"]) ? string.Empty : row["TIPSPE"].ToString(),
                CodGruAss = !DBNull.Value.Equals(row["CODGRUASS"]) ? Convert.ToInt32(row["CODGRUASS"]) : 0,
                Aliquota = !DBNull.Value.Equals(row["ALIQUOTA"]) ? Convert.ToDecimal(row["ALIQUOTA"]) : 0M,
                PerFap = !DBNull.Value.Equals(row["PERFAP"]) ? Convert.ToDecimal(row["PERFAP"]) : 0M,
                Mat = !DBNull.Value.Equals(row["MAT"]) ? Convert.ToInt32(row["MAT"]) : 0,
                ProRap = !DBNull.Value.Equals(row["PRORAP"]) ? Convert.ToInt32(row["PRORAP"]) : 0,
                DatNas = DBNull.Value.Equals(row["DATNAS"]) ? string.Empty : row["DATNAS"].ToString(),
                DatDec = DBNull.Value.Equals(row["DATDEC"]) ? string.Empty : row["DATDEC"].ToString(),
                DatCes = DBNull.Value.Equals(row["DATCES"]) ? string.Empty : row["DATCES"].ToString(),
                Prev = DBNull.Value.Equals(row["PREV"]) ? string.Empty : row["PREV"].ToString(),
                ImpRet = !DBNull.Value.Equals(row["IMPRET"]) ? Convert.ToDecimal(row["IMPRET"]) : 0M,
                ImpOcc = !DBNull.Value.Equals(row["IMPOCC"]) ? Convert.ToDecimal(row["IMPOCC"]) : 0M,
                ImpCon = !DBNull.Value.Equals(row["IMPCON"]) ? Convert.ToDecimal(row["IMPCON"]) : 0M,
                ImpFig = !DBNull.Value.Equals(row["IMPFIG"]) ? Convert.ToDecimal(row["IMPFIG"]) : 0M,
                ImpFap = !DBNull.Value.Equals(row["IMPFAP"]) ? Convert.ToDecimal(row["IMPFAP"]) : 0M,
                NumGGAzi = !DBNull.Value.Equals(row["NUMGGAZI"]) ? Convert.ToDecimal(row["NUMGGAZI"]) : 0M,
                NumGGFig = !DBNull.Value.Equals(row["NUMGGFIG"]) ? Convert.ToDecimal(row["NUMGGFIG"]) : 0M,
                NumGGSos = !DBNull.Value.Equals(row["NUMGGSOS"]) ? Convert.ToDecimal(row["NUMGGSOS"]) : 0M,
                NumGGPer = !DBNull.Value.Equals(row["NUMGGPER"]) ? Convert.ToInt32(row["NUMGGPER"]) : 0,
                NumGGDom = !DBNull.Value.Equals(row["NUMGGDOM"]) ? Convert.ToInt32(row["NUMGGDOM"]) : 0,
                NumGGConAzi = !DBNull.Value.Equals(row["NUMGGCONAZI"]) ? Convert.ToDecimal(row["NUMGGCONAZI"]) : 0M,
                ProDenDet = !DBNull.Value.Equals(row["PRODENDET"]) ? Convert.ToInt32(row["PRODENDET"]) : 0,
                DatEro = DBNull.Value.Equals(row["DATERO"]) ? string.Empty : row["DATERO"].ToString()
            };

            dataFromDendet.Add(retribuzioneRdl);
          }
          outcome = 0;
          return dataFromDendet;
        }
        outcome = 2;
        return null;
      }
      catch (Exception ex)
      {
        outcome = 1;
        return null;
      }
    }

    public static Dentes_Data GetDataFromDENTES(
      string codPos,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      var codPosParam = dataLayer.CreateParameter("@codPos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
      var annoParam = dataLayer.CreateParameter("@anno", iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input, anno.ToString());
      var meseParam = dataLayer.CreateParameter("@mese", iDB2DbType.iDB2Decimal, 2, ParameterDirection.Input, mese.ToString());
      var proDenParam = dataLayer.CreateParameter("@proDen", iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, proDen.ToString());
      try
      {
        string strSQL = "SELECT CHAR(ULTAGG) AS TIMESTAMP, CHAR(DATAPE) AS TMSTAPE, DENTES.* FROM DENTES " +
                        "WHERE CODPOS = @codPos AND ANNDEN = @anno AND MESDEN = @mese AND PRODEN = @proDen";

        DataTable dataTable = dataLayer.GetDataTableWithParameters(strSQL, codPosParam, annoParam, meseParam, proDenParam);

        if (dataTable.Rows.Count > 0)
        {
          var dataFromDentes = new Dentes_Data
          {
              CodPos = !DBNull.Value.Equals(dataTable.Rows[0]["CODPOS"]) ? dataTable.Rows[0]["CODPOS"].ToString() : string.Empty,
              AnnDen = !DBNull.Value.Equals(dataTable.Rows[0]["ANNDEN"]) ? Convert.ToDecimal(dataTable.Rows[0]["ANNDEN"]) : 0.00M,
              MesDen = !DBNull.Value.Equals(dataTable.Rows[0]["MESDEN"]) ? Convert.ToDecimal(dataTable.Rows[0]["MESDEN"]) : 0.00M,
              TimeStamp = !DBNull.Value.Equals(dataTable.Rows[0]["TIMESTAMP"]) ? dataTable.Rows[0]["TIMESTAMP"].ToString() : string.Empty,
              TmstApe = !DBNull.Value.Equals(dataTable.Rows[0]["TMSTAPE"]) ? dataTable.Rows[0]["TMSTAPE"].ToString() : string.Empty,
              TipMov = !DBNull.Value.Equals(dataTable.Rows[0]["TIPMOV"]) ? dataTable.Rows[0]["TIPMOV"].ToString() : string.Empty,
              ImpDis = !DBNull.Value.Equals(dataTable.Rows[0]["IMPDIS"]) ? Convert.ToDecimal(dataTable.Rows[0]["IMPDIS"]) : 0.00M,
              ImpAbb = !DBNull.Value.Equals(dataTable.Rows[0]["IMPABB"]) ? Convert.ToDecimal(dataTable.Rows[0]["IMPABB"]) : 0.00M,
              ImpAddRec = !DBNull.Value.Equals(dataTable.Rows[0]["IMPADDREC"]) ? Convert.ToDecimal(dataTable.Rows[0]["IMPADDREC"]) : 0.00M,
              ImpAssCon = !DBNull.Value.Equals(dataTable.Rows[0]["IMPASSCON"]) ? Convert.ToDecimal(dataTable.Rows[0]["IMPASSCON"]) : 0.00M
          };
          outcome = 0;
          return dataFromDentes;
        }
        outcome = 2;
        return null;
      }
      catch (Exception ex)
      {
        outcome = 1;
        return null;
      }
    }

    public static int CheckNotificheUfficio(string codPos, int anno, int mese, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      var codPosParam = dataLayer.CreateParameter("@codPos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
      var annoParam = dataLayer.CreateParameter("@anno", iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input, anno.ToString());
      var meseParam = dataLayer.CreateParameter("@mese", iDB2DbType.iDB2Decimal, 2, ParameterDirection.Input, mese.ToString());

      try
      {
        //string strSQL = $"SELECT COUNT(*) AS NOTIFICHE FROM DENTES WHERE CODPOS = {codPos} " +
        //                $"AND ANNDEN = {anno} " +
        //                $"AND MESDEN = {mese} " +
        //                "AND TIPMOV = 'NU' AND DATCONMOV IS NOT NULL AND DATMOVANN IS NULL";
        string strSQL = "SELECT COUNT(*) AS NOTIFICHE FROM DENTES " +
                        "WHERE CODPOS = @codPos AND ANNDEN = @anno AND MESDEN = @mese AND TIPMOV = 'NU' AND DATCONMOV IS NOT NULL AND DATMOVANN IS NULL";
                //string strNumNotifiche = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        string strNumNotifiche = dataLayer.GetDataTableWithParameters(strSQL, codPosParam, annoParam, meseParam).Rows[0].ElementAt("NOTIFICHE");
        if (!string.IsNullOrEmpty(strNumNotifiche))
        {
          outcome = 0;
          return Convert.ToInt32(strNumNotifiche);
        }
        outcome = 2;
        return 0;
      }
      catch (Exception ex)
      {
        outcome = 1;
        return 0;
      }
    }

    public static DatiSanitario GetDataSanit2(int idDipa, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = string.Format("SELECT * FROM FIACONSIP.DIPATESTMP WHERE IDDIPA = {0}", (object) idDipa);
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          DatiSanitario dataSanit2 = new DatiSanitario();
          dataSanit2.IdDipaDef = !DBNull.Value.Equals(dataTable.Rows[0]["IDDIPADEF"]) ? Convert.ToInt32(dataTable.Rows[0]["IDDIPA"]) : 0;
          dataSanit2.CodModPag = !DBNull.Value.Equals(dataTable.Rows[0]["CODMODPAG"]) ? Convert.ToInt32(dataTable.Rows[0]["CODMODPAG"]) : 0;
          dataSanit2.DatVer = !DBNull.Value.Equals(dataTable.Rows[0]["DATVER"]) ? dataTable.Rows[0]["DATVER"].ToString() : string.Empty;
          dataSanit2.ImpVer = !DBNull.Value.Equals(dataTable.Rows[0]["IMPVER"]) ? Convert.ToDecimal(dataTable.Rows[0]["IMPVER"]) : 0.00M;
          outcome = new int?(0);
          return dataSanit2;
        }
        outcome = new int?(2);
        return (DatiSanitario) null;
      }
      catch (Exception ex)
      {
        outcome = new int?(1);
        return (DatiSanitario) null;
      }
    }

    public static DatiSanitario GetDataSanit(int idDipa, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = string.Format("SELECT * FROM FIACONSIP.DIPATES WHERE IDDIPA = {0}", (object) idDipa);
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          DatiSanitario dataSanit = new DatiSanitario();
          dataSanit.IdDipa = !DBNull.Value.Equals(dataTable.Rows[0]["IDDIPA"]) ? Convert.ToInt32(dataTable.Rows[0]["IDDIPA"]) : 0;
          dataSanit.CodModPag = !DBNull.Value.Equals(dataTable.Rows[0]["CODMODPAG"]) ? Convert.ToInt32(dataTable.Rows[0]["CODMODPAG"]) : 0;
          dataSanit.DatVer = !DBNull.Value.Equals(dataTable.Rows[0]["DATVER"]) ? dataTable.Rows[0]["DATVER"].ToString() : string.Empty;
          dataSanit.ImpVer = !DBNull.Value.Equals(dataTable.Rows[0]["IMPVER"]) ? Convert.ToDecimal(dataTable.Rows[0]["IMPVER"]) : 0.00M;
          outcome = new int?(0);
          return dataSanit;
        }
        outcome = new int?(2);
        return (DatiSanitario) null;
      }
      catch (Exception ex)
      {
        outcome = new int?(1);
        return (DatiSanitario) null;
      }
    }

    public static Decimal GetTotFondoSanitario(
      string codPos,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = " SELECT VALUE(SUM(IMPSAN), 0) FROM FIACONSIP.DIPADET WHERE CODPOS = " + codPos + string.Format(" AND ANNDEN = {0} AND MESDEN = {1} AND PRODEN = {2} AND DATMOVANN IS NULL", (object) anno, (object) mese, (object) proDen);
        string str = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        if (!string.IsNullOrEmpty(str))
        {
          outcome = new int?(0);
          return Convert.ToDecimal(str);
        }
        outcome = new int?(2);
        return 0M;
      }
      catch (Exception ex)
      {
        outcome = new int?(1);
        return 0M;
      }
    }

    public static int GetPeriodiSenzaImporto(
      string codPos,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT COUNT(MAT) FROM DENDET WHERE IMPRET = 0 AND IMPFIG = 0 AND CODPOS = " + codPos + string.Format(" AND ANNDEN = {0} AND MESDEN = {1} AND PRODEN = {2}", (object) anno, (object) mese, (object) proDen);
        string str = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        if (!string.IsNullOrEmpty(str))
        {
          outcome = new int?(0);
          return Convert.ToInt32(str);
        }
        outcome = new int?(2);
        return 0;
      }
      catch (Exception ex)
      {
        outcome = new int?(1);
        return 0;
      }
    }

    public static string GetTimeStampDipa(
      string codPos,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
      try
      {
        DataLayer dataLayer = new DataLayer();
        string strSQL = string.Format("SELECT CHAR(ULTAGG) AS ULTAGG FROM DENTES WHERE CODPOS = {0} AND ANNDEN = {1} And MESDEN = {2}", (object) codPos, (object) anno, (object) mese) + string.Format(" AND PRODEN = {0} AND TIPMOV ='DP'", (object) proDen);
        outcome = new int?(0);
        return dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        outcome = new int?(1);
        return (string) null;
      }
    }

    public static Decimal GetAddizionale(out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT VALUE(VALORE, '0') FROM PARGEN A INNER JOIN PARGENDET B ON A.CODPAR = " + "B.CODPAR WHERE A.CODPAR = 5 AND CURRENT DATE BETWEEN B.DATINI AND B.DATFIN";
        string str = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        if (!string.IsNullOrEmpty(str))
        {
          outcome = new int?(0);
          return Convert.ToDecimal(str);
        }
        outcome = new int?(2);
        return 0M;
      }
      catch (Exception ex)
      {
        outcome = new int?(1);
        return 0M;
      }
    }

    public static string GetIBAN(string codPos, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT IBAN FROM DENTES WHERE CODPOS = " + codPos + "  AND IBAN IS NOT NULL ORDER BY ANNDEN DESC FETCH FIRST 1 ROWS ONLY";
        string iban = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        if (!string.IsNullOrEmpty(iban))
        {
          outcome = new int?(0);
          return iban;
        }
        outcome = new int?(2);
        return (string) null;
      }
      catch (Exception ex)
      {
        outcome = new int?(1);
        return (string) null;
      }
    }

    public static Decimal GetTasso(out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT VALUE(TASSO, 0.00) FROM TIPMOVCAU WHERE TIPMOV = 'RD' AND CURRENT_DATE BETWEEN DATINI AND DATFIN " + " AND TIPMOV = 'RD'";
        string str = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        if (!string.IsNullOrEmpty(str))
        {
          outcome = new int?(0);
          return Convert.ToDecimal(str);
        }
        outcome = new int?(2);
        return 0M;
      }
      catch (Exception ex)
      {
        outcome = new int?(1);
        return 0M;
      }
    }

    public static TotaliDenuncia CaricaTotaliDenuncia(
      string codPos,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      List<TotaliDenuncia> totaliDenunciaList = new List<TotaliDenuncia>();
      try
      {
        string strSQL = "SELECT IMPDEC, IMPCON, IMPADDREC, IMPABB, IMPASSCON, VALUE(IMPSANDET, 0) AS IMPSANDET, " + "VALUE(CODMODPAG, 0) AS CODMODPAG, IMPVER, DATVER, UFFPOS, CITDIC, PRODIC, IBAN, NUMMOV, TIPMOV, " + "STADEN, NUMSANANN, SANSOTSOG, VALUE(ESIRET, 'N') AS ESIRET, IMPCONDEL, IMPADDRECDEL, " + "IMPABBDEL, IMPASSCONDEL, VALUE(IMPSANRET, 0) AS IMPSANRET, NUMSAN, CODLINE, PROTRIC FROM " + string.Format("DENTES WHERE CODPOS = {0} AND ANNDEN = {1} AND MESDEN = {2} AND PRODEN = {3}", (object) codPos, (object) anno, (object) mese, (object) proDen);
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            totaliDenunciaList.Add(new TotaliDenuncia()
            {
              ImpDec = !DBNull.Value.Equals(row["IMPDEC"]) ? Convert.ToDecimal(row["IMPDEC"]) : 0.00M,
              ImpCon = !DBNull.Value.Equals(row["IMPCON"]) ? Convert.ToDecimal(row["IMPCON"]) : 0.00M,
              ImpAddRec = !DBNull.Value.Equals(row["IMPADDREC"]) ? Convert.ToDecimal(row["IMPADDREC"]) : 0.00M,
              ImpAbb = !DBNull.Value.Equals(row["IMPABB"]) ? Convert.ToDecimal(row["IMPABB"]) : 0.00M,
              ImpAssCon = !DBNull.Value.Equals(row["IMPASSCON"]) ? Convert.ToDecimal(row["IMPASSCON"]) : 0.00M,
              ImpSanDet = !DBNull.Value.Equals(row["IMPSANDET"]) ? Convert.ToDecimal(row["IMPSANDET"]) : 0.00M,
              CodModPag = !DBNull.Value.Equals(row["CODMODPAG"]) ? Convert.ToInt32(row["CODMODPAG"]) : 0,
              ImpVer = !DBNull.Value.Equals(row["IMPVER"]) ? Convert.ToDecimal(row["IMPVER"]) : 0.00M,
              DatVer = !DBNull.Value.Equals(row["DATVER"]) ? row["DATVER"].ToString().Trim() : string.Empty,
              UffPos = !DBNull.Value.Equals(row["UFFPOS"]) ? row["UFFPOS"].ToString().Trim() : string.Empty,
              CitDic = !DBNull.Value.Equals(row["CITDIC"]) ? row["CITDIC"].ToString().Trim() : string.Empty,
              ProDic = !DBNull.Value.Equals(row["PRODIC"]) ? row["PRODIC"].ToString().Trim() : string.Empty,
              Iban = !DBNull.Value.Equals(row["IBAN"]) ? row["IBAN"].ToString().Trim() : string.Empty,
              NumMov = !DBNull.Value.Equals(row["NUMMOV"]) ? row["NUMMOV"].ToString().Trim() : string.Empty,
              TipMov = !DBNull.Value.Equals(row["TIPMOV"]) ? row["TIPMOV"].ToString().Trim() : string.Empty,
              StaDen = !DBNull.Value.Equals(row["STADEN"]) ? row["STADEN"].ToString().Trim() : string.Empty,
              NumSanAnn = !DBNull.Value.Equals(row["NUMSANANN"]) ? row["NUMSANANN"].ToString().Trim() : string.Empty,
              SanSotSog = !DBNull.Value.Equals(row["SANSOTSOG"]) ? row["SANSOTSOG"].ToString().Trim() : string.Empty,
              Esiret = !DBNull.Value.Equals(row["ESIRET"]) ? row["ESIRET"].ToString().Trim() : string.Empty,
              ImpConDel = !DBNull.Value.Equals(row["IMPCONDEL"]) ? Convert.ToDecimal(row["IMPCONDEL"]) : 0.00M,
              ImpAddRecDel = !DBNull.Value.Equals(row["IMPADDRECDEL"]) ? Convert.ToDecimal(row["IMPADDRECDEL"]) : 0.00M,
              ImpAbbDel = !DBNull.Value.Equals(row["IMPABBDEL"]) ? Convert.ToDecimal(row["IMPABBDEL"]) : 0.00M,
              ImpAssConDel = !DBNull.Value.Equals(row["IMPASSCONDEL"]) ? Convert.ToDecimal(row["IMPASSCONDEL"]) : 0.00M,
              ImpSanRet = !DBNull.Value.Equals(row["IMPSANRET"]) ? Convert.ToDecimal(row["IMPSANRET"]) : 0.00M,
              NumSan = !DBNull.Value.Equals(row["NUMSAN"]) ? row["NUMSAN"].ToString().Trim() : string.Empty,
              CodLine = !DBNull.Value.Equals(row["CODLINE"]) ? row["CODLINE"].ToString().Trim() : string.Empty,
              ProtRic = !DBNull.Value.Equals(row["PROTRIC"]) ? row["PROTRIC"].ToString().Trim() : string.Empty,
              Protocollo = new(!DBNull.Value.Equals(row["PROTRIC"]) ? row["PROTRIC"].ToString().Trim() : string.Empty)
            });
          outcome = new int?(0);
          return totaliDenunciaList[0];
        }
        outcome = new int?(2);
        return (TotaliDenuncia) null;
      }
      catch (Exception ex)
      {
        outcome = new int?(1);
        return (TotaliDenuncia) null;
      }
    }

    public static bool AggiornaVariazSospensioni(
      DataLayer db,
      RetribuzioneRDL denuncia,
      RetribuzioneRDL report,
      string codPos,
      int anno,
      int mese,
      int proDen)
    {
        var impFigParam = db.CreateParameter("@impFig", iDB2DbType.iDB2Decimal, 12, ParameterDirection.Input,
            report.ImpFig.ToString().Replace(",", "."));

        var numGgSosParam = db.CreateParameter("@numGgSos", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input,
            report.NumGGSos.ToString().Replace(",", "."));

        var numGgAziParam = db.CreateParameter("@numGgAzi", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input,
            report.NumGGSos.ToString().Replace(",", "."));

        var numGgFigParam = db.CreateParameter("@numGgFig", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input,
            report.NumGGFig.ToString().Replace(",", "."));

        var numGgPerParam = db.CreateParameter("@numGgPer", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input,
            report.NumGGPer.ToString().Replace(",", "."));

        var numGgDomParam = db.CreateParameter("@numGgDom", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input,
            report.NumGGDom.ToString().Replace(",", "."));

        var numGgConAziParam = db.CreateParameter("@numGgConAzi", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input,
            report.NumGGConAzi.ToString().Replace(",", "."));

        var codPosParam = db.CreateParameter("@codPos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
        var annoParam = db.CreateParameter("@anno", iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input, anno.ToString());
        var meseParam = db.CreateParameter("@mese", iDB2DbType.iDB2Decimal, 2, ParameterDirection.Input, mese.ToString());
        var proDenParam = db.CreateParameter("@proDen", iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, proDen.ToString());

        var matParam = db.CreateParameter("@mat", iDB2DbType.iDB2Decimal, 9, ParameterDirection.Input,
            denuncia.Mat.ToString());

        var proDenDetParam = db.CreateParameter("@proDenDet", iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input,
            denuncia.ProDenDet.ToString());
      try
      {
                //string strSQL = $"UPDATE DENDET SET " +
                //                $"IMPFIG = {report.ImpFig.ToString().Replace(",", ".")}, " + 
                //                $"NUMGGSOS = {report.NumGGSos.ToString().Replace(",", ".")}, " + 
                //                $"NUMGGAZI = {report.NumGGAzi.ToString().Replace(",", ".")}, " + 
                //                $"NUMGGFIG = {report.NumGGFig.ToString().Replace(",", ".")}, " + 
                //                $"NUMGGPER = {report.NumGGPer.ToString().Replace(",", ".")}, " + 
                //                $"NUMGGDOM = {report.NumGGDom.ToString().Replace(",", ".")}, " +
                //                $"NUMGGCONAZI = {report.NumGGConAzi.ToString().Replace(",", ".")} " +
                //                $"WHERE CODPOS = {codPos} " +
                //                $"AND ANNDEN = {anno} " +
                //                $"AND MESDEN = {mese} " +
                //                $"AND PRODEN = {proDen} " +
                //                $"AND MAT = {denuncia.Mat} " +
                //                $"AND PRODENDET = {denuncia.ProDenDet}";
                string strSQL = "UPDATE DENDET SET " +
                                "IMPFIG = @impFig, NUMGGSOS = @numGgSos, NUMGGAZI = @numGgAzi, NUMGGFIG = @numGgFig, NUMGGPER = @numGgPer, NUMGGDOM = @numGgDom, NUMGGCONAZI = @numGgConAzi " +
                                "WHERE CODPOS = @codPos AND ANNDEN = @anno AND MESDEN = @mese AND PRODEN = @proDen AND MAT = @mat AND PRODENDET = @proDenDet";

                return db.WriteTransactionDataWithParametersAndDontCall(strSQL, CommandType.Text, 
                    impFigParam, numGgSosParam, numGgAziParam, numGgFigParam, numGgPerParam, numGgDomParam, numGgConAziParam, 
                    codPosParam, annoParam, meseParam, proDenParam, matParam, proDenDetParam);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject(HttpContext.Current.Session["utente"] as OCM.Utente.Utente), "");
        throw;
      }
    }

    public static bool EliminaDenuncia(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen,
      int idDipa)
    {
        var codPosParam = db.CreateParameter("@codPos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
        var annoParam = db.CreateParameter("@anno", iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input, anno.ToString());
        var meseParam = db.CreateParameter("@mese", iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, mese.ToString());
        var proDenParam = db.CreateParameter("@proDen", iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, proDen.ToString());

      try
      {
        string strSQL1 = "UPDATE MODPREDET SET PRODEN = NULL, PRODENDET = NULL " + 
                         "WHERE CODPOS = @codPos " +
                         "AND ANNDEN = @anno " +
                         "AND MESDEN = @mese " +
                         "AND PRODEN = @proDen " +
                         "AND EXISTS (SELECT * FROM MODPRE " +
                            "INNER JOIN MODPREDET ON MODPREDET.CODPOS = MODPRE.CODPOS AND MODPREDET.MAT = MODPRE.MAT AND MODPREDET.PRORAP = MODPRE.PRORAP AND MODPREDET.PROMOD = MODPRE.PROMOD " +
                            "WHERE MODPRE.CODPOS = @codPos " +
                            "AND ANNDEN = @anno " +
                            "AND MESDEN = @mese " +
                            "AND PRODEN = @proDen " +
                            "AND CODSTAPRE = 0)";
        if (!db.WriteTransactionDataWithParametersAndDontCall(strSQL1, CommandType.Text, codPosParam, annoParam, meseParam, proDenParam, codPosParam, annoParam, meseParam, proDenParam))
            return false;
        
        string strSQL2 = "DELETE FROM DENDETALI " +
                         "WHERE CODPOS = @codPos " +
                         "AND ANNDEN = @anno " +
                         "AND MESDEN = @mese " +
                         "AND PRODEN = @proDen";
        if (!db.WriteTransactionDataWithParametersAndDontCall(strSQL2, CommandType.Text, codPosParam, annoParam, meseParam, proDenParam)) 
            return false;

        string strSQL3 = "DELETE FROM DENDET " +
                         "WHERE CODPOS = @codPos " +
                         "AND ANNDEN = @anno " +
                         "AND MESDEN = @mese " +
                         "AND PRODEN = @proDen";
        if (!db.WriteTransactionDataWithParametersAndDontCall(strSQL3, CommandType.Text, codPosParam, annoParam, meseParam, proDenParam))
            return false;

        string strSQL4 = "DELETE FROM DENTES " +
                         "WHERE CODPOS = @codPos " +
                         "AND ANNDEN = @anno " +
                         "AND MESDEN = @mese " +
                         "AND PRODEN = @proDen";
        return db.WriteTransactionDataWithParametersAndDontCall(strSQL4, CommandType.Text, codPosParam, annoParam, meseParam, proDenParam);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject(HttpContext.Current.Session["utente"] as OCM.Utente.Utente), "");
        throw;
      }
    }

    public static int ControllaPrevDegliIscritti(
      string codPos,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT COUNT(*) FROM DENDET B " +
                        "INNER JOIN MODPREDET C ON B.CODPOS = C.CODPOS AND B.ANNDEN = C.ANNDEN AND B.MESDEN = C.MESDEN AND B.MAT = C.MAT AND B.PRORAP = C.PRORAP AND B.PRODEN = C.PRODEN AND B.PRODENDET = C.PRODENDET " +
                        "INNER JOIN MODPRE A ON A.CODPOS = C.CODPOS AND A.MAT = C.MAT AND A.PRORAP = C.PRORAP AND A.PROMOD = C.PROMOD " +
                        $"WHERE B.CODPOS = {codPos} " +
                        $"AND B.ANNDEN = {anno} " +
                        $"AND B.MESDEN = {mese} " +
                        $"AND B.PRODEN = {proDen} " +
                        $"AND B.PREV = 'S' " +
                        $"AND A.CODSTAPRE <> 0 " +
                        $"AND A.DATANN IS NULL " +
                        $"AND (B.IMPRET <> 0 OR B.IMPFIG <> 0)";
        string str = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        if (!string.IsNullOrEmpty(str))
        {
          outcome = 0;
          return Convert.ToInt32(str);
        }
        outcome = 2;
        return -1;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = 1;
        return -1;
      }
    }

    public static decimal GetTotFondo(
      string codPos,
      int anno,
      int mese,
      int idDipa,
      int proDen,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = $"SELECT VALUE(IMPDISSAN, 0) FROM FIACONSIP.DIPATES WHERE CODPOS = {(object)codPos} AND ANNDEN = {(object)anno} AND MESDEN = {(object)mese}" + string.Format(" AND PRODEN = {0} AND IDDIPA = {1} AND DATMOVANN IS NULL", (object) proDen, (object) idDipa);
        string str = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        if (!string.IsNullOrEmpty(str))
        {
          outcome = new int?(0);
          return Convert.ToDecimal(str);
        }
        outcome = new int?(2);
        return 0M;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0M;
      }
    }

    public static int GetProMod(string codPos, int mat, int proRap, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = string.Format("SELECT PROMOD FROM MODPRE WHERE CODPOS = {0} AND MAT = {1} AND PRORAP = {2} AND DATANN IS NULL", (object) codPos, (object) mat, (object) proRap);
        string str = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        if (!string.IsNullOrEmpty(str))
        {
          outcome = new int?(0);
          return Convert.ToInt32(str);
        }
        outcome = new int?(2);
        return 0;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0;
      }
    }

    public static List<RetribuzioneRDL> GetData(
      string codPos,
      int idDipa,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
      List<RetribuzioneRDL> data = new List<RetribuzioneRDL>();
      DataLayer dataLayer = new DataLayer();
      var codPosParam = dataLayer.CreateParameter("@codPos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
      var annoParam = dataLayer.CreateParameter("@anno", iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input, anno.ToString());
      var meseParam = dataLayer.CreateParameter("@mese", iDB2DbType.iDB2Decimal, 2, ParameterDirection.Input, mese.ToString());
      var proDenParam = dataLayer.CreateParameter("@proDen", iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, proDen.ToString());

      try
      {
        string strSQL = "SELECT B.MAT, (TRIM(A.COG) || ' ' || TRIM(A.NOM)) AS NOME, " +
                        "TRANSLATE(CHAR(DAL,EUR),'/','.') AS DAL, " +
                        "TRANSLATE(CHAR(AL,EUR),'/','.') AS AL, " +
                        "LEFT((SELECT DISTINCT TRIM(DENQUA) FROM QUACON WHERE QUACON.CODQUACON = B.CODQUACON), 1) AS DENQUA, " +
                        "CODLIV, PERAPP, IMPRET, " +
                        "(SELECT DENLIV FROM CONLIV WHERE CODCON = B.CODCON AND CODLIV = B.CODLIV AND PROCON = B.PROCON) AS LIVELLO, " +
                        "'' AS SANITARIO, '' AS IMPSAN, IMPOCC , IMPFIG , ALIQUOTA, IMPCON, " +
                        "(CASE IMPABB WHEN  0 THEN 'N' ELSE 'S' END) AS ABBPRE, " +
                        "(CASE IMPASSCON WHEN  0 THEN 'N' ELSE 'S' END) AS ASSCON, PRORAP, PRODENDET, VALUE(PREV, 'N') AS PREV, 0 AS PROMOD FROM DENDET B " +
                        "INNER JOIN ISCT A ON B.MAT = A.MAT " +
                        $"WHERE CODPOS = @codPos AND ANNDEN = @anno AND MESDEN = @mese AND PRODEN = @proDen AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL AND B.TIPMOV IN ('DP', 'NU') ORDER BY MAT, PRODENDET";
        DataTable dataTable = dataLayer.GetDataTableWithParameters(strSQL, codPosParam, annoParam, meseParam, proDenParam);

        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            data.Add(new RetribuzioneRDL()
            {
              Mat = !DBNull.Value.Equals(row["MAT"]) ? Convert.ToInt32(row["MAT"]) : 0,
              Nome = !DBNull.Value.Equals(row["NOME"]) ? row["NOME"].ToString() : string.Empty,
              Dal = !DBNull.Value.Equals(row["DAL"]) ? row["DAL"].ToString() : string.Empty,
              Al = !DBNull.Value.Equals(row["AL"]) ? row["AL"].ToString() : string.Empty,
              DenQua = !DBNull.Value.Equals(row["DENQUA"]) ? row["DENQUA"].ToString() : string.Empty,
              CodLiv = !DBNull.Value.Equals(row["CODLIV"]) ? Convert.ToInt32(row["CODLIV"]) : 0,
              PerApp = !DBNull.Value.Equals(row["PERAPP"]) ? Convert.ToDecimal(row["PERAPP"]) : 0M,
              ImpRet = !DBNull.Value.Equals(row["IMPRET"]) ? Convert.ToDecimal(row["IMPRET"]) : 0M,
              Livello = !DBNull.Value.Equals(row["LIVELLO"]) ? row["LIVELLO"].ToString() : string.Empty,
              Sanitario = !DBNull.Value.Equals(row["SANITARIO"]) ? row["SANITARIO"].ToString() : string.Empty,
              ImpSanit = !DBNull.Value.Equals(row["IMPSAN"]) ? row["IMPSAN"].ToString() : string.Empty, //A cosa serve? Viene usato l'iddipa per prenderlo
              ImpOcc = !DBNull.Value.Equals(row["IMPOCC"]) ? Convert.ToDecimal(row["IMPOCC"]) : 0M,
              ImpFig = !DBNull.Value.Equals(row["IMPFIG"]) ? Convert.ToDecimal(row["IMPFIG"]) : 0M,
              Aliquota = !DBNull.Value.Equals(row["ALIQUOTA"]) ? Convert.ToDecimal(row["ALIQUOTA"]) : 0M,
              ImpCon = !DBNull.Value.Equals(row["IMPCON"]) ? Convert.ToDecimal(row["IMPCON"]) : 0M,
              AbbPre = !DBNull.Value.Equals(row["ABBPRE"]) ? row["ABBPRE"].ToString() : string.Empty,
              AssCon = !DBNull.Value.Equals(row["ASSCON"]) ? row["ASSCON"].ToString() : string.Empty,
              ProRap = !DBNull.Value.Equals(row["PRORAP"]) ? Convert.ToInt32(row["PRORAP"]) : 0,
              ProDenDet = !DBNull.Value.Equals(row["PRODENDET"]) ? Convert.ToInt32(row["PRODENDET"]) : 0,
              Prev = !DBNull.Value.Equals(row["PREV"]) ? row["PREV"].ToString() : string.Empty,
              ProMod = !DBNull.Value.Equals(row["PROMOD"]) ? Convert.ToInt32(row["PROMOD"]) : 0
            });

          outcome = 0;
          return data;
        }

        outcome = 2;
        return null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) data));
        outcome = 1;
        return null;
      }
    }

    public static int CheckRettifiche(
      string codPos,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT COUNT(*) FROM DENDET " +
                        $"WHERE CODPOS = {codPos} AND ANNDEN = {anno} AND MESDEN = {mese} AND PRODEN = {proDen} " +
                        "AND TIPMOV = 'RT' AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL";
        string str = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);

        if (!string.IsNullOrEmpty(str))
        {
          outcome = 0;
          return Convert.ToInt32(str);
        }
        outcome = 2;
        return 0;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = 1;
        return 0;
      }
    }

    public static SediAziendali GetSedeLegale(string codPos, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      SediAziendali sedeLegale = new SediAziendali();
      try
      {
        string strSQL = "SELECT B.DENIND, CHAR(B.ULTAGG) AS ULTAGG, B.TIPIND, A.IND, A.DENLOC AS LOC_SEDE, A.CAP " + "AS CAP_SEDE, A.SIGPRO AS PROV_SEDE, A.TEL1, A.FAX, A.EMAIL,TRANSLATE(CHAR(A.DATINI, EUR), " + "'/', '.') AS DATINI FROM INDSED A INNER JOIN TIPIND B ON A.TIPIND = B.TIPIND LEFT JOIN " + "CODLOC AS LOCSEDE ON A.DENLOC = LOCSEDE.DENLOC AND A.CAP = LOCSEDE.CAP AND A.SIGPRO = " + "LOCSEDE.SIGPRO WHERE A.CODPOS = " + codPos + " AND current_date BETWEEN A.DATINI AND VALUE(A.DATFIN, '9999-12-31')";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          sedeLegale.Indirizzo = !DBNull.Value.Equals(dataTable.Rows[0]["IND"]) ? dataTable.Rows[0]["IND"].ToString() : string.Empty;
          sedeLegale.Localita = !DBNull.Value.Equals(dataTable.Rows[0]["LOC_SEDE"]) ? dataTable.Rows[0]["LOC_SEDE"].ToString() : string.Empty;
          sedeLegale.Provincia = !DBNull.Value.Equals(dataTable.Rows[0]["PROV_SEDE"]) ? dataTable.Rows[0]["PROV_SEDE"].ToString() : string.Empty;
          sedeLegale.Telefono = !DBNull.Value.Equals(dataTable.Rows[0]["TEL1"]) ? dataTable.Rows[0]["TEL1"].ToString() : string.Empty;
          outcome = new int?(0);
          return sedeLegale;
        }
        outcome = new int?(2);
        return (SediAziendali) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) sedeLegale));
        outcome = new int?(1);
        return (SediAziendali) null;
      }
    }

    public static DatiAnagraficiAzienda GetDatiAnagraficiAzienda_2(
      string codPos,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      var codPosParam = dataLayer.CreateParameter("@codPos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
      
      DatiAnagraficiAzienda anagraficiAzienda2 = new DatiAnagraficiAzienda();
      
      try
      {
        string strSQL = "SELECT DISTINCT A.INSAZI, A.AZINOTC, A.CODPOS, A.RAGSOC, A.RAGSOCBRE, A.CODFIS AS CODFISAZI, A.PARIVA, A.NATGIU, CHAR(A.ULTAGG) AS ULTAGG, NATGIU.DENNATGIU, TRANSLATE(CHAR(A.DATAPE, EUR), '/', '.') AS DATAPE, TRANSLATE(CHAR(A.DATCHI, EUR), '/', '.') AS DATCHI, C.CATATTCAM, D.DENATTCAM, C.CODATTCAM, TRANSLATE(CHAR(C.DATINI, EUR), '/', '.') AS DATINIATT, F.CODSTACON, F.DENCODSTA FROM AZI A " +
                        "INNER JOIN NATGIU ON A.NATGIU = NATGIU.NATGIU " +
                        "LEFT JOIN AZIATT AS C ON A.CODPOS = C.CODPOS AND C.DATFIN = '9999-12-31' " +
                        "LEFT JOIN TIPATT AS D ON C.CATATTCAM = D.CATATTCAM AND C.CODATTCAM = D.CODATTCAM " +
                        "LEFT JOIN AZISTO AS E ON A.CODPOS = E.CODPOS AND current_date BETWEEN E.DATINI AND VALUE(E.DATFIN, '9999-12-31') " +
                        "LEFT JOIN CODSTA AS F ON E.CODSTACON = F.CODSTACON " +
                        "WHERE A.CODPOS = @codPos";
        DataTable dataTable = dataLayer.GetDataTableWithParameters(strSQL, codPosParam);

        if (dataTable.Rows.Count > 0)
        {
          anagraficiAzienda2.CodFiscAziendale = dataTable.Rows[0]["CODFISAZI"].ToString();
          anagraficiAzienda2.PartitaIVA = dataTable.Rows[0]["PARIVA"].ToString();
          anagraficiAzienda2.NatGiu = (decimal) dataTable.Rows[0]["NATGIU"];
          
          outcome = 0;
          return anagraficiAzienda2;
        }

        outcome = 2;
        return null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) anagraficiAzienda2));
        outcome = 1;
        return null;
      }
    }

    public static int GetTotDipendenti(
      string codPos,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = $"SELECT COUNT(DISTINCT(MAT)) AS MAT FROM DENDET WHERE CODPOS = {codPos} AND ANNDEN = {anno} AND MESDEN = {mese} AND PRODEN = {proDen}";
        string totDipendenti = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        if (!string.IsNullOrEmpty(totDipendenti))
        {
          outcome = 0;
          return Convert.ToInt32(totDipendenti);
        }
        outcome = 2;
        return 0;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = 1;
        return 0;
      }
    }

    public static TotaliTestata GetTotaliTestata(
      string codPos,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      var codPosParam = dataLayer.CreateParameter("@codPos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
      var annoParam = dataLayer.CreateParameter("@anno", iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input, anno.ToString());
      var meseParam = dataLayer.CreateParameter("@mese", iDB2DbType.iDB2Decimal, 2, ParameterDirection.Input, mese.ToString());
      var proDenParam = dataLayer.CreateParameter("@proDen", iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, proDen.ToString());
      
      TotaliTestata totaliTestata = new TotaliTestata();
      try
      {
        string strSQL = "SELECT DATAPE, IMPRET, IMPOCC, IMPFIG, TIPMOV FROM DENTES WHERE CODPOS = @codPos AND ANNDEN = @anno AND MESDEN = @mese AND PRODEN = @proDen";
        DataTable dataTable = dataLayer.GetDataTableWithParameters(strSQL, codPosParam, annoParam, meseParam, proDenParam);

        if (dataTable.Rows.Count > 0)
        {
          totaliTestata.TotImpRet = !DBNull.Value.Equals(dataTable.Rows[0]["IMPRET"]) ? Convert.ToDecimal(dataTable.Rows[0]["IMPRET"]) : 0M;
          totaliTestata.TotImpOcc = !DBNull.Value.Equals(dataTable.Rows[0]["IMPOCC"]) ? Convert.ToDecimal(dataTable.Rows[0]["IMPOCC"]) : 0M;
          totaliTestata.TotImpFig = !DBNull.Value.Equals(dataTable.Rows[0]["IMPFIG"]) ? Convert.ToDecimal(dataTable.Rows[0]["IMPFIG"]) : 0M;
          totaliTestata.TipMov = !DBNull.Value.Equals(dataTable.Rows[0]["TIPMOV"]) ? dataTable.Rows[0]["TIPMOV"].ToString() : string.Empty;
          totaliTestata.DataDenuncia = !DBNull.Value.Equals(dataTable.Rows[0]["DATAPE"]) ? dataTable.Rows[0]["DATAPE"].ToString() : string.Empty;
          outcome = 0;
          return totaliTestata;
        }
        outcome = 2;
        return null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject(totaliTestata));
        outcome = 1;
        return null;
      }
    }

    public static int GetIdDipa(string codPos, int anno, int mese, int proDen, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = string.Format("SELECT IDDIPA FROM FIACONSIP.DIPATES WHERE CODPOS = {0} AND ANNDEN = {1} AND MESDEN = {2}", (object) codPos, (object) anno, (object) mese) + string.Format(" AND PRODEN = {0} AND DATMOVANN IS NULL", (object) proDen);
        string str = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        if (!string.IsNullOrEmpty(str))
        {
          outcome = new int?(0);
          return Convert.ToInt32(str);
        }
        outcome = new int?(2);
        return 0;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0;
      }
    }

    public static string GetTotSanitario(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen,
      int idDipa)
    {
      try
      {
        string strSQL = "SELECT VALUE(SUM(IMPSAN), 0) AS IMPSAN FROM FIACONSIP.DIPADETTMP WHERE CODPOS = " + codPos + string.Format(" AND ANNDEN = {0} AND MESDEN = {1} AND PRODEN = {2} AND IDDIPA = {3}", (object) anno, (object) mese, (object) proDen, (object) idDipa);
        string str = db.Get1ValueFromSQL(strSQL, CommandType.Text);
        return !string.IsNullOrEmpty(str) ? str : "0,00";
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        throw;
      }
    }

    public static bool AggiornaTestata(
      DataLayer db,
      string impSanitario,
      string codPos,
      int anno,
      int mese,
      int proDen,
      int idDipa)
    {
      try
      {
        string strSQL = "UPDATE FIACONSIP.DIPATESTMP SET IMPDISSAN = " + impSanitario + " WHERE CODPOS = " + codPos + string.Format(" AND ANNDEN = {0} AND MESDEN = {1} AND PRODEN = {2} AND IDDIPA = {3}", (object) anno, (object) mese, (object) proDen, (object) idDipa);
        return db.WriteTransactionData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        throw;
      }
    }

    public static bool AggiornaTestata(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen)
    {
      try
      {
        string strSQL = "UPDATE DENTES SET IMPDIS = IMPCON + IMPASSCON + IMPADDREC + IMPABB WHERE CODPOS = " + codPos + string.Format(" AND ANNDEN = {0} AND MESDEN = {1} AND PRODEN = {2} AND TIPMOV = 'DP'", (object) anno, (object) mese, (object) proDen);
        return db.WriteTransactionData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        throw;
      }
    }

    public static bool AggiornaTestata(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen,
      Decimal addRec)
    {
      try
      {
        string strSQL = string.Format("UPDATE DENTES SET IMPADDREC = ROUND(((IMPCON / 100) * {0} ), 2) WHERE CODPOS = {1}", (object) addRec, (object) codPos) + string.Format(" AND ANNDEN = {0} AND MESDEN = {1} AND PRODEN = {2} AND TIPMOV = 'DP'", (object) anno, (object) mese, (object) proDen);
        return db.WriteTransactionData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        throw;
      }
    }

    public static bool AggiornaTestata(
      DataLayer db,
      TotaliDettaglio totDett,
      Decimal impAbb,
      Decimal impAssCon,
      int count,
      string codPos,
      int anno,
      int mese,
      int proDen)
    {
      try
      {
        string strSQL = "UPDATE DENTES SET IMPRET = " + totDett.TotImpRet + ", IMPOCC = " + totDett.TotImpOcc + ", IMPFIG = " + totDett.TotImpFig + ", IMPCON = " + totDett.TotImpCon +
                        $", IMPABB = {impAbb.ToString().Replace(",", ".")}, IMPASSCON = {impAssCon.ToString().Replace(",", ".")}, NUMRIGDET = {(object)count} WHERE CODPOS = {(object)codPos} AND ANNDEN = {(object)anno} AND MESDEN = {(object)mese}" +
                        $" AND PRODEN = {(object)proDen} AND TIPMOV = 'DP'";
        return db.WriteTransactionData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        throw;
      }
    }

    public static TotaliDettaglio GetTotaliDettaglio(
      DataLayer db,
      string codPos,
      int anno,
      int mese,
      int proDen)
    {
      TotaliDettaglio totaliDettaglio = new TotaliDettaglio();
      try
      {
        string strSQL = "SELECT SUM(IMPRET) AS IMPRET, SUM(IMPOCC) AS IMPOCC, SUM(IMPFIG) AS IMPFIG, " + string.Format("SUM(IMPCON) AS IMPCON FROM DENDET WHERE CODPOS = {0} AND ANNDEN = {1}", (object) codPos, (object) anno) + string.Format(" AND MESDEN = {0} AND PRODEN = {1} AND TIPMOV = 'DP'", (object) mese, (object) proDen);
        DataTable dataTable = db.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (TotaliDettaglio) null;
        totaliDettaglio.TotImpRet = !DBNull.Value.Equals(dataTable.Rows[0]["IMPRET"]) ? dataTable.Rows[0]["IMPRET"].ToString().Replace(',', '.') : string.Empty;
        totaliDettaglio.TotImpOcc = !DBNull.Value.Equals(dataTable.Rows[0]["IMPOCC"]) ? dataTable.Rows[0]["IMPOCC"].ToString().Replace(',', '.') : string.Empty;
        totaliDettaglio.TotImpFig = !DBNull.Value.Equals(dataTable.Rows[0]["IMPFIG"]) ? dataTable.Rows[0]["IMPFIG"].ToString().Replace(',', '.') : string.Empty;
        totaliDettaglio.TotImpCon = !DBNull.Value.Equals(dataTable.Rows[0]["IMPCON"]) ? dataTable.Rows[0]["IMPCON"].ToString().Replace(',', '.') : string.Empty;
        return totaliDettaglio;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) totaliDettaglio));
        throw;
      }
    }

    public static string GetDataScadenza(int anno, string meseFixato, out int? outcome)
    {
      try
      {
        string dataScadenza = new DataLayer().Get1ValueFromSQL("SELECT VALORE FROM PARGEN A INNER JOIN PARGENDET B ON A.CODPAR = B.CODPAR WHERE A.CODPAR = 3 AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(string.Format("01/{0}/{1}", (object) meseFixato, (object) anno))) + " BETWEEN B.DATINI AND B.DATFIN", CommandType.Text);
        if (!string.IsNullOrEmpty(dataScadenza))
        {
          outcome = new int?(0);
          return dataScadenza;
        }
        outcome = new int?(2);
        return (string) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return (string) null;
      }
    }

    public static Decimal GetImportoAbbonamento(
      int anno,
      string meseFixato,
      out int? outcome)
    {
      try
      {
        string str = new DataLayer().Get1ValueFromSQL("SELECT VALORE FROM PARGEN A INNER JOIN PARGENDET B ON A.CODPAR = B.CODPAR WHERE A.CODPAR = 1" + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(string.Format("01/{0}/{1}", (object) meseFixato, (object) anno))) + " BETWEEN B.DATINI AND B.DATFIN", CommandType.Text);
        if (!string.IsNullOrEmpty(str))
        {
          outcome = new int?(0);
          return Convert.ToDecimal(str);
        }
        outcome = new int?(2);
        return 0M;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0M;
      }
    }

    public static Dictionary<string, string> GetTipoDenuncia(
      string codPos,
      int anno,
      string meseFixato,
      out int? outcome)
    {
      Dictionary<string, string> tipoDenuncia = new Dictionary<string, string>();
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT TIPISC, ABB FROM AZISTO WHERE CODPOS = " + codPos + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(string.Format("01/{0}/{1}", (object) meseFixato, (object) anno))) + " <= VALUE(DATFIN, '9999-12-31') ORDER BY DATFIN DESC";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          tipoDenuncia.Add("TIPISC", dataTable.Rows[0]["TIPISC"].ToString());
          tipoDenuncia.Add("ABB", dataTable.Rows[0]["ABB"].ToString());
          outcome = new int?(0);
          return tipoDenuncia;
        }
        outcome = new int?(2);
        return (Dictionary<string, string>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) tipoDenuncia));
        outcome = new int?(1);
        return (Dictionary<string, string>) null;
      }
    }

    public static string GetTimeStampDENTES(string codPos, int anno, int mese, out int? outcome)
    {
      try
      {
        string timeStampDentes = new DataLayer().Get1ValueFromSQL(string.Format("SELECT CHAR(ULTAGG) AS ULTAGG FROM DENTES WHERE CODPOS = {0} AND ANNDEN = {1}", (object) codPos, (object) anno) + string.Format(" AND MESDEN = {0} AND DATMOVANN IS NULL AND TIPMOV ='DP' AND STADEN = 'N'", (object) mese), CommandType.Text);
        outcome = new int?(0);
        return timeStampDentes;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return (string) null;
      }
    }

    public static List<SospensioneRapporto> GetListaSospensioni(
      string mat,
      string proRap,
      string codPos,
      string dataIniSos,
      string datFinSos,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      var matParam = dataLayer.CreateParameter("@mat", iDB2DbType.iDB2Decimal, 9, ParameterDirection.Input, mat);
      var proRapParam = dataLayer.CreateParameter("@proRap", iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, proRap);
      var codPosParam = dataLayer.CreateParameter("@codPos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
      var datIniSosParam = dataLayer.CreateParameter("@datIniSos", iDB2DbType.iDB2Date, 10, ParameterDirection.Input, DBMethods.Db2Date(dataIniSos));
      var datFinSosParam = dataLayer.CreateParameter("@datFinSos", iDB2DbType.iDB2Date, 10, ParameterDirection.Input, DBMethods.Db2Date(datFinSos));
      
      List<SospensioneRapporto> listaSospensioni = new List<SospensioneRapporto>();
      
      try
      {
        string strSQL = "SELECT TRANSLATE(CHAR(DATINISOS,EUR),'/','.') AS DAL, TRANSLATE(CHAR(DATFINSOS,EUR),'/','.') AS AL, TRIM(DENSOS) AS SOSPENSIONE FROM SOSRAP A, CODSOS B " +
                        "WHERE A.CODSOS = B.CODSOS AND A.MAT = @mat AND A.PRORAP = @proRap AND A.CODPOS = @codPos AND A.STASOS = 0 AND (" +
                        "(DATINISOS BETWEEN @datIniSos AND @datFinSos) OR (VALUE(DATFINSOS, '9999-12-31') BETWEEN @datIniSos AND @datFinSos) OR (@datIniSos BETWEEN DATINISOS AND DATFINSOS) OR (@datFinSos BETWEEN DATINISOS AND DATFINSOS)) " +
                        "ORDER BY DATINISOS DESC, DATFINSOS";
        // DataTable dataTable = dataLayer.GetDataTable(strSQL);
        DataTable dataTable = dataLayer.GetDataTableWithParameters(strSQL, matParam, proRapParam, codPosParam, datIniSosParam, datFinSosParam, datIniSosParam, datFinSosParam, datIniSosParam, datFinSosParam);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          {
            SospensioneRapporto sospensioneRapporto = new SospensioneRapporto((Decimal) Convert.ToInt32(mat), 0M, row["DAL"].ToString(), row["AL"].ToString(), 0M, 0M, 0M, row["SOSPENSIONE"].ToString());
            listaSospensioni.Add(sospensioneRapporto);
          }
          outcome = 0;
          return listaSospensioni;
        }
        outcome = 2;
        return null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject(HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente), JsonConvert.SerializeObject(listaSospensioni));
        outcome = 1;
        return null;
      }
    }

    public static Decimal GetImportoSanitario(
      string codPos,
      string anno,
      int mese,
      int proDen,
      RetribuzioneRDL report,
      int idDIPA,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = string.Format("SELECT IMPSAN FROM FIACONSIP.DIPADETTMP WHERE CODPOS = {0} AND ANNDEN = {1} AND MESDEN = {2}", (object) codPos, (object) anno, (object) mese) + string.Format(" AND PRODEN = {0} AND MAT = {1} AND IDDIPA = {2}", (object) proDen, (object) report.Mat, (object) idDIPA);
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          Decimal importoSanitario = Convert.ToDecimal(dataTable.Rows[0]["IMPSAN"]);
          outcome = new int?(0);
          return importoSanitario;
        }
        outcome = new int?(2);
        return 0M;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0M;
      }
    }

    public static bool UpdateDendetData_4(
      DataLayer db,
      string codPos,
      string anno,
      int mese,
      int proDen,
      string dataFine,
      Raplav_Dendet_Data RD_Data,
      ref string functionName)
    {
      functionName = nameof (UpdateDendetData_4);
      try
      {
        string strSQL = "UPDATE DENDET SET AL = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataFine)) + ", DATCES = NULL WHERE CODPOS = " + codPos + string.Format(" AND ANNDEN = {0} AND MESDEN = {1} AND PRODEN = {2} AND MAT = {3}", (object) anno, (object) mese, (object) proDen, (object) RD_Data.Mat) + string.Format(" AND PRODENDET = {0}", (object) RD_Data.ProDenDet);
        return db.WriteData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        throw;
      }
    }

    public static bool UpdateDendetData_3(
      DataLayer db,
      string codPos,
      string anno,
      int mese,
      int proDen,
      Raplav_Dendet_Data RD_Data,
      ref string functionName)
    {
      functionName = nameof (UpdateDendetData_3);
      try
      {
        string strSQL = "UPDATE DENDET SET DATCES = NULL" + string.Format(" WHERE CODPOS = {0} AND ANNDEN = {1} AND MESDEN = {2} AND PRODEN = {3}", (object) codPos, (object) anno, (object) mese, (object) proDen) + string.Format(" AND MAT = {0} AND PRORAP = {1}", (object) RD_Data.Mat, (object) RD_Data.ProRap);
        return db.WriteData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        throw;
      }
    }

    public static List<Raplav_Dendet_Data> GetRaplavDendetData_2(
      string codPos,
      string anno,
      int mese,
      ref string functionName,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      List<Raplav_Dendet_Data> raplavDendetData2 = new List<Raplav_Dendet_Data>();
      functionName = nameof (GetRaplavDendetData_2);
      try
      {
        string strSQL = "SELECT DISTINCT RAPLAV.MAT, RAPLAV.PRORAP, DAL, PRODENDET FROM RAPLAV INNER JOIN DENDET ON RAPLAV.CODPOS = " + "DENDET.CODPOS AND RAPLAV.MAT = DENDET.MAT AND RAPLAV.PRORAP = DENDET.PRORAP WHERE RAPLAV.CODPOS = " + codPos + string.Format(" AND ANNDEN = {0} AND MESDEN = {1} AND DENDET.DATCES IS NOT NULL AND DENDET.DATCES = DENDET.AL AND RAPLAV.DATCES IS NULL", (object) anno, (object) mese);
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            raplavDendetData2.Add(new Raplav_Dendet_Data()
            {
              Mat = DBNull.Value.Equals(row["MAT"]) ? 0 : Convert.ToInt32(row["MAT"]),
              Dal = DBNull.Value.Equals(row["DAL"]) ? string.Empty : row["DAL"].ToString(),
              ProRap = DBNull.Value.Equals(row["PRORAP"]) ? 0 : Convert.ToInt32(row["PRORAP"]),
              ProDenDet = DBNull.Value.Equals(row["PRODENDET"]) ? 0 : Convert.ToInt32(row["PRODENDET"])
            });
          outcome = new int?(0);
          return raplavDendetData2;
        }
        outcome = new int?(2);
        return (List<Raplav_Dendet_Data>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) raplavDendetData2));
        outcome = new int?(1);
        return (List<Raplav_Dendet_Data>) null;
      }
    }

    public static List<RetribuzioneRDL> GetDenunceSalvate(
      string codPos,
      string anno,
      int mese,
      int proDen,
      ref string functionName,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      List<RetribuzioneRDL> denunceSalvate = new List<RetribuzioneRDL>();
      functionName = nameof (GetDenunceSalvate);
      try
      {
        string strSQL = "SELECT DENDET.*, '' AS SANITARIO FROM DENDET WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno + string.Format(" AND MESDEN = {0} AND PRODEN = {1} AND TIPMOV = 'DP' ORDER BY MAT, DAL, AL", (object) mese, (object) proDen);
        foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataTable(strSQL).Rows)
          denunceSalvate.Add(new RetribuzioneRDL()
          {
            Dal = DBNull.Value.Equals(row["DAL"]) ? string.Empty : Convert.ToDateTime(row["DAL"]).ToString("d"),
            Al = DBNull.Value.Equals(row["AL"]) ? string.Empty : Convert.ToDateTime(row["AL"]).ToString("d"),
            Eta65 = Convert.ToChar(row["ETA65"]),
            TipRap = !DBNull.Value.Equals(row["TIPRAP"]) ? Convert.ToInt32(row["TIPRAP"]) : 0,
            CodCon = !DBNull.Value.Equals(row["CODCON"]) ? Convert.ToInt32(row["CODCON"]) : 0,
            ProCon = !DBNull.Value.Equals(row["PROCON"]) ? Convert.ToDecimal(row["PROCON"]) : 0M,
            CodLoc = !DBNull.Value.Equals(row["CODLOC"]) ? Convert.ToInt32(row["CODLOC"]) : 0,
            ProLoc = !DBNull.Value.Equals(row["PROLOC"]) ? Convert.ToDecimal(row["PROLOC"]) : 0M,
            CodLiv = !DBNull.Value.Equals(row["CODLIV"]) ? Convert.ToInt32(row["CODLIV"]) : 0,
            CodQuaCon = !DBNull.Value.Equals(row["CODQUACON"]) ? Convert.ToInt32(row["CODQUACON"]) : 0,
            PerPar = !DBNull.Value.Equals(row["PERPAR"]) ? Convert.ToDecimal(row["PERPAR"]) : 0M,
            PerApp = !DBNull.Value.Equals(row["PERAPP"]) ? Convert.ToDecimal(row["PERAPP"]) : 0M,
            ImpMin = !DBNull.Value.Equals(row["IMPMIN"]) ? Convert.ToDecimal(row["IMPMIN"]) : 0M,
            ImpTraEco = !DBNull.Value.Equals(row["IMPTRAECO"]) ? Convert.ToDecimal(row["IMPTRAECO"]) : 0M,
            ImpSca = !DBNull.Value.Equals(row["IMPSCA"]) ? Convert.ToDecimal(row["IMPSCA"]) : 0M,
            Fap = DBNull.Value.Equals(row["FAP"]) ? string.Empty : row["FAP"].ToString(),
            ImpAbb = !DBNull.Value.Equals(row["IMPABB"]) ? Convert.ToDecimal(row["IMPABB"]) : 0M,
            ImpAssCon = !DBNull.Value.Equals(row["IMPASSCON"]) ? Convert.ToDecimal(row["IMPASSCON"]) : 0M,
            TipSpe = DBNull.Value.Equals(row["TIPSPE"]) ? string.Empty : row["TIPSPE"].ToString(),
            CodGruAss = !DBNull.Value.Equals(row["CODGRUASS"]) ? Convert.ToInt32(row["CODGRUASS"]) : 0,
            Aliquota = !DBNull.Value.Equals(row["ALIQUOTA"]) ? Convert.ToDecimal(row["ALIQUOTA"]) : 0M,
            PerFap = !DBNull.Value.Equals(row["PERFAP"]) ? Convert.ToDecimal(row["PERFAP"]) : 0M,
            Mat = !DBNull.Value.Equals(row["MAT"]) ? Convert.ToInt32(row["MAT"]) : 0,
            ProRap = !DBNull.Value.Equals(row["PRORAP"]) ? Convert.ToInt32(row["PRORAP"]) : 0,
            DatNas = DBNull.Value.Equals(row["DATNAS"]) ? string.Empty : row["DATNAS"].ToString(),
            DatDec = DBNull.Value.Equals(row["DATDEC"]) ? string.Empty : row["DATDEC"].ToString(),
            DatCes = DBNull.Value.Equals(row["DATCES"]) ? string.Empty : row["DATCES"].ToString(),
            Prev = DBNull.Value.Equals(row["PREV"]) ? string.Empty : row["PREV"].ToString(),
            ImpRet = !DBNull.Value.Equals(row["IMPRET"]) ? Convert.ToDecimal(row["IMPRET"]) : 0M,
            ImpOcc = !DBNull.Value.Equals(row["IMPOCC"]) ? Convert.ToDecimal(row["IMPOCC"]) : 0M,
            ImpCon = !DBNull.Value.Equals(row["IMPCON"]) ? Convert.ToDecimal(row["IMPCON"]) : 0M,
            ImpFig = !DBNull.Value.Equals(row["IMPFIG"]) ? Convert.ToDecimal(row["IMPFIG"]) : 0M,
            ImpFap = !DBNull.Value.Equals(row["IMPFAP"]) ? Convert.ToDecimal(row["IMPFAP"]) : 0M,
            NumGGAzi = !DBNull.Value.Equals(row["NUMGGAZI"]) ? Convert.ToDecimal(row["NUMGGAZI"]) : 0M,
            NumGGFig = !DBNull.Value.Equals(row["NUMGGFIG"]) ? Convert.ToDecimal(row["NUMGGFIG"]) : 0M,
            NumGGSos = !DBNull.Value.Equals(row["NUMGGSOS"]) ? Convert.ToDecimal(row["NUMGGSOS"]) : 0M,
            NumGGPer = !DBNull.Value.Equals(row["NUMGGPER"]) ? Convert.ToInt32(row["NUMGGPER"]) : 0,
            NumGGDom = !DBNull.Value.Equals(row["NUMGGDOM"]) ? Convert.ToInt32(row["NUMGGDOM"]) : 0,
            NumGGConAzi = !DBNull.Value.Equals(row["NUMGGCONAZI"]) ? Convert.ToDecimal(row["NUMGGCONAZI"]) : 0M,
            ProDenDet = !DBNull.Value.Equals(row["PRODENDET"]) ? Convert.ToInt32(row["PRODENDET"]) : 0,
            DatEro = DBNull.Value.Equals(row["DATERO"]) ? string.Empty : row["DATERO"].ToString()
          });
        outcome = new int?(0);
        return denunceSalvate;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) denunceSalvate));
        outcome = new int?(1);
        return (List<RetribuzioneRDL>) null;
      }
    }

    public static bool UpdateDendetData_1(
      DataLayer db,
      string codPos,
      string anno,
      int mese,
      int proDen,
      Raplav_Dendet_Data RD_Data,
      ref string functionName)
    {
      functionName = nameof (UpdateDendetData_1);
      try
      {
        string strSQL = "UPDATE DENDET SET DATCES = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(RD_Data.DatCes)) + string.Format(" WHERE CODPOS = {0} AND ANNDEN = {1} AND MESDEN = {2} AND PRODEN = {3}", (object) codPos, (object) anno, (object) mese, (object) proDen) + string.Format(" AND MAT = {0} AND PRORAP = {1}", (object) RD_Data.Mat, (object) RD_Data.ProRap);
        return db.WriteData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        throw;
      }
    }

    public static bool UpdateDendetData_2(
      DataLayer db,
      string codPos,
      string anno,
      int mese,
      int proDen,
      Raplav_Dendet_Data RD_Data,
      ref string functionName)
    {
      functionName = nameof (UpdateDendetData_2);
      try
      {
        string strSQL = "UPDATE DENDET SET AL = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(RD_Data.DatCes)) + ", DATCES = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(RD_Data.DatCes)) + " WHERE CODPOS = " + codPos + string.Format(" AND ANNDEN = {0} AND MESDEN = {1} AND PRODEN = {2} AND MAT = {3}", (object) anno, (object) mese, (object) proDen, (object) RD_Data.Mat) + string.Format(" AND PRODENDET = {0}", (object) RD_Data.ProDenDet);
        return db.WriteData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        throw;
      }
    }

    public static List<Raplav_Dendet_Data> GetRaplavDendetData(
      DataLayer db,
      string codPos,
      string anno,
      int mese,
      int proDen,
      ref string functionName)
    {
      List<Raplav_Dendet_Data> raplavDendetData = new List<Raplav_Dendet_Data>();
      functionName = nameof (GetRaplavDendetData);
      try
      {
        string strSQL = "SELECT DISTINCT RAPLAV.MAT, RAPLAV.DATCES, RAPLAV.PRORAP, PRODENDET FROM RAPLAV INNER JOIN " + "DENDET ON RAPLAV.CODPOS = DENDET.CODPOS AND RAPLAV.MAT = DENDET.MAT AND RAPLAV.PRORAP = " + "DENDET.PRORAP AND RAPLAV.DATCES < DENDET.AL WHERE RAPLAV.CODPOS = " + codPos + " AND YEAR(RAPLAV.DATCES) = " + anno + string.Format(" AND MONTH(RAPLAV.DATCES) = {0} AND ANNDEN = {1} AND MESDEN = {2} AND PRODEN = {3}", (object) mese, (object) anno, (object) mese, (object) proDen);
        DataTable dataTable = db.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (List<Raplav_Dendet_Data>) null;
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          raplavDendetData.Add(new Raplav_Dendet_Data()
          {
            Mat = (int) row["MAT"],
            DatCes = DBNull.Value.Equals(row["DATCES"]) ? string.Empty : Convert.ToDateTime(row["DATCES"]).ToString("d"),
            ProRap = (int) row["PRORAP"],
            ProDenDet = (int) row["PRODENDET"]
          });
        return raplavDendetData;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) raplavDendetData));
        throw;
      }
    }

    public static bool DeleteRowsFromDENTES(
      DataLayer db,
      string codPos,
      string anno,
      int mese,
      int proDen,
      ref string functionName)
    {
      functionName = nameof (DeleteRowsFromDENTES);
      try
      {
        string strSQL = string.Format("DELETE FROM DENTES WHERE CODPOS = {0} AND ANNDEN = {1} AND MESDEN = {2} AND PRODEN = {3}", (object) codPos, (object) anno, (object) mese, (object) proDen);
        return db.WriteData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        throw;
      }
    }

    public static bool DeleteRowsFromDENDET(
      DataLayer db,
      string codPos,
      string anno,
      int mese,
      int proDen,
      ref string functionName)
    {
      functionName = nameof (DeleteRowsFromDENDET);
      try
      {
        string strSQL = string.Format("DELETE FROM DENDET WHERE CODPOS = {0} AND ANNDEN = {1} AND MESDEN = {2} AND PRODEN = {3}", (object) codPos, (object) anno, (object) mese, (object) proDen);
        return db.WriteData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        throw;
      }
    }

    public static int GetIDAzi(string codPos, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT IDAZI FROM FIACONSIP.AZI WHERE CODPOS = " + codPos;
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          outcome = new int?(0);
          return Convert.ToInt32(dataTable.Rows[0]["IDAZI"]);
        }
        outcome = new int?(2);
        return 0;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0;
      }
    }

    public static string GetCurrentTimeStampDIPA(
      string codPos,
      int anno,
      int mese,
      int proDen,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL =
            $"SELECT CHAR(ULTAGG) AS ULTAGG FROM DENTES WHERE CODPOS = {codPos} AND ANNDEN = {anno} And MESDEN = {mese}" +
            $" AND PRODEN = {proDen} AND DATMOVANN IS NULL AND TIPMOV ='DP' AND STADEN = 'N'";
        string currentTimeStampDipa = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        outcome = new int?(0);
        return currentTimeStampDipa;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return (string) null;
      }
    }

    public static List<SediAziendali> GetSediAziendali(
      string codPos,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      List<SediAziendali> sediAziendali = new List<SediAziendali>();
      try
      {
        string strSQL = "SELECT B.DENIND, CHAR(B.ULTAGG) AS ULTAGG, B.TIPIND, A.IND, DUG.DENDUG, A.NUMCIV, A.DENLOC AS LOC_SEDE, " + "A.CAP AS CAP_SEDE, A.SIGPRO AS PROV_SEDE, A.TEL1, A.FAX, E.EMAIL, TRANSLATE(CHAR(A.DATINI, EUR), " + 
                    "'/', '.') AS DATINI FROM INDSED A INNER JOIN TIPIND B ON A.TIPIND = B.TIPIND INNER JOIN AZEMAIL E ON A.CODPOS = E.CODPOS LEFT JOIN DUG ON " + "A.CODDUG = DUG.CODDUG LEFT JOIN CODLOC AS LOCSEDE ON A.DENLOC = LOCSEDE.DENLOC AND A.CAP = " + 
                    "LOCSEDE.CAP AND A.SIGPRO = LOCSEDE.SIGPRO WHERE A.CODPOS = " + codPos + " AND A.DATFIN = '9999-12-31' AND A.TIPIND = 1";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            sediAziendali.Add(new SediAziendali()
            {
              Denominazione = !DBNull.Value.Equals(row["DENIND"]) ? (string) row["DENIND"] : string.Empty,
              UltAgg = !DBNull.Value.Equals(row["ULTAGG"]) ? (string) row["ULTAGG"] : string.Empty,
              Tipo = !DBNull.Value.Equals(row["TIPIND"]) ? Convert.ToInt32(row["TIPIND"]) : 0,
              Indirizzo = !DBNull.Value.Equals(row["IND"]) ? (string) row["IND"] : string.Empty,
              DenDug = !DBNull.Value.Equals(row["DENDUG"]) ? (string) row["DENDUG"] : string.Empty,
              NumeroCivico = !DBNull.Value.Equals(row["NUMCIV"]) ? (string) row["NUMCIV"] : string.Empty,
              Localita = !DBNull.Value.Equals(row["LOC_SEDE"]) ? (string) row["LOC_SEDE"] : string.Empty,
              CAP = !DBNull.Value.Equals(row["CAP_SEDE"]) ? (string) row["CAP_SEDE"] : string.Empty,
              Provincia = !DBNull.Value.Equals(row["PROV_SEDE"]) ? (string) row["PROV_SEDE"] : string.Empty,
              Telefono = !DBNull.Value.Equals(row["TEL1"]) ? (string) row["TEL1"] : string.Empty,
              Fax = !DBNull.Value.Equals(row["FAX"]) ? (string) row["FAX"] : string.Empty,
              EMail = !DBNull.Value.Equals(row["EMAIL"]) ? (string) row["EMAIL"] : string.Empty,
              DataInizio = !DBNull.Value.Equals(row["DATINI"]) ? (string) row["DATINI"] : string.Empty
            });
          outcome = new int?(0);
          return sediAziendali;
        }
        outcome = new int?(2);
        return (List<SediAziendali>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) sediAziendali));
        outcome = new int?(1);
        return (List<SediAziendali>) null;
      }
    }

    public static DatiAnagraficiAzienda GetDatiAnagraficiAzienda(
      string codPos,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      DatiAnagraficiAzienda anagraficiAzienda = new DatiAnagraficiAzienda();
      try
      {
        string strSQL = "SELECT DISTINCT A.CODPOS, A.RAGSOC, A.RAGSOCBRE, A.CODFIS AS CODFISAZI, A.PARIVA, A.NATGIU, B.ABB " + "FROM AZI A INNER JOIN NATGIU ON A.NATGIU = NATGIU.NATGIU LEFT JOIN AZISTO B ON A.CODPOS = B.CODPOS " + "WHERE A.CODPOS = " + codPos + " AND B.DATFIN = '9999-12-31'";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          anagraficiAzienda.CodFiscAziendale = dataTable.Rows[0]["CODFISAZI"].ToString();
          anagraficiAzienda.PartitaIVA = dataTable.Rows[0]["PARIVA"].ToString();
          anagraficiAzienda.NatGiu = (Decimal) dataTable.Rows[0]["NATGIU"];
          anagraficiAzienda.RagioneSociale = dataTable.Rows[0]["RAGSOC"].ToString();
          outcome = new int?(0);
          return anagraficiAzienda;
        }
        outcome = new int?(2);
        return (DatiAnagraficiAzienda) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) anagraficiAzienda));
        outcome = new int?(1);
        return (DatiAnagraficiAzienda) null;
      }
    }

    public static void GetStatoAttualeDenuncia(
      int codPos,
      int anno,
      int mese,
      string tipMov,
      out int? outcome,
      ref StatoAttualeDenuncia statoAttuale,
      int proDen = 0)
    {
      DataLayer dataLayer = new DataLayer();
      string empty = string.Empty;
      string str = string.Empty;
      if (proDen != 0)
        str = string.Format(" AND PRODEN = {0}", (object) proDen);
      try
      {
        string strSQL = !(tipMov == "AR") ? string.Format("SELECT TIPMOV, STADEN, VALUE(CODMODPAG, 0) AS CODMODPAG FROM DENTES WHERE CODPOS = {0}", (object) codPos) + string.Format(" AND ANNDEN = {0} AND MESDEN = {1} {2} AND TIPMOV IN ('DP', 'NU') AND DATMOVANN IS NULL", (object) anno, (object) mese, (object) str) : string.Format("SELECT STADEN, VALUE(CODMODPAG, 0) AS CODMODPAG FROM DENTES WHERE CODPOS = {0}", (object) codPos) + " AND STADEN = 'N' AND TIPMOV ='AR' AND DATMOVANN IS NULL";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          statoAttuale = new StatoAttualeDenuncia();
          statoAttuale.TipMov = dataTable.Rows[0]["TIPMOV"].ToString();
          statoAttuale.StaDen = dataTable.Rows[0]["STADEN"].ToString();
          statoAttuale.CodModPag = Convert.ToDecimal(dataTable.Rows[0]["CODMODPAG"]);
          outcome = new int?(0);
        }
        else
          outcome = new int?(2);
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
      }
    }

    public static string GetDataScadenzaDIPA(string dataCorrente, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      string oggetto_Pagina = "";
      try
      {
        string strSQL = "SELECT VALUE(VALORE, '0') FROM PARGEN A INNER JOIN PARGENDET B ON A.CODPAR = B.CODPAR WHERE B.CODPAR = 3 " + "AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataCorrente)) + " BETWEEN B.DATINI AND B.DATFIN";
        oggetto_Pagina = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        outcome = new int?(0);
        return oggetto_Pagina;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), oggetto_Pagina);
        outcome = new int?(1);
        return (string) null;
      }
    }

    public static int CountRecordFromRaplav(int codPos, int annDen, int mesDen, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string str1 = string.Format("{0}-{1}-01", (object) annDen, (object) (mesDen + 1));
        string str2 = string.Format("{0}-{1}-{2}", (object) annDen, (object) (mesDen + 1), (object) DateTime.DaysInMonth(annDen, mesDen + 1));
        string strSQL = string.Format("SELECT COUNT(*) FROM RAPLAV WHERE CODPOS = {0} AND VALUE(DATCES, '9999-12-31') > '{1}' AND ", (object) codPos, (object) str1) + "(DATDEC <= '" + str1 + "' OR DATDEC BETWEEN '" + str1 + "' AND '" + str2 + "')";
        int int32 = Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text));
        outcome = new int?(0);
        return int32;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0;
      }
    }

    public static string GetFirstYear(int codPos, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = string.Format("SELECT MAX(YEAR(DATDEC)) FROM RAPLAV WHERE CODPOS = {0} AND VALUE(DATCES, '9999-12-31') = '9999-12-31'", (object) codPos);
        string firstYear = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        outcome = new int?(0);
        return firstYear;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return (string) null;
      }
    }

    public static Dentes_Data GetDentesData(int codPos, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      Dentes_Data dentesData = new Dentes_Data();
      try
      {
        string strSQL = string.Format("SELECT ANNDEN, MESDEN, (ANNDEN || RIGHT('00' || MESDEN, 2)) AS X, STADEN FROM DENTES WHERE CODPOS = {0}", (object) codPos) + " AND TIPMOV IN ('NU','DP') AND DATMOVANN IS NULL AND ANNDEN >= 2003 ORDER BY X DESC";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          dentesData.AnnDen = (Decimal) dataTable.Rows[0]["ANNDEN"];
          dentesData.MesDen = (Decimal) dataTable.Rows[0]["MESDEN"];
          dentesData.DataComposta = (string) dataTable.Rows[0]["X"];
          dentesData.StaDen = (string) dataTable.Rows[0]["STADEN"];
          outcome = new int?(0);
          return dentesData;
        }
        outcome = new int?(2);
        return (Dentes_Data) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) dentesData));
        outcome = new int?(1);
        return (Dentes_Data) null;
      }
    }

    public static int GetPrimoAnnoDIPA(int codPos, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = string.Format("SELECT VALUE(MIN(ANNDEN), 0) FROM DENTES WHERE CODPOS = {0} AND TIPMOV IN ('NU','DP') ", (object) codPos) + "AND ANNDEN >= 2003 ";
        int int32 = Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text));
        outcome = new int?(0);
        return int32;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0;
      }
    }

    public static int GetPrimoAnnoRapportiAttivi(int codPos, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = string.Format("SELECT VALUE(MIN(YEAR(DATDEC)), 2003) FROM RAPLAV WHERE CODPOS = {0} AND ", (object) codPos) + "VALUE(DATCES, '9999-12-31') = '9999-12-31' AND YEAR(DATDEC) >= 2003";
        int int32 = Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text));
        outcome = new int?(0);
        return int32;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0;
      }
    }

    public static int GetDenunceNonConfermate(int codPos, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = string.Format("SELECT COUNT(*) FROM DENTES WHERE CODPOS = {0} AND DATCHI IS NULL AND ANNDEN >= 2003", (object) codPos);
        string str = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        if (!string.IsNullOrEmpty(str))
        {
          outcome = new int?(0);
          return Convert.ToInt32(str);
        }
        outcome = new int?(2);
        return -1;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return -1;
      }
    }

    public static string GetPayments(string codPos, out int? outcome)
    {
      string payments = (string) null;
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT IDTIPPAG, DISPAG FROM FIACONSIP.AZI WHERE CODPOS = " + codPos;
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          if (!DBNull.Value.Equals(dataTable.Rows[0]["DISPAG"]))
          {
            if (dataTable.Rows[0]["DISPAG"].ToString().Trim() != "")
              payments = "99";
            else if (!DBNull.Value.Equals(dataTable.Rows[0]["IDTIPPAG"]))
              payments = !(dataTable.Rows[0]["IDTIPPAG"].ToString().Trim() != "") ? "99" : dataTable.Rows[0]["IDTIPPAG"].ToString();
          }
          outcome = new int?(0);
          return payments;
        }
        outcome = new int?(2);
        return (string) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return (string) null;
      }
    }

    public static List<ContrattoDiRiferimento> GetReferralContracts(
      int codCon,
      int codLiv,
      string dal,
      out int? outcome)
    {
      List<ContrattoDiRiferimento> referralContracts = new List<ContrattoDiRiferimento>();
      DataLayer dataLayer = new DataLayer();
      string empty = string.Empty;
      try
      {
        if (codCon != 0)
        {
          string strSQL = "SELECT A.CODCON, A.PROCON, A.CODQUACON, A.DATINI, A.DATFIN, A.DATDEC, B.DENQUA, C.CODLIV, C.DENLIV, A.TIPSPE " + "FROM CONRIF A INNER JOIN QUACON B ON A.CODQUACON = B.CODQUACON INNER JOIN CONLIV C ON A.CODCON = C.CODCON " + "AND A.PROCON = C.PROCON " + string.Format(" WHERE A.CODCON = {0} AND DATDEC <= '{1}' AND CODLIV = {2} ", (object) codCon, (object) DBMethods.Db2Date(dal), (object) codLiv) + " ORDER BY A.DATDEC DESC ";
          foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataTable(strSQL).Rows)
          {
            ContrattoDiRiferimento contrattoDiRiferimento = new ContrattoDiRiferimento(Convert.ToInt32(row["CODCON"]), Convert.ToInt32(row["PROCON"]), Convert.ToInt32(row["CODQUACON"]), row["DATINI"].ToString(), row["DATFIN"].ToString(), row["DATDEC"].ToString(), (string) row["DENQUA"], Convert.ToInt32(row["CODLIV"]), (string) row["DENLIV"], (string) row["TIPSPE"]);
            referralContracts.Add(contrattoDiRiferimento);
          }
          outcome = new int?(0);
          return referralContracts;
        }
        outcome = new int?(2);
        return (List<ContrattoDiRiferimento>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) referralContracts));
        outcome = new int?(1);
        return (List<ContrattoDiRiferimento>) null;
      }
    }

    public static List<Contratto> GetLocalContracts(
      int codLoc,
      string dal,
      out int? outcome)
    {
      List<Contratto> localContracts = new List<Contratto>();
      DataLayer dataLayer = new DataLayer();
      string str = string.Empty;
      try
      {
        if (codLoc != 0)
          str = string.Format(" WHERE CODLOC = {0} AND DATDEC <= '{1}' ", (object) codLoc, (object) DBMethods.Db2Date(dal));
        string strSQL = "SELECT CODLOC, PROLOC, DATINI, DATFIN, DATDEC, TIPSPE FROM CONLOC " + str + " ORDER BY DATDEC DESC";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          {
            Contratto contratto = new Contratto((Decimal) row["CODLOC"], (Decimal) row["PROLOC"], row["DATINI"].ToString(), row["DATFIN"].ToString(), row["DATDEC"].ToString(), (string) row["TIPSPE"]);
            localContracts.Add(contratto);
          }
          outcome = new int?(0);
          return localContracts;
        }
        outcome = new int?(2);
        return (List<Contratto>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) localContracts));
        outcome = new int?(1);
        return (List<Contratto>) null;
      }
    }

    public static List<MinimiContrattuali> GetReferenceContractualMin(
      int codCon,
      Decimal proCon,
      int codLiv,
      string dal,
      out int? outcome)
    {
      List<MinimiContrattuali> referenceContractualMin = new List<MinimiContrattuali>();
      DataLayer dataLayer = new DataLayer();
      string empty = string.Empty;
      try
      {
        string strSQL = "SELECT CODCON, PROCON, CODLIV, DATAPPINI, DATAPPFIN, IMPVOCRET FROM " + "CONRET WHERE CODVOCRET <> 4 " + (string.Format("AND CODCON = {0} AND PROCON = {1} AND CODLIV = {2}  AND DATAPPINI <= '{3}' ", (object) codCon, (object) proCon, (object) codLiv, (object) DBMethods.Db2Date(dal)) + " AND DATAPPFIN >= '" + DBMethods.Db2Date(dal) + "' ") + " ORDER BY DATAPPINI DESC";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          {
            MinimiContrattuali minimiContrattuali = new MinimiContrattuali((Decimal) row["CODCON"], (Decimal) row["PROCON"], (Decimal) row["CODLIV"], row["DATAPPINI"].ToString(), row["DATAPPFIN"].ToString(), (Decimal) row["IMPVOCRET"]);
            referenceContractualMin.Add(minimiContrattuali);
          }
          outcome = new int?(0);
          return referenceContractualMin;
        }
        outcome = new int?(2);
        return (List<MinimiContrattuali>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) referenceContractualMin));
        outcome = new int?(1);
        return (List<MinimiContrattuali>) null;
      }
    }

    public static List<MinimiContrattuali> GetLocalContractualMin(
      int codLoc,
      Decimal proLoc,
      int codLiv,
      string dal,
      out int? outcome)
    {
      List<MinimiContrattuali> localContractualMin = new List<MinimiContrattuali>();
      DataLayer dataLayer = new DataLayer();
      string empty = string.Empty;
      try
      {
        string strSQL = "SELECT CODLOC, PROLOC, CODLIV, DATAPPINI, DATAPPFIN, IMPVOCRET FROM " + "LOCRET WHERE CODVOCRET <> 4 " + (string.Format("AND CODLOC = {0} AND PROLOC = {1} AND CODLIV = {2} AND DATAPPINI <= '{3}' ", (object) codLoc, (object) proLoc, (object) codLiv, (object) DBMethods.Db2Date(dal)) + " AND DATAPPFIN >= '" + DBMethods.Db2Date(dal) + "' ") + " ORDER BY DATAPPINI DESC";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          {
            MinimiContrattuali minimiContrattuali = new MinimiContrattuali((Decimal) row["CODLOC"], (Decimal) row["PROLOC"], (Decimal) row["CODLIV"], row["DATAPPINI"].ToString(), row["DATAPPFIN"].ToString(), (Decimal) row["IMPVOCRET"]);
            localContractualMin.Add(minimiContrattuali);
          }
          outcome = new int?(0);
          return localContractualMin;
        }
        outcome = new int?(2);
        return (List<MinimiContrattuali>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) localContractualMin));
        outcome = new int?(1);
        return (List<MinimiContrattuali>) null;
      }
    }

    public static Decimal GetAliquota(
      int codQuaCon,
      int codGruAss,
      string data,
      char char65,
      string isFap,
      ref Decimal perFap,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      Decimal aliquota = 0M;
      string str = char65 == 'S' ? " AND CATFORASS <> 'PREV' " : string.Empty;
      try
      {
        string strSQL = "SELECT DISTINCT A.CODGRUASS, A.CODFORASS, A.ALIQUOTA, A.DATINI, VALUE(A.DATFIN, '9999-12-31') " + "AS DATFIN, B.CATFORASS, B.DENFORASS, A.CODQUACON FROM ALIFORASS A INNER JOIN FORASS B ON " + string.Format("A.CODFORASS = B.CODFORASS WHERE B.CATFORASS <> 'FAP' AND CODGRUASS = {0} AND CODQUACON = {1} ", (object) codGruAss, (object) codQuaCon) + "AND '" + DBMethods.Db2Date(data) + "' >= DATINI AND '" + DBMethods.Db2Date(data) + "' <= DATFIN " + str + " ORDER BY DATINI DESC";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            aliquota += Convert.ToDecimal(row["ALIQUOTA"]);
        }
        if (isFap == "S")
        {
          Decimal fap = DenunciaMensileDAL.GetFap(data, out outcome);
          int? nullable = outcome;
          int num = 0;
          if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
            return 0M;
          outcome = new int?();
          aliquota += fap;
          perFap = 0.0M;
        }
        else
        {
          Decimal fap = DenunciaMensileDAL.GetFap(data, out outcome);
          int? nullable = outcome;
          int num = 0;
          if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
            return 0M;
          perFap = fap;
        }
        outcome = new int?(0);
        return aliquota;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0M;
      }
    }

    public static Decimal GetFap(string data, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT DISTINCT CODFAP, DATINI, VALUE(DATFIN, '9999-12-31') AS DATFIN, VALFAP FROM CODFAP " + " WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(data)) + " >= DATINI AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(data)) + " <= DATFIN ORDER BY DATINI DESC ";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          outcome = new int?(0);
          return Convert.ToDecimal(dataTable.Rows[0]["VALFAP"]);
        }
        outcome = new int?(2);
        return 0M;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0M;
      }
    }

    public static List<ParametriGenerali> GetParametriGenerali(
      string codPos,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      List<ParametriGenerali> parametriGenerali = new List<ParametriGenerali>();
      try
      {
        string strSQL1 = $"SELECT COUNT(CODPOS) FROM PARGENPOS WHERE CODPOS = {codPos}";
        string strSQL2 = Convert.ToInt32("0" + dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text)) != 0 
            ? $"SELECT CODPAR, VALORE, DATINI, DATFIN FROM PARGENDET WHERE CODPAR IN (1, 4) UNION SELECT 5 AS CODPAR, CHAR(VALORE) AS VALORE, DATINI, DATFIN FROM PARGENPOS WHERE CODPOS = {codPos} ORDER BY DATINI DESC" 
            : "SELECT CODPAR, VALORE, DATINI, DATFIN FROM PARGENDET WHERE CODPAR IN (1, 4, 5) ORDER BY DATINI DESC";
        DataTable dataTable = dataLayer.GetDataTable(strSQL2);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            parametriGenerali.Add(new ParametriGenerali
            {
              CodPar = !DBNull.Value.Equals(row["CODPAR"]) ? Convert.ToDecimal(row["CODPAR"]) : 0M,
              Valore = !DBNull.Value.Equals(row["VALORE"]) ? row["VALORE"].ToString() : string.Empty,
              DataInizio = !DBNull.Value.Equals(row["DATINI"]) ? row["DATINI"].ToString() : string.Empty,
              DataFine = !DBNull.Value.Equals(row["DATFIN"]) ? row["DATFIN"].ToString() : string.Empty
            });
          outcome = 0;
          return parametriGenerali;
        }
        outcome = 2;
        return null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((HttpContext.Current.Session["utente"] as OCM.Utente.Utente)), JsonConvert.SerializeObject(parametriGenerali));
        outcome = 1;
        return null;
      }
    }

    public static List<AnagraficaLavorativa> GetPersonalAndWorkData(
      string annfia,
      string data_dal,
      string data_al,
      string codPos,
      out int? outcome)
    {
      List<AnagraficaLavorativa> personalAndWorkData = new List<AnagraficaLavorativa>();
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT TRIM(A.COG) || ' ' || TRIM(A.NOM) AS NOME, A.CODFIS, A.DATNAS, B.DATDEC, B.DATCES, " + "(SELECT COUNT(*) FROM RAPLAV WHERE CURRENT_DATE between DATDEC and VALUE(DATCES, '9999-12-31') and mat = a.mat) AS NUM_RAP, " +
                        "B.CODPOS, A.MAT, C.FAP, B.PRORAP, C.TIPRAP, C.CODGRUASS, DATINI, DATFIN, VALUE(C.CODLOC, 0) " + "AS CODLOC, VALUE(C.CODCON, 0) AS CODCON, C.CODLIV, 0 AS CODQUACON, 0 AS PROCON, 0 AS " + "PROLOC, TRAECO, PERPAR, PERAPP, VALUE(PERFAP, 0) AS PERFAP, C.ABBPRE, " + "C.ASSCON, IMPSCAMAT" +
                        " FROM ISCT A INNER JOIN RAPLAV B ON A.MAT = B.MAT INNER JOIN STORDL " + "C ON B.CODPOS = C.CODPOS AND B.MAT = C.MAT AND B.PRORAP = C.PRORAP " +
                        "WHERE B.CODPOS = " + codPos + " AND '" + DBMethods.Db2Date(data_dal) + "' <= VALUE(B.DATCES , '9999-12-31') " + "AND '" + DBMethods.Db2Date(data_al) + "' >= B.DATDEC " + "AND '" + DBMethods.Db2Date(data_dal) + "' <= VALUE(C.DATFIN , '9999-12-31') " + "AND '" + DBMethods.Db2Date(data_al) + "' >= C.DATINI AND VALUE(B.CODCAUCES, 0) <> 50 " +
                        "ORDER BY NOME, A.MAT, C.DATINI, C.DATFIN";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            personalAndWorkData.Add(new AnagraficaLavorativa()
            {
              Nome = !DBNull.Value.Equals(row["NOME"]) ? (string) row["NOME"] : string.Empty,
              CodFiscale = !DBNull.Value.Equals(row["CODFIS"]) ? (string) row["CODFIS"] : string.Empty,
              DataNas = !DBNull.Value.Equals(row["DATNAS"]) ? row["DATNAS"].ToString() : string.Empty,
              DataDec = !DBNull.Value.Equals(row["DATDEC"]) ? row["DATDEC"].ToString() : string.Empty,
              DataCes = !DBNull.Value.Equals(row["DATCES"]) ? row["DATCES"].ToString() : string.Empty,
              NumRap = !DBNull.Value.Equals(row["NUM_RAP"]) ? (int) row["NUM_RAP"] : 0,
              CodPos = !DBNull.Value.Equals(row["CODPOS"]) ? row["CODPOS"].ToString() : string.Empty,
              Mat = !DBNull.Value.Equals(row["MAT"]) ? Convert.ToInt32(row["MAT"]) : 0,
              Fap = !DBNull.Value.Equals(row["FAP"]) ? row["FAP"].ToString() : string.Empty,
              ProRap = !DBNull.Value.Equals(row["PRORAP"]) ? Convert.ToInt32(row["PRORAP"]) : 0,
              TipRap = !DBNull.Value.Equals(row["TIPRAP"]) ? Convert.ToInt32(row["TIPRAP"]) : 0,
              CodGruAss = !DBNull.Value.Equals(row["CODGRUASS"]) ? Convert.ToInt32(row["CODGRUASS"]) : 0,
              DataInizio = !DBNull.Value.Equals(row["DATINI"]) ? row["DATINI"].ToString() : string.Empty,
              DataFine = !DBNull.Value.Equals(row["DATFIN"]) ? row["DATFIN"].ToString() : string.Empty,
              CodLoc = !DBNull.Value.Equals(row["CODLOC"]) ? Convert.ToInt32(row["CODLOC"]) : 0,
              CodCon = !DBNull.Value.Equals(row["CODCON"]) ? Convert.ToInt32(row["CODCON"]) : 0,
              CodLiv = !DBNull.Value.Equals(row["CODLIV"]) ? Convert.ToInt32(row["CODLIV"]) : 0,
              CodQuaCon = !DBNull.Value.Equals(row["CODQUACON"]) ? (int) row["CODQUACON"] : 0,
              ProCon = !DBNull.Value.Equals(row["PROCON"]) ? (int) row["PROCON"] : 0,
              ProLoc = !DBNull.Value.Equals(row["PROLOC"]) ? (int) row["PROLOC"] : 0,
              TraEco = !DBNull.Value.Equals(row["TRAECO"]) ? (Decimal) row["TRAECO"] : 0M,
              PerPar = !DBNull.Value.Equals(row["PERPAR"]) ? (Decimal) row["PERPAR"] : 0M,
              PerApp = !DBNull.Value.Equals(row["PERAPP"]) ? (Decimal) row["PERAPP"] : 0M,
              PerFap = !DBNull.Value.Equals(row["PERFAP"]) ? (Decimal) row["PERFAP"] : 0M,
              AbbPre = !DBNull.Value.Equals(row["ABBPRE"]) ? (string) row["ABBPRE"] : string.Empty,
              AssCon = !DBNull.Value.Equals(row["ASSCON"]) ? (string) row["ASSCON"] : string.Empty,
              ImpScaMat = !DBNull.Value.Equals(row["IMPSCAMAT"]) ? (Decimal) row["IMPSCAMAT"] : 0M
            });
          outcome = new int?(0);
          return personalAndWorkData;
        }
        outcome = new int?(2);
        return (List<AnagraficaLavorativa>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) personalAndWorkData));
        outcome = new int?(1);
        return (List<AnagraficaLavorativa>) null;
      }
    }

    public static List<SospensioneRapporto> GetSuspensions(
      string codPos,
      string data_dal,
      string data_al,
      int mat,
      int proRap,
      out int? outcome)
    {
      List<SospensioneRapporto> suspensions = new List<SospensioneRapporto>();
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT MAT, PRORAP, DATINISOS, DATFINSOS, PERAZI, PERFIG, CODSOS FROM SOSRAP WHERE CODPOS = " + codPos + " " + "AND '" + DBMethods.Db2Date(data_dal) + "' <= VALUE(DATFINSOS , '9999-12-31') AND '" + DBMethods.Db2Date(data_al) + "' >= DATINISOS " + string.Format("AND STASOS = '0' AND MAT = {0} AND PRORAP = {1} ORDER BY MAT, PRORAP, DATINISOS", (object) mat, (object) proRap);
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          {
            SospensioneRapporto sospensioneRapporto = new SospensioneRapporto((Decimal) row["MAT"], (Decimal) row["PRORAP"], row["DATINISOS"].ToString(), row["DATFINSOS"].ToString(), (Decimal) row["PERAZI"], (Decimal) row["PERFIG"], (Decimal) row["CODSOS"], (string) null);
            suspensions.Add(sospensioneRapporto);
          }
          outcome = new int?(0);
          return suspensions;
        }
        outcome = new int?(2);
        return (List<SospensioneRapporto>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) suspensions));
        outcome = new int?(1);
        return (List<SospensioneRapporto>) null;
      }
    }

    public static List<ImportiPrev> GetImportiPrev(
      bool isArretrato,
      ref bool? isImportoPrev,
      RetribuzioneRDL record,
      string codPos,
      out int? outcome,
      int proDen = 0)
    {
      List<ImportiPrev> importiPrev1 = new List<ImportiPrev>();
      DataLayer dataLayer = new DataLayer();
      string empty = string.Empty;
      try
      {
          string str = "SELECT * FROM DENDET WHERE CODPOS = " + codPos + " " + string.Format(" AND MAT = {0} AND PRORAP = {1} AND DAL = {2} ", (object)record.Mat, (object)record.ProRap, (object)DBMethods.DoublePeakForSql(DBMethods.Db2Date(record.Dal))) + "AND AL = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(record.Al));
          string strSQL = !isArretrato ? str + " AND TIPMOV IN ('DP', 'NU')" : str + " AND TIPMOV = 'AR'";
          if(proDen != 0)
            strSQL += $" AND PRODEN = {proDen}";
          strSQL += " ORDER BY PRODENDET DESC LIMIT 1";

          DataTable dataTable = dataLayer.GetDataTable(strSQL);
          if (dataTable.Rows.Count > 0)
          {
              isImportoPrev = new bool?(true);
              foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
              {
                  int.TryParse(row["PRODENDET"].ToString(), out int proDenDet);
                  record.ProDenDet = proDenDet;
                  var impret = (decimal)row["IMPRET"];
                  var impocc = (decimal)row["IMPOCC"];
                  var impfig = (decimal)row["IMPFIG"];
                  var impcon = (decimal)row["IMPCON"];
                  var impretpre = string.IsNullOrWhiteSpace(row["IMPRETPRE"].ToString()) ? 0.0M : (decimal)row["IMPRETPRE"];
                  var impoccpre = string.IsNullOrWhiteSpace(row["IMPOCCPRE"].ToString()) ? 0.0M : (decimal)row["IMPOCCPRE"];
                  var impfigpre = string.IsNullOrWhiteSpace(row["IMPFIGPRE"].ToString()) ? 0.0M : (decimal)row["IMPFIGPRE"];
                  var impconpre = string.IsNullOrWhiteSpace(row["IMPCONPRE"].ToString()) ? 0.0M : (decimal)row["IMPCONPRE"];
                  ImportiPrev importiPrev2 = new ImportiPrev((int)0, (int)0, (Decimal)impret, (Decimal)impocc, (Decimal)impfig, (Decimal)impcon,
                      impretpre, impoccpre, impfigpre, impconpre, (string)row["TIPMOV"]);
                  importiPrev1.Add(importiPrev2);
              }
          outcome = new int?(0);
          return importiPrev1;
        }
        outcome = new int?(2);
        return (List<ImportiPrev>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) importiPrev1));
        outcome = new int?(1);
        return (List<ImportiPrev>) null;
      }
    }

    public static List<ImportiPrev> GetImportiPrev(
      RetribuzioneRDL record,
      string codPos,
      out int? outcome)
    {
      List<ImportiPrev> importiPrev1 = new List<ImportiPrev>();
      DataLayer dataLayer = new DataLayer();
      string empty = string.Empty;
      try
      {
        string strSQL = "SELECT MODPRE.PROMOD, CODSTAPRE, IMPRET, IMPOCC, IMPFIG, IMPCON, IMPRETPRV, IMPOCCPRV, IMPFIGPRV, IMPCONPRV, " + "TIPMOV FROM MODPRE INNER JOIN MODPREDET ON MODPRE.CODPOS = MODPREDET.CODPOS AND MODPRE.MAT = MODPREDET.MAT " + "AND MODPRE.PRORAP = MODPREDET.PRORAP AND MODPRE.PROMOD = MODPREDET.PROMOD WHERE MODPREDET.CODPOS = " + codPos + " " + string.Format(" AND MODPREDET.MAT = {0} AND MODPREDET.PRORAP = {1} AND MODPREDET.DAL = {2} ", (object) record.Mat, (object) record.ProRap, (object) DBMethods.DoublePeakForSql(DBMethods.Db2Date(record.Dal))) + " AND MODPRE.DATANN IS NULL";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          {
            ImportiPrev importiPrev2 = new ImportiPrev((int) row["PROMOD"], (int) row["CODSTAPRE"], (Decimal) row["IMPRET"], (Decimal) row["IMPOCC"], (Decimal) row["IMPFIG"], (Decimal) row["IMPCON"], (Decimal) row["IMPRETPRV"], (Decimal) row["IMPOCCPRV"], (Decimal) row["IMPFIGPRV"], (Decimal) row["IMPCONPRV"], (string) row["TIPMOV"]);
            importiPrev1.Add(importiPrev2);
          }
          outcome = new int?(0);
          return importiPrev1;
        }
        outcome = new int?(2);
        return (List<ImportiPrev>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) importiPrev1));
        outcome = new int?(1);
        return (List<ImportiPrev>) null;
      }
    }

    public static int TheRowsMatches(
      RetribuzioneRDL record,
      string strCodPos,
      int anno_dataDal,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      string empty = string.Empty;
      int num1 = 0;
      try
      {
        string strSQL = empty + "SELECT COUNT(*) FROM DENDET A INNER JOIN MODPREDET B ON A.CODPOS = B.CODPOS AND A.MAT = B.MAT AND A.PRORAP = B.PRORAP " + "AND A.TIPMOV = B.TIPMOV AND A.PRODEN = B.PRODEN AND A.PRODENDET = B.PRODENDET WHERE A.CODPOS = " + strCodPos + " AND " + string.Format("A.MAT = {0} AND A.PRORAP = {1} AND A.TIPMOV = 'AR' AND A.ANNCOM = {2} AND VALUE(A.NUMMOV, '') = ''", (object) record.Mat, (object) record.ProRap, (object) anno_dataDal);
        int num2 = num1 + Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text));
        outcome = new int?(0);
        return num2;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0;
      }
    }

    public static List<FigurativaInfortuni> GetFigurativaInfortuni(
      string strCodPos,
      int anno_dataDal,
      int mese_dataDal,
      int mat,
      int proRap,
      out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      List<FigurativaInfortuni> figurativaInfortuni1 = (List<FigurativaInfortuni>) null;
      string empty = string.Empty;
      try
      {
        string strSQL = empty + "SELECT CODPOS, MAT, PRORAP, IMPFIG FROM PRAINFRDLDET WHERE CODPOS = " + strCodPos + " " + string.Format(" AND ANNO = {0} AND MESE = {1} AND MAT = {2} AND PRORAP = {3}", (object) anno_dataDal, (object) mese_dataDal, (object) mat, (object) proRap);
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          {
            FigurativaInfortuni figurativaInfortuni2 = new FigurativaInfortuni((int) row["CODPOS"], (int) row["MAT"], (int) row["PRORAP"], (Decimal) row["IMPFIG"]);
            figurativaInfortuni1.Add(figurativaInfortuni2);
          }
          outcome = new int?(0);
          return figurativaInfortuni1;
        }
        outcome = new int?(2);
        return (List<FigurativaInfortuni>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) figurativaInfortuni1));
        outcome = new int?(1);
        return (List<FigurativaInfortuni>) null;
      }
    }

    public static List<string> GetParGen(int annoSelezionato, int mese, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      List<string> parGen = new List<string>();
      string empty = string.Empty;
      try
      {
        string strSQL = empty + "SELECT B.DATINI, VALUE(VALORE, '0') AS VALORE FROM PARGEN A INNER JOIN PARGENDET B ON A.CODPAR = B.CODPAR " + string.Format("WHERE B.CODPAR = 3 AND YEAR(B.DATINI) = {0} AND DATINI = '{1}' ORDER BY B.DATINI", (object) annoSelezionato, (object) DBMethods.Db2Date(string.Format("01/{0}/{1}", (object) mese, (object) annoSelezionato)));
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            parGen.Add((string) row["VALORE"]);
          outcome = new int?(0);
          return parGen;
        }
        outcome = new int?(2);
        return (List<string>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) parGen));
        outcome = new int?(1);
        return (List<string>) null;
      }
    }

    public static List<DenunciaMensileSalvata> GetSavedReport(
      int annoSelezionato,
      int codPos,
      int mese,
      out int? outcome,
      bool flag)
    {
      DataLayer dataLayer = new DataLayer();
      List<DenunciaMensileSalvata> savedReport = new List<DenunciaMensileSalvata>();
      string empty = string.Empty;
      string str = flag ? string.Format("AND MESDEN = {0}", (object) mese) : string.Empty;
      try
      {
        string strSQL = empty + "SELECT A.*, (VALUE(IMPCONDEL, 0) + VALUE(IMPADDRECDEL, 0)) AS RETTIF, CASE TIPMOV WHEN 'DP' THEN 0 WHEN 'NU' THEN 1 END AS ORDINE" + string.Format(" FROM DENTES A WHERE CODPOS = {0} AND ANNDEN = {1} {2} AND ((TIPMOV ='NU' AND DATCONMOV IS NOT NULL) ", (object) codPos, (object) annoSelezionato, (object) str) + "OR (TIPMOV = 'DP')) AND NUMMOVANN IS NULL ORDER BY MESDEN, ORDINE";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            savedReport.Add(new DenunciaMensileSalvata()
            {
              TipMov = DBNull.Value.Equals(row["TIPMOV"]) ? string.Empty : row["TIPMOV"].ToString(),
              DatSca = DBNull.Value.Equals(row["DATSCA"]) ? string.Empty : row["DATSCA"].ToString(),
              StaDen = DBNull.Value.Equals(row["STADEN"]) ? string.Empty : row["STADEN"].ToString(),
              ImpCon = DBNull.Value.Equals(row["IMPCON"]) ? string.Empty : row["IMPCON"].ToString(),
              ImpSanRet = DBNull.Value.Equals(row["IMPSANRET"]) ? string.Empty : row["IMPSANRET"].ToString(),
              UteChi = DBNull.Value.Equals(row["UTECHI"]) ? string.Empty : row["UTECHI"].ToString(),
              CodModPag = DBNull.Value.Equals(row["CODMODPAG"]) ? 0M : Convert.ToDecimal(row["CODMODPAG"]),
              ProDen = DBNull.Value.Equals(row["PRODEN"]) ? 0M : Convert.ToDecimal(row["PRODEN"]),
              //IdDipa = DBNull.Value.Equals(row["IDDIPA"]) ? 0 : Convert.ToInt32(row["IDDIPA"]),
              DatChi = DBNull.Value.Equals(row["DATCHI"]) ? string.Empty : row["DATCHI"].ToString(),
              ImpAddRec = DBNull.Value.Equals(row["IMPADDREC"]) ? 0M : Convert.ToDecimal(row["IMPADDREC"]),
              ImpAbb = DBNull.Value.Equals(row["IMPABB"]) ? 0M : Convert.ToDecimal(row["IMPABB"]),
              ImpAssCon = DBNull.Value.Equals(row["IMPASSCON"]) ? 0M : Convert.ToDecimal(row["IMPASSCON"]),
              EsiRet = DBNull.Value.Equals(row["ESIRET"]) ? string.Empty : row["ESIRET"].ToString(),
              Rettif = DBNull.Value.Equals(row["RETTIF"]) ? 0M : Convert.ToDecimal(row["RETTIF"]),
              ImpSanDet = DBNull.Value.Equals(row["IMPSANDET"]) ? 0M : Convert.ToDecimal(row["IMPSANDET"]),
              DatSanAnn = DBNull.Value.Equals(row["DATSANANN"]) ? (string) null : row["DATSANANN"].ToString(),
              SanSotSog = DBNull.Value.Equals(row["SANSOTSOG"]) ? string.Empty : row["SANSOTSOG"].ToString()
            });
          outcome = new int?(0);
          return savedReport;
        }
        outcome = new int?(2);
        return (List<DenunciaMensileSalvata>) null;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), JsonConvert.SerializeObject((object) savedReport));
        outcome = new int?(1);
        return (List<DenunciaMensileSalvata>) null;
      }
    }

    public static int GetTOTRapLav(int codPos, string dataInizio, string data, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      string empty = string.Empty;
      try
      {
        string strSQL = empty + string.Format("SELECT COUNT(MAT) AS TOT FROM RAPLAV WHERE CODPOS = {0} AND '{1}' <= ", (object) codPos, (object) dataInizio) + "VALUE(DATCES, '2100-01-01') AND '" + data + "' >= DATDEC AND VALUE(CODCAUCES, 0) <> 50";
        if (dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text) != null)
        {
          outcome = new int?(0);
          return Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text));
        }
        outcome = new int?(2);
        return 0;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0;
      }
    }

    public static int GetDenunceInSospeso(int codPos, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      string empty = string.Empty;
      try
      {
        string strSQL = empty + string.Format("SELECT COUNT(*) FROM DENTES WHERE CODPOS = {0} AND TIPMOV = 'DP' AND STADEN = 'N'", (object) codPos);
        if (dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text) != null)
        {
          outcome = new int?(0);
          return Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text));
        }
        outcome = new int?(2);
        return 0;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return 0;
      }
    }

    public static bool Aggiorna_Contabilita_Dettaglio(
      DataLayer dataLayer,
      string codPos,
      int anno,
      int mese,
      int proDen)
    {
      try
      {
        string strSQL1 = "SELECT COUNT(*) FROM DENDET " +
                         $"WHERE CODPOS = {codPos} " +
                         $"AND ANNDEN = {anno} " +
                         $"AND MESDEN = {mese} " +
                         $"AND PRODEN = {proDen} " +
                         $"AND (NUMMOV IS NULL OR DATCONMOV IS NULL) " +
                         $"AND TIPMOV = 'DP'";

        if (Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text)) > 0)
        {
          string strSQL2 = "SELECT NUMMOV, DATCONMOV FROM DENTES " +
                           $"WHERE CODPOS = {codPos} " +
                           $"AND ANNDEN = {anno} " +
                           $"AND MESDEN = {mese} " +
                           $"AND PRODEN = {proDen}";

          DataTable dataTable1 = dataLayer.GetDataTable(strSQL2);

          if (dataTable1.Rows.Count > 0 && !DBNull.Value.Equals(dataTable1.Rows[0]["NUMMOV"]))
          {
            string numMov = dataTable1.Rows[0]["NUMMOV"].ToString();
            string datConMov = dataTable1.Rows[0]["DATCONMOV"].ToString();

            string strSQL3 = "SELECT PARTITA, PROGMOV FROM MOVIMSAP " +
                             $"WHERE CODCAUS = {DBMethods.DoublePeakForSql(numMov.Substring(0, 2))} " +
                             $"AND ANNORIF = {numMov.Substring(3, 4)} " +
                             $"AND NUMERORIF = {numMov.Substring(8)} " +
                             $"AND CODPOSIZ = {codPos}";
            DataTable dataTable2 = dataLayer.GetDataTable(strSQL3);

            if (dataTable2.Rows.Count > 0)
            {
              string partita = dataTable2.Rows[0]["PARTITA"].ToString();
              string progMov = dataTable2.Rows[0]["PROGMOV"].ToString();

              string strSQL4 = "UPDATE DENDET SET " +
                               $"NUMMOV = {DBMethods.DoublePeakForSql(numMov)}, " +
                               $"DATCONMOV = {DBMethods.DoublePeakForSql(DBMethods.Db2Date(datConMov))}, " +
                               $"PARTITA = {DBMethods.DoublePeakForSql(partita)}, " +
                               $"PROGMOV = {progMov} " +
                               $"WHERE CODPOS = {codPos} " +
                               $"AND ANNDEN = {anno} " +
                               $"AND MESDEN = {mese} " +
                               $"AND PRODEN = {proDen} " +
                               $"AND TIPMOV = 'DP'";
              dataLayer.WriteTransactionData(strSQL4, CommandType.Text);
            }
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static List<RetribuzioneRDL> PreparaDenunciaMensile(
      TFI.OCM.Utente.Utente utente,
      int proDen,
      string data_dal,
      string data_al,
      string codPos,
      string mat = "",
      bool isArretrato = false,
      int annFia = 0,
      int mesFia = 0,
      int idDipa = 0)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      int IDPOLIZZA = 0;
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string empty3 = string.Empty;
      string tipSpe = string.Empty;
      string empty4 = string.Empty;
      string qualifica = string.Empty;
      string livello = string.Empty;
      Decimal impSca = 0.0M;
      Decimal impTraEco = 0.0M;
      Decimal perFap = 0.0M;
      Decimal proLoc = 0.0M;
      Decimal proCon = 0.0M;
      string str1 = "N";
      string empty5 = string.Empty;
      bool? isImportoPrev = new bool?();
      bool flag = false;
      DataLayer db = new DataLayer();
      List<RetribuzioneRDL> listaReport = (List<RetribuzioneRDL>) null;
      try
      {
        int? outcome;
        string payments = DenunciaMensileDAL.GetPayments(codPos, out outcome);
        int? nullable1 = outcome;
        int num5 = 2;
        if (!(nullable1.GetValueOrDefault() == num5 & nullable1.HasValue))
        {
          outcome = new int?();
          listaReport = new List<RetribuzioneRDL>();
          DateTime dateTime1 = DateTime.Parse(data_dal);
          int month1 = dateTime1.Month;
          dateTime1 = DateTime.Parse(data_al);
          int month2 = dateTime1.Month;
          if (month1 == month2)
          {
            dateTime1 = DateTime.Parse(data_dal);
            int year = dateTime1.Year;
            dateTime1 = DateTime.Parse(data_dal);
            int month3 = dateTime1.Month;
            num1 = DateTime.DaysInMonth(year, month3);
          }
          List<AnagraficaLavorativa> personalAndWorkData = DenunciaMensileDAL.GetPersonalAndWorkData(annFia.ToString(), data_dal, data_al, codPos, out outcome);
          int? nullable2 = outcome;
          int num6 = 2;
          if (nullable2.GetValueOrDefault() == num6 & nullable2.HasValue)
          {
            DenunciaMensileDAL.ErrorMessage = "La funzione GetPersonalAndWorkData() non ha restituito risultati";
            return (List<RetribuzioneRDL>) null;
          }
          db.StartTransaction();
          flag = true;
          outcome = new int?();
          foreach (AnagraficaLavorativa anagraficaLavorativa in personalAndWorkData)
          {
            string codFiscale = anagraficaLavorativa.CodFiscale;
            string tipFondo = anagraficaLavorativa.CodCon == 121 || anagraficaLavorativa.CodCon == 1 || anagraficaLavorativa.CodCon == 16 ? (string.IsNullOrEmpty(anagraficaLavorativa.TipFondo) ? string.Empty : "N") : "N";
            if (!string.IsNullOrEmpty(anagraficaLavorativa.DataAnnFia))
              tipFondo = anagraficaLavorativa.AdeVal == 0 ? "N" : string.Empty;
            if (anagraficaLavorativa.IdIsc != 0)
              num3 = anagraficaLavorativa.IdIsc;
            if (anagraficaLavorativa.IdAde != 0)
              num2 = anagraficaLavorativa.IdAde;
            codPos = anagraficaLavorativa.CodPos;
            int mat1 = anagraficaLavorativa.Mat;
            int proRap = anagraficaLavorativa.ProRap;
            string nome = anagraficaLavorativa.Nome;
            DenunciaMensileDAL.listaParametriGenerali = DenunciaMensileDAL.GetParametriGenerali(codPos, out outcome);
            outcome = new int?();
            if (!string.IsNullOrEmpty(anagraficaLavorativa.DataNas))
            {
              string dataNas = anagraficaLavorativa.DataNas;
              dateTime1 = Convert.ToDateTime(dataNas);
              dateTime1 = dateTime1.AddYears(65);
              string s1 = dateTime1.ToString();
              if (!string.IsNullOrEmpty(anagraficaLavorativa.DataDec))
              {
                string dataDec = anagraficaLavorativa.DataDec;
                if (!string.IsNullOrEmpty(anagraficaLavorativa.DataCes))
                {
                  dateTime1 = Convert.ToDateTime(anagraficaLavorativa.DataCes);
                  empty4 = dateTime1.ToString("d");
                  dateTime1 = Convert.ToDateTime(empty4);
                  int year = dateTime1.Year;
                  dateTime1 = Convert.ToDateTime(data_dal);
                  int num7 = dateTime1.Year - 1;
                  if (year >= num7)
                    str1 = "S";
                }
                string str2;
                if (DateTime.Compare(Convert.ToDateTime(data_dal), Convert.ToDateTime(anagraficaLavorativa.DataInizio)) >= 0)
                {
                  dateTime1 = Convert.ToDateTime(data_dal);
                  str2 = dateTime1.ToString("d");
                }
                else
                {
                  dateTime1 = Convert.ToDateTime(anagraficaLavorativa.DataInizio);
                  str2 = dateTime1.ToString("d");
                }
                string s2;
                if (DateTime.Compare(DateTime.Parse(data_al), DateTime.Parse(anagraficaLavorativa.DataFine)) <= 0)
                {
                  dateTime1 = Convert.ToDateTime(data_al);
                  s2 = dateTime1.ToString("d");
                  if (!string.IsNullOrEmpty(empty4) && DateTime.Compare(DateTime.Parse(empty4), DateTime.Parse(data_al)) <= 0)
                    s2 = empty4;
                }
                else
                {
                  dateTime1 = Convert.ToDateTime(anagraficaLavorativa.DataFine);
                  s2 = dateTime1.ToString("d");
                }
                int int32_1 = Convert.ToInt32(anagraficaLavorativa.TipRap);
                int int32_2 = Convert.ToInt32(anagraficaLavorativa.CodCon);
                int int32_3 = Convert.ToInt32(anagraficaLavorativa.CodLoc);
                int int32_4 = Convert.ToInt32(anagraficaLavorativa.CodLiv);
                int int32_5 = Convert.ToInt32(anagraficaLavorativa.CodGruAss);
                Decimal int32_6 = (Decimal) Convert.ToInt32(anagraficaLavorativa.PerPar);
                Decimal int32_7 = (Decimal) Convert.ToInt32(anagraficaLavorativa.PerApp);
                string fap = anagraficaLavorativa.Fap;
                string abbPre = anagraficaLavorativa.AbbPre;
                string assCon = anagraficaLavorativa.AssCon;
                Decimal d = 0M;
                List<ContrattoDiRiferimento> referralContracts = DenunciaMensileDAL.GetReferralContracts(int32_2, int32_4, str2, out outcome);
                int? nullable3 = outcome;
                int num8 = 2;
                Decimal num9;
                if (!(nullable3.GetValueOrDefault() == num8 & nullable3.HasValue))
                {
                  outcome = new int?();
                  qualifica = referralContracts[0].DenQua;
                  num4 = referralContracts[0].CodQuaCon;
                  proCon = referralContracts[0].Progressivo;
                  livello = referralContracts[0].DenLiv;
                  tipSpe = referralContracts[0].TipSpe;
                  if (referralContracts[0].TipSpe == "S")
                  {
                    List<MinimiContrattuali> referenceContractualMin = DenunciaMensileDAL.GetReferenceContractualMin(int32_2, referralContracts[0].Progressivo, int32_4, str2, out outcome);
                    nullable3 = outcome;
                    num8 = 2;
                    if (!(nullable3.GetValueOrDefault() == num8 & nullable3.HasValue))
                    {
                      outcome = new int?();
                      foreach (MinimiContrattuali minimiContrattuali in referenceContractualMin)
                        d += minimiContrattuali.ImpVocRet;
                      impSca = Convert.ToDecimal("0" + anagraficaLavorativa.ImpScaMat.ToString());
                    }
                  }
                  else
                  {
                    num9 = anagraficaLavorativa.TraEco;
                    impTraEco = Convert.ToDecimal("0" + num9.ToString());
                  }
                }
                else
                {
                  DenunciaMensileDAL.log.Info((object) string.Format("[TFI.BLL] : Preparazione denuncia mensile - Il contratto di riferimento dell'utente: {0} risulta mancante sul DB in data: {1}", (object) anagraficaLavorativa.Nome, (object) DateTime.Now));
                  outcome = new int?();
                }
                if (int32_3 != 0)
                {
                  List<Contratto> localContracts = DenunciaMensileDAL.GetLocalContracts(int32_3, str2, out outcome);
                  nullable3 = outcome;
                  num8 = 2;
                  if (!(nullable3.GetValueOrDefault() == num8 & nullable3.HasValue))
                  {
                    outcome = new int?();
                    proLoc = localContracts[0].Progressivo;
                    if (localContracts[0].TipSpe == "S")
                    {
                      List<MinimiContrattuali> localContractualMin = DenunciaMensileDAL.GetLocalContractualMin(int32_3, proLoc, int32_4, str2, out outcome);
                      nullable3 = outcome;
                      num8 = 2;
                      if (!(nullable3.GetValueOrDefault() == num8 & nullable3.HasValue))
                      {
                        outcome = new int?();
                        foreach (MinimiContrattuali minimiContrattuali in localContractualMin)
                          d += minimiContrattuali.ImpVocRet;
                      }
                    }
                  }
                }
                else
                  proLoc = 0M;
                num9 = anagraficaLavorativa.PerApp;
                if (Convert.ToDecimal("0" + num9.ToString()) > 0M)
                  d = d / 100M * anagraficaLavorativa.PerApp;
                num9 = anagraficaLavorativa.PerPar;
                if (Convert.ToDecimal("0" + num9.ToString()) > 0M)
                  d = d / 100M * anagraficaLavorativa.PerPar;
                Decimal impMin = Decimal.Round(d, 2);
                RetribuzioneRDL report = new RetribuzioneRDL();
                DenunciaMensileDAL.FillBasicFields(report, int32_1, int32_2, proCon, int32_3, proLoc, int32_4, livello, qualifica, num4, int32_6, int32_7, impMin, impTraEco, impSca, fap, assCon, abbPre, tipSpe, int32_5, tipFondo, mat1, proRap, nome, dataNas, dataDec, empty4);
                Decimal importoParametro1 = DenunciaMensileDAL.GetImportoParametro(1, str2, report.AbbPre);
                Decimal importoParametro2 = DenunciaMensileDAL.GetImportoParametro(4, str2, report.AssCon);
                if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(str2)) > 0 & DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(s2)) < 0)
                {
                  RetribuzioneRDL retribuzioneRdl1 = report;
                  retribuzioneRdl1.Dal = str2;
                  RetribuzioneRDL retribuzioneRdl2 = retribuzioneRdl1;
                  dateTime1 = Convert.ToDateTime(s1);
                  dateTime1 = dateTime1.AddDays(-1.0);
                  string str3 = dateTime1.ToString("d");
                  retribuzioneRdl2.Al = str3;
                  retribuzioneRdl1.Eta65 = 'N';
                  retribuzioneRdl1.ImpAbb = importoParametro1;
                  retribuzioneRdl1.ImpAssCon = importoParametro2;
                  Decimal aliquota1 = DenunciaMensileDAL.GetAliquota(num4, int32_5, str2, retribuzioneRdl1.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                  nullable3 = outcome;
                  num8 = 0;
                  if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                  {
                    outcome = new int?();
                    retribuzioneRdl1.Aliquota = aliquota1;
                  }
                  else
                  {
                    if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                      return (List<RetribuzioneRDL>) null;
                    outcome = new int?();
                  }
                  retribuzioneRdl1.PerFap = perFap;
                  retribuzioneRdl1.Prev = str1;
                  if (retribuzioneRdl1.TipFondo != "N")
                  {
                    dateTime1 = Convert.ToDateTime(str2);
                    if (dateTime1.Year >= 2016)
                    {
                      int[] source1 = new int[7]
                      {
                        1,
                        2,
                        4,
                        7,
                        10,
                        12,
                        13
                      };
                      int[] source2 = new int[2]{ 3, 11 };
                      if (((IEnumerable<int>) source1).Contains<int>(retribuzioneRdl1.TipRap))
                      {
                        RetribuzioneRDL retribuzioneRdl3 = retribuzioneRdl1;
                        string str4;
                        if (Convert.ToDouble(payments) != 99.0)
                        {
                          num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                          str4 = num9.ToString();
                        }
                        else
                          str4 = "0,00";
                        retribuzioneRdl3.ImpSanit = str4;
                        if (retribuzioneRdl1.ImpSanit == "-1")
                          throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                        if (retribuzioneRdl1.ImpSanit == "0")
                        {
                          retribuzioneRdl1.ImpSanit = "0,00";
                          retribuzioneRdl1.Sanitario = "N";
                        }
                      }
                      else if (((IEnumerable<int>) source2).Contains<int>(retribuzioneRdl1.TipRap))
                      {
                        num8 = anagraficaLavorativa.NumRap;
                        if (num8.ToString().Trim() == "1")
                        {
                          RetribuzioneRDL retribuzioneRdl4 = retribuzioneRdl1;
                          string str5;
                          if (Convert.ToDouble(payments) != 99.0)
                          {
                            num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                            str5 = num9.ToString();
                          }
                          else
                            str5 = "0,00";
                          retribuzioneRdl4.ImpSanit = str5;
                          if (retribuzioneRdl1.ImpSanit == "-1")
                            throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                          if (retribuzioneRdl1.ImpSanit == "0")
                          {
                            retribuzioneRdl1.ImpSanit = "0,00";
                            retribuzioneRdl1.Sanitario = "N";
                          }
                        }
                        else
                        {
                          retribuzioneRdl1.ImpSanit = "0,00";
                          retribuzioneRdl1.IdPolizza = 0;
                          retribuzioneRdl1.Sanitario = "N";
                        }
                      }
                      else
                      {
                        retribuzioneRdl1.ImpSanit = "0,00";
                        retribuzioneRdl1.IdPolizza = 0;
                        retribuzioneRdl1.Sanitario = "N";
                      }
                    }
                    else if (retribuzioneRdl1.TipRap != 1)
                    {
                      retribuzioneRdl1.ImpSanit = "0,00";
                      retribuzioneRdl1.IdPolizza = 0;
                      retribuzioneRdl1.Sanitario = "N";
                    }
                    else
                    {
                      RetribuzioneRDL retribuzioneRdl5 = retribuzioneRdl1;
                      string str6;
                      if (Convert.ToDouble(payments) != 99.0)
                      {
                        num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                        str6 = num9.ToString();
                      }
                      else
                        str6 = "0,00";
                      retribuzioneRdl5.ImpSanit = str6;
                      if (retribuzioneRdl1.ImpSanit == "-1")
                        throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                      if (retribuzioneRdl1.ImpSanit == "0")
                      {
                        retribuzioneRdl1.ImpSanit = "0,00";
                        retribuzioneRdl1.Sanitario = "N";
                      }
                    }
                  }
                  else
                  {
                    retribuzioneRdl1.ImpSanit = "0,00";
                    retribuzioneRdl1.IdPolizza = 0;
                    retribuzioneRdl1.Sanitario = "N";
                  }
                  listaReport.Add(retribuzioneRdl1);
                  RetribuzioneRDL retribuzioneRdl6 = report;
                  RetribuzioneRDL retribuzioneRdl7 = retribuzioneRdl6;
                  dateTime1 = Convert.ToDateTime(s1);
                  string str7 = dateTime1.ToString("d");
                  retribuzioneRdl7.Dal = str7;
                  retribuzioneRdl6.Al = s2;
                  retribuzioneRdl6.Eta65 = 'S';
                  retribuzioneRdl6.ImpAbb = importoParametro1;
                  retribuzioneRdl6.ImpAssCon = importoParametro2;
                  Decimal aliquota2 = DenunciaMensileDAL.GetAliquota(num4, int32_5, str2, retribuzioneRdl6.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                  nullable3 = outcome;
                  num8 = 0;
                  if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                  {
                    retribuzioneRdl6.Aliquota = aliquota2;
                    outcome = new int?();
                  }
                  else
                  {
                    if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                      return (List<RetribuzioneRDL>) null;
                    outcome = new int?();
                  }
                  retribuzioneRdl6.PerFap = perFap;
                  retribuzioneRdl6.ImpSanit = "0.00";
                  retribuzioneRdl6.IdPolizza = 0;
                  retribuzioneRdl6.Sanitario = "N";
                  retribuzioneRdl6.IdIsc = num3;
                  retribuzioneRdl6.IdAde = num2;
                  retribuzioneRdl6.Prev = str1;
                  listaReport.Add(retribuzioneRdl6);
                }
                else if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(str2)) == 0)
                {
                  RetribuzioneRDL retribuzioneRdl8 = report;
                  dateTime1 = Convert.ToDateTime(s1);
                  string str8 = dateTime1.ToString("d");
                  retribuzioneRdl8.Dal = str8;
                  report.Al = s2;
                  report.Eta65 = 'S';
                  report.ImpAbb = importoParametro1;
                  report.ImpAssCon = importoParametro2;
                  Decimal aliquota = DenunciaMensileDAL.GetAliquota(num4, int32_5, str2, report.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                  nullable3 = outcome;
                  num8 = 0;
                  if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                  {
                    outcome = new int?();
                    report.Aliquota = aliquota;
                  }
                  else
                  {
                    if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                      return (List<RetribuzioneRDL>) null;
                    outcome = new int?();
                  }
                  report.PerFap = perFap;
                  report.IdIsc = num3;
                  report.IdAde = num2;
                  report.Prev = str1;
                  if (report.TipFondo != "N")
                  {
                    dateTime1 = Convert.ToDateTime(str2);
                    if (dateTime1.Year >= 2016)
                    {
                      int[] source3 = new int[7]
                      {
                        1,
                        2,
                        4,
                        7,
                        10,
                        12,
                        13
                      };
                      int[] source4 = new int[2]{ 3, 11 };
                      if (((IEnumerable<int>) source3).Contains<int>(report.TipRap))
                      {
                        RetribuzioneRDL retribuzioneRdl9 = report;
                        string str9;
                        if (Convert.ToDouble(payments) != 99.0)
                        {
                          num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                          str9 = num9.ToString();
                        }
                        else
                          str9 = "0,00";
                        retribuzioneRdl9.ImpSanit = str9;
                        if (report.ImpSanit == "-1")
                          throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                        if (report.ImpSanit == "0")
                        {
                          report.ImpSanit = "0,00";
                          report.Sanitario = "N";
                        }
                        else
                        {
                          report.IdPolizza = IDPOLIZZA;
                          report.Sanitario = "S";
                        }
                      }
                      else if (((IEnumerable<int>) source4).Contains<int>(report.TipRap))
                      {
                        num8 = anagraficaLavorativa.NumRap;
                        if (num8.ToString().Trim() == "1")
                        {
                          RetribuzioneRDL retribuzioneRdl10 = report;
                          string str10;
                          if (Convert.ToDouble(payments) != 99.0)
                          {
                            num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                            str10 = num9.ToString();
                          }
                          else
                            str10 = "0,00";
                          retribuzioneRdl10.ImpSanit = str10;
                          if (report.ImpSanit == "-1")
                            throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                          if (report.ImpSanit == "0")
                          {
                            report.ImpSanit = "0,00";
                            report.Sanitario = "N";
                          }
                          else
                          {
                            report.IdPolizza = IDPOLIZZA;
                            report.Sanitario = "S";
                          }
                        }
                        else
                        {
                          report.ImpSanit = "0,00";
                          report.IdPolizza = 0;
                          report.Sanitario = "N";
                        }
                      }
                      else
                      {
                        report.ImpSanit = "0,00";
                        report.IdPolizza = 0;
                        report.Sanitario = "N";
                      }
                    }
                    else if (report.TipRap != 1)
                    {
                      report.ImpSanit = "0,00";
                      report.IdPolizza = 0;
                      report.Sanitario = "N";
                    }
                    else
                    {
                      RetribuzioneRDL retribuzioneRdl11 = report;
                      string str11;
                      if (Convert.ToDouble(payments) != 99.0)
                      {
                        num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                        str11 = num9.ToString();
                      }
                      else
                        str11 = "0,00";
                      retribuzioneRdl11.ImpSanit = str11;
                      if (report.ImpSanit == "-1")
                        throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                      if (report.ImpSanit == "0")
                      {
                        report.ImpSanit = "0,00";
                        report.Sanitario = "N";
                      }
                      else
                      {
                        report.IdPolizza = IDPOLIZZA;
                        report.Sanitario = "S";
                      }
                    }
                  }
                  else
                  {
                    report.ImpSanit = "0,00";
                    report.IdPolizza = 0;
                    report.Sanitario = "N";
                  }
                  listaReport.Add(report);
                }
                else if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(str2)) < 0)
                {
                  report.Dal = str2;
                  report.Al = s2;
                  report.Eta65 = 'S';
                  report.ImpAbb = importoParametro1;
                  report.ImpAssCon = importoParametro2;
                  Decimal aliquota = DenunciaMensileDAL.GetAliquota(num4, int32_5, str2, report.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                  nullable3 = outcome;
                  num8 = 0;
                  if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                  {
                    outcome = new int?();
                    report.Aliquota = aliquota;
                  }
                  else
                  {
                    if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                      return (List<RetribuzioneRDL>) null;
                    outcome = new int?();
                  }
                  report.PerFap = perFap;
                  report.IdIsc = num3;
                  report.IdAde = num2;
                  report.Prev = str1;
                  if (report.TipFondo != "N")
                  {
                    dateTime1 = Convert.ToDateTime(str2);
                    if (dateTime1.Year >= 2016)
                    {
                      int[] source5 = new int[7]
                      {
                        1,
                        2,
                        4,
                        7,
                        10,
                        12,
                        13
                      };
                      int[] source6 = new int[2]{ 3, 11 };
                      if (((IEnumerable<int>) source5).Contains<int>(report.TipRap))
                      {
                        RetribuzioneRDL retribuzioneRdl = report;
                        string str12;
                        if (Convert.ToDouble(payments) != 99.0)
                        {
                          num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                          str12 = num9.ToString();
                        }
                        else
                          str12 = "0,00";
                        retribuzioneRdl.ImpSanit = str12;
                        if (report.ImpSanit == "-1")
                          throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                        if (report.ImpSanit == "0")
                        {
                          report.ImpSanit = "0,00";
                          report.Sanitario = "N";
                        }
                        else
                        {
                          report.IdPolizza = IDPOLIZZA;
                          report.Sanitario = "S";
                        }
                      }
                      else if (((IEnumerable<int>) source6).Contains<int>(report.TipRap))
                      {
                        num8 = anagraficaLavorativa.NumRap;
                        if (num8.ToString().Trim() == "1")
                        {
                          RetribuzioneRDL retribuzioneRdl = report;
                          string str13;
                          if (Convert.ToDouble(payments) != 99.0)
                          {
                            num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                            str13 = num9.ToString();
                          }
                          else
                            str13 = "0,00";
                          retribuzioneRdl.ImpSanit = str13;
                          if (report.ImpSanit == "-1")
                            throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                          if (report.ImpSanit == "0")
                          {
                            report.ImpSanit = "0,00";
                            report.Sanitario = "N";
                          }
                          else
                          {
                            report.IdPolizza = IDPOLIZZA;
                            report.Sanitario = "S";
                          }
                        }
                        else
                        {
                          report.ImpSanit = "0,00";
                          report.IdPolizza = 0;
                          report.Sanitario = "N";
                        }
                      }
                      else
                      {
                        report.ImpSanit = "0,00";
                        report.IdPolizza = 0;
                        report.Sanitario = "N";
                      }
                    }
                    else if (report.TipRap != 1)
                    {
                      report.ImpSanit = "0,00";
                      report.IdPolizza = 0;
                      report.Sanitario = "N";
                    }
                    else
                    {
                      RetribuzioneRDL retribuzioneRdl = report;
                      string str14;
                      if (Convert.ToDouble(payments) != 99.0)
                      {
                        num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                        str14 = num9.ToString();
                      }
                      else
                        str14 = "0,00";
                      retribuzioneRdl.ImpSanit = str14;
                      if (report.ImpSanit == "-1")
                        throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                      if (report.ImpSanit == "0")
                      {
                        report.ImpSanit = "0,00";
                        report.Sanitario = "N";
                      }
                      else
                      {
                        report.IdPolizza = IDPOLIZZA;
                        report.Sanitario = "S";
                      }
                    }
                  }
                  else
                  {
                    report.ImpSanit = "0,00";
                    report.IdPolizza = 0;
                    report.Sanitario = "N";
                  }
                  listaReport.Add(report);
                }
                else if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(s2)) == 0)
                {
                  RetribuzioneRDL retribuzioneRdl12 = report;
                  retribuzioneRdl12.Dal = str2;
                  RetribuzioneRDL retribuzioneRdl13 = retribuzioneRdl12;
                  dateTime1 = Convert.ToDateTime(s1);
                  dateTime1 = dateTime1.AddDays(-1.0);
                  string str15 = dateTime1.ToString("d");
                  retribuzioneRdl13.Al = str15;
                  retribuzioneRdl12.Eta65 = 'N';
                  retribuzioneRdl12.ImpAbb = importoParametro1;
                  retribuzioneRdl12.ImpAssCon = importoParametro2;
                  Decimal aliquota3 = DenunciaMensileDAL.GetAliquota(num4, int32_5, str2, retribuzioneRdl12.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                  nullable3 = outcome;
                  num8 = 0;
                  if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                  {
                    outcome = new int?();
                    retribuzioneRdl12.Aliquota = aliquota3;
                  }
                  else
                  {
                    if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                      return (List<RetribuzioneRDL>) null;
                    outcome = new int?();
                  }
                  retribuzioneRdl12.PerFap = perFap;
                  if (retribuzioneRdl12.TipFondo != "N")
                  {
                    dateTime1 = Convert.ToDateTime(str2);
                    if (dateTime1.Year >= 2016)
                    {
                      int[] source7 = new int[7]
                      {
                        1,
                        2,
                        4,
                        7,
                        10,
                        12,
                        13
                      };
                      int[] source8 = new int[2]{ 3, 11 };
                      if (((IEnumerable<int>) source7).Contains<int>(retribuzioneRdl12.TipRap))
                      {
                        RetribuzioneRDL retribuzioneRdl14 = retribuzioneRdl12;
                        string str16;
                        if (Convert.ToDouble(payments) != 99.0)
                        {
                          num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                          str16 = num9.ToString();
                        }
                        else
                          str16 = "0,00";
                        retribuzioneRdl14.ImpSanit = str16;
                        if (retribuzioneRdl12.ImpSanit == "-1")
                          throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                        if (retribuzioneRdl12.ImpSanit == "0")
                        {
                          retribuzioneRdl12.ImpSanit = "0,00";
                          retribuzioneRdl12.Sanitario = "N";
                        }
                        else
                        {
                          retribuzioneRdl12.IdPolizza = IDPOLIZZA;
                          retribuzioneRdl12.Sanitario = "S";
                        }
                      }
                      else if (((IEnumerable<int>) source8).Contains<int>(retribuzioneRdl12.TipRap))
                      {
                        num8 = anagraficaLavorativa.NumRap;
                        if (num8.ToString().Trim() == "1")
                        {
                          RetribuzioneRDL retribuzioneRdl15 = retribuzioneRdl12;
                          string str17;
                          if (Convert.ToDouble(payments) != 99.0)
                          {
                            num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                            str17 = num9.ToString();
                          }
                          else
                            str17 = "0,00";
                          retribuzioneRdl15.ImpSanit = str17;
                          if (retribuzioneRdl12.ImpSanit == "-1")
                            throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                          if (retribuzioneRdl12.ImpSanit == "0")
                          {
                            retribuzioneRdl12.ImpSanit = "0,00";
                            retribuzioneRdl12.Sanitario = "N";
                          }
                          else
                          {
                            retribuzioneRdl12.IdPolizza = IDPOLIZZA;
                            retribuzioneRdl12.Sanitario = "S";
                          }
                        }
                        else
                        {
                          retribuzioneRdl12.ImpSanit = "0,00";
                          retribuzioneRdl12.IdPolizza = 0;
                          retribuzioneRdl12.Sanitario = "N";
                        }
                      }
                      else
                      {
                        retribuzioneRdl12.ImpSanit = "0,00";
                        retribuzioneRdl12.IdPolizza = 0;
                        retribuzioneRdl12.Sanitario = "N";
                      }
                    }
                    else if (retribuzioneRdl12.TipRap != 1)
                    {
                      retribuzioneRdl12.ImpSanit = "0,00";
                      retribuzioneRdl12.IdPolizza = 0;
                      retribuzioneRdl12.Sanitario = "N";
                    }
                    else
                    {
                      RetribuzioneRDL retribuzioneRdl16 = retribuzioneRdl12;
                      string str18;
                      if (Convert.ToDouble(payments) != 99.0)
                      {
                        num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                        str18 = num9.ToString();
                      }
                      else
                        str18 = "0,00";
                      retribuzioneRdl16.ImpSanit = str18;
                      if (retribuzioneRdl12.ImpSanit == "-1")
                        throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                      if (retribuzioneRdl12.ImpSanit == "0")
                      {
                        retribuzioneRdl12.ImpSanit = "0,00";
                        retribuzioneRdl12.Sanitario = "N";
                      }
                      else
                      {
                        retribuzioneRdl12.IdPolizza = IDPOLIZZA;
                        retribuzioneRdl12.Sanitario = "S";
                      }
                    }
                  }
                  else
                  {
                    retribuzioneRdl12.ImpSanit = "0,00";
                    retribuzioneRdl12.IdPolizza = 0;
                    retribuzioneRdl12.Sanitario = "N";
                  }
                  listaReport.Add(retribuzioneRdl12);
                  RetribuzioneRDL retribuzioneRdl17 = report;
                  RetribuzioneRDL retribuzioneRdl18 = retribuzioneRdl17;
                  dateTime1 = Convert.ToDateTime(s1);
                  string str19 = dateTime1.ToString("d");
                  retribuzioneRdl18.Dal = str19;
                  retribuzioneRdl17.Al = s2;
                  retribuzioneRdl17.Eta65 = 'S';
                  retribuzioneRdl17.ImpAbb = importoParametro1;
                  retribuzioneRdl17.ImpAssCon = importoParametro2;
                  Decimal aliquota4 = DenunciaMensileDAL.GetAliquota(num4, int32_5, str2, retribuzioneRdl17.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                  nullable3 = outcome;
                  num8 = 0;
                  if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                  {
                    outcome = new int?();
                    retribuzioneRdl17.Aliquota = aliquota4;
                  }
                  else
                  {
                    if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                      return (List<RetribuzioneRDL>) null;
                    outcome = new int?();
                  }
                  retribuzioneRdl17.PerFap = perFap;
                  retribuzioneRdl17.ImpSanit = "0.00";
                  retribuzioneRdl17.IdPolizza = 0;
                  retribuzioneRdl17.Sanitario = "N";
                  retribuzioneRdl17.IdIsc = num3;
                  retribuzioneRdl17.IdAde = num2;
                  retribuzioneRdl17.Prev = str1;
                  listaReport.Add(retribuzioneRdl17);
                }
                else
                {
                  report.Dal = str2;
                  report.Al = s2;
                  report.Eta65 = 'N';
                  report.ImpAbb = importoParametro1;
                  report.ImpAssCon = importoParametro2;
                  Decimal aliquota = DenunciaMensileDAL.GetAliquota(num4, int32_5, str2, report.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                  nullable3 = outcome;
                  num8 = 0;
                  if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                  {
                    outcome = new int?();
                    report.Aliquota = aliquota;
                  }
                  else
                  {
                    if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                      return (List<RetribuzioneRDL>) null;
                    outcome = new int?();
                  }
                  report.PerFap = perFap;
                  report.IdIsc = num3;
                  report.IdAde = num2;
                  report.Prev = str1;
                  if (report.TipFondo != "N")
                  {
                    dateTime1 = Convert.ToDateTime(str2);
                    if (dateTime1.Year >= 2016)
                    {
                      int[] source9 = new int[7]
                      {
                        1,
                        2,
                        4,
                        7,
                        10,
                        12,
                        13
                      };
                      int[] source10 = new int[2]{ 3, 11 };
                      if (((IEnumerable<int>) source9).Contains<int>(report.TipRap))
                      {
                        RetribuzioneRDL retribuzioneRdl = report;
                        string str20;
                        if (Convert.ToDouble(payments) != 99.0)
                        {
                          num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                          str20 = num9.ToString();
                        }
                        else
                          str20 = "0,00";
                        retribuzioneRdl.ImpSanit = str20;
                        if (report.ImpSanit == "-1")
                          throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                        if (report.ImpSanit == "0")
                        {
                          report.ImpSanit = "0,00";
                          report.Sanitario = "N";
                        }
                        else
                        {
                          report.IdPolizza = IDPOLIZZA;
                          report.Sanitario = "S";
                        }
                      }
                      else if (((IEnumerable<int>) source10).Contains<int>(report.TipRap))
                      {
                        num8 = anagraficaLavorativa.NumRap;
                        if (num8.ToString().Trim() == "1")
                        {
                          RetribuzioneRDL retribuzioneRdl = report;
                          string str21;
                          if (Convert.ToDouble(payments) != 99.0)
                          {
                            num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                            str21 = num9.ToString();
                          }
                          else
                            str21 = "0,00";
                          retribuzioneRdl.ImpSanit = str21;
                          if (report.ImpSanit == "-1")
                            throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                          if (report.ImpSanit == "0")
                          {
                            report.ImpSanit = "0,00";
                            report.Sanitario = "N";
                          }
                          else
                          {
                            report.IdPolizza = IDPOLIZZA;
                            report.Sanitario = "S";
                          }
                        }
                        else
                        {
                          report.ImpSanit = "0,00";
                          report.IdPolizza = 0;
                          report.Sanitario = "N";
                        }
                      }
                      else
                      {
                        report.ImpSanit = "0,00";
                        report.IdPolizza = 0;
                        report.Sanitario = "N";
                      }
                    }
                    else if (report.TipRap != 1)
                    {
                      report.ImpSanit = "0,00";
                      report.IdPolizza = 0;
                      report.Sanitario = "N";
                    }
                    else
                    {
                      RetribuzioneRDL retribuzioneRdl = report;
                      string str22;
                      if (Convert.ToDouble(payments) != 99.0)
                      {
                        num9 = WriteDIPA.FIA_GET_IMP_FORMULE_E_DIPA(db, utente, utente.CodPosizione, num2.ToString(), num3.ToString(), annFia, mesFia, "A", ref IDPOLIZZA, codFiscale, num4, mat1, Convert.ToInt32(payments), dataDec, true, idDipa);
                        str22 = num9.ToString();
                      }
                      else
                        str22 = "0,00";
                      retribuzioneRdl.ImpSanit = str22;
                      if (report.ImpSanit == "-1")
                        throw new Exception("Attenzione verificare importo del Fondo Sanitario");
                      if (report.ImpSanit == "0")
                      {
                        report.ImpSanit = "0,00";
                        report.Sanitario = "N";
                      }
                      else
                      {
                        report.IdPolizza = IDPOLIZZA;
                        report.Sanitario = "S";
                      }
                    }
                  }
                  else
                  {
                    report.ImpSanit = "0,00";
                    report.IdPolizza = 0;
                    report.Sanitario = "N";
                  }
                  listaReport.Add(report);
                }
              }
              else
              {
                DenunciaMensileDAL.log.Info((object) string.Format("[TFI.BLL] : Preparazione denuncia mensile - La data di iscrizione dell'utente: {0} risulta mancante sul DB in data: {1}", (object) anagraficaLavorativa.Nome, (object) DateTime.Now));
                break;
              }
            }
            else
            {
              DenunciaMensileDAL.log.Info((object) string.Format("[TFI.BLL] : Preparazione denuncia mensile - La data di nascita dell'utente: {0} risulta mancante sul DB in data: {1}", (object) anagraficaLavorativa.Nome, (object) DateTime.Now));
              break;
            }
          }
          db.EndTransaction(true);
          flag = false;
          DenunciaMensileDAL.MergingDuplicateRows(listaReport);
          DateTime dateTime2;
          for (int index = listaReport.Count - 1; index >= 0; --index)
          {
            if (listaReport[index].Rimuovi)
              listaReport.RemoveAt(index);
            else if (listaReport[index].Prev == "S")
            {
              List<ImportiPrev> importiPrev1;
              if ((importiPrev1 = DenunciaMensileDAL.GetImportiPrev(isArretrato, ref isImportoPrev, listaReport[index], codPos, out outcome)) != null)
              {
                outcome = new int?();
                listaReport[index].ProMod = importiPrev1[0].ProMod;
                if (importiPrev1[0].CodStaPre != 0)
                {
                  listaReport[index].Prev = "T";
                  if (isArretrato)
                  {
                    RetribuzioneRDL record = listaReport[index];
                    string strCodPos = codPos;
                    dateTime2 = Convert.ToDateTime(data_dal);
                    int year = dateTime2.Year;
                    ref int? local = ref outcome;
                    if (DenunciaMensileDAL.TheRowsMatches(record, strCodPos, year, out local) == 0)
                    {
                      int? nullable4 = outcome;
                      int num10 = 0;
                      if (nullable4.GetValueOrDefault() == num10 & nullable4.HasValue)
                      {
                        outcome = new int?();
                        isImportoPrev = new bool?(false);
                        goto label_278;
                      }
                    }
                    int? nullable5 = outcome;
                    int num11 = 1;
                    if (nullable5.GetValueOrDefault() == num11 & nullable5.HasValue)
                    {
                      DenunciaMensileDAL.log.Info((object) string.Format("[TFI.BLL] : Preparazione denuncia mensile - La funzione TheRowsMatches() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
                      DenunciaMensileDAL.ErrorMessage = "La funzione TheRowsMatches() ha generato un'eccezione";
                      return (List<RetribuzioneRDL>) null;
                    }
                  }
                }
label_278:
                if (isImportoPrev.HasValue && isImportoPrev.Value)
                {
                  listaReport[index].ImpRet = !DBNull.Value.Equals((object) importiPrev1[0].ImpRetPrv) ? importiPrev1[0].ImpRetPrv : importiPrev1[0].ImpRet;
                  listaReport[index].ImpOcc = !DBNull.Value.Equals((object) importiPrev1[0].ImpOccPrv) ? importiPrev1[0].ImpOccPrv : importiPrev1[0].ImpOcc;
                  listaReport[index].ImpFig = !DBNull.Value.Equals((object) importiPrev1[0].ImpFigPrv) ? importiPrev1[0].ImpFigPrv : importiPrev1[0].ImpFig;
                  listaReport[index].ImpCon = !DBNull.Value.Equals((object) importiPrev1[0].ImpConPrv) ? importiPrev1[0].ImpConPrv : importiPrev1[0].ImpCon;
                }
              }
              else if (isArretrato)
              {
                outcome = new int?();
                List<ImportiPrev> importiPrev2;
                if ((importiPrev2 = DenunciaMensileDAL.GetImportiPrev(listaReport[index], codPos, out outcome)) != null)
                {
                  listaReport[index].ProMod = importiPrev2[0].ProMod;
                  if (importiPrev2[0].CodStaPre != 0)
                    listaReport[index].Prev = "T";
                }
                else
                {
                  int? nullable6 = outcome;
                  int num12 = 1;
                  if (nullable6.GetValueOrDefault() == num12 & nullable6.HasValue)
                  {
                    DenunciaMensileDAL.log.Info((object) string.Format("[TFI.BLL] : Preparazione denuncia mensile - La funzione GetImportiPrev() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
                    DenunciaMensileDAL.ErrorMessage = "La funzione GetImportiPrev() ha generato un'eccezione";
                    return (List<RetribuzioneRDL>) null;
                  }
                }
              }
              else
              {
                int? nullable7 = outcome;
                int num13 = 1;
                if (nullable7.GetValueOrDefault() == num13 & nullable7.HasValue)
                {
                  DenunciaMensileDAL.log.Info((object) string.Format("[TFI.BLL] : Preparazione denuncia mensile - La funzione GetImportiPrev() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
                  DenunciaMensileDAL.ErrorMessage = "La funzione GetImportiPrev() ha generato un'eccezione";
                  return (List<RetribuzioneRDL>) null;
                }
              }
            }
          }
          if (!isArretrato)
          {
            foreach (RetribuzioneRDL report in listaReport)
            {
              report.NumGGPer = DenunciaMensileDAL.GetNumGGPeriodo(report.Dal, report.Al);
              report.NumGGDom = DenunciaMensileDAL.GetNumGGDomeniche(report.Dal, report.Al);
              report.NumGGAzi = 0.0M;
              report.NumGGSos = 0.0M;
              report.NumGGFig = 0.0M;
              outcome = new int?();
              if (DenunciaMensileDAL.GetNumGGSospensioni(codPos, report, out outcome))
              {
                outcome = new int?();
                if (num1 > 0)
                  report.NumGGConAzi = !(report.NumGGSos > 0M) ? (!(Convert.ToDateTime(report.DatDec) <= Convert.ToDateTime(report.Dal)) ? Decimal.Round((Decimal) num1 / Convert.ToDecimal(26), 0) * (Decimal) report.NumGGPer : (!(report.DatCes.Trim() == string.Empty) ? (!(Convert.ToDateTime(report.DatCes) > Convert.ToDateTime(report.Al)) ? Decimal.Round((Decimal) num1 / Convert.ToDecimal(26), 0) * (Decimal) report.NumGGPer : (Decimal) (report.NumGGPer - report.NumGGDom) - report.NumGGSos + report.NumGGAzi) : Decimal.Round(Convert.ToDecimal(report.NumGGPer) * Convert.ToDecimal(26M / (Decimal) num1), 0))) : (Decimal) (report.NumGGPer - report.NumGGDom) - report.NumGGSos + report.NumGGAzi;
                report.ImpFig = !(report.TipSpe == "S") ? Convert.ToDecimal(report.ImpTraEco) * report.NumGGFig / 26M : (Convert.ToDecimal(report.ImpMin) + Convert.ToDecimal(report.ImpSca)) * report.NumGGFig / 26M;
                report.ImpFig = Math.Round(Convert.ToDecimal(report.ImpFig));
                report.ImpFap = 0.0M;
                string strCodPos = codPos.ToString();
                dateTime2 = Convert.ToDateTime(data_dal);
                int year = dateTime2.Year;
                dateTime2 = Convert.ToDateTime(data_dal);
                int month4 = dateTime2.Month;
                int mat2 = report.Mat;
                int proRap = report.ProRap;
                ref int? local = ref outcome;
                List<FigurativaInfortuni> figurativaInfortuni;
                if ((figurativaInfortuni = DenunciaMensileDAL.GetFigurativaInfortuni(strCodPos, year, month4, mat2, proRap, out local)) != null)
                {
                  outcome = new int?();
                  report.ImpFig = figurativaInfortuni[0].ImpFig;
                }
                else
                {
                  int? nullable8 = outcome;
                  int num14 = 1;
                  if (nullable8.GetValueOrDefault() == num14 & nullable8.HasValue)
                  {
                    DenunciaMensileDAL.log.Info((object) string.Format("[TFI.BLL] : Preparazione denuncia mensile - La funzione GetFigurativaInfortuni() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
                    DenunciaMensileDAL.ErrorMessage = "La funzione GetFigurativaInfortuni() ha generato un'eccezione";
                    return (List<RetribuzioneRDL>) null;
                  }
                }
              }
              else
              {
                DenunciaMensileDAL.ErrorMessage = "La funzione GetSuspensions() ha generato un'eccezione";
                return (List<RetribuzioneRDL>) null;
              }
            }
          }
          else
          {
            foreach (RetribuzioneRDL retribuzioneRdl in listaReport)
            {
              retribuzioneRdl.NumGGConAzi = 0M;
              retribuzioneRdl.NumGGPer = 0;
              retribuzioneRdl.NumGGDom = 0;
              retribuzioneRdl.NumGGAzi = 0M;
              retribuzioneRdl.NumGGSos = 0M;
              retribuzioneRdl.NumGGFig = 0M;
              retribuzioneRdl.ImpFig = 0.0M;
              retribuzioneRdl.ImpFap = 0.0M;
            }
          }
        }
        return listaReport;
      }
      catch (Exception ex)
      {
        if (flag)
          db.EndTransaction(false);
        DenunciaMensileDAL.log.Info((object) string.Format("[TFI.BLL] : Preparazione denuncia mensile - La funzione ha generato un'eccezione in data: {0}. Messaggio: {1}", (object) DateTime.Now, (object) ex.Message));
        DenunciaMensileDAL.ErrorMessage = "La funzione di preparazione della nuova denuncia ha generato un'eccezione: " + ex.Message;
        return (List<RetribuzioneRDL>) null;
      }
    }

    public static string GetMatricolaByCodFiscale(string cFiscale, out int? outcome)
    {
      DataLayer dataLayer = new DataLayer();
      string strSQL = "SELECT MAT FROM ISCT WHERE CODFIS = '" + cFiscale + "'";
      try
      {
        string matricolaByCodFiscale = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
        if (!string.IsNullOrEmpty(matricolaByCodFiscale))
        {
          outcome = new int?(0);
          return matricolaByCodFiscale;
        }
        outcome = new int?(2);
        return string.Empty;
      }
      catch (Exception ex)
      {
        ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object) (HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente)), "");
        outcome = new int?(1);
        return string.Empty;
      }
    }

    private static void FillBasicFields(
      RetribuzioneRDL report,
      int tipRap,
      int codCon,
      Decimal proCon,
      int codLoc,
      Decimal proLoc,
      int codLiv,
      string livello,
      string qualifica,
      int codQuaCon,
      Decimal perPar,
      Decimal perApp,
      Decimal impMin,
      Decimal impTraEco,
      Decimal impSca,
      string fap,
      string assCon,
      string abbPre,
      string tipSpe,
      int codGruAss,
      string tipFondo,
      int mat,
      int proRap,
      string nome,
      string datNas,
      string datDec,
      string datCes)
    {
      report.TipRap = tipRap;
      report.CodCon = codCon;
      report.ProCon = proCon;
      report.CodLoc = codLoc;
      report.ProLoc = proLoc;
      report.CodLiv = codLiv;
      report.Livello = livello;
      report.Qualifica = qualifica;
      report.CodQuaCon = codQuaCon;
      report.PerPar = perPar;
      report.PerApp = perApp;
      report.ImpMin = impMin;
      report.ImpTraEco = impTraEco;
      report.ImpSca = impSca;
      report.Fap = fap;
      report.AssCon = assCon;
      report.AbbPre = abbPre;
      report.TipSpe = tipSpe;
      report.CodGruAss = codGruAss;
      report.TipFondo = tipFondo;
      report.Mat = mat;
      report.ProRap = proRap;
      report.Nome = nome;
      report.DatNas = datNas;
      report.DatDec = datDec;
      report.DatCes = datCes;
    }

    private static Decimal GetImportoParametro(
      List<ParametriGenerali> parameters,
      int codPar,
      string dal,
      string flag = "S")
    {
      DateTime data = Convert.ToDateTime(dal);
      Decimal importoParametro;
      if (flag == "S")
      {
        parameters = DenunciaMensileDAL.listaParametriGenerali.Where<ParametriGenerali>((Func<ParametriGenerali, bool>) (p => p.CodPar == (Decimal) codPar && DateTime.Parse(p.DataInizio) <= data)).ToList<ParametriGenerali>();
        importoParametro = parameters[0].Valore != null ? Convert.ToDecimal(parameters[0].Valore.Trim()) : 0M;
      }
      else
        importoParametro = !string.IsNullOrEmpty(DenunciaMensileDAL.listaParametriGenerali[0].Valore) ? Convert.ToDecimal(DenunciaMensileDAL.listaParametriGenerali[0].Valore.Trim()) : 0M;
      return importoParametro;
    }

    private static Decimal GetImportoParametro(int codPar, string dal, string flag = "S")
    {
      DateTime data = Convert.ToDateTime(dal);
      Decimal importoParametro;
      if (flag == "S")
      {
        List<ParametriGenerali> list = DenunciaMensileDAL.listaParametriGenerali.Where<ParametriGenerali>((Func<ParametriGenerali, bool>) (p => p.CodPar == (Decimal) codPar && DateTime.Parse(p.DataInizio) <= data)).ToList<ParametriGenerali>();
        importoParametro = list[0].Valore != null ? Convert.ToDecimal(list[0].Valore.Trim()) : 0M;
      }
      else
        importoParametro = !string.IsNullOrEmpty(DenunciaMensileDAL.listaParametriGenerali[0].Valore) ? Convert.ToDecimal(DenunciaMensileDAL.listaParametriGenerali[0].Valore.Trim()) : 0M;
      return importoParametro;
    }

    private static void MergingDuplicateRows(List<RetribuzioneRDL> listaReport)
    {
      int num1 = 0;
      int num2 = 0;
      Decimal num3 = 0M;
      string str = string.Empty;
      for (int index = 0; index <= listaReport.Count - 1; ++index)
      {
        if (listaReport[index].Mat == num1 && listaReport[index].Aliquota == num3 && listaReport[index].DatCes == str)
        {
          listaReport[index - num2].Al = listaReport[index].Al;
          listaReport[index - num2].ImpSca = listaReport[index].ImpSca;
          listaReport[index - num2].ImpMin = listaReport[index].ImpMin;
          listaReport[index - num2].DatDec = listaReport[index].DatDec;
          listaReport[index - num2].ImpTraEco = listaReport[index].ImpTraEco;
          listaReport[index - num2].AbbPre = listaReport[index].AbbPre;
          listaReport[index - num2].ImpAbb = listaReport[index].ImpAbb;
          listaReport[index - num2].AssCon = listaReport[index].AssCon;
          listaReport[index - num2].ImpAssCon = listaReport[index].ImpAssCon;
          listaReport[index].Rimuovi = true;
          ++num2;
        }
        else
          num2 = 1;
        listaReport[index].ImpRet = 0.00M;
        listaReport[index].ImpOcc = 0.00M;
        listaReport[index].ImpCon = 0.00M;
        listaReport[index].ImpFig = 0.00M;
        num1 = listaReport[index].Mat;
        num3 = listaReport[index].Aliquota;
        str = listaReport[index].DatCes;
      }
    }

    private static int GetNumGGPeriodo(string dataDal, string dataAl) => Convert.ToDateTime(dataAl).Subtract(Convert.ToDateTime(dataDal)).Days + 1;

    private static int GetNumGGDomeniche(string dataDal, string dataAl)
    {
      int numGgDomeniche = 0;
      DateTime dateTime1 = Convert.ToDateTime(dataDal);
      for (DateTime dateTime2 = Convert.ToDateTime(dataAl); dateTime1 <= dateTime2; dateTime1 = dateTime1.AddDays(1.0))
      {
        if (dateTime1.DayOfWeek == DayOfWeek.Sunday)
          ++numGgDomeniche;
      }
      return numGgDomeniche;
    }

    private static bool GetNumGGSospensioni(
      string codPos,
      RetribuzioneRDL report,
      out int? outcome)
    {
      string dal = report.Dal;
      string al = report.Al;
      int mat = report.Mat;
      int proRap = report.ProRap;
      int num1 = 0;
      Decimal num2 = 0.0M;
      Decimal num3 = 0.0M;
      Decimal codSos = (Decimal) report.CodSos;
      DateTime dateTime1 = Convert.ToDateTime(dal);
      DateTime dateTime2 = Convert.ToDateTime(al);
      List<SospensioneRapporto> sospensioneRapportoList = new List<SospensioneRapporto>();
      List<SospensioneRapporto> suspensions;
      if ((suspensions = DenunciaMensileDAL.GetSuspensions(codPos, dal, al, mat, proRap, out outcome)) != null)
      {
        outcome = new int?();
        foreach (SospensioneRapporto sospensioneRapporto in suspensions)
        {
          if (Convert.ToDateTime(sospensioneRapporto.DataFine) >= dateTime1)
          {
            if (codSos == 0M)
              codSos += sospensioneRapporto.CodSos;
            else
              string.Format("{0}-{1}", (object) codSos, (object) sospensioneRapporto.CodSos);
            TimeSpan timeSpan;
            DateTime dateTime3;
            if (Convert.ToDateTime(sospensioneRapporto.DataInizio) <= dateTime2)
            {
              if (dateTime1 >= Convert.ToDateTime(sospensioneRapporto.DataInizio))
              {
                if (dateTime2 <= Convert.ToDateTime(sospensioneRapporto.DataFine))
                {
                  int numGgDomeniche = DenunciaMensileDAL.GetNumGGDomeniche(dal, al);
                  timeSpan = dateTime2.Subtract(dateTime1);
                  num1 = timeSpan.Days + 1 - numGgDomeniche;
                  if (sospensioneRapporto.PerAzi > 0M)
                    num2 = (Decimal) num1 * sospensioneRapporto.PerAzi / 100M;
                  if (sospensioneRapporto.PerFig > 0M)
                  {
                    num3 = (Decimal) num1 * sospensioneRapporto.PerFig / 100M;
                    break;
                  }
                  break;
                }
                int numGgDomeniche1 = DenunciaMensileDAL.GetNumGGDomeniche(dal, sospensioneRapporto.DataFine);
                dateTime3 = Convert.ToDateTime(sospensioneRapporto.DataFine);
                timeSpan = dateTime3.Subtract(dateTime1);
                num1 = timeSpan.Days + 1 - numGgDomeniche1;
                if (sospensioneRapporto.PerAzi > 0M)
                  num2 = (Decimal) num1 * sospensioneRapporto.PerAzi / 100M;
                if (sospensioneRapporto.PerFig > 0M)
                  num3 = (Decimal) num1 * sospensioneRapporto.PerFig / 100M;
              }
              else
              {
                if (dateTime2 <= Convert.ToDateTime(sospensioneRapporto.DataFine))
                {
                  int numGgDomeniche = DenunciaMensileDAL.GetNumGGDomeniche(sospensioneRapporto.DataInizio, al);
                  int num4 = num1;
                  timeSpan = dateTime2.Subtract(Convert.ToDateTime(sospensioneRapporto.DataInizio));
                  int num5 = timeSpan.Days + 1 - numGgDomeniche;
                  num1 = num4 + num5;
                  if (sospensioneRapporto.PerAzi > 0M)
                  {
                    Decimal num6 = num2;
                    timeSpan = dateTime2.Subtract(Convert.ToDateTime(sospensioneRapporto.DataInizio));
                    Decimal num7 = (Decimal) (timeSpan.Days + 1) - (Decimal) numGgDomeniche * sospensioneRapporto.PerAzi / 100M;
                    num2 = num6 + num7;
                  }
                  if (sospensioneRapporto.PerFig > 0M)
                  {
                    Decimal num8 = num3;
                    timeSpan = dateTime2.Subtract(Convert.ToDateTime(sospensioneRapporto.DataInizio));
                    Decimal num9 = (Decimal) (timeSpan.Days + 1) - (Decimal) numGgDomeniche * sospensioneRapporto.PerFig / 100M;
                    num3 = num8 + num9;
                    break;
                  }
                  break;
                }
                int numGgDomeniche2 = DenunciaMensileDAL.GetNumGGDomeniche(sospensioneRapporto.DataInizio, sospensioneRapporto.DataFine);
                int num10 = num1;
                dateTime3 = Convert.ToDateTime(sospensioneRapporto.DataFine);
                timeSpan = dateTime3.Subtract(Convert.ToDateTime(sospensioneRapporto.DataInizio));
                int num11 = timeSpan.Days + 1 - numGgDomeniche2;
                num1 = num10 + num11;
                if (sospensioneRapporto.PerAzi > 0M)
                {
                  Decimal num12 = num2;
                  dateTime3 = Convert.ToDateTime(sospensioneRapporto.DataFine);
                  timeSpan = dateTime3.Subtract(Convert.ToDateTime(sospensioneRapporto.DataInizio));
                  Decimal num13 = (Decimal) (timeSpan.Days + 1) - (Decimal) numGgDomeniche2 * sospensioneRapporto.PerAzi / 100M;
                  num2 = num12 + num13;
                }
                if (sospensioneRapporto.PerFig > 0M)
                {
                  Decimal num14 = num3;
                  dateTime3 = Convert.ToDateTime(sospensioneRapporto.DataFine);
                  timeSpan = dateTime3.Subtract(Convert.ToDateTime(sospensioneRapporto.DataInizio));
                  Decimal num15 = (Decimal) (timeSpan.Days + 1) - (Decimal) numGgDomeniche2 * sospensioneRapporto.PerFig / 100M;
                  num3 = num14 + num15;
                }
              }
            }
          }
        }
        outcome = new int?(0);
      }
      else
      {
        int? nullable = outcome;
        int num16 = 1;
        if (nullable.GetValueOrDefault() == num16 & nullable.HasValue)
          return false;
      }
      report.NumGGSos = (Decimal) num1;
      report.NumGGFig = num3;
      report.NumGGAzi = num2;
      return true;
    }

    public static bool CheckDenunciaEsistente(int anno, int mese, string codPos)
    {
        var dataLayer = new DataLayer();
        var codPosParam = dataLayer.CreateParameter("@codPos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
        var annoParam = dataLayer.CreateParameter("@anno", iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input, anno.ToString());
        var meseParam = dataLayer.CreateParameter("@mese", iDB2DbType.iDB2Decimal, 2, ParameterDirection.Input, mese.ToString());

        var sqlQuery = 
            "SELECT COUNT(*) AS COUNT FROM DENTES " +
            "WHERE CODPOS = @codPos AND ANNDEN = @anno AND MESDEN = @mese AND " +
                "((TIPMOV = 'DP' AND NUMMOV IS NOT NULL AND DATMOVANN IS NULL) OR (TIPMOV = 'NU' AND NUMMOV IS NOT NULL AND DATMOVANN IS NULL))";

        return dataLayer.GetDataTableWithParameters(sqlQuery, codPosParam, annoParam, meseParam).Rows[0].ElementAt("COUNT") != "0";

    }

    public static string GetEmailAzienda(string codPos)
    {
        DataLayer dataLayer = new DataLayer();
        var codPosParam = dataLayer.CreateParameter("@codPos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
        var getEmailSqlQuery = "SELECT EMAIL FROM AZEMAIL WHERE CODPOS = @codPos AND DATINI = (SELECT MAX(DATINI) FROM AZEMAIL WHERE CODPOS = @codPos)";
        var emailAzienda = dataLayer.GetDataTableWithParameters(getEmailSqlQuery, codPosParam, codPosParam);
        return emailAzienda.Rows.Count > 0 ? emailAzienda.Rows[0]["EMAIL"].ToString() : default;
    }

    public static bool AggiornaDenunciaTestataConProtocollo(Protocollo protocollo, int anno, int mese, int proDen, string codPosizione)
    {
      DataLayer dataLayer = new DataLayer();

      var codPosParam = dataLayer.CreateParameter(CodicePosizione, iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPosizione);
      var annoParam = dataLayer.CreateParameter(AnnoDenuncia, iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input, anno.ToString());
      var meseParam = dataLayer.CreateParameter(MeseDenuncia, iDB2DbType.iDB2Decimal, 2, ParameterDirection.Input, mese.ToString());
      var proDenParam = dataLayer.CreateParameter(ProgressivoDenuncia, iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, proDen.ToString());

      var sqlQuery =
        $"UPDATE DENTES SET PROTRIC = '{protocollo.ProtocolloCompleto}', NUMPROTRIC = '{protocollo.NumeroProtocollo}', DATPROTRIC = '{protocollo.DataProtocollo.StandardizeDateString(StandardUse.Internal)}' " +
        $"WHERE CODPOS = {CodicePosizione} AND ANNDEN = {AnnoDenuncia} AND MESDEN = {MeseDenuncia} AND PRODEN = {ProgressivoDenuncia}";
      return dataLayer.WriteDataWithParameters(sqlQuery, CommandType.Text, codPosParam, annoParam,
        meseParam, proDenParam);
    }

    public static Protocollo GetProtocolloDenunciaMensile(int anno, int mese, int proDen, string codPos)
    {
      var dataLayer = new DataLayer();

      var codPosParam =
        dataLayer.CreateParameter(CodicePosizione, iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
      var annoParam = dataLayer.CreateParameter(AnnoDenuncia, iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input,
        anno.ToString());
      var meseParam = dataLayer.CreateParameter(MeseDenuncia, iDB2DbType.iDB2Decimal, 2, ParameterDirection.Input,
        mese.ToString());
      var proDenParam = dataLayer.CreateParameter(ProgressivoDenuncia, iDB2DbType.iDB2Decimal, 3,
        ParameterDirection.Input, proDen.ToString());

      var sqlQuery =
        $"SELECT PROTRIC FROM DENTES WHERE CODPOS = {CodicePosizione} AND ANNDEN = {AnnoDenuncia} AND MESDEN = {MeseDenuncia} AND PRODEN = {ProgressivoDenuncia}";
      var denunciaMensileTestata = dataLayer.GetDataTableWithParameters(sqlQuery, codPosParam, annoParam, meseParam, proDenParam);
      var protRic = denunciaMensileTestata.Rows[0].ElementAt("PROTRIC");
      return denunciaMensileTestata.Rows.Count == 0 || string.IsNullOrWhiteSpace(protRic)
        ? new Protocollo() 
        : new Protocollo(protRic){Success = true};
    }

    public static bool IsDenunciaMesePrecedenteConfermata(int anno, int mese, int codPos)
    {
      var dataLayer = new DataLayer();
      var codPosParam = dataLayer.CreateParameter(CodicePosizione, iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos.ToString());
      var annoParam = dataLayer.CreateParameter(AnnoDenuncia, iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input, anno.ToString());
      var meseParam = dataLayer.CreateParameter(MeseDenuncia, iDB2DbType.iDB2Decimal, 2, ParameterDirection.Input, mese.ToString());

      var denunciaSqlQuery = "SELECT * FROM DENTES " +
                                  $"WHERE CODPOS = {CodicePosizione} AND ANNDEN = {AnnoDenuncia} AND MESDEN = {MeseDenuncia} " +
                                  "AND STADEN = 'A' AND TIPMOV = 'DP'";
      
      return dataLayer.GetDataTableWithParameters(denunciaSqlQuery, codPosParam, annoParam, meseParam).Rows.Count != 0;
    }

    public static int GetProDen(int annoDenuncia, int meseDenuncia, string codPosAzienda)
    {
      var dataLayer = new DataLayer();
      var codPosParam = dataLayer.CreateParameter(CodicePosizione, iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPosAzienda);
      var annoParam = dataLayer.CreateParameter(AnnoDenuncia, iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input, annoDenuncia.ToString());
      var meseParam = dataLayer.CreateParameter(MeseDenuncia, iDB2DbType.iDB2Decimal, 2, ParameterDirection.Input, meseDenuncia.ToString());

      var denunciaSqlQuery = "SELECT PRODEN FROM DENTES " +
                                 $"WHERE CODPOS = {CodicePosizione} AND ANNDEN = {AnnoDenuncia} AND MESDEN = {MeseDenuncia} " +
                                  "AND TIPMOV = 'DP'";
      var result = dataLayer.GetDataTableWithParameters(denunciaSqlQuery, codPosParam, annoParam, meseParam);
      return result.Rows.Count == 0 
                                ? 0 
                                : result.Rows[0].IntElementAt("PRODEN").Value;
    }
  }
}
