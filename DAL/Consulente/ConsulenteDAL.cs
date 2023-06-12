using System;
using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2.iSeries;
using log4net;
using OCM.TFI.OCM.AziendaConsulente;
using System.Linq;
using TFI.DAL.ConnectorDB;
using TFI.OCM.AziendaConsulente;
using Utilities;
using static Utilities.DbParameters;

namespace TFI.DAL.Consulente
{
    public class ConsulenteDAL
    {
        private readonly DataLayer _dataLayer = new DataLayer();
        private static readonly ILog _log = LogManager.GetLogger("RollingFile");

        public List<AziendaLight> GetAziendeConDelegaAttivaOrInAttesa(string codTer)
        {
            var codTerParameter = _dataLayer.CreateParameter(DbParameters.CodiceTerritoriale, iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, codTer);
            var selectAziendeConDelegaAttivaSqlQuery =
                      $"SELECT d.*, az.RAGSOC FROM DELEGHE d INNER JOIN AZI az ON az.CODPOS = d.CODPOS " +
                      $"WHERE d.CODTER = {DbParameters.CodiceTerritoriale} AND (d.FLGATT = 1 OR d.FLGATT IS NULL)";
            var aziendeConDelegaAttiva = _dataLayer.GetDataTableWithParameters(selectAziendeConDelegaAttivaSqlQuery, codTerParameter).Rows.OfType<DataRow>();

            return aziendeConDelegaAttiva.Select(aziendaFromDb => new AziendaLight
            {
                CodiceIdentificativo = aziendaFromDb.ElementAt("CODPOS"),
                RagioneSociale = aziendaFromDb.ElementAt("RAGSOC"),
                IsDelegaConfermata = !string.IsNullOrEmpty(aziendaFromDb.ElementAt("FLGATT")),
            }).ToList();
        }

        public AziendaLight GetDatiAzienda(string identificativo)
        {
            var codiceFiscaleParameter = _dataLayer.CreateParameter(PartitaIva, iDB2DbType.iDB2Char, 16, ParameterDirection.Input, identificativo);
            var partitaIvaParameter = _dataLayer.CreateParameter(CodiceFiscale, iDB2DbType.iDB2Char, 11, ParameterDirection.Input, identificativo);
            var codicePosizioneParameter = _dataLayer.CreateParameter(CodiceFiscale, iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, identificativo.Length <= 8 && int.TryParse(identificativo, out var codPos) ? codPos.ToString() : null);
            const string strSQL = "SELECT A.CODPOS, PARIVA, RAGSOC, AZ.EMAIL FROM AZI A LEFT JOIN AZEMAIL AZ ON A.CODPOS = AZ.CODPOS " +
                                  $"WHERE PARIVA = {PartitaIva} OR CODFIS = {CodiceFiscale} OR A.CODPOS = {CodicePosizione} ORDER BY DATINI DESC LIMIT 1";
            var aziende = _dataLayer.GetDataTableWithParameters(strSQL, partitaIvaParameter, codiceFiscaleParameter, codicePosizioneParameter).Rows;
            if (aziende.Count > 0)
                return new AziendaLight()
                {
                    CodiceIdentificativo = aziende[0].ElementAt("CODPOS"),
                    RagioneSociale = aziende[0].ElementAt("RAGSOC"),
                    Email = aziende[0].ElementAt("EMAIL"),
                    PartitaIva = aziende[0].ElementAt("PARIVA")
                };
            _log.Info($"[ConsulenteDAL] - Non è stata trovata nessuna azienda che corrisponda a questo identificativo. Data: {DateTime.Now}. Messaggio: {_dataLayer.objException?.Message}. StackTrace: {_dataLayer.objException?.StackTrace}");
            return default;
        }

