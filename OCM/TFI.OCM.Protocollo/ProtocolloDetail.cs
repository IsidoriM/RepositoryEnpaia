using System;

namespace OCM.TFI.OCM.Protocollo;

public class ProtocolloDetail
{
    public ProtocolloDetail()
    {
        
    }
    
    public ProtocolloDetail(Protocollo protocollo)
    {
        IdProtocollo = protocollo.IdProtocollo;
        NumeroProtocollo = protocollo.NumeroProtocollo;
        DataProtocollo = protocollo.DataProtocollo;
    }
    
    public int IdProtocollo { get; set; }
    public string NumeroProtocollo { get; set; }
    public DateTime DataProtocollo { get; set; }
}