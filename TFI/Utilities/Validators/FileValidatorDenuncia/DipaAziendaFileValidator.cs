using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TFI.OCM.AziendaConsulente;

namespace TFI.Utilities.Validators.FileValidatorDenuncia
{
    public class DipaAziendaFileValidator : DenunciaFileValidator
    {
        public static ValidationResult ReadAndValidateDipaUploadFile(List<string> fileContent, int anno, DatiNuovaDenuncia dati)
        {
            if (fileContent.Count == 0)
                return new ValidationResult("Il file non ha contenuto");
            
            var validationResult = new ValidationResult { IsSucceeded = true };

            if (fileContent.Count != dati.ListaReport.Count)
            {
                validationResult = ValidateCodFis(fileContent, dati);
                validationResult += new ValidationResult($"Il numero delle righe non coincide con il numero di rapporti di lavoro presenti: attesi {dati.ListaReport.Count}, trovati {fileContent.Count}");
                return validationResult;
            }

            validationResult += ValidateCodFis(fileContent, dati);
            if (!validationResult.IsSucceeded)
            {
                return validationResult;
            }

            var index = 1;

            foreach (var line in fileContent)
            {
                var result = ValidateDipaUpload(index++, line, anno, dati);
                validationResult += result;
            }
            
            return validationResult;
        }
        
        protected static ValidationResult ValidateDipaUpload(int index, string line, int anno, DatiNuovaDenuncia dati)
        {
            var validationResult = new ValidationResult { IsSucceeded = true };

            var validation1 = ValidateLineLength(index, line);
            if (!validation1.IsSucceeded)
                return validation1;

            var validation2 = ValidateNumberFormat(index, line);
            if (!validation2.IsSucceeded)
                return validation2;

            //var validation3 = ValidateCodFis(index, line, dati);
            //if (!validation3.IsSucceeded)
            //    return validation3;

            var validation4 = ValidateProDip(index, line, dati);
            if (!validation4.IsSucceeded)
                return validation4;

            var retribuzione = dati.ListaReport.Single(e =>
                string.Equals(e.CodFis, line.Substring(0, 16), StringComparison.InvariantCultureIgnoreCase) &&
                e.ProDenDet == int.Parse(line.Substring(16, 2)));

            validationResult += ValidateNumberValue(index, line);
            validationResult += ValidateDalAl(index, line, anno, retribuzione.Dal, retribuzione.Al);
            validationResult += ValidateTipoRetr(index, line);
            validationResult += ValidateImpOcc(index, line);
            validationResult += ValidateImpFig(index, line, Convert.ToInt32(retribuzione.NumGGSos));

            return validationResult;
        }

        protected static ValidationResult ValidateProDip(int index, string line, DatiNuovaDenuncia dati)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var retribuzione = dati.ListaReport.SingleOrDefault(e =>
                string.Equals(e.CodFis, line.Substring(0, 16), StringComparison.InvariantCultureIgnoreCase) &&
                e.ProDenDet == int.Parse(line.Substring(16, 2)));

            return retribuzione == null 
                ? new ValidationResult($"Errore linea {index} - codice fiscale {codFis}: Progressivo non trovato per l'utente {line.Substring(0, 16)}") 
                : new ValidationResult { IsSucceeded = true };
        }

        protected static ValidationResult ValidateTipoRetr(int index, string line)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var tipoRetr = line.Substring(18, 1);

            return tipoRetr.ToUpper() == "M"
                ? new ValidationResult { IsSucceeded = true } 
                : new ValidationResult($"Errore linea {index} - codice fiscale {codFis}: valore non valido, per il caricamento della denuncia mensile inserire M");
        }

        protected static ValidationResult ValidateCodFis(List<string> fileContent, DatiNuovaDenuncia dati)
        {
            var expectedCodsFis = dati.ListaReport.Select(r => r.CodFis.ToUpper()).ToList();
            var codsFis = fileContent.Select(line => line.Substring(0, 16).ToUpper()).ToList();

            var missingCodsFis = expectedCodsFis.Except(codsFis).ToList();
            var offListCodsFis = codsFis.Except(expectedCodsFis).ToList();

            var validationResult = new ValidationResult { IsSucceeded = true };

            if (!missingCodsFis.Any() && !offListCodsFis.Any())
            {
                return validationResult;
            }

            if (missingCodsFis.Any())
            {
                var joinedMissingCodsFis = string.Join("</li><li>", missingCodsFis);
                var missingCodsFisUnorderedList = $"<ul style=\"list-style-type: none\"><li>{joinedMissingCodsFis}</li></ul>";
                validationResult += new ValidationResult($"Errore - non sono stati trovati i seguenti codici fiscali:{missingCodsFisUnorderedList}");
            }

            if (offListCodsFis.Any())
            {
                var joinedOffListCodsFis = string.Join("</li><li>", offListCodsFis);
                var offListCodsFisUnorderedList = $"<ul style=\"list-style-type: none\"><li>{joinedOffListCodsFis}</li></ul>";
                validationResult += new ValidationResult($"Errore - i seguenti codici fiscali non fanno parte della denuncia:{offListCodsFisUnorderedList}");
            }

            return validationResult;
        }
    }
}