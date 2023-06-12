using System;
using IBM.Data.DB2.iSeries;
using OCM.TFI.OCM.Registrazione;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using log4net;
using OCM.Enums;
using OCM.TFI.OCM.Protocollo;
using OCM.TFI.OCM.Utilities;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Amministrativo;
using TFI.OCM.Utilities;
using Utilities;
using static Utilities.DateFormats;
using static Utilities.DbParameters;

namespace TFI.DAL.Registrazione
{
    public class RegistrazioneDAL : BaseRegistrazioneDAL
    {
        private readonly DataLayer _dataLayer = new();
        private static readonly ILog _logger = LogManager.GetLogger("RollingFile");

        public bool SalvaAzienda(
            RegistrazioneAziendaModel nuovaAzienda,
            Protocollo protocollo,
            ref string erroreMsg)
        {
            try
            {
                var codPos = _dataLayer.GetDataTable("SELECT VALUE(MAX(CODICE),0) + 1 AS NEWCODPOS FROM AZIWEB").Rows[0]
                    .ElementAt("NEWCODPOS");

                var codPosParam = _dataLayer.CreateParameter(CodicePosizione, iDB2DbType.iDB2Decimal, 8,
                    ParameterDirection.Input, codPos);
                var utenteAggiornamentoParam = _dataLayer.CreateParameter(UtenteAggiornamento, iDB2DbType.iDB2VarChar, 20,
                    ParameterDirection.Input, FillUtenteAggiornamentoValue());
                
                SalvaAnagraficaAzienda(nuovaAzienda.DatiAzienda, codPosParam, utenteAggiornamentoParam);
                
                SalvaProtocollo(codPosParam, protocollo);

                SalvaSedeLegale(nuovaAzienda.SedeLegale, codPosParam, utenteAggiornamentoParam);

                SalvaCorrispondenza(nuovaAzienda.Corrispondenza, codPosParam, utenteAggiornamentoParam);

                SalvaRappresentanteLegale(nuovaAzienda.RappresentanteLegale, codPosParam, utenteAggiornamentoParam);

                SalvaAltriDati(nuovaAzienda.AltriDati, codPosParam, utenteAggiornamentoParam);

                SalvaCodiceStatistico(nuovaAzienda.AltriDati, codPosParam, utenteAggiornamentoParam);

                nuovaAzienda.DatiAzienda.CodiceAziendaWeb = codPos;
                return true;

                string FillUtenteAggiornamentoValue()
                    => string.IsNullOrEmpty(nuovaAzienda.DatiAzienda.PartitaIva)
                        ? nuovaAzienda.DatiAzienda.CodiceFiscale
                        : nuovaAzienda.DatiAzienda.PartitaIva;
            }
            catch (Exception e)
            {
                erroreMsg = e.Message;
                return false;
            }
        }
        
        private void SalvaProtocollo(iDB2Parameter codicePosizioneParam, Protocollo protocollo)
        {
            var dataProtocolloParam = _dataLayer.CreateParameter(DataProtocollo, iDB2DbType.iDB2Date, 10,
                ParameterDirection.Input, protocollo.DataProtocollo.ToString(DateOnlyDbFormat));
            var numeroProtocolloParam = _dataLayer.CreateParameter(NumeroProtocollo, iDB2DbType.iDB2VarChar, 25,
                ParameterDirection.Input, protocollo.NumeroProtocollo.ToString());
            var protocolloCompletoParam = _dataLayer.CreateParameter(ProtocolloCompleto, iDB2DbType.iDB2VarChar, 50,
                ParameterDirection.Input, protocollo.ProtocolloCompleto);
            
            const string query = $"UPDATE AZIWEB SET DATPROT = {DataProtocollo}, NUMPROT = {NumeroProtocollo}, PROT = {ProtocolloCompleto} WHERE CODICE = {CodicePosizione}";
            
            var result = _dataLayer.WriteTransactionDataWithParametersAndDontCall(query,
                CommandType.Text, dataProtocolloParam, numeroProtocolloParam, protocolloCompletoParam, codicePosizioneParam);
                
            if (!result) throw new DataException("Errore nel salvataggio del protocollo");
        }

