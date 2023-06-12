using System.Collections.Generic;

namespace TFI.OCM.Amministrativo
{
    public class GestioneRapportiLavoroIscrittiOCM
    {
        public class listdebiti2
        {
            public string tiptratt { get; set; }

            public string TipDeb { get; set; }
        }

        public class Iscritti
        {
            public string ultagg { get; set; }

            public string strdatnasold { get; set; }

            public string mat { get; set; }

            public string nominativo { get; set; }

            public string cognome { get; set; }

            public string nome { get; set; }

            public string codfis { get; set; }

            public string datnas { get; set; }

            public string sesso { get; set; }

            public string PRORAP { get; set; }

            public string CODPOS { get; set; }

            public string dug { get; set; }

            public string coddug { get; set; }

            public string co { get; set; }

            public string indirizzo { get; set; }

            public string numciv { get; set; }

            public string comuneN { get; set; }

            public string comuneCodN { get; set; }

            public string provN { get; set; }

            public string statoEsN { get; set; }

            public string statoEsCodN { get; set; }

            public string statoEs { get; set; }

            public string cap { get; set; }

            public string localita { get; set; }

            public string comuneCod { get; set; }

            public string comune { get; set; }

            public string prov { get; set; }

            public string statocivile { get; set; }

            public string titoloStudio { get; set; }

            public string titoloStudioCod { get; set; }

            public string pec { get; set; }

            public string email { get; set; }

            public string cellulare { get; set; }

            public string tel { get; set; }

            public string tel2 { get; set; }

            public string fax { get; set; }

            public string url { get; set; }

            public bool privacy1SI { get; set; }

            public bool privacy1NO { get; set; }

            public bool privacy2SI { get; set; }

            public bool privacy2NO { get; set; }

            public string datiNONModificati { get; set; }

            public string datiRecapitiOld { get; set; }

            public string datiAnagraficiOld { get; set; }

            public string codposAz { get; set; }

            public string ragsocAz { get; set; }

            public string parivaAz { get; set; }

            public int statocheck { get; set; }

            public string datini { get; set; }

            public string datfin { get; set; }

            public string motivo { get; set; }
        }

        public class Indirizzi
        {
            public string coddug { get; set; }

            public string dendug { get; set; }
        }

        public class TitoliStudio
        {
            public string codtitstu { get; set; }

            public string dentistu { get; set; }
        }

        public class DatiBancariIscritto
        {
            public string mat { get; set; }

            public string datini { get; set; }

            public string datfin { get; set; }

            public string modpag { get; set; }

            public string iban { get; set; }

            public string proiscban { get; set; }

            public int occorrenze { get; set; }

            public string swift { get; set; }

            public string istCre { get; set; }

            public string age { get; set; }

            public string indAge { get; set; }
        }

        public class DebitiIscritto
        {
            public int occorrenze { get; set; }

            public string codpos { get; set; }

            public string mat { get; set; }

            public string prorap { get; set; }

            public string protratt { get; set; }

            public string tiptratt { get; set; }

            public string desctratt { get; set; }

            public string impdebito { get; set; }

            public string impestinto { get; set; }

            public string impresiduo { get; set; }

            public string flgsap { get; set; }

            public string ultagg { get; set; }

            public string uteagg { get; set; }

            public string azi { get; set; }

            public string datiniDeb { get; set; }

            public string datfinDeb { get; set; }

            public string TipDeb { get; set; }
        }

        public class Eredi
        {
            public int occorrenzeE { get; set; }

            public string cognomeE { get; set; }

            public string nomeE { get; set; }

            public string sessoE { get; set; }

            public string codfisE { get; set; }

            public string datnasE { get; set; }

            public string denparE { get; set; }

            public string proereE { get; set; }

            public string comuneNE { get; set; }

            public string comuneCodNE { get; set; }

            public string provNE { get; set; }

            public string statoEsNE { get; set; }

            public string statoEsCodNE { get; set; }

            public string statoEs_E { get; set; }

            public string capE { get; set; }

            public string localitaE { get; set; }

            public string comuneCodE { get; set; }

            public string comuneE { get; set; }

            public string provE { get; set; }

            public string coE { get; set; }

            public string emailE { get; set; }

            public string telE { get; set; }

            public string dugE { get; set; }

            public string coddugE { get; set; }

            public string indirizzoE { get; set; }

            public string numcivE { get; set; }

            public string datiniE { get; set; }

            public string parentelaE { get; set; }

            public string perTFRE { get; set; }

            public string perFPE { get; set; }

            public string maggPerE { get; set; }

            public string maggE { get; set; }

            public string IBANE { get; set; }

            public string SWIFTE { get; set; }

            public string istCreE { get; set; }

            public string AgenziaE { get; set; }

            public string indBE { get; set; }

            public string provBE { get; set; }

            public string probancE { get; set; }
        }

        public class BlocchiIscritto
        {
            public int occorrenze { get; set; }

            public string id { get; set; }

            public string tipoBlocco { get; set; }

