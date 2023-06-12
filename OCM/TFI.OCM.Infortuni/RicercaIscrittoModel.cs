using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OCM.TFI.OCM.Infortuni;

public class RicercaIscrittoModel
{
    [DisplayName("Matricola")]
    public string MatricolaFornita { get; set; }
    [DisplayName("Cognome")]
    public string CognomeFornito { get; set; }
    [DisplayName("Nome")]
    public string NomeFornito { get; set; }
    [DisplayName("Codice Fiscale ")]
    public string CodiceFiscaleFornito { get; set; }
    
    public IEnumerable<IscrittoTrovato> IscrittiTrovati { get; set; }
}