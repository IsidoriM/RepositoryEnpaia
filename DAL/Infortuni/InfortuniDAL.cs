using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using IBM.Data.DB2.iSeries;
using OCM.TFI.OCM;
using OCM.TFI.OCM.Infortuni;
using TFI.DAL.ConnectorDB;
using Utilities;
using static Utilities.DbParameters;

namespace DAL.Infortuni;

public class InfortuniDAL
{
    private readonly DataLayer _dataLayer;

    public InfortuniDAL()
    {
        _dataLayer = new DataLayer();
    }
    public ResultDtoWithContent<List<IscrittoTrovato>> RicercaIscritto(RicercaIscritto ricercaIscritto)
    {
        try
        {
            var codicePosizioneParameter = _dataLayer.CreateParameter(CodicePosizione, iDB2DbType.iDB2Decimal, 8,
                ParameterDirection.Input, ricercaIscritto.CodicePosizione);

            var queryBuilder = new StringBuilder();
            queryBuilder.Append(
                $@"SELECT DISTINCT ISCRITTO.MAT, ISCRITTO.NOM, ISCRITTO.COG, ISCRITTO.CODFIS, COMUNE.DENCOM FROM ISCT AS ISCRITTO
                       INNER JOIN CODCOM AS COMUNE ON COMUNE.CODCOM = ISCRITTO.CODCOM
                       INNER JOIN RAPLAV AS RAPPORTO ON RAPPORTO.CODPOS = {CodicePosizione} AND ISCRITTO.MAT = RAPPORTO.MAT");

            if (ricercaIscritto.Matricola != default) return RicercaPerMatricola();


            if (string.IsNullOrEmpty(ricercaIscritto.Nome) && string.IsNullOrEmpty(ricercaIscritto.Cognome) &&
                string.IsNullOrEmpty(ricercaIscritto.CodiceFiscale))
                return RicercaTutto();

            return RicercaPerValoriForniti();

            ResultDtoWithContent<List<IscrittoTrovato>> RicercaPerMatricola()
            {
                var matricolaParameter = _dataLayer.CreateParameter(Matricola, iDB2DbType.iDB2Decimal, 9,
                    ParameterDirection.Input, ricercaIscritto.Matricola.Trim());

                queryBuilder.Append($" AND RAPPORTO.MAT = {Matricola}");

                var getMatricolaResult = _dataLayer.GetDataTableWithParameters(queryBuilder.ToString(), matricolaParameter,
                        codicePosizioneParameter);

                var iscrittiTorvati = getMatricolaResult.Rows.OfType<DataRow>().Select(MapToIscrittoTrovato).ToList();

                return new ResultDtoWithContent<List<IscrittoTrovato>>(true, iscrittiTorvati);
            }

            ResultDtoWithContent<List<IscrittoTrovato>> RicercaTutto()
            {
                var allResult = _dataLayer.GetDataTableWithParameters(queryBuilder.ToString(), codicePosizioneParameter);
                var allIscritti = allResult.Rows.OfType<DataRow>().Select(MapToIscrittoTrovato).ToList();
                return new ResultDtoWithContent<List<IscrittoTrovato>>(true, allIscritti);
            }

            ResultDtoWithContent<List<IscrittoTrovato>> RicercaPerValoriForniti()
            {
                var parameters = new List<iDB2Parameter>();
                parameters.Add(codicePosizioneParameter);
                if (!string.IsNullOrEmpty(ricercaIscritto.Nome))
                {
                    queryBuilder.Append($" AND LOWER(ISCRITTO.NOM) = {Nome}");
                    parameters.Add(_dataLayer.CreateParameter(Nome, iDB2DbType.iDB2VarChar, 40, ParameterDirection.Input,
                        ricercaIscritto.Nome?.ToLower()));
                }

                if (!string.IsNullOrEmpty(ricercaIscritto.Cognome))
                {
                    queryBuilder.Append($" AND LOWER(ISCRITTO.COG) = {Cognome}");
                    parameters.Add(_dataLayer.CreateParameter(Cognome, iDB2DbType.iDB2VarChar, 80, ParameterDirection.Input,
                        ricercaIscritto.Cognome?.ToLower()));
                }

                if (!string.IsNullOrEmpty(ricercaIscritto.CodiceFiscale))
                {
                    queryBuilder.Append($" AND LOWER(ISCRITTO.CODFIS) = {CodiceFiscale}");
                    parameters.Add(_dataLayer.CreateParameter(CodiceFiscale, iDB2DbType.iDB2VarChar, 16, ParameterDirection.Input,
                        ricercaIscritto.CodiceFiscale?.ToLower()));
                }

                var result = _dataLayer.GetDataTableWithParameters(queryBuilder.ToString(), parameters.ToArray());
                var iscritti = result.Rows.OfType<DataRow>().Select(MapToIscrittoTrovato).ToList();

                return new ResultDtoWithContent<List<IscrittoTrovato>>(true, iscritti);
            }

            IscrittoTrovato MapToIscrittoTrovato(DataRow row)
                => new()
                {
                    Matricola = row.ElementAt("MAT"),
                    Nome = row.ElementAt("NOM"),
                    Cognome = row.ElementAt("COG"),
                    CodiceFiscale = row.ElementAt("CODFIS")
                };
        }
        catch (Exception ex)
        {
            return new ResultDtoWithContent<List<IscrittoTrovato>>(false, "Errore nel recupero degli iscritti");
        }
    }
}