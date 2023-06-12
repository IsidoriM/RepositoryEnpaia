using System.Collections.Generic;

namespace TFI.OCM.AziendaConsulente
{
    public class DatiDenuncia
    {
        public int IdDipa { get; set; }

        public int Anno { get; set; }

        public int Mese { get; set; }

        public int ProDen { get; set; }

        public int TotDipendenti { get; set; }

        public string Periodo { get; set; }

        public bool BtnRettificheIsVisible { get; set; }

        public bool ImpFigColumnIsVisible { get; set; }

        public decimal TotFondoSanitario { get; set; }

        public string SedeLegale { get; set; }

        public List<RetribuzioneRDL> Records { get; set; }

        public TotaliTestata TotTestata { get; set; }

        public DatiAnagraficiAzienda DatiAnagrafici { get; set; }

        public DatiDenuncia()
        {
            Records = new List<RetribuzioneRDL>();
            TotTestata = new TotaliTestata();
            DatiAnagrafici = new DatiAnagraficiAzienda();
        }
    }
}
