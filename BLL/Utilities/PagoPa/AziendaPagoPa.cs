using System;
using System.Data;

namespace TFI.BLL.Utilities.PagoPa
{
    public class AziendaPagoPa
    {
        public string RagioneSociale { get; }
        public string PIVA { get; }
        public string Indirizzo { get; }
        public string Cap { get; }
        public string Localita { get; }
        public string Prov { get; }
        public string Mail { get; }
        public string Nazione { get; }
        public string Civ { get; }

        private AziendaPagoPa(string ragioneSociale, string pIVA, string indirizzo, string cap, string localita, string prov, string mail, string nazione, string civ)
        {
            RagioneSociale = ragioneSociale;
            PIVA = pIVA;
            Indirizzo = indirizzo;
            Cap = cap;
            Localita = localita;
            Prov = prov;
            Mail = mail;
            Nazione = nazione;
            Civ = civ;
        }

        public static AziendaPagoPa? Create(DataTable? azienda, DataTable? aziendaAltriDati)
        {
            string pIva = string.Empty;
            string indirizzo = string.Empty;
            string nazione = string.Empty;
            if (azienda != null && azienda.Rows.Count > 0)
            {
                pIva = azienda.Rows[0]["CODFIS"].ToString().Trim() != string.Empty ? azienda.Rows[0]["CODFIS"].ToString().Trim().ToUpper()
                        : azienda.Rows[0]["PARIVA"].ToString().Trim().ToUpper();
            }
            else
            {
                return null;
            }
            if (aziendaAltriDati != null && aziendaAltriDati.Rows.Count > 0)
            {
                indirizzo = aziendaAltriDati.Rows[0]["DENDUG"].ToString().Trim() + " " + aziendaAltriDati.Rows[0]["IND"].ToString().Trim().Replace("'", "") + " " + aziendaAltriDati.Rows[0]["NUMCIV"].ToString().Trim();
                indirizzo = indirizzo.ToUpper();
                if (aziendaAltriDati.Rows[0]["DENSTAEST"].ToString().Trim() == string.Empty)
                {
                    nazione = "IT";
                }
                else {
                    nazione = aziendaAltriDati.Rows[0]["DENSTAEST"].ToString().Trim();
                }
            }
            AziendaPagoPa aziendaPagoPa = new AziendaPagoPa(
                 azienda.Rows[0]["RAGSOC"].ToString().Trim().ToUpper(),
                 pIva, indirizzo, aziendaAltriDati.Rows[0]["CAP"].ToString().Trim(),
                 aziendaAltriDati.Rows[0]["DENLOC"].ToString().Trim().ToUpper(),
                 aziendaAltriDati.Rows[0]["SIGPRO"].ToString().Trim().ToUpper(),
                 aziendaAltriDati.Rows[0]["EMAIL"].ToString().Trim().ToLower().Replace(";", ""),
                 nazione, aziendaAltriDati.Rows[0]["NUMCIV"].ToString().Trim().ToUpper()
                );
            return aziendaPagoPa;
        }
    }


}

                   