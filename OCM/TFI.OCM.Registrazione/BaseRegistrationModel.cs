using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OCM.TFI.OCM.Utilities;
using TFI.OCM.Amministrativo;

namespace OCM.TFI.OCM.Registrazione;

public class BaseRegistrationModel : GestioneAziendeWebOCM.IndirizzoModel
{

    [DisplayName("Cellulare:")]
    [Required(ErrorMessage = "Cellulare obbligatorio")]
    [RegularExpression(RegexRules.CellulareRegex, ErrorMessage = "Cellulare non valido")]
    [MaxLength(20, ErrorMessage = "Cellulare troppo lungo")]
    public string Cell { get; set; }
    
    [DisplayName("Telefono:")]
    [RegularExpression(RegexRules.TelefonoRegex, ErrorMessage = "Numero di telefono non valido")]
    [MaxLength(13, ErrorMessage = "Telefono troppo lungo")]
    public string Telefono { get; set; }
    
    [DisplayName("Email:")]
    [Required(ErrorMessage = "Email obbligatoria"), EmailAddress(ErrorMessage = "Email non valida")]
    [MaxLength(100, ErrorMessage = "Email troppo lunga")]
    public string EmailRegistrazione { get => _emailRegistrazione; set => _emailRegistrazione = value?.ToLower(); }
    private string _emailRegistrazione;
    
    [DisplayName("PEC:")]
    [Required(ErrorMessage = "Pec obbligatoria"), EmailAddress(ErrorMessage = "Pec non valida")]
    [RegularExpression(RegexRules.PecRegex, ErrorMessage = "Formato pec non valido")]
    [MaxLength(100, ErrorMessage = "Pec troppo lunga")]

    public string Pec { get => _pec; set => _pec = value?.ToLower(); }
    private string _pec;
}
