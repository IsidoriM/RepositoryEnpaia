using System.Collections.Generic;
using System.Data;
using IBM.Data.DB2.iSeries;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Amministrativo;
using Utilities;
using static Utilities.DbParameters;

namespace TFI.DAL.Registrazione;

public class BaseRegistrazioneDAL
{
    protected IEnumerable<iDB2Parameter> GetIndirizzoParameters(DataLayer dataLayer,
        GestioneAziendeWebOCM.IndirizzoModel indirizzoModel)
    {
        return new[]
        {
            dataLayer.CreateParameter(CodiceDug, iDB2DbType.iDB2Decimal, 5,
                ParameterDirection.Input, indirizzoModel.SelectedTipoIndirizzo.ToString()),
            dataLayer.CreateParameter(Indirizzo, iDB2DbType.iDB2VarChar,
                80, ParameterDirection.Input, indirizzoModel.Indirizzo),
            dataLayer.CreateParameter(Civico, iDB2DbType.iDB2VarChar, 20,
                ParameterDirection.Input, indirizzoModel.Civico),
            dataLayer.CreateParameter(SiglaProvincia, iDB2DbType.iDB2Char, 2,
                ParameterDirection.Input, indirizzoModel.SelectedProvincia),
            dataLayer.CreateParameter(CodiceComune, iDB2DbType.iDB2Char, 4,
                ParameterDirection.Input, indirizzoModel.SelectedComune),
            dataLayer.CreateParameter(DenominazioneLocalita, iDB2DbType.iDB2VarChar, 100,
                ParameterDirection.Input, indirizzoModel.SelectedLocalita),
            dataLayer.CreateParameter(Cap, iDB2DbType.iDB2VarChar, 9,
                ParameterDirection.Input, indirizzoModel.CAP),
            GetDenominazioneStatoEsteroParam(indirizzoModel.SelectedStatoEstero)
        };

        iDB2Parameter GetDenominazioneStatoEsteroParam(string selectedStatoEstero)
        {
            if (string.IsNullOrWhiteSpace(selectedStatoEstero))
                return new iDB2Parameter()
                {
                    ParameterName = DenominazioneStatoEstero
                };

            var parameter = dataLayer.CreateParameter(CodiceStatoEstero, iDB2DbType.iDB2Char,
                4, ParameterDirection.Input, selectedStatoEstero);

            var denominazioneStatoEstero = dataLayer
                .GetDataTableWithParameters($"SELECT DENCOM FROM CODCOM WHERE CODCOM = {CodiceStatoEstero}",
                    parameter).Rows[0]
                .ElementAt("DENCOM");

            return dataLayer.CreateParameter(DenominazioneStatoEstero, iDB2DbType.iDB2VarChar,
                50, ParameterDirection.Input, denominazioneStatoEstero);
        }
    }
}