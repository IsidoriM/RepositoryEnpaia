// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.GeneraDenunciaDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;
using Utilities;

namespace TFI.DAL.AziendaConsulente
{
  public class GeneraDenunciaDAL
  {
    private static readonly ILog log = LogManager.GetLogger("RollingFile");
    private static readonly ILog TrackLog = LogManager.GetLogger("Track");
    private DateTimeFormatInfo objDTFI = new CultureInfo("it-IT", false).DateTimeFormat;
    public static DataView objDvPargen = new DataView();
    private DataView objDvAliquote = new DataView();
    private DataView objDvPerFap = new DataView();
    private DenunciaArretrati listaDenuncia = new DenunciaArretrati();

    public List<DenunciaArretrati> GeneraDenunciaArr(
      string strDataDal,
      string strDataAl,
      ref List<ParametriGenerali> listaParametriGen,
      string strCodPos = "",
      string strMat = "",
      bool blnArretrato = false)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      DataTable dataTable4 = new DataTable();
      DataView dataView1 = new DataView();
      DataView dataView2 = new DataView();
      DataView dataView3 = new DataView();
      DataView dataView4 = new DataView();
      DataView dataView5 = new DataView();
      DataTable dataTable5 = new DataTable();
      DataTable dataTable6 = new DataTable();
      int num1 = 0;
      int num2 = 0;
      Decimal num3 = 0M;
      Decimal num4 = 0M;
      Decimal num5 = 0M;
      string str1 = "";
      string str2 = "";
      int num6 = 0;
      DataLayer dataLayer = new DataLayer();
      try
      {
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "CODPOS"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "MAT"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "CODFIS"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "NOME"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "PRORAP"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "TIPRAP"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "DATNAS"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "DATDEC"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "DATCES"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "DAL",
          DataType = Type.GetType("System.DateTime")
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "AL",
          DataType = Type.GetType("System.DateTime")
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "ETA65"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "CODCON"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "PROCON"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "CODLOC"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "PROLOC"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "TIPSPE"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "CODLIV"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "LIVELLO"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "QUALIFICA"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "CODQUACON"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "CODGRUASS"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "ALIQUOTA"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPTRAECO"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPMIN"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPRET"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPOCC"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "NUMGGAZI"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "NUMGGFIG"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "NUMGGDOM"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "NUMGGSOS"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "NUMGGCONAZI"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "NUMGGPER"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPFIG"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "CODSOS",
          DefaultValue = (object) ""
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPCON"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPSCA"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "DATERO"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "FAP"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "PERFAP"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPFAP"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "PERPAR"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "PERAPP"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "ABBPRE"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "ASSCON"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPABB"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPASSCON"
        });
        DataColumn column = new DataColumn();
        column.ColumnName = "ANNCOM";
        if (blnArretrato)
          column.DefaultValue = (object) DateTime.Parse(strDataDal).Year;
        dataTable1.Columns.Add(column);
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "PREV",
          DefaultValue = (object) "N"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "PROMOD",
          DefaultValue = (object) 0
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "MOD"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "FLAG"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "RIMUOVI"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "PRODENDET"
        });
        string dateTimeFormat = DateTime.Today.GetDateTimeFormats((IFormatProvider) this.objDTFI)[0];
        string str3 = "";
        int CODQUACON = 0;
        int num7 = 0;
        string str4 = "";
        Convert.ToDecimal(0.0);
        Convert.ToDecimal(0.0);
        Convert.ToDecimal(0.0);
        Decimal num8 = Convert.ToDecimal(0.0);
        Convert.ToDecimal(0.0);
        Decimal PERFAP = Convert.ToDecimal(0.0);
        Decimal num9 = Convert.ToDecimal(0.0);
        Convert.ToDecimal(0.0);
        Convert.ToDecimal(0.0);
        Convert.ToDecimal(0.0);
        string str5 = "";
        int num10 = 0;
        int? outcome = new int?();
        DateTime dateTime = DateTime.Parse(strDataDal);
        int month1 = dateTime.Month;
        dateTime = DateTime.Parse(strDataAl);
        int month2 = dateTime.Month;
        if (month1 == month2)
        {
          dateTime = DateTime.Parse(strDataDal);
          int year = dateTime.Year;
          dateTime = DateTime.Parse(strDataDal);
          int month3 = dateTime.Month;
          num10 = DateTime.DaysInMonth(year, month3);
        }
        string strSQL1 = "SELECT A.CODCON, A.PROCON, A.CODQUACON, A.DATINI, A.DATFIN, A.DATDEC, B.DENQUA, " + "C.CODLIV, C.DENLIV, A.TIPSPE FROM CONRIF A INNER JOIN QUACON B ON A.CODQUACON = " + "B.CODQUACON INNER JOIN CONLIV C ON A.CODCON = C.CODCON AND A.PROCON = C.PROCON";
        DataView dataView6 = dataLayer.GetDataView(strSQL1);
        dataView6.Sort = "DATDEC DESC";
        string strSQL2 = "SELECT CODLOC, PROLOC, DATINI, DATFIN, DATDEC, TIPSPE FROM CONLOC";
        DataView dataView7 = dataLayer.GetDataView(strSQL2);
        dataView7.Sort = "DATDEC DESC";
        string strSQL3 = "SELECT CODCON, PROCON, CODLIV, DATAPPINI, DATAPPFIN, IMPVOCRET FROM " + "CONRET WHERE CODVOCRET <> 4";
        DataView dataView8 = dataLayer.GetDataView(strSQL3);
        dataView8.Sort = "DATAPPINI DESC";
        string strSQL4 = "SELECT CODLOC, PROLOC, CODLIV, DATAPPINI, DATAPPFIN, IMPVOCRET FROM " + "LOCRET WHERE CODVOCRET <> 4";
        DataView dataView9 = dataLayer.GetDataView(strSQL4);
        dataView9.Sort = "DATAPPINI DESC";
        this.CaricaAliquote();
        string strSQL5 = "SELECT COUNT(CODPOS) FROM PARGENPOS WHERE CODPOS = " + strCodPos;
        string strSQL6 = Convert.ToInt32("0" + dataLayer.Get1ValueFromSQL(strSQL5, CommandType.Text)) != 0 ? "SELECT CODPAR, VALORE, DATINI, DATFIN FROM PARGENDET WHERE CODPAR IN (1, 4)" + " UNION " + "SELECT 5 AS CODPAR, char(VALORE)  AS VALORE, DATINI, DATFIN FROM PARGENPOS WHERE CODPOS = " + strCodPos : "SELECT CODPAR, VALORE, DATINI, DATFIN FROM PARGENDET WHERE CODPAR IN (1, 4, 5)";
        GeneraDenunciaDAL.objDvPargen = dataLayer.GetDataView(strSQL6);
        GeneraDenunciaDAL.objDvPargen.Sort = "DATINI DESC";
        listaParametriGen = DenunciaMensileDAL.GetParametriGenerali(strCodPos, out outcome);
        string strSQL7 = "SELECT TRIM(A.COG) || ' ' || TRIM(A.NOM) AS NOME, A.CODFIS, A.DATNAS, B.DATDEC, B.DATCES, " + "B.CODPOS, A.MAT, C.FAP, B.PRORAP, C.TIPRAP, C.CODGRUASS, DATINI, DATFIN, VALUE(C.CODLOC, 0) " + "AS CODLOC, VALUE(C.CODCON, 0) AS CODCON, C.CODLIV, 0 AS CODQUACON, 0 AS PROCON, 0 AS " + "PROLOC, TRAECO, PERPAR, PERAPP, C.CODGRUASS, C.FAP, VALUE(PERFAP, 0) AS PERFAP, C.ABBPRE, " + "C.ASSCON, IMPSCAMAT FROM ISCT A INNER JOIN RAPLAV B ON A.MAT = B.MAT INNER JOIN STORDL " + "C ON B.CODPOS = C.CODPOS AND B.MAT = C.MAT AND B.PRORAP = C.PRORAP WHERE B.CODPOS = " + strCodPos + " AND '" + DBMethods.Db2Date(strDataDal) + "' <= VALUE(B.DATCES , '9999-12-31') " + "AND '" + DBMethods.Db2Date(strDataAl) + "' >= B.DATDEC " + "AND '" + DBMethods.Db2Date(strDataDal) + "' <= VALUE(C.DATFIN , '9999-12-31') " + "AND '" + DBMethods.Db2Date(strDataAl) + "' >= C.DATINI AND VALUE(B.CODCAUCES, 0) <> 50 " + "ORDER BY NOME, A.MAT, C.DATINI, C.DATFIN";
        dataTable2.Clear();
        DataTable dataTable7 = dataLayer.GetDataTable(strSQL7);
        string strSQL8 = " SELECT MAT, PRORAP, DATINISOS, DATFINSOS, PERAZI, PERFIG, CODSOS FROM SOSRAP" + " WHERE CODPOS = " + strCodPos + " AND '" + DBMethods.Db2Date(strDataDal) + "' <= VALUE(DATFINSOS , '9999-12-31') " + " AND '" + DBMethods.Db2Date(strDataAl) + "' >= DATINISOS " + " AND STASOS = '0'" + " ORDER BY MAT, PRORAP, DATINISOS";
        DataView dataView10 = dataLayer.GetDataView(strSQL8);
        for (int index = 0; index <= dataTable7.Rows.Count - 1; ++index)
        {
          string codFis = dataTable7.Rows[index]["CODFIS"].ToString().Trim();
          int int32_1 = Convert.ToInt32(dataTable7.Rows[index]["CODPOS"]);
          int matricola = Convert.ToInt32(dataTable7.Rows[index]["MAT"]);
          int proRap = Convert.ToInt32(dataTable7.Rows[index]["PRORAP"]);
          string str6 = dataTable7.Rows[index]["NOME"].ToString();
          if (!DBNull.Value.Equals(dataTable7.Rows[index]["DATNAS"]))
          {
            dateTime = Convert.ToDateTime(Convert.ToDateTime(dataTable7.Rows[index]["DATNAS"]));
            string str7 = dateTime.ToString();
            dateTime = Convert.ToDateTime(str7);
            dateTime = Convert.ToDateTime(dateTime.AddYears(65));
            string s1 = dateTime.ToString();
            if (!DBNull.Value.Equals(dataTable7.Rows[index]["DATDEC"]))
            {
              string str8 = dataTable7.Rows[index]["DATDEC"].ToString();
              string str9 = "N";
              string s2;
              if (!DBNull.Value.Equals(dataTable7.Rows[index]["DATCES"]))
              {
                dateTime = Convert.ToDateTime(dataTable7.Rows[index]["DATCES"]);
                s2 = dateTime.ToString().Substring(0, 10);
                dateTime = DateTime.Parse(s2);
                int year = dateTime.Year;
                dateTime = DateTime.Parse(strDataDal);
                int num11 = dateTime.Year - 1;
                if (year >= num11)
                  str9 = "S";
              }
              else
                s2 = "";
              string str10;
              if (DateTime.Compare(DateTime.Parse(strDataDal), DateTime.Parse(dataTable7.Rows[index]["DATINI"].ToString())) >= 0)
              {
                dateTime = Convert.ToDateTime(strDataDal);
                str10 = dateTime.ToString().Substring(0, 10);
              }
              else
              {
                dateTime = Convert.ToDateTime(dataTable7.Rows[index]["DATINI"]);
                str10 = dateTime.ToString().Substring(0, 10);
              }
              string s3;
              if (DateTime.Compare(DateTime.Parse(strDataAl), DateTime.Parse(dataTable7.Rows[index]["DATFIN"].ToString())) <= 0)
              {
                dateTime = Convert.ToDateTime(strDataAl);
                s3 = dateTime.ToString().Substring(0, 10);
                if (s2 != "" && DateTime.Compare(DateTime.Parse(s2), DateTime.Parse(strDataAl)) <= 0)
                  s3 = s2;
              }
              else
              {
                dateTime = Convert.ToDateTime(dataTable7.Rows[index]["DATFIN"]);
                s3 = dateTime.ToString().Substring(0, 10);
              }
              int int32_4 = Convert.ToInt32("0" + dataTable7.Rows[index]["TIPRAP"]?.ToString());
              int int32_5 = Convert.ToInt32("0" + dataTable7.Rows[index]["CODCON"]?.ToString());
              int int32_6 = Convert.ToInt32("0" + dataTable7.Rows[index]["CODLOC"]?.ToString());
              int int32_7 = Convert.ToInt32("0" + dataTable7.Rows[index]["CODLIV"]?.ToString());
              int int32_8 = Convert.ToInt32("0" + dataTable7.Rows[index]["CODGRUASS"]?.ToString());
              Decimal num12 = Convert.ToDecimal("0" + dataTable7.Rows[index]["PERPAR"]?.ToString());
              Decimal num13 = Convert.ToDecimal("0" + dataTable7.Rows[index]["PERAPP"]?.ToString());
              string str11 = dataTable7.Rows[index]["FAP"]?.ToString() ?? "";
              string strFlag1 = dataTable7.Rows[index]["ABBPRE"]?.ToString() ?? "";
              string strFlag2 = dataTable7.Rows[index]["ASSCON"]?.ToString() ?? "";
              if (int32_5 != 0)
              {
                string str12 = "CODCON = " + int32_5.ToString() + " AND DATDEC <= '" + DBMethods.Db2Date(str10) + "' " + " AND CODLIV = " + int32_7.ToString();
                dataView6.RowFilter = "";
                dataView6.Sort = "DATDEC DESC";
                dataView6.RowFilter = str12;
                if (int32_6 != 0)
                {
                  string str13 = "CODLOC = " + int32_6.ToString() + " AND DATDEC <= '" + DBMethods.Db2Date(str10) + "'";
                  dataView7.RowFilter = "";
                  dataView7.Sort = "DATDEC DESC";
                  dataView7.RowFilter = str13;
                }
                Decimal d = 0M;
                if (dataView6.Count > 0)
                {
                  str3 = dataView6[0]["DENQUA"].ToString();
                  CODQUACON = Convert.ToInt32(dataView6[0]["CODQUACON"]);
                  num7 = Convert.ToInt32(dataView6[0]["PROCON"]);
                  str4 = dataView6[0]["DENLIV"].ToString();
                  str5 = dataView6[0]["TIPSPE"].ToString();
                  if (dataView6[0]["TIPSPE"].ToString() == "S")
                  {
                    string str14 = "CODCON = " + int32_5.ToString() + " AND PROCON = " + dataView6[0]["PROCON"]?.ToString() + " AND CODLIV = " + int32_7.ToString() + " AND DATAPPINI <= '" + DBMethods.Db2Date(str10) + "' " + " AND DATAPPFIN >= '" + DBMethods.Db2Date(str10) + "' ";
                    dataView8.RowFilter = "";
                    dataView8.RowFilter = str14;
                    for (int recordIndex = 0; recordIndex <= dataView8.Count - 1; ++recordIndex)
                      d += Convert.ToDecimal(dataView8[recordIndex]["IMPVOCRET"]);
                    num8 = Convert.ToDecimal("0" + dataTable7.Rows[index]["IMPSCAMAT"]?.ToString());
                  }
                  else
                    num9 = Convert.ToDecimal("0" + dataTable7.Rows[index]["TRAECO"]?.ToString());
                }
                int num14;
                if (int32_6 != 0)
                {
                  num14 = Convert.ToInt32(dataView7[0]["PROLOC"]);
                  if (dataView7[0]["TIPSPE"].ToString() == "S")
                  {
                    string str15 = "CODLOC = " + int32_6.ToString() + " AND PROLOC = " + dataView7[0]["PROLOC"]?.ToString() + " AND CODLIV = " + int32_7.ToString() + " AND DATAPPINI <= '" + DBMethods.Db2Date(str10) + "' " + " AND DATAPPFIN >= '" + DBMethods.Db2Date(str10) + "' ";
                    dataView9.RowFilter = "";
                    dataView9.RowFilter = str15;
                    for (int recordIndex = 0; recordIndex <= dataView9.Count - 1; ++recordIndex)
                      d += Convert.ToDecimal(dataView9[recordIndex]["IMPVOCRET"]);
                  }
                }
                else
                  num14 = 0;
                if (Convert.ToDecimal("0" + dataTable7.Rows[index]["PERAPP"]?.ToString()) > 0M)
                  d = d / 100M * Convert.ToDecimal(dataTable7.Rows[index]["PERAPP"]);
                if (Convert.ToDecimal("0" + dataTable7.Rows[index]["PERPAR"]?.ToString()) > 0M)
                  d = d / 100M * Convert.ToDecimal(dataTable7.Rows[index]["PERPAR"]);
                Decimal num15 = Decimal.Round(d, 2);
                if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(str10)) > 0 & DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(s3)) < 0)
                {
                  dataTable1.Rows.Add(dataTable1.NewRow());
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"] = (object) str10;
                  DataRow row1 = dataTable1.Rows[dataTable1.Rows.Count - 1];
                  dateTime = Convert.ToDateTime(s1);
                  dateTime = dateTime.AddDays(-1.0);
                  string str16 = dateTime.ToString().Substring(0, 10);
                  row1["AL"] = (object) str16;
                  var proDenDet = GetProDenDet(dataTable1, matricola, proRap);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PRODENDET"] = proDenDet;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"] = (object) "N";
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPRAP"] = (object) int32_4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCON"] = (object) int32_5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROCON"] = (object) num7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLOC"] = (object) int32_6;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROLOC"] = (object) num14;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLIV"] = (object) int32_7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["LIVELLO"] = (object) str4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["QUALIFICA"] = (object) str3;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODQUACON"] = (object) CODQUACON;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERPAR"] = (object) num12;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERAPP"] = (object) num13;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) num15;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) num9;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) num8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["FAP"] = (object) str11;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ASSCON"] = (object) strFlag2;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ABBPRE"] = (object) strFlag1;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) this.GetImportoParametro((short) 1, str10, strFlag1);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) this.GetImportoParametro((short) 4, str10, strFlag2);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPSPE"] = (object) str5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODGRUASS"] = (object) int32_8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.GetAliquota(str10, CODQUACON, int32_8, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"].ToString(), dataTable7.Rows[index]["FAP"].ToString(), ref PERFAP);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERFAP"] = (object) PERFAP;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODPOS"] = (object) int32_1;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["MAT"] = (object) matricola;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODFIS"] = codFis;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PRORAP"] = (object) proRap;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["NOME"] = (object) str6;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["DATNAS"] = (object) str7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["DATDEC"] = (object) str8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["DATCES"] = (object) s2;
                  dataTable1.Rows.Add(dataTable1.NewRow());
                  DataRow row2 = dataTable1.Rows[dataTable1.Rows.Count - 1];
                  dateTime = Convert.ToDateTime(s1);
                  string str17 = dateTime.ToString().Substring(0, 10);
                  row2["DAL"] = (object) str17;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s3;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PRODENDET"] = proDenDet + 1;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"] = (object) "S";
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPRAP"] = (object) int32_4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCON"] = (object) int32_5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROCON"] = (object) num7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLOC"] = (object) int32_6;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROLOC"] = (object) num14;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLIV"] = (object) int32_7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["LIVELLO"] = (object) str4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["QUALIFICA"] = (object) str3;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODQUACON"] = (object) CODQUACON;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERPAR"] = (object) num12;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERAPP"] = (object) num13;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) num15;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) num9;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) num8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["FAP"] = (object) str11;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ASSCON"] = (object) strFlag2;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ABBPRE"] = (object) strFlag1;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) this.GetImportoParametro((short) 1, str10, strFlag1);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) this.GetImportoParametro((short) 4, str10, strFlag2);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPSPE"] = (object) str5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODGRUASS"] = (object) int32_8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.GetAliquota(str10, CODQUACON, int32_8, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"].ToString(), dataTable7.Rows[index]["FAP"].ToString(), ref PERFAP);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERFAP"] = (object) PERFAP;
                }
                else if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(str10)) == 0)
                {
                  dataTable1.Rows.Add(dataTable1.NewRow());
                  DataRow row = dataTable1.Rows[dataTable1.Rows.Count - 1];
                  dateTime = Convert.ToDateTime(s1);
                  string str18 = dateTime.ToString().Substring(0, 10);
                  row["DAL"] = (object) str18;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s3;
                  var proDenDet = GetProDenDet(dataTable1, matricola, proRap);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PRODENDET"] = proDenDet;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"] = (object) "S";
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPRAP"] = (object) int32_4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCON"] = (object) int32_5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROCON"] = (object) num7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLOC"] = (object) int32_6;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROLOC"] = (object) num14;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLIV"] = (object) int32_7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["LIVELLO"] = (object) str4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["QUALIFICA"] = (object) str3;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODQUACON"] = (object) CODQUACON;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERPAR"] = (object) num12;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERAPP"] = (object) num13;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) num15;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) num9;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) num8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["FAP"] = (object) str11;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ASSCON"] = (object) strFlag2;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ABBPRE"] = (object) strFlag1;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) this.GetImportoParametro((short) 1, str10, strFlag1);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) this.GetImportoParametro((short) 4, str10, strFlag2);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPSPE"] = (object) str5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODGRUASS"] = (object) int32_8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.GetAliquota(str10, CODQUACON, int32_8, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"].ToString(), dataTable7.Rows[index]["FAP"].ToString(), ref PERFAP);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERFAP"] = (object) PERFAP;
                }
                else if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(str10)) < 0)
                {
                  dataTable1.Rows.Add(dataTable1.NewRow());
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"] = (object) str10;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s3;
                  var proDenDet = GetProDenDet(dataTable1, matricola, proRap);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PRODENDET"] = proDenDet;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"] = (object) "S";
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPRAP"] = (object) int32_4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCON"] = (object) int32_5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROCON"] = (object) num7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLOC"] = (object) int32_6;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROLOC"] = (object) num14;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLIV"] = (object) int32_7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["LIVELLO"] = (object) str4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["QUALIFICA"] = (object) str3;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODQUACON"] = (object) CODQUACON;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERPAR"] = (object) num12;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERAPP"] = (object) num13;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) num15;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) num9;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) num8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["FAP"] = (object) str11;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ASSCON"] = (object) strFlag2;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ABBPRE"] = (object) strFlag1;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) this.GetImportoParametro((short) 1, str10, strFlag1);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) this.GetImportoParametro((short) 4, str10, strFlag2);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPSPE"] = (object) str5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODGRUASS"] = (object) int32_8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.GetAliquota(str10, CODQUACON, int32_8, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"].ToString(), dataTable7.Rows[index]["FAP"].ToString(), ref PERFAP);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERFAP"] = (object) PERFAP;
                }
                else if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(s3)) == 0)
                {
                  dataTable1.Rows.Add(dataTable1.NewRow());
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"] = (object) str10;
                  DataRow row3 = dataTable1.Rows[dataTable1.Rows.Count - 1];
                  dateTime = Convert.ToDateTime(s1);
                  dateTime = dateTime.AddDays(-1.0);
                  string str19 = dateTime.ToString().Substring(0, 10);
                  row3["AL"] = (object) str19;
                  var proDenDet = GetProDenDet(dataTable1, matricola, proRap);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PRODENDET"] = proDenDet;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"] = (object) "N";
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPRAP"] = (object) int32_4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCON"] = (object) int32_5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROCON"] = (object) num7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLOC"] = (object) int32_6;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROLOC"] = (object) num14;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLIV"] = (object) int32_7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["LIVELLO"] = (object) str4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["QUALIFICA"] = (object) str3;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODQUACON"] = (object) CODQUACON;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERPAR"] = (object) num12;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERAPP"] = (object) num13;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) num15;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) num9;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) num8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["FAP"] = (object) str11;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ASSCON"] = (object) strFlag2;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ABBPRE"] = (object) strFlag1;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) this.GetImportoParametro((short) 1, str10, strFlag1);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) this.GetImportoParametro((short) 4, str10, strFlag2);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPSPE"] = (object) str5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODGRUASS"] = (object) int32_8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.GetAliquota(str10, CODQUACON, int32_8, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"].ToString(), dataTable7.Rows[index]["FAP"].ToString(), ref PERFAP);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERFAP"] = (object) PERFAP;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODPOS"] = (object) int32_1;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["MAT"] = (object) matricola;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODFIS"] = codFis;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PRORAP"] = (object) proRap;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["NOME"] = (object) str6;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["DATNAS"] = (object) str7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["DATDEC"] = (object) str8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["DATCES"] = (object) s2;
                  dataTable1.Rows.Add(dataTable1.NewRow());
                  DataRow row4 = dataTable1.Rows[dataTable1.Rows.Count - 1];
                  dateTime = Convert.ToDateTime(s1);
                  string str20 = dateTime.ToString().Substring(0, 10);
                  row4["DAL"] = (object) str20;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s3;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PRODENDET"] = proDenDet + 1;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"] = (object) "S";
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPRAP"] = (object) int32_4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCON"] = (object) int32_5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROCON"] = (object) num7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLOC"] = (object) int32_6;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROLOC"] = (object) num14;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLIV"] = (object) int32_7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["LIVELLO"] = (object) str4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["QUALIFICA"] = (object) str3;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODQUACON"] = (object) CODQUACON;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERPAR"] = (object) num12;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERAPP"] = (object) num13;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) num15;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) num9;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) num8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["FAP"] = (object) str11;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ASSCON"] = (object) strFlag2;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ABBPRE"] = (object) strFlag1;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) this.GetImportoParametro((short) 1, str10, strFlag1);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) this.GetImportoParametro((short) 4, str10, strFlag2);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPSPE"] = (object) str5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODGRUASS"] = (object) int32_8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.GetAliquota(str10, CODQUACON, int32_8, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"].ToString(), dataTable7.Rows[index]["FAP"].ToString(), ref PERFAP);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERFAP"] = (object) PERFAP;
                }
                else
                {
                  dataTable1.Rows.Add(dataTable1.NewRow());
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"] = (object) str10;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s3;
                  var proDenDet = GetProDenDet(dataTable1, matricola, proRap);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PRODENDET"] = proDenDet;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"] = (object) "N";
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPRAP"] = (object) int32_4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCON"] = (object) int32_5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROCON"] = (object) num7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLOC"] = (object) int32_6;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PROLOC"] = (object) num14;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLIV"] = (object) int32_7;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["LIVELLO"] = (object) str4;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["QUALIFICA"] = (object) str3;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODQUACON"] = (object) CODQUACON;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERPAR"] = (object) num12;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERAPP"] = (object) num13;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) num15;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) num9;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) num8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["FAP"] = (object) str11;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ASSCON"] = (object) strFlag2;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ABBPRE"] = (object) strFlag1;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) this.GetImportoParametro((short) 1, str10, strFlag1);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) this.GetImportoParametro((short) 4, str10, strFlag2);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPSPE"] = (object) str5;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["CODGRUASS"] = (object) int32_8;
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.GetAliquota(str10, CODQUACON, int32_8, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETA65"].ToString(), dataTable7.Rows[index]["FAP"].ToString(), ref PERFAP);
                  dataTable1.Rows[dataTable1.Rows.Count - 1]["PERFAP"] = (object) PERFAP;
                }
                dataTable1.Rows[dataTable1.Rows.Count - 1]["CODPOS"] = (object) int32_1;
                dataTable1.Rows[dataTable1.Rows.Count - 1]["MAT"] = (object) matricola;
                dataTable1.Rows[dataTable1.Rows.Count - 1]["CODFIS"] = codFis;
                dataTable1.Rows[dataTable1.Rows.Count - 1]["PRORAP"] = (object) proRap;
                dataTable1.Rows[dataTable1.Rows.Count - 1]["NOME"] = (object) str6;
                dataTable1.Rows[dataTable1.Rows.Count - 1]["DATNAS"] = (object) str7;
                dataTable1.Rows[dataTable1.Rows.Count - 1]["DATDEC"] = (object) str8;
                dataTable1.Rows[dataTable1.Rows.Count - 1]["DATCES"] = (object) s2;
                dataTable1.Rows[dataTable1.Rows.Count - 1]["PREV"] = (object) str9;
              }
              else
                break;
            }
            else
              break;
          }
          else
            break;
        }
        for (int index = 0; index <= dataTable1.Rows.Count - 1; ++index)
        {
          if (Convert.ToInt32(dataTable1.Rows[index]["CODPOS"]) == num1 
              & Convert.ToInt32(dataTable1.Rows[index]["MAT"]) == num2 
              & Convert.ToDecimal(dataTable1.Rows[index]["ALIQUOTA"]) == num3
              & (dataTable1.Rows[index]["DATCES"]?.ToString() ?? "") == str2)
          {
            dataTable1.Rows[index - num6]["AL"] = dataTable1.Rows[index]["AL"];
            dataTable1.Rows[index - num6]["IMPSCA"] = dataTable1.Rows[index]["IMPSCA"];
            dataTable1.Rows[index - num6]["IMPMIN"] = dataTable1.Rows[index]["IMPMIN"];
            dataTable1.Rows[index - num6]["DATDEC"] = dataTable1.Rows[index]["DATDEC"];
            dataTable1.Rows[index - num6]["IMPTRAECO"] = dataTable1.Rows[index]["IMPTRAECO"];
            dataTable1.Rows[index - num6]["IMPSCA"] = dataTable1.Rows[index]["IMPSCA"];
            dataTable1.Rows[index - num6]["ABBPRE"] = dataTable1.Rows[index]["ABBPRE"];
            dataTable1.Rows[index - num6]["IMPABB"] = dataTable1.Rows[index]["IMPABB"];
            dataTable1.Rows[index - num6]["ASSCON"] = dataTable1.Rows[index]["ASSCON"];
            dataTable1.Rows[index - num6]["IMPASSCON"] = dataTable1.Rows[index]["IMPASSCON"];
            dataTable1.Rows[index - num6]["RIMUOVI"] = (object) "SI";

            var dataDalRecord1 = dataTable1.Rows[index].GetRawDateCurrentCultureElementAt("DAL");
            var dataDalRecord2 = dataTable1.Rows[index - num6].GetRawDateCurrentCultureElementAt("DAL");
            var minDataDal1DataDal2 = Math.Min(dataDalRecord1.Value.Ticks, dataDalRecord2.Value.Ticks);
            var dal = Math.Max(minDataDal1DataDal2, DateTime.Parse(strDataDal).Ticks);
            dataTable1.Rows[index]["DAL"] = new DateTime(dal);
            
            var dataAlRecord1 = dataTable1.Rows[index].GetRawDateCurrentCultureElementAt("AL");
            var dataAlRecord2 = dataTable1.Rows[index - num6].GetRawDateCurrentCultureElementAt("AL");
            var minDataAl1DataAl2 = Math.Max(dataAlRecord1.Value.Ticks, dataAlRecord2.Value.Ticks);
            var al = Math.Min(minDataAl1DataAl2, DateTime.Parse(strDataAl).Ticks);
            dataTable1.Rows[index]["AL"] = new DateTime(al);
            dataTable1.Rows[index]["PRODENDET"] = dataTable1.Rows[index - num6]["PRODENDET"];
            ++num6;
          }
          else
            num6 = 1;
          dataTable1.Rows[index]["IMPRET"] = (object) "0,00";
          dataTable1.Rows[index]["IMPOCC"] = (object) "0,00";
          dataTable1.Rows[index]["IMPCON"] = (object) "0,00";
          dataTable1.Rows[index]["IMPFIG"] = (object) "0,00";
          num1 = Convert.ToInt32(dataTable1.Rows[index]["CODPOS"]);
          num2 = Convert.ToInt32(dataTable1.Rows[index]["MAT"]);
          num3 = Convert.ToDecimal(dataTable1.Rows[index]["ALIQUOTA"]);
          num4 = Convert.ToDecimal(dataTable1.Rows[index]["PERAPP"]);
          num5 = Convert.ToDecimal(dataTable1.Rows[index]["PERPAR"]);
          str1 = dataTable1.Rows[index]["TIPSPE"].ToString();
          str2 = dataTable1.Rows[index]["DATCES"]?.ToString() ?? "";
        }
        for (int index = dataTable1.Rows.Count - 1; index >= 0; index += -1)
        {
          if (dataTable1.Rows[index]["RIMUOVI"].ToString() == "SI")
            dataTable1.Rows.RemoveAt(index);
          else if (dataTable1.Rows[index]["PREV"].ToString() == "S")
          {
            string str21 = "SELECT MODPRE.PROMOD, CODSTAPRE, IMPRET, IMPOCC, IMPFIG, IMPCON, IMPRETPRV, IMPOCCPRV, IMPFIGPRV, IMPCONPRV, " + "TIPMOV FROM MODPRE INNER JOIN MODPREDET ON MODPRE.CODPOS = MODPREDET.CODPOS AND MODPRE.MAT = MODPREDET.MAT " + "AND MODPRE.PRORAP = MODPREDET.PRORAP AND MODPRE.PROMOD = MODPREDET.PROMOD WHERE MODPREDET.CODPOS = " + dataTable1.Rows[index]["CODPOS"]?.ToString() + " AND MODPREDET.MAT = " + dataTable1.Rows[index]["MAT"]?.ToString() + " AND MODPREDET.PRORAP = " + dataTable1.Rows[index]["PRORAP"]?.ToString() + " AND MODPREDET.DAL = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable1.Rows[index]["DAL"].ToString())) + " AND AL = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable1.Rows[index]["AL"].ToString())) + " AND MODPRE.DATANN IS NULL";
            string strSQL9 = !blnArretrato ? str21 + " AND TIPMOV IN ('DP', 'NU')" : str21 + " AND TIPMOV = 'AR'";
            DataTable dataTable8 = dataLayer.GetDataTable(strSQL9);
            if (dataTable8.Rows.Count > 0)
            {
              bool flag = true;
              dataTable1.Rows[index]["PROMOD"] = dataTable8.Rows[0]["PROMOD"];
              if (dataTable8.Rows[0]["CODSTAPRE"].ToString() != "0")
              {
                dataTable1.Rows[index]["PREV"] = (object) "T";
                if (blnArretrato)
                {
                  string str22 = "SELECT COUNT(*) FROM DENDET A INNER JOIN MODPREDET B ON A.CODPOS = B.CODPOS AND A.MAT = B.MAT AND A.PRORAP = B.PRORAP " + "AND A.TIPMOV = B.TIPMOV AND A.PRODEN = B.PRODEN AND A.PRODENDET = B.PRODENDET WHERE A.CODPOS = " + strCodPos + " AND " + "A.MAT = " + dataTable1.Rows[index]["MAT"]?.ToString() + " AND A.PRORAP = " + dataTable1.Rows[index]["PRORAP"]?.ToString() + " AND A.TIPMOV = 'AR' AND A.ANNCOM = ";
                  dateTime = DateTime.Parse(strDataDal);
                  string str23 = dateTime.Year.ToString();
                  string strSQL10 = str22 + str23 + " AND VALUE(A.NUMMOV, '') = ''";
                  if (Convert.ToInt32("0" + dataLayer.Get1ValueFromSQL(strSQL10, CommandType.Text)) == 0)
                    flag = false;
                }
              }
              if (flag)
              {
                dataTable1.Rows[index]["IMPRET"] = !DBNull.Value.Equals(dataTable8.Rows[0]["IMPRETPRV"]) ? dataTable8.Rows[0]["IMPRETPRV"] : dataTable8.Rows[0]["IMPRET"];
                dataTable1.Rows[index]["IMPOCC"] = !DBNull.Value.Equals(dataTable8.Rows[0]["IMPOCCPRV"]) ? dataTable8.Rows[0]["IMPOCCPRV"] : dataTable8.Rows[0]["IMPOCC"];
                dataTable1.Rows[index]["IMPFIG"] = !DBNull.Value.Equals(dataTable8.Rows[0]["IMPFIGPRV"]) ? dataTable8.Rows[0]["IMPFIGPRV"] : dataTable8.Rows[0]["IMPFIG"];
                dataTable1.Rows[index]["IMPCON"] = !DBNull.Value.Equals(dataTable8.Rows[0]["IMPCONPRV"]) ? dataTable8.Rows[0]["IMPCONPRV"] : dataTable8.Rows[0]["IMPCON"];
              }
            }
            else if (blnArretrato)
            {
              string strSQL11 = "SELECT MODPRE.PROMOD, CODSTAPRE, IMPRET, IMPOCC, IMPFIG, IMPCON, IMPRETPRV, IMPOCCPRV, IMPFIGPRV, IMPCONPRV, " + "TIPMOV FROM MODPRE INNER JOIN MODPREDET ON MODPRE.CODPOS = MODPREDET.CODPOS AND MODPRE.MAT = MODPREDET.MAT " + "AND MODPRE.PRORAP = MODPREDET.PRORAP AND MODPRE.PROMOD = MODPREDET.PROMOD WHERE MODPREDET.CODPOS = " + dataTable1.Rows[index]["CODPOS"]?.ToString() + " AND MODPREDET.MAT = " + dataTable1.Rows[index]["MAT"]?.ToString() + " AND MODPREDET.PRORAP = " + dataTable1.Rows[index]["PRORAP"]?.ToString() + " AND MODPREDET.DAL = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable1.Rows[index]["DAL"].ToString())) + " AND MODPRE.DATANN IS NULL";
              DataTable dataTable9 = dataLayer.GetDataTable(strSQL11);
              if (dataTable9.Rows.Count > 0)
              {
                dataTable1.Rows[index]["PROMOD"] = dataTable9.Rows[0]["PROMOD"];
                if (dataTable9.Rows[0]["CODSTAPRE"].ToString() != "0")
                  dataTable1.Rows[index]["PREV"] = (object) "T";
              }
            }
          }
        }
        if (!blnArretrato)
        {
          string[] strArray = new string[5]
          {
            "SELECT CODPOS, MAT, PRORAP, IMPFIG FROM PRAINFRDLDET WHERE CODPOS = " + strCodPos,
            " AND ANNO = ",
            null,
            null,
            null
          };
          dateTime = DateTime.Parse(strDataDal);
          strArray[2] = dateTime.Year.ToString();
          strArray[3] = " AND MESE = ";
          dateTime = DateTime.Parse(strDataDal);
          strArray[4] = dateTime.Month.ToString();
          string strSQL12 = string.Concat(strArray);
          DataView dataView11 = dataLayer.GetDataView(strSQL12);
          for (int index = 0; index <= dataTable1.Rows.Count - 1; ++index)
          {
            dataTable1.Rows[index]["NUMGGPER"] = (object) this.Get_NumGG_Periodo(Convert.ToDateTime(dataTable1.Rows[index]["DAL"]), Convert.ToDateTime(dataTable1.Rows[index]["AL"]));
            dataTable1.Rows[index]["NUMGGDOM"] = (object) this.Get_NumGG_Domeniche(Convert.ToDateTime(dataTable1.Rows[index]["DAL"]), Convert.ToDateTime(dataTable1.Rows[index]["AL"]));
            Decimal NUMGGAZI = Convert.ToDecimal(dataTable1.Rows[index]["NUMGGAZI"] = (object) 0.0);
            int int32 = Convert.ToInt32(dataTable1.Rows[index]["NUMGGSOS"] = (object) 0.0);
            Decimal NUMGGFIG = Convert.ToDecimal(dataTable1.Rows[index]["NUMGGFIG"] = (object) 0.0);
            string CODSOS = dataTable1.Rows[index]["CODSOS"].ToString();
            this.Get_NumGG_Sospensioni(ref dataView10, Convert.ToDateTime(dataTable1.Rows[index]["DAL"]), Convert.ToDateTime(dataTable1.Rows[index]["AL"]), Convert.ToInt32(dataTable1.Rows[index]["MAT"]), Convert.ToInt32(dataTable1.Rows[index]["PRORAP"]), ref NUMGGAZI, ref int32, ref NUMGGFIG, ref CODSOS);
            dataTable1.Rows[index]["NUMGGAZI"] = (object) NUMGGAZI;
            dataTable1.Rows[index]["NUMGGSOS"] = (object) int32;
            dataTable1.Rows[index]["NUMGGFIG"] = (object) NUMGGFIG;
            dataTable1.Rows[index]["CODSOS"] = (object) CODSOS;
            if (num10 > 0)
              dataTable1.Rows[index]["NUMGGCONAZI"] = Convert.ToInt32(dataTable1.Rows[index]["NUMGGSOS"]) <= 0 ? (!(Convert.ToDateTime(dataTable1.Rows[index]["DATDEC"]) <= Convert.ToDateTime(dataTable1.Rows[index]["DAL"])) ? (object) (Decimal.Round((Decimal) num10 / Convert.ToDecimal(26), 0) * Convert.ToDecimal(dataTable1.Rows[index]["NUMGGPER"])) : (!(dataTable1.Rows[index]["DATCES"].ToString().Trim() == "") ? (!(Convert.ToDateTime(dataTable1.Rows[index]["DATCES"]) > Convert.ToDateTime(dataTable1.Rows[index]["AL"])) ? (object) (Decimal.Round((Decimal) num10 / Convert.ToDecimal(26), 0) * Convert.ToDecimal(dataTable1.Rows[index]["NUMGGPER"])) : (object) (Convert.ToDecimal(dataTable1.Rows[index]["NUMGGPER"]) - Convert.ToDecimal(dataTable1.Rows[index]["NUMGGDOM"]) - Convert.ToDecimal(dataTable1.Rows[index]["NUMGGSOS"]) + Convert.ToDecimal(dataTable1.Rows[index]["NUMGGAZI"]))) : (object) Decimal.Round(Convert.ToDecimal(dataTable1.Rows[index]["NUMGGPER"]) * 26M / (Decimal) num10, 0))) : (object) (Convert.ToInt32(dataTable1.Rows[index]["NUMGGPER"]) - Convert.ToInt32(dataTable1.Rows[index]["NUMGGDOM"]) - Convert.ToInt32(dataTable1.Rows[index]["NUMGGSOS"]) + Convert.ToInt32(dataTable1.Rows[index]["NUMGGAZI"]));
            dataTable1.Rows[index]["IMPFIG"] = !(dataTable1.Rows[index]["TIPSPE"].ToString() == "S") ? (object) (Convert.ToDecimal(dataTable1.Rows[index]["IMPTRAECO"]) * Convert.ToDecimal(dataTable1.Rows[index]["NUMGGFIG"]) / 26M) : (object) ((Convert.ToDecimal(dataTable1.Rows[index]["IMPMIN"]) + Convert.ToDecimal(dataTable1.Rows[index]["IMPSCA"])) * Convert.ToDecimal(dataTable1.Rows[index]["NUMGGFIG"]) / 26M);
            dataTable1.Rows[index]["IMPFIG"] = (object) Math.Round(Convert.ToDecimal(dataTable1.Rows[index]["IMPFIG"]));
            dataTable1.Rows[index]["IMPFAP"] = (object) 0.0;
            dataView11.RowFilter = "";
            dataView11.RowFilter = "MAT = " + dataTable1.Rows[index]["MAT"]?.ToString() + " AND PRORAP = " + dataTable1.Rows[index]["PRORAP"]?.ToString();
            if (dataView11.Count > 0)
              dataTable1.Rows[index]["IMPFIG"] = dataView11[0]["IMPFIG"];
          }
        }
        else
        {
          for (int index = 0; index <= dataTable1.Rows.Count - 1; ++index)
          {
            dataTable1.Rows[index]["NUMGGCONAZI"] = (object) 0;
            dataTable1.Rows[index]["NUMGGPER"] = (object) 0;
            dataTable1.Rows[index]["NUMGGDOM"] = (object) 0;
            dataTable1.Rows[index]["NUMGGAZI"] = (object) 0;
            dataTable1.Rows[index]["NUMGGSOS"] = (object) 0;
            dataTable1.Rows[index]["NUMGGFIG"] = (object) 0;
            dataTable1.Rows[index]["IMPFIG"] = (object) 0.0;
            dataTable1.Rows[index]["IMPFAP"] = (object) 0.0;
          }
        }
        List<DenunciaArretrati> denunciaArretratiList = new List<DenunciaArretrati>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
        {
          DenunciaArretrati denunciaArretrati1 = new DenunciaArretrati();
          denunciaArretrati1.mat = Convert.ToInt32(row["MAT"]);
          denunciaArretrati1.CodFis = row["CODFIS"].ToString();
          denunciaArretrati1.nome = row["NOME"].ToString();
          dateTime = Convert.ToDateTime(row["DAL"]);
          denunciaArretrati1.datadal = dateTime.ToString();
          dateTime = Convert.ToDateTime(row["AL"]);
          denunciaArretrati1.dataal = dateTime.ToString();
          denunciaArretrati1.qualifica = row["QUALIFICA"].ToString();
          denunciaArretrati1.livello = row["LIVELLO"].ToString();
          denunciaArretrati1.eta65 = row["ETA65"].ToString();
          denunciaArretrati1.impret = row["IMPRET"].ToString();
          denunciaArretrati1.impocc = row["IMPOCC"].ToString();
          denunciaArretrati1.aliquota = Convert.ToDecimal(row["ALIQUOTA"]).ToString().Replace(".", ",");
          denunciaArretrati1.impcon = row["IMPCON"].ToString();
          denunciaArretrati1.perfap = row["PERFAP"].ToString();
          denunciaArretrati1.promod = Convert.ToInt32(row["PROMOD"]);
          denunciaArretrati1.prorap = Convert.ToInt32(row["PRORAP"]);
          denunciaArretrati1.impfap = Convert.ToDecimal(row["IMPFAP"]);
          denunciaArretrati1.perapp = Convert.ToDecimal(row["PERAPP"]);
          denunciaArretrati1.fap = row["FAP"].ToString();
          denunciaArretrati1.tipse = row["TIPSPE"].ToString();
          denunciaArretrati1.proDen = row.IntElementAt("PRODENDET").Value;
          dateTime = Convert.ToDateTime(row["DATNAS"]);
          denunciaArretrati1.datnas = dateTime.ToString("d");
          DenunciaArretrati denunciaArretrati2 = denunciaArretrati1;
          denunciaArretratiList.Add(denunciaArretrati2);
        }
        HttpContext.Current.Session["tbArretrati"] = (object) dataTable1;
        return denunciaArretratiList;
      }
      catch (Exception ex)
      {
        string message = ex.Message;
        return (List<DenunciaArretrati>) null;
      }
    }

    private int GetProDenDet(DataTable dataTable, int matricola, int proRap)
    {
      var proDenDetCount = 1;
      
      foreach (DataRow row in  dataTable.Rows as InternalDataCollectionBase)
      {
        if (row.ElementAt("MAT") != matricola.ToString() || (row.ElementAt("PRORAP") == proRap.ToString() || row.ElementAt("ETA65") == "N")) continue;
        proDenDetCount++;
      }

      return proDenDetCount;
    }

    public void CaricaAliquote()
    {
      bool flag = false;
      DataLayer dataLayer = new DataLayer();
      if (this.objDvAliquote.Table != null)
        return;
      if (dataLayer == null)
        flag = true;
      string strSQL1 = "SELECT DISTINCT A.CODGRUASS, A.CODFORASS, A.ALIQUOTA, A.DATINI, VALUE(A.DATFIN, '9999-12-31') " + "AS DATFIN, B.CATFORASS, B.DENFORASS, A.CODQUACON FROM ALIFORASS A INNER JOIN FORASS B ON " + "A.CODFORASS = B.CODFORASS WHERE B.CATFORASS <> 'FAP'";
      this.objDvAliquote = dataLayer.GetDataView(strSQL1);
      this.objDvAliquote.Sort = "DATINI DESC";
      string strSQL2 = "SELECT DISTINCT CODFAP, DATINI, VALUE(DATFIN, '9999-12-31') AS DATFIN, VALFAP " + "FROM CODFAP";
      this.objDvPerFap = dataLayer.GetDataView(strSQL2);
      this.objDvPerFap.Sort = "DATINI DESC";
      if (!flag)
        ;
    }

    public Decimal GetAliquota(
      string strData,
      int CODQUACON,
      int CODGRUASS,
      string str65,
      string strFAP,
      ref Decimal PERFAP)
    {
      Decimal aliquota = Convert.ToDecimal(0.0);
      string str1 = "CODGRUASS = " + CODGRUASS.ToString() + " AND CODQUACON = " + CODQUACON.ToString() + " AND '" + DBMethods.Db2Date(strData) + "' >= DATINI AND '" + DBMethods.Db2Date(strData) + "' <= DATFIN";
      if (str65 == "S")
        str1 += " AND CATFORASS <> 'PREV'";
      this.objDvAliquote.RowFilter = "";
      this.objDvAliquote.RowFilter = str1;
      if (this.objDvAliquote.Count > 0)
      {
        for (int recordIndex = 0; recordIndex <= this.objDvAliquote.Count - 1; ++recordIndex)
          aliquota += Convert.ToDecimal(this.objDvAliquote[recordIndex]["ALIQUOTA"]);
      }
      if (strFAP == "S")
      {
        string str2 = DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " >= DATINI AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " <= DATFIN";
        this.objDvPerFap.RowFilter = "";
        this.objDvPerFap.RowFilter = str2;
        if (this.objDvPerFap.Count > 0)
        {
          aliquota += Convert.ToDecimal(this.objDvPerFap[0]["VALFAP"]);
          PERFAP = Convert.ToDecimal(0.0);
        }
      }
      else
        PERFAP = Convert.ToDecimal(this.objDvPerFap[0]["VALFAP"]);
      return aliquota;
    }

    public Decimal GetImportoParametro(short intCodPar, string strDataDal, string strFlag = "S")
    {
      Decimal d = 0.0M;
      if (strFlag == "S")
      {
        string str = "CODPAR = " + intCodPar.ToString() + " AND DATINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strDataDal));
        GeneraDenunciaDAL.objDvPargen.RowFilter = "";
        GeneraDenunciaDAL.objDvPargen.RowFilter = str;
        if (GeneraDenunciaDAL.objDvPargen.Count > 0)
          d = !(GeneraDenunciaDAL.objDvPargen[0]["VALORE"].ToString() == ".00") ? Convert.ToDecimal(GeneraDenunciaDAL.objDvPargen[0]["VALORE"]) : 0M;
      }
      return Decimal.Round(d, 2);
    }

    public int Get_NumGG_Domeniche(DateTime DataDal, DateTime DataAl)
    {
      int numGgDomeniche = 0;
      for (; DataDal <= DataAl; DataDal = DataDal.AddDays(1.0))
      {
        if (DataDal.DayOfWeek == DayOfWeek.Sunday)
          ++numGgDomeniche;
      }
      return numGgDomeniche;
    }

    public int Get_NumGG_Periodo(DateTime DataDal, DateTime DataAl) => Convert.ToDateTime(DataAl).Subtract(DataDal).Days + 1;

    public void Get_NumGG_Sospensioni(
      ref DataView dvSos,
      DateTime DataDal,
      DateTime DataAl,
      int MAT,
      int PRORAP,
      ref Decimal NUMGGAZI,
      ref int NUMGGSOS,
      ref Decimal NUMGGFIG,
      ref string CODSOS)
    {
      int num1 = 0;
      Decimal num2 = Convert.ToDecimal(0.0);
      Decimal num3 = Convert.ToDecimal(0.0);
      dvSos.RowFilter = "";
      dvSos.RowFilter = "MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString();
      if (dvSos.Count > 0)
      {
        for (int recordIndex = 0; recordIndex <= dvSos.Count - 1; ++recordIndex)
        {
          if (Convert.ToDateTime(dvSos[recordIndex]["DATFINSOS"]) >= Convert.ToDateTime(DataDal))
          {
            CODSOS = !(CODSOS == "") ? CODSOS + "-" + dvSos[recordIndex][nameof (CODSOS)].ToString() : CODSOS + dvSos[recordIndex][nameof (CODSOS)].ToString();
            DateTime dateTime;
            if (Convert.ToDateTime(dvSos[recordIndex]["DATINISOS"]) <= Convert.ToDateTime(DataAl))
            {
              if (DataDal >= Convert.ToDateTime(dvSos[recordIndex]["DATINISOS"]))
              {
                if (DataAl <= Convert.ToDateTime(dvSos[recordIndex]["DATFINSOS"]))
                {
                  int numGgDomeniche = this.Get_NumGG_Domeniche(DataDal, DataAl);
                  dateTime = Convert.ToDateTime(DataAl);
                  num1 = dateTime.Subtract(DataDal).Days + 1 - numGgDomeniche;
                  if (Convert.ToInt32(dvSos[recordIndex]["PERAZI"]) > 0)
                    num2 = Convert.ToDecimal(num1) * Convert.ToDecimal(dvSos[recordIndex]["PERAZI"]) / 100M;
                  if (Convert.ToInt32(dvSos[recordIndex]["PERFIG"]) > 0)
                  {
                    num3 = Convert.ToDecimal(num1) * Convert.ToDecimal(dvSos[recordIndex]["PERFIG"]) / 100M;
                    break;
                  }
                  break;
                }
                int numGgDomeniche1 = this.Get_NumGG_Domeniche(DataDal, Convert.ToDateTime(dvSos[recordIndex]["DATFINSOS"]));
                dateTime = Convert.ToDateTime(dvSos[recordIndex]["DATFINSOS"]);
                num1 = dateTime.Subtract(DataDal).Days + 1 - numGgDomeniche1;
                if (Convert.ToInt32(dvSos[recordIndex]["PERAZI"]) > 0)
                  num2 = Convert.ToDecimal(num1) * Convert.ToDecimal(dvSos[recordIndex]["PERAZI"]) / 100M;
                if (Convert.ToInt32(dvSos[recordIndex]["PERFIG"]) > 0)
                  num3 = Convert.ToDecimal(num1) * Convert.ToDecimal(dvSos[recordIndex]["PERFIG"]) / 100M;
              }
              else
              {
                if (DataAl <= Convert.ToDateTime(dvSos[recordIndex]["DATFINSOS"]))
                {
                  int numGgDomeniche = this.Get_NumGG_Domeniche(Convert.ToDateTime(dvSos[recordIndex]["DATINISOS"]), DataAl);
                  int num4 = num1;
                  dateTime = Convert.ToDateTime(DataAl);
                  int num5 = dateTime.Subtract(Convert.ToDateTime(dvSos[recordIndex]["DATINISOS"])).Days + 1 - numGgDomeniche;
                  num1 = num4 + num5;
                  if (Convert.ToInt32(dvSos[recordIndex]["PERAZI"]) > 0)
                  {
                    Decimal num6 = num2;
                    dateTime = Convert.ToDateTime(DataAl);
                    Decimal num7 = (Decimal) (dateTime.Subtract(Convert.ToDateTime(dvSos[recordIndex]["DATINISOS"])).Days + 1) - (Decimal) numGgDomeniche * Convert.ToDecimal(dvSos[recordIndex]["PERAZI"]) / 100M;
                    num2 = num6 + num7;
                  }
                  if (Convert.ToInt32(dvSos[recordIndex]["PERFIG"]) > 0)
                  {
                    Decimal num8 = num3;
                    dateTime = Convert.ToDateTime(DataAl);
                    Decimal num9 = (Decimal) (dateTime.Subtract(Convert.ToDateTime(dvSos[recordIndex]["DATINISOS"])).Days + 1) - (Decimal) numGgDomeniche * Convert.ToDecimal(dvSos[recordIndex]["PERFIG"]) / 100M;
                    num3 = num8 + num9;
                    break;
                  }
                  break;
                }
                int numGgDomeniche2 = this.Get_NumGG_Domeniche(Convert.ToDateTime(dvSos[recordIndex]["DATINISOS"]), Convert.ToDateTime(dvSos[recordIndex]["DATFINSOS"]));
                int num10 = num1;
                dateTime = Convert.ToDateTime(dvSos[recordIndex]["DATFINSOS"]);
                int num11 = dateTime.Subtract(Convert.ToDateTime(dvSos[recordIndex]["DATINISOS"])).Days + 1 - numGgDomeniche2;
                num1 = num10 + num11;
                if (Convert.ToInt32(dvSos[recordIndex]["PERAZI"]) > 0)
                {
                  Decimal num12 = num2;
                  dateTime = Convert.ToDateTime(dvSos[recordIndex]["DATFINSOS"]);
                  Decimal num13 = (Decimal) (dateTime.Subtract(Convert.ToDateTime(dvSos[recordIndex]["DATINISOS"])).Days + 1) - (Decimal) numGgDomeniche2 * Convert.ToDecimal(dvSos[recordIndex]["PERAZI"]) / 100M;
                  num2 = num12 + num13;
                }
                if (Convert.ToInt32(dvSos[recordIndex]["PERFIG"]) > 0)
                {
                  Decimal num14 = num3;
                  dateTime = Convert.ToDateTime(dvSos[recordIndex]["DATFINSOS"]);
                  Decimal num15 = (Decimal) (dateTime.Subtract(Convert.ToDateTime(dvSos[recordIndex]["DATINISOS"])).Days + 1) - (Decimal) numGgDomeniche2 * Convert.ToDecimal(dvSos[recordIndex]["PERFIG"]) / 100M;
                  num3 = num14 + num15;
                }
              }
            }
          }
        }
      }
      NUMGGSOS = num1;
      NUMGGFIG = num3;
      NUMGGAZI = num2;
    }

    public List<DenunciaArretrati> CaricaArretrati(
      int txtAnno,
      string hdnSalva,
      int hdnAnnComp,
      string radio,
      string txtDataDenuncia,
      List<DenunciaArretrati> ListaDenunce,
      ref string ErrorMSG,
      ref string SuccessMSG)
    {
      if (hdnSalva == "S")
      {
        this.SalvaArretrati(radio, txtDataDenuncia, ListaDenunce, ref ErrorMSG, ref SuccessMSG);
        return (List<DenunciaArretrati>) null;
      }
      if (this.listaDenuncia.hdnProden == "" || this.listaDenuncia.hdnProden == null)
      {
        this.Page_Load();
        hdnAnnComp = txtAnno;
        return this.LoadData(hdnAnnComp);
      }
      this.Page_Load();
      hdnAnnComp = txtAnno;
      return this.LoadData(hdnAnnComp, true);
    }

    private List<DenunciaArretrati> LoadData(int hdnAnnComp, bool blnAll = false)
    {
      HttpContext.Current.Session["tbArretrati"] = (object) null;
      string strCodPos = ((TFI.OCM.Utente.Utente) HttpContext.Current.Session["utente"]).CodPosizione.ToString();
      DataLayer dataLayer = new DataLayer();
      List<DenunciaArretrati> denunciaArretratiList1 = new List<DenunciaArretrati>();
      List<DenunciaArretrati> denunciaArretratiList2 = new List<DenunciaArretrati>();
      this.listaDenuncia.hdnSalva = "";
      this.listaDenuncia.hdnModifica = "";
      this.listaDenuncia.decTotRetribuzioni = 0.00M;
      this.listaDenuncia.decTotOccasionali = 0.00M;
      this.listaDenuncia.decTotContributi = 0.00M;
      DateTime dateTime;
      if (!blnAll)
      {
        string strDataAl;
        if (Convert.ToInt32("0" + hdnAnnComp.ToString()) == DateTime.Today.Year)
        {
          string[] strArray = new string[5];
          dateTime = DateTime.Today;
          int year = dateTime.Year;
          dateTime = DateTime.Today;
          int month = dateTime.Month;
          int num = DateTime.DaysInMonth(year, month);
          strArray[0] = num.ToString();
          strArray[1] = "/";
          dateTime = DateTime.Today;
          num = dateTime.Month;
          strArray[2] = num.ToString();
          strArray[3] = "/";
          strArray[4] = hdnAnnComp.ToString();
          strDataAl = string.Concat(strArray);
        }
        else
          strDataAl = "31/12/" + hdnAnnComp.ToString();
        List<ParametriGenerali> listaParametriGen = (List<ParametriGenerali>) null;
        this.GeneraDenunciaArr("01/01/" + hdnAnnComp.ToString(), strDataAl, ref listaParametriGen, strCodPos, blnArretrato: true);
        HttpContext.Current.Session["ListaParGen"] = (object) listaParametriGen;
      }
      else
      {
        string[] strArray1 = new string[6]
        {
          "SELECT CHAR(ULTAGG) AS ULTAGG FROM DENTES WHERE CODPOS = " + strCodPos,
          " AND ANNDEN = ",
          null,
          null,
          null,
          null
        };
        int num = this.listaDenuncia.hdnAnno;
        strArray1[2] = num.ToString();
        strArray1[3] = " And MESDEN = ";
        num = this.listaDenuncia.hdnMese;
        strArray1[4] = num.ToString();
        strArray1[5] = " AND PRODEN = ";
        string strSQL1 = string.Concat(strArray1) + this.listaDenuncia.hdnProden + " AND DATMOVANN IS NULL";
        HttpContext.Current.Session["CurTimeStampArretrati"] = (object) (dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text) ?? "");
        string str = "SELECT DISTINCT ANNCOM FROM DENDET WHERE CODPOS = " + strCodPos + " AND ANNDEN = ";
        string[] strArray2 = new string[6];
        strArray2[0] = str;
        num = this.listaDenuncia.hdnAnno;
        strArray2[1] = num.ToString();
        strArray2[2] = " AND MESDEN = ";
        num = this.listaDenuncia.hdnMese;
        strArray2[3] = num.ToString();
        strArray2[4] = " AND PRODEN = ";
        strArray2[5] = this.listaDenuncia.hdnProden;
        string strSQL2 = string.Concat(strArray2) + " ORDER BY ANNCOM ASC";
        DataTable dataTable = dataLayer.GetDataTable(strSQL2);
        for (int index = 0; index <= dataTable.Rows.Count - 1; ++index)
        {
          int int32 = Convert.ToInt32(dataTable.Rows[index]["ANNCOM"]);
          dateTime = DateTime.Today;
          int year1 = dateTime.Year;
          string strDataAl;
          if (int32 == year1)
          {
            string[] strArray3 = new string[5];
            dateTime = DateTime.Today;
            int year2 = dateTime.Year;
            dateTime = DateTime.Today;
            int month = dateTime.Month;
            num = DateTime.DaysInMonth(year2, month);
            strArray3[0] = num.ToString();
            strArray3[1] = "/";
            dateTime = DateTime.Today;
            num = dateTime.Month;
            strArray3[2] = num.ToString();
            strArray3[3] = "/";
            strArray3[4] = dataTable.Rows[index]["ANNCOM"]?.ToString();
            strDataAl = string.Concat(strArray3);
          }
          else
            strDataAl = "31/12/" + dataTable.Rows[index]["ANNCOM"]?.ToString();
          List<ParametriGenerali> listaParametriGen = (List<ParametriGenerali>) null;
          List<DenunciaArretrati> denunciaArretratiList3 = this.GeneraDenunciaArr("01/01/" + dataTable.Rows[index]["ANNCOM"]?.ToString(), strDataAl, ref listaParametriGen, strCodPos, blnArretrato: true);
          if (denunciaArretratiList1 == null)
            denunciaArretratiList1 = denunciaArretratiList3;
        }
      }
      DataTable dataTable1 = (DataTable) HttpContext.Current.Session["tbArretrati"];
      if (!string.IsNullOrEmpty(listaDenuncia.hdnProden))
        this.VerificaArretrati();
      for (int index = 0; index <= dataTable1.Rows.Count - 1; ++index)
      {
        this.listaDenuncia.decTotRetribuzioni += Convert.ToDecimal(dataTable1.Rows[index]["IMPRET"].ToString().Replace(",", "."));
        this.listaDenuncia.decTotOccasionali += Convert.ToDecimal(dataTable1.Rows[index]["IMPOCC"].ToString().Replace(",", "."));
        this.listaDenuncia.decTotContributi += Convert.ToDecimal(dataTable1.Rows[index]["IMPCON"].ToString().Replace(",", "."));
      }
      this.listaDenuncia.lblTotRetribuzioni = this.listaDenuncia.decTotRetribuzioni;
      this.listaDenuncia.lblTotOccasionali = this.listaDenuncia.decTotOccasionali;
      this.listaDenuncia.lblTotContributi = this.listaDenuncia.decTotContributi;
      this.listaDenuncia.hdnTotRetribuzioni = this.listaDenuncia.lblTotRetribuzioni;
      this.listaDenuncia.hdnTotOccasionali = this.listaDenuncia.lblTotOccasionali;
      this.listaDenuncia.hdnTotContributi = this.listaDenuncia.lblTotContributi;
      List<DenunciaArretrati> denunciaArretratiList4 = new List<DenunciaArretrati>();
      foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
      {
        DenunciaArretrati denunciaArretrati1 = new DenunciaArretrati();
        denunciaArretrati1.anno = Convert.ToInt32(row["ANNCOM"]);
        denunciaArretrati1.mat = Convert.ToInt32(row["MAT"]);
        denunciaArretrati1.nome = row["NOME"].ToString();
        dateTime = Convert.ToDateTime(row["DAL"]);
        denunciaArretrati1.datadal = dateTime.ToString();
        dateTime = Convert.ToDateTime(row["AL"]);
        denunciaArretrati1.dataal = dateTime.ToString();
        denunciaArretrati1.qualifica = row["QUALIFICA"].ToString();
        denunciaArretrati1.livello = row["LIVELLO"].ToString();
        denunciaArretrati1.eta65 = row["ETA65"].ToString();
        denunciaArretrati1.impret = row["IMPRET"].ToString();
        denunciaArretrati1.impocc = row["IMPOCC"].ToString();
        denunciaArretrati1.aliquota = Convert.ToDecimal(row["ALIQUOTA"]).ToString().Replace(".", ",");
        denunciaArretrati1.impcon = row["IMPCON"].ToString();
        denunciaArretrati1.perfap = row["PERFAP"].ToString();
        denunciaArretrati1.promod = Convert.ToInt32(row["PROMOD"]);
        denunciaArretrati1.prorap = Convert.ToInt32(row["PRORAP"]);
        DenunciaArretrati denunciaArretrati2 = denunciaArretrati1;
        if (!string.IsNullOrWhiteSpace(listaDenuncia.hdnProden))
        {
          string ricSanUteSqlQuery =
            $"SELECT RICSANUTE FROM DENTES WHERE CODPOS = {strCodPos} AND ANNDEN = {listaDenuncia.hdnAnno} AND MESDEN = {listaDenuncia.hdnMese} AND PRODEN = {listaDenuncia.hdnProden} AND VALUE(CODMODPAG, 0) = 0 AND TIPMOV = 'AR'";
          var denunciaTbl = dataLayer.GetDataTable(ricSanUteSqlQuery);
          var ricSanUte = denunciaTbl.Rows[0].ElementAt("RICSANUTE");
          if (ricSanUte == "EVASIONE")
            denunciaArretrati2.rbtnEvasione = true;
          else if (ricSanUte == "OMISSIONE")
            denunciaArretrati2.rbtnOmissione = true;
        }

        denunciaArretratiList4.Add(denunciaArretrati2);
      }
      return denunciaArretratiList4;
    }

    public bool VerificaArretrati()
    {
      TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente) HttpContext.Current.Session["utente"];
      DataTable dataTable1 = new DataTable();
      bool blnCommit = false;
      string str1 = utente.CodPosizione.ToString();
      DataLayer dataLayer = new DataLayer();
      string strSQL1 = "SELECT DISTINCT RAPLAV.MAT, RAPLAV.DATCES, PRODENDET FROM RAPLAV INNER JOIN DENDET " + "ON RAPLAV.CODPOS = DENDET.CODPOS AND RAPLAV.MAT = DENDET.MAT AND RAPLAV.PRORAP = " + "DENDET.PRORAP AND RAPLAV.DATCES < DENDET.AL WHERE RAPLAV.CODPOS = " + str1 + " AND YEAR(RAPLAV.DATCES) = ANNCOM AND ANNDEN = " + this.listaDenuncia.hdnAnno.ToString() + " AND MESDEN = " + this.listaDenuncia.hdnMese.ToString() + " AND PRODEN = " + this.listaDenuncia.hdnProden;
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
      if (dataTable2.Rows.Count > 0)
      {
        dataLayer.StartTransaction();
        for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
        {
          string strSQL2 = "UPDATE DENDET SET AL = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable2.Rows[index]["DATCES"].ToString())) + ", DATCES = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable2.Rows[index]["DATCES"].ToString())) + " WHERE CODPOS = " + str1 + " AND ANNDEN = " + this.listaDenuncia.hdnAnno.ToString() + " AND MESDEN = " + this.listaDenuncia.hdnMese.ToString() + " AND PRODEN = " + this.listaDenuncia.hdnProden + " AND " + "PRODENDET = " + dataTable2.Rows[index]["PRODENDET"]?.ToString();
          blnCommit = dataLayer.WriteTransactionData(strSQL2, CommandType.Text);
        }
        dataLayer.EndTransaction(blnCommit);
      }
      string strSQL3 = "SELECT COUNT(*) FROM DENDET WHERE CODPOS = " + str1 + " AND ANNDEN = " + this.listaDenuncia.hdnAnno.ToString() + " AND MESDEN = " + this.listaDenuncia.hdnMese.ToString() + " AND PRODEN = " + this.listaDenuncia.hdnProden;
      if (this.listaDenuncia.hdnAnnComp != "" && this.listaDenuncia.hdnAnnComp != null)
        strSQL3 = strSQL3 + " AND ANNCOM = " + this.listaDenuncia.hdnAnnComp;
      int int32 = Convert.ToInt32("0" + dataLayer.Get1ValueFromSQL(strSQL3, CommandType.Text));
      string str2 = "SELECT * FROM DENDET WHERE CODPOS = " + str1 + " AND ANNDEN = " + this.listaDenuncia.hdnAnno.ToString();
      int num = this.listaDenuncia.hdnMese;
      string str3 = num.ToString();
      string str4 = str2 + " AND MESDEN = " + str3 + " AND PRODEN = " + this.listaDenuncia.hdnProden;
      if (this.listaDenuncia.hdnAnnComp != "" && this.listaDenuncia.hdnAnnComp != null)
        str4 = str4 + " AND ANNCOM = " + this.listaDenuncia.hdnAnnComp;
      string strSQL4 = str4 + " ORDER BY ANNCOM, MAT, DAL";
      DataTable dataTable3 = dataLayer.GetDataTable(strSQL4);
      bool flag;
      if (dataTable3.Rows.Count > 0)
      {
        string[] strArray = new string[6]
        {
          "SELECT DATAPE, RICSANUTE FROM DENTES WHERE CODPOS = " + str1,
          " AND ANNDEN = ",
          null,
          null,
          null,
          null
        };
        num = this.listaDenuncia.hdnAnno;
        strArray[2] = num.ToString();
        strArray[3] = " AND MESDEN = ";
        num = this.listaDenuncia.hdnMese;
        strArray[4] = num.ToString();
        strArray[5] = " AND";
        string strSQL5 = string.Concat(strArray) + " PRODEN = " + this.listaDenuncia.hdnProden;
        DataTable dataTable4 = dataLayer.GetDataTable(strSQL5);
        if (dataTable4.Rows.Count > 0)
        {
          string str5 = dataTable4.Rows[0]["RICSANUTE"]?.ToString() ?? "";
          if (!(str5 == "EVASIONE"))
          {
            if (!(str5 == "RITARDO"))
            {
              if (str5 == "ESCLUDI")
                this.listaDenuncia.rbtnEscludi = true;
            }
            else
              this.listaDenuncia.rbtnRitardo = true;
          }
          else
            this.listaDenuncia.rbtnEvasione = true;
          this.listaDenuncia.txtDataDenuncia = DateTime.Parse(Convert.ToString(dataTable4.Rows[0]["DATAPE"].ToString() ?? "").Substring(0, 10)).GetDateTimeFormats((IFormatProvider) this.objDTFI)[0];
        }
        if (int32 > 0)
        {
          DataTable DtSessione = (DataTable) HttpContext.Current.Session["tbArretrati"];
          flag = this.VerificaDenuncia(ref dataTable3, ref DtSessione);
        }
        else
          flag = true;
      }
      else
        flag = true;
      return flag;
    }

    public bool VerificaDenuncia(ref DataTable DtDenuncia, ref DataTable DtSessione)
    {
      bool blnCommit = true;
      int num1 = 0;
      TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente) HttpContext.Current.Session["utente"];
      DataLayer dataLayer = new DataLayer();      
      
      dataLayer.StartTransaction();
      if (DtDenuncia.Rows.Count > 0)
      {
        if (DtSessione is null)
        {
          int annCom = int.TryParse(DtDenuncia.Rows[0].ElementAt("ANNCOM"), out int annComFromTable) ? annComFromTable: 0;
          LoadData(annCom);
          DtSessione = (DataTable)HttpContext.Current.Session["tbArretrati"];
        }

        int num2 = DtSessione.Rows.Count - 1;
        int num3 = DtDenuncia.Rows.Count - 1;
        for (int index1 = 0; index1 <= num2; ++index1)
        {
          DtSessione.Rows[index1]["MOD"] = (object) "S";
          for (int index2 = 0; index2 <= num3; ++index2)
          {
            if (DtDenuncia.Rows[index2]["MAT"].ToString() == DtSessione.Rows[index1]["MAT"].ToString() && DtDenuncia.Rows[index2]["PRORAP"].ToString() == DtSessione.Rows[index1]["PRORAP"].ToString())
            {
              DtSessione.Rows[index1]["IMPRET"] = (object) Convert.ToDecimal(DtDenuncia.Rows[index2]["IMPRET"]);
              DtSessione.Rows[index1]["IMPOCC"] = (object) Convert.ToDecimal(DtDenuncia.Rows[index2]["IMPOCC"]);
              DtSessione.Rows[index1]["IMPCON"] = (object) Convert.ToDecimal(DtDenuncia.Rows[index2]["IMPCON"]);
              if (DtDenuncia.Rows[index2]["NUMGGSOS"] != DtSessione.Rows[index1]["NUMGGSOS"] | DtDenuncia.Rows[index2]["NUMGGFIG"] != DtSessione.Rows[index1]["NUMGGFIG"] | DtDenuncia.Rows[index2]["NUMGGAZI"] != DtSessione.Rows[index1]["NUMGGAZI"] | DtDenuncia.Rows[index2]["NUMGGCONAZI"] != DtSessione.Rows[index1]["NUMGGCONAZI"])
              {
                string strSQL = "UPDATE DENDET SET IMPFIG = " + DtSessione.Rows[index1]["IMPFIG"].ToString().Replace(",", ".") + ", " + "NUMGGSOS = " + DtSessione.Rows[index1]["NUMGGSOS"].ToString().Replace(",", ".") + ", " + "NUMGGAZI = " + DtSessione.Rows[index1]["NUMGGAZI"].ToString().Replace(",", ".") + ", " + "NUMGGFIG = " + DtSessione.Rows[index1]["NUMGGFIG"].ToString().Replace(",", ".") + ", " + "NUMGGPER = " + DtSessione.Rows[index1]["NUMGGPER"].ToString().Replace(",", ".") + ", " + "NUMGGDOM = " + DtSessione.Rows[index1]["NUMGGDOM"].ToString().Replace(",", ".") + ", " + "NUMGGCONAZI = " + DtSessione.Rows[index1]["NUMGGCONAZI"].ToString().Replace(",", ".") + " WHERE CODPOS = " + utente.CodPosizione.ToString() + " AND ANNDEN = " + DtDenuncia.Rows[index2]["ANNDEN"]?.ToString() + " AND MESDEN = " + DtDenuncia.Rows[index2]["MESDEN"]?.ToString() + " AND PRODEN = " + DtDenuncia.Rows[index2]["PRODEN"]?.ToString() + " AND MAT = " + DtDenuncia.Rows[index2]["MAT"]?.ToString() + " AND PRODENDET = " + DtDenuncia.Rows[index2]["PRODENDET"]?.ToString();
                blnCommit = dataLayer.WriteTransactionData(strSQL, CommandType.Text);
                DtSessione.Rows[index1]["NUMGGSOS"] = DtDenuncia.Rows[index2]["NUMGGSOS"];
                DtSessione.Rows[index1]["NUMGGFIG"] = DtDenuncia.Rows[index2]["NUMGGFIG"];
                DtSessione.Rows[index1]["NUMGGAZI"] = DtDenuncia.Rows[index2]["NUMGGAZI"];
                DtSessione.Rows[index1]["NUMGGPER"] = DtDenuncia.Rows[index2]["NUMGGPER"];
                DtSessione.Rows[index1]["NUMGGDOM"] = DtDenuncia.Rows[index2]["NUMGGDOM"];
                DtSessione.Rows[index1]["NUMGGCONAZI"] = DtDenuncia.Rows[index2]["NUMGGCONAZI"];
              }
              else if (DtSessione.Rows[index1]["IMPFIG"] != DtDenuncia.Rows[index2]["IMPFIG"])
                DtSessione.Rows[index1]["IMPFIG"] = DtDenuncia.Rows[index2]["IMPFIG"];
              if (DtDenuncia.Rows[index2]["DATERO"] != DBNull.Value)
                DtSessione.Rows[index1]["DATERO"] = (object) Convert.ToDateTime(DtDenuncia.Rows[index2]["DATERO"]).GetDateTimeFormats((IFormatProvider) this.objDTFI)[0];
              DtSessione.Rows[index1]["MOD"] = (object) "";
            }
          }
        }
        dataLayer.EndTransaction(blnCommit);
        DataView defaultView = DtDenuncia.DefaultView;
        for (int index3 = 0; index3 <= num2; ++index3)
        {
          if (DtSessione.Rows[index3]["MOD"].ToString() == "S")
          {
            defaultView.RowFilter = "";
            defaultView.RowFilter = "MAT = " + DtSessione.Rows[index3]["MAT"]?.ToString();
            if (defaultView.Count > 0)
            {
              for (int index4 = 0; index4 <= num2; ++index4)
              {
                if (DtSessione.Rows[index4]["MAT"] == DtSessione.Rows[index3]["MAT"] && DtSessione.Rows[index4]["PRORAP"] == DtSessione.Rows[index3]["PRORAP"])
                {
                  DtSessione.Rows[index4]["IMPRET"] = (object) "0,00";
                  DtSessione.Rows[index4]["IMPOCC"] = (object) "0,00";
                  DtSessione.Rows[index4]["IMPCON"] = (object) "0,00";
                  DtSessione.Rows[index4]["MOD"] = (object) "S";
                  ++num1;
                }
              }
            }
            else
              DtSessione.Rows[index3]["MOD"] = (object) "";
          }
        }
      }
      return num1 == 0;
    }

    public DenunciaArretrati SalvaArretrati(
      string radio,
      string txtDataDenuncia,
      List<DenunciaArretrati> ListaDenunce,
      ref string ErrorMSG,
      ref string SuccessMSG)
    {
      this.RicercaArretrato();
      DenunciaArretrati denunciaArretrati = new DenunciaArretrati();
      TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente) HttpContext.Current.Session["utente"];
      object obj = HttpContext.Current.Session["CurTimeStampArretrati"];
      string CODPOS = utente.CodPosizione.ToString();
      string tipo = utente.Tipo;
      DataTable dataTable1 = new DataTable();
      string str1 = "";
      bool flag = false;
      string TIPDEN = "";
      string RICSANUTE = "";
      int ANNDEN = 0;
      int MESDEN = 0;
      int ANNCOM = 0;
      this.listaDenuncia.blnCommit = false;
      Decimal num1 = 0M;
      int PRODEN = 0;
      int num2 = 0;
      int intProDenDet = 0;
      this.listaDenuncia.txtDataDenuncia = txtDataDenuncia;
      DataLayer db = new DataLayer();
      if (this.listaDenuncia.hdnProden == null)
        this.listaDenuncia.hdnProden = "";
      denunciaArretrati.rbtnEvasione = radio == "rbEvasione_ON";
      denunciaArretrati.rbtnOmissione = radio == "rbOmissione_ON";
      denunciaArretrati.rbtnEscludi = radio == "rbEscludi_ON";
      try
      {
        RICSANUTE = GetRicSanUte(tipo, denunciaArretrati);
        
        string GetRicSanUte(string tipoUtente, DenunciaArretrati denuncia)
        {
          if (tipoUtente != "E") return "OMISSIONE";
          if (denuncia.rbtnOmissione) return "OMISSIONE";
          if (denuncia.rbtnEvasione) return "EVASIONE";
          if (denuncia.rbtnEscludi) return "ESCLUDI";
          return "RITARDO";
        }

        int num3;
        if (this.listaDenuncia.hdnProden != "")
        {
          ANNDEN = Convert.ToInt32(this.listaDenuncia.hdnAnno);
          MESDEN = Convert.ToInt32(this.listaDenuncia.hdnMese);
          PRODEN = Convert.ToInt32(this.listaDenuncia.hdnProden);
          string[] strArray = new string[6]
          {
            "SELECT CHAR(ULTAGG) AS ULTAGG FROM DENTES WHERE CODPOS = " + CODPOS,
            " AND ANNDEN = ",
            null,
            null,
            null,
            null
          };
          num3 = this.listaDenuncia.hdnAnno;
          strArray[2] = num3.ToString();
          strArray[3] = " AND MESDEN = ";
          num3 = this.listaDenuncia.hdnMese;
          strArray[4] = num3.ToString();
          strArray[5] = " AND PRODEN = ";
          string strSQL = string.Concat(strArray) + this.listaDenuncia.hdnProden + " AND DATMOVANN IS NULL AND STADEN = 'N'";
          str1 = db.Get1ValueFromSQL(strSQL, CommandType.Text) ?? "";
          if (DateTime.Parse(this.listaDenuncia.txtDataDenuncia).Month != MESDEN | DateTime.Parse(this.listaDenuncia.txtDataDenuncia).Year != ANNDEN)
            flag = true;
        }
        if (str1 != "" && str1 != HttpContext.Current.Session["CurTimeStampArretrati"].ToString())
        {
          ErrorMSG = "I dati sono stati modificati da un altro utente. Impossibile continuare.";
        }
        else
        {
          db.StartTransaction();
          DateTime today;
          if (this.listaDenuncia.hdnProden == "" || flag)
          {
            today = DateTime.Parse(this.listaDenuncia.txtDataDenuncia);
            ANNDEN = today.Year;
            today = DateTime.Parse(this.listaDenuncia.txtDataDenuncia);
            MESDEN = today.Month;
            string DATAPE = DBMethods.Db2Date(this.listaDenuncia.txtDataDenuncia);
            string strSQL = "SELECT TIPISC FROM AZISTO WHERE CODPOS = " + CODPOS + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(this.listaDenuncia.txtDataDenuncia)) + " BETWEEN DATINI AND " + "VALUE(DATFIN, '9999-12-31')";
            TIPDEN = db.Get1ValueFromSQL(strSQL, CommandType.Text);
            PRODEN = WriteDIPA.WRITE_INSERT_DENTES(db, utente.Username, CODPOS, ANNDEN, MESDEN, DATAPE, tipo.Trim(), "", "", "AR", TIPDEN, "N", 0.00M, 0.00M, 0.00M, 0.00M, 0.00M, 0.00M, 0.00M, 0.00M, 0.00M, "", "", "", 0, "", "0", 0, 0.00M, "", "", "", "", 0M, 0M, "", 0, 0, "N", RICSANUTE, "");
          }
          else
          {
            string strSQL = "UPDATE DENTES SET RICSANUTE = " + (!(RICSANUTE != "") ? "Null" : DBMethods.DoublePeakForSql(RICSANUTE)) + " WHERE CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND " + "PRODEN = " + PRODEN.ToString();
            this.listaDenuncia.blnCommit = db.WriteTransactionData(strSQL, CommandType.Text);
          }
          if (PRODEN > 0)
          {
            if (this.listaDenuncia.hdnAnnComp != "")
              ANNCOM = Convert.ToInt32(this.listaDenuncia.hdnAnnComp);
            this.listaDenuncia.blnCommit = WriteDIPA.WRITE_DELETE_DENDET(db, CODPOS, ANNDEN, MESDEN, PRODEN, ANNCOM, "AR");
            if (this.listaDenuncia.blnCommit)
            {
              DataTable dataTable2 = (DataTable) HttpContext.Current.Session["tbArretrati"];
              for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
              {
                if (Convert.ToInt32(dataTable2.Rows[index]["MAT"]) == ListaDenunce[index].mat)
                {
                  dataTable2.Rows[index]["IMPRET"] = (object) Convert.ToDecimal(ListaDenunce[index].impret);
                  dataTable2.Rows[index]["IMPOCC"] = (object) Convert.ToDecimal(ListaDenunce[index].impocc);
                  dataTable2.Rows[index]["IMPCON"] = (object) Convert.ToDecimal(ListaDenunce[index].impcon);
                }
              }
              for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
              {
                this.listaDenuncia.blnCommit = WriteDIPA.WRITE_INSERT_DENDET(db, (List<RetribuzioneRDL>) null, (List<ParametriGenerali>) null, utente, CODPOS, ANNDEN, MESDEN, PRODEN, Convert.ToInt32(dataTable2.Rows[index]["MAT"]), "AR", dataTable2.Rows[index]["DAL"].ToString(), dataTable2.Rows[index]["AL"].ToString(), "", Convert.ToDecimal(dataTable2.Rows[index]["IMPRET"]), Convert.ToDecimal(dataTable2.Rows[index]["IMPOCC"]), 0M, 0M, 0M, Convert.ToDecimal(dataTable2.Rows[index]["IMPCON"]), Convert.ToDecimal(0.0), dataTable2.Rows[index]["PREV"].ToString(), Convert.ToInt32(dataTable2.Rows[index]["PROMOD"]), dataTable2.Rows[index]["DATDEC"].ToString(), dataTable2.Rows[index]["DATCES"].ToString(), Convert.ToDecimal(0.0), 0M, Convert.ToDecimal(0.0), Convert.ToDecimal(0.0), Convert.ToDecimal(0.0), Convert.ToDecimal(0.0), Convert.ToDecimal(dataTable2.Rows[index]["IMPSCA"]), Convert.ToDecimal(dataTable2.Rows[index]["IMPTRAECO"]), dataTable2.Rows[index]["ETA65"].ToString(), Convert.ToInt32(dataTable2.Rows[index]["TIPRAP"]), dataTable2.Rows[index]["FAP"].ToString(), Convert.ToDecimal(dataTable2.Rows[index]["PERFAP"].ToString()), Convert.ToDecimal(dataTable2.Rows[index]["IMPFAP"]), Convert.ToDecimal(dataTable2.Rows[index]["PERPAR"]), Convert.ToDecimal(dataTable2.Rows[index]["PERAPP"]), Convert.ToInt32(dataTable2.Rows[index]["PRORAP"]), Convert.ToInt32(dataTable2.Rows[index]["CODCON"]), Convert.ToInt32(dataTable2.Rows[index]["PROCON"]), dataTable2.Rows[index]["TIPSPE"].ToString(), Convert.ToInt32(dataTable2.Rows[index]["CODLOC"]), Convert.ToInt32(dataTable2.Rows[index]["PROLOC"]), Convert.ToInt32(dataTable2.Rows[index]["CODLIV"]), Convert.ToInt32(dataTable2.Rows[index]["CODGRUASS"]), Convert.ToInt32(dataTable2.Rows[index]["CODQUACON"]), Convert.ToDecimal(dataTable2.Rows[index]["ALIQUOTA"]), dataTable2.Rows[index]["DATNAS"].ToString(), Convert.ToInt32(dataTable2.Rows[index]["ANNCOM"]), TIPDEN, ref intProDenDet);
                if (this.listaDenuncia.blnCommit)
                  ++num2;
                else
                  break;
              }
              if (this.listaDenuncia.blnCommit)
              {
                if (flag)
                {
                  string str2 = "DELETE FROM DENDET WHERE CODPOS = " + CODPOS + " AND ANNDEN = ";
                  string[] strArray1 = new string[6];
                  strArray1[0] = str2;
                  num3 = this.listaDenuncia.hdnAnno;
                  strArray1[1] = num3.ToString();
                  strArray1[2] = " AND MESDEN = ";
                  num3 = this.listaDenuncia.hdnMese;
                  strArray1[3] = num3.ToString();
                  strArray1[4] = " AND PRODEN = ";
                  strArray1[5] = this.listaDenuncia.hdnProden;
                  string strSQL1 = string.Concat(strArray1);
                  this.listaDenuncia.blnCommit = db.WriteTransactionData(strSQL1, CommandType.Text);
                  if (this.listaDenuncia.blnCommit)
                  {
                    string[] strArray2 = new string[5]
                    {
                      "DELETE FROM DENTES WHERE CODPOS = " + CODPOS + " AND ANNDEN = ",
                      null,
                      null,
                      null,
                      null
                    };
                    num3 = this.listaDenuncia.hdnAnno;
                    strArray2[1] = num3.ToString();
                    strArray2[2] = " AND MESDEN = ";
                    num3 = this.listaDenuncia.hdnMese;
                    strArray2[3] = num3.ToString();
                    strArray2[4] = " AND PRODEN = ";
                    string strSQL2 = string.Concat(strArray2) + this.listaDenuncia.hdnProden + " AND DATMOVANN IS NULL AND STADEN = 'N'";
                    this.listaDenuncia.blnCommit = db.WriteTransactionData(strSQL2, CommandType.Text);
                  }
                }
                if (this.listaDenuncia.blnCommit)
                {
                  string strSQL3 = "SELECT SUM(IMPRET) AS IMPRET, SUM(IMPOCC) AS IMPOCC, SUM(IMPFIG) AS IMPFIG, SUM(IMPCON) " + "AS IMPCON, SUM(IMPABB) AS IMPABB, SUM(IMPASSCON) AS IMPASSCON FROM DENDET WHERE " + "CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND TIPMOV = 'AR'";
                  DataTable dataTable3 = db.GetDataTable(strSQL3);
                  if (dataTable3.Rows.Count > 0)
                  {
                    string strSQL4 = "UPDATE DENTES SET IMPRET = " + dataTable3.Rows[0]["IMPRET"].ToString().Replace(",", ".") + ", IMPOCC = " + dataTable3.Rows[0]["IMPOCC"].ToString().Replace(",", ".") + ", IMPFIG = " + dataTable3.Rows[0]["IMPFIG"].ToString().Replace(",", ".") + ", IMPCON = " + dataTable3.Rows[0]["IMPCON"].ToString().Replace(",", ".") + ", IMPABB = " + dataTable3.Rows[0]["IMPABB"].ToString().Replace(",", ".") + ", IMPASSCON = " + dataTable3.Rows[0]["IMPASSCON"].ToString().Replace(",", ".") + ", NUMRIGDET = " + num2.ToString() + " WHERE CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND TIPMOV = 'AR'";
                    this.listaDenuncia.blnCommit = db.WriteTransactionData(strSQL4, CommandType.Text);
                    if (this.listaDenuncia.blnCommit)
                    {
                      today = DateTime.Today;
                      Decimal importoParametro = this.GetImportoParametro((short) 5, today.GetDateTimeFormats((IFormatProvider) this.objDTFI)[0]);
                      string strSQL5 = "SELECT IMPCON FROM DENDET WHERE CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND TIPMOV = 'AR'";
                      DataTable dataTable4 = db.GetDataTable(strSQL5);
                      for (int index = 0; index <= dataTable4.Rows.Count - 1; ++index)
                        num1 += Convert.ToDecimal(dataTable4.Rows[index]["IMPCON"]) / 100M * importoParametro;
                      string strSQL6 = "UPDATE DENTES SET IMPADDREC = " + num1.ToString().Replace(",", ".") + " WHERE CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND TIPMOV = 'AR'";
                      this.listaDenuncia.blnCommit = db.WriteTransactionData(strSQL6, CommandType.Text);
                      if (this.listaDenuncia.blnCommit)
                      {
                        string strSQL7 = "UPDATE DENTES SET IMPDIS = IMPCON + IMPASSCON + IMPADDREC + IMPABB" + " WHERE CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND TIPMOV = 'AR'";
                        this.listaDenuncia.blnCommit = db.WriteTransactionData(strSQL7, CommandType.Text);
                      }
                    }
                  }
                }
              }
            }
          }
          db.EndTransaction(this.listaDenuncia.blnCommit);
          if (this.listaDenuncia.blnCommit)
          {
            string strSQL = "SELECT CHAR(ULTAGG) AS ULTAGG FROM DENTES WHERE CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND " + "PRODEN = " + PRODEN.ToString() + " AND DATMOVANN IS NULL AND STADEN = 'N'";
            HttpContext.Current.Session["CurTimeStampArretrati"] = (object) (db.Get1ValueFromSQL(strSQL, CommandType.Text) ?? "");
            if (this.listaDenuncia.hdnProden == "" | flag)
            {
              this.listaDenuncia.hdnAnno = ANNDEN;
              this.listaDenuncia.hdnMese = MESDEN;
              this.listaDenuncia.hdnProden = PRODEN.ToString();
            }
          }
        }
      }
      catch (Exception ex)
      {
        db.EndTransaction(false);
        HttpContext.Current.Session["LastException"] = (object) ex;
        ErrorMSG = ex.ToString();
      }
      finally
      {
        if (this.listaDenuncia.blnCommit)
        {
          SuccessMSG = "Operazione effettuata con successo!";
          this.listaDenuncia.codpos = Convert.ToInt32(CODPOS);
        }
        else
          ErrorMSG = "Si sono verificati dei problemi nel salvataggio della denuncia!";
      }
      return this.listaDenuncia;
    }

    public void Page_Load()
    {
      this.RicercaArretrato();
      if (this.listaDenuncia.hdnProden == null)
      {
        this.listaDenuncia.hdnProden = "";
      }
      else
      {
        if (this.listaDenuncia.hdnProden == "0")
          this.listaDenuncia.hdnProden = "";
        else
          this.listaDenuncia.lblPeriodo = !(this.listaDenuncia.hdnProden == "") ? "Denuncia Arretrati" : "Inserimento Denuncia Arretrati";
        if (this.listaDenuncia.hdnProden != "")
          this.LoadData(Convert.ToInt32(this.listaDenuncia.hdnAnnComp), true);
      }
      TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente) HttpContext.Current.Session["utente"];
      utente.CodPosizione.ToString();
      string tipo = utente.Tipo;
      if (!(tipo == "A") && !(tipo == "C") && (tipo == null || tipo.Length != 0))
      {
        int num = tipo == "E" ? 1 : 0;
      }
      else
      {
        this.listaDenuncia.flagDatadenuncia = true;
        this.listaDenuncia.txtDataDenuncia = DateTime.Now.GetDateTimeFormats((IFormatProvider) this.objDTFI)[0];
      }
    }

    private void RicercaArretrato()
    {
      DataTable dataTable1 = new DataTable();
      bool flag = false;
      DataLayer dataLayer = new DataLayer();
      TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente) HttpContext.Current.Session["utente"];
      string str = utente.CodPosizione.ToString();
      string tipo = utente.Tipo;
      string strSQL = "SELECT ANNDEN, MESDEN, PRODEN, STADEN, TIPMOV FROM DENTES WHERE CODPOS = " + str + " AND STADEN = 'N' AND TIPMOV ='AR' AND DATMOVANN IS NULL";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
      if (DBNull.Value.Equals((object) dataTable2))
        return;
      if (dataTable2.Rows.Count > 0)
      {
        flag = true;
        this.listaDenuncia.hdnAnno = Convert.ToInt32(dataTable2.Rows[0]["ANNDEN"]);
        this.listaDenuncia.hdnMese = Convert.ToInt32(dataTable2.Rows[0]["MESDEN"]);
        this.listaDenuncia.hdnProden = dataTable2.Rows[0]["PRODEN"].ToString();
        this.listaDenuncia.hdnStaden = dataTable2.Rows[0]["STADEN"].ToString();
      }
      else
      {
        this.listaDenuncia.hdnAnno = 0;
        this.listaDenuncia.hdnMese = 0;
        this.listaDenuncia.hdnProden = "";
        this.listaDenuncia.hdnStaden = "";
      }
    }

    public List<DenunciaArretrati> VediArretrati(
      string codPos,
      int hdnAnno,
      int hdnMese,
      int hdnProden)
    {
      this.listaDenuncia.lblPeriodo = "Riepilogo Denuncia Arretrati presentata in data ";
      DataLayer dataLayer = new DataLayer();
      string strSQL1 = "SELECT B.MAT, (TRIM(COG) || ' ' || TRIM(NOM)) AS NOMINATIVO, B.IMPRET, B.IMPOCC, " + "B.IMPCON, ANNCOM FROM DENTES A INNER JOIN DENDET B ON A.CODPOS = B.CODPOS AND " + "A.ANNDEN = B.ANNDEN AND A.MESDEN = B.MESDEN AND A.PRODEN = B.PRODEN INNER JOIN " + "ISCT C ON B.MAT = C.MAT WHERE A.CODPOS = " + codPos + " AND A.ANNDEN = " + hdnAnno.ToString() + " AND A.MESDEN = " + hdnMese.ToString() + " AND A.PRODEN = " + hdnProden.ToString() + " AND VALUE(A.CODMODPAG, 0) = 0 AND A.TIPMOV = 'AR'" + " AND B.IMPRET <> 0 ORDER BY ANNCOM, MAT";
      DataTable dataTable = dataLayer.GetDataTable(strSQL1);
      string strSQL2 = "SELECT DATAPE FROM DENTES WHERE CODPOS = " + codPos + " AND ANNDEN = " + hdnAnno.ToString() + " AND MESDEN = " + hdnMese.ToString() + " AND PRODEN = " + hdnProden.ToString() + " AND VALUE(CODMODPAG, 0) = 0 AND TIPMOV = 'AR'";
      var denunciaTbl = dataLayer.GetDataTable(strSQL2);
      
      var datApe = DateTime.Parse(dataLayer.GetDataTable(strSQL2).Rows[0].ElementAt("DATAPE").Substring(0, 10))
        .GetDateTimeFormats((IFormatProvider) this.objDTFI)[0];
      this.listaDenuncia.lblPeriodo += datApe;
      List<DenunciaArretrati> denunciaArretratiList = new List<DenunciaArretrati>();
      foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
      {
        DenunciaArretrati denunciaArretrati = new DenunciaArretrati()
        {
          mat = Convert.ToInt32(row["MAT"]),
          nome = row["NOMINATIVO"].ToString(),
          impret = row["IMPRET"].ToString(),
          impocc = row["IMPOCC"].ToString(),
          impcon = row["IMPCON"].ToString(),
          anno = Convert.ToInt32(row["ANNCOM"]),
          lblPeriodo = this.listaDenuncia.lblPeriodo,
          DatApe = datApe
        };
        denunciaArretratiList.Add(denunciaArretrati);
      }
      return denunciaArretratiList;
    }
  }
}
