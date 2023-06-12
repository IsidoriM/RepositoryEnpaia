using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using log4net;
using OCM.Enums;
using OCM.TFI.OCM.Protocollo;
using OCM.TFI.OCM.Registrazione;
using OCM.TFI.OCM.Utilities;
using TFI.BLL.DllProtocollo;
using TFI.BLL.Utilities;
using TFI.BLL.Utilities.Pdf;
using TFI.DAL.Registrazione;
using TFI.OCM.Amministrativo;
using TFI.OCM.Utilities;

namespace TFI.BLL.Registrazione
{
    public class RegistrazioneAziendaBLL
    {
        private readonly RegistrazioneDAL _registrazioneDAL = new();
        private readonly PdfService _pdfService = new();
        private readonly ProtocolloDll _protocolloDll = new();
        private static readonly ILog _logger = LogManager.GetLogger("RollingFile");
        
        public ResultRegistrazioneAzienda SalvaAzienda(RegistrazioneAziendaModel nuovaAzienda, ref string errorMsg,
            ref string successMsg)
        {
            if (_registrazioneDAL.PartitaIvaExists(nuovaAzienda.DatiAzienda.PartitaIva))
            {
                errorMsg = "Azienda con questa partita iva è gia presente";
                return new ResultRegistrazioneAzienda { StatoRegistrazione = StatoRegistrazioneAzienda.Fallita };
            }
            if (_registrazioneDAL.CodiceFiscaleExists(nuovaAzienda.DatiAzienda.CodiceFiscale))
            {
                errorMsg = "Azienda con questo Codice Fiscale gia presente";
                return new ResultRegistrazioneAzienda { StatoRegistrazione = StatoRegistrazioneAzienda.Fallita };
            }
            
            ManageStringCaseProperty(nuovaAzienda);
            _registrazioneDAL.AvviaTransazione();
            var protocollo = Protocolla(nuovaAzienda, ref errorMsg);
            if (!protocollo.Success)
            {
                _registrazioneDAL.RollbackTransazione();
                return new ResultRegistrazioneAzienda { StatoRegistrazione = StatoRegistrazioneAzienda.Fallita };
            }
            
            var resultSalvataggio = _registrazioneDAL.SalvaAzienda(nuovaAzienda, protocollo, ref errorMsg);
            if (!resultSalvataggio)
            {
                _registrazioneDAL.RollbackTransazione();
                return new ResultRegistrazioneAzienda { StatoRegistrazione = StatoRegistrazioneAzienda.Fallita };
            }
            
            foreach (var documento in GetDocumentList(nuovaAzienda.DocumentiRegistrazioneAzienda))
            {
                if (documento == null || documento.File == null) continue;
                
                var (success, result) = AllegaFile(new AllegaProtocolloModel
                {
                    FileByteArray = documento.File.FromHttpPostedFileBaseToByteArray(),
                    FileName = documento.File.FileName,
                    UserIdentifier = nuovaAzienda.DatiAzienda.CodiceAziendaWeb,
                    TipoDocumento =  documento.TipoDocumento,
                    IdProtocollo = protocollo.IdProtocollo,
                    TipoPratica = TipoPratica.RegistrazioneAzienda
                }, ref errorMsg);
                if (!success) continue;
                RegistraDocumentiAllegati(nuovaAzienda.DatiAzienda.PartitaIva, protocollo.IdProtocollo,
                    documento.TipoDocumento, result.Data.IdAllegato, result.Data.UuidAllegato, ref errorMsg);
            }

            var documentiSalvati = _registrazioneDAL.GetDocumentiCaricatiRegistrazioneAzienda(protocollo.IdProtocollo);
            var dettaglioResult = new ResultRegistrazioneAzienda
            {
                Protocollo = new ProtocolloDetail(protocollo),
                DocumentiCaricati = documentiSalvati,
                DocumentiDaCaricare = GetDocumentiDaCaricare(documentiSalvati, GetDocumentList(nuovaAzienda.DocumentiRegistrazioneAzienda)),
                StatoRegistrazione = StatoRegistrazioneAzienda.InAttestaDiDocumentazione,
                PartitaIva = nuovaAzienda.DatiAzienda.PartitaIva,
                CodicePosizione = nuovaAzienda.DatiAzienda.CodiceAziendaWeb
            };
            
            if (documentiSalvati.Count == GetDocumentList(nuovaAzienda.DocumentiRegistrazioneAzienda).Count())
                dettaglioResult.StatoRegistrazione = StatoRegistrazioneAzienda.InLavorazione;

            if (!_registrazioneDAL.AggiornaStatoPraticaRegistrazioneAzienda(nuovaAzienda.DatiAzienda.CodiceAziendaWeb,
                    dettaglioResult.StatoRegistrazione, ref errorMsg))
            {
                _registrazioneDAL.RollbackTransazione();
                return new ResultRegistrazioneAzienda { StatoRegistrazione = StatoRegistrazioneAzienda.Fallita };
            }
            
            var pdfAllegato = _pdfService.CreaPdfRicevutaRegistrazioneAzienda(GetDatiPdf(nuovaAzienda, protocollo), ref errorMsg);
            if (pdfAllegato == null)
            {
                _registrazioneDAL.RollbackTransazione();
                return new ResultRegistrazioneAzienda { StatoRegistrazione = StatoRegistrazioneAzienda.Fallita };
            }

            var identificativoAziendaPerToken = string.IsNullOrEmpty(nuovaAzienda.DatiAzienda.PartitaIva)
                ? nuovaAzienda.DatiAzienda.CodiceFiscale
                : nuovaAzienda.DatiAzienda.PartitaIva;
            var tokenPerLinkEmail = CreateToken(identificativoAziendaPerToken, protocollo.NumeroProtocollo);
            var (sendResult, _) = SmtpEmailService.InviaEmailSalvataggioAzienda(
                nuovaAzienda.DatiAzienda.Email, nuovaAzienda.DatiAzienda.PartitaIva, nuovaAzienda.DatiAzienda.RagioneSociale, tokenPerLinkEmail, pdfAllegato);
            if (!sendResult)
            {
                errorMsg = "Errore nell'invio dell'email";
                _registrazioneDAL.RollbackTransazione();
                return new ResultRegistrazioneAzienda { StatoRegistrazione = StatoRegistrazioneAzienda.Fallita };
            }
            
            var resultAllegaPdf =
                _protocolloDll.AllegaFilePratica(new AllegaProtocolloModel
                {
                    IdProtocollo = protocollo.IdProtocollo,
                    TipoPratica = TipoPratica.RegistrazioneAzienda,
                    FileName = $"RegistrazionePratica_{protocollo.IdProtocollo}.pdf",
                    FileByteArray = pdfAllegato,
                    UserIdentifier = nuovaAzienda.DatiAzienda.CodiceAziendaWeb,
                    TipoDocumento = TipoDocumento.RicevutaRegistrazioneAzienda
                });
            if (!resultAllegaPdf.IsSuccessfull)
            {
                _registrazioneDAL.RollbackTransazione();
                errorMsg = "Errore nella creazione della ricevuta";
                return new ResultRegistrazioneAzienda { StatoRegistrazione = StatoRegistrazioneAzienda.Fallita };
            }
            
            _registrazioneDAL.CommitTransazione();
            successMsg = "Registrazione avvenuta con successo";
            return dettaglioResult;

            Protocollo Protocolla(RegistrazioneAziendaModel registrazioneModel, ref string errorMsg)
            {
                try
                {
                    return _protocolloDll
                        .SetupProtocollo(ProtocolloDll.ConfigurazioneRegistrazioneAzienda(registrazioneModel))
                        .ProtocollaPratica(ref errorMsg);
                }
                catch (Exception ex)
                {
                    _logger.Info(string.Format(
                        "[TFI.BLL] : RegistrazioneBLL - E' stata generata un'eccezione in data: {0}. Messaggio: {1}",
                        DateTime.Now, ex.Message));
                    return new Protocollo{Success = false};
                }
            }

            string CreateToken(string identificativoAzienda, string numeroProtocollo)
            {
                var claims = new List<Claim>
                {
                    new("identificativoAzienda", identificativoAzienda),
                    new("numeroProtocollo", numeroProtocollo.ToString())
                };
                return TokenHelper.CreateToken(claims, 1000);
            }
            
            DatiPdfRegistrazioneAzienda GetDatiPdf(RegistrazioneAziendaModel nuovaAzienda, Protocollo protocollo)
            {
                return new DatiPdfRegistrazioneAzienda
                {
                    DatiAzienda = new GestioneAziendeWebOCM.DatiAzienda
                    {
                        PartitaIva = nuovaAzienda.DatiAzienda.PartitaIva,
                        CodiceFiscale = nuovaAzienda.DatiAzienda.CodiceFiscale,
                        RagioneSociale = nuovaAzienda.DatiAzienda.RagioneSociale
                    },
                    Protocollo = protocollo
                };
            }
        }

