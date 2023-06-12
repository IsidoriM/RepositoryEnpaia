// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.NotificheUfficioBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using TFI.BLL.Utilities;
using TFI.DAL.Amministrativo;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Amministrativo
{
  public class NotificheUfficioBLL
  {
    public static string errorMessage;
    public static string infoMessage;
    private static readonly ILog log = LogManager.GetLogger("RollingFile");
    private static readonly ILog TrackLog = LogManager.GetLogger("Track");
    private readonly NotificheUfficioDAL Dal = new NotificheUfficioDAL();

    public static Dictionary<int, string> PopolaDDLMesi() => Utils.GetMesi();

    public static bool AutorizzazioniSpecialiSoloEnpaia(ref int codPos, string sistema) => new Utile().Module_Autorizzazioni_Speciali_Solo_Enpaia(ref codPos, sistema);

    public static bool AutorizzazioniSpeciali(string codPos, string sistema) => new Utile().Module_Autorizzazioni_Speciali(codPos, sistema);

    public static List<NotificheUfficioOCM> GetData(
      string msk_codPos_da,
      string msk_codPos_a,
      string msk_anno,
      int msk_meseDa,
      int msk_meseA)
    {
      string empty = string.Empty;
      int num1 = 0;
      int num2 = 0;
      string dataSistema = Utile.GetDataSistema();
      List<NotificheUfficioOCM> listaNotifiche = new List<NotificheUfficioOCM>();
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      DataLayer objDataAccess = new DataLayer();
      DateTime dateTime1 = Convert.ToDateTime(dataSistema);
      dateTime1 = dateTime1.AddMonths(-1);
      string strDataDecorrenza = dateTime1.ToString().Trim();
      int year = Convert.ToDateTime(strDataDecorrenza).Year;
      int month = Convert.ToDateTime(strDataDecorrenza).Month;
      string str = clsWriteDb.Module_GetValorePargen(objDataAccess, 3, strDataDecorrenza).ToString();
      if (str != string.Empty)
      {
        if (Convert.ToDateTime(str) < Convert.ToDateTime(dataSistema))
        {
          num1 = month;
          num2 = year;
        }
        else if (month == 1)
        {
          num1 = 12;
          num2 = year - 1;
        }
        else
        {
          num1 = month - 1;
          num2 = year;
        }
      }
      int num3;
      int num4;
      if (!string.IsNullOrEmpty(msk_anno))
      {
        if (Convert.ToInt32(msk_anno.PadLeft(4, '0') + msk_meseDa.ToString().PadLeft(2, '0')) > Convert.ToInt32(num2.ToString().PadLeft(4, '0') + num1.ToString().PadLeft(2, '0')))
        {
          NotificheUfficioBLL.errorMessage = "Impossibile notificare per il periodo selezionato";
          return (List<NotificheUfficioOCM>) null;
        }
        num3 = Convert.ToInt32(msk_anno);
        num4 = msk_meseDa;
        if (num2 == Convert.ToInt32(msk_anno))
        {
          if (msk_meseA < num1)
            num1 = msk_meseA;
        }
        else
        {
          num2 = Convert.ToInt32(msk_anno);
          num1 = msk_meseA;
        }
      }
      else
      {
        string dataDecorrenza = NotificheUfficioDAL.GetDataDecorrenza(msk_codPos_da);
        if (dataDecorrenza == string.Empty)
        {
          NotificheUfficioBLL.errorMessage = "Nessun rapporto presente per la posizione in questione";
          return (List<NotificheUfficioOCM>) null;
        }
        DateTime dateTime2 = Convert.ToDateTime(dataDecorrenza);
        num3 = dateTime2.Year;
        dateTime2 = Convert.ToDateTime(dataDecorrenza);
        num4 = dateTime2.Month;
      }
      if (string.IsNullOrEmpty(msk_anno))
      {
        if (num3 < 2003)
          num3 = 2003;
        if (num2 < 2003)
          num2 = 2003;
      }
      else
      {
        if (num3 < 2003)
        {
          NotificheUfficioBLL.errorMessage = "Impossibile notificare con questa funzione per gli anni ante 2003";
          return (List<NotificheUfficioOCM>) null;
        }
        if (num2 < 2003)
        {
          NotificheUfficioBLL.errorMessage = "Impossibile notificare con questa funzione per gli anni ante 2003";
          return (List<NotificheUfficioOCM>) null;
        }
      }
      for (int index1 = num3; index1 <= num2; ++index1)
      {
        int num5;
        int num6;
        if (index1 == num3)
        {
          if (index1 < num2)
          {
            num5 = num4;
            num6 = 12;
          }
          else
          {
            num5 = num4;
            num6 = num1;
          }
        }
        else if (index1 < num2)
        {
          num5 = 1;
          num6 = 12;
        }
        else
        {
          num5 = 1;
          num6 = num1;
        }
        for (int index2 = num5; index2 <= num6; ++index2)
        {
          string dataDa = "01/" + index2.ToString().Trim().PadLeft(2, '0') + "/" + index1.ToString().Trim();
          NotificheUfficioDAL.GetData(DateTime.DaysInMonth(index1, index2).ToString().Trim().PadLeft(2, '0') + "/" + index2.ToString().Trim().PadLeft(2, '0') + "/" + index1.ToString().Trim(), dataDa, index1, index2, msk_codPos_da, msk_codPos_a, empty, listaNotifiche);
        }
      }
      if (listaNotifiche.Count != 0)
        return listaNotifiche;
      NotificheUfficioBLL.errorMessage = "Nessuna occorrenza trovata per i parametri specificati";
      return (List<NotificheUfficioOCM>) null;
    }

    public static string GetDataSistema()
    {
      try
      {
        return Utile.GetDataSistema();
      }
      catch (Exception ex)
      {
        NotificheUfficioBLL.log.Info((object) string.Format("[TFI.BLL] : NotificheUfficio - La funzione GetDataSistema() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
        NotificheUfficioBLL.errorMessage = "L'applicazione ha generato un'eccezione " + ex.Message;
        return (string) null;
      }
    }

    public static bool GeneraSalvaNotifiche(
      TFI.OCM.Utente.Utente utente,
      DataTable dtAziende,
      DataTable dtLog,
      string tipMovSan,
      string dataRiferimento,
      string strRicPrev)
    {
      DataLayer objDataAccess = new DataLayer();
      try
      {
        objDataAccess.StartTransaction();
        ModGeneraNotifiche modGeneraNotifiche = new ModGeneraNotifiche();
        DataTable dt = modGeneraNotifiche.Module_Genera_Notifiche(objDataAccess, utente, ref dtAziende, ref dtLog, tipMovSan, dataRiferimento, strRicPrev);
        if (dt.Rows.Count > 0)
        {
          if (modGeneraNotifiche.Module_SalvaNotifiche(objDataAccess, utente, ref dtAziende, ref dt, tipMovSan, dataRiferimento))
          {
            objDataAccess.EndTransaction(true);
            return true;
          }
          objDataAccess.EndTransaction(false);
          return false;
        }
        NotificheUfficioBLL.errorMessage = "Nessuna notifica da generare";
        objDataAccess.EndTransaction(false);
        return false;
      }
      catch (Exception ex)
      {
        objDataAccess.EndTransaction(false);
        NotificheUfficioBLL.log.Info((object) string.Format("[TFI.BLL] : NotificheUfficio - La funzione GeneraNotifiche() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
        NotificheUfficioBLL.errorMessage = "L'applicazione ha generato un'eccezione: " + ex.Message;
        return false;
      }
    }

    public static string GetCausaleMovimento(string tipMov)
    {
      try
      {
        return NotificheUfficioDAL.GetCausaleMovimento(tipMov);
      }
      catch (Exception ex)
      {
        NotificheUfficioBLL.log.Info((object) string.Format("[TFI.BLL] : NotificheUfficio - La funzione GetCausaleMovimento() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
        NotificheUfficioBLL.errorMessage = "L'applicazione ha generato un'eccezione: " + ex.Message;
        return (string) null;
      }
    }

    public static List<TFI.OCM.Amministrativo.CercaNotifiche> CercaNotifiche(
      ref Decimal tempoMassimoPrevisto,
      ref string tempoInizio)
    {
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      DataLayer objDataAccess = new DataLayer();
      try
      {
        tempoMassimoPrevisto = Convert.ToDecimal(clsWriteDb.Module_GetValorePargen(objDataAccess, 12));
        tempoInizio = clsWriteDb.Module_GetDataSistema(objDataAccess);
        List<TFI.OCM.Amministrativo.CercaNotifiche> cercaNotificheList = NotificheUfficioDAL.CercaNotifiche();
        if (cercaNotificheList != null)
          return cercaNotificheList;
        NotificheUfficioBLL.errorMessage = "Nessuna occorrenza trovata";
        return (List<TFI.OCM.Amministrativo.CercaNotifiche>) null;
      }
      catch (Exception ex)
      {
        NotificheUfficioBLL.log.Info((object) string.Format("[TFI.BLL] : NotificheUfficio - La funzione CercaNotifiche() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
        NotificheUfficioBLL.errorMessage = "L'applicazione ha generato un'eccezione: " + ex.Message;
        return (List<TFI.OCM.Amministrativo.CercaNotifiche>) null;
      }
    }

    public static List<NotificheBase> Contabilizza_Step2(
      List<TFI.OCM.Amministrativo.CercaNotifiche> listaNotifiche,
      string dataRiferimento,
      TFI.OCM.Utente.Utente utente,
      ref bool cont_effettuata,
      string filePath)
    {
      bool flag1 = false;
      bool flag2 = false;
      int num = 0;
      string PARTITA_MOVIMENTO = "";
      string PARTITA_SANZIONE = "";
      string NUMERO_SANZIONE_SAP = "";
      Decimal PROGMOV_SANZIONE = 0M;
      Decimal PROGMOV_MOVIMENTO = 0M;
      DataLayer dataLayer = new DataLayer();
      DataTable dataTable = new DataTable();
      DataTable DT = new DataTable();
      ModGetDati modGetDati = new ModGetDati();
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      IDOC_RDL idocRdl = new IDOC_RDL();
      ExcelExport excelExport = new ExcelExport();
      try
      {
        dataLayer.StartTransaction();
        flag2 = true;
        string[] strArray = NotificheUfficioDAL.Module_GetNewTimeStamp(dataLayer).Split(' ');
        string str1 = strArray[0].Replace("/", "-");
        string str2 = strArray[1].Replace(".", "_");
        if (str2.Length < 8)
          str2 = "0" + str2;
        string str3 = str1 + " " + str2;
        string str4 = !(dataRiferimento == string.Empty) ? dataRiferimento : Utile.GetDataSistema();
        string causaleMovimento = NotificheUfficioDAL.GetCausaleMovimento("NU");
        foreach (TFI.OCM.Amministrativo.CercaNotifiche cercaNotifiche in listaNotifiche)
        {
          if (cercaNotifiche.Checked == "S")
          {
            if (!NotificheUfficioDAL.ChecK_Dipa_Confermato(dataLayer, cercaNotifiche.CodPos, cercaNotifiche.AnnDen, cercaNotifiche.MesDen))
            {
              int annoBilancio = modGetDati.Module_GetAnnoBilancio(dataLayer, false, cercaNotifiche.AnnDen);
              string tipoAnno = modGetDati.Module_GetTipoAnno(cercaNotifiche.AnnDen, annoBilancio);
              string NUMERO_MOVIMENTO_SAP;
              if (cercaNotifiche.ImpDis == 0M)
              {
                NUMERO_MOVIMENTO_SAP = "000000000";
                flag1 = true;
              }
              else
              {
                NUMERO_MOVIMENTO_SAP = clsWriteDb.WRITE_INSERT_MOVIMSAP(dataLayer, utente, ref NotificheUfficioBLL.errorMessage, cercaNotifiche.CodPos, cercaNotifiche.AnnDen, cercaNotifiche.MesDen, cercaNotifiche.ProDen, causaleMovimento, str4, annoBilancio, cercaNotifiche.DatSca, cercaNotifiche.ImpDis, cercaNotifiche.ImpAbb, cercaNotifiche.ImpAddRec, cercaNotifiche.ImpAssCon, "", "", tipoAnno, ref PARTITA_MOVIMENTO, ref PROGMOV_MOVIMENTO, ref PARTITA_SANZIONE, ref PROGMOV_SANZIONE, TIPO_OPERAZIONE: "NOTIFICHE");
                NUMERO_SANZIONE_SAP = string.Empty;
                if (cercaNotifiche.CodCauSan != string.Empty && cercaNotifiche.SanSotSog != "S")
                  NUMERO_SANZIONE_SAP = clsWriteDb.WRITE_INSERT_MOVIMSAP(dataLayer, utente, ref NotificheUfficioBLL.errorMessage, cercaNotifiche.CodPos, cercaNotifiche.AnnDen, cercaNotifiche.MesDen, cercaNotifiche.ProDen, cercaNotifiche.CodCauSan, str4, annoBilancio, cercaNotifiche.DatSca, cercaNotifiche.ImpSanDet, 0M, 0M, 0M, "S", "", tipoAnno, ref PARTITA_MOVIMENTO, ref PROGMOV_MOVIMENTO, ref PARTITA_SANZIONE, ref PROGMOV_SANZIONE, TIPO_OPERAZIONE: "SANZIONI NOTIFICHE");
              }
              string strNomFileMav = string.Format("NU {0}-{1} {2}", (object) cercaNotifiche.AnnDen, (object) cercaNotifiche.MesDen.ToString().PadLeft(2, '0'), (object) str3);
              NotificheUfficioDAL.Contabilizza_Notifiche(dataLayer, cercaNotifiche.CodPos, cercaNotifiche.AnnDen, cercaNotifiche.MesDen, cercaNotifiche.ProDen, causaleMovimento, NUMERO_MOVIMENTO_SAP, cercaNotifiche.CodCauSan.Trim(), NUMERO_SANZIONE_SAP, annoBilancio, tipoAnno, str4, strNomFileMav, PARTITA_MOVIMENTO, PROGMOV_MOVIMENTO, PARTITA_SANZIONE, PROGMOV_SANZIONE, utente);
              dataTable.Clear();
              dataTable = idocRdl.GET_IDOC_DATI_E1PITYPE(dataLayer, "9004", cercaNotifiche.CodPos, 0, 0, 0, "", "", "9999-12-31", "", "", "", cercaNotifiche.AnnDen, cercaNotifiche.MesDen, cercaNotifiche.ProDen, "", "", "NU", "");
              foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
              {
                idocRdl.WRITE_IDOC_TESTATA(dataLayer, row);
                DT.Clear();
                DT = dataTable.Clone();
                DT.ImportRow(row);
                idocRdl.WRITE_IDOC_E1PITYP(dataLayer, "9004", DT, false);
              }
            }
            else
            {
              ++num;
              cercaNotifiche.CancellateDalDIPA = "S";
              NotificheUfficioDAL.Cancella_Notifiche(dataLayer, cercaNotifiche.CodPos, cercaNotifiche.AnnDen, cercaNotifiche.MesDen, cercaNotifiche.ProDen);
            }
          }
          else
          {
            ++num;
            cercaNotifiche.CancellateDalDIPA = "S";
            NotificheUfficioDAL.Cancella_Notifiche(dataLayer, cercaNotifiche.CodPos, cercaNotifiche.AnnDen, cercaNotifiche.MesDen, cercaNotifiche.ProDen);
          }
          PARTITA_MOVIMENTO = "";
          PROGMOV_MOVIMENTO = 0M;
          PARTITA_SANZIONE = "";
          PROGMOV_SANZIONE = 0M;
        }
        idocRdl.Aggiorna_IDOC(dataLayer);
        idocRdl.objDtCONTIDOC = (DataTable) null;
        dataLayer.EndTransaction(true);
        flag2 = false;
        cont_effettuata = true;
        List<NotificheBase> items = new List<NotificheBase>();
        foreach (TFI.OCM.Amministrativo.CercaNotifiche cercaNotifiche in listaNotifiche)
        {
          if (cercaNotifiche.Checked == "S" && cercaNotifiche.CancellateDalDIPA != "S" && cercaNotifiche.ImpDis != 0M)
            items.Add(new NotificheBase()
            {
              CodPos = cercaNotifiche.CodPos,
              AnnDen = cercaNotifiche.AnnDen,
              MesDen = cercaNotifiche.MesDen,
              ProDen = cercaNotifiche.ProDen
            });
        }
        if (items.Count > 0)
        {
          new Pdf().MODULE_STAMPA_DOCUMENTO_DENUNCIA(0, 0, 0, 0, new ListtoDataTableConverter().ToDataTable<NotificheBase>(items));
          if (num > 0)
          {
            NotificheUfficioBLL.infoMessage = "Alcune notifiche non sono state contabilizzate a seguito di Dipa confermati dalla/e Aziende (vedi elenco posizioni su file Excel).";
            items.Clear();
            foreach (TFI.OCM.Amministrativo.CercaNotifiche cercaNotifiche in listaNotifiche)
            {
              if (cercaNotifiche.Checked == "S" && cercaNotifiche.CancellateDalDIPA == "S")
                items.Add(new NotificheBase()
                {
                  CodPos = cercaNotifiche.CodPos,
                  AnnDen = cercaNotifiche.AnnDen,
                  MesDen = cercaNotifiche.MesDen,
                  ProDen = cercaNotifiche.ProDen
                });
            }
            int count = items.Count;
            return items;
          }
        }
        else if (!flag1)
          NotificheUfficioBLL.infoMessage = "Le notifiche sono state cancellate a seguito di Dipa confermati dalla/e Aziende. Nessuna notifica è stata contabilizzata";
        return items;
      }
      catch (Exception ex)
      {
        if (flag2)
          dataLayer.EndTransaction(false);
        NotificheUfficioBLL.log.Info((object) string.Format("[TFI.BLL] : NotificheUfficio - La funzione Contabilizza_Step2() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
        NotificheUfficioBLL.errorMessage = "L'applicazione ha generato un'eccezione: " + ex.Message;
        return (List<NotificheBase>) null;
      }
    }

    public static void ContabilizzazioneNotifiche_exit(List<TFI.OCM.Amministrativo.CercaNotifiche> listaNotifiche)
    {
      DataLayer db = new DataLayer();
      try
      {
        db.StartTransaction();
        foreach (TFI.OCM.Amministrativo.CercaNotifiche cercaNotifiche in listaNotifiche)
          NotificheUfficioDAL.Cancella_Notifiche(db, cercaNotifiche.CodPos, cercaNotifiche.AnnDen, cercaNotifiche.MesDen, cercaNotifiche.ProDen);
        db.EndTransaction(true);
      }
      catch (Exception ex)
      {
        db.EndTransaction(false);
        NotificheUfficioBLL.log.Info((object) string.Format("[TFI.BLL] : NotificheUfficio - La funzione ContabilizzazioneNotifiche_exit() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
        NotificheUfficioBLL.errorMessage = "L'applicazione ha generato un'eccezione: " + ex.Message;
      }
    }

    public static List<NotificheUfficioOCM> CaricaDatiDettaglioImporti(
      NotificheUfficioOCM notifiche,
      string MovRet)
    {
      try
      {
        List<NotificheUfficioOCM> notificheUfficioOcmList = NotificheUfficioDAL.CaricaDati(notifiche, MovRet);
        if (notificheUfficioOcmList != null)
          return notificheUfficioOcmList;
        NotificheUfficioBLL.errorMessage = "Nessuna occorrenza trovata";
        return (List<NotificheUfficioOCM>) null;
      }
      catch (Exception ex)
      {
        NotificheUfficioBLL.log.Info((object) string.Format("[TFI.BLL]: Dettaglio Importi - La funzione CaricaDatiDettaglioImporti() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
        NotificheUfficioBLL.errorMessage = "L'applicazione ha generato un'eccezione :" + ex.Message;
        return (List<NotificheUfficioOCM>) null;
      }
    }

    public static List<NotificheUfficioOCM> DettaglioDenuncia(
      NotificheUfficioOCM notifiche,
      ref string RagSoc,
      ref string tipomovimento)
    {
      try
      {
        bool DOCUMENTO_ORIGINALE = false;
        List<NotificheUfficioOCM> notificheUfficioOcmList = NotificheUfficioDAL.DettaglioDenuncia(notifiche, DOCUMENTO_ORIGINALE, ref RagSoc, ref tipomovimento);
        if (notificheUfficioOcmList != null)
          return notificheUfficioOcmList;
        NotificheUfficioBLL.errorMessage = "Nessuna occorrenza trovata";
        return (List<NotificheUfficioOCM>) null;
      }
      catch (Exception ex)
      {
        NotificheUfficioBLL.log.Info((object) string.Format("[TFI.BLL] : Dettaglio Denuncia - La funzione DettaglioDenuncia() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
        NotificheUfficioBLL.errorMessage = "L'applicazione ha generato un'eccezione: " + ex.Message;
        return (List<NotificheUfficioOCM>) null;
      }
    }

    public static StampaDocumentoNotifiche GetStampaObj(
      string codPos,
      string annDen,
      string mesDen,
      string proDen)
    {
      try
      {
        return NotificheUfficioDAL.GetStampaObj(codPos, annDen, mesDen, proDen);
      }
      catch (Exception ex)
      {
        NotificheUfficioBLL.log.Info((object) string.Format("[TFI.BLL] : Dettaglio Denuncia - La funzione GetStampaObj() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
        NotificheUfficioBLL.errorMessage = "L'applicazione ha generato un'eccezione: " + ex.Message;
        return (StampaDocumentoNotifiche) null;
      }
    }

    public static List<StampaDocumentoNotifiche> GetStampaList(
      string codPos,
      string annDen,
      string mesDen,
      string proDen)
    {
      try
      {
        return NotificheUfficioDAL.GetStampaList(codPos, annDen, mesDen, proDen);
      }
      catch (Exception ex)
      {
        NotificheUfficioBLL.log.Info((object) string.Format("[TFI.BLL] : Dettaglio Denuncia - La funzione GetStampaList() ha generato un'eccezione in data: {0}", (object) DateTime.Now));
        NotificheUfficioBLL.errorMessage = "L'applicazione ha generato un'eccezione: " + ex.Message;
        return (List<StampaDocumentoNotifiche>) null;
      }
    }
  }
}
