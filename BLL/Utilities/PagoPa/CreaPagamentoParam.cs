namespace TFI.BLL.Utilities.PagoPa
{
    public partial class PagoPa
    {
        public class CreaPagamentoParam{
            public string IdServizio { get; }
            public string DataRimozione { get; }
            public string DataScadenza { get; }
            public string CodiceDebito { get;  }
            public string TipoPagatore { get;  }
            public string CfPivaPagatore { get; }
            public string NomePagatore { get; }
            public string IndirizzoPagatore { get; }
            public string CivPagatore { get; }
            public string CapPagatore { get; }
            public string LocalitaPagatore { get; }
            public string ProvinciaPagatore { get; }
            public string NazionePagatore { get; }
            public string EmailPagatore { get; }
            public string Importo { get; }
            public string CausaleVersamento { get; }
            public string DescrizioneCausaleVersamento { get; }
            public string UtenteInserimento { get; }
            public string ReturnUrl { get; }

            private CreaPagamentoParam(string idServizio, string dataRimozione, string dataScadenza, 
                string codiceDebito, string tipoPagatore, string cfPivaPagatore, string nomePagatore, 
                string indirizzoPagatore, string civPagatore, string capPagatore, string localitaPagatore, 
                string provinciaPagatore, string nazionePagatore, string emailPagatore, string importo, 
                string causaleVersamento, string descrizioneCausaleVersamento, string utenteInserimento, string returnUrl)
            {
                IdServizio = idServizio;
                DataRimozione = dataRimozione;
                DataScadenza = dataScadenza;
                CodiceDebito = codiceDebito;
                TipoPagatore = tipoPagatore;
                CfPivaPagatore = cfPivaPagatore;
                NomePagatore = nomePagatore;
                IndirizzoPagatore = indirizzoPagatore;
                CivPagatore = civPagatore;
                CapPagatore = capPagatore;
                LocalitaPagatore = localitaPagatore;
                ProvinciaPagatore = provinciaPagatore;
                NazionePagatore = nazionePagatore;
                EmailPagatore = emailPagatore;
                Importo = importo;
                CausaleVersamento = causaleVersamento;
                DescrizioneCausaleVersamento = descrizioneCausaleVersamento;
                UtenteInserimento = utenteInserimento;
                ReturnUrl = returnUrl;
            }

            public static CreaPagamentoParam Create(string idServizio, string dataRimozione, string dataScadenza,
                string codiceDebito, string tipoPagatore, string cfPivaPagatore, string nomePagatore,
                string indirizzoPagatore, string civPagatore, string capPagatore, string localitaPagatore,
                string provinciaPagatore, string nazionePagatore, string emailPagatore, string importo,
                string causaleVersamento, string descrizioneCausaleVersamento, string utenteInserimento, string returnUrl)
            {
                return new CreaPagamentoParam(
                    idServizio, dataRimozione , dataScadenza , codiceDebito , tipoPagatore , cfPivaPagatore , 
                    nomePagatore, indirizzoPagatore, civPagatore , capPagatore , localitaPagatore , provinciaPagatore ,
                    nazionePagatore, emailPagatore, importo, causaleVersamento, descrizioneCausaleVersamento, 
                    utenteInserimento, returnUrl);
            }
        }

    }
}