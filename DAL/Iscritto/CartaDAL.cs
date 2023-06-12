// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Iscritto.CartaDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Iscritto;

namespace TFI.DAL.Iscritto
{
  public class CartaDAL
  {
    public Anagrafica getCartaEnapia(string cf)
    {
      DataLayer dataLayer1 = new DataLayer();
      string strSQL1 = "SELECT DISTINCT I.MAT,I.COGNOME,I.NOME,I.IND,I.NUMCIV,I.CAP,I.EMAIL,I.EMAILCERT,I.CELL,C.SIGPRO,C.DENCOM,D.DENDUG FROM  ISCTWEB I,  CODCOM C,  DUG D  WHERE CODFIS ='" + cf + "' AND I.CODCOMNAS=C.CODCOM  AND I.CODDUG=D.CODDUG ";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = dataLayer1.GetDataTable(strSQL1);
      Decimal num = Convert.ToDecimal(dataTable2.Rows[0]["MAT"]);
      DataTable dt = new DataTable();
      string str1;
      string str2;
      try
      {
        string strSQL2 = "SELECT DISTINCT TEL1,TEL2 FROM  ISCD  WHERE  MAT='" + num.ToString() + "' ";
        dt = dataLayer1.GetDataTable(strSQL2);
        str1 = dt.Rows[0]["TEL1"].ToString();
        str2 = dt.Rows[0]["TEL2"].ToString();
      }
      catch (Exception ex)
      {
        str1 = "";
        str2 = "";
      }
      Anagrafica cartaEnapia = (Anagrafica) null;
      DataLayer dataLayer2 = new DataLayer();
      if (dataLayer2.queryOk(dataTable2) && dataLayer2.queryOk(dt))
        cartaEnapia = new Anagrafica()
        {
          Mat = num,
          Nome = dataTable2.Rows[0]["NOME"].ToString(),
          Cognome = dataTable2.Rows[0]["COGNOME"].ToString(),
          CodiceFiscale = cf,
          Indirizzo = dataTable2.Rows[0]["IND"].ToString(),
          NumeroCivico = dataTable2.Rows[0]["NUMCIV"].ToString(),
          Cap = dataTable2.Rows[0]["CAP"].ToString(),
          Email = dataTable2.Rows[0]["EMAIL"].ToString(),
          EmailCert = dataTable2.Rows[0]["EMAILCERT"].ToString(),
          Cellulare = dataTable2.Rows[0]["CELL"].ToString(),
          Telefono1 = str1,
          Telefono2 = str2,
          TipoResidenza = dataTable2.Rows[0]["DENDUG"].ToString(),
          ComuneResidenza = dataTable2.Rows[0]["DENCOM"].ToString(),
          SigproResidenza = dataTable2.Rows[0]["SIGPRO"].ToString()
        };
      return cartaEnapia;
    }

