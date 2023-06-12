// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Utilities.ModGeneraNotifiche
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Data;
using TFI.DAL.ConnectorDB;

namespace TFI.DAL.Utilities
{
  public class ModGeneraNotifiche
  {
    private clsWRITE_DB clsWRITE_DB = new clsWRITE_DB();
    private Utile Utils = new Utile();
    private ModGetDati ModGetDati = new ModGetDati();

    public int Get_NumGG_Domeniche(DateTime DataDal, DateTime DataAl)
    {
      int numGgDomeniche = 0;
      for (; DataDal < DataAl; DataDal = DataDal.AddDays(1.0))
      {
        if (DataDal.DayOfWeek == DayOfWeek.Sunday)
          ++numGgDomeniche;
      }
      return numGgDomeniche;
    }

    public int Get_NumGG_Periodo(DateTime DataDal, DateTime DataAl) => Convert.ToDateTime(DataAl).Subtract(DataDal).Days + 1;

    public void Get_NumGG_Sospensioni(
      ref DataTable dtSos,
      DateTime DataDal,
      DateTime DataAl,
      int CODPOS,
      int MAT,
      int PRORAP,
      ref Decimal NUMGGAZI,
      ref Decimal NUMGGSOS,
      ref Decimal NUMGGFIG)
    {
      int num1 = 0;
      Decimal num2 = 0.0M;
      Decimal num3 = 0.0M;
      for (int index = 0; index <= dtSos.Rows.Count - 1; ++index)
      {
        int num4 = !DBNull.Value.Equals(dtSos.Rows[index][nameof (CODPOS)]) ? Convert.ToInt32(dtSos.Rows[index][nameof (CODPOS)]) : 0;
        int num5 = !DBNull.Value.Equals(dtSos.Rows[index][nameof (MAT)]) ? Convert.ToInt32(dtSos.Rows[index][nameof (MAT)]) : 0;
        int num6 = !DBNull.Value.Equals(dtSos.Rows[index][nameof (PRORAP)]) ? Convert.ToInt32(dtSos.Rows[index][nameof (PRORAP)]) : 0;
        int num7 = !DBNull.Value.Equals(dtSos.Rows[index]["PERAZI"]) ? Convert.ToInt32(dtSos.Rows[index]["PERAZI"]) : 0;
        int num8 = !DBNull.Value.Equals(dtSos.Rows[index]["PERFIG"]) ? Convert.ToInt32(dtSos.Rows[index]["PERFIG"]) : 0;
        DateTime dateTime;
        if (CODPOS == num4 && MAT == num5 && PRORAP == num6 && Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]) >= Convert.ToDateTime(DataDal) && Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"]) <= Convert.ToDateTime(DataAl))
        {
          if (DataDal >= Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"]))
          {
            if (DataAl <= Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]))
            {
              int numGgDomeniche = this.Get_NumGG_Domeniche(DataDal, DataAl);
              dateTime = Convert.ToDateTime(DataAl);
              num1 = dateTime.Subtract(DataDal).Days + 1 - numGgDomeniche;
              if (num7 > 0)
                num2 = (Decimal) (num1 * num7 / 100);
              if (num8 > 0)
              {
                num3 = (Decimal) (num1 * num8 / 100);
                break;
              }
              break;
            }
            int numGgDomeniche1 = this.Get_NumGG_Domeniche(DataDal, Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]));
            dateTime = Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]);
            num1 = dateTime.Subtract(DataDal).Days + 1 - numGgDomeniche1;
            if (num7 > 0)
              num2 = (Decimal) (num1 * num7 / 100);
            if (num8 > 0)
              num3 = (Decimal) (num1 * num8 / 100);
          }
          else
          {
            if (DataAl <= Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]))
            {
              int numGgDomeniche = this.Get_NumGG_Domeniche(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"]), DataAl);
              int num9 = num1;
              dateTime = Convert.ToDateTime(DataAl);
              int num10 = dateTime.Subtract(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"])).Days + 1 - numGgDomeniche;
              num1 = num9 + num10;
              TimeSpan timeSpan;
              if (num7 > 0)
              {
                Decimal num11 = num2;
                dateTime = Convert.ToDateTime(DataAl);
                timeSpan = dateTime.Subtract(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"]));
                Decimal num12 = (Decimal) (timeSpan.Days + 1 - numGgDomeniche * num7 / 100);
                num2 = num11 + num12;
              }
              if (num8 > 0)
              {
                Decimal num13 = num3;
                dateTime = Convert.ToDateTime(DataAl);
                timeSpan = dateTime.Subtract(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"]));
                Decimal num14 = (Decimal) (timeSpan.Days + 1 - numGgDomeniche * num8 / 100);
                num3 = num13 + num14;
                break;
              }
              break;
            }
            int numGgDomeniche2 = this.Get_NumGG_Domeniche(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"]), Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]));
            int num15 = num1;
            dateTime = Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]);
            int num16 = dateTime.Subtract(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"])).Days + 1 - numGgDomeniche2;
            num1 = num15 + num16;
            if (num7 > 0)
            {
              Decimal num17 = num2;
              dateTime = Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]);
              Decimal num18 = (Decimal) (dateTime.Subtract(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"])).Days + 1 - numGgDomeniche2 * num7 / 100);
              num2 = num17 + num18;
            }
            if (num8 > 0)
            {
              Decimal num19 = num3;
              dateTime = Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]);
              Decimal num20 = (Decimal) (dateTime.Subtract(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"])).Days + 1 - numGgDomeniche2 * num8 / 100);
              num3 = num19 + num20;
            }
          }
        }
      }
      NUMGGSOS = (Decimal) num1;
      NUMGGFIG = num3;
      NUMGGAZI = num2;
    }

    public bool Module_SalvaNotifiche(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      ref DataTable dtAziende,
      ref DataTable dt,
      string TIPMOVSAN,
      string DATA_RIFERIMENTO)
    {
      try
      {
        string str1 = "";
        string RICSANUTE = TIPMOVSAN == "SAN_MD" ? "EVASIONE" : (TIPMOVSAN == "SAN_RD" ? "RITARDO" : "ESCLUDI");
        DataTable dataTable1 = new DataTable();
        Decimal num1 = 0.0M;
        Decimal num2 = 0.0M;
        string str2 = "";
        int num3 = 0;
        Decimal TASSO = 0.0M;
        Decimal num4 = 0.0M;
        Decimal num5 = 0.0M;
        Decimal num6 = 0.0M;
        Decimal num7 = 0.0M;
        if (RICSANUTE != "ESCLUDI")
          str1 = this.ModGetDati.Module_Get_CAUSALE_MOVIMENTO(objDataAccess, TIPMOVSAN);
        string str3 = objDataAccess.Get1ValueFromSQL("SELECT CURRENT_TIMESTAMP AS NEWTIMESTAMP FROM DBUNICONET.TIPIND", CommandType.Text);
        DateTime dateTime;
        if (DATA_RIFERIMENTO != "")
        {
          string str4 = str3;
          dateTime = this.Utils.Module_GetDataSistema();
          string oldValue = dateTime.ToString();
          string newValue = DATA_RIFERIMENTO;
          str3 = str4.Replace(oldValue, newValue);
        }
        for (int index1 = 0; index1 <= dtAziende.Rows.Count - 1; ++index1)
        {
          bool flag = false;
          for (int index2 = 0; index2 <= dt.Rows.Count - 1; ++index2)
          {
            if (dtAziende.Rows[index1]["CODPOS"].ToString().Trim() + dtAziende.Rows[index1]["ANNO"].ToString().Trim() + dtAziende.Rows[index1]["MESE"].ToString().Trim() == dt.Rows[index2]["CODPOS"].ToString().Trim() + dt.Rows[index2]["ANNDEN"].ToString().Trim() + dt.Rows[index2]["MESDEN"].ToString().Trim())
              flag = true;
          }
          if (flag)
          {
            Decimal num8 = Convert.ToDecimal(this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 5, Convert.ToString(this.Utils.Module_GetDataSistema()), Convert.ToInt32(dtAziende.Rows[index1]["CODPOS"])));
            int int32_1 = Convert.ToInt32(dtAziende.Rows[index1]["ANNO"]);
            int int32_2 = Convert.ToInt32(dtAziende.Rows[index1]["MESE"]);
            string strSQL1 = "SELECT VALUE(VALFAP, 0) AS VALFAP FROM CODFAP WHERE " + DBMethods.DoublePeakForSql(int32_1.ToString() + "-" + int32_2.ToString() + "-01") + " BETWEEN DATINI AND DATFIN";
            Decimal num9 = Convert.ToDecimal(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
            string str5 = "01/" + int32_2.ToString().Trim().PadLeft(2, '0') + "/" + int32_1.ToString();
            string strSQL2 = "DELETE FROM DENDETALI WHERE CODPOS=" + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN=" + int32_1.ToString() + " AND MESDEN=" + int32_2.ToString() + " AND PRODEN IN (SELECT PRODEN FROM DENTES WHERE CODPOS=" + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN=" + int32_1.ToString() + " AND MESDEN=" + int32_2.ToString() + " AND TIPMOV='NU' AND DATCONMOV IS NULL AND DATMOVANN IS NULL) ";
            objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
            string strSQL3 = "DELETE FROM DENDETSOS WHERE CODPOS=" + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN=" + int32_1.ToString() + " AND MESDEN=" + int32_2.ToString() + " AND PRODEN IN (SELECT PRODEN FROM DENTES WHERE CODPOS=" + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN=" + int32_1.ToString() + " AND MESDEN=" + int32_2.ToString() + " AND TIPMOV='NU' AND DATCONMOV IS NULL AND DATMOVANN IS NULL) ";
            objDataAccess.WriteTransactionData(strSQL3, CommandType.Text);
            string strSQL4 = "DELETE FROM DENDET WHERE CODPOS=" + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN=" + int32_1.ToString() + " AND MESDEN=" + int32_2.ToString() + " AND PRODEN IN(SELECT PRODEN FROM DENTES WHERE CODPOS=" + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN=" + int32_1.ToString() + " AND MESDEN=" + int32_2.ToString() + " AND TIPMOV='NU' AND DATCONMOV IS NULL AND DATMOVANN IS NULL) ";
            objDataAccess.WriteTransactionData(strSQL4, CommandType.Text);
            string strSQL5 = "DELETE FROM DENTES WHERE CODPOS=" + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN=" + int32_1.ToString() + " AND MESDEN=" + int32_2.ToString() + " AND TIPMOV='NU' AND DATCONMOV IS NULL AND DATMOVANN IS NULL";
            objDataAccess.WriteTransactionData(strSQL5, CommandType.Text);
            if (dtAziende.Rows[index1]["RIMUOVI"].ToString() == "NO")
            {
              string str6 = (string) this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 1, str5);
              if (str6 == "")
                str6 = "0";
              int num10 = 0;
              num4 = 0.0M;
              TASSO = 0.0M;
              str2 = "";
              num5 = 0.0M;
              dateTime = Convert.ToDateTime(this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 3, str5).ToString());
              dateTime = dateTime.AddDays(1.0);
              string str7 = dateTime.ToString();
              string str8 = str3;
              if (RICSANUTE != "ESCLUDI")
              {
                dateTime = Convert.ToDateTime(str8);
                num3 = dateTime.Subtract(Convert.ToDateTime(str7)).Days + 1;
                if (TIPMOVSAN == "SAN_RD" || TIPMOVSAN == "SAN_MD")
                {
                  string strSQL6 = "SELECT VALUE(GIORNI, 0) AS GIORNI, VALUE(TASSO, 0.0) AS TASSO, VALUE(GGSOGLIA, 0) AS GGSOGLIA, VALUE(IMPSOGLIA, 0.0) AS IMPSOGLIA, VALUE(PERMAXSOGLIA, 0.0) AS PERMAXSOGLIA " + " FROM TIPMOVCAU WHERE TIPMOV = '" + TIPMOVSAN + "' AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str5)) + " BETWEEN DATINI AND DATFIN ";
                  dataTable1.Clear();
                  DataTable dataTable2 = objDataAccess.GetDataTable(strSQL6);
                  if (dataTable2.Rows.Count > 0)
                  {
                    if (TIPMOVSAN == "SAN_MD")
                      num3 += Convert.ToInt32(dataTable2.Rows[0]["GIORNI"]);
                    TASSO = Convert.ToDecimal(dataTable2.Rows[0]["TASSO"]);
                    num10 = Convert.ToInt32(dataTable2.Rows[0]["GGSOGLIA"]);
                    num4 = Convert.ToDecimal(dataTable2.Rows[0]["IMPSOGLIA"]);
                    num5 = Convert.ToDecimal(dataTable2.Rows[0]["PERMAXSOGLIA"]);
                  }
                }
              }
              int PRODEN = this.clsWRITE_DB.WRITE_INSERT_DENTES(objDataAccess, u, Convert.ToInt32(dtAziende.Rows[index1]["CODPOS"]), int32_1, int32_2, "NU", dtAziende.Rows[index1]["TIPISC"].ToString(), "N", str3, "E", str3, "E", "0", dtAziende.Rows[index1]["DATSCA"].ToString(), RICSANUTE);
              Decimal num11;
              for (int index3 = 0; index3 <= dt.Rows.Count - 1; ++index3)
              {
                if (dtAziende.Rows.Count == 1 && dt.Rows[index3]["CODPOS"] == dtAziende.Rows[index1]["CODPOS"] && dt.Rows[index3]["ANNDEN"] == dtAziende.Rows[index1]["ANNO"] && dt.Rows[index3]["MESDEN"] == dtAziende.Rows[index1]["MESE"])
                {
                  string PREV = !(dt.Rows[index3]["PREV"].ToString().Trim() == "S") ? "N" : "S";
                  int num12 = this.clsWRITE_DB.WRITE_INSERT_DENDET(objDataAccess, u, Convert.ToInt32(dt.Rows[index3]["CODPOS"]), int32_1, int32_2, PRODEN, Convert.ToInt32(dt.Rows[index3]["MAT"]), dt.Rows[index3]["DAL"].ToString(), dt.Rows[index3]["AL"].ToString(), dt.Rows[index3]["FAP"].ToString().Trim(), dt.Rows[index3]["PERFAP"].ToString().Trim(), Convert.ToDecimal(dt.Rows[index3]["IMPRET"]), Convert.ToDecimal(dt.Rows[index3]["IMPOCC"]), Convert.ToDecimal(dt.Rows[index3]["IMPFIG"]), Convert.ToDecimal(dt.Rows[index3]["IMPABB"]), Convert.ToDecimal(dt.Rows[index3]["IMPASSCON"]), Convert.ToDecimal(dt.Rows[index3]["IMPCON"]), Convert.ToDecimal(dt.Rows[index3]["IMPMIN"]), PREV, Convert.ToInt32(dt.Rows[index3]["PRORAP"]), Convert.ToInt32(dt.Rows[index3]["CODCON"]), Convert.ToInt32(dt.Rows[index3]["PROCON"]), Convert.ToInt32(dt.Rows[index3]["CODLOC"]), Convert.ToInt32(dt.Rows[index3]["PROLOC"]), Convert.ToInt32(dt.Rows[index3]["CODLIV"]), Convert.ToInt32(dt.Rows[index3]["CODQUACON"]), dt.Rows[index3]["DATNAS"].ToString().Trim(), dt.Rows[index3]["ETAENNE"].ToString().Trim(), "NU", dt.Rows[index3]["DATDEC"].ToString().Trim(), dt.Rows[index3]["DATCES"].ToString().Trim(), Convert.ToDecimal(dt.Rows[index3]["NUMGGAZI"]), Convert.ToDecimal(dt.Rows[index3]["NUMGGFIG"]), Convert.ToDecimal(dt.Rows[index3]["NUMGGPER"]), Convert.ToDecimal(dt.Rows[index3]["NUMGGDOM"]), Convert.ToDecimal(dt.Rows[index3]["NUMGGSOS"]), Convert.ToDecimal(dt.Rows[index3]["NUMGGCONAZI"]), Convert.ToDecimal(dt.Rows[index3]["IMPSCA"]), Convert.ToDecimal(dt.Rows[index3]["IMPTRAECO"]), dt.Rows[index3]["TIPRAP"].ToString().Trim(), Convert.ToDecimal(dt.Rows[index3]["PERPAR"]), Convert.ToDecimal(dt.Rows[index3]["PERAPP"]), dt.Rows[index3]["TIPSPE"].ToString().Trim(), Convert.ToInt32(dt.Rows[index3]["CODGRUASS"]), Convert.ToDecimal(dt.Rows[index3]["ALIQUOTA"]), dtAziende.Rows[index1]["TIPISC"].ToString().Trim(), str7.Trim(), str8.Trim(), TASSO);
                  string strSQL7 = "INSERT INTO DENDETSOS(" + "CODPOS, ANNDEN, MESDEN, PRODEN, MAT, PRODENDET, " + "PRORAP, CODSOS, DATINISOS, DATFINSOS, PERAZI,  " + "PERCFIG, ULTAGG, UTEAGG)" + " SELECT " + "CODPOS , " + int32_1.ToString() + " AS ANNDEN , " + int32_2.ToString() + " AS MESDEN , " + PRODEN.ToString() + " AS PRODEN , " + "MAT , " + num12.ToString() + " AS PRODENDET , " + "PRORAP, CODSOS, DATINISOS, DATFINSOS, PERAZI, " + "PERFIG, CURRENT_TIMESTAMP, " + " " + DBMethods.DoublePeakForSql(u.Username) + " AS UTEAGG " + " FROM SOSRAP WHERE " + " CODPOS = " + dt.Rows[index3]["CODPOS"]?.ToString() + " AND MAT = " + dt.Rows[index3]["MAT"]?.ToString() + " AND PRORAP = " + dt.Rows[index3]["PRORAP"]?.ToString() + " AND DATINISOS <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dt.Rows[index3]["AL"].ToString())) + " AND " + " VALUE(DATFINSOS,'9999-12-31') >= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dt.Rows[index3]["DAL"].ToString())) + " AND STASOS = '0'";
                  objDataAccess.WriteTransactionData(strSQL7, CommandType.Text);
                  string str9 = "INSERT INTO DENDETALI( " + "CODPOS , ANNDEN , MESDEN, PRODEN, MAT, " + "PRODENDET , CODFORASS, ALIQUOTA, IMPCON, ULTAGG,UTEAGG) " + " SELECT " + dt.Rows[index3]["CODPOS"]?.ToString() + " AS CODPOS , " + int32_1.ToString() + " AS ANNDEN , " + int32_2.ToString() + " AS MESDEN , " + PRODEN.ToString() + " AS PRODEN , " + dt.Rows[index3]["MAT"]?.ToString() + " AS MAT , " + num12.ToString() + " AS PRODENDET , " + " CODFORASS, ALIQUOTA, " + " (ALIQUOTA * " + dt.Rows[index3]["IMPRET"].ToString().Replace(",", ".") + "/100) AS IMPCON, " + " CURRENT_TIMESTAMP, " + " " + DBMethods.DoublePeakForSql(u.Username) + " AS UTEAGG " + " FROM ALIFORASS " + " WHERE CODGRUASS= " + dt.Rows[index3]["CODGRUASS"]?.ToString() + " AND CODQUACON = " + dt.Rows[index3]["CODQUACON"]?.ToString() + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dt.Rows[index3]["DAL"].ToString())) + " BETWEEN DATINI AND VALUE(DATFIN,'9999-12-31')";
                  string strSQL8 = !(dt.Rows[index3]["ETAENNE"].ToString() == "N") ? str9 + " AND CODFORASS NOT IN(SELECT CODFORASS FROM FORASS " + "WHERE CATFORASS IN('PREV', 'FAP' , 'TFR'))" : str9 + " AND CODFORASS NOT IN(SELECT CODFORASS FROM FORASS " + "WHERE CATFORASS IN( 'FAP', 'TFR'))";
                  objDataAccess.WriteTransactionData(strSQL8, CommandType.Text);
                  string str10 = "INSERT INTO DENDETALI( " + "CODPOS , ANNDEN , MESDEN, PRODEN, MAT, " + "PRODENDET , CODFORASS, ALIQUOTA, IMPCON, ULTAGG,UTEAGG) " + " SELECT " + dt.Rows[index3]["CODPOS"]?.ToString() + " AS CODPOS , " + int32_1.ToString() + " AS ANNDEN , " + int32_2.ToString() + " AS MESDEN , " + PRODEN.ToString() + " AS PRODEN , " + dt.Rows[index3]["MAT"]?.ToString() + " AS MAT , " + num12.ToString() + " AS PRODENDET , " + " CODFORASS, ";
                  string str11;
                  if (dt.Rows[index3]["FAP"].ToString().Trim() == "S")
                    str11 = str10 + " ALIQUOTA + " + num9.ToString().Replace(",", ".") + ", " + " ((ALIQUOTA + " + num9.ToString().Replace(",", ".") + ") * " + dt.Rows[index3]["IMPRET"].ToString().Replace(",", ".") + "/100) AS IMPCON, ";
                  else
                    str11 = str10 + " ALIQUOTA, " + " (ALIQUOTA * " + dt.Rows[index3]["IMPRET"].ToString().Replace(",", ".") + "/100) AS IMPCON, ";
                  string strSQL9 = str11 + " CURRENT_TIMESTAMP, " + " " + DBMethods.DoublePeakForSql(u.Username) + " AS UTEAGG " + " FROM ALIFORASS " + " WHERE CODGRUASS= " + dt.Rows[index3]["CODGRUASS"]?.ToString() + " AND CODQUACON = " + dt.Rows[index3]["CODQUACON"]?.ToString() + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dt.Rows[index3]["DAL"].ToString())) + " BETWEEN DATINI AND VALUE(DATFIN,'9999-12-31')" + " AND CODFORASS IN (SELECT CODFORASS FROM FORASS " + " WHERE CATFORASS = 'TFR')";
                  objDataAccess.WriteTransactionData(strSQL9, CommandType.Text);
                  string str12 = "UPDATE DENDET SET IMPFAP = ";
                  string str13;
                  if (dt.Rows[index3]["FAP"].ToString().Trim() == "S")
                  {
                    string str14 = str12;
                    num11 = Convert.ToDecimal(num9.ToString().Replace(",", ".")) * Convert.ToDecimal(dt.Rows[index3]["IMPRET"].ToString().Replace(",", ".")) / 100M;
                    string str15 = num11.ToString();
                    str13 = str14 + str15;
                  }
                  else
                    str13 = str12 + "0";
                  string strSQL10 = str13 + " WHERE CODPOS = " + dt.Rows[index3]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN = " + int32_2.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND MAT = " + dt.Rows[index3]["MAT"]?.ToString() + " AND PRODENDET = " + num12.ToString();
                  objDataAccess.WriteTransactionData(strSQL10, CommandType.Text);
                  string strSQL11 = "SELECT VALUE(SUM(IMPCON),0.0) FROM DENDETALI " + " WHERE CODPOS = " + dt.Rows[index3]["CODPOS"]?.ToString() + " AND ANNDEN = " + dt.Rows[index3]["ANNDEN"]?.ToString() + " AND MESDEN = " + dt.Rows[index3]["MESDEN"]?.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND MAT = " + dt.Rows[index3]["MAT"]?.ToString() + " AND PRODENDET = " + num12.ToString();
                  string str16 = objDataAccess.Get1ValueFromSQL(strSQL11, CommandType.Text);
                  if (str16 != string.Empty)
                    num6 = Convert.ToDecimal(str16);
                  string strSQL12 = "SELECT IMPCON FROM DENDET " + " WHERE CODPOS = " + dt.Rows[index3]["CODPOS"]?.ToString() + " AND ANNDEN = " + dt.Rows[index3]["ANNDEN"]?.ToString() + " AND MESDEN = " + dt.Rows[index3]["MESDEN"]?.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND MAT = " + dt.Rows[index3]["MAT"]?.ToString() + " AND PRODENDET = " + num12.ToString();
                  string str17 = objDataAccess.Get1ValueFromSQL(strSQL12, CommandType.Text);
                  if (str17 != string.Empty)
                    num7 = Convert.ToDecimal(str17);
                  Decimal num13 = num7 - num6;
                  if (num13 <= -0.01M)
                  {
                    if (num13 <= -0.06M)
                    {
                      if (num13 <= -0.08M)
                      {
                        if (!(num13 == -0.09M) && !(num13 == -0.08M))
                          goto label_50;
                      }
                      else if (!(num13 == -0.07M) && !(num13 == -0.06M))
                        goto label_50;
                    }
                    else if (num13 <= -0.04M)
                    {
                      if (!(num13 == -0.05M) && !(num13 == -0.04M))
                        goto label_50;
                    }
                    else if (!(num13 == -0.03M) && !(num13 == -0.02M) && !(num13 == -0.01M))
                      goto label_50;
                    num11 = num7 - num6;
                    string strSQL13 = "UPDATE DENDETALI SET IMPCON = IMPCON + " + num11.ToString().Trim().Replace(",", ".") + " WHERE CODPOS = " + dt.Rows[index3]["CODPOS"]?.ToString() + " AND ANNDEN = " + dt.Rows[index3]["ANNDEN"]?.ToString() + " AND MESDEN = " + dt.Rows[index3]["MESDEN"]?.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND MAT = " + dt.Rows[index3]["MAT"]?.ToString() + " AND PRODENDET = " + num12.ToString() + " AND CODFORASS = (SELECT MIN(CODFORASS) FROM DENDETALI " + " WHERE CODPOS = " + dt.Rows[index3]["CODPOS"]?.ToString() + " AND ANNDEN = " + dt.Rows[index3]["ANNDEN"]?.ToString() + " AND MESDEN = " + dt.Rows[index3]["MESDEN"]?.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND MAT = " + dt.Rows[index3]["MAT"]?.ToString() + " AND PRODENDET = " + num12.ToString() + ")";
                    objDataAccess.WriteTransactionData(strSQL13, CommandType.Text);
                    continue;
                  }
                  if (num13 <= 0.04M)
                  {
                    if (num13 <= 0.01M)
                    {
                      if (!(num13 == 0M))
                      {
                        if (!(num13 == 0.01M))
                          goto label_50;
                      }
                      else
                        continue;
                    }
                    else if (!(num13 == 0.02M) && !(num13 == 0.03M) && !(num13 == 0.04M))
                      goto label_50;
                  }
                  else if (num13 <= 0.06M)
                  {
                    if (!(num13 == 0.05M) && !(num13 == 0.06M))
                      goto label_50;
                  }
                  else if (!(num13 == 0.07M) && !(num13 == 0.08M) && !(num13 == 0.09M))
                    goto label_50;
                  num11 = num7 - num6;
                  string strSQL14 = "UPDATE DENDETALI SET IMPCON = IMPCON + " + num11.ToString().Trim().Replace(",", ".") + " WHERE CODPOS = " + dt.Rows[index3]["CODPOS"]?.ToString() + " AND ANNDEN = " + dt.Rows[index3]["ANNDEN"]?.ToString() + " AND MESDEN = " + dt.Rows[index3]["MESDEN"]?.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND MAT = " + dt.Rows[index3]["MAT"]?.ToString() + " AND PRODENDET = " + num12.ToString() + " AND CODFORASS = (SELECT MIN(CODFORASS) FROM DENDETALI " + " WHERE CODPOS = " + dt.Rows[index3]["CODPOS"]?.ToString() + " AND ANNDEN = " + dt.Rows[index3]["ANNDEN"]?.ToString() + " AND MESDEN = " + dt.Rows[index3]["MESDEN"]?.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND MAT = " + dt.Rows[index3]["MAT"]?.ToString() + " AND PRODENDET = " + num12.ToString() + ")";
                  objDataAccess.WriteTransactionData(strSQL14, CommandType.Text);
                  continue;
label_50:
                  throw new Exception("Attenzione... errore nello splittamento in DENDETALI del contributo per: CODPOS = " + dt.Rows[index3]["CODPOS"]?.ToString() + " ANNDEN = " + dt.Rows[index3]["ANNDEN"]?.ToString() + " MESDEN = " + dt.Rows[index3]["MESDEN"]?.ToString() + " PRODEN = " + PRODEN.ToString() + " MAT = " + dt.Rows[index3]["MAT"]?.ToString() + " PRODENDET = " + num12.ToString());
                }
              }
              string str18 = "UPDATE DENDET SET " + " IMPFAP = IMPRET * VALUE(PERFAP, 0.0) / 100";
              if (str1 != "")
                str18 = str18 + ", CODCAUSAN = " + DBMethods.DoublePeakForSql(str1);
              string strSQL15 = str18 + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString();
              objDataAccess.WriteTransactionData(strSQL15, CommandType.Text);
              string strSQL16 = "UPDATE DENTES SET " + " IMPRET=(SELECT SUM(IMPRET) FROM DENDET" + " WHERE CODPOS = DENTES.CODPOS AND ANNDEN=DENTES.ANNDEN AND" + " MESDEN=DENTES.MESDEN AND PRODEN=DENTES.PRODEN), " + " IMPFIG=(SELECT SUM(IMPFIG) FROM DENDET" + " WHERE CODPOS = DENTES.CODPOS AND ANNDEN=DENTES.ANNDEN AND" + " MESDEN=DENTES.MESDEN AND PRODEN=DENTES.PRODEN), " + " IMPCON = (SELECT SUM(IMPCON) FROM DENDET" + " WHERE CODPOS = DENTES.CODPOS AND ANNDEN=DENTES.ANNDEN AND" + " MESDEN=DENTES.MESDEN AND PRODEN=DENTES.PRODEN), " + " NUMRIGDET=(SELECT COUNT(*) FROM DENDET" + " WHERE CODPOS = DENTES.CODPOS AND ANNDEN=DENTES.ANNDEN AND" + " MESDEN=DENTES.MESDEN AND PRODEN=DENTES.PRODEN) " + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString();
              objDataAccess.WriteTransactionData(strSQL16, CommandType.Text);
              string strSQL17 = "UPDATE DENTES SET " + " IMPADDREC=ROUND(IMPCON * " + num8.ToString().Replace(",", ".") + "/100, 2)" + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString();
              objDataAccess.WriteTransactionData(strSQL17, CommandType.Text);
              string strSQL18 = "SELECT ABB FROM AZISTO WHERE CODPOS = " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date("01/" + int32_2.ToString().Trim().PadLeft(2, '0') + "/" + int32_1.ToString())) + " BETWEEN DATINI AND VALUE(DATFIN,'9999-12-31')";
              DataTable dataTable3 = objDataAccess.GetDataTable(strSQL18);
              Decimal num14 = dataTable3.Rows.Count != 0 ? (!(dataTable3.Rows[0]["ABB"].ToString() == "S") ? 0M : Convert.ToDecimal(str6)) : 0M;
              string strSQL19 = "SELECT MAT, MAX(IMPABB) AS IMPABB FROM DENDET " + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString() + " AND IMPABB > 0" + " GROUP BY MAT";
              num1 = 0.0M;
              DataTable dataTable4 = objDataAccess.GetDataTable(strSQL19);
              for (int index4 = 0; index4 <= dataTable4.Rows.Count - 1; ++index4)
                num1 += Convert.ToDecimal(dataTable4.Rows[index4]["IMPABB"]);
              num11 = num14 + num1;
              string strSQL20 = " UPDATE DENTES SET IMPABB = " + num11.ToString().Replace(",", ".") + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString();
              objDataAccess.WriteTransactionData(strSQL20, CommandType.Text);
              string strSQL21 = "SELECT MAT, MAX(IMPASSCON) AS IMPASSCON FROM DENDET " + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString() + " AND IMPASSCON > 0" + " GROUP BY MAT";
              num2 = 0.0M;
              dataTable1 = objDataAccess.GetDataTable(strSQL21);
              for (int index5 = 0; index5 <= dataTable1.Rows.Count - 1; ++index5)
                num2 += Convert.ToDecimal(dataTable1.Rows[index5]["IMPASSCON"]);
              string strSQL22 = " UPDATE DENTES SET IMPASSCON = " + num2.ToString().Replace(",", ".") + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString();
              objDataAccess.WriteTransactionData(strSQL22, CommandType.Text);
              string strSQL23 = "UPDATE DENTES SET " + " IMPDIS = (IMPCON + IMPABB + IMPADDREC + IMPASSCON) " + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString();
              objDataAccess.WriteTransactionData(strSQL23, CommandType.Text);
              if (RICSANUTE != "ESCLUDI" && (TIPMOVSAN == "SAN_RD" || TIPMOVSAN == "SAN_MD"))
              {
                string str19;
                if (int32_1 < 2003)
                  str19 = "UPDATE DENDET SET IMPSANDET = IMPRET * ALIQUOTA/100 * " + TASSO.ToString().Replace(",", ".") + "/36500";
                else
                  str19 = "UPDATE DENDET SET IMPSANDET = IMPRET * ALIQUOTA/100 * " + num3.ToString().Replace(",", ".") + " * " + TASSO.ToString().Replace(",", ".") + "/36500";
                string strSQL24 = str19 + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString();
                objDataAccess.WriteTransactionData(strSQL24, CommandType.Text);
                if (num5 > 0M)
                {
                  string strSQL25 = "UPDATE DENDET SET IMPSANDET = IMPCON * " + num5.ToString().Replace(",", ".") + "/100" + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString() + " AND IMPCON * " + num5.ToString().Replace(",", ".") + "/100 < IMPSANDET";
                  objDataAccess.WriteTransactionData(strSQL25, CommandType.Text);
                }
                string strSQL26 = "UPDATE DENTES SET " + " IMPSANDET = (SELECT SUM(IMPSANDET) FROM DENDET" + " WHERE CODPOS = DENTES.CODPOS AND ANNDEN=DENTES.ANNDEN AND" + " MESDEN=DENTES.MESDEN AND PRODEN=DENTES.PRODEN), " + " CODCAUSAN = " + DBMethods.DoublePeakForSql(str1.Trim().PadLeft(2, '0')) + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString();
                objDataAccess.WriteTransactionData(strSQL26, CommandType.Text);
                if (num3 <= num10)
                {
                  string strSQL27 = "UPDATE DENTES SET SANSOTSOG = 'S'" + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString();
                  objDataAccess.WriteTransactionData(strSQL27, CommandType.Text);
                }
                else
                {
                  string strSQL28 = "SELECT VALUE(IMPSANDET, 0.0) FROM DENTES " + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString();
                  string str20 = objDataAccess.Get1ValueFromSQL(strSQL28, CommandType.Text);
                  if (str20 != string.Empty)
                  {
                    if (Convert.ToDecimal(str20) <= num4)
                    {
                      string strSQL29 = "UPDATE DENTES SET SANSOTSOG = 'S'" + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString();
                      objDataAccess.WriteTransactionData(strSQL29, CommandType.Text);
                    }
                    else
                    {
                      string strSQL30 = "UPDATE DENTES SET SANSOTSOG = 'N'" + " WHERE CODPOS= " + dtAziende.Rows[index1]["CODPOS"]?.ToString() + " AND ANNDEN = " + int32_1.ToString() + " AND MESDEN =" + int32_2.ToString() + " AND PRODEN =" + PRODEN.ToString();
                      objDataAccess.WriteTransactionData(strSQL30, CommandType.Text);
                    }
                  }
                }
              }
            }
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public DataTable Module_Genera_Notifiche(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      ref DataTable dtAziende,
      ref DataTable dtLOG,
      string TIPMOVSAN,
      string DATA_RIFERIMENTO,
      string STRRICPREV = "",
      int OPTIONAL_MATRICOLA = 0,
      int OPTIONAL_PRORAP = 0,
      bool PREV = false)
    {
      try
      {
        int num1 = 0;
        int num2 = 0;
        Decimal num3 = 0.0M;
        Decimal num4 = 0.0M;
        Decimal num5 = 0.0M;
        int index1 = 1;
        DataTable dataTable1 = new DataTable();
        DataTable dataTable2 = new DataTable();
        DataTable dataTable3 = new DataTable();
        DataTable dataTable4 = new DataTable();
        DataTable dataTable5 = new DataTable();
        DataTable dataTable6 = new DataTable();
        int PROLOC = 0;
        string str1 = "";
        Decimal num6 = 0.0M;
        int num7 = 0;
        int num8 = 0;
        DataTable dataTable7 = new DataTable();
        DataTable dataTable8 = new DataTable();
        DataTable dataTable9 = new DataTable();
        DateTime dateTime;
        string str2;
        if (DATA_RIFERIMENTO == "")
        {
          dateTime = this.Utils.Module_GetDataSistema();
          str2 = dateTime.ToString();
        }
        else
          str2 = DATA_RIFERIMENTO;
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "CODPOS"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "ANNDEN"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "MESDEN"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "MAT"
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
          ColumnName = "DAL"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "AL"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "ETAENNE"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "QUALIFICA"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "ALIQUOTA"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "DATA_INIZIO_STORDL"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "DATA_FINE_STORDL"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "TIPO_RAPPORTO"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "TIPRAP"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "CODQUACON"
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
          ColumnName = "CODLIV"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "FAP"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "CODGRUASS"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "DENLIV"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPRET"
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
          ColumnName = "PERFAP"
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
          ColumnName = "IMPABB"
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
          ColumnName = "IMPADDREC"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPASSCON"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPCON"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPMIN"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPSCA"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPTRAECO"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "TIPSPE"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = nameof (PREV)
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "GIORNI_MESE"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "IMPOCC"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "RIMUOVI"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "NUMMEN"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "MESMEN14"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "MESMEN15"
        });
        dataTable1.Columns.Add(new DataColumn()
        {
          ColumnName = "MESMEN16"
        });
        dataTable8.Columns.Add(new DataColumn()
        {
          ColumnName = "ANNO"
        });
        dataTable8.Columns.Add(new DataColumn()
        {
          ColumnName = "MESE"
        });
        string strSQL1 = " SELECT CODPOS, MAT, PRORAP, DATINISOS, DATFINSOS, PERAZI, PERFIG FROM SOSRAP" + " WHERE CODPOS = -1";
        DataTable dataTable10 = objDataAccess.GetDataTable(strSQL1);
        for (int index2 = 0; index2 <= dtAziende.Rows.Count - 1; ++index2)
        {
          int year = (int) dtAziende.Rows[index2]["ANNO"];
          int month = (int) dtAziende.Rows[index2]["MESE"];
          dataTable8.Rows.Add();
          dataTable8.Rows[dataTable8.Rows.Count - 1]["ANNO"] = (object) year;
          dataTable8.Rows[dataTable8.Rows.Count - 1]["MESE"] = (object) month;
          dateTime = Convert.ToDateTime("01/" + month.ToString() + "/" + year.ToString());
          string str3 = dateTime.ToString();
          dateTime = Convert.ToDateTime(DateTime.DaysInMonth(year, month).ToString() + "/" + month.ToString() + "/" + year.ToString());
          string str4 = dateTime.ToString();
          int num9 = DateTime.DaysInMonth(year, month);
          string valorePargen = (string) this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 3, str3);
          dtAziende.Rows[index2]["DATSCA"] = (object) valorePargen;
          string str5 = "SELECT TRIM(COG) || ' ' || TRIM(NOM) AS NOME , DATNAS, RAPLAV.CODPOS, " + " RAPLAV.MAT, RAPLAV.PRORAP, RAPLAV.DATDEC, RAPLAV.DATCES " + " FROM ISCT, RAPLAV " + " WHERE ISCT.MAT=RAPLAV.MAT " + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " <= VALUE(RAPLAV.DATCES , '9999-12-31') " + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str4)) + " >=RAPLAV.DATDEC " + " AND RAPLAV.CODPOS=" + dtAziende.Rows[index2]["CODPOS"]?.ToString();
          if (OPTIONAL_MATRICOLA != 0)
            str5 = str5 + " AND RAPLAV.MAT = " + OPTIONAL_MATRICOLA.ToString() + " AND RAPLAV.PRORAP = " + OPTIONAL_PRORAP.ToString();
          string strSQL2 = str5 + " ORDER BY NOME, MAT";
          dataTable4.Rows.Clear();
          dataTable4 = objDataAccess.GetDataTable(strSQL2);
          if (dataTable4.Rows.Count > 0)
          {
            string str6 = " SELECT CODPOS, MAT, PRORAP, DATINISOS, DATFINSOS, PERAZI, PERFIG FROM SOSRAP" + " WHERE CODPOS = " + dtAziende.Rows[index2]["CODPOS"]?.ToString();
            if (OPTIONAL_MATRICOLA != 0)
              str6 = str6 + " AND MAT = " + OPTIONAL_MATRICOLA.ToString() + " AND PRORAP = " + OPTIONAL_PRORAP.ToString();
            string strSQL3 = str6 + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " <= VALUE(DATFINSOS , '9999-12-31') " + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str4)) + " >=DATINISOS " + " AND STASOS = '0' AND CODSOS <> 15" + " ORDER BY MAT, PRORAP, DATINISOS";
            dataTable6.Clear();
            dataTable6 = objDataAccess.GetDataTable(strSQL3);
            for (index1 = 0; index1 <= dataTable6.Rows.Count - 1; ++index1)
            {
              DataRow row = dataTable10.NewRow();
              for (int columnIndex = 0; columnIndex <= dataTable6.Columns.Count - 1; ++columnIndex)
                row[columnIndex] = dataTable6.Rows[index1][columnIndex];
              dataTable10.Rows.Add(row);
            }
            for (int index3 = 0; index3 <= dataTable4.Rows.Count - 1; ++index3)
            {
              int num10 = !DBNull.Value.Equals(dataTable4.Rows[index3]["CODPOS"]) ? Convert.ToInt32(dataTable4.Rows[index3]["CODPOS"]) : 0;
              int MAT = !DBNull.Value.Equals(dataTable4.Rows[index3]["MAT"]) ? Convert.ToInt32(dataTable4.Rows[index3]["MAT"]) : 0;
              int PRORAP = !DBNull.Value.Equals(dataTable4.Rows[index3]["PRORAP"]) ? Convert.ToInt32(dataTable4.Rows[index3]["PRORAP"]) : 0;
              string str7 = !DBNull.Value.Equals(dataTable4.Rows[index3]["NOME"]) ? dataTable4.Rows[index3]["NOME"].ToString() : string.Empty;
              string str8 = !DBNull.Value.Equals(dataTable4.Rows[index3]["DATNAS"]) ? dataTable4.Rows[index3]["DATNAS"].ToString() : string.Empty;
              string str9 = !DBNull.Value.Equals(dataTable4.Rows[index3]["DATDEC"]) ? dataTable4.Rows[index3]["DATDEC"].ToString() : string.Empty;
              string str10 = !DBNull.Value.Equals(dataTable4.Rows[index3]["DATCES"]) ? dataTable4.Rows[index3]["DATCES"].ToString() : string.Empty;
              if (!string.IsNullOrEmpty(str8))
              {
                string str11 = Convert.ToString(Convert.ToDateTime(str8));
                dateTime = Convert.ToDateTime(str11);
                string s1 = Convert.ToString(dateTime.AddYears(65));
                if (!string.IsNullOrEmpty(str9))
                {
                  string str12 = str9;
                  string str13;
                  if (!string.IsNullOrEmpty(str10))
                  {
                    dateTime = Convert.ToDateTime(str10);
                    str13 = dateTime.ToString().Substring(0, 10);
                  }
                  else
                    str13 = "";
                  string strSQL4 = " SELECT CODPOS, MAT, FAP, PRORAP,TIPRAP, CODGRUASS, VALUE(IMPSCAMAT,0.00) AS IMPSCAMAT, DATINI, DATFIN, VALUE(CODLOC, 0) AS CODLOC, VALUE(CODCON, 0) AS CODCON, " + " (" + " SELECT DENTIPRAP FROM TIPRAP WHERE TIPRAP = STORDL.TIPRAP " + " ) AS RAPPORTO, CODLIV, " + " '' AS DENLIV, " + " 0 AS CODQUACON," + " 0 AS PROCON," + " 0 AS PROLOC, " + " VALUE(TRAECO, 0.00) AS TRAECO, " + " VALUE(PERAPP, 0.00) AS PERAPP, " + " VALUE(PERPAR, 0.00) AS PERPAR, " + " VALUE(ABBPRE, 'N') AS ABBPRE, " + " VALUE(ASSCON, 'N') AS ASSCON, " + " VALUE(NUMMEN, 12) AS NUMMEN, " + " VALUE(MESMEN14, 14) AS MESMEN14, " + " VALUE(MESMEN15, 15) AS MESMEN15, " + " VALUE(MESMEN16, 16) AS MESMEN16 " + " FROM STORDL " + " WHERE CODPOS = " + num10.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " <= VALUE(DATFIN , '9999-12-31') " + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str4)) + " >= DATINI ORDER BY MAT, DATINI";
                  dataTable2.Clear();
                  dataTable2 = objDataAccess.GetDataTable(strSQL4);
                  if (dataTable2.Rows.Count == 0)
                  {
                    dtLOG.Rows.Add();
                    dtLOG.Rows[dtLOG.Rows.Count - 1]["CODPOS"] = (object) num10;
                    dtLOG.Rows[dtLOG.Rows.Count - 1]["ANNDEN"] = (object) year;
                    dtLOG.Rows[dtLOG.Rows.Count - 1]["MESDEN"] = (object) month;
                    dtLOG.Rows[dtLOG.Rows.Count - 1]["MAT"] = (object) MAT;
                    dtLOG.Rows[dtLOG.Rows.Count - 1]["DESERR"] = (object) "TRATTAMENTI ECONOMICI INESISTENTI";
                  }
                  else
                  {
                    for (int index4 = 0; index4 <= dataTable2.Rows.Count - 1; ++index4)
                    {
                      string str14 = !DBNull.Value.Equals(dataTable2.Rows[index4]["DATINI"]) ? dataTable2.Rows[index4]["DATINI"].ToString() : string.Empty;
                      string str15 = !DBNull.Value.Equals(dataTable2.Rows[index4]["DATFIN"]) ? dataTable2.Rows[index4]["DATFIN"].ToString() : string.Empty;
                      dateTime = Convert.ToDateTime(str14);
                      dateTime.ToString().Substring(0, 10);
                      dateTime = Convert.ToDateTime(str15);
                      dateTime.ToString().Substring(0, 10);
                      string str16;
                      if (DateTime.Compare(DateTime.Parse(str3), DateTime.Parse(dataTable2.Rows[index4]["DATINI"].ToString())) >= 0)
                      {
                        dateTime = Convert.ToDateTime(str3);
                        str16 = dateTime.ToString().Substring(0, 10);
                      }
                      else
                      {
                        dateTime = Convert.ToDateTime(dataTable2.Rows[index4]["DATINI"]);
                        str16 = dateTime.ToString().Substring(0, 10);
                      }
                      string s2;
                      if (DateTime.Compare(DateTime.Parse(str4), DateTime.Parse(dataTable2.Rows[index4]["DATFIN"].ToString())) <= 0)
                      {
                        dateTime = Convert.ToDateTime(str4);
                        s2 = dateTime.ToString().Substring(0, 10);
                      }
                      else
                      {
                        dateTime = Convert.ToDateTime(dataTable2.Rows[index4]["DATFIN"]);
                        s2 = dateTime.ToString().Substring(0, 10);
                      }
                      int CODCON = !DBNull.Value.Equals(dataTable2.Rows[index4]["CODCON"]) ? Convert.ToInt32(dataTable2.Rows[index4]["CODCON"]) : 0;
                      int CODLOC = !DBNull.Value.Equals(dataTable2.Rows[index4]["CODLOC"]) ? Convert.ToInt32(dataTable2.Rows[index4]["CODLOC"]) : 0;
                      string FAP = !DBNull.Value.Equals(dataTable2.Rows[index4]["FAP"]) ? dataTable2.Rows[index4]["FAP"].ToString() : string.Empty;
                      int CODGRUASS = !DBNull.Value.Equals(dataTable2.Rows[index4]["CODGRUASS"]) ? Convert.ToInt32(dataTable2.Rows[index4]["CODGRUASS"]) : 0;
                      Decimal PERPAR = !DBNull.Value.Equals(dataTable2.Rows[index4]["PERAPP"]) ? Convert.ToDecimal(dataTable2.Rows[index4]["PERAPP"]) : 0M;
                      Decimal PERAPP = !DBNull.Value.Equals(dataTable2.Rows[index4]["PERPAR"]) ? Convert.ToDecimal(dataTable2.Rows[index4]["PERPAR"]) : 0M;
                      Decimal num11 = !DBNull.Value.Equals(dataTable2.Rows[index4]["IMPSCAMAT"]) ? Convert.ToDecimal(dataTable2.Rows[index4]["IMPSCAMAT"]) : 0M;
                      string str17 = !DBNull.Value.Equals(dataTable2.Rows[index4]["ABBPRE"]) ? dataTable2.Rows[index4]["ABBPRE"].ToString() : string.Empty;
                      string str18 = !DBNull.Value.Equals(dataTable2.Rows[index4]["ASSCON"]) ? dataTable2.Rows[index4]["ASSCON"].ToString() : string.Empty;
                      Decimal num12 = !DBNull.Value.Equals(dataTable2.Rows[index4]["TRAECO"]) ? Convert.ToDecimal(dataTable2.Rows[index4]["TRAECO"]) : 0M;
                      int num13 = !DBNull.Value.Equals(dataTable2.Rows[index4]["NUMMEN"]) ? Convert.ToInt32(dataTable2.Rows[index4]["NUMMEN"]) : 0;
                      int num14 = !DBNull.Value.Equals(dataTable2.Rows[index4]["MESMEN14"]) ? Convert.ToInt32(dataTable2.Rows[index4]["MESMEN14"]) : 0;
                      int num15 = !DBNull.Value.Equals(dataTable2.Rows[index4]["MESMEN15"]) ? Convert.ToInt32(dataTable2.Rows[index4]["MESMEN15"]) : 0;
                      int num16 = !DBNull.Value.Equals(dataTable2.Rows[index4]["MESMEN16"]) ? Convert.ToInt32(dataTable2.Rows[index4]["MESMEN16"]) : 0;
                      string str19 = !DBNull.Value.Equals(dataTable2.Rows[index4]["TIPRAP"]) ? dataTable2.Rows[index4]["TIPRAP"].ToString() : string.Empty;
                      if (str19.Trim() == "")
                      {
                        dtLOG.Rows.Add();
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["CODPOS"] = (object) num10;
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["ANNDEN"] = (object) year;
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["MESDEN"] = (object) month;
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["MAT"] = (object) MAT;
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["DESERR"] = (object) "TIPO RAPPORTO NON TROVATO";
                        break;
                      }
                      string str20 = str19;
                      if (FAP == "N")
                      {
                        string strSQL5 = "SELECT VALFAP FROM CODFAP WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str16)) + " " + " BETWEEN DATINI AND  VALUE(DATFIN, '9999-12-31')";
                        num6 = Convert.ToDecimal(objDataAccess.Get1ValueFromSQL(strSQL5, CommandType.Text));
                      }
                      else
                        num6 = 0.0M;
                      if (CODLOC != 0)
                        dataTable6 = this.ModGetDati.Module_GetDatiContratto_Locale(objDataAccess, CODLOC, str16);
                      else if (CODCON != 0)
                      {
                        dataTable6 = this.ModGetDati.Module_GetDatiContratto_Riferimento(objDataAccess, CODCON, str16);
                      }
                      else
                      {
                        dtLOG.Rows.Add();
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["CODPOS"] = (object) num10;
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["ANNDEN"] = (object) year;
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["MESDEN"] = (object) month;
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["MAT"] = (object) MAT;
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["DESERR"] = (object) "CONTRATTO NON TROVATO";
                        break;
                      }
                      if (dataTable6.Rows.Count == 0)
                      {
                        dtLOG.Rows.Add();
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["CODPOS"] = (object) num10;
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["ANNDEN"] = (object) year;
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["MESDEN"] = (object) month;
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["MAT"] = (object) MAT;
                        dtLOG.Rows[dtLOG.Rows.Count - 1]["DESERR"] = (object) "CONTRATTO NON TROVATO";
                        break;
                      }
                      if (CODLOC != 0)
                        PROLOC = !DBNull.Value.Equals(dataTable6.Rows[0]["PROLOC"]) ? Convert.ToInt32(dataTable6.Rows[0]["PROLOC"]) : 0;
                      string str21 = !DBNull.Value.Equals(dataTable6.Rows[0]["TIPSPE"]) ? dataTable6.Rows[0]["TIPSPE"].ToString() : string.Empty;
                      string str22 = !DBNull.Value.Equals(dataTable6.Rows[0]["DENQUA"]) ? dataTable6.Rows[0]["DENQUA"].ToString() : string.Empty;
                      string str23 = !DBNull.Value.Equals(dataTable2.Rows[index4]["RAPPORTO"]) ? dataTable2.Rows[index4]["RAPPORTO"].ToString().Trim() : string.Empty;
                      int CODQUACON = !DBNull.Value.Equals(dataTable6.Rows[0]["CODQUACON"]) ? Convert.ToInt32(dataTable6.Rows[0]["CODQUACON"]) : 0;
                      int PROCON = !DBNull.Value.Equals(dataTable6.Rows[0]["PROCON"]) ? Convert.ToInt32(dataTable6.Rows[0]["PROCON"]) : 0;
                      int CODLIV = !DBNull.Value.Equals(dataTable2.Rows[index4]["CODLIV"]) ? Convert.ToInt32(dataTable2.Rows[index4]["CODLIV"]) : 0;
                      string strSQL6 = "SELECT DENLIV FROM CONLIV WHERE CODCON = " + CODCON.ToString() + " AND PROCON = " + PROCON.ToString() + " AND CODLIV = " + CODLIV.ToString();
                      dataTable6.Clear();
                      dataTable6 = objDataAccess.GetDataTable(strSQL6);
                      if (dataTable6.Rows.Count > 0)
                        str1 = dataTable6.Rows[0]["DENLIV"].ToString().Trim();
                      if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(str16)) > 0 & DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(s2)) < 0)
                      {
                        dataTable1.Rows.Add();
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"] = (object) str16;
                        DataRow row1 = dataTable1.Rows[dataTable1.Rows.Count - 1];
                        dateTime = Convert.ToDateTime(s1);
                        dateTime = dateTime.AddDays(-1.0);
                        string str24 = dateTime.ToString().Substring(0, 10);
                        row1["AL"] = (object) str24;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "N";
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Percentuale_Aliquota(objDataAccess, str16, num10.ToString(), PRORAP, (long) MAT, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"], FAP, CODQUACON, CODGRUASS);
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["CODPOS"] = (object) num10;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ANNDEN"] = (object) year;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["MESDEN"] = (object) month;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["MAT"] = (object) MAT;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["PRORAP"] = (object) PRORAP;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["NOME"] = (object) str7;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DATNAS"] = (object) str11;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DATDEC"] = (object) str12;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DATCES"] = (object) str13;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["CODQUACON"] = (object) CODQUACON;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["CODGRUASS"] = (object) CODGRUASS;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCON"] = (object) CODCON;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["PROCON"] = (object) PROCON;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLOC"] = (object) CODLOC;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["PROLOC"] = (object) PROLOC;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLIV"] = (object) CODLIV;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DENLIV"] = (object) str1;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["PERAPP"] = (object) PERPAR;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["PERPAR"] = (object) PERAPP;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["QUALIFICA"] = (object) str22;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPO_RAPPORTO"] = (object) str23;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DATA_INIZIO_STORDL"] = (object) str16;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DATA_FINE_STORDL"] = (object) s2;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["PERFAP"] = (object) num6;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["FAP"] = (object) FAP;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ABBPRE"] = (object) str17;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ASSCON"] = (object) str18;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPSPE"] = (object) str21;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPRAP"] = (object) str20;
                        if (str17 == "S")
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 1, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"]);
                        else
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) 0.0M;
                        if (str18 == "S")
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 4, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"]);
                        else
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) 0.0M;
                        if (str21 == "S")
                        {
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) this.ModGetDati.Module_GetMinimoContrattuale(objDataAccess, CODCON, PROCON, CODLOC, PROLOC, CODLIV, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"], PERAPP, PERPAR);
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) num11;
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) 0.0M;
                        }
                        else
                        {
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) 0.0M;
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) 0.0M;
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) num12;
                        }
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["GIORNI_MESE"] = (object) num9;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["NUMMEN"] = (object) num13;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN14"] = (object) num14;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN15"] = (object) num15;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN16"] = (object) num16;
                        dataTable1.Rows.Add();
                        DataRow row2 = dataTable1.Rows[dataTable1.Rows.Count - 1];
                        dateTime = Convert.ToDateTime(s1);
                        string str25 = dateTime.ToString().Substring(0, 10);
                        row2["DAL"] = (object) str25;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s2;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "S";
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Percentuale_Aliquota(objDataAccess, str16, num10.ToString(), PRORAP, (long) MAT, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"], FAP, CODQUACON, CODGRUASS);
                      }
                      else if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(str16)) == 0)
                      {
                        dataTable1.Rows.Add();
                        DataRow row = dataTable1.Rows[dataTable1.Rows.Count - 1];
                        dateTime = Convert.ToDateTime(s1);
                        string str26 = dateTime.ToString().Substring(0, 10);
                        row["DAL"] = (object) str26;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s2;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "S";
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Percentuale_Aliquota(objDataAccess, str16, num10.ToString(), PRORAP, (long) MAT, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"], FAP, CODQUACON, CODGRUASS);
                      }
                      else if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(str16)) < 0)
                      {
                        dataTable1.Rows.Add();
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"] = (object) str16;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s2;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "S";
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Percentuale_Aliquota(objDataAccess, str16, num10.ToString(), PRORAP, (long) MAT, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"], FAP, CODQUACON, CODGRUASS);
                      }
                      else if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(s2)) == 0)
                      {
                        dataTable1.Rows.Add();
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"] = (object) str16;
                        DataRow row3 = dataTable1.Rows[dataTable1.Rows.Count - 1];
                        dateTime = Convert.ToDateTime(s1);
                        dateTime = dateTime.AddDays(-1.0);
                        string str27 = dateTime.ToString().Substring(0, 10);
                        row3["AL"] = (object) str27;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "N";
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Percentuale_Aliquota(objDataAccess, str16, Convert.ToString(num10), PRORAP, (long) MAT, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"], FAP, CODQUACON, CODGRUASS);
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["CODPOS"] = (object) num10;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ANNDEN"] = (object) year;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["MESDEN"] = (object) month;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["MAT"] = (object) MAT;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["PRORAP"] = (object) PRORAP;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["NOME"] = (object) str7;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DATNAS"] = (object) str11;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DATDEC"] = (object) str12;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DATCES"] = (object) str13;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["CODQUACON"] = (object) CODQUACON;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["CODGRUASS"] = (object) CODGRUASS;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCON"] = (object) CODCON;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["PROCON"] = (object) PROCON;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLOC"] = (object) CODLOC;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["PROLOC"] = (object) PROLOC;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLIV"] = (object) CODLIV;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DENLIV"] = (object) str1;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["PERAPP"] = (object) PERPAR;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["PERPAR"] = (object) PERAPP;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["QUALIFICA"] = (object) str22;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPO_RAPPORTO"] = (object) str23;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DATA_INIZIO_STORDL"] = (object) str16;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DATA_FINE_STORDL"] = (object) s2;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["PERFAP"] = (object) num6;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["FAP"] = (object) FAP;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ABBPRE"] = (object) str17;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ASSCON"] = (object) str18;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPSPE"] = (object) str21;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPRAP"] = (object) str20;
                        if (str17 == "S")
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 1, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"]);
                        else
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) 0.0M;
                        if (str18 == "S")
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 4, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"]);
                        else
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) 0.0M;
                        if (str21 == "S")
                        {
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) this.ModGetDati.Module_GetMinimoContrattuale(objDataAccess, CODCON, PROCON, CODLOC, PROLOC, CODLIV, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"], PERAPP, PERPAR);
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) num11;
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) 0.0M;
                        }
                        else
                        {
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) 0.0M;
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) 0.0M;
                          dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) num12;
                        }
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["GIORNI_MESE"] = (object) num9;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["NUMMEN"] = (object) num13;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN14"] = (object) num14;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN15"] = (object) num15;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN16"] = (object) num16;
                        dataTable1.Rows.Add();
                        DataRow row4 = dataTable1.Rows[dataTable1.Rows.Count - 1];
                        dateTime = Convert.ToDateTime(s1);
                        string str28 = dateTime.ToString().Substring(0, 10);
                        row4["DAL"] = (object) str28;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s2;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "S";
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Percentuale_Aliquota(objDataAccess, str16, Convert.ToString(num10), PRORAP, (long) MAT, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"], FAP, CODQUACON, CODGRUASS);
                      }
                      else
                      {
                        dataTable1.Rows.Add();
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"] = (object) str16;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s2;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "N";
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Percentuale_Aliquota(objDataAccess, str16, Convert.ToString(num10), PRORAP, (long) MAT, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"], FAP, CODQUACON, CODGRUASS);
                      }
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODPOS"] = (object) num10;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MAT"] = (object) MAT;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ANNDEN"] = (object) year;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MESDEN"] = (object) month;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PRORAP"] = (object) PRORAP;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["NOME"] = (object) str7;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATNAS"] = (object) str11;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATDEC"] = (object) str12;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATCES"] = (object) str13;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODQUACON"] = (object) CODQUACON;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODGRUASS"] = (object) CODGRUASS;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCON"] = (object) CODCON;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PROCON"] = (object) PROCON;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLOC"] = (object) CODLOC;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PROLOC"] = (object) PROLOC;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLIV"] = (object) CODLIV;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DENLIV"] = (object) str1;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PERAPP"] = (object) PERPAR;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PERPAR"] = (object) PERAPP;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["QUALIFICA"] = (object) str22;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPO_RAPPORTO"] = (object) str23;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATA_INIZIO_STORDL"] = (object) str16;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATA_FINE_STORDL"] = (object) s2;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PERFAP"] = (object) num6;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["FAP"] = (object) FAP;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ABBPRE"] = (object) str17;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ASSCON"] = (object) str18;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPSPE"] = (object) str21;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPRAP"] = (object) str20;
                      if (str17 == "S")
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 1, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"]);
                      else
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) 0.0M;
                      if (str18 == "S")
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 4, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"]);
                      else
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) 0.0M;
                      if (str21 == "S")
                      {
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) this.ModGetDati.Module_GetMinimoContrattuale(objDataAccess, CODCON, PROCON, CODLOC, PROLOC, CODLIV, (string) dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"], PERAPP, PERPAR);
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) num11;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) 0.0M;
                      }
                      else
                      {
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) 0.0M;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) 0.0M;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) num12;
                      }
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["GIORNI_MESE"] = (object) num9;
                      dtAziende.Rows[index2]["RIMUOVI"] = (object) "NO";
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["NUMMEN"] = (object) num13;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN14"] = (object) num14;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN15"] = (object) num15;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN16"] = (object) num16;
                    }
                  }
                }
                else
                {
                  dtLOG.Rows.Add();
                  dtLOG.Rows[dtLOG.Rows.Count - 1]["CODPOS"] = (object) num10;
                  dtLOG.Rows[dtLOG.Rows.Count - 1]["ANNDEN"] = (object) year;
                  dtLOG.Rows[dtLOG.Rows.Count - 1]["MESDEN"] = (object) month;
                  dtLOG.Rows[dtLOG.Rows.Count - 1]["MAT"] = (object) MAT;
                  dtLOG.Rows[dtLOG.Rows.Count - 1]["DESERR"] = (object) "DATA DI ISCRIZIONE DELLA MATRICOLA NON TROVATA";
                }
              }
              else
              {
                dtLOG.Rows.Add();
                dtLOG.Rows[dtLOG.Rows.Count - 1]["CODPOS"] = (object) num10;
                dtLOG.Rows[dtLOG.Rows.Count - 1]["ANNDEN"] = (object) year;
                dtLOG.Rows[dtLOG.Rows.Count - 1]["MESDEN"] = (object) month;
                dtLOG.Rows[dtLOG.Rows.Count - 1]["MAT"] = (object) MAT;
                dtLOG.Rows[dtLOG.Rows.Count - 1]["DESERR"] = (object) "DATA DI NASCITA DELLA MATRICOLA NON TROVATA";
              }
            }
          }
        }
        for (int index5 = 0; index5 <= dataTable1.Rows.Count - 1; ++index5)
        {
          int num17 = !DBNull.Value.Equals(dataTable1.Rows[index5]["CODPOS"]) ? Convert.ToInt32(dataTable1.Rows[index5]["CODPOS"]) : 0;
          int num18 = !DBNull.Value.Equals(dataTable1.Rows[index5]["ANNDEN"]) ? Convert.ToInt32(dataTable1.Rows[index5]["ANNDEN"]) : 0;
          int num19 = !DBNull.Value.Equals(dataTable1.Rows[index5]["MESDEN"]) ? Convert.ToInt32(dataTable1.Rows[index5]["MESDEN"]) : 0;
          int num20 = !DBNull.Value.Equals(dataTable1.Rows[index5]["MAT"]) ? Convert.ToInt32(dataTable1.Rows[index5]["MAT"]) : 0;
          Decimal num21 = !DBNull.Value.Equals(dataTable1.Rows[index5]["ALIQUOTA"]) ? Convert.ToDecimal(dataTable1.Rows[index5]["ALIQUOTA"]) : 0M;
          Decimal num22 = !DBNull.Value.Equals(dataTable1.Rows[index5]["PERAPP"]) ? Convert.ToDecimal(dataTable1.Rows[index5]["PERAPP"]) : 0M;
          Decimal num23 = !DBNull.Value.Equals(dataTable1.Rows[index5]["PERPAR"]) ? Convert.ToDecimal(dataTable1.Rows[index5]["PERPAR"]) : 0M;
          string str29 = !DBNull.Value.Equals(dataTable1.Rows[index5]["TIPSPE"]) ? dataTable1.Rows[index5]["TIPSPE"].ToString() : string.Empty;
          if (num17 == num1 && num18 == num7 && num19 == num8 && num20 == num2 && num21 == num3)
          {
            dataTable1.Rows[index5 - index1]["AL"] = dataTable1.Rows[index5]["AL"];
            dataTable1.Rows[index5 - index1]["IMPSCA"] = dataTable1.Rows[index5]["IMPSCA"];
            dataTable1.Rows[index5 - index1]["IMPMIN"] = dataTable1.Rows[index5]["IMPMIN"];
            dataTable1.Rows[index5 - index1]["NUMMEN"] = dataTable1.Rows[index5]["NUMMEN"];
            dataTable1.Rows[index5 - index1]["MESMEN14"] = dataTable1.Rows[index5]["MESMEN14"];
            dataTable1.Rows[index5 - index1]["MESMEN15"] = dataTable1.Rows[index5]["MESMEN15"];
            dataTable1.Rows[index5 - index1]["MESMEN16"] = dataTable1.Rows[index5]["MESMEN16"];
            dataTable1.Rows[index5 - index1]["DATDEC"] = dataTable1.Rows[index5]["DATDEC"];
            dataTable1.Rows[index5 - index1]["IMPTRAECO"] = dataTable1.Rows[index5]["IMPTRAECO"];
            dataTable1.Rows[index5 - index1]["IMPSCA"] = dataTable1.Rows[index5]["IMPSCA"];
            dataTable1.Rows[index5 - index1]["ABBPRE"] = dataTable1.Rows[index5]["ABBPRE"];
            dataTable1.Rows[index5 - index1]["IMPABB"] = dataTable1.Rows[index5]["IMPABB"];
            dataTable1.Rows[index5 - index1]["ASSCON"] = dataTable1.Rows[index5]["ASSCON"];
            dataTable1.Rows[index5 - index1]["IMPASSCON"] = dataTable1.Rows[index5]["IMPASSCON"];
            dataTable1.Rows[index5]["RIMUOVI"] = (object) "SI";
            ++index1;
          }
          else
            index1 = 1;
          num1 = num17;
          num7 = num18;
          num8 = num19;
          num2 = num20;
          num3 = num21;
          num4 = num22;
          num5 = num23;
        }
        for (int index6 = dataTable1.Rows.Count - 1; index6 >= 0; index6 += -1)
        {
          if (dataTable1.Rows[index6]["RIMUOVI"].ToString() == "SI")
            dataTable1.Rows.RemoveAt(index6);
        }
        for (int index7 = 0; index7 <= dataTable1.Rows.Count - 1; ++index7)
        {
          dataTable1.Rows[index7]["NUMGGPER"] = (object) this.Get_NumGG_Periodo(Convert.ToDateTime(dataTable1.Rows[index7]["DAL"]), Convert.ToDateTime(dataTable1.Rows[index7]["AL"]));
          dataTable1.Rows[index7]["NUMGGDOM"] = (object) this.Get_NumGG_Domeniche(Convert.ToDateTime(dataTable1.Rows[index7]["DAL"]), Convert.ToDateTime(dataTable1.Rows[index7]["AL"]));
          dataTable1.Rows[index7]["NUMGGAZI"] = (object) 0.0M;
          dataTable1.Rows[index7]["NUMGGSOS"] = (object) 0.0M;
          dataTable1.Rows[index7]["NUMGGFIG"] = (object) 0.0M;
          dataTable1.Rows[index7]["IMPOCC"] = (object) 0.0M;
          Decimal NUMGGAZI = !DBNull.Value.Equals(dataTable1.Rows[index7]["NUMGGAZI"]) ? Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGAZI"]) : 0M;
          Decimal NUMGGSOS = !DBNull.Value.Equals(dataTable1.Rows[index7]["NUMGGSOS"]) ? Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGSOS"]) : 0M;
          Decimal NUMGGFIG = !DBNull.Value.Equals(dataTable1.Rows[index7]["NUMGGFIG"]) ? Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGFIG"]) : 0M;
          int CODPOS = !DBNull.Value.Equals(dataTable1.Rows[index7]["CODPOS"]) ? Convert.ToInt32(dataTable1.Rows[index7]["CODPOS"]) : 0;
          int MAT = !DBNull.Value.Equals(dataTable1.Rows[index7]["MAT"]) ? Convert.ToInt32(dataTable1.Rows[index7]["MAT"]) : 0;
          int PRORAP = !DBNull.Value.Equals(dataTable1.Rows[index7]["PRORAP"]) ? Convert.ToInt32(dataTable1.Rows[index7]["PRORAP"]) : 0;
          this.Get_NumGG_Sospensioni(ref dataTable10, Convert.ToDateTime(dataTable1.Rows[index7]["DAL"]), Convert.ToDateTime(dataTable1.Rows[index7]["AL"]), CODPOS, MAT, PRORAP, ref NUMGGAZI, ref NUMGGSOS, ref NUMGGFIG);
          Decimal num24 = !DBNull.Value.Equals(dataTable1.Rows[index7]["NUMGGPER"]) ? Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGPER"]) : 0M;
          Decimal num25 = !DBNull.Value.Equals(dataTable1.Rows[index7]["NUMGGDOM"]) ? Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGDOM"]) : 0M;
          Decimal num26 = !DBNull.Value.Equals(dataTable1.Rows[index7]["NUMGGSOS"]) ? Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGSOS"]) : 0M;
          Decimal num27 = !DBNull.Value.Equals(dataTable1.Rows[index7]["NUMGGAZI"]) ? Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGAZI"]) : 0M;
          Decimal num28 = !DBNull.Value.Equals(dataTable1.Rows[index7]["GIORNI_MESE"]) ? Convert.ToDecimal(dataTable1.Rows[index7]["GIORNI_MESE"]) : 0M;
          dataTable1.Rows[index7]["NUMGGCONAZI"] = !(NUMGGSOS > 0M) ? (!(Convert.ToDateTime(dataTable1.Rows[index7]["DATDEC"]) <= Convert.ToDateTime(dataTable1.Rows[index7]["DAL"])) ? (object) (Decimal.Round(num28 / 26M) * num24) : (!(dataTable1.Rows[index7]["DATCES"].ToString().Trim() == "") ? (!(Convert.ToDateTime(dataTable1.Rows[index7]["DATCES"]) > Convert.ToDateTime(dataTable1.Rows[index7]["AL"])) ? (object) (Decimal.Round(num28 / 26M) * num24) : (object) (num24 - num25 - num26 + num27)) : (object) Decimal.Round(num24 * 26M / num28))) : (object) (num24 - num25 - num26 + num27);
          Decimal num29 = Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGCONAZI"]);
          dataTable1.Rows[index7]["IMPRET"] = !(dataTable1.Rows[index7]["TIPSPE"].ToString() == "S") ? (object) (Convert.ToDecimal(dataTable1.Rows[index7]["IMPTRAECO"]) * num29 / 26M) : (object) ((Convert.ToDecimal(dataTable1.Rows[index7]["IMPMIN"]) + Convert.ToDecimal(dataTable1.Rows[index7]["IMPSCA"])) * num29 / 26M);
          string str30 = dataTable1.Rows[index7]["ANNDEN"]?.ToString() + "-" + dataTable1.Rows[index7]["MESDEN"]?.ToString() + "-25";
          dateTime = Convert.ToDateTime(dataTable1.Rows[index7]["DATDEC"]);
          int month1 = dateTime.Month;
          dateTime = Convert.ToDateTime(str30);
          int month2 = dateTime.Month;
          int num30 = month1 - month2;
          Decimal num31 = 0M;
          int num32 = num30 + 1;
          if (num32 > 12)
            num32 = 12;
          int num33 = !DBNull.Value.Equals(dataTable1.Rows[index7]["MESDEN"]) ? Convert.ToInt32(dataTable1.Rows[index7]["MESDEN"]) : 0;
          Decimal num34 = !DBNull.Value.Equals(dataTable1.Rows[index7]["IMPRET"]) ? Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"]) : 0M;
          switch (Convert.ToInt32(dataTable1.Rows[index7]["NUMMEN"]))
          {
            case 13:
              if (num33 == 12)
              {
                num31 = num34 / 12M * (Decimal) num32;
                break;
              }
              break;
            case 14:
              if (num33 == 12)
                num31 = num34 / 12M * (Decimal) num32;
              if (dataTable1.Rows[index7]["MESMEN14"] == dataTable1.Rows[index7]["MESDEN"])
              {
                num31 += num34 / 12M * (Decimal) num32;
                break;
              }
              break;
            case 15:
              if (num33 == 12)
                num31 = num34 / 12M * (Decimal) num32;
              if (dataTable1.Rows[index7]["MESMEN14"] == dataTable1.Rows[index7]["MESDEN"])
                num31 += num34 / 12M * (Decimal) num32;
              if (dataTable1.Rows[index7]["MESMEN15"] == dataTable1.Rows[index7]["MESDEN"])
              {
                num31 += num34 / 12M * (Decimal) num32;
                break;
              }
              break;
            case 16:
              if (num33 == 12)
                num31 = num34 / 12M * (Decimal) num32;
              if (dataTable1.Rows[index7]["MESMEN14"] == dataTable1.Rows[index7]["MESDEN"])
                num31 += num34 / 12M * (Decimal) num32;
              if (dataTable1.Rows[index7]["MESMEN15"] == dataTable1.Rows[index7]["MESDEN"])
                num31 += num34 / 12M * (Decimal) num32;
              if (dataTable1.Rows[index7]["MESMEN16"] == dataTable1.Rows[index7]["MESDEN"])
              {
                num31 += num34 / 12M * (Decimal) num32;
                break;
              }
              break;
          }
          dataTable1.Rows[index7]["IMPRET"] = (object) (num34 + num31);
          if (Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"]) > 0M)
          {
            int length = Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"]).ToString().LastIndexOf(",");
            if (length > -1)
              dataTable1.Rows[index7]["IMPRET"] = !(Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"].ToString().Substring(length + 1)) < 50M) ? (object) (Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"].ToString().Substring(0, length)) + 1M) : (object) Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"].ToString().Substring(0, length));
          }
          dataTable1.Rows[index7]["IMPCON"] = (object) (Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"]) * Convert.ToDecimal(dataTable1.Rows[index7]["ALIQUOTA"]) / 100M);
          dataTable1.Rows[index7]["IMPFIG"] = !(dataTable1.Rows[index7]["TIPSPE"].ToString() == "S") ? (object) (Convert.ToDecimal(dataTable1.Rows[index7]["IMPTRAECO"]) * NUMGGFIG / 26M) : (object) ((Convert.ToDecimal(dataTable1.Rows[index7]["IMPMIN"]) + Convert.ToDecimal(dataTable1.Rows[index7]["IMPSCA"])) * NUMGGFIG / 26M);
          dataTable1.Rows[index7]["IMPADDREC"] = (object) (Convert.ToDecimal(dataTable1.Rows[index7]["IMPCON"]) * Convert.ToDecimal(this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 5, dataTable1.Rows[index7]["DAL"].ToString(), CODPOS)) / 100M);
          if (!PREV && Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGPER"]) - Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGDOM"]) == Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGSOS"]) & Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"]) == 0M & Convert.ToDecimal(dataTable1.Rows[index7]["IMPCON"]) == 0M & Convert.ToDecimal(dataTable1.Rows[index7]["IMPFIG"]) == 0M)
            dataTable1.Rows[index7]["RIMUOVI"] = (object) "SI";
        }
        for (int index8 = dataTable1.Rows.Count - 1; index8 >= 0; index8 += -1)
        {
          if (dataTable1.Rows[index8]["RIMUOVI"].ToString() == "SI")
            dataTable1.Rows.RemoveAt(index8);
        }
        if (STRRICPREV != "")
        {
          DataTable dataTable11 = new DataTable();
          string strSQL7 = "  SELECT DISTINCT MODPREDET.MAT, MODPREDET.CODPOS, MODPREDET.DAL, MODPREDET.AL," + " MODPREDET.ANNDEN, MODPREDET.MESDEN, '' AS FLAGMOD " + " FROM MODPREDET, MODPRE " + " WHERE MODPREDET.CODPOS = MODPRE.CODPOS " + " AND MODPREDET.MAT = MODPRE.MAT " + " AND MODPREDET.PRORAP = MODPRE.PRORAP " + " AND MODPREDET.PROMOD = MODPRE.PROMOD " + " AND MODPRE.DATANN IS NULL " + STRRICPREV;
          DataTable dataTable12 = objDataAccess.GetDataTable(strSQL7);
          if (dataTable12.Rows.Count > 0)
          {
            for (int index9 = 0; index9 <= dataTable12.Rows.Count - 1; ++index9)
            {
              for (int index10 = dataTable1.Rows.Count - 1; index10 >= 0; index10 += -1)
              {
                if (dataTable12.Rows[index9]["CODPOS"] == dataTable1.Rows[index10]["CODPOS"] && dataTable12.Rows[index9]["MAT"] == dataTable1.Rows[index10]["MAT"] && dataTable12.Rows[index9]["ANNDEN"] == dataTable1.Rows[index10]["ANNDEN"] && dataTable12.Rows[index9]["MESDEN"] == dataTable1.Rows[index10]["MESDEN"] && dataTable12.Rows[index9]["DAL"] == dataTable1.Rows[index10]["DAL"] && dataTable12.Rows[index9]["AL"] == dataTable1.Rows[index10]["AL"])
                {
                  dataTable1.Rows.RemoveAt(index10);
                  dataTable12.Rows[index9]["FLAGMOD"] = (object) "S";
                  break;
                }
              }
            }
          }
          DataTable dataTable13 = dataTable1.Clone();
          for (int index11 = 0; index11 <= dataTable12.Rows.Count - 1; ++index11)
          {
            if (dataTable12.Rows[index11]["FLAGMOD"].ToString() == "S")
            {
              string strSQL8 = " SELECT MODPREDET.CODPOS, MODPREDET.ANNDEN, MODPREDET.MESDEN," + " MODPREDET.MAT, '' AS NOME, MODPREDET.PRORAP, MODPREDET.DATNAS," + " MODPREDET.DATDEC, MODPREDET.DATCES, MODPREDET.DAL," + " MODPREDET.AL, MODPREDET.ETA65 AS ETAENNE, '' AS QUALIFICA," + " MODPREDET.ALIQUOTA, '' AS DATA_INIZIO_STORDL, '' AS DATA_FINE_STORDL," + " '' AS TIPO_RAPPORTO, MODPREDET.TIPRAP, MODPREDET.CODQUACON," + " MODPREDET.CODCON, MODPREDET.PROCON, MODPREDET.CODLOC," + " MODPREDET.PROLOC, MODPREDET.CODLIV, MODPREDET.FAP," + " MODPREDET.CODGRUASS, '' AS DENLIV, MODPREDET.IMPRET," + " MODPREDET.PERPAR, MODPREDET.PERAPP, MODPREDET.PERFAP," + " MODPREDET.NUMGGAZI, MODPREDET.NUMGGFIG, MODPREDET.NUMGGDOM," + " MODPREDET.NUMGGSOS, MODPREDET.NUMGGCONAZI, MODPREDET.NUMGGPER," + " MODPREDET.IMPFIG, MODPREDET.IMPABB, '' AS ABBPRE, '' AS ASSCON, 0.0 AS IMPADDREC," + " IMPASSCON, IMPCON,IMPMIN,IMPSCA,IMPTRAECO,TIPSPE, 'S' AS PREV, 0 AS GIORNI_MESE, MODPREDET.IMPOCC, " + " '' AS RIMUOVI, MODPRE.CODSTAPRE" + " FROM MODPREDET, MODPRE" + " WHERE MODPREDET.CODPOS = MODPRE.CODPOS" + " AND MODPREDET.MAT = MODPRE.MAT" + " AND MODPREDET.PRORAP = MODPRE.PRORAP" + " AND MODPREDET.PROMOD = MODPRE.PROMOD" + " AND MODPREDET.CODPOS = " + dataTable12.Rows[index11]["CODPOS"]?.ToString() + " AND MODPREDET.MAT = " + dataTable12.Rows[index11]["MAT"]?.ToString() + " AND MODPREDET.ANNDEN = " + dataTable12.Rows[index11]["ANNDEN"]?.ToString() + " AND MODPREDET.MESDEN = " + dataTable12.Rows[index11]["MESDEN"]?.ToString() + " AND MODPREDET.TIPMOV <> 'AR' ";
              dataTable13.Clear();
              dataTable13 = objDataAccess.GetDataTable(strSQL8);
              for (int index12 = 0; index12 <= dataTable13.Rows.Count - 1; ++index12)
              {
                if (Convert.ToInt32(dataTable13.Rows[index12]["CODSTAPRE"]) == 0)
                {
                  if (Convert.ToDecimal(dataTable13.Rows[index12]["IMPRET"]) == 0M)
                  {
                    dataTable13.Rows[index12]["IMPRET"] = !(dataTable13.Rows[index12]["TIPSPE"].ToString() == "S") ? (object) (Convert.ToDecimal(dataTable13.Rows[index12]["IMPTRAECO"]) * Convert.ToDecimal(dataTable13.Rows[index12]["NUMGGCONAZI"]) / 26M) : (object) ((Convert.ToDecimal(dataTable13.Rows[index12]["IMPMIN"]) + Convert.ToDecimal(dataTable13.Rows[index12]["IMPSCA"])) * Convert.ToDecimal(dataTable13.Rows[index12]["NUMGGCONAZI"]) / 26M);
                    if (Convert.ToDecimal(dataTable13.Rows[index12]["IMPRET"]) > 0M)
                    {
                      int length = dataTable13.Rows[index12]["IMPRET"].ToString().LastIndexOf(",");
                      if (length > -1)
                        dataTable13.Rows[index12]["IMPRET"] = !(Convert.ToDecimal(dataTable13.Rows[index12]["IMPRET"].ToString().Substring(length + 1)) < 50M) ? (object) (Convert.ToDecimal(dataTable13.Rows[index12]["IMPRET"].ToString().Substring(0, length)) + 1M) : (object) Convert.ToDecimal(dataTable13.Rows[index12]["IMPRET"].ToString().Substring(0, length));
                    }
                    dataTable13.Rows[index12]["IMPCON"] = (object) (Convert.ToDecimal(dataTable13.Rows[index12]["IMPRET"]) * Convert.ToDecimal(dataTable13.Rows[index12]["ALIQUOTA"]) / 100M);
                    dataTable13.Rows[index12]["IMPADDREC"] = (object) (Convert.ToDecimal(dataTable13.Rows[index12]["IMPCON"]) * Convert.ToDecimal(this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 5, dataTable13.Rows[index12]["DAL"].ToString(), Convert.ToInt32(dataTable13.Rows[index12]["CODPOS"]))) / 100M);
                  }
                  if (Convert.ToDecimal(dataTable13.Rows[index12]["IMPFIG"]) == 0M)
                    dataTable13.Rows[index12]["IMPFIG"] = !(dataTable13.Rows[index12]["TIPSPE"].ToString() == "S") ? (object) (Convert.ToDecimal(dataTable13.Rows[index12]["IMPTRAECO"]) * Convert.ToDecimal(dataTable13.Rows[index12]["NUMGGFIG"]) / 26M) : (object) ((Convert.ToDecimal(dataTable13.Rows[index12]["IMPMIN"]) + Convert.ToDecimal(dataTable13.Rows[index12]["IMPSCA"])) * Convert.ToDecimal(dataTable13.Rows[index12]["NUMGGFIG"]) / 26M);
                }
                dataTable1.ImportRow(dataTable13.Rows[index12]);
              }
            }
          }
        }
        for (int index13 = 0; index13 <= dataTable8.Rows.Count - 1; ++index13)
        {
          string strSQL9 = "SELECT * FROM PRAINFRDLDET WHERE ANNO = " + dataTable8.Rows[index13]["ANNO"]?.ToString() + " AND MESE = " + dataTable8.Rows[index13]["MESE"]?.ToString();
          DataTable dataTable14 = objDataAccess.GetDataTable(strSQL9);
          for (int index14 = 0; index14 <= dataTable14.Rows.Count - 1; ++index14)
          {
            for (int index15 = 0; index15 <= dataTable1.Rows.Count - 1; ++index15)
            {
              if (dataTable1.Rows[index15]["CODPOS"] == dataTable14.Rows[index14]["CODPOS"] && dataTable1.Rows[index15]["MAT"] == dataTable14.Rows[index14]["MAT"] && dataTable1.Rows[index15]["PRORAP"] == dataTable14.Rows[index14]["PRORAP"])
              {
                dataTable1.Rows[index15]["IMPFIG"] = dataTable14.Rows[index14]["IMPFIG"];
                break;
              }
            }
          }
        }
        return dataTable1;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public Decimal Get_Percentuale_Aliquota(
      DataLayer objDataAccess,
      string strData,
      string CODPOS,
      int PRORAP,
      long MAT,
      string STR65,
      string FAP,
      int CODQUACON,
      int CODGRUASS)
    {
      DataTable dataTable1 = new DataTable();
      string str = "Select VALUE(SUM(ALIQUOTA), 0.00) AS ALIQUOTA " + " FROM ALIFORASS " + " WHERE ALIFORASS.CODGRUASS = " + CODGRUASS.ToString() + " AND ALIFORASS.CODQUACON=" + CODQUACON.ToString() + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + "  BETWEEN ALIFORASS.DATINI AND VALUE(ALIFORASS.DATFIN,'9999-12-31') ";
      if (STR65 == "S")
        str += " AND ALIFORASS.CODFORASS IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS <> 'PREV') ";
      string strSQL1 = str + " AND ALIFORASS.CODFORASS NOT IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS = 'FAP') ";
      dataTable1.Clear();
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL1);
      Decimal percentualeAliquota = !DBNull.Value.Equals(dataTable2.Rows[0]["ALIQUOTA"]) ? Convert.ToDecimal(dataTable2.Rows[0]["ALIQUOTA"]) : 0M;
      if (FAP == "S")
      {
        string strSQL2 = "SELECT VALFAP FROM CODFAP WHERE " + DBMethods.Db2Date(strData) + " BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')";
        dataTable2.Clear();
        DataTable dataTable3 = objDataAccess.GetDataTable(strSQL2);
        if (dataTable3.Rows.Count > 0)
          percentualeAliquota += (Decimal) dataTable3.Rows[0]["VALFAP"];
      }
      return percentualeAliquota;
    }
  }
}
