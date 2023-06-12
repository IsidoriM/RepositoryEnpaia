using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TFI.OCM.AziendaConsulente
{
      
    public class NuovoInfortunio
    {

        public NuovoInfortunio()
        {
           
        }
        public NuovoInfortunio(string Proinf)
        {
            this.Proinf = Proinf;
        }

        

        public NuovoInfortunio(string matricola, string nome, string cognome, string areaPer, string codPos, string datainf, string dataden, string tipinf, string indrc, string indrd,string causa,string sede,string natura,string agente,string tipage,string tipoforma,string diagnosi,string prognosi) 
        {
            Matricola = matricola;
            Nome = nome;
            Cognome = cognome;
            AreaPer = areaPer;
            CodPos = codPos;
            Datainf = datainf;
            Dataden = dataden;
            Tipoinf = tipinf;
            Indrc = indrc;
            Indrd = indrd;
            Codcauinf = causa;
            Codsedinf = sede;
            Codnatinf = natura;
            Codageinf = agente;
            Tipage = tipage;
            Codform = tipoforma;
            Diagnosi = diagnosi;
            Prognosi = prognosi;   


        }

        public String CodPos { get; set; }

       
        public String Cognome { get; set; }

        public String Nome { get; set; }

        public String Codfiscale { get; set; }

        public String Prorap { get; set; }
        public String Proinf { get; set; }

        public String Datainf { get; set; }

        public String Dataden { get; set; }

        public String Tipoinf { get; set; }

        public String Indrc { get; set; }

        public String Indrd { get; set; }

        public String Accmedleg { get; set; }
        public String DataNas { get; set; }
        public String Matricola { get; set; }

        public String Tutela { get; set; }

        public String Tipoescl { get; set; }
        public String Codcauinf { get; set; }

        public String Codsedinf { get; set; }

        public String Codnatinf { get; set; }

        public String Codageinf { get; set; }

        public String Tipage { get; set; }

        public String Codform { get; set; }

        public String Diagnosi { get; set; }

        public String Prognosi { get; set; }

        public String Codstapra { get; set; }

        public String Datann { get; set; }
        public String Ultagg { get; set; }

        public String Uteagg { get; set; }
         
        public String Stadip { get; set; }

        public String AreaPer { get; set; }

        public String Tipodip { get; set; }

        public String Valido { get; set; }

        public Boolean Rdl { get; set; }

        public String  Via { get; set; }

        public String Localita { get; set; }

        public String Indirizzo { get; set; }

        public String Descrizione { get; set; }


    }
}
