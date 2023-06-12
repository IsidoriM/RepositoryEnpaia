using IBM.Data.DB2.iSeries;
using OCM.Iscritto;
using OCM.TFI.OCM.Iscritto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFI.CRYPTO.Crypto;
using TFI.DAL.ConnectorDB;

namespace Iscritto
{
    public  class RetribuzioneDAL
    {
        private readonly DataLayer objDataAccess = new DataLayer();

        public List<RetribuzioneAnnuale> GetRetribuzioneAnnuale(int matricola)
        {
            string strQuery = $@"SELECT CODPOS AS POSIZIONE
                                , ANNDEN AS ANNO
                                , MAT AS MATRICOLA
                                , PRORAP AS PROGRESSIVO_RDL
                                , SUM(IMPRET) AS RETRIBUZIONE
                                , SUM(IMPOCC) AS OCCASIONALE
                                , SUM(IMPFIG) AS FIGURATIVA
                                , SUM(IMPCON) AS CONTRIBUTO
                                , SUM(IMPASSCON) AS ASSISTENZA_CONTRATTUALE
                                , SUM(IMPABB) AS ABBONAMENTO_PA 
                                FROM (
                                        SELECT DENDET.*
                                        , DENTES.NUMMOV AS NUMMOV_TEST
                                        , DENTES.NUMSAN
                                        , DENTES.NUMSANANN
                                        , CASE DENTES.TIPMOV WHEN 'AR' THEN DENDET.ANNCOM 
                                            ELSE DENDET.ANNDEN END AS ANNO
                                        FROM DENDET INNER JOIN DENTES ON
                                            DENDET.CODPOS = DENTES.CODPOS  
                                            AND DENDET.ANNDEN = DENTES.ANNDEN  
                                            AND DENDET.MESDEN = DENTES.MESDEN
                                            AND DENDET.PRODEN = DENTES.PRODEN
                                        WHERE DENDET.MAT = {matricola}
                                        AND DENTES.ANNDEN >= (YEAR(current_date)-5)
                                        AND VALUE(DENDET.ESIRET, '') <> 'S'
                                        AND DENDET.NUMMOVANN IS NULL
                                    ) AS TABELLA
                                 GROUP BY TABELLA.CODPOS, TABELLA.ANNDEN, TABELLA.MAT, TABELLA.PRORAP
                                 ORDER BY TABELLA. CODPOS, TABELLA.MAT , TABELLA.PRORAP, TABELLA.ANNDEN DESC";
            iDB2DataReader dataReaderFromQuery = objDataAccess.GetDataReaderFromQuery(strQuery, CommandType.Text);
            List<RetribuzioneAnnuale> retribuzioneAnnuales = new List<RetribuzioneAnnuale>();
            while (dataReaderFromQuery.Read())
                retribuzioneAnnuales.Add(new RetribuzioneAnnuale()
                {
                   Abbonamento_Pa = Convert.ToInt32(dataReaderFromQuery["ABBONAMENTO_PA"]),
                   Anno = Convert.ToInt32(dataReaderFromQuery["ANNO"]),
                   Assistenza_Contrattuale = Convert.ToInt32(dataReaderFromQuery["ASSISTENZA_CONTRATTUALE"]),
                   Contributo = Convert.ToInt32(dataReaderFromQuery["CONTRIBUTO"]),
                   Figurativa = Convert.ToInt32(dataReaderFromQuery["FIGURATIVA"]),
                   Matricola = Convert.ToInt32(dataReaderFromQuery["MATRICOLA"]),
                   Occasionale = Convert.ToInt32(dataReaderFromQuery["OCCASIONALE"]),
                   Posizione = Convert.ToInt32(dataReaderFromQuery["POSIZIONE"]),
                   Progressivo_Rdl = Convert.ToInt32(dataReaderFromQuery["PROGRESSIVO_RDL"]),
                   Retribuzione = Convert.ToDecimal(dataReaderFromQuery["RETRIBUZIONE"])
                });
            return retribuzioneAnnuales;
        }

