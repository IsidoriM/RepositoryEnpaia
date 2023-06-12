using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace OCM.TFI.OCM.Protocollo;

public class Protocollo
{
    public string ProtocolloCompleto { get; }
    public int IdProtocollo { get; set; }
    public string NumeroProtocollo { get; set; }
    public string NumeroProgressivoProtoccollo { get; set; }
    public DateTime DataProtocollo { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }

    public Protocollo(string protocollo)
    {
        if (string.IsNullOrWhiteSpace(protocollo)) return;
        var splittedProtocollo = protocollo.Split(';');
        if (splittedProtocollo.Length != 3) return;
        
        ProtocolloCompleto = protocollo;
        
        var isTryParseExact = DateTime.TryParseExact(splittedProtocollo[2], "G", new CultureInfo("it-IT"), DateTimeStyles.None, out var parseResult);

        IdProtocollo = int.Parse(splittedProtocollo[0]);
        NumeroProtocollo = splittedProtocollo[1].Split('/')[1];
        DataProtocollo = isTryParseExact ? parseResult : throw new InvalidDataException("Data protocollo invalida");
    }

    public Protocollo() { }
}