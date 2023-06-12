using System.Collections.Generic;

namespace TFI.OCM.Amministrativo
{
    public class DiffideOCM
    {
        public class Diffide
        {
            public string CodPos { get; set; }

            public string Anno { get; set; }

            public string RagSoc { get; set; }

            public string CodFis { get; set; }

            public string ParIva { get; set; }

            public string Nfile { get; set; }

            public string DatConsegna { get; set; }

            public string CodUnivoco { get; set; }
        }

        public Diffide Diff { get; set; }

        public List<Diffide> ListaDiffide { get; set; }

        public DiffideOCM()
        {
            Diff = new Diffide();
            ListaDiffide = new List<Diffide>();
        }
    }
}