    public Anagrafica RicCartaEnpaia(
      Anagrafica AnagCarta,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      string username = ((TFI.OCM.Utente.Utente) HttpContext.Current.Session["utente"]).Username;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      DataLayer dataLayer = new DataLayer();
      int num = 0;
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string empty3 = string.Empty;
      string nome = AnagCarta.Nome;
      string cognome = AnagCarta.Cognome;
      string str1 = AnagCarta.Mat.ToString();
      string strSQL1 = "SELECT * FROM DBUNICONET.RICFINBPS WHERE DB='GSP' AND MAT ='" + str1 + "' AND DATANN IS NULL ORDER BY ULTAGG DESC";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2;
      try
      {
        dataTable2 = dataLayer.GetDataTable(strSQL1);
        if (dataTable2.Rows.Count <= 0)
          throw new Exception();
        if (dataTable2.Rows[0]["DATINVBPS"].ToString() == string.Empty)
          num = Convert.ToInt32(dataTable2.Rows[0]["ID"]);
      }
      catch (Exception ex)
      {
        ErroreMSG = "Richiesta già inviata per l'utente corrente.";
        return this.getCartaEnapia(username);
      }
      try
      {
        int int32 = Convert.ToInt32(dataTable2.Rows[0]["CODPOS"]);
        string strSQL2 = "SELECT TRIM(BON_RAG_SOC || ' ' || VALUE(BON_RAG_SOC_SIN,'')) AS RAGSOC  FROM GESTCONSIP.TBCON WHERE BON_POS = '" + int32.ToString() + "'";
        DataTable dataTable3 = new DataTable();
        string str2;
        try
        {
          str2 = dataLayer.GetDataTable(strSQL2).Rows[0]["RAGSOC"].ToString();
        }
        catch (Exception ex)
        {
          ErroreMSG = "Impossibile effettuare la richiesta. Ragione sociale inesistente.";
          return this.getCartaEnapia(username);
        }
        string strSQL3 = "SELECT MAX(ID) + 1 AS ID FROM DBUNICONET.RICFINBPS";
        DataTable dataTable4 = new DataTable();
        string str3;
        try
        {
          str3 = dataLayer.GetDataTable(strSQL3).Rows[0]["ID"].ToString();
        }
        catch (Exception ex)
        {
          ErroreMSG = ex.Message;
          return this.getCartaEnapia(username);
        }
        string strSQL4 = "SELECT 'GSP' AS DB, ISC_MAT, REPLACE(REPLACE(REPLACE(REPLACE(VARCHAR(CURRENT_TIMESTAMP),'-',''),':',''),'.',''),' ','') AS KEY," + " ISC_COG_NOM, CURRENT_TIMESTAMP FROM GESTCONSIP.TBISC WHERE ISC_MAT ='" + str1 + "' AND ISC_POS = '" + int32.ToString() + "'";
        DataTable dataTable5 = new DataTable();
        string str4;
        try
        {
          DataTable dataTable6 = dataLayer.GetDataTable(strSQL4);
          string str5 = "00000000" + dataTable6.Rows[0]["ISC_MAT"].ToString().Trim();
          str4 = "GSP" + str5.Substring(str5.Length - 8, 8) + dataTable6.Rows[0]["KEY"].ToString();
        }
        catch (Exception ex)
        {
          ErroreMSG = ex.Message;
          return this.getCartaEnapia(username);
        }
        string strSQL5 = "DELETE FROM DBUNICONET.RICFINBPS WHERE ID = '" + num.ToString() + "'";
        try
        {
          dataLayer.WriteData(strSQL5, CommandType.Text);
        }
        catch (Exception ex)
        {
          ErroreMSG = "Richiesta già inviata per l'utente corrente.";
          return this.getCartaEnapia(username);
        }
        string str6 = dataTable2.Rows[0]["DB"].ToString();
        string str7 = "INSERT INTO DBUNICONET.RICFINBPS (ID, DB,KEYWEB,MAT,CODPOS, RAGSOC, COG, NOM, CODFIS,DATNAS,SES,DENCOMNAS, SIGPRONAS, " + " IND,CAP,DENLOC,SIGPRO,ESTINTO,UTEAGG,ULTAGG) VALUES ('" + str3 + "', '" + str6 + "', '" + str4 + "', '" + str1 + "', '" + int32.ToString() + "', '" + str2 + "', '" + nome + "', '" + cognome + "'";
        string str8 = !(dataTable2.Rows[0]["CODFIS"].ToString().Trim() != string.Empty) ? str7 + "NULL, " : str7 + dataTable2.Rows[0]["CODFIS"].ToString() + ",";
        string str9 = !(dataTable2.Rows[0]["DATNAS"].ToString().Trim() != string.Empty) ? str8 + "NULL, " : str8 + Convert.ToDateTime(dataTable2.Rows[0]["DATNAS"].ToString()).ToString() + ",";
        string str10 = !(dataTable2.Rows[0]["SES"].ToString().Trim() != string.Empty) ? str9 + "NULL, " : str9 + dataTable2.Rows[0]["SES"].ToString().Trim().Replace("'", "''") + ",";
        string str11 = !(dataTable2.Rows[0]["DENCOMNAS"].ToString().Trim() != string.Empty) ? str10 + "NULL, " : str10 + dataTable2.Rows[0]["DENCOMNAS"].ToString().Trim().Replace("'", "''") + ",";
        string str12 = !(dataTable2.Rows[0]["SIGPRONAS"].ToString().Trim() != string.Empty) ? str11 + "NULL, " : str11 + dataTable2.Rows[0]["SIGPRONAS"].ToString().Trim().Replace("'", "''") + ",";
        string str13 = !(dataTable2.Rows[0]["IND"].ToString().Trim() != string.Empty) ? str12 + "NULL, " : str12 + dataTable2.Rows[0]["IND"].ToString().Trim().Replace("'", "''") + ",";
        string str14 = !(dataTable2.Rows[0]["CAP"].ToString().Trim() != string.Empty) ? str13 + "NULL, " : str13 + dataTable2.Rows[0]["CAP"].ToString().Trim().Replace("'", "''") + ",";
        string str15 = !(dataTable2.Rows[0]["DENLOC"].ToString().Trim() != string.Empty) ? str14 + "NULL, " : str14 + dataTable2.Rows[0]["DENLOC"].ToString().Trim().Replace("'", "''") + ",";
        string strSQL6 = (!(dataTable2.Rows[0]["SIGPRO"].ToString().Trim() != string.Empty) ? str15 + "NULL, " : str15 + dataTable2.Rows[0]["SIGPRO"].ToString().Trim().Replace("'", "''") + ",") + "'N', 'UTENTE WEB', CURRENT_TIMESTAMP)";
        dictionary.Add("DB", (string) dataTable2.Rows[0]["DB"]);
        dictionary.Add("CODPOS", int32.ToString());
        dictionary.Add("MAT", dataTable2.Rows[0]["MAT"].ToString().Trim());
        dictionary.Add("CODFIS", dataTable2.Rows[0]["CODFIS"].ToString().Trim());
        dictionary.Add("COG", cognome);
        dictionary.Add("NOM", nome);
        dictionary.Add("RAGSOC", str2);
        HttpContext.Current.Session["datiRichiestaFinanziamento"] = (object) dictionary;
        try
        {
          dataLayer.WriteData(strSQL6, CommandType.Text);
          SuccessMSG = "Richiesta inviata con successo!";
        }
        catch (Exception ex)
        {
          ErroreMSG = "Richiesta non inviata correttamente";
          return this.getCartaEnapia(username);
        }
        return this.getCartaEnapia(username);
      }
      catch (Exception ex)
      {
        ErroreMSG = "Non è possibile effettuare la richiesta";
        return this.getCartaEnapia(username);
      }
    }
  }
}
