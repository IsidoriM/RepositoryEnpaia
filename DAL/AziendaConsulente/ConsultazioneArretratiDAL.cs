// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.ConsultazioneArretratiDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Collections.Generic;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;

namespace TFI.DAL.AziendaConsulente
{
  public class ConsultazioneArretratiDAL
  {
    public static List<DenunciaArretrati> LoadData(
      string CodPos,
      string anno,
      string mese,
      string parm,
      string staDen,
      string proDen,
      string mat,
      string anncom)
    {
      DataTable dataTable = new DataTable();
      DataLayer dataLayer = new DataLayer();
      List<DenunciaArretrati> denunciaArretratiList = new List<DenunciaArretrati>();
      try
      {
        string strSQL = "SELECT A.ANNDEN, A.MESDEN, A.PRODEN, B.ANNCOM, B.MAT, (TRIM(C.COG) || ' ' || TRIM(C.NOM)) AS " + "NOMINATIVO, A.DATEROARR, C.DATNAS, B.DAL, B.AL, B.IMPRET, B.IMPOCC, B.IMPFIG, B.IMPCON, 0 AS " + "ALIQUOTA, A.STADEN, SUBSTRING(D.DENQUA, 1, 1) AS DENQUA, VALUE(A.CODMODPAG, 0) AS CODMODPAG " + "FROM DENTES A INNER JOIN " + "DENDET B ON A.CODPOS = B.CODPOS AND A.PRODEN = B.PRODEN AND A.ANNDEN = B.ANNDEN AND A.MESDEN = " + "B.MESDEN INNER JOIN ISCT C ON B.MAT = C.MAT LEFT JOIN QUACON D ON B.CODQUACON = D.CODQUACON " + "WHERE B.IMPRET <> 0 AND " + "A.CODPOS = " + CodPos + " AND B.TIPMOV = 'AR'";
        if (anno != "")
          strSQL = strSQL + " AND A.ANNDEN = " + anno + " AND A.MESDEN = " + mese + " AND A.PRODEN = " + proDen;
        string str1;
        if (!string.IsNullOrWhiteSpace(anncom))
        {
          strSQL = strSQL + " AND B.ANNCOM = " + anncom;
          str1 = $"Dettaglio Denuncia Arretrati relativo all'Anno di Competenza {anncom}";
          if (!string.IsNullOrWhiteSpace(mat) && mat != "0")
          {
            strSQL = strSQL + " AND B.MAT = " + mat;
            str1 = str1 + " per la Matricola " + mat;
          }
        }
        else if (!string.IsNullOrEmpty(mat) && mat != "0")
        {
          strSQL = strSQL + " AND B.MAT = " + mat;
          str1 = "Dettaglio Denuncia Arretrati relativo alla Matricola " + mat + " - " + dataLayer.Get1ValueFromSQL("SELECT TRIM(COG) || ' ' || TRIM(NOM) FROM ISCT WHERE MAT = " + mat, CommandType.Text) + " per il periodo " + DBMethods.GetMesi()[int.Parse(mese)].ToUpper() + " " + anno;
        }
        else
          str1 = "Dettaglio Denuncia Arretrati relativo al Periodo " + DBMethods.GetMesi()[int.Parse(mese)].ToUpper() + " " + anno;
        foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataTable(strSQL).Rows)
        {
          DenunciaArretrati denunciaArretrati1 = new DenunciaArretrati();
          denunciaArretrati1.anno = Convert.ToInt32(row["ANNDEN"]);
          denunciaArretrati1.meseLett = DBMethods.GetMesi()[Convert.ToInt32(row["MESDEN"])];
          denunciaArretrati1.proDen = Convert.ToInt32(row["PRODEN"]);
          denunciaArretrati1.anncom = Convert.ToInt32(row["ANNCOM"]);
          denunciaArretrati1.mat = Convert.ToInt32(row["MAT"]);
          denunciaArretrati1.nome = row["NOMINATIVO"].ToString();
          denunciaArretrati1.datero = row["NOMINATIVO"].ToString();
          denunciaArretrati1.datnas = row["DATNAS"].ToString();
          DateTime dateTime = DateTime.Parse(row["DATNAS"].ToString());
          denunciaArretrati1.calcoloEta = dateTime.AddYears(65);
          dateTime = Convert.ToDateTime(row["DAL"]);
          denunciaArretrati1.datadal = dateTime.ToString();
          dateTime = Convert.ToDateTime(row["AL"]);
          denunciaArretrati1.dataal = dateTime.ToString();
          denunciaArretrati1.impret = row["IMPRET"].ToString();
          denunciaArretrati1.impocc = row["IMPOCC"].ToString();
          denunciaArretrati1.impfig = Convert.ToInt32(row["IMPFIG"]);
          denunciaArretrati1.impcon = row["IMPCON"].ToString();
          Decimal num1 = Convert.ToDecimal(row["ALIQUOTA"]);
          denunciaArretrati1.aliquota = num1.ToString().Replace(".", ",");
          denunciaArretrati1.hdnStaden = row["STADEN"].ToString();
          denunciaArretrati1.denqua = row["DENQUA"].ToString();
          denunciaArretrati1.codmodpag = Convert.ToInt32(row["CODMODPAG"]);
          denunciaArretrati1.lblIntestazione = str1;
          DenunciaArretrati denunciaArretrati2 = denunciaArretrati1;
          int num2 = DateTime.Compare(denunciaArretrati2.calcoloEta, DateTime.Parse(denunciaArretrati2.datnas));
          int num3 = DateTime.Compare(denunciaArretrati2.calcoloEta, DateTime.Parse(denunciaArretrati2.datadal));
          denunciaArretrati2.eta65 = num2 != 1 || num3 > 0 ? "N" : "S";
          if (Convert.ToDecimal(denunciaArretrati2.impcon) > 0M)
          {
            DenunciaArretrati denunciaArretrati3 = denunciaArretrati2;
            num1 = 100M / (Convert.ToDecimal(denunciaArretrati2.impret) / Convert.ToDecimal(denunciaArretrati2.impcon));
            string str2 = num1.ToString();
            denunciaArretrati3.aliquota = str2;
          }
          if (denunciaArretrati2.datero != "nbsp;")
            denunciaArretrati2.datero = denunciaArretrati2.datero.Substring(0, 10);
          string hdnStaden = denunciaArretrati2.hdnStaden;
          if (!(hdnStaden == "N"))
          {
            if (hdnStaden == "A")
              denunciaArretrati2.hdnStaden = denunciaArretrati2.codmodpag != 0 ? "Acquisita con D.P." : "Acquisita";
          }
          else
            denunciaArretrati2.hdnStaden = "Non Valido";
          denunciaArretratiList.Add(denunciaArretrati2);
        }
        return denunciaArretratiList;
      }
      catch
      {
        return (List<DenunciaArretrati>) null;
      }
    }

