using System.Collections.Generic;

namespace TFI.OCM.AziendaConsulente
{
    public class RettificheDIPA
    {
        public int Anno { get; set; }

        public int Mese { get; set; }

        public int ProDen { get; set; }

        public int IdDipa { get; set; }

        public string DatApe { get; set; }

        public decimal ImpConDel { get; set; }

        public decimal ImpAddRecDel { get; set; }

        public decimal ImpAbbDel { get; set; }

        public decimal ImpAssConDel { get; set; }

        public decimal ImpSanRet { get; set; }

        public decimal ImpTotale { get; set; }

        public string TipMov { get; set; }

        public string SanSotSog { get; set; }

        public string NumSan { get; set; }

        public string NumSanAnn { get; set; }

        public string Periodo { get; set; }

        public string BtnOriginale { get; set; }

        public string CodFis { get; set; }

        public string PartitaIVA { get; set; }

        public string NatGiu { get; set; }

        public string SedeLegale { get; set; }

        public List<DatiRettificheDIPA> ListaRettifiche { get; set; }
    }
}