        public Comune GetDenominazioneComuneAndSiglaProvinciaFrom(string codiceComune) => _registrazioneDAL.GetDenominazioneComuneAndSiglaProvinciaFrom(codiceComune);

        private (bool Success, DllProtocolloAllegaResponse Response) AllegaFile(AllegaProtocolloModel model, ref string errorMsg)
        {
            if (model.FileByteArray == null || model.FileByteArray.Length <= 0) return (false, null);
            if (!Utils.FileExtensionsIsValid(model.FileName))
            {
                errorMsg +=
                    $"Errore: l'estensione del file {model.FileName} è errata. Sono accettati solamente: .pdf, .jpeg, .jpg, .png";
                return (false, null);
            }
            
            var resultAllega =
                _protocolloDll.AllegaFilePratica(model);
            
            return !resultAllega.IsSuccessfull 
                ? (false, null)
                : (true, resultAllega.Content);
        }

        public DettaglioPraticaRegistrazioneAzienda GetDettaglioPratica(string identificativoAzienda, string numProtocollo, ref string errorMsg)
        {
            var result = _registrazioneDAL.GetDettaglioPratica(identificativoAzienda, numProtocollo, ref errorMsg);
            if (result == null) return default;
            
            result.DocumentiDaCaricare =
                GetDocumentiDaCaricare(result.DocumentiCaricati, GetAllDocumentiRegistrazioneAzienda());

            return result;
        }
        
