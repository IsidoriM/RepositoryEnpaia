namespace TFI.OCM.Utente
{
    public class Utente
    {
        public int Id { get; }

        public string Username { get; set; }

        public string Denominazione { get; }

        public string Tipo { get; }

        public string Password { get; set; }

        public string ScadenzaPwd { get; set; }

        public string StatoPwd { get; set; }

        public string CodPosizione { get; set; }

        public string CodFiscale { get; set; }

        public string DescrizioneAzienda { get; set; }

        public string Nome { get; set; }

        public string Cognome { get; set; }

        public string Tipopin { get; set; }

        public string Stato { get; set; }

        public bool Loggato { get; set; }

        public bool ErroreConnessione { get; set; }

        public bool ErroreQuery { get; set; }

        public int IdPosizione { get; set; }

        public string Posizione { get; set; }

        public string ConsorzioOfConsulente { get; set; }

        public string Ruolo { get; set; }

        public string Tabella { get; set; }

        public string Selected { get; set; }

        public string NomeAzienda { get; set; }

        public bool SenzaDeleghe { get; set; }

        public string CodTer { get; set; }

        public Utente(string username, string denominazione, string tipo, string password, string scadenzaPwd, string codPosizione, string codFiscale, bool senzaDeleghe)
        {
            Username = username;
            Denominazione = denominazione;
            Tipo = tipo;
            Password = password;
            ScadenzaPwd = scadenzaPwd;
            CodPosizione = codPosizione;
            CodFiscale = codFiscale;
        }

        public Utente(string username, string denominazione, string tipo, string password, string scadenzaPwd, string codFiscale, string ragioneSociale, string codTer) 
        {
            Username = username;
            Denominazione = denominazione;
            Tipo = tipo;
            Password = password;
            ScadenzaPwd = scadenzaPwd;
            CodFiscale = codFiscale;
            DescrizioneAzienda = ragioneSociale;
            CodTer = codTer;
        }
    }
}