        private void SalvaCodiceStatistico(GestioneAziendeWebOCM.AltriDati altriDati,
            iDB2Parameter codPosParam, iDB2Parameter utenteAggiornamentoParam)
        {
            const string insertAzistoSqlCommand =
                $@"INSERT INTO AZISTOWEB (CODICE, PROREC, DATINI, DATFIN, ABB, TIPISC, CODSTACON, ABBDEFMAT, ULTAGG, UTEAGG)
                   VALUES( {CodicePosizione}, 1, CURRENT_DATE, '9999-12-31', 'N', NULL, {CodiceStatistico}, 'S', CURRENT TIMESTAMP, {UtenteAggiornamento})";

            var codStaParam = _dataLayer.CreateParameter(CodiceStatistico, iDB2DbType.iDB2Decimal, 5,
                ParameterDirection.Input, altriDati.CodiceStatistico);

            var result = _dataLayer.WriteTransactionDataWithParametersAndDontCall(
                insertAzistoSqlCommand,
                CommandType.Text, codPosParam, codStaParam, utenteAggiornamentoParam);

            if (!result) throw new DataException("Errore nel salvataggio del codice statistico");
        }

        private void SalvaAltriDati(GestioneAziendeWebOCM.AltriDati altriDati, iDB2Parameter codPosParam,
            iDB2Parameter utenteAggiornamentoParam)
        {
            const string insertAziAttSqlCommand =
                $@"INSERT INTO AZIATTWEB(CODICE, PROREC, DATINI, DATFIN, CATATTCAM, CODATTCAM, ULTAGG, UTEAGG)
                   VALUES({CodicePosizione}, 1, CURRENT DATE, '9999-12-31', {CodiceCategoriaAttivita}, {CodiceTipoAttivita}, CURRENT TIMESTAMP, {UtenteAggiornamento})";

            var categoriaAttivitaParam = _dataLayer.CreateParameter(CodiceCategoriaAttivita,
                iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input,
                altriDati.SelectedCategorieAttivita);
            var tipoAttivitaParam = _dataLayer.CreateParameter(CodiceTipoAttivita, iDB2DbType.iDB2Char, 10,
                ParameterDirection.Input, altriDati.SelectedTipoAttivita);

            var result = _dataLayer.WriteTransactionDataWithParametersAndDontCall(insertAziAttSqlCommand,
                CommandType.Text, codPosParam, categoriaAttivitaParam, tipoAttivitaParam, utenteAggiornamentoParam);

            if (!result) throw new DataException("Errore nel salvataggio di altri dati");
        }