        public List<RetribuzioneMensile> GetRetribuzioneMensile(int posizione, int matricola, int progressivo, int anno)
        {
            string strQuery = $@"SELECT CODPOS AS POSIZIONE
                                , ANNDEN AS ANNO
                                , MESDEN AS MESE
                                , MAT AS MATRICOLA,  
                                   CASE TIPMOV WHEN 'DP' THEN
                                    'DENUNCIA MENSILE' WHEN 'NU' THEN
                                        'NOTIFICHE D''UFFICIO' ELSE
                                        'ARRETRATI' END AS TIPO_MOVIMENTO
                                , DAL
                                , AL
                                , IMPRET AS RETRIBUZIONE
                                , IMPOCC AS OCCASIONALE
                                , IMPFIG AS FIGURATIVA
                                , IMPCON AS CONTRIBUTO
                                , IMPASSCON AS ASSISTENZA_CONTRATTUALE
                                , IMPABB AS ABBONAMENTO_PA
                                FROM(
                                        SELECT DENDET.*
                                        , DENTES.NUMMOV AS NUMMOV_TEST
                                        , DENTES.NUMSAN
                                        , DENTES.NUMSANANN
                                        , CASE DENTES.TIPMOV WHEN 'AR' THEN
                                            DENDET.ANNCOM ELSE
                                            DENDET.ANNDEN END AS ANNO
                                        FROM DENDET INNER JOIN DENTES ON
                                            DENDET.CODPOS = DENTES.CODPOS
                                            AND DENDET.ANNDEN = DENTES.ANNDEN
                                            AND DENDET.MESDEN = DENTES.MESDEN
                                            AND DENDET.PRODEN = DENTES.PRODEN
                                        WHERE DENDET.CODPOS = {posizione} 
                                        AND DENDET.MAT = {matricola}
                                        AND DENDET.PRORAP = {progressivo}
                                        AND VALUE(DENDET.ESIRET, '') <> 'S'
                                        AND DENDET.NUMMOVANN IS NULL
                                    ) AS TABELLA
                                WHERE TABELLA.ANNO = {anno}
                                ORDER BY TABELLA.MESDEN";

            iDB2DataReader dataReaderFromQuery = objDataAccess.GetDataReaderFromQuery(strQuery, CommandType.Text);
            List<RetribuzioneMensile> retribuzioneMensiles = new List<RetribuzioneMensile>();
            while (dataReaderFromQuery.Read())
                retribuzioneMensiles.Add(new RetribuzioneMensile()
                {
                    AbbonamentoPa = Convert.ToInt32(dataReaderFromQuery["ABBONAMENTO_PA"]),
                    Al = Convert.ToDateTime(dataReaderFromQuery["AL"]),
                    Anno = Convert.ToInt32(dataReaderFromQuery["ANNO"]),
                    AssistenzaContrattuale = Convert.ToString(dataReaderFromQuery["ASSISTENZA_CONTRATTUALE"]),
                    Contributo = Convert.ToDecimal(dataReaderFromQuery["CONTRIBUTO"]),
                    Dal = Convert.ToDateTime(dataReaderFromQuery["DAL"]),
                    Figurativa= Convert.ToInt32(dataReaderFromQuery["FIGURATIVA"]),
                    Matricola = Convert.ToInt32(dataReaderFromQuery["MATRICOLA"]),
                    Mese = Convert.ToInt32(dataReaderFromQuery["MESE"]),
                    Occasionale = Convert.ToInt32(dataReaderFromQuery["OCCASIONALE"]),
                    Posizione = Convert.ToInt32(dataReaderFromQuery["POSIZIONE"]),
                    Retribuzione = Convert.ToDecimal(dataReaderFromQuery["RETRIBUZIONE"]),
                    TipoMovimento = Convert.ToString(dataReaderFromQuery["TIPO_MOVIMENTO"]),
                });
            return retribuzioneMensiles;
        }

    }
}
