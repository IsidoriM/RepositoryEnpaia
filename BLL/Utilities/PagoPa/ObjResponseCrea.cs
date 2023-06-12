namespace TFI.BLL.Utilities.PagoPa
{
    public class ObjResponseCrea
    {
        public string Esito { get; set; }
        public string Message { get; set; }
        public ObjResDataCrea Data { get; set; }

        public ObjResponseCrea(string esito, string message, ObjResDataCrea data)
        {
            Esito = esito;
            Message = message;
            Data = data;
        }
        public ObjResponseCrea()
        {
            
        }
    }
}
