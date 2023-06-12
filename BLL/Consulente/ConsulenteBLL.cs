using TFI.DAL.Consulente;
using System.Collections.Generic;
using System.Security.Claims;
using log4net;
using OCM.TFI.OCM.AziendaConsulente;
using TFI.BLL.Utilities;
using TFI.OCM;
using static TFI.BLL.Utilities.SmtpEmailService;
using System.Linq;

namespace TFI.BLL.Consulente
{
    public class ConsulenteBLL
    {
        private readonly ConsulenteDAL _consulenteDal = new ConsulenteDAL();
        private static readonly ILog _log = LogManager.GetLogger("RollingFile");

        public List<AziendaLight> GetAziendeConDelegaAttivaOrInAttesa(string codTer)
        {
            var aziendeConDelegaAttivaOrInAttesa = _consulenteDal.GetAziendeConDelegaAttivaOrInAttesa(codTer);
            return aziendeConDelegaAttivaOrInAttesa.OrderByDescending(azi => azi.IsDelegaConfermata).ToList();           
        }

        public AziendaLight CercaAziendaSenzaDelegaAttiva(string identificativo, ref string errorMsg)
        {
            var azienda = _consulenteDal.GetDatiAzienda(identificativo);
            if (azienda == default) 
            {
                errorMsg = "Non è stata trovata nessuna azienda con questo identificativo.";
                return azienda;                
            }

            var isAziendaConDelegaAttiva = _consulenteDal.CheckDelegaAttiva(azienda.CodiceIdentificativo);
            if (isAziendaConDelegaAttiva)
            {
                errorMsg = "L'azienda ricercata risulta avere una delega attiva.";
                return null;
            }

            return azienda;
        }

        public ResultDto RichiediDelega(string codPos, string codTer)
        {
            var esitoRichiestaDelega = new ResultDto();
            
            if(_consulenteDal.CheckDelegaAttivaOrInAttesa(codPos, codTer))
            {
                SetResponse(false, "Risulta già una delega attiva o in attesa di conferma con questa posizione.");
                return esitoRichiestaDelega;
            }
            
            var result = _consulenteDal.RichiediDelega(codPos, codTer);
            if (!result)
            {
                SetResponse(false, "Errore nella richiesta di delega.");
                return esitoRichiestaDelega;
            }

            var azienda = _consulenteDal.GetDatiAzienda(codPos);
            var consulente = _consulenteDal.GetDatiConsulente(codTer);
            
            var resultInvioEmailConsulente = InviaMailRichiestaDelegaConsulente(consulente.Email, consulente.RagioneSociale,
                azienda.RagioneSociale);
            if (!resultInvioEmailConsulente.Succeded)
                esitoRichiestaDelega.Warnings.Add("Invio mail consulente non riuscita.");
            
            var resultInvioEmailAzienda = InviaMailRichiestaDelegaAzienda(azienda.Email, azienda.RagioneSociale,
                consulente.RagioneSociale, CreateToken(codPos, codTer));
            if (!resultInvioEmailAzienda.Succeded)
                esitoRichiestaDelega.Warnings.Add("Invio mail azienda non riuscita.");

            SetResponse(true, string.Empty);
            return esitoRichiestaDelega;

            string CreateToken(string codPos, string codTer)
            {
                var claims = new List<Claim>
                {
                    new("codPos", codPos),
                    new("codTer", codTer)
                };
                return TokenHelper.CreateToken(claims, 1000);
            }

            void SetResponse(bool operationResult, string responseMsg)
            {
                esitoRichiestaDelega.IsSuccessfull = operationResult;
                esitoRichiestaDelega.Message = responseMsg;
            }
        }
    }
}