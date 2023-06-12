using System.Collections.Generic;
using System.Linq;

namespace TFI.OCM;

public class ResultDto
{
    public ResultDto() { }
    public ResultDto(bool isSuccessfull)
    {
        IsSuccessfull = isSuccessfull;
    }
    public ResultDto(bool isSuccessfull, string message)
    {
        IsSuccessfull = isSuccessfull;
        Message = message;
    }
    public ResultDto(bool isSuccessfull, string message, List<string> warnings)
    {
        IsSuccessfull = isSuccessfull;
        Message = message;
        Warnings = warnings;
    }
    public bool IsSuccessfull { get; set; }

    private string _message;
    public string Message
    {
        get => IsSuccessfull && string.IsNullOrWhiteSpace(_message) ? "Operazione Completata" : _message;
        set => _message = value;
    }

    public List<string> Warnings { get; set; } = new List<string>();

    public string GetBulletListOfWarningsForHtmlRaw()
    {
        FormatWarningsForHtmlRaw();
        return JoinWarningMessage("<br/>");

        void FormatWarningsForHtmlRaw()
        {
            Warnings = Warnings.Select(msg => " &#x2022; " + msg).ToList();
        }
    }

    public string JoinWarningMessage(string joinChar) => string.Join(joinChar, Warnings);

}