        public ResultRegistrazioneAzienda SalvaDocumentiRegistrazioneAzienda(DettaglioPraticaRegistrazioneAzienda praticaRegistrazione, ref string errorMsg)
        {
            foreach (var documento in GetDocumentList(praticaRegistrazione.DocumentiInCaricamento))
            {
                if (documento == null || documento.File == null) continue;
                var (success, result) = AllegaFile(new AllegaProtocolloModel
                {
                    FileByteArray = documento.File.FromHttpPostedFileBaseToByteArray(),
                    FileName = documento.File.FileName,
                    IdProtocollo = praticaRegistrazione.Protocollo.IdProtocollo,
                    TipoDocumento = documento.TipoDocumento,
                    UserIdentifier = praticaRegistrazione.CodicePosizione,
                    TipoPratica = TipoPratica.RegistrazioneAzienda
                }, ref errorMsg);
                
                if (!success) continue;
                
                _registrazioneDAL.AvviaTransazione();
                RegistraDocumentiAllegati(praticaRegistrazione.PartitaIva, praticaRegistrazione.Protocollo.IdProtocollo,
                    documento.TipoDocumento, result.Data.IdAllegato, result.Data.UuidAllegato, ref errorMsg);
                _registrazioneDAL.CommitTransazione();
            }

            var documentiSalvati = _registrazioneDAL.GetDocumentiCaricatiRegistrazioneAzienda(praticaRegistrazione.Protocollo.IdProtocollo);

            _registrazioneDAL.GetDettaglioPratica(praticaRegistrazione.PartitaIva,
                praticaRegistrazione.Protocollo.NumeroProtocollo, ref errorMsg);

            var documentiDaCaricare = GetAllDocumentiRegistrazioneAzienda().Count;

            if (documentiSalvati.Count == documentiDaCaricare)
                _registrazioneDAL.AggiornaStatoPraticaRegistrazioneAzienda(
                    praticaRegistrazione.CodicePosizione, StatoRegistrazioneAzienda.InLavorazione,
                    ref errorMsg);

            var dettaglioResult = new DettaglioPraticaRegistrazioneAzienda
            {
                Protocollo = praticaRegistrazione.Protocollo,
                DocumentiCaricati = documentiSalvati,
                DocumentiDaCaricare = GetDocumentiDaCaricare(documentiSalvati,
                    GetDocumentList(praticaRegistrazione.DocumentiInCaricamento)),
                StatoRegistrazione = StatoRegistrazioneAzienda.InAttestaDiDocumentazione
            };

            if (documentiSalvati.Count != GetDocumentList(praticaRegistrazione.DocumentiInCaricamento).Count())
                return dettaglioResult;
            
            _registrazioneDAL.AvviaTransazione();
            if (!_registrazioneDAL.AggiornaStatoPraticaRegistrazioneAzienda(
                    praticaRegistrazione.CodicePosizione, StatoRegistrazioneAzienda.InLavorazione,
                    ref errorMsg))
            {
                _registrazioneDAL.RollbackTransazione();
                return dettaglioResult;
            }
            _registrazioneDAL.CommitTransazione();
            
            dettaglioResult.StatoRegistrazione = StatoRegistrazioneAzienda.InLavorazione;
            return dettaglioResult;
        }
        
