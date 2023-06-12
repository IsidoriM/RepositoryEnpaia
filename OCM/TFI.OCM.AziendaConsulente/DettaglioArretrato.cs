using System;

namespace OCM.TFI.OCM.AziendaConsulente;

public class DettaglioArretrato
{
    public string StatoDenuncia { get; set; }
    public DateTime? DataChiusura { get; set; }
    public string TotaleDovuto { get; set; }
    public string TotaleDaPagare { get; set; }
    public string AnnoDenuncia { get; set; }
    public string MeseDenuncia { get; set; }
    public string AnnoCompetenza { get; set; }
    public string ProgressivoDenuncia { get; set; }
    public Protocollo.Protocollo Protocollo { get; set; }
}