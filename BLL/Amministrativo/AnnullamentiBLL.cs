// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.AnnullamentiBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using TFI.BLL.Utilities;
using TFI.DAL.Amministrativo;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Amministrativo
{
  public class AnnullamentiBLL
  {
    private static readonly ILog log = LogManager.GetLogger("RollingFile");
    private static readonly ILog TrackLog = LogManager.GetLogger("Track");
    public static string ErrorMessage;
    public static string SuccessMessage;
    private clsSediinNet2010.clsSediinNet2010 objNet = new clsSediinNet2010.clsSediinNet2010();

    public static Dictionary<int, string> PopolaDDLMesi() => Utils.GetMesi();

    public static List<TipiMovimento> PopolaDDLTipiMovimento()
    {
      try
      {
        return AnnullamentiDAL.GetTipiMovimento();
      }
      catch (Exception ex)
      {
        AnnullamentiBLL.log.Info((object) string.Format("[TFI.BLL] : Annullamento Denunce - La funzione PopolaDDLTipiMovimento() ha generato un'eccezione in data: {0} - ErrorMessage: {1}", (object) DateTime.Now, (object) ex.Message));
        AnnullamentiBLL.ErrorMessage = "L'applicazione ha generato un'eccezione in fase di caricamento dati";
        return (List<TipiMovimento>) null;
      }
    }

    public static string GetRagioneSociale(string codPos)
    {
      try
      {
        return AnnullamentiDAL.GetRagioneSociale(codPos);
      }
      catch (Exception ex)
      {
        AnnullamentiBLL.log.Info((object) string.Format("[TFI.BLL] : Annullamento Denunce - La funzione GetRagioneSociale() ha generato un'eccezione in data: {0} - ErrorMessage: {1}", (object) DateTime.Now, (object) ex.Message));
        return (string) null;
      }
    }

    public static bool AutorizzazioniSpecialiSoloEnpaia(ref int codPos, string sistema) => new Utile().Module_Autorizzazioni_Speciali_Solo_Enpaia(ref codPos, sistema);

    public static bool AutorizzazioniSpeciali(string codPos, string sistema) => new Utile().Module_Autorizzazioni_Speciali(codPos, sistema);

    public static List<AnnullamentoDenunceOCM> CaricaDati(
      string codPos,
      string anno_da,
      string mese_da,
      string anno_a,
      string mese_a,
      string tipMov)
    {
      try
      {
        List<AnnullamentoDenunceOCM> annullamentoDenunceOcmList;
        if ((annullamentoDenunceOcmList = AnnullamentiDAL.CaricaDati(codPos, anno_da, mese_da, anno_a, mese_a, tipMov)) != null)
          return annullamentoDenunceOcmList;
        AnnullamentiBLL.ErrorMessage = "Nessun dato trovato.";
        return (List<AnnullamentoDenunceOCM>) null;
      }
      catch (Exception ex)
      {
        AnnullamentiBLL.log.Info((object) string.Format("[TFI.BLL] : Annullamento Denunce - La funzione CaricaDati() ha generato un'eccezione in data: {0} - ErrorMessage: {1}", (object) DateTime.Now, (object) ex.Message));
        AnnullamentiBLL.ErrorMessage = "L'applicazione ha generato un'eccezione in fase di caricamento dati";
        return (List<AnnullamentoDenunceOCM>) null;
      }
    }

    public static List<string> AnnullaDenuncia_Step1(
      List<AnnullamentoDenunceOCM> listaDenunceDaAnnullare)
    {
      bool flag = true;
      int num1 = 0;
      string str = listaDenunceDaAnnullare[0].OldTipMov.Trim();
      Decimal num2 = 0M;
      Decimal num3 = 0M;
      Decimal num4 = 0M;
      Decimal num5 = 0M;
      Decimal num6 = 0M;
      Decimal num7 = 0M;
      Decimal num8 = 0M;
      Decimal num9 = 0M;
      ImportiAnnullamentoDenunce annullamentoDenunce = new ImportiAnnullamentoDenunce();
      List<string> stringList = new List<string>();
      List<ImportiFromDENTES> importiFromDentesList = new List<ImportiFromDENTES>();
      try
      {
        foreach (AnnullamentoDenunceOCM denuncia in listaDenunceDaAnnullare)
        {
          flag = false;
          importiFromDentesList = AnnullamentiDAL.GetDataFromDentes(denuncia);
          if (importiFromDentesList != null && importiFromDentesList.Count > 1)
          {
            if (!(str == "DP") && !(str == "NU"))
            {
              if (str == "AR")
              {
                stringList.Add(string.Format("Attenzione...insieme all'ARRETRATO di {0} {1} verrà annullato anche il DIPA collegato di euro {2}. Vuoi continuare?", (object) Utile.GetMesi()[denuncia.Mese], (object) denuncia.AnnDen, (object) importiFromDentesList[1].ImpDis));
                flag = true;
                num2 += importiFromDentesList[1].ImpAbb;
                num3 += importiFromDentesList[1].ImpCon;
                if (importiFromDentesList[1].NumSan.Trim() != string.Empty && importiFromDentesList[1].NumSanAnn.Trim() == string.Empty)
                  num4 += importiFromDentesList[1].ImpSanDet;
                num5 += importiFromDentesList[1].ImpAddRec;
                num6 += importiFromDentesList[1].ImpAssCon;
                num7 += importiFromDentesList[1].ImpConDel;
                if (importiFromDentesList[1].ImpAddRecDel != 0M)
                  num8 += importiFromDentesList[1].ImpAddRecDel;
                num9 += importiFromDentesList[1].ImpSanRet;
              }
            }
            else
            {
              stringList.Add(string.Format("Attenzione...insieme al DIPA di {0} {1} verrà annullato anche l'ARRETRATO collegato di euro {2}. Vuoi continuare?", (object) Utile.GetMesi()[denuncia.Mese], (object) denuncia.AnnDen, (object) importiFromDentesList[0].ImpDis));
              flag = true;
              num2 += importiFromDentesList[0].ImpAbb;
              num3 += importiFromDentesList[0].ImpCon;
              if (importiFromDentesList[0].NumSan.Trim() != string.Empty && importiFromDentesList[0].NumSanAnn.Trim() == string.Empty)
                num4 += importiFromDentesList[0].ImpSanDet;
              num5 += importiFromDentesList[0].ImpAddRec;
              num6 += importiFromDentesList[0].ImpAssCon;
              num7 += importiFromDentesList[0].ImpConDel;
              if (importiFromDentesList[0].ImpAddRecDel != 0M)
                num8 += importiFromDentesList[0].ImpAddRecDel;
              num9 += importiFromDentesList[0].ImpSanRet;
            }
          }
          num2 += denuncia.ImpAbb;
          num3 += denuncia.ImpCon;
          if (denuncia.NumSan != string.Empty && denuncia.NumSanAnn == string.Empty)
            num4 += denuncia.ImpSanDet;
          num5 += denuncia.ImpAddRec;
          num6 += denuncia.ImpAssCon;
          num7 += denuncia.ImpConDel;
          if (denuncia.ImpAddRecDel != 0M)
            num8 += denuncia.ImpAddRecDel;
          num9 += denuncia.ImpSanRet;
          ++num1;
        }
        annullamentoDenunce.Importo_abb = num2;
        annullamentoDenunce.Importo_contributo = num3;
        annullamentoDenunce.Importo_sanzione = num4;
        annullamentoDenunce.Importo_addizionale = num5;
        annullamentoDenunce.Importo_assistenza = num6;
        annullamentoDenunce.Importo_rett_contrib = num7;
        annullamentoDenunce.Importo_rett_addiz = num8;
        annullamentoDenunce.Importo_rett_sanzione = num9;
        annullamentoDenunce.FlgArr = flag;
        HttpContext.Current.Session["objImporti_annullDen"] = (object) annullamentoDenunce;
        HttpContext.Current.Session["listaImporti_annullDen"] = (object) importiFromDentesList;
        return stringList;
      }
      catch (Exception ex)
      {
        AnnullamentiBLL.log.Info((object) string.Format("[TFI.BLL] : Annullamento Denunce - La funzione AnnullaDenuncia_Step1() ha generato un'eccezione in data: {0} - ErrorMessage: {1}", (object) DateTime.Now, (object) ex.Message));
        AnnullamentiBLL.ErrorMessage = "L'applicazione ha generato un'eccezione in fase di elaborazione dati";
        return (List<string>) null;
      }
    }

    public static bool AnnullaDenuncia_Step3(
      List<AnnullamentoDenunceOCM> listaDenunceDaAnnullare,
      bool flgArr,
      List<ImportiFromDENTES> listaImporti,
      TFI.OCM.Utente.Utente utente)
    {
      Pdf pdf = new Pdf();
      Decimal PROGMOV_MOVIMENTO = 0M;
      string PARTITA_MOVIMENTO = string.Empty;
      string codCauAnn = string.Empty;
      string numeroMovimentoSap = string.Empty;
      string empty = string.Empty;
      string str1 = string.Empty;
      string str2 = string.Empty;
      string numSanAnn = string.Empty;
      string tipMov = listaDenunceDaAnnullare[0].OldTipMov.Trim();
      Decimal num1 = 0M;
      Decimal addizionale = 0M;
      Decimal impConRet = 0M;
      Decimal impAddRecRet = 0M;
      Decimal impAbbRet = 0M;
      Decimal impAssConRet = 0M;
      ModGetDati modGetDati = new ModGetDati();
      ModContabilita modContabilita = new ModContabilita();
      IDOC_RDL idocRdl = new IDOC_RDL();
      List<AnnullamentoDenunceOCM> annullamentoDenunceOcmList = new List<AnnullamentoDenunceOCM>();
      List<DENDET_Data> list_stampeRet = new List<DENDET_Data>();
      List<AnnullamentoDenunceOCM> items = new List<AnnullamentoDenunceOCM>();
      DataTable dataTable1 = new DataTable();
      DataTable DT = new DataTable();
      DataTable dataTable2 = new DataTable();
      clsWRITE_DB clsWRITE_DB = new clsWRITE_DB();
      DataLayer dataLayer = new DataLayer();
      try
      {
        dataLayer.StartTransaction();
        string dataSistema = AnnullamentiDAL.GetDataSistema(dataLayer);
        foreach (AnnullamentoDenunceOCM annullamentoDenunceOcm in listaDenunceDaAnnullare)
        {
          int codPos = annullamentoDenunceOcm.CodPos;
          int annDen = annullamentoDenunceOcm.AnnDen;
          int mese = annullamentoDenunceOcm.Mese;
          int proDen = annullamentoDenunceOcm.ProDen;
          Decimal impDis = annullamentoDenunceOcm.ImpDis;
          Decimal impAbb = annullamentoDenunceOcm.ImpAbb;
          Decimal impSanDet = annullamentoDenunceOcm.ImpSanDet;
          Decimal impAddRec = annullamentoDenunceOcm.ImpAddRec;
          Decimal impAssCon = annullamentoDenunceOcm.ImpAssCon;
          string numMov = annullamentoDenunceOcm.NumMov;
          string filtroSql = string.Format(" WHERE CODPOS = {0} AND ANNDEN = {1} AND MESDEN = {2} AND PRODEN = {3}", (object) codPos, (object) annDen, (object) mese, (object) proDen);
          string ultAgg = annullamentoDenunceOcm.UltAgg;
          int annoBilancio = modGetDati.Module_GetAnnoBilancio(dataLayer, false, annDen);
          string tipoAnno = AnnullamentiBLL.Module_GetTipoAnno(annDen, annoBilancio);
          int num2;
          if (annullamentoDenunceOcm.NumMov.Trim() == string.Empty)
          {
            if (!AnnullamentiDAL.MovimentoAnnullamento(dataLayer, filtroSql, ultAgg))
            {
              dataLayer.EndTransaction(false);
              AnnullamentiBLL.ErrorMessage = "Operazione non eseguita";
              return false;
            }
          }
          else
          {
            if (tipMov == "NU" || tipMov == "DP")
            {
              num2 = 0;
              dataTable1.Clear();
              dataTable1 = idocRdl.GET_IDOC_DATI_E1PITYPE(dataLayer, "9004", codPos, 0, 0, 0, "", "", "9999-12-31", "", "", "", annDen, mese, proDen, "", "ANNULLAMENTO", tipMov, "");
              foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
              {
                idocRdl.WRITE_IDOC_TESTATA(dataLayer, row);
                DT.Clear();
                DT = dataTable1.Clone();
                DT.ImportRow(row);
                idocRdl.WRITE_IDOC_E1PITYP(dataLayer, "9004", DT, false);
              }
              items.Add(new AnnullamentoDenunceOCM()
              {
                CodPos = codPos,
                AnnDen = annDen,
                Mese = mese,
                MesDen = mese.ToString(),
                ProDen = proDen,
                TipMov = tipMov
              });
              if (flgArr)
              {
                foreach (ImportiFromDENTES importiFromDentes in listaImporti)
                {
                  if (importiFromDentes.TipMov.Trim() != "DP")
                    items.Add(new AnnullamentoDenunceOCM()
                    {
                      CodPos = importiFromDentes.CodPos,
                      AnnDen = importiFromDentes.AnnDen,
                      Mese = importiFromDentes.Mese,
                      MesDen = importiFromDentes.Mese.ToString(),
                      ProDen = importiFromDentes.ProDen,
                      TipMov = importiFromDentes.TipMov
                    });
                }
              }
            }
            if (tipMov != "SAN")
            {
              tipMov = "ANN_" + annullamentoDenunceOcm.TipMov.Trim();
              string CODCAU = annullamentoDenunceOcm.CodCauMov.ToString().Trim();
              List<DENDET_Data> dataForDelete1 = AnnullamentiDAL.GetDataForDelete_1(dataLayer, codPos, annDen, mese, proDen);
              if (dataForDelete1 != null)
              {
                foreach (DENDET_Data dendetData in dataForDelete1)
                {
                  if (!(!(tipMov == "ANN_AR") ? clsWRITE_DB.AGGIORNA_RETANN(dataLayer, utente, dendetData.CodPos.ToString(), dendetData.Mat.ToString(), dendetData.ProRap, dendetData.AnnCom, dendetData.ImpRet, dendetData.ImpOcc, dendetData.ImpFig, "-") : clsWRITE_DB.AGGIORNA_RETANN(dataLayer, utente, dendetData.CodPos.ToString(), dendetData.Mat.ToString(), dendetData.ProRap, dendetData.AnnCom, dendetData.ImpRet, dendetData.ImpOcc, dendetData.ImpFig, "-", true)))
                    throw new Exception("Si sono verificati errori durante l'annullamento delle denunce");
                }
              }
              numeroMovimentoSap = modContabilita.WRITE_CONTABILITA_ANNULLAMENTO_DIPA_NOTIFICHE(dataLayer, utente, ref empty, codPos, annDen, mese, proDen, tipoAnno, CODCAU, annoBilancio.ToString(), tipMov, ref PARTITA_MOVIMENTO, ref PROGMOV_MOVIMENTO, numMov.Trim());
              if (!string.IsNullOrEmpty(empty))
              {
                dataLayer.EndTransaction(false);
                AnnullamentiBLL.ErrorMessage = "Operazione non eseguita";
                return false;
              }
              List<DENDET_Data> dataForDelete2 = AnnullamentiDAL.GetDataForDelete_2(dataLayer, codPos, annDen, mese, proDen);
              if (dataForDelete2 != null && dataForDelete2.Count > 0)
              {
                foreach (DENDET_Data dendetData in dataForDelete2)
                  list_stampeRet.Add(new DENDET_Data()
                  {
                    CodPos = dendetData.CodPos,
                    AnnDen = dendetData.AnnDen,
                    MesDen = dendetData.MesDen,
                    ProDen = dendetData.ProDen
                  });
              }
            }
            if (annullamentoDenunceOcm.NumSan.Trim() != string.Empty && annullamentoDenunceOcm.NumSanAnn.Trim() == string.Empty)
            {
              string codCau = annullamentoDenunceOcm.CodCauSan.Trim();
              if ((tipMov = AnnullamentiDAL.GetFieldFirstValue_TipMov(dataLayer, codCau)) != null)
                tipMov = "ANN_" + tipMov.Trim();
              if ((codCauAnn = AnnullamentiDAL.GetFieldFirstValue_CodCauAnn(dataLayer, tipMov, dataSistema)) != null)
                codCauAnn = codCauAnn.Trim();
              str1 = modContabilita.WRITE_CONTABILITA_ANNULLAMENTO_SANZIONI(dataLayer, utente, ref empty, codPos, annDen, mese, proDen, tipoAnno, codCauAnn, annoBilancio.ToString(), tipMov, ref impSanDet, ref impAbb, ref impAddRec, ref impAssCon, ref str2, ref num1);
              if (!string.IsNullOrEmpty(empty))
              {
                dataLayer.EndTransaction(false);
                AnnullamentiBLL.ErrorMessage = "Operazione non eseguita";
                return false;
              }
              if (annullamentoDenunceOcm.TipMov.Trim() == "NU")
              {
                numSanAnn = AnnullamentiDAL.GetFieldFirstValue_NumSanAnn(dataLayer, codPos, annDen, mese, proDen);
                annullamentoDenunceOcmList.Add(new AnnullamentoDenunceOCM()
                {
                  CodPos = codPos,
                  AnnDen = annDen,
                  Mese = mese,
                  ProDen = proDen,
                  TipMov = annullamentoDenunceOcm.TipMov.Trim(),
                  NumSan = annullamentoDenunceOcm.NumSan.Trim(),
                  NumSanAnn = numSanAnn
                });
              }
              else
                annullamentoDenunceOcmList.Add(new AnnullamentoDenunceOCM()
                {
                  CodPos = codPos,
                  AnnDen = annDen,
                  Mese = mese,
                  ProDen = proDen,
                  TipMov = annullamentoDenunceOcm.TipMov.Trim(),
                  NumSan = annullamentoDenunceOcm.NumSan.Trim(),
                  NumSanAnn = string.Empty
                });
            }
          }
          if (annullamentoDenunceOcm.TipMov.Trim() != "SAN")
          {
            if (AnnullamentiDAL.AnnullamentoRettifiche_1(dataLayer, codPos, annDen, mese, proDen))
            {
              for (int i = 1; i <= 2; ++i)
              {
                if (!AnnullamentiDAL.AnnullamentoRettifiche_2(dataLayer, codPos, annDen, mese, proDen, i, numeroMovimentoSap, dataSistema, ref tipoAnno, ref addizionale, clsWRITE_DB, annoBilancio, ref impConRet, ref impAddRecRet, ref impAbbRet, ref impAssConRet, ref codCauAnn, ref str2, ref num1, ref numSanAnn, modContabilita, utente, ref tipMov, ref impSanDet, ref impAbb, ref impAddRec, ref impAssCon))
                {
                  dataLayer.EndTransaction(false);
                  AnnullamentiBLL.ErrorMessage = "Operazione non eseguita";
                  return false;
                }
              }
            }
            else
            {
              dataLayer.EndTransaction(false);
              AnnullamentiBLL.ErrorMessage = "Operazione non eseguita";
              return false;
            }
          }
          if (annullamentoDenunceOcm.TipMov.Trim() == "AR")
          {
            num2 = 0;
            dataTable1.Clear();
            dataTable1 = idocRdl.GET_IDOC_DATI_E1PITYPE(dataLayer, "9005", codPos, 0, 0, 0, "", "", "", "", "", "", annDen, mese, proDen, "", "", tipMov.Trim(), "");
            foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
            {
              idocRdl.WRITE_IDOC_TESTATA(dataLayer, row);
              DT.Clear();
              DT = dataTable1.Clone();
              DT.ImportRow(row);
              idocRdl.WRITE_IDOC_E1PITYP(dataLayer, "9005", DT, false);
            }
            items.Add(new AnnullamentoDenunceOCM()
            {
              CodPos = codPos,
              AnnDen = annDen,
              Mese = mese,
              MesDen = mese.ToString(),
              ProDen = proDen,
              TipMov = "AR"
            });
            if (flgArr)
            {
              foreach (ImportiFromDENTES importiFromDentes in listaImporti)
              {
                if (importiFromDentes.TipMov.Trim() != "AR")
                  items.Add(new AnnullamentoDenunceOCM()
                  {
                    CodPos = importiFromDentes.CodPos,
                    AnnDen = importiFromDentes.AnnDen,
                    MesDen = importiFromDentes.Mese.ToString(),
                    Mese = importiFromDentes.Mese,
                    ProDen = importiFromDentes.ProDen,
                    TipMov = importiFromDentes.TipMov.Trim()
                  });
              }
            }
          }
          PARTITA_MOVIMENTO = "";
          PROGMOV_MOVIMENTO = 0M;
          str2 = "";
          num1 = 0M;
        }
        if (tipMov != "SAN")
          idocRdl.Aggiorna_IDOC(dataLayer);
        dataLayer.EndTransaction(true);
        if (items.Count > 0)
        {
          DataTable dataTable3 = new ListtoDataTableConverter().ToDataTable<AnnullamentoDenunceOCM>(items);
          pdf.MODULE_STAMPA_DOCUMENTO_DENUNCIA(0, 0, 0, 0, dataTable3);
        }
        if (list_stampeRet.Count > 0)
          pdf.MODULE_STAMPA_DOCUMENTO_RETTIFICA(list_stampeRet);
        return true;
      }
      catch (Exception ex)
      {
        dataLayer.EndTransaction(false);
        AnnullamentiBLL.log.Info((object) string.Format("[TFI.BLL] : Annullamento Denunce - La funzione AnnullaDenuncia_Step3() ha generato un'eccezione in data: {0} - ErrorMessage: {1}", (object) DateTime.Now, (object) ex.Message));
        AnnullamentiBLL.ErrorMessage = "L'applicazione ha generato un'eccezione in fase di annullamento denunce";
        return false;
      }
    }

    private static string Module_GetTipoAnno(int annDen, int annoBilancio)
    {
      if (annDen == annoBilancio)
        return "AC";
      if (annoBilancio < annDen)
        throw new Exception("Contattare l'Amministratore del Sistema. Anno di bilancio inferiore all'anno della distinta");
      return "AP";
    }
  }
}