        private static List<Documento> GetDocumentiDaCaricare(List<Documento> documentiCaricati, IEnumerable<Documento> tuttiDocumenti)
        {
            return tuttiDocumenti.Where(documento => documento !=  null &&
                !documentiCaricati.Select(doc => doc.TipoDocumento).Contains(documento.TipoDocumento)).ToList();
        }

        private static List<Documento> GetAllDocumentiRegistrazioneAzienda()
        {
            return new List<Documento>
            {
                new (TipoDocumento.DelegaConsulente),
                new (TipoDocumento.DocumentoLegaleRappresentante),
                new (TipoDocumento.PartitaIva),
                new (TipoDocumento.DM80),
                new (TipoDocumento.CertificatoIscrizioneCCIAA),
                new (TipoDocumento.StatutoAttoCostitutivo),
                new (TipoDocumento.InformativaPrivacy)
            };
        }

        private static IEnumerable<Documento> GetDocumentList(FileDocumentiRegistrazioneAzienda documentiRegistrazione)
        {
            return new List<Documento>
            {
                documentiRegistrazione.DelegaConsulente,
                documentiRegistrazione.DM80,
                documentiRegistrazione.InformativaPrivacy,
                documentiRegistrazione.StatutoAttoCostitutivo,
                documentiRegistrazione.DocumentoLegaleRappresentante,
                documentiRegistrazione.PartitaIva,
                documentiRegistrazione.CertificatoIscrizioneCCIAA
            };
        }
        
        private void RegistraDocumentiAllegati(string partitaIva, int idProtocollo, TipoDocumento tipoDocumento, int idAllegato, string uuidAllegato, ref string errorMsg)
        {
            _registrazioneDAL.SalvaPraticaDocumenti(GetRecordRegistrazioneDocumento(), ref errorMsg);

            RecordRegistrazioneDocumentoAzienda GetRecordRegistrazioneDocumento()
            {
                return new RecordRegistrazioneDocumentoAzienda
                {
                    PartitvaIva = partitaIva,
                    IdProtocollo = idProtocollo,
                    IdTipoDocumento = (int)tipoDocumento,
                    IdAllegato = idAllegato,
                    UUID = uuidAllegato,
                    IdReg = _protocolloDll.Input_IDRegistro
                };
            }
        }
        
