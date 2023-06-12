using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using OCM.TFI.OCM.Utilities;
using TFI.OCM.AziendaConsulente;

namespace TFI.Utilities.Validators.FileValidatorDenuncia
{
    public class DenunciaFileValidator
    {
        protected static ValidationResult ValidateImpFig(int index, string line, int ggSospensione)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var figurat = decimal.Parse(line.Substring(50, 9));

            return figurat > 0 && ggSospensione == 0 
                ? new ValidationResult($"Errore linea {index} - codice fiscale {codFis}: Retribuzione figurativa senza sospensione")
                : new ValidationResult { IsSucceeded = true };
        }

        protected static ValidationResult ValidateLineLength(int index, string line)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            if (line.Length != 59 && line.Length != 63)
                return new ValidationResult(
                    $"Errore linea {index} - codice fiscale {codFis}: numero di caratteri non valido, attesi 59 o 63 ma trovati {line.Length}");

            return new ValidationResult { IsSucceeded = true };
        }

        protected static ValidationResult ValidateDalAl(int index, string line, int anno, string dal, string al)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var mgDal = line.Substring(19, 4);
            var mgAl = line.Substring(23, 4);
            var result = new ValidationResult { IsSucceeded = true };

            Action<string, string, ValidationResult> checkFormat = (value, propName, v) =>
            {
                if (int.TryParse(value, out _)) return;

                v.IsSucceeded = false;
                v.Errors.Add($"Errore linea {index} - codice fiscale {codFis}: {propName} deve essere nel formato 'mmgg'");
            };

            Action<string, string, string, ValidationResult> checkValidity = (value, propName, dataAspettata, v) =>
            {
                var dataInseritaString = $"{value.Substring(2, 2)}/{value.Substring(0, 2)}/{anno}";
                var parseResult = DateTime.TryParse(dataInseritaString, out var dataInserita);
                //var dataInserita = new DateTime(anno, int.Parse(value.Substring(0, 2)), int.Parse(value.Substring(2, 2)));
                var dataAspettataParsed = DateTime.ParseExact(dataAspettata, "dd/MM/yyyy", CultureInfo.CurrentCulture);

                if (!parseResult || dataInserita.Date != dataAspettataParsed.Date)
                {
                    v.IsSucceeded = false;
                    v.Errors.Add($"Errore linea {index} - codice fiscale {codFis}: La data di {propName} non e' valida");
                    return;
                } 
            };

            checkFormat(mgDal, "mgDal", result);
            if (result.IsSucceeded)
                checkValidity(mgDal, "inizio", dal, result);

            checkFormat(mgAl, "mgAl", result);
            if (result.IsSucceeded)
                checkValidity(mgAl, "fine", al, result);

            return result;
        }
        
        protected static ValidationResult ValidateNumberFormat(int index, string line)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var aliquota = line.Substring(27, 5);
            var retribu = line.Substring(32, 9);
            var occasio = line.Substring(41, 9);
            var figurat = line.Substring(50, 9);
            var progrDip = line.Substring(16, 2);
            var result = new ValidationResult { IsSucceeded = true };

            Action<string, string, ValidationResult> checkFormat = (value, propName, v) =>
            {
                if (decimal.TryParse(value, out _)) return;

                v.IsSucceeded = false;
                v.Errors.Add($"Errore linea {index} - codice fiscale {codFis}: {propName} deve essere in un formato numerico valido");
            };

            checkFormat(retribu, "L'importo retribuito", result);
            checkFormat(occasio, "L'importo occasionale", result);
            checkFormat(aliquota, "L'importo dell'aliquota", result);
            checkFormat(figurat, "L'importo figurativo", result);
            checkFormat(progrDip, "Il progressivo dipendente", result);

            return result;
        }

        protected static ValidationResult ValidateNumberValue(int index, string line)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var aliquota = line.Substring(27, 5);
            var retribu = line.Substring(32, 9);
            var occasio = line.Substring(41, 9);
            var figurat = line.Substring(50, 9);
            var progrDip = line.Substring(16, 2);
            var result = new ValidationResult { IsSucceeded = true };

            Action<string, string, decimal, decimal, ValidationResult> checkValue = (value, propName, min, max, v) =>
            {
                var number = decimal.Parse(value);
                if (number >= min && number <= max) return;

                v.IsSucceeded = false;
                v.Errors.Add($"Errore linea {index} - codice fiscale {codFis}: {propName} deve essere compreso tra {min} e {max}");
            };

            checkValue(retribu, "L'importo retribuito", 0M, 999999999M, result);
            checkValue(occasio, "L'importo occasionale", 0M, 999999999M, result);
            checkValue(aliquota, "L'importo dell'aliquota", 0M, 99999M, result);
            checkValue(figurat, "L'importo figurativo", 0M, 999999999M, result);
            checkValue(progrDip, "Il progressivo dipendente", 1M, 99M, result);

            return result;
        }

        protected static ValidationResult ValidateImpOcc(int index, string line)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var retribu = decimal.Parse(line.Substring(32, 9));
            var occasio = decimal.Parse(line.Substring(41, 9));
            var result = new ValidationResult { IsSucceeded = true };

            if (!(occasio > retribu) || !(occasio > 0)) return result;

            result.IsSucceeded = false;
            result.Errors.Add($"Errore linea {index} - codice fiscale {codFis}: L'importo occasionale non puo' essere maggiore dell'imponibile!");

            return result;
        }
        
        protected static ValidationResult ValidateAllLineLength(List<string> fileContent)
        {
            var validationResult = new ValidationResult(){IsSucceeded = true};
            for (var i = 0; i < fileContent.Count; i++)
            {
                if (fileContent[i].Length != 63)
                {
                    validationResult += new ValidationResult($"Errore linea {i+1}: numero di caratteri non valido, attesi 63 ma trovati {fileContent[i].Length}" );
                }
            }

            return validationResult;
        }
        
        public static ValidationResult CheckFileExtension(HttpPostedFileBase fileUpload)
        {
            var fileExtension = new FileInfo(fileUpload.FileName).Extension;
            var isValidExtension = fileExtension.Equals(".txt");
            return isValidExtension ? new ValidationResult() { IsSucceeded = true } : new ValidationResult($"Formato file non valido.");
        }
        
        protected static ValidationResult ValidateAliquota(int index, string line, string arretratoAliquota)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var aliquota = line.Substring(27, 5);
            int.TryParse(aliquota, out var aliquotaParsed);
            var expectedDecimalAliquota = Convert.ToDecimal(arretratoAliquota) * 1000;
            return aliquotaParsed == expectedDecimalAliquota
                ? new ValidationResult { IsSucceeded = true } 
                : new ValidationResult($"Errore linea {index} - codice fiscale {codFis}: Aliquota non corretta");
        }
    }
}