namespace OCM.TFI.OCM.Protocollo;

public class DllProtocolloAllegaResponse
{
    public string Esito { get; set; }
    public string TipoMessaggio { get; set; }
    public string Messaggio { get; set; }
    public InformazioniAllegato Data { get; set; }
}