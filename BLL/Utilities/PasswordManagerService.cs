using System;
using System.Text;

namespace TFI.BLL.Utilities;

public class PasswordManagerService
{
    public string GenerateStrongPassword(int lunghezzaMassima = 8, string caratteriConsentiti = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890")
    {
        var random = new Random();
        var password = new StringBuilder();
        for (var i = 0; i < lunghezzaMassima; i++)
        {
            var index = random.Next(caratteriConsentiti.Length);
            password.Append(caratteriConsentiti[index]);
        }

        return password.ToString();
    }
}