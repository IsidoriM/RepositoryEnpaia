using System.Collections.Generic;

namespace TFI.OCM.Amministrativo
{
    public class EstrattoContoOCM
    {
        public class Azienda
        {
            public string Posizione { get; set; }

            public string RagioneSociale { get; set; }

            public string PartitaIva { get; set; }

            public string CodiceFis { get; set; }

            public string Indirizzo { get; set; }

            public string NumeroCivico { get; set; }

            public string CAP { get; set; }

            public string Provincia { get; set; }

            public string StaestRes { get; set; }

            public string Comune { get; set; }
        }

        public class Iscritto
        {
            public string matrciola { get; set; }

            public string nome { get; set; }

            public string cognome { get; set; }

            public string codfis { get; set; }

            public string Indirizzo { get; set; }

            public string NumeroCivico { get; set; }

            public string CAP { get; set; }

            public string comune { get; set; }

            public string prov { get; set; }
        }

        public class EstrattiContoIsc
        {
            public decimal Anno { get; set; }

            public string NomeFile { get; set; }
        }

        public class EstrattiConto
        {
            public decimal Anno { get; set; }

            public string NomeFile { get; set; }
        }

        public List<EstrattiConto> estrattiContos { get; set; }

        public List<EstrattiContoIsc> estrattiContoIsc { get; set; }

        public EstrattiConto ec { get; set; }

        public Azienda az { get; set; }

        public Iscritto isc { get; set; }

        public EstrattiContoIsc eci { get; set; }

        public EstrattoContoOCM()
        {
            az = az;
            ec = ec;
            isc = isc;
            eci = eci;
        }
    }
}