        private void SalvaRappresentanteLegale(GestioneAziendeWebOCM.RapLeg rapLeg,
            iDB2Parameter codPosParam, iDB2Parameter utenteAggiornamentoParam)
        {
            var parameters = GetIndirizzoParameters(_dataLayer, rapLeg).Concat(GetRapLegParameters(rapLeg)).Append(codPosParam)
                .Append(utenteAggiornamentoParam).ToArray();

            const string insertRappresentanteLegaleSqlCommand =
                $@"INSERT INTO AZIRAPWEB (CODICE, PROREC, DATINI, DATFIN, CODFUNRAP, RAPPRI, COG, NOM, CODFIS, DATNAS, CODCOMNAS, SES, CODDUG, IND, NUMCIV, CAP, DENLOC, DENSTAEST, CODCOMRES, SIGPRO, TEL, EMAIL, EMAILCERT, CELL, ULTAGG, UTEAGG)
                   VALUES ( {CodicePosizione}, 1, {DataDecorrenzaRappresentanzaLegale}, '9999-12-31', {FunzioneRappresenteLegale}, 'S', {Cognome}, {Nome}, {CodiceFiscale}, {DataNascita}, {CodiceComuneNascita}, {Sesso}, {CodiceDug}, {Indirizzo}, {Civico}, {Cap}, {DenominazioneLocalita}, {DenominazioneStatoEstero}, {CodiceComune}, {SiglaProvincia}, {Telefono}, {Email}, {Pec}, {Cellulare}, CURRENT TIMESTAMP, {UtenteAggiornamento})";

            var result = _dataLayer.WriteTransactionDataWithParametersAndDontCall(
                insertRappresentanteLegaleSqlCommand,
                CommandType.Text, parameters);

            IEnumerable<iDB2Parameter> GetRapLegParameters(GestioneAziendeWebOCM.RapLeg model)
            {
                return new[]
                {
                    _dataLayer.CreateParameter(DataDecorrenzaRappresentanzaLegale, iDB2DbType.iDB2Date, 10,
                        ParameterDirection.Input, model.DataDecIncDateTime.Date.ToString("s")),
                    _dataLayer.CreateParameter(FunzioneRappresenteLegale, iDB2DbType.iDB2Char, 4,
                        ParameterDirection.Input, model.SelectedIncarico.ToString()),
                    _dataLayer.CreateParameter(Cognome, iDB2DbType.iDB2VarChar, 75,
                        ParameterDirection.Input, model.Cognome),
                    _dataLayer.CreateParameter(Nome, iDB2DbType.iDB2VarChar, 50,
                        ParameterDirection.Input, model.Nome),
                    _dataLayer.CreateParameter(CodiceFiscale, iDB2DbType.iDB2VarChar, 16,
                        ParameterDirection.Input, model.CodiceFiscale),
                    _dataLayer.CreateParameter(DataNascita, iDB2DbType.iDB2Date, 10,
                        ParameterDirection.Input, model.DataNascitaDateTime?.Date.StandardizeDateString(StandardUse.Internal)),
                    _dataLayer.CreateParameter(CodiceComuneNascita, iDB2DbType.iDB2Char, 4,
                        ParameterDirection.Input, model.SelectedComuneDiNascita),
                    _dataLayer.CreateParameter(Sesso, iDB2DbType.iDB2Char, 1,
                        ParameterDirection.Input, model.SelectedGenere?.ToString()),
                    _dataLayer.CreateParameter(Telefono, iDB2DbType.iDB2VarChar, 13,
                        ParameterDirection.Input, model.Telefono1),
                    _dataLayer.CreateParameter(Email, iDB2DbType.iDB2VarChar, 100,
                        ParameterDirection.Input, model.Email),
                    _dataLayer.CreateParameter(Pec, iDB2DbType.iDB2VarChar, 100,
                        ParameterDirection.Input, model.EmailCert),
                    _dataLayer.CreateParameter(Cellulare, iDB2DbType.iDB2VarChar, 13,
                        ParameterDirection.Input, model.Cell)
                };
            }


            if (!result) throw new DataException("Errore nel salvataggio del rappresentante legale");
        }

        private void SalvaCorrispondenza(GestioneAziendeWebOCM.IndirizzoCorrispondenza indirizzoCorrispondenza,
            iDB2Parameter codPosParam,
            iDB2Parameter utenteAggiornamentoParam)
        {
            var insertSedeCorrispondenzaSqlCommand =
                $@"INSERT INTO INDSEDWEB (CODICE, PROREC, TIPIND, DATINI, DATFIN, CODDUG, IND, NUMCIV, CAP, DENLOC, DENSTAEST, CODCOM, SIGPRO, TEL, CELL, ULTAGG, UTEAGG)
                   VALUES ({CodicePosizione}, {ProgressivoRecapito}, {(int) TipoIndirizzoSede.SedeCorrispondenza}, CURRENT DATE, '9999-12-31', {CodiceDug}, {Indirizzo}, {Civico}, {Cap}, {DenominazioneLocalita}, {DenominazioneStatoEstero}, {CodiceComune}, {SiglaProvincia}, {Telefono}, {Cellulare}, CURRENT TIMESTAMP, {UtenteAggiornamento})";

            var parameters = GetIndirizzoParameters(_dataLayer, indirizzoCorrispondenza)
                .Concat(GetCorrispondenzaParameters(indirizzoCorrispondenza)).Append(codPosParam).Append(utenteAggiornamentoParam)
                .ToArray();


            var result = _dataLayer.WriteTransactionDataWithParametersAndDontCall(insertSedeCorrispondenzaSqlCommand,
                CommandType.Text, parameters);

            if (!result) throw new DataException("Errore nel salvataggio dell'indirizzo di corrispondenza");

            IEnumerable<iDB2Parameter> GetCorrispondenzaParameters(GestioneAziendeWebOCM.IndirizzoCorrispondenza model)
            {
                var progressivoRecapitoSedeCorrispondenza = _dataLayer
                    .GetDataTableWithParameters(
                        $"SELECT VALUE(MAX(PROREC),0) + 1 AS NEWPROREC FROM INDSEDWEB WHERE CODICE = {CodicePosizione}",
                        codPosParam).Rows[0]
                    .ElementAt("NEWPROREC");

                return new[]
                {
                    _dataLayer.CreateParameter(ProgressivoRecapito, iDB2DbType.iDB2Decimal, 3,
                        ParameterDirection.Input, progressivoRecapitoSedeCorrispondenza),
                    _dataLayer.CreateParameter($"{Telefono}", iDB2DbType.iDB2VarChar, 13,
                        ParameterDirection.Input, model.Telefono1),
                    _dataLayer.CreateParameter($"{Cellulare}", iDB2DbType.iDB2VarChar, 13,
                        ParameterDirection.Input, model.Cellulare)
                };
            }
        }

