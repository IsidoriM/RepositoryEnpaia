using IBM.Data.DB2.iSeries;
using System;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;

namespace Utilities
{
    public static class IbanUtils
    {
        public static bool CheckAndInsertIban(string iban, string matricola, DataLayer dataLayer, ref string errorMSG, string cf)
        {
            if (Utile.CheckIban(iban))
            {
                string abiCheck = Utile.CheckAbiAndCab(iban) ? "S" : "N";

                string ibanInTblQuery = $"SELECT IBAN, DATINI FROM TBIBAN WHERE MAT = {matricola} ORDER BY DATFIN DESC LIMIT 1";
                DataTable lastIbanTable = dataLayer.GetDataTable(ibanInTblQuery);

                string insertIbanQuery = "INSERT INTO TBIBAN (MAT, IBAN, DATINI, DATFIN, VALIDO, ULTAGG, UTEAGG) " +
                                         "VALUES (@mat, @iban, CURRENT_DATE, '9999-12-31', @abicab, CURRENT_TIMESTAMP, @user)";
                iDB2Parameter userParam = dataLayer.CreateParameter("@user", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, cf);
                iDB2Parameter matParam = dataLayer.CreateParameter("@mat", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, matricola);
                iDB2Parameter ibanParam = dataLayer.CreateParameter("@iban", iDB2DbType.iDB2VarChar, 40, ParameterDirection.Input, iban);
                iDB2Parameter abicabParam = dataLayer.CreateParameter("@abicab", iDB2DbType.iDB2Char, 1, ParameterDirection.Input, abiCheck);

                if (lastIbanTable.Rows.Count == 0)
                {
                    bool insertIbanResult = dataLayer.WriteTransactionDataWithParametersAndDontCall(insertIbanQuery, CommandType.Text, matParam, ibanParam, abicabParam, userParam);
                    if (!insertIbanResult)
                    {
                        errorMSG = "Errore nell'salvataggio dell'iban";
                        dataLayer.EndTransaction(false);
                        return false;
                    }
                    return true;
                }

                string lastIban = lastIbanTable.Rows[0]["IBAN"].ToString().Trim();
                if (lastIban == iban)
                    return true;

                string updateOldIban = $"UPDATE TBIBAN SET DATFIN = '{DateTime.Today.Date.AddDays(-1).ToString("yyyy-MM-dd")}', ULTAGG = CURRENT_TIMESTAMP, UTEAGG = '{cf}' " +
                    $"WHERE MAT = {matricola} AND IBAN = '{lastIban}' AND DATINI = '{DateTime.Parse(lastIbanTable.Rows[0]["DATINI"].ToString()).ToString("yyyy-MM-dd")}'";
                bool updateIbanResult = dataLayer.WriteTransactionData(updateOldIban, CommandType.Text);
                bool insertIbanResult2 = dataLayer.WriteTransactionDataWithParametersAndDontCall(insertIbanQuery, CommandType.Text, matParam, ibanParam, abicabParam, userParam);
                if (!updateIbanResult || !insertIbanResult2)
                {
                    errorMSG = "Errore nell'salvataggio dell'iban";
                    dataLayer.EndTransaction(false);
                    return false;
                }
                return true;
            }
            dataLayer.EndTransaction(false);
            errorMSG = "Iban non valido";
            return false;
        }
    }
}