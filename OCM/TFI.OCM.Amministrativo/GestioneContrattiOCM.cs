using System.Collections.Generic;

namespace TFI.OCM.Amministrativo
{
    public class GestioneContrattiOCM
    {
        public class Contratti
        {
            public string DENLIV { get; set; }

            public string checkInput { get; set; }

            public string checkOutput { get; set; }

            public string Importo { get; set; }

            public string Mansione { get; set; }

            public string Qualifica { get; set; }

            public string CODCON { get; set; }

            public string PROCON { get; set; }

            public string CODQUACON { get; set; }

            public string CODTIPCON { get; set; }

            public string Denominazione { get; set; }

            public string DATINI { get; set; }

            public string DATAPPINI { get; set; }

            public string DATFIN { get; set; }

            public string DATAPPFIN { get; set; }

            public string ASSCON { get; set; }

            public string MAXSCA { get; set; }

            public string PERSCA { get; set; }

            public string NUMMEN { get; set; }

            public string RIVAUT { get; set; }

            public string ULTAGG { get; set; }

            public string UTEAGG { get; set; }

            public string CODLOC { get; set; }

            public string PROLOC { get; set; }

            public string livello { get; set; }
        }

        public class Livello
        {
            public string CODQUACON { get; set; }

            public string DENQUA { get; set; }
        }

        public class TIPCON
        {
            public string CODTIPCON { get; set; }

            public string DENTIPCON { get; set; }
        }

        public List<Livello> ListLivello { get; set; }

        public List<TIPCON> ListTIPCON { get; set; }

        public List<Contratti> ListCont { get; set; }

        public Contratti contratti { get; set; }

        public Livello livello { get; set; }

        public TIPCON tipcon { get; set; }

        public GestioneContrattiOCM()
        {
            livello = new Livello();
            contratti = new Contratti();
            tipcon = new TIPCON();
        }
    }
}
