using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TFI.OCM.AziendaConsulente;

namespace TFI.Utilities.Validators
{
    public static class DipaFileValidator
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
        public static ValidationResult ReadAndValidateArretratoUploadFile(List<string> fileContent, int anno, List<DenunciaArretrati> dati)
        {
            if (fileContent.Count == 0)
                return new ValidationResult("Il file non ha contenuto");
            
            var validationResult = new ValidationResult { IsSucceeded = true };

            if (fileContent.Count != dati.Count)
            {
                validationResult = ValidateCodFis(fileContent, dati);
                validationResult += new ValidationResult($"Il numero delle righe non coincide con il numero di rapporti di lavoro presenti: attesi {dati.Count}, trovati {fileContent.Count}");
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

        private static ValidationResult ValidateDipaUpload(int index, string line, int anno, DatiNuovaDenuncia dati)
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

        private static ValidationResult ValidateArretratiUpload(int index, string line, int anno, List<DenunciaArretrati> dati)
        {
            var validationResult = new ValidationResult { IsSucceeded = true };

            var validation1 = ValidateLineLength(index, line);
            if (!validation1.IsSucceeded)
                return validation1;
            
            var validation2 = ValidateAnnoCompetenzaArretrato(index, line, anno);
            if (!validation2.IsSucceeded)
                return validation2;

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
            validationResult += ValidateDalAl(index, line, anno, arretrato.datadal, arretrato.dataal);
            validationResult += ValidateTipoRetrArretrati(index, line);
            validationResult += ValidateImpOcc(index, line);
            validationResult += ValidateImpFig(index, line, arretrato.numggsos);

            return validationResult;
        }

        private static ValidationResult ValidateProDenArretrato(int index, string line, List<DenunciaArretrati> dati)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var retribuzione = dati.SingleOrDefault(e =>
                string.Equals(e.CodFis, line.Substring(0, 16), StringComparison.InvariantCultureIgnoreCase) &&
                e.proDen == int.Parse(line.Substring(16, 2)));

            return retribuzione == null 
                ? new ValidationResult($"Errore linea {index} - codice fiscale {codFis}: Progressivo non trovato per l'utente {line.Substring(0, 16)}") 
                : new ValidationResult { IsSucceeded = true };
        }

        private static ValidationResult ValidateAnnoCompetenzaArretrato(int index, string line, int anno)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var annoCompetenza = line.Substring(line.Length - 4);

            return annoCompetenza == anno.ToString()
                ? new ValidationResult { IsSucceeded = true } 
                : new ValidationResult($"Errore linea {index} - codice fiscale {codFis}: anno competenza non valido");
        }

        private static ValidationResult ValidateImpFig(int index, string line, int ggSospensione)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var figurat = decimal.Parse(line.Substring(50, 9));

            return figurat > 0 && ggSospensione == 0 
                ? new ValidationResult($"Errore linea {index} - codice fiscale {codFis}: Retribuzione figurativa senza sospensione")
                : new ValidationResult { IsSucceeded = true };
        }

        private static ValidationResult ValidateProDip(int index, string line, DatiNuovaDenuncia dati)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var retribuzione = dati.ListaReport.SingleOrDefault(e =>
                string.Equals(e.CodFis, line.Substring(0, 16), StringComparison.InvariantCultureIgnoreCase) &&
                e.ProDenDet == int.Parse(line.Substring(16, 2)));

            return retribuzione == null 
                ? new ValidationResult($"Errore linea {index} - codice fiscale {codFis}: Progressivo non trovato per l'utente {line.Substring(0, 16)}") 
                : new ValidationResult { IsSucceeded = true };
        }

        private static ValidationResult ValidateLineLength(int index, string line)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            if (line.Length != 59 && line.Length != 63)
                return new ValidationResult(
                    $"Errore linea {index} - codice fiscale {codFis}: numero di caratteri non valido, attesi 59 o 63 ma trovati {line.Length}");

            return new ValidationResult { IsSucceeded = true };
        }

        private static ValidationResult ValidateDalAl(int index, string line, int anno, string dal, string al)
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
                var dataInserita = new DateTime(anno, int.Parse(value.Substring(0, 2)), int.Parse(value.Substring(2, 2)));
                var dataAspettataParsed = DateTime.ParseExact(dataAspettata, "dd/MM/yyyy", CultureInfo.CurrentCulture);

                if (dataInserita.Date != dataAspettataParsed.Date)
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

        private static ValidationResult ValidateTipoRetr(int index, string line)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var tipoRetr = line.Substring(18, 1);

            return tipoRetr.ToUpper() == "M"
                ? new ValidationResult { IsSucceeded = true } 
                : new ValidationResult($"Errore linea {index} - codice fiscale {codFis}: valore non valido, per il caricamento della denuncia mensile inserire M");
        }
        
        private static ValidationResult ValidateTipoRetrArretrati(int index, string line)
        {
            var codFis = line.Substring(0, 16).ToUpper();
            var tipoRetr = line.Substring(18, 1);

            return tipoRetr.ToUpper() == "A"
                ? new ValidationResult { IsSucceeded = true } 
                : new ValidationResult($"Errore linea {index} - codice fiscale {codFis}: valore non valido, per il caricamento degli arretrati inserire A");
        }

        //private static ValidationResult ValidateCodFis(int index, string line, DatiNuovaDenuncia dati)
        //{
        //    var codfis = line.Substring(0, 16);
        //    return dati.ListaReport.Any(e => string.Equals(e.CodFis, codfis, StringComparison.InvariantCultureIgnoreCase)) 
        //        ? new ValidationResult { IsSucceeded = true } 
        //        : new ValidationResult($"Errore linea {index}: codice fiscale {codfis} non trovato");
        //}

        private static ValidationResult ValidateCodFis(List<string> fileContent, DatiNuovaDenuncia dati)
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
        
        private static ValidationResult ValidateCodFis(List<string> fileContent, List<DenunciaArretrati> dati)
        {
            var expectedCodsFis = dati.Select(r => r.CodFis.ToUpper()).ToList();
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

        private static ValidationResult ValidateNumberFormat(int index, string line)
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

        private static ValidationResult ValidateNumberValue(int index, string line)
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

        private static ValidationResult ValidateImpOcc(int index, string line)
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

    }
}