            public string tipoErrore { get; set; }

            public string denblocco { get; set; }

            public string denerrore { get; set; }
        }

        public class NoteIscritto
        {
            public int occorrenze { get; set; }

            public string idnote { get; set; }

            public string note { get; set; }
        }

        public class RapportiLavoro
        {
            public string ProRap { get; set; }

            public string datass { get; set; }

            public string datces { get; set; }

            public string ragsoc { get; set; }

            public string dences { get; set; }

            public int occorrenze { get; set; }
        }

        public class DatiContrattuali
        {
            public string datisc { get; set; }

            public string datdec { get; set; }

            public string dectratec { get; set; }

            public string tiprap { get; set; }

            public string tiprapCod { get; set; }

            public string ibldal { get; set; }

            public string iblal { get; set; }

            public string datScadenzaTermine { get; set; }

            public string datScaFas { get; set; }

            public string datScadenzaTermineDa { get; set; }

            public string datScadenzaTermineAl { get; set; }

            public string datTerParTime { get; set; }

            public string perParTime { get; set; }

            public string contratto { get; set; }

            public string livello { get; set; }

            public string qualifica { get; set; }

            public string mensilita { get; set; }

            public string mens14 { get; set; }

            public string mens15 { get; set; }

            public string mens16 { get; set; }

            public string asscontr { get; set; }

            public string abbRivista { get; set; }

            public string PerApp { get; set; }

            public string PerPar { get; set; }

            public string DatScaTerPart { get; set; }

            public string DataNuovaVariazione { get; set; }

            public string DataUltimaVar { get; set; }

            public string DataVariazione { get; set; }

            public string CODLOC { get; set; }

            public string CODQUA { get; set; }

            public string CODGRUASS { get; set; }

            public string CODLIV { get; set; }

            public string PROCON { get; set; }

            public string PROLOC { get; set; }

            public string CODCON { get; set; }

            public string CODDUG { get; set; }

            public string aliqContval { get; set; }

            public string aliqCont { get; set; }

            public string ContRif { get; set; }

            public string FAP { get; set; }

            public string PA { get; set; }

            public string TIPSPE { get; set; }

            public string AbbPre { get; set; }

            public string DatIni { get; set; }

            public string strGruCon { get; set; }

            public string CbAssCon { get; set; }

            public string gbAliquota { get; set; }

            public string PrimoImpiegoAl1 { get; set; }

            public string PrimoImpiegoAl2 { get; set; }

            public string PrimoImpiegoAl3 { get; set; }

            public string PrimoImpiegoDal1 { get; set; }

            public string PrimoImpiegoDal2 { get; set; }

            public string PrimoImpiegoDal3 { get; set; }

            public string FrazPrimoImpiego1 { get; set; }

            public string FrazPrimoImpiego2 { get; set; }

            public string FrazPrimoImpiego3 { get; set; }

            public string PrimoImpiegoPartTimeAl1 { get; set; }

            public string PrimoImpiegoPartTimeAl2 { get; set; }

            public string PrimoImpiegoPartTimeAl3 { get; set; }

            public string PrimoImpiegoPartTimeDal2 { get; set; }

            public string PrimoImpiegoPartTimeDal3 { get; set; }

            public string FrazPrimoImpiegoPartTime1 { get; set; }

            public string FrazPrimoImpiegoPartTime2 { get; set; }

            public string FrazPrimoImpiegoPartTime3 { get; set; }

            public string PartTimePrimoImpiego { get; set; }

            public string PartTimeVerticale { get; set; }

            public string durataperiodo { get; set; }

            public string durataperiodo2 { get; set; }
        }

        public class AltriDati
        {
            public string datAssunzione { get; set; }

            public string datDenuncia { get; set; }

            public string fondoSanitario { get; set; }

            public string datProtocollo { get; set; }

            public string numProtocollo { get; set; }

            public string datCess { get; set; }

            public string datDenCess { get; set; }

            public string datPagamentoTFR { get; set; }

            public string causale { get; set; }

            public string datLiquiTFR { get; set; }
        }

        public class AziendaUtilizzatrice
        {
            public string ragsocAz { get; set; }

            public string codposAz { get; set; }

            public string codfisAz { get; set; }

            public string parivaAz { get; set; }

            public string dugAz { get; set; }

            public string dugCodAz { get; set; }

            public string indirizzoAz { get; set; }

            public string civicoAz { get; set; }

            public string comuneAz { get; set; }

            public string comuneCodAz { get; set; }

            public string provAz { get; set; }

            public string capAz { get; set; }

            public string localitaAz { get; set; }

            public string telefonoAz { get; set; }

            public string emailAz { get; set; }

            public string pecAz { get; set; }
        }

        public class Mesi
        {
            public bool gennaio { get; set; }

            public bool febbraio { get; set; }

            public bool marzo { get; set; }

            public bool aprile { get; set; }

            public bool maggio { get; set; }

            public bool giugno { get; set; }

            public bool luglio { get; set; }

