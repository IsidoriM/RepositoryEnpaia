using System.Collections.Generic;

namespace TFI.OCM.AziendaConsulente
{
    public class DatiNuovaDenuncia
    {
        public string Anno { get; set; }

        public int IntMese { get; set; }

        public string StrMese { get; set; }

        public int AnnFia { get; set; }

        public int MesFia { get; set; }

        public int ProDen { get; set; }

        public int IdDIPA { get; set; }

        public int IdAzi { get; set; }

        public string Fia { get; set; }

        public bool IsNotEnpaiaUser { get; set; }

        public string DataDenuncia { get; set; }

        public string IndirizzoSedeLegale { get; set; }

        public bool ABBPREColumnIsHidden { get; set; }

        public bool HdnImportoZero { get; set; }

        public bool BtnTotali_Enabled { get; set; }

        public string Imponibile { get; set; }

        public string Occasionali { get; set; }

        public string Figurative { get; set; }

        public string Sanitario { get; set; }

        public string TotDip { get; set; }

        public DatiAnagraficiAzienda DatiAnagrafici { get; set; }

        public SediAziendali DatiSedeLegale { get; set; }

        public List<RetribuzioneRDL> ListaReport { get; set; }

        public List<ParametriGenerali> ListaParametriGenerali { get; set; }

        public string Contributi { get; set; }
    }
}
