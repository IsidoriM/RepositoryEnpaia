public class ObjResponseGet{
    public string Esito { get; }
    public string Message { get; }
    public ObjResDataGet Data { get; }

    public ObjResponseGet(string esito, string message, ObjResDataGet data)
    {
        Esito = esito;
        Message = message;
        Data = data;
    }
}
