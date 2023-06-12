using OCM.TFI.OCM.Iscritto;
using System;
using System.Data;
using System.Web;
using TFI.CRYPTO.Crypto;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Iscritto;
using TFI.OCM.Utente;

namespace TFI.DAL.Iscritto
{
    public class GestionePraticheDAL
    {
        private readonly DataLayer objDataAccess = new DataLayer();

        public DataRowCollection GetPratiche(Utente utente, ref string errorMSG, ref string successMSG)
        {
            try
            {
                DataTable result = GetPraticheTable(utente);
                if (result.Rows.Count == 0)
                    errorMSG = "Non ci sono pratiche attive";
                return result.Rows;
            }
            catch
            {
                errorMSG = "Errore nel salvataggio";
                return null;
            }
        }

        public bool CheckHasPratiche(Utente utente)
        {
            DataTable result = GetPraticheTable(utente);
            if (result.Rows.Count == 0)
                return false;
            return true;
        }

        private DataTable GetPraticheTable(Utente utente)
        {
            var getMatricolaQuery = "SELECT i.MAT FROM UTENTI u INNER JOIN ISCT i ON u.CODFIS = i.CODFIS WHERE u.CODTIPUTE = 'I' AND u.CODUTE = '" + utente.Username + "'";
            var matricola = objDataAccess.Get1ValueFromSQL(getMatricolaQuery, CommandType.Text);
            var TBRICLIQQuery = $"SELECT P.ID, P.CODPOS, A.RAGSOC, T.DENTIPLIQ, S.STATO, P.ALTFR, P.DALTFR, P.DATFINRDL FROM " +
                $"TBRICLIQ AS P LEFT JOIN AZI AS A ON P.CODPOS = A.CODPOS " +
                $"JOIN STAPRA AS S ON P.CODSTAPRA = S.CODSTATO " +
                $"JOIN TIPLIQ AS T ON P.TIPLIQ = T.TIPLIQ WHERE MAT = {matricola}";
            var result = objDataAccess.GetDataTable(TBRICLIQQuery);
            return result;
        }

        public DataRow GetPraticaById(int idPratica)
        {
            string praticaSqlQuery = "SELECT * FROM TBRICLIQ AS P LEFT JOIN AZI AS A ON P.CODPOS = A.CODPOS " +
                $"LEFT JOIN STAPRA AS S ON P.CODSTAPRA = S.CODSTATO " +
                $"LEFT JOIN TIPLIQ AS T ON P.TIPLIQ = T.TIPLIQ " +
                $"LEFT JOIN TBRICLIQNOT AS N ON P.ID = N.IDPRA " +
                $"LEFT JOIN MODOPAG as M ON P.MODPAG = M.MODPAG " +
                $"LEFT JOIN MOTANTLIQ as MOT ON MOT.CODMOTANT = P.CODMOTANT " +
                $"WHERE ID = {idPratica}";
            var result = objDataAccess.GetDataTable(praticaSqlQuery);
            if(result.Rows.Count > 0)
                return result.Rows[0];
            return null;
        }
    }
}
