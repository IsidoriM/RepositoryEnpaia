using OCM.Enums;
using TFI.OCM.Utilities;

namespace OCM.TFI.OCM.Protocollo;

public class SalvataggioSuMacchinaModel
{
    public byte[] FileByteArray { get; set; }
    public string FileName { get; set; }
    public string UserIdentifier { get; set; }
    public TipoDocumento TipoDocumento { get; set; }
    public TipoPratica? TipoPratica { get; set; }
}