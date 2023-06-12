using OCM.TFI.OCM.Protocollo;

namespace OCM.TFI.OCM.Registrazione;

public class PraticaRegistrazioneLight
{
    public ProtocolloDetail Protocollo { get; set; }
    public string CodicePosizione { get; set; }
    public StatoRegistrazioneAzienda StatoRegistrazione { get; set; }
}