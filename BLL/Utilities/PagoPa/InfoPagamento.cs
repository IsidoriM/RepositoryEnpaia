using System;
using System.Data;

namespace TFI.BLL.Utilities.PagoPa
{
    public class InfoPagamento
    {
        public string IdServizio { get; }
        public string CodiceDebito { get; }
        public decimal Importo { get; }
        public string NumMov { get; }
        public string CodeLine { get; }
        public DateTime DataScadenza { get; }
        public string TipMov { get; }
        public string Causale { get; }
        public DateTime DataRimozione { get; }
        private InfoPagamento(string codiceDebito, decimal importo, string numMov, string codeLine, 
            DateTime dataScadenza, string tipMov, string causale, DateTime dataRimozione)
        {
            CodiceDebito = codiceDebito;
            Importo = importo;
            NumMov = numMov;
            CodeLine = codeLine;
            DataScadenza = dataScadenza;
            TipMov = tipMov;
            Causale = causale;
            DataRimozione = dataRimozione;
            IdServizio = "46";
        }

        public static InfoPagamento? Create(DataTable objDt, string tipoDenuncia, string posizione, int anno, int mese, int progressivo)
        {
            decimal IMPDIS;
            decimal IMPDIFF;
            string numMov = string.Empty;
            IMPDIS = 0;
            if (objDt.Rows.Count > 0)
            {
                if (objDt.Rows[0]["ESIRET"].ToString() == "S")
                {
                    IMPDIS = decimal.Parse(objDt.Rows[0]["IMPCON"].ToString());
                    IMPDIS += decimal.Parse(objDt.Rows[0]["IMPADDREC"].ToString());
                    IMPDIS += decimal.Parse(objDt.Rows[0]["IMPABB"].ToString()) + decimal.Parse(objDt.Rows[0]["IMPABBDEL"].ToString());
                    IMPDIS += decimal.Parse(objDt.Rows[0]["IMPASSCON"].ToString()) + decimal.Parse(objDt.Rows[0]["IMPASSCONDEL"].ToString());
                }
                else
                    IMPDIS = decimal.Parse(objDt.Rows[0]["IMPCON"].ToString()) + decimal.Parse(objDt.Rows[0]["IMPADDREC"].ToString())
                        + decimal.Parse(objDt.Rows[0]["IMPASSCON"].ToString()) + decimal.Parse(objDt.Rows[0]["IMPABB"].ToString());
                IMPDIS = IMPDIS - decimal.Parse(objDt.Rows[0]["IMPDEC"].ToString());
                if (objDt.Rows[0]["NUMSANANN"].ToString().Trim() == "" & objDt.Rows[0]["NUMSAN"].ToString().Trim() != "")
                    IMPDIS += decimal.Parse(objDt.Rows[0]["IMPSANDET"].ToString());
                numMov = objDt.Rows[0]["NUMMOV"].ToString();
                DateTime dataScadenza = DateTime.Parse(objDt.Rows[0]["DATSCA"].ToString());
                string causale = ImpostaCausale(objDt.Rows[0]["TIPMOV"].ToString().Trim(), anno, mese);
                var numMovPlitted = numMov.Split('/');
                InfoPagamento infoPagamento = new InfoPagamento(
                    tipoDenuncia + " - POSIZIONE: " + posizione + " ANNO:" + anno + " MESE: " + mese + " PRODEN: " + progressivo,
                    IMPDIS,
                    numMov,
                    posizione.PadLeft(6, '0') + numMovPlitted[1].Trim().Substring(2, 2) + numMovPlitted[0].Trim() + numMovPlitted[2].Trim().PadLeft(3, '0'),
                    dataScadenza,
                    objDt.Rows[0]["TIPMOV"].ToString().Trim(),
                    causale,
                    dataScadenza.AddYears(20)
                    );
                return infoPagamento;
            }
            else { return null; }
        }

        public static InfoPagamento? CreateDiffida(DataTable objDt, string tipoDenuncia, string posizione, int anno, int mese, int progressivo)
        {
            if (objDt.Rows.Count > 0)
            {
                string CODELINE = string.Empty;
                CODELINE = posizione + objDt.Rows[0]["mesediff"].ToString().Trim().PadLeft(2, '0') + objDt.Rows[0]["annodiff"].ToString() + "D01";
                CODELINE = CODELINE.PadLeft(15, '0');
                DateTime dataScadenza = DateTime.Parse("31/01/" + objDt.Rows[0]["annodiff"] + 1);
                InfoPagamento infoPagamento = new InfoPagamento(
                    tipoDenuncia + " - POSIZIONE: " + posizione + " ANNO:" + anno + " MESE: " + mese,
                    decimal.Parse(objDt.Rows[0]["impdiff"].ToString()),
                    string.Empty,
                    CODELINE,
                    dataScadenza,
                    string.Empty,
                    "Saldo diffide " + anno + " " + objDt.Rows[0]["impdiff"].ToString().Replace(",", "."),
                    dataScadenza.AddYears(20));
                return infoPagamento;
            }
            else { return null; }
        }

        private static string ImpostaCausale(string tipMov, int anno, int mese)
        {
            var mesi = Utils.GetMesi();
            
            if (tipMov == "AR")
                return "PAGAMENTO ARRETRATO MESE DI " + mesi[mese].ToUpper() + " " + anno;
            else
                return "PAGAMENTO AUTODENUNCIA MESE DI " + mesi[mese].ToUpper() + " " + anno;
        }
    }
}
