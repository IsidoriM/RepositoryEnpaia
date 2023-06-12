using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Iscritto;

namespace Convenzioni
{
    public class ConvenzioniDAL
    {
        public IEnumerable<ListaConvenzioniIscritto> GetConvenzioni()
        {
            DataLayer dataLayer = new DataLayer();
            string Err = "";
            string str = "SELECT TITOLO, PATH, DESCRIZIONE, DATINS, PATHIMG FROM CONVENZIONI WHERE TIPO IN ('I', 'T') AND DATANN IS NULL ORDER BY DATINS DESC";
            DataSet dataSet2 = dataLayer.GetDataSet(str, ref Err);
            dataSet2.Tables[0].Rows[0]["TITOLO"].ToString();
            dataSet2.Tables[0].Rows[0]["PATH"].ToString();
            dataSet2.Tables[0].Rows[0]["DESCRIZIONE"].ToString();
            dataSet2.Tables[0].Rows[0]["DATINS"].ToString();
            dataSet2.Tables[0].Rows[0]["PATHIMG"].ToString();
            List<ListaConvenzioniIscritto> convenzioniIscrittoList = new List<ListaConvenzioniIscritto>();
            iDB2DataReader dataReaderFromQuery = dataLayer.GetDataReaderFromQuery(str, CommandType.Text);
            while (dataReaderFromQuery.Read())
                convenzioniIscrittoList.Add(new ListaConvenzioniIscritto()
                {
                    DataInserimento = dataReaderFromQuery["DATINS"].ToString().Substring(0, 10),
                    Titolo = dataReaderFromQuery["TITOLO"].ToString(),
                    Descrizione = dataReaderFromQuery["DESCRIZIONE"].ToString(),
                    Path = FileExists(string.Format("{0}{1}{2}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["path_convenzioni"], dataReaderFromQuery["PATH"])),
                    PathImg = ImgExists(string.Format("{0}{1}{2}", AppDomain.CurrentDomain.BaseDirectory, "Images/Convenzioni/", dataReaderFromQuery["PATHIMG"]))
                });
            return convenzioniIscrittoList;
        }

        private string FileExists(string path) => !File.Exists(path)
                ? "#"
                : path.Replace(AppDomain.CurrentDomain.BaseDirectory, "../../");
        private string ImgExists(string path) => !File.Exists(path)
                ? null
                : path.Replace(AppDomain.CurrentDomain.BaseDirectory, "../../");
    }
}

