using System;
using System.IO;
using System.Web;

namespace TFI.BLL.Utilities;

public static class FIleUtils
{
    public static byte[] FromHttpPostedFileBaseToByteArray(this HttpPostedFileBase file)
    {
        if (file == null || file.ContentLength <= 0) return Array.Empty<byte>();
        
        var buffer = new byte[file.ContentLength]; 
        var nrBytesWritten = file.InputStream.Read(buffer, 0, file.ContentLength);
        
        return nrBytesWritten == default 
            ? Array.Empty<byte>() 
            : buffer;
    }
}