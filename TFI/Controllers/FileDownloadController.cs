using System.IO;
using System.Web.Mvc;
using TFI.BLL.FileDownload;
using TFI.Utilities;

namespace TFI.Controllers
{
    public class FileDownloadController : Controller
    {
        private readonly FileDownloadBll _fileDownloadBll = new FileDownloadBll();
        
        public FileResult DownloadFileRegistrazione(int idAllegato, string uuid)
        {
            var file = _fileDownloadBll.GetFileFromDll(idAllegato, uuid);
            return new FileStreamResult(GetMemoryStream(file), "application/pdf");
        }
        
        [UserExpiredCheck]
        public FileResult Download(string id, string FullPath)
        {
            string[] arF = FullPath.Split('\\');
            return File(FullPath, "application/octet-stream", arF[arF.Length - 1]);
        }

        private MemoryStream GetMemoryStream(byte[] file)
        {
            MemoryStream workStream = new MemoryStream();
            workStream.Write(file, 0, file.Length);
            workStream.Position = 0;
            return workStream;
        }
    }
}
