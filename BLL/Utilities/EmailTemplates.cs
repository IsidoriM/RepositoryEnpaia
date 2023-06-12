namespace TFI.BLL.Utilities;

public static class EmailTemplates
{
    public static string GetRegistrazioneConsulenteTemplate(string username, string password, string partitaiva)
        => $@"
                <div style='display: block'>
                  <div>
                    <label>Ciao <b>{username}</b> benvenuto sul portale Enpaia.</label>
                  </div>
                  <div>
                    <hr />
                  </div>
                  <div>
                    <div>
                      <label
                        >Da ora potrai effettuare il login direttamente con le seguenti
                        credenziali:</label
                      >
                    </div>
                    <div>
                      <ul>
                        <li>Username: {partitaiva}</li>
                        <li>Password: {password}</li>
                      </ul>
                    </div>
                  </div>
                </div>
            ";
}