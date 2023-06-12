// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Utilities.ModGeneraRettifiche
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Data;
using TFI.DAL.ConnectorDB;

namespace TFI.DAL.Utilities
{
  public class ModGeneraRettifiche
  {
    public bool Module_ContabilizzaRettifiche(
      DataLayer objDAtaAccess,
      TFI.OCM.Utente.Utente u,
      ref DataTable dgDati,
      ref bool RETTIFICHE_NON_CONTABILIZZATE,
      ref DataTable DTSTAMPE,
      ref string ErroreMSG,
      bool FLGRETAPP,
      string PREV = "N")
    {
      string NUMGRURET = "";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      DataTable dataTable4 = new DataTable();
      Decimal num1 = 0.0M;
      int num2 = 0;
      string TIPANN = "";
      DataTable dataTable5 = new DataTable();
      Decimal num3 = 0.0M;
      Decimal num4 = 0.0M;
      Decimal num5 = 0.0M;
      Decimal num6 = 0.0M;
      Decimal num7 = 0.0M;
      DataTable dataTable6 = new DataTable();
      DataTable dataTable7 = new DataTable();
      DataTable dataTable8 = new DataTable();
      DataTable dataTable9 = new DataTable();
      DataTable dataTable10 = new DataTable();
      DataTable dataTable11 = new DataTable();
      DataTable dataTable12 = new DataTable();
      DataTable dataTable13 = new DataTable();
      DataTable dataTable14 = new DataTable();
      string PARTITA_MOVIMENTO = "";
      Decimal PROGMOV_MOVIMENTO = 0.0M;
      string PARTITA_SANZIONE = "";
      Decimal PROGMOV_SANZIONE = 0.0M;
      int num8 = 0;
      DataTable dataTable15 = new DataTable();
      int num9 = 0;
      int num10 = 0;
      DataTable dataTable16 = new DataTable();
      IDOC_RDL idocRdl = new IDOC_RDL();
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      ModGetDati modGetDati = new ModGetDati();
      ModContabilita modContabilita = new ModContabilita();
      try
      {
        int index1 = 0;
        while (index1 <= dgDati.Rows.Count - 1 && !Convert.ToBoolean(dgDati.Rows[index1][0]))
          ++index1;
        if (index1 > dgDati.Rows.Count - 1)
        {
          ErroreMSG = "Nessuna rettifica selezionata da contabilizzare. Impossibile continuare";
          return false;
        }
        string str1 = new Utile().Module_GetDataSistema().ToString().Substring(0, 10);
        int year = Convert.ToDateTime(str1).Year;
        int PRORET = clsWriteDb.WRITE_INSERT_RETTESGRU(objDAtaAccess, u, year, ref NUMGRURET);
        Decimal num11 = 0M;
        for (int index2 = 0; index2 <= dgDati.Rows.Count - 1; ++index2)
        {
          if (Convert.ToBoolean(dgDati.Rows[index2][0]))
          {
            num2 = modGetDati.Module_GetAnnoBilancio(objDAtaAccess, false, (int) dgDati.Rows[index2]["ANNDEN"]);
            TIPANN = !(dgDati.Rows[index2]["DENTES_TIPMOV"].ToString().Trim() == "AR") ? modGetDati.Module_GetTipoAnno((int) dgDati.Rows[index2]["ANNDEN"], num2) : modGetDati.Module_GetTipoAnno((int) dgDati.Rows[index2]["ANNCOM"], num2);
            int num12 = clsWriteDb.WRITE_INSERT_RETTES(objDAtaAccess, u, year, PRORET, NUMGRURET, (int) dgDati.Rows[index2]["CODPOS"], (int) dgDati.Rows[index2]["ANNDEN"], (int) dgDati.Rows[index2]["MESDEN"], TIPANN, dgDati.Rows[index2]["TIPIMP"].ToString(), dgDati.Rows[index2]["CODCAUSAN"].ToString().Trim(), num2);
            string strSQL1 = "UPDATE DENDET SET ANNRET = " + year.ToString() + ", " + " PRORET = " + PRORET.ToString() + ", " + " PRORETTES = " + num12.ToString() + ", " + " NUMGRURET = " + DBMethods.DoublePeakForSql(NUMGRURET) + ", " + " NUMRETTES = " + DBMethods.DoublePeakForSql(NUMGRURET + "/" + num12.ToString().Trim().PadLeft(3, '0')) + " WHERE CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " AND ANNDEN = " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AND MESDEN = " + dgDati.Rows[index2]["MESDEN"].ToString() + " AND PRODEN = " + dgDati.Rows[index2]["PRODEN"].ToString() + " AND MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " AND PRODENDET = " + dgDati.Rows[index2]["PRODENDET"].ToString();
            objDAtaAccess.WriteTransactionData(strSQL1, CommandType.Text);
            string strSQL2 = "INSERT INTO DENDETSOS(" + "CODPOS, ANNDEN, MESDEN, PRODEN, MAT, PRODENDET, " + "PRORAP, CODSOS, DATINISOS, DATFINSOS, PERAZI,  " + "PERCFIG, ULTAGG, UTEAGG)" + " SELECT " + "CODPOS , " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AS ANNDEN , " + dgDati.Rows[index2]["MESDEN"].ToString() + " AS MESDEN , " + dgDati.Rows[index2]["PRODEN"].ToString() + " AS PRODEN , " + "MAT , " + dgDati.Rows[index2]["PRODENDET"].ToString() + " AS PRODENDET , " + "PRORAP, CODSOS, DATINISOS, DATFINSOS, PERAZI, " + "PERFIG, CURRENT_TIMESTAMP, " + " " + DBMethods.DoublePeakForSql(u.Username) + " AS UTEAGG " + " FROM SOSRAP WHERE " + " CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " AND MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " AND PRORAP = " + dgDati.Rows[index2]["PRORAP"].ToString() + " AND DATINISOS <= " + DBMethods.Db2Date(dgDati.Rows[index2]["AL"].ToString()) + " AND " + " VALUE(DATFINSOS,'9999-12-31') >= " + DBMethods.Db2Date(dgDati.Rows[index2]["DAL"].ToString()) + " AND STASOS = '0'";
            objDAtaAccess.WriteTransactionData(strSQL2, CommandType.Text);
            if (Convert.ToInt32(dgDati.Rows[index2]["IMPRETDEL"]) == 0 & Convert.ToInt32(dgDati.Rows[index2]["IMPCONDEL"]) != 0)
            {
              int num13 = 0;
              num8 = 0;
              num9 = 0;
              num10 = 0;
              string str2 = "";
              string strSQL3 = "SELECT CODGRUASS FROM DENDET WHERE CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " AND ANNDEN = " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AND MESDEN = " + dgDati.Rows[index2]["MESDEN"].ToString() + " AND PRODEN = " + dgDati.Rows[index2]["PRODEN"].ToString() + " AND MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " AND PRODENDET = '" + (Convert.ToInt32(dgDati.Rows[index2]["PRODENDET"]) - 1).ToString() + "' " + " AND ESIRET = 'S'";
              num13 = Convert.ToInt32(objDAtaAccess.Get1ValueFromSQL(strSQL3, CommandType.Text));
              string strSQL4 = "SELECT CODQUACON FROM DENDET WHERE CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " AND ANNDEN = " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AND MESDEN = " + dgDati.Rows[index2]["MESDEN"].ToString() + " AND PRODEN = " + dgDati.Rows[index2]["PRODEN"].ToString() + " AND MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " AND PRODENDET = " + ((int) dgDati.Rows[index2]["PRODENDET"] - 1).ToString() + " AND ESIRET = 'S'";
              int int32 = Convert.ToInt32(objDAtaAccess.Get1ValueFromSQL(strSQL4, CommandType.Text));
              int num14 = (int) dgDati.Rows[index2]["CODGRUASS"];
              int num15 = (int) dgDati.Rows[index2]["CODQUACON"];
              string strSQL5 = "SELECT (SELECT DISTINCT CATFORASS FROM FORASS WHERE CODFORASS = A.CODFORASS AND CATFORASS NOT IN ('FAP')) AS CATFORASS FROM ALIFORASS A" + " WHERE CODGRUASS = " + num13.ToString() + " AND CODQUACON = " + int32.ToString() + " ORDER BY CATFORASS ASC";
              dataTable15.Clear();
              dataTable15 = objDAtaAccess.GetDataTable(strSQL5);
              string strSQL6 = "SELECT (SELECT DISTINCT CATFORASS FROM FORASS WHERE CODFORASS = A.CODFORASS AND CATFORASS NOT IN ('FAP')) AS CATFORASS FROM ALIFORASS A" + " WHERE CODGRUASS = " + num14.ToString() + " AND CODQUACON = " + num15.ToString() + " ORDER BY CATFORASS ASC";
              dataTable16.Clear();
              dataTable16 = objDAtaAccess.GetDataTable(strSQL6);
              for (int index3 = 0; index3 <= dataTable15.Rows.Count - 1; ++index3)
              {
                int index4 = 0;
                while (index4 <= dataTable16.Rows.Count - 1 && dataTable15.Rows[index3]["CATFORASS"].ToString().Trim() != "" && !(dataTable15.Rows[index3]["CATFORASS"].ToString().Trim() == dataTable16.Rows[index4]["CATFORASS"].ToString().Trim()))
                  ++index4;
                if (index4 == dataTable16.Rows.Count)
                  str2 = dataTable15.Rows[index3]["CATFORASS"].ToString().Trim();
              }
              if (str2 != "")
              {
                string str3 = "SELECT CODFORASS, ALIQUOTA FROM ALIFORASS " + " WHERE CODGRUASS= " + num13.ToString() + " AND CODQUACON = " + int32.ToString() + " AND " + DBMethods.Db2Date(dgDati.Rows[index2]["DAL"].ToString()) + " BETWEEN DATINI AND VALUE(DATFIN,'9999-12-31')" + " AND CODFORASS IN (SELECT CODFORASS FROM FORASS ";
                string strSQL7 = !(str2 == "") ? str3 + " WHERE CATFORASS = '" + str2 + "')" : str3 + " WHERE CATFORASS = 'TFR')";
                dataTable3.Clear();
                dataTable3 = objDAtaAccess.GetDataTable(strSQL7);
                Decimal num16 = Math.Round((Decimal) dgDati.Rows[index2]["IMPCONDEL"] / (Decimal) dataTable3.Rows[0]["ALIQUOTA"], 2);
                string strSQL8 = "INSERT INTO DENDETALI( " + "CODPOS , ANNDEN , MESDEN, PRODEN, MAT, " + "PRODENDET , CODFORASS, ALIQUOTA, IMPCON, ULTAGG,UTEAGG) " + " VALUES (" + dgDati.Rows[index2]["CODPOS"].ToString() + ", " + dgDati.Rows[index2]["ANNDEN"].ToString() + ", " + dgDati.Rows[index2]["MESDEN"].ToString() + ", " + dgDati.Rows[index2]["PRODEN"].ToString() + ", " + dgDati.Rows[index2]["MAT"].ToString() + ", " + dgDati.Rows[index2]["PRODENDET"].ToString() + ", " + dataTable3.Rows[0]["CODFORASS"]?.ToString() + ", " + dataTable3.Rows[0]["ALIQUOTA"].ToString().Replace(",", ".") + ", " + " ROUND( " + num16.ToString().Replace(",", ".") + " * " + dataTable3.Rows[0]["ALIQUOTA"].ToString().Replace(",", ".") + ", 2), " + " CURRENT_TIMESTAMP, " + " " + DBMethods.DoublePeakForSql(u.Username) + ")";
                objDAtaAccess.WriteTransactionData(strSQL8, CommandType.Text);
              }
              else
              {
                Decimal num17 = Math.Round((Decimal) dgDati.Rows[index2]["IMPCONDEL"] / (Decimal) dgDati.Rows[index2]["ALIQUOTA"], 2);
                string str4 = "INSERT INTO DENDETALI( " + "CODPOS , ANNDEN , MESDEN, PRODEN, MAT, " + "PRODENDET , CODFORASS, ALIQUOTA, IMPCON, ULTAGG,UTEAGG) " + " SELECT " + dgDati.Rows[index2]["CODPOS"].ToString() + " AS CODPOS , " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AS ANNDEN , " + dgDati.Rows[index2]["MESDEN"].ToString() + " AS MESDEN , " + dgDati.Rows[index2]["PRODEN"].ToString() + " AS PRODEN , " + dgDati.Rows[index2]["MAT"].ToString() + " AS MAT , " + dgDati.Rows[index2]["PRODENDET"].ToString() + " AS PRODENDET , " + " CODFORASS, ALIQUOTA, " + " ROUND( " + num17.ToString().Replace(",", ".") + " * ALIQUOTA, 2) AS IMPCON, " + " CURRENT_TIMESTAMP, " + " " + DBMethods.DoublePeakForSql(u.Username) + " AS UTEAGG " + " FROM ALIFORASS " + " WHERE CODGRUASS= " + dgDati.Rows[index2]["CODGRUASS"].ToString() + " AND CODQUACON = " + dgDati.Rows[index2]["CODQUACON"].ToString() + " AND " + DBMethods.Db2Date(dgDati.Rows[index2]["DAL"].ToString()) + " BETWEEN DATINI AND VALUE(DATFIN,'9999-12-31')";
                string strSQL9 = !(dgDati.Rows[index2]["ETA65"].ToString() == "N") ? str4 + " AND CODFORASS NOT IN(SELECT CODFORASS FROM FORASS " + "WHERE CATFORASS IN('PREV', 'FAP' , 'TFR'))" : str4 + " AND CODFORASS NOT IN(SELECT CODFORASS FROM FORASS " + "WHERE CATFORASS IN( 'FAP', 'TFR'))";
                objDAtaAccess.WriteTransactionData(strSQL9, CommandType.Text);
                string str5 = "INSERT INTO DENDETALI( " + "CODPOS , ANNDEN , MESDEN, PRODEN, MAT, " + "PRODENDET , CODFORASS, ALIQUOTA, IMPCON, ULTAGG,UTEAGG) " + " SELECT " + dgDati.Rows[index2]["CODPOS"].ToString() + " AS CODPOS , " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AS ANNDEN , " + dgDati.Rows[index2]["MESDEN"].ToString() + " AS MESDEN , " + dgDati.Rows[index2]["PRODEN"].ToString() + " AS PRODEN , " + dgDati.Rows[index2]["MAT"].ToString() + " AS MAT , " + dgDati.Rows[index2]["PRODENDET"].ToString() + " AS PRODENDET , " + " CODFORASS, ";
                string[] strArray = Math.Round(Convert.ToDecimal(dgDati.Rows[index2]["IMPCONDEL"]) / Convert.ToDecimal(dgDati.Rows[index2]["ALIQUOTA"]), 2).ToString().Trim().Split(',');
                string str6 = strArray[0];
                string str7 = strArray.Length <= 1 ? "0" : strArray[1];
                num11 = str7.Length <= 2 ? Convert.ToDecimal(str6 + "," + str7) : Convert.ToDecimal(str6 + "," + str7.Substring(0, 2));
                string str8;
                if (dgDati.Rows[index2]["FAP"].ToString().ToString().Trim() == "S")
                {
                  dgDati.Rows[index2]["PERFAP"] = !(dgDati.Rows[index2]["ANNDEN"].ToString() == "2011" & dgDati.Rows[index2]["MESDEN"].ToString() == "8" & dgDati.Rows[index2]["PRODENDET"].ToString() == "4") ? (object) 0.5 : (object) 0.0M;
                  str8 = str5 + " ALIQUOTA + " + dgDati.Rows[index2]["PERFAP"].ToString().ToString().Replace(",", ".") + ", " + " ROUND( " + num11.ToString().Replace(",", ".") + " * (ALIQUOTA + " + dgDati.Rows[index2]["PERFAP"].ToString().ToString().Replace(",", ".") + ") ,2) AS IMPCON, ";
                }
                else
                  str8 = str5 + " ALIQUOTA, " + " ROUND(" + num11.ToString().Replace(",", ".") + " * ALIQUOTA,2) AS IMPCON, ";
                string strSQL10 = str8 + " CURRENT_TIMESTAMP, " + " " + DBMethods.DoublePeakForSql(u.Username) + " AS UTEAGG " + " FROM ALIFORASS " + " WHERE CODGRUASS= " + dgDati.Rows[index2]["CODGRUASS"].ToString() + " AND CODQUACON = " + dgDati.Rows[index2]["CODQUACON"].ToString() + " AND " + DBMethods.Db2Date(dgDati.Rows[index2]["DAL"].ToString()) + " BETWEEN DATINI AND VALUE(DATFIN,'9999-12-31')" + " AND CODFORASS IN (SELECT CODFORASS FROM FORASS " + " WHERE CATFORASS = 'TFR')";
                objDAtaAccess.WriteTransactionData(strSQL10, CommandType.Text);
              }
            }
            else
            {
              Decimal num18 = Math.Round((Decimal) dgDati.Rows[index2]["IMPCONDEL"] / (Decimal) dgDati.Rows[index2]["ALIQUOTA"], 2);
              string str9 = "INSERT INTO DENDETALI( " + "CODPOS , ANNDEN , MESDEN, PRODEN, MAT, " + "PRODENDET , CODFORASS, ALIQUOTA, IMPCON, ULTAGG,UTEAGG) " + " SELECT " + dgDati.Rows[index2]["CODPOS"].ToString() + " AS CODPOS , " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AS ANNDEN , " + dgDati.Rows[index2]["MESDEN"].ToString() + " AS MESDEN , " + dgDati.Rows[index2]["PRODEN"].ToString() + " AS PRODEN , " + dgDati.Rows[index2]["MAT"].ToString() + " AS MAT , " + dgDati.Rows[index2]["PRODENDET"].ToString() + " AS PRODENDET , " + " CODFORASS, ALIQUOTA, " + " ROUND( " + num18.ToString().Replace(",", ".") + " * ALIQUOTA, 2) AS IMPCON, " + " CURRENT_TIMESTAMP, " + " " + DBMethods.DoublePeakForSql(u.Username) + " AS UTEAGG " + " FROM ALIFORASS " + " WHERE CODGRUASS= " + dgDati.Rows[index2]["CODGRUASS"].ToString() + " AND CODQUACON = " + dgDati.Rows[index2]["CODQUACON"].ToString() + " AND " + DBMethods.Db2Date(dgDati.Rows[index2]["DAL"].ToString()) + " BETWEEN DATINI AND VALUE(DATFIN,'9999-12-31')";
              string strSQL11 = !(dgDati.Rows[index2]["ETA65"].ToString() == "N") ? str9 + " AND CODFORASS NOT IN(SELECT CODFORASS FROM FORASS " + "WHERE CATFORASS IN('PREV', 'FAP' , 'TFR'))" : str9 + " AND CODFORASS NOT IN(SELECT CODFORASS FROM FORASS " + "WHERE CATFORASS IN( 'FAP', 'TFR'))";
              objDAtaAccess.WriteTransactionData(strSQL11, CommandType.Text);
              string str10 = "INSERT INTO DENDETALI( " + "CODPOS , ANNDEN , MESDEN, PRODEN, MAT, " + "PRODENDET , CODFORASS, ALIQUOTA, IMPCON, ULTAGG,UTEAGG) " + " SELECT " + dgDati.Rows[index2]["CODPOS"].ToString() + " AS CODPOS , " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AS ANNDEN , " + dgDati.Rows[index2]["MESDEN"].ToString() + " AS MESDEN , " + dgDati.Rows[index2]["PRODEN"].ToString() + " AS PRODEN , " + dgDati.Rows[index2]["MAT"].ToString() + " AS MAT , " + dgDati.Rows[index2]["PRODENDET"].ToString() + " AS PRODENDET , " + " CODFORASS, ";
              string[] strArray = Math.Round(Convert.ToDecimal(dgDati.Rows[index2]["IMPCONDEL"]) / Convert.ToDecimal(dgDati.Rows[index2]["ALIQUOTA"]), 2).ToString().Trim().Split(',');
              string str11 = strArray[0];
              string str12 = strArray.Length <= 1 ? "0" : strArray[1];
              num11 = str12.Length <= 2 ? Convert.ToDecimal(str11 + "," + str12) : Convert.ToDecimal(str11 + "," + str12.Substring(0, 2));
              string str13;
              if (dgDati.Rows[index2]["FAP"].ToString().ToString().Trim() == "S")
              {
                dgDati.Rows[index2]["PERFAP"] = !(dgDati.Rows[index2]["ANNDEN"].ToString() == "2011" & dgDati.Rows[index2]["MESDEN"].ToString() == "8" & dgDati.Rows[index2]["PRODENDET"].ToString() == "4") ? (object) 0.5 : (object) 0.0M;
                str13 = str10 + " ALIQUOTA + " + dgDati.Rows[index2]["PERFAP"].ToString().ToString().Replace(",", ".") + ", " + " ROUND( " + num11.ToString().Replace(",", ".") + " * (ALIQUOTA + " + dgDati.Rows[index2]["PERFAP"].ToString().ToString().Replace(",", ".") + ") ,2) AS IMPCON, ";
              }
              else
                str13 = str10 + " ALIQUOTA, " + " ROUND(" + num11.ToString().Replace(",", ".") + " * ALIQUOTA,2) AS IMPCON, ";
              string strSQL12 = str13 + " CURRENT_TIMESTAMP, " + " " + DBMethods.DoublePeakForSql(u.Username) + " AS UTEAGG " + " FROM ALIFORASS " + " WHERE CODGRUASS= " + dgDati.Rows[index2]["CODGRUASS"].ToString() + " AND CODQUACON = " + dgDati.Rows[index2]["CODQUACON"].ToString() + " AND " + DBMethods.Db2Date(dgDati.Rows[index2]["DAL"].ToString()) + " BETWEEN DATINI AND VALUE(DATFIN,'9999-12-31')" + " AND CODFORASS IN (SELECT CODFORASS FROM FORASS " + " WHERE CATFORASS = 'TFR')";
              objDAtaAccess.WriteTransactionData(strSQL12, CommandType.Text);
            }
            string str14 = "UPDATE DENDET SET ";
            string str15;
            if (dgDati.Rows[index2]["FAP"].ToString().ToString().Trim() == "S")
              str15 = str14 + " IMPFAP = " + num11.ToString().Replace(",", ".") + " * " + dgDati.Rows[index2]["PERFAP"].ToString().ToString().Replace(",", ".");
            else
              str15 = str14 + " IMPFAP = 0";
            string strSQL13 = str15 + " WHERE CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " AND ANNDEN = " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AND MESDEN = " + dgDati.Rows[index2]["MESDEN"].ToString() + " AND PRODEN = " + dgDati.Rows[index2]["PRODEN"].ToString() + " AND MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " AND PRODENDET = " + dgDati.Rows[index2]["PRODENDET"].ToString();
            objDAtaAccess.WriteTransactionData(strSQL13, CommandType.Text);
            string strSQL14 = "SELECT VALUE(SUM(IMPCON), 0) AS IMPCON FROM DENDETALI " + " WHERE CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " AND ANNDEN = " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AND MESDEN = " + dgDati.Rows[index2]["MESDEN"].ToString() + " AND PRODEN = " + dgDati.Rows[index2]["PRODEN"].ToString() + " AND MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " AND PRODENDET = " + dgDati.Rows[index2]["PRODENDET"].ToString();
            Decimal num19 = Convert.ToDecimal(objDAtaAccess.Get1ValueFromSQL(strSQL14, CommandType.Text));
            string strSQL15 = "SELECT IMPCONDEL FROM DENDET " + " WHERE CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " AND ANNDEN = " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AND MESDEN = " + dgDati.Rows[index2]["MESDEN"].ToString() + " AND PRODEN = " + dgDati.Rows[index2]["PRODEN"].ToString() + " AND MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " AND PRODENDET = " + dgDati.Rows[index2]["PRODENDET"].ToString();
            Decimal num20 = Convert.ToDecimal(objDAtaAccess.Get1ValueFromSQL(strSQL15, CommandType.Text));
            Decimal num21 = num20 - num19;
            Decimal num22;
            if (num21 <= -0.1M)
            {
              if (num21 <= -0.6M)
              {
                if (num21 <= -0.8M)
                {
                  if (!(num21 == -0.9M) && !(num21 == -0.8M))
                    goto label_48;
                }
                else if (!(num21 == -0.7M) && !(num21 == -0.6M))
                  goto label_48;
              }
              else if (num21 <= -0.4M)
              {
                if (!(num21 == -0.5M) && !(num21 == -0.4M))
                  goto label_48;
              }
              else if (!(num21 == -0.3M) && !(num21 == -0.2M) && !(num21 == -0.1M))
                goto label_48;
              num22 = num20 - num19;
              string strSQL16 = "UPDATE DENDETALI SET IMPCON = IMPCON + " + num22.ToString().Trim().Replace(",", ".") + " WHERE CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " AND ANNDEN = " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AND MESDEN = " + dgDati.Rows[index2]["MESDEN"].ToString() + " AND PRODEN = " + dgDati.Rows[index2]["PRODEN"].ToString() + " AND MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " AND PRODENDET = " + dgDati.Rows[index2]["PRODENDET"].ToString() + " AND CODFORASS = (SELECT MIN(CODFORASS) FROM DENDETALI " + " WHERE CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " AND ANNDEN = " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AND MESDEN = " + dgDati.Rows[index2]["MESDEN"].ToString() + " AND PRODEN = " + dgDati.Rows[index2]["PRODEN"].ToString() + " AND MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " AND PRODENDET = " + dgDati.Rows[index2]["PRODENDET"].ToString() + ")";
              objDAtaAccess.WriteTransactionData(strSQL16, CommandType.Text);
              goto label_49;
            }
            else
            {
              if (num21 <= 0.4M)
              {
                if (num21 <= 0.1M)
                {
                  if (!(num21 == 0M))
                  {
                    if (!(num21 == 0.1M))
                      goto label_48;
                  }
                  else
                    goto label_49;
                }
                else if (!(num21 == 0.2M) && !(num21 == 0.3M) && !(num21 == 0.4M))
                  goto label_48;
              }
              else if (num21 <= 0.6M)
              {
                if (!(num21 == 0.5M) && !(num21 == 0.6M))
                  goto label_48;
              }
              else if (!(num21 == 0.7M) && !(num21 == 0.8M) && !(num21 == 0.9M))
                goto label_48;
              num22 = num20 - num19;
              string strSQL17 = "UPDATE DENDETALI SET IMPCON = IMPCON + " + num22.ToString().Trim().Replace(",", ".") + " WHERE CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " AND ANNDEN = " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AND MESDEN = " + dgDati.Rows[index2]["MESDEN"].ToString() + " AND PRODEN = " + dgDati.Rows[index2]["PRODEN"].ToString() + " AND MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " AND PRODENDET = " + dgDati.Rows[index2]["PRODENDET"].ToString() + " AND CODFORASS = (SELECT MIN(CODFORASS) FROM DENDETALI " + " WHERE CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " AND ANNDEN = " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AND MESDEN = " + dgDati.Rows[index2]["MESDEN"].ToString() + " AND PRODEN = " + dgDati.Rows[index2]["PRODEN"].ToString() + " AND MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " AND PRODENDET = " + dgDati.Rows[index2]["PRODENDET"].ToString() + ")";
              objDAtaAccess.WriteTransactionData(strSQL17, CommandType.Text);
              goto label_49;
            }
label_48:
            Decimal num23 = num20 - num19;
            ErroreMSG = "Attenzione... errore nello splittamento in DENDETALI del contributo per: CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " ANNDEN = " + dgDati.Rows[index2]["ANNDEN"].ToString() + " MESDEN = " + dgDati.Rows[index2]["MESDEN"].ToString() + " PRODEN = " + dgDati.Rows[index2]["PRODEN"].ToString() + " MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " PRODENDET = " + dgDati.Rows[index2]["PRODENDET"].ToString() + ". Differenza per euro " + num23.ToString() + " ";
            throw new Exception();
label_49:
            string strSQL18 = "UPDATE DENDET SET NUMGRURETRIF = " + DBMethods.DoublePeakForSql(NUMGRURET) + "," + " NUMRETTESRIF = " + DBMethods.DoublePeakForSql(NUMGRURET + "/" + num12.ToString().Trim().PadLeft(3, '0')) + " " + " WHERE CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " AND ANNDEN = " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AND MESDEN = " + dgDati.Rows[index2]["MESDEN"].ToString() + " AND PRODEN = " + dgDati.Rows[index2]["PRODEN"].ToString() + " AND MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " AND DAL = " + DBMethods.Db2Date(dgDati.Rows[index2]["DAL"].ToString()) + " AND PRODENDET = (SELECT MAX(PRODENDET) FROM DENDET " + " WHERE CODPOS = " + dgDati.Rows[index2]["CODPOS"].ToString() + " AND ANNDEN = " + dgDati.Rows[index2]["ANNDEN"].ToString() + " AND MESDEN = " + dgDati.Rows[index2]["MESDEN"].ToString() + " AND PRODEN = " + dgDati.Rows[index2]["PRODEN"].ToString() + " AND MAT = " + dgDati.Rows[index2]["MAT"].ToString() + " AND DAL = " + DBMethods.Db2Date(dgDati.Rows[index2]["DAL"].ToString()) + " AND PRODENDET <> " + dgDati.Rows[index2]["PRODENDET"].ToString() + ")" + " AND ESIRET = 'S' AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL";
            objDAtaAccess.WriteTransactionData(strSQL18, CommandType.Text);
          }
          else
            RETTIFICHE_NON_CONTABILIZZATE = true;
        }
        string strSQL19 = " SELECT * FROM RETTES WHERE " + " ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString();
        dataTable1.Clear();
        DataTable dataTable17 = objDAtaAccess.GetDataTable(strSQL19);
        Decimal num24;
        for (int index5 = 0; index5 <= dataTable17.Rows.Count - 1; ++index5)
        {
          string strSQL20 = "SELECT * FROM DENDET" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + " AND CODPOS = " + dataTable17.Rows[index5]["CODPOS"]?.ToString();
          dataTable4.Clear();
          dataTable4 = objDAtaAccess.GetDataTable(strSQL20);
          for (int index6 = 0; index6 <= dataTable4.Rows.Count - 1; ++index6)
          {
            if ((int) dataTable4.Rows[index6]["IMPRETDEL"] != 0 | (int) dataTable4.Rows[index6]["IMPOCCDEL"] != 0 | (int) dataTable4.Rows[index6]["IMPFIGDEL"] != 0 && !(!(dataTable4.Rows[index6]["ANNCOM"].ToString().Trim() != "") ? clsWriteDb.AGGIORNA_RETANN(objDAtaAccess, u, dataTable4.Rows[index6]["CODPOS"].ToString(), dataTable4.Rows[index6]["MAT"].ToString(), (int) dataTable4.Rows[index6]["PRORAP"], (int) dataTable4.Rows[index6]["ANNDEN"], (Decimal) dataTable4.Rows[index6]["IMPRETDEL"], (Decimal) dataTable4.Rows[index6]["IMPOCCDEL"], (Decimal) dataTable4.Rows[index6]["IMPFIGDEL"], "+") : clsWriteDb.AGGIORNA_RETANN(objDAtaAccess, u, dataTable4.Rows[index6]["CODPOS"].ToString(), dataTable4.Rows[index6]["MAT"].ToString(), (int) dataTable4.Rows[index6]["PRORAP"], (int) dataTable4.Rows[index6]["ANNDEN"], (Decimal) dataTable4.Rows[index6]["IMPRETDEL"], (Decimal) dataTable4.Rows[index6]["IMPOCCDEL"], (Decimal) dataTable4.Rows[index6]["IMPFIGDEL"], "+", true)))
              throw new Exception("Si sono verificati errori durante la contabilizzazione delle rettifiche.");
          }
          num24 = Convert.ToDecimal(clsWriteDb.Module_GetValorePargen(objDAtaAccess, 5, str1, (int) dataTable17.Rows[index5]["CODPOS"]));
          string str16 = "";
          string str17 = "";
          num7 = 0M;
          num1 = 0M;
          num6 = 0M;
          num5 = 0M;
          num4 = 0M;
          num3 = 0M;
          string strSQL21 = "SELECT ROUND(VALUE(SUM(IMPCONDEL+IMPCONDEL/100*" + num24.ToString().Replace(",", ".") + "), 0.0m), 2) AS IMPORTO " + " FROM DENDET WHERE" + " ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
          Decimal IMPORTO1 = Convert.ToDecimal(objDAtaAccess.Get1ValueFromSQL(strSQL21, CommandType.Text));
          string CODCAU;
          if (IMPORTO1 < 0M)
          {
            string strSQL22 = "SELECT VALUE(CODCAU,0) FROM TIPMOVCAU WHERE TIPMOV = 'RT_NEG'";
            CODCAU = objDAtaAccess.Get1ValueFromSQL(strSQL22, CommandType.Text).ToString().Trim();
          }
          else
          {
            string strSQL23 = "SELECT VALUE(CODCAU,0) FROM TIPMOVCAU WHERE TIPMOV = 'RT_POS'";
            CODCAU = objDAtaAccess.Get1ValueFromSQL(strSQL23, CommandType.Text).ToString().Trim();
          }
          Decimal IMPASSCON = 0M;
          Decimal IMPABB = 0M;
          string strSQL24 = "SELECT ROUND(VALUE(SUM(IMPCONDEL/100*" + num24.ToString().Replace(",", ".") + "), 0.0m), 2) AS IMPORTO " + " FROM DENDET WHERE" + " ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
          Decimal IMPADD = Convert.ToDecimal(objDAtaAccess.Get1ValueFromSQL(strSQL24, CommandType.Text));
          string NUMMOV;
          if (IMPORTO1 != 0M)
          {
            string strSQL25 = "  SELECT CODPOS, ANNDEN, MESDEN, PRODEN, MAT, PRODENDET FROM DENDET WHERE " + " CODPOS = " + dataTable17.Rows[index5]["CODPOS"]?.ToString() + " AND ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + " AND IMPCONDEL <> 0" + " ORDER BY ANNDEN,MESDEN ";
            DataTable dataTable18 = objDAtaAccess.GetDataTable(strSQL25);
            NUMMOV = clsWriteDb.WRITE_INSERT_MOVIMSAP(objDAtaAccess, u, ref ErroreMSG, (int) dataTable17.Rows[index5]["CODPOS"], 0, 0, 0, CODCAU, str1, num2, str1, IMPORTO1, IMPABB, IMPADD, IMPASSCON, "", "", dataTable17.Rows[index5]["TIPANN"].ToString(), ref PARTITA_MOVIMENTO, ref PROGMOV_MOVIMENTO, ref PARTITA_SANZIONE, ref PROGMOV_SANZIONE, dataTable18, "RETTIFICHE", dataTable17.Rows[index5]["TIPIMP"].ToString().Trim());
            string strSQL26 = " UPDATE DENDET SET NUMMOV = " + DBMethods.DoublePeakForSql(NUMMOV) + ", " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + ", " + " PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + ", " + " DATCONMOV = CURRENT_DATE " + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + " AND IMPCONDEL <> 0";
            objDAtaAccess.WriteTransactionData(strSQL26, CommandType.Text);
            if (dataTable17.Rows[index5]["CODCAUSAN"].ToString().Trim() != "")
            {
              string strSQL27 = "SELECT VALUE(SUM(IMPSANDET),0.0m) AS IMPORTO FROM DENDET WHERE" + " ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + " AND IMPSANDET > 0 ";
              Decimal IMPORTO2 = Convert.ToDecimal(objDAtaAccess.Get1ValueFromSQL(strSQL27, CommandType.Text));
              if (IMPORTO2 > 0M)
              {
                string str18 = dataTable17.Rows[index5]["CODCAUSAN"].ToString().Trim();
                if (!(str18 == "36"))
                {
                  if (str18 == "39")
                    strSQL27 = "SELECT IMPSOGLIA FROM TIPMOVCAU WHERE CODCAU=" + DBMethods.DoublePeakForSql(dataTable17.Rows[index5]["CODCAUSAN"].ToString().Trim()) + " AND " + DBMethods.Db2Date(str1) + " BETWEEN DATINI AND DATFIN" + " AND TIPMOV = 'SAN_RT_RD' ";
                }
                else
                  strSQL27 = "SELECT IMPSOGLIA FROM TIPMOVCAU WHERE CODCAU=" + DBMethods.DoublePeakForSql(dataTable17.Rows[index5]["CODCAUSAN"].ToString().Trim()) + " AND " + DBMethods.Db2Date(str1) + " BETWEEN DATINI AND DATFIN" + " AND TIPMOV = 'SAN_RT_MD' ";
                if (Convert.ToDecimal(objDAtaAccess.Get1ValueFromSQL(strSQL27, CommandType.Text)) < IMPORTO2)
                {
                  str17 = "N";
                  str16 = clsWriteDb.WRITE_INSERT_MOVIMSAP(objDAtaAccess, u, ref ErroreMSG, (int) dataTable17.Rows[index5]["CODPOS"], (int) dataTable18.Rows[dataTable18.Rows.Count - 1]["ANNDEN"], (int) dataTable18.Rows[dataTable18.Rows.Count - 1]["MESDEN"], 0, dataTable17.Rows[index5]["CODCAUSAN"].ToString(), str1, num2, str1, IMPORTO2, 0.0M, 0.0M, 0.0M, "S", "", dataTable17.Rows[index5]["TIPANN"].ToString(), ref PARTITA_MOVIMENTO, ref PROGMOV_MOVIMENTO, ref PARTITA_SANZIONE, ref PROGMOV_SANZIONE, TIPO_OPERAZIONE: "SANZIONI RETTIFICHE");
                  string strSQL28 = " UPDATE DENDET SET NUMSAN = " + DBMethods.DoublePeakForSql(str16) + ", " + " PARTITASAN = " + DBMethods.DoublePeakForSql(PARTITA_SANZIONE) + ", " + " PROGMOVSAN = " + PROGMOV_SANZIONE.ToString() + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
                  objDAtaAccess.WriteTransactionData(strSQL28, CommandType.Text);
                }
                else
                  str17 = "S";
              }
            }
          }
          else
            NUMMOV = "999999999";
          string str19 = "UPDATE RETTES SET NUMMOV = " + DBMethods.DoublePeakForSql(NUMMOV) + ", " + " DATCONMOV = CURRENT_DATE, " + " CODCAUMOV = " + DBMethods.DoublePeakForSql(CODCAU) + ", " + " SANSOTSOG = " + DBMethods.DoublePeakForSql(str17) + ", ";
          if (str16 != "")
            str19 = str19 + " DATSAN = CURRENT_DATE, " + " NUMSAN = " + DBMethods.DoublePeakForSql(str16) + ", " + " ANNBILSAN = ANNBILMOV, ";
          string strSQL29 = str19 + " NUMRIGDET = (SELECT COUNT(*) FROM DENDET " + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + ") " + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
          objDAtaAccess.WriteTransactionData(strSQL29, CommandType.Text);
          string strSQL30 = "UPDATE RETTES SET " + " IMPRET = (SELECT VALUE(SUM(IMPRET), 0) FROM DENDET" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + ") " + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
          objDAtaAccess.WriteTransactionData(strSQL30, CommandType.Text);
          string strSQL31 = "UPDATE RETTES SET " + " IMPOCC = (SELECT VALUE(SUM(IMPOCC), 0) FROM DENDET" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + ")" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
          objDAtaAccess.WriteTransactionData(strSQL31, CommandType.Text);
          string strSQL32 = "UPDATE RETTES SET " + " IMPFIG = (SELECT VALUE(SUM(IMPFIG), 0) FROM DENDET" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + ")" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
          objDAtaAccess.WriteTransactionData(strSQL32, CommandType.Text);
          string strSQL33 = "UPDATE RETTES SET " + " IMPCON = (SELECT VALUE(SUM(IMPCON), 0) FROM DENDET" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + ")" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
          objDAtaAccess.WriteTransactionData(strSQL33, CommandType.Text);
          string strSQL34 = "UPDATE RETTES SET " + " IMPSANDET = (SELECT VALUE(SUM(IMPSANDET), 0) FROM DENDET" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + ") " + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
          objDAtaAccess.WriteTransactionData(strSQL34, CommandType.Text);
          string strSQL35 = "UPDATE RETTES SET" + " IMPRETDEL = (SELECT VALUE(SUM(IMPRETDEL), 0) FROM DENDET" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + ") " + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
          objDAtaAccess.WriteTransactionData(strSQL35, CommandType.Text);
          string strSQL36 = "UPDATE RETTES SET" + " IMPFIGDEL = (SELECT VALUE(SUM(IMPFIGDEL), 0) FROM DENDET" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + ") " + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
          objDAtaAccess.WriteTransactionData(strSQL36, CommandType.Text);
          string strSQL37 = "UPDATE RETTES SET" + " IMPOCCDEL = (SELECT VALUE(SUM(IMPOCCDEL), 0) FROM DENDET" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + ") " + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
          objDAtaAccess.WriteTransactionData(strSQL37, CommandType.Text);
          string strSQL38 = "UPDATE RETTES SET" + " IMPCONDEL = (SELECT VALUE(SUM(IMPCONDEL), 0) FROM DENDET" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + ") " + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
          objDAtaAccess.WriteTransactionData(strSQL38, CommandType.Text);
          string strSQL39 = "UPDATE RETTES SET" + " IMPADDDEL = ROUND(IMPCONDEL * " + num24.ToString().Replace(",", ".") + "/100, 2) , " + " IMPADDREC = ROUND(IMPCON * " + num24.ToString().Replace(",", ".") + "/100, 2)" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
          objDAtaAccess.WriteTransactionData(strSQL39, CommandType.Text);
          string strSQL40 = " UPDATE DENDET SET NUMMOV = " + DBMethods.DoublePeakForSql(NUMMOV) + ", " + " DATCONMOV = CURRENT_DATE " + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString() + " AND IMPCONDEL = 0";
          objDAtaAccess.WriteTransactionData(strSQL40, CommandType.Text);
          string strSQL41 = "UPDATE RETTESGRU " + " SET NUMRIGRETTES = (SELECT COUNT(*) FROM RETTES " + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + ")" + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString();
          objDAtaAccess.WriteTransactionData(strSQL41, CommandType.Text);
          if (dataTable17.Rows[index5]["TIPIMP"].ToString().Trim() == "+")
          {
            string str20;
            if (dataTable17.Rows[index5]["CODCAUSAN"].ToString().Trim() == "")
            {
              str20 = "ESCLUDI";
            }
            else
            {
              string str21 = dataTable17.Rows[index5]["CODCAUSAN"].ToString().Trim();
              if (!(str21 == "36"))
              {
                if (str21 == "39")
                  strSQL41 = "SELECT DISTINCT TIPMOV FROM TIPMOVCAU WHERE CODCAU= " + DBMethods.DoublePeakForSql(dataTable17.Rows[index5]["CODCAUSAN"].ToString()) + " AND TIPMOV = 'SAN_RT_RD' ";
              }
              else
                strSQL41 = "SELECT DISTINCT TIPMOV FROM TIPMOVCAU WHERE CODCAU= " + DBMethods.DoublePeakForSql(dataTable17.Rows[index5]["CODCAUSAN"].ToString()) + " AND TIPMOV = 'SAN_RT_MD' ";
              str20 = objDAtaAccess.Get1ValueFromSQL(strSQL41, CommandType.Text).ToString().Trim();
              if (str20 != "")
              {
                string str22 = str20.Substring(str20.Length - 5);
                if (!(str22 == "RT_MD"))
                {
                  if (str22 == "RT_RD")
                    str20 = "RITARDO";
                }
                else
                  str20 = "EVASIONE";
              }
            }
            string strSQL42 = "UPDATE RETTES SET" + " RICSANUTE = " + DBMethods.DoublePeakForSql(str20) + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
            objDAtaAccess.WriteTransactionData(strSQL42, CommandType.Text);
          }
          else
          {
            string strSQL43 = "UPDATE DENDET SET CODCAUSAN=NULL " + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
            objDAtaAccess.WriteTransactionData(strSQL43, CommandType.Text);
            string strSQL44 = "UPDATE RETTES SET CODCAUSAN=NULL, RICSANUTE= NULL, SANSOTSOG=NULL " + " WHERE ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + dataTable17.Rows[index5]["PRORETTES"]?.ToString();
            objDAtaAccess.WriteTransactionData(strSQL44, CommandType.Text);
          }
          if (NUMMOV != "999999999")
          {
            DataTable dataTable19 = modContabilita.WRITE_CONTABILITA_RETTIFICHE(objDAtaAccess, u, year, PRORET, (int) dataTable17.Rows[index5]["PRORETTES"], TIPANN, (string) dataTable17.Rows[index5]["TIPIMP"], NUMMOV);
            if (dataTable19.Rows.Count > 0 && !DBNull.Value.Equals(dataTable19.Rows[0]["PARTITA"]))
            {
              string strSQL45 = "SELECT PARTRET, PROGRRET FROM MOVRETSAP  WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable19.Rows[0]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable19.Rows[0]["PROGMOV"]?.ToString() + " GROUP BY PARTRET, PROGRRET";
              dataTable14.Clear();
              dataTable14 = objDAtaAccess.GetDataTable(strSQL45);
              if (dataTable17.Rows[index5]["TIPIMP"].ToString() == "+")
              {
                if (dataTable14.Rows.Count == 1)
                {
                  string strSQL46 = "SELECT DATAMOV FROM MOVIMSAP  WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable14.Rows[0]["PARTRET"].ToString()) + " AND PROGMOV = " + dataTable14.Rows[0]["PROGRRET"]?.ToString();
                  string strSQL47 = " UPDATE MOVIMSAP SET " + " DATACOM = " + DBMethods.DoublePeakForSql(objDAtaAccess.Get1ValueFromSQL(strSQL46, CommandType.Text)) + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable19.Rows[0]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable19.Rows[0]["PROGMOV"]?.ToString();
                  objDAtaAccess.WriteTransactionData(strSQL47, CommandType.Text);
                }
                else
                {
                  string strSQL48 = " UPDATE MOVIMSAP SET " + " DATACOM = " + DBMethods.Db2Date(str1).Replace("-", "") + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable19.Rows[0]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable19.Rows[0]["PROGMOV"]?.ToString();
                  objDAtaAccess.WriteTransactionData(strSQL48, CommandType.Text);
                }
              }
              else if (dataTable14.Rows.Count > 1)
              {
                string strSQL49 = " UPDATE MOVIMSAP SET " + " DATACOM = 0 " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable19.Rows[0]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable19.Rows[0]["PROGMOV"]?.ToString();
                objDAtaAccess.WriteTransactionData(strSQL49, CommandType.Text);
              }
            }
          }
          num24 = 0.0M;
          PARTITA_MOVIMENTO = "";
          PROGMOV_MOVIMENTO = 0M;
          PARTITA_SANZIONE = "";
          PROGMOV_SANZIONE = 0M;
        }
        string strSQL50 = " SELECT DISTINCT CODPOS, ANNDEN, MESDEN, PRODEN FROM DENDET WHERE " + " ANNRET = " + year.ToString() + " AND PRORET = " + PRORET.ToString();
        dataTable17.Clear();
        DataTable dataTable20 = objDAtaAccess.GetDataTable(strSQL50);
        for (int index7 = 0; index7 <= dataTable20.Rows.Count - 1; ++index7)
        {
          num24 = Convert.ToDecimal(clsWriteDb.Module_GetValorePargen(objDAtaAccess, 5, str1, (int) dataTable20.Rows[index7]["CODPOS"]));
          string strSQL51 = "UPDATE DENTES SET ESIRET = 'S', " + " IMPRETDEL = (SELECT VALUE(SUM(IMPRETDEL), 0) FROM DENDET" + " WHERE CODPOS = " + dataTable20.Rows[index7]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable20.Rows[index7]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable20.Rows[index7]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable20.Rows[index7]["PRODEN"]?.ToString() + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL AND TIPMOV='RT')," + " IMPFIGDEL = (SELECT VALUE(SUM(IMPFIGDEL), 0) FROM DENDET" + " WHERE CODPOS = " + dataTable20.Rows[index7]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable20.Rows[index7]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable20.Rows[index7]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable20.Rows[index7]["PRODEN"]?.ToString() + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL AND TIPMOV='RT')," + " IMPOCCDEL = (SELECT VALUE(SUM(IMPOCCDEL), 0) FROM DENDET" + " WHERE CODPOS = " + dataTable20.Rows[index7]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable20.Rows[index7]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable20.Rows[index7]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable20.Rows[index7]["PRODEN"]?.ToString() + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL AND TIPMOV='RT')," + " IMPCONDEL = (SELECT VALUE(SUM(IMPCONDEL), 0) FROM DENDET" + " WHERE CODPOS = " + dataTable20.Rows[index7]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable20.Rows[index7]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable20.Rows[index7]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable20.Rows[index7]["PRODEN"]?.ToString() + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL AND TIPMOV='RT')," + " IMPSANRET = (SELECT VALUE(SUM(IMPSANDET), 0.0m) FROM DENDET" + " WHERE CODPOS = " + dataTable20.Rows[index7]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable20.Rows[index7]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable20.Rows[index7]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable20.Rows[index7]["PRODEN"]?.ToString() + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL " + " AND TIPMOV = 'RT' AND IMPRETDEL<>0 AND NUMSAN IS NOT NULL)" + " WHERE CODPOS = " + dataTable20.Rows[index7]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable20.Rows[index7]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable20.Rows[index7]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable20.Rows[index7]["PRODEN"]?.ToString();
          objDAtaAccess.WriteTransactionData(strSQL51, CommandType.Text);
          string strSQL52 = "UPDATE DENTES SET " + " IMPADDRECDEL = ROUND( IMPCONDEL * " + num24.ToString().Replace(",", ".") + "/100, 2)" + " WHERE CODPOS = " + dataTable20.Rows[index7]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable20.Rows[index7]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable20.Rows[index7]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable20.Rows[index7]["PRODEN"]?.ToString();
          objDAtaAccess.WriteTransactionData(strSQL52, CommandType.Text);
          num24 = 0.0M;
        }
        if (DTSTAMPE != null)
        {
          string strSQL53 = "SELECT ANNRET, PRORET, PRORETTES FROM RETTES WHERE ANNRET=" + year.ToString() + " AND PRORET =" + PRORET.ToString() + " AND IMPCONDEL<>0";
          DTSTAMPE = objDAtaAccess.GetDataTable(strSQL53);
        }
        if (FLGRETAPP)
        {
          string strSQL54 = "UPDATE MODRDL SET DATELA= " + DBMethods.Db2Date(str1) + " WHERE CODPOS =" + dgDati.Rows[0]["CODPOS"].ToString() + " AND DATELA IS NULL";
          objDAtaAccess.WriteTransactionData(strSQL54, CommandType.Text);
        }
      }
      catch (Exception ex)
      {
        ErroreMSG = "Errore durante la procedura";
        return false;
      }
      return true;
    }

    public DataTable Module_GeneraRettifiche(
      DataLayer objDAtaAccess,
      TFI.OCM.Utente.Utente u,
      int CODPOS,
      object MESE,
      object MESE_A,
      string MAT,
      string ANNO,
      string ANNO_A,
      bool FLGDP = false,
      bool FLGARR = false,
      bool FLGPREV = false)
    {
      string str1 = "";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      int num = 0;
      if (Convert.ToInt32(ANNO) < 2003)
        MESE = (object) "00";
      if (Convert.ToInt32(ANNO_A) < 2003)
        MESE_A = (object) "12";
      try
      {
        string str2 = "SELECT DISTINCT DENDET.CODPOS, DENDET.ANNDEN, DENDET.MESDEN, DENDET.PRODEN, " + " DENDET.MAT  " + " FROM DENDET INNER JOIN DENTES ON" + " DENDET.CODPOS = DENTES.CODPOS AND" + " DENDET.ANNDEN = DENTES.ANNDEN AND" + " DENDET.MESDEN = DENTES.MESDEN AND" + " DENDET.PRODEN = DENTES.PRODEN " + " WHERE DENDET.CODPOS = " + CODPOS.ToString();
        if (!FLGPREV)
          str2 += " AND DENTES.NUMMOV IS NOT NULL";
        string strSQL1 = str2 + " AND DENTES.NUMMOVANN IS NULL";
        if (FLGPREV)
        {
          if (FLGARR)
            strSQL1 += " AND DENTES.TIPMOV = 'AR' ";
          if (FLGDP)
            strSQL1 += " AND DENTES.TIPMOV <> 'AR' ";
        }
        if (ANNO != "" & MESE == null)
          str1 = str1 + " AND TRIM(CHAR(RIGHT('0000' || DENDET.ANNDEN , 4))) >= " + DBMethods.DoublePeakForSql(ANNO);
        else if (MESE != null && ANNO != "" & MESE.ToString() == "")
          str1 = str1 + " AND TRIM(CHAR(RIGHT('0000' || DENDET.ANNDEN , 4))) >= " + DBMethods.DoublePeakForSql(ANNO);
        if (ANNO == "" & MESE != null && MESE.ToString() != "")
          str1 = str1 + " AND TRIM(CHAR(RIGHT('00' || DENDET.MESDEN , 2))) >= " + DBMethods.DoublePeakForSql(MESE.ToString().PadLeft(2, '0'));
        if (ANNO != "" & MESE != null && MESE.ToString() != "")
          str1 = str1 + " AND TRIM(CHAR(RIGHT('0000' || DENDET.ANNDEN , 4))) || TRIM(CHAR(RIGHT('00' || DENDET.MESDEN , 2))) >= " + DBMethods.DoublePeakForSql(ANNO + MESE.ToString().PadLeft(2, '0'));
        if (ANNO_A != "" & MESE_A == null)
          str1 = str1 + " AND TRIM(CHAR(RIGHT('0000' || DENDET.ANNDEN , 4))) <= " + DBMethods.DoublePeakForSql(ANNO_A);
        else if (MESE_A != null && ANNO_A != "" & MESE_A.ToString() == "")
          str1 = str1 + " AND TRIM(CHAR(RIGHT('0000' || DENDET.ANNDEN , 4))) <= " + DBMethods.DoublePeakForSql(ANNO_A);
        if (ANNO_A == "" & MESE_A != null && MESE_A.ToString() != "")
          str1 = str1 + " AND TRIM(CHAR(RIGHT('00' || DENDET.MESDEN , 2))) <= " + DBMethods.DoublePeakForSql(MESE_A.ToString().PadLeft(2, '0'));
        if (ANNO_A != "" & MESE_A != null && MESE_A.ToString() != "")
          str1 = str1 + " AND TRIM(CHAR(RIGHT('0000' || DENDET.ANNDEN , 4))) || TRIM(CHAR(RIGHT('00' || DENDET.MESDEN , 2))) <= " + DBMethods.DoublePeakForSql(ANNO_A + MESE_A.ToString().PadLeft(2, '0'));
        if (MAT != "")
          str1 = str1 + " AND DENDET.MAT = " + MAT;
        if (str1 != "")
          strSQL1 = strSQL1 + str1 + " ORDER BY DENDET.ANNDEN, DENDET.MESDEN, DENDET.MAT";
        DataTable dataTable4 = objDAtaAccess.GetDataTable(strSQL1);
        for (int index1 = 0; index1 <= dataTable4.Rows.Count - 1; ++index1)
        {
          string str3 = "SELECT CODPOS, NUMGRURETRIF, ANNDEN, MESDEN, PRODEN, " + " PRODENDET, MAT, TIPMOV, " + " IMPOCC, IMPFIG, IMPABB, " + " VALUE(IMPOCCPRE,0.0) AS IMPOCCPRE,  VALUE(IMPFIGPRE,0.0) AS IMPFIGPRE, VALUE(IMPRETPRE,0.0) AS IMPRETPRE," + "  VALUE(IMPSANDETPRE,0.0) AS IMPSANDETPRE,  VALUE(IMPCONPRE,0.0) AS IMPCONPRE,  VALUE(IMPABBPRE,0.0) AS IMPABBPRE,  " + " VALUE(IMPASSCONPRE,0.0) AS IMPASSCONPRE, " + " IMPABBDEL, IMPASSCONDEL, " + " 0.0 AS IMPRETBK, 0.0 AS IMPOCCBK, 0.0 AS IMPFIGBK, " + " 0.0 AS IMPCONBK, 0.0 AS IMPSANDETBK," + " 0.0 AS IMPABBBK, 0.0 AS IMPASSCONBK, " + " IMPASSCON, IMPCON, IMPMIN, DATDEC, " + " DATCES, NUMGGAZI, NUMGGFIG, NUMGGPER, " + " NUMGGDOM, NUMGGSOS, NUMGGCONAZI, ";
          if (FLGPREV)
            str3 = str3 + " (" + " SELECT MESE FROM MESI WHERE CODMES = DENDET.MESDEN " + " )AS DENMESE, ";
          string strSQL2 = str3 + " IMPSCA, IMPTRAECO, ETA65, VALUE(TIPRAP, 0) AS TIPRAP, " + " FAP, VALUE(PERFAP, 0.0) AS PERFAP, IMPFAP, VALUE(PERPAR, 0.0) AS PERPAR, " + " PRORAP, CODCON, PROCON, TIPSPE, " + " CODLOC, PROLOC, CODLIV, CODGRUASS, " + " CODQUACON,  ALIQUOTA, DATNAS, DATINPS, " + " DATSAP, ANNCOM, IMPSANDET, NUMMOV, " + " ULTAGG, UTEAGG, TIPDEN, DATERO, DATINISAN, VALUE(DATFINSAN, CURRENT_DATE)  AS DATFINSAN, DAL, AL," + " NUMGRURET, " + " 0 AS NUMGGRITARDO, VALUE(TASSAN,0) AS TASSAN, " + " (SELECT TIPMOV FROM DENTES WHERE" + " DENDET.CODPOS = DENTES.CODPOS AND" + " DENDET.ANNDEN = DENTES.ANNDEN AND" + " DENDET.MESDEN = DENTES.MESDEN AND" + " DENDET.PRODEN = DENTES.PRODEN) AS TIPMOV_DENTES," + " IMPRET, PERAPP, ESIRET, PREV, IMPRETDEL, IMPOCCDEL, IMPFIGDEL, IMPCONDEL, " + " (SELECT  COUNT(*) FROM SOSRAP, CODSOS " + " WHERE SOSRAP.CODSOS = CODSOS.CODSOS " + " AND PERFIG>0 AND VALUE(SOSRAP.STASOS, 0) = 0" + " AND SOSRAP.CODPOS=DENDET.CODPOS AND SOSRAP.MAT=DENDET.MAT AND SOSRAP.PRORAP=DENDET.PRORAP " + " AND DATINISOS<=DENDET.AL AND DATFINSOS>=DENDET.DAL) AS SOS " + " FROM DENDET " + " WHERE CODPOS = '" + CODPOS.ToString() + "'" + " AND ANNDEN = '" + dataTable4.Rows[index1]["ANNDEN"]?.ToString() + "'" + " AND MESDEN = '" + dataTable4.Rows[index1]["MESDEN"]?.ToString() + "'" + " AND PRODEN = '" + dataTable4.Rows[index1]["PRODEN"]?.ToString() + "'" + " AND MAT = '" + dataTable4.Rows[index1][nameof (MAT)]?.ToString() + "'" + " AND VALUE(ESIRET, '') <> 'S' " + " AND NUMMOVANN IS NULL";
          dataTable1.Clear();
          dataTable1 = objDAtaAccess.GetDataTable(strSQL2);
          if (num == 0 && dataTable1.Rows.Count > 0)
          {
            dataTable3 = dataTable1.Clone();
            num = 1;
          }
          for (int index2 = 0; index2 <= dataTable1.Rows.Count - 1; ++index2)
            dataTable3.ImportRow(dataTable1.Rows[index2]);
        }
      }
      catch (Exception ex)
      {
        return (DataTable) null;
      }
      return dataTable3;
    }

    public int Module_ContaRettificheDaContabilizzare(DataLayer objDAtaAccess, TFI.OCM.Utente.Utente u)
    {
      try
      {
        string strSQL = "SELECT COUNT(*) AS TOT " + " FROM DENDET " + " WHERE " + " TIPMOV = 'RT' " + " AND NUMMOV IS NULL" + " AND NUMMOVANN IS NULL";
        return Convert.ToInt32(objDAtaAccess.Get1ValueFromSQL(strSQL, CommandType.Text));
      }
      catch (Exception ex)
      {
        return 0;
      }
    }

    public DataTable Module_RettificheDaContabilizzare(
      DataLayer objDAtaAccess,
      TFI.OCM.Utente.Utente u,
      int CODPOS = 0,
      string STRCODPOS = "",
      int MAT = 0)
    {
      DataTable dataTable = new DataTable();
      try
      {
        string str = "SELECT DENTES.TIPMOV AS DENTES_TIPMOV, DENDET.*, " + " CASE WHEN DENDET.IMPCONDEL < 0 THEN '-' ELSE '+' END AS TIPIMP " + " FROM DENTES, DENDET " + " WHERE DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN ";
        if (CODPOS != 0)
          str = !(STRCODPOS != "") ? str + " AND DENTES.CODPOS = " + CODPOS.ToString() : str + " AND DENTES.CODPOS IN ( " + STRCODPOS + " )";
        if (MAT != 0)
          str = str + " AND DENDET.MAT=" + MAT.ToString();
        string strSQL = str + " AND DENDET.TIPMOV = 'RT' " + " AND DENDET.NUMMOV IS NULL" + " AND DENDET.NUMMOVANN IS NULL" + " ORDER BY DENTES.CODPOS, DENTES.ANNDEN ";
        return objDAtaAccess.GetDataTable(strSQL);
      }
      catch (Exception ex)
      {
        return (DataTable) null;
      }
    }

    public void Module_Rettifiche_01(
      DataLayer objDAtaAccess,
      TFI.OCM.Utente.Utente u,
      ref DataTable dtNot,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int MAT,
      string TIPMOVSAN,
      string DATA_ORA_SISTEMA,
      string TIPPRI,
      string TIPISC,
      string DATINISAN,
      string DATFINSAN,
      string PREV = "N")
    {
      Decimal TASSO = 0.0M;
      IDOC_RDL idocRdl = new IDOC_RDL();
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      ModGetDati modGetDati = new ModGetDati();
      string CODCAUSAN = "";
      DataTable dataTable = new DataTable();
      DataTable DT = new DataTable();
      for (int index = 0; index <= dtNot.Rows.Count - 1; ++index)
      {
        Decimal num1 = Convert.ToDecimal(dtNot.Rows[index]["IMPRET"]);
        Decimal num2 = Convert.ToDecimal(dtNot.Rows[index]["IMPFIG"]);
        Decimal num3 = Convert.ToDecimal(dtNot.Rows[index]["IMPCON"]);
        Decimal num4 = Convert.ToDecimal(dtNot.Rows[index]["IMPABB"]);
        Decimal num5 = Convert.ToDecimal(dtNot.Rows[index]["IMPASSCON"]);
        Decimal PERMAXSOGLIA = 0.0M;
        Decimal IMPSANDET = modGetDati.MODULE_GENERA_SANZIONE(objDAtaAccess, ref PERMAXSOGLIA, num1, ref TASSO, (Decimal) dtNot.Rows[index]["ALIQUOTA"], TIPMOVSAN, DATINISAN, DATFINSAN, ref CODCAUSAN, ANNO: ANNDEN);
        int PRODENDET = clsWriteDb.WRITE_INSERT_DENDET(objDAtaAccess, u, CODPOS, ANNDEN, MESDEN, PRODEN, MAT, dtNot.Rows[index]["DAL"].ToString(), dtNot.Rows[index]["AL"].ToString(), dtNot.Rows[index]["FAP"].ToString(), dtNot.Rows[index]["PERFAP"].ToString(), num1, 0.0M, num2, num4, num5, num3, (Decimal) dtNot.Rows[index]["IMPMIN"], PREV, (int) dtNot.Rows[index]["PRORAP"], (int) dtNot.Rows[index]["CODCON"], (int) dtNot.Rows[index]["PROCON"], (int) dtNot.Rows[index]["CODLOC"], (int) dtNot.Rows[index]["PROLOC"], (int) dtNot.Rows[index]["CODLIV"], (int) dtNot.Rows[index]["CODQUACON"], dtNot.Rows[index]["DATNAS"].ToString(), dtNot.Rows[index]["ETAENNE"].ToString(), "RT", dtNot.Rows[index]["DATDEC"].ToString(), dtNot.Rows[index]["DATCES"].ToString(), (Decimal) dtNot.Rows[index]["NUMGGAZI"], (Decimal) dtNot.Rows[index]["NUMGGFIG"], (Decimal) dtNot.Rows[index]["NUMGGPER"], (Decimal) dtNot.Rows[index]["NUMGGDOM"], (Decimal) dtNot.Rows[index]["NUMGGSOS"], (Decimal) dtNot.Rows[index]["NUMGGCONAZI"], (Decimal) dtNot.Rows[index]["IMPSCA"], (Decimal) dtNot.Rows[index]["IMPTRAECO"], (string) dtNot.Rows[index]["TIPRAP"], (Decimal) dtNot.Rows[index]["PERPAR"], (Decimal) dtNot.Rows[index]["PERAPP"], dtNot.Rows[index]["TIPSPE"].ToString(), (int) dtNot.Rows[index]["CODGRUASS"], (Decimal) dtNot.Rows[index]["ALIQUOTA"], TIPISC, DATINISAN, DATFINSAN, TASSO, Convert.ToDecimal(TIPPRI), num1, IMPFIGDEL: num2, IMPCONDEL: num3, IMPSANDET: IMPSANDET, CODCAUSAN: CODCAUSAN, IMPABBDEL: num4, IMPASSCONDEL: num5);
        if (PREV == "X")
        {
          DT.Clear();
          DT = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9004", CODPOS, MAT, (int) dtNot.Rows[index]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET);
          idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, DT.Rows[0]);
          idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9004", DT, false);
        }
      }
    }

