using System.Collections.Generic;
using OCM.TFI.OCM.Protocollo;
using OCM.TFI.OCM.Utilities;

namespace OCM.TFI.OCM.Registrazione;

public class ResultRegistrazioneAzienda
{
    public ProtocolloDetail Protocollo { get; set; }
    public string CodicePosizione { get; set; }
    public string PartitaIva { get; set; }
    public List<Documento> DocumentiCaricati { get; set; }
    public List<Documento> DocumentiDaCaricare { get; set; }
    public StatoRegistrazioneAzienda StatoRegistrazione { get; set; }
}