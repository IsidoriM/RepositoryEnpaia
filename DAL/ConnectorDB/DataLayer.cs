// Decompiled with JetBrains decompiler
// Type: TFI.DAL.ConnectorDB.DataLayer
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using log4net;
using Newtonsoft.Json;
using TFI.CRYPTO.Crypto;
using Utilities;


namespace TFI.DAL.ConnectorDB
{
    public class DataLayer
    {
        private string strConn;
        public int intNumRecords;
        public iDB2ProviderSettings objClear;
        public iDB2Connection objConnection = new iDB2Connection();
        public iDB2Transaction objTransaction;
        public iDB2Command objCommand;
        private bool blnTrans;
        private string strTimeStamp;
        private string strLastSQLExecuted;
        private readonly ILog _log;
        public Exception objException;

        public object DBMetods { get; internal set; }

        public bool isInTransaction => this.blnTrans;


        public DataLayer()
        {
            this.strConn = Cypher.DeCryptPassword(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.ToString());
            _log = LogManager.GetLogger("RollingFile");
        }

        public void StartTransaction()
        {
            try
            {
                this.objConnection = new iDB2Connection(this.strConn);
                this.objConnection.Open();
                this.CreateTimestamp();
                this.objTransaction = this.objConnection.BeginTransaction();
                this.blnTrans = true;
                this.objCommand = new iDB2Command();
                this.objCommand.CommandTimeout = 180;
                this.objCommand.Connection = this.objConnection;
                this.objCommand.Transaction = this.objTransaction;
            }
            catch (Exception ex)
            {
                _log.Info(ex);
                this.objException = ex;
            }
        }

        public DataSet GetDataSet(string strSQL, ref string Err)
        {
            DataSet dataSet1 = (DataSet)null;
            iDB2DataAdapter iDb2DataAdapter = (iDB2DataAdapter)null;
            DataSet dataSet2 = new DataSet();
            iDB2Connection connection = (iDB2Connection)null;
            try
            {
                connection = new iDB2Connection(this.strConn);
                connection.Open();
                dataSet1 = new DataSet();
                iDb2DataAdapter = new iDB2DataAdapter(strSQL, connection);
                iDb2DataAdapter.Fill(dataSet1);
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strSQL}. Eccezione: {ex}");
                Err = ex.Message;
                dataSet2?.Dispose();
                this.objException = ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                iDb2DataAdapter?.Dispose();
            }
            return dataSet1;
        }

        private void CreateTimestamp()
        {
            int num = DateTime.Today.Year;
            string str1 = num.ToString();
            num = DateTime.Today.Month;
            string str2 = num.ToString().PadLeft(2, '0');
            string str3 = str1 + "-" + str2 + "-" + DateTime.Today.Day.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + "." + DateTime.Now.Minute.ToString().PadLeft(2, '0') + "." + DateTime.Now.Second.ToString().PadLeft(2, '0') + ".";
            long ticks = DateTime.Now.Ticks;
            string str4 = ticks.ToString();
            ticks = DateTime.Now.Ticks;
            int startIndex = ticks.ToString().Length - 6;
            string str5 = str4.Substring(startIndex, 6);
            this.strTimeStamp = str3 + str5;
        }

        public void EndTransaction(bool blnCommit)
        {
            try
            {
                if (blnCommit)
                    this.objTransaction.Commit();
                else
                {
                    if (objTransaction != null)
                        this.objTransaction.Rollback();
                }

            }
            catch (Exception ex)
            {
                _log.Info(ex);
                this.objException = ex;
            }
            finally
            {
                this.objConnection.Close();
                if (objTransaction != null)
                    this.objTransaction.Dispose();
                this.objTransaction = (iDB2Transaction)null;
                this.blnTrans = false;
                if (this.objCommand != null)
                {
                    this.objCommand.Dispose();
                    this.objCommand = (iDB2Command)null;
                }
            }
        }

