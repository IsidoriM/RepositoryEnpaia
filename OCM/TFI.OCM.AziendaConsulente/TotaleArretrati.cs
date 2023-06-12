using System;
using OCM.TFI.OCM.Protocollo;
using TFI.BLL.Utilities.PagoPa;

namespace TFI.OCM.AziendaConsulente
{
    public class TotaleArretrati
    {
        public int hdnAnno { get; set; }

        public int hdnProden { get; set; }

        public int hdnMese { get; set; }

        public string staDen { get; set; }

        public string mat { get; set; }

        public string anncom { get; set; }

        public string hdnQueryString { get; set; }

        public string lblAddizionale { get; set; }

        public string lblContributi { get; set; }

        public string lblTotContributi { get; set; }

        public string lblTotPagare { get; set; }

        public string lblTotDovuto { get; set; }

        public string lbltotpag { get; set; }

        public string lblDecurtato { get; set; }

        public string txtCrediti { get; set; }

        public string hdnMav { get; set; }

        public string txtDataVersamento { get; set; }

        public string txtImportoVersato { get; set; }

        public string hdnTipMov { get; set; }

        public string hdnProt { get; set; }

        public string hdnFLGRIC { get; set; }

        public string lblDataDenuncia { get; set; }

        public string strStaDen { get; set; }

        public string lblTotSanzione { get; set; }

        public string txtNote { get; set; }

        public string Label5 { get; set; }

        public decimal decPercen { get; set; }

        public string strStaDen2 { get; set; }

        public string btnType { get; set; }

        public string parm { get; set; }

        public int flagLoad { get; set; }

        public bool tbIntestazionePagamentoVisibile { get; set; }

        public bool btnDettaglioVisibile { get; set; }

        public bool lblCreditiVisibile { get; set; }

        public bool txtCreditiVisibile { get; set; }

        public bool lblTotaleVisibile { get; set; }

        public bool lblTotPagareVisibile { get; set; }

        public bool tbPagamentoVisibile { get; set; }

        public bool btnStampa_MAV { get; set; }

        public bool btnMav { get; set; }

        public bool btnConfermaTotaliVisible { get; set; }

        public bool btnCreditiVisible { get; set; }

        public bool txtCreditiReadonly { get; set; }

        public bool txtNoteReadonly { get; set; }

        public bool txtDataVersamentoReadonly { get; set; }

        public bool txtImportoVersatoReadonly { get; set; }

        public bool tbTotaliRows5Visible { get; set; }

        public bool btnStampaArretratoVisible { get; set; }

        public bool btnStampaRicevutaVisible { get; set; }

        public bool lblDataErogazioneVisible { get; set; }

        public bool isRicerca { get; set; }
        public string IuvCode { get; set; }
        public string TransActionId { get; set; }
        public StatoPagoPa StatoPagamentoPagoPa { get; set; }
        public DateTime? DataChiusura { get; set; }
        public Protocollo Protocollo { get; set; }
        public int? ModalitaPagamento { get; set; }
    }
}