        private void SalvaSedeLegale(GestioneAziendeWebOCM.SedeLegale sedeLegale, iDB2Parameter codPosParam,
            iDB2Parameter utenteAggiornamentoParam)
        {
            var parameters = GetIndirizzoParameters(_dataLayer, sedeLegale).Concat(GetSedeLegaleParameters(sedeLegale))
                .Append(codPosParam).Append(utenteAggiornamentoParam).ToArray();

            const string insertSedeLegaleSqlCommand =
                $@"INSERT INTO INDSEDWEB (CODICE, PROREC, TIPIND, DATINI, DATFIN, CODDUG, IND, NUMCIV, CAP, DENLOC, DENSTAEST, CODCOM, SIGPRO, TEL, CELL, ULTAGG, UTEAGG) 
                   VALUES ({CodicePosizione}, {ProgressivoRecapito}, 1, CURRENT DATE, '9999-12-31', {CodiceDug},{Indirizzo}, {Civico}, {Cap}, {DenominazioneLocalita}, {DenominazioneStatoEstero}, {CodiceComune}, {SiglaProvincia}, {Telefono}, {Cellulare}, CURRENT TIMESTAMP, {UtenteAggiornamento})";

            var result = _dataLayer.WriteTransactionDataWithParametersAndDontCall(insertSedeLegaleSqlCommand,
                CommandType.Text, parameters);

            if (!result) throw new DataException("Errore nel salvataggio della sede legale");

            IEnumerable<iDB2Parameter> GetSedeLegaleParameters(GestioneAziendeWebOCM.SedeLegale model)
            {
                var progRecapitoSedeLegale = _dataLayer
                    .GetDataTableWithParameters(
                        $"SELECT VALUE(MAX(PROREC),0) + 1 AS NEWPROREC FROM INDSEDWEB WHERE CODICE = {CodicePosizione}",
                        codPosParam).Rows[0]
                    .ElementAt("NEWPROREC");

                return new[]
                {
                    _dataLayer.CreateParameter(ProgressivoRecapito, iDB2DbType.iDB2Decimal, 3,
                        ParameterDirection.Input, progRecapitoSedeLegale),
                    _dataLayer.CreateParameter(Telefono, iDB2DbType.iDB2VarChar, 13,
                        ParameterDirection.Input, model.Telefono1),
                    _dataLayer.CreateParameter(Cellulare, iDB2DbType.iDB2VarChar, 13,
                        ParameterDirection.Input, model.Cellulare)
                };
            }
        }

