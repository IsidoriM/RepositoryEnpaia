using System.Linq;

namespace TFI.Utilities.Validators
{
    public static class PasswordHelper
    {
        public static bool CheckPassword(string password, out string error)
        {
            var validationResult = password.PasswordLength();
            validationResult += password.PasswordContainsLetter();
            validationResult += password.PasswordContainsLower();
            validationResult += password.PasswordContainsUpper();
            validationResult += password.PasswordContainsDigit();
            validationResult += password.PasswordSpecialChar();

            error = validationResult.ToString();
            return validationResult.IsSucceeded;
        }

        private static ValidationResult PasswordContainsUpper(this string password)
        {
            var result = password.Any(char.IsUpper);

            var validationResult = new ValidationResult() { IsSucceeded = result };
            var error = "Inserire almeno un carattere maiuscolo";
            if (!result)
                validationResult.Errors.Add(error);
            return validationResult;
        }
        private static ValidationResult PasswordContainsLower(this string password)
        {
            var result = password.Any(char.IsLower);

            var validationResult = new ValidationResult() { IsSucceeded = result };
            var error = "Inserire almeno un carattere minuscolo";
            if (!result)
                validationResult.Errors.Add(error);
            return validationResult;
        }

        private static ValidationResult PasswordContainsDigit(this string password)
        {
            var result = password.Any(char.IsDigit);

            var validationResult = new ValidationResult() { IsSucceeded = result };
            var error = "Inserire almeno un numero";
            if (!result)
                validationResult.Errors.Add(error);
            return validationResult;
        }

        private static ValidationResult PasswordContainsLetter(this string password)
        {
            var result = password.Any(char.IsLetter);

            var validationResult = new ValidationResult() { IsSucceeded = result };
            var error = "Inserire almeno una lettera";
            if (!result)
                validationResult.Errors.Add(error);
            return validationResult;
        }

        private static ValidationResult PasswordLength(this string password, int start = 8, int end = 16)
        {
            var result = password.Length >= start && password.Length <= end;

            var validationResult = new ValidationResult() { IsSucceeded = result };
            var error = $"Inserire tra {start} e {end} caratteri";
            if (!result)
                validationResult.Errors.Add(error);
            return validationResult;
        }

        private static ValidationResult PasswordSpecialChar(this string password)
        {
            var specialChars = new[] { '!', '"', '?', '$', '%', '&', '(', ')', '=', '[', ']', '<', '>', '+', '.', '-', '_', '*', '@', '#' };

            var result = password.Any(c => specialChars.Contains(c));

            var validationResult = new ValidationResult() { IsSucceeded = result };
            var error = $"Inserire almeno un carattere speciale tra: {string.Join(", ", specialChars)}";
            if (!result)
                validationResult.Errors.Add(error);
            return validationResult;
        }

    }
}