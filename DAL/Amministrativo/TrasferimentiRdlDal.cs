// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.TrasferimentiRdlDal
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Amministrativo
{
  public class TrasferimentiRdlDal
  {
    private DataLayer objDataAccess = new DataLayer();
    private IDOC_RDL iDOC_RDL = new IDOC_RDL();

    public void Salva_EseguiTrasferimento(
      TrasferimentiRdlOCM trasferimentiRdlOCM,
      TFI.OCM.Utente.Utente u,
      string CodPos,
      string DatDec,
      ref string MsgErrore,
      ref string MsgSuccess)
    {
      string[] strArray1 = new string[5]
      {
        "RAPLAV",
        "STORDL",
        "IMPAGG",
        "PARTIMM",
        "SOSRAP"
      };
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      DataTable dataTable4 = new DataTable();
      string DATINI = (string) null;
      DataTable DT = new DataTable();
      int num1 = 0;
      bool flag1 = false;
      string str1 = " ";
      try
      {
        this.objDataAccess.StartTransaction();
        bool flag2;
        for (int index1 = 0; index1 <= trasferimentiRdlOCM.ListaAziendes.Count - 1; ++index1)
        {
          for (int index2 = 0; index2 <= trasferimentiRdlOCM.ListaIscrittis.Count - 1; ++index2)
          {
            if (!this.VerificaStordl(trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString(), trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString(), CodPos, DatDec, ref MsgErrore))
              return;
            DT.Clear();
            DataTable idocDatiE1Pitype1 = this.iDOC_RDL.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0000", 0, Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString()), 0, 0, "", "", "", "", "", "", 0, 0, 0, "", "TRASFERIMENTO", "", "");
            this.iDOC_RDL.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype1.Rows[0]);
            string strSQL1 = "SELECT PROSOS FROM SOSRAP " + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString() + " AND VALUE(STASOS, 0) = 0";
            dataTable2.Clear();
            DataTable dataTable5 = this.objDataAccess.GetDataTable(strSQL1);
            int num2 = dataTable5.Rows.Count - 1;
            for (int index3 = 0; index3 <= num2; ++index3)
            {
              idocDatiE1Pitype1.Clear();
              idocDatiE1Pitype1 = this.iDOC_RDL.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(CodPos), Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString()), Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString()), Convert.ToInt32(dataTable5.Rows[index3]["PROSOS"]), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
              this.iDOC_RDL.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype1, true);
            }
            string str2 = "SELECT COUNT(MAT) AS NUMRAP FROM RAPLAV WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString();
            DateTime dateTime = Convert.ToDateTime(DatDec.Trim());
            string str3 = DBMethods.Db2Date(dateTime.ToString());
            DataTable dataTable6 = this.objDataAccess.GetDataTable(str2 + " AND '" + str3 + "' BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31')");
            if (dataTable6.Rows.Count > 0 && Convert.ToInt32(dataTable6.Rows[0]["NUMRAP"]) == 0)
            {
              MsgErrore = "POSIZIONE:" + CodPos + " E MATRICOLA:" + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + "  NON VALIDE";
              this.objDataAccess.EndTransaction(false);
              return;
            }
            string str4 = this.objDataAccess.Get1ValueFromSQL("SELECT DATDEC FROM RAPLAV WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString(), CommandType.Text);
            if (Convert.ToDateTime(DatDec) <= Convert.ToDateTime(str4))
            {
              MsgErrore = "La data di decorrenza del trasferimento per la matricola " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " deve essere maggiore della data d'iscrizione";
              this.objDataAccess.EndTransaction(false);
              return;
            }
            string str5 = "SELECT VALUE(COUNT(*), 0) AS TOT FROM TRARAP WHERE MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND CODPOS = " + CodPos;
            dateTime = Convert.ToDateTime(trasferimentiRdlOCM.ListaAziendes[index1].datden.Trim());
            string str6 = DBMethods.Db2Date(dateTime.ToString());
            if (Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL(str5 + " AND DATTRA >= '" + str6 + "' " + " AND STAELA <> 3 " + " AND DATANN IS NULL", CommandType.Text)) > 0)
            {
              ref string local = ref MsgErrore;
              string str7 = trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString();
              dateTime = Convert.ToDateTime(trasferimentiRdlOCM.ListaAziendes[index1].datden.Trim());
              string str8 = DBMethods.Db2Date(dateTime.ToString());
              string str9 = "Per la matricola " + str7 + " esiste già un trasferimento con data successiva al " + str8;
              local = str9;
              this.objDataAccess.EndTransaction(false);
              return;
            }
            if (!this.VerificaAzioniLegali(CodPos, DatDec, ref MsgErrore))
            {
              this.objDataAccess.EndTransaction(false);
              return;
            }
            int int32_1 = Convert.ToInt32(CodPos);
            int int32_2 = Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString());
            int int32_3 = Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString());
            string str10 = "SELECT PRORAP FROM RAPLAV WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString();
            dateTime = Convert.ToDateTime(DatDec.Trim());
            string str11 = DBMethods.Db2Date(dateTime.ToString());
            DataTable dataTable7 = this.objDataAccess.GetDataTable(str10 + " AND '" + str11 + "' BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31')");
            int PRORAP = dataTable7.Rows.Count <= 0 ? Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL(" SELECT VALUE(MAX(PRORAP),0) + 1 AS NEW_PRORAP FROM RAPLAV WHERE " + " MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString(), CommandType.Text)) : Convert.ToInt32(dataTable7.Rows[0]["PRORAP"]);
            int int32_4 = Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL(" SELECT VALUE(MAX(PROTRARAP),0) + 1 FROM  TRARAP " + " WHERE CODPOS = " + int32_1.ToString() + " AND MAT = " + int32_2.ToString() + " AND PRORAP = " + int32_3.ToString(), CommandType.Text));
            if (num1 == 0)
              num1 = int32_4;
            string str12 = "INSERT INTO TRARAP ( CODPOS , MAT , PRORAP, " + " PROTRARAP , GRUPPO, DATTRA , CODPOSTRA , MATTRA ," + " PRORAPTRA , PERTRA , DATANN , STAELA , " + " ULTAGG , UTEAGG ) VALUES ( " + CodPos + ", " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + ", " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString() + ", " + int32_4.ToString() + ", " + num1.ToString() + ", '";
            dateTime = Convert.ToDateTime(trasferimentiRdlOCM.ListaAziendes[index1].datden.Trim());
            string str13 = DBMethods.Db2Date(dateTime.ToString());
            string str14 = str12 + str13 + "', " + trasferimentiRdlOCM.ListaAziendes[index1].codpos.ToString() + ", " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + ", " + PRORAP.ToString() + ", ";
            this.objDataAccess.WriteTransactionData((string.IsNullOrEmpty(trasferimentiRdlOCM.ListaAziendes[index1].Percen.ToString()) ? str14 + " Null, " : str14 + trasferimentiRdlOCM.ListaAziendes[index1].Percen.ToString().Replace(",", ".") + ", ") + " Null, " + " 3, " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")", CommandType.Text);
            int num3 = strArray1.Length - 1;
            for (int index4 = 0; index4 <= num3; ++index4)
            {
              dataTable1 = this.objDataAccess.GetDataTable("SELECT * FROM " + strArray1[index4] + " WHERE CODPOS = " + int32_1.ToString() + " AND MAT = " + int32_2.ToString() + " AND PRORAP = " + int32_3.ToString());
              string str15 = "INSERT INTO BK" + strArray1[index4] + " ( " + "PROTRARAP ";
              int num4 = dataTable1.Columns.Count - 1;
              for (int index5 = 0; index5 <= num4; ++index5)
                str15 = str15 + ", " + dataTable1.Columns[index5].ColumnName;
              string str16 = str15 + " ) " + " SELECT " + int32_4.ToString();
              int num5 = dataTable1.Columns.Count - 1;
              for (int index6 = 0; index6 <= num5; ++index6)
                str16 = str16 + ", " + dataTable1.Columns[index6].ColumnName;
              flag2 = this.objDataAccess.WriteTransactionData(str16 + " FROM " + strArray1[index4] + " WHERE CODPOS = " + int32_1.ToString() + " AND MAT = " + int32_2.ToString() + " AND PRORAP = " + int32_3.ToString(), CommandType.Text);
            }
            dataTable7.Clear();
            string str17 = "SELECT * FROM RAPLAV WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString();
            dateTime = Convert.ToDateTime(DatDec.Trim());
            string str18 = DBMethods.Db2Date(dateTime.ToString());
            DataTable dataTable8 = this.objDataAccess.GetDataTable(str17 + " AND '" + str18 + "' BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31')");
            if (dataTable8.Rows.Count > 0)
            {
              flag2 = this.objDataAccess.WriteTransactionData("UPDATE RAPLAV SET " + " CODPOSDA = " + CodPos + ", " + " ESIPRITRA = 'S' " + " WHERE CODPOS = " + dataTable8.Rows[0]["CODPOS"]?.ToString() + " AND MAT = " + dataTable8.Rows[0]["MAT"]?.ToString() + " AND PRORAP = " + dataTable8.Rows[0]["PRORAP"]?.ToString(), CommandType.Text);
            }
            else
            {
              string str19 = " INSERT INTO RAPLAV" + " (CODPOS, MAT, PRORAP, DATDEC,  " + " DATCES, CODCAUCES, CODPOSDA, DATDENCES, " + " DATASS, DATDEN, DATPRE, DATPRO, NUMPRO, MATFON, PROFON, " + " CODPOSFOR, RAGSOCFOR, PARIVAFOR, CODFISFOR, INDFOR, TELFOR, CODDUGFOR, NUMCIVFOR, DENLOCFOR, CAPFOR, SIGPROFOR, " + " DATLIQ, DATPAG, FLGAPP, ULTAGG, UTEAGG)" + " SELECT " + CodPos + " AS CODPOS, " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AS MAT, " + PRORAP.ToString() + " AS PRORAP, ";
              dateTime = Convert.ToDateTime(DatDec.Trim());
              string str20 = DBMethods.Db2Date(dateTime.ToString());
              string str21 = str19 + str20 + " AS DATDEC, " + " DATCES, CODCAUCES, " + CodPos + ", DATDENCES, ";
              dateTime = Convert.ToDateTime(DatDec.Trim());
              string str22 = DBMethods.Db2Date(dateTime.ToString());
              string str23 = str21 + str22 + " AS DATASS, ";
              dateTime = Convert.ToDateTime(trasferimentiRdlOCM.ListaAziendes[index1].datden.Trim());
              string str24 = DBMethods.Db2Date(dateTime.ToString());
              flag2 = this.objDataAccess.WriteTransactionData(str23 + str24 + " AS DATDEN, " + " DATPRE, DATPRO, NUMPRO, MATFON, PROFON, " + " CODPOSFOR, RAGSOCFOR, PARIVAFOR, CODFISFOR, INDFOR, TELFOR, CODDUGFOR, NUMCIVFOR, DENLOCFOR, CAPFOR, SIGPROFOR, " + " DATLIQ, DATPAG, 'I', CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + " FROM RAPLAV WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString(), CommandType.Text);
              string str25 = " SELECT * FROM SOSRAP " + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString();
              dateTime = Convert.ToDateTime(DatDec.Trim());
              string str26 = DBMethods.Db2Date(dateTime.ToString());
              string str27 = str25 + " AND ((" + str26 + " BETWEEN DATINISOS AND DATFINSOS) ";
              dateTime = Convert.ToDateTime(trasferimentiRdlOCM.ListaAziendes[index1].datden.Trim());
              string str28 = DBMethods.Db2Date(dateTime.ToString());
              string strSQL2 = str27 + " OR ( DATINISOS > " + str28 + ")) " + " ORDER BY DATINISOS";
              dataTable1.Clear();
              DataTable dataTable9 = this.objDataAccess.GetDataTable(strSQL2);
              if (dataTable9.Rows.Count > 0)
              {
                int num6 = dataTable9.Rows.Count - 1;
                for (int index7 = 0; index7 <= num6; ++index7)
                {
                  DATINI = index7 != 0 ? dataTable9.Rows[index7]["DATINISOS"].ToString() : (!(Convert.ToDateTime(DatDec.Trim()) >= Convert.ToDateTime(dataTable9.Rows[index7]["DATINISOS"]) & Convert.ToDateTime(DatDec.Trim()) <= Convert.ToDateTime(dataTable9.Rows[index7]["DATFINSOS"])) ? dataTable9.Rows[index7]["DATINISOS"].ToString() : DatDec.Trim());
                  string str29 = "INSERT INTO SOSRAP (CODSOS, CODPOS, MAT, PRORAP, PROSOS," + " DATINISOS, DATFINSOS, PERAZI, PERFIG, STASOS, CODPOSDA, CODPOSA, UTEAGG, ULTAGG )" + " SELECT CODSOS, " + CodPos + " AS CODPOS, " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AS MAT, " + PRORAP.ToString() + " AS PRORAP, " + " PROSOS, ";
                  dateTime = Convert.ToDateTime(DATINI);
                  string str30 = DBMethods.Db2Date(dateTime.ToString());
                  string str31 = str29 + str30 + " AS DATINISOS , " + " DATFINSOS, PERAZI, PERFIG, STASOS, " + CodPos + " AS CODPOSDA, " + " CODPOSA , " + " UTEAGG,ULTAGG FROM SOSRAP " + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString();
                  dateTime = Convert.ToDateTime(dataTable9.Rows[index7]["DATINISOS"]);
                  string str32 = DBMethods.Db2Date(dateTime.ToString());
                  string str33 = str31 + " AND DATINISOS = " + str32;
                  flag2 = this.objDataAccess.WriteTransactionData(" AND PROSOS = " + str1, CommandType.Text);
                }
                string strSQL3 = "SELECT PROSOS FROM SOSRAP " + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND VALUE(STASOS, 0) = 0";
                dataTable5.Clear();
                dataTable5 = this.objDataAccess.GetDataTable(strSQL3);
                int num7 = dataTable5.Rows.Count - 1;
                for (int index8 = 0; index8 <= num7; ++index8)
                {
                  idocDatiE1Pitype1.Clear();
                  idocDatiE1Pitype1 = this.iDOC_RDL.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(CodPos), Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString()), PRORAP, Convert.ToInt32(dataTable5.Rows[index8]["PROSOS"]), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                  this.iDOC_RDL.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype1, false);
                }
              }
              string str34 = " SELECT * FROM STORDL " + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString();
              dateTime = Convert.ToDateTime(DatDec.Trim());
              string str35 = DBMethods.Db2Date(dateTime.ToString());
              string str36 = str34 + " AND ((" + str35 + " BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31'))";
              dateTime = Convert.ToDateTime(DatDec.Trim());
              string str37 = DBMethods.Db2Date(dateTime.ToString());
              string strSQL4 = str36 + " OR  (DATINI >= " + str37 + "))" + " ORDER BY DATINI";
              dataTable9.Clear();
              dataTable1 = this.objDataAccess.GetDataTable(strSQL4);
              if (dataTable1.Rows.Count == 0)
              {
                string strSQL5 = "SELECT * FROM STORDL " + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString() + " AND DATINI = (SELECT MAX (DATINI) FROM STORDL " + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString();
                dataTable1.Clear();
                dataTable1 = this.objDataAccess.GetDataTable(strSQL5);
              }
              int num8 = dataTable1.Rows.Count - 1;
              for (int index9 = 0; index9 <= num8; ++index9)
              {
                DATINI = index9 != 0 ? dataTable1.Rows[index9]["DATINI"].ToString() : DatDec.Trim();
                string str38 = " INSERT INTO STORDL " + " (CODPOS,MAT,PRORAP,DATINI," + "DATFIN,TIPRAP,CODCON,CODLOC,CODLIV,TRAECO," + "FAP,NUMMEN,PERAPP,MESMEN14,MESMEN15,MESMEN16,INDANN, CODGRUASS, ASSCON," + "ABBPRE, PERPAR, DATSCATER, NUMSCAMAT, IMPSCAMAT, DATULTSCA, DATNEWSCA," + "CODPOSDA,CODPOSA,ULTAGG, UTEAGG) " + " SELECT " + CodPos + " AS CODPOS, " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AS MAT, " + PRORAP.ToString() + " AS PRORAP, ";
                dateTime = Convert.ToDateTime(DATINI);
                string str39 = DBMethods.Db2Date(dateTime.ToString());
                string str40 = str38 + str39 + " AS DATINI , " + " DATFIN, TIPRAP, CODCON, CODLOC, CODLIV, TRAECO," + " FAP, NUMMEN, PERAPP, MESMEN14, MESMEN15, MESMEN16, " + " INDANN, ";
                string str41 = (!Convert.ToBoolean(this.objDataAccess.Get1ValueFromSQL("SELECT NATGIU FROM AZI WHERE CODPOS= " + CodPos, CommandType.Text)) ? str40 + " 1 AS CODGRUASS," : str40 + " 2 AS CODGRUASS, ") + " ASSCON, ABBPRE, PERPAR, DATSCATER, " + " NUMSCAMAT, IMPSCAMAT, DATULTSCA, DATNEWSCA, " + CodPos + " AS CODPOSDA, " + " CODPOSA , " + " ULTAGG, UTEAGG FROM STORDL " + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString();
                dateTime = Convert.ToDateTime(dataTable1.Rows[index9]["DATINI"]);
                string str42 = DBMethods.Db2Date(dateTime.ToString());
                flag2 = this.objDataAccess.WriteTransactionData(str41 + " AND DATINI = " + str42, CommandType.Text);
                string str43 = " INSERT INTO IMPAGG (CODPOS,MAT,PRORAP,DATINI,PROIMP," + " MENAGG,IMPAGG, CODPOSDA, CODPOSA, ULTAGG,UTEAGG)" + " SELECT " + CodPos + " AS CODPOS, " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AS MAT, " + PRORAP.ToString() + " AS PRORAP, ";
                dateTime = Convert.ToDateTime(DATINI);
                string str44 = DBMethods.Db2Date(dateTime.ToString());
                string str45 = str43 + str44 + " AS DATINI , " + " PROIMP, MENAGG, IMPAGG, " + CodPos + " AS CODPOSDA, " + " CODPOSA , " + " ULTAGG, UTEAGG FROM IMPAGG " + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString();
                dateTime = Convert.ToDateTime(dataTable1.Rows[index9]["DATINI"]);
                string str46 = DBMethods.Db2Date(dateTime.ToString());
                flag2 = this.objDataAccess.WriteTransactionData(str45 + " AND DATINI = " + str46, CommandType.Text);
                string str47 = "INSERT INTO PARTIMM (CODPOS,MAT,PRORAP,DATINI,PARMES, CODPOSDA, CODPOSA, " + " ULTAGG,UTEAGG)" + " SELECT " + CodPos + " AS CODPOS, " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AS MAT, " + PRORAP.ToString() + " AS PRORAP, ";
                dateTime = Convert.ToDateTime(DATINI);
                string str48 = DBMethods.Db2Date(dateTime.ToString());
                string str49 = str47 + str48 + " AS DATINI , " + " PARMES, " + CodPos + " AS CODPOSDA, " + " CODPOSA , " + " ULTAGG, UTEAGG FROM PARTIMM " + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString();
                dateTime = Convert.ToDateTime(dataTable1.Rows[index9]["DATINI"]);
                string str50 = DBMethods.Db2Date(dateTime.ToString());
                flag2 = this.objDataAccess.WriteTransactionData(str49 + " AND DATINI = " + str50, CommandType.Text);
              }
            }
            this.iDOC_RDL.AGGIORNA_RAPLAV_INPS(this.objDataAccess, u, Convert.ToInt32(trasferimentiRdlOCM.ListaAziendes[index1].codpos.ToString()), Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString()), PRORAP);
            this.iDOC_RDL.AGGIORNA_RAPLAV_INPS(this.objDataAccess, u, Convert.ToInt32(CodPos), Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString()), Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString()));
            this.iDOC_RDL.Module_AggiornaStordl(this.objDataAccess, u, Convert.ToInt32(CodPos), Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString()), PRORAP);
            this.iDOC_RDL.Module_AggiornaStordl(this.objDataAccess, u, Convert.ToInt32(CodPos), Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString()), PRORAP, DATINI);
            DataTable dataTable10 = new DataTable();
            int int32_5 = Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(PROGTRA, 0) FROM RAPLAV WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString(), CommandType.Text));
            if (int32_5 == 0)
            {
              string strSQL6 = "SELECT VALUE(MAX(PROGTRA), 0) + 1 FROM RAPLAV WHERE MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString();
              int num9 = int32_5 + Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL(strSQL6, CommandType.Text));
              DataTable dataTable11 = this.objDataAccess.GetDataTable("SELECT DATDEC FROM RAPLAV WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString());
              if (dataTable11.Rows.Count > 0 && !string.IsNullOrEmpty(dataTable11.Rows[0]["DATDEC"].ToString().Trim()))
              {
                flag2 = this.objDataAccess.WriteTransactionData("UPDATE RAPLAV SET PROGTRA = " + num9.ToString() + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString(), CommandType.Text);
                string[] strArray2 = new string[6]
                {
                  "UPDATE RAPLAV SET PROGTRA = ",
                  num9.ToString(),
                  ", DATLIQTFR = '",
                  null,
                  null,
                  null
                };
                dateTime = Convert.ToDateTime(dataTable11.Rows[0]["DATDEC"]);
                strArray2[3] = DBMethods.Db2Date(dateTime.ToString());
                strArray2[4] = "' WHERE CODPOS = ";
                strArray2[5] = CodPos;
                flag2 = this.objDataAccess.WriteTransactionData(string.Concat(strArray2) + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + PRORAP.ToString(), CommandType.Text);
              }
            }
            else
            {
              DataTable dataTable12 = this.objDataAccess.GetDataTable("SELECT DATLIQTFR FROM RAPLAV WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString());
              if (dataTable12.Rows.Count > 0 && !string.IsNullOrEmpty(dataTable12.Rows[0]["DATLIQTFR"].ToString().Trim()))
              {
                string[] strArray3 = new string[6]
                {
                  "UPDATE RAPLAV SET PROGTRA = ",
                  int32_5.ToString(),
                  ", DATLIQTFR = ",
                  null,
                  null,
                  null
                };
                dateTime = Convert.ToDateTime(dataTable12.Rows[0]["DATLIQTFR"]);
                strArray3[3] = DBMethods.Db2Date(dateTime.ToString());
                strArray3[4] = " WHERE CODPOS = ";
                strArray3[5] = CodPos;
                flag2 = this.objDataAccess.WriteTransactionData(string.Concat(strArray3) + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + PRORAP.ToString(), CommandType.Text);
              }
            }
            string str51 = " DELETE FROM IMPAGG WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString();
            dateTime = Convert.ToDateTime(trasferimentiRdlOCM.ListaAziendes[index1].datden.Trim());
            dateTime = dateTime.AddDays(-1.0);
            string str52 = DBMethods.Db2Date(dateTime.ToString());
            flag2 = this.objDataAccess.WriteTransactionData(str51 + " AND DATINI > '" + str52 + "' ", CommandType.Text);
            string str53 = "DELETE FROM PARTIMM WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString();
            dateTime = Convert.ToDateTime(trasferimentiRdlOCM.ListaAziendes[index1].datden.Trim());
            dateTime = dateTime.AddDays(-1.0);
            string str54 = DBMethods.Db2Date(dateTime.ToString());
            flag2 = this.objDataAccess.WriteTransactionData(str53 + " AND DATINI > '" + str54 + "' ", CommandType.Text);
            string str55 = " DELETE FROM SOSRAP WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString();
            dateTime = Convert.ToDateTime(trasferimentiRdlOCM.ListaAziendes[index1].datden.Trim());
            dateTime = dateTime.AddDays(-1.0);
            string str56 = DBMethods.Db2Date(dateTime.ToString());
            flag2 = this.objDataAccess.WriteTransactionData(str55 + " AND DATINISOS > '" + str56 + "' ", CommandType.Text);
            string str57 = " DELETE FROM STORDL WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString();
            dateTime = Convert.ToDateTime(trasferimentiRdlOCM.ListaAziendes[index1].datden.Trim());
            dateTime = dateTime.AddDays(-1.0);
            string str58 = DBMethods.Db2Date(dateTime.ToString());
            flag2 = this.objDataAccess.WriteTransactionData(str57 + " AND DATINI > '" + str58 + "' ", CommandType.Text);
            dateTime = Convert.ToDateTime(trasferimentiRdlOCM.ListaAziendes[index1].datden.Trim());
            dateTime = dateTime.AddDays(-1.0);
            flag2 = this.objDataAccess.WriteTransactionData(" UPDATE STORDL SET " + " DATFIN = '" + DBMethods.Db2Date(dateTime.ToString()) + "' " + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString() + " AND DATINI = ( SELECT MAX ( DATINI ) FROM STORDL " + " WHERE CODPOS =  " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString() + ")", CommandType.Text);
            dateTime = Convert.ToDateTime(trasferimentiRdlOCM.ListaAziendes[index1].datden.Trim());
            dateTime = dateTime.AddDays(-1.0);
            string str59 = " UPDATE SOSRAP SET " + " DATFINSOS = '" + DBMethods.Db2Date(dateTime.ToString()) + "' " + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString();
            dateTime = Convert.ToDateTime(DatDec.Trim());
            string str60 = DBMethods.Db2Date(dateTime.ToString());
            flag2 = this.objDataAccess.WriteTransactionData(str59 + " AND '" + str60 + "' BETWEEN DATINISOS AND DATFINSOS", CommandType.Text);
            dateTime = Convert.ToDateTime(trasferimentiRdlOCM.ListaAziendes[index1].datden.Trim());
            flag1 = this.objDataAccess.WriteTransactionData("UPDATE RAPLAV SET " + " DATCES = '" + DBMethods.Db2Date(dateTime.ToString()) + "', " + " CODCAUCES = 4, " + " CODPOSA = " + trasferimentiRdlOCM.ListaAziendes[index1].codpos.ToString() + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString(), CommandType.Text);
            idocDatiE1Pitype1.Clear();
            DataTable idocDatiE1Pitype2 = this.iDOC_RDL.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0000", 0, Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString()), 0, 0, "T1", "01", "9999-12-31", DatDec.Trim().Trim(), "", "", 0, 0, 0, "", "TRASFERIMENTO", "", "", "T1");
            this.iDOC_RDL.WRITE_IDOC_E1PITYP(this.objDataAccess, "0000", idocDatiE1Pitype2, false);
            idocDatiE1Pitype2.Clear();
            DT = this.iDOC_RDL.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0016", 0, Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString()), 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "TRASFERIMENTO", "", "", "T1");
            this.iDOC_RDL.WRITE_IDOC_E1PITYP(this.objDataAccess, "9001", DT, false);
            this.iDOC_RDL.WRITE_IDOC_E1PITYP(this.objDataAccess, "0016", DT, false);
            string strSQL7 = "SELECT PROSOS FROM SOSRAP " + " WHERE CODPOS = " + CodPos + " AND MAT = " + trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString() + " AND PRORAP = " + trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString() + " AND VALUE(STASOS, 0) = 0";
            dataTable5.Clear();
            dataTable2 = this.objDataAccess.GetDataTable(strSQL7);
            int num10 = dataTable2.Rows.Count - 1;
            for (int index10 = 0; index10 <= num10; ++index10)
            {
              DT.Clear();
              DT = this.iDOC_RDL.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(CodPos), Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].Matricola.ToString()), Convert.ToInt32(trasferimentiRdlOCM.ListaIscrittis[index2].ProRap.ToString()), Convert.ToInt32(dataTable2.Rows[index10]["PROSOS"]), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
              this.iDOC_RDL.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", DT, false);
            }
          }
        }
        if (!flag1)
        {
          this.objDataAccess.EndTransaction(false);
          MsgErrore = "Trasferimento RDL non riuscito";
        }
        else
        {
          flag2 = this.objDataAccess.WriteTransactionData("UPDATE TRARAP SET STAELA = 1 " + " WHERE CODPOS = " + CodPos + " AND STAELA = 3 ", CommandType.Text);
          this.iDOC_RDL.Aggiorna_IDOC(this.objDataAccess);
          this.iDOC_RDL.objDtCONTIDOC = (DataTable) null;
          this.objDataAccess.EndTransaction(true);
          MsgSuccess = "Trasferimento RDL completato";
        }
      }
      catch (Exception ex)
      {
        this.objDataAccess.EndTransaction(false);
        MsgErrore = "Trasferimento RDL non riuscito";
      }
    }

    public bool VerificaAzioniLegali(string CodPos, string DatDec, ref string MsgErrore)
    {
      if (Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(COUNT(*),0) AS CNT FROM STAAZILEG WHERE CODPOS = " + CodPos + " AND '" + DBMethods.Db2Date(DatDec) + "' BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')", CommandType.Text)) <= 0)
        return true;
      MsgErrore = "L'azienda " + CodPos + " di destinazione ha azioni legali in corso.";
      return false;
    }

    public void CaricaDati_ListaIscrittiXTrasferimenti(
      TrasferimentiRdlOCM trasferimentiRdlOCM,
      string DatDec,
      string CodPos,
      ref string MsgErrore)
    {
      DataTable dataTable1 = new DataTable();
      if (Convert.ToDateTime(DatDec) > Convert.ToDateTime(DateTime.Now))
      {
        MsgErrore = "La data di denuncia non può essere maggiore della data di sistema";
      }
      else
      {
        try
        {
          string str = this.objDataAccess.Get1ValueFromSQL("SELECT RAGSOC FROM AZI WHERE CODPOS=" + CodPos, CommandType.Text);
          trasferimentiRdlOCM.datitrasferimento.RagSoc = str;
          DataTable dataTable2 = this.objDataAccess.GetDataTable("SELECT RAPLAV.CODPOS, ISCT.MAT, " + " ISCT.COG, ISCT.NOM, ISCT.CODFIS, ISCT.SES, RAPLAV.DATDEC, " + " RAPLAV.DATCES, CODCOM.DENCOM, RAPLAV.PRORAP,  " + " ISCT.DATNAS, CODCOM.SIGPRO, " + " STACIV.DENSTACIV, " + " TITSTU.DENTITSTU, " + " ISCT.ULTAGG, ISCT.UTEAGG " + " FROM CODCOM RIGHT JOIN ISCT ON CODCOM.CODCOM = ISCT.CODCOM " + " LEFT JOIN STACIV ON ISCT.STACIV = STACIV.CODSTACIV " + " LEFT JOIN TITSTU ON ISCT.TITSTU = TITSTU.CODTITSTU " + " INNER JOIN RAPLAV ON ISCT.MAT = RAPLAV.MAT " + " WHERE RAPLAV.CODPOS = " + CodPos + " AND '" + DBMethods.Db2Date(DatDec) + "' BETWEEN RAPLAV.DATDEC AND VALUE ( RAPLAV.DATCES, '9999-12-31') ");
          List<TrasferimentiRdlOCM.DatiTrasferimento> datiTrasferimentoList = new List<TrasferimentiRdlOCM.DatiTrasferimento>();
          if (dataTable2.Rows.Count <= 0)
            return;
          foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
          {
            TrasferimentiRdlOCM.DatiTrasferimento datiTrasferimento = new TrasferimentiRdlOCM.DatiTrasferimento()
            {
              CodPos = row["CODPOS"].ToString(),
              Matricola = row["MAT"].ToString(),
              Nome = row["NOM"].ToString(),
              CodFis = row["CODFIS"].ToString(),
              Cognome = row["COG"].ToString(),
              DatIsc = row["DATDEC"].ToString(),
              ProRap = row["PRORAP"].ToString(),
              DatDec = DatDec,
              RagSoc = str
            };
            datiTrasferimentoList.Add(datiTrasferimento);
          }
          trasferimentiRdlOCM.ListaIscrittis = datiTrasferimentoList;
        }
        catch (Exception ex)
        {
          MsgErrore = "Caricamento dati non riuscito";
        }
      }
    }

    private bool VerificaStordl(
      string ProRap,
      string Matricola,
      string CodPos,
      string DatDec,
      ref string MsgErrore)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = this.objDataAccess.GetDataTable("SELECT TIPRAP, DATSCATER FROM STORDL WHERE CODPOS= " + CodPos + " AND MAT =" + Matricola + " AND PRORAP =" + ProRap + " AND DATINI =(SELECT MAX(DATINI) FROM STORDL WHERE CODPOS=" + CodPos + " AND MAT =" + Matricola + " AND PRORAP =" + ProRap + ")");
      if (dataTable2.Rows.Count <= 0 || !(dataTable2.Rows[0]["TIPRAP"] is int num))
        return true;
      switch (num)
      {
        case 2:
        case 4:
        case 6:
        case 7:
        case 8:
        case 10:
          if (!(Convert.ToDateTime(dataTable2.Rows[0]["DATSCATER"]) > Convert.ToDateTime(DatDec)))
            return true;
          MsgErrore = "La data scadenza termine del rapporto di lavoro per la matricola " + Matricola + " è successiva alla data di trasferimento. Continuare?";
          return false;
        default:
          return true;
      }
    }

    public bool BtnAggiungi_Click(
      TrasferimentiRdlOCM tra,
      string CodPos,
      string CodPosNew,
      string DatDec,
      ref string MsgErrore)
    {
      DataTable dataTable1 = new DataTable();
      try
      {
        if (!this.Module_Autorizzazioni_Speciali(CodPosNew, "CONTRIBUTI", ref MsgErrore))
        {
          MsgErrore = "Utente non abilitato";
          return false;
        }
        if (CodPosNew == CodPos)
        {
          MsgErrore = "L'azienda di destinazione non può essere quella di origine";
          return false;
        }
        string strSQL = "SELECT VALUE(DATCHI, '9999-12-31') AS DATCHI FROM AZI WHERE CODPOS = " + CodPosNew;
        dataTable1.Clear();
        DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
        if (Convert.ToDateTime(DatDec) > Convert.ToDateTime(dataTable2.Rows[0]["DATCHI"]))
        {
          MsgErrore = "Alla data del trasferimento l'Azienda di destinazione risulta chiusa";
          return false;
        }
        for (int index = 0; index <= tra.ListaAziendes.Count - 1; ++index)
        {
          if (CodPosNew == tra.ListaAziendes[index].codpos)
          {
            MsgErrore = "Azienda già presente in lista";
            return false;
          }
        }
        int int32_1 = Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT NATGIU FROM AZI WHERE CODPOS= " + CodPos, CommandType.Text));
        int int32_2 = Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT NATGIU FROM AZI WHERE CODPOS= " + CodPosNew, CommandType.Text));
        if (int32_1 != 10 & int32_2 == 10)
        {
          MsgErrore = "L'Azienda di origine (Azienda) e di destinazione (Consorzio di Bonifica) non sono della stessa tipologia. ";
          return false;
        }
        if (!(int32_1 == 10 & int32_2 != 10))
          return true;
        MsgErrore = "L'Azienda di origine (Consorzio di Bonifica) e di destinazione (Azienda) non sono della stessa tipologia. ";
        return false;
      }
      catch (Exception ex)
      {
        MsgErrore = " Errore.";
        return false;
      }
    }

    public TrasferimentiRdlOCM CaricaDatiRicercaTrasferimenti(
      TrasferimentiRdlOCM.DatiTrasferimento rdl,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      string str1 = "";
      string str2 = "";
      string str3 = "";
      DataTable dataTable1 = new DataTable();
      IDOC_RDL idocRdl = new IDOC_RDL();
      Utile utile = new Utile();
      try
      {
        DataTable dataTable2 = utile.CREA_DT_AUTORIZZAZIONI_ENPAIA(ref ErroreMSG);
        if (string.IsNullOrEmpty(rdl.CodPos) && !this.Module_Autorizzazioni_Speciali(rdl.CodPos, "CONTRIBUTI", ref ErroreMSG))
        {
          ErroreMSG = "Utente non abilitato";
          return (TrasferimentiRdlOCM) null;
        }
        if (string.IsNullOrEmpty(rdl.CodPosTra) && !this.Module_Autorizzazioni_Speciali(rdl.CodPosTra, "CONTRIBUTI", ref ErroreMSG))
        {
          ErroreMSG = "Utente non abilitato";
          return (TrasferimentiRdlOCM) null;
        }
        string str4 = "SELECT TRARAP.CODPOS, A.RAGSOC, TRARAP.MAT, TRARAP.PRORAP," + " ISCT.COG, ISCT.NOM, ISCT.CODFIS, TRARAP.GRUPPO, " + " TRARAP.PROTRARAP, B.RAGSOC AS RAGSOCTRA, TRARAP.DATTRA," + " TRARAP.CODPOSTRA, TRARAP.MATTRA, TRARAP.PRORAPTRA, TRARAP.PERTRA, TRARAP.PERTFR," + " TRARAP.DATANN, TRARAP.STAELA, TRARAP.ULTAGG, TRARAP.UTEAGG" + " FROM TRARAP" + " INNER JOIN ISCT ON TRARAP.MAT = ISCT.MAT " + " INNER JOIN AZI A ON TRARAP.CODPOS = A.CODPOS" + " INNER JOIN AZI B ON TRARAP.CODPOSTRA = B.CODPOS";
        if (rdl.CodPos == "")
        {
          for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
          {
            if (dataTable2.Rows[index]["ABILITATO"].ToString() == "NO")
              str3 = str3 + "," + dataTable2.Rows[index]["POSIZIONE"]?.ToString();
          }
          if (str3.ToString().Trim() != "")
          {
            str3 = str3.Substring(1);
            str1 = str1 + " AND TRARAP.CODPOS NOT IN (" + str3 + ") ";
          }
        }
        if (rdl.CodPosTra == "")
        {
          for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
          {
            if (dataTable2.Rows[index]["ABILITATO"].ToString() == "NO")
              str3 = str3 + "," + dataTable2.Rows[index]["POSIZIONE"]?.ToString();
          }
          if (str2.ToString().Trim() != "")
          {
            string str5 = str2.Substring(1);
            str1 = str1 + " AND TRARAP.CODPOSTRA NOT IN (" + str5 + ") ";
          }
        }
        string stato = rdl.Stato;
        if (!(stato == "Tutti") && (stato == null || stato.Length != 0))
        {
          if (!(stato == "Annullati"))
          {
            if (stato == "Validi")
              str1 += " AND DATANN IS NULL";
          }
          else
            str1 += " AND DATANN IS NOT NULL";
        }
        if (!string.IsNullOrEmpty(rdl.CodPos))
          str1 = str1 + " And TRARAP.CODPOS = " + rdl.CodPos;
        if (!string.IsNullOrEmpty(rdl.CodPosTra))
          str1 = str1 + " And TRARAP.CODPOSTRA = " + rdl.CodPosTra;
        if (!string.IsNullOrEmpty(rdl.Matricola))
          str1 = str1 + " And TRARAP.MAT = " + rdl.Matricola;
        if (!string.IsNullOrEmpty(rdl.RagSoc))
          str1 = str1 + " And A.RAGSOC = '" + rdl.RagSoc + "'";
        if (!string.IsNullOrEmpty(rdl.RagSocTra))
          str1 = str1 + " And B.RAGSOC = '" + rdl.RagSocTra + "'";
        if (!string.IsNullOrEmpty(rdl.Cognome))
          str1 = str1 + " AND ISCT.COG = " + DBMethods.DoublePeakForSql(rdl.Cognome);
        if (!string.IsNullOrEmpty(rdl.Nome))
          str1 = str1 + " AND ISCT.NOM = " + DBMethods.DoublePeakForSql(rdl.Nome);
        if (!string.IsNullOrEmpty(rdl.DatTraDal))
          str1 = str1 + " AND TRARAP.DATTRA >= " + DBMethods.Db2Date(rdl.DatTraDal);
        if (!string.IsNullOrEmpty(rdl.DatTraAl))
          str1 = str1 + " AND TRARAP.DATTRA <= " + DBMethods.Db2Date(rdl.DatTraAl);
        if (!string.IsNullOrEmpty(rdl.CodFis))
          str1 = str1 + " AND ISCT.CODFIS = " + DBMethods.DoublePeakForSql(rdl.CodFis);
        if (!string.IsNullOrEmpty(rdl.DatNas))
          str1 = str1 + " AND ISCT.DATNAS = " + DBMethods.Db2Date(rdl.DatNas);
        if (str1 != "")
          str1 = " WHERE " + str1.Substring(4);
        string str6 = str4 + str1 + " ORDER BY TRARAP.CODPOS, TRARAP.MAT, TRARAP.PRORAP, TRARAP.CODPOSTRA, TRARAP.MATTRA, TRARAP.PROTRARAP";
        if (string.IsNullOrEmpty(rdl.CodPos) && string.IsNullOrEmpty(rdl.CodPosTra) && string.IsNullOrEmpty(rdl.Matricola) && string.IsNullOrEmpty(rdl.RagSoc) && string.IsNullOrEmpty(rdl.RagSocTra) && string.IsNullOrEmpty(rdl.Cognome))
          str6 = str6 + str1 + "LIMIT 0";
        dataTable1 = this.objDataAccess.GetDataTable(str6);
        TrasferimentiRdlOCM trasferimentiRdlOcm = new TrasferimentiRdlOCM();
        List<TrasferimentiRdlOCM.DatiTrasferimento> datiTrasferimentoList = new List<TrasferimentiRdlOCM.DatiTrasferimento>();
        iDB2DataReader dataReaderFromQuery = this.objDataAccess.GetDataReaderFromQuery(str6, CommandType.Text);
        while (dataReaderFromQuery.Read())
          datiTrasferimentoList.Add(new TrasferimentiRdlOCM.DatiTrasferimento()
          {
            DatTra = !DBNull.Value.Equals(dataReaderFromQuery["DATTRA"]) ? dataReaderFromQuery["DATTRA"].ToString().Substring(0, 10) : "Dato non disponibile",
            CodFis = !DBNull.Value.Equals(dataReaderFromQuery["CODFIS"]) ? dataReaderFromQuery["CODFIS"].ToString() : "Dato non disponibile",
            CodPos = !DBNull.Value.Equals(dataReaderFromQuery["CODPOS"]) ? dataReaderFromQuery["CODPOS"].ToString() : "0",
            CodPosTra = !DBNull.Value.Equals(dataReaderFromQuery["CODPOSTRA"]) ? dataReaderFromQuery["CODPOSTRA"].ToString() : "0",
            RagSoc = !DBNull.Value.Equals(dataReaderFromQuery["RAGSOC"]) ? dataReaderFromQuery["RAGSOC"].ToString() : "Dato non disponibile",
            RagSocTra = !DBNull.Value.Equals(dataReaderFromQuery["RAGSOCTRA"]) ? dataReaderFromQuery["RAGSOCTRA"].ToString() : "Dato non disponibile",
            Nome = !DBNull.Value.Equals(dataReaderFromQuery["NOM"]) ? dataReaderFromQuery["NOM"].ToString() : "Dato non disponibile",
            Cognome = !DBNull.Value.Equals(dataReaderFromQuery["COG"]) ? dataReaderFromQuery["COG"].ToString() : "Dato non disponibile",
            Matricola = !DBNull.Value.Equals(dataReaderFromQuery["MAT"]) ? dataReaderFromQuery["MAT"].ToString() : "Dato non disponibile",
            ProRap = !DBNull.Value.Equals(dataReaderFromQuery["PRORAP"]) ? dataReaderFromQuery["PRORAP"].ToString() : "Dato non disponibile",
            ProRapTra = !DBNull.Value.Equals(dataReaderFromQuery["PRORAPTRA"]) ? dataReaderFromQuery["PRORAPTRA"].ToString() : "Dato non disponibile",
            ProTraRap = !DBNull.Value.Equals(dataReaderFromQuery["PROTRARAP"]) ? dataReaderFromQuery["PROTRARAP"].ToString() : "Dato non disponibile",
            Gruppo = !DBNull.Value.Equals(dataReaderFromQuery["GRUPPO"]) ? dataReaderFromQuery["GRUPPO"].ToString() : (string) null
          });
        if (datiTrasferimentoList.Count > 0)
        {
          trasferimentiRdlOcm.datiTrasferimentoList = datiTrasferimentoList;
          return trasferimentiRdlOcm;
        }
        ErroreMSG = "Nessun risultato trovato ";
        return (TrasferimentiRdlOCM) null;
      }
      catch (Exception ex)
      {
        ErroreMSG = "Nessun risultato trovato ";
        return (TrasferimentiRdlOCM) null;
      }
    }

    public bool Module_Autorizzazioni_Speciali(string CODPOS, string SISTEMA, ref string ErroreMSG)
    {
      DataTable dataTable = new Utile().CREA_DT_AUTORIZZAZIONI_ENPAIA(ref ErroreMSG);
      if (!(SISTEMA == "CONTRIBUTI"))
      {
        if (SISTEMA == "AGRIFONDO")
        {
          for (int index = 0; index <= dataTable.Rows.Count - 1; ++index)
          {
            if (dataTable.Rows[index]["POSIZIONE_AGF"].ToString() == CODPOS)
              return dataTable.Rows[index]["ABILITATO_AGF"].ToString() == "SI";
          }
        }
      }
      else
      {
        for (int index = 0; index <= dataTable.Rows.Count - 1; ++index)
        {
          if (dataTable.Rows[index]["POSIZIONE"].ToString() == CODPOS)
            return dataTable.Rows[index]["ABILITATO"].ToString() == "SI";
        }
      }
      return true;
    }

    public bool Check_Input(TrasferimentiRdlOCM rdl, ref string ErrorMSG)
    {
      DataLayer dataLayer = new DataLayer();
      IDOC_RDL idocRdl = new IDOC_RDL();
      ModGetDati modGetDati = new ModGetDati();
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      if (!string.IsNullOrEmpty(rdl.datitrasferimento.DatAnn))
      {
        ErrorMSG = "Trasferimento già annullato";
        return false;
      }
      DataTable dataTable1 = new DataTable();
      string strSQL1 = "SELECT VALUE(COUNT(*), 0) AS TOT  FROM BKRAPLAV WHERE CODPOS =" + rdl.datitrasferimento.CodPos + " AND MAT = " + rdl.datitrasferimento.Matricola + " AND PRORAP = '" + rdl.datitrasferimento.ProRap + "'" + " AND PROTRARAP = '" + rdl.datitrasferimento.ProTraRap + "'";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
      if (dataTable2.Rows[0]["TOT"].ToString() == "0")
      {
        ErrorMSG = "Attenzione... questo trasferimento non può essere annullato perchè proveniente dal vecchio sistema";
        return false;
      }
      string strSQL2 = "SELECT * FROM TRARAP WHERE MAT = " + rdl.datitrasferimento.Matricola + " AND DATANN IS NULL ORDER BY DATTRA DESC, PROTRARAP DESC";
      dataTable2.Rows.Clear();
      DataTable dataTable3 = dataLayer.GetDataTable(strSQL2);
      if (dataTable3.Rows.Count > 0)
      {
        if (dataTable3.Rows[0]["CODPOS"].ToString() == rdl.datitrasferimento.CodPos)
        {
          if (dataTable3.Rows[0]["MAT"].ToString() == rdl.datitrasferimento.Matricola)
          {
            if (dataTable3.Rows[0]["PRORAP"].ToString() == rdl.datitrasferimento.ProRap)
            {
              if (dataTable3.Rows[0]["PROTRARAP"].ToString() == rdl.datitrasferimento.ProTraRap)
              {
                if (!(dataTable3.Rows[0]["GRUPPO"].ToString() == rdl.datitrasferimento.Gruppo))
                  ;
              }
              else if (!(dataTable3.Rows[0]["GRUPPO"].ToString() == rdl.datitrasferimento.Gruppo))
              {
                ErrorMSG = "Attenzione... questo trasferimento non può essere annullato perchè la matricola è stata ulteriormente trasferita";
                return false;
              }
            }
            else
            {
              ErrorMSG = "Attenzione... questo trasferimento non può essere annullato perchè la matricola è stata ulteriormente trasferita";
              return false;
            }
          }
          else
          {
            ErrorMSG = "Attenzione... questo trasferimento non può essere annullato perchè la matricola è stata ulteriormente trasferita";
            return false;
          }
        }
        return true;
      }
      ErrorMSG = "Attenzione... questo trasferimento non può essere annullato perchè la matricola è stata ulteriormente trasferita";
      return false;
    }

    public void EseguiAnnullamentoTrasferimento(
      TFI.OCM.Utente.Utente u,
      string CodPos,
      string CodPosTra,
      string RagSoc,
      string RagSocTra,
      string Matricola,
      string Nome,
      string Cognome,
      string ProRap,
      string ProRapTra,
      string ProTraRap,
      string Gruppo,
      string ProSos,
      string DatAnn,
      ref string ErrorMSG,
      ref string SuccessMSG)
    {
      DataTable dataTable1 = new DataTable();
      int num = 0;
      bool flag1 = false;
      bool flag2 = false;
      string str1 = "";
      DataTable DT = new DataTable();
      DataTable dataTable2 = new DataTable();
      IDOC_RDL idocRdl = new IDOC_RDL();
      TrasferimentiRdlOCM rdl = new TrasferimentiRdlOCM();
      rdl.datitrasferimento.CodPos = CodPos;
      rdl.datitrasferimento.CodPosTra = CodPosTra;
      rdl.datitrasferimento.RagSoc = RagSoc;
      rdl.datitrasferimento.RagSocTra = RagSocTra;
      rdl.datitrasferimento.Matricola = Matricola;
      rdl.datitrasferimento.Nome = Nome;
      rdl.datitrasferimento.Cognome = Cognome;
      rdl.datitrasferimento.ProRap = ProRap;
      rdl.datitrasferimento.ProRapTra = ProRapTra;
      rdl.datitrasferimento.ProTraRap = ProTraRap;
      rdl.datitrasferimento.Gruppo = Gruppo;
      rdl.datitrasferimento.ProSos = ProSos;
      rdl.datitrasferimento.DatAnn = DatAnn;
      try
      {
        this.objDataAccess.StartTransaction();
        if (this.Check_Input(rdl, ref ErrorMSG))
        {
          if (str1.ToString() != rdl.datitrasferimento.Matricola)
          {
            num = 0;
            DT.Clear();
            DT = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0000", 0, Convert.ToInt32(rdl.datitrasferimento.Matricola), 0, 0, "", "", "", "", "", "", 0, 0, 0, "", "ANNULLAMENTO TRASFERIMENTO", "", "");
            idocRdl.WRITE_IDOC_TESTATA(this.objDataAccess, DT.Rows[0]);
            flag1 = true;
            string strSQL = " SELECT VALUE(COUNT(*), 0) AS CONTA_RIGHE FROM TRARAP WHERE ";
            if (!string.IsNullOrEmpty(rdl.datitrasferimento.CodPos))
              strSQL = strSQL + " CODPOS = " + rdl.datitrasferimento.CodPos;
            if (!string.IsNullOrEmpty(rdl.datitrasferimento.Matricola))
              strSQL = strSQL + " AND MAT = " + rdl.datitrasferimento.Matricola;
            if (!string.IsNullOrEmpty(rdl.datitrasferimento.ProRap))
              strSQL = strSQL + " AND PRORAP = " + rdl.datitrasferimento.ProRap;
            if (!string.IsNullOrEmpty(rdl.datitrasferimento.Gruppo))
              strSQL = strSQL + " AND GRUPPO =" + Gruppo;
            dataTable1 = this.objDataAccess.GetDataTable(strSQL);
          }
          if (str1.ToString() != rdl.datitrasferimento.Matricola)
          {
            DT.Clear();
            DT = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0000", 0, Convert.ToInt32(rdl.datitrasferimento.Matricola), 0, 0, "AT", "01", "9999-12-31", rdl.datitrasferimento.DatTra, "", "", 0, 0, 0, "", "ANNULLAMENTO TRASFERIMENTO", "", "", "AT");
            idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0000", DT, true);
            flag2 = true;
          }
          string strSQL1 = "SELECT PROSOS FROM SOSRAP " + " WHERE CODPOS = " + rdl.datitrasferimento.CodPos + " AND MAT = " + rdl.datitrasferimento.Matricola + " AND PRORAP = " + rdl.datitrasferimento.ProRapTra + " AND VALUE(STASOS, 0) = 0";
          dataTable2.Clear();
          DataTable dataTable3 = this.objDataAccess.GetDataTable(strSQL1);
          string str2 = this.objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text);
          for (int index = 0; index <= dataTable3.Rows.Count - 1; ++index)
          {
            DT.Clear();
            DT = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(rdl.datitrasferimento.CodPos), Convert.ToInt32(rdl.datitrasferimento.Matricola), Convert.ToInt32(rdl.datitrasferimento.ProRapTra), Convert.ToInt32(str2), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
            idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", DT, true);
          }
          string strSQL2 = "SELECT PROSOS FROM SOSRAP " + " WHERE CODPOS = " + rdl.datitrasferimento.CodPos + " AND MAT = " + rdl.datitrasferimento.Matricola + " AND PRORAP = " + rdl.datitrasferimento.ProRap + " AND VALUE(STASOS, 0) = 0";
          dataTable3.Clear();
          DataTable dataTable4 = this.objDataAccess.GetDataTable(strSQL2);
          string str3 = this.objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text);
          for (int index = 0; index <= dataTable4.Rows.Count - 1; ++index)
          {
            DT.Clear();
            DT = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(rdl.datitrasferimento.CodPos), Convert.ToInt32(rdl.datitrasferimento.Matricola), Convert.ToInt32(rdl.datitrasferimento.ProRap), Convert.ToInt32(str3), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
            idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", DT, true);
          }
          if (!this.Module_Annullamento_Trasferimento(this.objDataAccess, u, Convert.ToInt32(rdl.datitrasferimento.CodPos), Convert.ToInt32(rdl.datitrasferimento.Matricola), Convert.ToInt32(rdl.datitrasferimento.ProRapTra), Convert.ToInt32(rdl.datitrasferimento.CodPosTra), Convert.ToInt32(rdl.datitrasferimento.ProRap), Convert.ToInt32(rdl.datitrasferimento.ProTraRap), rdl.datitrasferimento.DatTra, ref ErrorMSG))
          {
            this.objDataAccess.EndTransaction(false);
          }
          else
          {
            string matricola = rdl.datitrasferimento.Matricola;
            if (Gruppo == dataTable1.Rows[0]["CONTA_RIGHE"].ToString())
            {
              DT.Clear();
              DataTable idocDatiE1Pitype = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0016", 0, Convert.ToInt32(rdl.datitrasferimento.Matricola), 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "ANNULLAMENTO TRASFERIMENTO", "", "", "T1");
              idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "9001", idocDatiE1Pitype, false);
              idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0016", idocDatiE1Pitype, false);
              string strSQL3 = "SELECT PROSOS FROM SOSRAP " + " WHERE CODPOS = " + rdl.datitrasferimento.CodPos + " AND MAT = " + rdl.datitrasferimento.Matricola + " AND PRORAP = " + rdl.datitrasferimento.ProRapTra + " AND VALUE(STASOS, 0) = 0";
              dataTable4.Clear();
              DataTable dataTable5 = this.objDataAccess.GetDataTable(strSQL3);
              string str4 = this.objDataAccess.Get1ValueFromSQL(strSQL3, CommandType.Text);
              for (int index = 0; index <= dataTable5.Rows.Count - 1; ++index)
              {
                idocDatiE1Pitype.Clear();
                idocDatiE1Pitype = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(rdl.datitrasferimento.CodPos), Convert.ToInt32(rdl.datitrasferimento.Matricola), Convert.ToInt32(rdl.datitrasferimento.ProRapTra), Convert.ToInt32(str4), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype, true);
              }
              string strSQL4 = "SELECT PROSOS FROM SOSRAP " + " WHERE CODPOS = " + rdl.datitrasferimento.CodPos + " AND MAT = " + rdl.datitrasferimento.Matricola + " AND PRORAP = " + rdl.datitrasferimento.ProRap + " AND VALUE(STASOS, 0) = 0";
              dataTable5.Clear();
              DataTable dataTable6 = this.objDataAccess.GetDataTable(strSQL4);
              string str5 = this.objDataAccess.Get1ValueFromSQL(strSQL4, CommandType.Text);
              for (int index = 0; index <= dataTable6.Rows.Count - 1; ++index)
              {
                idocDatiE1Pitype.Clear();
                idocDatiE1Pitype = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(rdl.datitrasferimento.CodPos), Convert.ToInt32(rdl.datitrasferimento.Matricola), Convert.ToInt32(rdl.datitrasferimento.ProRapTra), Convert.ToInt32(str5), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype, true);
              }
            }
            idocRdl.Aggiorna_IDOC(this.objDataAccess);
            SuccessMSG = "Annullamento del trasferimento effettuato con successo";
            this.objDataAccess.EndTransaction(true);
          }
        }
        else
          this.objDataAccess.EndTransaction(false);
      }
      catch (Exception ex)
      {
        ErrorMSG = "Annullamento fallito riprovare";
        this.objDataAccess.EndTransaction(false);
      }
    }

    public bool Module_Autorizzato(string CodFunsis, ref string ErrorMSG)
    {
      DataTable dataTable = new DataTable();
      int index = 0;
      while (index <= dataTable.Rows.Count - 1 && !(dataTable.Rows[index]["CODFUNSIS"].ToString() == CodFunsis))
        ++index;
      if (index <= dataTable.Rows.Count - 1)
        return true;
      ErrorMSG = "L'utente non è abilitato a questa funzione";
      return false;
    }

    public bool Module_Annullamento_Trasferimento(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int CODPOS_PARTENZA,
      int MAT_DESTINAZIONE,
      int PRORAP_DESTINAZIONE,
      int CODPOS_DESTINAZIONE,
      int PRORAP_PARTENZA,
      int PROTRARAP,
      string DATTRA,
      ref string ErrorMSG)
    {
      string[] strArray = new string[5]
      {
        "RAPLAV",
        "STORDL",
        "IMPAGG",
        "PARTIMM",
        "SOSRAP"
      };
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      bool flag = false;
      int num1 = 0;
      int num2 = 0;
      try
      {
        string strSQL1 = "SELECT VALUE(ESIPRITRA,'N') AS ESIPRITRA FROM RAPLAV" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString();
        if (objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text) == "S")
        {
          string strSQL2 = "UPDATE RAPLAV SET CODPOSDA = NULL, CODPOSA = NULL, ESIPRITRA=NULL " + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString();
          objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
        }
        else
        {
          for (int index = strArray.Length - 1; index >= 1; index += -1)
          {
            string strSQL3 = "DELETE FROM " + strArray[index] + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString();
            objDataAccess.WriteTransactionData(strSQL3, CommandType.Text);
          }
          string strSQL4 = " SELECT (IMPRET + IMPOCC + IMPFIG + IMPASSCON + IMPABB) AS TOT, ANNDEN, MESDEN, PRODEN, PRODENDET, TIPMOV FROM DENDET " + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString() + " ORDER BY MESDEN ASC, PRODENDET DESC";
          dataTable2.Clear();
          DataTable dataTable3 = objDataAccess.GetDataTable(strSQL4);
          if (dataTable3.Rows.Count > 0)
          {
            for (int index = 0; index <= dataTable3.Rows.Count - 1; ++index)
            {
              if (Convert.ToInt32(dataTable3.Rows[index]["TOT"]) == 0)
              {
                if (dataTable3.Rows[index]["TIPMOV"].ToString() == "RT")
                {
                  flag = true;
                  num1 = Convert.ToInt32(dataTable3.Rows[index]["MESDEN"]);
                  num2 = Convert.ToInt32(dataTable3.Rows[index]["ANNDEN"]);
                }
                else
                  flag = false;
                string strSQL5 = "DELETE FROM DENDETALI" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND ANNDEN = " + dataTable3.Rows[index]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable3.Rows[index]["MESDEN"]?.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRODEN = " + dataTable3.Rows[index]["PRODEN"]?.ToString() + " AND PRODENDET = " + dataTable3.Rows[index]["PRODENDET"]?.ToString();
                objDataAccess.WriteTransactionData(strSQL5, CommandType.Text);
                string strSQL6 = "DELETE FROM DENDETSOS" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND ANNDEN = " + dataTable3.Rows[index]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable3.Rows[index]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable3.Rows[index]["PRODEN"]?.ToString() + " AND PRODENDET = " + dataTable3.Rows[index]["PRODENDET"]?.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString();
                objDataAccess.WriteTransactionData(strSQL6, CommandType.Text);
                string strSQL7 = "DELETE FROM DENDET" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND ANNDEN = " + dataTable3.Rows[index]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable3.Rows[index]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable3.Rows[index]["PRODEN"]?.ToString() + " AND PRODENDET = " + dataTable3.Rows[index]["PRODENDET"]?.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString();
                objDataAccess.WriteTransactionData(strSQL7, CommandType.Text);
              }
              else if (flag && num1 == Convert.ToInt32(dataTable3.Rows[index]["MESDEN"]) & num2 == Convert.ToInt32(dataTable3.Rows[index]["ANNDEN"]))
              {
                string strSQL8 = "DELETE FROM DENDETALI" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND ANNDEN = " + dataTable3.Rows[index]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable3.Rows[index]["MESDEN"]?.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRODEN = " + dataTable3.Rows[index]["PRODEN"]?.ToString() + " AND PRODENDET = " + dataTable3.Rows[index]["PRODENDET"]?.ToString();
                objDataAccess.WriteTransactionData(strSQL8, CommandType.Text);
                string strSQL9 = "DELETE FROM DENDETSOS" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND ANNDEN = " + dataTable3.Rows[index]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable3.Rows[index]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable3.Rows[index]["PRODEN"]?.ToString() + " AND PRODENDET = " + dataTable3.Rows[index]["PRODENDET"]?.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString();
                objDataAccess.WriteTransactionData(strSQL9, CommandType.Text);
                string strSQL10 = "DELETE FROM DENDET" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND ANNDEN = " + dataTable3.Rows[index]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable3.Rows[index]["MESDEN"]?.ToString() + " AND PRODEN = " + dataTable3.Rows[index]["PRODEN"]?.ToString() + " AND PRODENDET = " + dataTable3.Rows[index]["PRODENDET"]?.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString();
                objDataAccess.WriteTransactionData(strSQL10, CommandType.Text);
              }
            }
          }
          for (int index = 0; index <= dataTable3.Rows.Count - 1; ++index)
          {
            string strSQL11 = "DELETE FROM DENDETALI" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND ANNDEN = " + dataTable3.Rows[index]["ANNDEN"]?.ToString() + " AND MESDEN = " + dataTable3.Rows[index]["MESDEN"]?.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRODEN = " + dataTable3.Rows[index]["PRODEN"]?.ToString() + " AND PRODENDET = " + dataTable3.Rows[index]["PRODENDET"]?.ToString();
            objDataAccess.WriteTransactionData(strSQL11, CommandType.Text);
            string strSQL12 = "DELETE FROM DENDET" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString();
            objDataAccess.WriteTransactionData(strSQL12, CommandType.Text);
          }
          string strSQL13 = " SELECT CODPOS, ANNDEN, MESDEN, MAT FROM DENDET " + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString() + " AND DATMOVANN IS NULL";
          dataTable3.Clear();
          DataTable dataTable4 = objDataAccess.GetDataTable(strSQL13);
          for (int index = 0; index <= dataTable4.Rows.Count - 1; ++index)
          {
            if (index == 0)
              ErrorMSG += "Prima di annullare il trasferimento, rettificare a 0 i DIPA";
          }
          if (ErrorMSG != "")
            return false;
          string strSQL14 = "DELETE FROM MODRDL" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString();
          objDataAccess.WriteTransactionData(strSQL14, CommandType.Text);
          string strSQL15 = "DELETE FROM TRARAP" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString();
          objDataAccess.WriteTransactionData(strSQL15, CommandType.Text);
          string strSQL16 = "DELETE FROM DENDETALI WHERE CODPOS || MAT || ANNDEN || MESDEN || PRODEN || PRODENDET IN (SELECT CODPOS || MAT || ANNDEN || MESDEN || PRODEN || PRODENDET  FROM DENDET" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString() + " AND NUMMOVANN IS NOT NULL)";
          objDataAccess.WriteTransactionData(strSQL16, CommandType.Text);
          string strSQL17 = "DELETE FROM DENDETSOS WHERE CODPOS || MAT || ANNDEN || MESDEN || PRODEN || PRODENDET IN (SELECT CODPOS || MAT || ANNDEN || MESDEN || PRODEN || PRODENDET  FROM DENDET" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString() + " AND NUMMOVANN IS NOT NULL)";
          objDataAccess.WriteTransactionData(strSQL17, CommandType.Text);
          string strSQL18 = "DELETE FROM DENDET" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString() + " AND NUMMOVANN IS NOT NULL";
          objDataAccess.WriteTransactionData(strSQL18, CommandType.Text);
          string strSQL19 = "DELETE FROM RAPLAV" + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_PARTENZA.ToString();
          objDataAccess.WriteTransactionData(strSQL19, CommandType.Text);
        }
        for (int index = strArray.Length - 1; index >= 1; index += -1)
        {
          string strSQL20 = "DELETE FROM " + strArray[index] + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_DESTINAZIONE.ToString();
          objDataAccess.WriteTransactionData(strSQL20, CommandType.Text);
        }
        string strSQL21 = "UPDATE RAPLAV SET CODCAUCES = NULL, DATCES = NULL, CODPOSDA = NULL, CODPOSA = NULL, ESIPRITRA=NULL " + " WHERE CODPOS = " + CODPOS_PARTENZA.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_DESTINAZIONE.ToString();
        objDataAccess.WriteTransactionData(strSQL21, CommandType.Text);
        for (int index1 = 1; index1 <= strArray.Length - 1; ++index1)
        {
          string str1 = "";
          string str2 = "";
          string strSQL22 = "SELECT * FROM BK" + strArray[index1] + " WHERE CODPOS = " + CODPOS_DESTINAZIONE.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " And PRORAP = " + PRORAP_PARTENZA.ToString() + " And PROTRARAP = " + PROTRARAP.ToString();
          DataTable dataTable5 = objDataAccess.GetDataTable(strSQL22);
          string str3 = "INSERT INTO " + strArray[index1] + " ( ";
          for (int index2 = 0; index2 <= dataTable5.Columns.Count - 1; ++index2)
          {
            if (dataTable5.Columns[index2].ColumnName != nameof (PROTRARAP))
              str1 = str1 + ", " + dataTable5.Columns[index2].ColumnName;
          }
          string str4 = str1.Substring(1);
          string str5 = str3 + str4 + " ) " + " SELECT ";
          for (int index3 = 0; index3 <= dataTable5.Columns.Count - 1; ++index3)
          {
            if (!(dataTable5.Columns[index3].ColumnName == nameof (PROTRARAP)))
              str2 = str2 + ", " + dataTable5.Columns[index3].ColumnName;
          }
          string str6 = str2.Substring(1);
          string strSQL23 = str5 + str6 + " FROM BK" + strArray[index1] + " WHERE CODPOS = " + CODPOS_DESTINAZIONE.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_DESTINAZIONE.ToString() + " AND PROTRARAP = " + PROTRARAP.ToString();
          objDataAccess.WriteTransactionData(strSQL23, CommandType.Text);
        }
        for (int index = 0; index <= strArray.Length - 1; ++index)
        {
          string strSQL24 = "DELETE FROM BK" + strArray[index] + " WHERE CODPOS = " + CODPOS_DESTINAZIONE.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_DESTINAZIONE.ToString() + " AND PROTRARAP = " + PROTRARAP.ToString();
          objDataAccess.WriteTransactionData(strSQL24, CommandType.Text);
        }
        string strSQL25 = "UPDATE TRARAP SET DATANN = CURRENT_DATE, ULTAGG = CURRENT_TIMESTAMP, UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + " WHERE CODPOS = " + CODPOS_DESTINAZIONE.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_DESTINAZIONE.ToString() + " AND PROTRARAP = " + PROTRARAP.ToString();
        objDataAccess.WriteTransactionData(strSQL25, CommandType.Text);
        string strSQL26 = "SELECT PROGTRA FROM RAPLAV" + " WHERE CODPOS = " + CODPOS_DESTINAZIONE.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_DESTINAZIONE.ToString();
        string str = objDataAccess.Get1ValueFromSQL(strSQL26, CommandType.Text);
        string strSQL27 = "SELECT COUNT(*) FROM RAPLAV WHERE MAT = " + MAT_DESTINAZIONE.ToString() + " AND PROGTRA = " + str;
        if (objDataAccess.Get1ValueFromSQL(strSQL27, CommandType.Text) == "0")
        {
          string strSQL28 = "UPDATE RAPLAV SET PROGTRA = NULL WHERE CODPOS = " + CODPOS_DESTINAZIONE.ToString() + " AND MAT = " + MAT_DESTINAZIONE.ToString() + " AND PRORAP = " + PRORAP_DESTINAZIONE.ToString();
          objDataAccess.WriteTransactionData(strSQL28, CommandType.Text);
        }
        return true;
      }
      catch (Exception ex)
      {
        ErrorMSG = "Annullamento Fallito";
        return false;
      }
    }

    public string Module_GetNomeMese(int NUMMESE)
    {
      switch (NUMMESE)
      {
        case 1:
          return "Gennaio";
        case 2:
          return "Febbraio";
        case 3:
          return "Marzo";
        case 4:
          return "Aprile";
        case 5:
          return "Maggio";
        case 6:
          return "Giugno";
        case 7:
          return "Luglio";
        case 8:
          return "Agosto";
        case 9:
          return "Settembre";
        case 10:
          return "Ottobre";
        case 11:
          return "Novembre";
        case 12:
          return "Dicembre";
        default:
          return (string) null;
      }
    }
  }
}
