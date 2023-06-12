// Decompiled with JetBrains decompiler
// Type: TFI.Utilities.FTP
// Assembly: TFI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8C322276-BA13-4F76-8B41-0B6E94A1BA76
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\TFI.dll

using System;
using System.Configuration;
using System.IO;
using System.Net;
using TFI.CRYPTO.Crypto;

namespace TFI.Utilities
{
  public class FTP
  {
    public byte[] DownloadFile(string fileName)
    {
      byte[] array;
      try
      {
        string userName = Cypher.DeCryptPassword(ConfigurationManager.AppSettings.Get("Login").ToString());
        string password = Cypher.DeCryptPassword(ConfigurationManager.AppSettings.Get("Password").ToString());
        FtpWebRequest ftpWebRequest1 = (FtpWebRequest) WebRequest.Create(fileName);
        ftpWebRequest1.Method = "SIZE";
        ftpWebRequest1.Credentials = (ICredentials) new NetworkCredential(userName, password);
        ftpWebRequest1.UsePassive = true;
        ftpWebRequest1.UseBinary = true;
        ftpWebRequest1.KeepAlive = true;
        long contentLength = ftpWebRequest1.ContentLength;
        long num = 0;
        FtpWebRequest ftpWebRequest2 = (FtpWebRequest) WebRequest.Create(fileName);
        ftpWebRequest2.Method = "RETR";
        ftpWebRequest2.Credentials = (ICredentials) new NetworkCredential(userName, password);
        ftpWebRequest2.UsePassive = true;
        ftpWebRequest2.UseBinary = true;
        ftpWebRequest2.KeepAlive = true;
        using (FtpWebResponse response = (FtpWebResponse) ftpWebRequest2.GetResponse())
        {
          using (Stream responseStream = response.GetResponseStream())
          {
            using (MemoryStream memoryStream = new MemoryStream())
            {
              byte[] buffer = new byte[1024];
              while (true)
              {
                int count = responseStream.Read(buffer, 0, 1024);
                if (count != 0)
                {
                  memoryStream.Write(buffer, 0, count);
                  num += (long) count;
                }
                else
                  break;
              }
              array = memoryStream.ToArray();
              memoryStream.Close();
            }
            responseStream.Close();
          }
          response.Close();
        }
      }
      catch (Exception ex)
      {
        throw;
      }
      return array;
    }
  }
}