        public bool RichiediDelega(string codicePosizione, string codTer)
        {
            var codicePosizioneParameter = _dataLayer.CreateParameter(CodicePosizione, iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codicePosizione);
            var codiceTerritorialeParameter = _dataLayer.CreateParameter(CodiceTerritoriale, iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, codTer);
            const string strSQL = "INSERT INTO DELEGHE (CODPOS, CODTER, DATINI, ULTAGG, UTEAGG) " +
                                  $"VALUES ({CodicePosizione}, {CodiceTerritoriale}, CURRENT_DATE, CURRENT_TIMESTAMP, {CodiceTerritoriale})";
            var result = _dataLayer.WriteDataWithParameters(strSQL, CommandType.Text, codicePosizioneParameter, codiceTerritorialeParameter, codiceTerritorialeParameter);
            if (!result)
                _log.Info($"[ConsulenteDAL] - Richiesta Delega - Errore nel salvataggio della richiesta. Data: {DateTime.Now}. Messaggio: {_dataLayer.objException?.Message}. StackTrace: {_dataLayer.objException?.StackTrace}");
            return result;
        }

        public AziendaConsulenteLight GetDatiConsulente(string codTer)
        {
            var codiceTerritorialeParameter = _dataLayer.CreateParameter(CodiceTerritoriale, iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, codTer);
            const string strSQL = $"SELECT CODTER, RAGSOC, EMAIL FROM ASSTER WHERE CODTER = {CodiceTerritoriale}";
            var consulente = _dataLayer.GetDataTableWithParameters(strSQL, codiceTerritorialeParameter).Rows;

            if (consulente.Count > 0)
                return new AziendaConsulenteLight()
                {
                    CodiceIdentificativo = consulente[0].ElementAt("CODTER"),
                    RagioneSociale = consulente[0].ElementAt("RAGSOC"),
                    Email = consulente[0].ElementAt("EMAIL")
                };
            _log.Info($"[ConsulenteDAL] - Non è stata trovato nessun consulente che corrisponda a questo identificativo. Data: {DateTime.Now}. Messaggio: {_dataLayer.objException?.Message}. StackTrace: {_dataLayer.objException?.StackTrace}");
            return default;
        }

        public bool CheckDelegaAttivaOrInAttesa(string codPos, string codTer)
        {
            var codicePosizioneParameter = _dataLayer.CreateParameter(CodicePosizione, iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
            var codiceTerritorialeParameter = _dataLayer.CreateParameter(CodiceTerritoriale, iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, codTer);
            const string delegaAttivaOrInAttesaQuery = $"SELECT CODPOS FROM DELEGHE WHERE CODPOS = {CodicePosizione} AND CODTER = {CodiceTerritoriale} AND (FLGATT = 1 OR FLGATT IS NULL)";
            var table = _dataLayer.GetDataTableWithParameters(delegaAttivaOrInAttesaQuery, codicePosizioneParameter, codiceTerritorialeParameter);
            if (table.Rows.Count > 0)
                _log.Info($"[ConsulenteDAL] - Richiesta Delega - Errore nel salvataggio della richiesta, " +
                          $"esiste già una delega attiva o in attesa per questo codice territoriale {codTer} e questa posizione {codPos}." +
                          $" Data: {DateTime.Now}. Messaggio: {_dataLayer.objException?.Message}. StackTrace: {_dataLayer.objException?.StackTrace}");
            return table.Rows.Count > 0;
        }

        public bool CheckDelegaAttiva(string codicePosizione)
        {
            var codicePosizioneParameter = _dataLayer.CreateParameter(CodicePosizione, iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codicePosizione);
            const string delegaAttivaQuery = $"SELECT CODPOS FROM DELEGHE WHERE CODPOS = {CodicePosizione} AND FLGATT = 1";
            var delegaAttivaOrQueryResult = _dataLayer.GetDataTableWithParameters(delegaAttivaQuery, codicePosizioneParameter).Rows.OfType<DataRow>();

            return delegaAttivaOrQueryResult.Any();
        }
    }
}
