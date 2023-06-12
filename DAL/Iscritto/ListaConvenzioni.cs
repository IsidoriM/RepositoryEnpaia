// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Iscritto.ListaConvenzioniDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Iscritto;

namespace TFI.DAL.Iscritto
{
  public class ListaConvenzioniDAL
  {
    public List<ListaConvenzioniIscritto> Convenzioni()
    {
      DataLayer dataLayer = new DataLayer();
      DataSet dataSet1 = new DataSet();
      string Err = "";
      string str = "SELECT TITOLO, PATH, DESCRIZIONE, DATINS, PATHIMG FROM CONVENZIONI WHERE TIPO IN ('I', 'T') AND DATANN IS NULL ORDER BY DATINS DESC";
      DataSet dataSet2 = dataLayer.GetDataSet(str, ref Err);
      dataSet2.Tables[0].Rows[0]["TITOLO"].ToString();
      dataSet2.Tables[0].Rows[0]["PATH"].ToString();
      dataSet2.Tables[0].Rows[0]["DESCRIZIONE"].ToString();
      dataSet2.Tables[0].Rows[0]["DATINS"].ToString();
      dataSet2.Tables[0].Rows[0]["PATHIMG"].ToString();
      List<ListaConvenzioniIscritto> convenzioniIscrittoList = new List<ListaConvenzioniIscritto>();
      iDB2DataReader dataReaderFromQuery = dataLayer.GetDataReaderFromQuery(str, CommandType.Text);
      while (dataReaderFromQuery.Read())
        convenzioniIscrittoList.Add(new ListaConvenzioniIscritto()
        {
          DataInserimento = dataReaderFromQuery["DATINS"].ToString().Substring(0, 10),
          Titolo = dataReaderFromQuery["TITOLO"].ToString(),
          Descrizione = dataReaderFromQuery["DESCRIZIONE"].ToString(),
          Path = FileExists(string.Format("{0}{1}{2}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["path_convenzioni"], dataReaderFromQuery["PATH"])),
          PathImg = ImgExists(string.Format("{0}{1}{2}", AppDomain.CurrentDomain.BaseDirectory, "Images/Convenzioni/", dataReaderFromQuery["PATHIMG"]))
        });
      return convenzioniIscrittoList;
    }

    private string FileExists(string path) => !File.Exists(path) 
            ? "#" 
            : path.Replace(AppDomain.CurrentDomain.BaseDirectory, "../../");
    private string ImgExists(string path) => !File.Exists(path) 
            ? null
            : path.Replace(AppDomain.CurrentDomain.BaseDirectory, "../../");
    }
}
