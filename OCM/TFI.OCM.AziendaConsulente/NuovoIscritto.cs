using System;
using System.Collections.Generic;

namespace TFI.OCM.AziendaConsulente
{
    public class NuovoIscritto
    {
        private string _nome { get; set; }
        private string _cognome { get; set; }
        private string _codFis { get; set; }
        public int checkIscritto { get; set; }

        public string prot { get; set; }

        public string numScatti { get; set; }

        public string codCon { get; set; }

        public string traeco { get; set; }

        public string CodcomFor { get; set; }

        public string prorap { get; set; }

        public string matricola { get; set; }

        public string cognome 
        {
            get
            {
                return _cognome;
            }
            set
            {
                _cognome = value?.Trim().ToUpper();
            }
        }

        public string nome
        {
            get
            {
                return _nome;
            }
            set
            {
                _nome = value?.Trim().ToUpper();
            }
        }

        public string codFis
        {
            get
            {
                return _codFis;
            }
            set
            {
                _codFis = value?.Trim().ToUpper();
            }
        }

        public string dataNas { get; set; }

        public string sesso { get; set; }

        public string comuneN { get; set; }

        public string comuneNCod { get; set; }

        public string provinciaN { get; set; }

        public string statoEsN { get; set; }

        public string titoloStudio { get; set; }

        public string titoloStudioCod { get; set; }

        public string indirizzoCod { get; set; }

        public string indirizzo { get; set; }

        public string residenza { get; set; }

        public string civico { get; set; }

        public string comune { get; set; }

        public string comuneCod { get; set; }

        public string cap { get; set; }

        public string provincia { get; set; }

        public string localita { get; set; }

        public string statoEs { get; set; }

        public string co { get; set; }

        public string tel { get; set; }

        public string tel2 { get; set; }

        public string cell { get; set; }

        public string fax { get; set; }

        public string email { get; set; }

        public string pec { get; set; }

        public string datIsc { get; set; }

        public string datDen { get; set; }
        public string datDecMod { get; set; }

        public string tipRap { get; set; }

        public string codtiprap { get; set; }

        public string dentipRap { get; set; }

        public string qualifica { get; set; }

        public string codqualifica { get; set; }

        public string contratto { get; set; }

        public string contrattocod { get; set; }

        public string livello { get; set; }

        public string codlivello { get; set; }

        public string denlivello { get; set; }

        public string numMes { get; set; }

        public string m14 { get; set; }

        public string m15 { get; set; }

        public string m16 { get; set; }

        public string tipspe { get; set; }

        public string aliqCont { get; set; }

        public string aliqContval { get; set; }

        public string aliquota { get; set; }

        public bool assCon { get; set; }

        public bool fap { get; set; }

        public string gruass { get; set; }

        public string fondo { get; set; }
        public bool assistenzaCon { get; set; }

        public List<Mesi> mesiSel { get; set; }
        public List<string> meseSel { get; set; }
        public string codloc { get; set; }

        public string PercPT { get; set; }

        public string periodo { get; set; }

        public string daal { get; set; }

        public string alal { get; set; }

        public string daal2 { get; set; }

        public string alal2 { get; set; }

        public string daal3 { get; set; }

        public string alal3 { get; set; }

        public string perce1 { get; set; }

        public string perce2 { get; set; }

        public string perce3 { get; set; }

        public string percPeri { get; set; }

        public string datTerm { get; set; }

        public string emolumenti { get; set; }

        public string retribuzione { get; set; }

        public string retDic { get; set; }

        public string scattiAnz { get; set; }

        public string dataUltSc { get; set; }

        public string prossimoSc { get; set; }

        public string importoSc { get; set; }

        public string S12 { get; set; }

        public string S13 { get; set; }

        public string S14 { get; set; }

        public string S15 { get; set; }

        public string S16 { get; set; }

        public string totaleS { get; set; }

        public string pozioneAZ { get; set; }

        public string ragsocAZ { get; set; }

        public string cfAz { get; set; }

        public string piAZ { get; set; }

        public string residenzaAZ { get; set; }

        public string residenzaAZCod { get; set; }

        public string indirizzoAZ { get; set; }

        public string numCivAZ { get; set; }

        public string comuneAZ { get; set; }

        public string comuneAZCod { get; set; }

        public string capAZ { get; set; }

        public string provinciaAZ { get; set; }

        public string localitaAZ { get; set; }

        public string hdnProt { get; set; }

        public string RapportoDiLavoroCorrente { get; set; }

    }
}
