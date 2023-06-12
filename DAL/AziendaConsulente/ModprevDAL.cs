// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.ModprevDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;

namespace TFI.DAL.AziendaConsulente
{
  public class ModprevDAL
  {
    public ModPrevOCM GetModPre(
      int codPos,
      string mat,
      string nom,
      string cog,
      string prorap,
      string promod,
      ref string errorMessage)
    {
      ModPrevOCM modPre = new ModPrevOCM();
      DataLayer dataLayer = new DataLayer();
      string empty = string.Empty;
      try
      {
        string strSQL1 = "SELECT ISCT.MAT, ISCT.COG, ISCT.NOM, ISCT.SES, RAPLAV.DATCES, CAUCES.DENCES, RAPLAV.DATDEC, MODPRE.COGRAP, MODPRE.NOMRAP, MODPRE.CODFISRAP, ISCT.CODFIS" + " FROM CONTCONSIP.ISCT, CONTCONSIP.RAPLAV, CONTCONSIP.CAUCES, CONTCONSIP.MODPRE WHERE (ISCT.MAT = RAPLAV.MAT) AND (RAPLAV.CODCAUCES = CAUCES.CODCAUCES)" + string.Format(" AND (RAPLAV.CODPOS = MODPRE.CODPOS) AND (RAPLAV.MAT = MODPRE.MAT) AND (RAPLAV.PRORAP = MODPRE.PRORAP) AND (MODPRE.CODPOS = '{0}') AND (MODPRE.MAT = '", (object) codPos) + mat + "')" + " AND (MODPRE.PRORAP = '" + prorap + "') AND (MODPRE.PROMOD = '" + promod + "')";
        string Err = "";
        DataSet dataSet1 = new DataSet();
        DataSet dataSet2 = dataLayer.GetDataSet(strSQL1, ref Err);
        if (dataSet2.Tables.Count > 0)
        {
          if (dataSet2.Tables[0].Rows.Count > 0)
          {
            modPre.datGen.rapplegale = dataSet2.Tables[0].Rows[0]["COGRAP"].ToString() + " " + dataSet2.Tables[0].Rows[0]["NOMRAP"].ToString();
            modPre.datGen.matricola = dataSet2.Tables[0].Rows[0]["MAT"].ToString().Trim();
            modPre.datGen.nome = dataSet2.Tables[0].Rows[0]["NOM"].ToString().Trim();
            modPre.datGen.cognome = dataSet2.Tables[0].Rows[0]["COG"].ToString().Trim();
            modPre.datGen.dataini = dataSet2.Tables[0].Rows[0]["DATDEC"].ToString().Trim();
            modPre.datGen.datacess = dataSet2.Tables[0].Rows[0]["DATCES"].ToString().Trim();
            modPre.datGen.causale = dataSet2.Tables[0].Rows[0]["DENCES"].ToString().Trim();
            string str1 = dataSet2.Tables[0].Rows[0]["SES"].ToString().Trim();
            modPre.datGen.prorap = prorap;
            modPre.datGen.promod = promod;
            Decimal num1 = Convert.ToDecimal(modPre.datGen.datacess.Substring(6, 4));
            Decimal num2 = Convert.ToDecimal(Convert.ToDateTime(modPre.datGen.datacess).AddYears(-1).ToString().Substring(6, 4));
            string strSQL2 = " SELECT DENDET.ANNDEN, DENDET.MESDEN, DENDET.PRODEN, DENDET.PRODENDET, CONTCONSIP.DENDET.DAL, " + " CONTCONSIP.DENDET.AL, CONTCONSIP.DENDET.PRORAP, DENDET.IMPRET, DENDET.IMPOCC, DENDET.IMPFIG, " + " DENDET.IMPABB, DENDET.IMPASSCON, CONTCONSIP.DENDET.ALIQUOTA, DENDET.IMPCON, CONTCONSIP.DENDET.IMPMIN, " + " CONTCONSIP.DENDET.DATDEC, CONTCONSIP.DENDET.DATCES, CONTCONSIP.DENDET.NUMGGAZI, CONTCONSIP.DENDET.NUMGGFIG, " + " CONTCONSIP.DENDET.NUMGGPER, CONTCONSIP.DENDET.NUMGGDOM, CONTCONSIP.DENDET.NUMGGSOS, CONTCONSIP.DENDET.NUMGGCONAZI, " + " CONTCONSIP.DENDET.IMPSCA, CONTCONSIP.DENDET.IMPTRAECO, CONTCONSIP.DENDET.ETA65, CONTCONSIP.DENDET.TIPRAP, " + " CONTCONSIP.DENDET.FAP, CONTCONSIP.DENDET.PERFAP, CONTCONSIP.DENDET.IMPFAP, CONTCONSIP.DENDET.PERPAR, CONTCONSIP.DENDET.PERAPP, " + " CONTCONSIP.DENDET.CODCON, CONTCONSIP.DENDET.PROCON, CONTCONSIP.DENDET.TIPSPE, CONTCONSIP.DENDET.CODLOC, CONTCONSIP.DENDET.PROLOC, " + " CONTCONSIP.DENDET.CODLIV, CONTCONSIP.DENDET.CODGRUASS, CONTCONSIP.DENDET.CODQUACON, CONTCONSIP.DENDET.DATNAS, CONTCONSIP.DENDET.ANNCOM, " + " 'N' AS PREV, DENDET.NUMMOV, DENDET.TIPMOV " + " FROM CONTCONSIP.DENDET, CONTCONSIP.DENTES " + string.Format(" WHERE DENDET.CODPOS = DENTES.CODPOS AND DENDET.ANNDEN = DENTES.ANNDEN AND DENDET.MESDEN = DENTES.MESDEN AND(DENDET.CODPOS = '{0}') ", (object) codPos) + " AND(CONTCONSIP.DENDET.MAT = '" + mat + "') AND (DENDET.NUMMOVANN IS NULL) AND(VALUE(DENDET.ESIRET, '') <> 'S') AND(DENDET.TIPMOV <> 'AR') " + " AND (DENDET.ANNDEN >= '" + num2.ToString() + "') AND (DENDET.ANNDEN <= '" + num1.ToString() + "') " + " ORDER BY DENDET.ANNDEN, DENDET.MESDEN, DENDET.DAL";
            DataSet dataSet3 = new DataSet();
            DataSet dataSet4 = dataLayer.GetDataSet(strSQL2, ref Err);
            foreach (DataRow row in (InternalDataCollectionBase) dataSet4.Tables[0].Rows)
            {
              ModPrevOCM.denunce denunce = new ModPrevOCM.denunce()
              {
                anno = row["ANNDEN"].ToString(),
                mese = row["MESDEN"].ToString(),
                iniperiodo = row["DAL"].ToString(),
                finperiodo = row["AL"].ToString(),
                retrimp = row["IMPRET"].ToString(),
                occ = row["IMPOCC"].ToString(),
                Aliq = row["ALIQUOTA"].ToString(),
                fig = row["IMPFIG"].ToString()
              };
              modPre.denunces.Add(denunce);
            }
            modPre.Totali.anno1 = num2.ToString();
            modPre.Totali.anno2 = num1.ToString();
            modPre.datGen.Aliq = dataSet4.Tables[0].Rows[0]["ALIQUOTA"].ToString();
            string strSQL3 = "SELECT DISTINCT DATINI, DATFIN, PERPAR" + "FROM CONTCONSIP.STORDL" + "WHERE(VALUE(PERPAR, 0) > 0) AND(PRORAP = '" + prorap + string.Format("') AND(CODPOS = '{0}') AND MAT='", (object) codPos) + mat + "'" + "ORDER BY DATINI";
            DataSet dataSet5 = new DataSet();
            DataSet dataSet6 = dataLayer.GetDataSet(strSQL3, ref Err);
            if (dataSet6.Tables.Count != 0 && !DBNull.Value.Equals(dataSet6.Tables[0].Rows[0]["DATINI"]) && !DBNull.Value.Equals(dataSet6.Tables[0].Rows[0]["DATFIN"]) && !DBNull.Value.Equals(dataSet6.Tables[0].Rows[0]["PERPAR"]))
            {
              foreach (DataRow row in (InternalDataCollectionBase) dataSet6.Tables[0].Rows)
              {
                ModPrevOCM.part_Time partTime = new ModPrevOCM.part_Time()
                {
                  datini = row["DATINI"].ToString(),
                  datfin = row["DATFIN"].ToString(),
                  prepar = row["PERPAR"].ToString()
                };
                modPre.part_time.Add(partTime);
              }
            }
            string datacess = modPre.datGen.datacess;
            DateTime dateTime = Convert.ToDateTime(datacess).AddYears(-1);
            string dataini = modPre.datGen.dataini;
            string strData = "01/01/" + dateTime.ToString().Substring(6, 4);
            string str2 = "SELECT TRANSLATE(CHAR(DATINISOS, EUR),'/','.') AS DAL, " + " TRANSLATE(CHAR(DATFINSOS, EUR),'/','.') AS AL, DENSOS AS SOSPENSIONE " + "  FROM SOSRAP A, CODSOS B WHERE A.CODSOS = B.CODSOS " + " AND A.MAT = '" + mat + "' " + " AND A.PRORAP = '" + prorap + "' " + string.Format(" AND A.CODPOS = '{0}' AND A.STASOS = 0 ", (object) codPos) + " AND DATINISOS <= '" + DBMethods.Db2Date(datacess.Substring(0, 10)) + "' ";
            string strSQL4 = (!(dataini.Substring(6, 4) == dateTime.ToString().Substring(6, 4)) ? str2 + " AND DATFINSOS >= '" + DBMethods.Db2Date(strData) + "' " : str2 + " AND DATFINSOS >= '" + DBMethods.Db2Date(dataini.Substring(0, 10)) + "' ") + " ORDER BY DATINISOS DESC, DATFINSOS";
            DataSet dataSet7 = new DataSet();
            DataSet dataSet8 = dataLayer.GetDataSet(strSQL4, ref Err);
            if (dataSet8.Tables[0].Rows.Count != 0 && !DBNull.Value.Equals(dataSet8.Tables[0].Rows[0]["DAL"]) && !DBNull.Value.Equals(dataSet8.Tables[0].Rows[0]["AL"]) && !DBNull.Value.Equals(dataSet8.Tables[0].Rows[0]["SOSPENSIONE"]))
            {
              foreach (DataRow row in (InternalDataCollectionBase) dataSet8.Tables[0].Rows)
              {
                ModPrevOCM.sospensioni sospensioni = new ModPrevOCM.sospensioni()
                {
                  dal = row["DAL"].ToString(),
                  al = row["AL"].ToString(),
                  motsosp = row["SOSPENSIONE"].ToString()
                };
                modPre.sosp.Add(sospensioni);
              }
            }
            string strSQL5 = "SELECT CODSOS, DENSOS FROM CODSOS WHERE CODSOS<> 0 AND UTESOS IN('A', 'T') AND " + " TIPSES IN('" + str1 + "', 'E') AND current_date BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31') " + " AND CODSOS <> 4 ORDER BY DENSOS ";
            DataSet dataSet9 = new DataSet();
            DataSet dataSet10 = dataLayer.GetDataSet(strSQL5, ref Err);
            if (dataSet10.Tables.Count != 0)
            {
              foreach (DataRow row in (InternalDataCollectionBase) dataSet10.Tables[0].Rows)
              {
                ModPrevOCM.listsosp listsosp = new ModPrevOCM.listsosp()
                {
                  codsos = row["CODSOS"].ToString(),
                  densos = row["DENSOS"].ToString()
                };
                modPre.listsosps.Add(listsosp);
              }
            }
          }
          else
            errorMessage = "Dati non trovati";
        }
        else
          errorMessage = "Dati non trovati";
        return modPre;
      }
      catch (Exception ex)
      {
        return (ModPrevOCM) null;
      }
    }

