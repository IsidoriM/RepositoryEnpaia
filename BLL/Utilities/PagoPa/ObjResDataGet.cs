public class ObjResDataGet
{
    public string idPagamento { get; }
    public string IdServizio { get; }
    public string DataRimozione { get; }
    public string DataScadenza { get; }
    public string Iuv { get; }
    public string CodiceDebito { get; }
    public string TipoPagatore { get; }
    public string CfPivaPagatore { get; }
    public string NomePagatore { get; }
    public string IndirizzoPagatore { get; }
    public string CivPagatore { get; }
    public string CapPagatore { get; }
    public string LocalitaPagatore { get; }
    public string ProvinciaPagatore { get; }
    public string NazionePagatore { get; }
    public string EmailPagatore { get; }
    public string Importo { get; }
    public string CausaleVersamento { get; }
    public string Descrizionecausaleversamento { get; }
    public string Esito { get; }
    public string UtenteInserimento { get; }
    public string DataInserimento { get; }
    public string UtenteCancellazione { get; }
    public string DataCancellazione { get; }
    public string TransActionId { get; set; }

    public ObjResDataGet(string idPagamento, string idServizio, string dataRimozione, string dataScadenza, string iuv, string codiceDebito, string tipoPagatore, string cfPivaPagatore, string nomePagatore, string indirizzoPagatore, string civPagatore, string capPagatore, string localitaPagatore, string provinciaPagatore, string nazionePagatore, string emailPagatore, string importo, string causaleVersamento, string descrizionecausaleversamento, string esito, string utenteInserimento, string dataInserimento, string utenteCancellazione, string dataCancellazione)
    {
        this.idPagamento = idPagamento;
        IdServizio = idServizio;
        DataRimozione = dataRimozione;
        DataScadenza = dataScadenza;
        Iuv = iuv;
        CodiceDebito = codiceDebito;
        TipoPagatore = tipoPagatore;
        CfPivaPagatore = cfPivaPagatore;
        NomePagatore = nomePagatore;
        IndirizzoPagatore = indirizzoPagatore;
        CivPagatore = civPagatore;
        CapPagatore = capPagatore;
        LocalitaPagatore = localitaPagatore;
        ProvinciaPagatore = provinciaPagatore;
        NazionePagatore = nazionePagatore;
        EmailPagatore = emailPagatore;
        Importo = importo;
        CausaleVersamento = causaleVersamento;
        Descrizionecausaleversamento = descrizionecausaleversamento;
        Esito = esito;
        UtenteInserimento = utenteInserimento;
        DataInserimento = dataInserimento;
        UtenteCancellazione = utenteCancellazione;
        DataCancellazione = dataCancellazione;
    }
}

