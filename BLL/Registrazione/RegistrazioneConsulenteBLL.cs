using System;
using System.Linq;
using log4net;
using OCM.TFI.OCM.Registrazione;
using TFI.BLL.Utilities;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Registrazione;
using TFI.OCM;

namespace TFI.BLL.Registrazione;

public class RegistrazioneConsulenteBLL
{
    private readonly RegistrazioneConsulenteDAL _registrazioneConsulenteDal;
    private readonly PasswordManagerService _passwordManagerService;
    private readonly ILog _log;

    public RegistrazioneConsulenteBLL()
    {
        _registrazioneConsulenteDal = new RegistrazioneConsulenteDAL();
        _passwordManagerService = new PasswordManagerService();
        _log = LogManager.GetLogger("RollingFile");
    }

    public ResultDto Registra(RegistrazioneConsulenteModel registrazioneConsulenteModel)
    {
        try
        {
            var password = _passwordManagerService.GenerateStrongPassword();
            var partitaIvaOrCodiceFiscaleExists = _registrazioneConsulenteDal.PartitaIvaOrCodiceFiscaleExists(registrazioneConsulenteModel.PartitaIva,
                registrazioneConsulenteModel.CodiceFiscale);
            if (partitaIvaOrCodiceFiscaleExists.IsSuccessfull)
            {
                return new ResultDto(false, partitaIvaOrCodiceFiscaleExists.Message);
            }
            _registrazioneConsulenteDal.AvvioTransazione();
            var isRegistered = _registrazioneConsulenteDal.Registra(registrazioneConsulenteModel, password);
            if (!isRegistered)
            {
                _registrazioneConsulenteDal.RollbackTransazione();
                return new ResultDto(false, "Errore nella registrazione");
            }
            var (succeded, _) = SmtpEmailService.InviaRegistrazioneConsulente(
                registrazioneConsulenteModel.EmailRegistrazione,
                registrazioneConsulenteModel.Denominazione, password, GetIdentificativo(registrazioneConsulenteModel));
    
            if (!succeded)
            {
                _registrazioneConsulenteDal.RollbackTransazione();
                return new ResultDto(false, "Errore nell'invio dell'email");
            }
            
            _registrazioneConsulenteDal.CommitTransazione();
            return new ResultDto(true);
        }
        catch (Exception e)
        {
            return new ResultDto(false, e.Message);
        }

        string GetIdentificativo(RegistrazioneConsulenteModel registrazioneConsulenteModel)
        {
            return string.IsNullOrWhiteSpace(registrazioneConsulenteModel.PartitaIva)
                ? registrazioneConsulenteModel.CodiceFiscale
                : registrazioneConsulenteModel.PartitaIva;
        }
    }
}


