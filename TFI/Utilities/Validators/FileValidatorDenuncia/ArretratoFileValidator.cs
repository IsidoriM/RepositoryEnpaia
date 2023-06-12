using System;
using System.Collections.Generic;
using System.Linq;
using TFI.OCM.AziendaConsulente;

namespace TFI.Utilities.Validators.FileValidatorDenuncia
{
    public class ArretratoFileValidator : DenunciaFileValidator
    {
        public static ValidationResult ReadAndValidateArretratoUploadFile(List<string> fileContent, int anno, List<DenunciaArretrati> dati)
        {
            if (fileContent.Count == 0)
                return new ValidationResult("Il file non ha contenuto");
            
            var validationResult = new ValidationResult { IsSucceeded = true };

            // if (fileContent.Count != dati.Count)
            // {
            //     validationResult = ValidateCodFis(fileContent, dati);
            //     validationResult += new ValidationResult($"Il numero delle righe non coincide con il numero di rapporti di lavoro presenti: attesi {dati.Count}, trovati {fileContent.Count}");
            //     return validationResult;
            // }
            validationResult += ValidateAllLineLength(fileContent);
            if (!validationResult.IsSucceeded)
            {
                return validationResult;
            }

            validationResult += ValidateAnnoCompetenzaArretrato(fileContent, anno);
            if (!validationResult.IsSucceeded)
            {
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
                var result = ValidateArretratiUpload(index++, line, anno, dati);
                validationResult += result;
            }
            
            return validationResult;
        }
        
        private static ValidationResult ValidateArretratiUpload(int index, string line, int anno, List<DenunciaArretrati> dati)
        {
            var validationResult = new ValidationResult { IsSucceeded = true };

            var validation1 = ValidateLineLength(index, line);
            if (!validation1.IsSucceeded)
                return validation1;
            
            // var validation2 = ValidateAnnoCompetenzaArretrato(index, line, anno);
            // if (!validation2.IsSucceeded)
            //     return validation2;

            var validation3 = ValidateNumberFormat(index, line);
            if (!validation3.IsSucceeded)
                return validation3;
            
            //var validation3 = ValidateCodFis(index, line, dati);
            //if (!validation3.IsSucceeded)
            //    return validation3;

            // var validation4 = ValidateProDenArretrato(index, line, dati);
            // if (!validation4.IsSucceeded)
            //     return validation4;
            
            var arretrato = dati.Single(e =>
                string.Equals(e.CodFis, line.Substring(0, 16), StringComparison.InvariantCultureIgnoreCase) &&
                e.proDen == int.Parse(line.Substring(16, 2)));
            
            validationResult += ValidateNumberValue(index, line);
            validationResult += ValidateDalAl(index, line, anno, arretrato.datadal.Substring(0,10), arretrato.dataal.Substring(0,10));
            validationResult += ValidateAliquota(index, line, arretrato.aliquota);
            validationResult += ValidateTipoRetrArretrati(index, line);
            validationResult += ValidateImpOcc(index, line);
            validationResult += ValidateImpFig(index, line, arretrato.numggsos);

            return validationResult;
        }
        
        private static ValidationResult ValidateAnnoCompetenzaArretrato(List<string> fileContent, int anno)
        {
            var validationResult = new ValidationResult() { IsSucceeded = true };
            for (int i = 0; i < fileContent.Count; i++)
            {
                var codFis = fileContent[i].Substring(0, 16).ToUpper();
                var annoCompetenza = fileContent[i].Substring(fileContent[i].Length - 4);
                if (annoCompetenza != anno.ToString())
                {
                    validationResult += new ValidationResult($"Errore linea {i+1} - codice fiscale {codFis}: anno competenza non valido");
                }
            }

            return validationResult;
        }

        private static ValidationResult ValidateTipoRetrArretrati(int index, string line)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var tipoRetr = line.Substring(18, 1);

            return tipoRetr.ToUpper() == "A"
                ? new ValidationResult { IsSucceeded = true } 
                : new ValidationResult($"Errore linea {index} - codice fiscale {codFis}: valore non valido, per il caricamento degli arretrati inserire A");
        }
        
        private static ValidationResult ValidateCodFis(List<string> fileContent, List<DenunciaArretrati> dati)
        {
            var expectedCodsFis = dati.Select(r => r.CodFis.ToUpper()).ToList();
            var codsFis = fileContent.Select(line => line.Substring(0, 16).ToUpper()).ToList();
            
            var offListCodsFis = codsFis.Except(expectedCodsFis).ToList();

            var validationResult = new ValidationResult { IsSucceeded = true };

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