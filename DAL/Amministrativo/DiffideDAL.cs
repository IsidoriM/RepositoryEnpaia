// Decompiled with JetBrains decompiler
// Type: DAL.Amministrativo.DiffideDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Collections.Generic;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Amministrativo
{
  public class DiffideDAL
  {
    private DataLayer objDataAccess = new DataLayer();

    public void btnCerca_Click(DiffideOCM oCM, string CodPos, string Anno, ref string MsgErrore)
    {
      string str1 = "";
      DataTable dataTable1 = new DataTable();
      try
      {
        string str2 = "SELECT A.ANNO, A.CODPOS, A.RAGSOC, B.CODFIS, B.PARIVA, A.NOMFILE, DATCON, CODUNI" + " FROM DIFFIDE A, AZI B WHERE A.CODPOS = B.CODPOS";
        if (!string.IsNullOrEmpty(CodPos))
          str1 = " AND A.CODPOS = " + CodPos;
        if (!string.IsNullOrEmpty(Anno))
          str1 = str1 + " AND A.ANNO = " + Anno;
        DataTable dataTable2 = this.objDataAccess.GetDataTable(str2 + str1 + " ORDER BY A.ANNO, A.CODPOS");
        List<DiffideOCM.Diffide> diffideList = new List<DiffideOCM.Diffide>();
        if (dataTable2.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
          {
            DiffideOCM.Diffide diffide = new DiffideOCM.Diffide()
            {
              Anno = row["ANNO"].ToString(),
              CodPos = row["CODPOS"].ToString(),
              RagSoc = row["RAGSOC"].ToString(),
              CodFis = row["CODFIS"].ToString(),
              ParIva = row["PARIVA"].ToString(),
              Nfile = row["NOMFILE"].ToString(),
              DatConsegna = row["DATCON"].ToString().Substring(0, 10),
              CodUnivoco = row["CODUNI"].ToString()
            };
            diffideList.Add(diffide);
          }
          oCM.ListaDiffide = diffideList;
        }
        if (dataTable2.Rows.Count > 0)
          return;
        MsgErrore = "Nessun risultato trovato";
      }
      catch (Exception ex)
      {
        MsgErrore = "Errore nella ricerca dei dati. Riprovare";
      }
    }
  }
}