        public iDB2DataReader GetDataReaderFromProcedure(
          string strSQLWithoutCALLString,
          iDB2Parameter[] sqlParameters)
        {
            iDB2Command iDb2Command = (iDB2Command)null;
            iDB2Connection connection = (iDB2Connection)null;
            iDB2DataReader readerFromProcedure;
            try
            {
                connection = new iDB2Connection(this.strConn);
                iDb2Command = new iDB2Command("{CALL " + strSQLWithoutCALLString + "}", connection);
                iDb2Command.CommandType = CommandType.StoredProcedure;
                foreach (iDB2Parameter sqlParameter in sqlParameters)
                    iDb2Command.Parameters.Add(sqlParameter);
                connection.Open();
                readerFromProcedure = iDb2Command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strSQLWithoutCALLString}. Eccezione: {ex}");
                readerFromProcedure = (iDB2DataReader)null;
                iDb2Command?.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                this.objException = ex;
            }
            return readerFromProcedure;
        }

        public DataSet GetDataSetFromProcedure(
          string strSQLWithoutCALLString,
          iDB2Parameter[] sqlParameters)
        {
            iDB2DataAdapter iDb2DataAdapter = (iDB2DataAdapter)null;
            DataSet dataSet = (DataSet)null;
            iDB2Command iDb2Command = (iDB2Command)null;
            iDB2Connection connection = (iDB2Connection)null;
            try
            {
                connection = new iDB2Connection(this.strConn);
                iDb2DataAdapter = new iDB2DataAdapter();
                iDb2Command = new iDB2Command("{CALL " + strSQLWithoutCALLString + "}", connection);
                iDb2DataAdapter.SelectCommand = iDb2Command;
                foreach (iDB2Parameter sqlParameter in sqlParameters)
                    iDb2Command.Parameters.Add(sqlParameter);
                dataSet = new DataSet();
                iDb2DataAdapter.Fill(dataSet);
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strSQLWithoutCALLString}. Eccezione: {ex}");
                iDb2Command?.Dispose();
                dataSet?.Dispose();
                dataSet = (DataSet)null;
                this.objException = ex;
            }
            finally
            {
                iDb2DataAdapter?.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return dataSet;
        }

        public iDB2DataReader GetDataReaderFromProcedureOnTrans(
          string strSQLWithoutCALLString,
          iDB2Parameter[] sqlParameters)
        {
            iDB2Command iDb2Command = (iDB2Command)null;
            iDB2DataReader procedureOnTrans;
            try
            {
                iDb2Command = new iDB2Command("{CALL " + strSQLWithoutCALLString + "}", this.objConnection);
                iDb2Command.Transaction = this.objTransaction;
                iDb2Command.CommandType = CommandType.StoredProcedure;
                foreach (iDB2Parameter sqlParameter in sqlParameters)
                    iDb2Command.Parameters.Add(sqlParameter);
                procedureOnTrans = iDb2Command.ExecuteReader();
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strSQLWithoutCALLString}. Eccezione: {ex}");
                procedureOnTrans = (iDB2DataReader)null;
                iDb2Command?.Dispose();
                this.objException = ex;
            }
            return procedureOnTrans;
        }

        public iDB2DataReader GetDataReaderFromQuery(
          string strQuery,
          CommandType intCommandType)
        {
            iDB2Command iDb2Command = (iDB2Command)null;
            iDB2Connection connection = (iDB2Connection)null;
            iDB2DataReader dataReaderFromQuery;
            try
            {
                connection = new iDB2Connection(this.strConn);
                iDb2Command = new iDB2Command(strQuery, connection);
                iDb2Command.CommandType = intCommandType;
                connection.Open();
                dataReaderFromQuery = iDb2Command.ExecuteReader();
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strQuery}. Eccezione: {ex}");
                dataReaderFromQuery = (iDB2DataReader)null;
                iDb2Command?.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                this.objException = ex;
            }
            return dataReaderFromQuery;
        }

        public iDB2DataReader GetDataReaderFromQueryOnTrans(
          string strQuery,
          CommandType intCommandType)
        {
            iDB2Command iDb2Command = (iDB2Command)null;
            iDB2DataReader fromQueryOnTrans;
            try
            {
                iDb2Command = new iDB2Command(strQuery, this.objConnection);
                iDb2Command.CommandType = intCommandType;
                iDb2Command.Transaction = this.objTransaction;
                fromQueryOnTrans = iDb2Command.ExecuteReader();
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strQuery}. Eccezione: {ex}");
                fromQueryOnTrans = (iDB2DataReader)null;
                iDb2Command?.Dispose();
            }
            return fromQueryOnTrans;
        }

        public iDB2Parameter CreateParameter(
          string sName,
          iDB2DbType parameterType,
          int parameterSize,
          ParameterDirection paramDirection,
          string parameterValue)
        {
            iDB2Parameter parameter = new iDB2Parameter(sName, parameterType, parameterSize);
            parameter.Value = (object)parameterValue;
            parameter.Direction = paramDirection;
            return parameter;
        }

        public string Get1ValueFromSQL(string strSQL, CommandType tipoComando)
        {
            string str = "";
            iDB2Connection connection = (iDB2Connection)null;
            iDB2Command iDb2Command = (iDB2Command)null;
            try
            {
                if (!this.blnTrans)
                {
                    connection = new iDB2Connection(this.strConn);
                    iDb2Command = new iDB2Command(strSQL, connection);
                    connection.Open();
                }
                else
                    iDb2Command = new iDB2Command(strSQL, this.objConnection, this.objTransaction);
                iDb2Command.CommandType = tipoComando;
                str = Convert.ToString(iDb2Command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strSQL}. Eccezione: {ex}");
                str = "";
                this.objException = ex;
            }
            finally
            {
                iDb2Command?.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return str;
        }

        public DataTable GetDataTable(string strSQL)
        {
            iDB2Connection connection = (iDB2Connection)null;
            iDB2DataAdapter iDb2DataAdapter = (iDB2DataAdapter)null;
            DataTable dataTable;
            try
            {
                if (!this.blnTrans)
                {
                    connection = new iDB2Connection(this.strConn);
                    iDb2DataAdapter = new iDB2DataAdapter(strSQL, connection);
                }
                else
                {
                    iDb2DataAdapter = new iDB2DataAdapter();
                    iDb2DataAdapter.SelectCommand = new iDB2Command(strSQL, this.objConnection, this.objTransaction);
                }
                this.strLastSQLExecuted = strSQL;
                dataTable = new DataTable();
                iDb2DataAdapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strSQL}. Eccezione: {ex}");
                throw new Exception(strSQL);
            }
            finally
            {
                if (iDb2DataAdapter != null)
                {
                    iDb2DataAdapter.SelectCommand.Dispose();
                    iDb2DataAdapter.Dispose();
                }
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return dataTable;
        }

        public DataTable GetDataTableWithParameters(
          string strSQL,
          params iDB2Parameter[] parameters)
        {
            iDB2Connection connection = (iDB2Connection)null;
            iDB2DataAdapter iDb2DataAdapter = (iDB2DataAdapter)null;
            DataTable dataTable;
            try
            {
                if (!this.blnTrans)
                {
                    connection = new iDB2Connection(this.strConn);
                    iDb2DataAdapter = new iDB2DataAdapter();
                    iDb2DataAdapter.SelectCommand = new iDB2Command(strSQL, connection);
                }
                else
                {
                    iDb2DataAdapter = new iDB2DataAdapter();
                    iDb2DataAdapter.SelectCommand = new iDB2Command(strSQL, this.objConnection, this.objTransaction);
                }
                iDb2DataAdapter.SelectCommand.Parameters.AddRange(parameters);
                this.strLastSQLExecuted = strSQL;
                dataTable = new DataTable();
                iDb2DataAdapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strSQL}.\n         Parametri: {FromParamsToSimpleParamsStringed(parameters)}.\n          Eccezione: {ex}");
                throw new Exception(strSQL);
            }
            finally
            {
                if (iDb2DataAdapter != null)
                {
                    iDb2DataAdapter.SelectCommand.Dispose();
                    iDb2DataAdapter.Dispose();
                }
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return dataTable;
        }

        public (IEnumerable<DataRow> Records, int TotalItemCount) GetPaginatedDataTableAndTotalCountWithParameters(string strSQL, int pageSize, int pageNumber, string countParameter = "*", params iDB2Parameter[] parameters)
        {
            var queryTotali = $"SELECT COUNT({countParameter}) AS TOTALI FROM {RemoveOrderBy(QueryAfterFrom(strSQL.ToUpper()))}";

            var totali = GetDataTableWithParameters(queryTotali, parameters).Rows[0].IntElementAt("TOTALI") ?? 0;

            strSQL += $" OFFSET {(pageNumber - 1) * (pageSize)} ROWS FETCH NEXT {pageSize} ROWS ONLY";
            return (GetDataTableWithParameters(strSQL, parameters).Rows.OfType<DataRow>(), totali);

            string QueryAfterFrom(string query)
            {
                if (query.Contains("*FROM "))
                    return query.Split(new[] { "*FROM " }, StringSplitOptions.None)[1];
                return query.Split(new[] { " FROM " }, StringSplitOptions.None)[1];
            }

            string RemoveOrderBy(string query)
            {
                return query.Split(new[] { " ORDER " }, StringSplitOptions.None)[0];
            }
        }

        public bool WriteDataWithParameters(
          string strSQL,
          CommandType intCmdType,
          params iDB2Parameter[] parameters)
        {
            try
            {
                iDB2Connection connection = new iDB2Connection(this.strConn);
                connection.Open();
                this.objCommand = new iDB2Command(strSQL, connection);
                this.objCommand.CommandType = intCmdType;
                this.objCommand.CommandText = strSQL;
                this.objCommand.Parameters.AddRange(parameters);
                this.intNumRecords = this.objCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strSQL}.\n         Parametri: {FromParamsToSimpleParamsStringed(parameters)}.\n          Eccezione: {ex}");

                this.objException = ex;
                return false;
            }
            return true;
        }

        public bool WriteData(string strSQL, CommandType intCmdType)
        {
            iDB2Connection connection = new iDB2Connection(this.strConn);
            try
            {
                connection.Open();
                this.objCommand = new iDB2Command(strSQL, connection);
                this.objCommand.CommandType = intCmdType;
                this.objCommand.CommandText = strSQL;
                this.intNumRecords = this.objCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strSQL}. Eccezione: {ex}");
                this.objException = ex;
                return false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public bool WriteTransactionData(string strSQL, CommandType intCmdType)
        {
            bool flag = true;
            try
            {
                if (this.blnTrans)
                {
                    iDB2Command iDb2Command = new iDB2Command();
                    iDb2Command.CommandType = intCmdType;
                    iDb2Command.Connection = this.objConnection;
                    iDb2Command.Transaction = this.objTransaction;
                    iDb2Command.CommandText = strSQL;
                    this.objCommand = iDb2Command;
                    this.intNumRecords = this.objCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strSQL}. Eccezione: {ex}");
                flag = false;
                this.objException = ex;
            }
            finally
            {
                iDB2Command objCommand = this.objCommand;
            }
            return flag;
        }

        public bool WriteTransactionDataWithParameters(
          string strSQLWithoutCallString,
          CommandType intCmdType,
          iDB2Parameter[] sqlParam)
        {
            iDB2Command iDb2Command = new iDB2Command();
            bool flag = true;
            try
            {
                if (this.blnTrans)
                {
                    iDb2Command.CommandType = intCmdType;
                    foreach (iDB2Parameter iDb2Parameter in sqlParam)
                        iDb2Command.Parameters.Add(iDb2Parameter);
                    iDb2Command.Connection = this.objConnection;
                    iDb2Command.Transaction = this.objTransaction;
                    iDb2Command.CommandText = "{CALL " + strSQLWithoutCallString + "}";
                    iDb2Command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strSQLWithoutCallString}.\n         Parametri: {FromParamsToSimpleParamsStringed(sqlParam)}.\n          Eccezione: {ex}");
                flag = false;
                this.objException = ex;
            }
            finally
            {
                iDb2Command?.Dispose();
            }
            return flag;
        }

        public bool WriteTransactionDataWithParametersAndDontCall(
          string strSQL,
          CommandType intCmdType,
          params iDB2Parameter[] parameters)
        {
            try
            {
                if (this.blnTrans)
                {
                    iDB2Command iDb2Command = new iDB2Command();
                    iDb2Command.CommandType = intCmdType;
                    iDb2Command.Connection = this.objConnection;
                    iDb2Command.Parameters.AddRange(parameters);
                    iDb2Command.Transaction = this.objTransaction;
                    iDb2Command.CommandText = strSQL;
                    this.objCommand = iDb2Command;
                    this.intNumRecords = this.objCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _log.Info($"Errore nell'esecuzione query in db. Query Eseguita: {strSQL}.\n         Parametri: {FromParamsToSimpleParamsStringed(parameters)}.\n          Eccezione: {ex}");
                this.objException = ex;
                return false;
            }
            return true;
        }

        public bool queryOk(DataSet dS2) => dS2 != null && dS2.Tables.Count > 0 && dS2.Tables[0].Rows.Count > 0;

        public bool queryOk(DataTable dt) => dt != null && dt.Rows.Count > 0;

        public DataView GetDataView(string strSQL)
        {
            iDB2Connection connection = (iDB2Connection)null;
            iDB2DataAdapter iDb2DataAdapter = (iDB2DataAdapter)null;
            try
            {
                if (!this.blnTrans)
                {
                    connection = new iDB2Connection(this.strConn);
                    iDb2DataAdapter = new iDB2DataAdapter(strSQL, connection);
                }
                else
                {
                    iDb2DataAdapter = new iDB2DataAdapter();
                    iDb2DataAdapter.SelectCommand = new iDB2Command(strSQL, this.objConnection, this.objTransaction);
                }
                DataSet dataSet = new DataSet();
                iDb2DataAdapter.Fill(dataSet);
                return new DataView(dataSet.Tables[0], "", "", DataViewRowState.OriginalRows);
            }
            finally
            {
                iDb2DataAdapter?.Dispose();
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public string FromParamsToSimpleParamsStringed(params iDB2Parameter[] parameters)
        => JsonConvert.SerializeObject(parameters.Select(param =>
            new { name = param.ParameterName, value = param.Value, type = param.iDB2DbType.ToString() }).ToList());

    }
}
