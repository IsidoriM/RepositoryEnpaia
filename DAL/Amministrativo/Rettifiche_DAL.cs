// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.Rettifiche_DAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Data;
using TFI.DAL.AziendaConsulente;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Amministrativo
{
  public class Rettifiche_DAL
  {
    private DataLayer objDataAccess = new DataLayer();

    public Rettifiche_OCM SearchRett(
      Rettifiche_OCM rettifiche_OCM,
      ref string MSGErorre,
      ref string MSGSuccess,
      TFI.OCM.Utente.Utente u)
    {
      int index1 = 0;
      string str1 = "";
      Utile utile = new Utile();
      string str2 = "";
      if (!utile.Module_Autorizzazioni_Speciali(rettifiche_OCM.searchRett.Codpos.ToString(), "CONTRIBUTI"))
      {
        MSGErorre = "Utente non abilitato";
        return rettifiche_OCM;
      }
      try
      {
        this.objDataAccess.StartTransaction();
        ModGeneraRettifiche generaRettifiche = new ModGeneraRettifiche();
        string MESE_A = "";
        string ANNO_A = "";
        string MAT = "";
        int num = 0;
        string str3 = "";
        string str4 = "";
        if (!string.IsNullOrEmpty(rettifiche_OCM.searchRett.MeseA))
          MESE_A = rettifiche_OCM.searchRett.MeseA.ToString();
        if (!string.IsNullOrEmpty(rettifiche_OCM.searchRett.AnnoA))
          ANNO_A = rettifiche_OCM.searchRett.AnnoA.ToString();
        if (!string.IsNullOrEmpty(rettifiche_OCM.searchRett.Mat))
          MAT = rettifiche_OCM.searchRett.Mat.ToString();
        DataTable dataTable1 = generaRettifiche.Module_GeneraRettifiche(this.objDataAccess, u, Convert.ToInt32(rettifiche_OCM.searchRett.Codpos), (object) rettifiche_OCM.searchRett.MeseDA, (object) MESE_A, MAT, rettifiche_OCM.searchRett.AnnoDA.ToString(), ANNO_A);
        DataTable dataTable2 = dataTable1;
        if (dataTable1 != null)
        {
          if (dataTable1.Rows.Count > 0)
          {
            rettifiche_OCM.searchRett.Occorrenze = dataTable2.Rows.Count.ToString();
            int index2 = 0;
            foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
            {
              if (index2 < dataTable1.Rows.Count)
              {
                string str5 = "";
                switch (Convert.ToInt32(dataTable1.Rows[index2]["MESDEN"].ToString()))
                {
                  case 1:
                    str5 = "Gennaio";
                    break;
                  case 2:
                    str5 = "Febbraio";
                    break;
                  case 3:
                    str5 = "Marzo";
                    break;
                  case 4:
                    str5 = "Aprile";
                    break;
                  case 5:
                    str5 = "Maggio";
                    break;
                  case 6:
                    str5 = "Giugno";
                    break;
                  case 7:
                    str5 = "Luglio";
                    break;
                  case 8:
                    str5 = "Agosto";
                    break;
                  case 9:
                    str5 = "Settembre";
                    break;
                  case 10:
                    str5 = "Ottobre";
                    break;
                  case 11:
                    str5 = "Novembre";
                    break;
                  case 12:
                    str5 = "Dicembre";
                    break;
                }
                if (!string.IsNullOrEmpty(dataTable2.Rows[index2]["IMPSANDET"].ToString()) && Convert.ToInt32(dataTable2.Rows[index2]["IMPSANDET"]) != 0 && dataTable2.Rows[index2]["DATFINSAN"].ToString() != "" && dataTable2.Rows[index2]["DATINISAN"].ToString() != "")
                {
                  rettifiche_OCM.listRett[index2].NumGGRit = Convert.ToDateTime(dataTable2.Rows[index2]["DATFINSAN"].ToString()).Subtract(Convert.ToDateTime(dataTable2.Rows[index2]["DATINISAN"].ToString())).ToString("dd");
                  if (str2 == "SAN_RT_MD")
                  {
                    int int32 = Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(GIORNI, 0) AS GIORNI " + " FROM TIPMOVCAU WHERE TIPMOV = '" + str2 + "' AND '" + DBMethods.Db2Date(dataTable2.Rows[index2]["DAL"].ToString()) + "' BETWEEN DATINI AND DATFIN ", CommandType.Text));
                    num = Convert.ToInt32(dataTable1.Rows[index2]["NUMGGRITARDO"]) + int32;
                  }
                }
                if (dataTable2.Rows[index2]["DATINISAN"].ToString() == "" && !string.IsNullOrEmpty(dataTable2.Rows[index2]["TIPMOV"].ToString()))
                {
                  string str6 = dataTable2.Rows[index2]["TIPMOV"].ToString().Trim();
                  if (!(str6 == "DP"))
                  {
                    if (str6 == "AR" && Convert.ToInt32(dataTable2.Rows[index2]["ANNDEN"]) > 2003)
                      str3 = (Convert.ToInt32(dataTable2.Rows[index2]["ANNCOM"]) + 1).ToString() + "-01-01";
                  }
                  else if (Convert.ToInt32(dataTable2.Rows[index2]["ANNDEN"]) > 2003)
                    str1 = this.objDataAccess.Get1ValueFromSQL("SELECT VALORE FROM PARGENDET WHERE CODPAR = 3" + " AND '" + dataTable2.Rows[index2]["ANNDEN"].ToString() + "-" + dataTable2.Rows[index2]["MESDEN"].ToString().PadLeft(2, '0') + "-01' " + " BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31') ", CommandType.Text).ToString();
                }
                if (string.IsNullOrEmpty(dataTable2.Rows[index1]["ANNCOM"].ToString()))
                  str4 = dataTable2.Rows[index1]["ANNCOM"].ToString();
                Rettifiche_OCM.ListRett listRett1 = new Rettifiche_OCM.ListRett();
                listRett1.Codpos = row["CODPOS"].ToString();
                listRett1.AnnoDen = row["ANNDEN"].ToString();
                listRett1.MeseDen = str5;
                listRett1.Mat = row["MAT"].ToString();
                listRett1.Proden = row["PRODEN"].ToString();
                listRett1.Prodendet = row["PRODENDET"].ToString();
                listRett1.TipMov = row["TIPMOV"].ToString();
                listRett1.NumGGRit = num.ToString();
                listRett1.DatIniSanz = str3;
                listRett1.DatFinSanz = str1;
                listRett1.Retribuzione = row["IMPRET"].ToString();
                listRett1.Occ = row["IMPOCC"].ToString();
                listRett1.Fig = row["IMPFIG"].ToString();
                listRett1.TassoSanz = row["TASSAN"].ToString();
                DateTime dateTime = Convert.ToDateTime(row["DAL"]);
                listRett1.Dal = dateTime.ToString("dd-MM-yyyy");
                dateTime = Convert.ToDateTime(row["AL"]);
                listRett1.Al = dateTime.ToString("dd-MM-yyyy");
                listRett1.Prorap = row["PRORAP"].ToString();
                listRett1.CodContr = row["CODCON"].ToString();
                listRett1.Procon = row["PROCON"].ToString();
                listRett1.CodLiv = row["CODLIV"].ToString();
                listRett1.CodQua = row["CODQUACON"].ToString();
                listRett1.Codgruass = row["CODGRUASS"].ToString();
                listRett1.CodquaCon = row["CODQUACON"].ToString();
                listRett1.Aliq = row["ALIQUOTA"].ToString();
                listRett1.FAP = row["FAP"].ToString();
                listRett1.Perfap = row["PERFAP"].ToString();
                listRett1.Contributo = row["IMPCON"].ToString();
                listRett1.OccPre = row["IMPOCCPRE"].ToString();
                listRett1.FigPre = row["IMPFIGPRE"].ToString();
                listRett1.RetPre = row["IMPRETPRE"].ToString();
                listRett1.ContributoPre = row["IMPCONPRE"].ToString();
                listRett1.Sanz = row["IMPSANDET"].ToString();
                listRett1.SanzPre = row["IMPSANDETPRE"].ToString();
                listRett1.Abb = row["IMPABB"].ToString();
                listRett1.AbbPre = row["IMPABBPRE"].ToString();
                listRett1.AssCon = row["IMPASSCON"].ToString();
                listRett1.AssConPre = row["IMPASSCONPRE"].ToString();
                listRett1.AnnoComp = str4;
                listRett1.Impsca = row["IMPSCA"].ToString();
                listRett1.CodLoc = row["CODLOC"].ToString();
                listRett1.ProLoc = row["PROLOC"].ToString();
                listRett1.DatCes = row["DATCES"].ToString();
                listRett1.DatDec = row["DATDEC"].ToString();
                listRett1.DatNas = row["DATNAS"].ToString();
                listRett1.Eta65 = row["ETA65"].ToString();
                listRett1.Sos = row["SOS"].ToString();
                listRett1.ImpAbbDel = row["IMPABBDEL"].ToString();
                listRett1.ImpAssConDel = row["IMPASSCONDEL"].ToString();
                listRett1.Prev = row["PREV"].ToString();
                listRett1.ImpConDel = row["IMPCONDEL"].ToString();
                listRett1.ImpFigDel = row["IMPFIGDEL"].ToString();
                listRett1.ImpOccDel = row["IMPOCCDEL"].ToString();
                listRett1.Min = row["IMPMIN"].ToString();
                listRett1.ImpRetDel = row["IMPRETDEL"].ToString();
                listRett1.NumGgAzi = row["NUMGGAZI"].ToString();
                listRett1.ImpSanDet = row["IMPSANDET"].ToString();
                listRett1.ImpTraEco = row["IMPTRAECO"].ToString();
                listRett1.NumGgConAzi = row["NUMGGCONAZI"].ToString();
                listRett1.NumGgDom = row["NUMGGDOM"].ToString();
                listRett1.NumGgFig = row["NUMGGFIG"].ToString();
                listRett1.NumGgPer = row["NUMGGPER"].ToString();
                listRett1.NumGgSos = row["NUMGGSOS"].ToString();
                listRett1.NumMov = row["NUMMOV"].ToString();
                listRett1.PerApp = row["PERAPP"].ToString();
                listRett1.PerPar = row["PERPAR"].ToString();
                listRett1.TipDen = row["TIPDEN"].ToString();
                listRett1.TipMovDenTes = row["TIPMOV_DENTES"].ToString();
                listRett1.TipRap = row["TIPRAP"].ToString();
                listRett1.TipSpe = row["TIPSPE"].ToString();
                listRett1.RetDiff = row["IMPRET"].ToString();
                listRett1.OccDiff = row["IMPFIG"].ToString();
                listRett1.FigDiff = row["IMPFIG"].ToString();
                Rettifiche_OCM.ListRett listRett2 = listRett1;
                rettifiche_OCM.listRett.Add(listRett2);
                ++index2;
                str3 = "";
                str1 = "";
              }
            }
          }
        }
        else
          MSGErorre = "Nessun risultato trovato per la ricerca";
        this.objDataAccess.EndTransaction(true);
        return rettifiche_OCM;
      }
      catch (Exception ex)
      {
        this.objDataAccess.EndTransaction(false);
        MSGErorre = "Nessun risultato per la ricerca ";
        return (Rettifiche_OCM) null;
      }
    }

    public void SalvaRett(
      TFI.OCM.Utente.Utente u,
      Rettifiche_OCM rettifiche_OCM,
      ref string ErrorMSG,
      ref string SuccessMSG)
    {
      int num1 = 0;
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      bool flag1 = false;
      bool flag2 = true;
      Decimal IMPRETPRE = 0.0M;
      Decimal IMPOCCPRE = 0.0M;
      Decimal IMPFIGPRE = 0.0M;
      Decimal IMPCONPRE = 0.0M;
      Decimal IMPSANDETPRE = 0.0M;
      Decimal IMPABBPRE = 0.0M;
      Decimal IMPASSCONPRE = 0.0M;
      string CODCAUSAN = "";
      string str1 = "0";
      string str2 = "";
      DataTable DT = new DataTable();
      Decimal PERMAXSOGLIA = 0.0M;
      string TIPMOVSAN = "SAN_RT_RD";
      bool flag3 = true;
      string str3 = "";
      Utile utile = new Utile();
      clsIDOC clsIdoc = new clsIDOC();
      ClsProtocolloDll clsProtocolloDll = new ClsProtocolloDll();
      try
      {
        this.objDataAccess.StartTransaction();
        if (flag3)
          TIPMOVSAN = "";
        num1 = 0;
        for (int index = 0; index <= rettifiche_OCM.listRett.Count - 1; ++index)
        {
          switch (rettifiche_OCM.listRett[index].MeseDen)
          {
            case "Agosto":
              rettifiche_OCM.listRett[index].MeseDen = "8";
              break;
            case "Aprile":
              rettifiche_OCM.listRett[index].MeseDen = "4";
              break;
            case "Dicembre":
              rettifiche_OCM.listRett[index].MeseDen = "12";
              break;
            case "Febbraio":
              rettifiche_OCM.listRett[index].MeseDen = "2";
              break;
            case "Gennaio":
              rettifiche_OCM.listRett[index].MeseDen = "1";
              break;
            case "Giugno":
              rettifiche_OCM.listRett[index].MeseDen = "6";
              break;
            case "Luglio":
              rettifiche_OCM.listRett[index].MeseDen = "7";
              break;
            case "Maggio":
              rettifiche_OCM.listRett[index].MeseDen = "5";
              break;
            case "Marzo":
              rettifiche_OCM.listRett[index].MeseDen = "3";
              break;
            case "Novembre":
              rettifiche_OCM.listRett[index].MeseDen = "11";
              break;
            case "Ottobre":
              rettifiche_OCM.listRett[index].MeseDen = "10";
              break;
            case "Settembre":
              rettifiche_OCM.listRett[index].MeseDen = "9";
              break;
          }
          Decimal TASSO = 0.0M;
          if (string.IsNullOrEmpty(rettifiche_OCM.listRett[index].Retribuzione))
            rettifiche_OCM.listRett[index].Retribuzione = "0,00";
          if (string.IsNullOrEmpty(rettifiche_OCM.listRett[index].Occ))
            rettifiche_OCM.listRett[index].Occ = "0,00";
          if (string.IsNullOrEmpty(rettifiche_OCM.listRett[index].Fig))
            rettifiche_OCM.listRett[index].Fig = "0,00";
          if (this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM DENDET WHERE" + " CODPOS = '" + rettifiche_OCM.listRett[index].Codpos + "'" + " AND MAT = '" + rettifiche_OCM.listRett[index].Mat + "'" + " AND PRORAP = '" + rettifiche_OCM.listRett[index].Prorap + "'" + " AND ANNDEN = '" + rettifiche_OCM.listRett[index].AnnoDen + "'" + " AND MESDEN  = '" + rettifiche_OCM.listRett[index].MeseDen + "'" + " AND TIPMOV <> 'RT'", CommandType.Text) == "0" || rettifiche_OCM.listRett[index].OccDiff.ToString() != rettifiche_OCM.listRett[index].Occ.ToString() || rettifiche_OCM.listRett[index].FigDiff.ToString() != rettifiche_OCM.listRett[index].Fig.ToString() || rettifiche_OCM.listRett[index].RetDiff.ToString() != rettifiche_OCM.listRett[index].Retribuzione.ToString())
          {
            if (Convert.ToInt32(rettifiche_OCM.listRett[index].AnnoDen) < 2003)
            {
              this.objDataAccess.EndTransaction(false);
              ErrorMSG = "Attenzione... per le denunce ante 2003 utilizzare la funzione presente nei Rapporti di Lavoro";
              return;
            }
            flag1 = true;
            string str4 = this.objDataAccess.Get1ValueFromSQL(" SELECT VALUE(CHAR(DATCES),'') FROM RAPLAV WHERE" + " CODPOS = '" + rettifiche_OCM.listRett[index].Codpos + "'" + " AND MAT = '" + rettifiche_OCM.listRett[index].Mat + "'" + " AND PRORAP = '" + rettifiche_OCM.listRett[index].Prorap + "'", CommandType.Text);
            int num2;
            if (str3 != "PREV" && str4.Trim() != "")
            {
              DateTime dateTime = Convert.ToDateTime(str4);
              string strSQL;
              if (rettifiche_OCM.listRett[index].TipMov == "AR")
              {
                string str5 = " SELECT VALUE(COUNT(*),0) FROM MODPREDET, MODPRE WHERE " + " MODPREDET.CODPOS = MODPRE.CODPOS" + " AND MODPREDET.MAT = MODPRE.MAT" + " AND MODPREDET.PRORAP = MODPRE.PRORAP" + " AND MODPREDET.PROMOD = MODPRE.PROMOD" + " AND MODPREDET.CODPOS = '" + rettifiche_OCM.listRett[index].Codpos + "'" + " AND MODPREDET.MAT = '" + rettifiche_OCM.listRett[index].Mat + "'" + " AND MODPREDET.PRORAP = '" + rettifiche_OCM.listRett[index].Prorap + "'";
                num2 = Convert.ToDateTime(str4).Year;
                string str6 = num2.ToString();
                string str7 = str5 + " AND ( MODPREDET.ANNCOM  = '" + str6 + "'";
                num2 = dateTime.AddYears(-1).Year;
                string str8 = num2.ToString();
                strSQL = str7 + " OR MODPREDET.ANNCOM  = '" + str8 + "')" + " AND MODPRE.CODSTAPRE = 0";
              }
              else
              {
                string str9 = " SELECT VALUE(COUNT(*),0) FROM MODPREDET, MODPRE WHERE " + " MODPREDET.CODPOS = MODPRE.CODPOS" + " AND MODPREDET.MAT = MODPRE.MAT" + " AND MODPREDET.PRORAP = MODPRE.PRORAP" + " AND MODPREDET.PROMOD = MODPRE.PROMOD" + " AND MODPREDET.CODPOS = '" + rettifiche_OCM.listRett[index].Codpos + "'" + " AND MODPREDET.MAT = '" + rettifiche_OCM.listRett[index].Mat + "'" + " AND MODPREDET.PRORAP = '" + rettifiche_OCM.listRett[index].Prorap + "'";
                num2 = Convert.ToDateTime(str4).Year;
                string str10 = num2.ToString();
                string str11 = str9 + " AND ( MODPREDET.ANNDEN  = '" + str10 + "'";
                num2 = dateTime.AddYears(-1).Year;
                string str12 = num2.ToString();
                strSQL = str11 + " OR MODPREDET.ANNCOM  = '" + str12 + "')" + " AND MODPRE.CODSTAPRE = 0";
              }
              if (Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text)) > 0)
              {
                ErrorMSG = "Attenzione per la matricola " + rettifiche_OCM.listRett[index].Mat + " esiste un Prev da trasmettere. Impossibile continuare";
                this.objDataAccess.EndTransaction(false);
                return;
              }
              str1 = "0";
              str2 = "";
            }
            if (rettifiche_OCM.listRett[index].Aliq == "0")
            {
              this.objDataAccess.EndTransaction(false);
              ErrorMSG = "Attenzione... nella denuncia non è presente la percentuale dell'aliquota";
              return;
            }
            if (Convert.ToDecimal(rettifiche_OCM.listRett[index].Retribuzione) < 0M)
            {
              this.objDataAccess.EndTransaction(false);
              ErrorMSG = "Inserire una retribuzione positiva";
              return;
            }
            if (Convert.ToDecimal(rettifiche_OCM.listRett[index].Occ) < 0M)
            {
              this.objDataAccess.EndTransaction(false);
              ErrorMSG = "Inserire un importo occasionale positivo";
              return;
            }
            if (Convert.ToDecimal(rettifiche_OCM.listRett[index].Occ) > Convert.ToDecimal(rettifiche_OCM.listRett[index].Retribuzione))
            {
              this.objDataAccess.EndTransaction(false);
              ErrorMSG = "L'importo occasionale non può essere maggiore della retribuzione";
              return;
            }
            if (Convert.ToDecimal(rettifiche_OCM.listRett[index].Fig) < 0M)
            {
              this.objDataAccess.EndTransaction(false);
              ErrorMSG = "Inserire una figurativa positiva";
              return;
            }
            if (Convert.ToDecimal(rettifiche_OCM.listRett[index].Fig) > 0M && Convert.ToDecimal(rettifiche_OCM.listRett[index].Sos) == 0M)
            {
              this.objDataAccess.EndTransaction(false);
              ErrorMSG = "Non esistono sospensioni che prevedono la figurativa per questo periodo";
              return;
            }
            if (string.IsNullOrEmpty(rettifiche_OCM.listRett[index].DatIniSanz.ToString()))
            {
              if (!flag3)
              {
                this.objDataAccess.EndTransaction(false);
                ErrorMSG = "Inserire la data di inizio sanzione";
                return;
              }
            }
            else if (rettifiche_OCM.listRett[index].DatIniSanz.ToString().Substring(0, 10) == "01/01/0001" && !flag3)
            {
              this.objDataAccess.EndTransaction(false);
              ErrorMSG = "Inserire la data di inizio sanzione";
              return;
            }
            if (string.IsNullOrEmpty(rettifiche_OCM.listRett[index].DatFinSanz))
            {
              if (!flag3)
              {
                this.objDataAccess.EndTransaction(false);
                ErrorMSG = "Inserire la data di fine sanzione";
                return;
              }
            }
            else if (rettifiche_OCM.listRett[index].DatFinSanz.ToString().Substring(0, 10) == "01/01/0001" && !flag3)
            {
              this.objDataAccess.EndTransaction(false);
              ErrorMSG = "Inserire la data di fine sanzione";
              return;
            }
            if (Convert.ToInt32(rettifiche_OCM.listRett[index].Retribuzione) != 0)
            {
              int length = rettifiche_OCM.listRett[index].Retribuzione.LastIndexOf(",");
              if (length > -1)
              {
                if (Convert.ToDecimal(rettifiche_OCM.listRett[index].Retribuzione.Substring(length + 1)) < 50M)
                {
                  rettifiche_OCM.listRett[index].Retribuzione = rettifiche_OCM.listRett[index].Retribuzione.Substring(0, length);
                }
                else
                {
                  Rettifiche_OCM.ListRett listRett = rettifiche_OCM.listRett[index];
                  string str13 = rettifiche_OCM.listRett[index].Retribuzione.Substring(0, length);
                  num2 = 1;
                  string str14 = num2.ToString();
                  string str15 = str13 + str14;
                  listRett.Retribuzione = str15;
                }
              }
            }
            Rettifiche_OCM.ListRett listRett1 = rettifiche_OCM.listRett[index];
            Decimal num3 = Convert.ToDecimal(rettifiche_OCM.listRett[index].Retribuzione) - IMPRETPRE;
            string str16 = num3.ToString();
            listRett1.ImpRetDel = str16;
            Rettifiche_OCM.ListRett listRett2 = rettifiche_OCM.listRett[index];
            num3 = Convert.ToDecimal(rettifiche_OCM.listRett[index].Occ) - IMPOCCPRE;
            string str17 = num3.ToString();
            listRett2.ImpOccDel = str17;
            Rettifiche_OCM.ListRett listRett3 = rettifiche_OCM.listRett[index];
            num3 = Convert.ToDecimal(rettifiche_OCM.listRett[index].Fig) - IMPFIGPRE;
            string str18 = num3.ToString();
            listRett3.ImpFigDel = str18;
            Rettifiche_OCM.ListRett listRett4 = rettifiche_OCM.listRett[index];
            num3 = Convert.ToDecimal(rettifiche_OCM.listRett[index].Retribuzione) * Convert.ToDecimal(rettifiche_OCM.listRett[index].Aliq) / 100M;
            string str19 = num3.ToString();
            listRett4.ImpCon = str19;
            Rettifiche_OCM.ListRett listRett5 = rettifiche_OCM.listRett[index];
            num3 = Math.Round(Convert.ToDecimal(rettifiche_OCM.listRett[index].ImpRetDel) * Convert.ToDecimal(rettifiche_OCM.listRett[index].Aliq) / 100M, 2);
            string str20 = num3.ToString();
            listRett5.ImpConDel = str20;
            Rettifiche_OCM.ListRett listRett6 = rettifiche_OCM.listRett[index];
            num3 = Convert.ToDecimal(rettifiche_OCM.listRett[index].Abb) - IMPABBPRE;
            string str21 = num3.ToString();
            listRett6.ImpAbbDel = str21;
            Rettifiche_OCM.ListRett listRett7 = rettifiche_OCM.listRett[index];
            num3 = Convert.ToDecimal(rettifiche_OCM.listRett[index].AssCon) - IMPASSCONPRE;
            string str22 = num3.ToString();
            listRett7.ImpAssConDel = str22;
            if (Convert.ToDecimal(rettifiche_OCM.listRett[index].ImpRetDel) > 0M)
            {
              if (!flag3)
              {
                Rettifiche_OCM.ListRett listRett8 = rettifiche_OCM.listRett[index];
                num3 = this.MODULE_GENERA_SANZIONE(this.objDataAccess, Convert.ToDecimal(rettifiche_OCM.listRett[index].ImpRetDel), ref TASSO, Convert.ToDecimal(rettifiche_OCM.listRett[index].Aliq), TIPMOVSAN, rettifiche_OCM.listRett[index].DatIniSanz, rettifiche_OCM.listRett[index].DatFinSanz, CODCAUSAN, rettifiche_OCM.listRett[index].Dal, PERMAXSOGLIA: PERMAXSOGLIA);
                string str23 = num3.ToString();
                listRett8.ImpSanDet = str23;
                if (PERMAXSOGLIA > 0M && Convert.ToDecimal(Convert.ToDecimal(rettifiche_OCM.listRett[index].ImpConDel) * PERMAXSOGLIA) / 100M < Convert.ToDecimal(rettifiche_OCM.listRett[index].ImpSanDet))
                {
                  Rettifiche_OCM.ListRett listRett9 = rettifiche_OCM.listRett[index];
                  num3 = Convert.ToDecimal(rettifiche_OCM.listRett[index].ImpConDel) * PERMAXSOGLIA / 100M;
                  string str24 = num3.ToString();
                  listRett9.ImpSanDet = str24;
                }
              }
              else
                rettifiche_OCM.listRett[index].ImpSanDet = "0.0";
            }
            else
              rettifiche_OCM.listRett[index].ImpSanDet = "0.0";
            flag2 = true;
            if (rettifiche_OCM.listRett[index].TipMov == "RT" && rettifiche_OCM.listRett[index].NumMov == "")
            {
              string strSQL = "SELECT IMPRET, IMPFIG, IMPOCC FROM DENDET WHERE " + " CODPOS=" + rettifiche_OCM.listRett[index].Codpos + " AND ANNDEN=" + rettifiche_OCM.listRett[index].AnnoDen + " AND MESDEN=" + rettifiche_OCM.listRett[index].MeseDen + " AND PRODEN=" + rettifiche_OCM.listRett[index].Proden + " AND MAT=" + rettifiche_OCM.listRett[index].Mat + " AND PRODENDET=( SELECT MAX(PRODENDET) FROM DENDET WHERE " + " CODPOS=" + rettifiche_OCM.listRett[index].Codpos + " AND ANNDEN=" + rettifiche_OCM.listRett[index].AnnoDen + " AND MESDEN=" + rettifiche_OCM.listRett[index].MeseDen + " AND PRODEN=" + rettifiche_OCM.listRett[index].Proden + " AND MAT=" + rettifiche_OCM.listRett[index].Mat + " AND PRODENDET <> " + rettifiche_OCM.listRett[index].Prodendet + ")" + " AND NUMMOV IS NOT NULL" + " AND NUMMOVANN IS NULL";
              dataTable1.Clear();
              dataTable1 = this.objDataAccess.GetDataTable(strSQL);
              if (dataTable1.Rows.Count > 0)
              {
                if (dataTable1.Rows[0]["IMPRET"].ToString() == rettifiche_OCM.listRett[index].Retribuzione && dataTable1.Rows[0]["IMPOCC"].ToString() == rettifiche_OCM.listRett[index].Occ && dataTable1.Rows[0]["IMPFIG"].ToString() == rettifiche_OCM.listRett[index].Fig)
                {
                  this.objDataAccess.WriteTransactionData("DELETE FROM DENDET" + " CODPOS=" + rettifiche_OCM.listRett[index].Codpos + " AND ANNDEN=" + rettifiche_OCM.listRett[index].AnnoDen + " AND MESDEN=" + rettifiche_OCM.listRett[index].MeseDen + " AND PRODEN=" + rettifiche_OCM.listRett[index].Proden + " AND MAT=" + rettifiche_OCM.listRett[index].Mat + " AND PRODENDET = " + rettifiche_OCM.listRett[index].Prodendet + ")" + " AND TIPMOV='RT' ", CommandType.Text);
                  this.objDataAccess.WriteTransactionData("UPDATE DENDET SET ESIRET = NULL " + " CODPOS=" + rettifiche_OCM.listRett[index].Codpos + " AND ANNDEN=" + rettifiche_OCM.listRett[index].AnnoDen + " AND MESDEN=" + rettifiche_OCM.listRett[index].MeseDen + " AND PRODEN=" + rettifiche_OCM.listRett[index].Proden + " AND MAT=" + rettifiche_OCM.listRett[index].Mat + " AND PRODENDET=( SELECT MAX(PRODENDET) FROM DENDET WHERE " + " CODPOS=" + rettifiche_OCM.listRett[index].Codpos + " AND ANNDEN=" + rettifiche_OCM.listRett[index].AnnoDen + " AND MESDEN=" + rettifiche_OCM.listRett[index].MeseDen + " AND PRODEN=" + rettifiche_OCM.listRett[index].Proden + " AND MAT=" + rettifiche_OCM.listRett[index].Mat + " AND PRODENDET <> " + rettifiche_OCM.listRett[index].Prodendet + ")" + " AND NUMMOV IS NOT NULL" + " AND NUMMOVANN IS NULL", CommandType.Text);
                  flag2 = false;
                }
                else
                  this.objDataAccess.WriteTransactionData("DELETE FROM DENDET" + " CODPOS=" + rettifiche_OCM.listRett[index].Codpos + " AND ANNDEN=" + rettifiche_OCM.listRett[index].AnnoDen + " AND MESDEN=" + rettifiche_OCM.listRett[index].MeseDen + " AND PRODEN=" + rettifiche_OCM.listRett[index].Proden + " AND MAT=" + rettifiche_OCM.listRett[index].Mat + " AND PRODENDET = " + rettifiche_OCM.listRett[index].Prodendet + ")" + " AND TIPMOV='RT' AND NUMMOV IS NULL", CommandType.Text);
              }
              else
                this.objDataAccess.WriteTransactionData("DELETE FROM DENDET" + " CODPOS=" + rettifiche_OCM.listRett[index].Codpos + " AND ANNDEN=" + rettifiche_OCM.listRett[index].AnnoDen + " AND MESDEN=" + rettifiche_OCM.listRett[index].MeseDen + " AND PRODEN=" + rettifiche_OCM.listRett[index].Proden + " AND MAT=" + rettifiche_OCM.listRett[index].Mat + " AND PRODENDET = " + rettifiche_OCM.listRett[index].Prodendet + ")" + " AND TIPMOV='RT' AND NUMMOV IS NULL", CommandType.Text);
            }
            if (flag2)
            {
              this.objDataAccess.WriteTransactionData("UPDATE DENDET SET ESIRET = 'S' WHERE" + " CODPOS='" + rettifiche_OCM.listRett[index].Codpos + "'" + " AND ANNDEN='" + rettifiche_OCM.listRett[index].AnnoDen + "'" + " AND MESDEN='" + rettifiche_OCM.listRett[index].MeseDen + "'" + " AND PRODEN='" + rettifiche_OCM.listRett[index].Proden + "'" + " AND MAT='" + rettifiche_OCM.listRett[index].Mat + "'" + " AND PRODENDET = '" + rettifiche_OCM.listRett[index].Prodendet + "'" + " AND TIPMOV=" + DBMethods.DoublePeakForSql(rettifiche_OCM.listRett[index].TipMov).Trim() + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL", CommandType.Text);
              int int32 = Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(PRIORITA,0) FROM TIPPRIRET WHERE CODTIPRET=1", CommandType.Text));
              string str25 = clsProtocolloDll.WRITE_INSERT_DENDET(u, rettifiche_OCM.listRett[index].Codpos, rettifiche_OCM.listRett[index].AnnoDen, rettifiche_OCM.listRett[index].MeseDen, rettifiche_OCM.listRett[index].Proden, rettifiche_OCM.listRett[index].Mat, rettifiche_OCM.listRett[index].Dal, rettifiche_OCM.listRett[index].Al, rettifiche_OCM.listRett[index].FAP, rettifiche_OCM.listRett[index].Perfap, rettifiche_OCM.listRett[index].Retribuzione, rettifiche_OCM.listRett[index].Occ, rettifiche_OCM.listRett[index].Fig, rettifiche_OCM.listRett[index].Abb, rettifiche_OCM.listRett[index].AssCon, rettifiche_OCM.listRett[index].Contributo, rettifiche_OCM.listRett[index].Min, rettifiche_OCM.listRett[index].Prev, rettifiche_OCM.listRett[index].Prorap, rettifiche_OCM.listRett[index].CodContr, rettifiche_OCM.listRett[index].Procon, rettifiche_OCM.listRett[index].CodLoc, rettifiche_OCM.listRett[index].ProLoc, rettifiche_OCM.listRett[index].CodLiv, rettifiche_OCM.listRett[index].CodquaCon, rettifiche_OCM.listRett[index].DatNas, rettifiche_OCM.listRett[index].Eta65, "RT", rettifiche_OCM.listRett[index].DatDec, rettifiche_OCM.listRett[index].DatCes, rettifiche_OCM.listRett[index].NumGgAzi, rettifiche_OCM.listRett[index].NumGgFig, rettifiche_OCM.listRett[index].NumGgPer, rettifiche_OCM.listRett[index].NumGgDom, rettifiche_OCM.listRett[index].NumGgSos, rettifiche_OCM.listRett[index].NumGgConAzi, rettifiche_OCM.listRett[index].Impsca, rettifiche_OCM.listRett[index].ImpTraEco, rettifiche_OCM.listRett[index].TipRap, rettifiche_OCM.listRett[index].PerPar, rettifiche_OCM.listRett[index].PerApp, rettifiche_OCM.listRett[index].TipSpe, rettifiche_OCM.listRett[index].Codgruass, rettifiche_OCM.listRett[index].Aliq, rettifiche_OCM.listRett[index].TipDen, rettifiche_OCM.listRett[index].DatIniSanz, rettifiche_OCM.listRett[index].DatFinSanz, TASSO, (Decimal) int32, rettifiche_OCM.listRett[index].ImpRetDel, rettifiche_OCM.listRett[index].ImpOccDel, rettifiche_OCM.listRett[index].ImpFigDel, rettifiche_OCM.listRett[index].ImpConDel, IMPRETPRE, IMPOCCPRE, IMPFIGPRE, IMPSANDETPRE, IMPCONPRE, rettifiche_OCM.listRett[index].ImpSanDet, CODCAUSAN, IMPABBPRE, IMPASSCONPRE, rettifiche_OCM.listRett[index].ImpAbbDel, rettifiche_OCM.listRett[index].ImpAssConDel, rettifiche_OCM.listRett[index].AnnoComp, rettifiche_OCM.listRett[index].NumMov).ToString();
              string tipMovDenTes = rettifiche_OCM.listRett[index].TipMovDenTes;
              if (!(tipMovDenTes == "DP") && !(tipMovDenTes == "NU") && !(tipMovDenTes == "RT"))
              {
                if (tipMovDenTes == "AR")
                {
                  DT.Clear();
                  DT = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9005", Convert.ToInt32(rettifiche_OCM.listRett[index].Codpos), Convert.ToInt32(rettifiche_OCM.listRett[index].Mat), Convert.ToInt32(rettifiche_OCM.listRett[index].Prorap), 0, "", "", "9999-12-31", "", "", "", Convert.ToInt32(rettifiche_OCM.listRett[index].AnnoDen), Convert.ToInt32(rettifiche_OCM.listRett[index].MeseDen), Convert.ToInt32(rettifiche_OCM.listRett[index].Proden), "", "", "RT", "", flgWEB: "", PRODENDET: Convert.ToInt32(str25));
                  clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, DT.Rows[0]);
                  clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9005", DT, false);
                }
              }
              else
              {
                DT.Clear();
                DT = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9004", Convert.ToInt32(rettifiche_OCM.listRett[index].Codpos), Convert.ToInt32(rettifiche_OCM.listRett[index].Mat), Convert.ToInt32(rettifiche_OCM.listRett[index].Prorap), 0, "", "", "9999-12-31", "", "", "", Convert.ToInt32(rettifiche_OCM.listRett[index].AnnoDen), Convert.ToInt32(rettifiche_OCM.listRett[index].MeseDen), Convert.ToInt32(rettifiche_OCM.listRett[index].Proden), "", "", "RT", "", flgWEB: "", PRODENDET: Convert.ToInt32(str25));
                clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, DT.Rows[0]);
                clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9004", DT, false);
              }
            }
          }
        }
        if (!flag1)
        {
          ErrorMSG = "Nessuna modifica effettuata.Impossibile continuare";
          this.objDataAccess.EndTransaction(false);
        }
        else
        {
          if (flag2)
          {
            clsIdoc.objDtCONTIDOC = DT;
            clsIdoc.Aggiorna_IDOC(ref this.objDataAccess);
            clsIdoc.objDtCONTIDOC = (DataTable) null;
          }
          this.objDataAccess.EndTransaction(true);
          SuccessMSG = "Salvataggio effettuato con successo";
        }
      }
      catch (Exception ex)
      {
        this.objDataAccess.EndTransaction(false);
      }
    }

    public Decimal MODULE_GENERA_SANZIONE(
      DataLayer objDataAccess,
      Decimal IMPORTO_RETRIBUZIONE,
      ref Decimal TASSO,
      Decimal ALIQUOTA,
      string TIPMOVSAN,
      string DATINISAN,
      string DATFINSAN,
      string CODCAUSAN,
      string DATRETT = "",
      int ANNO = 0,
      Decimal PERMAXSOGLIA = 0.0M)
    {
      DataTable dataTable1 = new DataTable();
      Decimal num1 = 0.0M;
      if (!string.IsNullOrEmpty(TIPMOVSAN))
      {
        CODCAUSAN = this.Module_Get_CAUSALE_MOVIMENTO(TIPMOVSAN);
        Decimal days = (Decimal) Convert.ToDateTime(DATFINSAN).Subtract(Convert.ToDateTime(DATINISAN)).Days;
        string str = "SELECT VALUE(GIORNI, 0) AS GIORNI, VALUE(TASSO, 0.0) AS TASSO, VALUE(PERMAXSOGLIA, 0.0) AS PERMAXSOGLIA ";
        string strSQL;
        if (DATRETT == "")
          strSQL = str + " FROM TIPMOVCAU WHERE TIPMOV = '" + TIPMOVSAN + "' AND " + DBMethods.Db2Date(DATINISAN) + " BETWEEN DATINI AND DATFIN ";
        else
          strSQL = str + " FROM TIPMOVCAU WHERE TIPMOV = '" + TIPMOVSAN + "' AND " + DBMethods.Db2Date(DATRETT) + " BETWEEN DATINI AND DATFIN ";
        dataTable1.Clear();
        DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
        if (dataTable2.Rows.Count > 0)
        {
          if (TIPMOVSAN == "SAN_RT_MD")
            days += (Decimal) dataTable2.Rows[0]["GIORNI"];
          TASSO = (Decimal) dataTable2.Rows[0][nameof (TASSO)];
          PERMAXSOGLIA = (Decimal) dataTable2.Rows[0][nameof (PERMAXSOGLIA)];
        }
        Decimal num2;
        if (ANNO == 0)
        {
          num2 = IMPORTO_RETRIBUZIONE * ALIQUOTA / 100M * days * TASSO / 36500M;
          num1 = Convert.ToDecimal(IMPORTO_RETRIBUZIONE) * ALIQUOTA / 100M * days * TASSO / 36500M;
        }
        else
        {
          num2 = IMPORTO_RETRIBUZIONE * ALIQUOTA / 100M * days * TASSO / 36500M;
          num1 = Convert.ToDecimal(IMPORTO_RETRIBUZIONE) * ALIQUOTA / 100M * days * TASSO / 36500M;
        }
      }
      else
        num1 = 0.0M;
      return num1;
    }

    public string Module_Get_CAUSALE_MOVIMENTO(string TIPMOV) => this.objDataAccess.Get1ValueFromSQL("SELECT CODCAU FROM TIPMOVCAU WHERE TIPMOV = " + DBMethods.DoublePeakForSql(TIPMOV) + " AND CURRENT_DATE BETWEEN DATINI AND DATFIN", CommandType.Text).ToString();

    public string GetNominativo(string mat)
    {
      try
      {
        DataTable dataTable = new DataLayer().GetDataTable("SELECT NOM,COG FROM ISCT WHERE MAT='" + mat + "'");
        return dataTable.Rows.Count > 0 ? dataTable.Rows[0]["NOM"].ToString() + " " + dataTable.Rows[0]["COG"].ToString() : string.Empty;
      }
      catch (Exception ex)
      {
        throw;
      }
    }
  }
}
