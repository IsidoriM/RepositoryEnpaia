
using Iscritto;
using OCM.TFI.OCM.Iscritto;
using System.Collections.Generic;
using TFI.DAL.Iscritto;
using TFI.DAL.Login;
using TFI.OCM.Utente;

namespace TFI.BLL.Iscritto
{
    public class RetribuzioneBLL
    {
        private readonly RetribuzioneDAL retDAL = new RetribuzioneDAL();

        public List<RetribuzioneAnnuale> GetRetribuzioneAnnuales (Utente utente)
        {
            var matricola = LoginDAL.GetMatricola(utente);
            var retAnn = retDAL.GetRetribuzioneAnnuale(matricola);
            return retAnn;
        }
        public List<RetribuzioneMensile> GetRetribuzioneMensile(int posizione, int matricola, int progressivo, int anno)
        {
            var retMen = retDAL.GetRetribuzioneMensile(posizione, matricola, progressivo, anno);
            return retMen;
        }
    }
}
