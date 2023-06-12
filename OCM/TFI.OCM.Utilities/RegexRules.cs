namespace OCM.TFI.OCM.Utilities;

public static class RegexRules
{
    public const string PecRegex = ".+@(legalmail|pec)\\.[a-zA-Z]{2,}";
    public const string TelefonoRegex = "([0-9]{3}[0-9]{7}|[0-9]{2}[0-9]{7})";
    public const string CellulareRegex = TelefonoRegex;
    public const string CodiceFiscaleRegex = @"^[A-Z]{6}\d{2}[A-Z]\d{2}[A-Z]\d{3}[A-Z]$";
    public const string CivicoRegex = @"^(^\d+(\/[a-zA-Z])?[a-zA-Z]?$)|(SNC)$";
}