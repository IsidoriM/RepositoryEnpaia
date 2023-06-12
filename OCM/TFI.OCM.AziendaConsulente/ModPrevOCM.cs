using System.Collections.Generic;

namespace TFI.OCM.AziendaConsulente
{
    public class ModPrevOCM
    {
        public class DatGen
        {
            public string rapplegale { get; set; }

            public string matricola { get; set; }

            public string nome { get; set; }

            public string cognome { get; set; }

            public string dataini { get; set; }

            public string datacess { get; set; }

            public string causale { get; set; }

            public string dataIndeInn { get; set; }

            public string dataIndeCess { get; set; }

            public string indennita { get; set; }

            public string importo { get; set; }

            public string consulente { get; set; }

            public string telefono { get; set; }

            public string Aliq { get; set; }

            public string prorap { get; set; }

            public string promod { get; set; }
        }

        public class denunce
        {
            public string anno { get; set; }

            public string mese { get; set; }

            public string iniperiodo { get; set; }

            public string finperiodo { get; set; }

            public string retrimp { get; set; }

            public string occ { get; set; }

            public string Aliq { get; set; }

            public string fig { get; set; }
        }

        public class arretrati
        {
            public string annoDen { get; set; }

            public string annoComp { get; set; }

            public string iniperiodo { get; set; }

            public string finperiodo { get; set; }

            public string retrimp { get; set; }

            public string occ { get; set; }

            public string Aliq { get; set; }

            public string retrimpRett { get; set; }

            public string occRett { get; set; }

            public string id { get; set; }
        }

        public class totali
        {
            public string anno1 { get; set; }

            public string totImpAcc1 { get; set; }

            public string totOccAcc1 { get; set; }

            public string totFigAcc1 { get; set; }

            public string totImpRett1 { get; set; }

            public string totOccRett1 { get; set; }

            public string totFigRett1 { get; set; }

            public string anno2 { get; set; }

            public string totImpAcc2 { get; set; }

            public string totOccAcc2 { get; set; }

            public string totFigAcc2 { get; set; }

            public string totImpRett2 { get; set; }

            public string totOccRett2 { get; set; }

            public string totFigRett2 { get; set; }
        }

        public class part_Time
        {
            public string datini { get; set; }

            public string datfin { get; set; }

            public string prepar { get; set; }
        }

        public class sospensioni
        {
            public string dal { get; set; }

            public string al { get; set; }

            public string motsosp { get; set; }
        }

        public class listsosp
        {
            public string codsos { get; set; }

            public string densos { get; set; }
        }

        public List<denunce> denunces { get; set; }

        public DatGen datGen { get; set; }

        public totali Totali { get; set; }

        public List<arretrati> Arretrati { get; set; }

        public List<part_Time> part_time { get; set; }

        public List<sospensioni> sosp { get; set; }

        public List<listsosp> listsosps { get; set; }

        public ModPrevOCM()
        {
            denunces = new List<denunce>();
            datGen = new DatGen();
            Totali = new totali();
            Arretrati = new List<arretrati>();
            part_time = new List<part_Time>();
            sosp = new List<sospensioni>();
            listsosps = new List<listsosp>();
        }
    }
}