        private static void ManageStringCaseProperty(RegistrazioneAziendaModel nuovaAzienda)
        {
            UpperDatiAzienda(nuovaAzienda.DatiAzienda);
            LowerEmailPecDatiAzienda(nuovaAzienda.DatiAzienda);
            UpperRapLeg(nuovaAzienda.RappresentanteLegale);
            LowerEmailPecRapLeg(nuovaAzienda.RappresentanteLegale);
            UpperAltriDati(nuovaAzienda.AltriDati);
            UpperSedeLegale(nuovaAzienda.SedeLegale);
            UpperCorrispondenza(nuovaAzienda.Corrispondenza);

            void UpperDatiAzienda(GestioneAziendeWebOCM.DatiAzienda datiAzienda)
            {
                datiAzienda.RagioneSociale = datiAzienda.RagioneSociale?.ToUpper();
                datiAzienda.RagioneSocialeBreve = datiAzienda.RagioneSocialeBreve?.ToUpper();
                datiAzienda.PartitaIva = datiAzienda.PartitaIva?.ToUpper();
                datiAzienda.CodiceFiscale = datiAzienda.CodiceFiscale?.ToUpper();
                datiAzienda.URL = datiAzienda.URL?.ToUpper();
                datiAzienda.MezziComunicazione = datiAzienda.MezziComunicazione?.ToUpper();
                datiAzienda.NaturaGiuridica = datiAzienda.NaturaGiuridica?.ToUpper();
            }
            void LowerEmailPecDatiAzienda(GestioneAziendeWebOCM.DatiAzienda datiAzienda)
            {
                datiAzienda.Email = datiAzienda.Email?.ToLower();
                datiAzienda.Pec = datiAzienda.Pec?.ToLower();
            }

            void UpperRapLeg(GestioneAziendeWebOCM.RapLeg rapLeg)
            {
                rapLeg.RapLegPrinc = rapLeg.RapLegPrinc?.ToUpper();
                rapLeg.TipoVia = rapLeg.TipoVia?.ToUpper();
                rapLeg.Indirizzo = rapLeg.Indirizzo?.ToUpper();
                rapLeg.Civico = rapLeg.Civico?.ToUpper();
                rapLeg.Comune = rapLeg.Comune?.ToUpper();
                rapLeg.Localita = rapLeg.Localita?.ToUpper();
                rapLeg.ComuneNascita = rapLeg.ComuneNascita?.ToUpper();
                rapLeg.Provincia = rapLeg.Provincia?.ToUpper();
                rapLeg.TipoRapp = rapLeg.TipoRapp?.ToUpper();
                rapLeg.Nome = rapLeg.Nome?.ToUpper();
                rapLeg.Cognome = rapLeg.Cognome?.ToUpper();
                rapLeg.StatoEstero = rapLeg.StatoEstero?.ToUpper();
                rapLeg.ProvinciaNas = rapLeg.ProvinciaNas?.ToUpper();
                rapLeg.CodiceFiscale = rapLeg.CodiceFiscale?.ToUpper();
                rapLeg.Sesso = rapLeg.Sesso?.ToUpper();
                rapLeg.StatoEsteroNascita = rapLeg.StatoEsteroNascita?.ToUpper();
                rapLeg.SigPro = rapLeg.SigPro?.ToUpper();
            }
            
            void LowerEmailPecRapLeg(GestioneAziendeWebOCM.RapLeg rapLeg)
            {
                rapLeg.Email = rapLeg.Email?.ToLower();
                rapLeg.EmailCert = rapLeg.EmailCert?.ToLower();
            }

            void UpperAltriDati(GestioneAziendeWebOCM.AltriDati altriDati)
            {
                altriDati.CategoriaAttivita = altriDati.CategoriaAttivita?.ToUpper();
                altriDati.TipoAttivita = altriDati.TipoAttivita?.ToUpper();
                altriDati.TipoIscrizione = altriDati.TipoIscrizione?.ToUpper();
                altriDati.AbbonamentoPA = altriDati.AbbonamentoPA?.ToUpper();
                altriDati.AbbonamentoPAiscrizione = altriDati.AbbonamentoPAiscrizione?.ToUpper();
            }

            void UpperSedeLegale(GestioneAziendeWebOCM.SedeLegale sedeLegale)
            {
                sedeLegale.TipoIndirizzo = sedeLegale.TipoIndirizzo?.ToUpper();
                sedeLegale.Indirizzo = sedeLegale.Indirizzo?.ToUpper();
                sedeLegale.Civico = sedeLegale.Civico?.ToUpper();
                sedeLegale.Comune = sedeLegale.Comune?.ToUpper();
                sedeLegale.Provincia = sedeLegale.Provincia?.ToUpper();
                sedeLegale.Localita = sedeLegale.Localita?.ToUpper();
                sedeLegale.StatoEstero = sedeLegale.StatoEstero?.ToUpper();
            }

            void UpperCorrispondenza(GestioneAziendeWebOCM.IndirizzoCorrispondenza indirizzo)
            {
                indirizzo.Destinatario = indirizzo.Destinatario?.ToUpper();
                indirizzo.TipoIndirizzo = indirizzo.TipoIndirizzo?.ToUpper();
                indirizzo.Indirizzo = indirizzo.Indirizzo?.ToUpper();
                indirizzo.Civico = indirizzo.Civico?.ToUpper();
                indirizzo.Comune = indirizzo.Comune?.ToUpper();
                indirizzo.Provincia = indirizzo.Provincia?.ToUpper();
                indirizzo.Localita = indirizzo.Localita?.ToUpper();
                indirizzo.StatoEstero = indirizzo.StatoEstero?.ToUpper();
            }
        }

        public bool IsCodiceFiscalePresent(string codiceFiscale) => _registrazioneDAL.CodiceFiscaleExists(codiceFiscale);

        public bool IsPartitaIvaPresent(string partitaIva) => _registrazioneDAL.PartitaIvaExists(partitaIva);
    }
}