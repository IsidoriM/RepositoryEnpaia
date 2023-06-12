namespace TFI.OCM.AziendaConsulente
{
    public class ContrattoDiRiferimento : Contratto
    {
        public int CodQuaCon { get; set; }

        public string DenQua { get; set; }

        public int CodLiv { get; set; }

        public string DenLiv { get; set; }

        public ContrattoDiRiferimento(int codice, int progressivo, int codQuaCon, string dataInizio, string dataFine, string datDec, string denQua, int codLiv, string denLiv, string tipSpe)
            : base(codice, progressivo, dataInizio, dataFine, datDec, tipSpe)
        {
            CodQuaCon = codQuaCon;
            DenQua = denQua;
            CodLiv = codLiv;
            DenLiv = denLiv;
        }
    }
}
