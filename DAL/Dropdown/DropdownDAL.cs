using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using IBM.Data.DB2.iSeries;
using log4net;
using OCM.TFI.OCM.Registrazione;
using TFI.DAL.ConnectorDB;
using Utilities;
using static Utilities.DbParameters;

namespace Dropdown;

public class DropdownDAL
{
    private readonly DataLayer _dataLayer;
    private readonly ILog _log;

    public DropdownDAL()
    {
        _log = LogManager.GetLogger("RollingFile");
        _dataLayer = new DataLayer();
    }

    public IEnumerable<DropdownModel> GetAssociazioni()
        => GetDefaultDropdownModel("SELECT CODNAZ, RAGSOC FROM ASSNAZ", "CODNAZ", "RAGSOC");

    public IEnumerable<DropdownModel> GetTipiVia()
        => GetDefaultDropdownModel("SELECT CODDUG, DENDUG FROM DUG", "CODDUG", "DENDUG");

    public IEnumerable<DropdownModel> GetProvince()
        => GetDefaultDropdownModel("SELECT SIGPRO, CODREG, DENPRO FROM SIGPRO", "SIGPRO", "DENPRO");

    public IEnumerable<DropdownModel> GetStatiEsteri() =>
        GetDefaultDropdownModel("SELECT CODCOM, DENCOM FROM COM_ESTERO", "CODCOM", "DENCOM");

    public IEnumerable<DropdownModel> GetNatureGiuridiche() =>
        GetDefaultDropdownModel("SELECT NATGIU, DENNATGIU FROM NATGIU", "NATGIU", "DENNATGIU");

    public IEnumerable<DropdownModel> GetGeneri() =>
        GetDefaultDropdownModel("SELECT ID, DESCRIZIONE FROM TBGENERE", "ID", "DESCRIZIONE");

    public IEnumerable<DropdownModel> GetIncarichiRappresentante() =>
        GetDefaultDropdownModel("SELECT CODFUNRAP, DENFUNRAP FROM FUNRAP", "CODFUNRAP", "DENFUNRAP");

    public IEnumerable<DropdownModel> GetCategorieAttivita() =>
        GetDefaultDropdownModel("SELECT CATATTCAM, DENCATATT FROM CATATT", "CATATTCAM", "DENCATATT");

    public IEnumerable<DropdownModel> GetCodiciStatistici() =>
        GetDefaultDropdownModel("SELECT CODSTACON, DENCODSTA FROM CODSTA", "CODSTACON", "DENCODSTA");

    public IEnumerable<DropdownModel> GetComuniFrom(string provincia)
    {
        var provinciaParameter = _dataLayer.CreateParameter(DbParameters.Provincia, iDB2DbType.iDB2Char, 2,
            ParameterDirection.Input, provincia.Trim());

        return GetDefaultDropdownModel($"SELECT CODCOM, DENCOM FROM CODCOM WHERE SIGPRO = {DbParameters.Provincia}",
            "CODCOM",
            "DENCOM", provinciaParameter);
    }

    public IEnumerable<DropdownModel> GetLocalitaFrom(string codiceComune)
    {
        var codiceComuneParameter = _dataLayer.CreateParameter(CodiceComune, iDB2DbType.iDB2Char, 4,
            ParameterDirection.Input, codiceComune.Trim());

        return GetCustomValueDropdownModel($"SELECT DENLOC, CODLOC, CAP FROM CODLOC WHERE CODCOM = {CodiceComune}",
            "CODLOC",
            row => $"{row.ElementAt("DENLOC")} - {row.ElementAt("CAP")}",
            codiceComuneParameter);
    }
    
    public IEnumerable<DropdownModel> GetTipiAttivitaFrom(string categoria)
    {
        var categoriaParameter = _dataLayer.CreateParameter($"{CodiceCategoriaAttivita}", iDB2DbType.iDB2Char, 5,
            ParameterDirection.Input, categoria.Trim());
        return GetDefaultDropdownModel($"SELECT CODATTCAM, DENATTCAM FROM TIPATT WHERE CATATTCAM = {CodiceCategoriaAttivita}",
            "CODATTCAM", "DENATTCAM", categoriaParameter);
    } 
    
    private IEnumerable<DropdownModel> GetDefaultDropdownModel(string query, string key, string value,
        params iDB2Parameter[] parametes)
    {
        try
        {
            return GetRows(query, parametes).Select(row => new DropdownModel
            {
                Key = row.ElementAt(key),
                Value = row.ElementAt(value)
            }).OrderBy(row => row.Key);
        }
        catch (Exception e)
        {
            _log.Info(e);
            return new List<DropdownModel>();
        }
    }
    
    public IEnumerable<DropdownModel> GetAnniCompetenzaNonPrescrittiFrom(string codicePosizione)
    {
        var codicePosizioneParam = _dataLayer.CreateParameter(CodicePosizione, iDB2DbType.iDB2Decimal, 8,
            ParameterDirection.Input, codicePosizione);

        const string query = $"SELECT DISTINCT TRIM(CHAR(ANNDEN)) AS TESTO, TRIM(CHAR(ANNDEN)) AS VALORE FROM DENTES " +
                             $"WHERE CODPOS = {CodicePosizione} AND ANNDEN >= YEAR(current_date) - 5 AND TIPMOV = 'DP'";

        return GetDefaultDropdownModel(query, "TESTO", "VALORE", codicePosizioneParam).OrderByDescending(d => d.Value);
    }

    public IEnumerable<DropdownModel> GetTipiInfortunio() =>
        GetDefaultDropdownModel("SELECT CODINF, DESCRTIPO FROM INFORTUSVI.TBTIPOINF", "CODINF", "DESCRTIPO");
    private IEnumerable<DropdownModel> GetCustomValueDropdownModel(string query, string key,
        Func<DataRow, string> customValueFunc, params iDB2Parameter[] parameters)
    {
        try
        {
            return GetRows(query, parameters).Select(row => new DropdownModel
            {
                Key = row.ElementAt(key),
                Value = customValueFunc.Invoke(row)
            }).OrderBy(row => row.Key);            
        }
        catch (Exception e)
        {
            _log.Info(e);
            return new List<DropdownModel>();
        }
    }
    
    private IEnumerable<DataRow> GetRows(string query, params iDB2Parameter[] parameters)
    {
        return parameters != default
            ? _dataLayer.GetDataTableWithParameters(query, parameters).Rows.OfType<DataRow>()
            : _dataLayer.GetDataTable(query).Rows.OfType<DataRow>();
    }

    
}