    public void Module_Rettifiche_02(
      DataLayer objDAtaAccess,
      TFI.OCM.Utente.Utente u,
      ref DataTable dtDet,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int MAT,
      string TIPMOVSAN,
      string DATA_ORA_SISTEMA,
      string TIPPRI,
      string TIPISC,
      string DATINISAN,
      string DATFINSAN,
      string PREV = "N")
    {
      Decimal IMPRET = 0.0M;
      Decimal IMPOCC = 0.0M;
      Decimal IMPFIG = 0.0M;
      Decimal IMPSANDET = 0.0M;
      Decimal IMPCON = 0.0M;
      Decimal IMPABB = 0.0M;
      Decimal IMPASSCON = 0.0M;
      Decimal TASSO = 0.0M;
      IDOC_RDL idocRdl = new IDOC_RDL();
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      string CODCAUSAN = "";
      DataTable dataTable = new DataTable();
      DataTable DT = new DataTable();
      for (int index = 0; index <= dtDet.Rows.Count - 1; ++index)
      {
        string strSQL = "UPDATE DENDET SET ESIRET = 'S' WHERE" + " CODPOS=" + CODPOS.ToString() + " AND ANNDEN=" + ANNDEN.ToString() + " AND MESDEN=" + MESDEN.ToString() + " AND PRODEN=" + PRODEN.ToString() + " AND MAT=" + MAT.ToString() + " AND PRODENDET=" + dtDet.Rows[index]["PRODENDET"]?.ToString();
        objDAtaAccess.WriteTransactionData(strSQL, CommandType.Text);
        IMPRET = 0.0M;
        IMPOCC = 0.0M;
        IMPFIG = 0.0M;
        IMPCON = 0.0M;
        IMPABB = 0.0M;
        IMPASSCON = 0.0M;
        Decimal IMPRETPRE = Convert.ToDecimal(dtDet.Rows[index]["IMPRET"]);
        Decimal IMPOCCPRE = Convert.ToDecimal(dtDet.Rows[index]["IMPOCC"]);
        Decimal IMPFIGPRE = Convert.ToDecimal(dtDet.Rows[index]["IMPFIG"]);
        Decimal IMPCONPRE = Convert.ToDecimal(dtDet.Rows[index]["IMPCON"]);
        Decimal IMPABBPRE = Convert.ToDecimal(dtDet.Rows[index]["IMPABB"]);
        Decimal IMPASSCONPRE = Convert.ToDecimal(dtDet.Rows[index]["IMPASSCON"]);
        Decimal IMPSANDETPRE = Convert.ToDecimal(dtDet.Rows[index]["IMPSANDET"]);
        Decimal IMPRETDEL = Convert.ToDecimal(IMPRET) - Convert.ToDecimal(IMPRETPRE);
        Decimal IMPOCCDEL = Convert.ToDecimal(IMPOCC) - Convert.ToDecimal(IMPOCCPRE);
        Decimal IMPFIGDEL = Convert.ToDecimal(IMPFIG) - Convert.ToDecimal(IMPFIGPRE);
        Decimal IMPCONDEL = Convert.ToDecimal(IMPCON) - Convert.ToDecimal(IMPCONPRE);
        Decimal IMPABBDEL = Convert.ToDecimal(IMPABB) - Convert.ToDecimal(IMPABBPRE);
        Decimal IMPASSCONDEL = Convert.ToDecimal(IMPASSCON) - Convert.ToDecimal(IMPASSCONPRE);
        IMPSANDET = 0.0M;
        int PRODENDET = clsWriteDb.WRITE_INSERT_DENDET(objDAtaAccess, u, CODPOS, ANNDEN, MESDEN, PRODEN, (int) dtDet.Rows[index][nameof (MAT)], dtDet.Rows[index]["DAL"].ToString(), dtDet.Rows[index]["AL"].ToString(), dtDet.Rows[index]["FAP"].ToString(), dtDet.Rows[index]["PERFAP"].ToString(), IMPRET, IMPOCC, IMPFIG, IMPABB, IMPASSCON, IMPCON, (Decimal) dtDet.Rows[index]["IMPMIN"], dtDet.Rows[index][nameof (PREV)].ToString(), (int) dtDet.Rows[index]["PRORAP"], (int) dtDet.Rows[index]["CODCON"], (int) dtDet.Rows[index]["PROCON"], (int) dtDet.Rows[index]["CODLOC"], (int) dtDet.Rows[index]["PROLOC"], (int) dtDet.Rows[index]["CODLIV"], (int) dtDet.Rows[index]["CODQUACON"], dtDet.Rows[index]["DATNAS"].ToString(), dtDet.Rows[index]["ETA65"].ToString(), "RT", dtDet.Rows[index]["DATDEC"].ToString(), dtDet.Rows[index]["DATCES"].ToString(), (Decimal) dtDet.Rows[index]["NUMGGAZI"], (Decimal) dtDet.Rows[index]["NUMGGFIG"], (Decimal) dtDet.Rows[index]["NUMGGPER"], (Decimal) dtDet.Rows[index]["NUMGGDOM"], (Decimal) dtDet.Rows[index]["NUMGGSOS"], (Decimal) dtDet.Rows[index]["NUMGGCONAZI"], (Decimal) dtDet.Rows[index]["IMPSCA"], (Decimal) dtDet.Rows[index]["IMPTRAECO"], dtDet.Rows[index]["TIPRAP"].ToString(), (Decimal) dtDet.Rows[index]["PERPAR"], (Decimal) dtDet.Rows[index]["PERAPP"], dtDet.Rows[index]["TIPSPE"].ToString(), (int) dtDet.Rows[index]["CODGRUASS"], (Decimal) dtDet.Rows[index]["ALIQUOTA"], dtDet.Rows[index]["TIPDEN"].ToString(), DATINISAN, DATFINSAN, TASSO, Convert.ToDecimal(TIPPRI), IMPRETDEL, IMPOCCDEL, IMPFIGDEL, IMPCONDEL, IMPRETPRE, IMPOCCPRE, IMPFIGPRE, IMPSANDETPRE, IMPCONPRE, IMPSANDET, CODCAUSAN, IMPABBPRE, IMPASSCONPRE, IMPABBDEL, IMPASSCONDEL, dtDet.Rows[index]["ANNCOM"].ToString().Trim());
        if (PREV == "X")
        {
          DT.Clear();
          DT = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9004", CODPOS, MAT, (int) dtDet.Rows[index]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET);
          idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, DT.Rows[0]);
          idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9004", DT, false);
        }
      }
    }

