using System.Collections.Generic;

namespace TFI.OCM.Amministrativo
{
    public class ModelloPrevOCM
    {
        public class DatiModello
        {
            public string CodPos { get; set; }

            public string Matricola { get; set; }

            public string RagSoc { get; set; }

            public string Nome { get; set; }

            public string Cognome { get; set; }

            public string Stato { get; set; }

            public string ValidiAnn { get; set; }

            public string Tipo { get; set; }

            public string Compilazione { get; set; }

            public string Utenti { get; set; }

            public string UteAss { get; set; }

            public string DataApePrev { get; set; }

            public string DataChiPrev { get; set; }

            public string UteApePrev { get; set; }

            public string UteChiPrev { get; set; }

            public string DatCesRDL { get; set; }

            public string AnnCesRDL { get; set; }

            public string Atti { get; set; }

            public string CesDal { get; set; }

            public string CesAl { get; set; }
        }

        public DatiModello dati { get; set; }

        public List<DatiModello> datiList { get; set; }

        public ModelloPrevOCM()
        {
            dati = new DatiModello();
            datiList = new List<DatiModello>();
        }
    }
}
