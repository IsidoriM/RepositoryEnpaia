using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using static System.Char;
using static OCM.TFI.OCM.Utilities.RegexRules;

namespace OCM.TFI.OCM.Utilities.CustomValidators;

public class CodiceFiscale : ValidationAttribute, IClientValidatable
{
    private readonly string _nome;
    private readonly string _cognome;


    private const string Vocali = "AEIOUY";

    public CodiceFiscale()
    {
        
    }
    public CodiceFiscale(string nome, string cognome)
    {
        _nome = nome;
        _cognome = cognome;
    }

    protected override ValidationResult IsValid(object codiceFiscaleValue, ValidationContext validationContext)
    {
        if (codiceFiscaleValue == default) return ValidationResult.Success;
        if(string.IsNullOrWhiteSpace(codiceFiscaleValue.ToString())) return new ValidationResult(ErrorMessage);
        
        var basicRegexValidation = BasicRegexValidation(codiceFiscaleValue.ToString());
        var isLastCharValid = IsLastCharValid(codiceFiscaleValue.ToString());

        if (string.IsNullOrEmpty(_nome) || string.IsNullOrEmpty(_cognome))
            return basicRegexValidation && isLastCharValid ? ValidationResult.Success : new ValidationResult(ErrorMessage);

        if (!basicRegexValidation || !isLastCharValid) return new ValidationResult(ErrorMessage);
        
        var isCognomeValid = IsCognomeValid(codiceFiscaleValue.ToString(), GetPropertyValue(_cognome));
        var isNomeValid = IsNomeValid(codiceFiscaleValue.ToString(), GetPropertyValue(_nome));

        if (!isCognomeValid || !isNomeValid) return new ValidationResult(ErrorMessage);

        bool BasicRegexValidation(string codiceFiscale) 
            => Regex.IsMatch(codiceFiscale, CodiceFiscaleRegex);
        
        string GetPropertyValue(string propertyName)
        {
            var nomeProperty = validationContext.ObjectType.GetProperty(propertyName);
            
            return nomeProperty == null ? string.Empty : nomeProperty.GetValue(validationContext.ObjectInstance)?.ToString();
        }
        
        bool IsCognomeValid(string codiceFiscale, string cognome)
        {
            if (string.IsNullOrEmpty(cognome)) return false;
            
            var consonantiCognome = GetConsonanti(cognome);
            
            if (consonantiCognome.Count >= 3)
                return IsValidParolaBase(0, codiceFiscale, consonantiCognome[0], consonantiCognome[1],
                    consonantiCognome[2]);

            var vocaliCognome = GetVocali(cognome);
            if (vocaliCognome.Count == default) return false;

            if (consonantiCognome.Count == 2)
                return IsValidParolaBase(0, codiceFiscale, consonantiCognome[0], consonantiCognome[1],
                    vocaliCognome[0]);
            if (consonantiCognome.Count == 1 && vocaliCognome.Count >= 2)
                return IsValidParolaBase(0, codiceFiscale, consonantiCognome[0], vocaliCognome[0],
                    vocaliCognome[1]);
            return IsValidParolaBase(0, codiceFiscale, consonantiCognome[0], vocaliCognome[0], 'X');
        }

        bool IsNomeValid(string codiceFiscale, string nome)
        {
            if (string.IsNullOrEmpty(nome)) return false;
            var consonantiNome = GetConsonanti(nome);
            switch (consonantiNome.Count)
            {
                case >= 4:
                    return IsValidParolaBase(3, codiceFiscale, consonantiNome[0], consonantiNome[2], consonantiNome[3]);
                case 3:
                    return IsValidParolaBase(3, codiceFiscale, consonantiNome[0], consonantiNome[1], consonantiNome[2]);
                default:
                {
                    var vocaliNome = GetVocali(nome);
                    if (vocaliNome.Count == default) return false;
                    
                    if (consonantiNome.Count == 2)
                        return IsValidParolaBase(3, codiceFiscale, consonantiNome[0], consonantiNome[1],
                            vocaliNome[0]);
                    if (consonantiNome.Count == 1 && vocaliNome.Count >= 2)
                        return IsValidParolaBase(3, codiceFiscale, consonantiNome[0], vocaliNome[0], vocaliNome[1]);
                    
                    return IsValidParolaBase(3, codiceFiscale, consonantiNome[0], vocaliNome[0], 'X');
                }
            }
        }

        IList<char> GetVocali(string parola)
            => parola?.ToCharArray().Where(carattere => Vocali.Contains(ToUpper(carattere))).ToList();
        IList<char> GetConsonanti(string parola)
            => parola?.ToCharArray().Where(carattere => !Vocali.Contains(ToUpper(carattere))).ToList();

        bool IsValidParolaBase(int startingPos, string codiceFiscale, params char[] carattere)
        {
            var caratteriCodiceFiscale = codiceFiscale.Substring(startingPos).ToCharArray();   
            
            for (var i = 0; i < carattere.Length; i++)
            {
                if (!ToUpper(caratteriCodiceFiscale[i]).Equals(ToUpper(carattere[i]))) return false;
            }

            return true;
        }

        bool IsLastCharValid(string codiceFiscale)
        {
            if (codiceFiscale.Length != 16)
                return false;

            return CalculateLastLetter() == codiceFiscale[15];

            char CalculateLastLetter()
            {
                const string numberAndAlphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                
                int[] evenValues = {0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5, 6, 7 ,8 ,9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
                int[] oddValues = {1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23};
                
                var sum = 0;
                for (var i = 0; i < codiceFiscale.Length -1; i++)
                {
                    if (i % 2 == 0)
                    {
                        sum += oddValues[numberAndAlphabet.IndexOf(codiceFiscale[i])];
                        continue;
                    }
                
                    sum += evenValues[numberAndAlphabet.IndexOf(codiceFiscale[i])];
                }

                return alphabet[sum % 26];
            }
            
        }
        return ValidationResult.Success;
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
        var rule = new ModelClientValidationRule
        {
            ValidationType = "customcodicefiscale",
            ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
        };

        yield return rule;
    }
}