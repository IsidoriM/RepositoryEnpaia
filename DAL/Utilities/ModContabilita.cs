// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Utilities.ModContabilita
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Data;
using TFI.DAL.ConnectorDB;

namespace TFI.DAL.Utilities
{
  public class ModContabilita
  {
    public DataTable WRITE_CONTABILITA_RETTIFICHE(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int ANNORET,
      int PRORET,
      int PRORETTES,
      string TIPANN,
      string TIPIMP,
      string NUMMOV)
    {
      int num1 = 0;
      DataTable dataTable1 = new DataTable();
      string str1 = "";
      Decimal num2 = 0.0M;
      string str2 = "";
      string str3 = "";
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      Decimal num3 = 0.0M;
      Decimal num4 = 0.0M;
      Decimal num5 = 0.0M;
      Decimal IMPAB = 0.0M;
      Decimal IMPAC = 0.0M;
      Decimal num6 = 0.0M;
      Decimal num7 = 0.0M;
      Decimal num8 = 0.0M;
      Decimal num9 = 0.0M;
      Decimal num10 = 0.0M;
      Decimal num11 = 0.0M;
      Decimal num12 = 0.0M;
      Decimal num13 = 0.0M;
      Decimal num14 = 0.0M;
      Decimal num15 = 0.0M;
      DataTable dataTable4 = new DataTable();
      DataTable dataTable5 = new DataTable();
      DataTable dataTable6 = new DataTable();
      DataTable dataTable7 = new DataTable();
      DataTable dataTable8 = new DataTable();
      DataTable dataTable9 = new DataTable();
      DataTable dataTable10 = new DataTable();
      DataTable dataTable11 = new DataTable();
      int num16 = 0;
      int num17 = 0;
      string str4 = "";
      DataTable dataTable12 = new DataTable();
      bool FLGMULTIPLE = false;
      Decimal num18 = 0.0M;
      DataColumn column = new DataColumn();
      Utile utile = new Utile();
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      column.ColumnName = "PARTITA";
      dataTable1.Columns.Add(column);
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "PROGMOV"
      });
      string strData = utile.Module_GetDataSistema().ToString();
      DataTable dataTable13;
      int PROGSPLID;
      if (TIPIMP == "-")
      {
        string strSQL1 = " SELECT CODPOS, MAT, ANNDEN, MESDEN, PRODEN, PRODENDET, IMPCONDEL, IMPFAP, PARTITA, PROGMOV FROM " + " DENDET WHERE ANNRET = " + ANNORET.ToString() + " AND PRORET = " + PRORET.ToString() + " AND PRORETTES = " + PRORETTES.ToString() + " ORDER BY DATCONMOV";
        dataTable13 = objDataAccess.GetDataTable(strSQL1);
        for (int index = 0; index <= dataTable13.Rows.Count - 1; ++index)
        {
          if (index > 0)
          {
            Convert.ToInt32(num17);
            Convert.ToInt32(dataTable13.Rows[index]["MESDEN"]);
            FLGMULTIPLE = true;
            break;
          }
          num17 = Convert.ToInt32(dataTable13.Rows[index]["MESDEN"]);
        }
        for (int index = 0; index <= dataTable13.Rows.Count - 1; ++index)
          num18 += Convert.ToDecimal(dataTable13.Rows[index]["IMPCONDEL"]);
        Decimal num19 = Decimal.Round(Math.Abs(num18) * 4M / 100M, 2);
        for (int index1 = 0; index1 <= dataTable13.Rows.Count - 1; ++index1)
        {
          num9 = 0.0M;
          num10 = 0.0M;
          num11 = 0.0M;
          num12 = 0.0M;
          num13 = 0.0M;
          num14 = 0.0M;
          num15 = 0.0M;
          num8 = 0.0M;
          num2 = 0.0M;
          num3 = 0.0M;
          num4 = 0.0M;
          num5 = 0.0M;
          IMPAB = 0.0M;
          IMPAC = 0.0M;
          num6 = 0.0M;
          num7 = 0.0M;
          string strSQL2 = " SELECT FORASS.CATFORASS, SUM(DENDETALI.IMPCON) AS IMPCON, " + " SUM(DENDETALI.IMPRIDUZ) AS IMPRIDUZ " + " FROM DENDET, DENDETALI, FORASS" + " WHERE DENDET.CODPOS = DENDETALI.CODPOS AND" + " DENDET.MAT = DENDETALI.MAT AND" + " DENDET.ANNDEN = DENDETALI.ANNDEN AND" + " DENDET.MESDEN = DENDETALI.MESDEN AND" + " DENDET.PRODEN = DENDETALI.PRODEN AND" + " DENDET.PRODENDET = DENDETALI.PRODENDET AND" + " DENDETALI.CODFORASS = FORASS.CODFORASS" + " AND DENDETALI.ANNDEN = " + dataTable13.Rows[index1]["ANNDEN"]?.ToString() + " AND DENDETALI.MESDEN = " + dataTable13.Rows[index1]["MESDEN"]?.ToString() + " AND DENDETALI.MAT = " + dataTable13.Rows[index1]["MAT"]?.ToString() + " AND DENDETALI.PRODEN = " + dataTable13.Rows[index1]["PRODEN"]?.ToString() + " AND DENDETALI.PRODENDET = " + dataTable13.Rows[index1]["PRODENDET"]?.ToString() + " GROUP BY FORASS.CATFORASS" + " ORDER BY FORASS.CATFORASS";
          dataTable5.Clear();
          dataTable5 = objDataAccess.GetDataTable(strSQL2);
          for (int index2 = 0; index2 <= dataTable5.Rows.Count - 1; ++index2)
          {
            if (dataTable5.Rows[index2]["CATFORASS"].ToString().Trim() == "TFR")
              num9 = Math.Abs(Convert.ToDecimal(dataTable5.Rows[index2]["IMPCON"])) - Math.Abs(Convert.ToDecimal(dataTable5.Rows[index2]["IMPRIDUZ"]));
            if (dataTable5.Rows[index2]["CATFORASS"].ToString().Trim() == "PREV")
              num10 = Math.Abs(Convert.ToDecimal(dataTable5.Rows[index2]["IMPCON"])) - Math.Abs(Convert.ToDecimal(dataTable5.Rows[index2]["IMPRIDUZ"]));
            if (dataTable5.Rows[index2]["CATFORASS"].ToString().Trim() == "INF")
              num11 = Math.Abs(Convert.ToDecimal(dataTable5.Rows[index2]["IMPCON"])) - Math.Abs(Convert.ToDecimal(dataTable5.Rows[index2]["IMPRIDUZ"]));
          }
          Decimal num20 = num9 + num10 + num11;
          Decimal num21 = num20;
          num14 = Convert.ToDecimal(dataTable13.Rows[index1]["IMPFAP"]);
          Decimal d1 = num20 * 4M / 100M;
          if (d1 != 0M)
          {
            string[] strArray = d1.ToString().Trim().Split(',');
            string str5 = strArray[0];
            if (strArray.Length > 1)
              str3 = strArray[1];
            num8 = str3.Length <= 2 ? Decimal.Round(d1, 2) : (!(str3.Substring(2, 1) == "5") ? Decimal.Round(d1, 2) : Convert.ToDecimal(str5 + "." + str3.Substring(0, 2)));
          }
          else
            num8 = 0M;
          num15 = num20 + num8;
          string strSQL3 = " SELECT * FROM (SELECT B.DATCONMOV, A.ALIQUOTA,  A.CODPOS, A.ANNDEN, A.MESDEN," + " A.PRODEN, A.PRODENDET, A.IMPRIDUZ, A.TIPMOV AS TIPMOVDENDET, " + " CASE A.TIPMOV WHEN 'RT' THEN A.IMPCONDEL" + " ELSE A.IMPCON END AS IMPORTO," + " A.MAT, A.NUMMOV, A.IMPCON, A.IMPCONDEL, A.PERFAP," + " B.TIPMOV, B.TIPANNMOV, A.TIPANNMOVARR FROM DENDET A, DENTES B WHERE" + " A.CODPOS = B.CODPOS" + " AND A.ANNDEN = B.ANNDEN" + " AND A.MESDEN = B.MESDEN" + " AND A.PRODEN = B.PRODEN" + " AND A.CODPOS = " + dataTable13.Rows[index1]["CODPOS"]?.ToString() + " AND A.ANNDEN = " + dataTable13.Rows[index1]["ANNDEN"]?.ToString() + " AND A.MESDEN = " + dataTable13.Rows[index1]["MESDEN"]?.ToString() + " AND A.PRODEN = " + dataTable13.Rows[index1]["PRODEN"]?.ToString() + " AND A.MAT = " + dataTable13.Rows[index1]["MAT"]?.ToString() + " AND A.NUMMOV IS NOT NULL AND A.NUMMOVANN IS NULL" + " AND A.IMPCONDEL >=0) AS TABELLA" + " WHERE (IMPORTO - IMPRIDUZ) > 0" + " ORDER BY TIPMOVDENDET DESC, DATCONMOV";
          DataTable dataTable14 = objDataAccess.GetDataTable(strSQL3);
          for (int index3 = 0; index3 <= dataTable14.Rows.Count - 1; ++index3)
          {
            num3 = 0.0M;
            num4 = 0.0M;
            num5 = 0.0M;
            IMPAB = 0.0M;
            IMPAC = 0.0M;
            num6 = 0.0M;
            num7 = 0.0M;
            num2 = 0.0M;
            dataTable14.Rows[index3]["DATCONMOV"].ToString();
            string str6 = !(dataTable14.Rows[index3]["TIPMOV"].ToString().Trim() == "AR") ? dataTable14.Rows[index3]["TIPANNMOV"].ToString().Trim() : dataTable14.Rows[index3]["TIPANNMOVARR"].ToString().Trim();
            Decimal num22 = Convert.ToDecimal(dataTable14.Rows[index3]["PERFAP"]);
            Decimal num23 = Convert.ToDecimal(dataTable14.Rows[index3]["ALIQUOTA"]);
            Decimal num24;
            if (num21 >= Convert.ToDecimal(dataTable14.Rows[index3]["IMPORTO"]) - Convert.ToDecimal(dataTable14.Rows[index3]["IMPRIDUZ"]))
            {
              string strSQL4 = " SELECT FORASS.CATFORASS, SUM(DENDETALI.IMPCON) AS IMPCON, " + " SUM(DENDETALI.IMPRIDUZ) AS IMPRIDUZ " + " FROM DENDET, DENDETALI, FORASS" + " WHERE DENDET.CODPOS = DENDETALI.CODPOS AND" + " DENDET.MAT = DENDETALI.MAT AND" + " DENDET.ANNDEN = DENDETALI.ANNDEN AND" + " DENDET.MESDEN = DENDETALI.MESDEN AND" + " DENDET.PRODEN = DENDETALI.PRODEN AND" + " DENDET.PRODENDET = DENDETALI.PRODENDET AND" + " DENDETALI.CODFORASS = FORASS.CODFORASS" + " AND DENDETALI.ANNDEN = " + dataTable14.Rows[index3]["ANNDEN"]?.ToString() + " AND DENDETALI.MESDEN = " + dataTable14.Rows[index3]["MESDEN"]?.ToString() + " AND DENDETALI.MAT = " + dataTable14.Rows[index3]["MAT"]?.ToString() + " AND DENDETALI.PRODEN = " + dataTable14.Rows[index3]["PRODEN"]?.ToString() + " AND DENDETALI.PRODENDET = " + dataTable14.Rows[index3]["PRODENDET"]?.ToString() + " GROUP BY FORASS.CATFORASS" + " ORDER BY FORASS.CATFORASS";
              dataTable5.Clear();
              dataTable5 = objDataAccess.GetDataTable(strSQL4);
              for (int index4 = 0; index4 <= dataTable5.Rows.Count - 1; ++index4)
              {
                if (dataTable5.Rows[index4]["CATFORASS"].ToString().Trim() == "TFR")
                  num3 = Math.Abs(Convert.ToDecimal(dataTable5.Rows[index4]["IMPCON"])) - Math.Abs(Convert.ToDecimal(dataTable5.Rows[index4]["IMPRIDUZ"]));
                if (dataTable5.Rows[index4]["CATFORASS"].ToString().Trim() == "PREV")
                  num4 = Math.Abs(Convert.ToDecimal(dataTable5.Rows[index4]["IMPCON"])) - Math.Abs(Convert.ToDecimal(dataTable5.Rows[index4]["IMPRIDUZ"]));
                if (dataTable5.Rows[index4]["CATFORASS"].ToString().Trim() == "INF")
                  num5 = Math.Abs(Convert.ToDecimal(dataTable5.Rows[index4]["IMPCON"])) - Math.Abs(Convert.ToDecimal(dataTable5.Rows[index4]["IMPRIDUZ"]));
              }
              num7 = num3 + num4 + num5;
              num2 = num7 * 4M / 100M;
              string[] strArray1 = num2.ToString().Trim().Split(',');
              string str7 = strArray1[0];
              if (strArray1.Length > 1)
                str3 = strArray1[1];
              num2 = str3.Length <= 2 ? Decimal.Round(num2, 2) : (!(str3.Substring(str3.Length - 1, 1) == "5") ? Decimal.Round(num2, 2) : Convert.ToDecimal(str7 + "," + str3.Substring(0, 2)));
              Decimal d2 = num7 / num23 * num22;
              string[] strArray2 = d2.ToString().Trim().Split(',');
              string str8 = strArray2[0];
              str3 = strArray2.Length <= 1 ? "0" : strArray2[1];
              num6 = str3.Length <= 2 ? Decimal.Round(d2, 2) : (!(str3.Substring(2, 1) == "5") ? Decimal.Round(d2, 2) : Convert.ToDecimal(str8 + "," + str3.Substring(0, 2)));
              num24 = num7;
              num7 += num2;
              num15 -= num7;
              num11 -= num5;
              num10 -= num4;
              num9 -= num3;
              num14 -= num6;
              num8 -= num2;
            }
            else
            {
              num24 = num15 - num8;
              num7 = num15;
              num5 = num11;
              num4 = num10;
              num3 = num9;
              num6 = num14;
              num2 = num8;
              num15 -= num7;
              num11 -= num5;
              num10 -= num4;
              num9 -= num3;
              num14 -= num6;
              num8 -= num2;
            }
            string strSQL5 = " UPDATE DENDET SET IMPRIDUZ = IMPRIDUZ + " + num24.ToString().Replace(',', '.') + " " + " WHERE CODPOS = " + dataTable14.Rows[index3]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable14.Rows[index3]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable14.Rows[index3]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable14.Rows[index3]["PRODEN"]?.ToString() + " AND PRODENDET = " + dataTable14.Rows[index3]["PRODENDET"]?.ToString() + " AND MAT = " + dataTable14.Rows[index3]["MAT"]?.ToString();
            objDataAccess.WriteTransactionData(strSQL5, CommandType.Text);
            string strSQL6 = " UPDATE DENDETALI SET IMPRIDUZ = IMPRIDUZ + " + num3.ToString().Replace(',', '.') + " " + " WHERE CODPOS = " + dataTable14.Rows[index3]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable14.Rows[index3]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable14.Rows[index3]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable14.Rows[index3]["PRODEN"]?.ToString() + " AND PRODENDET = " + dataTable14.Rows[index3]["PRODENDET"]?.ToString() + " AND MAT = " + dataTable14.Rows[index3]["MAT"]?.ToString() + " AND CODFORASS IN ( SELECT CODFORASS FROM FORASS WHERE " + " CATFORASS = 'TFR')";
            objDataAccess.WriteTransactionData(strSQL6, CommandType.Text);
            string strSQL7 = " UPDATE DENDETALI SET IMPRIDUZ = IMPRIDUZ + " + num4.ToString().Replace(',', '.') + " " + " WHERE CODPOS = " + dataTable14.Rows[index3]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable14.Rows[index3]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable14.Rows[index3]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable14.Rows[index3]["PRODEN"]?.ToString() + " AND PRODENDET = " + dataTable14.Rows[index3]["PRODENDET"]?.ToString() + " AND MAT = " + dataTable14.Rows[index3]["MAT"]?.ToString() + " AND CODFORASS IN ( SELECT CODFORASS FROM FORASS WHERE " + " CATFORASS = 'PREV')";
            objDataAccess.WriteTransactionData(strSQL7, CommandType.Text);
            string strSQL8 = " UPDATE DENDETALI SET IMPRIDUZ = IMPRIDUZ + " + num5.ToString().Replace(',', '.') + " " + " WHERE CODPOS = " + dataTable14.Rows[index3]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable14.Rows[index3]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable14.Rows[index3]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable14.Rows[index3]["PRODEN"]?.ToString() + " AND PRODENDET = " + dataTable14.Rows[index3]["PRODENDET"]?.ToString() + " AND MAT = " + dataTable14.Rows[index3]["MAT"]?.ToString() + " AND CODFORASS IN ( SELECT CODFORASS FROM FORASS WHERE " + " CATFORASS = 'INF')";
            objDataAccess.WriteTransactionData(strSQL8, CommandType.Text);
            string strSQL9 = " SELECT * FROM MOVIMSAP A" + " WHERE A.CODCAUS = " + DBMethods.DoublePeakForSql(dataTable14.Rows[index3][nameof (NUMMOV)].ToString().Substring(0, 2)) + " AND A.ANNORIF = " + dataTable14.Rows[index3][nameof (NUMMOV)].ToString().Substring(3, 4) + " AND A.NUMERORIF =" + dataTable14.Rows[index3][nameof (NUMMOV)].ToString().Substring(8) + " AND A.CODPOSIZ = " + dataTable14.Rows[index3]["CODPOS"]?.ToString() + " AND A.STATOSED <> 'A'";
            dataTable6 = objDataAccess.GetDataTable(strSQL9);
            string strSQL10 = " SELECT PROGMOV FROM MOVIMSAP WHERE " + " CODCAUS = " + DBMethods.DoublePeakForSql(NUMMOV.Substring(0, 2)) + " AND ANNORIF = " + NUMMOV.Substring(3, 4) + " AND NUMERORIF = " + NUMMOV.Substring(8) + " AND CODPOSIZ = " + dataTable14.Rows[index3]["CODPOS"]?.ToString() + " AND STATOSED <> 'A' ";
            num16 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL10, CommandType.Text));
            string strSQL11 = " SELECT PARTITA FROM MOVIMSAP WHERE " + " CODCAUS = " + DBMethods.DoublePeakForSql(NUMMOV.Substring(0, 2)) + " AND ANNORIF = " + NUMMOV.Substring(3, 4) + " AND NUMERORIF = " + NUMMOV.Substring(8) + " AND CODPOSIZ = " + dataTable14.Rows[index3]["CODPOS"]?.ToString() + " AND STATOSED <> 'A' ";
            string str9 = objDataAccess.Get1ValueFromSQL(strSQL11, CommandType.Text);
            num19 -= num2;
            if (num19 <= -0.01M)
            {
              if (num19 <= -0.04M)
              {
                if (!(num19 == -0.05M))
                {
                  if (num19 == -0.04M)
                  {
                    num2 -= 0.04M;
                    num7 -= 0.04M;
                  }
                }
                else
                {
                  num2 -= 0.05M;
                  num7 -= 0.05M;
                }
              }
              else if (!(num19 == -0.03M))
              {
                if (!(num19 == -0.02M))
                {
                  if (num19 == -0.01M)
                  {
                    num2 -= 0.01M;
                    num7 -= 0.01M;
                  }
                }
                else
                {
                  num2 -= 0.02M;
                  num7 -= 0.02M;
                }
              }
              else
              {
                num2 -= 0.03M;
                num7 -= 0.03M;
              }
            }
            else if (num19 <= 0.02M)
            {
              if (!(num19 == 0.01M))
              {
                if (num19 == 0.02M)
                {
                  num2 += 0.02M;
                  num7 += 0.02M;
                }
              }
              else
              {
                num2 += 0.01M;
                num7 += 0.01M;
              }
            }
            else if (!(num19 == 0.03M))
            {
              if (!(num19 == 0.04M))
              {
                if (num19 == 0.05M)
                {
                  num2 += 0.05M;
                  num7 += 0.05M;
                }
              }
              else
              {
                num2 += 0.04M;
                num7 += 0.04M;
              }
            }
            else
            {
              num2 += 0.03M;
              num7 += 0.03M;
            }
            if (dataTable6.Rows.Count > 0)
            {
              if (FLGMULTIPLE)
                ModContabilita.MODULE_PULISCI_CONTABILITA(objDataAccess, dataTable6.Rows[0]["PARTITA"].ToString(), Convert.ToInt32(dataTable6.Rows[0]["PROGMOV"]), str9, num16, num7, Convert.ToDecimal(dataTable6.Rows[0]["IMPORTO"]), FLGMULTIPLE, num3, num4, num5, num2);
              else if (index1 == 0)
                ModContabilita.MODULE_PULISCI_CONTABILITA(objDataAccess, dataTable6.Rows[0]["PARTITA"].ToString(), Convert.ToInt32(dataTable6.Rows[0]["PROGMOV"]), str9, num16, num7, Convert.ToDecimal(dataTable6.Rows[0]["IMPORTO"]), FLGMULTIPLE);
            }
            int index5;
            for (index5 = 0; index5 <= dataTable6.Rows.Count - 1; ++index5)
            {
              str2 = dataTable6.Rows[index5]["PARTITA"].ToString();
              string strSQL12 = "SELECT * FROM MOVRETSAP WHERE " + " PARTRET = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGRRET = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString();
              Decimal num25;
              int PROGSPLIA;
              if (objDataAccess.GetDataTable(strSQL12).Rows.Count > 0)
              {
                num25 = num7 * -1M;
                string str10 = " UPDATE MOVRETSAP SET" + " IMPRET = (IMPRET + " + num25.ToString().Replace(',', '.') + "), ";
                num25 = num3 * -1M;
                string str11 = num25.ToString().Replace(',', '.');
                string str12 = str10 + " IMPTFR = (IMPTFR + " + str11 + "), ";
                num25 = num4 * -1M;
                string str13 = num25.ToString().Replace(',', '.');
                string str14 = str12 + " IMPFP = (IMPFP + " + str13 + "), ";
                num25 = num5 * -1M;
                string str15 = num25.ToString().Replace(',', '.');
                string str16 = str14 + " IMPINF = (IMPINF + " + str15 + "), " + " IMPFAP = 0, ";
                num25 = num2 * -1M;
                string str17 = num25.ToString().Replace(',', '.');
                string strSQL13 = str16 + " IMPAD = (IMPAD + " + str17 + "), " + " IMPALTRO= 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND PARTRET = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGRRET = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim();
                objDataAccess.WriteTransactionData(strSQL13, CommandType.Text);
                string strSQL14 = " SELECT IMPAD FROM MOVRETSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND PARTRET = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGRRET = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim();
                Decimal num26 = Convert.ToDecimal(objDataAccess.Get1ValueFromSQL(strSQL14, CommandType.Text));
                num25 = num7 * -1M;
                string strSQL15 = " UPDATE MOVASSAP SET" + " IMPASS = (IMPASS + " + num25.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND PARTASS = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGRASS = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND CODCAUASS =" + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["CODCAUS"].ToString().Trim());
                objDataAccess.WriteTransactionData(strSQL15, CommandType.Text);
                num25 = num7 * -1M;
                string strSQL16 = " UPDATE ABBINSAP SET" + " IMPORTO = (IMPORTO - " + num25.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOVD = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOVA = " + num16.ToString();
                objDataAccess.WriteTransactionData(strSQL16, CommandType.Text);
                if (num3 != 0M)
                {
                  string str18 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'TFR' ";
                  string strSQL17 = !(TIPANN == "AC") ? str18 + " AND ANNPRE = 'S' " : str18 + " AND ANNPRE = 'N' ";
                  string str19 = objDataAccess.Get1ValueFromSQL(strSQL17, CommandType.Text);
                  string strSQL18 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str19.Trim());
                  PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL18, CommandType.Text));
                  string strSQL19 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str19.Trim());
                  PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL19, CommandType.Text));
                  if (PROGSPLID.ToString() == "0")
                  {
                    string strSQL20 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON1 = 'TFR' ";
                    PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL20, CommandType.Text));
                  }
                  num25 = num3 * -1M;
                  string strSQL21 = " UPDATE SPLABSAP SET" + " IMPORTO = (IMPORTO - " + num25.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOVD = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOVA = " + num16.ToString() + " AND PROGSPLID = " + PROGSPLID.ToString() + " AND PROGSPLIA = " + PROGSPLIA.ToString();
                  objDataAccess.WriteTransactionData(strSQL21, CommandType.Text);
                  str1 = "";
                  PROGSPLID = 0;
                  PROGSPLIA = 0;
                }
                if (num4 != 0M)
                {
                  string str20 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'PREV' ";
                  string strSQL22 = !(TIPANN == "AC") ? str20 + " AND ANNPRE = 'S' " : str20 + " AND ANNPRE = 'N' ";
                  string str21 = objDataAccess.Get1ValueFromSQL(strSQL22, CommandType.Text);
                  string strSQL23 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str21.Trim());
                  PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL23, CommandType.Text));
                  string strSQL24 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str21.Trim());
                  PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL24, CommandType.Text));
                  if (PROGSPLID.ToString() == "0")
                  {
                    string strSQL25 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON1 = 'FP' ";
                    PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL25, CommandType.Text));
                  }
                  num25 = num4 * -1M;
                  string strSQL26 = " UPDATE SPLABSAP SET" + " IMPORTO = (IMPORTO - " + num25.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOVD = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOVA = " + num16.ToString() + " AND PROGSPLID = " + PROGSPLID.ToString() + " AND PROGSPLIA = " + PROGSPLIA.ToString();
                  objDataAccess.WriteTransactionData(strSQL26, CommandType.Text);
                  str1 = "";
                  PROGSPLID = 0;
                  PROGSPLIA = 0;
                }
                if (num5 != 0M)
                {
                  string str22 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'INF' ";
                  string strSQL27 = !(TIPANN == "AC") ? str22 + " AND ANNPRE = 'S' " : str22 + " AND ANNPRE = 'N' ";
                  string str23 = objDataAccess.Get1ValueFromSQL(strSQL27, CommandType.Text);
                  string strSQL28 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str23.Trim());
                  PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL28, CommandType.Text));
                  string strSQL29 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str23.Trim());
                  PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL29, CommandType.Text));
                  if (PROGSPLID.ToString() == "0")
                  {
                    string strSQL30 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON1 = 'INF' ";
                    PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL30, CommandType.Text));
                  }
                  num25 = num5 * -1M;
                  string strSQL31 = " UPDATE SPLABSAP SET" + " IMPORTO = (IMPORTO - " + num25.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOVD = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOVA = " + num16.ToString() + " AND PROGSPLID = " + PROGSPLID.ToString() + " AND PROGSPLIA = " + PROGSPLIA.ToString();
                  objDataAccess.WriteTransactionData(strSQL31, CommandType.Text);
                  str1 = "";
                  PROGSPLID = 0;
                  PROGSPLIA = 0;
                }
                if (num2 != 0M)
                {
                  string str24 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ADDIZIONALE' ";
                  string strSQL32 = !(TIPANN == "AC") ? str24 + " AND ANNPRE = 'S' " : str24 + " AND ANNPRE = 'N' ";
                  string str25 = objDataAccess.Get1ValueFromSQL(strSQL32, CommandType.Text);
                  string strSQL33 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str25.Trim());
                  PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL33, CommandType.Text));
                  string strSQL34 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str25.Trim());
                  PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL34, CommandType.Text));
                  if (PROGSPLID.ToString() == "0")
                  {
                    string strSQL35 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON1 = 'AD' ";
                    PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL35, CommandType.Text));
                  }
                  num25 = num2 * -1M;
                  string strSQL36 = " UPDATE SPLABSAP SET" + " IMPORTO = (IMPORTO - " + num25.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOVD = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOVA = " + num16.ToString() + " AND PROGSPLID = " + PROGSPLID.ToString() + " AND PROGSPLIA = " + PROGSPLIA.ToString();
                  objDataAccess.WriteTransactionData(strSQL36, CommandType.Text);
                  num25 = Math.Abs(num26);
                  string strSQL37 = " UPDATE SPLABSAP SET" + " IMPORTO = " + num25.ToString().Replace(',', '.') + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOVD = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOVA = " + num16.ToString() + " AND PROGSPLID = " + PROGSPLID.ToString() + " AND PROGSPLIA = " + PROGSPLIA.ToString();
                  objDataAccess.WriteTransactionData(strSQL37, CommandType.Text);
                  str1 = "";
                  PROGSPLID = 0;
                  PROGSPLIA = 0;
                }
              }
              else
              {
                string str26 = dataTable6.Rows[index5]["PARTITA"].ToString().Trim();
                int int32 = Convert.ToInt32(dataTable6.Rows[index5]["PROGMOV"]);
                string str27 = dataTable6.Rows[index5]["CODCAUS"].ToString().Trim();
                clsWriteDb.WRITE_INSERT_MOVRETSAP(objDataAccess, u, ref str9, ref num16, ref str26, ref int32, ref str27, num7 * -1M, num3 * -1M, num4 * -1M, num5 * -1M, 0M, num2 * -1M, IMPAC * -1M, IMPAB * -1M, 0M);
                clsWriteDb.WRITE_INSERT_MOVASSAP(objDataAccess, u, ref str9, ref num16, ref str26, ref int32, ref str27, num7);
                int PROGABBIN = clsWriteDb.WRITE_INSERT_ABBINSAP(objDataAccess, u, ref str26, ref int32, ref str9, ref num16, num7);
                if (num3 != 0M)
                {
                  string str28 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'TFR' ";
                  string strSQL38 = !(TIPANN == "AC") ? str28 + " AND ANNPRE = 'S' " : str28 + " AND ANNPRE = 'N' ";
                  string GESCON = objDataAccess.Get1ValueFromSQL(strSQL38, CommandType.Text);
                  string strSQL39 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                  PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL39, CommandType.Text));
                  if (PROGSPLIA.ToString() == "0")
                  {
                    string strSQL40 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'TFR' ";
                    PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL40, CommandType.Text));
                  }
                  if (PROGSPLIA.ToString() == "0")
                  {
                    string strSQL41 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'RTFR' ";
                    PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL41, CommandType.Text));
                  }
                  string strSQL42 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                  PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL42, CommandType.Text));
                  if (PROGSPLID.ToString() == "0")
                  {
                    string strSQL43 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = 'TFR' ";
                    PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL43, CommandType.Text));
                  }
                  if (PROGSPLID.ToString() == "0")
                  {
                    string strSQL44 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = 'PTFR' ";
                    PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL44, CommandType.Text));
                  }
                  clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, dataTable6.Rows[index5]["PARTITA"].ToString().Trim(), Convert.ToInt32(dataTable6.Rows[index5]["PROGMOV"]), str9, num16, PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, num3);
                  str1 = "";
                  PROGSPLID = 0;
                  PROGSPLIA = 0;
                }
                if (num4 != 0M)
                {
                  string str29 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'PREV' ";
                  string strSQL45 = !(TIPANN == "AC") ? str29 + " AND ANNPRE = 'S' " : str29 + " AND ANNPRE = 'N' ";
                  string GESCON = objDataAccess.Get1ValueFromSQL(strSQL45, CommandType.Text);
                  string strSQL46 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                  PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL46, CommandType.Text));
                  if (PROGSPLIA.ToString() == "0")
                  {
                    string strSQL47 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'FP' ";
                    PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL47, CommandType.Text));
                  }
                  if (PROGSPLIA.ToString() == "0")
                  {
                    string strSQL48 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'RFP' ";
                    PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL48, CommandType.Text));
                  }
                  string strSQL49 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                  PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL49, CommandType.Text));
                  if (PROGSPLID.ToString() == "0")
                  {
                    string strSQL50 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = 'FP' ";
                    PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL50, CommandType.Text));
                  }
                  if (PROGSPLID.ToString() == "0")
                  {
                    string strSQL51 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = 'PFP' ";
                    PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL51, CommandType.Text));
                  }
                  clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, dataTable6.Rows[index5]["PARTITA"].ToString().Trim(), Convert.ToInt32(dataTable6.Rows[index5]["PROGMOV"]), str9, num16, PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, num4);
                  str1 = "";
                  PROGSPLID = 0;
                  PROGSPLIA = 0;
                }
                if (num5 != 0M)
                {
                  string str30 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'INF' ";
                  string strSQL52 = !(TIPANN == "AC") ? str30 + " AND ANNPRE = 'S' " : str30 + " AND ANNPRE = 'N' ";
                  string GESCON = objDataAccess.Get1ValueFromSQL(strSQL52, CommandType.Text);
                  string strSQL53 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                  PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL53, CommandType.Text));
                  if (PROGSPLIA.ToString() == "0")
                  {
                    string strSQL54 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'INF' ";
                    PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL54, CommandType.Text));
                  }
                  if (PROGSPLIA.ToString() == "0")
                  {
                    string strSQL55 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'RINF' ";
                    PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL55, CommandType.Text));
                  }
                  string strSQL56 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                  PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL56, CommandType.Text));
                  if (PROGSPLID.ToString() == "0")
                  {
                    string strSQL57 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = 'INF' ";
                    PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL57, CommandType.Text));
                  }
                  if (PROGSPLID.ToString() == "0")
                  {
                    string strSQL58 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = 'PINF' ";
                    PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL58, CommandType.Text));
                  }
                  clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, dataTable6.Rows[index5]["PARTITA"].ToString().Trim(), Convert.ToInt32(dataTable6.Rows[index5]["PROGMOV"]), str9, num16, PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, num5);
                  str1 = "";
                  PROGSPLID = 0;
                  PROGSPLIA = 0;
                }
                if (num2 != 0M)
                {
                  string str31 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ADDIZIONALE' ";
                  string strSQL59 = !(TIPANN == "AC") ? str31 + " AND ANNPRE = 'S' " : str31 + " AND ANNPRE = 'N' ";
                  string GESCON = objDataAccess.Get1ValueFromSQL(strSQL59, CommandType.Text);
                  string strSQL60 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON1= " + DBMethods.DoublePeakForSql(GESCON.Trim());
                  PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL60, CommandType.Text));
                  if (PROGSPLIA.ToString() == "0")
                  {
                    string strSQL61 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'AD' ";
                    PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL61, CommandType.Text));
                  }
                  if (PROGSPLIA.ToString() == "0")
                  {
                    string strSQL62 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'RAD' ";
                    PROGSPLIA = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL62, CommandType.Text));
                  }
                  string strSQL63 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                  PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL63, CommandType.Text));
                  if (PROGSPLID.ToString() == "0")
                  {
                    string strSQL64 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = 'AD' ";
                    PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL64, CommandType.Text));
                  }
                  if (PROGSPLID.ToString() == "0")
                  {
                    string strSQL65 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = 'PAD' ";
                    PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL65, CommandType.Text));
                  }
                  clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, dataTable6.Rows[index5]["PARTITA"].ToString().Trim(), Convert.ToInt32(dataTable6.Rows[index5]["PROGMOV"]), str9, num16, PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, num2);
                  str1 = "";
                  PROGSPLID = 0;
                  PROGSPLIA = 0;
                }
              }
              string strSQL66 = " UPDATE MOVIMSAP SET" + " IMPRIDUZ = IMPRIDUZ + " + num7.ToString().Replace(',', '.') + ", " + " IMPABB = IMPABB + " + num7.ToString().Replace(',', '.') + ", " + " IMPRESID = IMPRESID - " + num7.ToString().Replace(',', '.') + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim();
              objDataAccess.WriteTransactionData(strSQL66, CommandType.Text);
              string strSQL67 = " UPDATE MOVIMSAP SET" + " IMPABB = IMPABB + (" + num7.ToString().Replace(',', '.') + " * -1 ), " + " IMPRESID = IMPRESID - (" + num7.ToString().Replace(',', '.') + " * -1 ) " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString();
              objDataAccess.WriteTransactionData(strSQL67, CommandType.Text);
              string strSQL68 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON1 IN ('TFR', 'TFRO')";
              dataTable2.Clear();
              dataTable2 = objDataAccess.GetDataTable(strSQL68);
              bool flag = dataTable2.Rows.Count > 1;
              if (num3 != 0M)
              {
                if (flag)
                {
                  string str32 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'N' " + " AND CATFORASS = 'TFR' ";
                  string strSQL69 = !(str6 == "AC") ? str32 + " AND ANNPRE = 'S' " : str32 + " AND ANNPRE = 'N' ";
                  string str33 = objDataAccess.Get1ValueFromSQL(strSQL69, CommandType.Text);
                  string strSQL70 = "SELECT COUNT(*) FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str33.Trim());
                  if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL70, CommandType.Text)) == 0)
                    str33 = "TFR";
                  string strSQL71 = "SELECT IMPRES FROM SPLIMPOSAP WHERE" + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str33.Trim());
                  dataTable2.Clear();
                  dataTable2 = objDataAccess.GetDataTable(strSQL71);
                  if (Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]) < num3)
                  {
                    string strSQL72 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + dataTable2.Rows[0]["IMPRES"].ToString().Replace(',', '.') + "), " + " IMPABB = ( IMPABB + " + dataTable2.Rows[0]["IMPRES"].ToString().Replace(',', '.') + "), " + " IMPRES = ( IMPRES - " + dataTable2.Rows[0]["IMPRES"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str33.ToString().Trim());
                    objDataAccess.WriteTransactionData(strSQL72, CommandType.Text);
                    if (str33.Trim() == "TFR")
                    {
                      num25 = num3 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str34 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num25.ToString().Replace(',', '.') + "), ";
                      num25 = num3 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str35 = num25.ToString().Replace(',', '.');
                      string str36 = str34 + " IMPABB = ( IMPABB + " + str35 + "), ";
                      num25 = num3 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str37 = num25.ToString().Replace(',', '.');
                      string strSQL73 = str36 + " IMPRES = ( IMPRES - " + str37 + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = 'PTFR'";
                      objDataAccess.WriteTransactionData(strSQL73, CommandType.Text);
                    }
                    else
                    {
                      num25 = num3 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str38 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num25.ToString().Replace(',', '.') + "), ";
                      num25 = num3 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str39 = num25.ToString().Replace(',', '.');
                      string str40 = str38 + " IMPABB = ( IMPABB + " + str39 + "), ";
                      num25 = num3 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str41 = num25.ToString().Replace(',', '.');
                      string strSQL74 = str40 + " IMPRES = ( IMPRES - " + str41 + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = 'TFR'";
                      objDataAccess.WriteTransactionData(strSQL74, CommandType.Text);
                    }
                  }
                  else
                  {
                    string strSQL75 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num3.ToString().Replace(',', '.') + "), " + " IMPABB = ( IMPABB + " + num3.ToString().Replace(',', '.') + "), " + " IMPRES = ( IMPRES - " + num3.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str33.ToString().Trim());
                    objDataAccess.WriteTransactionData(strSQL75, CommandType.Text);
                  }
                  string str42 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'TFR' ";
                  string strSQL76 = !(str6 == "AC") ? str42 + " AND ANNPRE = 'S' " : str42 + " AND ANNPRE = 'N' ";
                  string str43 = objDataAccess.Get1ValueFromSQL(strSQL76, CommandType.Text);
                  string strSQL77 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num3.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num3.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str43.ToString().Trim());
                  int int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL77, CommandType.Text));
                  if (int32 == 0)
                  {
                    string strSQL78 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num3.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num3.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'PTFR' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL78, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL79 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num3.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num3.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'TFR' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL79, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL80 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num3.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num3.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'RTFR' ";
                    num1 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL80, CommandType.Text));
                  }
                }
                else
                {
                  string str44 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'N' " + " AND CATFORASS = 'TFR' ";
                  string strSQL81 = !(str6 == "AC") ? str44 + " AND ANNPRE = 'S' " : str44 + " AND ANNPRE = 'N' ";
                  string str45 = objDataAccess.Get1ValueFromSQL(strSQL81, CommandType.Text);
                  string strSQL82 = "SELECT COUNT(*) FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str45.Trim());
                  if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL82, CommandType.Text)) == 0)
                    str45 = "TFR";
                  string strSQL83 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num3.ToString().Replace(',', '.') + "), " + " IMPABB = ( IMPABB + " + num3.ToString().Replace(',', '.') + "), " + " IMPRES = ( IMPRES - " + num3.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str45.ToString().Trim());
                  objDataAccess.WriteTransactionData(strSQL83, CommandType.Text);
                  string str46 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'TFR' ";
                  string strSQL84 = !(str6 == "AC") ? str46 + " AND ANNPRE = 'S' " : str46 + " AND ANNPRE = 'N' ";
                  string str47 = objDataAccess.Get1ValueFromSQL(strSQL84, CommandType.Text);
                  string strSQL85 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num3.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num3.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str47.ToString().Trim());
                  int int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL85, CommandType.Text));
                  if (int32 == 0)
                  {
                    string strSQL86 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num3.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num3.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'PTFR' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL86, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL87 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num3.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num3.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'TFR' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL87, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL88 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num3.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num3.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'RTFR' ";
                    num1 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL88, CommandType.Text));
                  }
                }
              }
              if (num4 != 0M)
              {
                if (flag)
                {
                  string str48 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'N' " + " AND CATFORASS = 'PREV' ";
                  string strSQL89 = !(str6 == "AC") ? str48 + " AND ANNPRE = 'S' " : str48 + " AND ANNPRE = 'N' ";
                  string str49 = objDataAccess.Get1ValueFromSQL(strSQL89, CommandType.Text);
                  string strSQL90 = "SELECT COUNT(*) FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str49.Trim());
                  if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL90, CommandType.Text)) == 0)
                    str49 = "FP";
                  string strSQL91 = "SELECT IMPRES FROM SPLIMPOSAP WHERE" + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str49.Trim());
                  dataTable2.Clear();
                  dataTable2 = objDataAccess.GetDataTable(strSQL91);
                  if (Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]) < num4)
                  {
                    string strSQL92 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + dataTable2.Rows[0]["IMPRES"].ToString().Replace(',', '.') + "), " + " IMPABB = ( IMPABB + " + dataTable2.Rows[0]["IMPRES"].ToString().Replace(',', '.') + "), " + " IMPRES = ( IMPRES - " + dataTable2.Rows[0]["IMPRES"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str49.ToString().Trim());
                    objDataAccess.WriteTransactionData(strSQL92, CommandType.Text);
                    if (str49.Trim() == "FP")
                    {
                      num25 = num4 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str50 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num25.ToString().Replace(',', '.') + "), ";
                      num25 = num4 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str51 = num25.ToString().Replace(',', '.');
                      string str52 = str50 + " IMPABB = ( IMPABB + " + str51 + "), ";
                      num25 = num4 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str53 = num25.ToString().Replace(',', '.');
                      string strSQL93 = str52 + " IMPRES = ( IMPRES - " + str53 + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = 'PFP'";
                      objDataAccess.WriteTransactionData(strSQL93, CommandType.Text);
                    }
                    else
                    {
                      num25 = num4 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str54 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num25.ToString().Replace(',', '.') + "), ";
                      num25 = num4 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str55 = num25.ToString().Replace(',', '.');
                      string str56 = str54 + " IMPABB = ( IMPABB + " + str55 + "), ";
                      num25 = num4 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str57 = num25.ToString().Replace(',', '.');
                      string strSQL94 = str56 + " IMPRES = ( IMPRES - " + str57 + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = 'FP'";
                      objDataAccess.WriteTransactionData(strSQL94, CommandType.Text);
                    }
                  }
                  else
                  {
                    string strSQL95 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num4.ToString().Replace(',', '.') + "), " + " IMPABB = ( IMPABB + " + num4.ToString().Replace(',', '.') + "), " + " IMPRES = ( IMPRES - " + num4.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str49.ToString().Trim());
                    objDataAccess.WriteTransactionData(strSQL95, CommandType.Text);
                  }
                  string str58 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'PREV' ";
                  string strSQL96 = !(str6 == "AC") ? str58 + " AND ANNPRE = 'S' " : str58 + " AND ANNPRE = 'N' ";
                  string str59 = objDataAccess.Get1ValueFromSQL(strSQL96, CommandType.Text);
                  string strSQL97 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num4.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num4.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str59.ToString().Trim());
                  int int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL97, CommandType.Text));
                  if (int32 == 0)
                  {
                    string strSQL98 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num4.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num4.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'PFP' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL98, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL99 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num4.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num4.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'FP' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL99, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL100 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num4.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num4.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'RFP' ";
                    num1 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL100, CommandType.Text));
                  }
                }
                else
                {
                  string str60 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'N' " + " AND CATFORASS = 'PREV' ";
                  string strSQL101 = !(str6 == "AC") ? str60 + " AND ANNPRE = 'S' " : str60 + " AND ANNPRE = 'N' ";
                  string str61 = objDataAccess.Get1ValueFromSQL(strSQL101, CommandType.Text);
                  string strSQL102 = "SELECT COUNT(*) FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str61.Trim());
                  if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL102, CommandType.Text)) == 0)
                    str61 = "FP";
                  string strSQL103 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num4.ToString().Replace(',', '.') + "), " + " IMPABB = ( IMPABB + " + num4.ToString().Replace(',', '.') + "), " + " IMPRES = ( IMPRES - " + num4.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str61.ToString().Trim());
                  objDataAccess.WriteTransactionData(strSQL103, CommandType.Text);
                  string str62 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'PREV' ";
                  string strSQL104 = !(str6 == "AC") ? str62 + " AND ANNPRE = 'S' " : str62 + " AND ANNPRE = 'N' ";
                  string str63 = objDataAccess.Get1ValueFromSQL(strSQL104, CommandType.Text);
                  string strSQL105 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num4.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num4.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str63.ToString().Trim());
                  int int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL105, CommandType.Text));
                  if (int32 == 0)
                  {
                    string strSQL106 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num4.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num4.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'PFP' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL106, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL107 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num4.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num4.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'FP' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL107, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL108 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num4.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num4.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'RFP' ";
                    num1 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL108, CommandType.Text));
                  }
                }
              }
              if (num5 != 0M)
              {
                if (flag)
                {
                  string str64 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'N' " + " AND CATFORASS = 'INF' ";
                  string strSQL109 = !(str6 == "AC") ? str64 + " AND ANNPRE = 'S' " : str64 + " AND ANNPRE = 'N' ";
                  string str65 = objDataAccess.Get1ValueFromSQL(strSQL109, CommandType.Text);
                  string strSQL110 = "SELECT COUNT(*) FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str65.Trim());
                  if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL110, CommandType.Text)) == 0)
                    str65 = "INF";
                  string strSQL111 = "SELECT IMPRES FROM SPLIMPOSAP WHERE" + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str65.Trim());
                  dataTable2.Clear();
                  dataTable2 = objDataAccess.GetDataTable(strSQL111);
                  if (Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]) < num5)
                  {
                    string strSQL112 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + dataTable2.Rows[0]["IMPRES"].ToString().Replace(',', '.') + "), " + " IMPABB = ( IMPABB + " + dataTable2.Rows[0]["IMPRES"].ToString().Replace(',', '.') + "), " + " IMPRES = ( IMPRES - " + dataTable2.Rows[0]["IMPRES"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str65.ToString().Trim());
                    objDataAccess.WriteTransactionData(strSQL112, CommandType.Text);
                    if (str65.Trim() == "INF")
                    {
                      num25 = num5 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str66 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num25.ToString().Replace(',', '.') + "), ";
                      num25 = num5 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str67 = num25.ToString().Replace(',', '.');
                      string str68 = str66 + " IMPABB = ( IMPABB + " + str67 + "), ";
                      num25 = num5 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str69 = num25.ToString().Replace(',', '.');
                      string strSQL113 = str68 + " IMPRES = ( IMPRES - " + str69 + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = 'PINF'";
                      objDataAccess.WriteTransactionData(strSQL113, CommandType.Text);
                    }
                    else
                    {
                      num25 = num5 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str70 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num25.ToString().Replace(',', '.') + "), ";
                      num25 = num5 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str71 = num25.ToString().Replace(',', '.');
                      string str72 = str70 + " IMPABB = ( IMPABB + " + str71 + "), ";
                      num25 = num5 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str73 = num25.ToString().Replace(',', '.');
                      string strSQL114 = str72 + " IMPRES = ( IMPRES - " + str73 + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = 'INF'";
                      objDataAccess.WriteTransactionData(strSQL114, CommandType.Text);
                    }
                  }
                  else
                  {
                    string strSQL115 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num5.ToString().Replace(',', '.') + "), " + " IMPABB = ( IMPABB + " + num5.ToString().Replace(',', '.') + "), " + " IMPRES = ( IMPRES - " + num5.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str65.ToString().Trim());
                    objDataAccess.WriteTransactionData(strSQL115, CommandType.Text);
                  }
                  string str74 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'INF' ";
                  string strSQL116 = !(str6 == "AC") ? str74 + " AND ANNPRE = 'S' " : str74 + " AND ANNPRE = 'N' ";
                  string str75 = objDataAccess.Get1ValueFromSQL(strSQL116, CommandType.Text);
                  string strSQL117 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num5.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num5.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str75.ToString().Trim());
                  int int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL117, CommandType.Text));
                  if (int32 == 0)
                  {
                    string strSQL118 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num5.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num5.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'PINF' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL118, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL119 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num5.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num5.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'INF' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL119, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL120 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num5.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num5.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'RINF' ";
                    num1 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL120, CommandType.Text));
                  }
                }
                else
                {
                  string str76 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'N' " + " AND CATFORASS = 'INF' ";
                  string strSQL121 = !(str6 == "AC") ? str76 + " AND ANNPRE = 'S' " : str76 + " AND ANNPRE = 'N' ";
                  string str77 = objDataAccess.Get1ValueFromSQL(strSQL121, CommandType.Text);
                  string strSQL122 = "SELECT COUNT(*) FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str77.Trim());
                  if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL122, CommandType.Text)) == 0)
                    str77 = "INF";
                  string strSQL123 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num5.ToString().Replace(',', '.') + "), " + " IMPABB = ( IMPABB + " + num5.ToString().Replace(',', '.') + "), " + " IMPRES = ( IMPRES - " + num5.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str77.ToString().Trim());
                  objDataAccess.WriteTransactionData(strSQL123, CommandType.Text);
                  string str78 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'INF' ";
                  string strSQL124 = !(str6 == "AC") ? str78 + " AND ANNPRE = 'S' " : str78 + " AND ANNPRE = 'N' ";
                  string str79 = objDataAccess.Get1ValueFromSQL(strSQL124, CommandType.Text);
                  string strSQL125 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num5.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num5.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str79.ToString().Trim());
                  int int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL125, CommandType.Text));
                  if (int32 == 0)
                  {
                    string strSQL126 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num5.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num5.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'PINF' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL126, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL127 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num5.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num5.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'INF' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL127, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL128 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num5.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num5.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'RINF' ";
                    num1 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL128, CommandType.Text));
                  }
                }
              }
              if (num2 != 0M)
              {
                if (flag)
                {
                  string str80 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'N' " + " AND CATIMP = 'ADDIZIONALE' ";
                  string strSQL129 = !(str6 == "AC") ? str80 + " AND ANNPRE = 'S' " : str80 + " AND ANNPRE = 'N' ";
                  string str81 = objDataAccess.Get1ValueFromSQL(strSQL129, CommandType.Text);
                  string strSQL130 = "SELECT COUNT(*) FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str81.Trim());
                  if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL130, CommandType.Text)) == 0)
                    str81 = "AD";
                  string strSQL131 = "SELECT IMPRES FROM SPLIMPOSAP WHERE" + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str81.Trim());
                  dataTable2.Clear();
                  dataTable2 = objDataAccess.GetDataTable(strSQL131);
                  if (Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]) < num2)
                  {
                    string strSQL132 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + dataTable2.Rows[0]["IMPRES"].ToString().Replace(',', '.') + "), " + " IMPABB = ( IMPABB + " + dataTable2.Rows[0]["IMPRES"].ToString().Replace(',', '.') + "), " + " IMPRES = ( IMPRES - " + dataTable2.Rows[0]["IMPRES"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str81.ToString().Trim());
                    objDataAccess.WriteTransactionData(strSQL132, CommandType.Text);
                    if (str81.Trim() == "AD")
                    {
                      num25 = num2 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str82 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num25.ToString().Replace(',', '.') + "), ";
                      num25 = num2 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str83 = num25.ToString().Replace(',', '.');
                      string str84 = str82 + " IMPABB = ( IMPABB + " + str83 + "), ";
                      num25 = num2 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str85 = num25.ToString().Replace(',', '.');
                      string strSQL133 = str84 + " IMPRES = ( IMPRES - " + str85 + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = 'PAD'";
                      objDataAccess.WriteTransactionData(strSQL133, CommandType.Text);
                    }
                    else
                    {
                      num25 = num2 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str86 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num25.ToString().Replace(',', '.') + "), ";
                      num25 = num2 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str87 = num25.ToString().Replace(',', '.');
                      string str88 = str86 + " IMPABB = ( IMPABB + " + str87 + "), ";
                      num25 = num2 - Convert.ToDecimal(dataTable2.Rows[0]["IMPRES"]);
                      string str89 = num25.ToString().Replace(',', '.');
                      string strSQL134 = str88 + " IMPRES = ( IMPRES - " + str89 + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = 'AD'";
                      objDataAccess.WriteTransactionData(strSQL134, CommandType.Text);
                    }
                  }
                  else
                  {
                    string strSQL135 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num2.ToString().Replace(',', '.') + "), " + " IMPABB = ( IMPABB + " + num2.ToString().Replace(',', '.') + "), " + " IMPRES = ( IMPRES - " + num2.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str81.ToString().Trim());
                    objDataAccess.WriteTransactionData(strSQL135, CommandType.Text);
                  }
                  string str90 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ADDIZIONALE' ";
                  string strSQL136 = !(str6 == "AC") ? str90 + " AND ANNPRE = 'S' " : str90 + " AND ANNPRE = 'N' ";
                  string str91 = objDataAccess.Get1ValueFromSQL(strSQL136, CommandType.Text);
                  string strSQL137 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num2.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num2.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str91.ToString().Trim());
                  int int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL137, CommandType.Text));
                  if (int32 == 0)
                  {
                    string strSQL138 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num2.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num2.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'PAD' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL138, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL139 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num2.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num2.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'AD' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL139, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL140 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num2.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num2.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'RAD' ";
                    num1 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL140, CommandType.Text));
                  }
                }
                else
                {
                  string str92 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'N' " + " AND CATIMP = 'ADDIZIONALE' ";
                  string strSQL141 = !(str6 == "AC") ? str92 + " AND ANNPRE = 'S' " : str92 + " AND ANNPRE = 'N' ";
                  string str93 = objDataAccess.Get1ValueFromSQL(strSQL141, CommandType.Text);
                  string strSQL142 = "SELECT COUNT(*) FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str93.Trim());
                  if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL142, CommandType.Text)) == 0)
                    str93 = "AD";
                  string strSQL143 = " UPDATE SPLIMPOSAP SET" + " IMPRIDU = ( IMPRIDU + " + num2.ToString().Replace(',', '.') + "), " + " IMPABB = ( IMPABB + " + num2.ToString().Replace(',', '.') + "), " + " IMPRES = ( IMPRES - " + num2.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index5]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index5]["PROGMOV"].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str93.ToString().Trim());
                  objDataAccess.WriteTransactionData(strSQL143, CommandType.Text);
                  string str94 = "SELECT GESCON FROM CONTI WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ADDIZIONALE' ";
                  string strSQL144 = !(str6 == "AC") ? str94 + " AND ANNPRE = 'S' " : str94 + " AND ANNPRE = 'N' ";
                  string str95 = objDataAccess.Get1ValueFromSQL(strSQL144, CommandType.Text);
                  string strSQL145 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num2.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num2.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(str95.ToString().Trim());
                  objDataAccess.WriteTransactionData(strSQL145, CommandType.Text);
                  int int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL145, CommandType.Text));
                  if (int32 == 0)
                  {
                    string strSQL146 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num2.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num2.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'PAD' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL146, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL147 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num2.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num2.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'AD' ";
                    int32 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL147, CommandType.Text));
                  }
                  if (int32 == 0)
                  {
                    string strSQL148 = " UPDATE SPLIMPOSAP SET" + " IMPABB = ( IMPABB + (" + num2.ToString().Replace(',', '.') + ") * -1), " + " IMPRES = ( IMPRES - (" + num2.ToString().Replace(',', '.') + ") * -1)" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(str9) + " AND PROGMOV = " + num16.ToString() + " AND GESTCON = 'RAD' ";
                    num1 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL148, CommandType.Text));
                  }
                }
              }
              if (num15 == 0M)
                break;
            }
            if (index5 <= dataTable6.Rows.Count - 1)
              break;
          }
        }
      }
      else
      {
        string strSQL149 = " SELECT A.CODPOS, A.MAT, A.ANNDEN, A.MESDEN, A.PRODEN, A.PRODENDET,  " + " A.IMPCONDEL, A.ALIQUOTA, A.PERFAP, A.IMPFAP, B.NUMMOV" + " FROM  DENDET A LEFT JOIN DENTES B ON" + " A.CODPOS = B.CODPOS AND" + " A.ANNDEN = B.ANNDEN AND" + " A.MESDEN = B.MESDEN AND" + " A.PRODEN = B.PRODEN" + " WHERE A.ANNRET = " + ANNORET.ToString() + " AND A.PRORET = " + PRORET.ToString() + " AND A.PRORETTES = " + PRORETTES.ToString() + " ORDER BY A.DATCONMOV";
        dataTable13 = objDataAccess.GetDataTable(strSQL149);
        for (int index6 = 0; index6 <= dataTable13.Rows.Count - 1; ++index6)
        {
          Decimal num27 = Convert.ToDecimal(dataTable13.Rows[index6]["ALIQUOTA"].ToString().Trim());
          Decimal num28 = Convert.ToDecimal(dataTable13.Rows[index6]["PERFAP"].ToString().Trim());
          string strSQL150 = " SELECT * FROM MOVIMSAP " + " WHERE CODCAUS = " + DBMethods.DoublePeakForSql(dataTable13.Rows[index6][nameof (NUMMOV)].ToString().Substring(0, 2)) + " AND ANNORIF = " + dataTable13.Rows[index6][nameof (NUMMOV)].ToString().Substring(3, 4) + " AND NUMERORIF =" + dataTable13.Rows[index6][nameof (NUMMOV)].ToString().Substring(8) + " AND CODPOSIZ = " + dataTable13.Rows[index6]["CODPOS"]?.ToString() + " AND STATOSED <> 'A'";
          dataTable6 = objDataAccess.GetDataTable(strSQL150);
          for (int index7 = 0; index7 <= dataTable6.Rows.Count - 1; ++index7)
          {
            str2 = dataTable6.Rows[index7]["PARTITA"].ToString();
            string strSQL151 = " SELECT FORASS.CATFORASS, SUM(DENDETALI.IMPCON) AS IMPCON" + " FROM DENDET, DENDETALI, FORASS" + " WHERE DENDET.CODPOS = DENDETALI.CODPOS AND" + " DENDET.MAT = DENDETALI.MAT AND" + " DENDET.ANNDEN = DENDETALI.ANNDEN AND" + " DENDET.MESDEN = DENDETALI.MESDEN AND" + " DENDET.PRODEN = DENDETALI.PRODEN AND" + " DENDET.PRODENDET = DENDETALI.PRODENDET AND" + " DENDETALI.CODFORASS = FORASS.CODFORASS" + " AND DENDETALI.ANNDEN = " + dataTable13.Rows[index6]["ANNDEN"]?.ToString() + " AND DENDETALI.MESDEN = " + dataTable13.Rows[index6]["MESDEN"]?.ToString() + " AND DENDETALI.MAT = " + dataTable13.Rows[index6]["MAT"]?.ToString() + " AND DENDETALI.PRODEN = " + dataTable13.Rows[index6]["PRODEN"]?.ToString() + " AND DENDETALI.PRODENDET = " + dataTable13.Rows[index6]["PRODENDET"]?.ToString() + " GROUP BY FORASS.CATFORASS" + " ORDER BY FORASS.CATFORASS";
            DataTable dataTable15 = objDataAccess.GetDataTable(strSQL151);
            string strSQL152 = " SELECT PROGMOV FROM MOVIMSAP WHERE " + " CODCAUS = " + DBMethods.DoublePeakForSql(NUMMOV.Substring(0, 2)) + " AND ANNORIF = " + NUMMOV.Substring(3, 4) + " AND NUMERORIF = " + NUMMOV.Substring(8) + " AND CODPOSIZ = " + dataTable13.Rows[index6]["CODPOS"]?.ToString() + " AND STATOSED <> 'A' ";
            num16 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL152, CommandType.Text));
            string strSQL153 = " SELECT PARTITA FROM MOVIMSAP WHERE " + " CODCAUS = " + DBMethods.DoublePeakForSql(NUMMOV.Substring(0, 2)) + " AND ANNORIF = " + NUMMOV.Substring(3, 4) + " AND NUMERORIF = " + NUMMOV.Substring(8) + " AND CODPOSIZ = " + dataTable13.Rows[index6]["CODPOS"]?.ToString() + " AND STATOSED <> 'A' ";
            string PARTITA = objDataAccess.Get1ValueFromSQL(strSQL153, CommandType.Text);
            Decimal IMPTFR = 0M;
            Decimal IMPFP = 0M;
            Decimal IMPINF = 0M;
            for (int index8 = 0; index8 <= dataTable15.Rows.Count - 1; ++index8)
            {
              if (dataTable15.Rows[index8]["CATFORASS"].ToString().Trim() == "TFR")
                IMPTFR = Convert.ToDecimal(dataTable15.Rows[index8]["IMPCON"]);
              if (dataTable15.Rows[index8]["CATFORASS"].ToString().Trim() == "PREV")
                IMPFP = Convert.ToDecimal(dataTable15.Rows[index8]["IMPCON"]);
              if (dataTable15.Rows[index8]["CATFORASS"].ToString().Trim() == "INF")
                IMPINF = Convert.ToDecimal(dataTable15.Rows[index8]["IMPCON"]);
            }
            Decimal d3 = Convert.ToDecimal(IMPTFR + IMPINF + IMPFP) * 4M / 100M;
            string[] strArray3 = d3.ToString().Trim().Split(',');
            string str96 = strArray3[0];
            if (strArray3.Length > 1)
              str3 = strArray3[1];
            Decimal IMPAD = str3.Length <= 2 ? Decimal.Round(d3, 2) : (!(str3.Substring(2, 1) == "5") ? Decimal.Round(d3, 2) : Convert.ToDecimal(str96 + "," + str3.Substring(0, 2)));
            Decimal d4 = (IMPTFR + IMPINF + IMPFP) / num27 * num28;
            string[] strArray4 = d4.ToString().Trim().Split(',');
            string str97 = strArray4[0];
            str3 = strArray4.Length <= 1 ? "0" : strArray4[1];
            Decimal num29 = str3.Length <= 2 ? Decimal.Round(d4, 2) : (!(str3.Substring(2, 1) == "5") ? Decimal.Round(d4, 2) : Convert.ToDecimal(str97 + "," + str3.Substring(0, 2)));
            Decimal IMP_RETTIFICA = IMPTFR + IMPINF + IMPFP + IMPAD;
            string strSQL154 = "SELECT * FROM MOVRETSAP WHERE " + " PARTRET = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index7]["PARTITA"].ToString()) + " AND PROGRRET = " + dataTable6.Rows[index7]["PROGMOV"]?.ToString() + " AND PARTITA = " + DBMethods.DoublePeakForSql(PARTITA) + " AND PROGMOV = " + num16.ToString();
            if (objDataAccess.GetDataTable(strSQL154).Rows.Count > 0)
            {
              string strSQL155 = " UPDATE MOVRETSAP SET" + " IMPRET = (IMPRET + " + IMP_RETTIFICA.ToString().Replace(',', '.') + "), " + " IMPTFR = (IMPTFR + (" + IMPTFR.ToString().Replace(',', '.') + ")), " + " IMPFP = (IMPFP + " + IMPFP.ToString().Replace(',', '.') + "), " + " IMPINF = (IMPINF + " + IMPINF.ToString().Replace(',', '.') + "), " + " IMPFAP = 0, " + " IMPAD = (IMPAD + " + IMPAD.ToString().Replace(',', '.') + "), " + " IMPAC = (IMPAC + " + IMPAC.ToString().Replace(',', '.') + "), " + " IMPAB = (IMPAB + " + IMPAB.ToString().Replace(',', '.') + "), " + " IMPALTRO= 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITA) + " AND PROGMOV = " + num16.ToString() + " AND PARTRET = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index7]["PARTITA"].ToString().Trim()) + " AND PROGRRET = " + dataTable6.Rows[index7]["PROGMOV"].ToString().Trim();
              objDataAccess.WriteTransactionData(strSQL155, CommandType.Text);
              string strSQL156 = " UPDATE MOVRETSAP SET" + " IMPAD = ROUND((IMPTFR + IMPFP + IMPINF+IMPFAP)* 4 / 100, 2) " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITA) + " AND PROGMOV = " + num16.ToString() + " AND PARTRET = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index7]["PARTITA"].ToString().Trim()) + " AND PROGRRET = " + dataTable6.Rows[index7]["PROGMOV"].ToString().Trim();
              objDataAccess.WriteTransactionData(strSQL156, CommandType.Text);
            }
            else
            {
              string PARTITARET = dataTable6.Rows[index7]["PARTITA"].ToString().Trim();
              int int32 = Convert.ToInt32(dataTable6.Rows[index7]["PROGMOV"]);
              string CODCAURET = dataTable6.Rows[index7]["CODCAUS"].ToString().Trim();
              clsWriteDb.WRITE_INSERT_MOVRETSAP(objDataAccess, u, ref PARTITA, ref num16, ref PARTITARET, ref int32, ref CODCAURET, IMP_RETTIFICA, IMPTFR, IMPFP, IMPINF, 0M, IMPAD, IMPAC, IMPAB, 0M);
            }
          }
        }
      }
      if (TIPIMP == "-")
      {
        for (int index9 = 0; index9 <= dataTable13.Rows.Count - 1; ++index9)
        {
          string strSQL157 = "SELECT NUMMOV, CODPOS, ANNDEN, MESDEN, PRODEN FROM DENDET WHERE CODPOS = " + dataTable13.Rows[index9]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable13.Rows[index9]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable13.Rows[index9]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable13.Rows[index9]["PRODEN"]?.ToString() + " AND ESIRET = 'S'" + " AND PRODENDET = ( SELECT MAX(PRODENDET) FROM DENDET WHERE CODPOS = " + dataTable13.Rows[index9]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable13.Rows[index9]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable13.Rows[index9]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable13.Rows[index9]["PRODEN"]?.ToString() + " AND ESIRET = 'S')";
          dataTable2.Clear();
          dataTable2 = objDataAccess.GetDataTable(strSQL157);
          if (dataTable2.Rows.Count > 0)
          {
            for (int index10 = 0; index10 <= dataTable2.Rows.Count - 1; ++index10)
            {
              if (dataTable2.Rows[index10][nameof (NUMMOV)].ToString().Trim() != str4)
              {
                string strSQL158 = "SELECT COUNT(*) FROM DENTES WHERE CODPOS = " + dataTable2.Rows[index10]["CODPOS"]?.ToString() + " AND ANNDEN = " + dataTable2.Rows[index10]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable2.Rows[index10]["MESDEN"]?.ToString() + " AND NUMMOV = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index10][nameof (NUMMOV)].ToString().Trim()) + " AND NUMMOVANN IS NULL";
                if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL158, CommandType.Text)) > 1)
                {
                  string strSQL159 = "SELECT PARTITA, PROGMOV FROM MOVIMSAP " + " WHERE CODCAUS = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index10][nameof (NUMMOV)].ToString().Substring(0, 2)) + " AND ANNORIF = " + dataTable2.Rows[index10][nameof (NUMMOV)].ToString().Substring(3, 4) + " AND NUMERORIF =" + dataTable2.Rows[index10][nameof (NUMMOV)].ToString().Substring(8) + " AND CODPOSIZ = " + dataTable2.Rows[index10]["CODPOS"]?.ToString();
                  dataTable6.Clear();
                  dataTable6 = objDataAccess.GetDataTable(strSQL159);
                  if (dataTable6.Rows.Count > 0)
                  {
                    string strSQL160 = " SELECT * FROM SPLIMPOSAP WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[0]["PROGMOV"]?.ToString() + " AND IMPRES < 0" + " AND GESTCON <> 'CAZA'";
                    dataTable3.Clear();
                    dataTable3 = objDataAccess.GetDataTable(strSQL160);
                    if (dataTable3.Rows.Count > 0)
                    {
                      switch (dataTable3.Rows[0]["GESTCON1"].ToString().Trim())
                      {
                        case "AD":
                        case "FP":
                        case "INF":
                        case "TFR":
                          string strSQL161 = "SELECT * FROM SPLABSAP WHERE " + " PARTITAD = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString()) + " AND PROGMOVD = " + dataTable6.Rows[0]["PROGMOV"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(str2.ToString()) + " AND PROGMOVA = " + num16.ToString();
                          dataTable12.Clear();
                          dataTable12 = objDataAccess.GetDataTable(strSQL161);
                          for (int index11 = 0; index11 <= dataTable12.Rows.Count - 1; ++index11)
                          {
                            string str98 = " UPDATE SPLIMPOSAP SET IMPRES = IMPRES - " + dataTable12.Rows[index11]["IMPORTO"].ToString().Trim().Replace(',', '.') + ", " + " IMPRIDU = IMPRIDU + " + dataTable12.Rows[index11]["IMPORTO"].ToString().Trim().Replace(',', '.') + ", " + " IMPABB = IMPABB + " + dataTable12.Rows[index11]["IMPORTO"].ToString().Trim().Replace(',', '.') + " " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[0]["PROGMOV"]?.ToString();
                            string strSQL162 = !(dataTable12.Rows[index11]["GESTCON"].ToString().Trim().Substring(dataTable12.Rows[index11]["GESTCON"].ToString().Trim().Length - 1, 1) == "O") ? str98 + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(dataTable12.Rows[index11]["GESTCON"].ToString().Trim() + "O") : str98 + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(dataTable12.Rows[index11]["GESTCON"].ToString().Trim());
                            objDataAccess.WriteTransactionData(strSQL162, CommandType.Text);
                            string strSQL163 = " UPDATE SPLIMPOSAP SET IMPRES = IMPRES + " + dataTable12.Rows[index11]["IMPORTO"].ToString().Trim().Replace(',', '.') + ", " + " IMPRIDU = IMPRIDU - " + dataTable12.Rows[index11]["IMPORTO"].ToString().Trim().Replace(',', '.') + ", " + " IMPABB = IMPABB - " + dataTable12.Rows[index11]["IMPORTO"].ToString().Trim().Replace(',', '.') + " " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[0]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(dataTable12.Rows[index11]["GESTCON"].ToString().Trim());
                            objDataAccess.WriteTransactionData(strSQL163, CommandType.Text);
                            string str99 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[0]["PROGMOV"]?.ToString();
                            string strSQL164 = !(dataTable12.Rows[index11]["GESTCON"].ToString().Trim().Substring(dataTable12.Rows[index11]["GESTCON"].ToString().Trim().Length - 1, 1) == "O") ? str99 + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(dataTable12.Rows[index11]["GESTCON"].ToString().Trim() + "O") : str99 + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(dataTable12.Rows[index11]["GESTCON"].ToString().Trim());
                            PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL164, CommandType.Text));
                            string strSQL165 = (!(dataTable12.Rows[index11]["GESTCON"].ToString().Trim().Substring(dataTable12.Rows[index11]["GESTCON"].ToString().Trim().Length - 1, 1) == "O") ? " UPDATE SPLABSAP SET GESTCON = " + DBMethods.DoublePeakForSql(dataTable12.Rows[index11]["GESTCON"].ToString().Trim() + "O") + ", " : " UPDATE SPLABSAP SET GESTCON = " + DBMethods.DoublePeakForSql(dataTable12.Rows[index11]["GESTCON"].ToString().Trim()) + ", ") + " PROGSPLID = " + PROGSPLID.ToString() + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString()) + " AND PROGMOVD = " + dataTable6.Rows[0]["PROGMOV"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(str2) + " AND PROGMOVA = " + num16.ToString() + " AND PROGSPLID = " + dataTable12.Rows[index11]["PROGSPLID"]?.ToString() + " AND PROGSPLIA = " + dataTable12.Rows[index11]["PROGSPLIA"]?.ToString();
                            objDataAccess.WriteTransactionData(strSQL165, CommandType.Text);
                            PROGSPLID = 0;
                          }
                          break;
                        case "ADO":
                        case "FPO":
                        case "INFO":
                        case "TFRO":
                          string strSQL166 = "SELECT * FROM SPLABSAP WHERE " + " PARTITAD = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString()) + " AND PROGMOVD = " + dataTable6.Rows[0]["PROGMOV"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(str2) + " AND PROGMOVA = " + num16.ToString();
                          dataTable12.Clear();
                          dataTable12 = objDataAccess.GetDataTable(strSQL166);
                          for (int index12 = 0; index12 <= dataTable12.Rows.Count - 1; ++index12)
                          {
                            string strSQL167 = " UPDATE SPLIMPOSAP SET IMPRES = IMPRES - " + dataTable12.Rows[index12]["IMPORTO"].ToString().Trim().Replace(',', '.') + ", " + " IMPRIDU = IMPRIDU + " + dataTable12.Rows[index12]["IMPORTO"].ToString().Trim().Replace(',', '.') + ", " + " IMPABB = IMPABB + " + dataTable12.Rows[index12]["IMPORTO"].ToString().Trim().Replace(',', '.') + " " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[0]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(dataTable12.Rows[index12]["GESTCON"].ToString().Trim().Substring(0, dataTable12.Rows[index12]["GESTCON"].ToString().Trim().Length - 1));
                            objDataAccess.WriteTransactionData(strSQL167, CommandType.Text);
                            string strSQL168 = " UPDATE SPLIMPOSAP SET IMPRES = IMPRES + " + dataTable12.Rows[index12]["IMPORTO"].ToString().Trim().Replace(',', '.') + ", " + " IMPRIDU = IMPRIDU - " + dataTable12.Rows[index12]["IMPORTO"].ToString().Trim().Replace(',', '.') + ", " + " IMPABB = IMPABB - " + dataTable12.Rows[index12]["IMPORTO"].ToString().Trim().Replace(',', '.') + " " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[0]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(dataTable12.Rows[index12]["GESTCON"].ToString());
                            objDataAccess.WriteTransactionData(strSQL168, CommandType.Text);
                            string strSQL169 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[0]["PROGMOV"]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable12.Rows[index12]["GESTCON"].ToString().Trim().Substring(0, dataTable12.Rows[index12]["GESTCON"].ToString().Trim().Length - 1));
                            PROGSPLID = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL169, CommandType.Text));
                            string strSQL170 = " UPDATE SPLABSAP SET GESTCON = " + DBMethods.DoublePeakForSql(dataTable12.Rows[index12]["GESTCON"].ToString().Trim().Substring(0, dataTable12.Rows[index12]["GESTCON"].ToString().Trim().Length - 1)) + ", " + " PROGSPLID = " + PROGSPLID.ToString() + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString()) + " AND PROGMOVD = " + dataTable6.Rows[0]["PROGMOV"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(str2) + " AND PROGMOVA = " + num16.ToString() + " AND PROGSPLID = " + dataTable12.Rows[index12]["PROGSPLID"]?.ToString() + " AND PROGSPLIA = " + dataTable12.Rows[index12]["PROGSPLIA"]?.ToString();
                            objDataAccess.WriteTransactionData(strSQL170, CommandType.Text);
                            PROGSPLID = 0;
                          }
                          break;
                      }
                    }
                  }
                }
              }
              str4 = dataTable2.Rows[index10][nameof (NUMMOV)].ToString().Trim();
            }
          }
        }
        dataTable1.Rows.Add();
        dataTable1.Rows[dataTable1.Rows.Count - 1]["PARTITA"] = (object) str2;
        dataTable1.Rows[dataTable1.Rows.Count - 1]["PROGMOV"] = (object) num16;
      }
      else
        dataTable1.Clear();
      return dataTable1;
    }

    public string WRITE_CONTABILITA_ANNULLAMENTO_SANZIONI(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      ref string MSGErrore,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      string TIPANN,
      string CODCAUANN,
      string ANNO_BILANCIO,
      string TIPMOV,
      ref Decimal IMPORTO_SANZIONE,
      ref Decimal IMPABB,
      ref Decimal IMPADDREC,
      ref Decimal IMPASSCON,
      ref string PARTITA_SANZIONE,
      ref Decimal PROGMOV_SANZIONE,
      string CODCAUSAN = "",
      int ANNCOM = 0)
    {
      string str1 = "";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      DataTable dataTable4 = new DataTable();
      DataTable dataTable5 = new DataTable();
      int num1 = 0;
      string str2 = "";
      string PARTITA_MOVIMENTO = "";
      Decimal PROGMOV_MOVIMENTO = 0M;
      int num2 = 0;
      int num3 = 0;
      Decimal num4 = 0.0M;
      Decimal IMPORTO = 0.0M;
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      string str3 = new Utile().Module_GetDataSistema().ToString();
      string str4 = TIPMOV.Trim();
      if (!(str4 == "ANN_SAN_RT_MD") && !(str4 == "ANN_SAN_RT_RD"))
      {
        if (str4 == "ANN_SAN_MD" || str4 == "ANN_SAN_RD")
        {
          string TIPO_OPERAZIONE = "ANNULLAMENTO SANZIONI NOTIFICHE DIPA";
          str2 = clsWriteDb.WRITE_INSERT_MOVIMSAP(objDataAccess, u, ref MSGErrore, CODPOS, ANNDEN, MESDEN, PRODEN, CODCAUANN, str3, Convert.ToInt32(ANNO_BILANCIO), str3, IMPORTO_SANZIONE * -1M, IMPABB, IMPADDREC, IMPASSCON, "S", "S", TIPANN, ref PARTITA_MOVIMENTO, ref PROGMOV_MOVIMENTO, ref PARTITA_SANZIONE, ref PROGMOV_SANZIONE, TIPO_OPERAZIONE: TIPO_OPERAZIONE);
          string str5 = "UPDATE DENTES SET " + " DATSANANN = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + ", " + " CODSANANN = " + DBMethods.DoublePeakForSql(CODCAUANN.Trim().PadLeft(2, '0')) + ", " + " NUMSANANN = " + DBMethods.DoublePeakForSql(str2) + ", " + " BILSANANN = " + ANNO_BILANCIO + ", ";
          if (TIPMOV != "ANN_AR")
            str5 = str5 + " TIPANNSANANN = " + DBMethods.DoublePeakForSql(TIPANN) + ", ";
          string strSQL1 = str5 + " UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " ULTAGG = CURRENT_TIMESTAMP" + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString();
          objDataAccess.WriteTransactionData(strSQL1, CommandType.Text);
          string strSQL2 = "UPDATE DENDET SET " + " DATSANANN = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + ", " + " CODSANANN = " + DBMethods.DoublePeakForSql(CODCAUANN.Trim().PadLeft(2, '0')) + ", " + " NUMSANANN = " + DBMethods.DoublePeakForSql(str2) + ", " + " BILSANANN = " + ANNO_BILANCIO + ", " + " PARTITASANANN = " + DBMethods.DoublePeakForSql(PARTITA_SANZIONE) + ", " + " PROGMOVSANANN = " + PROGMOV_SANZIONE.ToString() + ", " + " UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " ULTAGG = CURRENT_TIMESTAMP" + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND TIPMOV<>'RT'";
          objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
        }
      }
      else
      {
        string TIPO_OPERAZIONE = "ANNULLAMENTO SANZIONI RETTIFICHE";
        string str6 = CODPOS.ToString();
        str2 = clsWriteDb.WRITE_INSERT_MOVIMSAP(objDataAccess, u, ref MSGErrore, Convert.ToInt32(str6), ANNDEN, MESDEN, PRODEN, CODCAUANN, str3, Convert.ToInt32(ANNO_BILANCIO), str3.ToString(), IMPORTO_SANZIONE * -1M, 0.0M, 0.0M, 0.0M, "S", "S", TIPANN, ref PARTITA_MOVIMENTO, ref PROGMOV_MOVIMENTO, ref PARTITA_SANZIONE, ref PROGMOV_SANZIONE, TIPO_OPERAZIONE: TIPO_OPERAZIONE);
        string str7 = "UPDATE DENDET SET " + " DATSANANN = " + DBMethods.Db2Date(str3) + ", " + " CODSANANN = " + DBMethods.DoublePeakForSql(CODCAUANN.Trim().PadLeft(2, '0')) + ", " + " NUMSANANN = " + DBMethods.DoublePeakForSql(str2) + ", ";
        if (ANNCOM != 9999)
          str7 = str7 + " TIPANNMOVARRANN = " + DBMethods.DoublePeakForSql(TIPANN) + ", ";
        string strSQL = str7 + " PARTITASANANN = " + DBMethods.DoublePeakForSql(PARTITA_SANZIONE) + ", " + " PROGMOVSANANN = " + PROGMOV_SANZIONE.ToString() + ", " + " BILSANANN = " + ANNO_BILANCIO + ", " + " UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " ULTAGG = CURRENT_TIMESTAMP" + " WHERE DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN=" + ANNDEN.ToString() + " AND DENDET.MESDEN=" + MESDEN.ToString() + " AND DENDET.PRODEN=" + PRODEN.ToString() + " AND NUMMOVANN = " + DBMethods.DoublePeakForSql(str2) + " AND CODCAUSAN= " + DBMethods.DoublePeakForSql(CODCAUSAN) + " AND VALUE(ANNCOM, 9999) = " + ANNCOM.ToString() + " AND NUMSAN IS NOT NULL";
        objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
      }
      string strSQL3 = " SELECT NUMSAN, CODPOS, CODCAUSAN FROM DENTES WHERE NUMSANANN = " + DBMethods.DoublePeakForSql(str2) + " AND CODPOS = " + CODPOS.ToString();
      DataTable dataTable6 = objDataAccess.GetDataTable(strSQL3);
      string strSQL4 = " SELECT * FROM MOVIMSAP " + " WHERE ANNORIF = " + dataTable6.Rows[0]["NUMSAN"]?.ToString() + " AND NUMERORIF =" + dataTable6.Rows[0]["NUMSAN"]?.ToString() + " AND CODPOSIZ = " + dataTable6.Rows[0][nameof (CODPOS)]?.ToString() + " AND CODCAUS = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0][nameof (CODCAUSAN)].ToString().Trim()) + " AND STATOSED <> 'A'";
      dataTable6.Clear();
      DataTable dataTable7 = objDataAccess.GetDataTable(strSQL4);
      for (int index1 = 0; index1 <= dataTable7.Rows.Count - 1; ++index1)
      {
        if (index1 == 0)
        {
          string strSQL5 = "SELECT A.PARTITAD, A.PROGMOVD, A.PARTITAA, A.PROGMOVA, A.PROGABBIN, A.IMPORTO, B.CODCAUS " + " FROM ABBINSAP A, MOVIMSAP B" + " WHERE A.PARTITAA = B.PARTITA" + " AND A.PROGMOVA = B.PROGMOV" + " AND B.CODCAUS IN ('09', '03', '71')" + " AND A.PARTITAD = " + DBMethods.DoublePeakForSql(dataTable7.Rows[index1]["PARTITA"].ToString()) + " AND A.PROGMOVD = " + dataTable7.Rows[index1]["PROGMOV"]?.ToString() + " AND A.STATOVAL = 'V' " + " ORDER BY INT(CODCAUS) DESC";
          dataTable2.Clear();
          dataTable2 = objDataAccess.GetDataTable(strSQL5);
          for (int index2 = 0; index2 <= dataTable2.Rows.Count - 1; ++index2)
          {
            string strSQL6 = "SELECT COUNT(*) FROM MOVASSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAA"].ToString()) + " AND PROGMOV = " + dataTable2.Rows[index2]["PROGMOVA"]?.ToString() + " AND PARTASS = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAD"].ToString()) + " AND PROGRASS = " + dataTable2.Rows[index2]["PROGMOVD"]?.ToString();
            if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL6, CommandType.Text)) == 0)
            {
              string strSQL7 = "UPDATE MOVIMSAP SET " + " IMPABB = (IMPABB + " + dataTable2.Rows[index2]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRESID = (IMPRESID - " + dataTable2.Rows[index2]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAA"].ToString()) + " AND PROGMOV = " + dataTable2.Rows[index2]["PROGMOVA"]?.ToString();
              objDataAccess.WriteTransactionData(strSQL7, CommandType.Text);
              string strSQL8 = "UPDATE MOVIMSAP SET " + " IMPABB = (IMPABB + " + dataTable2.Rows[index2]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRESID = (IMPRESID - " + dataTable2.Rows[index2]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable7.Rows[index1]["PROGMOV"]?.ToString();
              objDataAccess.WriteTransactionData(strSQL8, CommandType.Text);
              string strSQL9 = " SELECT * FROM SPLABSAP " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAD"].ToString()) + " AND PROGMOVD = " + dataTable2.Rows[index2]["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAA"].ToString()) + " AND PROGMOVA = " + dataTable2.Rows[index2]["PROGMOVA"]?.ToString() + " AND PROGABBIN = " + dataTable2.Rows[index2]["PROGABBIN"]?.ToString();
              dataTable5.Clear();
              dataTable5 = objDataAccess.GetDataTable(strSQL9);
              for (int index3 = 0; index3 <= dataTable5.Rows.Count - 1; ++index3)
              {
                string strSQL10 = "UPDATE SPLIMPOSAP SET " + " IMPABB = (IMPABB + " + dataTable5.Rows[index3]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRES = (IMPRES - " + dataTable5.Rows[index3]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAA"].ToString()) + " AND PROGMOV = " + dataTable2.Rows[index2]["PROGMOVA"]?.ToString() + " AND PROGSPLI = " + dataTable5.Rows[index3]["PROGSPLIA"]?.ToString();
                objDataAccess.WriteTransactionData(strSQL10, CommandType.Text);
                string strSQL11 = "UPDATE SPLIMPOSAP SET " + " IMPABB = (IMPABB + " + dataTable5.Rows[index3]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRES = (IMPRES - " + dataTable5.Rows[index3]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable7.Rows[index1]["PROGMOV"]?.ToString() + " AND PROGSPLI = " + dataTable5.Rows[index3]["PROGSPLID"]?.ToString();
                objDataAccess.WriteTransactionData(strSQL11, CommandType.Text);
              }
              string strSQL12 = "DELETE FROM ABBINSAP " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAD"].ToString()) + " AND PROGMOVD = " + dataTable2.Rows[index2]["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAA"].ToString()) + " AND PROGMOVA = " + dataTable2.Rows[index2]["PROGMOVA"]?.ToString() + " AND PROGABBIN = " + dataTable2.Rows[index2]["PROGABBIN"]?.ToString();
              objDataAccess.WriteTransactionData(strSQL12, CommandType.Text);
              string strSQL13 = "DELETE FROM SPLABSAP " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAD"].ToString()) + " AND PROGMOVD = " + dataTable2.Rows[index2]["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAA"].ToString()) + " AND PROGMOVA = " + dataTable2.Rows[index2]["PROGMOVA"]?.ToString() + " AND PROGABBIN = " + dataTable2.Rows[index2]["PROGABBIN"]?.ToString();
              objDataAccess.WriteTransactionData(strSQL13, CommandType.Text);
            }
          }
        }
        string strSQL14 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable7.Rows[index1]["PROGMOV"]?.ToString();
        dataTable2.Clear();
        dataTable2 = objDataAccess.GetDataTable(strSQL14);
        for (int index4 = 0; index4 <= dataTable2.Rows.Count - 1; ++index4)
        {
          string str8 = dataTable2.Rows[index4]["GESTCON"].ToString().Trim();
          if (!(str8 == "SAAM") && !(str8 == "PSAA") && !(str8 == "RSAA"))
          {
            if (str8 == "CSAA")
            {
              num4 = Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
              if (num4 < 0M)
                num4 *= -1M;
            }
          }
          else if (IMPORTO > 0M)
          {
            IMPORTO = Convert.ToDecimal(IMPORTO) + Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
          }
          else
          {
            IMPORTO = Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
            if (IMPORTO < 0M)
              IMPORTO *= -1M;
          }
        }
        string str9 = dataTable7.Rows[index1]["PARTITA"].ToString().Trim();
        int int32_1 = Convert.ToInt32(PROGMOV_SANZIONE);
        int int32_2 = Convert.ToInt32(dataTable7.Rows[index1]["PROGMOV"]);
        string CODCAU_ASS = dataTable7.Rows[index1]["CODCAUS"].ToString().Trim();
        clsWriteDb.WRITE_INSERT_MOVASSAP(objDataAccess, u, ref PARTITA_SANZIONE, ref int32_1, ref str9, ref int32_2, ref CODCAU_ASS, num4);
        int PROGABBIN = clsWriteDb.WRITE_INSERT_ABBINSAP(objDataAccess, u, ref str9, ref int32_2, ref PARTITA_SANZIONE, ref int32_1, num4);
        if (IMPORTO != 0M)
        {
          string str10 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(str3) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = " + DBMethods.DoublePeakForSql(TIPMOV.Trim());
          string strSQL15 = !(TIPANN == "AC") ? str10 + " AND ANNPRE = 'S' " : str10 + " AND ANNPRE = 'N' ";
          string GESCON = objDataAccess.Get1ValueFromSQL(strSQL15, CommandType.Text);
          string strSQL16 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_SANZIONE) + " AND PROGMOV = " + PROGMOV_SANZIONE.ToString() + " AND GESTCON =  'PSAA' ";
          int int32_3 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL16, CommandType.Text));
          if (int32_3.ToString() == "0")
          {
            string strSQL17 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_SANZIONE) + " AND PROGMOV = " + PROGMOV_SANZIONE.ToString() + " AND GESTCON = 'SAAM' ";
            int32_3 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL17, CommandType.Text));
          }
          if (int32_3.ToString() == "0")
          {
            string strSQL18 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_SANZIONE) + " AND PROGMOV = " + PROGMOV_SANZIONE.ToString() + " AND GESTCON = 'RSAA' ";
            int32_3 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL18, CommandType.Text));
          }
          string strSQL19 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable7.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON =  'PSAA' ";
          int int32_4 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL19, CommandType.Text));
          if (int32_4.ToString() == "0")
          {
            string strSQL20 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable7.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'SAAM' ";
            int32_4 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL20, CommandType.Text));
          }
          if (int32_4.ToString() == "0")
          {
            string strSQL21 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable7.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'RSAA' ";
            int32_4 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL21, CommandType.Text));
          }
          clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, dataTable7.Rows[index1]["PARTITA"].ToString().Trim(), Convert.ToInt32(dataTable7.Rows[index1]["PROGMOV"]), PARTITA_SANZIONE, Convert.ToInt32(PROGMOV_SANZIONE), PROGABBIN, int32_4, int32_3, GESCON, IMPORTO);
          str1 = "";
          num2 = 0;
          num3 = 0;
        }
        string strSQL22 = "UPDATE MOVIMSAP SET " + " IMPRIDUZ =  " + num4.ToString().Replace(',', '.') + ", " + " IMPABB = " + num4.ToString().Replace(',', '.') + ", " + " IMPRESID = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[index1]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable7.Rows[index1]["PROGMOV"]?.ToString();
        objDataAccess.WriteTransactionData(strSQL22, CommandType.Text);
        string strSQL23 = "UPDATE SPLIMPOSAP SET " + " IMPRIDU = IMPORTO, " + " IMPABB = IMPORTO, " + " IMPRES = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[index1]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable7.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON <> 'CSAA'";
        objDataAccess.WriteTransactionData(strSQL23, CommandType.Text);
        if (index1 == 0)
        {
          string strSQL24 = "UPDATE MOVIMSAP SET " + " IMPABB = " + (num4 * -1M).ToString().Replace(',', '.') + ", " + " IMPRESID = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_SANZIONE) + " AND PROGMOV = " + PROGMOV_SANZIONE.ToString();
          objDataAccess.WriteTransactionData(strSQL24, CommandType.Text);
          string strSQL25 = "UPDATE SPLIMPOSAP SET " + " IMPABB = IMPORTO, " + " IMPRES = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_SANZIONE) + " AND PROGMOV = " + PROGMOV_SANZIONE.ToString() + " AND GESTCON <> 'CSAA'";
          objDataAccess.WriteTransactionData(strSQL25, CommandType.Text);
        }
        num4 = 0.0M;
        IMPORTO = 0.0M;
        num1 = 0;
      }
      return str2;
    }

    public string WRITE_CONTABILITA_ANNULLAMENTO_DIPA_NOTIFICHE(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      ref string ErrorMSG,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      string TIPANN,
      string CODCAU,
      string ANNO_BILANCIO,
      string TIPMOV,
      ref string PARTITA_MOVIMENTO,
      ref Decimal PROGMOV_MOVIMENTO,
      string NUMERO_MOVIMENTO_ORIGINE)
    {
      string str1 = "";
      string str2 = "";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      DataTable dataTable4 = new DataTable();
      DataTable dataTable5 = new DataTable();
      DataTable dataTable6 = new DataTable();
      int num1 = 0;
      string PARTITA_SANZIONE = "";
      Decimal PROGMOV_SANZIONE = 0M;
      string TIPO_OPERAZIONE1 = "";
      int num2 = 0;
      int num3 = 0;
      Decimal num4 = 0.0M;
      Decimal IMPORTO1 = 0.0M;
      Decimal IMPORTO2 = 0.0M;
      Decimal IMPORTO3 = 0.0M;
      Decimal IMPORTO4 = 0.0M;
      Decimal IMPORTO5 = 0.0M;
      Decimal IMPORTO6 = 0.0M;
      DataTable dataTable7 = new DataTable();
      Utile utile = new Utile();
      bool flag = false;
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      try
      {
        string str3 = utile.Module_GetDataSistema().ToString();
        string CODCAU1 = objDataAccess.Get1ValueFromSQL("SELECT CODCAU FROM TIPMOVCAU WHERE TIPMOV = " + DBMethods.DoublePeakForSql(TIPMOV) + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN", CommandType.Text).ToString().Trim();
        string str4 = TIPMOV.Trim();
        if (!(str4 == "ANN_AR"))
        {
          if (!(str4 == "ANN_NU"))
          {
            if (str4 == "ANN_DP")
              TIPO_OPERAZIONE1 = "ANNULLAMENTO DIPA";
          }
          else
            TIPO_OPERAZIONE1 = "ANNULLAMENTO NOTIFICHE";
        }
        else
          TIPO_OPERAZIONE1 = "ANNULLAMENTO ARRETRATI";
        string strSQL1 = "SELECT * FROM DENTES WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND NUMMOV = " + DBMethods.DoublePeakForSql(NUMERO_MOVIMENTO_ORIGINE) + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL";
        dataTable1.Clear();
        DataTable dataTable8 = objDataAccess.GetDataTable(strSQL1);
        string str5;
        if (dataTable8.Rows.Count > 1)
        {
          string strSQL2 = " SELECT SUM(IMPORTO) AS IMPCONTRIBUTO FROM " + " (SELECT IMPCON AS IMPORTO FROM DENDET WHERE " + " CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND NUMMOV = " + DBMethods.DoublePeakForSql(NUMERO_MOVIMENTO_ORIGINE) + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL ) AS TABELLA";
          DataTable dataTable9 = objDataAccess.GetDataTable(strSQL2);
          DataTable dataTable10 = new DataTable();
          string strSQL3 = " SELECT VALUE(SUM(IMPORTO),0) AS IMPCONTRIBUTO FROM " + " (SELECT IMPCONDEL AS IMPORTO FROM DENDET WHERE " + " CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND TIPMOV = 'RT'" + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL ) AS TABELLA";
          DataTable dataTable11 = objDataAccess.GetDataTable(strSQL3);
          str2 = " SELECT SUM(IMPORTO) AS IMPCONTRIBUTO FROM " + " (SELECT CASE TIPMOV " + " WHEN 'RT' THEN IMPCONDEL " + " ELSE IMPCON END AS IMPORTO FROM DENDET WHERE " + " CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND NUMMOV = " + DBMethods.DoublePeakForSql(NUMERO_MOVIMENTO_ORIGINE) + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL ) AS TABELLA";
          string strSQL4 = " SELECT SUM(IMPABB) AS IMPABB, SUM(IMPASSCON) AS  IMPASSCON" + " FROM DENTES WHERE " + " CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND NUMMOV = " + DBMethods.DoublePeakForSql(NUMERO_MOVIMENTO_ORIGINE) + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL";
          DataTable dataTable12 = objDataAccess.GetDataTable(strSQL4);
          if (Convert.ToInt32(dataTable11.Rows[0]["IMPCONTRIBUTO"]) != 0)
          {
            Decimal num5 = Convert.ToDecimal(dataTable9.Rows[0]["IMPCONTRIBUTO"]) + Convert.ToDecimal(dataTable11.Rows[0]["IMPCONTRIBUTO"]) + Decimal.Round(Convert.ToDecimal(dataTable9.Rows[0]["IMPCONTRIBUTO"]) * 4M / 100M, 2) + Decimal.Round(Convert.ToDecimal(dataTable11.Rows[0]["IMPCONTRIBUTO"]) * 4M / 100M, 2) + Convert.ToDecimal(dataTable12.Rows[0]["IMPASSCON"]) + Convert.ToDecimal(dataTable12.Rows[0]["IMPABB"]);
            if (num5 == 0M)
              throw new Exception("Attenzione... l'importo è già stato stornato per intero. Impossibile continuare");
            string TIPO_OPERAZIONE2 = "ANNULLAMENTO DIPA";
            str5 = clsWriteDb.WRITE_INSERT_MOVIMSAP(objDataAccess, u, ref ErrorMSG, CODPOS, ANNDEN, MESDEN, PRODEN, CODCAU1, str3, Convert.ToInt32(ANNO_BILANCIO), str3, num5 * -1M, Convert.ToDecimal(dataTable12.Rows[0]["IMPABB"]), Decimal.Round(Convert.ToDecimal(dataTable9.Rows[0]["IMPCONTRIBUTO"]) * 4M / 100M, 2), Convert.ToDecimal(dataTable12.Rows[0]["IMPASSCON"]), "N", "S", TIPANN, ref PARTITA_MOVIMENTO, ref PROGMOV_MOVIMENTO, ref PARTITA_SANZIONE, ref PROGMOV_SANZIONE, TIPO_OPERAZIONE: TIPO_OPERAZIONE2, NUMMOV_ORIGINE: NUMERO_MOVIMENTO_ORIGINE.Trim(), IMPADD_RET: Decimal.Round(Convert.ToDecimal(dataTable11.Rows[0]["IMPCONTRIBUTO"]) * 4M / 100M, 2));
          }
          else
          {
            Decimal num6 = Convert.ToDecimal(dataTable9.Rows[0]["IMPCONTRIBUTO"]) + Decimal.Round((Decimal) (Convert.ToInt32(dataTable9.Rows[0]["IMPCONTRIBUTO"]) * 4 / 100), 2) + Convert.ToDecimal(dataTable12.Rows[0]["IMPASSCON"]) + Convert.ToDecimal(dataTable12.Rows[0]["IMPABB"]);
            if (num6 == 0M)
              throw new Exception("Attenzione... l'importo è già stato stornato per intero. Impossibile continuare");
            string TIPO_OPERAZIONE3 = "ANNULLAMENTO DIPA";
            int int32 = Convert.ToInt32(CODPOS);
            str5 = clsWriteDb.WRITE_INSERT_MOVIMSAP(objDataAccess, u, ref ErrorMSG, int32, ANNDEN, MESDEN, PRODEN, CODCAU1, str3, Convert.ToInt32(ANNO_BILANCIO), str3, num6 * -1M, Convert.ToDecimal(dataTable12.Rows[0]["IMPABB"]), Decimal.Round((Decimal) (Convert.ToInt32(dataTable9.Rows[0]["IMPCONTRIBUTO"]) * 4 / 100), 2), Convert.ToDecimal(dataTable12.Rows[0]["IMPASSCON"]), "N", "S", TIPANN, ref PARTITA_MOVIMENTO, ref PROGMOV_MOVIMENTO, ref PARTITA_SANZIONE, ref PROGMOV_SANZIONE, TIPO_OPERAZIONE: TIPO_OPERAZIONE3, NUMMOV_ORIGINE: NUMERO_MOVIMENTO_ORIGINE.Trim());
          }
          string strSQL5 = "UPDATE DENDET SET FLGAPP = 'D', " + " DATMOVANN = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + ", " + " NUMMOVANN = " + DBMethods.DoublePeakForSql(str5) + ", " + " BILMOVANN = " + ANNO_BILANCIO + ", " + " PARTITAANN = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + ", " + " PROGMOVANN = " + PROGMOV_MOVIMENTO.ToString() + ", " + " UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " ULTAGG = CURRENT_TIMESTAMP" + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND NUMMOV = " + DBMethods.DoublePeakForSql(NUMERO_MOVIMENTO_ORIGINE) + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL";
          objDataAccess.WriteTransactionData(strSQL5, CommandType.Text);
          string str6 = "UPDATE DENTES SET FLGAPP = 'D', " + " DATMOVANN = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + ", " + " CODCAUANN = " + DBMethods.DoublePeakForSql(CODCAU1.Trim().PadLeft(2, '0')) + ", " + " NUMMOVANN = " + DBMethods.DoublePeakForSql(str5) + ", " + " BILMOVANN = " + ANNO_BILANCIO + ", ";
          if (TIPMOV != "ANN_AR")
            str6 = str6 + " TIPANNMOVANN = " + DBMethods.DoublePeakForSql(TIPANN) + ", ";
          string strSQL6 = str6 + " UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " ULTAGG = CURRENT_TIMESTAMP" + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND NUMMOV = " + DBMethods.DoublePeakForSql(NUMERO_MOVIMENTO_ORIGINE) + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL";
          objDataAccess.WriteTransactionData(strSQL6, CommandType.Text);
        }
        else
        {
          NUMERO_MOVIMENTO_ORIGINE = "";
          string strSQL7 = " SELECT SUM(IMPORTO) AS IMPCONTRIBUTO FROM " + " (SELECT CASE TIPMOV " + " WHEN 'RT' THEN IMPCONDEL " + " ELSE IMPCON END AS IMPORTO FROM DENDET WHERE " + " CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL ) AS TABELLA";
          DataTable dataTable13 = objDataAccess.GetDataTable(strSQL7);
          string strSQL8 = " SELECT IMPABB, IMPASSCON " + " FROM DENTES WHERE " + " CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL";
          DataTable dataTable14 = objDataAccess.GetDataTable(strSQL8);
          Decimal num7 = !DBNull.Value.Equals(dataTable13.Rows[0]["IMPCONTRIBUTO"]) ? Convert.ToDecimal(dataTable13.Rows[0]["IMPCONTRIBUTO"]) : 0M;
          Decimal num8 = !DBNull.Value.Equals(dataTable14.Rows[0]["IMPASSCON"]) ? Convert.ToDecimal(dataTable14.Rows[0]["IMPASSCON"]) : 0M;
          Decimal num9 = !DBNull.Value.Equals(dataTable14.Rows[0]["IMPABB"]) ? Convert.ToDecimal(dataTable14.Rows[0]["IMPABB"]) : 0M;
          Decimal num10 = num7 + Decimal.Round(num7 * 4M / 100M, 2) + num8 + num9;
          if (num10 == 0M)
            throw new Exception("Attenzione... l'importo è già stato stornato per intero. Impossibile continuare");
          str5 = clsWriteDb.WRITE_INSERT_MOVIMSAP(objDataAccess, u, ref ErrorMSG, CODPOS, ANNDEN, MESDEN, PRODEN, CODCAU1, str3, Convert.ToInt32(ANNO_BILANCIO), str3, num10 * -1M, Convert.ToDecimal(dataTable14.Rows[0]["IMPABB"]), Decimal.Round(Convert.ToDecimal(dataTable13.Rows[0]["IMPCONTRIBUTO"]) * 4M / 100M, 2), Convert.ToDecimal(dataTable14.Rows[0]["IMPASSCON"]), "N", "S", TIPANN, ref PARTITA_MOVIMENTO, ref PROGMOV_MOVIMENTO, ref PARTITA_SANZIONE, ref PROGMOV_SANZIONE, TIPO_OPERAZIONE: TIPO_OPERAZIONE1);
          string strSQL9 = "UPDATE DENDET SET FLGAPP = 'D', " + " DATMOVANN = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + ", " + " NUMMOVANN = " + DBMethods.DoublePeakForSql(str5) + ", " + " BILMOVANN = " + ANNO_BILANCIO + ", " + " PARTITAANN = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + ", " + " PROGMOVANN = " + PROGMOV_MOVIMENTO.ToString() + ", " + " UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " ULTAGG = CURRENT_TIMESTAMP" + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString();
          objDataAccess.WriteTransactionData(strSQL9, CommandType.Text);
          string str7 = "UPDATE DENTES SET FLGAPP = 'D', " + " DATMOVANN = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + ", " + " CODCAUANN = " + DBMethods.DoublePeakForSql(CODCAU1.Trim().PadLeft(2, '0')) + ", " + " NUMMOVANN = " + DBMethods.DoublePeakForSql(str5) + ", " + " BILMOVANN = " + ANNO_BILANCIO + ", ";
          if (TIPMOV != "ANN_AR")
            str7 = str7 + " TIPANNMOVANN = " + DBMethods.DoublePeakForSql(TIPANN) + ", ";
          string strSQL10 = str7 + " UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " ULTAGG = CURRENT_TIMESTAMP" + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString();
          objDataAccess.WriteTransactionData(strSQL10, CommandType.Text);
        }
        if (dataTable8.Rows.Count > 1)
        {
          string strSQL11 = " SELECT ESIRET FROM DENDET " + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND NUMMOVANN = " + DBMethods.DoublePeakForSql(str5);
          DataTable dataTable15 = objDataAccess.GetDataTable(strSQL11);
          for (int index = 0; index <= dataTable15.Rows.Count - 1; ++index)
          {
            if (dataTable15.Rows[index]["ESIRET"].ToString().Trim() == "S")
            {
              flag = true;
              break;
            }
          }
        }
        else
        {
          string strSQL12 = " SELECT ESIRET FROM DENDET " + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND NUMMOVANN = " + DBMethods.DoublePeakForSql(str5);
          DataTable dataTable16 = objDataAccess.GetDataTable(strSQL12);
          for (int index = 0; index <= dataTable16.Rows.Count - 1; ++index)
          {
            if (dataTable16.Rows[index]["ESIRET"].ToString().Trim() == "S")
            {
              flag = true;
              break;
            }
          }
        }
        if (!flag)
        {
          string strSQL13 = " SELECT NUMMOV, CODPOS, CODCAUMOV FROM DENTES WHERE NUMMOVANN = " + DBMethods.DoublePeakForSql(str5) + " AND CODPOS = " + CODPOS.ToString();
          DataTable dataTable17 = objDataAccess.GetDataTable(strSQL13);
          if (dataTable17.Rows.Count >= 1)
          {
            string str8 = !DBNull.Value.Equals(dataTable17.Rows[0]["NUMMOV"]) ? dataTable17.Rows[0]["NUMMOV"].ToString() : string.Empty;
            string str9 = !DBNull.Value.Equals(dataTable17.Rows[0][nameof (CODPOS)]) ? dataTable17.Rows[0][nameof (CODPOS)].ToString() : string.Empty;
            string str10 = !DBNull.Value.Equals(dataTable17.Rows[0]["CODCAUMOV"]) ? dataTable17.Rows[0]["CODCAUMOV"].ToString() : string.Empty;
            strSQL13 = " SELECT * FROM MOVIMSAP " + " WHERE  STATOSED <> 'A' ";
            if (!string.IsNullOrEmpty(str8))
              strSQL13 = strSQL13 + " AND NUMERORIF =" + str8.Substring(8) + " AND ANNORIF = " + str8.Substring(3, 4);
            if (!string.IsNullOrEmpty(str9))
              strSQL13 = strSQL13 + " AND CODPOSIZ = " + str9;
            if (!string.IsNullOrEmpty(str10))
              strSQL13 = strSQL13 + " AND CODCAUS = " + DBMethods.DoublePeakForSql(str10.Trim());
          }
          dataTable17.Clear();
          DataTable dataTable18 = objDataAccess.GetDataTable(strSQL13);
          for (int index1 = 0; index1 <= dataTable18.Rows.Count - 1; ++index1)
          {
            if (index1 == 0)
            {
              string strSQL14 = "SELECT A.PARTITAD, A.PROGMOVD, A.PARTITAA, A.PROGMOVA, A.PROGABBIN, A.IMPORTO, B.CODCAUS " + " FROM ABBINSAP A, MOVIMSAP B" + " WHERE A.PARTITAA = B.PARTITA" + " AND A.PROGMOVA = B.PROGMOV" + " AND B.CODCAUS IN ('09', '03', '71')" + " AND A.PARTITAD = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND A.PROGMOVD = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND A.STATOVAL = 'V' " + " ORDER BY INT(CODCAUS) DESC";
              dataTable2.Clear();
              dataTable2 = objDataAccess.GetDataTable(strSQL14);
              for (int index2 = 0; index2 <= dataTable2.Rows.Count - 1; ++index2)
              {
                string strSQL15 = "SELECT COUNT(*) FROM MOVASSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAA"].ToString()) + " AND PROGMOV = " + dataTable2.Rows[index2]["PROGMOVA"]?.ToString() + " AND PARTASS = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAD"].ToString()) + " AND PROGRASS = " + dataTable2.Rows[index2]["PROGMOVD"]?.ToString();
                if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL15, CommandType.Text)) == 0)
                {
                  string strSQL16 = "SELECT COUNT(*) FROM MOVIMSAP WHERE" + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAD"].ToString()) + " AND PROGMOV = " + dataTable2.Rows[index2]["PROGMOVD"]?.ToString() + " AND CODCAUS IN ('36','39')";
                  if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL16, CommandType.Text)) == 0)
                  {
                    string strSQL17 = "UPDATE MOVIMSAP SET " + " IMPABB = (IMPABB + " + dataTable2.Rows[index2]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRESID = (IMPRESID - " + dataTable2.Rows[index2]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAA"].ToString()) + " AND PROGMOV = " + dataTable2.Rows[index2]["PROGMOVA"]?.ToString();
                    objDataAccess.WriteTransactionData(strSQL17, CommandType.Text);
                    string strSQL18 = "UPDATE MOVIMSAP SET " + " IMPABB = (IMPABB - " + dataTable2.Rows[index2]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRESID = (IMPRESID + " + dataTable2.Rows[index2]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString();
                    objDataAccess.WriteTransactionData(strSQL18, CommandType.Text);
                    string strSQL19 = " SELECT * FROM SPLABSAP " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAD"].ToString()) + " AND PROGMOVD = " + dataTable2.Rows[index2]["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAA"].ToString()) + " AND PROGMOVA = " + dataTable2.Rows[index2]["PROGMOVA"]?.ToString() + " AND PROGABBIN = " + dataTable2.Rows[index2]["PROGABBIN"]?.ToString();
                    dataTable6.Clear();
                    dataTable6 = objDataAccess.GetDataTable(strSQL19);
                    for (int index3 = 0; index3 <= dataTable6.Rows.Count - 1; ++index3)
                    {
                      string strSQL20 = "UPDATE SPLIMPOSAP SET " + " IMPABB = (IMPABB + " + dataTable6.Rows[index3]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRES = (IMPRES - " + dataTable6.Rows[index3]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAA"].ToString()) + " AND PROGMOV = " + dataTable2.Rows[index2]["PROGMOVA"]?.ToString() + " AND PROGSPLI = " + dataTable6.Rows[index3]["PROGSPLIA"]?.ToString();
                      objDataAccess.WriteTransactionData(strSQL20, CommandType.Text);
                      string strSQL21 = "UPDATE SPLIMPOSAP SET " + " IMPABB = (IMPABB - " + dataTable6.Rows[index3]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRES = (IMPRES + " + dataTable6.Rows[index3]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND PROGSPLI = " + dataTable6.Rows[index3]["PROGSPLID"]?.ToString();
                      objDataAccess.WriteTransactionData(strSQL21, CommandType.Text);
                    }
                    string strSQL22 = "DELETE FROM ABBINSAP " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAD"].ToString()) + " AND PROGMOVD = " + dataTable2.Rows[index2]["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAA"].ToString()) + " AND PROGMOVA = " + dataTable2.Rows[index2]["PROGMOVA"]?.ToString() + " AND PROGABBIN = " + dataTable2.Rows[index2]["PROGABBIN"]?.ToString();
                    objDataAccess.WriteTransactionData(strSQL22, CommandType.Text);
                    string strSQL23 = "DELETE FROM SPLABSAP " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAD"].ToString()) + " AND PROGMOVD = " + dataTable2.Rows[index2]["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index2]["PARTITAA"].ToString()) + " AND PROGMOVA = " + dataTable2.Rows[index2]["PROGMOVA"]?.ToString() + " AND PROGABBIN = " + dataTable2.Rows[index2]["PROGABBIN"]?.ToString();
                    objDataAccess.WriteTransactionData(strSQL23, CommandType.Text);
                  }
                }
              }
            }
            string strSQL24 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString();
            dataTable2.Clear();
            dataTable2 = objDataAccess.GetDataTable(strSQL24);
            for (int index4 = 0; index4 <= dataTable2.Rows.Count - 1; ++index4)
            {
              switch (dataTable2.Rows[index4]["GESTCON"].ToString().Trim())
              {
                case "AB":
                case "PAB":
                case "RAB":
                  if (IMPORTO6 > 0M)
                  {
                    IMPORTO6 = IMPORTO5 + Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
                    break;
                  }
                  IMPORTO6 = Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
                  if (IMPORTO6 < 0M)
                  {
                    IMPORTO6 *= -1M;
                    break;
                  }
                  break;
                case "AC":
                case "PAC":
                case "RAC":
                  if (IMPORTO5 > 0M)
                  {
                    IMPORTO5 += Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
                    break;
                  }
                  IMPORTO5 = Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
                  if (IMPORTO5 < 0M)
                  {
                    IMPORTO5 *= -1M;
                    break;
                  }
                  break;
                case "AD":
                case "PAD":
                case "RAD":
                  if (IMPORTO1 > 0M)
                  {
                    IMPORTO1 += Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
                    break;
                  }
                  IMPORTO1 = Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
                  if (IMPORTO1 < 0M)
                  {
                    IMPORTO1 *= -1M;
                    break;
                  }
                  break;
                case "CAZA":
                  num4 = Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
                  if (num4 < 0M)
                  {
                    num4 *= -1M;
                    break;
                  }
                  break;
                case "FP":
                case "PFP":
                case "RFP":
                  if (IMPORTO4 > 0M)
                  {
                    IMPORTO4 += Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
                    break;
                  }
                  IMPORTO4 = Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
                  if (IMPORTO4 < 0M)
                  {
                    IMPORTO4 *= -1M;
                    break;
                  }
                  break;
                case "INF":
                case "PINF":
                case "RINF":
                  if (IMPORTO3 > 0M)
                  {
                    IMPORTO3 += Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
                    break;
                  }
                  IMPORTO3 = Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
                  if (IMPORTO3 < 0M)
                  {
                    IMPORTO3 *= -1M;
                    break;
                  }
                  break;
                case "PTFR":
                case "RTFR":
                case "TFR":
                  if (IMPORTO2 > 0M)
                  {
                    IMPORTO2 += Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
                    break;
                  }
                  IMPORTO2 = Convert.ToDecimal(dataTable2.Rows[index4]["IMPORTO"]);
                  if (IMPORTO2 < 0M)
                  {
                    IMPORTO2 *= -1M;
                    break;
                  }
                  break;
              }
            }
            int int32 = Convert.ToInt32(PROGMOV_MOVIMENTO);
            string str11 = Convert.ToString(dataTable18.Rows[index1]["PARTITA"]);
            int num11 = !DBNull.Value.Equals(dataTable18.Rows[index1]["PROGMOV"]) ? Convert.ToInt32(dataTable18.Rows[index1]["PROGMOV"]) : 0;
            string str12 = !DBNull.Value.Equals(dataTable18.Rows[index1]["CODCAUS"]) ? dataTable18.Rows[index1]["CODCAUS"].ToString().Trim() : string.Empty;
            clsWriteDb.WRITE_INSERT_MOVRETSAP(objDataAccess, u, ref PARTITA_MOVIMENTO, ref int32, ref str11, ref num11, ref str12, num4 * -1M, IMPORTO2 * -1M, IMPORTO4 * -1M, IMPORTO3 * -1M, 0M, IMPORTO1 * -1M, IMPORTO5 * -1M, IMPORTO6 * -1M, 0M);
            clsWriteDb.WRITE_INSERT_MOVASSAP(objDataAccess, u, ref PARTITA_MOVIMENTO, ref int32, ref str11, ref num11, ref str12, num4);
            int PROGABBIN = clsWriteDb.WRITE_INSERT_ABBINSAP(objDataAccess, u, ref str11, ref num11, ref PARTITA_MOVIMENTO, ref int32, num4);
            if (IMPORTO2 != 0M)
            {
              if (NUMERO_MOVIMENTO_ORIGINE != "")
              {
                string str13 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'TFR' ";
                string strSQL25 = !(TIPANN == "AC") ? str13 + " AND ANNPRE = 'S' " : str13 + " AND ANNPRE = 'N' ";
                string str14 = objDataAccess.Get1ValueFromSQL(strSQL25, CommandType.Text);
                string strSQL26 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON1 IN('TFR', 'TFRO')" + " ORDER BY IMPORTO DESC";
                dataTable2.Clear();
                dataTable2 = objDataAccess.GetDataTable(strSQL26);
                for (int index5 = 0; index5 <= dataTable2.Rows.Count - 1; ++index5)
                {
                  string strSQL27 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str14.Trim());
                  string str15 = objDataAccess.Get1ValueFromSQL(strSQL27, CommandType.Text);
                  int PROGSPLIA = str15 != string.Empty ? Convert.ToInt32(str15) : 0;
                  if (PROGSPLIA == 0)
                  {
                    string strSQL28 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'TFR' ";
                    string str16 = objDataAccess.Get1ValueFromSQL(strSQL28, CommandType.Text);
                    PROGSPLIA = str16 != string.Empty ? Convert.ToInt32(str16) : 0;
                  }
                  if (PROGSPLIA == 0)
                  {
                    string strSQL29 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RTFR' ";
                    string str17 = objDataAccess.Get1ValueFromSQL(strSQL29, CommandType.Text);
                    PROGSPLIA = str17 != string.Empty ? Convert.ToInt32(str17) : 0;
                  }
                  string PARTITA_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PARTITA"]) ? dataTable18.Rows[index1]["PARTITA"].ToString().Trim() : string.Empty;
                  int PROGMOV_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PROGMOV"]) ? Convert.ToInt32(dataTable18.Rows[index1]["PROGMOV"]) : 0;
                  clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, Convert.ToInt32(dataTable2.Rows[index5]["PROGSPLI"]), PROGSPLIA, dataTable2.Rows[index5]["GESTCON1"].ToString().Trim(), Convert.ToDecimal(dataTable2.Rows[index5]["IMPRES"]));
                  num3 = 0;
                }
                str1 = "";
              }
              else
              {
                string str18 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'TFR' ";
                string strSQL30 = !(TIPANN == "AC") ? str18 + " AND ANNPRE = 'S' " : str18 + " AND ANNPRE = 'N' ";
                string GESCON = objDataAccess.Get1ValueFromSQL(strSQL30, CommandType.Text);
                string strSQL31 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str19 = objDataAccess.Get1ValueFromSQL(strSQL31, CommandType.Text);
                int PROGSPLIA = str19 != string.Empty ? Convert.ToInt32(str19) : 0;
                if (PROGSPLIA == 0)
                {
                  string strSQL32 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'TFR' ";
                  string str20 = objDataAccess.Get1ValueFromSQL(strSQL32, CommandType.Text);
                  PROGSPLIA = str20 != string.Empty ? Convert.ToInt32(str20) : 0;
                }
                if (PROGSPLIA == 0)
                {
                  string strSQL33 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RTFR' ";
                  string str21 = objDataAccess.Get1ValueFromSQL(strSQL33, CommandType.Text);
                  PROGSPLIA = str21 != string.Empty ? Convert.ToInt32(str21) : 0;
                }
                string strSQL34 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str22 = objDataAccess.Get1ValueFromSQL(strSQL34, CommandType.Text);
                int PROGSPLID = str22 != string.Empty ? Convert.ToInt32(str22) : 0;
                if (PROGSPLID == 0)
                {
                  string strSQL35 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'TFR' ";
                  string str23 = objDataAccess.Get1ValueFromSQL(strSQL35, CommandType.Text);
                  PROGSPLID = str23 != string.Empty ? Convert.ToInt32(str23) : 0;
                }
                if (PROGSPLID == 0)
                {
                  string strSQL36 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'PTFR' ";
                  string str24 = objDataAccess.Get1ValueFromSQL(strSQL36, CommandType.Text);
                  PROGSPLID = str24 != string.Empty ? Convert.ToInt32(str24) : 0;
                }
                string PARTITA_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PARTITA"]) ? dataTable18.Rows[index1]["PARTITA"].ToString().Trim() : string.Empty;
                int PROGMOV_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PROGMOV"]) ? Convert.ToInt32(dataTable18.Rows[index1]["PROGMOV"]) : 0;
                clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, IMPORTO2);
                str1 = "";
                num2 = 0;
                num3 = 0;
              }
            }
            if (IMPORTO4 != 0M)
            {
              if (NUMERO_MOVIMENTO_ORIGINE != "")
              {
                string str25 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'PREV' ";
                string strSQL37 = !(TIPANN == "AC") ? str25 + " AND ANNPRE = 'S' " : str25 + " AND ANNPRE = 'N' ";
                string str26 = objDataAccess.Get1ValueFromSQL(strSQL37, CommandType.Text);
                string strSQL38 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON1 IN('FP', 'FPO')" + " ORDER BY IMPORTO DESC";
                dataTable2.Clear();
                dataTable2 = objDataAccess.GetDataTable(strSQL38);
                for (int index6 = 0; index6 <= dataTable2.Rows.Count - 1; ++index6)
                {
                  string strSQL39 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str26.Trim());
                  string str27 = objDataAccess.Get1ValueFromSQL(strSQL39, CommandType.Text);
                  int PROGSPLIA = str27 != string.Empty ? Convert.ToInt32(str27) : 0;
                  if (PROGSPLIA == 0)
                  {
                    string strSQL40 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'FP' ";
                    string str28 = objDataAccess.Get1ValueFromSQL(strSQL40, CommandType.Text);
                    PROGSPLIA = str28 != string.Empty ? Convert.ToInt32(str28) : 0;
                  }
                  if (PROGSPLIA == 0)
                  {
                    string strSQL41 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RFP' ";
                    string str29 = objDataAccess.Get1ValueFromSQL(strSQL41, CommandType.Text);
                    PROGSPLIA = str29 != string.Empty ? Convert.ToInt32(str29) : 0;
                  }
                  string PARTITA_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PARTITA"]) ? dataTable18.Rows[index1]["PARTITA"].ToString().Trim() : string.Empty;
                  int PROGMOV_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PROGMOV"]) ? Convert.ToInt32(dataTable18.Rows[index1]["PROGMOV"]) : 0;
                  clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, Convert.ToInt32(dataTable2.Rows[index6]["PROGSPLI"]), PROGSPLIA, dataTable2.Rows[index6]["GESTCON1"].ToString().Trim(), Convert.ToDecimal(dataTable2.Rows[index6]["IMPRES"]));
                  num3 = 0;
                }
                str1 = "";
              }
              else
              {
                string str30 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'PREV' ";
                string strSQL42 = !(TIPANN == "AC") ? str30 + " AND ANNPRE = 'S' " : str30 + " AND ANNPRE = 'N' ";
                string GESCON = objDataAccess.Get1ValueFromSQL(strSQL42, CommandType.Text).Trim();
                string strSQL43 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str31 = objDataAccess.Get1ValueFromSQL(strSQL43, CommandType.Text);
                int PROGSPLIA = str31 != string.Empty ? Convert.ToInt32(str31) : 0;
                if (PROGSPLIA == 0)
                {
                  string strSQL44 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'FP' ";
                  string str32 = objDataAccess.Get1ValueFromSQL(strSQL44, CommandType.Text);
                  PROGSPLIA = str32 != string.Empty ? Convert.ToInt32(str32) : 0;
                }
                if (PROGSPLIA == 0)
                {
                  string strSQL45 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RFP' ";
                  string str33 = objDataAccess.Get1ValueFromSQL(strSQL45, CommandType.Text);
                  PROGSPLIA = str33 != string.Empty ? Convert.ToInt32(str33) : 0;
                }
                string strSQL46 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str34 = objDataAccess.Get1ValueFromSQL(strSQL46, CommandType.Text);
                int PROGSPLID = str34 != string.Empty ? Convert.ToInt32(str34) : 0;
                if (PROGSPLID == 0)
                {
                  string strSQL47 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'FP' ";
                  string str35 = objDataAccess.Get1ValueFromSQL(strSQL47, CommandType.Text);
                  PROGSPLID = str35 != string.Empty ? Convert.ToInt32(str35) : 0;
                }
                if (PROGSPLID == 0)
                {
                  string strSQL48 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'PFP' ";
                  string str36 = objDataAccess.Get1ValueFromSQL(strSQL48, CommandType.Text);
                  PROGSPLID = str36 != string.Empty ? Convert.ToInt32(str36) : 0;
                }
                string PARTITA_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PARTITA"]) ? dataTable18.Rows[index1]["PARTITA"].ToString().Trim() : string.Empty;
                int PROGMOV_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PROGMOV"]) ? Convert.ToInt32(dataTable18.Rows[index1]["PROGMOV"]) : 0;
                clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, IMPORTO4);
                str1 = "";
                num2 = 0;
                num3 = 0;
              }
            }
            if (IMPORTO3 != 0M)
            {
              if (NUMERO_MOVIMENTO_ORIGINE != "")
              {
                string str37 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'INF' ";
                string strSQL49 = !(TIPANN == "AC") ? str37 + " AND ANNPRE = 'S' " : str37 + " AND ANNPRE = 'N' ";
                string str38 = objDataAccess.Get1ValueFromSQL(strSQL49, CommandType.Text);
                string strSQL50 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON1 IN('INF', 'INFO')" + " ORDER BY IMPORTO DESC";
                dataTable2.Clear();
                dataTable2 = objDataAccess.GetDataTable(strSQL50);
                for (int index7 = 0; index7 <= dataTable2.Rows.Count - 1; ++index7)
                {
                  string strSQL51 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str38.Trim());
                  string str39 = objDataAccess.Get1ValueFromSQL(strSQL51, CommandType.Text);
                  int PROGSPLIA = str39 != string.Empty ? Convert.ToInt32(str39) : 0;
                  if (PROGSPLIA == 0)
                  {
                    string strSQL52 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'INF' ";
                    string str40 = objDataAccess.Get1ValueFromSQL(strSQL52, CommandType.Text);
                    PROGSPLIA = str40 != string.Empty ? Convert.ToInt32(str40) : 0;
                  }
                  if (PROGSPLIA == 0)
                  {
                    string strSQL53 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RINF' ";
                    string str41 = objDataAccess.Get1ValueFromSQL(strSQL53, CommandType.Text);
                    PROGSPLIA = str41 != string.Empty ? Convert.ToInt32(str41) : 0;
                  }
                  string PARTITA_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PARTITA"]) ? dataTable18.Rows[index1]["PARTITA"].ToString().Trim() : string.Empty;
                  int PROGMOV_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PROGMOV"]) ? Convert.ToInt32(dataTable18.Rows[index1]["PROGMOV"]) : 0;
                  clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, Convert.ToInt32(dataTable2.Rows[index7]["PROGSPLI"]), PROGSPLIA, dataTable2.Rows[index7]["GESTCON1"].ToString().Trim(), Convert.ToDecimal(dataTable2.Rows[index7]["IMPRES"]));
                  num3 = 0;
                }
                str1 = "";
              }
              else
              {
                string str42 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'INF' ";
                string strSQL54 = !(TIPANN == "AC") ? str42 + " AND ANNPRE = 'S' " : str42 + " AND ANNPRE = 'N' ";
                string GESCON = objDataAccess.Get1ValueFromSQL(strSQL54, CommandType.Text);
                string strSQL55 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str43 = objDataAccess.Get1ValueFromSQL(strSQL55, CommandType.Text);
                int PROGSPLIA = str43 != string.Empty ? Convert.ToInt32(str43) : 0;
                if (PROGSPLIA == 0)
                {
                  string strSQL56 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'INF' ";
                  string str44 = objDataAccess.Get1ValueFromSQL(strSQL56, CommandType.Text);
                  PROGSPLIA = str44 != string.Empty ? Convert.ToInt32(str44) : 0;
                }
                if (PROGSPLIA == 0)
                {
                  string strSQL57 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RINF' ";
                  string str45 = objDataAccess.Get1ValueFromSQL(strSQL57, CommandType.Text);
                  PROGSPLIA = str45 != string.Empty ? Convert.ToInt32(str45) : 0;
                }
                string strSQL58 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str46 = objDataAccess.Get1ValueFromSQL(strSQL58, CommandType.Text);
                int PROGSPLID = str46 != string.Empty ? Convert.ToInt32(str46) : 0;
                if (PROGSPLID == 0)
                {
                  string strSQL59 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'INF' ";
                  string str47 = objDataAccess.Get1ValueFromSQL(strSQL59, CommandType.Text);
                  PROGSPLID = str47 != string.Empty ? Convert.ToInt32(str47) : 0;
                }
                if (PROGSPLID == 0)
                {
                  string strSQL60 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'PINF' ";
                  string str48 = objDataAccess.Get1ValueFromSQL(strSQL60, CommandType.Text);
                  PROGSPLID = str48 != string.Empty ? Convert.ToInt32(str48) : 0;
                }
                string PARTITA_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PARTITA"]) ? dataTable18.Rows[index1]["PARTITA"].ToString().Trim() : string.Empty;
                int PROGMOV_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PROGMOV"]) ? Convert.ToInt32(dataTable18.Rows[index1]["PROGMOV"]) : 0;
                clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, IMPORTO3);
                str1 = "";
                num2 = 0;
                num3 = 0;
              }
            }
            if (IMPORTO1 != 0M)
            {
              if (NUMERO_MOVIMENTO_ORIGINE != "")
              {
                string str49 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ADDIZIONALE' ";
                string strSQL61 = !(TIPANN == "AC") ? str49 + " AND ANNPRE = 'S' " : str49 + " AND ANNPRE = 'N' ";
                string str50 = objDataAccess.Get1ValueFromSQL(strSQL61, CommandType.Text);
                string strSQL62 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON1 IN('AD', 'ADO')" + " ORDER BY IMPORTO DESC";
                dataTable2.Clear();
                dataTable2 = objDataAccess.GetDataTable(strSQL62);
                for (int index8 = 0; index8 <= dataTable2.Rows.Count - 1; ++index8)
                {
                  string strSQL63 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str50.Trim());
                  string str51 = objDataAccess.Get1ValueFromSQL(strSQL63, CommandType.Text);
                  int PROGSPLIA = str51 != string.Empty ? Convert.ToInt32(str51) : 0;
                  if (PROGSPLIA == 0)
                  {
                    string strSQL64 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'AD' ";
                    string str52 = objDataAccess.Get1ValueFromSQL(strSQL64, CommandType.Text);
                    PROGSPLIA = str52 != string.Empty ? Convert.ToInt32(str52) : 0;
                  }
                  if (PROGSPLIA == 0)
                  {
                    string strSQL65 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RAD' ";
                    string str53 = objDataAccess.Get1ValueFromSQL(strSQL65, CommandType.Text);
                    PROGSPLIA = str53 != string.Empty ? Convert.ToInt32(str53) : 0;
                  }
                  string PARTITA_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PARTITA"]) ? dataTable18.Rows[index1]["PARTITA"].ToString().Trim() : string.Empty;
                  int PROGMOV_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PROGMOV"]) ? Convert.ToInt32(dataTable18.Rows[index1]["PROGMOV"]) : 0;
                  clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, Convert.ToInt32(dataTable2.Rows[index8]["PROGSPLI"]), PROGSPLIA, dataTable2.Rows[index8]["GESTCON1"].ToString().Trim(), Convert.ToDecimal(dataTable2.Rows[index8]["IMPRES"]));
                  num3 = 0;
                }
                str1 = "";
              }
              else
              {
                string str54 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ADDIZIONALE' ";
                string strSQL66 = !(TIPANN == "AC") ? str54 + " AND ANNPRE = 'S' " : str54 + " AND ANNPRE = 'N' ";
                string GESCON = objDataAccess.Get1ValueFromSQL(strSQL66, CommandType.Text);
                string strSQL67 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str55 = objDataAccess.Get1ValueFromSQL(strSQL67, CommandType.Text);
                int PROGSPLIA = str55 != string.Empty ? Convert.ToInt32(str55) : 0;
                if (PROGSPLIA == 0)
                {
                  string strSQL68 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'AD' ";
                  string str56 = objDataAccess.Get1ValueFromSQL(strSQL68, CommandType.Text);
                  PROGSPLIA = str56 != string.Empty ? Convert.ToInt32(str56) : 0;
                }
                if (PROGSPLIA == 0)
                {
                  string strSQL69 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RAD' ";
                  string str57 = objDataAccess.Get1ValueFromSQL(strSQL69, CommandType.Text);
                  PROGSPLIA = str57 != string.Empty ? Convert.ToInt32(str57) : 0;
                }
                string strSQL70 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str58 = objDataAccess.Get1ValueFromSQL(strSQL70, CommandType.Text);
                int PROGSPLID = str58 != string.Empty ? Convert.ToInt32(str58) : 0;
                if (PROGSPLID == 0)
                {
                  string strSQL71 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'AD' ";
                  string str59 = objDataAccess.Get1ValueFromSQL(strSQL71, CommandType.Text);
                  PROGSPLID = str59 != string.Empty ? Convert.ToInt32(str59) : 0;
                }
                if (PROGSPLID == 0)
                {
                  string strSQL72 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'PAD' ";
                  string str60 = objDataAccess.Get1ValueFromSQL(strSQL72, CommandType.Text);
                  PROGSPLID = str60 != string.Empty ? Convert.ToInt32(str60) : 0;
                }
                string PARTITA_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PARTITA"]) ? dataTable18.Rows[index1]["PARTITA"].ToString().Trim() : string.Empty;
                int PROGMOV_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PROGMOV"]) ? Convert.ToInt32(dataTable18.Rows[index1]["PROGMOV"]) : 0;
                clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, IMPORTO1);
                str1 = "";
                num2 = 0;
                num3 = 0;
              }
            }
            if (IMPORTO5 != 0M)
            {
              if (NUMERO_MOVIMENTO_ORIGINE != "")
              {
                string str61 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ASSCON' ";
                string strSQL73 = !(TIPANN == "AC") ? str61 + " AND ANNPRE = 'S' " : str61 + " AND ANNPRE = 'N' ";
                string str62 = objDataAccess.Get1ValueFromSQL(strSQL73, CommandType.Text);
                string strSQL74 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON1 IN('AC', 'ACO')" + " ORDER BY IMPORTO DESC";
                dataTable2.Clear();
                dataTable2 = objDataAccess.GetDataTable(strSQL74);
                for (int index9 = 0; index9 <= dataTable2.Rows.Count - 1; ++index9)
                {
                  string strSQL75 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str62.Trim());
                  string str63 = objDataAccess.Get1ValueFromSQL(strSQL75, CommandType.Text);
                  int PROGSPLIA = str63 != string.Empty ? Convert.ToInt32(str63) : 0;
                  if (PROGSPLIA == 0)
                  {
                    string strSQL76 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'AC' ";
                    string str64 = objDataAccess.Get1ValueFromSQL(strSQL76, CommandType.Text);
                    PROGSPLIA = str64 != string.Empty ? Convert.ToInt32(str64) : 0;
                  }
                  if (PROGSPLIA == 0)
                  {
                    string strSQL77 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RAC' ";
                    string str65 = objDataAccess.Get1ValueFromSQL(strSQL77, CommandType.Text);
                    PROGSPLIA = str65 != string.Empty ? Convert.ToInt32(str65) : 0;
                  }
                  string PARTITA_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PARTITA"]) ? dataTable18.Rows[index1]["PARTITA"].ToString().Trim() : string.Empty;
                  int PROGMOV_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PROGMOV"]) ? Convert.ToInt32(dataTable18.Rows[index1]["PROGMOV"]) : 0;
                  clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, Convert.ToInt32(dataTable2.Rows[index9]["PROGSPLI"]), PROGSPLIA, dataTable2.Rows[index9]["GESTCON1"].ToString().Trim(), Convert.ToDecimal(dataTable2.Rows[index9]["IMPRES"]));
                  num3 = 0;
                }
                str1 = "";
              }
              else
              {
                string str66 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ASSCON' ";
                string strSQL78 = !(TIPANN == "AC") ? str66 + " AND ANNPRE = 'S' " : str66 + " AND ANNPRE = 'N' ";
                string GESCON = objDataAccess.Get1ValueFromSQL(strSQL78, CommandType.Text);
                string strSQL79 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str67 = objDataAccess.Get1ValueFromSQL(strSQL79, CommandType.Text);
                int PROGSPLIA = str67 != string.Empty ? Convert.ToInt32(str67) : 0;
                if (PROGSPLIA == 0)
                {
                  string strSQL80 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'AC' ";
                  string str68 = objDataAccess.Get1ValueFromSQL(strSQL80, CommandType.Text);
                  PROGSPLIA = str68 != string.Empty ? Convert.ToInt32(str68) : 0;
                }
                if (PROGSPLIA == 0)
                {
                  string strSQL81 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RAC' ";
                  string str69 = objDataAccess.Get1ValueFromSQL(strSQL81, CommandType.Text);
                  PROGSPLIA = str69 != string.Empty ? Convert.ToInt32(str69) : 0;
                }
                string strSQL82 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str70 = objDataAccess.Get1ValueFromSQL(strSQL82, CommandType.Text);
                int PROGSPLID = str70 != string.Empty ? Convert.ToInt32(str70) : 0;
                if (PROGSPLID == 0)
                {
                  string strSQL83 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'AC' ";
                  string str71 = objDataAccess.Get1ValueFromSQL(strSQL83, CommandType.Text);
                  PROGSPLID = str71 != string.Empty ? Convert.ToInt32(str71) : 0;
                }
                if (PROGSPLID == 0)
                {
                  string strSQL84 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'PAC' ";
                  string str72 = objDataAccess.Get1ValueFromSQL(strSQL84, CommandType.Text);
                  PROGSPLID = str72 != string.Empty ? Convert.ToInt32(str72) : 0;
                }
                string PARTITA_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PARTITA"]) ? dataTable18.Rows[index1]["PARTITA"].ToString().Trim() : string.Empty;
                int PROGMOV_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PROGMOV"]) ? Convert.ToInt32(dataTable18.Rows[index1]["PROGMOV"]) : 0;
                clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, IMPORTO5);
                str1 = "";
                num2 = 0;
                num3 = 0;
              }
            }
            if (IMPORTO6 != 0M)
            {
              if (NUMERO_MOVIMENTO_ORIGINE != "")
              {
                string str73 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ABBPRE' ";
                string strSQL85 = !(TIPANN == "AC") ? str73 + " AND ANNPRE = 'S' " : str73 + " AND ANNPRE = 'N' ";
                string str74 = objDataAccess.Get1ValueFromSQL(strSQL85, CommandType.Text);
                string strSQL86 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON1 IN('AB', 'ABO')" + " ORDER BY IMPORTO DESC";
                dataTable2.Clear();
                dataTable2 = objDataAccess.GetDataTable(strSQL86);
                for (int index10 = 0; index10 <= dataTable2.Rows.Count - 1; ++index10)
                {
                  string strSQL87 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str74.Trim());
                  string str75 = objDataAccess.Get1ValueFromSQL(strSQL87, CommandType.Text);
                  int PROGSPLIA = str75 != string.Empty ? Convert.ToInt32(str75) : 0;
                  if (PROGSPLIA == 0)
                  {
                    string strSQL88 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'AB' ";
                    string str76 = objDataAccess.Get1ValueFromSQL(strSQL88, CommandType.Text);
                    PROGSPLIA = str76 != string.Empty ? Convert.ToInt32(str76) : 0;
                  }
                  if (PROGSPLIA == 0)
                  {
                    string strSQL89 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RAB' ";
                    string str77 = objDataAccess.Get1ValueFromSQL(strSQL89, CommandType.Text);
                    PROGSPLIA = str77 != string.Empty ? Convert.ToInt32(str77) : 0;
                  }
                  string PARTITA_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PARTITA"]) ? dataTable18.Rows[index1]["PARTITA"].ToString().Trim() : string.Empty;
                  int PROGMOV_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PROGMOV"]) ? Convert.ToInt32(dataTable18.Rows[index1]["PROGMOV"]) : 0;
                  clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, Convert.ToInt32(dataTable2.Rows[index10]["PROGSPLI"]), PROGSPLIA, dataTable2.Rows[index10]["GESTCON1"].ToString().Trim(), Convert.ToDecimal(dataTable2.Rows[index10]["IMPRES"]));
                  num3 = 0;
                }
                str1 = "";
              }
              else
              {
                string str78 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ABBPRE' ";
                string strSQL90 = !(TIPANN == "AC") ? str78 + " AND ANNPRE = 'S' " : str78 + " AND ANNPRE = 'N' ";
                string GESCON = objDataAccess.Get1ValueFromSQL(strSQL90, CommandType.Text);
                string strSQL91 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str79 = objDataAccess.Get1ValueFromSQL(strSQL91, CommandType.Text);
                int PROGSPLIA = str79 != string.Empty ? Convert.ToInt32(str79) : 0;
                if (PROGSPLIA == 0)
                {
                  string strSQL92 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'AB' ";
                  string str80 = objDataAccess.Get1ValueFromSQL(strSQL92, CommandType.Text);
                  PROGSPLIA = str80 != string.Empty ? Convert.ToInt32(str80) : 0;
                }
                if (PROGSPLIA == 0)
                {
                  string strSQL93 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RAB' ";
                  string str81 = objDataAccess.Get1ValueFromSQL(strSQL93, CommandType.Text);
                  PROGSPLIA = str81 != string.Empty ? Convert.ToInt32(str81) : 0;
                }
                string strSQL94 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str82 = objDataAccess.Get1ValueFromSQL(strSQL94, CommandType.Text);
                int PROGSPLID = str82 != string.Empty ? Convert.ToInt32(str82) : 0;
                if (PROGSPLID == 0)
                {
                  string strSQL95 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'AB' ";
                  string str83 = objDataAccess.Get1ValueFromSQL(strSQL95, CommandType.Text);
                  PROGSPLID = str83 != string.Empty ? Convert.ToInt32(str83) : 0;
                }
                if (PROGSPLID == 0)
                {
                  string strSQL96 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON = 'PAB' ";
                  string str84 = objDataAccess.Get1ValueFromSQL(strSQL96, CommandType.Text);
                  PROGSPLID = str84 != string.Empty ? Convert.ToInt32(str84) : 0;
                }
                string PARTITA_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PARTITA"]) ? dataTable18.Rows[index1]["PARTITA"].ToString().Trim() : string.Empty;
                int PROGMOV_DA = !DBNull.Value.Equals(dataTable18.Rows[index1]["PROGMOV"]) ? Convert.ToInt32(dataTable18.Rows[index1]["PROGMOV"]) : 0;
                clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, IMPORTO6);
                str1 = "";
                num2 = 0;
                num3 = 0;
              }
            }
            string strSQL97 = "UPDATE MOVIMSAP SET " + " IMPRIDUZ =  " + num4.ToString().Replace(',', '.') + ", " + " IMPABB = " + num4.ToString().Replace(',', '.') + ", " + " IMPRESID = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString();
            objDataAccess.WriteTransactionData(strSQL97, CommandType.Text);
            string strSQL98 = "UPDATE SPLIMPOSAP SET " + " IMPRIDU = IMPORTO, " + " IMPABB = IMPORTO, " + " IMPRES = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable18.Rows[index1]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable18.Rows[index1]["PROGMOV"]?.ToString() + " AND GESTCON <> 'CAZA'";
            objDataAccess.WriteTransactionData(strSQL98, CommandType.Text);
            if (index1 == 0)
            {
              string strSQL99 = "UPDATE MOVIMSAP SET " + " IMPORTO = " + (num4 * -1M).ToString().Replace(',', '.') + ", " + " IMPABB = " + (num4 * -1M).ToString().Replace(',', '.') + ", " + " IMPRESID = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString();
              objDataAccess.WriteTransactionData(strSQL99, CommandType.Text);
              string strSQL100 = "UPDATE SPLIMPOSAP SET " + " IMPABB = IMPORTO, " + " IMPRES = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON <> 'CAZA'";
              objDataAccess.WriteTransactionData(strSQL100, CommandType.Text);
              string strSQL101 = "UPDATE SPLIMPOSAP SET " + " IMPORTO = " + (IMPORTO1 * -1M).ToString().Replace(',', '.') + ", " + " IMPABB = " + (IMPORTO1 * -1M).ToString().Replace(',', '.') + ", " + " IMPRES = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON IN ('AD','PAD','RAD')";
              objDataAccess.WriteTransactionData(strSQL101, CommandType.Text);
              string strSQL102 = "UPDATE SPLIMPOSAP SET " + " IMPORTO = " + (IMPORTO2 + IMPORTO4 + IMPORTO3 + IMPORTO1 + IMPORTO5 + IMPORTO6).ToString().Replace(',', '.') + ", " + " IMPABB = " + (IMPORTO2 + IMPORTO4 + IMPORTO3 + IMPORTO1 + IMPORTO5 + IMPORTO6).ToString().Replace(',', '.') + ", " + " IMPRES = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON IN ('CAZA')";
              objDataAccess.WriteTransactionData(strSQL102, CommandType.Text);
            }
            num4 = 0.0M;
            IMPORTO1 = 0.0M;
            IMPORTO2 = 0.0M;
            IMPORTO3 = 0.0M;
            IMPORTO4 = 0.0M;
            IMPORTO5 = 0.0M;
            IMPORTO6 = 0.0M;
            num1 = 0;
          }
        }
        else
        {
          string strSQL103 = " SELECT NUMMOV, CODPOS, CODCAUMOV FROM DENTES WHERE NUMMOVANN = " + DBMethods.DoublePeakForSql(str5) + " AND CODPOS = " + CODPOS.ToString();
          DataTable dataTable19 = objDataAccess.GetDataTable(strSQL103);
          string strSQL104 = " SELECT * FROM MOVIMSAP " + " WHERE ANNORIF = " + dataTable19.Rows[0]["NUMMOV"].ToString().Substring(3, 4) + " AND NUMERORIF =" + dataTable19.Rows[0]["NUMMOV"].ToString().Substring(8) + " AND CODPOSIZ = " + dataTable19.Rows[0][nameof (CODPOS)]?.ToString() + " AND CODCAUS = " + DBMethods.DoublePeakForSql(dataTable19.Rows[0]["CODCAUMOV"].ToString().Trim()) + " AND STATOSED <> 'A'";
          dataTable19.Clear();
          DataTable dataTable20 = objDataAccess.GetDataTable(strSQL104);
          for (int index11 = 0; index11 <= dataTable20.Rows.Count - 1; ++index11)
          {
            if (index11 == 0)
            {
              string strSQL105 = "SELECT A.PARTITAD, A.PROGMOVD, A.PARTITAA, A.PROGMOVA, A.PROGABBIN, A.IMPORTO, B.CODCAUS " + " FROM ABBINSAP A, MOVIMSAP B" + " WHERE A.PARTITAA = B.PARTITA" + " AND A.PROGMOVA = B.PROGMOV" + " AND B.CODCAUS IN ('09', '03', '71')" + " AND A.PARTITAD = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND A.PROGMOVD = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND A.STATOVAL = 'V' " + " ORDER BY INT(CODCAUS) DESC";
              dataTable2.Clear();
              dataTable2 = objDataAccess.GetDataTable(strSQL105);
              for (int index12 = 0; index12 <= dataTable2.Rows.Count - 1; ++index12)
              {
                string strSQL106 = "SELECT COUNT(*) FROM MOVASSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index12]["PARTITAA"].ToString()) + " AND PROGMOV = " + dataTable2.Rows[index12]["PROGMOVA"]?.ToString() + " AND PARTASS = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index12]["PARTITAD"].ToString()) + " AND PROGRASS = " + dataTable2.Rows[index12]["PROGMOVD"]?.ToString();
                if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL106, CommandType.Text)) == 0)
                {
                  string strSQL107 = "SELECT COUNT(*) FROM MOVIMSAP WHERE" + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index12]["PARTITAD"].ToString()) + " AND PROGMOV = " + dataTable2.Rows[index12]["PROGMOVD"]?.ToString() + " AND CODCAUS IN ('36','39')";
                  if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL107, CommandType.Text)) == 0)
                  {
                    string strSQL108 = "UPDATE MOVIMSAP SET " + " IMPABB = (IMPABB + " + dataTable2.Rows[index12]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRESID = (IMPRESID - " + dataTable2.Rows[index12]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index12]["PARTITAA"].ToString()) + " AND PROGMOV = " + dataTable2.Rows[index12]["PROGMOVA"]?.ToString();
                    objDataAccess.WriteTransactionData(strSQL108, CommandType.Text);
                    string strSQL109 = "UPDATE MOVIMSAP SET " + " IMPABB = (IMPABB - " + dataTable2.Rows[index12]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRESID = (IMPRESID + " + dataTable2.Rows[index12]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString();
                    objDataAccess.WriteTransactionData(strSQL109, CommandType.Text);
                    string strSQL110 = " SELECT * FROM SPLABSAP " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index12]["PARTITAD"].ToString()) + " AND PROGMOVD = " + dataTable2.Rows[index12]["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index12]["PARTITAA"].ToString()) + " AND PROGMOVA = " + dataTable2.Rows[index12]["PROGMOVA"]?.ToString() + " AND PROGABBIN = " + dataTable2.Rows[index12]["PROGABBIN"]?.ToString();
                    dataTable6.Clear();
                    dataTable6 = objDataAccess.GetDataTable(strSQL110);
                    for (int index13 = 0; index13 <= dataTable6.Rows.Count - 1; ++index13)
                    {
                      string strSQL111 = "UPDATE SPLIMPOSAP SET " + " IMPABB = (IMPABB + " + dataTable6.Rows[index13]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRES = (IMPRES - " + dataTable6.Rows[index13]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index12]["PARTITAA"].ToString()) + " AND PROGMOV = " + dataTable2.Rows[index12]["PROGMOVA"]?.ToString() + " AND PROGSPLI = " + dataTable6.Rows[index13]["PROGSPLIA"]?.ToString();
                      objDataAccess.WriteTransactionData(strSQL111, CommandType.Text);
                      string strSQL112 = "UPDATE SPLIMPOSAP SET " + " IMPABB = (IMPABB - " + dataTable6.Rows[index13]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRES = (IMPRES + " + dataTable6.Rows[index13]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND PROGSPLI = " + dataTable6.Rows[index13]["PROGSPLID"]?.ToString();
                      objDataAccess.WriteTransactionData(strSQL112, CommandType.Text);
                    }
                    string strSQL113 = "DELETE FROM ABBINSAP " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index12]["PARTITAD"].ToString()) + " AND PROGMOVD = " + dataTable2.Rows[index12]["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index12]["PARTITAA"].ToString()) + " AND PROGMOVA = " + dataTable2.Rows[index12]["PROGMOVA"]?.ToString() + " AND PROGABBIN = " + dataTable2.Rows[index12]["PROGABBIN"]?.ToString();
                    objDataAccess.WriteTransactionData(strSQL113, CommandType.Text);
                    string strSQL114 = "DELETE FROM SPLABSAP " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index12]["PARTITAD"].ToString()) + " AND PROGMOVD = " + dataTable2.Rows[index12]["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(dataTable2.Rows[index12]["PARTITAA"].ToString()) + " AND PROGMOVA = " + dataTable2.Rows[index12]["PROGMOVA"]?.ToString() + " AND PROGABBIN = " + dataTable2.Rows[index12]["PROGABBIN"]?.ToString();
                    objDataAccess.WriteTransactionData(strSQL114, CommandType.Text);
                  }
                }
              }
            }
            string strSQL115 = " SELECT * FROM SPLABSAP " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOVD = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString();
            dataTable6.Clear();
            DataTable dataTable21 = objDataAccess.GetDataTable(strSQL115);
            if (dataTable21.Rows.Count > 0)
            {
              string strSQL116 = "SELECT COUNT(*) FROM MOVASSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable21.Rows[0]["PARTITAA"].ToString()) + " AND PROGMOV = " + dataTable21.Rows[0]["PROGMOVA"]?.ToString() + " AND PARTASS = " + DBMethods.DoublePeakForSql(dataTable21.Rows[0]["PARTITAD"].ToString()) + " AND PROGRASS = " + dataTable21.Rows[0]["PROGMOVD"]?.ToString();
              if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL116, CommandType.Text)) == 0)
              {
                for (int index14 = 0; index14 <= dataTable21.Rows.Count - 1; ++index14)
                {
                  string strSQL117 = "UPDATE SPLIMPOSAP SET " + " IMPABB = (IMPABB + " + dataTable21.Rows[index14]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRES = (IMPRES - " + dataTable21.Rows[index14]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND PROGSPLI = " + dataTable21.Rows[index14]["PROGSPLID"]?.ToString();
                  objDataAccess.WriteTransactionData(strSQL117, CommandType.Text);
                }
              }
            }
            string strSQL118 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString();
            dataTable2.Clear();
            dataTable2 = objDataAccess.GetDataTable(strSQL118);
            for (int index15 = 0; index15 <= dataTable2.Rows.Count - 1; ++index15)
            {
              switch (dataTable2.Rows[index15]["GESTCON"].ToString().Trim())
              {
                case "AB":
                case "PAB":
                case "RAB":
                  if (IMPORTO6 > 0M)
                  {
                    IMPORTO6 = IMPORTO5 + Convert.ToDecimal(dataTable2.Rows[index15]["IMPORTO"]);
                    break;
                  }
                  IMPORTO6 = Convert.ToDecimal(dataTable2.Rows[index15]["IMPORTO"]);
                  if (IMPORTO6 < 0M)
                  {
                    IMPORTO6 *= -1M;
                    break;
                  }
                  break;
                case "AC":
                case "PAC":
                case "RAC":
                  if (IMPORTO5 > 0M)
                  {
                    IMPORTO5 += Convert.ToDecimal(dataTable2.Rows[index15]["IMPORTO"]);
                    break;
                  }
                  IMPORTO5 = Convert.ToDecimal(dataTable2.Rows[index15]["IMPORTO"]);
                  if (IMPORTO5 < 0M)
                  {
                    IMPORTO5 *= -1M;
                    break;
                  }
                  break;
                case "AD":
                case "PAD":
                case "RAD":
                  if (IMPORTO1 > 0M)
                  {
                    IMPORTO1 += Convert.ToDecimal(dataTable2.Rows[index15]["IMPORTO"]);
                    break;
                  }
                  IMPORTO1 = Convert.ToDecimal(dataTable2.Rows[index15]["IMPORTO"]);
                  if (IMPORTO1 < 0M)
                  {
                    IMPORTO1 *= -1M;
                    break;
                  }
                  break;
                case "CAZA":
                  num4 = Convert.ToDecimal(dataTable2.Rows[index15]["IMPORTO"]);
                  if (num4 < 0M)
                  {
                    num4 *= -1M;
                    break;
                  }
                  break;
                case "FP":
                case "PFP":
                case "RFP":
                  if (IMPORTO4 > 0M)
                  {
                    IMPORTO4 += Convert.ToDecimal(dataTable2.Rows[index15]["IMPORTO"]);
                    break;
                  }
                  IMPORTO4 = Convert.ToDecimal(dataTable2.Rows[index15]["IMPORTO"]);
                  if (IMPORTO4 < 0M)
                  {
                    IMPORTO4 *= -1M;
                    break;
                  }
                  break;
                case "INF":
                case "PINF":
                case "RINF":
                  if (IMPORTO3 > 0M)
                  {
                    IMPORTO3 += Convert.ToDecimal(dataTable2.Rows[index15]["IMPORTO"]);
                    break;
                  }
                  IMPORTO3 = Convert.ToDecimal(dataTable2.Rows[index15]["IMPORTO"]);
                  if (IMPORTO3 < 0M)
                  {
                    IMPORTO3 *= -1M;
                    break;
                  }
                  break;
                case "PTFR":
                case "RTFR":
                case "TFR":
                  if (IMPORTO2 > 0M)
                  {
                    IMPORTO2 += Convert.ToDecimal(dataTable2.Rows[index15]["IMPORTO"]);
                    break;
                  }
                  IMPORTO2 = Convert.ToDecimal(dataTable2.Rows[index15]["IMPORTO"]);
                  if (IMPORTO2 < 0M)
                  {
                    IMPORTO2 *= -1M;
                    break;
                  }
                  break;
              }
            }
            int int32 = Convert.ToInt32(PROGMOV_MOVIMENTO);
            string str85 = !DBNull.Value.Equals(dataTable20.Rows[index11]["PARTITA"]) ? dataTable20.Rows[index11]["PARTITA"].ToString().Trim() : string.Empty;
            int num12 = !DBNull.Value.Equals(dataTable20.Rows[index11]["PROGMOV"]) ? Convert.ToInt32(dataTable20.Rows[index11]["PROGMOV"]) : 0;
            string str86 = !DBNull.Value.Equals(dataTable20.Rows[index11]["CODCAUS"]) ? dataTable20.Rows[index11]["CODCAUS"].ToString().Trim() : string.Empty;
            clsWriteDb.WRITE_INSERT_MOVRETSAP(objDataAccess, u, ref PARTITA_MOVIMENTO, ref int32, ref str85, ref num12, ref str86, num4 * -1M, IMPORTO2 * -1M, IMPORTO4 * -1M, IMPORTO3 * -1M, 0M, IMPORTO1 * -1M, IMPORTO5 * -1M, IMPORTO6 * -1M, 0M);
            clsWriteDb.WRITE_INSERT_MOVASSAP(objDataAccess, u, ref PARTITA_MOVIMENTO, ref int32, ref str85, ref num12, ref str86, num4);
            int PROGABBIN = clsWriteDb.WRITE_INSERT_ABBINSAP(objDataAccess, u, ref str85, ref num12, ref PARTITA_MOVIMENTO, ref int32, num4);
            if (IMPORTO2 != 0M)
            {
              if (NUMERO_MOVIMENTO_ORIGINE != "")
              {
                string str87 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(str3) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'TFR' ";
                string strSQL119 = !(TIPANN == "AC") ? str87 + " AND ANNPRE = 'S' " : str87 + " AND ANNPRE = 'N' ";
                string str88 = objDataAccess.Get1ValueFromSQL(strSQL119, CommandType.Text);
                string strSQL120 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON1 IN('TFR', 'TFRO')" + " ORDER BY IMPORTO DESC";
                dataTable2.Clear();
                dataTable2 = objDataAccess.GetDataTable(strSQL120);
                for (int index16 = 0; index16 <= dataTable2.Rows.Count - 1; ++index16)
                {
                  string strSQL121 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str88.Trim());
                  string str89 = objDataAccess.Get1ValueFromSQL(strSQL121, CommandType.Text);
                  int PROGSPLIA = str89 != string.Empty ? Convert.ToInt32(str89) : 0;
                  if (PROGSPLIA == 0)
                  {
                    string strSQL122 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'TFR' ";
                    string str90 = objDataAccess.Get1ValueFromSQL(strSQL122, CommandType.Text);
                    PROGSPLIA = str90 != string.Empty ? Convert.ToInt32(str90) : 0;
                  }
                  if (PROGSPLIA == 0)
                  {
                    string strSQL123 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RTFR' ";
                    string str91 = objDataAccess.Get1ValueFromSQL(strSQL123, CommandType.Text);
                    PROGSPLIA = str91 != string.Empty ? Convert.ToInt32(str91) : 0;
                  }
                  string PARTITA_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PARTITA"]) ? dataTable20.Rows[index11]["PARTITA"].ToString().Trim() : string.Empty;
                  int PROGMOV_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PROGMOV"]) ? Convert.ToInt32(dataTable20.Rows[index11]["PROGMOV"]) : 0;
                  if (Convert.ToInt32(dataTable2.Rows[index16]["IMPRES"]) > 0)
                    clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, Convert.ToInt32(dataTable2.Rows[index16]["PROGSPLI"]), PROGSPLIA, dataTable2.Rows[index16]["GESTCON1"].ToString().Trim(), Convert.ToDecimal(dataTable2.Rows[index16]["IMPRES"]));
                  num3 = 0;
                }
                str1 = "";
              }
              else
              {
                string str92 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(str3) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'TFR' ";
                string strSQL124 = !(TIPANN == "AC") ? str92 + " AND ANNPRE = 'S' " : str92 + " AND ANNPRE = 'N' ";
                string GESCON = objDataAccess.Get1ValueFromSQL(strSQL124, CommandType.Text);
                string strSQL125 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str93 = objDataAccess.Get1ValueFromSQL(strSQL125, CommandType.Text);
                int PROGSPLIA = str93 != string.Empty ? Convert.ToInt32(str93) : 0;
                if (PROGSPLIA == 0)
                {
                  string strSQL126 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'TFR' ";
                  string str94 = objDataAccess.Get1ValueFromSQL(strSQL126, CommandType.Text);
                  PROGSPLIA = str94 != string.Empty ? Convert.ToInt32(str94) : 0;
                }
                if (PROGSPLIA == 0)
                {
                  string strSQL127 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RTFR' ";
                  string str95 = objDataAccess.Get1ValueFromSQL(strSQL127, CommandType.Text);
                  PROGSPLIA = str95 != string.Empty ? Convert.ToInt32(str95) : 0;
                }
                string strSQL128 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str96 = objDataAccess.Get1ValueFromSQL(strSQL128, CommandType.Text);
                int PROGSPLID = str96 != string.Empty ? Convert.ToInt32(str96) : 0;
                if (PROGSPLID == 0)
                {
                  string strSQL129 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON = 'TFR' ";
                  string str97 = objDataAccess.Get1ValueFromSQL(strSQL129, CommandType.Text);
                  PROGSPLID = str97 != string.Empty ? Convert.ToInt32(str97) : 0;
                }
                if (PROGSPLID == 0)
                {
                  string strSQL130 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON = 'PTFR' ";
                  string str98 = objDataAccess.Get1ValueFromSQL(strSQL130, CommandType.Text);
                  PROGSPLID = str98 != string.Empty ? Convert.ToInt32(str98) : 0;
                }
                string PARTITA_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PARTITA"]) ? dataTable20.Rows[index11]["PARTITA"].ToString().Trim() : string.Empty;
                int PROGMOV_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PROGMOV"]) ? Convert.ToInt32(dataTable20.Rows[index11]["PROGMOV"]) : 0;
                clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, IMPORTO2);
                str1 = "";
                num2 = 0;
                num3 = 0;
              }
            }
            if (IMPORTO4 != 0M)
            {
              if (NUMERO_MOVIMENTO_ORIGINE != "")
              {
                string str99 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(str3) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'PREV' ";
                string strSQL131 = !(TIPANN == "AC") ? str99 + " AND ANNPRE = 'S' " : str99 + " AND ANNPRE = 'N' ";
                string str100 = objDataAccess.Get1ValueFromSQL(strSQL131, CommandType.Text);
                string strSQL132 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON1 IN('FP', 'FPO')" + " ORDER BY IMPORTO DESC";
                dataTable2.Clear();
                dataTable2 = objDataAccess.GetDataTable(strSQL132);
                for (int index17 = 0; index17 <= dataTable2.Rows.Count - 1; ++index17)
                {
                  string strSQL133 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str100.Trim());
                  string str101 = objDataAccess.Get1ValueFromSQL(strSQL133, CommandType.Text);
                  int PROGSPLIA = str101 != string.Empty ? Convert.ToInt32(str101) : 0;
                  if (PROGSPLIA == 0)
                  {
                    string strSQL134 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'FP' ";
                    string str102 = objDataAccess.Get1ValueFromSQL(strSQL134, CommandType.Text);
                    PROGSPLIA = str102 != string.Empty ? Convert.ToInt32(str102) : 0;
                  }
                  if (PROGSPLIA == 0)
                  {
                    string strSQL135 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RFP' ";
                    string str103 = objDataAccess.Get1ValueFromSQL(strSQL135, CommandType.Text);
                    PROGSPLIA = str103 != string.Empty ? Convert.ToInt32(str103) : 0;
                  }
                  string PARTITA_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PARTITA"]) ? dataTable20.Rows[index11]["PARTITA"].ToString().Trim() : string.Empty;
                  int PROGMOV_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PROGMOV"]) ? Convert.ToInt32(dataTable20.Rows[index11]["PROGMOV"]) : 0;
                  if (Convert.ToInt32(dataTable2.Rows[index17]["IMPRES"]) > 0)
                    clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, Convert.ToInt32(dataTable2.Rows[index17]["PROGSPLI"]), PROGSPLIA, dataTable2.Rows[index17]["GESTCON1"].ToString().Trim(), Convert.ToDecimal(dataTable2.Rows[index17]["IMPRES"]));
                  num3 = 0;
                }
                str1 = "";
              }
              else
              {
                string str104 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(str3) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'PREV' ";
                string strSQL136 = !(TIPANN == "AC") ? str104 + " AND ANNPRE = 'S' " : str104 + " AND ANNPRE = 'N' ";
                string GESCON = objDataAccess.Get1ValueFromSQL(strSQL136, CommandType.Text);
                string strSQL137 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str105 = objDataAccess.Get1ValueFromSQL(strSQL137, CommandType.Text);
                int PROGSPLIA = str105 != string.Empty ? Convert.ToInt32(str105) : 0;
                if (PROGSPLIA == 0)
                {
                  string strSQL138 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'FP' ";
                  string str106 = objDataAccess.Get1ValueFromSQL(strSQL138, CommandType.Text);
                  PROGSPLIA = str106 != string.Empty ? Convert.ToInt32(str106) : 0;
                }
                if (PROGSPLIA == 0)
                {
                  string strSQL139 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RFP' ";
                  string str107 = objDataAccess.Get1ValueFromSQL(strSQL139, CommandType.Text);
                  PROGSPLIA = str107 != string.Empty ? Convert.ToInt32(str107) : 0;
                }
                string strSQL140 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str108 = objDataAccess.Get1ValueFromSQL(strSQL140, CommandType.Text);
                int PROGSPLID = str108 != string.Empty ? Convert.ToInt32(str108) : 0;
                if (PROGSPLID == 0)
                {
                  string strSQL141 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON = 'FP' ";
                  string str109 = objDataAccess.Get1ValueFromSQL(strSQL141, CommandType.Text);
                  PROGSPLID = str109 != string.Empty ? Convert.ToInt32(str109) : 0;
                }
                if (PROGSPLID == 0)
                {
                  string strSQL142 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON = 'PFP' ";
                  string str110 = objDataAccess.Get1ValueFromSQL(strSQL142, CommandType.Text);
                  PROGSPLID = str110 != string.Empty ? Convert.ToInt32(str110) : 0;
                }
                string PARTITA_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PARTITA"]) ? dataTable20.Rows[index11]["PARTITA"].ToString().Trim() : string.Empty;
                int PROGMOV_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PROGMOV"]) ? Convert.ToInt32(dataTable20.Rows[index11]["PROGMOV"]) : 0;
                clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, IMPORTO4);
                str1 = "";
                num2 = 0;
                num3 = 0;
              }
            }
            if (IMPORTO3 != 0M)
            {
              if (NUMERO_MOVIMENTO_ORIGINE != "")
              {
                string str111 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(str3) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'INF' ";
                string strSQL143 = !(TIPANN == "AC") ? str111 + " AND ANNPRE = 'S' " : str111 + " AND ANNPRE = 'N' ";
                string str112 = objDataAccess.Get1ValueFromSQL(strSQL143, CommandType.Text);
                string strSQL144 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON1 IN('INF', 'INFO')" + " ORDER BY IMPORTO DESC";
                dataTable2.Clear();
                dataTable2 = objDataAccess.GetDataTable(strSQL144);
                for (int index18 = 0; index18 <= dataTable2.Rows.Count - 1; ++index18)
                {
                  string strSQL145 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str112.Trim());
                  string str113 = objDataAccess.Get1ValueFromSQL(strSQL145, CommandType.Text);
                  int PROGSPLIA = str113 != string.Empty ? Convert.ToInt32(str113) : 0;
                  if (PROGSPLIA == 0)
                  {
                    string strSQL146 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'INF' ";
                    string str114 = objDataAccess.Get1ValueFromSQL(strSQL146, CommandType.Text);
                    PROGSPLIA = str114 != string.Empty ? Convert.ToInt32(str114) : 0;
                  }
                  if (PROGSPLIA == 0)
                  {
                    string strSQL147 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RINF' ";
                    string str115 = objDataAccess.Get1ValueFromSQL(strSQL147, CommandType.Text);
                    PROGSPLIA = str115 != string.Empty ? Convert.ToInt32(str115) : 0;
                  }
                  string PARTITA_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PARTITA"]) ? dataTable20.Rows[index11]["PARTITA"].ToString().Trim() : string.Empty;
                  int PROGMOV_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PROGMOV"]) ? Convert.ToInt32(dataTable20.Rows[index11]["PROGMOV"]) : 0;
                  if (Convert.ToInt32(dataTable2.Rows[index18]["IMPRES"]) > 0)
                    clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, Convert.ToInt32(dataTable2.Rows[index18]["PROGSPLI"]), PROGSPLIA, dataTable2.Rows[index18]["GESTCON1"].ToString().Trim(), Convert.ToDecimal(dataTable2.Rows[index18]["IMPRES"]));
                  num3 = 0;
                }
                str1 = "";
              }
              else
              {
                string str116 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(str3) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATFORASS = 'INF' ";
                string strSQL148 = !(TIPANN == "AC") ? str116 + " AND ANNPRE = 'S' " : str116 + " AND ANNPRE = 'N' ";
                string GESCON = objDataAccess.Get1ValueFromSQL(strSQL148, CommandType.Text);
                string strSQL149 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str117 = objDataAccess.Get1ValueFromSQL(strSQL149, CommandType.Text);
                int PROGSPLIA = str117 != string.Empty ? Convert.ToInt32(str117) : 0;
                if (PROGSPLIA == 0)
                {
                  string strSQL150 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'INF' ";
                  string str118 = objDataAccess.Get1ValueFromSQL(strSQL150, CommandType.Text);
                  PROGSPLIA = str118 != string.Empty ? Convert.ToInt32(str118) : 0;
                }
                if (PROGSPLIA == 0)
                {
                  string strSQL151 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RINF' ";
                  string str119 = objDataAccess.Get1ValueFromSQL(strSQL151, CommandType.Text);
                  PROGSPLIA = str119 != string.Empty ? Convert.ToInt32(str119) : 0;
                }
                string strSQL152 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str120 = objDataAccess.Get1ValueFromSQL(strSQL152, CommandType.Text);
                int PROGSPLID = str120 != string.Empty ? Convert.ToInt32(str120) : 0;
                if (PROGSPLID == 0)
                {
                  string strSQL153 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON = 'INF' ";
                  string str121 = objDataAccess.Get1ValueFromSQL(strSQL153, CommandType.Text);
                  PROGSPLID = str121 != string.Empty ? Convert.ToInt32(str121) : 0;
                }
                if (PROGSPLID == 0)
                {
                  string strSQL154 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON = 'PINF' ";
                  string str122 = objDataAccess.Get1ValueFromSQL(strSQL154, CommandType.Text);
                  PROGSPLID = str122 != string.Empty ? Convert.ToInt32(str122) : 0;
                }
                string PARTITA_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PARTITA"]) ? dataTable20.Rows[index11]["PARTITA"].ToString().Trim() : string.Empty;
                int PROGMOV_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PROGMOV"]) ? Convert.ToInt32(dataTable20.Rows[index11]["PROGMOV"]) : 0;
                clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, IMPORTO3);
                str1 = "";
                num2 = 0;
                num3 = 0;
              }
            }
            if (IMPORTO1 != 0M)
            {
              if (NUMERO_MOVIMENTO_ORIGINE != "")
              {
                string str123 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(str3) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ADDIZIONALE' ";
                string strSQL155 = !(TIPANN == "AC") ? str123 + " AND ANNPRE = 'S' " : str123 + " AND ANNPRE = 'N' ";
                string str124 = objDataAccess.Get1ValueFromSQL(strSQL155, CommandType.Text);
                string strSQL156 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON1 IN('AD', 'ADO')" + " ORDER BY IMPORTO DESC";
                dataTable2.Clear();
                dataTable2 = objDataAccess.GetDataTable(strSQL156);
                for (int index19 = 0; index19 <= dataTable2.Rows.Count - 1; ++index19)
                {
                  string strSQL157 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str124.Trim());
                  string str125 = objDataAccess.Get1ValueFromSQL(strSQL157, CommandType.Text);
                  int PROGSPLIA = str125 != string.Empty ? Convert.ToInt32(str125) : 0;
                  if (PROGSPLIA == 0)
                  {
                    string strSQL158 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'AD' ";
                    string str126 = objDataAccess.Get1ValueFromSQL(strSQL158, CommandType.Text);
                    PROGSPLIA = str126 != string.Empty ? Convert.ToInt32(str126) : 0;
                  }
                  if (PROGSPLIA == 0)
                  {
                    string strSQL159 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RAD' ";
                    string str127 = objDataAccess.Get1ValueFromSQL(strSQL159, CommandType.Text);
                    PROGSPLIA = str127 != string.Empty ? Convert.ToInt32(str127) : 0;
                  }
                  string PARTITA_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PARTITA"]) ? dataTable20.Rows[index11]["PARTITA"].ToString().Trim() : string.Empty;
                  int PROGMOV_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PROGMOV"]) ? Convert.ToInt32(dataTable20.Rows[index11]["PROGMOV"]) : 0;
                  if (Convert.ToInt32(dataTable2.Rows[index19]["IMPRES"]) > 0)
                    clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, Convert.ToInt32(dataTable2.Rows[index19]["PROGSPLI"]), PROGSPLIA, dataTable2.Rows[index19]["GESTCON1"].ToString().Trim(), Convert.ToDecimal(dataTable2.Rows[index19]["IMPRES"]));
                  num3 = 0;
                }
                str1 = "";
              }
              else
              {
                string str128 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(str3) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ADDIZIONALE' ";
                string strSQL160 = !(TIPANN == "AC") ? str128 + " AND ANNPRE = 'S' " : str128 + " AND ANNPRE = 'N' ";
                string GESCON = objDataAccess.Get1ValueFromSQL(strSQL160, CommandType.Text);
                string strSQL161 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str129 = objDataAccess.Get1ValueFromSQL(strSQL161, CommandType.Text);
                int PROGSPLIA = str129 != string.Empty ? Convert.ToInt32(str129) : 0;
                if (PROGSPLIA == 0)
                {
                  string strSQL162 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'AD' ";
                  string str130 = objDataAccess.Get1ValueFromSQL(strSQL162, CommandType.Text);
                  PROGSPLIA = str130 != string.Empty ? Convert.ToInt32(str130) : 0;
                }
                if (PROGSPLIA == 0)
                {
                  string strSQL163 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RAD' ";
                  string str131 = objDataAccess.Get1ValueFromSQL(strSQL163, CommandType.Text);
                  PROGSPLIA = str131 != string.Empty ? Convert.ToInt32(str131) : 0;
                }
                string strSQL164 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str132 = objDataAccess.Get1ValueFromSQL(strSQL164, CommandType.Text);
                int PROGSPLID = str132 != string.Empty ? Convert.ToInt32(str132) : 0;
                if (PROGSPLID == 0)
                {
                  string strSQL165 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON = 'AD' ";
                  string str133 = objDataAccess.Get1ValueFromSQL(strSQL165, CommandType.Text);
                  PROGSPLID = str133 != string.Empty ? Convert.ToInt32(str133) : 0;
                }
                if (PROGSPLID == 0)
                {
                  string strSQL166 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON = 'PAD' ";
                  string str134 = objDataAccess.Get1ValueFromSQL(strSQL166, CommandType.Text);
                  PROGSPLID = str134 != string.Empty ? Convert.ToInt32(str134) : 0;
                }
                string PARTITA_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PARTITA"]) ? dataTable20.Rows[index11]["PARTITA"].ToString().Trim() : string.Empty;
                int PROGMOV_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PROGMOV"]) ? Convert.ToInt32(dataTable20.Rows[index11]["PROGMOV"]) : 0;
                clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, IMPORTO1);
                str1 = "";
                num2 = 0;
                num3 = 0;
              }
            }
            if (IMPORTO5 != 0M)
            {
              if (NUMERO_MOVIMENTO_ORIGINE != "")
              {
                string str135 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(str3) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ASSCON' ";
                string strSQL167 = !(TIPANN == "AC") ? str135 + " AND ANNPRE = 'S' " : str135 + " AND ANNPRE = 'N' ";
                string str136 = objDataAccess.Get1ValueFromSQL(strSQL167, CommandType.Text);
                string strSQL168 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON1 IN('AC', 'ACO')" + " ORDER BY IMPORTO DESC";
                dataTable2.Clear();
                dataTable2 = objDataAccess.GetDataTable(strSQL168);
                for (int index20 = 0; index20 <= dataTable2.Rows.Count - 1; ++index20)
                {
                  string strSQL169 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str136.Trim());
                  string str137 = objDataAccess.Get1ValueFromSQL(strSQL169, CommandType.Text);
                  int PROGSPLIA = str137 != string.Empty ? Convert.ToInt32(str137) : 0;
                  if (PROGSPLIA == 0)
                  {
                    string strSQL170 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'AC' ";
                    string str138 = objDataAccess.Get1ValueFromSQL(strSQL170, CommandType.Text);
                    PROGSPLIA = str138 != string.Empty ? Convert.ToInt32(str138) : 0;
                  }
                  if (PROGSPLIA == 0)
                  {
                    string strSQL171 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RAC' ";
                    string str139 = objDataAccess.Get1ValueFromSQL(strSQL171, CommandType.Text);
                    PROGSPLIA = str139 != string.Empty ? Convert.ToInt32(str139) : 0;
                  }
                  string PARTITA_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PARTITA"]) ? dataTable20.Rows[index11]["PARTITA"].ToString().Trim() : string.Empty;
                  int PROGMOV_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PROGMOV"]) ? Convert.ToInt32(dataTable20.Rows[index11]["PROGMOV"]) : 0;
                  if (Convert.ToInt32(dataTable2.Rows[index20]["IMPRES"]) > 0)
                    clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, Convert.ToInt32(dataTable2.Rows[index20]["PROGSPLI"]), PROGSPLIA, dataTable2.Rows[index20]["GESTCON1"].ToString().Trim(), Convert.ToDecimal(dataTable2.Rows[index20]["IMPRES"]));
                  num3 = 0;
                }
                str1 = "";
              }
              else
              {
                string str140 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(str3) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ASSCON' ";
                string strSQL172 = !(TIPANN == "AC") ? str140 + " AND ANNPRE = 'S' " : str140 + " AND ANNPRE = 'N' ";
                string GESCON = objDataAccess.Get1ValueFromSQL(strSQL172, CommandType.Text);
                string strSQL173 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str141 = objDataAccess.Get1ValueFromSQL(strSQL173, CommandType.Text);
                int PROGSPLIA = str141 != string.Empty ? Convert.ToInt32(str141) : 0;
                if (PROGSPLIA == 0)
                {
                  string strSQL174 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'AC' ";
                  string str142 = objDataAccess.Get1ValueFromSQL(strSQL174, CommandType.Text);
                  PROGSPLIA = str142 != string.Empty ? Convert.ToInt32(str142) : 0;
                }
                if (PROGSPLIA == 0)
                {
                  string strSQL175 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RAC' ";
                  string str143 = objDataAccess.Get1ValueFromSQL(strSQL175, CommandType.Text);
                  PROGSPLIA = str143 != string.Empty ? Convert.ToInt32(str143) : 0;
                }
                string strSQL176 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str144 = objDataAccess.Get1ValueFromSQL(strSQL176, CommandType.Text);
                int PROGSPLID = str144 != string.Empty ? Convert.ToInt32(str144) : 0;
                if (PROGSPLID == 0)
                {
                  string strSQL177 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON = 'AC' ";
                  string str145 = objDataAccess.Get1ValueFromSQL(strSQL177, CommandType.Text);
                  PROGSPLID = str145 != string.Empty ? Convert.ToInt32(str145) : 0;
                }
                if (PROGSPLID == 0)
                {
                  string strSQL178 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON = 'PAC' ";
                  string str146 = objDataAccess.Get1ValueFromSQL(strSQL178, CommandType.Text);
                  PROGSPLID = str146 != string.Empty ? Convert.ToInt32(str146) : 0;
                }
                string PARTITA_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PARTITA"]) ? dataTable20.Rows[index11]["PARTITA"].ToString().Trim() : string.Empty;
                int PROGMOV_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PROGMOV"]) ? Convert.ToInt32(dataTable20.Rows[index11]["PROGMOV"]) : 0;
                clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, IMPORTO5);
                str1 = "";
                num2 = 0;
                num3 = 0;
              }
            }
            if (IMPORTO6 != 0M)
            {
              if (NUMERO_MOVIMENTO_ORIGINE != "")
              {
                string str147 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(str3) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ABBPRE' ";
                string strSQL179 = !(TIPANN == "AC") ? str147 + " AND ANNPRE = 'S' " : str147 + " AND ANNPRE = 'N' ";
                string str148 = objDataAccess.Get1ValueFromSQL(strSQL179, CommandType.Text);
                string strSQL180 = "SELECT * FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON1 IN('AB', 'ABO')" + " ORDER BY IMPORTO DESC";
                dataTable2.Clear();
                dataTable2 = objDataAccess.GetDataTable(strSQL180);
                for (int index21 = 0; index21 <= dataTable2.Rows.Count - 1; ++index21)
                {
                  string strSQL181 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(str148.Trim());
                  string str149 = objDataAccess.Get1ValueFromSQL(strSQL181, CommandType.Text);
                  int PROGSPLIA = str149 != string.Empty ? Convert.ToInt32(str149) : 0;
                  if (PROGSPLIA == 0)
                  {
                    string strSQL182 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'AB' ";
                    string str150 = objDataAccess.Get1ValueFromSQL(strSQL182, CommandType.Text);
                    PROGSPLIA = str150 != string.Empty ? Convert.ToInt32(str150) : 0;
                  }
                  if (PROGSPLIA == 0)
                  {
                    string strSQL183 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RAB' ";
                    string str151 = objDataAccess.Get1ValueFromSQL(strSQL183, CommandType.Text);
                    PROGSPLIA = str151 != string.Empty ? Convert.ToInt32(str151) : 0;
                  }
                  string PARTITA_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PARTITA"]) ? dataTable20.Rows[index11]["PARTITA"].ToString().Trim() : string.Empty;
                  int PROGMOV_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PROGMOV"]) ? Convert.ToInt32(dataTable20.Rows[index11]["PROGMOV"]) : 0;
                  if (Convert.ToInt32(dataTable2.Rows[index21]["IMPRES"]) > 0)
                    clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, Convert.ToInt32(dataTable2.Rows[index21]["PROGSPLI"]), PROGSPLIA, dataTable2.Rows[index21]["GESTCON1"].ToString().Trim(), Convert.ToDecimal(dataTable2.Rows[index21]["IMPRES"]));
                  num3 = 0;
                }
                str1 = "";
              }
              else
              {
                string str152 = "SELECT GESCON1 FROM CONTI WHERE " + DBMethods.Db2Date(str3) + " BETWEEN DATINI AND DATFIN" + " AND MOVANN = 'S' " + " AND CATIMP = 'ABBPRE' ";
                string strSQL184 = !(TIPANN == "AC") ? str152 + " AND ANNPRE = 'S' " : str152 + " AND ANNPRE = 'N' ";
                string GESCON = objDataAccess.Get1ValueFromSQL(strSQL184, CommandType.Text);
                string strSQL185 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str153 = objDataAccess.Get1ValueFromSQL(strSQL185, CommandType.Text);
                int PROGSPLIA = str153 != string.Empty ? Convert.ToInt32(str153) : 0;
                if (PROGSPLIA == 0)
                {
                  string strSQL186 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'AB' ";
                  string str154 = objDataAccess.Get1ValueFromSQL(strSQL186, CommandType.Text);
                  PROGSPLIA = str154 != string.Empty ? Convert.ToInt32(str154) : 0;
                }
                if (PROGSPLIA == 0)
                {
                  string strSQL187 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON = 'RAB' ";
                  string str155 = objDataAccess.Get1ValueFromSQL(strSQL187, CommandType.Text);
                  PROGSPLIA = str155 != string.Empty ? Convert.ToInt32(str155) : 0;
                }
                string strSQL188 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON1 = " + DBMethods.DoublePeakForSql(GESCON.Trim());
                string str156 = objDataAccess.Get1ValueFromSQL(strSQL188, CommandType.Text);
                int PROGSPLID = str156 != string.Empty ? Convert.ToInt32(str156) : 0;
                if (PROGSPLID == 0)
                {
                  string strSQL189 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON = 'AB' ";
                  string str157 = objDataAccess.Get1ValueFromSQL(strSQL189, CommandType.Text);
                  PROGSPLID = str157 != string.Empty ? Convert.ToInt32(str157) : 0;
                }
                if (PROGSPLID == 0)
                {
                  string strSQL190 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND GESTCON = 'PAB' ";
                  string str158 = objDataAccess.Get1ValueFromSQL(strSQL190, CommandType.Text);
                  PROGSPLID = str158 != string.Empty ? Convert.ToInt32(str158) : 0;
                }
                string PARTITA_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PARTITA"]) ? dataTable20.Rows[index11]["PARTITA"].ToString().Trim() : string.Empty;
                int PROGMOV_DA = !DBNull.Value.Equals(dataTable20.Rows[index11]["PROGMOV"]) ? Convert.ToInt32(dataTable20.Rows[index11]["PROGMOV"]) : 0;
                clsWriteDb.WRITE_INSERT_SPLABSAP(objDataAccess, u, PARTITA_DA, PROGMOV_DA, PARTITA_MOVIMENTO, Convert.ToInt32(PROGMOV_MOVIMENTO), PROGABBIN, PROGSPLID, PROGSPLIA, GESCON, IMPORTO6);
                str1 = "";
                num2 = 0;
                num3 = 0;
              }
            }
            string strSQL191 = "UPDATE MOVIMSAP SET " + " IMPRIDUZ = (IMPRIDUZ + " + num4.ToString().Replace(',', '.') + "), " + " IMPABB = (IMPABB + " + num4.ToString().Replace(',', '.') + "), " + " IMPRESID = (IMPRESID - " + num4.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString();
            objDataAccess.WriteTransactionData(strSQL191, CommandType.Text);
            string strSQL192 = " SELECT * FROM SPLABSAP " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString().Trim()) + " AND PROGMOVD = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOVA = " + PROGMOV_MOVIMENTO.ToString() + " AND PROGABBIN = " + PROGABBIN.ToString();
            dataTable21.Clear();
            dataTable6 = objDataAccess.GetDataTable(strSQL192);
            for (int index22 = 0; index22 <= dataTable6.Rows.Count - 1; ++index22)
            {
              string strSQL193 = "UPDATE SPLIMPOSAP SET " + " IMPRIDU = (IMPRIDU + " + dataTable6.Rows[index22]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPABB = (IMPABB + " + dataTable6.Rows[index22]["IMPORTO"].ToString().Replace(',', '.') + "), " + " IMPRES = (IMPRES - " + dataTable6.Rows[index22]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable20.Rows[index11]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable20.Rows[index11]["PROGMOV"]?.ToString() + " AND PROGSPLI = " + dataTable6.Rows[index22]["PROGSPLID"]?.ToString();
              objDataAccess.WriteTransactionData(strSQL193, CommandType.Text);
            }
            if (index11 == 0)
            {
              string strSQL194 = "UPDATE MOVIMSAP SET " + " IMPORTO = " + (num4 * -1M).ToString().Replace(',', '.') + ", " + " IMPABB = " + (num4 * -1M).ToString().Replace(',', '.') + ", " + " IMPRESID = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString();
              objDataAccess.WriteTransactionData(strSQL194, CommandType.Text);
              string strSQL195 = "UPDATE SPLIMPOSAP SET " + " IMPABB = IMPORTO, " + " IMPRES = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON <> 'CAZA'";
              objDataAccess.WriteTransactionData(strSQL195, CommandType.Text);
              string strSQL196 = "UPDATE SPLIMPOSAP SET " + " IMPORTO = " + (IMPORTO1 * -1M).ToString().Replace(',', '.') + ", " + " IMPABB = " + (IMPORTO1 * -1M).ToString().Replace(',', '.') + ", " + " IMPRES = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON IN ('AD','PAD','RAD')";
              objDataAccess.WriteTransactionData(strSQL196, CommandType.Text);
              string strSQL197 = "UPDATE SPLIMPOSAP SET " + " IMPORTO = " + (IMPORTO2 + IMPORTO4 + IMPORTO3 + IMPORTO1 + IMPORTO5 + IMPORTO6).ToString().Replace(',', '.') + ", " + " IMPABB = " + (IMPORTO2 + IMPORTO4 + IMPORTO3 + IMPORTO1 + IMPORTO5 + IMPORTO6).ToString().Replace(',', '.') + ", " + " IMPRES = 0" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + " AND PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " AND GESTCON IN ('CAZA')";
              objDataAccess.WriteTransactionData(strSQL197, CommandType.Text);
            }
            num4 = 0.0M;
            IMPORTO1 = 0.0M;
            IMPORTO2 = 0.0M;
            IMPORTO3 = 0.0M;
            IMPORTO4 = 0.0M;
            IMPORTO5 = 0.0M;
            IMPORTO6 = 0.0M;
            PROGABBIN = 0;
          }
        }
        return str5;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    private static void DELETE_VERSAMENTO(
      DataLayer objDataAccess,
      string PARTITAMOV,
      int PROGMOV,
      DataRow ROW_VER,
      ref Decimal IMPTFR_AB,
      ref Decimal IMPFP_AB,
      ref Decimal IMPINF_AB,
      ref Decimal IMPAD_AB,
      ref Decimal IMPAC_AB,
      ref Decimal IMPAB_AB,
      string MOVIMENTO,
      Decimal IMPTFR_RETTIFICA,
      Decimal IMPFP_RETTIFICA,
      Decimal IMPINF_RETTIFICA,
      Decimal IMPAD_RETTIFICA)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      Decimal num1 = 0.0M;
      Decimal num2 = 0.0M;
      Decimal num3 = 0.0M;
      Decimal num4 = 0.0M;
      Decimal num5 = 0.0M;
      string str1 = "";
      string str2 = "";
      string str3 = "";
      string str4 = "";
      string strSQL1 = "SELECT IMPORTO, GESTCON FROM SPLABSAP  " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOVD = " + PROGMOV.ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"].ToString().Trim();
      dataTable1.Clear();
      DataTable dataTable4 = objDataAccess.GetDataTable(strSQL1);
      if (dataTable4.Rows.Count > 0)
      {
        for (int index = 0; index <= dataTable4.Rows.Count - 1; ++index)
        {
          switch (dataTable4.Rows[index]["GESTCON"].ToString().Trim())
          {
            case "AB":
            case "ABO":
              IMPAB_AB = Convert.ToDecimal(dataTable4.Rows[index]["IMPORTO"]);
              num1 += IMPAB_AB;
              break;
            case "AC":
            case "ACO":
              IMPAC_AB = Convert.ToDecimal(dataTable4.Rows[index]["IMPORTO"]);
              num1 += IMPAC_AB;
              break;
            case "AD":
            case "ADO":
              IMPAD_AB = Convert.ToDecimal(dataTable4.Rows[index]["IMPORTO"]);
              num1 += IMPAD_AB;
              break;
            case "FP":
            case "FPO":
              IMPFP_AB = Convert.ToDecimal(dataTable4.Rows[index]["IMPORTO"]);
              num1 += IMPFP_AB;
              break;
            case "INF":
            case "INFO":
              IMPINF_AB = Convert.ToDecimal(dataTable4.Rows[index]["IMPORTO"]);
              num1 += IMPINF_AB;
              break;
            case "TFR":
            case "TFRO":
              IMPTFR_AB = Convert.ToDecimal(dataTable4.Rows[index]["IMPORTO"]);
              num1 += IMPTFR_AB;
              break;
          }
        }
      }
      string strSQL2 = " SELECT * FROM SPLIMPOSAP WHERE PARTITA  = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOV = " + PROGMOV.ToString().Trim() + " AND GESTCON NOT IN ('CAZA', 'AB', 'AC', 'PAC', 'RAC', 'PAB', 'RAB')";
      dataTable2.Clear();
      DataTable dataTable5 = objDataAccess.GetDataTable(strSQL2);
      for (int index = 0; index <= dataTable5.Rows.Count - 1; ++index)
      {
        switch (dataTable5.Rows[index]["GESTCON"].ToString().Trim())
        {
          case "AD":
          case "PAD":
          case "RAD":
            num5 = Convert.ToDecimal(dataTable5.Rows[index]["IMPRES"]) - IMPAD_RETTIFICA;
            if (num5 < 0M)
            {
              num5 = 0M;
            }
            else
            {
              num5 -= IMPAD_AB;
              if (num5 == 0M)
                num5 = 0M;
              else if (num5 < 0M)
                num5 = IMPAD_AB - (Convert.ToDecimal(dataTable5.Rows[index]["IMPRES"]) - IMPAD_RETTIFICA);
              else if (num5 > 0M)
                num5 = IMPAD_AB;
            }
            if (num5 == 0M)
            {
              string strSQL3 = "DELETE FROM SPLABSAP WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOVD = " + PROGMOV.ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND PROGSPLID = " + dataTable5.Rows[index]["PROGSPLI"].ToString().Trim();
              objDataAccess.WriteTransactionData(strSQL3, CommandType.Text);
            }
            else
            {
              string strSQL4 = "UPDATE SPLABSAP SET IMPORTO = " + num5.ToString().Replace(',', '.') + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOVD = " + PROGMOV.ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND PROGSPLID = " + dataTable5.Rows[index]["PROGSPLI"].ToString().Trim();
              objDataAccess.WriteTransactionData(strSQL4, CommandType.Text);
            }
            str4 = dataTable5.Rows[index]["GESTCON"].ToString().Trim();
            break;
          case "FP":
          case "PFP":
          case "RFP":
            num3 = Convert.ToDecimal(dataTable5.Rows[index]["IMPRES"]) - IMPFP_RETTIFICA;
            if (num3 < 0M)
            {
              num3 = 0M;
            }
            else
            {
              num3 -= IMPFP_AB;
              if (num3 == 0M)
                num3 = 0M;
              else if (num3 < 0M)
                num3 = IMPFP_AB - (Convert.ToDecimal(dataTable5.Rows[index]["IMPRES"]) - IMPFP_RETTIFICA);
              else if (num3 > 0M)
                num3 = IMPFP_AB;
            }
            if (num3 == 0M)
            {
              string strSQL5 = "DELETE FROM SPLABSAP WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOVD = " + PROGMOV.ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND PROGSPLID = " + dataTable5.Rows[index]["PROGSPLI"].ToString().Trim();
              objDataAccess.WriteTransactionData(strSQL5, CommandType.Text);
            }
            else
            {
              string strSQL6 = "UPDATE SPLABSAP SET IMPORTO = " + num3.ToString().Replace(',', '.') + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOVD = " + PROGMOV.ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND PROGSPLID = " + dataTable5.Rows[index]["PROGSPLI"].ToString().Trim();
              objDataAccess.WriteTransactionData(strSQL6, CommandType.Text);
            }
            str3 = dataTable5.Rows[index]["GESTCON"].ToString().Trim();
            break;
          case "INF":
          case "PINF":
          case "RINF":
            num4 = Convert.ToDecimal(dataTable5.Rows[index]["IMPRES"]) - IMPINF_RETTIFICA;
            if (num4 < 0M)
            {
              num4 = 0M;
            }
            else
            {
              num4 -= IMPINF_AB;
              if (num4 == 0M)
                num4 = 0M;
              else if (num4 < 0M)
                num4 = IMPINF_AB - (Convert.ToDecimal(dataTable5.Rows[index]["IMPRES"]) - IMPINF_RETTIFICA);
              else if (num4 > 0M)
                num4 = IMPINF_AB;
            }
            if (num4 == 0M)
            {
              string strSQL7 = "DELETE FROM SPLABSAP WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOVD = " + PROGMOV.ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND PROGSPLID = " + dataTable5.Rows[index]["PROGSPLI"].ToString().Trim();
              objDataAccess.WriteTransactionData(strSQL7, CommandType.Text);
            }
            else
            {
              string strSQL8 = "UPDATE SPLABSAP SET IMPORTO = " + num4.ToString().Replace(',', '.') + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOVD = " + PROGMOV.ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND PROGSPLID = " + dataTable5.Rows[index]["PROGSPLI"].ToString().Trim();
              objDataAccess.WriteTransactionData(strSQL8, CommandType.Text);
            }
            str2 = dataTable5.Rows[index]["GESTCON"].ToString().Trim();
            break;
          case "PTFR":
          case "RTFR":
          case "TFR":
            num2 = Convert.ToDecimal(dataTable5.Rows[index]["IMPRES"]) - IMPTFR_RETTIFICA;
            if (num2 < 0M)
            {
              num2 = 0M;
            }
            else
            {
              num2 -= IMPTFR_AB;
              if (num2 == 0M)
                num2 = 0M;
              else if (num2 < 0M)
                num2 = IMPTFR_AB - (Convert.ToDecimal(dataTable5.Rows[index]["IMPRES"]) - IMPTFR_RETTIFICA);
              else if (num2 > 0M)
                num2 = IMPTFR_AB;
            }
            if (num2 == 0M)
            {
              string strSQL9 = "DELETE FROM SPLABSAP WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOVD = " + PROGMOV.ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND PROGSPLID = " + dataTable5.Rows[index]["PROGSPLI"].ToString().Trim();
              objDataAccess.WriteTransactionData(strSQL9, CommandType.Text);
            }
            else
            {
              string strSQL10 = "UPDATE SPLABSAP SET IMPORTO = " + num2.ToString().Replace(',', '.') + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOVD = " + PROGMOV.ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND PROGSPLID = " + dataTable5.Rows[index]["PROGSPLI"].ToString().Trim();
              objDataAccess.WriteTransactionData(strSQL10, CommandType.Text);
            }
            str1 = dataTable5.Rows[index]["GESTCON"].ToString().Trim();
            break;
        }
      }
      string strSQL11 = "SELECT VALUE(SUM(IMPORTO),0.0) AS IMPORTO FROM SPLABSAP WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOVD = " + PROGMOV.ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"].ToString().Trim();
      dataTable4.Clear();
      DataTable dataTable6 = objDataAccess.GetDataTable(strSQL11);
      if (dataTable6.Rows.Count > 0)
      {
        if (Convert.ToDecimal(dataTable6.Rows[0]["IMPORTO"]) == 0.0M)
        {
          string strSQL12 = "DELETE FROM ABBINSAP WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOVD = " + PROGMOV.ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"].ToString().Trim();
          objDataAccess.WriteTransactionData(strSQL12, CommandType.Text);
        }
        else
        {
          for (int index = 0; index <= dataTable6.Rows.Count - 1; ++index)
          {
            if (index == 0)
            {
              string strSQL13 = "UPDATE ABBINSAP SET IMPORTO = 0" + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOVD = " + PROGMOV.ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"].ToString().Trim();
              objDataAccess.WriteTransactionData(strSQL13, CommandType.Text);
            }
            string strSQL14 = "UPDATE ABBINSAP SET IMPORTO = IMPORTO + " + dataTable6.Rows[0]["IMPORTO"].ToString().Replace(',', '.') + " " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND PROGMOVD = " + PROGMOV.ToString().Trim() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"].ToString().Trim();
            objDataAccess.WriteTransactionData(strSQL14, CommandType.Text);
          }
        }
      }
      string strSQL15 = " SELECT * FROM SPLIMPOSAP WHERE PARTITA  = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOV = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND FLSAP = 'N'";
      dataTable5.Clear();
      DataTable dataTable7 = objDataAccess.GetDataTable(strSQL15);
      string str5 = MOVIMENTO.Trim();
      if (!(str5 == "03") && !(str5 == "71"))
      {
        if (str5 == "09")
        {
          string strSQL16 = "  SELECT VALUE(SUM(IMPORTO),0.0) FROM SPLABSAP " + " WHERE PARTITAA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[0]["PARTITA"].ToString().Trim()) + " AND PROGMOVA = " + dataTable7.Rows[0][nameof (PROGMOV)].ToString().Trim() + " AND PROGSPLIA = " + dataTable7.Rows[0]["PROGSPLI"].ToString().Trim();
          Decimal num6 = Convert.ToDecimal(objDataAccess.Get1ValueFromSQL(strSQL16, CommandType.Text));
          string strSQL17 = "SELECT * FROM SPLIMPOSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOV = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND GESTCON <> 'CAZA'";
          dataTable6.Clear();
          dataTable6 = objDataAccess.GetDataTable(strSQL17);
          for (int index = 0; index <= dataTable6.Rows.Count - 1; ++index)
          {
            string str6 = " UPDATE SPLIMPOSAP SET" + " IMPABB = " + num6.ToString().Replace(',', '.') + " * -1, ";
            string strSQL18 = (Convert.ToInt32(dataTable6.Rows[index]["IMPORTO"]) >= 0 ? str6 + " IMPRES = (IMPORTO  - " + num6.ToString().Replace(',', '.') + ") " : str6 + " IMPRES = (IMPORTO  - (" + num6.ToString().Replace(',', '.') + " * - 1)) ") + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[index][nameof (PROGMOV)].ToString().Trim() + " AND PROGSPLI = " + dataTable6.Rows[index]["PROGSPLI"].ToString().Trim();
            objDataAccess.WriteTransactionData(strSQL18, CommandType.Text);
          }
          string strSQL19 = "  SELECT VALUE(SUM(IMPABB),0.0) AS IMPABB, VALUE(SUM(IMPRIDU),0.0) AS IMPRIDU" + " FROM SPLIMPOSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[0]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable7.Rows[0][nameof (PROGMOV)].ToString().Trim() + " AND FLSAP = 'N' ";
          dataTable3.Clear();
          DataTable dataTable8 = objDataAccess.GetDataTable(strSQL19);
          string str7 = " UPDATE MOVIMSAP SET" + " IMPABB = " + dataTable8.Rows[0]["IMPABB"].ToString().Replace(',', '.') + " , ";
          string strSQL20 = (Convert.ToInt32(ROW_VER["IMPORTO"]) <= 0 ? str7 + " IMPRESID = (IMPORTO - " + dataTable8.Rows[0]["IMPABB"].ToString().Replace(',', '.') + ")  " : str7 + " IMPRESID = (IMPORTO * - 1 + " + dataTable8.Rows[0]["IMPABB"].ToString().Replace(',', '.') + ") * - 1") + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOV = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND CODCAUS = " + DBMethods.DoublePeakForSql(MOVIMENTO);
          objDataAccess.WriteTransactionData(strSQL20, CommandType.Text);
        }
      }
      else
      {
        for (int index = 0; index <= dataTable7.Rows.Count - 1; ++index)
        {
          switch (dataTable7.Rows[index]["GESTCON"].ToString().Trim())
          {
            case "AD":
            case "PAD":
              string strSQL21 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB + (" + IMPAD_AB.ToString().Replace(',', '.') + "), " + " IMPRES = IMPRES + (" + IMPAD_AB.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[0]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable7.Rows[0][nameof (PROGMOV)].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable7.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL21, CommandType.Text);
              break;
            case "FP":
            case "PFP":
              string strSQL22 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB + (" + IMPFP_AB.ToString().Replace(',', '.') + "), " + " IMPRES = IMPRES + (" + IMPFP_AB.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[0]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable7.Rows[0][nameof (PROGMOV)].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable7.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL22, CommandType.Text);
              break;
            case "INF":
            case "PINF":
              string strSQL23 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB + (" + IMPINF_AB.ToString().Replace(',', '.') + "), " + " IMPRES = IMPRES + (" + IMPINF_AB.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[0]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable7.Rows[0][nameof (PROGMOV)].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable7.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL23, CommandType.Text);
              break;
            case "PTFR":
            case "TFR":
              string strSQL24 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB + (" + IMPTFR_AB.ToString().Replace(',', '.') + "), " + " IMPRES = IMPRES + (" + IMPTFR_AB.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[0]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable7.Rows[0][nameof (PROGMOV)].ToString().Trim() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable7.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL24, CommandType.Text);
              break;
          }
        }
        string strSQL25 = "  SELECT VALUE(SUM(IMPABB),0.0) AS IMPABB, VALUE(SUM(IMPRIDU),0.0) AS IMPRIDU" + " FROM SPLIMPOSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable7.Rows[0]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable7.Rows[0][nameof (PROGMOV)].ToString().Trim() + " AND FLSAP = 'N' ";
        dataTable3.Clear();
        DataTable dataTable9 = objDataAccess.GetDataTable(strSQL25);
        string str8 = " UPDATE MOVIMSAP SET" + " IMPABB = " + dataTable9.Rows[0]["IMPABB"].ToString().Replace(',', '.') + " * -1, ";
        string strSQL26 = (Convert.ToInt32(ROW_VER["IMPORTO"]) <= 0 ? str8 + " IMPRESID = (IMPORTO - " + dataTable9.Rows[0]["IMPABB"].ToString().Replace(',', '.') + ")  " : str8 + " IMPRESID = (IMPORTO * - 1 + " + dataTable9.Rows[0]["IMPABB"].ToString().Replace(',', '.') + ") * - 1") + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOV = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND CODCAUS = " + DBMethods.DoublePeakForSql(MOVIMENTO);
        objDataAccess.WriteTransactionData(strSQL26, CommandType.Text);
      }
      Decimal num7 = Convert.ToDecimal(ROW_VER["IMPORTO"]) - IMPAC_AB - IMPAB_AB - num5 - num3 - num4 - num2;
      string str9 = " UPDATE MOVIMSAP SET" + " IMPABB = IMPABB - (" + num7.ToString().Replace(',', '.') + "), ";
      if (MOVIMENTO.Trim() == "71")
      {
        string str10 = str9;
        num7 = Convert.ToDecimal(ROW_VER["IMPORTO"]) - IMPAC_AB - IMPAB_AB - num5 - num3 - num4 - num2;
        string str11 = num7.ToString().Replace(',', '.');
        str9 = str10 + " IMPRIDUZ = IMPRIDUZ - (" + str11 + "), ";
      }
      string str12 = str9;
      num7 = Convert.ToDecimal(ROW_VER["IMPORTO"]) - IMPAC_AB - IMPAB_AB - num5 - num3 - num4 - num2;
      string str13 = num7.ToString().Replace(',', '.');
      string strSQL27 = str12 + " IMPRESID = IMPRESID + (" + str13 + ")" + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString();
      objDataAccess.WriteTransactionData(strSQL27, CommandType.Text);
      string strSQL28 = "SELECT * FROM SPLIMPOSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON IN ('" + str1 + "', '" + str2 + "', '" + str3 + "', '" + str4 + "') ";
      dataTable6.Clear();
      DataTable dataTable10 = objDataAccess.GetDataTable(strSQL28);
      for (int index = 0; index <= dataTable10.Rows.Count - 1; ++index)
      {
        switch (dataTable10.Rows[index]["GESTCON"].ToString().Trim())
        {
          case "AD":
          case "PAD":
            num7 = IMPAD_AB - num5;
            string str14 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + num7.ToString().Replace(',', '.') + "), ";
            if (MOVIMENTO.Trim() == "71")
            {
              string str15 = str14;
              num7 = IMPAD_AB - num5;
              string str16 = num7.ToString().Replace(',', '.');
              str14 = str15 + " IMPRIDU = IMPRIDU - (" + str16 + "), ";
            }
            string str17 = str14;
            num7 = IMPAD_AB - num5;
            string str18 = num7.ToString().Replace(',', '.');
            string strSQL29 = str17 + " IMPRES = IMPRES + (" + str18 + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable10.Rows[index]["GESTCON"].ToString().Trim());
            objDataAccess.WriteTransactionData(strSQL29, CommandType.Text);
            break;
          case "FP":
          case "PFP":
            num7 = IMPFP_AB - num3;
            string str19 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + num7.ToString().Replace(',', '.') + "), ";
            if (MOVIMENTO.Trim() == "71")
            {
              string str20 = str19;
              num7 = IMPFP_AB - num3;
              string str21 = num7.ToString().Replace(',', '.');
              str19 = str20 + " IMPRIDU = IMPRIDU - (" + str21 + "), ";
            }
            string str22 = str19;
            num7 = IMPFP_AB - num3;
            string str23 = num7.ToString().Replace(',', '.');
            string strSQL30 = str22 + " IMPRES = IMPRES + (" + str23 + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable10.Rows[index]["GESTCON"].ToString().Trim());
            objDataAccess.WriteTransactionData(strSQL30, CommandType.Text);
            break;
          case "INF":
          case "PINF":
            num7 = IMPINF_AB - num4;
            string str24 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + num7.ToString().Replace(',', '.') + "), ";
            if (MOVIMENTO.Trim() == "71")
            {
              string str25 = str24;
              num7 = IMPINF_AB - num4;
              string str26 = num7.ToString().Replace(',', '.');
              str24 = str25 + " IMPRIDU = IMPRIDU - (" + str26 + "), ";
            }
            string str27 = str24;
            num7 = IMPINF_AB - num4;
            string str28 = num7.ToString().Replace(',', '.');
            string strSQL31 = str27 + " IMPRES = IMPRES + (" + str28 + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable10.Rows[index]["GESTCON"].ToString().Trim());
            objDataAccess.WriteTransactionData(strSQL31, CommandType.Text);
            break;
          case "PTFR":
          case "TFR":
            num7 = IMPTFR_AB - num2;
            string str29 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + num7.ToString().Replace(',', '.') + "), ";
            if (MOVIMENTO.Trim() == "71")
            {
              string str30 = str29;
              num7 = IMPTFR_AB - num2;
              string str31 = num7.ToString().Replace(',', '.');
              str29 = str30 + " IMPRIDU = IMPRIDU - (" + str31 + "), ";
            }
            string str32 = str29;
            num7 = IMPTFR_AB - num2;
            string str33 = num7.ToString().Replace(',', '.');
            string strSQL32 = str32 + " IMPRES = IMPRES + (" + str33 + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable10.Rows[index]["GESTCON"].ToString().Trim());
            objDataAccess.WriteTransactionData(strSQL32, CommandType.Text);
            break;
        }
      }
    }

    private static void UPDATE_VERSAMENTO(
      DataLayer objDataAccess,
      string PARTITAMOV,
      int PROGMOV,
      DataRow ROW_VER,
      Decimal IMPRET_RESID,
      Decimal IMPTFR_RESID,
      Decimal IMPFP_RESID,
      Decimal IMPINF_RESID,
      Decimal IMPAD_RESID,
      string MOVIMENTO)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      string str1 = "";
      string str2 = "";
      string str3 = "";
      string str4 = "";
      DataTable dataTable4 = new DataTable();
      Decimal num1 = 0.0M;
      Decimal num2 = 0.0M;
      Decimal num3 = 0.0M;
      Decimal num4 = 0.0M;
      Decimal num5 = 0.0M;
      Decimal num6 = 0.0M;
      Decimal num7 = 0.0M;
      Decimal num8 = 0.0M;
      bool flag = false;
      string strSQL1 = " UPDATE ABBINSAP SET" + " IMPORTO = IMPORTO - (" + IMPRET_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString();
      objDataAccess.WriteTransactionData(strSQL1, CommandType.Text);
      string strSQL2 = "SELECT PROGSPLI FROM SPLIMPOSAP WHERE " + " PARTITA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOV = " + ROW_VER["PROGMOVA"]?.ToString() + " AND GESTCON <> 'CAZA'";
      int int32_1 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text));
      Decimal num9;
      int int32_2;
      if (IMPTFR_RESID > 0M)
      {
        string strSQL3 = "SELECT * FROM SPLABSAP WHERE " + " PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PROGSPLIA = " + int32_1.ToString() + " AND GESTCON = 'TFR'";
        dataTable4.Clear();
        dataTable4 = objDataAccess.GetDataTable(strSQL3);
        if (dataTable4.Rows.Count > 0)
        {
          if (Convert.ToDecimal(dataTable4.Rows[0]["IMPORTO"]) < IMPTFR_RESID)
          {
            flag = true;
            string strSQL4 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + dataTable4.Rows[0]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'TFR'";
            objDataAccess.WriteTransactionData(strSQL4, CommandType.Text);
            num9 = IMPTFR_RESID - Convert.ToDecimal(dataTable4.Rows[0]["IMPORTO"]);
            string strSQL5 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + num9.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'TFRO'";
            objDataAccess.WriteTransactionData(strSQL5, CommandType.Text);
            num1 = Convert.ToDecimal(dataTable4.Rows[0]["IMPORTO"]);
            num5 = IMPTFR_RESID - Convert.ToDecimal(dataTable4.Rows[0]["IMPORTO"]);
            str1 = "TFR', 'PTFR";
          }
          else
          {
            string strSQL6 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + IMPTFR_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'TFR'";
            objDataAccess.WriteTransactionData(strSQL6, CommandType.Text);
            str1 = "TFR";
          }
        }
        else
        {
          string strSQL7 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + IMPTFR_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'TFRO'";
          if (Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL7, CommandType.Text)) == 0)
          {
            string strSQL8 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + IMPTFR_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'TFR'";
            int32_2 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL8, CommandType.Text));
            str1 = "TFR";
          }
          else
            str1 = "PTFR";
        }
      }
      if (IMPINF_RESID > 0M)
      {
        string strSQL9 = "SELECT * FROM SPLABSAP WHERE " + " PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PROGSPLIA = " + int32_1.ToString() + " AND GESTCON = 'INF'";
        dataTable4.Clear();
        dataTable4 = objDataAccess.GetDataTable(strSQL9);
        if (dataTable4.Rows.Count > 0)
        {
          if (Convert.ToDecimal(dataTable4.Rows[0]["IMPORTO"]) < IMPINF_RESID)
          {
            flag = true;
            string strSQL10 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + dataTable4.Rows[0]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'INF'";
            objDataAccess.WriteTransactionData(strSQL10, CommandType.Text);
            num9 = IMPINF_RESID - Convert.ToDecimal(dataTable4.Rows[0]["IMPORTO"]);
            string strSQL11 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + num9.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'INFO'";
            objDataAccess.WriteTransactionData(strSQL11, CommandType.Text);
            num3 = Convert.ToDecimal(dataTable4.Rows[0]["IMPORTO"]);
            num7 = IMPINF_RESID - Convert.ToDecimal(dataTable4.Rows[0]["IMPORTO"]);
            str2 = "INF', 'PINF";
          }
          else
          {
            string strSQL12 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + IMPINF_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'INF'";
            objDataAccess.WriteTransactionData(strSQL12, CommandType.Text);
            str2 = "INF";
          }
        }
        else
        {
          string strSQL13 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + IMPINF_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'INFO'";
          if (Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL13, CommandType.Text)) == 0)
          {
            string strSQL14 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + IMPINF_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'INF'";
            int32_2 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL14, CommandType.Text));
            str2 = "INF";
          }
          else
            str2 = "PINF";
        }
      }
      if (IMPFP_RESID > 0M)
      {
        string strSQL15 = "SELECT * FROM SPLABSAP WHERE " + " PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PROGSPLIA = " + int32_1.ToString() + " AND GESTCON = 'FP'";
        dataTable4.Clear();
        dataTable4 = objDataAccess.GetDataTable(strSQL15);
        if (dataTable4.Rows.Count > 0)
        {
          if (Convert.ToDecimal(dataTable4.Rows[0]["IMPORTO"]) < IMPFP_RESID)
          {
            flag = true;
            string strSQL16 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + dataTable4.Rows[0]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'FP'";
            objDataAccess.WriteTransactionData(strSQL16, CommandType.Text);
            num9 = IMPFP_RESID - Convert.ToDecimal(dataTable4.Rows[0]["IMPORTO"]);
            string strSQL17 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + num9.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'FPO'";
            objDataAccess.WriteTransactionData(strSQL17, CommandType.Text);
            num2 = Convert.ToDecimal(dataTable4.Rows[0]["IMPORTO"]);
            num6 = IMPFP_RESID - Convert.ToDecimal(dataTable4.Rows[0]["IMPORTO"]);
            str3 = "FP', 'PFP";
          }
          else
          {
            string strSQL18 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + IMPFP_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'FP'";
            objDataAccess.WriteTransactionData(strSQL18, CommandType.Text);
            str3 = "FP";
          }
        }
        else
        {
          string strSQL19 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + IMPFP_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'FPO'";
          if (Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL19, CommandType.Text)) == 0)
          {
            string strSQL20 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + IMPFP_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'FP'";
            int32_2 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL20, CommandType.Text));
            str3 = "FP";
          }
          else
            str3 = "PFP";
        }
      }
      if (IMPAD_RESID > 0M)
      {
        string strSQL21 = "SELECT * FROM SPLABSAP WHERE " + " PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PROGSPLIA = " + int32_1.ToString() + " AND GESTCON = 'AD'";
        dataTable4.Clear();
        DataTable dataTable5 = objDataAccess.GetDataTable(strSQL21);
        if (dataTable5.Rows.Count > 0)
        {
          if (Convert.ToDecimal(dataTable5.Rows[0]["IMPORTO"]) < IMPAD_RESID)
          {
            flag = true;
            string strSQL22 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + dataTable5.Rows[0]["IMPORTO"].ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'AD'";
            objDataAccess.WriteTransactionData(strSQL22, CommandType.Text);
            num9 = IMPAD_RESID - Convert.ToDecimal(dataTable5.Rows[0]["IMPORTO"]);
            string strSQL23 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + num9.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'ADO'";
            objDataAccess.WriteTransactionData(strSQL23, CommandType.Text);
            num4 = Convert.ToDecimal(dataTable5.Rows[0]["IMPORTO"]);
            num8 = IMPAD_RESID - Convert.ToDecimal(dataTable5.Rows[0]["IMPORTO"]);
            str4 = "AD', 'PAD";
          }
          else
          {
            string strSQL24 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + IMPAD_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'AD'";
            objDataAccess.WriteTransactionData(strSQL24, CommandType.Text);
            str4 = "AD";
          }
        }
        else
        {
          string strSQL25 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + IMPAD_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'ADO'";
          if (Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL25, CommandType.Text)) == 0)
          {
            string strSQL26 = " UPDATE SPLABSAP SET" + " IMPORTO = IMPORTO - (" + IMPAD_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + ROW_VER["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + ROW_VER["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + ROW_VER["PROGABBIN"]?.ToString() + " AND GESTCON = 'AD'";
            int32_2 = Convert.ToInt32(objDataAccess.WriteTransactionData(strSQL26, CommandType.Text));
            str4 = "AD";
          }
          else
            str4 = "PAD";
        }
      }
      string strSQL27 = " SELECT * FROM SPLIMPOSAP WHERE PARTITA  = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOV = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND FLSAP = 'N'";
      dataTable2.Clear();
      DataTable dataTable6 = objDataAccess.GetDataTable(strSQL27);
      string str5 = MOVIMENTO.Trim();
      if (!(str5 == "03") && !(str5 == "71"))
      {
        if (str5 == "09")
        {
          string strSQL28 = "  SELECT VALUE(SUM(IMPORTO),0.0) FROM SPLABSAP " + " WHERE PARTITAA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString().Trim()) + " AND PROGMOVA = " + dataTable6.Rows[0][nameof (PROGMOV)].ToString().Trim() + " AND PROGSPLIA = " + dataTable6.Rows[0]["PROGSPLI"].ToString().Trim();
          Decimal num10 = Convert.ToDecimal(objDataAccess.Get1ValueFromSQL(strSQL28, CommandType.Text));
          string strSQL29 = "SELECT * FROM SPLIMPOSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOV = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND GESTCON <> 'CAZA'";
          dataTable1.Clear();
          dataTable1 = objDataAccess.GetDataTable(strSQL29);
          for (int index = 0; index <= dataTable1.Rows.Count - 1; ++index)
          {
            string str6 = " UPDATE SPLIMPOSAP SET";
            string str7 = Convert.ToInt32(dataTable1.Rows[index]["IMPORTO"]) >= 0 ? str6 + " IMPABB = " + num10.ToString().Replace(',', '.') + ", " : str6 + " IMPABB = (" + num10.ToString().Replace(',', '.') + " * -1), ";
            string strSQL30 = (Convert.ToInt32(dataTable1.Rows[index]["IMPORTO"]) >= 0 ? str7 + " IMPRES = (IMPORTO  - " + num10.ToString().Replace(',', '.') + ") " : str7 + " IMPRES = (IMPORTO  - (" + num10.ToString().Replace(',', '.') + " * - 1)) ") + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable1.Rows[index]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable1.Rows[index][nameof (PROGMOV)].ToString().Trim() + " AND PROGSPLI = " + dataTable1.Rows[index]["PROGSPLI"].ToString().Trim();
            objDataAccess.WriteTransactionData(strSQL30, CommandType.Text);
          }
          string strSQL31 = "  SELECT VALUE(SUM(IMPABB),0.0) AS IMPABB, VALUE(SUM(IMPRIDU),0.0) AS IMPRIDU" + " FROM SPLIMPOSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[0][nameof (PROGMOV)].ToString().Trim() + " AND FLSAP = 'N' ";
          dataTable3.Clear();
          DataTable dataTable7 = objDataAccess.GetDataTable(strSQL31);
          string str8 = " UPDATE MOVIMSAP SET";
          string str9 = Convert.ToInt32(ROW_VER["IMPORTO"]) <= 0 ? str8 + " IMPABB = (" + dataTable7.Rows[0]["IMPABB"].ToString().Replace(',', '.') + " * -1), " : str8 + " IMPABB = " + dataTable7.Rows[0]["IMPABB"].ToString().Replace(',', '.') + ", ";
          string strSQL32 = (Convert.ToInt32(ROW_VER["IMPORTO"]) <= 0 ? str9 + " IMPRESID = (IMPORTO - " + dataTable7.Rows[0]["IMPABB"].ToString().Replace(',', '.') + ")  " : str9 + " IMPRESID = (IMPORTO * - 1 + " + dataTable7.Rows[0]["IMPABB"].ToString().Replace(',', '.') + ") * - 1") + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOV = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND CODCAUS = " + DBMethods.DoublePeakForSql(MOVIMENTO);
          objDataAccess.WriteTransactionData(strSQL32, CommandType.Text);
        }
      }
      else
      {
        for (int index = 0; index <= dataTable6.Rows.Count - 1; ++index)
        {
          switch (dataTable6.Rows[index]["GESTCON"].ToString().Trim())
          {
            case "AD":
            case "PAD":
              string strSQL33 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB + (" + IMPAD_RESID.ToString().Replace(',', '.') + "), " + " IMPRES = IMPRES + (" + IMPAD_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index][nameof (PROGMOV)]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL33, CommandType.Text);
              break;
            case "FP":
            case "PFP":
              string strSQL34 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB + (" + IMPFP_RESID.ToString().Replace(',', '.') + "), " + " IMPRES = IMPRES + (" + IMPFP_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index][nameof (PROGMOV)]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL34, CommandType.Text);
              break;
            case "INF":
            case "PINF":
              string strSQL35 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB + (" + IMPINF_RESID.ToString().Replace(',', '.') + "), " + " IMPRES = IMPRES + (" + IMPINF_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index][nameof (PROGMOV)]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL35, CommandType.Text);
              break;
            case "PTFR":
            case "TFR":
              string strSQL36 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB + (" + IMPTFR_RESID.ToString().Replace(',', '.') + "), " + " IMPRES = IMPRES + (" + IMPTFR_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["PARTITA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index][nameof (PROGMOV)]?.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL36, CommandType.Text);
              break;
          }
        }
        string strSQL37 = "  SELECT VALUE(SUM(IMPABB),0.0) AS IMPABB, VALUE(SUM(IMPRIDU),0.0) AS IMPRIDU" + " FROM SPLIMPOSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["PARTITA"].ToString().Trim()) + " AND PROGMOV = " + dataTable6.Rows[0][nameof (PROGMOV)].ToString().Trim() + " AND FLSAP = 'N' ";
        dataTable3.Clear();
        DataTable dataTable8 = objDataAccess.GetDataTable(strSQL37);
        string str10 = " UPDATE MOVIMSAP SET";
        string str11 = Convert.ToInt32(ROW_VER["IMPORTO"]) <= 0 ? str10 + " IMPABB = (" + dataTable8.Rows[0]["IMPABB"].ToString().Replace(',', '.') + " * -1), " : str10 + " IMPABB = " + dataTable8.Rows[0]["IMPABB"].ToString().Replace(',', '.') + ", ";
        string strSQL38 = (Convert.ToInt32(ROW_VER["IMPORTO"]) <= 0 ? str11 + " IMPRESID = (IMPORTO - " + dataTable8.Rows[0]["IMPABB"].ToString().Replace(',', '.') + ")  " : str11 + " IMPRESID = (IMPORTO * - 1 + " + dataTable8.Rows[0]["IMPABB"].ToString().Replace(',', '.') + ") * - 1") + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(ROW_VER["PARTITAA"].ToString().Trim()) + " AND PROGMOV = " + ROW_VER["PROGMOVA"].ToString().Trim() + " AND CODCAUS = " + DBMethods.DoublePeakForSql(MOVIMENTO);
        objDataAccess.WriteTransactionData(strSQL38, CommandType.Text);
      }
      string str12 = " UPDATE MOVIMSAP SET" + " IMPABB = IMPABB - (" + IMPRET_RESID.ToString().Replace(',', '.') + "), ";
      if (MOVIMENTO.Trim() == "71")
        str12 = str12 + " IMPRIDUZ = IMPRIDUZ - (" + IMPRET_RESID.ToString().Replace(',', '.') + "), ";
      string strSQL39 = str12 + " IMPRESID = IMPRESID + (" + IMPRET_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString();
      objDataAccess.WriteTransactionData(strSQL39, CommandType.Text);
      string strSQL40 = "SELECT * FROM SPLIMPOSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON IN ('" + str1 + "', '" + str2 + "', '" + str3 + "', '" + str4 + "') ";
      dataTable1.Clear();
      DataTable dataTable9 = objDataAccess.GetDataTable(strSQL40);
      if (flag)
      {
        for (int index = 0; index <= dataTable9.Rows.Count - 1; ++index)
        {
          switch (dataTable9.Rows[index]["GESTCON"].ToString().Trim())
          {
            case "AD":
            case "PAD":
              if (dataTable9.Rows[index]["GESTCON"].ToString().Trim() == "AD")
              {
                string str13 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + num4.ToString().Replace(',', '.') + "), ";
                if (MOVIMENTO.Trim() == "71")
                  str13 = str13 + " IMPRIDU = IMPRIDU - (" + num4.ToString().Replace(',', '.') + "), ";
                string strSQL41 = str13 + " IMPRES = IMPRES + (" + num4.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable9.Rows[index]["GESTCON"].ToString().Trim());
                objDataAccess.WriteTransactionData(strSQL41, CommandType.Text);
                break;
              }
              string str14 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + num8.ToString().Replace(',', '.') + "), ";
              if (MOVIMENTO.Trim() == "71")
                str14 = str14 + " IMPRIDU = IMPRIDU - (" + num8.ToString().Replace(',', '.') + "), ";
              string strSQL42 = str14 + " IMPRES = IMPRES + (" + num8.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable9.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL42, CommandType.Text);
              break;
            case "FP":
            case "PFP":
              if (dataTable9.Rows[index]["GESTCON"].ToString().Trim() == "FP")
              {
                string str15 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + num2.ToString().Replace(',', '.') + "), ";
                if (MOVIMENTO.Trim() == "71")
                  str15 = str15 + " IMPRIDU = IMPRIDU - (" + num2.ToString().Replace(',', '.') + "), ";
                string strSQL43 = str15 + " IMPRES = IMPRES + (" + num2.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable9.Rows[index]["GESTCON"].ToString().Trim());
                objDataAccess.WriteTransactionData(strSQL43, CommandType.Text);
                break;
              }
              string str16 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + num6.ToString().Replace(',', '.') + "), ";
              if (MOVIMENTO.Trim() == "71")
                str16 = str16 + " IMPRIDU = IMPRIDU - (" + num6.ToString().Replace(',', '.') + "), ";
              string strSQL44 = str16 + " IMPRES = IMPRES + (" + num6.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable9.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL44, CommandType.Text);
              break;
            case "INF":
            case "PINF":
              if (dataTable9.Rows[index]["GESTCON"].ToString().Trim() == "INF")
              {
                string str17 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + num3.ToString().Replace(',', '.') + "), ";
                if (MOVIMENTO.Trim() == "71")
                  str17 = str17 + " IMPRIDU = IMPRIDU - (" + num3.ToString().Replace(',', '.') + "), ";
                string strSQL45 = str17 + " IMPRES = IMPRES + (" + num3.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable9.Rows[index]["GESTCON"].ToString().Trim());
                objDataAccess.WriteTransactionData(strSQL45, CommandType.Text);
                break;
              }
              string str18 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + num7.ToString().Replace(',', '.') + "), ";
              if (MOVIMENTO.Trim() == "71")
                str18 = str18 + " IMPRIDU = IMPRIDU - (" + num7.ToString().Replace(',', '.') + "), ";
              string strSQL46 = str18 + " IMPRES = IMPRES + (" + num7.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable9.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL46, CommandType.Text);
              break;
            case "PTFR":
            case "TFR":
              if (dataTable9.Rows[index]["GESTCON"].ToString().Trim() == "TFR")
              {
                string str19 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + num1.ToString().Replace(',', '.') + "), ";
                if (MOVIMENTO.Trim() == "71")
                  str19 = str19 + " IMPRIDU = IMPRIDU - (" + num1.ToString().Replace(',', '.') + "), ";
                string strSQL47 = str19 + " IMPRES = IMPRES + (" + num1.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable9.Rows[index]["GESTCON"].ToString().Trim());
                objDataAccess.WriteTransactionData(strSQL47, CommandType.Text);
                break;
              }
              string str20 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + num5.ToString().Replace(',', '.') + "), ";
              if (MOVIMENTO.Trim() == "71")
                str20 = str20 + " IMPRIDU = IMPRIDU - (" + num5.ToString().Replace(',', '.') + "), ";
              string strSQL48 = str20 + " IMPRES = IMPRES + (" + num5.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable9.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL48, CommandType.Text);
              break;
          }
        }
      }
      else
      {
        for (int index = 0; index <= dataTable9.Rows.Count - 1; ++index)
        {
          switch (dataTable9.Rows[index]["GESTCON"].ToString().Trim())
          {
            case "AD":
            case "PAD":
              string str21 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + IMPAD_RESID.ToString().Replace(',', '.') + "), ";
              if (MOVIMENTO.Trim() == "71")
                str21 = str21 + " IMPRIDU = IMPRIDU - (" + IMPAD_RESID.ToString().Replace(',', '.') + "), ";
              string strSQL49 = str21 + " IMPRES = IMPRES + (" + IMPAD_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable9.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL49, CommandType.Text);
              break;
            case "FP":
            case "PFP":
              string str22 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + IMPFP_RESID.ToString().Replace(',', '.') + "), ";
              if (MOVIMENTO.Trim() == "71")
                str22 = str22 + " IMPRIDU = IMPRIDU - (" + IMPFP_RESID.ToString().Replace(',', '.') + "), ";
              string strSQL50 = str22 + " IMPRES = IMPRES + (" + IMPFP_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable9.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL50, CommandType.Text);
              break;
            case "INF":
            case "PINF":
              string str23 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + IMPINF_RESID.ToString().Replace(',', '.') + "), ";
              if (MOVIMENTO.Trim() == "71")
                str23 = str23 + " IMPRIDU = IMPRIDU - (" + IMPINF_RESID.ToString().Replace(',', '.') + "), ";
              string strSQL51 = str23 + " IMPRES = IMPRES + (" + IMPINF_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable9.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL51, CommandType.Text);
              break;
            case "PTFR":
            case "TFR":
              string str24 = " UPDATE SPLIMPOSAP SET" + " IMPABB = IMPABB - (" + IMPTFR_RESID.ToString().Replace(',', '.') + "), ";
              if (MOVIMENTO.Trim() == "71")
                str24 = str24 + " IMPRIDU = IMPRIDU - (" + IMPTFR_RESID.ToString().Replace(',', '.') + "), ";
              string strSQL52 = str24 + " IMPRES = IMPRES + (" + IMPTFR_RESID.ToString().Replace(',', '.') + ") " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITAMOV) + " AND PROGMOV = " + PROGMOV.ToString() + " AND GESTCON = " + DBMethods.DoublePeakForSql(dataTable9.Rows[index]["GESTCON"].ToString().Trim());
              objDataAccess.WriteTransactionData(strSQL52, CommandType.Text);
              break;
          }
        }
      }
    }

    private static void MODULE_PULISCI_CONTABILITA(
      DataLayer objDataAccess,
      string PARTITAMOV,
      int PROGMOV,
      string PARTITARET,
      int PROGMOVRET,
      Decimal IMPORTO_RETTIFICA,
      Decimal IMPORTO_MOVIMENTO,
      bool FLGMULTIPLE,
      Decimal IMPTFR_RETTIFICA = 0.0M,
      Decimal IMPFP_RETTIFICA = 0.0M,
      Decimal IMPINF_RETTIFICA = 0.0M,
      Decimal IMPAD_RETTIFICA = 0.0M)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      DataTable dataTable4 = new DataTable();
      Decimal num1 = 0.0M;
      Decimal num2 = 0.0M;
      Decimal num3 = 0.0M;
      Decimal num4 = 0.0M;
      Decimal IMPTFR_AB = 0.0M;
      Decimal IMPFP_AB = 0.0M;
      Decimal IMPINF_AB = 0.0M;
      Decimal IMPAD_AB = 0.0M;
      Decimal IMPAC_AB = 0.0M;
      Decimal IMPAB_AB = 0.0M;
      Decimal num5 = 0.0M;
      Decimal num6 = 0.0M;
      Decimal num7 = 0.0M;
      Decimal num8 = 0.0M;
      Decimal IMPRET_RESID;
      Decimal num9;
      Decimal num10;
      Decimal num11;
      Decimal num12;
      if (FLGMULTIPLE)
      {
        IMPRET_RESID = IMPORTO_RETTIFICA;
        num9 = IMPTFR_RETTIFICA;
        num10 = IMPFP_RETTIFICA;
        num11 = IMPINF_RETTIFICA;
        num12 = IMPAD_RETTIFICA;
        num5 = IMPTFR_RETTIFICA;
        num6 = IMPFP_RETTIFICA;
        num7 = IMPINF_RETTIFICA;
        num8 = IMPAD_RETTIFICA;
      }
      else
      {
        string strSQL1 = "SELECT * FROM SPLIMPOSAP  " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITARET) + " AND PROGMOV = " + PROGMOVRET.ToString() + " AND GESTCON <> 'CAZA'";
        dataTable4.Clear();
        DataTable dataTable5 = objDataAccess.GetDataTable(strSQL1);
        if (dataTable5.Rows.Count > 0)
        {
          for (int index = 0; index <= dataTable5.Rows.Count - 1; ++index)
          {
            switch (dataTable5.Rows[index]["GESTCON"].ToString().Trim())
            {
              case "AD":
              case "PAD":
              case "RAD":
                num4 = (Decimal) (Convert.ToInt32(dataTable5.Rows[index]["IMPORTO"]) * -1);
                break;
              case "FP":
              case "PFP":
              case "RFP":
                num2 = (Decimal) (Convert.ToInt32(dataTable5.Rows[index]["IMPORTO"]) * -1);
                break;
              case "INF":
              case "PINF":
              case "RINF":
                num3 = (Decimal) (Convert.ToInt32(dataTable5.Rows[index]["IMPORTO"]) * -1);
                break;
              case "PTFR":
              case "RTFR":
              case "TFR":
                num1 = (Decimal) (Convert.ToInt32(dataTable5.Rows[index]["IMPORTO"]) * -1);
                break;
            }
          }
        }
        string strSQL2 = "SELECT VALUE(IMPORTO,0.0) FROM MOVIMSAP  " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITARET) + " AND PROGMOV = " + PROGMOVRET.ToString();
        IMPRET_RESID = Convert.ToDecimal(objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text)) * -1M;
        num9 = !(num1 > 0M) ? 0M : num1;
        num10 = !(num2 > 0M) ? 0M : num2;
        num11 = !(num3 > 0M) ? 0M : num3;
        num12 = !(num4 > 0M) ? 0M : num4;
      }
      string strSQL3 = "SELECT A.PARTITAD, A.PROGMOVD, A.PARTITAA, A.PROGMOVA, A.PROGABBIN, A.IMPORTO, B.CODCAUS " + " FROM ABBINSAP A, MOVIMSAP B" + " WHERE A.PARTITAA = B.PARTITA" + " AND A.PROGMOVA = B.PROGMOV" + " AND B.CODCAUS IN ('09', '03', '71')" + " AND A.PARTITAD = " + DBMethods.DoublePeakForSql(PARTITAMOV.ToString().Trim()) + " AND A.PROGMOVD = " + PROGMOV.ToString() + " AND A.STATOVAL = 'V' ORDER BY B.CODCAUS ASC, A.IMPORTO ASC";
      dataTable1.Clear();
      DataTable dataTable6 = objDataAccess.GetDataTable(strSQL3);
      for (int index = 0; index <= dataTable6.Rows.Count - 1; ++index)
      {
        string MOVIMENTO = dataTable6.Rows[index]["CODCAUS"].ToString().Trim();
        string strSQL4 = "SELECT COUNT(*) FROM MOVASSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["PARTITAA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index]["PROGMOVA"]?.ToString() + " AND PARTASS = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["PARTITAD"].ToString()) + " AND PROGRASS = " + dataTable6.Rows[index]["PROGMOVD"]?.ToString();
        if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL4, CommandType.Text)) == 0)
        {
          if (FLGMULTIPLE)
          {
            Decimal num13 = IMPRET_RESID - Convert.ToDecimal(dataTable6.Rows[index]["IMPORTO"]);
            if (IMPRET_RESID - Convert.ToDecimal(dataTable6.Rows[index]["IMPORTO"]) == 0M)
              ModContabilita.DELETE_VERSAMENTO(objDataAccess, PARTITAMOV, PROGMOV, dataTable6.Rows[index], ref IMPTFR_AB, ref IMPFP_AB, ref IMPINF_AB, ref IMPAD_AB, ref IMPAC_AB, ref IMPAB_AB, MOVIMENTO, num9, num10, num11, num12);
            else if (IMPRET_RESID - Convert.ToDecimal(dataTable6.Rows[index]["IMPORTO"]) > 0M)
              ModContabilita.DELETE_VERSAMENTO(objDataAccess, PARTITAMOV, PROGMOV, dataTable6.Rows[index], ref IMPTFR_AB, ref IMPFP_AB, ref IMPINF_AB, ref IMPAD_AB, ref IMPAC_AB, ref IMPAB_AB, MOVIMENTO, num9, num10, num11, num12);
            else if (IMPRET_RESID - Convert.ToDecimal(dataTable6.Rows[index]["IMPORTO"]) < 0M)
              ModContabilita.UPDATE_VERSAMENTO(objDataAccess, PARTITAMOV, PROGMOV, dataTable6.Rows[index], IMPRET_RESID, num9, num10, num11, num12, MOVIMENTO);
          }
          else
          {
            Decimal num14 = IMPRET_RESID - Convert.ToDecimal(dataTable6.Rows[index]["IMPORTO"]);
            if (IMPRET_RESID - Convert.ToDecimal(dataTable6.Rows[index]["IMPORTO"]) == 0M)
              ModContabilita.DELETE_VERSAMENTO(objDataAccess, PARTITAMOV, PROGMOV, dataTable6.Rows[index], ref IMPTFR_AB, ref IMPFP_AB, ref IMPINF_AB, ref IMPAD_AB, ref IMPAC_AB, ref IMPAB_AB, MOVIMENTO, num9, num10, num11, num12);
            else if (IMPRET_RESID - Convert.ToDecimal(dataTable6.Rows[index]["IMPORTO"]) > 0M)
              ModContabilita.DELETE_VERSAMENTO(objDataAccess, PARTITAMOV, PROGMOV, dataTable6.Rows[index], ref IMPTFR_AB, ref IMPFP_AB, ref IMPINF_AB, ref IMPAD_AB, ref IMPAC_AB, ref IMPAB_AB, MOVIMENTO, num9, num10, num11, num12);
            else if (IMPRET_RESID - Convert.ToDecimal(dataTable6.Rows[index]["IMPORTO"]) < 0M)
              ModContabilita.UPDATE_VERSAMENTO(objDataAccess, PARTITAMOV, PROGMOV, dataTable6.Rows[index], IMPRET_RESID, num9, num10, num11, num12, MOVIMENTO);
          }
          if (FLGMULTIPLE)
          {
            Decimal num15 = Convert.ToDecimal(dataTable6.Rows[index]["IMPORTO"]) - IMPAB_AB - IMPAC_AB;
            if (num5 == 0M)
              num15 -= IMPTFR_AB;
            else
              num9 -= IMPTFR_AB;
            if (num6 == 0M)
              num15 -= IMPFP_AB;
            else
              num10 -= IMPFP_AB;
            if (num7 == 0M)
              num15 -= IMPINF_AB;
            else
              num11 -= IMPINF_AB;
            if (num8 == 0M)
              num15 -= IMPAD_AB;
            else
              num12 -= IMPAD_AB;
            IMPRET_RESID -= num15;
          }
          else
          {
            Decimal num16 = Convert.ToDecimal(dataTable6.Rows[index]["IMPORTO"]) - IMPAB_AB - IMPAC_AB;
            if (num1 == 0M)
              num16 -= IMPTFR_AB;
            else
              num9 -= IMPTFR_AB;
            if (num2 == 0M)
              num16 -= IMPFP_AB;
            else
              num10 -= IMPFP_AB;
            if (num3 == 0M)
              num16 -= IMPINF_AB;
            else
              num11 -= IMPINF_AB;
            if (num4 == 0M)
              num16 -= IMPAD_AB;
            else
              num12 -= IMPAD_AB;
            IMPRET_RESID -= num16;
          }
        }
      }
      for (int index = 0; index <= dataTable6.Rows.Count - 1; ++index)
      {
        string strSQL5 = "SELECT COUNT(*) FROM MOVASSAP " + " WHERE PARTITA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["PARTITAA"].ToString()) + " AND PROGMOV = " + dataTable6.Rows[index]["PROGMOVA"]?.ToString() + " AND PARTASS = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["PARTITAD"].ToString()) + " AND PROGRASS = " + dataTable6.Rows[index]["PROGMOVD"]?.ToString();
        if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL5, CommandType.Text)) == 0)
        {
          string strSQL6 = "DELETE FROM SPLABSAP " + " WHERE PARTITAD = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["PARTITAD"].ToString().Trim()) + " AND PROGMOVD = " + dataTable6.Rows[index]["PROGMOVD"]?.ToString() + " AND PARTITAA = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["PARTITAA"].ToString().Trim()) + " AND PROGMOVA  = " + dataTable6.Rows[index]["PROGMOVA"]?.ToString() + " AND PROGABBIN  = " + dataTable6.Rows[index]["PROGABBIN"]?.ToString() + " AND IMPORTO = 0";
          objDataAccess.WriteTransactionData(strSQL6, CommandType.Text);
        }
      }
    }
  }
}