        private void SalvaAnagraficaAzienda(GestioneAziendeWebOCM.DatiAzienda datiAzienda,
            iDB2Parameter codPosParameter, iDB2Parameter utenteAggiornamentoParam)
        {
            var ragSoc = _dataLayer.CreateParameter(RagioneSociale, iDB2DbType.iDB2VarChar, 100,
                ParameterDirection.Input, datiAzienda.RagioneSociale);
            var codFisParam = _dataLayer.CreateParameter(CodiceFiscale, iDB2DbType.iDB2VarChar, 16,
                ParameterDirection.Input, datiAzienda.CodiceFiscale);
            var natGiuParam = _dataLayer.CreateParameter(CodiceNaturaGiuridica, iDB2DbType.iDB2Decimal, 5,
                ParameterDirection.Input, datiAzienda.SelectedNaturaGiuridica.ToString());
            var noteParam = _dataLayer.CreateParameter(Note, iDB2DbType.iDB2VarChar, 2000,
                ParameterDirection.Input, datiAzienda.Note);
            var partitaIvaParam = _dataLayer.CreateParameter(PartitaIva, iDB2DbType.iDB2VarChar, 11,
                ParameterDirection.Input, datiAzienda.PartitaIva);


            const string insertAziWebSqlCommand =
                $@"INSERT INTO AZIWEB(CODICE, RAGSOC, PARIVA, CODFIS, NATGIU, DATAPE, DATCOM, DATDENAPE, CODMEZ, NOTE, ULTAGG, UTEAGG) 
                   VALUES( {CodicePosizione}, {RagioneSociale}, {PartitaIva}, {CodiceFiscale}, {CodiceNaturaGiuridica}, CURRENT TIMESTAMP, CURRENT TIMESTAMP, CURRENT TIMESTAMP, 0, {Note}, CURRENT TIMESTAMP, {UtenteAggiornamento})";

            var result = _dataLayer.WriteTransactionDataWithParametersAndDontCall(insertAziWebSqlCommand,
                CommandType.Text, codPosParameter, ragSoc, partitaIvaParam, codFisParam, natGiuParam, noteParam,
                utenteAggiornamentoParam);

            if (!result) throw new DataException("Errore nel salvataggio dell'anagrafica");
            
            const string query = $@"INSERT INTO AZEMAILWEB (CODICE ,EMAIL, EMAILCERT, UTEAGG, ULTAGG, DATINI, DATFIN)
                                    VALUES ({CodicePosizione}, {Email}, {Pec}, {PartitaIva}, CURRENT_TIMESTAMP, {DataInizio}, {DataFine}) ";

            var emailParameter = _dataLayer.CreateParameter(Email, iDB2DbType.iDB2VarChar, 100,
                ParameterDirection.Input, datiAzienda.Email);
            var pecParameter = _dataLayer.CreateParameter(Pec, iDB2DbType.iDB2VarChar, 100,
                ParameterDirection.Input, datiAzienda.Pec);
            var dataInizioParameter = _dataLayer.CreateParameter(DataInizio, iDB2DbType.iDB2Date, 10,
                ParameterDirection.Input, DateTime.Now.StandardizeDateString(StandardUse.Internal));
            var dataFineParameter = _dataLayer.CreateParameter(DataFine, iDB2DbType.iDB2Date, 10, ParameterDirection.Input,
                DateTime.MaxValue.StandardizeDateString(StandardUse.Internal));
            var resultAzemailweb = _dataLayer.WriteTransactionDataWithParametersAndDontCall(query, CommandType.Text, codPosParameter,  emailParameter, pecParameter,
                utenteAggiornamentoParam, dataInizioParameter, dataFineParameter);
            
            if(!resultAzemailweb) throw new DataException("Errore nel salvataggio delle email");
        }

        #region Geografia

