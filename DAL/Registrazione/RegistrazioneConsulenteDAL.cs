using System;
using System.Data;
using System.IO;
using System.Linq;
using IBM.Data.DB2.iSeries;
using iTextSharp.text;
using log4net;
using OCM.TFI.OCM.Registrazione;
using OCM.TFI.OCM.Utilities;
using TFI.CRYPTO.Crypto;
using TFI.DAL.ConnectorDB;
using TFI.OCM;
using Utilities;
using static Utilities.DbParameters;

namespace TFI.DAL.Registrazione;

public class RegistrazioneConsulenteDAL : BaseRegistrazioneDAL
{
    private readonly DataLayer _dataLayer;
    private readonly ILog _log;

    public RegistrazioneConsulenteDAL()
    {
        _dataLayer = new DataLayer();
        _log = LogManager.GetLogger("RollingFile");
    }

    public void AvvioTransazione() => _dataLayer.StartTransaction();
    public void CommitTransazione() => _dataLayer.EndTransaction(true);
    public void RollbackTransazione() => _dataLayer.EndTransaction(false);

    public ResultDto PartitaIvaOrCodiceFiscaleExists(string partitaIva, string codiceFiscale)
    {
        const string query = $"SELECT CODTER  FROM ASSTER WHERE CODFIS = {CodiceFiscale} OR PARIVA = {PartitaIva}";
        
        var codiceFiscaleParameter = _dataLayer.CreateParameter(CodiceFiscale, iDB2DbType.iDB2VarChar, 16,
            ParameterDirection.Input, codiceFiscale);
        var partitaIvaParameter = _dataLayer.CreateParameter(PartitaIva, iDB2DbType.iDB2VarChar, 11,
            ParameterDirection.Input, partitaIva);

        var resultAsster = _dataLayer.GetDataTableWithParameters(query, codiceFiscaleParameter, partitaIvaParameter)?.Rows.OfType<DataRow>();
        if (resultAsster != null && resultAsster.Any()) return new ResultDto(true, "Consulente già esistente");
        
        const string queryUtenti = $"SELECT CODFIS FROM UTENTI WHERE CODFIS = {CodiceFiscale}";
        var resultUtenti = _dataLayer.GetDataTableWithParameters(queryUtenti, codiceFiscaleParameter)?.Rows.OfType<DataRow>();
        if (resultUtenti != null && resultUtenti.Any()) return new ResultDto(true, "Codice fiscale già associato ad un'altra utenza");

        const string queryPartitaIva = $"SELECT AW.PARIVA FROM AZIWEB AW WHERE AW.PARIVA = {PartitaIva} UNION SELECT A.PARIVA FROM AZI A WHERE A.PARIVA = {PartitaIva}";
        var resultPartitaIva = _dataLayer.GetDataTableWithParameters(queryPartitaIva, partitaIvaParameter, partitaIvaParameter)?.Rows.OfType<DataRow>();
        if (resultPartitaIva != null && resultPartitaIva.Any()) return new ResultDto(true, "Partita iva già associata ad un'altra utenza");
        
        if (resultAsster == default || resultUtenti == default || resultPartitaIva == default) throw new InvalidDataException("Errore nel recupero dell'esistenza del codice fiscale o partita iva");

        return new ResultDto(false);
    }

