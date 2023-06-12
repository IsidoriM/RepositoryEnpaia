using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TFI.DAL.AziendaConsulente;
using TFI.OCM.AziendaConsulente;

namespace TFI.BLL.AziendaConsulente
{
    public class RicercaArretratiBLL
    {
        public static string ErrorMessage = string.Empty;
        public static string WarningMessage = string.Empty;
        public static string SuccessMessage = string.Empty;

        public static RicercaArretrati CaricaDDL(string codPos) => new RicercaArretrati()
        {
            lblIntestazioneisVisible = false,
            listaAnniDenuncia = RicercaArretratiDAL.LoadAnniDenuncia(codPos),
            listaAnniCompetenza = RicercaArretratiDAL.LoadAnniCompetenza(codPos)
        };

        public static RicercaArretrati GetArretrati(
          int radioButtonVal,
          string codPos,
          string annoDenuncia,
          string annoCompetenza,
          string matricola,
          string parm,
          string proden,
          string anno,
          string mat)
        {
            RicercaArretrati ricercaArretrati = new();
            string lblIntestazione = string.Empty;
            ricercaArretrati.ModDettaglio = 1;
            ricercaArretrati.ModVisualizzazione = string.Empty;

            ricercaArretrati.listaDatiRicerca = RicercaArretratiDAL.GetArretrati(codPos, matricola, annoCompetenza, annoDenuncia, ref lblIntestazione);

            SetModelPropertiesIfRicercaArretratiReturnsValues();

            return ricercaArretrati;

            void SetModelPropertiesIfRicercaArretratiReturnsValues()
            {
                if (ricercaArretrati.listaDatiRicerca is null)
                {
                    ricercaArretrati.lbldatiNulliisVisible = true;
                    return;
                }

                SetIdRigaSelezionataIfParmIsValorized();
                GetArretratoNonConfermatoIfPresentAndIntegrateItsData();

                ricercaArretrati.listaDatiRicerca = ricercaArretrati.listaDatiRicerca.OrderByDescending(arretrato => arretrato.StaDen).ThenByDescending(arr => arr.AnnCom).ToList();
                ricercaArretrati.listaDatiRicerca.ForEach(arretrato => arretrato.StaDen = FormatStatoDenunciaBasedOnHisValue(arretrato));

                SetPropertiesForViewModel();

                void SetIdRigaSelezionataIfParmIsValorized()
                {
                    if (string.IsNullOrEmpty(parm))
                        return;
                    switch (radioButtonVal)
                    {
                        case 1:
                            ricercaArretrati.IdRigaSelezionata = 
                                ricercaArretrati.listaDatiRicerca.FirstOrDefault(r => r.Mat == int.Parse(mat) && r.ProDen == int.Parse(proden) && r.AnnCom == int.Parse(anno)).Id;
                            break;
                        case 2:
                            ricercaArretrati.IdRigaSelezionata = ricercaArretrati.listaDatiRicerca.FirstOrDefault(r => r.Mat == int.Parse(mat) && r.ProDen == int.Parse(proden)).Id;
                            break;
                    }
                }

                void GetArretratoNonConfermatoIfPresentAndIntegrateItsData()
                {
                    ricercaArretrati.ArretratoNonConfermato = RicercaArretratiDAL.GetArretratoNonConfermatoFromDentesWith(codPos);
                    var AreArretratiInListaDatiRicerca = ricercaArretrati.listaDatiRicerca.Any(arretrato => arretrato.StaDen != "A");

                    if (AreArretratiInListaDatiRicerca)
                    {
                        ricercaArretrati.ArretratoNonConfermato.Id = ricercaArretrati.listaDatiRicerca.FirstOrDefault(arretrato => arretrato.StaDen == "N").Id;
                        ricercaArretrati.ArretratoNonConfermato.AnnCom = ricercaArretrati.listaDatiRicerca.FirstOrDefault(arretrato => arretrato.StaDen == "N").AnnCom;
                        ricercaArretrati.AreArretratiNonConfermati = true;
                    }
                    else if (ricercaArretrati.ArretratoNonConfermato != null && !AreArretratiInListaDatiRicerca)
                    {
                        var intestazioneFinta = string.Empty;
                        var tuttiGliArretrati = RicercaArretratiDAL.GetArretrati(codPos, string.Empty, "0", "0", ref intestazioneFinta);
                        ricercaArretrati.ArretratoNonConfermato.Id = tuttiGliArretrati.FirstOrDefault(arretrato => arretrato.StaDen == "N").Id;
                        ricercaArretrati.ArretratoNonConfermato.AnnCom = tuttiGliArretrati.FirstOrDefault(arretrato => arretrato.StaDen == "N").AnnCom;
                        ricercaArretrati.AreArretratiNonConfermati = true;
                    }
                }

                string FormatStatoDenunciaBasedOnHisValue(RicercaArretrati_Data arretratoFromDb)
                {
                    if(arretratoFromDb.StaDen != "A")
                        return "Non Confermata";
                    else
                        return (arretratoFromDb.CodModPag <= 0 ? "Acquisita senza estrami di pag." : "Acquisita con estremi di pag.");
                }

                void SetPropertiesForViewModel()
                {
                    ricercaArretrati.ModVisualizzazione = "Dettaglio";
                    ricercaArretrati.btnSelezionaisVisible = ricercaArretrati.listaDatiRicerca.Count > 0;
                    ricercaArretrati.ModDettaglio = 2;
                    ricercaArretrati.isRicerca = true;
                    ricercaArretrati.lblIntestazioneisVisible = true;
                }
            }
        }

        public static RettificheArretrati LoadDataRettifiche(
          string codPos,
          string anno,
          string mese,
          string proDen,
          string anncom,
          string mat)
        {
            RettificheArretrati arretrato = new RettificheArretrati();
            arretrato.listaRettifiche = RicercaArretratiDAL.LoadRettifiche(codPos, anno, mese, proDen, anncom, mat, arretrato);
            return arretrato;
        }

        public static bool UploadArretrati(
          TFI.OCM.Utente.Utente utente,
          int anno,
          HttpPostedFileBase dipa,
          string path,
          ref string proDen,
          ref bool btnStampaIsVisible,
          ref bool btnConfermaIsVisible)
        {
            List<ParametriGenerali> parametriGeneraliList = new List<ParametriGenerali>();
            parametriGeneraliList = (List<ParametriGenerali>) null;
            try
            {
                if (RicercaArretratiDAL.UploadArretrati(utente, utente.CodPosizione, anno, dipa, path, ref proDen, ref btnStampaIsVisible, ref btnConfermaIsVisible))
                {
                    RicercaArretratiBLL.SuccessMessage = "Operazione effettuata con successo!";
                    return true;
                }
                RicercaArretratiBLL.ErrorMessage = "Caricamento della denuncia arretrati non riuscito! " + Environment.NewLine + DenunciaMensileDAL.ErrorMessage;
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
