namespace TFI.BLL.Utilities.PagoPa
{
    public class ObjResDataCrea
    {
        public string IuvCode { get; set; }
        public string TransActionId { get; set; }

        public ObjResDataCrea(string iuvCode, string transActionId)
        {
            IuvCode = iuvCode;
            TransActionId = transActionId;
        }

        public ObjResDataCrea()
        {
            
        }
    }
}
