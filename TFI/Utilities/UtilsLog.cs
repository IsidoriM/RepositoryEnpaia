// Decompiled with JetBrains decompiler
// Type: TFI.Utilities.UtilsLog
// Assembly: TFI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8C322276-BA13-4F76-8B41-0B6E94A1BA76
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\TFI.dll

using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace TFI.Utilities
{
  public class UtilsLog
  {
    private string GetDayForLog() => ConfigurationManager.AppSettings.Get("DayForLog");

    public void CheckLog()
    {
      string[] files = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Log"));
      string str = string.Empty;
      int index1 = 0;
      for (int index2 = 0; index2 <= 365; ++index2)
      {
        try
        {
          string[] strArray = files[index1].Split('\\');
          str = strArray[strArray.Length - 1].Substring(8, 8);
          string path2 = "TFI-Log_" + DateTime.Now.AddDays(Convert.ToDouble(this.GetDayForLog()) - (double) index2).ToString("ddMMyyyy") + ".txt";
          File.Delete(Path.Combine(HttpContext.Current.Server.MapPath("~/Log"), path2));
        }
        catch (Exception ex)
        {
          index1 = 0;
        }
        ++index1;
      }
    }
  }
}
