using System.Collections.Generic;

namespace TFI.OCM.AziendaConsulente
{
    public class ConsultazioneArretrati
    {
        public string anno { get; set; }

        public string mese { get; set; }

        public string proDen { get; set; }

        public string staDen { get; set; }

        public string matricola { get; set; }

        public string parm { get; set; }

        public string annCom { get; set; }

        public int codmodpag { get; set; }

        public bool isRicerca { get; set; }

        public decimal lblTotRetribuzioni { get; set; }

        public decimal lblTotOccasionali { get; set; }

        public decimal lblTotContributi { get; set; }

        public string intestazione { get; set; }

        public bool lblIntestazioneisVisible { get; set; }

        public bool colAnnComVisible { get; set; }

        public bool colMesVisible { get; set; }

        public bool colAnnoVisible { get; set; }

        public bool btnTotaliVisible { get; set; }

        public bool btnRettificheVisible { get; set; }

        public List<DenunciaArretrati> listaSelezione { get; set; }
    }
}
