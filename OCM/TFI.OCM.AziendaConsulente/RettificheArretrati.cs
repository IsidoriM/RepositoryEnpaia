using System.Collections.Generic;

namespace TFI.OCM.AziendaConsulente
{
    public class RettificheArretrati
    {
        public string txtAnno { get; set; }

        public string denuncia { get; set; }

        public string sedeLegale { get; set; }

        public string CodFis { get; set; }

        public string pIVA { get; set; }

        public string hdnMese { get; set; }

        public string hdnAnno { get; set; }

        public string hdnProDen { get; set; }

        public string hdnMat { get; set; }

        public string hdnAnncom { get; set; }

        public string hdnTipMov { get; set; }

        public string hdNatGiu { get; set; }

        public string lblPeriodo { get; set; }

        public decimal lblTotContributi { get; set; }

        public decimal lblAddizionale { get; set; }

        public decimal lblTotale { get; set; }

        public decimal lblSanzione { get; set; }

        public bool btnTotali { get; set; }

        public bool btnChiudi { get; set; }

        public bool btnOriginale { get; set; }

        public bool annoCompetenzaVisible { get; set; }

        public List<TabRettifiche> listaRettifiche { get; set; }
    }
}