    public bool Registra(RegistrazioneConsulenteModel registrazioneConsulenteModel, string password)
    {

        try
        {
            var codFiscParam = _dataLayer.CreateParameter(CodiceFiscale, iDB2DbType.iDB2Char, 20,
                ParameterDirection.Input, registrazioneConsulenteModel.CodiceFiscale);
            
            var identificativoParam = GetIdentificativoParameters(registrazioneConsulenteModel.PartitaIva, registrazioneConsulenteModel.CodiceFiscale);
            
            if (!SaveUtenti(codFiscParam, identificativoParam)) 
                return false;

            if (!SaveUtePin(identificativoParam))
                return false;

            if (!SaveAssTer(registrazioneConsulenteModel, codFiscParam, identificativoParam))
                return false;
            
            return true;
            
            iDB2Parameter GetIdentificativoParameters(string partitaIva, string codiceFiscale)
            {
                if(!string.IsNullOrWhiteSpace(partitaIva))
                    return _dataLayer.CreateParameter(Identificativo, iDB2DbType.iDB2Char, 20,
                        ParameterDirection.Input, partitaIva);
                return _dataLayer.CreateParameter(Identificativo, iDB2DbType.iDB2Char, 20,
                        ParameterDirection.Input, codiceFiscale);
            }

            bool SaveUtenti(iDB2Parameter codiceFiscaleParameter, iDB2Parameter identificativoParam)
            {
                const string queryUtenti = $@"INSERT INTO UTENTI (CODUTE , DENUTE,  CODTIPUTE, CODFIS, ULTAGG , UTEAGG) 
                                 VALUES ({Identificativo}, {DenominazioneUtente}, {CodiceTipoUtente}, {CodiceFiscale}, CURRENT_TIMESTAMP, {Identificativo})";

                var denominazioneUtenteParameter = _dataLayer.CreateParameter(DenominazioneUtente,
                    iDB2DbType.iDB2VarChar,
                    100, ParameterDirection.Input, registrazioneConsulenteModel.Denominazione);
                var codiceTipoUtenteParameter = _dataLayer.CreateParameter(CodiceTipoUtente, iDB2DbType.iDB2Char, 4,
                    ParameterDirection.Input, "C");


                var result = _dataLayer.WriteTransactionDataWithParametersAndDontCall(queryUtenti, CommandType.Text,
                    identificativoParam, denominazioneUtenteParameter, codiceTipoUtenteParameter, codiceFiscaleParameter, identificativoParam);
                return result;
            }

            bool SaveUtePin(iDB2Parameter identificativoParam)
            {
                const string queryUtepin = $@"INSERT INTO UTEPIN (CODUTE, PIN, DATINI, DATFIN, STAPIN, ULTAGG, UTEAGG) 
                                          VALUES ({Identificativo}, {Password}, {DataInizio}, {DataFine}, {StatoPin}, CURRENT_TIMESTAMP, {Identificativo})";

                var passwordParameter = _dataLayer.CreateParameter(Password, iDB2DbType.iDB2Char, 500,
                    ParameterDirection.Input, Cypher.CryptPassword(password));
                var datIniParameter = _dataLayer.CreateParameter(DataInizio, iDB2DbType.iDB2Date, 10,
                    ParameterDirection.Input, DateTime.Now.StandardizeDateString(StandardUse.Internal));
                var dataFineParameter = _dataLayer.CreateParameter(DataFine, iDB2DbType.iDB2Date, 10,
                    ParameterDirection.Input, DateTime.Now.AddDays(90).StandardizeDateString(StandardUse.Internal));
                var staPinParameter =
                    _dataLayer.CreateParameter(StatoPin, iDB2DbType.iDB2Char, 1, ParameterDirection.Input, "A");

                var utePinResult = _dataLayer.WriteTransactionDataWithParametersAndDontCall(queryUtepin,
                    CommandType.Text,
                    identificativoParam, passwordParameter, datIniParameter, dataFineParameter, staPinParameter, identificativoParam);
                return utePinResult;
            }

            bool SaveAssTer(RegistrazioneConsulenteModel model, iDB2Parameter codiceFiscaleParameter, iDB2Parameter identificativoParam)
            {
                const string query =
                    $@"INSERT INTO ASSTER (CODTER, CODNAZ, PARIVA, CODFIS, CAP, TEL1, CELL, EMAIL, EMAILCERT, CODDUG, IND, NUMCIV, SIGPRO, CODCOM, DENLOC, RAGSOC, ULTAGG, UTEAGG) 
                       VALUES ({CodiceTerritoriale}, {CodiceNazionale}, {PartitaIva}, {CodiceFiscale}, {Cap}, {Telefono}, {Cellulare}, {Email}, {Pec}, {CodiceDug}, {Indirizzo}, {Civico},
                               {SiglaProvincia}, {CodiceComune}, {DenominazioneLocalita}, {RagioneSociale}, CURRENT_TIMESTAMP, {Identificativo})";
                
                var codter = _dataLayer.GetDataTable("SELECT MAX(CODTER) + 1 AS NEWCODTER FROM ASSTER").Rows[0].ElementAt("NEWCODTER");

                var codTerParameter =
                    _dataLayer.CreateParameter(CodiceTerritoriale, iDB2DbType.iDB2Char, 5, ParameterDirection.Input, codter);
                var codNazParamenter = _dataLayer.CreateParameter(CodiceNazionale, iDB2DbType.iDB2Decimal, 5,
                    ParameterDirection.Input, model.SelectedAssociazione);
                var partitaIvaParameter = _dataLayer.CreateParameter(PartitaIva, iDB2DbType.iDB2VarChar, 11,
                    ParameterDirection.Input, model.PartitaIva);
                var capParameter = _dataLayer.CreateParameter(Cap, iDB2DbType.iDB2VarChar, 9, ParameterDirection.Input,
                    model.CAP);
                var telefonoParameter = _dataLayer.CreateParameter(Telefono, iDB2DbType.iDB2VarChar, 13,
                    ParameterDirection.Input, model.Telefono);
                var cellulareParameter = _dataLayer.CreateParameter(Cellulare, iDB2DbType.iDB2VarChar, 13,
                    ParameterDirection.Input, model.Cell);
                var emailParameter = _dataLayer.CreateParameter(Email, iDB2DbType.iDB2VarChar, 100,
                    ParameterDirection.Input, model.EmailRegistrazione);
                var pecParameter = _dataLayer.CreateParameter(Pec, iDB2DbType.iDB2VarChar, 100,
                    ParameterDirection.Input, model.Pec);

                var parameters = GetIndirizzoParameters(_dataLayer, model).Append(codTerParameter)
                    .Append(codNazParamenter)
                    .Append(codiceFiscaleParameter)
                    .Append(partitaIvaParameter)
                    .Append(capParameter)
                    .Append(telefonoParameter)
                    .Append(cellulareParameter)
                    .Append(emailParameter)
                    .Append(pecParameter)
                    .Append(identificativoParam).ToArray(); 
                
                return _dataLayer.WriteTransactionDataWithParametersAndDontCall(query, CommandType.Text, parameters);
            }
        }
        catch (Exception e)
        {
            _log.Info(e);
            return false;
        }
    }
}