using System;
using System.ComponentModel;

namespace OCM.TFI.OCM.Infortuni;

public class InserimentoInfortunio
{
    public string Matricola { get; set; }
    public string Nome { get; set; }
    public string Cognome { get; set; }
    public string Regione { get; set; }
    public string Qualifica { get; set; }
    public string Dug { get; set; }
    [DisplayName("Indirizzo di Residenza")]
    public string Indirizzo { get; set; }
    [DisplayName("Comune di Residenza")]
    public string Comune { get; set; }
    [DisplayName("Data Infortunio")]
    public DateTime DataInfortunio { get; set; }
    [DisplayName("Data Denuncia")]
    public DateTime DataDenuncia { get; set; }
    [DisplayName("Tipo Infortunio")]
    public string SelectedTipoInfortunio { get; set; }
}