            public bool agosto { get; set; }

            public bool settembre { get; set; }

            public bool ottobre { get; set; }

            public bool novembre { get; set; }

            public bool dicembre { get; set; }
        }

        public class DatiRetributivi
        {
            public string emolumenti { get; set; }

            public string TOTALIQ { get; set; }

            public string RetDichiarata { get; set; }

            public string scattiAnz { get; set; }

            public string NumScatt { get; set; }

            public string datUltSc { get; set; }

            public string datProsSc { get; set; }

            public string importoSc { get; set; }

            public string S12 { get; set; }

            public string S13 { get; set; }

            public string S14 { get; set; }

            public string S15 { get; set; }

            public string S16 { get; set; }

            public string totaleS { get; set; }
        }

        public class TipoRapLav
        {
            public string tiprap { get; set; }

            public string dentiprap { get; set; }
        }

        public class Contratti
        {
            public string CODCON { get; set; }

            public string PROCON { get; set; }

            public string CODLOC { get; set; }

            public string PROLOC { get; set; }

            public string CODQUACON { get; set; }

            public string DENQUA { get; set; }

            public string DATDEC { get; set; }

            public string DATINI { get; set; }

            public string DATFIN { get; set; }

            public string ASSCON { get; set; }

            public string DENCON { get; set; }

            public string MAXSCA { get; set; }

            public string PERSCA { get; set; }

            public string NUMMEN { get; set; }

            public string M14 { get; set; }

            public string M15 { get; set; }

            public string M16 { get; set; }

            public string RIVAUT { get; set; }

            public string TIPSPE { get; set; }

            public string RIMUOVI { get; set; }

            public string CODCONLOC { get; set; }

            public string denconnew { get; set; }
        }

        public class ListAliquota
        {
            public string DENFORASS { get; set; }

            public string CODFORASS { get; set; }

            public string ALIQUOTA { get; set; }
        }

        public class Livello
        {
            public string CODLIV { get; set; }

            public string DENLIV { get; set; }
        }

        public class eredi2
        {
            public string codpar { get; set; }

            public string denpar { get; set; }
        }

        public class tipERR
        {
            public string denerrore { get; set; }

            public string tiperrore { get; set; }
        }

        public class tipBlocco
        {
            public string denblocco { get; set; }

            public string tipblocco { get; set; }
        }

        public List<listdebiti2> listdebiti { get; set; }

        public List<Iscritti> listiscritti { get; set; }

        public List<Indirizzi> listaIndirizzi { get; set; }

        public List<TitoliStudio> listTitoli { get; set; }

        public List<DatiBancariIscritto> listDatiBancariIscritto { get; set; }

        public List<DebitiIscritto> listDebitiIscritto { get; set; }

        public List<Eredi> listEredi { get; set; }

        public List<BlocchiIscritto> listBlocchiIscritto { get; set; }

        public List<NoteIscritto> listNoteIscritto { get; set; }

        public List<RapportiLavoro> listRapportiLav { get; set; }

        public List<AziendaUtilizzatrice> listAzienda { get; set; }

        public List<TipoRapLav> listTipRap { get; set; }

        public List<Contratti> listContratti { get; set; }

        public List<ListAliquota> ListAliq { get; set; }

        public List<Livello> listLivello { get; set; }

        public List<eredi2> listeredi { get; set; }

        public List<tipBlocco> listblocco { get; set; }

        public List<tipERR> listErr { get; set; }

        public Iscritti iscritti { get; set; }

        public DatiBancariIscritto datiBancariIscritto { get; set; }

        public DebitiIscritto debitiIscritto { get; set; }

        public Eredi eredi { get; set; }

        public BlocchiIscritto blocchiIscritto { get; set; }

        public NoteIscritto noteIscritto { get; set; }

        public RapportiLavoro rapportiLavoro { get; set; }

        public AziendaUtilizzatrice aziendaUtilizzatrice { get; set; }

        public DatiContrattuali datiContrattuali { get; set; }

        public Mesi mesi { get; set; }

        public DatiRetributivi datiRetributivi { get; set; }

        public TipoRapLav tipRapLav { get; set; }

        public AltriDati altriDati { get; set; }

        public ListAliquota listAliquota { get; set; }

        public Livello livello { get; set; }

        public GestioneRapportiLavoroIscrittiOCM()
        {
            datiRetributivi = new DatiRetributivi();
            iscritti = new Iscritti();
            datiBancariIscritto = new DatiBancariIscritto();
            debitiIscritto = new DebitiIscritto();
            eredi = new Eredi();
            blocchiIscritto = new BlocchiIscritto();
            noteIscritto = new NoteIscritto();
            rapportiLavoro = new RapportiLavoro();
            aziendaUtilizzatrice = new AziendaUtilizzatrice();
            datiContrattuali = new DatiContrattuali();
            tipRapLav = new TipoRapLav();
            altriDati = new AltriDati();
            listAliquota = new ListAliquota();
            livello = new Livello();
        }
    }
}
