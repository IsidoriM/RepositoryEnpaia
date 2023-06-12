// Decompiled with JetBrains decompiler
// Type: TFI.DAL.clsPrev
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Data;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;

namespace TFI.DAL
{
    public class clsPrev
    {
        private DataLayer objDataAccess = new DataLayer();

        public bool WRITE_INSERT_SOSPREV(
          string CODPOS,
          string MAT,
          string PRORAP,
          string PROMOD,
          string DATCES)
        {
            string str1 = $"01/01/'{Convert.ToDateTime(DATCES).AddYears(-1).ToString().Substring(6, 4)}'";
            string Err = "";
            bool blnCommit = false;
            this.objDataAccess.StartTransaction();
            if (this.CalcoloFigurativa(CODPOS, MAT, PRORAP, PROMOD, DATCES))
            {
                string strSQL =
                    $"DELETE FROM MODPRESOS WHERE CODPOS = '{CODPOS}' AND MAT = '{MAT}'  AND PRORAP = '{PRORAP}' AND PROMOD = '{PROMOD}' ";
                if (this.objDataAccess.WriteTransactionData(strSQL, CommandType.Text))
                {
                    strSQL =
                        $"SELECT CODPOS, MAT, PRORAP, PROSOS, CODSOS, DATINISOS, DATFINSOS, PERAZI, PERFIG, STASOS, CODPOSDA, CODPOSA  FROM SOSRAP  WHERE CODPOS = '{CODPOS}' AND MAT = '{MAT}' AND PRORAP = '{PRORAP}' AND DATINISOS <= '{DATCES}' AND DATFINSOS >= '{str1}' AND STASOS = '0' ";
                    DataSet dataSet1 = new DataSet();
                    DataSet dataSet2 = this.objDataAccess.GetDataSet(strSQL, ref Err);
                    string str2 = dataSet2.Tables[0].Rows[0]["PROSOS"].ToString();
                    string str3 = dataSet2.Tables[0].Rows[0]["CODSOS"].ToString();
                    string str4 = dataSet2.Tables[0].Rows[0]["DATINISOS"].ToString();
                    string str5 = dataSet2.Tables[0].Rows[0]["DATFINSOS"].ToString();
                    string str6 = dataSet2.Tables[0].Rows[0]["PERAZI"].ToString();
                    string str7 = dataSet2.Tables[0].Rows[0]["PERFIG"].ToString();
                    string str8 = dataSet2.Tables[0].Rows[0]["STASOS"].ToString();
                    string str9 = dataSet2.Tables[0].Rows[0]["CODPOSDA"].ToString();
                    string str10 = dataSet2.Tables[0].Rows[0]["CODPOSA"].ToString();
                    blnCommit = this.objDataAccess.WriteTransactionData(
                        $"INSERT INTO MODPRESOS (CODPOS, MAT, PRORAP, PROSOS, CODSOS, PROMOD, DATINISOS, DATFINSOS, PERAZI, PERFIG, STASOS, CODPOSDA, CODPOSA)  VALUES ('{CODPOS}', '{MAT}', '{PRORAP}', '{str2}', '{str3}', '{PROMOD}', '{str4}',  '{str5}', '{str6}', '{str7}', '{str8}', '{str9}', '{str10}') ", CommandType.Text);
                }
            }
            this.objDataAccess.EndTransaction(blnCommit);
            return blnCommit;
        }

        public bool WRITE_UPDATE_MODPREPAR(
          string CODPOS,
          string MAT,
          string PRORAP,
          string PROMOD,
          string DATCES)
        {
            Decimal num1 = 0M;
            bool flag1 = false;
            bool flag2 = false;
            bool blnCommit = true;
            string str1 = (DateTime.Parse(DATCES).Year - 1).ToString() + "-01-01";
            string strSQL1 = "SELECT CODPOS, MAT, PRORAP, DATINI, CASE DATFIN WHEN '9999-12-31' THEN " + DATCES + "' " + " ELSE DATFIN END AS DATFIN, TIPRAP, PERPAR, PERAPP FROM STORDL WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + MAT + "' " + " AND PRORAP = '" + PRORAP + "' AND (DATINI >= '" + str1 + "' OR '" + str1 + "' " + " BETWEEN DATINI AND DATFIN OR '" + DATCES + "' " + " BETWEEN DATINI AND DATFIN) AND DATINI <= '" + DATCES + "' AND VALUE(PERPAR, 0) > 0 " + " ORDER BY DATINI ";
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL1);
            string strSQL2 = "SELECT DATINI, DATFIN, PERPAR, PERAPP FROM MODPREPAR WHERE CODPOS ='" + CODPOS + "' " + " AND MAT = '" + MAT + "' AND PRORAP = '" + PRORAP + "' AND PROMOD = '" + PROMOD + "' " + " ORDER BY DATINI ";
            DataTable dataTable3 = new DataTable();
            DataTable dataTable4 = this.objDataAccess.GetDataTable(strSQL2);
            int index1 = 0;
            if (dataTable4.Rows.Count == dataTable2.Rows.Count)
            {
                if (dataTable2.Rows.Count > 0)
                {
                    while (!flag1)
                    {
                        if (index1 > 0 && Convert.ToDecimal(dataTable2.Rows[index1]["PERPAR"]) == num1)
                        {
                            dataTable2.Rows[index1 - 1]["DATFIN"] = dataTable2.Rows[index1]["DATFIN"];
                            dataTable2.Rows.RemoveAt(index1);
                        }
                        if (index1 < dataTable2.Rows.Count - 1)
                        {
                            num1 = Convert.ToDecimal(dataTable2.Rows[index1]["PERPAR"]);
                            ++index1;
                        }
                        else
                            flag1 = true;
                    }
                    int num2 = dataTable2.Rows.Count - 1;
                    for (int index2 = 0; index2 <= num2; ++index2)
                    {
                        if (dataTable2.Rows[index2]["DATINI"] != dataTable4.Rows[index2]["DATINI"] || dataTable2.Rows[index2]["DATFIN"] != dataTable4.Rows[index2]["DATFIN"] || dataTable2.Rows[index2]["PERPAR"] != dataTable4.Rows[index2]["PERPAR"])
                        {
                            flag2 = true;
                            break;
                        }
                    }
                }
            }
            else if (dataTable2.Rows.Count > 0)
                flag2 = true;
            if (flag2)
            {
                this.objDataAccess.StartTransaction();
                blnCommit = this.objDataAccess.WriteTransactionData("DELETE FROM MODPREPAR WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + MAT + "' AND PROMOD = '" + PROMOD.ToString() + "' ", CommandType.Text);
                if (blnCommit)
                {
                    int num3 = dataTable2.Rows.Count - 1;
                    for (int index3 = 0; index3 <= num3; ++index3)
                    {
                        Decimal num4 = Convert.ToDecimal(dataTable2.Rows[index3]["PERPAR"]);
                        string str2 = "INSERT INTO MODPREPAR (CODPOS, MAT, PRORAP, PROMOD, DATINI, DATFIN, TIPRAP, " + "PERAPP, PERPAR, ULTAGG, UTEAGG) VALUES ('" + CODPOS + "', '" + MAT + "', '" + PRORAP + "', " + " '" + PROMOD + "', '" + dataTable2.Rows[index3]["DATINI"].ToString() + "', " + " '" + dataTable2.Rows[index3]["DATFIN"]?.ToString() + "', '" + dataTable2.Rows[index3]["TIPRAP"].ToString() + "' ";
                        blnCommit = this.objDataAccess.WriteTransactionData((!(dataTable2.Rows[index3]["PERAPP"].ToString() == "") ? " '" + dataTable2.Rows[index3]["PERAPP"]?.ToString() + "', " : str2 + "Null,") + " '" + num4.ToString().Replace(",", ".") + "', CURRENT_TIMESTAMP ", CommandType.Text);
                    }
                }
                this.objDataAccess.EndTransaction(blnCommit);
            }
            return blnCommit;
        }

