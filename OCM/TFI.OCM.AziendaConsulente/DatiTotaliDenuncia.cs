using TFI.BLL.Utilities.PagoPa;
using OCM.TFI.OCM.Protocollo;
namespace TFI.OCM.AziendaConsulente
{
    public class DatiTotaliDenuncia
    {
        public string CodPos { get; set; }

        public int Anno { get; set; }

        public int Mese { get; set; }

        public string NomeMese { get; set; }

        public int ProDen { get; set; }

        public int IdDipa { get; set; }

        public string Prot { get; set; }

        public string StaDen { get; set; }

        public string TipMov { get; set; }

        public string IntestazioneTotali { get; set; }

        public bool BtnStampaRicevutaIsVisible { get; set; }

        public bool TbIntestazionePagamentoIsVisible { get; set; }

        public bool BtnCreditiIsVisible { get; set; }

        public bool TbPagamentoIsVisible { get; set; }

        public bool BtnDettagliDipaIsVisible { get; set; }

        public bool LblCreditiIsVisible { get; set; }

        public bool TxtCreditiIsVisible { get; set; }

        public bool LblTotaleIsVisible { get; set; }

        public bool LblTotPagareIsVisible { get; set; }

        public decimal LblContributi { get; set; }

        public decimal LblAddizionale { get; set; }

        public string LblDescrizAddizionale { get; set; }

        public decimal LblAbbonamento { get; set; }

        public decimal LblAssistenza { get; set; }

        public decimal LblTotContributi { get; set; }

        public decimal LblSanzioni { get; set; }

        public decimal TxtCrediti { get; set; }

        public decimal LblDecurtato { get; set; }

        public decimal LblTotDovuto { get; set; }

        public decimal LblTotPagare { get; set; }

        public decimal TxtImportoVersato { get; set; }

        public string TxtDataVersamento { get; set; }

        public string TxtDataVersamento_attr_today { get; set; }

        public string TxtDataVersamento_attr_value { get; set; }

        public string TxtImportoVersato_attr_value { get; set; }

        public string HdnCheck { get; set; }

        public bool BtnStampaMAVIsVisible { get; set; }

        public bool BtnMAVIsVisible { get; set; }

        public string HdnMAV { get; set; }

        public int PeriodiSenzaImporto { get; set; }

        public decimal LblTotFondo { get; set; }

        public int MesFIA { get; set; }

        public int AnnFIA { get; set; }

        public bool BtnMAVSanitIsVisible { get; set; }

        public bool BtnStampaMAVSanitIsVisible { get; set; }

        public bool TbPagamentoSanitarioIsVisible { get; set; }

        public bool TbIntestazionePagamentoFondoSanitarioIsVisible { get; set; }

        public string TxtDataVersamentoSanit { get; set; }

        public decimal TxtImportoVersatoSanit { get; set; }

        public bool TxtDataVersamento_readonly { get; set; }

        public bool TxtImportoVersato_readonly { get; set; }

        public bool TxtDataVersamentoSanit_readonly { get; set; }

        public bool TxtImportoVersatoSanit_readonly { get; set; }

        public bool FondoSanitarioIsVisible { get; set; }

        public bool RigaSanzioniIsVisible { get; set; }

        public bool BtnRipristinaImportoIsVisible { get; set; }

        public bool BtnConfermaTotaliIsVisible { get; set; }

        public bool TxtCrediti_readonly { get; set; }

        public string HdnMavSanit { get; set; }

        public int IdDipaDef { get; set; }
        public decimal ImpDec { get; set; }
        public string IuvCode { get; set; }
        public string TransActionId { get; set; }
        public StatoPagoPa StatoPagamentoPagoPa { get; set; }
        public Protocollo Protocollo { get; set; }
    }
}
