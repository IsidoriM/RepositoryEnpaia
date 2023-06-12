namespace TFI.OCM.AziendaConsulente
{
    public class SospensioneRapporto
    {
        public decimal Mat { get; set; }

        public decimal ProRap { get; set; }

        public string DataInizio { get; set; }

        public string DataFine { get; set; }

        public decimal PerAzi { get; set; }

        public decimal PerFig { get; set; }

        public decimal CodSos { get; set; }

        public string DenSos { get; set; }

        public SospensioneRapporto(decimal mat, decimal proRap, string dataInizio, string dataFine, decimal perAzi, decimal perFig, decimal codSos, string denSos)
        {
            Mat = mat;
            ProRap = proRap;
            DataInizio = dataInizio;
            DataFine = dataFine;
            PerAzi = perAzi;
            PerFig = perFig;
            CodSos = codSos;
            DenSos = denSos;
        }
    }
}
