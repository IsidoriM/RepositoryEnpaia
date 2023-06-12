using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCM.TFI.OCM.Infortuni
{
    public class PraticaInfortunio
    {
        public int CodicePosizione { get; set; }
        public int Matricola { get; set; }
        public string NomeCompleto { get; set; }
        public int ProgressivoInfortunio { get; set; }
        public int ProgressivoRapportoDiLavoro { get; set; }
        public DateTime DataInfortunio { get; set; }
        public DateTime DataDenunciaInfortunio { get; set; }
        public int TipoInfortunio { get; set; }

        public int TotalPages { get; set; }
        public string DescrizioneTipoInfortunio { get; set; }
        public string DescrizioneAzienda { get; set; }
        public string DescrizioneIscritto { get; set; }
    }
}