    public void Module_Rettifiche_03(
      DataLayer objDAtaAccess,
      TFI.OCM.Utente.Utente u,
      ref DataTable dtDet,
      ref DataTable dtNot,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int MAT,
      string TIPMOVSAN,
      string DATA_ORA_SISTEMA,
      string TIPPRI,
      string TIPISC,
      string DATINISAN,
      string DATFINSAN,
      string PREV = "N")
    {
      string AL1 = "";
      Decimal num1 = 0.0M;
      Decimal IMPRETPRE1 = 0.0M;
      Decimal IMPOCCPRE1 = 0.0M;
      Decimal IMPFIGPRE1 = 0.0M;
      Decimal IMPSANDETPRE1 = 0.0M;
      Decimal IMPCONPRE1 = 0.0M;
      Decimal num2 = 0.0M;
      Decimal num3 = 0.0M;
      Decimal IMPABBPRE1 = 0.0M;
      Decimal IMPASSCONPRE1 = 0.0M;
      Decimal TASSO = 0.0M;
      bool flag = false;
      string CODCAUSAN = "";
      DataTable dataTable = new DataTable();
      DataTable DT = new DataTable();
      IDOC_RDL idocRdl = new IDOC_RDL();
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      ModGetDati modGetDati = new ModGetDati();
      string strSQL = "UPDATE DENDET SET ESIRET = 'S' WHERE" + " CODPOS=" + CODPOS.ToString() + " AND ANNDEN=" + ANNDEN.ToString() + " AND MESDEN=" + MESDEN.ToString() + " AND PRODEN=" + PRODEN.ToString() + " AND MAT=" + MAT.ToString() + " AND PRODENDET=" + dtDet.Rows[0]["PRODENDET"]?.ToString();
      objDAtaAccess.WriteTransactionData(strSQL, CommandType.Text);
      Decimal num4 = !(dtDet.Rows[0]["TIPMOV"].ToString().Trim() == "NU") ? ((int) dtDet.Rows[0]["NUMGGPER"] != 0 ? Decimal.Round(Convert.ToDecimal((Decimal) dtDet.Rows[0]["IMPRET"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[0]["NUMGGPER"]), 0) : (Decimal) dtDet.Rows[0]["IMPRET"]) : ((int) dtDet.Rows[0]["NUMGGPER"] != 0 ? Decimal.Round(Convert.ToDecimal((Decimal) dtNot.Rows[0]["IMPMIN"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[0]["NUMGGPER"]), 0) : (Decimal) dtNot.Rows[0]["IMPMIN"]);
      Decimal IMPOCC1 = (int) dtDet.Rows[0]["NUMGGPER"] != 0 ? (Decimal) dtDet.Rows[0]["IMPOCC"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[0]["NUMGGPER"] : (Decimal) dtDet.Rows[0]["IMPOCC"];
      num1 = num4 * (Decimal) dtNot.Rows[0]["ALIQUOTA"] / 100M;
      Decimal IMPFIG1 = (Decimal) dtNot.Rows[0]["IMPFIG"];
      Decimal IMPABB = (Decimal) dtNot.Rows[0]["IMPABB"];
      Decimal IMPASSCON = (Decimal) dtNot.Rows[0]["IMPASSCON"];
      if (dtDet.Rows[0]["ALIQUOTA"] == dtNot.Rows[0]["ALIQUOTA"])
      {
        IMPRETPRE1 = (Decimal) dtDet.Rows[0]["IMPRET"];
        IMPOCCPRE1 = (Decimal) dtDet.Rows[0]["IMPOCC"];
        IMPCONPRE1 = (Decimal) dtDet.Rows[0]["IMPCON"];
        IMPFIGPRE1 = (Decimal) dtDet.Rows[0]["IMPFIG"];
        IMPSANDETPRE1 = (Decimal) dtDet.Rows[0]["IMPSANDET"];
        IMPABBPRE1 = (Decimal) dtDet.Rows[0]["IMPABB"];
        IMPASSCONPRE1 = (Decimal) dtDet.Rows[0]["IMPASSCON"];
        flag = true;
      }
      Decimal IMPRETDEL1 = Convert.ToDecimal(num4) - Convert.ToDecimal(IMPRETPRE1);
      Decimal IMPOCCDEL1 = Convert.ToDecimal(IMPOCC1) - Convert.ToDecimal(num2);
      Decimal IMPCON1;
      Decimal IMPCONDEL1;
      if (dtNot.Rows[0]["ALIQUOTA"] != dtDet.Rows[0]["ALIQUOTA"])
      {
        IMPCON1 = num4 * (Decimal) dtNot.Rows[0]["ALIQUOTA"] / 100M;
        IMPCONDEL1 = num4 * ((Decimal) dtDet.Rows[0]["ALIQUOTA"] - (Decimal) dtNot.Rows[0]["ALIQUOTA"]) / 100M * -1M;
        string[] strArray = IMPCONDEL1.ToString().Split(',');
        if (strArray.Length > 1 && strArray[1].Length > 2)
        {
          if (Convert.ToInt32(strArray[1].Substring(2)) >= 5)
          {
            Decimal int32 = (Decimal) Convert.ToInt32(IMPCONDEL1.ToString().Substring(0, IMPCONDEL1.ToString().Length - strArray[1].Length + 2));
            IMPCONDEL1 = !(int32 < 0M) ? int32 + 0.1M : int32 - 0.1M;
          }
          else
            IMPCONDEL1 = Convert.ToDecimal(IMPCONDEL1.ToString().Substring(0, IMPCONDEL1.ToString().Length - strArray[1].Length + 2));
        }
      }
      else
      {
        IMPCON1 = num4 * (Decimal) dtNot.Rows[0]["ALIQUOTA"] / 100M;
        IMPCONDEL1 = Convert.ToDecimal(IMPCON1) - Convert.ToDecimal(IMPCONPRE1);
        string[] strArray = IMPCONDEL1.ToString().Split(',');
        if (strArray.Length > 1 && strArray[1].Length > 2)
        {
          if (Convert.ToInt32(strArray[1].Substring(2)) >= 5)
          {
            Decimal int32 = (Decimal) Convert.ToInt32(IMPCONDEL1.ToString().Substring(0, IMPCONDEL1.ToString().Length - strArray[1].Length + 2));
            IMPCONDEL1 = !(int32 < 0M) ? int32 + 0.1M : int32 - 0.1M;
          }
          else
            IMPCONDEL1 = Convert.ToDecimal(IMPCONDEL1.ToString().Substring(0, IMPCONDEL1.ToString().Length - strArray[1].Length + 2));
        }
      }
      num3 = Convert.ToDecimal(IMPFIG1) - Convert.ToDecimal(IMPFIGPRE1);
      Decimal IMPFIGDEL1 = Convert.ToDecimal(IMPFIG1) - Convert.ToDecimal(IMPFIGPRE1);
      Decimal IMPABBDEL1 = Convert.ToDecimal(IMPABB) - Convert.ToDecimal(IMPABBPRE1);
      Decimal IMPASSCONDEL1 = Convert.ToDecimal(IMPASSCON) - Convert.ToDecimal(IMPASSCONPRE1);
      Decimal PERMAXSOGLIA = 0.0M;
      Decimal IMPSANDET1 = modGetDati.MODULE_GENERA_SANZIONE(objDAtaAccess, ref PERMAXSOGLIA, num4, ref TASSO, (Decimal) dtNot.Rows[0]["ALIQUOTA"], TIPMOVSAN, DATINISAN, DATFINSAN, ref CODCAUSAN, ANNO: ANNDEN);
      string str1 = dtDet.Rows[0]["TIPMOV"].ToString().Trim();
      DateTime dateTime;
      if (!(str1 == "DP") && !(str1 == "NU"))
      {
        if (str1 == "AR")
        {
          dateTime = Convert.ToDateTime(dtNot.Rows[0]["AL"]);
          AL1 = "31/12/" + dateTime.Year.ToString().Trim();
        }
      }
      else
        AL1 = dtNot.Rows[0]["AL"].ToString();
      int PRODENDET1 = clsWriteDb.WRITE_INSERT_DENDET(objDAtaAccess, u, CODPOS, ANNDEN, MESDEN, PRODEN, MAT, dtNot.Rows[0]["DAL"].ToString(), AL1, dtNot.Rows[0]["FAP"].ToString(), dtNot.Rows[0]["PERFAP"].ToString(), num4, IMPOCC1, IMPFIG1, IMPABB, IMPASSCON, IMPCON1, (Decimal) dtNot.Rows[0]["IMPMIN"], "N", (int) dtNot.Rows[0]["PRORAP"], (int) dtNot.Rows[0]["CODCON"], (int) dtNot.Rows[0]["PROCON"], (int) dtNot.Rows[0]["CODLOC"], (int) dtNot.Rows[0]["PROLOC"], (int) dtNot.Rows[0]["CODLIV"], (int) dtNot.Rows[0]["CODQUACON"], dtNot.Rows[0]["DATNAS"].ToString(), dtNot.Rows[0]["ETAENNE"].ToString(), "RT", dtNot.Rows[0]["DATDEC"].ToString(), dtNot.Rows[0]["DATCES"].ToString(), (Decimal) dtNot.Rows[0]["NUMGGAZI"], (Decimal) dtNot.Rows[0]["NUMGGFIG"], (Decimal) dtNot.Rows[0]["NUMGGPER"], (Decimal) dtNot.Rows[0]["NUMGGDOM"], (Decimal) dtNot.Rows[0]["NUMGGSOS"], (Decimal) dtNot.Rows[0]["NUMGGCONAZI"], (Decimal) dtNot.Rows[0]["IMPSCA"], (Decimal) dtNot.Rows[0]["IMPTRAECO"], dtNot.Rows[0]["TIPRAP"].ToString(), (Decimal) dtNot.Rows[0]["PERPAR"], (Decimal) dtNot.Rows[0]["PERAPP"], dtNot.Rows[0]["TIPSPE"].ToString(), (int) dtNot.Rows[0]["CODGRUASS"], (Decimal) dtNot.Rows[0]["ALIQUOTA"], TIPISC, DATINISAN, DATFINSAN, TASSO, Convert.ToDecimal(TIPPRI), IMPRETDEL1, IMPOCCDEL1, IMPFIGDEL1, IMPCONDEL1, IMPRETPRE1, IMPOCCPRE1, IMPFIGPRE1, IMPSANDETPRE1, IMPCONPRE1, IMPSANDET1, CODCAUSAN, IMPABBPRE1, IMPASSCONPRE1, IMPABBDEL1, IMPASSCONDEL1, dtDet.Rows[0]["ANNCOM"].ToString().Trim());
      if (PREV == "X")
      {
        string str2 = dtDet.Rows[0]["TIPMOV"].ToString().Trim();
        if (!(str2 == "DP") && !(str2 == "NU"))
        {
          if (str2 == "AR")
          {
            DT.Clear();
            DT = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9005", CODPOS, MAT, (int) dtNot.Rows[0]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET1);
            idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, DT.Rows[0]);
            idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9005", DT, false);
          }
        }
        else
        {
          DT.Clear();
          DT = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9004", CODPOS, MAT, (int) dtNot.Rows[0]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET1);
          idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, DT.Rows[0]);
          idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9004", DT, false);
        }
      }
      num4 = 0.0M;
      IMPOCC1 = 0.0M;
      IMPCON1 = 0.0M;
      IMPFIG1 = 0.0M;
      IMPSANDET1 = 0.0M;
      IMPABB = 0.0M;
      IMPASSCON = 0.0M;
      IMPRETPRE1 = 0.0M;
      IMPOCCPRE1 = 0.0M;
      IMPCONPRE1 = 0.0M;
      IMPFIGPRE1 = 0.0M;
      IMPSANDETPRE1 = 0.0M;
      IMPABBPRE1 = 0.0M;
      IMPASSCONPRE1 = 0.0M;
      IMPRETDEL1 = 0.0M;
      IMPOCCDEL1 = 0.0M;
      IMPCONDEL1 = 0.0M;
      IMPFIGDEL1 = 0.0M;
      IMPABBDEL1 = 0.0M;
      IMPASSCONDEL1 = 0.0M;
      string AL2 = "";
      Decimal num5 = !(dtDet.Rows[0]["TIPMOV"].ToString().Trim() == "NU") ? ((int) dtDet.Rows[0]["NUMGGPER"] != 0 ? Decimal.Round(Convert.ToDecimal((Decimal) dtDet.Rows[0]["IMPRET"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[1]["NUMGGPER"]), 0) : (Decimal) dtDet.Rows[0]["IMPRET"]) : ((int) dtDet.Rows[0]["NUMGGPER"] != 0 ? Decimal.Round(Convert.ToDecimal((Decimal) dtNot.Rows[1]["IMPMIN"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[1]["NUMGGPER"]), 0) : (Decimal) dtNot.Rows[1]["IMPMIN"]);
      Decimal IMPOCC2 = (int) dtDet.Rows[0]["NUMGGPER"] != 0 ? (Decimal) dtDet.Rows[0]["IMPOCC"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[1]["NUMGGPER"] : (Decimal) dtDet.Rows[0]["IMPOCC"];
      num1 = num5 * (Decimal) dtNot.Rows[1]["ALIQUOTA"] / 100M;
      Decimal IMPFIG2 = (Decimal) dtNot.Rows[1]["IMPFIG"];
      Decimal num6 = (Decimal) dtNot.Rows[1]["IMPABB"];
      Decimal num7 = (Decimal) dtNot.Rows[1]["IMPASSCON"];
      Decimal IMPRETPRE2 = (Decimal) dtDet.Rows[0]["IMPRET"];
      Decimal IMPOCCPRE2 = (Decimal) dtDet.Rows[0]["IMPOCC"];
      Decimal IMPCONPRE2 = (Decimal) dtDet.Rows[0]["IMPCON"];
      Decimal IMPFIGPRE2 = (Decimal) dtDet.Rows[0]["IMPFIG"];
      Decimal IMPSANDETPRE2 = (Decimal) dtDet.Rows[0]["IMPSANDET"];
      Decimal IMPABBPRE2 = (Decimal) dtDet.Rows[0]["IMPABB"];
      Decimal IMPASSCONPRE2 = (Decimal) dtDet.Rows[0]["IMPASSCON"];
      Decimal IMPRETDEL2 = Convert.ToDecimal(num5) - Convert.ToDecimal(IMPRETPRE2);
      Decimal IMPOCCDEL2 = Convert.ToDecimal(IMPOCC2) - Convert.ToDecimal(IMPOCCDEL1);
      Decimal IMPCON2;
      Decimal IMPCONDEL2;
      if (dtNot.Rows[1]["ALIQUOTA"] != dtDet.Rows[0]["ALIQUOTA"])
      {
        IMPCON2 = num5 * (Decimal) dtNot.Rows[0]["ALIQUOTA"] / 100M;
        IMPCONDEL2 = num5 * ((Decimal) dtDet.Rows[0]["ALIQUOTA"] - (Decimal) dtNot.Rows[1]["ALIQUOTA"]) / 100M * -1M;
        string[] strArray = IMPCONDEL2.ToString().Split(',');
        if (strArray.Length > 1 && strArray[1].Length > 2)
        {
          if (Convert.ToInt32(strArray[1].Substring(2)) >= 5)
          {
            Decimal num8 = Convert.ToDecimal(IMPCONDEL2.ToString().Substring(0, IMPCONDEL2.ToString().Length - strArray[1].Length + 2));
            IMPCONDEL2 = !(num8 < 0M) ? num8 + 0.1M : num8 - 0.1M;
          }
          else
            IMPCONDEL2 = Convert.ToDecimal(IMPCONDEL2.ToString().Substring(0, IMPCONDEL2.ToString().Length - strArray[1].Length + 2));
        }
      }
      else
      {
        IMPCON2 = num5 * (Decimal) dtNot.Rows[1]["ALIQUOTA"] / 100M;
        IMPCONDEL2 = Convert.ToDecimal(IMPCON2) - Convert.ToDecimal(IMPCONPRE2);
        string[] strArray = IMPCONDEL2.ToString().Split(',');
        if (strArray.Length > 1 && strArray[1].Length > 2)
        {
          if (Convert.ToInt32(strArray[1].Substring(2)) >= 5)
          {
            Decimal num9 = Convert.ToDecimal(IMPCONDEL2.ToString().Substring(0, IMPCONDEL2.ToString().Length - strArray[1].Length + 2));
            IMPCONDEL2 = !(num9 < 0M) ? num9 + 0.0M : num9 - 0.0M;
          }
          else
            IMPCONDEL2 = Convert.ToDecimal(IMPCONDEL2.ToString().Substring(0, IMPCONDEL2.ToString().Length - strArray[1].Length + 2));
        }
      }
      Decimal IMPFIGDEL2 = Convert.ToDecimal(IMPFIG2) - Convert.ToDecimal(IMPFIGPRE2);
      Decimal IMPABBDEL2 = Convert.ToDecimal(num6) - Convert.ToDecimal(IMPABBPRE2);
      Decimal IMPASSCONDEL2 = Convert.ToDecimal(num7) - Convert.ToDecimal(IMPASSCONPRE2);
      Decimal IMPSANDET2 = modGetDati.MODULE_GENERA_SANZIONE(objDAtaAccess, ref PERMAXSOGLIA, num5, ref TASSO, (Decimal) dtNot.Rows[1]["ALIQUOTA"], TIPMOVSAN, DATINISAN, DATFINSAN, ref CODCAUSAN, ANNO: ANNDEN);
      string str3 = dtDet.Rows[0]["TIPMOV"].ToString().Trim();
      if (!(str3 == "DP") && !(str3 == "NU"))
      {
        if (str3 == "AR")
        {
          dateTime = Convert.ToDateTime(dtNot.Rows[1]["AL"]);
          AL2 = "31/12/" + dateTime.Year.ToString().Trim();
        }
      }
      else
        AL2 = dtNot.Rows[1]["AL"].ToString();
      int PRODENDET2 = clsWriteDb.WRITE_INSERT_DENDET(objDAtaAccess, u, CODPOS, ANNDEN, MESDEN, PRODEN, MAT, dtNot.Rows[1]["DAL"].ToString(), AL2, dtNot.Rows[1]["FAP"].ToString(), dtNot.Rows[1]["PERFAP"].ToString(), num5, IMPOCC2, IMPFIG2, (Decimal) dtNot.Rows[1]["IMPABB"], (Decimal) dtNot.Rows[1]["IMPASSCON"], IMPCON2, (Decimal) dtNot.Rows[1]["IMPMIN"], "N", (int) dtNot.Rows[1]["PRORAP"], (int) dtNot.Rows[1]["CODCON"], (int) dtNot.Rows[1]["PROCON"], (int) dtNot.Rows[1]["CODLOC"], (int) dtNot.Rows[1]["PROLOC"], (int) dtNot.Rows[1]["CODLIV"], (int) dtNot.Rows[1]["CODQUACON"], dtNot.Rows[1]["DATNAS"].ToString(), dtNot.Rows[1]["ETAENNE"].ToString(), "RT", dtNot.Rows[0]["DATDEC"].ToString(), dtNot.Rows[1]["DATCES"].ToString(), (Decimal) dtNot.Rows[1]["NUMGGAZI"], (Decimal) dtNot.Rows[1]["NUMGGFIG"], (Decimal) dtNot.Rows[1]["NUMGGPER"], (Decimal) dtNot.Rows[1]["NUMGGDOM"], (Decimal) dtNot.Rows[1]["NUMGGSOS"], (Decimal) dtNot.Rows[1]["NUMGGCONAZI"], (Decimal) dtNot.Rows[1]["IMPSCA"], (Decimal) dtNot.Rows[1]["IMPTRAECO"], dtNot.Rows[1]["TIPRAP"].ToString(), (Decimal) dtNot.Rows[1]["PERPAR"], (Decimal) dtNot.Rows[1]["PERAPP"], dtNot.Rows[1]["TIPSPE"].ToString(), (int) dtNot.Rows[1]["CODGRUASS"], (Decimal) dtNot.Rows[1]["ALIQUOTA"], TIPISC, DATINISAN, DATFINSAN, TASSO, Convert.ToDecimal(TIPPRI), IMPRETDEL2, IMPOCCDEL2, IMPFIGDEL2, IMPCONDEL2, IMPRETPRE2, IMPOCCPRE2, IMPFIGPRE2, IMPSANDETPRE2, IMPCONPRE2, IMPSANDET2, CODCAUSAN, IMPABBPRE2, IMPASSCONPRE2, IMPABBDEL2, IMPASSCONDEL2, dtDet.Rows[0]["ANNCOM"].ToString().Trim());
      if (!(PREV == "X"))
        return;
      string str4 = dtDet.Rows[0]["TIPMOV"].ToString().Trim();
      if (!(str4 == "DP") && !(str4 == "NU"))
      {
        if (!(str4 == "AR"))
          return;
        DT.Clear();
        DataTable idocDatiE1Pitype = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9005", CODPOS, MAT, (int) dtNot.Rows[1]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET2);
        idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, idocDatiE1Pitype.Rows[0]);
        idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9005", idocDatiE1Pitype, false);
      }
      else
      {
        DT.Clear();
        DataTable idocDatiE1Pitype = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9004", CODPOS, MAT, (int) dtNot.Rows[1]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET2);
        idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, idocDatiE1Pitype.Rows[0]);
        idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9004", idocDatiE1Pitype, false);
      }
    }

    public void Module_Rettifiche_04(
      DataLayer objDAtaAccess,
      TFI.OCM.Utente.Utente u,
      ref DataTable dtDet,
      ref DataTable dtNot,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int MAT,
      string TIPMOVSAN,
      string DATA_ORA_SISTEMA,
      string TIPPRI,
      string TIPISC,
      string DATINISAN,
      string DATFINSAN,
      string PREV = "N")
    {
      Decimal IMPRETPRE = 0.0M;
      Decimal IMPOCCPRE = 0.0M;
      Decimal IMPFIGPRE = 0.0M;
      Decimal IMPSANDETPRE = 0.0M;
      Decimal IMPCONPRE = 0.0M;
      Decimal num1 = 0.0M;
      Decimal IMPABBPRE = 0.0M;
      Decimal IMPASSCONPRE = 0.0M;
      Decimal TASSO = 0.0M;
      string CODCAUSAN = "";
      bool flag = false;
      DataTable dataTable = new DataTable();
      DataTable DT = new DataTable();
      IDOC_RDL idocRdl = new IDOC_RDL();
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      ModGetDati modGetDati = new ModGetDati();
      string strSQL = "UPDATE DENDET SET ESIRET = 'S' WHERE" + " CODPOS=" + CODPOS.ToString() + " AND ANNDEN=" + ANNDEN.ToString() + " AND MESDEN=" + MESDEN.ToString() + " AND PRODEN=" + PRODEN.ToString() + " AND MAT=" + MAT.ToString() + " AND PRODENDET=" + dtDet.Rows[0]["PRODENDET"]?.ToString();
      objDAtaAccess.WriteTransactionData(strSQL, CommandType.Text);
      Decimal num2 = !(dtDet.Rows[0]["TIPMOV"].ToString().Trim() == "NU") ? ((int) dtDet.Rows[0]["NUMGGPER"] != 0 ? Decimal.Round(Convert.ToDecimal((Decimal) dtDet.Rows[0]["IMPRET"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[0]["NUMGGPER"]), 0) : (Decimal) dtDet.Rows[0]["IMPRET"]) : ((int) dtDet.Rows[0]["NUMGGPER"] != 0 ? Decimal.Round(Convert.ToDecimal((Decimal) dtNot.Rows[0]["IMPMIN"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[0]["NUMGGPER"]), 0) : (Decimal) dtNot.Rows[0]["IMPMIN"]);
      Decimal IMPOCC1 = (int) dtDet.Rows[0]["NUMGGPER"] != 0 ? (Decimal) dtDet.Rows[0]["IMPOCC"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[0]["NUMGGPER"] : (Decimal) dtDet.Rows[0]["IMPOCC"];
      Decimal IMPCON1 = num2 * (Decimal) dtNot.Rows[0]["ALIQUOTA"] / 100M;
      Decimal IMPFIG1 = (Decimal) dtNot.Rows[0]["IMPFIG"];
      Decimal IMPABB1 = (Decimal) dtNot.Rows[0]["IMPABB"];
      Decimal IMPASSCON1 = (Decimal) dtNot.Rows[0]["IMPASSCON"];
      if (dtDet.Rows[0]["ALIQUOTA"] == dtNot.Rows[0]["ALIQUOTA"])
      {
        IMPRETPRE = (Decimal) dtDet.Rows[0]["IMPRET"];
        IMPOCCPRE = (Decimal) dtDet.Rows[0]["IMPOCC"];
        IMPCONPRE = (Decimal) dtDet.Rows[0]["IMPCON"];
        IMPFIGPRE = (Decimal) dtDet.Rows[0]["IMPFIG"];
        IMPSANDETPRE = (Decimal) dtDet.Rows[0]["IMPSANDET"];
        IMPABBPRE = (Decimal) dtDet.Rows[0]["IMPABB"];
        IMPASSCONPRE = (Decimal) dtDet.Rows[0]["IMPASSCON"];
        flag = true;
      }
      Decimal IMPRETDEL1 = Convert.ToDecimal(num2) - Convert.ToDecimal(IMPRETPRE);
      Decimal IMPOCCDEL1 = Convert.ToDecimal(IMPOCC1) - Convert.ToDecimal(num1);
      Decimal IMPCONDEL1 = Convert.ToDecimal(IMPCON1) - Convert.ToDecimal(IMPCONPRE);
      Decimal IMPFIGDEL1 = Convert.ToDecimal(IMPFIG1) - Convert.ToDecimal(IMPFIGPRE);
      Decimal IMPABBDEL1 = Convert.ToDecimal(IMPABB1) - Convert.ToDecimal(IMPABBPRE);
      Decimal IMPASSCONDEL1 = Convert.ToDecimal(IMPASSCON1) - Convert.ToDecimal(IMPASSCONPRE);
      Decimal PERMAXSOGLIA = 0.0M;
      Decimal IMPSANDET1 = modGetDati.MODULE_GENERA_SANZIONE(objDAtaAccess, ref PERMAXSOGLIA, num2, ref TASSO, (Decimal) dtNot.Rows[0]["ALIQUOTA"], TIPMOVSAN, DATINISAN, DATFINSAN, ref CODCAUSAN, ANNO: ANNDEN);
      int PRODENDET1 = clsWriteDb.WRITE_INSERT_DENDET(objDAtaAccess, u, CODPOS, ANNDEN, MESDEN, PRODEN, MAT, dtNot.Rows[0]["DAL"].ToString(), dtNot.Rows[0]["AL"].ToString(), dtNot.Rows[0]["FAP"].ToString(), dtNot.Rows[0]["PERFAP"].ToString(), num2, IMPOCC1, IMPFIG1, IMPABB1, IMPASSCON1, IMPCON1, (Decimal) dtNot.Rows[0]["IMPMIN"], "N", (int) dtNot.Rows[0]["PRORAP"], (int) dtNot.Rows[0]["CODCON"], (int) dtNot.Rows[0]["PROCON"], (int) dtNot.Rows[0]["CODLOC"], (int) dtNot.Rows[0]["PROLOC"], (int) dtNot.Rows[0]["CODLIV"], (int) dtNot.Rows[0]["CODQUACON"], dtNot.Rows[0]["DATNAS"].ToString(), dtNot.Rows[0]["ETAENNE"].ToString(), "RT", dtNot.Rows[0]["DATDEC"].ToString(), dtNot.Rows[0]["DATCES"].ToString(), (Decimal) dtNot.Rows[0]["NUMGGAZI"], (Decimal) dtNot.Rows[0]["NUMGGFIG"], (Decimal) dtNot.Rows[0]["NUMGGPER"], (Decimal) dtNot.Rows[0]["NUMGGDOM"], (Decimal) dtNot.Rows[0]["NUMGGSOS"], (Decimal) dtNot.Rows[0]["NUMGGCONAZI"], (Decimal) dtNot.Rows[0]["IMPSCA"], (Decimal) dtNot.Rows[0]["IMPTRAECO"], dtNot.Rows[0]["TIPRAP"].ToString(), (Decimal) dtNot.Rows[0]["PERPAR"], (Decimal) dtNot.Rows[0]["PERAPP"], dtNot.Rows[0]["TIPSPE"].ToString(), (int) dtNot.Rows[0]["CODGRUASS"], (Decimal) dtNot.Rows[0]["ALIQUOTA"], TIPISC, DATINISAN, DATFINSAN, TASSO, Convert.ToDecimal(TIPPRI), IMPRETDEL1, IMPOCCDEL1, IMPFIGDEL1, IMPCONDEL1, IMPRETPRE, IMPOCCPRE, IMPFIGPRE, IMPSANDETPRE, IMPCONPRE, IMPSANDET1, CODCAUSAN, IMPABBPRE, IMPASSCONPRE, IMPABBDEL1, IMPASSCONDEL1, dtDet.Rows[0]["ANNCOM"].ToString().Trim());
      if (PREV == "X")
      {
        DT.Clear();
        DT = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9004", CODPOS, MAT, (int) dtNot.Rows[0]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET1);
        idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, DT.Rows[0]);
        idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9004", DT, false);
      }
      num2 = 0.0M;
      IMPOCC1 = 0.0M;
      IMPCON1 = 0.0M;
      IMPFIG1 = 0.0M;
      IMPSANDET1 = 0.0M;
      IMPABB1 = 0.0M;
      IMPASSCON1 = 0.0M;
      IMPRETPRE = 0.0M;
      IMPOCCPRE = 0.0M;
      IMPCONPRE = 0.0M;
      IMPFIGPRE = 0.0M;
      IMPSANDETPRE = 0.0M;
      IMPABBPRE = 0.0M;
      IMPASSCONPRE = 0.0M;
      IMPRETDEL1 = 0.0M;
      IMPOCCDEL1 = 0.0M;
      IMPCONDEL1 = 0.0M;
      IMPFIGDEL1 = 0.0M;
      IMPABBDEL1 = 0.0M;
      IMPASSCONDEL1 = 0.0M;
      Decimal num3 = !(dtDet.Rows[0]["TIPMOV"].ToString().Trim() == "NU") ? ((int) dtDet.Rows[0]["NUMGGPER"] != 0 ? Decimal.Round(Convert.ToDecimal((Decimal) dtDet.Rows[0]["IMPRET"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[1]["NUMGGPER"]), 0) : (Decimal) dtDet.Rows[0]["IMPRET"]) : ((int) dtDet.Rows[0]["NUMGGPER"] != 0 ? Decimal.Round(Convert.ToDecimal((Decimal) dtNot.Rows[1]["IMPMIN"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[1]["NUMGGPER"]), 0) : (Decimal) dtNot.Rows[1]["IMPMIN"]);
      Decimal IMPOCC2 = (int) dtDet.Rows[0]["NUMGGPER"] != 0 ? (Decimal) dtDet.Rows[0]["IMPOCC"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[1]["NUMGGPER"] : (Decimal) dtDet.Rows[0]["IMPOCC"];
      Decimal IMPCON2 = num3 * (Decimal) dtNot.Rows[1]["ALIQUOTA"] / 100M;
      Decimal IMPFIG2 = (Decimal) dtNot.Rows[1]["IMPFIG"];
      Decimal IMPABB2 = (Decimal) dtNot.Rows[1]["IMPABB"];
      Decimal IMPASSCON2 = (Decimal) dtNot.Rows[1]["IMPASSCON"];
      if (!flag)
      {
        IMPRETPRE = (Decimal) dtDet.Rows[0]["IMPRET"];
        IMPOCCPRE = (Decimal) dtDet.Rows[0]["IMPOCC"];
        IMPCONPRE = (Decimal) dtDet.Rows[0]["IMPCON"];
        IMPFIGPRE = (Decimal) dtDet.Rows[0]["IMPFIG"];
        IMPSANDETPRE = (Decimal) dtDet.Rows[0]["IMPSANDET"];
        IMPABBPRE = (Decimal) dtDet.Rows[0]["IMPABB"];
        IMPASSCONPRE = (Decimal) dtDet.Rows[0]["IMPASSCON"];
        flag = true;
      }
      Decimal IMPRETDEL2 = Convert.ToDecimal(num3) - Convert.ToDecimal(IMPRETPRE);
      Decimal IMPOCCDEL2 = Convert.ToDecimal(IMPOCC2) - Convert.ToDecimal(IMPOCCDEL1);
      Decimal IMPCONDEL2 = Convert.ToDecimal(IMPCON2) - Convert.ToDecimal(IMPCONPRE);
      Decimal IMPFIGDEL2 = Convert.ToDecimal(IMPFIG2) - Convert.ToDecimal(IMPFIGPRE);
      Decimal IMPABBDEL2 = Convert.ToDecimal(IMPABB2) - Convert.ToDecimal(IMPABBPRE);
      Decimal IMPASSCONDEL2 = Convert.ToDecimal(IMPASSCON2) - Convert.ToDecimal(IMPASSCONPRE);
      Decimal IMPSANDET2 = modGetDati.MODULE_GENERA_SANZIONE(objDAtaAccess, ref PERMAXSOGLIA, num3, ref TASSO, (Decimal) dtNot.Rows[1]["ALIQUOTA"], TIPMOVSAN, DATINISAN, DATFINSAN, ref CODCAUSAN, ANNO: ANNDEN);
      int PRODENDET2 = clsWriteDb.WRITE_INSERT_DENDET(objDAtaAccess, u, CODPOS, ANNDEN, MESDEN, PRODEN, MAT, dtNot.Rows[1]["DAL"].ToString(), dtNot.Rows[1]["AL"].ToString(), dtNot.Rows[1]["FAP"].ToString(), dtNot.Rows[1]["PERFAP"].ToString(), num3, IMPOCC2, IMPFIG2, IMPABB2, IMPASSCON2, IMPCON2, (Decimal) dtNot.Rows[1]["IMPMIN"], "N", (int) dtNot.Rows[1]["PRORAP"], (int) dtNot.Rows[1]["CODCON"], (int) dtNot.Rows[1]["PROCON"], (int) dtNot.Rows[1]["CODLOC"], (int) dtNot.Rows[1]["PROLOC"], (int) dtNot.Rows[1]["CODLIV"], (int) dtNot.Rows[1]["CODQUACON"], dtNot.Rows[1]["DATNAS"].ToString(), dtNot.Rows[1]["ETAENNE"].ToString(), "RT", dtNot.Rows[1]["DATDEC"].ToString(), dtNot.Rows[1]["DATCES"].ToString(), (Decimal) dtNot.Rows[1]["NUMGGAZI"], (Decimal) dtNot.Rows[1]["NUMGGFIG"], (Decimal) dtNot.Rows[1]["NUMGGPER"], (Decimal) dtNot.Rows[1]["NUMGGDOM"], (Decimal) dtNot.Rows[1]["NUMGGSOS"], (Decimal) dtNot.Rows[1]["NUMGGCONAZI"], (Decimal) dtNot.Rows[1]["IMPSCA"], (Decimal) dtNot.Rows[1]["IMPTRAECO"], dtNot.Rows[1]["TIPRAP"].ToString(), (Decimal) dtNot.Rows[1]["PERPAR"], (Decimal) dtNot.Rows[1]["PERAPP"], dtNot.Rows[1]["TIPSPE"].ToString(), (int) dtNot.Rows[1]["CODGRUASS"], (Decimal) dtNot.Rows[1]["ALIQUOTA"], TIPISC, DATINISAN, DATFINSAN, TASSO, Convert.ToDecimal(TIPPRI), IMPRETDEL2, IMPOCCDEL2, IMPFIGDEL2, IMPCONDEL2, IMPRETPRE, IMPOCCPRE, IMPFIGPRE, IMPSANDETPRE, IMPCONPRE, IMPSANDET2, CODCAUSAN, IMPABBPRE, IMPASSCONPRE, IMPABBDEL2, IMPASSCONDEL2, dtDet.Rows[0]["ANNCOM"].ToString().Trim());
      if (PREV == "X")
      {
        DT.Clear();
        DT = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9004", CODPOS, MAT, (int) dtNot.Rows[1]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET2);
        idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, DT.Rows[0]);
        idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9004", DT, false);
      }
      num3 = 0.0M;
      IMPOCC2 = 0.0M;
      IMPCON2 = 0.0M;
      IMPFIG2 = 0.0M;
      IMPSANDET2 = 0.0M;
      IMPABB2 = 0.0M;
      IMPASSCON2 = 0.0M;
      IMPRETPRE = 0.0M;
      IMPOCCPRE = 0.0M;
      IMPCONPRE = 0.0M;
      IMPFIGPRE = 0.0M;
      IMPSANDETPRE = 0.0M;
      IMPABBPRE = 0.0M;
      IMPASSCONPRE = 0.0M;
      IMPRETDEL2 = 0.0M;
      IMPOCCDEL2 = 0.0M;
      IMPCONDEL2 = 0.0M;
      IMPFIGDEL2 = 0.0M;
      IMPABBDEL2 = 0.0M;
      IMPASSCONDEL2 = 0.0M;
      Decimal num4 = !(dtDet.Rows[0]["TIPMOV"].ToString().Trim() == "NU") ? ((int) dtDet.Rows[0]["NUMGGPER"] != 0 ? Decimal.Round(Convert.ToDecimal((Decimal) dtDet.Rows[0]["IMPRET"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[2]["NUMGGPER"]), 0) : (Decimal) dtDet.Rows[0]["IMPRET"]) : ((int) dtDet.Rows[0]["NUMGGPER"] != 0 ? Decimal.Round(Convert.ToDecimal((Decimal) dtNot.Rows[2]["IMPMIN"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[2]["NUMGGPER"]), 0) : (Decimal) dtNot.Rows[2]["IMPMIN"]);
      Decimal IMPOCC3 = (int) dtDet.Rows[0]["NUMGGPER"] != 0 ? (Decimal) dtDet.Rows[0]["IMPOCC"] / (Decimal) dtDet.Rows[0]["NUMGGPER"] * (Decimal) dtNot.Rows[2]["NUMGGPER"] : (Decimal) dtDet.Rows[0]["IMPOCC"];
      Decimal IMPCON3 = num4 * (Decimal) dtNot.Rows[2]["ALIQUOTA"] / 100M;
      Decimal IMPFIG3 = (Decimal) dtNot.Rows[2]["IMPFIG"];
      Decimal IMPABB3 = (Decimal) dtNot.Rows[2]["IMPABB"];
      Decimal IMPASSCON3 = (Decimal) dtNot.Rows[2]["IMPASSCON"];
      if (!flag)
      {
        IMPRETPRE = (Decimal) dtDet.Rows[0]["IMPRET"];
        IMPOCCPRE = (Decimal) dtDet.Rows[0]["IMPOCC"];
        IMPCONPRE = (Decimal) dtDet.Rows[0]["IMPCON"];
        IMPFIGPRE = (Decimal) dtDet.Rows[0]["IMPFIG"];
        IMPSANDETPRE = (Decimal) dtDet.Rows[0]["IMPSANDET"];
        IMPABBPRE = (Decimal) dtDet.Rows[0]["IMPABB"];
        IMPASSCONPRE = (Decimal) dtDet.Rows[0]["IMPASSCON"];
      }
      Decimal IMPRETDEL3 = Convert.ToDecimal(num4) - Convert.ToDecimal(IMPRETPRE);
      Decimal IMPOCCDEL3 = Convert.ToDecimal(IMPOCC3) - Convert.ToDecimal(IMPOCCDEL2);
      Decimal IMPCONDEL3 = Convert.ToDecimal(IMPCON3) - Convert.ToDecimal(IMPCONPRE);
      Decimal IMPFIGDEL3 = Convert.ToDecimal(IMPFIG3) - Convert.ToDecimal(IMPFIGPRE);
      Decimal IMPABBDEL3 = Convert.ToDecimal(IMPABB3) - Convert.ToDecimal(IMPABBPRE);
      Decimal IMPASSCONDEL3 = Convert.ToDecimal(IMPASSCON3) - Convert.ToDecimal(IMPASSCONPRE);
      Decimal IMPSANDET3 = modGetDati.MODULE_GENERA_SANZIONE(objDAtaAccess, ref PERMAXSOGLIA, num4, ref TASSO, (Decimal) dtNot.Rows[2]["ALIQUOTA"], TIPMOVSAN, DATINISAN, DATFINSAN, ref CODCAUSAN, ANNO: ANNDEN);
      int PRODENDET3 = clsWriteDb.WRITE_INSERT_DENDET(objDAtaAccess, u, CODPOS, ANNDEN, MESDEN, PRODEN, MAT, dtNot.Rows[2]["DAL"].ToString(), dtNot.Rows[2]["AL"].ToString(), dtNot.Rows[2]["FAP"].ToString(), dtNot.Rows[2]["PERFAP"].ToString(), num4, IMPOCC3, IMPFIG3, IMPABB3, IMPASSCON3, IMPCON3, (Decimal) dtNot.Rows[2]["IMPMIN"], "N", (int) dtNot.Rows[2]["PRORAP"], (int) dtNot.Rows[2]["CODCON"], (int) dtNot.Rows[2]["PROCON"], (int) dtNot.Rows[2]["CODLOC"], (int) dtNot.Rows[2]["PROLOC"], (int) dtNot.Rows[2]["CODLIV"], (int) dtNot.Rows[2]["CODQUACON"], dtNot.Rows[2]["DATNAS"].ToString(), dtNot.Rows[2]["ETAENNE"].ToString(), "RT", dtNot.Rows[2]["DATDEC"].ToString(), dtNot.Rows[2]["DATCES"].ToString(), (Decimal) dtNot.Rows[2]["NUMGGAZI"], (Decimal) dtNot.Rows[2]["NUMGGFIG"], (Decimal) dtNot.Rows[2]["NUMGGPER"], (Decimal) dtNot.Rows[2]["NUMGGDOM"], (Decimal) dtNot.Rows[2]["NUMGGSOS"], (Decimal) dtNot.Rows[2]["NUMGGCONAZI"], (Decimal) dtNot.Rows[2]["IMPSCA"], (Decimal) dtNot.Rows[2]["IMPTRAECO"], dtNot.Rows[2]["TIPRAP"].ToString(), (Decimal) dtNot.Rows[2]["PERPAR"], (Decimal) dtNot.Rows[2]["PERAPP"], dtNot.Rows[2]["TIPSPE"].ToString(), (int) dtNot.Rows[2]["CODGRUASS"], (Decimal) dtNot.Rows[2]["ALIQUOTA"], TIPISC, DATINISAN, DATFINSAN, TASSO, Convert.ToDecimal(TIPPRI), IMPRETDEL3, IMPOCCDEL3, IMPFIGDEL3, IMPCONDEL3, IMPRETPRE, IMPOCCPRE, IMPFIGPRE, IMPSANDETPRE, IMPCONPRE, IMPSANDET3, CODCAUSAN, IMPABBPRE, IMPASSCONPRE, IMPABBDEL3, IMPASSCONDEL3, dtDet.Rows[0]["ANNCOM"].ToString().Trim());
      if (!(PREV == "X"))
        return;
      DT.Clear();
      DataTable idocDatiE1Pitype = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9004", CODPOS, MAT, (int) dtNot.Rows[2]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET3);
      idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, idocDatiE1Pitype.Rows[0]);
      idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9004", idocDatiE1Pitype, false);
    }

    public void Module_Rettifiche_05(
      DataLayer objDAtaAccess,
      TFI.OCM.Utente.Utente u,
      ref DataTable dtDet,
      ref DataTable dtNot,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int MAT,
      string TIPMOVSAN,
      string DATA_ORA_SISTEMA,
      string TIPPRI,
      string TIPISC,
      string DATINISAN,
      string DATFINSAN,
      string PREV = "N")
    {
      string AL = "";
      Decimal IMPRETPRE = 0.0M;
      Decimal IMPOCCPRE = 0.0M;
      Decimal IMPFIGPRE = 0.0M;
      Decimal IMPSANDETPRE = 0.0M;
      Decimal IMPCONPRE = 0.0M;
      Decimal num1 = 0.0M;
      Decimal num2 = 0.0M;
      Decimal IMPABBPRE = 0.0M;
      Decimal IMPASSCONPRE = 0.0M;
      Decimal TASSO = 0.0M;
      string CODCAUSAN = "";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      IDOC_RDL idocRdl = new IDOC_RDL();
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      ModGetDati modGetDati = new ModGetDati();
      for (int index = 0; index <= dtDet.Rows.Count - 1; ++index)
      {
        string strSQL = "UPDATE DENDET SET ESIRET = 'S' WHERE" + " CODPOS=" + CODPOS.ToString() + " AND ANNDEN=" + ANNDEN.ToString() + " AND MESDEN=" + MESDEN.ToString() + " AND PRODEN=" + PRODEN.ToString() + " AND MAT=" + MAT.ToString() + " AND PRODENDET=" + dtDet.Rows[index]["PRODENDET"]?.ToString();
        objDAtaAccess.WriteTransactionData(strSQL, CommandType.Text);
        IMPRETPRE += (Decimal) dtDet.Rows[index]["IMPRET"];
        IMPOCCPRE += (Decimal) dtDet.Rows[index]["IMPOCC"];
        IMPCONPRE += (Decimal) dtDet.Rows[index]["IMPCON"];
        IMPFIGPRE += (Decimal) dtDet.Rows[index]["IMPFIG"];
        IMPSANDETPRE += (Decimal) dtDet.Rows[index]["IMPSANDET"];
        IMPABBPRE += (Decimal) dtDet.Rows[index]["IMPABB"];
        IMPASSCONPRE += (Decimal) dtDet.Rows[index]["IMPASSCON"];
        num2 += (Decimal) dtDet.Rows[index]["NUMGGPER"];
      }
      Decimal num3 = !(dtDet.Rows[0]["TIPMOV"].ToString().Trim() == "NU") ? (!(num2 == 0M) ? Decimal.Round(Convert.ToDecimal(IMPRETPRE / num2 * (Decimal) dtNot.Rows[0]["NUMGGPER"]), 0) : IMPRETPRE) : (!(num2 == 0M) ? (!(dtNot.Rows[0]["IMPMIN"].ToString() == "S") ? Decimal.Round(Convert.ToDecimal((Decimal) dtNot.Rows[0]["IMPTRAECO"] / num2 * (Decimal) dtNot.Rows[0]["NUMGGPER"]), 0) : Decimal.Round(Convert.ToDecimal((Decimal) dtNot.Rows[0]["IMPMIN"] / num2 * (Decimal) dtNot.Rows[0]["NUMGGPER"]), 0)) : (Decimal) dtNot.Rows[0]["IMPMIN"]);
      Decimal IMPOCC = !(num2 == 0M) ? IMPOCCPRE / num2 * (Decimal) dtNot.Rows[0]["NUMGGPER"] : IMPOCCPRE;
      Decimal IMPCON;
      Decimal IMPCONDEL;
      if (dtNot.Rows[0]["ALIQUOTA"] != dtDet.Rows[0]["ALIQUOTA"])
      {
        IMPCON = num3 * (Decimal) dtNot.Rows[0]["ALIQUOTA"] / 100M;
        IMPCONDEL = num3 * ((Decimal) dtDet.Rows[0]["ALIQUOTA"] - (Decimal) dtNot.Rows[0]["ALIQUOTA"]) / 100M * -1M;
        string[] strArray = IMPCONDEL.ToString().Split(',');
        if (strArray.Length > 1 && strArray[1].Length > 2)
        {
          if (Convert.ToInt32(strArray[1].Substring(2)) >= 5)
          {
            IMPCONDEL = Convert.ToDecimal(IMPCONDEL.ToString().Substring(0, IMPCONDEL.ToString().Length - strArray[1].Length + 2));
            if (IMPCONDEL < 0M)
              IMPCONDEL -= 0.1M;
            else
              IMPCONDEL += 0.1M;
          }
          else
            IMPCONDEL = Convert.ToDecimal(IMPCONDEL.ToString().Substring(0, IMPCONDEL.ToString().Length - strArray[1].Length + 2));
        }
      }
      else
      {
        IMPCON = num3 * (Decimal) dtNot.Rows[0]["ALIQUOTA"] / 100M;
        IMPCONDEL = Convert.ToDecimal(IMPCON) - Convert.ToDecimal(IMPCONPRE);
        string[] strArray = IMPCONDEL.ToString().Split(',');
        if (strArray.Length > 1 && strArray[1].Length > 2)
        {
          if (Convert.ToDecimal(strArray[1].Substring(2)) >= 5M)
          {
            IMPCONDEL = Convert.ToDecimal(IMPCONDEL.ToString().Substring(0, IMPCONDEL.ToString().Length - strArray[1].Length + 2));
            if (IMPCONDEL < 0M)
              IMPCONDEL -= 0.1M;
            else
              IMPCONDEL += 0.1M;
          }
          else
            IMPCONDEL = Convert.ToDecimal(IMPCONDEL.ToString().Substring(0, IMPCONDEL.ToString().Length - strArray[1].Length + 2));
        }
      }
      Decimal IMPFIG = (Decimal) dtNot.Rows[0]["IMPFIG"];
      Decimal IMPABB = (Decimal) dtNot.Rows[0]["IMPABB"];
      Decimal IMPASSCON = (Decimal) dtNot.Rows[0]["IMPASSCON"];
      Decimal IMPRETDEL = Convert.ToDecimal(num3) - Convert.ToDecimal(IMPRETPRE);
      Decimal IMPOCCDEL = Convert.ToDecimal(IMPOCC) - Convert.ToDecimal(num1);
      Decimal IMPFIGDEL = Convert.ToDecimal(IMPFIG) - Convert.ToDecimal(IMPFIGPRE);
      Decimal IMPABBDEL = Convert.ToDecimal(IMPABB) - Convert.ToDecimal(IMPABBPRE);
      Decimal IMPASSCONDEL = Convert.ToDecimal(IMPASSCON) - Convert.ToDecimal(IMPASSCONPRE);
      Decimal PERMAXSOGLIA = 0.0M;
      Decimal IMPSANDET = modGetDati.MODULE_GENERA_SANZIONE(objDAtaAccess, ref PERMAXSOGLIA, num3, ref TASSO, (Decimal) dtNot.Rows[0]["ALIQUOTA"], TIPMOVSAN, DATINISAN, DATFINSAN, ref CODCAUSAN, ANNO: ANNDEN);
      string str1 = dtDet.Rows[0]["TIPMOV"].ToString().Trim();
      if (!(str1 == "DP") && !(str1 == "NU"))
      {
        if (str1 == "AR")
          AL = "31/12/" + Convert.ToDateTime(dtNot.Rows[0]["AL"]).Year.ToString().Trim();
      }
      else
        AL = dtNot.Rows[0]["AL"].ToString();
      int PRODENDET = clsWriteDb.WRITE_INSERT_DENDET(objDAtaAccess, u, CODPOS, ANNDEN, MESDEN, PRODEN, MAT, dtNot.Rows[0]["DAL"].ToString(), AL, dtNot.Rows[0]["FAP"].ToString(), dtNot.Rows[0]["PERFAP"].ToString(), num3, IMPOCC, IMPFIG, IMPABB, IMPASSCON, IMPCON, (Decimal) dtNot.Rows[0]["IMPMIN"], "N", (int) dtNot.Rows[0]["PRORAP"], (int) dtNot.Rows[0]["CODCON"], (int) dtNot.Rows[0]["PROCON"], (int) dtNot.Rows[0]["CODLOC"], (int) dtNot.Rows[0]["PROLOC"], (int) dtNot.Rows[0]["CODLIV"], (int) dtNot.Rows[0]["CODQUACON"], dtNot.Rows[0]["DATNAS"].ToString(), dtNot.Rows[0]["ETAENNE"].ToString(), "RT", dtNot.Rows[0]["DATDEC"].ToString(), dtNot.Rows[0]["DATCES"].ToString(), (Decimal) dtNot.Rows[0]["NUMGGAZI"], (Decimal) dtNot.Rows[0]["NUMGGFIG"], (Decimal) dtNot.Rows[0]["NUMGGPER"], (Decimal) dtNot.Rows[0]["NUMGGDOM"], (Decimal) dtNot.Rows[0]["NUMGGSOS"], (Decimal) dtNot.Rows[0]["NUMGGCONAZI"], (Decimal) dtNot.Rows[0]["IMPSCA"], (Decimal) dtNot.Rows[0]["IMPTRAECO"], dtNot.Rows[0]["TIPRAP"].ToString(), (Decimal) dtNot.Rows[0]["PERPAR"], (Decimal) dtNot.Rows[0]["PERAPP"], dtNot.Rows[0]["TIPSPE"].ToString(), (int) dtNot.Rows[0]["CODGRUASS"], (Decimal) dtNot.Rows[0]["ALIQUOTA"], TIPISC, DATINISAN, DATFINSAN, TASSO, Convert.ToDecimal(TIPPRI), IMPRETDEL, IMPOCCDEL, IMPFIGDEL, IMPCONDEL, IMPRETPRE, IMPOCCPRE, IMPFIGPRE, IMPSANDETPRE, IMPCONPRE, IMPSANDET, CODCAUSAN, IMPABBPRE, IMPASSCONPRE, IMPABBDEL, IMPASSCONDEL, dtDet.Rows[0]["ANNCOM"].ToString().Trim());
      if (!(PREV == "X"))
        return;
      string str2 = dtDet.Rows[0]["TIPMOV"].ToString().Trim();
      if (!(str2 == "DP") && !(str2 == "NU"))
      {
        if (!(str2 == "AR"))
          return;
        dataTable2.Clear();
        DataTable idocDatiE1Pitype = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9005", CODPOS, MAT, (int) dtNot.Rows[0]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET);
        idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, idocDatiE1Pitype.Rows[0]);
        idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9005", idocDatiE1Pitype, false);
      }
      else
      {
        dataTable2.Clear();
        DataTable idocDatiE1Pitype = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9004", CODPOS, MAT, (int) dtNot.Rows[0]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET);
        idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, idocDatiE1Pitype.Rows[0]);
        idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9004", idocDatiE1Pitype, false);
      }
    }

    public void Module_Rettifiche_06(
      DataLayer objDAtaAccess,
      TFI.OCM.Utente.Utente u,
      ref DataTable dtDet,
      ref DataTable dtNot,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int MAT,
      string TIPMOVSAN,
      string DATA_ORA_SISTEMA,
      string TIPPRI,
      string TIPISC,
      string DATINISAN,
      string DATFINSAN,
      string PREV = "N")
    {
      Decimal num1 = 0.0M;
      Decimal TASSO = 0.0M;
      Decimal num2 = 0.0M;
      string CODCAUSAN = "";
      DataTable dataTable = new DataTable();
      Decimal num3 = 0.0M;
      Decimal num4 = 0.0M;
      DataTable DT = new DataTable();
      IDOC_RDL idocRdl = new IDOC_RDL();
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      ModGetDati modGetDati = new ModGetDati();
      for (int index = 0; index <= dtDet.Rows.Count - 1; ++index)
      {
        string strSQL = "UPDATE DENDET SET ESIRET = 'S' WHERE" + " CODPOS=" + CODPOS.ToString() + " AND ANNDEN=" + ANNDEN.ToString() + " AND MESDEN=" + MESDEN.ToString() + " AND PRODEN=" + PRODEN.ToString() + " AND MAT=" + MAT.ToString() + " AND PRODENDET=" + dtDet.Rows[index]["PRODENDET"]?.ToString();
        objDAtaAccess.WriteTransactionData(strSQL, CommandType.Text);
        num3 += (Decimal) dtDet.Rows[index]["IMPRET"];
        num4 += (Decimal) dtDet.Rows[index]["IMPOCC"];
        num2 += (Decimal) dtDet.Rows[index]["NUMGGPER"];
        dtDet.Rows[index]["ANNCOM"].ToString().Trim();
      }
      Decimal IMPRETPRE1 = (Decimal) dtDet.Rows[0]["IMPRET"] + (Decimal) dtDet.Rows[1]["IMPRET"];
      Decimal IMPOCCPRE1 = (Decimal) dtDet.Rows[0]["IMPOCC"] + (Decimal) dtDet.Rows[1]["IMPOCC"];
      Decimal IMPCONPRE1 = (Decimal) dtDet.Rows[0]["IMPCON"] + (Decimal) dtDet.Rows[1]["IMPCON"];
      Decimal IMPFIGPRE1 = (Decimal) dtDet.Rows[0]["IMPFIG"] + (Decimal) dtDet.Rows[1]["IMPFIG"];
      Decimal IMPSANDETPRE1 = (Decimal) dtDet.Rows[0]["IMPSANDET"] + (Decimal) dtDet.Rows[1]["IMPSANDET"];
      Decimal IMPABBPRE1 = (Decimal) dtDet.Rows[0]["IMPABB"] + (Decimal) dtDet.Rows[1]["IMPABB"];
      Decimal IMPASSCONPRE1 = (Decimal) dtDet.Rows[0]["IMPASSCON"] + (Decimal) dtDet.Rows[1]["IMPASSCON"];
      Decimal num5 = !(num2 == 0M) ? Decimal.Round(Convert.ToDecimal(num3 / num2 * (Decimal) dtNot.Rows[0]["NUMGGPER"]), 0) : num3;
      Decimal IMPOCC1 = !(num2 == 0M) ? num4 / num2 * (Decimal) dtNot.Rows[0]["NUMGGPER"] : num4;
      Decimal IMPCON1 = num5 * (Decimal) dtNot.Rows[0]["ALIQUOTA"] / 100M;
      Decimal IMPFIG1 = (Decimal) dtNot.Rows[0]["IMPFIG"];
      Decimal IMPABB1 = (Decimal) dtNot.Rows[0]["IMPABB"];
      Decimal IMPASSCON1 = (Decimal) dtNot.Rows[0]["IMPASSCON"];
      Decimal IMPRETDEL1 = Convert.ToDecimal(num5) - Convert.ToDecimal(IMPRETPRE1);
      Decimal IMPOCCDEL1 = Convert.ToDecimal(IMPOCC1) - Convert.ToDecimal(num1);
      Decimal IMPCONDEL1 = Convert.ToDecimal(IMPCON1) - Convert.ToDecimal(IMPCONPRE1);
      Decimal IMPFIGDEL1 = Convert.ToDecimal(IMPFIG1) - Convert.ToDecimal(IMPFIGPRE1);
      Decimal IMPABBDEL1 = Convert.ToDecimal(IMPABB1) - Convert.ToDecimal(IMPABBPRE1);
      Decimal IMPASSCONDEL1 = Convert.ToDecimal(IMPASSCON1) - Convert.ToDecimal(IMPASSCONPRE1);
      Decimal PERMAXSOGLIA = 0.0M;
      Decimal IMPSANDET1 = modGetDati.MODULE_GENERA_SANZIONE(objDAtaAccess, ref PERMAXSOGLIA, num5, ref TASSO, (Decimal) dtNot.Rows[0]["ALIQUOTA"], TIPMOVSAN, DATINISAN, DATFINSAN, ref CODCAUSAN, ANNO: ANNDEN);
      int PRODENDET1 = clsWriteDb.WRITE_INSERT_DENDET(objDAtaAccess, u, CODPOS, ANNDEN, MESDEN, PRODEN, MAT, dtNot.Rows[0]["DAL"].ToString(), dtNot.Rows[0]["AL"].ToString(), dtNot.Rows[0]["FAP"].ToString(), dtNot.Rows[0]["PERFAP"].ToString(), num5, IMPOCC1, IMPFIG1, IMPABB1, IMPASSCON1, IMPCON1, (Decimal) dtNot.Rows[0]["IMPMIN"], "N", (int) dtNot.Rows[0]["PRORAP"], (int) dtNot.Rows[0]["CODCON"], (int) dtNot.Rows[0]["PROCON"], (int) dtNot.Rows[0]["CODLOC"], (int) dtNot.Rows[0]["PROLOC"], (int) dtNot.Rows[0]["CODLIV"], (int) dtNot.Rows[0]["CODQUACON"], dtNot.Rows[0]["DATNAS"].ToString(), dtNot.Rows[0]["ETAENNE"].ToString(), "RT", dtNot.Rows[0]["DATDEC"].ToString(), dtNot.Rows[0]["DATCES"].ToString(), (Decimal) dtNot.Rows[0]["NUMGGAZI"], (Decimal) dtNot.Rows[0]["NUMGGFIG"], (Decimal) dtNot.Rows[0]["NUMGGPER"], (Decimal) dtNot.Rows[0]["NUMGGDOM"], (Decimal) dtNot.Rows[0]["NUMGGSOS"], (Decimal) dtNot.Rows[0]["NUMGGCONAZI"], (Decimal) dtNot.Rows[0]["IMPSCA"], (Decimal) dtNot.Rows[0]["IMPTRAECO"], dtNot.Rows[0]["TIPRAP"].ToString(), (Decimal) dtNot.Rows[0]["PERPAR"], (Decimal) dtNot.Rows[0]["PERAPP"], dtNot.Rows[0]["TIPSPE"].ToString(), (int) dtNot.Rows[0]["CODGRUASS"], (Decimal) dtNot.Rows[0]["ALIQUOTA"], TIPISC, DATINISAN, DATFINSAN, TASSO, Convert.ToDecimal(TIPPRI), IMPRETDEL1, IMPOCCDEL1, IMPFIGDEL1, IMPCONDEL1, IMPRETPRE1, IMPOCCPRE1, IMPFIGPRE1, IMPSANDETPRE1, IMPCONPRE1, IMPSANDET1, CODCAUSAN, IMPABBPRE1, IMPASSCONPRE1, IMPABBDEL1, IMPASSCONDEL1, dtDet.Rows[0]["ANNCOM"].ToString().Trim());
      if (PREV == "X")
      {
        DT.Clear();
        DT = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9004", CODPOS, MAT, (int) dtNot.Rows[0]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET1);
        idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, DT.Rows[0]);
        idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9004", DT, false);
      }
      num5 = 0.0M;
      IMPOCC1 = 0.0M;
      IMPCON1 = 0.0M;
      IMPFIG1 = 0.0M;
      IMPSANDET1 = 0.0M;
      IMPABB1 = 0.0M;
      IMPASSCON1 = 0.0M;
      IMPRETPRE1 = 0.0M;
      IMPOCCPRE1 = 0.0M;
      IMPCONPRE1 = 0.0M;
      IMPFIGPRE1 = 0.0M;
      IMPSANDETPRE1 = 0.0M;
      IMPABBPRE1 = 0.0M;
      IMPASSCONPRE1 = 0.0M;
      IMPRETDEL1 = 0.0M;
      IMPOCCDEL1 = 0.0M;
      IMPCONDEL1 = 0.0M;
      IMPFIGDEL1 = 0.0M;
      IMPABBDEL1 = 0.0M;
      IMPASSCONDEL1 = 0.0M;
      Decimal IMPRETPRE2 = (Decimal) dtDet.Rows[1]["IMPRET"];
      Decimal IMPOCCPRE2 = (Decimal) dtDet.Rows[1]["IMPOCC"];
      Decimal IMPCONPRE2 = (Decimal) dtDet.Rows[1]["IMPCON"];
      Decimal IMPFIGPRE2 = (Decimal) dtDet.Rows[1]["IMPFIG"];
      Decimal IMPSANDETPRE2 = (Decimal) dtDet.Rows[1]["IMPSANDET"];
      Decimal IMPABBPRE2 = (Decimal) dtDet.Rows[1]["IMPABB"];
      Decimal IMPASSCONPRE2 = (Decimal) dtDet.Rows[1]["IMPASSCON"];
      Decimal num6 = !(num2 == 0M) ? Decimal.Round(Convert.ToDecimal(num3 / num2 * (Decimal) dtNot.Rows[1]["NUMGGPER"]), 0) : num3;
      Decimal IMPOCC2 = !(num2 == 0M) ? num4 / num2 * (Decimal) dtNot.Rows[1]["NUMGGPER"] : num4;
      Decimal IMPCON2 = num6 * (Decimal) dtNot.Rows[1]["ALIQUOTA"] / 100M;
      Decimal IMPFIG2 = (Decimal) dtNot.Rows[1]["IMPFIG"];
      Decimal IMPABB2 = (Decimal) dtNot.Rows[1]["IMPABB"];
      Decimal IMPASSCON2 = (Decimal) dtNot.Rows[1]["IMPASSCON"];
      Decimal IMPRETDEL2 = Convert.ToDecimal(num6) - Convert.ToDecimal(IMPRETPRE2);
      Decimal IMPOCCDEL2 = Convert.ToDecimal(IMPOCC2) - Convert.ToDecimal(IMPOCCDEL1);
      Decimal IMPCONDEL2 = Convert.ToDecimal(IMPCON2) - Convert.ToDecimal(IMPCONPRE2);
      Decimal IMPFIGDEL2 = Convert.ToDecimal(IMPFIG2) - Convert.ToDecimal(IMPFIGPRE2);
      Decimal IMPABBDEL2 = Convert.ToDecimal(IMPABB2) - Convert.ToDecimal(IMPABBPRE2);
      Decimal IMPASSCONDEL2 = Convert.ToDecimal(IMPASSCON2) - Convert.ToDecimal(IMPASSCONPRE2);
      Decimal IMPSANDET2 = modGetDati.MODULE_GENERA_SANZIONE(objDAtaAccess, ref PERMAXSOGLIA, num6, ref TASSO, (Decimal) (int) dtNot.Rows[1]["ALIQUOTA"], TIPMOVSAN, DATINISAN, DATFINSAN, ref CODCAUSAN, ANNO: ANNDEN);
      int PRODENDET2 = clsWriteDb.WRITE_INSERT_DENDET(objDAtaAccess, u, CODPOS, ANNDEN, MESDEN, PRODEN, MAT, dtNot.Rows[1]["DAL"].ToString(), dtNot.Rows[1]["AL"].ToString(), dtNot.Rows[1]["FAP"].ToString(), dtNot.Rows[1]["PERFAP"].ToString(), num6, IMPOCC2, IMPFIG2, IMPABB2, IMPASSCON2, IMPCON2, (Decimal) dtNot.Rows[1]["IMPMIN"], "N", (int) dtNot.Rows[1]["PRORAP"], (int) dtNot.Rows[1]["CODCON"], (int) dtNot.Rows[1]["PROCON"], (int) dtNot.Rows[1]["CODLOC"], (int) dtNot.Rows[1]["PROLOC"], (int) dtNot.Rows[1]["CODLIV"], (int) dtNot.Rows[1]["CODQUACON"], dtNot.Rows[1]["DATNAS"].ToString(), dtNot.Rows[1]["ETAENNE"].ToString(), "RT", dtNot.Rows[1]["DATDEC"].ToString(), dtNot.Rows[1]["DATCES"].ToString(), (Decimal) dtNot.Rows[1]["NUMGGAZI"], (Decimal) dtNot.Rows[1]["NUMGGFIG"], (Decimal) dtNot.Rows[1]["NUMGGPER"], (Decimal) dtNot.Rows[1]["NUMGGDOM"], (Decimal) dtNot.Rows[1]["NUMGGSOS"], (Decimal) dtNot.Rows[1]["NUMGGCONAZI"], (Decimal) dtNot.Rows[1]["IMPSCA"], (Decimal) dtNot.Rows[1]["IMPTRAECO"], dtNot.Rows[1]["TIPRAP"].ToString(), (Decimal) dtNot.Rows[1]["PERPAR"], (Decimal) dtNot.Rows[1]["PERAPP"], dtNot.Rows[1]["TIPSPE"].ToString(), (int) dtNot.Rows[1]["CODGRUASS"], (Decimal) dtNot.Rows[1]["ALIQUOTA"], TIPISC, DATINISAN, DATFINSAN, TASSO, Convert.ToDecimal(TIPPRI), IMPRETDEL2, IMPOCCDEL2, IMPFIGDEL2, IMPCONDEL2, IMPRETPRE2, IMPOCCPRE2, IMPFIGPRE2, IMPSANDETPRE2, IMPCONPRE2, IMPSANDET2, CODCAUSAN, IMPABBPRE2, IMPASSCONPRE2, IMPABBDEL2, IMPASSCONDEL2, dtDet.Rows[0]["ANNCOM"].ToString().Trim());
      if (PREV == "X")
      {
        DT.Clear();
        DT = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9004", CODPOS, MAT, (int) dtNot.Rows[1]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET2);
        idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, DT.Rows[0]);
        idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9004", DT, false);
      }
      num6 = 0.0M;
      IMPOCC2 = 0.0M;
      IMPCON2 = 0.0M;
      IMPFIG2 = 0.0M;
      IMPSANDET2 = 0.0M;
      IMPABB2 = 0.0M;
      IMPASSCON2 = 0.0M;
      IMPRETPRE2 = 0.0M;
      IMPOCCPRE2 = 0.0M;
      IMPCONPRE2 = 0.0M;
      IMPFIGPRE2 = 0.0M;
      IMPSANDETPRE2 = 0.0M;
      IMPABBPRE2 = 0.0M;
      IMPASSCONPRE2 = 0.0M;
      IMPRETDEL2 = 0.0M;
      IMPOCCDEL2 = 0.0M;
      IMPCONDEL2 = 0.0M;
      IMPFIGDEL2 = 0.0M;
      IMPABBDEL2 = 0.0M;
      IMPASSCONDEL2 = 0.0M;
      Decimal IMPRETPRE3 = (Decimal) dtDet.Rows[1]["IMPRET"];
      Decimal IMPOCCPRE3 = (Decimal) dtDet.Rows[1]["IMPOCC"];
      Decimal IMPCONPRE3 = (Decimal) dtDet.Rows[1]["IMPCON"];
      Decimal IMPFIGPRE3 = (Decimal) dtDet.Rows[1]["IMPFIG"];
      Decimal IMPSANDETPRE3 = (Decimal) dtDet.Rows[1]["IMPSANDET"];
      Decimal IMPABBPRE3 = (Decimal) dtDet.Rows[1]["IMPABB"];
      Decimal IMPASSCONPRE3 = (Decimal) dtDet.Rows[1]["IMPASSCON"];
      Decimal num7 = !(num2 == 0M) ? Decimal.Round(Convert.ToDecimal(num3 / num2 * (Decimal) dtNot.Rows[2]["NUMGGPER"]), 0) : num3;
      Decimal IMPOCC3 = !(num2 == 0M) ? num4 / num2 * (Decimal) dtNot.Rows[2]["NUMGGPER"] : num4;
      Decimal IMPCON3 = num7 * (Decimal) dtNot.Rows[2]["ALIQUOTA"] / 100M;
      Decimal IMPFIG3 = (Decimal) dtNot.Rows[2]["IMPFIG"];
      Decimal IMPABB3 = (Decimal) dtNot.Rows[2]["IMPABB"];
      Decimal IMPASSCON3 = (Decimal) dtNot.Rows[2]["IMPASSCON"];
      Decimal IMPRETDEL3 = Convert.ToDecimal(num7) - Convert.ToDecimal(IMPRETPRE3);
      Decimal IMPOCCDEL3 = Convert.ToDecimal(IMPOCC3) - Convert.ToDecimal(IMPOCCDEL2);
      Decimal IMPCONDEL3 = Convert.ToDecimal(IMPCON3) - Convert.ToDecimal(IMPCONPRE3);
      Decimal IMPFIGDEL3 = Convert.ToDecimal(IMPFIG3) - Convert.ToDecimal(IMPFIGPRE3);
      Decimal IMPABBDEL3 = Convert.ToDecimal(IMPABB3) - Convert.ToDecimal(IMPABBPRE3);
      Decimal IMPASSCONDEL3 = Convert.ToDecimal(IMPASSCON3) - Convert.ToDecimal(IMPASSCONPRE3);
      Decimal IMPSANDET3 = modGetDati.MODULE_GENERA_SANZIONE(objDAtaAccess, ref PERMAXSOGLIA, num7, ref TASSO, (Decimal) (int) dtNot.Rows[2]["ALIQUOTA"], TIPMOVSAN, DATINISAN, DATFINSAN, ref CODCAUSAN, ANNO: ANNDEN);
      int PRODENDET3 = clsWriteDb.WRITE_INSERT_DENDET(objDAtaAccess, u, CODPOS, ANNDEN, MESDEN, PRODEN, MAT, dtNot.Rows[2]["DAL"].ToString(), dtNot.Rows[2]["AL"].ToString(), dtNot.Rows[2]["FAP"].ToString(), dtNot.Rows[2]["PERFAP"].ToString(), num7, IMPOCC3, IMPFIG3, IMPABB3, IMPASSCON3, IMPCON3, (Decimal) dtNot.Rows[2]["IMPMIN"], "N", (int) dtNot.Rows[2]["PRORAP"], (int) dtNot.Rows[2]["CODCON"], (int) dtNot.Rows[2]["PROCON"], (int) dtNot.Rows[2]["CODLOC"], (int) dtNot.Rows[2]["PROLOC"], (int) dtNot.Rows[2]["CODLIV"], (int) dtNot.Rows[2]["CODQUACON"], dtNot.Rows[2]["DATNAS"].ToString(), dtNot.Rows[2]["ETAENNE"].ToString(), "RT", dtNot.Rows[2]["DATDEC"].ToString(), dtNot.Rows[2]["DATCES"].ToString(), (Decimal) dtNot.Rows[2]["NUMGGAZI"], (Decimal) dtNot.Rows[2]["NUMGGFIG"], (Decimal) dtNot.Rows[2]["NUMGGPER"], (Decimal) dtNot.Rows[2]["NUMGGDOM"], (Decimal) dtNot.Rows[2]["NUMGGSOS"], (Decimal) dtNot.Rows[2]["NUMGGCONAZI"], (Decimal) dtNot.Rows[2]["IMPSCA"], (Decimal) dtNot.Rows[2]["IMPTRAECO"], dtNot.Rows[2]["TIPRAP"].ToString(), (Decimal) dtNot.Rows[2]["PERPAR"], (Decimal) dtNot.Rows[2]["PERAPP"], dtNot.Rows[2]["TIPSPE"].ToString(), (int) dtNot.Rows[2]["CODGRUASS"], (Decimal) dtNot.Rows[2]["ALIQUOTA"], TIPISC, DATINISAN, DATFINSAN, TASSO, Convert.ToDecimal(TIPPRI), IMPRETDEL3, IMPOCCDEL3, IMPFIGDEL3, IMPCONDEL3, IMPRETPRE3, IMPOCCPRE3, IMPFIGPRE3, IMPSANDETPRE3, IMPCONPRE3, IMPSANDET3, CODCAUSAN, IMPABBPRE3, IMPASSCONPRE3, IMPABBDEL3, IMPASSCONDEL3, dtDet.Rows[0]["ANNCOM"].ToString().Trim());
      if (!(PREV == "X"))
        return;
      DT.Clear();
      DataTable idocDatiE1Pitype = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9004", CODPOS, MAT, (int) dtNot.Rows[2]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET3);
      idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, idocDatiE1Pitype.Rows[0]);
      idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9004", idocDatiE1Pitype, false);
    }

    public void Module_Rettifiche_07(
      DataLayer objDAtaAccess,
      TFI.OCM.Utente.Utente u,
      ref DataTable dtDet,
      ref DataTable dtNot,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int MAT,
      string TIPMOVSAN,
      string DATA_ORA_SISTEMA,
      string TIPPRI,
      string TIPISC,
      string DATINISAN,
      string DATFINSAN,
      string PREV = "N")
    {
      string str1 = "";
      Decimal IMPRETPRE = 0.0M;
      Decimal IMPOCCPRE = 0.0M;
      Decimal IMPFIGPRE = 0.0M;
      Decimal IMPSANDETPRE = 0.0M;
      Decimal IMPCONPRE = 0.0M;
      Decimal IMPOCCDEL = 0.0M;
      Decimal num1 = 0.0M;
      Decimal IMPABBPRE = 0.0M;
      Decimal IMPASSCONPRE = 0.0M;
      Decimal TASSO = 0.0M;
      string CODCAUSAN = "";
      DataTable dataTable = new DataTable();
      DataTable DT = new DataTable();
      IDOC_RDL idocRdl = new IDOC_RDL();
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      ModGetDati modGetDati = new ModGetDati();
      for (int index = 0; index <= dtDet.Rows.Count - 1; ++index)
      {
        string strSQL = "UPDATE DENDET SET ESIRET = 'S' WHERE" + " CODPOS=" + CODPOS.ToString() + " AND ANNDEN=" + ANNDEN.ToString() + " AND MESDEN=" + MESDEN.ToString() + " AND PRODEN=" + PRODEN.ToString() + " AND MAT=" + MAT.ToString() + " AND PRODENDET=" + dtDet.Rows[index]["PRODENDET"]?.ToString();
        objDAtaAccess.WriteTransactionData(strSQL, CommandType.Text);
        IMPRETPRE += (Decimal) dtDet.Rows[index]["IMPRET"];
        IMPOCCPRE += (Decimal) dtDet.Rows[index]["IMPOCC"];
        IMPCONPRE += (Decimal) dtDet.Rows[index]["IMPCON"];
        IMPFIGPRE += (Decimal) dtDet.Rows[index]["IMPFIG"];
        IMPSANDETPRE += (Decimal) dtDet.Rows[index]["IMPSANDET"];
        IMPABBPRE += (Decimal) dtDet.Rows[index]["IMPABB"];
        IMPASSCONPRE += (Decimal) dtDet.Rows[index]["IMPASSCON"];
        num1 += (Decimal) dtDet.Rows[index]["NUMGGPER"];
      }
      for (int index = 0; index <= dtDet.Rows.Count - 1; ++index)
      {
        Decimal num2 = !(dtDet.Rows[index]["TIPMOV"].ToString().Trim() == "NU") ? (!(num1 == 0M) ? Decimal.Round(Convert.ToDecimal(IMPRETPRE / num1 * (Decimal) dtNot.Rows[index]["NUMGGPER"]), 0) : IMPRETPRE) : (!(num1 == 0M) ? Decimal.Round(Convert.ToDecimal((Decimal) dtNot.Rows[index]["IMPMIN"] / num1 * (Decimal) dtNot.Rows[index]["NUMGGPER"]), 0) : (Decimal) dtNot.Rows[index]["IMPMIN"]);
        Decimal IMPOCC = !(num1 == 0M) ? IMPOCCPRE / num1 * (Decimal) dtNot.Rows[index]["NUMGGPER"] : IMPOCCPRE;
        Decimal IMPCON;
        Decimal IMPCONDEL;
        if (dtNot.Rows[0]["ALIQUOTA"] != dtDet.Rows[index]["ALIQUOTA"])
        {
          IMPCON = num2 * (Decimal) dtNot.Rows[0]["ALIQUOTA"] / 100M;
          IMPCONDEL = num2 * ((Decimal) dtDet.Rows[index]["ALIQUOTA"] - (Decimal) dtNot.Rows[0]["ALIQUOTA"]) / 100M * -1M;
          string[] strArray = IMPCONDEL.ToString().Split(',');
          if (strArray.Length > 1 && strArray[1].Length > 2)
          {
            if (Convert.ToInt32(strArray[1].Substring(2)) >= 5)
            {
              IMPCONDEL = (Decimal) Convert.ToInt32(IMPCONDEL.ToString().Substring(0, IMPCONDEL.ToString().Length - strArray[1].Length + 2));
              if (IMPCONDEL < 0M)
                IMPCONDEL -= 0.1M;
              else
                IMPCONDEL += 0.1M;
            }
            else
              IMPCONDEL = Convert.ToDecimal(IMPCONDEL.ToString().Substring(0, IMPCONDEL.ToString().Length - strArray[1].Length + 2));
          }
        }
        else
        {
          IMPCON = num2 * (Decimal) dtNot.Rows[0]["ALIQUOTA"] / 100M;
          IMPCONDEL = Convert.ToDecimal(IMPCON) - Convert.ToDecimal(IMPCONPRE);
          string[] strArray = IMPCONDEL.ToString().Split(',');
          if (strArray.Length > 1 && strArray[1].Length > 2)
          {
            if (Convert.ToInt32(strArray[1].Substring(2)) >= 5)
            {
              IMPCONDEL = (Decimal) Convert.ToInt32(IMPCONDEL.ToString().Substring(0, IMPCONDEL.ToString().Length - strArray[1].Length + 2));
              if (IMPCONDEL < 0M)
                IMPCONDEL -= 0.1M;
              else
                IMPCONDEL += 0.1M;
            }
            else
              IMPCONDEL = Convert.ToDecimal(IMPCONDEL.ToString().Substring(0, IMPCONDEL.ToString().Length - strArray[1].Length + 2));
          }
        }
        Decimal IMPFIG = (Decimal) dtNot.Rows[index]["IMPFIG"];
        Decimal IMPABB = (Decimal) dtNot.Rows[index]["IMPABB"];
        Decimal IMPASSCON = (Decimal) dtNot.Rows[index]["IMPASSCON"];
        Decimal IMPRETDEL = Convert.ToDecimal(num2) - Convert.ToDecimal(IMPRETPRE);
        IMPOCCDEL = Convert.ToDecimal(IMPOCC) - Convert.ToDecimal(IMPOCCDEL);
        Decimal IMPFIGDEL = Convert.ToDecimal(IMPFIG) - Convert.ToDecimal(IMPFIGPRE);
        Decimal IMPABBDEL = Convert.ToDecimal(IMPABB) - Convert.ToDecimal(IMPABBPRE);
        Decimal IMPASSCONDEL = Convert.ToDecimal(IMPASSCON) - Convert.ToDecimal(IMPASSCONPRE);
        Decimal PERMAXSOGLIA = 0.0M;
        Decimal IMPSANDET = modGetDati.MODULE_GENERA_SANZIONE(objDAtaAccess, ref PERMAXSOGLIA, num2, ref TASSO, (Decimal) (int) dtNot.Rows[index]["ALIQUOTA"], TIPMOVSAN, DATINISAN, DATFINSAN, ref CODCAUSAN, ANNO: ANNDEN);
        string str2 = dtDet.Rows[index]["TIPMOV"].ToString().Trim();
        if (!(str2 == "DP") && !(str2 == "NU"))
        {
          if (str2 == "AR")
            str1 = "31/12/" + Convert.ToDateTime(dtNot.Rows[index]["AL"]).Year.ToString().Trim();
        }
        else
          str1 = dtNot.Rows[index]["AL"].ToString();
        int PRODENDET = clsWriteDb.WRITE_INSERT_DENDET(objDAtaAccess, u, CODPOS, ANNDEN, MESDEN, PRODEN, (int) dtDet.Rows[index][nameof (MAT)], dtDet.Rows[index]["DAL"].ToString(), dtDet.Rows[index]["AL"].ToString(), dtDet.Rows[index]["FAP"].ToString(), dtDet.Rows[index]["PERFAP"].ToString(), num2, IMPOCC, IMPFIG, IMPABB, IMPASSCON, IMPCON, (Decimal) dtDet.Rows[index]["IMPMIN"], dtDet.Rows[index][nameof (PREV)].ToString(), (int) dtDet.Rows[index]["PRORAP"], (int) dtDet.Rows[index]["CODCON"], (int) dtDet.Rows[index]["PROCON"], (int) dtDet.Rows[index]["CODLOC"], (int) dtDet.Rows[index]["PROLOC"], (int) dtDet.Rows[index]["CODLIV"], (int) dtDet.Rows[index]["CODQUACON"], dtDet.Rows[index]["DATNAS"].ToString(), dtDet.Rows[index]["ETA65"].ToString(), "RT", dtDet.Rows[index]["DATDEC"].ToString(), dtDet.Rows[index]["DATCES"].ToString(), (Decimal) dtDet.Rows[index]["NUMGGAZI"], (Decimal) dtDet.Rows[index]["NUMGGFIG"], (Decimal) dtDet.Rows[index]["NUMGGPER"], (Decimal) dtDet.Rows[index]["NUMGGDOM"], (Decimal) dtDet.Rows[index]["NUMGGSOS"], (Decimal) dtDet.Rows[index]["NUMGGCONAZI"], (Decimal) dtDet.Rows[index]["IMPSCA"], (Decimal) dtDet.Rows[index]["IMPTRAECO"], dtDet.Rows[index]["TIPRAP"].ToString(), (Decimal) dtDet.Rows[index]["PERPAR"], (Decimal) dtDet.Rows[index]["PERAPP"], dtDet.Rows[index]["TIPSPE"].ToString(), (int) dtDet.Rows[index]["CODGRUASS"], (Decimal) dtDet.Rows[index]["ALIQUOTA"], dtDet.Rows[index]["TIPDEN"].ToString(), DATINISAN, DATFINSAN, TASSO, Convert.ToDecimal(TIPPRI), IMPRETDEL, IMPOCCDEL, IMPFIGDEL, IMPCONDEL, IMPRETPRE, IMPOCCPRE, IMPFIGPRE, IMPSANDETPRE, IMPCONPRE, IMPSANDET, CODCAUSAN, IMPABBPRE, IMPASSCONPRE, IMPABBDEL, IMPASSCONDEL, dtDet.Rows[index]["ANNCOM"].ToString().Trim());
        if (PREV == "X")
        {
          string str3 = dtDet.Rows[index]["TIPMOV"].ToString().Trim();
          if (!(str3 == "DP") && !(str3 == "NU"))
          {
            if (str3 == "AR")
            {
              DT.Clear();
              DT = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9005", CODPOS, MAT, (int) dtNot.Rows[index]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET);
              idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, DT.Rows[0]);
              idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9005", DT, false);
            }
          }
          else
          {
            DT.Clear();
            DT = idocRdl.GET_IDOC_DATI_E1PITYPE(objDAtaAccess, "9004", CODPOS, MAT, (int) dtNot.Rows[index]["PRORAP"], 0, "", "", "9999-12-31", "", "", "", ANNDEN, MESDEN, PRODEN, "", "", "RT", "", flgWEB: "", PRODENDET: PRODENDET);
            idocRdl.WRITE_IDOC_TESTATA(objDAtaAccess, DT.Rows[0]);
            idocRdl.WRITE_IDOC_E1PITYP(objDAtaAccess, "9004", DT, false);
          }
        }
        str1 = "";
      }
    }
  }
}
