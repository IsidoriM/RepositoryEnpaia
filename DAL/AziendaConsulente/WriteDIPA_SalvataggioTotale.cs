// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.WriteDIPA_SalvataggioTotale
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;
using Utilities;

namespace TFI.DAL.AziendaConsulente
{
  public class WriteDIPA_SalvataggioTotale
  {
    public static string Trace;

    public static bool WRITE_CONFERMA_DENUNCIA(
      DataLayer db,
      OCM.Utente.Utente utente,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      string TIPMOV,
      string DATCHI,
      string UTECHI,
      string STADEN,
      decimal IMPDEC,
      string DATORARIC,
      string RICSANUTE,
      string NOTE)
    {
      try
      {
        string strDatSca = (string) null;
        string str2 = "";
        int[,] sanzioni = new int[1, 3];
        int totGgRitardo = 0;
        decimal tasso = 0.0M;
        decimal impSoglia = 0.0M;
        decimal addizionale = 0.0M;
        bool flag1 = false;
        decimal perMaxSoglia = 0.0M;
        string str3 = "";

        int datChiYear = DateTime.Parse(DATCHI.Substring(0,10)).Year;

        string strSQL1 = $"SELECT VALUE(MAX( DECIMAL(VALUE(SUBSTR(NUMRIC, 6), '0'))), 0) AS NUMRIC FROM DENTES " +
                         $"WHERE SUBSTR(NUMRIC, 0, 5) = {datChiYear}";

        string numric = db.Get1ValueFromSQL(strSQL1, CommandType.Text);

        numric = Convert.ToInt32(numric) <= 0 
            ? "1" 
            : Convert.ToString(Convert.ToInt32(numric) + 1);
                
        numric = $"{datChiYear}/{numric}";
        
        //if (IDDIPA != 0)
        //{
        //  int year1 = DateTime.Now.Year;
        //  string strSQL2 = "SELECT VALUE(MAX(DECIMAL(VALUE(SUBSTR(NUMRIC, 6), '0'))), 0) AS NUMRIC " + " FROM FIACONSIP.DIPATES WHERE SUBSTR(NUMRIC, 0, 5) = " + year1.ToString();
        //  string str7 = db.Get1ValueFromSQL(strSQL2, CommandType.Text);
        //  string str8 = Convert.ToInt32("0" + str7) <= 0 ? "1" : Convert.ToString(Convert.ToInt32(str7) + 1);
        //  year1 = DateTime.Now.Year;
        //  str3 = year1.ToString() + "/" + str8.ToString();
        //}

        string str9 = "UPDATE DENTES SET";
        string str10 = (!string.IsNullOrEmpty(DATCHI) ? 
                           $"{str9} DATCHI = {DBMethods.DoublePeakForSql(DATCHI)}, " : 
                           $"{str9} DATCHI = Null, ") +
                       $"UTECHI = {DBMethods.DoublePeakForSql(UTECHI)}, " +
                       $"STADEN = {DBMethods.DoublePeakForSql(STADEN)}, " +
                       $"IMPDEC = {IMPDEC.ToString().Replace(",", ".")}, " + 
                       $"NUMRIC = {DBMethods.DoublePeakForSql(numric)}";

        string str11 = !string.IsNullOrEmpty(DATORARIC) ? 
            $"{str10}, DATORARIC = {DBMethods.DoublePeakForSql(DATORARIC)}" : 
            $"{str10}, DATORARIC = Null";

        string strSQL3 = (!string.IsNullOrEmpty(NOTE) ? 
                             $"{str11}, NOTE = {DBMethods.DoublePeakForSql(NOTE)}, " : 
                             $"{str11}, NOTE = Null, ") + 
                         "ULTAGG = CURRENT_TIMESTAMP, " + 
                         $"UTEAGG = {DBMethods.DoublePeakForSql(utente.Username)} " +
                         $"WHERE CODPOS = {CODPOS} " +
                         $"AND ANNDEN = {ANNDEN} " +
                         $"AND MESDEN = {MESDEN} " +
                         $"AND PRODEN = {PRODEN}";

        bool flag2 = db.WriteTransactionData(strSQL3, CommandType.Text);

        //if (IDDIPA != 0 && flag2)
        //{
        //  string strSQL4 = "UPDATE FIACONSIP.DIPATES SET NUMRIC = " + DBMethods.DoublePeakForSql(str3) + ", DATCHI = CURRENT_TIMESTAMP " + " WHERE IDDIPA = " + IDDIPA.ToString();
        //  flag2 = db.WriteTransactionData(strSQL4, CommandType.Text);
        //}

        if (flag2)
        {
          int giorni = 0;
          int ggSoglia = 0;
          impSoglia = 0.0M;
          tasso = 0.0M;
          str2 = "";
          perMaxSoglia = 0.0M;
          string ricSanUte = RICSANUTE ?? "";
          string tipMovSan = ricSanUte == "OMISSIONE" ? "SAN_MD" : (ricSanUte == "RITARDO" ? "SAN_RD" : string.Empty);
          if ((UTECHI.Trim() ?? "") == "A" && string.IsNullOrEmpty(tipMovSan) & TIPMOV == "AR")
              tipMovSan = "SAN_RD";
          if (!string.IsNullOrEmpty(tipMovSan))
          {
            string strSQL5 = $"SELECT CODCAU FROM TIPMOVCAU " +
                             $"WHERE TIPMOV = {DBMethods.DoublePeakForSql(tipMovSan)} " +
                             $"AND CURRENT_DATE BETWEEN DATINI AND DATFIN";

            string codCauSan = db.Get1ValueFromSQL(strSQL5, CommandType.Text);

            string tipMov = TIPMOV ?? "";
            if (tipMov != "DP")
            {
              if (tipMov == "AR")
              {
                string strSQL6 = $"SELECT DISTINCT ANNCOM FROM DENDET " +
                                 $"WHERE CODPOS = {CODPOS} " +
                                 $"AND ANNDEN = {ANNDEN} " +
                                 $"AND MESDEN = {MESDEN} " +
                                 $"AND PRODEN = {PRODEN} ORDER BY ANNCOM DESC";
                DataTable dataTable = db.GetDataTable(strSQL6);

                //sanzioni = (int[,]) Array.CreateInstance(Type.GetType("System.Int32"), dataTable.Rows.Count, 2);
                sanzioni = new int[dataTable.Rows.Count, 2];
                for (int index = 0; index <= dataTable.Rows.Count - 1; ++index)
                {
                  int annCom = Convert.ToInt32(dataTable.Rows[index]["ANNCOM"]);
                  DateTime now = DateTime.Now;
                  int currentYear = now.Year;
                  int ggRitardo;
                  if (annCom != currentYear)
                  {
                    string strDatSan = $"01/01/{annCom}";
                    now = DateTime.Parse(DATCHI.Substring(0, 10));
                    ggRitardo = now.Subtract(DateTime.Parse(strDatSan).AddYears(1)).Days;
                    if (ggRitardo > 0)
                      flag1 = true;
                  }
                  else
                      ggRitardo = 0;
                  
                  sanzioni.SetValue(ggRitardo, index, 0);
                  sanzioni.SetValue(annCom, index, 1);
                }

                dataTable.Rows.Clear();
              }
            }
            else
            {
              string strSQL7 = $"SELECT VALORE FROM PARGENDET WHERE CODPAR = 3 " +
                               $"AND {DBMethods.DoublePeakForSql(DBMethods.Db2Date($"01/{MESDEN}/{ANNDEN}"))} BETWEEN DATINI AND DATFIN";
              strDatSca = db.Get1ValueFromSQL(strSQL7, CommandType.Text);
              int ggRitardo = DateTime.Now.Subtract(DateTime.Parse(strDatSca)).Days;
              sanzioni.SetValue(ggRitardo, 0, 0);
              if (ggRitardo > 0)
                flag1 = true;
            }
            if (flag1)
            {
              //string str16 = tipMovSan ?? "";
              if (tipMovSan == "SAN_RD" || tipMovSan == "SAN_MD")
              {
                string strSQL8 = "SELECT VALUE(GIORNI, 0) AS GIORNI, VALUE(TASSO, 0.0) AS TASSO, VALUE(GGSOGLIA, 0) AS GGSOGLIA, VALUE(IMPSOGLIA, 0.0) AS IMPSOGLIA, VALUE(PERMAXSOGLIA, 0.0) AS PERMAXSOGLIA FROM TIPMOVCAU " +
                                 $"WHERE TIPMOV = {DBMethods.DoublePeakForSql(tipMovSan)} " + 
                                 $"AND {DBMethods.DoublePeakForSql($"{ANNDEN}-{MESDEN}-01")} BETWEEN DATINI AND DATFIN";
                DataTable dataTable = db.GetDataTable(strSQL8);
                if (dataTable.Rows.Count > 0)
                {
                  if (tipMovSan == "SAN_MD")
                    giorni = !DBNull.Value.Equals(dataTable.Rows[0]["GIORNI"]) ? Convert.ToInt32(dataTable.Rows[0]["GIORNI"]) : 0;

                  tasso = !DBNull.Value.Equals(dataTable.Rows[0]["TASSO"]) ? Convert.ToDecimal(dataTable.Rows[0]["TASSO"]) : 0.00M;
                  ggSoglia = !DBNull.Value.Equals(dataTable.Rows[0]["GGSOGLIA"]) ? Convert.ToInt32(dataTable.Rows[0]["GGSOGLIA"]) : 0;
                  impSoglia = !DBNull.Value.Equals(dataTable.Rows[0]["IMPSOGLIA"]) ? Convert.ToDecimal(dataTable.Rows[0]["IMPSOGLIA"]) : 0.00M;
                  perMaxSoglia = !DBNull.Value.Equals(dataTable.Rows[0]["PERMAXSOGLIA"]) ? Convert.ToDecimal(dataTable.Rows[0]["PERMAXSOGLIA"]) : 0.00M;
                }
                //string strSQL9 = "SELECT VALORE FROM PARGENDET WHERE CODPAR = 5 AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
                //addizionale = Convert.ToDecimal("0" + db.Get1ValueFromSQL(strSQL9, CommandType.Text)); 
                dataTable.Dispose();

                int upperBound = sanzioni.GetUpperBound(0);
                for (int index1 = 0; index1 <= upperBound; ++index1)
                {
                  int ggRitardo = Convert.ToInt32(sanzioni.GetValue(index1, 0));
                  if (ggRitardo > 0)
                  {
                    if (tipMovSan == "SAN_MD")
                        ggRitardo += giorni;

                    string strSQL10 = "UPDATE DENDET SET " +
                                      $"IMPSANDET = (IMPRET * ALIQUOTA / 100) * {ggRitardo.ToString().Replace(",", ".")} * {tasso.ToString().Replace(",", ".")} / 36500" + 
                                      $", TASSAN = {tasso.ToString().Replace(",", ".")}" +
                                      $" WHERE CODPOS = {CODPOS}" + 
                                      $" AND ANNDEN = {ANNDEN}" +
                                      $" AND MESDEN = {MESDEN}" + 
                                      $" AND PRODEN = {PRODEN}";
                    if (TIPMOV == "AR")
                      strSQL10 = $"{strSQL10} AND ANNCOM ={sanzioni.GetValue(index1, 1)}";

                    flag2 = db.WriteTransactionData(strSQL10, CommandType.Text);

                    if (flag2)
                    {
                      if (perMaxSoglia > 0M)
                      {
                        string str17 = "UPDATE DENDET SET " +
                                       $"IMPSANDET = IMPCON * {perMaxSoglia.ToString().Replace(",", ".")} / 100" + 
                                       $" WHERE CODPOS = {CODPOS}" + 
                                       $" AND ANNDEN = {ANNDEN}" + 
                                       $" AND MESDEN = {MESDEN}" +
                                       $" AND PRODEN = {PRODEN}";
                        if (TIPMOV == "AR")
                          str17 = $"{str17} AND ANNCOM ={sanzioni.GetValue(index1, 1)}";

                        string strSQL11 = $"{str17} AND IMPCON * {perMaxSoglia.ToString().Replace(",", ".")} / 100 < IMPSANDET";
                        flag2 = db.WriteTransactionData(strSQL11, CommandType.Text);
                        if (!flag2)
                          break;
                      }
                      totGgRitardo += ggRitardo;
                    }
                    else
                      break;
                  }
                }
                if (flag2)
                {
                  string strSQL12 = "UPDATE DENTES SET " +
                                    $"IMPSANDET = (SELECT SUM(IMPSANDET) FROM DENDET WHERE CODPOS = {CODPOS} AND ANNDEN = {ANNDEN} AND MESDEN = {MESDEN} AND PRODEN = {PRODEN}), " +
                                    $"CODCAUSAN = {DBMethods.DoublePeakForSql(codCauSan)} " + 
                                    $"WHERE CODPOS = {CODPOS} " + 
                                    $"AND ANNDEN = {ANNDEN} " + 
                                    $"AND MESDEN = {MESDEN} " +
                                    $"AND PRODEN = {PRODEN}";

                  flag2 = db.WriteTransactionData(strSQL12, CommandType.Text);
                  
                  if (flag2)
                  {
                    if (tipMov != "DP")
                    {
                      if (tipMov == "AR")
                        strSQL12 = "UPDATE DENDET SET " +
                                   "DATINISAN = '' || TRIM(CHAR(ANNCOM+1)) || '-01-01', " +
                                   $"DATFINSAN = {DBMethods.DoublePeakForSql(DATCHI.Substring(0, 10))}";
                    }
                    else
                      strSQL12 = "UPDATE DENDET SET " +
                                 $"DATINISAN = {DBMethods.DoublePeakForSql(DBMethods.Db2Date(strDatSca))}, " +
                                 $"DATFINSAN = {DBMethods.DoublePeakForSql(DATCHI.Substring(0, 10))}";
                    
                    string strSQL13 = $"{strSQL12}, CODCAUSAN = {DBMethods.DoublePeakForSql(codCauSan)} " + 
                                      $"WHERE CODPOS = {CODPOS} " + 
                                      $"AND ANNDEN = {ANNDEN} " +
                                      $"AND MESDEN = {MESDEN} " +
                                      $"AND PRODEN = {PRODEN} " + 
                                      "AND IMPSANDET > 0";
                    flag2 = db.WriteTransactionData(strSQL13, CommandType.Text);
                    if (flag2)
                    {
                      if (totGgRitardo <= ggSoglia)
                      {
                        string strSQL14 = "UPDATE DENTES SET SANSOTSOG = 'S' " +
                                          $"WHERE CODPOS= {CODPOS} " +
                                          $"AND ANNDEN = {ANNDEN} " +
                                          $"AND MESDEN = {MESDEN} " +
                                          $"AND PRODEN = {PRODEN}";
                        flag2 = db.WriteTransactionData(strSQL14, CommandType.Text);
                      }
                      else
                      {
                        string strSQL15 = "SELECT VALUE(IMPSANDET, 0.0) FROM DENTES " +
                                          $"WHERE CODPOS= {CODPOS} " +
                                          $"AND ANNDEN = {ANNDEN} " +
                                          $"AND MESDEN = {MESDEN} " +
                                          $"AND PRODEN = {PRODEN}";
                        if (Convert.ToDecimal(db.Get1ValueFromSQL(strSQL15, CommandType.Text)) <= impSoglia)
                        {
                          string strSQL16 = "UPDATE DENTES SET SANSOTSOG = 'S' " +
                                            $"WHERE CODPOS= {CODPOS} " +
                                            $"AND ANNDEN = {ANNDEN} " +
                                            $"AND MESDEN = {MESDEN} " +
                                            $"AND PRODEN = {PRODEN}";
                          flag2 = db.WriteTransactionData(strSQL16, CommandType.Text);
                        }
                        else
                        {
                          string strSQL17 = "UPDATE DENTES SET SANSOTSOG = 'N' " +
                                            $"WHERE CODPOS= {CODPOS} " +
                                            $"AND ANNDEN = {ANNDEN} " +
                                            $"AND MESDEN = {MESDEN} " +
                                            $"AND PRODEN = {PRODEN}";
                          flag2 = db.WriteTransactionData(strSQL17, CommandType.Text);
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        return flag2;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool WRITE_INSERT_ALISOS(
      DataLayer db,
      OCM.Utente.Utente utente,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN)
    {
      bool flag = false;
      try
      {
        string strSQL1 = "SELECT VALUE(VALFAP, 0) AS VALFAP FROM CODFAP " +
                         $"WHERE {DBMethods.DoublePeakForSql($"{ANNDEN}-{MESDEN}-01")} BETWEEN DATINI AND DATFIN";
        decimal perFap = Convert.ToDecimal(db.Get1ValueFromSQL(strSQL1, CommandType.Text));

        string strSQL2 = "SELECT MAT, PRORAP, PRODENDET, DAL, AL, IMPRET, IMPCON, CODGRUASS, CODQUACON, ETA65, FAP, VALUE(PERFAP, 0.0) AS PERFAP FROM DENDET " +
                         $"WHERE CODPOS = {CODPOS} " +
                         $"AND ANNDEN = {ANNDEN} " +
                         $"AND MESDEN = {MESDEN} " +
                         $"AND PRODEN = {PRODEN}";
        DataTable dataTable = db.GetDataTable(strSQL2);
        Trace = "alisos 1";
        for (int index = 0; index <= dataTable.Rows.Count - 1; ++index)
        {
          string strSQL3 = "INSERT INTO DENDETSOS (CODPOS, ANNDEN, MESDEN, PRODEN, MAT, PRODENDET, PRORAP, CODSOS, DATINISOS, DATFINSOS, PERAZI, PERCFIG, ULTAGG, UTEAGG) " + 
                           $"SELECT CODPOS, {ANNDEN} AS ANNDEN, {MESDEN} AS MESDEN, {PRODEN} AS PRODEN, MAT, {dataTable.Rows[index]["PRODENDET"]} AS PRODENDET, PRORAP, CODSOS, DATINISOS, DATFINSOS, PERAZI, PERFIG, CURRENT_TIMESTAMP, {DBMethods.DoublePeakForSql(utente.Username)} AS UTEAGG FROM SOSRAP " +
                           $"WHERE  CODPOS = {CODPOS} " +
                           $"AND MAT = {dataTable.Rows[index]["MAT"]} " +
                           $"AND PRORAP = {dataTable.Rows[index]["PRORAP"]} " +
                           $"AND DATINISOS <= {DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable.Rows[index]["AL"].ToString()))} " +
                           $"AND VALUE(DATFINSOS,'9999-12-31') >= {DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable.Rows[index]["DAL"].ToString()))} " +
                           $"AND STASOS = 0";
          flag = db.WriteTransactionData(strSQL3, CommandType.Text);
          WriteDIPA_SalvataggioTotale.Trace = "alisos 2";
          if (flag)
          {
            string str1 = "INSERT INTO DENDETALI(CODPOS , ANNDEN , MESDEN, PRODEN, MAT, PRODENDET, CODFORASS, ALIQUOTA, IMPCON, ULTAGG,UTEAGG) " +
                          $"SELECT {CODPOS} AS CODPOS, {ANNDEN} AS ANNDEN, {MESDEN} AS MESDEN, {PRODEN} AS PRODEN, {dataTable.Rows[index]["MAT"]} AS MAT, {dataTable.Rows[index]["PRODENDET"]} AS PRODENDET, CODFORASS, ALIQUOTA, ROUND({dataTable.Rows[index]["IMPRET"].ToString().Replace(",", ".")} * ALIQUOTA / 100, 2) AS IMPCON, CURRENT_TIMESTAMP, {DBMethods.DoublePeakForSql(utente.Username)} AS UTEAGG FROM ALIFORASS " +
                          $"WHERE CODGRUASS = {dataTable.Rows[index]["CODGRUASS"]} " +
                          $"AND CODQUACON = {dataTable.Rows[index]["CODQUACON"]} " +
                          $"AND {DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable.Rows[index]["DAL"].ToString()))} BETWEEN DATINI AND VALUE(DATFIN,'9999-12-31')";
            string strSQL4 = dataTable.Rows[index]["ETA65"].ToString() != "N" 
                ? $"{str1} AND CODFORASS NOT IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS IN ('PREV', 'FAP', 'TFR'))" 
                : $"{str1} AND CODFORASS NOT IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS IN ('FAP', 'TFR'))";
            flag = db.WriteTransactionData(strSQL4, CommandType.Text);
            Trace = "alisos 3";
            if (flag)
            {
              string str2 = "INSERT INTO DENDETALI(CODPOS, ANNDEN, MESDEN, PRODEN, MAT, PRODENDET, CODFORASS, ALIQUOTA, IMPCON, ULTAGG,UTEAGG) " +
                            $"SELECT {CODPOS} AS CODPOS, {ANNDEN} AS ANNDEN, {MESDEN} AS MESDEN, {PRODEN} AS PRODEN, {dataTable.Rows[index]["MAT"]} AS MAT, {dataTable.Rows[index]["PRODENDET"]} AS PRODENDET, CODFORASS, ";
              string strSQL5 = dataTable.Rows[index]["FAP"].ToString() != "S"
                  ? $"{str2} ALIQUOTA, ROUND(((ALIQUOTA * {dataTable.Rows[index]["IMPRET"].ToString().Replace(",", ".")}) / 100), 2) AS IMPCON, "
                  : $"{str2} ALIQUOTA + {perFap.ToString().Replace(",", ".")}, ROUND(((ALIQUOTA + {perFap.ToString().Replace(",", ".")}) * {dataTable.Rows[index]["IMPRET"].ToString().Replace(",", ".")} / 100), 2) AS IMPCON, ";
              strSQL5 += $"CURRENT_TIMESTAMP, {DBMethods.DoublePeakForSql(utente.Username)} AS UTEAGG FROM ALIFORASS " +
                         $"WHERE CODGRUASS = {dataTable.Rows[index]["CODGRUASS"]} " +
                         $"AND CODQUACON = {dataTable.Rows[index]["CODQUACON"]} " +
                         $"AND {DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable.Rows[index]["DAL"].ToString()))} BETWEEN DATINI AND VALUE(DATFIN,'9999-12-31') " +
                         $"AND CODFORASS IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS = 'TFR')";
              flag = db.WriteTransactionData(strSQL5, CommandType.Text);
              Trace = "alisos 4";
              string strSQL6 = "SELECT SUM(IMPCON) FROM DENDETALI " +
                               $"WHERE CODPOS = {CODPOS} " +
                               $"AND ANNDEN = {ANNDEN} " +
                               $"AND MESDEN = {MESDEN} " +
                               $"AND PRODEN = {PRODEN} " +
                               $"AND MAT = {dataTable.Rows[index]["MAT"]} " +
                               $"AND PRODENDET = {dataTable.Rows[index]["PRODENDET"]} " +
                               "GROUP BY MAT, PRODENDET";
              decimal decDiff = Convert.ToDecimal("0" + db.Get1ValueFromSQL(strSQL6, CommandType.Text)) - 
                                Convert.ToDecimal(dataTable.Rows[index]["IMPCON"]);
              Trace = "alisos 5";
              if (decDiff != 0M)
              {
                if (decDiff <= 0.01M & decDiff >= -0.01M)
                { 
                  decDiff *= -1M;
                  string strSQL7 = "UPDATE DENDETALI SET " +
                                   $"IMPCON = IMPCON + {decDiff.ToString().Replace(",", ".")} " +
                                   $"WHERE CODPOS = {CODPOS} " +
                                   $"AND ANNDEN = {ANNDEN} " +
                                   $"AND MESDEN = {MESDEN} " +
                                   $"AND PRODEN = {PRODEN} " +
                                   $"AND MAT = {dataTable.Rows[index]["MAT"]} " +
                                   $"AND PRODENDET = {dataTable.Rows[index]["PRODENDET"]} " +
                                   $"AND CODFORASS = (SELECT MAX(CODFORASS) FROM DENDETALI " +
                                   $"WHERE CODPOS = {CODPOS} " +
                                   $"AND ANNDEN = {ANNDEN} " +
                                   $"AND MESDEN = {MESDEN} " +
                                   $"AND PRODEN = {PRODEN} " +
                                   $"AND MAT = {dataTable.Rows[index]["MAT"]} " +
                                   $"AND PRODENDET = {dataTable.Rows[index]["PRODENDET"]})";
                  flag = db.WriteTransactionData(strSQL7, CommandType.Text);
                  Trace = "alisos 6";
                }
                else
                  flag = false;
              }
            }
          }
          if (!flag)
            break;
        }
        dataTable.Dispose();
        return flag;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string WRITE_INSERT_MOVIMSAP(
      DataLayer db,
      OCM.Utente.Utente utente,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      string CODCAU,
      string DATA,
      int ANNO_BILANCIO,
      string DATSCA,
      decimal IMPORTO,
      decimal IMPABB,
      decimal IMPADD,
      decimal IMPASSCON,
      string SANZIONE,
      string ANNULLAMENTO,
      string TIPO_OPERAZIONE,
      ref string PARTITAMOV,
      ref int PROGMOV,
      ref List<DatiContabilita> datiContabilita)
    {
      try
      {
        //iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
        string ANNO_PRECEDENTE1 = (string) null;
        //DataTable dataTable1 = new DataTable();
        string tipoAnno = default;
        bool blnRet = false;
        string dataSistema = DateTime.Now.ToString("d");
        string dataReg = DBMethods.Db2Date(dataSistema).Replace("-", "");
        string dataMov = !(TIPO_OPERAZIONE == "DIPA") 
            ? DBMethods.Db2Date(dataSistema).Replace("-", "") 
            : DBMethods.Db2Date(DATSCA).Replace("-", "");
        string str4 = dataMov;
        string dataScad;
        DateTime dateTime;
        if (SANZIONE == "S")
          dataScad = DBMethods.Db2Date(dataSistema).Replace("-", "");
        else if (TIPO_OPERAZIONE == "DIPA")
          dataScad = dataMov;
        else if (ANNULLAMENTO == "S")
        {
          dataScad = 0.ToString();
        }
        else
        {
            dataScad = Convert.ToDateTime(dataSistema).AddDays(30).ToString("d");
            dataScad = DBMethods.Db2Date(dataScad).Replace("-", "");
        }
        string partita = GET_PARTITA(db, utente, CODPOS, ANNDEN, MESDEN, ref blnRet);
        
        if (!blnRet)
          return "";
        
        string strSQL1 = $"SELECT VALUE(MAX(PROGMOV), 0) + 1 FROM MOVIMSAP WHERE PARTITA = '{partita}'";

        int newProgMov = Convert.ToInt32(db.Get1ValueFromSQL(strSQL1, CommandType.Text));
        
        string annoEser = ANNO_BILANCIO < ANNDEN ? ANNDEN.ToString() : ANNO_BILANCIO.ToString();
        
        dateTime = Convert.ToDateTime(dataSistema);
        string annoRif = dateTime.Year.ToString();
        
        string strSQL2 = "SELECT VALUE(MAX(NUMERORIF), 0) + 1 FROM MOVIMSAP " +
                         $"WHERE CODPOSIZ = {CODPOS} " +
                         $"AND ANNORIF = {annoRif} " + 
                         $"AND CODCAUS = {DBMethods.DoublePeakForSql(CODCAU.Trim())}";

        int newNumeroRif = Convert.ToInt32(db.Get1ValueFromSQL(strSQL2, CommandType.Text));

        string numMovSap = GetNumMovSap(newNumeroRif, CODCAU.ToString().Trim(), Convert.ToInt32(annoRif));

        string codLine = $"{CODPOS.ToString().PadLeft(6, '0')}{numMovSap.Substring(5, 2)}{numMovSap.Substring(0, 2)}{numMovSap.Substring(8).PadLeft(3, '0')}";
        
        string str10 = "INSERT INTO MOVIMSAP (PARTITA, PROGMOV, CODCAUS, DATAMOV, DATAREG, IMPORTO, DATACOM, DATASAP, DATASCAD, ANNOESER, NOTE, DECODMOV, CODPOSIZ, ANNORIF, NUMERORIF, PROGRIF, TIPORIF, POSTRASF, VERSTRASF, STATOVAL, STATOSAP, STATOSED, IMPRIDUZ, IMPABB, IMPRESID, CODELMAV, DTHHMAV, FLAGGRP, ANNOGRP, NUMGRP, USERID, DTDECOD, STATOABB) " +
                       $"VALUES ('{partita}', {newProgMov}, {DBMethods.DoublePeakForSql(CODCAU.Trim().PadLeft(2, '0'))}, " +
                       $"{Convert.ToDecimal(dataMov.Replace("'", ""))}, " +
                       $"{Convert.ToDecimal(dataReg.Replace("'", ""))}, " +
                       $"{IMPORTO.ToString().Replace(",", ".")}, " +
                       $"{Convert.ToDecimal(str4.Replace("'", ""))}, " +
                       $"0, " +
                       $"{Convert.ToDecimal(dataScad.Replace("'", ""))}, " +
                       $"{annoEser}, " +
                       $" '', ";
        
        //decimal num1 = Convert.ToDecimal(dataMov.Replace("'", ""));
        //string str11 = num1.ToString();
        //string str12 = str10 + str11 + ", ";
        //num1 = Convert.ToDecimal(dataReg.Replace("'", ""));
        //string str13 = num1.ToString();
        //string str14 = str12 + str13 + ", " + IMPORTO.ToString().Replace(",", ".") + ", ";
        //num1 = Convert.ToDecimal(str4.Replace("'", ""));
        //string str15 = num1.ToString();
        //string str16 = str14 + str15 + ", " + " 0, ";
        //num1 = Convert.ToDecimal(dataScad.Replace("'", ""));
        //string str17 = num1.ToString();
        //string str18 = str16 + str17 + ", " + annoEser + ", " + " '', ";
        string strAppo = db.Get1ValueFromSQL($"SELECT DESCAU FROM TIPMOVCAU WHERE CODCAU = '{CODCAU.Trim().PadLeft(2, '0')}'", CommandType.Text).Trim() + 
                       $" ({DBMethods.GetMesi()[MESDEN]} {ANNDEN})";
        if (strAppo.Length > 50)
          strAppo = strAppo.Substring(0, 50);
        //string str20 = str18 + DBMethods.DoublePeakForSql(strAppo) + ", " + CODPOS.ToString() + ", " + annoRif + ", " + newNumeroRif.ToString() + ", " + " 0, " + DBMethods.DoublePeakForSql(CODCAU.Trim().PadLeft(2, '0')) + ", " + " 0, " + " '', " + " 'V'," + " ''," + " 'V'," + " 0, " + " 0, " + IMPORTO.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(codLine) + ", " + " 0," + " 'N'," + " 0, " + " 0, " + DBMethods.DoublePeakForSql(utente.Username) + ", ";
        string str20 = $"{str10} {DBMethods.DoublePeakForSql(strAppo)}, " +
                       $"{CODPOS}, " +
                       $"{annoRif}, " +
                       $"{newNumeroRif}, " +
                       $"0, " + 
                       $"{DBMethods.DoublePeakForSql(CODCAU.Trim().PadLeft(2, '0'))}, " +
                       $"0, '', 'V', '', 'V', 0, 0, " + 
                       $"{IMPORTO.ToString().Replace(",", ".")}, " + 
                       $"{DBMethods.DoublePeakForSql(codLine)}, " +
                       $"0, 'N', 0, 0, " + 
                       $"{DBMethods.DoublePeakForSql(utente.Username)}, ";
        strAppo = DBMethods.Db2Date(dataSistema).Replace("-", "");
        //string str22 = str20;
        //num1 = Convert.ToDecimal(strAppo.Replace("'", ""));
        //string str23 = num1.ToString();
        string strSQL3 = $"{str20}{Convert.ToDecimal(strAppo.Replace("'", ""))}, '')";
        
        blnRet = db.WriteTransactionData(strSQL3, CommandType.Text);
        
        if (!blnRet)
        {
          numMovSap = "";
          blnRet = false;
        }

        if (blnRet)
        {
          if (SANZIONE != "S")
          {
            int moltiplicatore = !(IMPORTO * -1M < 0M) ? -1 : 1;
            decimal importoTotaleSplimPosAp = IMPORTO * -1M;
            blnRet = Convert.ToBoolean(WRITE_INSERT_SPLIMPOSAP(db, utente, partita, newProgMov, DATA, "TOTDIS", "", IMPORTO * -1M, "", ""));
            
            for (int index1 = 0; index1 <= datiContabilita.Count - 1; ++index1)
            {
              string tipMov = datiContabilita[index1].TipMov.Trim();
              if (!(tipMov == "AR"))
              {
                if (tipMov == "DP")
                { 
                  tipoAnno = GetTipoAnno(ANNDEN, Convert.ToInt32(datiContabilita[index1].AnnBil));
                  string strSQL4 = $"UPDATE DENTES SET " +
                                   $"TIPANNMOV = {DBMethods.DoublePeakForSql(tipoAnno)} " +
                                   $"WHERE CODPOS = {CODPOS} " +
                                   $"AND ANNDEN = {ANNDEN} " +
                                   $"AND MESDEN = {MESDEN} " +
                                   $"AND PRODEN = {PRODEN}";
                  blnRet = db.WriteTransactionData(strSQL4, CommandType.Text);
                }
              }
              else
              { 
                tipoAnno = GetTipoAnno(Convert.ToInt32(datiContabilita[index1].AnnCom), Convert.ToInt32(datiContabilita[index1].AnnBil));
                string strSQL5 = $"UPDATE DENDET SET " +
                                 $"TIPANNMOVARR = {DBMethods.DoublePeakForSql(tipoAnno)} " +
                                 $"WHERE CODPOS = {CODPOS} " +
                                 $"AND ANNDEN = {ANNDEN} " +
                                 $"AND MESDEN = {MESDEN} " +
                                 $"AND PRODEN = {PRODEN} " +
                                 $"AND ANNCOM = {Convert.ToInt32(datiContabilita[index1].AnnCom)}";
                blnRet = db.WriteTransactionData(strSQL5, CommandType.Text);
              }
              string ANNO_PRECEDENTE2 = !(tipoAnno == "AC") ? "S" : "N";
              if (blnRet & Convert.ToDecimal(datiContabilita[index1].ImpAbb) != 0M)
              {
                importoTotaleSplimPosAp += Convert.ToDecimal(datiContabilita[index1].ImpAbb) * moltiplicatore;
                blnRet = Convert.ToBoolean(WriteDIPA_SalvataggioTotale.WRITE_INSERT_SPLIMPOSAP(db, utente, partita, newProgMov, DATA, "ABBPRE", "", Convert.ToDecimal(datiContabilita[index1].ImpAbb) * moltiplicatore, "N", ANNO_PRECEDENTE2));
              }
              if (blnRet & Convert.ToDecimal(datiContabilita[index1].ImpAddRec) != 0M)
              {
                importoTotaleSplimPosAp += Convert.ToDecimal(datiContabilita[index1].ImpAddRec) * moltiplicatore;
                blnRet = Convert.ToBoolean(WriteDIPA_SalvataggioTotale.WRITE_INSERT_SPLIMPOSAP(db, utente, partita, newProgMov, DATA, "ADDIZIONALE", "", Convert.ToDecimal(datiContabilita[index1].ImpAddRec) * moltiplicatore, "N", ANNO_PRECEDENTE2));
              }
              if (blnRet & Convert.ToDecimal(datiContabilita[index1].ImpAssCon) != 0M)
              {
                importoTotaleSplimPosAp += Convert.ToDecimal(datiContabilita[index1].ImpAssCon) * moltiplicatore;
                blnRet = Convert.ToBoolean(WriteDIPA_SalvataggioTotale.WRITE_INSERT_SPLIMPOSAP(db, utente, partita, newProgMov, DATA, "ASSCON", "", Convert.ToDecimal(datiContabilita[index1].ImpAssCon) * moltiplicatore, "N", ANNO_PRECEDENTE2));
              }
              if (blnRet)
              {
                //DataTable dataTable2 = new DataTable();
                string str25 = "SELECT SUM(IMPCON) AS IMPORTO, (SELECT CATFORASS FROM FORASS WHERE CODFORASS = DENDETALI.CODFORASS) AS CATFORASS FROM DENDETALI ";
                
                if (datiContabilita[index1].TipMov.Trim() == "AR")
                  str25 = $"{str25} INNER JOIN (SELECT CODPOS, ANNDEN, MESDEN, PRODEN, MAT, PRODENDET FROM DENDET WHERE CODPOS = {CODPOS} AND ANNDEN = {ANNDEN} AND MESDEN = {MESDEN} AND PRODEN = {PRODEN} AND ANNCOM = {Convert.ToInt32(datiContabilita[index1].AnnCom)}) AS A " +
                          "ON DENDETALI.CODPOS = A.CODPOS AND DENDETALI.ANNDEN = A.ANNDEN AND DENDETALI.MESDEN = A.MESDEN AND DENDETALI.PRODEN = A.PRODEN AND DENDETALI.MAT = A.MAT AND DENDETALI.PRODENDET = A.PRODENDET";
                
                string strSQL6 = $"{str25} WHERE DENDETALI.CODPOS = {CODPOS} AND DENDETALI.ANNDEN = {ANNDEN} AND DENDETALI.MESDEN = {MESDEN} AND DENDETALI.PRODEN = {PRODEN} GROUP BY (SELECT CATFORASS FROM FORASS WHERE CODFORASS = DENDETALI.CODFORASS)";
                DataTable dataTable3 = db.GetDataTable(strSQL6);
                
                for (int index2 = 0; index2 <= dataTable3.Rows.Count - 1; ++index2)
                {
                  if (Convert.ToDecimal(dataTable3.Rows[index2]["IMPORTO"]) > 0M)
                  {
                    importoTotaleSplimPosAp += Convert.ToDecimal(Convert.ToDecimal(dataTable3.Rows[index2]["IMPORTO"]) * moltiplicatore);
                    blnRet = Convert.ToBoolean(WRITE_INSERT_SPLIMPOSAP(db, utente, partita, newProgMov, DATA, "", dataTable3.Rows[index2]["CATFORASS"].ToString(), Convert.ToDecimal(dataTable3.Rows[index2][nameof (IMPORTO)]) * moltiplicatore, "N", ANNO_PRECEDENTE2));
                  }
                }
              }
            }
            if (importoTotaleSplimPosAp != 0M)
              blnRet = false;
          }
          else
          {
            blnRet = Convert.ToBoolean(WriteDIPA_SalvataggioTotale.WRITE_INSERT_SPLIMPOSAP(db, utente, partita, newProgMov, DATA, "TOTSAN", "", IMPORTO * -1M, "", ""));
            if (blnRet)
            {
              string tipMovSan = db.Get1ValueFromSQL($"SELECT TIPMOV FROM TIPMOVCAU WHERE CODCAU = {DBMethods.DoublePeakForSql(CODCAU)}", CommandType.Text);
              blnRet = Convert.ToBoolean(WriteDIPA_SalvataggioTotale.WRITE_INSERT_SPLIMPOSAP(db, utente, partita, newProgMov, DATA, tipMovSan, "", IMPORTO, "N", ANNO_PRECEDENTE1));
            }
          }
        }
        if (!blnRet)
          return "";
        PARTITAMOV = partita;
        PROGMOV = newProgMov;
        return numMovSap;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string GET_PARTITA(
      DataLayer db,
      OCM.Utente.Utente utente,
      int CODPOS,
      int ANNO,
      int MESE,
      ref bool blnRet)
    {
      iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
      string partita = default;
      string strData = DateTime.Now.ToString("d");
      DataTable dataTable1 = new DataTable();
      blnRet = true;
      string meseDist = MESE.ToString().PadLeft(2, '0');
      try
      {
        string strSQL1 = ANNO >= 2003 
            ? $"SELECT * FROM PARTSAP WHERE CODPOSIZ = {CODPOS} AND ANNODIST = {ANNO} AND MESEDIST = {meseDist}"
            : $"SELECT * FROM PARTSAP WHERE PARTITA = {DBMethods.DoublePeakForSql(CODPOS.ToString().Trim().PadLeft(6, '0') + "000000")}";
        
        DataTable dataTable2 = db.GetDataTable(strSQL1);

        if (dataTable2.Rows.Count > 0)
        {
          if (dataTable2.Rows[0]["STATO"].ToString() == "C")
          {
            //string str3 = MESE.ToString().PadLeft(2, '0');
            string strSQL2 = "UPDATE PARTSAP  SET STATO = 'A' " +
                             $"WHERE  CODPOSIZ = {CODPOS} " + 
                             $"AND ANNODIST = {ANNO} " + 
                             $"AND MESEDIST = {meseDist} " + 
                             $"AND PARTITA = {DBMethods.DoublePeakForSql(dataTable2.Rows[0]["PARTITA"].ToString().Trim())}";
            
            blnRet = db.WriteTransactionData(strSQL2, CommandType.Text);
            
            if (blnRet)
            {
              partita = dataTable2.Rows[0]["PARTITA"].ToString().Trim();
              string strSQL3 = $"UPDATE DTPARTSAP SET DATACL = 0 WHERE PARTITA = {DBMethods.DoublePeakForSql(dataTable2.Rows[0]["PARTITA"].ToString())}";
              blnRet = db.WriteTransactionData(strSQL3, CommandType.Text);
            }
          }
          else
            partita = dataTable2.Rows[0]["PARTITA"].ToString().Trim();
        }
        else
        {
          partita = ANNO >= 2003 
              ? $"{CODPOS.ToString().Trim().PadLeft(6, '0')}{ANNO.ToString().Trim().Substring(2)}{MESE.ToString().PadLeft(2, '0')}01" 
              : $"{CODPOS.ToString().Trim().PadLeft(6, '0')}000000";

          string strSQL4 = "INSERT INTO PARTSAP (CODPOSIZ, ANNODIST, MESEDIST, PARTITA, STATO, USERID) " +
                           $"VALUES ({CODPOS}, {ANNO}, {DBMethods.DoublePeakForSql(meseDist)}, {DBMethods.DoublePeakForSql(partita.Trim())}, 'A', {DBMethods.DoublePeakForSql(utente.Username)})";
          
          blnRet = db.WriteTransactionData(strSQL4, CommandType.Text);
          
          if (blnRet)
          {
            string strSQL5 = "INSERT INTO DTPARTSAP (PARTITA, DATAAP, DATACL, USERID) " +
                             $"VALUES ({DBMethods.DoublePeakForSql(partita.Trim())}, {DBMethods.Db2Date(strData).Replace("-", "")}, 0, {DBMethods.DoublePeakForSql(utente.Username)})";
            
            blnRet = db.WriteTransactionData(strSQL5, CommandType.Text);
          }
        }
        return partita;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string GetNumMovSap(int NUMERO_RIF, string CODCAU, int ANNO) => $"{CODCAU.PadLeft(2, '0')}/{ANNO}/{NUMERO_RIF.ToString().Trim()}";

    public static string WRITE_INSERT_SPLIMPOSAP(
      DataLayer db,
      OCM.Utente.Utente utente,
      string PARTITA,
      int PROGMOV,
      string DATA,
      string CATIMP,
      string CATFORASS,
      decimal IMPORTO,
      string ANNULLAMENTO,
      string ANNO_PRECEDENTE)
    {
      DataTable dataTable1 = new DataTable();
      try
      {
        string strSQL1 = $"SELECT VALUE(MAX(PROGSPLI), 0) + 1 AS PROGSPLI FROM SPLIMPOSAP  WHERE PARTITA = '{PARTITA}' AND PROGMOV = {PROGMOV}";
        int progSpli = Convert.ToInt32(db.Get1ValueFromSQL(strSQL1, CommandType.Text));
        string strSQL2 = $"SELECT CODCON, GESCON, GESCON1 FROM CONTI WHERE {DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATA))} BETWEEN DATINI AND DATFIN";
        
        if (!string.IsNullOrEmpty(ANNULLAMENTO))
          strSQL2 = $"{strSQL2} AND MOVANN = {DBMethods.DoublePeakForSql(ANNULLAMENTO)}";

        if (!string.IsNullOrEmpty(CATIMP))
          strSQL2 = $"{strSQL2} AND CATIMP = {DBMethods.DoublePeakForSql(CATIMP)}";
        
        if (!string.IsNullOrEmpty(CATFORASS))
          strSQL2 = $"{strSQL2} AND CATFORASS = {DBMethods.DoublePeakForSql(CATFORASS)}";
        
        if (!string.IsNullOrEmpty(ANNO_PRECEDENTE))
          strSQL2 = $"{strSQL2} AND ANNPRE = {DBMethods.DoublePeakForSql(ANNO_PRECEDENTE)}";
        
        DataTable dataTable2 = db.GetDataTable(strSQL2);
        
        if (dataTable2.Rows.Count == 0)
          throw new Exception("Attenzione... Impossibile trovare il conto, per lo splittamento, nella tabella CONTI! La transazione sarà annullata. Analizzare la query: " + strSQL2);
        
        string codCon = dataTable2.Rows[0]["CODCON"].ToString().Trim();
        string gesCon = dataTable2.Rows[0]["GESCON"].ToString().Trim();
        string gesCon1 = dataTable2.Rows[0]["GESCON1"].ToString().Trim();
        
        string strSQL3 = "SELECT VALUE(COUNT(*), 0) FROM SPLIMPOSAP " +
                         $"WHERE PARTITA = {DBMethods.DoublePeakForSql(PARTITA)} " +
                         $"AND PROGMOV = {PROGMOV} " +
                         $"AND GESTCON = {DBMethods.DoublePeakForSql(gesCon)}";
        bool flag;
        if (Convert.ToInt32(db.Get1ValueFromSQL(strSQL3, CommandType.Text)) == 0)
        {
          string str4 = "INSERT INTO SPLIMPOSAP (PARTITA, PROGMOV, PROGSPLI, CODCONTO, GESTCON, IMPORTO, IMPRIDU, IMPABB, IMPRES, STATOSAP, USERID, GESTCON1, FLSAP) " +
                        $"VALUES ('{PARTITA}', {PROGMOV}, {progSpli}, {DBMethods.DoublePeakForSql(codCon)}, {DBMethods.DoublePeakForSql(gesCon)}, {IMPORTO.ToString().Trim().Replace(",", ".")}, " +
                        $"0, 0, {IMPORTO.ToString().Trim().Replace(",", ".")}, '', {DBMethods.DoublePeakForSql(utente.Username)}, ";
          
          string str5 = !string.IsNullOrEmpty(gesCon1) 
              ? $"{str4}{DBMethods.DoublePeakForSql(gesCon1)}, " 
              : $"{str4}'', ";
          
          string strSQL4 = !(gesCon == "CAZA" | gesCon == "CSAA") 
              ? $"{str5}'N')" 
              : $"{str5}'S')";

          flag = db.WriteTransactionData(strSQL4, CommandType.Text);
        }
        else
        {
          string strSQL5 = $"UPDATE SPLIMPOSAP SET " +
                           $"IMPORTO = (IMPORTO + {IMPORTO.ToString().Replace(",", ".")}), " +
                           $"IMPRES = (IMPRES + {IMPORTO.ToString().Replace(",", ".")}) " +
                           $"WHERE PARTITA = {DBMethods.DoublePeakForSql(PARTITA)} " +
                           $"AND PROGMOV = {PROGMOV} " +
                           $"AND GESTCON = {DBMethods.DoublePeakForSql(gesCon)}";

          flag = db.WriteTransactionData(strSQL5, CommandType.Text);
        }

        return flag.ToString();
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static int WRITE_INSERT_DIPATES(DataLayer db, int IDDIPA, int ANNFIA, TFI.OCM.Utente.Utente utente)
    {
      string strSQL1 = " SELECT VALUE(MAX(IDDIPA), 0) + 1  FROM FIACONSIP.DIPATES";
      int int32 = Convert.ToInt32(db.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string str = "01/" + ANNFIA.ToString() + "/" + int32.ToString();
      string strSQL2 = " INSERT INTO FIACONSIP.DIPATES SELECT " + int32.ToString() + ", CODPOS, ANNDEN, MESDEN, PRODEN, ANNFIA, MESFIA, VERSANTE, IDAZI, IDISC, CAUSALE, IMPDISSAN, DATCONMOV, " + DBMethods.DoublePeakForSql(str) + ", DATSCA, " + " CODMODPAG, IMPVER, DATVER, NUMMAV, " + DBMethods.DoublePeakForSql(DateTime.Now.ToString("d")) + ", " + DBMethods.DoublePeakForSql(utente.Username) + ", ANNBILMOV, " + " CODLINE, IBAN, NUMMOVANN, DATMOVANN, NUMRIC, DATCHI, IMPCOSMAV, '' " + " FROM FIACONSIP.DIPATESTMP WHERE IDDIPA = " + IDDIPA.ToString();
      if (!db.WriteTransactionData(strSQL2, CommandType.Text))
        return 0;
      string strSQL3 = "SELECT DATSCA FROM FIACONSIP.DIPATESTMP WHERE IDDIPA = " + IDDIPA.ToString();
      string strData = db.Get1ValueFromSQL(strSQL3, CommandType.Text) ?? "";
      if (string.IsNullOrEmpty(strData))
        return 0;
      string strSQL4 = "UPDATE FIACONSIP.DIPATES SET DATSCA = '" + DBMethods.Db2Date(strData) + "'" + " WHERE IDDIPA = " + int32.ToString();
      if (!db.WriteTransactionData(strSQL4, CommandType.Text))
        return 0;
      string causaleDistinta = WriteDIPA_SalvataggioTotale.FIA_GET_CAUSALE_DISTINTA(db, int32);
      if (string.IsNullOrEmpty(causaleDistinta))
        return 0;
      string strSQL5 = "UPDATE FIACONSIP.DIPATES SET CAUSALE = " + DBMethods.DoublePeakForSql(causaleDistinta) + " WHERE IDDIPA = " + int32.ToString();
      if (!db.WriteTransactionData(strSQL5, CommandType.Text))
        return 0;
      string strSQL6 = " DELETE FROM FIACONSIP.DIPATESTMP WHERE IDDIPA = " + IDDIPA.ToString();
      return db.WriteTransactionData(strSQL6, CommandType.Text) ? int32 : 0;
    }

    public static string FIA_GET_CAUSALE_DISTINTA(DataLayer db, int IDDIPA)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL = "SELECT D.IDAZI, D.IDISC, A.IDTIPPAG AS IDTIPPAGAZI, D.VERSANTE, D.MESFIA, D.ANNFIA FROM FIACONSIP.DIPATES D LEFT JOIN FIACONSIP.AZI A ON D.IDAZI=A.IDAZI WHERE D.IDDIPA = " + IDDIPA.ToString();
      DataTable dataTable2 = db.GetDataTable(strSQL);
      if (dataTable2.Rows.Count <= 0)
        return "";
      switch (Convert.ToInt32(dataTable2.Rows[0]["IDTIPPAGAZI"]))
      {
        case 1:
          return "PAGAMENTO DISTINTA ANNO " + dataTable2.Rows[0]["ANNFIA"]?.ToString();
        case 2:
          return Convert.ToInt32(dataTable2.Rows[0]["MESFIA"]) < 6 ? "PAGAMENTO DISTINTA PRIMO SEMESTRE " + dataTable2.Rows[0]["ANNFIA"]?.ToString() : "PAGAMENTO DISTINTA SECONDO SEMESTRE " + dataTable2.Rows[0]["ANNFIA"]?.ToString();
        case 12:
          return "PAGAMENTO DISTINTA MESE " + DBMethods.GetMesi()[Convert.ToInt32(dataTable2.Rows[0]["MESFIA"])] + " " + dataTable2.Rows[0]["ANNFIA"]?.ToString();
        default:
          return (string) null;
      }
    }

    public static bool WRITE_INSERT_DIPADET(
      DataLayer db,
      int IDDIPANEW,
      int IDDIPA,
      int ANNFIA,
      TFI.OCM.Utente.Utente utente)
    {
      DataTable dataTable1 = new DataTable();
      bool flag = false;
      string strSQL1 = " SELECT * FROM FIACONSIP.DIPADETTMP WHERE IDDIPA = " + IDDIPA.ToString();
      DataTable dataTable2 = db.GetDataTable(strSQL1);
      string str = "01/" + ANNFIA.ToString() + "/" + IDDIPANEW.ToString();
      int num = dataTable2.Rows.Count - 1;
      for (int index = 0; index <= num; ++index)
      {
        string strSQL2 = " SELECT VALUE(MAX(IDDIPADET), 0) + 1  FROM FIACONSIP.DIPADET";
        int int32 = Convert.ToInt32(db.Get1ValueFromSQL(strSQL2, CommandType.Text));
        string strSQL3 = "INSERT INTO FIACONSIP.DIPADET (" + " IDDIPADET, IDDIPA,\tCODPOS,\tANNDEN,\tMESDEN,\tPRODEN, " + " PRODENDET, MAT, ANNFIA, MESFIA, VERSANTE, IDAZI, IDISC, IDADE, " + " IDPOL, IMPSAN, ULTAGG, UTEAGG, NUMMOV, DATCONMOV, IMPABB, IMPDAABB) VALUES (" + int32.ToString() + ", " + IDDIPANEW.ToString() + ", " + dataTable2.Rows[index]["CODPOS"]?.ToString() + ", " + dataTable2.Rows[index]["ANNDEN"]?.ToString() + ", " + dataTable2.Rows[index]["MESDEN"]?.ToString() + ", " + dataTable2.Rows[index]["PRODEN"]?.ToString() + ", " + dataTable2.Rows[index]["PRODENDET"]?.ToString() + ", " + dataTable2.Rows[index]["MAT"]?.ToString() + ", " + dataTable2.Rows[index][nameof (ANNFIA)]?.ToString() + ", " + dataTable2.Rows[index]["MESFIA"]?.ToString() + ", " + DBMethods.DoublePeakForSql(dataTable2.Rows[index]["VERSANTE"].ToString()) + ", " + dataTable2.Rows[index]["IDAZI"]?.ToString() + ", " + dataTable2.Rows[index]["IDISC"]?.ToString() + ", " + dataTable2.Rows[index]["IDADE"]?.ToString() + ", " + dataTable2.Rows[index]["IDPOL"]?.ToString() + ", " + dataTable2.Rows[index]["IMPSAN"].ToString().Replace(",", ".") + ", CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(utente.Username) + ", " + DBMethods.DoublePeakForSql(str) + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable2.Rows[index]["DATCONMOV"].ToString())) + ", " + dataTable2.Rows[index]["IMPABB"].ToString().Replace(",", ".") + ", " + dataTable2.Rows[index]["IMPDAABB"].ToString().Replace(",", ".") + ")";
        flag = db.WriteTransactionData(strSQL3, CommandType.Text);
        if (flag)
        {
          string strSQL4 = "SELECT COUNT(*) FROM FIACONSIP.DIPADET D, FIACONSIP.ADESIONI A WHERE D.IDDIPADET = " + int32.ToString() + " AND A.IDAZI = D.IDAZI AND A.IDADE = D.IDADE AND A.IDISC = D.IDISC AND A.MAT = D.MAT AND A.CODPOS = D.CODPOS";
          if (Convert.ToInt32("0" + db.Get1ValueFromSQL(strSQL4, CommandType.Text)) == 0)
          {
            flag = false;
            break;
          }
        }
        else
          break;
      }
      if (flag)
      {
        string strSQL5 = " DELETE FROM FIACONSIP.DIPADETTMP WHERE IDDIPA = " + IDDIPA.ToString();
        flag = db.WriteTransactionData(strSQL5, CommandType.Text);
      }
      return flag;
    }

    public static bool AGGIORNA_RETANN(
      DataLayer db,
      OCM.Utente.Utente utente,
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
      bool flag1 = false;
      string datIni = (string) null;
      string datFin = (string) null;
      DataTable dataTable1 = new DataTable();
      try
      {
        bool flag2;
        if (FLGARR)
        {
          string strSQL1 = $"SELECT COUNT(*) FROM RETANN " +
                           $"WHERE CODPOS = {CODPOS} " +
                           $"AND MAT = {MAT} " +
                           $"AND PRORAP = {PRORAP} " +
                           $"AND ANNRET = {ANNO}";
          if (Convert.ToInt32("0" + db.Get1ValueFromSQL(strSQL1, CommandType.Text)) == 0)
          {
            string strSQL2 = $"SELECT DATDEC, DATCES FROM RAPLAV " +
                             $"WHERE CODPOS = {CODPOS} " +
                             $"AND MAT = {MAT} " +
                             $"AND PRORAP = {PRORAP}";
            DataTable dataTable2 = db.GetDataTable(strSQL2);
            if (dataTable2.Rows.Count > 0)
            {
              datIni = Convert.ToDateTime(dataTable2.Rows[0]["DATDEC"]).Year != ANNO 
                  ? $"{ANNO}-01-01" 
                  : dataTable2.Rows[0]["DATDEC"].ToString();

              datFin = dataTable2.Rows[0]["DATCES"].ToString().Trim() == "" 
                  ? $"{ANNO}-12-31" 
                  : (Convert.ToDateTime(dataTable2.Rows[0]["DATCES"]).Year != ANNO 
                      ? $"{ANNO}-12-31" 
                      : dataTable2.Rows[0]["DATCES"].ToString());
            }
            string strSQL3 = $"INSERT INTO RETANN (CODPOS, MAT, PRORAP, ANNRET, DATINI, DATFIN, RETIMP, RETOCC, RETFIG, RETUTILE, RETARRIMP, RETARROCC, FLAGRIC, ULTAGG, UTEAGG) " +
                             $"VALUES ({CODPOS}, {MAT}, {PRORAP}, {ANNO}, {AggiungiApici(DBMethods.Db2Date(datIni))}, {AggiungiApici(DBMethods.Db2Date(datFin))}, 0.0, 0.0, 0.0, 0.0, {RETIMP.ToString().Replace(",", ".")}, {RETOCC.ToString().Replace(",", ".")}, 'S', CURRENT_TIMESTAMP, {DBMethods.DoublePeakForSql(utente.Username)})";
            flag2 = db.WriteTransactionData(strSQL3, CommandType.Text);
          }
          else
          {
            string strSQL4 = $"UPDATE RETANN SET " +
                             $"FLAGRIC = 'S', " +
                             $"RETARRIMP = RETARRIMP {OPERAZIONE}{RETIMP.ToString().Replace(",", ".")}, " +
                             $"RETARROCC = RETARROCC {OPERAZIONE}{RETOCC.ToString().Replace(",", ".")}, " +
                             $"ULTAGG = CURRENT_TIMESTAMP, " +
                             $"UTEAGG = {DBMethods.DoublePeakForSql(utente.Username)} " +
                             $"WHERE CODPOS = {CODPOS} " +
                             $"AND MAT = {MAT} " +
                             $"AND PRORAP = {PRORAP} " +
                             $"AND ANNRET = {ANNO}";
            flag2 = db.WriteTransactionData(strSQL4, CommandType.Text);
          }
        }
        else
        {
          string strSQL5 = $"SELECT COUNT(*) FROM RETANN " +
                           $"WHERE CODPOS = {CODPOS} " +
                           $"AND MAT = {MAT} " +
                           $"AND PRORAP = {PRORAP} " +
                           $"AND ANNRET = {ANNO}";

          if (Convert.ToInt32("0" + db.Get1ValueFromSQL(strSQL5, CommandType.Text)) == 0)
          {
            string strSQL6 = $"SELECT DATDEC, DATCES FROM RAPLAV " +
                             $"WHERE CODPOS = {CODPOS} " +
                             $"AND MAT = {MAT} " +
                             $"AND PRORAP = {PRORAP}";
            DataTable dataTable3 = db.GetDataTable(strSQL6);

            if (dataTable3.Rows.Count > 0)
            {
              datIni = Convert.ToDateTime(dataTable3.Rows[0]["DATDEC"]).Year != ANNO 
                  ? $"{ANNO}-01-01" 
                  : dataTable3.Rows[0]["DATDEC"].ToString();

              datFin = !(dataTable3.Rows[0]["DATCES"].ToString().Trim() != "") 
                  ? $"{ANNO}-12-31" 
                  : (Convert.ToDateTime(dataTable3.Rows[0]["DATCES"]).Year != ANNO 
                      ? $"{ANNO}-12-31" 
                      : dataTable3.Rows[0]["DATCES"].ToString());
            }
            string strSQL7 = $"INSERT INTO RETANN (CODPOS, MAT, PRORAP, ANNRET, DATINI, DATFIN, RETIMP, RETOCC, RETFIG, RETUTILE, RETARRIMP, RETARROCC, FLAGRIC, ULTAGG, UTEAGG) " +
                             $"VALUES ({CODPOS}, {MAT}, {PRORAP}, {ANNO}, {AggiungiApici(DBMethods.Db2Date(datIni))}, {AggiungiApici(DBMethods.Db2Date(datFin))}, {RETIMP.ToString().Replace(",", ".")}, {RETOCC.ToString().Replace(",", ".")}, {RETFIG.ToString().Replace(",", ".")}, {(RETIMP - RETOCC + RETFIG).ToString().Replace(",", ".")}, 0.0, 0.0, 'S', CURRENT_TIMESTAMP, {DBMethods.DoublePeakForSql(utente.Username)})";
            flag2 = db.WriteTransactionData(strSQL7, CommandType.Text);
          }
          else
          {
            string strSQL8 = "UPDATE RETANN SET " +
                             "FLAGRIC = 'S', " +
                             $"RETIMP = RETIMP {OPERAZIONE}{RETIMP.ToString().Replace(",", ".")}, " +
                             $"RETOCC = RETOCC {OPERAZIONE}{RETOCC.ToString().Replace(",", ".")}, " +
                             $"RETFIG = RETFIG {OPERAZIONE}{RETFIG.ToString().Replace(",", ".")}, " +
                             $"RETUTILE = RETUTILE {OPERAZIONE}{(RETIMP - RETOCC + RETFIG).ToString().Replace(",", ".")}, " +
                             $"ULTAGG = CURRENT_TIMESTAMP, " +
                             $"UTEAGG = {DBMethods.DoublePeakForSql(utente.Username)} " +
                             $"WHERE CODPOS = {CODPOS} " +
                             $"AND MAT = {MAT} " +
                             $"AND PRORAP = {PRORAP} " +
                             $"AND ANNRET = {ANNO}";
            flag2 = db.WriteTransactionData(strSQL8, CommandType.Text);
          }
        }
        return flag2;
      }
      catch (Exception ex)
      {
        return flag1;
      }
    }

    public static bool WRITE_CONTABILITA_DENUNCIA(
      DataLayer db,
      TFI.OCM.Utente.Utente ute,
      List<ParametriGenerali> listaParametriGenerali,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      string TIPMOV,
      Decimal IMPDIS,
      Decimal IMPABB,
      Decimal IMPADDREC,
      Decimal IMPASSCON,
      string PARTITA_MOV = "",
      int PROGMOV_MOV = 0,
      string CODCAUS = "",
      bool SANITARIO = false,
      int IDDIPA = 0)
    {
      DateTimeFormatInfo dateTimeFormat1 = new CultureInfo("it-IT", false).DateTimeFormat;
      string strSQL1 = "";
      int num1 = 0;
      string str1 = "";
      string strData = "";
      DataTable dataTable1 = new DataTable();
      int? outcome = new int?();
      Decimal num2 = 0M;
      Decimal num3 = 0M;
      Decimal num4 = 0M;
      int num5 = 0;
      string str2 = "";
      int num6 = 0;
      string appSetting = ConfigurationManager.AppSettings["prefissoTabelle"];
      DataView dataView = new DataView();
      List<DatiContabilita> datiContabilitaList = new List<DatiContabilita>();
      if (!(TIPMOV == "AR"))
      {
        if (TIPMOV == "DP")
        {
          num1 = DenunciaMensileDAL.GetAnnoBilancio(ANNDEN, out outcome);
          strSQL1 = "SELECT TIPMOV, 0 AS ANNCOM, IMPADDREC, IMPABB, IMPASSCON, " + num1.ToString() + " AS ANNBIL " + "FROM DENTES WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString();
        }
      }
      else
      {
        if (listaParametriGenerali == null || listaParametriGenerali.Count == 0)
          listaParametriGenerali = DenunciaMensileDAL.GetParametriGenerali(CODPOS.ToString(), out outcome);
        listaParametriGenerali = (List<ParametriGenerali>) HttpContext.Current.Session["ListaParGen"];
        List<ParametriGenerali> listaParametriGenerali1 = listaParametriGenerali;
        DateTime today = DateTime.Today;
        string dateTimeFormat2 = today.GetDateTimeFormats((IFormatProvider) dateTimeFormat1)[0];
        Decimal importoParametro = WriteDIPA.GetImportoParametro(listaParametriGenerali1, 5, dateTimeFormat2);
        today = DateTime.Today;
        num1 = DenunciaMensileDAL.GetAnnoBilancio(today.Year, out outcome);
        strSQL1 = "SELECT TIPMOV, ANNCOM, ROUND(((IMPCON / 100) * " + importoParametro.ToString() + "), 2) " + "AS IMPADDREC, IMPABB, IMPASSCON, " + num1.ToString() + " AS ANNBIL FROM DENDET " + "WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " ORDER BY ANNCOM";
      }
      if (TIPMOV == "AR")
      {
        DataTable dataTable2 = db.GetDataTable(strSQL1);
        dataTable2.Clone();
        for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
        {
          if (num5 != Convert.ToInt32(dataTable2.Rows[index]["ANNCOM"]))
          {
            if (num5 != 0)
              datiContabilitaList.Add(new DatiContabilita()
              {
                TipMov = TIPMOV,
                AnnCom = num5,
                ImpAddRec = num2,
                ImpAssCon = num3,
                ImpAbb = num4,
                AnnBil = num1
              });
            num2 = 0M;
            num3 = 0M;
            num4 = 0M;
            num5 = Convert.ToInt32(dataTable2.Rows[index]["ANNCOM"]);
          }
          num2 += Convert.ToDecimal(dataTable2.Rows[index][nameof (IMPADDREC)]);
          num3 += Convert.ToDecimal(dataTable2.Rows[index][nameof (IMPASSCON)]);
          num4 += Convert.ToDecimal(dataTable2.Rows[index][nameof (IMPABB)]);
          if (index == dataTable2.Rows.Count - 1)
            datiContabilitaList.Add(new DatiContabilita()
            {
              TipMov = TIPMOV,
              AnnCom = num5,
              ImpAddRec = num2,
              ImpAssCon = num3,
              ImpAbb = num4,
              AnnBil = num1
            });
        }
      }
      else
      {
        foreach (DataRow row in (InternalDataCollectionBase) db.GetDataTable(strSQL1).Rows)
          datiContabilitaList.Add(new DatiContabilita()
          {
            TipMov = row[nameof (TIPMOV)].ToString(),
            AnnCom = Convert.ToInt32(row["ANNCOM"]),
            ImpAddRec = Convert.ToDecimal(row["decIMPADDREC"]),
            ImpAssCon = Convert.ToDecimal(row["decIMPASSCON"]),
            ImpAbb = Convert.ToDecimal(row["decIMPABB"]),
            AnnBil = Convert.ToInt32(row["ANNBIL"])
          });
      }
      string strSQL2 = "SELECT CODCAU FROM TIPMOVCAU WHERE TIPMOV = " + DBMethods.DoublePeakForSql(TIPMOV) + " AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
      string str3 = db.Get1ValueFromSQL(strSQL2, CommandType.Text);
      string str4 = TIPMOV.Trim();
      if (!(str4 == "DP"))
      {
        if (str4 == "AR")
        {
          string strSQL3 = "SELECT CHAR(DATAPE) FROM DENTES WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString();
          string s = db.Get1ValueFromSQL(strSQL3, CommandType.Text).Substring(0, 10);
          string strSQL4 = "SELECT VALORE FROM PARGENDET WHERE CODPAR = 13 AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date("01/" + MESDEN.ToString() + "/" + ANNDEN.ToString())) + " BETWEEN DATINI AND DATFIN";
          int int16 = (int) Convert.ToInt16("0" + db.Get1ValueFromSQL(strSQL4, CommandType.Text));
          strData = DateTime.Parse(s).AddDays((double) int16).GetDateTimeFormats((IFormatProvider) dateTimeFormat1)[0];
          str1 = "ARRETRATO";
        }
      }
      else
      {
        string strSQL5 = "SELECT VALORE FROM PARGENDET WHERE CODPAR = 3 AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date("01/" + MESDEN.ToString() + "/" + ANNDEN.ToString())) + " BETWEEN DATINI AND DATFIN";
        strData = db.Get1ValueFromSQL(strSQL5, CommandType.Text);
        str1 = "DIPA";
      }
      DataLayer db1 = db;
      TFI.OCM.Utente.Utente utente1 = ute;
      int CODPOS1 = CODPOS;
      int ANNDEN1 = ANNDEN;
      int MESDEN1 = MESDEN;
      int PRODEN1 = PRODEN;
      string CODCAU1 = str3.ToString();
      DateTime today1 = DateTime.Today;
      string dateTimeFormat3 = today1.GetDateTimeFormats((IFormatProvider) dateTimeFormat1)[0];
      int ANNO_BILANCIO1 = num1;
      string DATSCA1 = strData;
      Decimal IMPORTO1 = IMPDIS;
      Decimal IMPABB1 = IMPABB;
      Decimal IMPADD = IMPADDREC;
      Decimal IMPASSCON1 = IMPASSCON;
      string TIPO_OPERAZIONE1 = str1;
      ref string local1 = ref str2;
      ref int local2 = ref num6;
      ref List<DatiContabilita> local3 = ref datiContabilitaList;
      string str5 = WriteDIPA_SalvataggioTotale.WRITE_INSERT_MOVIMSAP(db1, utente1, CODPOS1, ANNDEN1, MESDEN1, PRODEN1, CODCAU1, dateTimeFormat3, ANNO_BILANCIO1, DATSCA1, IMPORTO1, IMPABB1, IMPADD, IMPASSCON1, "N", "N", TIPO_OPERAZIONE1, ref local1, ref local2, ref local3);
      PARTITA_MOV = str2;
      PROGMOV_MOV = num6;
      CODCAUS = str3;
      today1 = DateTime.Today;
      string strSQL6 = "UPDATE DENTES SET FLGAPP = 'I', DATCONMOV = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(today1.ToString())) + ", " + " CODCAUMOV = " + DBMethods.DoublePeakForSql(str3) + ", " + " NUMMOV = " + DBMethods.DoublePeakForSql(str5) + ", " + " DATSCA = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + ", " + " ANNBILMOV = " + num1.ToString() + ", " + " STADEN = 'A', " + " DATORARIC = CURRENT_TIMESTAMP " + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND TIPMOV = " + DBMethods.DoublePeakForSql(TIPMOV);
      bool flag = db.WriteTransactionData(strSQL6, CommandType.Text);
      if (SANITARIO)
      {
        string[] strArray = new string[5]
        {
          "UPDATE ",
          appSetting,
          ".DIPATES SET DATCONMOV = ",
          null,
          null
        };
        today1 = DateTime.Today;
        strArray[3] = DBMethods.DoublePeakForSql(DBMethods.Db2Date(today1.ToString()));
        strArray[4] = ", ";
        string strSQL7 = string.Concat(strArray) + " NUMMOV = " + DBMethods.DoublePeakForSql(str5) + ", " + " DATSCA = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + ", " + " ANNBILMOV = " + num1.ToString() + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND IDDIPA = " + IDDIPA.ToString();
        flag = db.WriteTransactionData(strSQL7, CommandType.Text);
      }
      if (flag)
      {
        today1 = DateTime.Today;
        string strSQL8 = "UPDATE DENDET SET FLGAPP = 'I', DATCONMOV = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(today1.ToString())) + ", " + " NUMMOV = " + DBMethods.DoublePeakForSql(str5) + ", " + " PARTITA = " + DBMethods.DoublePeakForSql(str2) + ", " + " PROGMOV = " + num6.ToString() + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND TIPMOV = " + DBMethods.DoublePeakForSql(TIPMOV);
        flag = db.WriteTransactionData(strSQL8, CommandType.Text);
        if (SANITARIO)
        {
          string[] strArray = new string[5]
          {
            "UPDATE ",
            appSetting,
            ".DIPADET SET DATCONMOV = ",
            null,
            null
          };
          today1 = DateTime.Today;
          strArray[3] = DBMethods.DoublePeakForSql(DBMethods.Db2Date(today1.ToString()));
          strArray[4] = ", ";
          string strSQL9 = string.Concat(strArray) + " NUMMOV = " + DBMethods.DoublePeakForSql(str5) + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND IDDIPA = " + IDDIPA.ToString();
          flag = db.WriteTransactionData(strSQL9, CommandType.Text);
        }
        if (flag)
        {
          string strSQL10 = "SELECT VALUE(IMPSANDET, 0) AS IMPSANDET, VALUE(SANSOTSOG, '') AS SANSOTSOG, VALUE(RICSANUTE, '') " + "AS RICSANUTE FROM DENTES WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND TIPMOV = " + DBMethods.DoublePeakForSql(TIPMOV);
          dataTable1 = db.GetDataTable(strSQL10);
          if (Convert.ToInt32(dataTable1.Rows[0]["IMPSANDET"]) > 0 && dataTable1.Rows[0]["SANSOTSOG"].ToString() == "N")
          {
            string str6 = dataTable1.Rows[0]["RICSANUTE"]?.ToString() ?? "";
            if (str6.Trim().ToUpper() != "ESCLUDI")
            {
              string upper = str6.Trim().ToUpper();
              string strSQL11 = upper == "RITARDO" ? "SELECT CODCAU FROM TIPMOVCAU WHERE TIPMOV = 'SAN_RD' AND CURRENT_DATE BETWEEN DATINI AND DATFIN" : (upper == "OMISSIONE" ? "SELECT CODCAU FROM TIPMOVCAU WHERE TIPMOV = 'SAN_MD' AND CURRENT_DATE BETWEEN DATINI AND DATFIN" : "SELECT CODCAU FROM TIPMOVCAU WHERE TIPMOV = 'SAN_RD' AND CURRENT_DATE BETWEEN DATINI AND DATFIN");
              string str7 = db.Get1ValueFromSQL(strSQL11, CommandType.Text);
              int num7 = num1;
              str2 = "";
              num6 = 0;
              DataLayer db2 = db;
              TFI.OCM.Utente.Utente utente2 = ute;
              int CODPOS2 = CODPOS;
              int ANNDEN2 = ANNDEN;
              int MESDEN2 = MESDEN;
              int PRODEN2 = PRODEN;
              string CODCAU2 = str7.ToString();
              today1 = DateTime.Today;
              string dateTimeFormat4 = today1.GetDateTimeFormats((IFormatProvider) dateTimeFormat1)[0];
              int ANNO_BILANCIO2 = num7;
              string DATSCA2 = strData;
              Decimal IMPORTO2 = Convert.ToDecimal(dataTable1.Rows[0]["IMPSANDET"]);
              string TIPO_OPERAZIONE2 = str1;
              ref string local4 = ref str2;
              ref int local5 = ref num6;
              ref List<DatiContabilita> local6 = ref datiContabilitaList;
              string str8 = WriteDIPA_SalvataggioTotale.WRITE_INSERT_MOVIMSAP(db2, utente2, CODPOS2, ANNDEN2, MESDEN2, PRODEN2, CODCAU2, dateTimeFormat4, ANNO_BILANCIO2, DATSCA2, IMPORTO2, 0M, 0M, 0M, "S", "N", TIPO_OPERAZIONE2, ref local4, ref local5, ref local6);
              if (str8 != "")
              {
                string str9 = "UPDATE DENTES SET DATSAN = current_date, " + " NUMSAN = " + DBMethods.DoublePeakForSql(str8) + ", " + " ANNBILSAN = " + num7.ToString();
                if (TIPMOV == "DP")
                  str9 += ", TIPANNSAN = TIPANNMOV";
                string strSQL12 = str9 + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString();
                flag = db.WriteTransactionData(strSQL12, CommandType.Text);
                if (flag)
                {
                  string strSQL13 = "UPDATE DENDET SET NUMSAN = " + DBMethods.DoublePeakForSql(str8) + ", " + " PARTITASAN = " + DBMethods.DoublePeakForSql(str2) + ", " + " PROGMOVSAN = " + num6.ToString() + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString();
                  flag = db.WriteTransactionData(strSQL13, CommandType.Text);
                }
              }
              else
                flag = false;
            }
          }
        }
      }
      if (flag & str5 != "")
      {
        string strSQL14 = "SELECT MAT, PRODENDET FROM DENDET " + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND PREV = 'S'";
        dataTable1.Clear();
        DataTable dataTable3 = db.GetDataTable(strSQL14);
        for (int index = 0; index <= dataTable3.Rows.Count - 1; ++index)
        {
          string strSQL15 = "UPDATE MODPREDET SET NUMMOV = " + DBMethods.DoublePeakForSql(str5) + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND PRODENDET = " + dataTable3.Rows[index]["PRODENDET"]?.ToString() + " AND MAT = " + dataTable3.Rows[index]["MAT"]?.ToString();
          flag = db.WriteTransactionData(strSQL15, CommandType.Text);
          if (!flag)
            break;
        }
      }
      return flag;
    }

    private static string GetTipoAnno(int anno, int annoBilancio)
    {
      if (anno == annoBilancio)
        return "AC";
      
      if (annoBilancio < anno)
        throw new Exception("Contattare l'Amministratore del Sistema. Anno di bilancio inferiore all'anno della distinta");

      return "AP";
    }

    private static string AggiungiApici(string s) => "'" + s + "'";
  }
}
