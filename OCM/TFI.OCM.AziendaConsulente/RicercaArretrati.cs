using OCM.TFI.OCM.Utilities;
using System.Collections.Generic;

namespace TFI.OCM.AziendaConsulente
{
    public class RicercaArretrati : PagingModel
    {
        public int IdRigaSelezionata { get; set; }

        public bool lblIntestazioneisVisible { get; set; }

        public int txtMatricola { get; set; }

        public string ModVisualizzazione { get; set; }

        public string All { get; set; }

        public bool btnSelezionaisVisible { get; set; }

        public int[] tbDettaglioColumnsVisible { get; set; }

        public int[] tbDettaglioColumnsHidden { get; set; }

        public int ModDettaglio { get; set; }

        public bool isRicerca { get; set; }

        public bool lbldatiNulliisVisible { get; set; }

        public int selectedRadioButton { get; set; }

        public int SelectedAnnoComp { get; set; }

        public int SelectedAnnoDen { get; set; }

        public int TotalPages { get; set; }

        public string SelectedColumnForSorting { get; set; }

        public bool IsSortingAscending { get; set; }

        public bool IsAnnoDenSelected { get; set; }

        public bool IsAnnoComSelected { get; set; }

        public bool IsMatricolaSelected { get; set; }

        public bool AreArretratiNonConfermati { get; set; }

        public bool IsAnniCompetenzaListEmpty { get; set; } = false;

        public bool IsArretratiListEmpty { get; set; } = false;

        public RicercaArretrati_Data ArretratoNonConfermato { get; set; }

        public List<RicercaArretrati_Data> listaDatiRicerca { get; set; }

        public List<CaricamentoAnniDenuncia> listaAnniDenuncia { get; set; }

        public List<CaricamentoAnniDenuncia> listaAnniCompetenza { get; set; }
    }
}
