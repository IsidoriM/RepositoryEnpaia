using DAL.Iscritto;
using OCM.TFI.OCM.Iscritto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using TFI.DAL.Iscritto;
using TFI.DAL.Utilities;
using TFI.OCM.Iscritto;
using TFI.OCM.Utente;

namespace TFI.BLL.Iscritto
{
    public class GestionePraticheBLL
    {
        private readonly GestionePraticheDAL _gestionePraticheDAL = new GestionePraticheDAL();

        public List<Pratica> GetPratiche(Utente utente, ref string errorMSG, ref string successMSG)
        {
            var dataRows = _gestionePraticheDAL.GetPratiche(utente, ref errorMSG, ref successMSG);
            List<Pratica> pratiche = new();

            if (dataRows == null)
                return pratiche;

            for (int i = 0; i < dataRows.Count; i++)
            {
                pratiche.Add(MapPratica(dataRows[i], utente.Denominazione));
            }

            return pratiche;
        }

        public bool CheckHasPratiche(Utente utente)
        {
           return _gestionePraticheDAL.CheckHasPratiche(utente);
        }

        public DettaglioPratica GetPraticaById(int idPratica, string denominazione, string username)
        {
            var matricola = Utile.GetMatricolaIscritto(username);
            var result = _gestionePraticheDAL.GetPraticaById(idPratica);
            if (result == default || result["MAT"].ToString().Trim() != matricola.Trim())
                return null;
            return MapDettaglioPratica(result, denominazione);
        }

        private string AlTfrORFineRDL(string altfr, string finerdl)
        {
            return altfr != "" ? altfr : finerdl;
        }

        private Pratica MapPratica(DataRow dataRow, string denominazione)
        {
            return new Pratica
            {
                Id = dataRow["ID"].ToString(),
                Posizione = dataRow["CODPOS"].ToString(),
                Nominativo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(denominazione.ToLower()),
                Ragione_Sociale = dataRow["RAGSOC"].ToString(),
                Tipo_Pratica = dataRow["DENTIPLIQ"].ToString(),
                Stato_Pratica = dataRow["STATO"].ToString(),
                Data_Fine_RDL = Utile.FromStringToDateTimeToFormatString(AlTfrORFineRDL(dataRow["ALTFR"].ToString(), dataRow["DATFINRDL"].ToString()), "dd-MM-yyyy"),
                Data_Inizio_RDL = Utile.FromStringToDateTimeToFormatString(dataRow["DALTFR"].ToString(), "dd-MM-yyyy")
            };
        }

        private DettaglioPratica MapDettaglioPratica(DataRow dataRow, string denominazione)
        {
            return new DettaglioPratica
            {
                Id = dataRow["ID"].ToString(),
                Posizione = dataRow["CODPOS"].ToString(),
                Nominativo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(denominazione.ToLower()),
                Ragione_Sociale = dataRow["RAGSOC"].ToString(),
                Tipo_Pratica = dataRow["DENTIPLIQ"].ToString(),
                Stato_Pratica = dataRow["STATO"].ToString(),
                Data_Fine_RDL = Utile.FromStringToDateTimeToFormatString(AlTfrORFineRDL(dataRow["ALTFR"].ToString(), dataRow["DATFINRDL"].ToString()), "dd-MM-yyyy"),
                Data_Inizio_RDL = Utile.FromStringToDateTimeToFormatString(dataRow["DALTFR"].ToString(), "dd-MM-yyyy"),
                Motivo_Anticipazione = dataRow["DENMOTANT"].ToString(),
                Iban = dataRow["IBAN"].ToString(),
                Swift = dataRow["SWIFT"].ToString(),
                Importo_TFR = string.IsNullOrWhiteSpace(dataRow["IMPTFR"].ToString()) ? "" : dataRow["IMPTFR"].ToString() + " EUR",
                Percentuale_TFR = !string.IsNullOrWhiteSpace(dataRow["PERCTFR"].ToString()) ? "70%" : dataRow["PERCTFR"].ToString(),
                Utente_Conferimento = dataRow["UTECONF"].ToString(),
                Modalità_Pagamento = dataRow["DENMODPAG"].ToString(),
                Utente_Richiesta = dataRow["UTERIC"].ToString(),
                Data_Richiesta = Utile.FromStringToDateTimeToFormatString(dataRow["DTRIC"].ToString(), "dd-MM-yyyy"),
                Data_Conferimento = Utile.FromStringToDateTimeToFormatString(dataRow["DATCONF"].ToString(), "dd-MM-yyyy"),
                Note = new Nota
                {
                    Id = dataRow["IDNOTA"].ToString(),
                    IdPratica = dataRow["IDPRA"].ToString(),
                    ContenutoNota = dataRow["NOTA"].ToString(),
                    DataDiInserimento = Utile.FromStringToDateTimeToFormatString(dataRow["DATAINS"].ToString(), "dd-MM-yyyy"),
                    UtenteInserimento = dataRow["UTEINS"].ToString()
                }
            };
        }
    }
}