    public ModPrevOCM Save_sosp(ModPrevOCM og)
    {
      DataLayer dataLayer = new DataLayer();
      clsPrev clsPrev = new clsPrev();
      string matricola = og.datGen.matricola;
      string prorap = og.datGen.prorap;
      string promod = og.datGen.promod;
      string datacess = og.datGen.datacess;
      string CODPOS = "1049";
      try
      {
        clsPrev.WRITE_INSERT_SOSPREV(CODPOS, matricola, prorap, promod, datacess);
        return og;
      }
      catch (Exception ex)
      {
        return og;
      }
    }

    public ModPrevOCM Save_arr(ModPrevOCM og)
    {
      DataLayer dataLayer = new DataLayer();
      clsPrev clsPrev = new clsPrev();
      string matricola = og.datGen.matricola;
      string prorap = og.datGen.prorap;
      string promod = og.datGen.promod;
      string datacess = og.datGen.datacess;
      string CODPOS = "1049";
      try
      {
        clsPrev.WRITE_UPDATE_MODPREDET(CODPOS, matricola, prorap, promod, datacess);
        return og;
      }
      catch (Exception ex)
      {
        return og;
      }
    }

    public ModPrevOCM Save_den(ModPrevOCM og)
    {
      DataLayer dataLayer = new DataLayer();
      clsPrev clsPrev = new clsPrev();
      string matricola = og.datGen.matricola;
      string prorap = og.datGen.prorap;
      string promod = og.datGen.promod;
      string datacess = og.datGen.datacess;
      string CODPOS = "1049";
      try
      {
        clsPrev.WRITE_UPDATE_MODPREPAR(CODPOS, matricola, prorap, promod, datacess);
        return og;
      }
      catch (Exception ex)
      {
        return og;
      }
    }
  }
}
