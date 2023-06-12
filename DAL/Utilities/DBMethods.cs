// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Utilities.DBMethods
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using log4net;
using System;
using System.Collections.Generic;

namespace TFI.DAL.Utilities
{
  public class DBMethods
  {
    private static readonly ILog log = LogManager.GetLogger("RollingFile");
    private static readonly ILog TrackLog = LogManager.GetLogger("Track");

    public static string Db2Date(string strData)
    {
      DateTime result;
      return strData.Trim() != string.Empty && DateTime.TryParse(strData, out result) ? result.ToString("u").Substring(0, 10) : "null";
    }

    public static string DoublePeakForSql(string value) => "'" + (value ?? "").Replace("'", "''").Trim() + "'";

    public static Dictionary<int, string> GetMesi() => new Dictionary<int, string>()
    {
      {
        0,
        "seleziona"
      },
      {
        1,
        "Gennaio"
      },
      {
        2,
        "Febbraio"
      },
      {
        3,
        "Marzo"
      },
      {
        4,
        "Aprile"
      },
      {
        5,
        "Maggio"
      },
      {
        6,
        "Giugno"
      },
      {
        7,
        "Luglio"
      },
      {
        8,
        "Agosto"
      },
      {
        9,
        "Settembre"
      },
      {
        10,
        "Ottobre"
      },
      {
        11,
        "Novembre"
      },
      {
        12,
        "Dicembre"
      }
    };
  }
}
