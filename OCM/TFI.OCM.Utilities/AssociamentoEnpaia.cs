using System.Collections.Generic;
using TFI.OCM.Utente;

namespace TFI.OCM.Utilities
{
    public class AssociamentoEnpaia
    {
        public enum ActionMethods
        {
            DenunciaMensile_Index = 1,
            RicercaArretrati,
            UploadArretrati
        }

        public bool _isAssociated { get; }

        public string _actionMethod { get; set; }

        public AssociamentoEnpaia(TFI.OCM.Utente.Utente utente, int action)
        {
            if (utente.Tipo == "E")
            {
                _isAssociated = !string.IsNullOrEmpty(utente.CodPosizione);
                _actionMethod = GetActionName(action);
            }
        }

        private static string GetActionName(int action)
        {
            Dictionary<int, string> ActionNames = new Dictionary<int, string>();
            ActionNames.Add(1, "DenunciaMensile_Index");
            ActionNames.Add(2, "RicercaArretrati");
            ActionNames.Add(3, "UploadArretrati");
            return ActionNames[action];
        }
    }
}