    public static ConsultazioneArretrati GetTotali(
      string CodPos,
      string anno,
      string mese,
      string parm,
      string staDen,
      string proDen,
      string mat,
      string anncom,
      ConsultazioneArretrati arretrati)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL1 = "SELECT VALUE(SUM(B.IMPRET), 0) AS IMPRET, VALUE(SUM(B.IMPOCC), 0) AS IMPOCC, VALUE(SUM(B.IMPCON), 0) " + "AS IMPCON FROM DENTES A INNER JOIN DENDET B ON A.CODPOS = B.CODPOS AND A.PRODEN = B.PRODEN AND A.ANNDEN = " + "B.ANNDEN AND A.MESDEN = B.MESDEN WHERE A.CODPOS = " + CodPos + " AND A.ANNDEN = " + anno + " AND A.MESDEN = " + mese + " AND A.PRODEN = " + proDen + " AND B.TIPMOV = 'AR'";
        if (anncom != "" && anncom != null)
          strSQL1 = strSQL1 + " AND B.ANNCOM = " + anncom;
        else if (!string.IsNullOrEmpty(mat) && mat != "0")
          strSQL1 = strSQL1 + " AND B.MAT = " + mat;
        DataTable dataTable = dataLayer.GetDataTable(strSQL1);
        if (dataTable.Rows.Count > 0)
        {
          arretrati.lblTotRetribuzioni = Convert.ToDecimal(dataTable.Rows[0]["IMPRET"].ToString());
          arretrati.lblTotOccasionali = Convert.ToDecimal(dataTable.Rows[0]["IMPOCC"].ToString());
          arretrati.lblTotContributi = Convert.ToDecimal(dataTable.Rows[0]["IMPCON"].ToString());
        }
        string strSQL2 = "SELECT COUNT(*) FROM DENDET WHERE CODPOS = " + CodPos + " AND ANNDEN = " + anno + " AND MESDEN = " + mese + " AND PRODEN = " + proDen + " AND TIPMOV = 'RT' AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL";
        arretrati.btnRettificheVisible = Convert.ToInt32("0" + dataLayer.Get1ValueFromSQL(strSQL2, CommandType.Text)) > 0;
        return arretrati;
      }
      catch
      {
        return (ConsultazioneArretrati) null;
      }
    }
  }
}
