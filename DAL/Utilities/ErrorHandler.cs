// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Utilities.ErrorHandler
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Data;
using System.Net;
using System.Web;
using TFI.DAL.ConnectorDB;

namespace TFI.DAL.Utilities
{
  public class ErrorHandler
  {
    public static void AggErrori(Exception ex, string utente, string oggetto_Pagina)
    {
      DataLayer dataLayer = new DataLayer();
      TFI.OCM.Utente.Utente utente1 = HttpContext.Current.Session[nameof (utente)] as TFI.OCM.Utente.Utente;
      try
      {
        dataLayer.StartTransaction();
        string strSQL1 = "SELECT VALUE(MAX(CODERR), 0) + 1 FROM ERRORI ";
        int int32 = Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text));
        string str1 = HttpContext.Current.Request.RawUrl.ToString().Substring(0, HttpContext.Current.Request.RawUrl.Length < 50 ? HttpContext.Current.Request.RawUrl.Length : 50);
        DateTime now = DateTime.Now;
        string strData = now.ToString().Substring(0, 10);
        now = DateTime.Now;
        string str2 = now.ToString("HH:mm:ss");
        string message = ex.Message.Replace("'", "").Replace('"', '°');
        string stackTrace = ex.StackTrace.Replace("'", "^").Replace('"', '°');
        string str3 = Environment.MachineName.ToString();
        string str4 = Dns.GetHostAddresses(Environment.MachineName)[2].ToString();
        string str5 = utente.Replace("'", "^").Replace('"', '°').Replace("{", "|").Replace("}", "|");
        string str6 = oggetto_Pagina.Replace("'", "^").Replace('"', '°').Replace("{", "|").Replace("}", "|");
        string str7 = !(str6 == "") ? stackTrace + str5 + str6 : stackTrace + str5;
        string str8 = "INSERT INTO ERRORI (CODERR, DATA, ORA, USERAPP, IP, NOMEPC, FORM, DESERR, STACKTRACE, AMBIENTE) " + " VALUES ('" + int32.ToString() + "','" + DBMethods.Db2Date(strData) + "','" + str2 + "','" + utente1.Username + "', '" + str4 + "','" + str3 + "', " + "'" + str1 + "','" + message + "','" + str7 + "', ";
        string strSQL2 = string.IsNullOrEmpty(str7) ? str8 + " 'I' ) " : str8 + " 'W' ) ";
        dataLayer.WriteTransactionData(strSQL2, CommandType.Text);
      }
      catch (Exception ex1)
      {
        dataLayer.EndTransaction(false);
      }
      dataLayer.EndTransaction(true);
    }
  }
}