        public Comune GetDenominazioneComuneAndSiglaProvinciaFrom(string codiceComune)
        {
            try
            {
                return GetDenominazioneComuneAndSiglaProvinciaFromDb();

                Comune GetDenominazioneComuneAndSiglaProvinciaFromDb()
                {
                    var codiceComuneParameter = _dataLayer.CreateParameter(DbParameters.CodiceComune, iDB2DbType.iDB2Char, 4, ParameterDirection.Input, codiceComune.Trim());
                    var getComuneCompletoFromCodiceComuneQuery = $"SELECT DENCOM, SIGPRO FROM CODCOM WHERE CODCOM = {DbParameters.CodiceComune}";
                    var comuneCompletoFromCodiceComuneResultSetRows = _dataLayer.GetDataTableWithParameters(getComuneCompletoFromCodiceComuneQuery, codiceComuneParameter).Rows[0];

                    return new Comune
                    {
                        Denominazione = comuneCompletoFromCodiceComuneResultSetRows.ElementAt("DENCOM"),
                        SiglaProvincia = comuneCompletoFromCodiceComuneResultSetRows.ElementAt("SIGPRO")
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Info($"[TFI.DAL] : RegistrazioneDAL - E' stata generata un'eccezione in data: {DateTime.Now}. Messaggio: {ex.Message}");
                return new Comune();
            }
        }

        #endregion


        public bool SalvaPraticaDocumenti(RecordRegistrazioneDocumentoAzienda record, ref string errorMsg)
        {
            record.Id = _dataLayer.GetDataTable("SELECT VALUE(MAX(IDDOC),0) + 1 AS NEWIDDOC FROM TBREGDOC").Rows[0]
                                  .IntElementAt("NEWIDDOC") ?? throw new InvalidDataException("Errore nella conversione dell'idDoc");
            try
            {
                var parameters = GetParameters();

                var registraDocumentoCommandSql =
                    $@"INSERT INTO TBREGDOC (IDDOC, IDREG, IDTIPODOC, IDPROTOCOLLO, IDALLEGATO, UUID, ULTAGG, UTEAGG) VALUES
                    ({IdDocumentoFile}, {IdRegistrazione}, {IdTipoDocumentoFile}, {IdProtocollo}, {IdAllegato}, {UUID}, CURRENT TIMESTAMP, {PartitaIva})";

                var result = _dataLayer.WriteTransactionDataWithParametersAndDontCall(registraDocumentoCommandSql,
                    CommandType.Text, parameters);
                if (!result) throw new Exception(_dataLayer.objException.Message);
                return true;
            }
            catch (Exception ex)
            {
                errorMsg +=
                    $" Errore nell'inserimento in tabella relativo al documento {record.IdTipoDocumento}";
                _logger.Info(
                    $"[TFI.DAL] : RegistrazioneDAL - E' stata generata un'eccezione in data: {DateTime.Now}. Messaggio: {ex.Message}");
                return false;
            }

            iDB2Parameter[] GetParameters()
            {
                return new[]
                {
                    _dataLayer.CreateParameter(IdDocumentoFile, iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input,
                        record.Id.ToString()),
                    _dataLayer.CreateParameter(IdProtocollo, iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input,
                        record.IdProtocollo.ToString()),
                    _dataLayer.CreateParameter(IdAllegato, iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input,
                        record.IdAllegato.ToString()),
                    _dataLayer.CreateParameter(IdTipoDocumentoFile, iDB2DbType.iDB2Decimal, 10,
                        ParameterDirection.Input, (record.IdTipoDocumento).ToString()),
                    _dataLayer.CreateParameter(UUID, iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input,
                        record.UUID),
                    _dataLayer.CreateParameter(PartitaIva, iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input,
                        record.PartitvaIva),
                    _dataLayer.CreateParameter(IdRegistrazione, iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input,
                        record.IdReg.ToString())
                };
            }
        }

        public List<Documento> GetDocumentiCaricatiRegistrazioneAzienda(int idProtocollo)
        {
            var idProtocolloParamas = _dataLayer.CreateParameter(IdProtocollo, iDB2DbType.iDB2Decimal, 10,
                ParameterDirection.Input, idProtocollo.ToString());
            
            var recordsDocumenti =
                _dataLayer.GetDataTableWithParameters(
                        $"SELECT IDTIPODOC, IDALLEGATO, UUID FROM TBREGDOC WHERE IDPROTOCOLLO = {IdProtocollo}", idProtocolloParamas).Rows
                    .OfType<DataRow>();

            return recordsDocumenti.Select(record =>
            {
                var isSucceededParse =
                    Enum.TryParse<TipoDocumento>(record.ElementAt("IDTIPODOC"), out var tipoDocumentoEnum);
                    
                if (!isSucceededParse) throw new InvalidDataException("Tipo documento non valido");
                
                return new Documento(tipoDocumentoEnum)
                {
                    Uuid = record.ElementAt("UUID"),
                    IdAllegato = record.IntElementAt("IDALLEGATO")
                };
            }).ToList();
        }

        public void AvviaTransazione()
        {
            _dataLayer.StartTransaction();
        }

        public void RollbackTransazione()
        {
            _dataLayer.EndTransaction(false);
        }

        public void CommitTransazione()
        {
            _dataLayer.EndTransaction(true);
        }

        public DettaglioPraticaRegistrazioneAzienda GetDettaglioPratica(string identificativoAzienda, string numProtocollo, ref string errorMsg)
        {
            try
            {
                var praticaRegistrazioneLight = GetPraticaRegistrazioneLight(numProtocollo, identificativoAzienda, ref errorMsg);
                var documentiCaricati = GetDocumentiCaricatiRegistrazioneAzienda(praticaRegistrazioneLight.Protocollo.IdProtocollo);

                return new DettaglioPraticaRegistrazioneAzienda
                {
                    DocumentiCaricati = documentiCaricati,
                    Protocollo = praticaRegistrazioneLight.Protocollo,
                    PartitaIva = identificativoAzienda,
                    CodicePosizione = praticaRegistrazioneLight.CodicePosizione,
                    StatoRegistrazione = praticaRegistrazioneLight.StatoRegistrazione
                };
            }
            catch (Exception ex)
            {
                _logger.Info(
                    $"[TFI.DAL] : RegistrazioneDAL - E' stata generata un'eccezione in data: {DateTime.Now}. Messaggio: {ex.Message}");
                return null;
            }

            PraticaRegistrazioneLight GetPraticaRegistrazioneLight(string numeroProtocollo, string identificativoAzienda, ref string errorMsg)
            {
                var numProtocolloParameters = _dataLayer.CreateParameter(NumeroProtocollo, iDB2DbType.iDB2VarChar, 25,
                    ParameterDirection.Input, numeroProtocollo);
                var partitaIvaParameters = _dataLayer.CreateParameter(PartitaIva, iDB2DbType.iDB2VarChar, 11,
                    ParameterDirection.Input,
                    identificativoAzienda);

                var result = _dataLayer.GetDataTableWithParameters(
                    $@"SELECT PROT, DATPROT, CODICE, IDSTATO FROM AZIWEB WHERE NUMPROT = {NumeroProtocollo} AND PARIVA = {PartitaIva} AND IDSTATO <> {(int)StatoRegistrazioneAzienda.Approvata} AND IDSTATO <> {(int)StatoRegistrazioneAzienda.Respinta}",
                    numProtocolloParameters, partitaIvaParameters);
                if (result.Rows.Count != 0)
                    return ParseDataProtocolloAndReturnPraticaRegistrazioneLight();

                var codiceFiscaleParameter = _dataLayer.CreateParameter(CodiceFiscale, iDB2DbType.iDB2VarChar, 16,
                    ParameterDirection.Input, identificativoAzienda);
                result = _dataLayer.GetDataTableWithParameters(
                    $@"SELECT PROT, DATPROT, CODICE, IDSTATO FROM AZIWEB WHERE NUMPROT = {NumeroProtocollo} AND CODFIS = {CodiceFiscale} AND IDSTATO <> {(int)StatoRegistrazioneAzienda.Approvata} AND IDSTATO <> {(int)StatoRegistrazioneAzienda.Respinta}",
                    numProtocolloParameters, codiceFiscaleParameter);

                if (result.Rows == null || result.Rows.Count == 0)
                {
                    errorMsg = "Non ci sono pratiche di registrazione attive associate a questa partita iva o a questo numero di protocollo.";
                    throw new DataException($"Non ci sono pratiche associate a questa partita iva o codice fiscale - {identificativoAzienda} o a questo numero di protocollo - {numeroProtocollo}.");
                }

                return ParseDataProtocolloAndReturnPraticaRegistrazioneLight();

                PraticaRegistrazioneLight ParseDataProtocolloAndReturnPraticaRegistrazioneLight()
                {
                    var isSucceededParse = int.TryParse(result.Rows[0].ElementAt("PROT").Split(';')[0], out var idProtocollo);
                    var dataProtocollo = result.Rows[0].GetRawDateCurrentCultureElementAt("DATPROT").Value;
                    var praticaRegistrazioneLight = new PraticaRegistrazioneLight()
                    {
                        Protocollo = new ProtocolloDetail
                        {
                            DataProtocollo = dataProtocollo,
                            NumeroProtocollo = numeroProtocollo,
                            IdProtocollo = isSucceededParse ? idProtocollo : throw new InvalidDataException("id protocollo mancante")
                        },
                        CodicePosizione = result.Rows[0].ElementAt("CODICE"),
                        StatoRegistrazione = (StatoRegistrazioneAzienda) result.Rows[0].IntElementAt("IDSTATO")
                    };
                    return praticaRegistrazioneLight;
                }
            }
        }

        public bool AggiornaStatoPraticaRegistrazioneAzienda(string codicePosizione, StatoRegistrazioneAzienda statoRegistrazione, ref string errorMsg)
        {
            var codicePosizioneParameter = _dataLayer.CreateParameter(CodicePosizione, iDB2DbType.iDB2Decimal, 8,
                ParameterDirection.Input,
                codicePosizione);
            var result = _dataLayer.WriteTransactionDataWithParametersAndDontCall(
                $"UPDATE AZIWEB SET IDSTATO={(int)statoRegistrazione} WHERE CODICE={CodicePosizione}", CommandType.Text, codicePosizioneParameter);
                
            if (result) return true;
            
            errorMsg += "Errore nell'aggiornamento di stato della pratica";
            _logger.Info(
                $"[TFI.DAL] : RegistrazioneDAL - Update stato registrazione azienda fallito. E' stata generata un'eccezione in data: {DateTime.Now}. Messaggio: {_dataLayer.objException.Message}");
            
            return false;
        }

        public bool PartitaIvaExists(string partitaIva)
        {
            var partitaIvaParameter = _dataLayer.CreateParameter(PartitaIva, iDB2DbType.iDB2VarChar, 11,
                ParameterDirection.Input, partitaIva);
            
            var isPartitaIvaPresentInAziWebResult = _dataLayer.GetDataTableWithParameters(
                $"SELECT * FROM AZIWEB WHERE PARIVA = {PartitaIva}", partitaIvaParameter).Rows.OfType<DataRow>();
            if(isPartitaIvaPresentInAziWebResult.Any())
                return true;

            var isPartitaIvaPresentInAziResult = _dataLayer.GetDataTableWithParameters(
                $"SELECT * FROM AZI WHERE PARIVA = {PartitaIva}", partitaIvaParameter).Rows.OfType<DataRow>();
            if (isPartitaIvaPresentInAziResult.Any())
                return true;

            return false;
        }

        public bool CodiceFiscaleExists(string codiceFiscale)
        {
            var codiceFiscaleParameter = _dataLayer.CreateParameter(CodiceFiscale, iDB2DbType.iDB2VarChar, 16,
                ParameterDirection.Input, codiceFiscale);

            var isCodiceFiscalePresentInAziWebResult = _dataLayer.GetDataTableWithParameters(
                $"SELECT * FROM AZIWEB WHERE CODFIS = {CodiceFiscale}", codiceFiscaleParameter).Rows.OfType<DataRow>();
            if (isCodiceFiscalePresentInAziWebResult.Any())
                return true;

            var isCodiceFiscalePresentInAziResult = _dataLayer.GetDataTableWithParameters(
                $"SELECT * FROM AZI WHERE CODFIS = {CodiceFiscale}", codiceFiscaleParameter).Rows.OfType<DataRow>();
            if(isCodiceFiscalePresentInAziResult.Any())
                return true;

            return false;
        }
    }
}