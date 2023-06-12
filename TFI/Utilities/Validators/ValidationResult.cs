using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFI.Utilities.Validators
{
    public class ValidationResult
    {
        public bool IsSucceeded { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public ValidationResult() { }

        public ValidationResult(string error)
        {
            IsSucceeded = false;
            Errors.Add(error);
        }

        public ValidationResult(List<string> errors)
        {
            IsSucceeded = false;
            Errors = errors;
        }

        public override string ToString()
        {
            var result = IsSucceeded ? "ha" : "non ha";
            var error = string.Join("\n", Errors);
            return $"L'operazione {result} avuto successo.\n{error}";
        }

        public static ValidationResult operator +(ValidationResult a, ValidationResult b) =>
            new ValidationResult()
            {
                IsSucceeded = a.IsSucceeded && b.IsSucceeded,
                Errors = a.Errors.Union(b.Errors).ToList()
            };
    }
}