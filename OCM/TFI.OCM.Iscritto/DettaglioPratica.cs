namespace OCM.TFI.OCM.Iscritto
{
    public class DettaglioPratica : Pratica
    {
        public Nota Note { get; set; }
        public string Modalità_Pagamento { get; set; }
        public string Iban { get; set; }
        public string Swift { get; set; }
        public string Data_Conferimento { get; set; }
        public string Utente_Conferimento { get; set; }
        public string Data_Richiesta { get; set; }
        public string Utente_Richiesta { get; set; }
        public string Motivo_Anticipazione { get; set; }
        public string Importo_TFR { get; set; }
        public string Percentuale_TFR { get; set; }    
    }
}
