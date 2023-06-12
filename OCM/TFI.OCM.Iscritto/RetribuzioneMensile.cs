using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCM.TFI.OCM.Iscritto
{
    public class RetribuzioneMensile
    {
        public int Posizione { get; set; }
        public int Anno { get; set; }
        public int Mese { get; set; }
        public int Matricola { get; set; }
        public string TipoMovimento { get; set; }
        public DateTime Dal { get; set; }
        public DateTime Al { get; set; }
        public decimal Retribuzione { get; set; }
        public int Occasionale { get; set; }
        public int Figurativa { get; set; }
        public decimal Contributo { get; set; }
        public string AssistenzaContrattuale { get; set; }
        public int AbbonamentoPa { get; set; }
    }
}