        public bool WRITE_UPDATE_MODPREDET(
          string CODPOS,
          string MAT,
          string PRORAP,
          string PROMOD,
          string DATCES)
        {
            DataView dataView1 = (DataView)null;
            short index1 = 0;
            bool flag1 = true;
            this.objDataAccess.StartTransaction();
            string strSQL1 = "UPDATE DENDET SET AL = '" + DATCES + "' WHERE CODPOS ='" + CODPOS + "' " + " AND MAT = '" + MAT + "' AND PRORAP = '" + PRORAP + "' AND ANNDEN = '" + DateTime.Parse(DATCES).Year.ToString() + "' " + " AND MESDEN = '" + DateTime.Parse(DATCES).Month.ToString() + "' AND AL > '" + DATCES + "' " + " AND NUMMOV IS NULL";
            bool flag2 = this.objDataAccess.WriteTransactionData(strSQL1, CommandType.Text);
            string strSQL2 = "SELECT * FROM DENDET WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + MAT + "' " + " AND PRORAP = '" + PRORAP + "' AND TIPMOV = 'AR' AND (ANNCOM = '" + DateTime.Parse(DATCES).Year.ToString() + "' " + " OR ANNCOM = '" + (DateTime.Parse(DATCES).Year - 1).ToString() + "' ) " + " AND NUMMOVANN IS NULL AND VALUE(ESIRET, '') <> 'S' ";
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL2);
            if (dataTable2.Rows.Count > 0)
            {
                string strSQL3 = "SELECT CODPOS, ANNDEN, MESDEN, PRODEN, ANNCOM FROM MODPREDET WHERE CODPOS = '" + CODPOS + "' " + " AND  MAT = '" + MAT + "' AND PRORAP = '" + PRORAP + "' AND PROMOD = '" + PROMOD + "' AND TIPMOV = 'AR' ";
                DataView dataView2 = new DataView();
                DataView dataView3 = this.objDataAccess.GetDataView(strSQL3);
                int num = dataTable2.Rows.Count - 1;
                for (int index2 = 0; index2 <= num; ++index2)
                {
                    dataView3.RowFilter = " AND ANNDEN = '" + dataTable2.Rows[index2]["ANNDEN"]?.ToString() + "' AND MESDEN = '" + dataTable2.Rows[index2]["MESDEN"]?.ToString() + "' AND PRODEN = '" + dataTable2.Rows[index2]["PRODEN"]?.ToString() + "' ";
                    if (dataView3.Count == 0)
                    {
                        int int32 = Convert.ToInt32("0" + this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(MAX(PROMODDET), 0) + 1 FROM MODPREDET WHERE CODPOS = '" + CODPOS + "' " + " AND  MAT = '" + MAT + "' AND PRORAP = '" + PRORAP + "' AND PROMOD = '" + PROMOD + "' ", CommandType.Text));
                        string str = "INSERT INTO MODPREDET SELECT CODPOS, MAT, PRORAP, PROMOD, PROMODDET, " + " ANNDEN, MESDEN, TIPMOV, DAL, AL, DATERO, IMPRET, IMPOCC, IMPFIG, IMPABB, " + " IMPASSCON, IMPMIN, DATDEC, DATCES, NUMGGAZI, NUMGGFIG, NUMGGPER, NUMGGDOM, NUMGGSOS, " + " NUMGGCONAZI, IMPSCA, IMPTRAECO, ETA65, TIPRAP, FAP, PERFAP, IMPFAP, PERPAR, PERAPP, CODCON, " + " PROCON, TIPSPE, CODLOC, PROLOC, CODLIV, CODGRUASS, CODQUACON, ALIQUOTA, DATNAS, DATINPS, " + " DATSAP, ANNCOM, NOTE, IMPSANDET, TIPDEN, NUMMOV, DATCONMOV, NUMMOVANN, DATMOVANN, NUMGRURET, " + " NUMGRURETRIF, ESIRET, IMPRETDEL, IMPOCCDEL, IMPFIGDEL, IMPABBDEL, IMPASSCONDEL, PRIORITA, " + " PRORETTES, ANNRET, PRORET, IMPCONDEL, DATINISAN, DATFINSAN, IMPRETPRE, IMPOCCPRE, IMPFIGPRE, " + " IMPSANDETPRE, IMPCONPRE, CODCAUSAN, TASSAN, IMPABBPRE, IMPASSCONPRE, NUMSANANN, DATSANANN, " + " CODSANANN, NUMRETTES, NUMRETTESRIF, BILMOVANN, BILSANANN, NUMSAN, TIPANNMOVARR, TIPANNMOVARRANN, " + " 0 AS IMPRETPRV, 0 AS IMPOCCPRV, 0 AS IMPFIGPRV, 0 AS IMPABBPRV, 0 AS IMPASSCONPRV, 0 AS ";
                        str = " IMPCONPRV, ULTAGG, UTEAGG, PRODENDET, PRODEN FROM DENDET WHERE CODPOS = '" + CODPOS + "' ";
                        str = " AND MAT = '" + MAT + "' AND ANNDEN = '" + dataTable2.Rows[index2]["ANNDEN"].ToString() + "' AND MESDEN = '" + dataTable2.Rows[index2]["MESDEN"].ToString() + "' ";
                        flag2 = this.objDataAccess.WriteTransactionData(" AND PRODEN ='" + dataTable2.Rows[index2]["PRODEN"].ToString() + "' AND TIPMOV = 'AR' ", CommandType.Text);
                        if (flag2)
                        {
                            strSQL1 = "UPDATE MODPREDET SET IMPRETPRV = Null, IMPOCCPRV = Null, IMPFIGPRV = Null, IMPABBPRV = Null, " + " IMPASSCONPRV = Null, IMPCONPRV = Null WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + MAT + "' " + " AND PROMOD = '" + PROMOD + "' AND PROMODDET = '" + int32.ToString() + "' ";
                            flag2 = this.objDataAccess.WriteTransactionData(strSQL1, CommandType.Text);
                        }
                        else
                            break;
                    }
                }
            }
            string str1 = " SELECT A.ANNDEN, A.MESDEN, A.PROMODDET, A.DAL, B.PRODEN, B.PRODENDET, A.NUMMOV, B.NUMMOV ";
            string strSQL4 = strSQL1 + " AS NUMMOVDIPA, B.NUMMOVANN, B.IMPRET AS IMPRETDIPA, B.IMPOCC AS IMPOCCDIPA, B.IMPFIG ";
            string strSQL5 = str1 + " AS IMPFIGDIPA, B.IMPABB AS IMPABBDIPA, B.IMPASSCON AS IMPASSCONDIPA, B.IMPCON AS " + " IMPCONDIPA, B.IMPSANDET AS IMPSANDETDIPA, B.NUMSAN AS NUMSANDIPA, B.DATCONMOV AS " + " DATCONMOVDIPA, B.DATINISAN AS DATINISANDIPA, B.DATFINSAN AS DATFINSANDIPA, B.CODCAUSAN " + " AS CODCAUSANDIPA, B.TASSAN AS TASSANDIPA FROM MODPREDET A INNER JOIN DENDET B ON " + " A.CODPOS = B.CODPOS AND A.MAT = B.MAT AND A.PRORAP = B.PRORAP AND A.ANNDEN = B.ANNDEN " + " AND A.MESDEN = B.MESDEN AND A.DAL = B.DAL AND A.AL = B.AL WHERE A.CODPOS = '" + CODPOS + "' " + " AND A.MAT = '" + MAT + "' AND A.PRORAP = '" + PRORAP + "' AND A.PROMOD = '" + PROMOD + "' " + " AND B.TIPMOV = 'NU' AND A.NUMMOV IS NULL AND B.NUMMOV IS NOT NULL AND B.NUMMOVANN IS " + " NULL AND VALUE(B.ESIRET, '') <> 'S'";
            DataTable dataTable3 = new DataTable();
            DataTable dataTable4 = this.objDataAccess.GetDataTable(strSQL5);
            int num1 = dataTable4.Rows.Count - 1;
            for (int index3 = 0; index3 <= num1; ++index3)
            {
                if (!string.IsNullOrEmpty(dataTable4.Rows[index3]["NUMMOVDIPA"].ToString()))
                {
                    string str2 = "UPDATE MODPREDET SET IMPRET = '" + dataTable4.Rows[index3]["IMPRETDIPA"].ToString().Replace(",", ".") + "', " + " IMPOCC = '" + dataTable4.Rows[index3]["IMPOCCDIPA"].ToString().Replace(",", ".") + "', " + " IMPFIG = '" + dataTable4.Rows[index3]["IMPFIGDIPA"].ToString().Replace(",", ".") + "', " + " IMPCON = '" + dataTable4.Rows[index3]["IMPCONDIPA"].ToString().Replace(",", ".") + "', " + " IMPABB = '" + dataTable4.Rows[index3]["IMPABBDIPA"].ToString().Replace(",", ".") + "', " + " IMPASSCON = '" + dataTable4.Rows[index3]["IMPASSCONDIPA"].ToString().Replace(",", ".") + "', " + " NUMMOV = '" + dataTable4.Rows[index3]["NUMMOVDIPA"]?.ToString() + "', " + " DATCONMOV = '" + dataTable4.Rows[index3]["DATCONMOVDIPA"]?.ToString() + "', ";
                    if (!string.IsNullOrEmpty(dataTable4.Rows[index3]["NUMSANDIPA"].ToString()))
                        str2 = str2 + " NUMSAN = '" + dataTable4.Rows[index3]["NUMSANDIPA"].ToString() + "', " + " IMPSANDET = " + dataTable4.Rows[index3]["IMPSANDETDIPA"].ToString().Replace(",", ".") + ", " + " CODCAUSAN = '" + dataTable4.Rows[index3]["CODCAUSANDIPA"].ToString() + "', ";
                    if (!string.IsNullOrEmpty(dataTable4.Rows[index3]["DATINISANDIPA"].ToString()))
                        str2 = str2 + " DATINISAN = '" + DBMethods.Db2Date(dataTable2.Rows[index3]["DATINISANDIPA"].ToString()) + "', ";
                    if (!string.IsNullOrEmpty(dataTable4.Rows[index3]["DATFINSANDIPA"].ToString()))
                        str2 = str2 + " DATFINSAN = '" + DBMethods.Db2Date(dataTable2.Rows[index3]["DATFINSANDIPA"].ToString()) + "', ";
                    if (!string.IsNullOrEmpty(dataTable2.Rows[index3]["TASSANDIPA"].ToString()))
                        str2 = str2 + " TASSAN = '" + dataTable2.Rows[index3]["TASSANDIPA"].ToString().Replace(",", ".") + "', ";
                    flag2 = this.objDataAccess.WriteTransactionData(str2 + " PRODEN = '" + dataTable4.Rows[index3]["PRODEN"].ToString() + "', " + " PRODENDET = '" + dataTable4.Rows[index3]["PRODENDET"].ToString().Substring(0, strSQL4.Length - 2) + "', " + "  WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + MAT + "' " + " AND PRORAP = " + PRORAP + " AND PROMOD = " + PROMOD + " AND ANNDEN = '" + dataTable4.Rows[index3]["ANNDEN"].ToString() + "' AND MESDEN = '" + dataTable4.Rows[index3]["MESDEN"].ToString() + "' " + " AND PROMODDET = '" + dataTable4.Rows[index3]["PROMODDET"].ToString() + "' AND TIPMOV <> 'AR' ", CommandType.Text);
                }
            }
            string strSQL6 = " SELECT * FROM DENDET WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + MAT + "' AND " + " PRORAP = '" + PRORAP + "' AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL AND (VALUE(ESIRET, '') = 'S' " + " OR TIPMOV = 'RT') ORDER BY ANNDEN, MESDEN, PRODEN, PRODENDET, DAL, AL ";
            DataTable dataTable5 = new DataTable();
            DataTable dataTable6 = this.objDataAccess.GetDataTable(strSQL6);
            DataView dataView4 = new DataView();
            if (dataTable6.Rows.Count > 0)
                dataView4 = this.objDataAccess.GetDataView(" SELECT * FROM MODPREDET WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + MAT + "' " + " AND PRORAP = '" + PRORAP + "' AND PROMOD = '" + PROMOD + "' ");
            for (; (int)index1 < dataTable6.Rows.Count - 1; ++index1)
            {
                dataView4.RowFilter = " AND ANNDEN = '" + Convert.ToDecimal(dataTable6.Rows[(int)index1]["ANNDEN"]).ToString() + "' AND MESDEN = '" + Convert.ToDecimal(dataTable6.Rows[(int)index1]["MESDEN"]).ToString() + "' ";
                DataView dataView5 = dataView4;
                dataView5.RowFilter = dataView5.RowFilter + " AND PRODEN = '" + dataTable6.Rows[(int)index1]["PRODEN"].ToString() + "' AND PRODENDET = '" + dataTable6.Rows[(int)index1]["PRODENDET"].ToString() + "' ";
                if (dataView4.Count > 0)
                {
                    int num2 = dataTable6.Rows.Count - 1;
                    for (int index4 = (int)index1; index4 <= num2; ++index4)
                    {
                        if (string.IsNullOrEmpty(dataTable6.Rows[index4]["ESIRET"].ToString()))
                        {
                            if (dataTable4.Rows[index4]["IMPRET"].ToString() != dataView4[0]["IMPRET"].ToString() || dataTable4.Rows[index4]["IMPOCC"].ToString() != dataView4[0]["IMPOCC"].ToString() || dataTable4.Rows[index4]["IMPFIG"].ToString() != dataView4[0]["IMPFIG"].ToString())
                            {
                                string str3 = "UPDATE MODPREDET SET IMPRET = '" + dataTable4.Rows[index4]["IMPRET"].ToString().Replace(",", ".") + "', " + " IMPOCC = '" + dataTable4.Rows[index4]["IMPOCC"].ToString().Replace(",", ".") + "', " + " IMPFIG = '" + dataTable4.Rows[index4]["IMPFIG"].ToString().Replace(",", ".") + "', " + " IMPCON = '" + dataTable4.Rows[index4]["IMPCON"].ToString().Replace(",", ".") + "', " + " IMPABB = '" + dataTable4.Rows[index4]["IMPABB"].ToString().Replace(",", ".") + "', " + " IMPASSCON = '" + dataTable4.Rows[index4]["IMPASSCON"].ToString().Replace(",", ".") + "', " + " NUMMOV = '" + dataTable4.Rows[index4]["NUMMOV"].ToString() + "', " + " DATCONMOV = '" + DBMethods.Db2Date(dataTable4.Rows[index4]["DATCONMOV"].ToString()) + "', " + " IMPRETPRV = Null, IMPOCCPRV = Null, IMPFIGPRV = Null, IMPCONPRV = Null, ";
                                if (!string.IsNullOrEmpty(dataTable4.Rows[index4]["NUMSAN"].ToString()))
                                    str3 = str3 + " NUMSAN = '" + dataTable4.Rows[index4]["NUMSAN"].ToString() + "', " + " IMPSANDET = '" + dataTable4.Rows[index4]["IMPSANDET"].ToString().Replace(",", ".") + "', " + " CODCAUSAN = '" + dataTable4.Rows[index4]["CODCAUSAN"].ToString() + "', ";
                                if (!string.IsNullOrEmpty(dataTable4.Rows[index4]["DATINISAN"].ToString()))
                                    str3 = str3 + " DATINISAN = '" + DBMethods.Db2Date(dataTable2.Rows[index4]["DATINISAN"].ToString()) + "', ";
                                if (!string.IsNullOrEmpty(dataTable2.Rows[index4]["DATFINSAN"].ToString()))
                                    str3 = str3 + " DATFINSAN = '" + DBMethods.Db2Date(dataTable2.Rows[index4]["DATFINSAN"].ToString()) + "', ";
                                if (!string.IsNullOrEmpty(dataTable2.Rows[index4]["TASSAN"].ToString()))
                                    str3 = str3 + " TASSAN = '" + dataTable2.Rows[index4]["TASSAN"].ToString().Replace(",", ".") + "', ";
                                string str4 = str3 + " PRODEN = '" + dataTable2.Rows[index4]["PRODEN"].ToString() + "', " + " PRODENDET = '" + dataTable2.Rows[index4]["PRODENDET"].ToString().Substring(0, strSQL4.Length - 2) + "', " + " WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + MAT + "' " + " AND PRORAP = '" + PRORAP + "' AND PROMOD = '" + PROMOD + "' " + " AND ANNDEN = '" + dataTable2.Rows[index4]["ANNDEN"].ToString() + "' AND MESDEN = '" + dataTable2.Rows[index4]["MESDEN"].ToString() + "' " + " AND PROMODDET = '" + dataView1[0]["PROMODDET"].ToString() + "' ";
                                flag2 = this.objDataAccess.WriteTransactionData(strSQL4, CommandType.Text);
                                break;
                            }
                            break;
                        }
                    }
                }
            }
            string strSQL7 = "SELECT A.ANNDEN, A.MESDEN, A.PROMODDET, A.PRODEN, A.PRODENDET, A.DAL, A.NUMMOV, A.TIPMOV, " + " B.NUMMOV AS NUMMOVDIPA, B.NUMMOVANN, B.IMPRET AS IMPRETDIPA, B.IMPOCC AS IMPOCCDIPA, " + " B.IMPFIG AS IMPFIGDIPA, B.IMPABB AS IMPABBDIPA, B.IMPASSCON AS IMPASSCONDIPA, B.IMPCON " + " AS IMPCONDIPA, B.IMPSANDET AS IMPSANDETDIPA, B.NUMSAN AS NUMSANDIPA, B.DATCONMOV AS " + " DATCONMOVDIPA, B.DATINISAN AS DATINISANDIPA, B.DATFINSAN AS DATFINSANDIPA, B.CODCAUSAN " + " AS CODCAUSANDIPA, B.TASSAN AS TASSANDIPA, IMPRETPRV, IMPOCCPRV, IMPFIGPRV, IMPCONPRV " + " FROM MODPREDET A INNER JOIN DENDET B ON A.CODPOS = B.CODPOS AND A.MAT = B.MAT AND " + " A.PRORAP = B.PRORAP AND A.ANNDEN = B.ANNDEN AND A.MESDEN = B.MESDEN AND A.PRODEN = " + " B.PRODEN AND A.PRODENDET = B.PRODENDET WHERE A.CODPOS = '" + CODPOS + "' " + " AND B.NUMMOV IS NOT NULL AND (B.NUMMOVANN IS NOT NULL OR A.NUMMOV IS NULL) AND " + " VALUE(B.ESIRET, '') <> 'S'";
            DataTable dataTable7 = new DataTable();
            DataTable dataTable8 = this.objDataAccess.GetDataTable(strSQL7);
            int num3 = dataTable8.Rows.Count - 1;
            for (int index5 = 0; index5 <= num3; ++index5)
            {
                if (!string.IsNullOrEmpty(dataTable8.Rows[index5]["NUMMOVANN"].ToString()))
                {
                    string str5 = "";
                    if (dataTable8.Rows[index5]["TIPMOV"].ToString() != "AR" && DateTime.Compare(DateTime.Parse(dataTable8.Rows[index5]["DAL"].ToString()), DateTime.Parse(DATCES)) > 0)
                    {
                        if (Convert.ToDecimal(dataTable8.Rows[index5]["IMPRETDIPA"]) == 0M & Convert.ToDecimal(dataTable8.Rows[index5]["IMPOCCDIPA"]) == 0M & Convert.ToDecimal(dataTable8.Rows[index5]["IMPFIGDIPA"]) == 0M)
                            str5 = "DELETE FROM MODPREDET ";
                    }
                    else
                    {
                        string str6 = "UPDATE MODPREDET SET ";
                        if (!string.IsNullOrEmpty(dataTable8.Rows[index5]["IMPRETPRV"].ToString()))
                            str6 += " IMPRET = VALUE(IMPRETPRV, 0), ";
                        if (!string.IsNullOrEmpty(dataTable8.Rows[index5]["IMPOCCPRV"].ToString()))
                            str6 += " IMPOCC = VALUE(IMPOCCPRV, 0), ";
                        if (!string.IsNullOrEmpty(dataTable8.Rows[index5]["IMPFIGPRV"].ToString()))
                            str6 += " IMPFIG = VALUE(IMPFIGPRV, 0), ";
                        if (!string.IsNullOrEmpty(dataTable8.Rows[index5]["IMPCONPRV"].ToString()))
                            str6 += "IMPCON = VALUE(IMPCONPRV, 0), ";
                        str5 = str6 + " IMPRETPRV = Null, IMPOCCPRV = Null, IMPFIGPRV = Null, IMPCONPRV = Null, " + " PRODEN = Null, PRODENDET = Null, NUMMOV = Null, DATCONMOV = Null,  " + " NUMSAN = Null, CODCAUSAN = Null, IMPSANDET = 0, DATINISAN = Null, " + " DATFINSAN = Null, TASSAN = Null ";
                    }
                    flag2 = this.objDataAccess.WriteTransactionData(str5 + " WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + MAT + "'" + " AND PRORAP = '" + PRORAP + "' AND PROMOD = '" + PROMOD + "' " + " AND ANNDEN = '" + dataTable8.Rows[index5]["ANNDEN"]?.ToString() + "' " + " AND MESDEN = '" + dataTable8.Rows[index5]["MESDEN"]?.ToString() + "' " + " AND PRODEN = '" + dataTable8.Rows[index5]["PRODEN"]?.ToString() + "' " + " AND PRODENDET = '" + dataTable8.Rows[index5]["PRODENDET"]?.ToString() + "' ", CommandType.Text);
                }
                if (!string.IsNullOrEmpty(dataTable8.Rows[index5]["NUMMOVDIPA"].ToString()) && string.IsNullOrEmpty(dataTable8.Rows[index5]["NUMMOV"].ToString()))
                {
                    string str7 = "UPDATE MODPREDET SET IMPRET = '" + dataTable8.Rows[index5]["IMPRETDIPA"].ToString().Replace(",", ".") + "', " + " IMPOCC = '" + dataTable8.Rows[index5]["IMPOCCDIPA"].ToString().Replace(",", ".") + "', " + " IMPFIG = '" + dataTable8.Rows[index5]["IMPFIGDIPA"].ToString().Replace(",", ".") + "', " + " IMPCON = '" + dataTable8.Rows[index5]["IMPCONDIPA"].ToString().Replace(",", ".") + "', " + " IMPABB = '" + dataTable8.Rows[index5]["IMPABBDIPA"].ToString().Replace(",", ".") + "', " + " IMPASSCON = '" + dataTable8.Rows[index5]["IMPASSCONDIPA"].ToString().Replace(",", ".") + "', " + " NUMMOV = '" + dataTable8.Rows[index5]["NUMMOVDIPA"].ToString() + "', " + " DATCONMOV = '" + DBMethods.Db2Date(dataTable8.Rows[index5]["DATCONMOVDIPA"].ToString()) + "', ";
                    if (!string.IsNullOrEmpty(dataTable8.Rows[index5]["NUMSANDIPA"].ToString()))
                        str7 = str7 + " NUMSAN = '" + dataTable8.Rows[index5]["NUMSANDIPA"]?.ToString() + "', " + "IMPSANDET = " + dataTable8.Rows[index5]["IMPSANDETDIPA"].ToString().Replace(",", ".") + ", " + " CODCAUSAN = '" + dataTable8.Rows[index5]["CODCAUSANDIPA"].ToString() + "', ";
                    if (!string.IsNullOrEmpty(dataTable2.Rows[index5]["DATINISANDIPA"].ToString()))
                        str7 = str7 + " DATINISAN = '" + DBMethods.Db2Date(dataTable8.Rows[index5]["DATINISANDIPA"].ToString()) + "', ";
                    if (!string.IsNullOrEmpty(dataTable2.Rows[index5]["DATFINSANDIPA"].ToString()))
                        str7 = str7 + " DATFINSAN = '" + DBMethods.Db2Date(dataTable8.Rows[index5]["DATFINSANDIPA"].ToString()) + "', ";
                    if (!string.IsNullOrEmpty(dataTable8.Rows[index5]["TASSANDIPA"].ToString()))
                        str7 = str7 + " TASSAN = '" + dataTable8.Rows[index5]["TASSANDIPA"].ToString().Replace(",", ".") + "', ";
                    string str8 = str7 + " WHERE CODPOS = '" + CODPOS + "' " + " AND MAT = '" + MAT + "' AND PRORAP = '" + PRORAP + "' AND PROMOD = '" + PROMOD + "' " + " AND ANNDEN = '" + dataTable8.Rows[index5]["ANNDEN"].ToString() + "' AND MESDEN = '" + dataTable8.Rows[index5]["MESDEN"].ToString() + "' " + " AND PRODEN = '" + dataTable8.Rows[index5]["PRODEN"].ToString() + "' " + " AND PRODENDET = '" + dataTable8.Rows[index5]["PRODENDET"].ToString() + "' ";
                    flag2 = this.objDataAccess.WriteTransactionData(strSQL4, CommandType.Text);
                }
                if (!flag2)
                    break;
            }
            flag1 = this.objDataAccess.WriteTransactionData("DELETE FROM MODPREDET WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + MAT + "' " + " AND PRORAP = '" + PRORAP + "' AND PROMOD = '" + PROMOD + "' AND ANNDEN >=  '" + DateTime.Parse(DATCES).Year.ToString() + "' " + " AND DAL > '" + DBMethods.Db2Date(DATCES) + "' " + " AND IMPRET = 0 AND IMPOCC = 0 AND IMPFIG = 0 AND TIPMOV <> 'AR' AND " + " VALUE(ANNCOM, 0) = 0 ", CommandType.Text);
            bool blnCommit = this.objDataAccess.WriteTransactionData(" DELETE FROM MODPREDET WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + MAT + "' " + " AND PRORAP = '" + PRORAP + "' AND PROMOD = '" + PROMOD + "' AND ANNCOM > '" + DateTime.Parse(DATCES).Year.ToString() + "' " + " AND IMPRET = 0 AND IMPOCC = 0 AND IMPFIG = 0 " + "AND TIPMOV = 'AR'", CommandType.Text);
            this.objDataAccess.EndTransaction(blnCommit);
            return blnCommit;
        }

        private bool CalcoloFigurativa(
          string CODPOS,
          string MAT,
          string PRORAP,
          string PROMOD,
          string DATCES)
        {
            int num1 = 0;
            Decimal num2 = 0.0M;
            Decimal num3 = 0.0M;
            short num4 = 0;
            short num5 = 0;
            bool flag1 = true;
            string str1 = "01/01/'" + Convert.ToDateTime(DATCES).AddYears(-1).ToString().Substring(6, 4) + "'";
            string str2 = " SELECT * FROM SOSRAP A WHERE A.CODPOS = '" + CODPOS + "' AND A.MAT = '" + MAT + "' AND A.PRORAP = '" + PRORAP + "' " + " AND A.DATINISOS <= '" + DATCES.Substring(0, 10) + "' AND A.DATFINSOS >= '" + DATCES + "' " + " and a.stasos = 0 and  A.DATINISOS not in (";
            string strSQL1 = " select DATINISOS " + " from sosrap " + " WHERE CODPOS = a.Codpos And MAT = a.mat And PRORAP = a.prorap " + " AND DATINISOS <= '" + DATCES + "' " + " AND DATFINSOS >= '" + DATCES + "' " + " AND PERFIG > 0 and stasos = 0 " + " AND codsos = 4 " + " ) " + " AND PERFIG > 0 ORDER BY DATINISOS ";
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL1);
            int num6 = dataTable2.Rows.Count - 1;
            for (int index1 = 0; index1 <= num6; ++index1)
            {
                DateTime DataDal = DateTime.Parse(Convert.ToString(dataTable2.Rows[index1]["DATINISOS"]));
                DateTime DataAl = DateTime.Parse(Convert.ToString(dataTable2.Rows[index1]["DATFINSOS"]));
                string[] strArray1 = new string[5]
                {
          "SELECT * FROM MODPREDET WHERE CODPOS = " + CODPOS + " AND MAT = " + MAT + " AND PRORAP = " + PRORAP + " AND PROMOD = " + PROMOD,
          " AND ANNDEN >= ",
          null,
          null,
          null
                };
                int num7 = DataDal.Year;
                strArray1[2] = num7.ToString();
                strArray1[3] = " AND ANNDEN <= ";
                num7 = DataAl.Year;
                strArray1[4] = num7.ToString();
                string[] strArray2 = new string[5]
                {
          string.Concat(strArray1),
          " AND MESDEN >= ",
          null,
          null,
          null
                };
                num7 = DataDal.Month;
                strArray2[2] = num7.ToString();
                strArray2[3] = " AND MESDEN <= ";
                num7 = DataAl.Month;
                strArray2[4] = num7.ToString();
                string strSQL2 = string.Concat(strArray2) + " AND TIPMOV IN ('NU', 'DP') AND NUMMOV IS NULL ";
                DataTable dataTable3 = new DataTable();
                DataTable dataTable4 = this.objDataAccess.GetDataTable(strSQL2);
                int num8 = dataTable4.Rows.Count - 1;
                for (int index2 = 0; index2 <= num8; ++index2)
                {
                    DateTime dateTime = DateTime.Parse(Convert.ToString(dataTable4.Rows[index2]["DAL"]));
                    int year = dateTime.Year;
                    dateTime = DateTime.Parse(Convert.ToString(dataTable4.Rows[index2]["AL"]));
                    int month = dateTime.Month;
                    short num9 = (short)DateTime.DaysInMonth(year, month);
                    dataTable4.Rows[index2]["NUMGGPER"] = (object)this.Get_NumGG_Periodo(Convert.ToDateTime(dataTable4.Rows[index2]["DAL"]), Convert.ToDateTime(dataTable4.Rows[index2]["AL"]));
                    dataTable4.Rows[index2]["NUMGGDOM"] = (object)this.Get_NumGG_Domeniche(Convert.ToDateTime(dataTable4.Rows[index2]["DAL"]), Convert.ToDateTime(dataTable4.Rows[index2]["AL"]));
                    dataTable4.Rows[index2]["NUMGGAZI"] = (object)0.0;
                    dataTable4.Rows[index2]["NUMGGSOS"] = (object)0.0;
                    dataTable4.Rows[index2]["NUMGGFIG"] = (object)0.0;
                    Decimal num10 = Convert.ToDecimal(dataTable2.Rows[index1]["PERAZI"].ToString().Replace(".", ","));
                    Decimal num11 = Convert.ToDecimal(dataTable2.Rows[index1]["PERFIG"].ToString().Replace(".", ","));
                    TimeSpan timeSpan;
                    if (DataAl >= Convert.ToDateTime(dataTable4.Rows[index2]["DAL"]) && DataDal <= Convert.ToDateTime(dataTable4.Rows[index2]["AL"]))
                    {
                        if (Convert.ToDateTime(dataTable4.Rows[index2]["DAL"]) >= DataDal)
                        {
                            if (Convert.ToDateTime(dataTable4.Rows[index2]["AL"]) <= DataAl)
                            {
                                int numGgDomeniche = this.Get_NumGG_Domeniche(Convert.ToDateTime(dataTable4.Rows[index2]["DAL"]), Convert.ToDateTime(dataTable4.Rows[index2]["AL"]));
                                dateTime = Convert.ToDateTime(dataTable4.Rows[index2]["AL"]);
                                timeSpan = dateTime.Subtract(Convert.ToDateTime(dataTable4.Rows[index2]["DAL"]));
                                num1 = Convert.ToInt32((object)((timeSpan.Days, 1), numGgDomeniche));
                                if (num10 > 0M)
                                    num2 = (Decimal)num1 * num10 / 100M;
                                if (num11 > 0M)
                                    num3 = (Decimal)num1 * num11 / 100M;
                            }
                            else
                            {
                                int numGgDomeniche = this.Get_NumGG_Domeniche(Convert.ToDateTime(dataTable4.Rows[index2]["DAL"]), DataAl);
                                dateTime = Convert.ToDateTime(dataTable4.Rows[index2]["AL"]);
                                timeSpan = dateTime.Subtract(Convert.ToDateTime(dataTable4.Rows[index2]["DAL"]));
                                num1 = Convert.ToInt32((object)((timeSpan.Days, 1), numGgDomeniche));
                                if (num10 > 0M)
                                    num2 = (Decimal)num1 * num10 / 100M;
                                if (num11 > 0M)
                                    num3 = (Decimal)num1 * num11 / 100M;
                            }
                        }
                        else if (Convert.ToDateTime(dataTable4.Rows[index2]["AL"]) <= DataAl)
                        {
                            int numGgDomeniche = this.Get_NumGG_Domeniche(DataDal, Convert.ToDateTime(dataTable4.Rows[index2]["AL"]));
                            int num12 = num1;
                            dateTime = Convert.ToDateTime(dataTable4.Rows[index2]["AL"]);
                            timeSpan = dateTime.Subtract(DataDal);
                            int num13 = timeSpan.Days + 1 - numGgDomeniche;
                            num1 = num12 + num13;
                            if (num10 > 0M)
                            {
                                Decimal num14 = num2;
                                dateTime = Convert.ToDateTime(dataTable4.Rows[index2]["AL"]);
                                timeSpan = dateTime.Subtract(DataDal);
                                Decimal num15 = (Decimal)(timeSpan.Days + 1) - (Decimal)numGgDomeniche * num10 / 100M;
                                num2 = num14 + num15;
                            }
                            if (num11 > 0M)
                            {
                                Decimal num16 = num3;
                                dateTime = Convert.ToDateTime(dataTable4.Rows[index2]["AL"]);
                                timeSpan = dateTime.Subtract(DataDal);
                                Decimal num17 = (Decimal)(timeSpan.Days + 1) - (Decimal)numGgDomeniche * num11 / 100M;
                                num3 = num16 + num17;
                            }
                        }
                        else
                        {
                            int numGgDomeniche = this.Get_NumGG_Domeniche(DataDal, DataAl);
                            int num18 = num1;
                            timeSpan = DataAl.Subtract(DataDal);
                            int num19 = timeSpan.Days + 1 - numGgDomeniche;
                            num1 = num18 + num19;
                            if (num10 > 0M)
                            {
                                Decimal num20 = num2;
                                timeSpan = DataAl.Subtract(DataDal);
                                Decimal num21 = (Decimal)(timeSpan.Days + 1) - (Decimal)numGgDomeniche * num10 / 100M;
                                num2 = num20 + num21;
                            }
                            if (num11 > 0M)
                            {
                                Decimal num22 = num3;
                                timeSpan = DataAl.Subtract(DataDal);
                                Decimal num23 = (Decimal)(timeSpan.Days + 1) - (Decimal)numGgDomeniche * num11 / 100M;
                                num3 = num22 + num23;
                            }
                        }
                    }
                    dataTable4.Rows[index2]["NUMGGSOS"] = (object)num1;
                    dataTable4.Rows[index2]["NUMGGFIG"] = (object)num3;
                    dataTable4.Rows[index2]["NUMGGAZI"] = (object)num2;
                    if (num9 > (short)0)
                    {
                        if (Convert.ToInt32(dataTable4.Rows[index2]["NUMGGSOS"]) > 0)
                            dataTable4.Rows[index2]["NUMGGCONAZI"] = (object)(Convert.ToInt32(dataTable4.Rows[index2]["NUMGGPER"]) - Convert.ToInt32(dataTable4.Rows[index2]["NUMGGDOM"]) - Convert.ToInt32(dataTable4.Rows[index2]["NUMGGSOS"]) + Convert.ToInt32(dataTable4.Rows[index2]["NUMGGAZI"]));
                        else if (Convert.ToDateTime(dataTable4.Rows[index2]["DATDEC"]) <= Convert.ToDateTime(dataTable4.Rows[index2]["DAL"]))
                        {
                            if (string.IsNullOrEmpty(dataTable4.Rows[index2][nameof(DATCES)].ToString().Trim()))
                            {
                                Decimal num24 = Convert.ToDecimal(dataTable4.Rows[index2]["NUMGGPER"]);
                                dataTable4.Rows[index2]["NUMGGCONAZI"] = (object)Convert.ToDecimal(num24 * 26M / (Decimal)num9);
                            }
                            else
                                dataTable4.Rows[index2]["NUMGGCONAZI"] = !(Convert.ToDateTime(dataTable4.Rows[index2][nameof(DATCES)]) > Convert.ToDateTime(dataTable4.Rows[index2]["AL"])) ? (object)(Decimal.Round((Decimal)num9 / Convert.ToDecimal(26), 0), dataTable4.Rows[index2]["NUMGGPER"]) : (object)(Convert.ToInt32(dataTable4.Rows[index2]["NUMGGPER"]) - Convert.ToInt32(dataTable4.Rows[index2]["NUMGGDOM"]) - Convert.ToInt32(dataTable4.Rows[index2]["NUMGGSOS"]) + Convert.ToInt32(dataTable4.Rows[index2]["NUMGGAZI"]));
                        }
                        else
                            dataTable4.Rows[index2]["NUMGGCONAZI"] = (object)(Decimal.Round((Decimal)num9 / Convert.ToDecimal(26), 0), dataTable4.Rows[index2]["NUMGGPER"]);
                    }
                    dataTable4.Rows[index2]["IMPFIG"] = !Convert.ToBoolean((object)(dataTable4.Rows[index2]["TIPSPE"], "S", false)) ? (object)(Convert.ToDecimal(dataTable4.Rows[index2]["IMPTRAECO"]) * Convert.ToDecimal(dataTable4.Rows[index2]["NUMGGFIG"]) / 26M) : (object)(Convert.ToDecimal(dataTable4.Rows[index2]["IMPMIN"]) + Convert.ToDecimal(dataTable4.Rows[index2]["IMPSCA"]) * Convert.ToDecimal(dataTable4.Rows[index2]["NUMGGFIG"]) / 26M);
                    dataTable4.Rows[index2]["IMPFIG"] = (object)Math.Round(Convert.ToDecimal(dataTable4.Rows[index2]["IMPFIG"]));
                    dataTable4.Rows[index2]["IMPFAP"] = (object)0.0;
                    if ((int)num4 != DataDal.Year | (int)num5 != DataDal.Month)
                    {
                        bool flag2 = this.objDataAccess.WriteTransactionData("UPDATE MODPREDET SET IMPFIG = 0, NUMGGSOS = 0, NUMGGFIG = 0, NUMGGAZI = 0, NUMGGCONAZI = 0 " + " WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + dataTable4.Rows[index2][nameof(MAT)].ToString() + "' AND " + " PRORAP = '" + dataTable4.Rows[index2][nameof(PRORAP)].ToString() + "' AND PROMOD = '" + dataTable4.Rows[index2][nameof(PROMOD)].ToString() + "' AND " + " PROMODDET = " + dataTable4.Rows[index2]["PROMODDET"].ToString() + "' ", CommandType.Text);
                        flag2 = this.objDataAccess.WriteTransactionData("UPDATE DENDET SET IMPFIG = 0, NUMGGSOS = 0, NUMGGFIG = 0, NUMGGAZI = 0, NUMGGCONAZI = 0 " + " WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + dataTable4.Rows[index2][nameof(MAT)].ToString() + "' AND " + " PRORAP = '" + dataTable4.Rows[index2][nameof(PRORAP)].ToString() + "' AND ANNDEN = '" + dataTable4.Rows[index2]["ANNDEN"].ToString() + "' " + " AND MESDEN = '" + dataTable4.Rows[index2]["MESDEN"].ToString() + "' AND PRODEN = '" + dataTable4.Rows[index2]["PRODEN"].ToString() + "' " + " AND PRODENDET = '" + dataTable4.Rows[index2]["PRODENDET"].ToString() + "' ", CommandType.Text);
                        num4 = (short)DataDal.Year;
                        num5 = (short)DataDal.Month;
                    }
                    flag1 = this.objDataAccess.WriteTransactionData("UPDATE MODPREDET SET IMPFIG = " + dataTable4.Rows[index2]["IMPFIG"].ToString() + ", NUMGGSOS = '" + dataTable4.Rows[index2]["NUMGGSOS"].ToString() + "', " + " NUMGGFIG = '" + dataTable4.Rows[index2]["NUMGGFIG"].ToString().Replace(",", ".") + "', NUMGGPER ='" + dataTable4.Rows[index2]["NUMGGPER"].ToString() + "', " + " NUMGGAZI = '" + dataTable4.Rows[index2]["NUMGGAZI"].ToString() + "', NUMGGDOM = '" + dataTable4.Rows[index2]["NUMGGDOM"]?.ToString() + "', NUMGGCONAZI = '" + dataTable4.Rows[index2]["NUMGGCONAZI"].ToString() + "' " + " WHERE CODPOS = '" + CODPOS + "' AND MAT = '" + dataTable4.Rows[index2][nameof(MAT)].ToString() + "' AND PRORAP = '" + dataTable4.Rows[index2][nameof(PRORAP)].ToString() + "' " + " AND PROMOD = '" + dataTable4.Rows[index2][nameof(PROMOD)].ToString() + "' AND PROMODDET = '" + dataTable4.Rows[index2]["PROMODDET"].ToString() + "' ", CommandType.Text);
                    if (flag1 && !string.IsNullOrEmpty(dataTable4.Rows[index2]["PRODEN"].ToString()))
                    {
                        strSQL1 = "UPDATE DENDET SET IMPFIG = '" + dataTable4.Rows[index2]["IMPFIG"].ToString() + "', NUMGGSOS = '" + dataTable4.Rows[index2]["NUMGGSOS"].ToString() + "', " + " NUMGGFIG = '" + dataTable4.Rows[index2]["NUMGGFIG"].ToString().Replace(",", ".") + "', NUMGGPER = '" + dataTable4.Rows[index2]["NUMGGPER"].ToString() + "', " + " NUMGGAZI = '" + dataTable4.Rows[index2]["NUMGGAZI"].ToString() + "', NUMGGDOM = '" + dataTable4.Rows[index2]["NUMGGDOM"].ToString() + "', " + " NUMGGCONAZI = '" + dataTable4.Rows[index2]["NUMGGCONAZI"].ToString() + "' WHERE CODPOS = '" + CODPOS + "' " + " AND MAT = '" + dataTable4.Rows[index2][nameof(MAT)].ToString() + "' AND PRORAP = '" + dataTable4.Rows[index2][nameof(PRORAP)].ToString() + "' " + " AND ANNDEN = '" + dataTable4.Rows[index2]["ANNDEN"].ToString() + "' AND MESDEN = '" + dataTable4.Rows[index2]["MESDEN"].ToString() + "' " + " AND PRODEN = '" + dataTable4.Rows[index2]["PRODEN"].ToString() + "' AND PRODENDET = '" + dataTable4.Rows[index2]["PRODENDET"].ToString() + "' ";
                        flag1 = this.objDataAccess.WriteTransactionData(strSQL1, CommandType.Text);
                    }
                    if (flag1)
                    {
                        num1 = 0;
                        num3 = 0M;
                        num2 = 0M;
                    }
                    else
                        break;
                }
            }
            return flag1;
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

        public bool WRITE_INSERT_DIPAPREV(
          string CODPOS,
          string MAT,
          string PRORAP,
          string PROMOD,
          string PROMODDET,
          string ANNDEN,
          string MESDEN,
          string PRODEN,
          string PRODENDET)
        {
            TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente)HttpContext.Current.Session["utente"];
            bool flag = this.objDataAccess.WriteTransactionData("INSERT INTO MODPREDET SELECT CODPOS, MAT, '" + PRORAP + "', '" + PROMOD + "', " + " '" + PROMODDET + "', ANNDEN, MESDEN, TIPMOV, DAL, AL, DATERO, IMPRET, IMPOCC, IMPFIG, IMPABB, IMPASSCON, " + " IMPCON, IMPMIN, DATDEC, DATCES, NUMGGAZI, NUMGGFIG, NUMGGPER, NUMGGDOM, NUMGGSOS, NUMGGCONAZI, " + " IMPSCA, IMPTRAECO, ETA65, TIPRAP, FAP, PERFAP, IMPFAP, PERPAR, PERAPP, CODCON, PROCON, TIPSPE, " + " CODLOC, PROLOC, CODLIV, CODGRUASS, CODQUACON, ALIQUOTA, DATNAS, DATINPS, DATSAP, ANNCOM, NOTE, " + " IMPSANDET, TIPDEN, NUMMOV, DATCONMOV, NUMMOVANN, DATMOVANN, NUMGRURET, NUMGRURETRIF, ESIRET, " + " IMPRETDEL, IMPOCCDEL, IMPFIGDEL, IMPABBDEL, IMPASSCONDEL, PRIORITA, PRORETTES, ANNRET, PRORET, " + " IMPCONDEL, DATINISAN, DATFINSAN, IMPRETPRE, IMPOCCPRE, IMPFIGPRE, IMPSANDETPRE, IMPCONPRE, " + " CODCAUSAN, TASSAN, IMPABBPRE, IMPASSCONPRE, NUMSANANN, DATSANANN, CODSANANN, NUMRETTES, " + " NUMRETTESRIF, BILMOVANN, BILSANANN, NUMSAN, TIPANNMOVARR, TIPANNMOVARRANN, " + " 0, 0, 0, 0, 0, 0, current_timestamp, '" + DBMethods.DoublePeakForSql(utente.CodPosizione) + "'" + " FROM DENDET WHERE CODPOS ='" + HttpContext.Current.Session["Posizione"]?.ToString() + "'  AND ANNDEN = '" + ANNDEN + "' AND MESDEN = '" + MESDEN + "' " + " AND PRODEN = '" + PRODEN + "' AND PRODENDET = '" + PRODENDET + "' AND MAT = '" + MAT + "' ", CommandType.Text);
            if (flag)
                flag = this.objDataAccess.WriteTransactionData("IMPASSCONPRV = NULL, IMPCONPRV = NULL WHERE CODPOS = '" + HttpContext.Current.Session["Posizione"]?.ToString() + "' AND MAT = '" + MAT + "' " + " AND PRORAP = " + PRORAP + " AND PROMOD = " + PROMOD + " AND PROMODDET = '" + PROMODDET.ToString() + "' ", CommandType.Text);
            return flag;
        }

        public bool WRITE_INSERT_PARPREV(string CODPOS, string MAT, string PRORAP, string PROMOD)
        {
            TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente)HttpContext.Current.Session["utente"];
            Decimal num1 = 0M;
            string s = (string)null;
            bool flag1 = false;
            bool flag2 = false;
            string strSQL = "SELECT CODPOS, MAT, PRORAP, DATINI, DATFIN, TIPRAP, PERAPP, PERPAR FROM STORDL " + " WHERE CODPOS = '" + CODPOS + "' And MAT = '" + MAT + "' AND PRORAP = '" + PRORAP + "' " + " AND VALUE(PERPAR, 0) > 0  ORDER BY DATINI ";
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
            short index1 = 0;
            if (dataTable2.Rows.Count > 0)
            {
                while (!flag1)
                {
                    if (Convert.ToDecimal(dataTable2.Rows[(int)index1]["PERPAR"]) != num1)
                    {
                        dataTable2.Rows[(int)index1]["DATINI"].ToString();
                        s = dataTable2.Rows[(int)index1]["DATFIN"].ToString();
                        num1 = Convert.ToDecimal(dataTable2.Rows[(int)index1]["PERPAR"]);
                        ++index1;
                    }
                    else if (DateTime.Compare(DateTime.Parse(s).AddDays(1.0), DateTime.Parse(dataTable2.Rows[(int)index1]["DATINI"].ToString())) == 0)
                    {
                        s = dataTable2.Rows[(int)index1]["DATFIN"].ToString();
                        dataTable2.Rows[(int)index1 - 1]["DATFIN"] = (object)s;
                        dataTable2.Rows.RemoveAt((int)index1);
                    }
                    else
                        ++index1;
                    if ((int)index1 == dataTable2.Rows.Count)
                        break;
                }
                short num2 = (short)(dataTable2.Rows.Count - 1);
                for (short index2 = 0; (int)index2 <= (int)num2; ++index2)
                {
                    string str1 = "INSERT INTO MODPREPAR (CODPOS, MAT, PRORAP, PROMOD, DATINI, DATFIN, TIPRAP, PERAPP, PERPAR, " + " ULTAGG, UTEAGG) VALUES ('" + CODPOS + "', '" + MAT + "', '" + PRORAP + "', '" + PROMOD + "', '" + DBMethods.Db2Date(dataTable2.Rows[(int)index2]["DATINI"].ToString()) + "', " + " '" + DBMethods.Db2Date(dataTable2.Rows[(int)index2]["DATFIN"].ToString()) + "', ";
                    string str2 = !(dataTable2.Rows[(int)index2]["TIPRAP"].ToString() != "") ? str1 + " Null, " : str1 + " '" + dataTable2.Rows[(int)index2]["TIPRAP"]?.ToString() + "', ";
                    string str3 = !(dataTable2.Rows[(int)index2]["PERAPP"].ToString() != "") ? str2 + " Null, " : str2 + " '" + dataTable2.Rows[(int)index2]["PERAPP"]?.ToString() + "', ";
                    flag2 = this.objDataAccess.WriteTransactionData((!(dataTable2.Rows[(int)index2]["PERPAR"].ToString() != "") ? str3 + " Null, " : str3 + " '" + dataTable2.Rows[(int)index2]["PERPAR"]?.ToString() + "', ") + " CURRENT_TIMESTAMP, '" + utente.CodPosizione + "')", CommandType.Text);
                    if (!flag2)
                        break;
                }
            }
            else
                flag2 = true;
            dataTable2.Dispose();
            return flag2;
        }

        public bool WRITE_INSERT_MODPRE(
          string CODPOS,
          string MAT,
          string PRORAP,
          string CODSTAPRE,
          string DATAPE,
          string UTEAPE,
          string DATCHI,
          string UTECHI,
          string PREVUFF,
          string PREAVVISO,
          string DATINIPRE,
          string DATSCAPRE,
          string GIOINDSOS,
          Decimal IMPINDSOS,
          string RIFNOME,
          string RIFTEL,
          string NOTE,
          string UTEASS,
          string PRESTITO,
          string CARTAENP)
        {
            string Err = "";
            TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente)HttpContext.Current.Session["utente"];
            try
            {
                string strSQL = " SELECT VALUE(MAX(PROMOD), 0) + 1  FROM MODPRE WHERE CODPOS = '" + CODPOS + "' " + " AND MAT = '" + MAT + "' " + " AND PRORAP = '" + PRORAP + "' ";
                DataSet dataSet = new DataSet();
                string str1 = this.objDataAccess.GetDataSet(strSQL, ref Err).Tables[0].Rows[0]["PROMOD"].ToString();
                string str2 = " INSERT INTO MODPRE (" + " CODPOS , " + " MAT , " + " PRORAP , " + " PROMOD , " + " CODSTAPRE , " + " DATAPE , " + " UTEAPE , " + " DATCHI , " + " UTECHI , " + " PREVUFF , " + " PREAVVISO , " + " DATINIPRE , " + " DATSCAPRE , " + " GIOINDSOS , " + " IMPINDSOS , " + " RIFNOME , " + " RIFTEL , " + " NOTE , " + " UTEASS , " + " PRESTITO , " + " CARTAENP , " + " ULTAGG, " + " UTEAGG) " + " VALUES (" + " '" + CODPOS + "', " + " '" + MAT + "', " + " '" + PRORAP + "', " + " '" + str1 + "', " + " '" + CODSTAPRE + "', ";
                string str3 = !string.IsNullOrEmpty(DATAPE) ? str2 + " '" + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATAPE)) + "', " : str2 + " NULL, ";
                string str4 = !string.IsNullOrEmpty(UTEAPE) ? str3 + " '" + DBMethods.DoublePeakForSql(UTEAPE) + "', " : str3 + " NULL, ";
                string str5 = !string.IsNullOrEmpty(DATCHI) ? str4 + " '" + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCHI)) + "', " : str4 + " NULL, ";
                string str6 = (!string.IsNullOrEmpty(UTECHI) ? str5 + " '" + DBMethods.DoublePeakForSql(UTECHI) + "', " : str5 + " NULL, ") + " '" + DBMethods.DoublePeakForSql(PREVUFF) + "', " + " '" + DBMethods.DoublePeakForSql(PREAVVISO) + "', ";
                string str7 = !string.IsNullOrEmpty(DATINIPRE) ? str6 + " '" + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATINIPRE)) + "', " : str6 + " NULL, ";
                string str8 = !string.IsNullOrEmpty(DATSCAPRE) ? str7 + " '" + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATSCAPRE)) + "', " : str7 + " NULL, ";
                string str9 = !(GIOINDSOS == "0") ? str8 + " '" + GIOINDSOS + ", " : str8 + " NULL, ";
                string str10 = (double)IMPINDSOS != 0.0 ? str9 + " '" + IMPINDSOS.ToString().Replace(",", ".") + "', " : str9 + " NULL, ";
                string str11 = !string.IsNullOrEmpty(RIFNOME) ? str10 + " '" + DBMethods.DoublePeakForSql(RIFNOME) + "', " : str10 + " NULL, ";
                string str12 = !string.IsNullOrEmpty(RIFTEL) ? str11 + " '" + DBMethods.DoublePeakForSql(RIFTEL) + "', " : str11 + " NULL, ";
                string str13 = !string.IsNullOrEmpty(NOTE) ? str12 + "'" + DBMethods.DoublePeakForSql(NOTE) + "', " : str12 + " NULL, ";
                string str14 = !string.IsNullOrEmpty(UTEASS) ? str13 + "'" + DBMethods.DoublePeakForSql(UTEASS) + "', " : str13 + " NULL, ";
                string str15 = !string.IsNullOrEmpty(PRESTITO) ? str14 + "'" + DBMethods.DoublePeakForSql(DBMethods.Db2Date(PRESTITO)) + "', " : str14 + " NULL, ";
                return this.objDataAccess.WriteTransactionData((!string.IsNullOrEmpty(CARTAENP) ? str15 + "'" + DBMethods.DoublePeakForSql(DBMethods.Db2Date(CARTAENP)) + "', " : str15 + "NULL, ") + " CURRENT_TIMESTAMP, " + "'" + DBMethods.DoublePeakForSql(utente.CodPosizione) + "') ", CommandType.Text);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool WRITE_INSERT_MODPREDET(
          string u,
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
            try
            {
                num = Convert.ToDecimal(this.objDataAccess.Get1ValueFromSQL("SELECT VALORE FROM PARGENDET WHERE CODPAR = 5" + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DAL)) + " BETWEEN DATINI AND DATFIN", CommandType.Text));
                int int32 = Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL(" SELECT VALUE(MAX(PROMODDET), 0) + 1  FROM MODPREDET " + " WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND PROMOD = " + PROMOD.ToString(), CommandType.Text));
                string str1 = "INSERT INTO MODPREDET (" + "CODPOS, " + "MAT, " + "PRORAP, " + "PROMOD, " + "PROMODDET, " + "ANNDEN, " + "MESDEN, " + "TIPMOV, " + "DAL, " + "AL, " + "DATERO, " + "IMPRET, " + "IMPOCC, " + "IMPFIG, " + "IMPABB, " + "IMPASSCON, " + "IMPCON, " + "IMPRETPRV, " + "IMPOCCPRV, " + "IMPFIGPRV, " + "IMPABBPRV, " + "IMPASSCONPRV, " + "IMPCONPRV, " + "IMPMIN, " + "DATDEC, " + "DATCES, " + "NUMGGAZI, " + "NUMGGFIG, " + "NUMGGPER, " + "NUMGGDOM, " + "NUMGGSOS, " + "NUMGGCONAZI, " + "IMPSCA, " + "IMPTRAECO, " + "ETA65 ," + "TIPRAP, " + "FAP, " + "PERFAP ," + "IMPFAP ," + "PERPAR, " + "PERAPP, " + "CODCON, " + "PROCON, " + "TIPSPE, " + "CODLOC, " + "PROLOC, " + "CODLIV, " + "CODGRUASS, " + "CODQUACON, " + "ALIQUOTA, " + "DATNAS, " + "ANNCOM, ";
                if (PRODEN != 0)
                    str1 = str1 + "PRODEN, " + "PRODENDET, ";
                string str2 = str1 + "ULTAGG, " + "UTEAGG) " + " VALUES (" + CODPOS.ToString() + ", " + MAT.ToString() + ", " + PRORAP.ToString() + ", " + PROMOD.ToString() + ", " + int32.ToString() + ", " + ANNDEN.ToString() + ", " + MESDEN.ToString() + ", " + DBMethods.DoublePeakForSql(TIPMOV) + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DAL)) + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(AL)) + ", ";
                string str3 = (string.IsNullOrEmpty(DATERO) ? str2 + "Null, " : str2 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATERO)) + ", ") + IMPRET.ToString().Replace(",", ".") + ", " + IMPOCC.ToString().Replace(",", ".") + ", " + IMPFIG.ToString().Replace(",", ".") + ", " + IMPABB.ToString().Replace(",", ".") + ", " + IMPASSCON.ToString().Replace(",", ".") + ", " + "ROUND(" + IMPRET.ToString().Replace(",", ".") + " * " + ALIQUOTA.ToString().Replace(",", ".") + " / 100, 2), ";
                string str4 = !string.IsNullOrEmpty(IMPRETPRV.Trim()) ? str3 + IMPRETPRV.Replace(",", ".") + ", " : str3 + "Null, ";
                string str5 = !string.IsNullOrEmpty(IMPOCCPRV.Trim()) ? str4 + IMPOCCPRV.Replace(",", ".") + ", " : str4 + "Null, ";
                string str6 = !string.IsNullOrEmpty(IMPFIGPRV.Trim()) ? str5 + IMPFIGPRV.ToString().Replace(",", ".") + ", " : str5 + "Null, ";
                string str7 = !string.IsNullOrEmpty(IMPABBPRV.Trim()) ? str6 + IMPABB.ToString().Replace(",", ".") + ", " : str6 + "Null, ";
                string str8;
                if (string.IsNullOrEmpty(IMPRETPRV.Trim()))
                    str8 = str7 + "Null, " + "Null, ";
                else
                    str8 = (!string.IsNullOrEmpty(IMPASSCONPRV.Trim()) ? str7 + IMPASSCONPRV.ToString().Replace(",", ".") + ", " : str7 + "Null, ") + "ROUND(" + IMPRETPRV.ToString().Replace(",", ".") + " * " + ALIQUOTA.ToString().Replace(",", ".") + " / 100, 2), ";
                string str9 = str8 + IMPMIN.ToString().Replace(",", ".") + ", ";
                string str10 = string.IsNullOrEmpty(DATDEC) ? str9 + "NULL, " : str9 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATDEC)) + ", ";
                string str11 = (string.IsNullOrEmpty(DATCES) ? str10 + "NULL, " : str10 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCES)) + ", ") + NUMGGAZI.ToString().Replace(",", ".") + ", " + NUMGGFIG.ToString().Replace(",", ".") + ", " + NUMGGPER.ToString().Replace(",", ".") + ", " + NUMGGDOM.ToString().Replace(",", ".") + ", " + NUMGGSOS.ToString().Replace(",", ".") + ", " + NUMGGCONAZI.ToString().Replace(",", ".") + ", " + IMPSCA.ToString().Replace(",", ".") + ", " + IMPTRAECO.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(ETA65) + ", ";
                string str12 = (TIPRAP == 0 ? str11 + "NULL, " : str11 + TIPRAP.ToString() + ", ") + DBMethods.DoublePeakForSql(FAP) + ", ";
                if (FAP == "S")
                    IMPFAP = !string.IsNullOrEmpty(IMPRETPRV.Trim()) ? (Decimal)(Convert.ToDouble(IMPRETPRV) / 100.0 * (double)PERFAP) : (Decimal)(0.0 * (double)PERFAP);
                string str13 = str12 + PERFAP.ToString().Replace(",", ".") + ", " + IMPFAP.ToString().Replace(",", ".") + ", " + PERPAR.ToString().Replace(",", ".") + ", " + PERAPP.ToString().Replace(",", ".") + ", " + CODCON.ToString() + ", " + PROCON.ToString() + ", " + DBMethods.DoublePeakForSql(TIPSPE) + ", " + CODLOC.ToString() + ", " + PROLOC.ToString() + ", " + CODLIV.ToString() + ", " + CODGRUASS.ToString() + ", " + CODQUACON.ToString() + ", " + ALIQUOTA.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATNAS)) + ", ";
                string str14 = ANNCOM == 0 ? str13 + "NULL, " : str13 + ANNCOM.ToString() + ", ";
                if (PRODEN != 0)
                    str14 = str14 + PRODEN.ToString() + ", " + PRODENDET.ToString() + ", ";
                return this.objDataAccess.WriteTransactionData(str14 + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u) + ")", CommandType.Text);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
