using System.Collections.Generic;
using System.Linq;
using TFI.OCM.AziendaConsulente;

namespace TFI.Utilities.Validators.FileValidatorDenuncia
{
    public class DipaConsulenteFileValidator : DipaAziendaFileValidator
    {
        public static ValidationResult ReadAndValidateDipaConsulenteUploadFile(List<FileLine> fileContent, int anno, DatiNuovaDenuncia dati)
        {
            if (fileContent.Count == 0)
                return new ValidationResult("Il file non ha contenuto");
            
            var validationResult = new ValidationResult { IsSucceeded = true };
            var lineContent = fileContent.Select(line => line.Content).ToList();

            if (fileContent.Count != dati.ListaReport.Count)
            {
                validationResult = ValidateCodFis(lineContent, dati);
                validationResult += new ValidationResult($"Il numero delle righe non coincide con il numero di rapporti di lavoro presenti: attesi {dati.ListaReport.Count}, trovati {fileContent.Count}");
                return validationResult;
            }

            validationResult += ValidateCodFis(lineContent, dati);
            if (!validationResult.IsSucceeded)
            {
                return validationResult;
            }
            
            foreach (var line in fileContent)
            {
                var result = ValidateDipaUpload(line.Index, line.Content, anno, dati);
                validationResult += result;
            }
            
            return validationResult;
        }
    }
}