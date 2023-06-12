// Decompiled with JetBrains decompiler
// Type: TFI.BLL.AziendaConsulente.TotaleArretratiBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System;
using OCM.TFI.OCM;
using OCM.TFI.OCM.Protocollo;
using OCM.TFI.OCM.Utilities;
using TFI.BLL.DllProtocollo;
using TFI.BLL.Utilities.PagoPa;
using TFI.BLL.Utilities.Pdf;
using TFI.DAL.AziendaConsulente;
using TFI.OCM.Utilities;

namespace TFI.BLL.AziendaConsulente
{
  public class TotaleArretratiBLL
  {
    private static readonly ProtocolloDll _protocollo = new ProtocolloDll();
    
    public TFI.OCM.AziendaConsulente.TotaleArretrati TotaleArretrati(
      string codPos,
      int hdnAnno,
      int hdnMese,
      int hdnProden,
      string ret,
      ref string errorMsg)
    {
      TotaleArretratiDAL totaleArretratiDal = new TotaleArretratiDAL();
      TFI.OCM.AziendaConsulente.TotaleArretrati totaleArretrati = new TFI.OCM.AziendaConsulente.TotaleArretrati();
      var arretrato = totaleArretratiDal.Page_Load(codPos, hdnAnno, hdnMese, hdnProden, ret);
      SetDettaglioPagoPa(ref errorMsg);
      
      return arretrato;
      
      void SetDettaglioPagoPa(ref string errorMsg)
      {
        var result = GetDettagliPagoPa(hdnAnno, hdnMese, hdnProden, codPos);

        if (string.IsNullOrWhiteSpace(result.EsitoIuvTransId.IuvCode) || string.IsNullOrWhiteSpace(result.EsitoIuvTransId.TransActionId))
        {
          arretrato.StatoPagamentoPagoPa = StatoPagoPa.DACREARE;
          return;
        }

        if (result.EsitoPagoPa.Esito != "OK")
        {
          errorMsg += "Errore nel prelievo dei dati PagoPa";
          return;
        }
                
        arretrato.IuvCode = result.EsitoPagoPa.Data.Iuv;
        arretrato.TransActionId = result.EsitoPagoPa.Data.TransActionId;
        if(Enum.TryParse<StatoPagoPa>(result.EsitoPagoPa.Data.Esito, true, out var statoEnum))
          arretrato.StatoPagamentoPagoPa = statoEnum;
      }
    }

    public void SalvaTotaleArretrati(
      string codPos,
      int hdnAnno,
      int hdnMese,
      int hdnProden,
      string txtCrediti,
      string TEXTAREA1,
      ref string ErrorMSG,
      ref string SuccessMSG)
    {
      TotaleArretratiDAL totaleArretratiDal = new TotaleArretratiDAL();
      TFI.OCM.AziendaConsulente.TotaleArretrati totaleArretrati = new TFI.OCM.AziendaConsulente.TotaleArretrati();
      totaleArretratiDal.btnConfermaTotali(codPos, hdnAnno, hdnMese, hdnProden, TEXTAREA1, txtCrediti, ref ErrorMSG, ref SuccessMSG);
    }
    
    public (ObjResponseGet EsitoPagoPa, ObjResDataCrea EsitoIuvTransId) GetDettagliPagoPa(int anno, int mese, int proDen, string utenteCodPosizione)
    {
      var pagoPa = new PagoPa();
      var dettagliPagoPaTable = pagoPa.GetDettagliPagoPa(anno, mese, proDen, utenteCodPosizione);
      var dettaglioObject = new ObjResDataCrea { TransActionId = dettagliPagoPaTable.transaActionId, IuvCode = dettagliPagoPaTable.iuvCod};
      return (pagoPa.GetPagamento(dettagliPagoPaTable.iuvCod, dettagliPagoPaTable.transaActionId), dettaglioObject);
    }

    public static ResultDtoWithContent<byte[]> GetRicevutaArretrato(int anno, int mese, int proDen, int annCom, string codPos, string denominazioneAzienda)
    {
      
      var totaleArretratiDal = new TotaleArretratiDAL();
      var dentes = totaleArretratiDal.GetDettaglioArretrato(anno, mese, proDen, annCom);
      if (dentes == default)
        return new ResultDtoWithContent<byte[]>(false, "Dati relativi all'arretrato non trovati.");

      if (dentes.Protocollo.Success)
      {
        var resultFile = _protocollo.GetFile(dentes.Protocollo.IdProtocollo);
        return new ResultDtoWithContent<byte[]>(resultFile.Length > 0, resultFile);
      }

      string errorMsg = string.Empty;
      var protocollo = _protocollo.ProtocollaPratica(ref errorMsg);
      if (!protocollo.Success)
        return new ResultDtoWithContent<byte[]>(false, protocollo.Message);
      
      protocollo.NumeroProgressivoProtoccollo = dentes.Protocollo.NumeroProgressivoProtoccollo;
      dentes.Protocollo = protocollo;
      
      var pdfService = new PdfService();
      byte[] ricevutaArretrato = pdfService.CreaPdfRicevutaArretrato(dentes, denominazioneAzienda);
      var fileName = $"RivcevutaArretrato_{mese}/{anno}_{codPos}.pdf";
      
      var result = _protocollo.AllegaFilePratica(new AllegaProtocolloModel
      {
        IdProtocollo = protocollo.IdProtocollo,
        FileName = fileName,
        FileByteArray = ricevutaArretrato,
        TipoDocumento = TipoDocumento.RicevutaArretrato,
        UserIdentifier = codPos
      });
      
      if (!result.IsSuccessfull)
        return new ResultDtoWithContent<byte[]>(false, "Errore nella generazione della ricevuta");
        
      var resultUpdate = totaleArretratiDal.UpdateDentesProtocollo(protocollo, anno, mese, proDen, codPos);
      if(!resultUpdate)
        return new ResultDtoWithContent<byte[]>(false, "Errore nell'aggiornamento della testata con i dati del protocollo.");
      
      return new ResultDtoWithContent<byte[]>(true, ricevutaArretrato);
    }
  }
}
