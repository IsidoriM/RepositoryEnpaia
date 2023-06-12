using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Utente;

namespace TFI.BLL.Utilities
{
    public class UtilsGetFromDB
    {
        public static List<string> GetTipoDocumentoList()
        {
            List<string> listTipoDoc = new List<string>();
            DataTable dataTable = new DataLayer().GetDataTable("SELECT DESCDOC FROM TIPODOC");
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                    listTipoDoc.Add(row["DESCDOC"].ToString());
            }
            return listTipoDoc;
        }

        public static int GetMatricola(Utente utente)
        {
            DataLayer dataLayer = new DataLayer();
            try
            {
                string strSQL = "SELECT DISTINCT MAT FROM RAPLAV WHERE VALUE(CODCAUCES, 0) <> 50 AND MAT IN (SELECT MAT FROM ISCT " + "WHERE CODFIS = '" + (!(utente.Tipo == "E") ? utente.CodFiscale : utente.Username) + "' AND CURRENT_DATE BETWEEN DATISC AND VALUE (DATCHIISC, '9999-12-31'))";
                string str = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
                return !string.IsNullOrEmpty(str) ? Convert.ToInt32(str) : 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string GetIban(string codFiscale)
        {
            string ibanInTblQuery = $"SELECT IBAN, DATINI FROM TBIBAN WHERE MAT = {GetMatricolaIscritto(codFiscale)} ORDER BY DATFIN DESC LIMIT 1";
            return new DataLayer().Get1ValueFromSQL(ibanInTblQuery, CommandType.Text);
        }

        public static string GetMatricolaIscritto(string username) => new DataLayer().Get1ValueFromSQL("SELECT MAT FROM ISCT WHERE CODFIS = '" + username + "'", CommandType.Text);

    }
}
