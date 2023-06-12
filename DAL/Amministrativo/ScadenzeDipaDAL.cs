// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.ScadenzeDipaDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Amministrativo
{
  public class ScadenzeDipaDAL
  {
    private DataLayer objDataAccess = new DataLayer();

    public ScadenzaDipaOCM AnnoScaDipa()
    {
      ScadenzaDipaOCM scadenzaDipaOcm = new ScadenzaDipaOCM();
      DataTable dataTable = this.objDataAccess.GetDataTable("SELECT DATINI FROM PARGENDET WHERE CODPAR =3 ORDER BY DATINI");
      int index = 0;
      if (dataTable.Rows.Count > 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
        {
          string str = dataTable.Rows[index]["DATINI"].ToString().Substring(6, 4);
          ++index;
          if (index < dataTable.Rows.Count && str != dataTable.Rows[index]["DATINI"].ToString().Substring(6, 4))
          {
            ScadenzaDipaOCM.Anno anno = new ScadenzaDipaOCM.Anno()
            {
              anno = str
            };
            scadenzaDipaOcm.listanni.Add(anno);
          }
        }
      }
      return scadenzaDipaOcm;
    }

    public void CercaDataDAL(string anno, ScadenzaDipaOCM scadipaOCM)
    {
      string str1 = anno + "-01-01";
      string str2 = anno + "-12-31";
      List<ScadenzaDipaOCM.DatiScadenzeDipa> datiScadenzeDipaList = new List<ScadenzaDipaOCM.DatiScadenzeDipa>();
      DataTable dataTable = this.objDataAccess.GetDataTable("SELECT * FROM PARGENDET WHERE CODPAR =3 AND DATFIN>'" + str1 + "' AND DATFIN <='" + str2 + "'");
      if (dataTable.Rows.Count > 0)
      {
        int index = 0;
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
        {
          if (index < dataTable.Rows.Count)
          {
            string str3 = "";
            switch (Convert.ToInt32(dataTable.Rows[index]["DATINI"].ToString().Substring(3, 2)))
            {
              case 1:
                str3 = "Gennaio";
                break;
              case 2:
                str3 = "Febbraio";
                break;
              case 3:
                str3 = "Marzo";
                break;
              case 4:
                str3 = "Aprile";
                break;
              case 5:
                str3 = "Maggio";
                break;
              case 6:
                str3 = "Giugno";
                break;
              case 7:
                str3 = "Luglio";
                break;
              case 8:
                str3 = "Agosto";
                break;
              case 9:
                str3 = "Settembre";
                break;
              case 10:
                str3 = "Ottobre";
                break;
              case 11:
                str3 = "Novembre";
                break;
              case 12:
                str3 = "Dicembre";
                break;
            }
            ScadenzaDipaOCM.DatiScadenzeDipa datiScadenzeDipa = new ScadenzaDipaOCM.DatiScadenzeDipa()
            {
              datinival = dataTable.Rows[index]["DATINI"].ToString().Substring(0, 10),
              datfinval = dataTable.Rows[index]["DATFIN"].ToString().Substring(0, 10),
              mese = str3,
              datsca = dataTable.Rows[index]["VALORE"].ToString().Substring(0, 10)
            };
            datiScadenzeDipaList.Add(datiScadenzeDipa);
            ++index;
          }
        }
      }
      scadipaOCM.listScadenzaDipa = datiScadenzeDipaList;
    }

    public ScadenzaDipaOCM ModDipaDAL(
      ScadenzaDipaOCM scadipaOCM,
      TFI.OCM.Utente.Utente u,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      this.objDataAccess.StartTransaction();
      try
      {
        string str = scadipaOCM.datiScadenzeDipa.datfinval_1.ToString().Substring(6, 4);
        int num = 0;
        for (int index = 0; index < scadipaOCM.listanni.Count; ++index)
        {
          if (scadipaOCM.listanni[index].anno.ToString() == str)
            ++num;
        }
        if (num == 0)
        {
          this.objDataAccess.WriteTransactionData("INSERT INTO PARGENDET(CODPAR,DATINI,DATFIN,VALORE,ULTAGG,UTEAGG) " + " VALUES('3','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datinival_1) + "','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datfinval_1) + "','" + scadipaOCM.datiScadenzeDipa.datsca_1 + "',CURRENT TIMESTAMP,'" + u.Username + "')", CommandType.Text);
          this.objDataAccess.WriteTransactionData("INSERT INTO PARGENDET(CODPAR,DATINI,DATFIN,VALORE,ULTAGG,UTEAGG) " + " VALUES('3','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datinival_2) + "','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datfinval_2) + "','" + scadipaOCM.datiScadenzeDipa.datsca_2 + "',CURRENT TIMESTAMP,'" + u.Username + "')", CommandType.Text);
          this.objDataAccess.WriteTransactionData("INSERT INTO PARGENDET(CODPAR,DATINI,DATFIN,VALORE,ULTAGG,UTEAGG) " + " VALUES('3','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datinival_3) + "','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datfinval_3) + "','" + scadipaOCM.datiScadenzeDipa.datsca_3 + "',CURRENT TIMESTAMP,'" + u.Username + "')", CommandType.Text);
          this.objDataAccess.WriteTransactionData("INSERT INTO PARGENDET(CODPAR,DATINI,DATFIN,VALORE,ULTAGG,UTEAGG) " + " VALUES('3','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datinival_4) + "','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datfinval_4) + "','" + scadipaOCM.datiScadenzeDipa.datsca_4 + "',CURRENT TIMESTAMP,'" + u.Username + "')", CommandType.Text);
          this.objDataAccess.WriteTransactionData("INSERT INTO PARGENDET(CODPAR,DATINI,DATFIN,VALORE,ULTAGG,UTEAGG) " + " VALUES('3','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datinival_5) + "','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datfinval_5) + "','" + scadipaOCM.datiScadenzeDipa.datsca_5 + "',CURRENT TIMESTAMP,'" + u.Username + "')", CommandType.Text);
          this.objDataAccess.WriteTransactionData("INSERT INTO PARGENDET(CODPAR,DATINI,DATFIN,VALORE,ULTAGG,UTEAGG) " + " VALUES('3','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datinival_6) + "','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datfinval_6) + "','" + scadipaOCM.datiScadenzeDipa.datsca_6 + "',CURRENT TIMESTAMP,'" + u.Username + "')", CommandType.Text);
          this.objDataAccess.WriteTransactionData("INSERT INTO PARGENDET(CODPAR,DATINI,DATFIN,VALORE,ULTAGG,UTEAGG) " + " VALUES('3','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datinival_7) + "','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datfinval_7) + "','" + scadipaOCM.datiScadenzeDipa.datsca_7 + "',CURRENT TIMESTAMP,'" + u.Username + "')", CommandType.Text);
          this.objDataAccess.WriteTransactionData("INSERT INTO PARGENDET(CODPAR,DATINI,DATFIN,VALORE,ULTAGG,UTEAGG) " + " VALUES('3','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datinival_8) + "','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datfinval_8) + "','" + scadipaOCM.datiScadenzeDipa.datsca_8 + "',CURRENT TIMESTAMP,'" + u.Username + "')", CommandType.Text);
          this.objDataAccess.WriteTransactionData("INSERT INTO PARGENDET(CODPAR,DATINI,DATFIN,VALORE,ULTAGG,UTEAGG) " + " VALUES('3','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datinival_9) + "','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datfinval_9) + "','" + scadipaOCM.datiScadenzeDipa.datsca_9 + "',CURRENT TIMESTAMP,'" + u.Username + "')", CommandType.Text);
          this.objDataAccess.WriteTransactionData("INSERT INTO PARGENDET(CODPAR,DATINI,DATFIN,VALORE,ULTAGG,UTEAGG) " + " VALUES('3','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datinival_10) + "','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datfinval_10) + "','" + scadipaOCM.datiScadenzeDipa.datsca_10 + "',CURRENT TIMESTAMP,'" + u.Username + "')", CommandType.Text);
          this.objDataAccess.WriteTransactionData("INSERT INTO PARGENDET(CODPAR,DATINI,DATFIN,VALORE,ULTAGG,UTEAGG) " + " VALUES('3','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datinival_11) + "','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datfinval_11) + "','" + scadipaOCM.datiScadenzeDipa.datsca_11 + "',CURRENT TIMESTAMP,'" + u.Username + "')", CommandType.Text);
          this.objDataAccess.WriteTransactionData("INSERT INTO PARGENDET(CODPAR,DATINI,DATFIN,VALORE,ULTAGG,UTEAGG) " + " VALUES('3','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datinival_12) + "','" + DBMethods.Db2Date(scadipaOCM.datiScadenzeDipa.datfinval_12) + "','" + scadipaOCM.datiScadenzeDipa.datsca_12 + "',CURRENT TIMESTAMP,'" + u.Username + "')", CommandType.Text);
        }
        else
        {
          this.objDataAccess.WriteTransactionData("UPDATE PARGENDET SET VALORE='" + scadipaOCM.datiScadenzeDipa.datsca_1 + "',ULTAGG = CURRENT TIMESTAMP,UTEAGG='" + u.Username + "' WHERE CODPAR='3' AND DATINI='" + str + "-01-01'", CommandType.Text);
          this.objDataAccess.WriteTransactionData("UPDATE PARGENDET SET VALORE='" + scadipaOCM.datiScadenzeDipa.datsca_2 + "',ULTAGG = CURRENT TIMESTAMP,UTEAGG='" + u.Username + "' WHERE CODPAR='3' AND DATINI='" + str + "-02-01'", CommandType.Text);
          this.objDataAccess.WriteTransactionData("UPDATE PARGENDET SET VALORE='" + scadipaOCM.datiScadenzeDipa.datsca_3 + "',ULTAGG = CURRENT TIMESTAMP,UTEAGG='" + u.Username + "' WHERE CODPAR='3' AND DATINI='" + str + "-03-01'", CommandType.Text);
          this.objDataAccess.WriteTransactionData("UPDATE PARGENDET SET VALORE='" + scadipaOCM.datiScadenzeDipa.datsca_4 + "',ULTAGG = CURRENT TIMESTAMP,UTEAGG='" + u.Username + "' WHERE CODPAR='3' AND DATINI='" + str + "-04-01'", CommandType.Text);
          this.objDataAccess.WriteTransactionData("UPDATE PARGENDET SET VALORE='" + scadipaOCM.datiScadenzeDipa.datsca_5 + "',ULTAGG = CURRENT TIMESTAMP,UTEAGG='" + u.Username + "' WHERE CODPAR='3' AND DATINI='" + str + "-05-01'", CommandType.Text);
          this.objDataAccess.WriteTransactionData("UPDATE PARGENDET SET VALORE='" + scadipaOCM.datiScadenzeDipa.datsca_6 + "',ULTAGG = CURRENT TIMESTAMP,UTEAGG='" + u.Username + "' WHERE CODPAR='3' AND DATINI='" + str + "-06-01'", CommandType.Text);
          this.objDataAccess.WriteTransactionData("UPDATE PARGENDET SET VALORE='" + scadipaOCM.datiScadenzeDipa.datsca_7 + "',ULTAGG = CURRENT TIMESTAMP,UTEAGG='" + u.Username + "' WHERE CODPAR='3' AND DATINI='" + str + "-07-01'", CommandType.Text);
          this.objDataAccess.WriteTransactionData("UPDATE PARGENDET SET VALORE='" + scadipaOCM.datiScadenzeDipa.datsca_8 + "',ULTAGG = CURRENT TIMESTAMP,UTEAGG='" + u.Username + "' WHERE CODPAR='3' AND DATINI='" + str + "-08-01'", CommandType.Text);
          this.objDataAccess.WriteTransactionData("UPDATE PARGENDET SET VALORE='" + scadipaOCM.datiScadenzeDipa.datsca_9 + "',ULTAGG = CURRENT TIMESTAMP,UTEAGG='" + u.Username + "' WHERE CODPAR='3' AND DATINI='" + str + "-09-01'", CommandType.Text);
          this.objDataAccess.WriteTransactionData("UPDATE PARGENDET SET VALORE='" + scadipaOCM.datiScadenzeDipa.datsca_10 + "',ULTAGG = CURRENT TIMESTAMP,UTEAGG='" + u.Username + "' WHERE CODPAR='3' AND DATINI='" + str + "-10-01'", CommandType.Text);
          this.objDataAccess.WriteTransactionData("UPDATE PARGENDET SET VALORE='" + scadipaOCM.datiScadenzeDipa.datsca_11 + "',ULTAGG = CURRENT TIMESTAMP,UTEAGG='" + u.Username + "' WHERE CODPAR='3' AND DATINI='" + str + "-11-01'", CommandType.Text);
          this.objDataAccess.WriteTransactionData("UPDATE PARGENDET SET VALORE='" + scadipaOCM.datiScadenzeDipa.datsca_12 + "',ULTAGG = CURRENT TIMESTAMP,UTEAGG='" + u.Username + "' WHERE CODPAR='3' AND DATINI='" + str + "-12-01'", CommandType.Text);
        }
        this.objDataAccess.EndTransaction(true);
      }
      catch (Exception ex)
      {
        ErroreMSG = "Errore durante il salvataggio";
        this.objDataAccess.EndTransaction(false);
        string utente = JsonConvert.SerializeObject((object) u);
        string oggetto_Pagina = JsonConvert.SerializeObject((object) scadipaOCM);
        ErrorHandler.AggErrori(ex, utente, oggetto_Pagina);
      }
      SuccessMSG = "Salvataggio avvenuto con successo";
      return scadipaOCM;
    }
  }
}
