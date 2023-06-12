using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using log4net;
using Newtonsoft.Json;
using OCM.TFI.OCM;
using OCM.TFI.OCM.Protocollo;
using OCM.TFI.OCM.Registrazione;
using ProtocolloDll_OPENKM;
using TFI.BLL.Crypto;
using TFI.OCM;
using TFI.OCM.Utilities;

namespace TFI.BLL.DllProtocollo;

public class ProtocolloDll : clsProtocolloDll_OPENKM
{
    private readonly ILog _logger = LogManager.GetLogger("RollingFile");

    public ProtocolloDll()
    {
        SetupProtocollo(ConfigurazioneBase());
    }
    public ResultDtoWithContent<DllProtocolloAllegaResponse> AllegaFilePratica(AllegaProtocolloModel model)
    {
        DllProtocolloAllegaResponse response;
        try
        {
            using (WebClient webClient = new WebClient())
            {
                string ip = GetIp();
                string timeStampMs = GetTimeStampMs();

                webClient.Headers.Add("content-type", "multipart/form-data");
                string requestUri = Input_Connection + "enpaiaDLL/service/allega/" + ip + "/" + timeStampMs + "/" +
                                    model.IdProtocollo;
                HttpClient httpClient = new HttpClient();
                MultipartFormDataContent content1 = new MultipartFormDataContent();
                ByteArrayContent content2 = new ByteArrayContent(model.FileByteArray);
                content2.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = model.FileName
                };
                content1.Add(content2, "file", model.FileName);
                HttpResponseMessage result = httpClient.PostAsync(requestUri, content1).Result;
                var resultString = result.Content.ReadAsStringAsync().Result;
                response =
                    JsonConvert.DeserializeObject<DllProtocolloAllegaResponse>(resultString);
                if (response.Esito != "OK")
                {
                    _logger.Info(
                        $"[TFI.BLL] - {DateTime.Now} : ProtocolloDll - AllegaFileCustom - La chiamata 'Allega File' non è andata a buon fine: {JsonConvert.SerializeObject(response)}");
                    return new ResultDtoWithContent<DllProtocolloAllegaResponse>(false, $"Errore nell'allegare il file {model.FileName}.\n");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Info($"[TFI.BLL] : ProtocolloDll - AllegaFileCustom - E' stata generata un'eccezione in data: {DateTime.Now}. Messaggio: {ex.Message}");
            return new ResultDtoWithContent<DllProtocolloAllegaResponse>(false, "Errore nell'allegare il file");
        }

        var salvaFileSuMacchinaResult = AllegaFileSuMacchina(model);
        
        return !salvaFileSuMacchinaResult.IsSuccessfull 
            ? new ResultDtoWithContent<DllProtocolloAllegaResponse>(false, "Errore nel salvataggio dei file") 
            : new ResultDtoWithContent<DllProtocolloAllegaResponse>(true, response);
    }

    private ResultDto AllegaFileSuMacchina(
        SalvataggioSuMacchinaModel model)
    {   
        var parameterValidationResult = ValidateSalvaSuMacchinaParameters(model);
        if (!parameterValidationResult.IsSuccessfull)
            return new ResultDto(false);
        
        var directoryToSaveResult = GenerateYearlyDirectory();
        if (!directoryToSaveResult.IsSuccessfull)
            return new ResultDto(false);
        
        
        var directoryToSave = directoryToSaveResult.Content;
        if (model.TipoPratica != default)
        {
            var generatePraticaDirectoryResult = CheckAndCreateDirectory(Path.Combine(directoryToSave, model.TipoPratica.ToString()));
            if (!generatePraticaDirectoryResult.IsSuccessfull)
                return new ResultDto(false);
            directoryToSave = generatePraticaDirectoryResult.Content;
        }            

        var generateUserDirectoryResult = GenerateUserDirectory(directoryToSave, model.UserIdentifier);
        if (!generateUserDirectoryResult.IsSuccessfull)
            return new ResultDto(false);

        var resultScriviFileSuMacchina = ScriviFileSuMacchina(generateUserDirectoryResult.Content, model);

        return resultScriviFileSuMacchina.IsSuccessfull 
            ? new ResultDto(true)
            : new ResultDto(false);
    }

    private ResultDto ScriviFileSuMacchina(string directory, SalvataggioSuMacchinaModel model)
    {
        try
        {
            var fileNameWithExtension = string.Concat(Guid.NewGuid().ToString(), Path.GetExtension(model.FileName));
            var writeAllBytesPath = string.Join("_", model.TipoDocumento.ToString(), fileNameWithExtension);
            File.WriteAllBytes(Path.Combine(directory, writeAllBytesPath), model.FileByteArray);
            return new ResultDto(true);
        }
        catch (Exception e)
        {
            _logger.Info(e);
            return new ResultDto(false, "Errore nella scrittura del file");
        }
    }

    private static ResultDtoWithContent<string> ValidateSalvaSuMacchinaParameters(SalvataggioSuMacchinaModel model)
    {
        if (Path.GetExtension(model.FileName).Count(extension => extension.Equals('.')) != 1)
            return new ResultDtoWithContent<string>(false, "estensione del file non valido");
        if (model.UserIdentifier == default) return new ResultDtoWithContent<string>(false, "user identifier fornito non valido");
        if (model.FileByteArray == default) return new ResultDtoWithContent<string>(false, "file in bytes fornito non valido");

        return new ResultDtoWithContent<string>(true);
    }

    private ResultDtoWithContent<string> GenerateYearlyDirectory() {
        var currentDomainBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            
        var baseDirectoryNameResult = CheckAndCreateDirectory(Path.Combine(currentDomainBaseDirectory, "FileProtocollati"));
        if (!baseDirectoryNameResult.IsSuccessfull) return baseDirectoryNameResult;
    
        var yearlySpecificDirectoryNameResult = CheckAndCreateDirectory(Path.Combine(baseDirectoryNameResult.Content, DateTime.Now.Year.ToString()));
        
        //TODO: Definire con giovanni se per i file delle pratiche bisogna salvare il CODPOS della pratica o il codice fiscale/partita iva
        return !yearlySpecificDirectoryNameResult.IsSuccessfull
            ? new ResultDtoWithContent<string>(false, "errore nella creazione della directory specifica per anno")
            : yearlySpecificDirectoryNameResult;
    }

    private ResultDtoWithContent<string> GenerateUserDirectory(string baseDirectoryName, string userIdentifier)
    {
        var userSpecificDirectoryNameResult = CheckAndCreateDirectory(Path.Combine(baseDirectoryName,
            $"{userIdentifier}_{DateTime.Now.Day}-{DateTime.Now.Month}"));
        
        return !userSpecificDirectoryNameResult.IsSuccessfull
            ? new ResultDtoWithContent<string>(false, "Errore nella creazione della directory per utente")
            : userSpecificDirectoryNameResult;
    }
    
    private ResultDtoWithContent<string> CheckAndCreateDirectory(string directory)
    {
        try
        {
            if (Directory.Exists(directory)) return new ResultDtoWithContent<string>(true)
            {
                Content = directory
            };
                    
            var directoryInfo = Directory.CreateDirectory(directory);
            return new ResultDtoWithContent<string>(true)
            {
                Content = directoryInfo.FullName
            };
        }
        catch (Exception e)
        {
            _logger.Info(e);
            return new ResultDtoWithContent<string>(false, "errore nella creazione della directory base");
        }
    }

    public Protocollo ProtocollaPratica(ref string errorMsg)
    {
        try
        {
            var result = Protocolla(ref errorMsg);
            var resultObject = new Protocollo(result);
            if (!string.IsNullOrWhiteSpace(errorMsg))
            {
                _logger.Info(
                    $"[TFI.BLL] - {DateTime.Now} : ProtocolloDll - ProtocollaCustom - Il metodo 'Protocolla' della dll non è andato a buon fine - errore: {errorMsg}");
                resultObject.Message = errorMsg;
                return resultObject;
            }

            resultObject.Success = true;
            return resultObject;
        }
        catch (Exception ex)
        {
            _logger.Info(string.Format(
                "[TFI.BLL] : ProtocolloDll - ProtocollaCustom - E' stata generata un'eccezione in data: {0}. Messaggio: {1}",
                DateTime.Now,
                ex.Message));
            return new Protocollo { Message = ex.Message };
        }
    }
    public byte[] GetFile(int idProtocollo)
    {
      try
      {
        string ip = GetIp();
        string timeStampMs = GetTimeStampMs();
        FormUrlEncodedContent content1 = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) new Dictionary<string, string>()
        {
          {
            nameof (idProtocollo),
            idProtocollo.ToString()
          },
          {
            "ipClient",
            ip
          },
          {
            "timestampRichiesta",
            timeStampMs
          }
        });
        byte[] bytes;
        using (HttpClient httpClient = new HttpClient())
        {
          using (HttpResponseMessage result = httpClient.PostAsync(this.Input_Connection + "enpaiaDLL/service/protocollo/downloadDocumentIAN", (HttpContent) content1).Result)
          {
            using (HttpContent content2 = result.Content)
            {
                if (!result.IsSuccessStatusCode)
                {
                    _logger.Info($"[TFI.BLL] : ProtocolloDll - ProtocollaCustom - E' stata generata un'eccezione in data: {DateTime.Now}. StatusCode: {result.StatusCode} Messaggio: {result.ReasonPhrase}");
                }
                bytes = clsProtocolloDll_OPENKM.ReadFully(content2.ReadAsStreamAsync().Result);
            }
          }
        }
        return bytes;
      }
      catch (Exception ex)
      {
          _logger.Info(string.Format(
              "[TFI.BLL] : ProtocolloDll - GetFileDll - E' stata generata un'eccezione in data: {0}. Messaggio: {1}",
              DateTime.Now,
              ex.Message));
          throw;
      }
    }
    
    public byte[] DownloadAllegato(int idAllegato, string uuid)
    {
      try
      {
        string ip = GetIp();
        string timeStampMs = GetTimeStampMs();
        FormUrlEncodedContent content1 = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) new Dictionary<string, string>()
        {
          {
            nameof (idAllegato),
            idAllegato.ToString()
          },
          {
            nameof (uuid),
            uuid
          },
          {
            "ipClient",
            ip
          },
          {
            "timestampRichiesta",
            timeStampMs
          }
        });
        byte[] bytes;
        using (HttpClient httpClient = new HttpClient())
        {
          using (HttpResponseMessage result = httpClient.PostAsync(this.Input_Connection + "enpaiaDLL/service/protocollo/downloadDocument", (HttpContent) content1).Result)
          {
            using (HttpContent content2 = result.Content)
            {
              ContentDispositionHeaderValue contentDisposition = content2.Headers.ContentDisposition;
              if (!result.IsSuccessStatusCode)
              {
                  _logger.Info($"[TFI.BLL] : ProtocolloDll - Download allegato - E' stata generata un'eccezione in data: {DateTime.Now}. StatusCode: {result.StatusCode} Messaggio: {result.ReasonPhrase}");
              }
              bytes = clsProtocolloDll_OPENKM.ReadFully(content2.ReadAsStreamAsync().Result);
            }
          }
        }
        return bytes;
      }
      catch (Exception ex)
      {
          _logger.Info(string.Format(
              "[TFI.BLL] : ProtocolloDll - DownloadAllegatoDll - E' stata generata un'eccezione in data: {0}. Messaggio: {1}",
              DateTime.Now,
              ex.Message));
        throw new Exception(ex.Message);
      }
    }

    public ProtocolloDll SetupProtocollo(ConfigurazioneProtocolloDll configurazione)
    {
        try
        {
            Input_CodUte = configurazione.User;
            Input_IDRegistro = configurazione.IdRegistro;
            Input_Connection =
                Cypher.DeCryptPassword(ConfigurationManager.AppSettings["linkOPENKM"]);

            Input_CodTit = configurazione.CodTit;

            Input_IDTipoDocumento = configurazione.IdTipoDocumento;

            Input_IDTipoSpedizione = configurazione.IdTipoSpedizione;

            Input_MitDes = new []
            {
                configurazione.TipoMittente + ";" + configurazione.CodicePosizione + ";" +
                configurazione.RagioneSociale + ";" + configurazione.PartitaIva + ";" +
                configurazione.Matricola + ";" + configurazione.Cognome + ";" + configurazione.Nome + ";" +
                configurazione.CodiceFiscale + ";" + configurazione.Indirizzo + ";" + configurazione.Civico +
                ";" + configurazione.Cap + ";" + configurazione.Localita + ";" + configurazione.Comune + ";" +
                configurazione.Provincia
            };
            Input_Oggetto = configurazione.Oggetto;
            Input_Verso = configurazione.Verso;
        }
        catch (Exception ex)
        {
            _logger.Info(string.Format(
                "[TFI.BLL] : ProtocolloDll - SetupProtocollo - E' stata generata un'eccezione in data: {0}. Messaggio: {1}",
                DateTime.Now,
                ex.Message));
        }
        return this;
    }

    public static ConfigurazioneProtocolloDll ConfigurazioneBase()
    {
        return new ConfigurazioneProtocolloDll()
        {
            IdRegistro = 3752069,
            TipoMittente = "Z",
            Oggetto = "oggetto",
            User = "UtenteWeb",
            IdTipoSpedizione = 4792888,
            IdTipoDocumento = 4216,
            CodTit = "1",
            Verso = "P",
            Cap = string.Empty,
            Civico = string.Empty,
            Cognome = string.Empty,
            Nome = string.Empty,
            Provincia = string.Empty,
            Comune = string.Empty,
            Indirizzo = string.Empty,
            Localita = string.Empty,
            Matricola = string.Empty,
            CodiceFiscale = string.Empty,
            RagioneSociale = string.Empty,
            CodicePosizione = string.Empty
        };
    }

    public static ConfigurazioneProtocolloDll ConfigurazioneRegistrazioneAzienda(RegistrazioneAziendaModel nuovaAzienda)
    {
        return new ConfigurazioneProtocolloDll()
        {
            IdRegistro = 3752069,
            TipoMittente = "AZ",
            Oggetto = $"Registrazione Azienda {nuovaAzienda.DatiAzienda.RagioneSociale}",
            User = "UtenteWeb",
            IdTipoSpedizione = 4792888,
            IdTipoDocumento = 4216,
            CodTit = "1",
            Verso = "P",
            Cap = nuovaAzienda.SedeLegale.CAP,
            Civico = nuovaAzienda.SedeLegale.Civico,
            Provincia = nuovaAzienda.SedeLegale.Provincia,
            Comune = nuovaAzienda.SedeLegale.Comune,
            Indirizzo = nuovaAzienda.SedeLegale.Indirizzo,
            Localita = nuovaAzienda.SedeLegale.Localita,
            CodiceFiscale = nuovaAzienda.DatiAzienda.CodiceFiscale ?? string.Empty,
            RagioneSociale = nuovaAzienda.DatiAzienda.RagioneSociale,
            PartitaIva = nuovaAzienda.DatiAzienda.PartitaIva ?? string.Empty,
            Matricola = string.Empty,
            CodicePosizione = string.Empty,
            Cognome = string.Empty,
            Nome = string.Empty
        };
    }

    private static string GetTimeStampMs() => DateTime.Now.ToString("yyyyMMddhhmmss");

    private static string GetIp() => Dns.GetHostEntry(Dns.GetHostName()).AddressList
        .LastOrDefault<IPAddress>()
        ?.ToString();
}