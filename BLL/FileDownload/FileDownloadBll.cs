using TFI.BLL.DllProtocollo;

namespace TFI.BLL.FileDownload;

public class FileDownloadBll
{
    private readonly ProtocolloDll _protocolloDll = new ProtocolloDll();
    
    public byte[] GetFileFromDll(int idAllegato, string uuid)
    {
        return _protocolloDll.DownloadAllegato(idAllegato, uuid);
    }
}