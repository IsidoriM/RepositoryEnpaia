// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.GestioneContrattiDAl
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Collections.Generic;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Amministrativo
{
  public class GestioneContrattiDAl
  {
    private DataLayer ObjDataAccess = new DataLayer();

    public void ModificaContratto(
      GestioneContrattiOCM.Contratti GC,
      TFI.OCM.Utente.Utente u,
      ref string MSGErorre,
      ref string MSGSuccess)
    {
      GC.checkOutput = GC.CODTIPCON + GC.RIVAUT + GC.PERSCA + GC.MAXSCA + GC.NUMMEN + GC.DATFIN + GC.DATINI + GC.Denominazione + GC.ASSCON + GC.Qualifica;
      if (GC.checkInput == GC.checkOutput)
      {
        MSGErorre = "Nessun campo modificato. Riprovare";
      }
      else
      {
        try
        {
          this.ObjDataAccess.StartTransaction();
          string strSQL = "SELECT ULTAGG FROM TB_CONTRATTI WHERE CODCON = " + GC.CODCON + " AND PROCON = " + GC.PROCON;
          modIDOC_TabelleDiServizio tabelleDiServizio = new modIDOC_TabelleDiServizio();
          this.ObjDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text);
          string str = "UPDATE TB_CONTRATTI SET" + " CODQUACON = '" + GC.Qualifica + "', " + " DATINI = '" + DBMethods.Db2Date(Convert.ToDateTime(GC.DATINI).ToString()) + "', " + " DATFIN = '" + DBMethods.Db2Date(Convert.ToDateTime(GC.DATFIN).ToString()) + "', " + " ASSCON = " + DBMethods.DoublePeakForSql(GC.ASSCON) + ", " + " DENCON = " + DBMethods.DoublePeakForSql(GC.Denominazione.Trim()) + ", ";
          bool flag = this.ObjDataAccess.WriteTransactionData((string.IsNullOrEmpty(GC.MAXSCA) ? str + " MAXSCA = 0, " + " PERSCA = 0, " + " RIVAUT = 'N', " : str + " MAXSCA = '" + GC.MAXSCA + "', " + " PERSCA = '" + GC.PERSCA + "', " + " RIVAUT = " + DBMethods.DoublePeakForSql(GC.RIVAUT.Trim().Substring(0, 1)) + ", ") + " NUMMEN = '" + Convert.ToInt16(GC.NUMMEN).ToString() + "', " + " ULTAGG = CURRENT_TIMESTAMP, " + " UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + " WHERE CODCON = " + GC.CODCON + " AND PROCON = " + GC.PROCON, CommandType.Text);
          tabelleDiServizio.WRITE_IDOC_TB_CONTRATTI_TdS(this.ObjDataAccess, GC.CODCON, GC.PROCON, "I");
          if (flag)
          {
            this.ObjDataAccess.EndTransaction(true);
            MSGSuccess = "Operazione effettuata";
          }
          else
          {
            this.ObjDataAccess.EndTransaction(false);
            MSGErorre = "Impossibile salvare errore";
          }
        }
        catch (Exception ex)
        {
          this.ObjDataAccess.EndTransaction(false);
          MSGErorre = "Impossibile salvare errore";
          throw;
        }
      }
    }

    public GestioneContrattiOCM CaricaContratto(GestioneContrattiOCM GC)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      if (dataTable2.Rows.Count > 0)
        dataTable2.Clear();
      DataTable dataTable3 = this.ObjDataAccess.GetDataTable("SELECT * FROM TB_CONTRATTI WHERE CODCON = " + GC.contratti.CODCON + " AND PROCON = " + GC.contratti.PROCON);
      if (dataTable3.Rows.Count > 0)
      {
        GC.contratti.ULTAGG = dataTable3.Rows[0]["ULTAGG"].ToString();
        GC.contratti.ASSCON = dataTable3.Rows[0]["ASSCON"].ToString();
        GC.contratti.DATINI = Convert.ToDateTime(dataTable3.Rows[0]["DATINI"]).ToString("yyyy-MM-dd");
        GC.contratti.DATFIN = Convert.ToDateTime(dataTable3.Rows[0]["DATFIN"]).ToString("yyyy-MM-dd");
        GC.contratti.NUMMEN = dataTable3.Rows[0]["NUMMEN"].ToString();
        GC.contratti.MAXSCA = dataTable3.Rows[0]["MAXSCA"].ToString();
        GC.contratti.PERSCA = dataTable3.Rows[0]["PERSCA"].ToString();
        GC.contratti.RIVAUT = dataTable3.Rows[0]["RIVAUT"].ToString();
        GC.contratti.CODQUACON = dataTable3.Rows[0]["CODQUACON"].ToString();
        DataTable dataTable4 = this.ObjDataAccess.GetDataTable("SELECT ALIAS_CONRET.DATAPPINI, ALIAS_CONRET.DATAPPFIN, " + " TB_CONTRATTO_LIVELLI.DENLIV, " + " (SELECT DENMANLIV FROM MANLIV WHERE CODMANLIV = (SELECT CODMANLIV FROM TB_CONTRATTO_LIVELLI WHERE CODCON = " + GC.contratti.CODCON + " AND PROCON = " + GC.contratti.PROCON + " AND TB_CONTRATTO_LIVELLI.CODLIV = ALIAS_CONRET.CODLIV)) AS MANLIV, " + " VOCRET.DENVOCRET, ALIAS_CONRET.IMPVOCRET, TB_CONTRATTO_LIVELLI.CODLIV , ALIAS_CONRET.CODVOCRET" + " FROM CONRET ALIAS_CONRET, VOCRET, TB_CONTRATTO_LIVELLI " + " WHERE ALIAS_CONRET.CODVOCRET = VOCRET.VOCRET " + " AND ALIAS_CONRET.CODLIV = TB_CONTRATTO_LIVELLI.CODLIV " + " AND ALIAS_CONRET.CODCON = TB_CONTRATTO_LIVELLI.CODCON" + " AND ALIAS_CONRET.PROCON = TB_CONTRATTO_LIVELLI.PROCON" + " AND ALIAS_CONRET.CODCON = " + GC.contratti.CODCON + " AND ALIAS_CONRET.PROCON = " + GC.contratti.PROCON + " ORDER BY TB_CONTRATTO_LIVELLI.DENLIV,ALIAS_CONRET.CODVOCRET, ALIAS_CONRET.DATAPPINI");
        List<GestioneContrattiOCM.Contratti> contrattiList = new List<GestioneContrattiOCM.Contratti>();
        if (dataTable4.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable4.Rows)
          {
            GestioneContrattiOCM.Contratti contratti = new GestioneContrattiOCM.Contratti()
            {
              DATAPPFIN = row["DATAPPFIN"].ToString(),
              Mansione = row["MANLIV"].ToString(),
              Importo = Convert.ToDecimal(row["IMPVOCRET"]).ToString("F"),
              DATAPPINI = row["DATAPPINI"].ToString(),
              DENLIV = row["DENLIV"].ToString()
            };
            contrattiList.Add(contratti);
          }
        }
        GC.ListCont = contrattiList;
      }
      return GC;
    }

    public void Elimina(
      GestioneContrattiOCM GC,
      TFI.OCM.Utente.Utente u,
      string CODCON,
      string PROCON,
      string DATINI,
      string DATFIN,
      ref string MSGErorre,
      ref string MSGSuccess)
    {
      modIDOC_TabelleDiServizio tabelleDiServizio = new modIDOC_TabelleDiServizio();
      bool flag = false;
      try
      {
        this.ObjDataAccess.StartTransaction();
        tabelleDiServizio.WRITE_IDOC_TB_CONTRATTI_TdS(this.ObjDataAccess, CODCON, PROCON, "D");
        if (Convert.ToDecimal((object) this.ObjDataAccess.Get1ValueFromSQL("Select COUNT(*) FROM STORDL WHERE CODCON = " + CODCON, CommandType.Text)) > 0M)
        {
          this.ObjDataAccess.EndTransaction(false);
          MSGErorre = "Attenzione! Esistono rapporti di lavoro con questo contratto. Impossibile eliminarlo";
        }
        else
        {
          flag = this.ObjDataAccess.WriteTransactionData("DELETE FROM TB_CONTRATTO_LIVELLI WHERE CODCON = " + CODCON + " AND PROCON = " + PROCON, CommandType.Text);
          flag = this.ObjDataAccess.WriteTransactionData("DELETE FROM TB_CONTRATTI WHERE CODCON = " + CODCON + " AND PROCON = " + PROCON, CommandType.Text);
          MSGSuccess = "Eliminazione del record avvenuta con successo";
          this.ObjDataAccess.EndTransaction(true);
        }
      }
      catch (Exception ex)
      {
        MSGErorre = "Eliminazione del record non andata a buon fine riprovare";
        this.ObjDataAccess.EndTransaction(false);
      }
    }

    public GestioneContrattiOCM CaricaDati(
      string Denominazione,
      string livello,
      ref string MSGErorre)
    {
      DataTable dataTable1 = new DataTable();
      GestioneContrattiOCM gestioneContrattiOcm = new GestioneContrattiOCM();
      string str1 = "SELECT QUACON.DENQUA, " + " TB_CONTRATTI.CODCON, TB_CONTRATTI.PROCON, TB_CONTRATTI.DENCON, " + " TB_CONTRATTI.DATINI, TB_CONTRATTI.DATFIN, " + " TB_CONTRATTI.ASSCON, TB_CONTRATTI.MAXSCA, TB_CONTRATTI.PERSCA, TB_CONTRATTI.NUMMEN, TB_CONTRATTI.RIVAUT " + " FROM TB_CONTRATTI, QUACON " + " WHERE TB_CONTRATTI.CODQUACON = QUACON.CODQUACON ";
      string str2 = " AND TB_CONTRATTI.PROCON = (SELECT MAX(PROCON) FROM TB_CONTRATTI WHERE TB_CONTRATTI.DENCON LIKE'%" + Denominazione.Trim().ToUpper() + "%')";
      if (!string.IsNullOrEmpty(livello))
        str2 = str2 + " AND TB_CONTRATTI.CODQUACON = " + livello;
      if (!string.IsNullOrEmpty(Denominazione))
        str2 = str2 + " AND TB_CONTRATTI.DENCON LIKE '%" + Denominazione.Trim().ToUpper() + "%'";
      DataTable dataTable2 = this.ObjDataAccess.GetDataTable(str1 + str2 + " ORDER BY TB_CONTRATTI.DENCON, TB_CONTRATTI.DATINI");
      List<GestioneContrattiOCM.Contratti> contrattiList = new List<GestioneContrattiOCM.Contratti>();
      if (dataTable2.Rows.Count > 0)
      {
        foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
        {
          GestioneContrattiOCM.Contratti contratti = new GestioneContrattiOCM.Contratti()
          {
            DATFIN = row["DATFIN"].ToString(),
            Denominazione = row["DENCON"].ToString(),
            CODCON = row["CODCON"].ToString(),
            Qualifica = row["DENQUA"].ToString(),
            PROCON = row["PROCON"].ToString(),
            ASSCON = row["ASSCON"].ToString(),
            DATINI = row["DATINI"].ToString(),
            NUMMEN = row["NUMMEN"].ToString(),
            MAXSCA = row["MAXSCA"].ToString(),
            PERSCA = row["PERSCA"].ToString(),
            RIVAUT = row["RIVAUT"].ToString()
          };
          contrattiList.Add(contratti);
        }
      }
      else
        MSGErorre = "Non sono stati trovati contratti per questa ricerca";
      gestioneContrattiOcm.ListCont = contrattiList;
      return gestioneContrattiOcm;
    }

    public void SalvaNuovoContratto(
      GestioneContrattiOCM.Contratti gc,
      TFI.OCM.Utente.Utente u,
      ref string MSGErorre,
      ref string MSGSuccess)
    {
      try
      {
        this.ObjDataAccess.StartTransaction();
        modIDOC_TabelleDiServizio tabelleDiServizio = new modIDOC_TabelleDiServizio();
        string CODCON = this.ObjDataAccess.Get1ValueFromSQL("SELECT VALUE(MAX(CODCON),0) + 1 FROM TB_CONTRATTI", CommandType.Text);
        string PROCON = "1";
        string str1 = "Insert Into TB_CONTRATTI (CODCON , PROCON, CODQUACON, DENCON," + " DATINI, DATFIN, " + "ASSCON, MAXSCA, PERSCA,NUMMEN, RIVAUT, " + " ULTAGG , UTEAGG)" + " Values ( '" + CODCON + "', '" + PROCON + "', '" + gc.Qualifica + "', '" + gc.Denominazione.Trim().ToUpper() + "', '" + DBMethods.Db2Date(gc.DATINI) + "', '" + DBMethods.Db2Date(gc.DATFIN) + "', '" + gc.ASSCON + "', '";
        string str2;
        if (gc.MAXSCA != "")
          str2 = str1 + gc.MAXSCA + "', '" + gc.PERSCA + "','" + gc.NUMMEN + "' ,'" + gc.RIVAUT + "', ";
        else
          str2 = str1 + "0, 0,0, 'N', ";
        bool flag = this.ObjDataAccess.WriteTransactionData(str2 + " CURRENT_TIMESTAMP, '" + u.Username + "')", CommandType.Text);
        tabelleDiServizio.WRITE_IDOC_TB_CONTRATTI_TdS(this.ObjDataAccess, CODCON, PROCON, "I");
        if (flag)
        {
          this.ObjDataAccess.EndTransaction(true);
          MSGSuccess = "Operazione effettuata";
        }
        else
        {
          this.ObjDataAccess.EndTransaction(false);
          MSGErorre = "Impossibile salvare errore";
        }
      }
      catch (Exception ex)
      {
        this.ObjDataAccess.EndTransaction(false);
        MSGErorre = "Impossibile salvare errore";
        throw;
      }
    }
  }
}
