// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.ModulisticaDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.OCM.AziendaConsulente;

namespace TFI.DAL.AziendaConsulente
{
  public class ModulisticaDAL
  {
    public List<Modulistica> ModuliDAL()
    {
      bool blnCommit = true;
      List<Modulistica> modulisticaList = new List<Modulistica>();
      string str1 = (string) null;
      string str2 = HttpContext.Current.Session["layout"].ToString().Substring(11, 1);
      string strSQL = !(str2 == "E") ? str1 + "SELECT * FROM DOWNLOAD WHERE CODTIPUTE = '" + str2 + "' ORDER BY TITOLO" : str1 + "SELECT * FROM DOWNLOAD WHERE CODTIPUTE = 'A' ORDER BY TITOLO";
      try
      {
        DataLayer dataLayer = new DataLayer();
        dataLayer.StartTransaction();
        DataSet dataSet1 = new DataSet();
        string Err = "";
        dataSet1 = dataLayer.GetDataSet(strSQL, ref Err);
        DataSet dataSet2 = new DataSet();
        foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataSet(strSQL, ref Err).Tables[0].Rows)
          modulisticaList.Add(new Modulistica()
          {
            Oggetto = row["TITOLO"].ToString(),
            Descrizione = row["DESCRIZIONE"].ToString(),
            File = row["PATH"].ToString(),
            NomeFile = this.FileExists(string.Format("{0}{1}{2}", (object) AppDomain.CurrentDomain.BaseDirectory, (object) ConfigurationManager.AppSettings["path_modulistica"], row["PATH"]))
          });
        dataLayer.EndTransaction(blnCommit);
        return modulisticaList;
      }
      catch (Exception ex)
      {
        return modulisticaList;
      }
    }

    private string FileExists(string path) => !File.Exists(path) ? "#" : path.Replace(AppDomain.CurrentDomain.BaseDirectory, "../../");
  }
}
