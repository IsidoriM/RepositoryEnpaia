namespace OCM.TFI.OCM.Registrazione;

public class RecordRegistrazioneDocumentoAzienda
{
    public int Id { get; set; }
    public int IdReg { get; set; }
    public int IdTipoDocumento { get; set; }
    public int IdProtocollo { get; set; }
    public int IdAllegato { get; set; }
    public string UUID { get; set; }
    public string PartitvaIva { get; set; }
}