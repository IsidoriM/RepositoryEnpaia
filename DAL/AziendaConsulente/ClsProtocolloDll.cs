// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.ClsProtocolloDll
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using iTextSharp.text.pdf;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using TFI.CRYPTO.Crypto;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;

namespace TFI.DAL.AziendaConsulente
{
  public class ClsProtocolloDll
  {
    public string Input_CodUte = "";
    public int Input_IDRegistro;
    public string Input_Oggetto = "";
    public string Input_CodTit = "";
    public string Input_Connection = "";
    public string Input_Verso = "";
    public int Input_IDTipoDocumento;
    public int Input_IDTipoSpedizione;
    public string[] Input_MitDes;
    private string Output_Protocollo;

    public string GeneraProtocolloLettera(
      string codpos,
      string ragsoc,
      string MAT,
      string COG,
      string NOM,
      string CODFIS,
      string tipoMittente,
      string CodTit,
      int idTipoDocumento,
      int idTipoSpedizione,
      string oggetto,
      string user,
      ref string NUMPRO,
      ref string DATPRO,
      string VERSO)
    {
      try
      {
        string ErroreMSG = (string) null;
        this.Input_CodUte = user;
        this.Input_Connection = Cypher.DeCryptPassword(ConfigurationManager.AppSettings.Get("ConnProtocollo").ToString());
        this.Input_IDRegistro = 3137;
        this.Input_CodTit = CodTit;
        this.Input_IDTipoDocumento = idTipoDocumento;
        this.Input_IDTipoSpedizione = idTipoSpedizione;
        this.Input_MitDes = new string[1];
        this.Input_MitDes[0] = tipoMittente + ";" + codpos + ";" + ragsoc + ";" + MAT + ";" + COG + ";" + NOM + ";" + CODFIS;
        this.Input_Oggetto = oggetto;
        this.Input_Verso = VERSO;
        string str = this.Protocolla(ref ErroreMSG, ref NUMPRO, ref DATPRO);
        return string.IsNullOrEmpty(ErroreMSG) ? str : "";
      }
      catch
      {
        throw new Exception("Connessione al protocollo non valida");
      }
    }

    public string Protocolla(ref string ErroreMSG, ref string NUMPRO, ref string DATPRO)
    {
      try
      {
        if (string.IsNullOrEmpty(this.Input_Connection.Trim()))
        {
          ErroreMSG = "Valorizzare Input_Connection";
          return Convert.ToString(false);
        }
        if (string.IsNullOrEmpty(this.Input_CodUte.Trim()))
        {
          ErroreMSG = "Valorizzare Input_CodUte";
          return Convert.ToString(false);
        }
        if (this.Input_IDRegistro == 0)
        {
          ErroreMSG = "Valorizzare Input_IDRegistro";
          return Convert.ToString(false);
        }
        if (string.IsNullOrEmpty(this.Input_Verso.Trim()))
        {
          ErroreMSG = "Valorizzare Input_Verso";
          return Convert.ToString(false);
        }
        if (!(this.Input_Verso.Trim().ToUpper() == "A" | this.Input_Verso.Trim().ToUpper() == "P"))
        {
          ErroreMSG = "Valorizzare Input_Verso con A oppure P";
          return Convert.ToString(false);
        }
        if (string.IsNullOrEmpty(this.Input_Oggetto.Trim()))
        {
          ErroreMSG = "Valorizzare Input_Oggetto";
          return Convert.ToString(false);
        }
        if (string.IsNullOrEmpty(this.Input_CodTit.Trim()))
        {
          ErroreMSG = "Valorizzare Input_CodTit";
          return Convert.ToString(false);
        }
        if (this.Input_MitDes.Length == 0)
        {
          ErroreMSG = "Valorizzare Input_MitDes_RagSoc() o Input_MitDes_Cog";
          return Convert.ToString(false);
        }
        if (this.Input_Verso == "A" & this.Input_MitDes.Length - 1 > 0)
        {
          ErroreMSG = "Troppi elementi in Input_MitDes_RagSoc()";
          return Convert.ToString(false);
        }
        ErroreMSG = this.Protocollo(ref NUMPRO, ref DATPRO);
        return !string.IsNullOrEmpty(ErroreMSG) ? "" : this.Output_Protocollo;
      }
      catch (Exception ex)
      {
        ErroreMSG = ex.Message;
      }
      return (string) null;
    }

    private string Protocollo(ref string NUMPRO, ref string DATPRO)
    {
      SqlConnection sqlConnection = new SqlConnection(this.Input_Connection);
      SqlTransaction sqlTransaction = (SqlTransaction) null;
      SqlCommand sqlCommand = new SqlCommand();
      SqlDataAdapter objDataAdapter = new SqlDataAdapter();
      string str1 = "";
      string str2 = "";
      try
      {
        sqlConnection.Open();
        sqlTransaction = sqlConnection.BeginTransaction();
        sqlCommand.CommandType = CommandType.Text;
        sqlCommand.Connection = sqlConnection;
        sqlCommand.Transaction = sqlTransaction;
        string str3 = Convert.ToString(this.SqlClient_GetFieldFirstValue("Select id from tbtitolario where registro=" + this.Input_IDRegistro.ToString() + " and CodTit=" + DBMethods.DoublePeakForSql(this.Input_CodTit), CommandType.Text, "", ref sqlConnection, ref sqlCommand));
        if (str3 == null)
          return "Attenzione... il titolario non appartiene al registro selezionato";
        if (Convert.ToBoolean((object) (this.SqlClient_GetFieldFirstValue("select count(*) from tbtitolario where registro=" + this.Input_IDRegistro.ToString() + " and id=" + str3, CommandType.Text, "", ref sqlConnection, ref sqlCommand), 0, false)))
        {
          sqlTransaction.Rollback();
          sqlConnection.Close();
          return "Attenzione... il titolario non appartiene al registro selezionato";
        }
        int int32_1 = Convert.ToInt32(this.SqlClient_GetFieldFirstValue("SELECT ISNULL(MAX(NUMERO), 0) + 1 FROM TBPROTOCOLLI " + " WHERE " + " ANNO = YEAR(GETDATE()) " + " AND IDREGISTRO = " + this.Input_IDRegistro.ToString(), CommandType.Text, "", ref sqlConnection, ref sqlCommand));
        string str4 = "Insert Into tbProtocolli " + " (" + " IDREGISTRO, VERSO, ANNO, NUMERO, ETICHETTA, DATA, " + " IDTIPODOCUMENTO, IDTIPOSPEDIZIONE, OGGETTO, " + " IDTITOLARIO, CODTIT, " + " CODUTEtitolario, DENUTEtitolario, " + " UTEAGG, ULTAGG) " + " VALUES (" + this.Input_IDRegistro.ToString() + ", " + " '" + this.Input_Verso + "', " + " YEAR(GETDATE()), " + int32_1.ToString() + ", 'ETICHETTA', " + "GETDATE(), " + this.Input_IDTipoDocumento.ToString() + ", " + this.Input_IDTipoSpedizione.ToString() + ", " + DBMethods.DoublePeakForSql(this.Input_Oggetto) + ", " + str3 + ", " + DBMethods.DoublePeakForSql(this.Input_CodTit) + ", " + DBMethods.DoublePeakForSql("") + ", " + DBMethods.DoublePeakForSql("") + ", " + DBMethods.DoublePeakForSql(this.Input_CodUte) + ", " + " CURRENT_TIMESTAMP)";
        sqlCommand.CommandText = str4;
        sqlCommand.ExecuteNonQuery();
        int int32_2 = Convert.ToInt32(this.SqlClient_GetFieldFirstValue("SELECT @@IDENTITY", CommandType.Text, "", ref sqlConnection, ref sqlCommand));
        int num = this.Input_MitDes.Length - 1;
        for (int index = 0; index <= num; ++index)
        {
          string[] strArray = this.Input_MitDes[index].Split(';');
          string str5 = "INSERT INTO TBPROTOCOLLIDETTAGLIO (IDPROTOCOLLO, MITINTERMEDIO, CODICEDIPENDENTE, " + " COGNOME, NOME, CODICEFISCALE, " + " TIPOMITTENTE, CODICEAZIENDA, RAGIONESOCIALE, PARTITAIVA, " + " UTEAGG, ULTAGG) VALUES( " + int32_2.ToString() + ", 'N', " + DBMethods.DoublePeakForSql(strArray[4]) + ", " + DBMethods.DoublePeakForSql(strArray[5]) + ", " + DBMethods.DoublePeakForSql(strArray[6]) + ", " + DBMethods.DoublePeakForSql(strArray[7]) + ", " + DBMethods.DoublePeakForSql(strArray[0]) + ", " + DBMethods.DoublePeakForSql(strArray[1]) + ", " + DBMethods.DoublePeakForSql(strArray[2]) + ", " + DBMethods.DoublePeakForSql(strArray[3]) + ", " + DBMethods.DoublePeakForSql(this.Input_CodUte) + ", " + " CURRENT_TIMESTAMP)";
          sqlCommand.CommandText = str5;
          sqlCommand.ExecuteNonQuery();
          if (!string.IsNullOrEmpty(strArray[2].Trim()))
            str1 = str1 + ", " + strArray[2].Trim();
          if (!string.IsNullOrEmpty(strArray[5].Trim()))
            str1 = str1 + ", " + (strArray[5].Trim() + " " + strArray[6].Trim()).Trim();
          str1 = DBMethods.DoublePeakForSql(str1.Substring(2));
          if (str1.Length > 890)
            str1 = str1.Substring(0, 890);
          string str6 = "UPDATE TBPROTOCOLLI SET MITDES = '" + str1 + "'" + " WHERE IDPROTOCOLLO = " + int32_2.ToString();
          sqlCommand.CommandText = str6;
          sqlCommand.ExecuteNonQuery();
          string str7 = "UPDATE TBPROTOCOLLI SET ETICHETTA = '" + this.GETETICHETTA(ref sqlConnection, ref sqlCommand, int32_2) + "'" + " WHERE IDPROTOCOLLO = " + int32_2.ToString();
          sqlCommand.CommandText = str7;
          sqlCommand.ExecuteNonQuery();
          string str8 = "INSERT INTO TBPROTOCOLLITITOLARI (IDProtocollo, IDTitolarioA, CODTIT_A, DESC_A, CODUTE_A, DENUTE_A, ULTAGG, UTEAGG) " + " VALUES (" + int32_2.ToString() + "," + str3 + ", " + DBMethods.DoublePeakForSql(this.Input_CodTit) + ", " + DBMethods.DoublePeakForSql(str2) + ", " + DBMethods.DoublePeakForSql("") + ", " + DBMethods.DoublePeakForSql("") + ", " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(this.Input_CodUte) + ")";
          sqlCommand.CommandText = str8;
          sqlCommand.ExecuteNonQuery();
        }
        this.SALVA_KEY(ref sqlConnection, ref sqlCommand, int32_2);
        string strSql = "select ETICHETTA, DATA FROM TBPROTOCOLLI WHERE IDPROTOCOLLO=" + int32_2.ToString();
        DataTable dataTable1 = new DataTable();
        DataTable dtTable = (DataTable) null;
        DataTable dataTable2 = this.SqlClient_GetDataTable(strSql, CommandType.Text, "", ref sqlConnection, ref dtTable, ref objDataAdapter, ref sqlCommand);
        this.Output_Protocollo = (int32_2.ToString() + ";" + dataTable2.Rows[0]["ETICHETTA"]?.ToString()).ToString() + ";" + dataTable2.Rows[0]["DATA"].ToString();
        sqlTransaction.Commit();
        sqlConnection.Close();
        NUMPRO = int32_1.ToString();
        DATPRO = Convert.ToString(dataTable2.Rows[0]["DATA"]);
        return "";
      }
      catch (Exception ex)
      {
        if (sqlConnection.State == ConnectionState.Open)
        {
          sqlTransaction.Rollback();
          sqlConnection.Close();
        }
        return ex.Message;
      }
    }

    private void SALVA_KEY(ref SqlConnection CONN, ref SqlCommand CMD, int IDPROTOCOLLO)
    {
      DataTable dtTable = new DataTable();
      string str1 = "DELETE FROM TBPROTOCOLLIKEY WHERE IDPROTOCOLLO=" + IDPROTOCOLLO.ToString();
      CMD.CommandText = str1;
      CMD.ExecuteNonQuery();
      string strSql1 = "SELECT * FROM TBPROTOCOLLI WHERE IDPROTOCOLLO = " + IDPROTOCOLLO.ToString();
      SqlDataAdapter objDataAdapter1 = (SqlDataAdapter) null;
      DataTable dataTable1 = this.SqlClient_GetDataTable(strSql1, CommandType.Text, "", ref CONN, ref dtTable, ref objDataAdapter1, ref CMD);
      int num1 = dataTable1.Columns.Count - 1;
      for (int index = 0; index <= num1; ++index)
      {
        string str2 = dataTable1.Columns[index].ColumnName.ToString().Trim().ToUpper() ?? "";
        if ((str2 == "OGGETTO" || str2 == "PROTOCOLLODOCUMENTO" || str2 == "NUMCORR" || str2 == "MOTIVOANNULLAMENTO") && dataTable1.Rows[0][index].ToString() == "")
        {
          string str3;
          if (dataTable1.Rows[0][index].ToString().Trim().Length > (int) byte.MaxValue)
            str3 = "INSERT INTO TBPROTOCOLLIKEY (IDPROTOCOLLO, CHIAVE) VALUES (" + IDPROTOCOLLO.ToString() + ", " + DBMethods.DoublePeakForSql(dataTable1.Rows[0][index].ToString().Trim().Substring(0, 250)) + ")";
          else
            str3 = "INSERT INTO TBPROTOCOLLIKEY (IDPROTOCOLLO, CHIAVE) VALUES (" + IDPROTOCOLLO.ToString() + ", " + DBMethods.DoublePeakForSql(Convert.ToString(dataTable1.Rows[0][index])) + ")";
          CMD.CommandText = str3;
          CMD.ExecuteNonQuery();
        }
      }
      string strSql2 = "SELECT * FROM TBPROTOCOLLIDETTAGLIO WHERE IDPROTOCOLLO = " + IDPROTOCOLLO.ToString();
      SqlDataAdapter objDataAdapter2 = (SqlDataAdapter) null;
      DataTable dataTable2 = this.SqlClient_GetDataTable(strSql2, CommandType.Text, "", ref CONN, ref dataTable1, ref objDataAdapter2, ref CMD);
      int num2 = dataTable2.Rows.Count - 1;
      for (int index1 = 0; index1 <= num2; ++index1)
      {
        int num3 = dataTable2.Columns.Count - 1;
        for (int index2 = 0; index2 <= num3; ++index2)
        {
          switch (dataTable2.Columns[index2].ColumnName.ToString().Trim().ToUpper() ?? "")
          {
            case "CODICEAZIENDA":
            case "CODICEDIPENDENTE":
            case "CODICEFISCALE":
            case "COGNOME":
            case "EMAIL":
            case "FAX":
            case "INDIRIZZO":
            case "NOME":
            case "PARTITAIVA":
            case "RAGIONESOCIALE":
            case "TELEFONO":
              if (dataTable2.Rows[index1][index2].ToString() == "")
              {
                string str4 = "INSERT INTO TBPROTOCOLLIKEY (IDPROTOCOLLO, CHIAVE) VALUES (" + IDPROTOCOLLO.ToString() + ", " + DBMethods.DoublePeakForSql(Convert.ToString(dataTable2.Rows[index1][index2])) + ")";
                CMD.CommandText = str4;
                CMD.ExecuteNonQuery();
                break;
              }
              break;
          }
        }
      }
      string strSql3 = "SELECT * FROM TBPROTOCOLLIMEMO WHERE IDPROTOCOLLO = " + IDPROTOCOLLO.ToString();
      SqlDataAdapter objDataAdapter3 = (SqlDataAdapter) null;
      DataTable dataTable3 = this.SqlClient_GetDataTable(strSql3, CommandType.Text, "", ref CONN, ref dataTable2, ref objDataAdapter3, ref CMD);
      int num4 = dataTable3.Rows.Count - 1;
      for (int index3 = 0; index3 <= num4; ++index3)
      {
        int num5 = dataTable3.Columns.Count - 1;
        for (int index4 = 0; index4 <= num5; ++index4)
        {
          if ((dataTable3.Columns[index4].ColumnName.ToString().Trim().ToUpper() ?? "") == "MEMO" && dataTable3.Rows[index3][index4].ToString() == " ")
          {
            string str5 = "INSERT INTO TBPROTOCOLLIKEY (IDPROTOCOLLO, CHIAVE) VALUES (" + IDPROTOCOLLO.ToString() + ", " + DBMethods.DoublePeakForSql(Convert.ToString(dataTable3.Rows[index3][index4])) + ")";
            CMD.CommandText = str5;
            CMD.ExecuteNonQuery();
          }
        }
      }
      string strSql4 = "SELECT * FROM TBPROTOCOLLIALLEGATI WHERE IDPROTOCOLLO = " + IDPROTOCOLLO.ToString() + " AND DATANN IS NULL";
      SqlDataAdapter objDataAdapter4 = (SqlDataAdapter) null;
      DataTable dataTable4 = this.SqlClient_GetDataTable(strSql4, CommandType.Text, "", ref CONN, ref dataTable3, ref objDataAdapter4, ref CMD);
      int num6 = dataTable4.Rows.Count - 1;
      for (int index5 = 0; index5 <= num6; ++index5)
      {
        int num7 = dataTable4.Columns.Count - 1;
        for (int index6 = 0; index6 <= num7; ++index6)
        {
          switch (dataTable4.Columns[index6].ColumnName.ToString().Trim().ToUpper() ?? "")
          {
            case "CODFIS":
            case "CODPOS":
            case "COGNOME":
            case "MAT":
            case "NOME":
            case "OGGETTO":
            case "PARIVA":
            case "RAGSOC":
              if (dataTable4.Rows[index5][index6].ToString() == "")
              {
                string str6 = "INSERT INTO TBPROTOCOLLIKEY (IDPROTOCOLLO, CHIAVE) VALUES (" + IDPROTOCOLLO.ToString() + ", " + DBMethods.DoublePeakForSql(Convert.ToString(dataTable4.Rows[index5][index6])) + ")";
                CMD.CommandText = str6;
                CMD.ExecuteNonQuery();
                break;
              }
              break;
          }
        }
      }
    }

    public DataTable SqlClient_GetDataTable(
      string strSql,
      CommandType TypeCmd,
      string connectionString,
      ref SqlConnection objConnection,
      ref DataTable dtTable,
      ref SqlDataAdapter objDataAdapter,
      ref SqlCommand objCommand)
    {
      DataTable dataTable1 = (DataTable) null;
      SqlConnection sqlConnection;
      if (string.IsNullOrEmpty(connectionString))
      {
        sqlConnection = objConnection;
      }
      else
      {
        sqlConnection = new SqlConnection(connectionString);
        sqlConnection.Open();
      }
      DataTable dataTable2 = dataTable1 != null ? dtTable : new DataTable();
      SqlCommand sqlCommand = objCommand != null ? objCommand : new SqlCommand();
      sqlCommand.CommandType = TypeCmd;
      sqlCommand.CommandText = strSql;
      sqlCommand.Connection = sqlConnection;
      sqlCommand.CommandTimeout = 0;
      SqlDataAdapter sqlDataAdapter = objDataAdapter != null ? objDataAdapter : new SqlDataAdapter();
      sqlDataAdapter.SelectCommand = sqlCommand;
      sqlDataAdapter.Fill(dataTable2);
      if (!string.IsNullOrEmpty(connectionString))
        sqlConnection.Close();
      return dataTable2;
    }

    private string GETETICHETTA(ref SqlConnection CONN, ref SqlCommand CMD, int IDPROTOCOLLO)
    {
      DataTable dtTable = new DataTable();
      string strSql = "SELECT NUMERO, ANNO, CODTIT, DATA, (SELECT CODREGISTRO FROM TBTIPOREGISTRO WHERE IDREGISTRO=TBPROTOCOLLI.IDREGISTRO) AS CODREGISTRO, VERSO FROM TBPROTOCOLLI WHERE IDPROTOCOLLO=" + IDPROTOCOLLO.ToString();
      SqlDataAdapter objDataAdapter = (SqlDataAdapter) null;
      DataTable dataTable = this.SqlClient_GetDataTable(strSql, CommandType.Text, "", ref CONN, ref dtTable, ref objDataAdapter, ref CMD);
      return !(dataTable.Rows[0]["VERSO"].ToString().Trim() == "I") ? ((dataTable.Rows[0]["VERSO"], "/"), (dataTable.Rows[0]["NUMERO"], "/"), (dataTable.Rows[0]["ANNO"], "/"), dataTable.Rows[0]["CODTIT"]).ToString().Trim() : Convert.ToString((object) ("SNP/", dataTable.Rows[0]["ANNO"]));
    }

    private string GET_DATA_FORSQL(DateTime DATA, bool CON_ORA_23_59)
    {
      string str = DATA.Year.ToString() + "-" + DATA.Month.ToString().Trim().PadLeft(2, '0') + "-" + DATA.Day.ToString().Trim().PadLeft(2, '0');
      return "CONVERT(DATETIME,'" + (CON_ORA_23_59 ? str + " 23:59:59" : str + " 00:00:00") + "',102)";
    }

    public object SqlClient_GetFieldFirstValue(
      string strSql,
      CommandType TypeCmd,
      string connectionString,
      ref SqlConnection objConnection,
      ref SqlCommand objCommand)
    {
      SqlConnection sqlConnection;
      if (string.IsNullOrEmpty(connectionString))
      {
        sqlConnection = objConnection;
      }
      else
      {
        sqlConnection = new SqlConnection(connectionString);
        sqlConnection.Open();
      }
      SqlCommand sqlCommand = objCommand != null ? objCommand : new SqlCommand();
      sqlCommand.CommandType = TypeCmd;
      sqlCommand.CommandText = strSql;
      sqlCommand.Connection = sqlConnection;
      sqlCommand.CommandTimeout = 0;
      object fieldFirstValue = sqlCommand.ExecuteScalar();
      if (!string.IsNullOrEmpty(connectionString))
        sqlConnection.Close();
      return fieldFirstValue;
    }

    private int GET_IDALLEGATO()
    {
      SqlConnection objConnection = new SqlConnection(this.Input_Connection);
      SqlTransaction sqlTransaction = (SqlTransaction) null;
      SqlCommand objCommand = new SqlCommand();
      try
      {
        objConnection.Open();
        sqlTransaction = objConnection.BeginTransaction();
        objCommand.CommandType = CommandType.Text;
        objCommand.Connection = objConnection;
        objCommand.Transaction = sqlTransaction;
        string str = "Insert Into tbALLEGATI (SOTTOCARTELLA, NOMEFILE, NOMEFILEORIGINALE, ULTAGG, UTEAGG)" + " VALUES(" + "'SOTTOCARTELLA', " + "'SCANSIONE', " + "'SCANSIONE', " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(this.Input_CodUte) + ")";
        objCommand.CommandText = str;
        objCommand.ExecuteNonQuery();
        int int32 = Convert.ToInt32(this.SqlClient_GetFieldFirstValue("SELECT @@IDENTITY", CommandType.Text, "", ref objConnection, ref objCommand));
        sqlTransaction.Commit();
        objConnection.Close();
        return int32;
      }
      catch (Exception ex)
      {
        sqlTransaction.Rollback();
        objConnection.Close();
        return 0;
      }
    }

    private string PROT_PRES_Module_GetDataSistema(ref object Conn, string strConn, ref object cmd)
    {
      SqlConnection objConnection = (SqlConnection) Conn;
      SqlCommand objCommand = (SqlCommand) cmd;
      return Convert.ToString(this.SqlClient_GetFieldFirstValue("SELECT GETDATE() AS DATASISTEMA", CommandType.Text, strConn, ref objConnection, ref objCommand));
    }

    private string GETMESE(int NUMMESE)
    {
      switch (NUMMESE)
      {
        case 1:
          return "GENNAIO";
        case 2:
          return "FEBBRAIO";
        case 3:
          return "MARZO";
        case 4:
          return "APRILE";
        case 5:
          return "MAGGIO";
        case 6:
          return "GIUGNO";
        case 7:
          return "LUGLIO";
        case 8:
          return "AGOSTO";
        case 9:
          return "SETTEMBRE";
        case 10:
          return "OTTOBRE";
        case 11:
          return "NOVEMBRE";
        case 12:
          return "DICEMBRE";
        default:
          return (string) null;
      }
    }

    public string PROT_PRES_GET_TEMP_PATH_APPLICATION()
    {
      string str = Path.GetTempPath();
      if (str.Substring(str.Length - 1) == "\\")
        str = str.Substring(0, str.Length - 1);
      string path = str + "\\contributi_new_version" + "\\tmp";
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      return path;
    }

    public bool Module_AllegaLetteraProtocollo(
      string path,
      string Protocollo_Data,
      ref string numProt)
    {
      string[] strArray = Protocollo_Data.Split(';');
      int int32 = Convert.ToInt32(strArray[0]);
      numProt = strArray[1];
      return string.IsNullOrEmpty(this.AllegaFile(int32, path));
    }

    public string AllegaFile(int IDPROTOCOLLO, string strNomeFile)
    {
      int idallegato = this.GET_IDALLEGATO();
      if (idallegato == 0)
        return "Impossibile ottenere un IDALLEGATO dal sistema";
      string extension = Path.GetExtension(strNomeFile);
      string fileName = Path.GetFileName(strNomeFile);
      string str1 = idallegato.ToString().Trim().PadLeft(9, '0') + extension.ToLower();
      SqlCommand objCommand = new SqlCommand();
      SqlConnection objConnection = new SqlConnection(this.Input_Connection);
      SqlTransaction sqlTransaction = (SqlTransaction) null;
      DataTable dtTable = new DataTable();
      int index1 = 0;
      try
      {
        objConnection.Open();
        sqlTransaction = objConnection.BeginTransaction();
        objCommand.Connection = objConnection;
        objCommand.Transaction = sqlTransaction;
        object Conn = (object) objConnection;
        object cmd = (object) objCommand;
        string dataSistema = this.PROT_PRES_Module_GetDataSistema(ref Conn, "", ref cmd);
        string[] strArray = new string[6]
        {
          Convert.ToDateTime(dataSistema).Year.ToString(),
          "/",
          null,
          null,
          null,
          null
        };
        DateTime dateTime = Convert.ToDateTime(dataSistema);
        strArray[2] = this.GETMESE(dateTime.Month);
        strArray[3] = "/";
        dateTime = Convert.ToDateTime(dataSistema);
        strArray[4] = dateTime.Day.ToString().PadLeft(2, '0');
        strArray[5] = "/";
        string str2 = "UPDATE TBALLEGATI SET SOTTOCARTELLA = '" + string.Concat(strArray) + "', " + " NOMEFILE = '" + str1 + "', " + " NOMEFILEORIGINALE = '" + fileName + "'" + " WHERE IDALLEGATO = " + idallegato.ToString();
        objCommand.CommandText = str2;
        objCommand.ExecuteNonQuery();
        if (Convert.ToBoolean((object) (this.SqlClient_GetFieldFirstValue("SELECT COUNT(*) " + " FROM TBPROTOCOLLIDETTAGLIO A, TBPROTOCOLLI B WHERE A.IDPROTOCOLLO=" + IDPROTOCOLLO.ToString() + " AND A.IDPROTOCOLLO=B.IDPROTOCOLLO " + " AND (A.CODICEAZIENDA IS NOT NULL OR ISNULL(A.PARTITAIVA, 'XXXXXXXXXXX') <> 'XXXXXXXXXXX' OR A.CODICEDIPENDENTE IS NOT NULL OR ISNULL(A.CODICEFISCALE, 'XXXXXXXXXXXXXXXX')<>'XXXXXXXXXXXXXXXX' )", CommandType.Text, "", ref objConnection, ref objCommand), 0, false)))
        {
          string strSql = "SELECT * " + " FROM TBPROTOCOLLI B WHERE B.IDPROTOCOLLO=" + IDPROTOCOLLO.ToString();
          SqlDataAdapter objDataAdapter = (SqlDataAdapter) null;
          dtTable = this.SqlClient_GetDataTable(strSql, CommandType.Text, "", ref objConnection, ref dtTable, ref objDataAdapter, ref objCommand);
          string str3 = "INSERT INTO TBPROTOCOLLIALLEGATI (IDPROTOCOLLO, IDALLEGATO, IDTIPODOCUMENTO, IDTIT, " + "TIPOMITTENTE, RAGSOC, " + " ULTAGG, UTEAGG) " + "VALUES(" + IDPROTOCOLLO.ToString() + ", " + idallegato.ToString() + ", " + DBMethods.DoublePeakForSql(dtTable.Rows[index1]["IDTIPODOCUMENTO"].ToString().Trim()) + ", " + DBMethods.DoublePeakForSql(dtTable.Rows[index1]["IDTITOLARIO"].ToString().Trim()) + ", " + DBMethods.DoublePeakForSql("ALTRO") + ", " + DBMethods.DoublePeakForSql(dtTable.Rows[index1]["MITDES"].ToString().Trim()) + ", " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(this.Input_CodUte) + ")";
          objCommand.CommandText = str3;
          objCommand.ExecuteNonQuery();
        }
        else
        {
          string strSql = "SELECT A.CODICEDIPENDENTE AS MAT, A.CODICEFISCALE AS CODFIS, " + " A.COGNOME, A.NOME, A. CODICEAZIENDA AS CODPOS, A.RAGIONESOCIALE AS RAGSOC, A.PARTITAIVA AS PARIVA, A.TIPOMITTENTE, B.IDTITOLARIO, B.IDTIPODOCUMENTO " + " FROM TBPROTOCOLLIDETTAGLIO A, TBPROTOCOLLI B WHERE A.IDPROTOCOLLO=" + IDPROTOCOLLO.ToString() + " AND A.IDPROTOCOLLO=B.IDPROTOCOLLO ";
          SqlDataAdapter objDataAdapter = (SqlDataAdapter) null;
          DataTable dataTable = this.SqlClient_GetDataTable(strSql, CommandType.Text, "", ref objConnection, ref dtTable, ref objDataAdapter, ref objCommand);
          int num = dataTable.Rows.Count - 1;
          for (int index2 = 0; index2 <= num; ++index2)
          {
            string str4 = "INSERT INTO TBPROTOCOLLIALLEGATI (IDPROTOCOLLO, IDALLEGATO, IDTIPODOCUMENTO, IDTIT, " + "TIPOMITTENTE, CODPOS, RAGSOC, PARIVA, MAT, COGNOME, NOME, CODFIS, " + " ULTAGG, UTEAGG) " + "VALUES(" + IDPROTOCOLLO.ToString() + ", " + idallegato.ToString() + ", " + DBMethods.DoublePeakForSql(dataTable.Rows[index2]["IDTIPODOCUMENTO"].ToString().Trim()) + ", " + DBMethods.DoublePeakForSql(dataTable.Rows[index2]["IDTITOLARIO"].ToString().Trim()) + ", " + DBMethods.DoublePeakForSql(dataTable.Rows[index2]["TIPOMITTENTE"].ToString().Trim()) + ", " + DBMethods.DoublePeakForSql(dataTable.Rows[index2]["CODPOS"].ToString().Trim()) + ", " + DBMethods.DoublePeakForSql(dataTable.Rows[index2]["RAGSOC"].ToString().Trim()) + ", " + DBMethods.DoublePeakForSql(dataTable.Rows[index2]["PARIVA"].ToString().Trim()) + ", " + DBMethods.DoublePeakForSql(dataTable.Rows[index2]["MAT"].ToString().Trim()) + ", " + DBMethods.DoublePeakForSql(dataTable.Rows[index2]["COGNOME"].ToString().Trim()) + ", " + DBMethods.DoublePeakForSql(dataTable.Rows[index2]["NOME"].ToString().Trim()) + ", " + DBMethods.DoublePeakForSql(dataTable.Rows[index2]["CODFIS"].ToString().Trim()) + ", " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(this.Input_CodUte) + ")";
            objCommand.CommandText = str4;
            objCommand.ExecuteNonQuery();
          }
        }
        sqlTransaction.Commit();
        objConnection.Close();
        return "";
      }
      catch (Exception ex)
      {
        if (objConnection.State == ConnectionState.Open)
        {
          sqlTransaction.Rollback();
          objConnection.Close();
        }
        this.CANCELLA_ALLEGATO(idallegato);
        return ex.Message;
      }
    }

    private void CANCELLA_ALLEGATO(int IDALLEGATO)
    {
      SqlConnection sqlConnection = new SqlConnection(this.Input_Connection);
      SqlCommand sqlCommand = new SqlCommand();
      sqlConnection.Open();
      sqlCommand.CommandText = "DELETE FROM TBALLEGATI WHERE IDALLEGATO = " + IDALLEGATO.ToString();
      sqlCommand.CommandType = CommandType.Text;
      sqlCommand.Connection = sqlConnection;
      sqlCommand.ExecuteNonQuery();
      sqlConnection.Close();
    }

    public DataTable Genera_Notifiche(
      ref DataTable dtAziende,
      ref DataTable dtLOG,
      string TIPMOVSAN,
      string DATA_RIFERIMENTO,
      string STRRICPREV = "",
      int OPTIONAL_MATRICOLA = 0,
      int OPTIONAL_PRORAP = 0,
      bool PREV = false)
    {
      int num1 = 0;
      int num2 = 0;
      Decimal num3 = 0.0M;
      Decimal num4 = 0.0M;
      Decimal num5 = 0.0M;
      int index1 = 1;
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      DataTable dataTable4 = new DataTable();
      DataTable dataTable5 = new DataTable();
      DataTable dataTable6 = new DataTable();
      int PROLOC = 0;
      string str1 = (string) null;
      Decimal num6 = 0.0M;
      int num7 = 0;
      int num8 = 0;
      DataTable dataTable7 = new DataTable();
      DataTable dataTable8 = new DataTable();
      DataTable dataTable9 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      string str2 = !string.IsNullOrEmpty(DATA_RIFERIMENTO) ? DATA_RIFERIMENTO : DateTime.Now.ToString();
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "CODPOS"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "ANNDEN"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "MESDEN"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "MAT"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "NOME"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "PRORAP"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "DATNAS"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "DATDEC"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "DATCES"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "DAL"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "AL"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "ETAENNE"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "QUALIFICA"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "ALIQUOTA"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "DATA_INIZIO_STORDL"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "DATA_FINE_STORDL"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "TIPO_RAPPORTO"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "TIPRAP"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "CODQUACON"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "CODCON"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "PROCON"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "CODLOC"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "PROLOC"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "CODLIV"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "FAP"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "CODGRUASS"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "DENLIV"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPRET"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "PERPAR"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "PERAPP"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "PERFAP"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "NUMGGAZI"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "NUMGGFIG"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "NUMGGDOM"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "NUMGGSOS"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "NUMGGCONAZI"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "NUMGGPER"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPFIG"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPABB"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "ABBPRE"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "ASSCON"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPADDREC"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPASSCON"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPCON"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPMIN"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPSCA"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPTRAECO"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "TIPSPE"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = nameof (PREV)
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "GIORNI_MESE"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPOCC"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "RIMUOVI"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "NUMMEN"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "MESMEN14"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "MESMEN15"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "MESMEN16"
      });
      dataTable8.Columns.Add(new DataColumn()
      {
        ColumnName = "ANNO"
      });
      dataTable8.Columns.Add(new DataColumn()
      {
        ColumnName = "MESE"
      });
      string strSQL1 = " SELECT CODPOS, MAT, PRORAP, DATINISOS, DATFINSOS, PERAZI, PERFIG FROM SOSRAP" + " WHERE CODPOS = -1";
      DataTable dataTable10 = dataLayer.GetDataTable(strSQL1);
      int num9 = dtAziende.Rows.Count - 1;
      DateTime dateTime;
      for (int index2 = 0; index2 <= num9; ++index2)
      {
        int int32_1 = Convert.ToInt32(dtAziende.Rows[index2]["ANNO"]);
        int int32_2 = Convert.ToInt32(dtAziende.Rows[index2]["MESE"]);
        dataTable8.Rows.Add(dataTable8.NewRow());
        dataTable8.Rows[dataTable8.Rows.Count - 1]["ANNO"] = (object) int32_1;
        dataTable8.Rows[dataTable8.Rows.Count - 1]["MESE"] = (object) int32_2;
        dateTime = Convert.ToDateTime("01/" + int32_2.ToString() + "/" + int32_1.ToString());
        string str3 = dateTime.ToString().Trim();
        dateTime = Convert.ToDateTime(DateTime.DaysInMonth(int32_1, int32_2).ToString() + "/" + int32_2.ToString() + "/" + int32_1.ToString());
        string str4 = dateTime.ToString().Trim();
        int num10 = DateTime.DaysInMonth(int32_1, int32_2);
        string strSQL2 = "SELECT VALORE FROM PARGENDET WHERE CODPAR = 3" + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " BETWEEN DATINI AND DATFIN";
        string str5 = dataLayer.Get1ValueFromSQL(strSQL2, CommandType.Text);
        dtAziende.Rows[index2]["DATSCA"] = (object) str5;
        string str6 = "SELECT TRIM(COG) || ' ' || TRIM(NOM) AS NOME , DATNAS, RAPLAV.CODPOS, " + " RAPLAV.MAT, RAPLAV.PRORAP, RAPLAV.DATDEC, RAPLAV.DATCES " + " FROM ISCT, RAPLAV " + " WHERE ISCT.MAT=RAPLAV.MAT " + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " <= VALUE(RAPLAV.DATCES , '9999-12-31') " + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str4)) + " >=RAPLAV.DATDEC " + " AND RAPLAV.CODPOS=" + dtAziende.Rows[index2]["CODPOS"]?.ToString();
        if (OPTIONAL_MATRICOLA != 0)
          str6 = str6 + " AND RAPLAV.MAT = " + OPTIONAL_MATRICOLA.ToString() + " AND RAPLAV.PRORAP = " + OPTIONAL_PRORAP.ToString();
        string strSQL3 = str6 + " ORDER BY NOME, MAT";
        dataTable4.Rows.Clear();
        dataTable4 = dataLayer.GetDataTable(strSQL3);
        if (dataTable4.Rows.Count > 0)
        {
          string str7 = " SELECT CODPOS, MAT, PRORAP, DATINISOS, DATFINSOS, PERAZI, PERFIG FROM SOSRAP" + " WHERE CODPOS = " + dtAziende.Rows[index2]["CODPOS"]?.ToString();
          if (OPTIONAL_MATRICOLA != 0)
            str7 = str7 + " AND MAT = " + OPTIONAL_MATRICOLA.ToString() + " AND PRORAP = " + OPTIONAL_PRORAP.ToString();
          string strSQL4 = str7 + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " <= VALUE(DATFINSOS , '9999-12-31') " + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str4)) + " >=DATINISOS " + " AND STASOS = '0'" + " ORDER BY MAT, PRORAP, DATINISOS";
          dataTable6.Clear();
          dataTable6 = dataLayer.GetDataTable(strSQL4);
          int num11 = dataTable6.Rows.Count - 1;
          for (index1 = 0; index1 <= num11; ++index1)
          {
            DataRow row = dataTable10.NewRow();
            int num12 = dataTable6.Columns.Count - 1;
            for (int columnIndex = 0; columnIndex <= num12; ++columnIndex)
              row[columnIndex] = dataTable6.Rows[index1][columnIndex];
            dataTable10.Rows.Add(row);
          }
          int num13 = dataTable4.Rows.Count - 1;
          for (int index3 = 0; index3 <= num13; ++index3)
          {
            int int32_3 = Convert.ToInt32(dataTable4.Rows[index3]["CODPOS"]);
            int int32_4 = Convert.ToInt32(dataTable4.Rows[index3]["MAT"]);
            int int32_5 = Convert.ToInt32(dataTable4.Rows[index3]["PRORAP"]);
            string str8 = dataTable4.Rows[index3]["NOME"].ToString();
            if (dataTable4.Rows[index3]["DATNAS"].ToString() == " ")
            {
              dateTime = Convert.ToDateTime(Convert.ToDateTime(dataTable4.Rows[index3]["DATNAS"]));
              string str9 = dateTime.ToString().Trim();
              dateTime = Convert.ToDateTime(str9);
              dateTime = Convert.ToDateTime(dateTime.AddYears(65));
              string s1 = dateTime.ToString().Trim();
              if (dataTable4.Rows[index3]["DATDEC"].ToString() == "")
              {
                dateTime = Convert.ToDateTime(dataTable4.Rows[index3]["DATDEC"]);
                string str10 = dateTime.ToString();
                string str11;
                if (dataTable4.Rows[index3]["DATCES"].ToString() == "")
                {
                  dateTime = Convert.ToDateTime(dataTable4.Rows[index3]["DATCES"]);
                  str11 = dateTime.ToString().Substring(0, 10);
                }
                else
                  str11 = "";
                string strSQL5 = " SELECT CODPOS, MAT, FAP, PRORAP,TIPRAP, CODGRUASS, VALUE(IMPSCAMAT,0.00) AS IMPSCAMAT, DATINI, DATFIN, VALUE(CODLOC, 0) AS CODLOC, VALUE(CODCON, 0) AS CODCON, " + " (" + " SELECT DENTIPRAP FROM TIPRAP WHERE TIPRAP = STORDL.TIPRAP " + " ) AS RAPPORTO, CODLIV, " + " '' AS DENLIV, " + " 0 AS CODQUACON," + " 0 AS PROCON," + " 0 AS PROLOC, " + " VALUE(TRAECO, 0.00) AS TRAECO, " + " VALUE(PERAPP, 0.00) AS PERAPP, " + " VALUE(PERPAR, 0.00) AS PERPAR, " + " VALUE(ABBPRE, 'N') AS ABBPRE, " + " VALUE(ASSCON, 'N') AS ASSCON, " + " VALUE(NUMMEN, 12) AS NUMMEN, " + " VALUE(MESMEN14, 14) AS MESMEN14, " + " VALUE(MESMEN15, 15) AS MESMEN15, " + " VALUE(MESMEN16, 16) AS MESMEN16 " + " FROM STORDL " + " WHERE CODPOS = " + dataTable4.Rows[index3]["CODPOS"]?.ToString() + " AND MAT = " + dataTable4.Rows[index3]["MAT"]?.ToString() + " AND PRORAP = " + dataTable4.Rows[index3]["PRORAP"]?.ToString() + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str3)) + " <= VALUE(DATFIN , '9999-12-31') " + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str4)) + " >= DATINI ORDER BY MAT, DATINI";
                dataTable2.Clear();
                dataTable2 = dataLayer.GetDataTable(strSQL5);
                if (dataTable2.Rows.Count == 0)
                {
                  dtLOG.Rows.Add(dtLOG.NewRow());
                  dtLOG.Rows[dtLOG.Rows.Count - 1]["CODPOS"] = (object) int32_3;
                  dtLOG.Rows[dtLOG.Rows.Count - 1]["ANNDEN"] = (object) int32_1;
                  dtLOG.Rows[dtLOG.Rows.Count - 1]["MESDEN"] = (object) int32_2;
                  dtLOG.Rows[dtLOG.Rows.Count - 1]["MAT"] = (object) int32_4;
                  dtLOG.Rows[dtLOG.Rows.Count - 1]["DESERR"] = (object) "TRATTAMENTI ECONOMICI INESISTENTI";
                }
                else
                {
                  int num14 = dataTable2.Rows.Count - 1;
                  for (int index4 = 0; index4 <= num14; ++index4)
                  {
                    dateTime = Convert.ToDateTime(dataTable2.Rows[index4]["DATINI"]);
                    dateTime.ToString().Substring(0, 10);
                    dateTime = Convert.ToDateTime(dataTable2.Rows[index4]["DATFIN"]);
                    dateTime.ToString().Substring(0, 10);
                    string str12;
                    if (DateTime.Compare(DateTime.Parse(str3), DateTime.Parse(dataTable2.Rows[index4]["DATINI"].ToString())) >= 0)
                    {
                      dateTime = Convert.ToDateTime(str3);
                      str12 = dateTime.ToString().Substring(0, 10);
                    }
                    else
                    {
                      dateTime = Convert.ToDateTime(dataTable2.Rows[index4]["DATINI"]);
                      str12 = dateTime.ToString().Substring(0, 10);
                    }
                    string s2;
                    if (DateTime.Compare(DateTime.Parse(str4), DateTime.Parse(dataTable2.Rows[index4]["DATFIN"].ToString())) <= 0)
                    {
                      dateTime = Convert.ToDateTime(str4);
                      s2 = dateTime.ToString().Substring(0, 10);
                    }
                    else
                    {
                      dateTime = Convert.ToDateTime(dataTable2.Rows[index4]["DATFIN"]);
                      s2 = dateTime.ToString().Substring(0, 10);
                    }
                    int int32_6 = Convert.ToInt32(dataTable2.Rows[index4]["CODCON"]);
                    int int32_7 = Convert.ToInt32(dataTable2.Rows[index4]["CODLOC"]);
                    string FAP = dataTable2.Rows[index4]["FAP"].ToString();
                    int int32_8 = Convert.ToInt32(dataTable2.Rows[index4]["CODGRUASS"]);
                    Decimal int32_9 = (Decimal) Convert.ToInt32(dataTable2.Rows[index4]["PERAPP"]);
                    Decimal PERAPP = Convert.ToDecimal(dataTable2.Rows[index4]["PERPAR"]);
                    Decimal num15 = Convert.ToDecimal(dataTable2.Rows[index4]["IMPSCAMAT"]);
                    string str13 = dataTable2.Rows[index4]["ABBPRE"].ToString();
                    string str14 = dataTable2.Rows[index4]["ASSCON"].ToString();
                    Decimal num16 = Convert.ToDecimal(dataTable2.Rows[index4]["TRAECO"]);
                    int int32_10 = Convert.ToInt32(dataTable2.Rows[index4]["NUMMEN"]);
                    int int32_11 = Convert.ToInt32(dataTable2.Rows[index4]["MESMEN14"]);
                    int int32_12 = Convert.ToInt32(dataTable2.Rows[index4]["MESMEN15"]);
                    int int32_13 = Convert.ToInt32(dataTable2.Rows[index4]["MESMEN16"]);
                    if (dataTable2.Rows[index4]["TIPRAP"].ToString().Trim() == "")
                    {
                      dtLOG.Rows.Add(dtLOG.NewRow());
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["CODPOS"] = (object) int32_3;
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["ANNDEN"] = (object) int32_1;
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["MESDEN"] = (object) int32_2;
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["MAT"] = (object) int32_4;
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["DESERR"] = (object) "TIPO RAPPORTO NON TROVATO";
                      break;
                    }
                    string str15 = dataTable2.Rows[index4]["TIPRAP"].ToString();
                    if (FAP == "N")
                    {
                      string strSQL6 = "SELECT VALFAP FROM CODFAP WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(str12)) + " " + " BETWEEN DATINI AND  VALUE(DATFIN, '9999-12-31')";
                      num6 = Convert.ToDecimal(dataLayer.Get1ValueFromSQL(strSQL6, CommandType.Text));
                    }
                    else
                      num6 = 0.0M;
                    if (int32_7 != 0)
                      dataTable6 = this.GetDatiContratto_Locale(int32_7, str12);
                    else if (int32_6 != 0)
                    {
                      dataTable6 = this.GetDatiContratto_Riferimento(int32_6, str12);
                    }
                    else
                    {
                      dtLOG.Rows.Add(dtLOG.NewRow());
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["CODPOS"] = (object) int32_3;
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["ANNDEN"] = (object) int32_1;
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["MESDEN"] = (object) int32_2;
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["MAT"] = (object) int32_4;
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["DESERR"] = (object) "CONTRATTO NON TROVATO";
                      break;
                    }
                    if (dataTable6.Rows.Count == 0)
                    {
                      dtLOG.Rows.Add(dtLOG.NewRow());
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["CODPOS"] = (object) int32_3;
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["ANNDEN"] = (object) int32_1;
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["MESDEN"] = (object) int32_2;
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["MAT"] = (object) int32_4;
                      dtLOG.Rows[dtLOG.Rows.Count - 1]["DESERR"] = (object) "CONTRATTO NON TROVATO";
                      break;
                    }
                    if (Convert.ToInt32(dataTable2.Rows[index4]["CODLOC"]) != 0)
                      PROLOC = Convert.ToInt32(dataTable6.Rows[0]["PROLOC"]);
                    string str16 = dataTable6.Rows[0]["TIPSPE"].ToString().Trim();
                    string str17 = dataTable6.Rows[0]["DENQUA"].ToString().Trim();
                    string str18 = dataTable2.Rows[index4]["RAPPORTO"].ToString().Trim();
                    int int32_14 = Convert.ToInt32(dataTable6.Rows[0]["CODQUACON"]);
                    int int32_15 = Convert.ToInt32(dataTable6.Rows[0]["PROCON"]);
                    int int32_16 = Convert.ToInt32(dataTable2.Rows[index4]["CODLIV"]);
                    string strSQL7 = "SELECT DENLIV FROM CONLIV WHERE CODCON = " + int32_6.ToString() + " AND PROCON = " + int32_15.ToString() + " AND CODLIV = " + int32_16.ToString();
                    dataTable6.Clear();
                    dataTable6 = dataLayer.GetDataTable(strSQL7);
                    if (dataTable6.Rows.Count > 0)
                      str1 = dataTable6.Rows[0]["DENLIV"].ToString().Trim();
                    if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(str12)) > 0 & DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(s2)) < 0)
                    {
                      dataTable1.Rows.Add(dataTable1.NewRow());
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"] = (object) str12;
                      DataRow row1 = dataTable1.Rows[dataTable1.Rows.Count - 1];
                      dateTime = Convert.ToDateTime(s1);
                      dateTime = dateTime.AddDays(-1.0);
                      string str19 = dateTime.ToString().Substring(0, 10);
                      row1["AL"] = (object) str19;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "N";
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Aliquota(str12, int32_3.ToString().Trim(), int32_5, (long) int32_4, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"].ToString(), FAP, int32_14, int32_8);
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODPOS"] = (object) int32_3;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ANNDEN"] = (object) int32_1;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MESDEN"] = (object) int32_2;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MAT"] = (object) int32_4;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PRORAP"] = (object) int32_5;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["NOME"] = (object) str8;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATNAS"] = (object) str9;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATDEC"] = (object) str10;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATCES"] = (object) str11;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODQUACON"] = (object) int32_14;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODGRUASS"] = (object) int32_8;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCON"] = (object) int32_6;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PROCON"] = (object) int32_15;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLOC"] = (object) int32_7;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PROLOC"] = (object) PROLOC;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLIV"] = (object) int32_16;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DENLIV"] = (object) str1;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PERAPP"] = (object) int32_9;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PERPAR"] = (object) PERAPP;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["QUALIFICA"] = (object) str17;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPO_RAPPORTO"] = (object) str18;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATA_INIZIO_STORDL"] = (object) str12;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATA_FINE_STORDL"] = (object) s2;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PERFAP"] = (object) num6;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["FAP"] = (object) FAP;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ABBPRE"] = (object) str13;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ASSCON"] = (object) str14;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPSPE"] = (object) str16;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPRAP"] = (object) str15;
                      if (str13 == "S")
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) this.GetImportoParametro((short) 1, str12);
                      else
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) 0.0;
                      if (str14 == "S")
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) this.GetImportoParametro((short) 4, str12);
                      else
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) 0.0;
                      if (str16 == "S")
                      {
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) this.GetMinimoContrattuale(int32_6, int32_15, int32_7, PROLOC, int32_16, dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"].ToString().Trim(), PERAPP, int32_9);
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) num15;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) 0.0;
                      }
                      else
                      {
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) 0.0;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) 0.0;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) num16;
                      }
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["GIORNI_MESE"] = (object) num10;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["NUMMEN"] = (object) int32_10;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN14"] = (object) int32_11;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN15"] = (object) int32_12;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN16"] = (object) int32_13;
                      dataTable1.Rows.Add(dataTable1.NewRow());
                      DataRow row2 = dataTable1.Rows[dataTable1.Rows.Count - 1];
                      dateTime = Convert.ToDateTime(s1);
                      string str20 = dateTime.ToString().Substring(0, 10);
                      row2["DAL"] = (object) str20;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s2;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "S";
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Aliquota(str12, int32_3.ToString().Trim(), int32_5, (long) int32_4, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"].ToString().Trim(), FAP, int32_14, int32_8);
                    }
                    else if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(str12)) == 0)
                    {
                      dataTable1.Rows.Add(dataTable1.NewRow());
                      DataRow row = dataTable1.Rows[dataTable1.Rows.Count - 1];
                      dateTime = Convert.ToDateTime(s1);
                      string str21 = dateTime.ToString().Substring(0, 10);
                      row["DAL"] = (object) str21;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s2;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "S";
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Aliquota(str12, int32_3.ToString().Trim(), int32_5, (long) int32_4, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"].ToString().Trim(), FAP, int32_14, int32_8);
                    }
                    else if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(str12)) < 0)
                    {
                      dataTable1.Rows.Add(dataTable1.NewRow());
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"] = (object) str12;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s2;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "S";
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Aliquota(str12, int32_3.ToString().Trim(), int32_5, (long) int32_4, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"].ToString().Trim(), FAP, int32_14, int32_8);
                    }
                    else if (DateTime.Compare(DateTime.Parse(s1), DateTime.Parse(s2)) == 0)
                    {
                      dataTable1.Rows.Add(dataTable1.NewRow());
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"] = (object) str12;
                      DataRow row3 = dataTable1.Rows[dataTable1.Rows.Count - 1];
                      dateTime = Convert.ToDateTime(s1);
                      dateTime = dateTime.AddDays(-1.0);
                      string str22 = dateTime.ToString().Substring(0, 10);
                      row3["AL"] = (object) str22;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "N";
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Aliquota(str12, int32_3.ToString().Trim(), int32_5, (long) int32_4, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"].ToString().Trim(), FAP, int32_14, int32_8);
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODPOS"] = (object) int32_3;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ANNDEN"] = (object) int32_1;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MESDEN"] = (object) int32_2;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MAT"] = (object) int32_4;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PRORAP"] = (object) int32_5;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["NOME"] = (object) str8;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATNAS"] = (object) str9;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATDEC"] = (object) str10;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATCES"] = (object) str11;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODQUACON"] = (object) int32_14;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODGRUASS"] = (object) int32_8;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCON"] = (object) int32_6;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PROCON"] = (object) int32_15;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLOC"] = (object) int32_7;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PROLOC"] = (object) PROLOC;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLIV"] = (object) int32_16;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DENLIV"] = (object) str1;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PERAPP"] = (object) int32_9;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PERPAR"] = (object) PERAPP;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["QUALIFICA"] = (object) str17;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPO_RAPPORTO"] = (object) str18;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATA_INIZIO_STORDL"] = (object) str12;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DATA_FINE_STORDL"] = (object) s2;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["PERFAP"] = (object) num6;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["FAP"] = (object) FAP;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ABBPRE"] = (object) str13;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ASSCON"] = (object) str14;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPSPE"] = (object) str16;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPRAP"] = (object) str15;
                      if (str13 == "S")
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) this.GetImportoParametro((short) 1, str12);
                      else
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) 0.0;
                      if (str14 == "S")
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) this.GetImportoParametro((short) 4, str12);
                      else
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) 0.0;
                      if (str16 == "S")
                      {
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) this.GetMinimoContrattuale(int32_6, int32_15, int32_7, PROLOC, int32_16, dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"].ToString().Trim(), PERAPP, int32_9);
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) num15;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) 0.0;
                      }
                      else
                      {
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) 0.0;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) 0.0;
                        dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) num16;
                      }
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["GIORNI_MESE"] = (object) num10;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["NUMMEN"] = (object) int32_10;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN14"] = (object) int32_11;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN15"] = (object) int32_12;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN16"] = (object) int32_13;
                      dataTable1.Rows.Add(dataTable1.NewRow());
                      DataRow row4 = dataTable1.Rows[dataTable1.Rows.Count - 1];
                      dateTime = Convert.ToDateTime(s1);
                      string str23 = dateTime.ToString().Substring(0, 10);
                      row4["DAL"] = (object) str23;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s2;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "S";
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Aliquota(str12, int32_3.ToString().Trim(), int32_5, (long) int32_4, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"].ToString().Trim(), FAP, int32_14, int32_8);
                    }
                    else
                    {
                      dataTable1.Rows.Add(dataTable1.NewRow());
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"] = (object) str12;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = (object) s2;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"] = (object) "N";
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) this.Get_Aliquota(str12, int32_3.ToString().Trim(), int32_5, (long) int32_4, dataTable1.Rows[dataTable1.Rows.Count - 1]["ETAENNE"].ToString().Trim(), FAP, int32_14, int32_8);
                    }
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["CODPOS"] = (object) int32_3;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["MAT"] = (object) int32_4;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["ANNDEN"] = (object) int32_1;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["MESDEN"] = (object) int32_2;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["PRORAP"] = (object) int32_5;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["NOME"] = (object) str8;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["DATNAS"] = (object) str9;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["DATDEC"] = (object) str10;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["DATCES"] = (object) str11;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["CODQUACON"] = (object) int32_14;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["CODGRUASS"] = (object) int32_8;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCON"] = (object) int32_6;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["PROCON"] = (object) int32_15;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLOC"] = (object) int32_7;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["PROLOC"] = (object) PROLOC;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["CODLIV"] = (object) int32_16;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["DENLIV"] = (object) str1;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["PERAPP"] = (object) int32_9;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["PERPAR"] = (object) PERAPP;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["QUALIFICA"] = (object) str17;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPO_RAPPORTO"] = (object) str18;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["DATA_INIZIO_STORDL"] = (object) str12;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["DATA_FINE_STORDL"] = (object) s2;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["PERFAP"] = (object) num6;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["FAP"] = (object) FAP;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["ABBPRE"] = (object) str13;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["ASSCON"] = (object) str14;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPSPE"] = (object) str16;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["TIPRAP"] = (object) str15;
                    if (str13 == "S")
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) this.GetImportoParametro((short) 1, str12);
                    else
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPABB"] = (object) 0.0;
                    if (str14 == "S")
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) this.GetImportoParametro((short) 4, str12);
                    else
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPASSCON"] = (object) 0.0;
                    if (str16 == "S")
                    {
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) this.GetMinimoContrattuale(int32_6, int32_15, int32_7, PROLOC, int32_16, dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"].ToString(), PERAPP, int32_9);
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) num15;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) 0.0;
                    }
                    else
                    {
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPMIN"] = (object) 0.0;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSCA"] = (object) 0.0;
                      dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPTRAECO"] = (object) num16;
                    }
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["GIORNI_MESE"] = (object) num10;
                    dtAziende.Rows[index2]["RIMUOVI"] = (object) "NO";
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["NUMMEN"] = (object) int32_10;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN14"] = (object) int32_11;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN15"] = (object) int32_12;
                    dataTable1.Rows[dataTable1.Rows.Count - 1]["MESMEN16"] = (object) int32_13;
                  }
                }
              }
              else
              {
                dtLOG.Rows.Add(dtLOG.NewRow());
                dtLOG.Rows[dtLOG.Rows.Count - 1]["CODPOS"] = (object) int32_3;
                dtLOG.Rows[dtLOG.Rows.Count - 1]["ANNDEN"] = (object) int32_1;
                dtLOG.Rows[dtLOG.Rows.Count - 1]["MESDEN"] = (object) int32_2;
                dtLOG.Rows[dtLOG.Rows.Count - 1]["MAT"] = (object) int32_4;
                dtLOG.Rows[dtLOG.Rows.Count - 1]["DESERR"] = (object) "DATA DI ISCRIZIONE DELLA MATRICOLA NON TROVATA";
              }
            }
            else
            {
              dtLOG.Rows.Add(dtLOG.NewRow());
              dtLOG.Rows[dtLOG.Rows.Count - 1]["CODPOS"] = (object) int32_3;
              dtLOG.Rows[dtLOG.Rows.Count - 1]["ANNDEN"] = (object) int32_1;
              dtLOG.Rows[dtLOG.Rows.Count - 1]["MESDEN"] = (object) int32_2;
              dtLOG.Rows[dtLOG.Rows.Count - 1]["MAT"] = (object) int32_4;
              dtLOG.Rows[dtLOG.Rows.Count - 1]["DESERR"] = (object) "DATA DI NASCITA DELLA MATRICOLA NON TROVATA";
            }
          }
        }
      }
      int num17 = dataTable1.Rows.Count - 1;
      for (int index5 = 0; index5 <= num17; ++index5)
      {
        if (Convert.ToInt32(dataTable1.Rows[index5]["CODPOS"]) == num1 & Convert.ToInt32(dataTable1.Rows[index5]["ANNDEN"]) == num7 & Convert.ToInt32(dataTable1.Rows[index5]["MESDEN"]) == num8 & Convert.ToInt32(dataTable1.Rows[index5]["MAT"]) == num2 & (Decimal) Convert.ToInt32(dataTable1.Rows[index5]["ALIQUOTA"]) == num3)
        {
          dataTable1.Rows[index5 - index1]["AL"] = dataTable1.Rows[index5]["AL"];
          dataTable1.Rows[index5 - index1]["IMPSCA"] = dataTable1.Rows[index5]["IMPSCA"];
          dataTable1.Rows[index5 - index1]["IMPMIN"] = dataTable1.Rows[index5]["IMPMIN"];
          dataTable1.Rows[index5 - index1]["NUMMEN"] = dataTable1.Rows[index5]["NUMMEN"];
          dataTable1.Rows[index5 - index1]["MESMEN14"] = dataTable1.Rows[index5]["MESMEN14"];
          dataTable1.Rows[index5 - index1]["MESMEN15"] = dataTable1.Rows[index5]["MESMEN15"];
          dataTable1.Rows[index5 - index1]["MESMEN16"] = dataTable1.Rows[index5]["MESMEN16"];
          dataTable1.Rows[index5 - index1]["DATDEC"] = dataTable1.Rows[index5]["DATDEC"];
          dataTable1.Rows[index5 - index1]["IMPTRAECO"] = dataTable1.Rows[index5]["IMPTRAECO"];
          dataTable1.Rows[index5 - index1]["IMPSCA"] = dataTable1.Rows[index5]["IMPSCA"];
          dataTable1.Rows[index5 - index1]["ABBPRE"] = dataTable1.Rows[index5]["ABBPRE"];
          dataTable1.Rows[index5 - index1]["IMPABB"] = dataTable1.Rows[index5]["IMPABB"];
          dataTable1.Rows[index5 - index1]["ASSCON"] = dataTable1.Rows[index5]["ASSCON"];
          dataTable1.Rows[index5 - index1]["IMPASSCON"] = dataTable1.Rows[index5]["IMPASSCON"];
          dataTable1.Rows[index5]["RIMUOVI"] = (object) "SI";
          ++index1;
        }
        else
          index1 = 1;
        num1 = Convert.ToInt32(dataTable1.Rows[index5]["CODPOS"]);
        num7 = Convert.ToInt32(dataTable1.Rows[index5]["ANNDEN"]);
        num8 = Convert.ToInt32(dataTable1.Rows[index5]["MESDEN"]);
        num2 = Convert.ToInt32(dataTable1.Rows[index5]["MAT"]);
        num3 = (Decimal) Convert.ToInt32(dataTable1.Rows[index5]["ALIQUOTA"]);
        num4 = (Decimal) Convert.ToInt32(dataTable1.Rows[index5]["PERAPP"]);
        num5 = (Decimal) Convert.ToInt32(dataTable1.Rows[index5]["PERPAR"]);
        dataTable1.Rows[index5]["TIPSPE"].ToString().Trim();
      }
      for (int index6 = dataTable1.Rows.Count - 1; index6 >= 0; --index6)
      {
        if (dataTable1.Rows[index6]["RIMUOVI"].ToString() == "SI")
          dataTable1.Rows.RemoveAt(index6);
      }
      clsPrev clsPrev = new clsPrev();
      int num18 = dataTable1.Rows.Count - 1;
      for (int index7 = 0; index7 <= num18; ++index7)
      {
        dataTable1.Rows[index7]["NUMGGPER"] = (object) clsPrev.Get_NumGG_Periodo(Convert.ToDateTime(dataTable1.Rows[index7]["DAL"]), Convert.ToDateTime(dataTable1.Rows[index7]["AL"]));
        dataTable1.Rows[index7]["NUMGGDOM"] = (object) clsPrev.Get_NumGG_Domeniche(Convert.ToDateTime(dataTable1.Rows[index7]["DAL"]), Convert.ToDateTime(dataTable1.Rows[index7]["AL"]));
        dataTable1.Rows[index7]["NUMGGAZI"] = (object) 0.0;
        dataTable1.Rows[index7]["NUMGGSOS"] = (object) 0.0;
        dataTable1.Rows[index7]["NUMGGFIG"] = (object) 0.0;
        dataTable1.Rows[index7]["IMPOCC"] = (object) 0.0;
        Decimal NUMGGAZI = Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGAZI"]);
        int int32 = Convert.ToInt32(dataTable1.Rows[index7]["NUMGGSOS"]);
        Decimal NUMGGFIG = Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGFIG"]);
        this.Get_NumGG_Sospensioni(ref dataTable10, Convert.ToDateTime(dataTable1.Rows[index7]["DAL"]), Convert.ToDateTime(dataTable1.Rows[index7]["AL"]), Convert.ToInt32(dataTable1.Rows[index7]["CODPOS"]), Convert.ToInt32(dataTable1.Rows[index7]["MAT"]), Convert.ToInt32(dataTable1.Rows[index7]["PRORAP"]), ref NUMGGAZI, ref int32, ref NUMGGFIG);
        dataTable1.Rows[index7]["NUMGGCONAZI"] = Convert.ToInt32(dataTable1.Rows[index7]["NUMGGSOS"]) <= 0 ? (!(Convert.ToDateTime(dataTable1.Rows[index7]["DATDEC"]) <= Convert.ToDateTime(dataTable1.Rows[index7]["DAL"])) ? (object) (Math.Round(Convert.ToDecimal(dataTable1.Rows[index7]["GIORNI_MESE"]) / 26M) * Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGPER"])) : (!(dataTable1.Rows[index7]["DATCES"].ToString().Trim() == "") ? (!(Convert.ToDateTime(dataTable1.Rows[index7]["DATCES"]) > Convert.ToDateTime(dataTable1.Rows[index7]["AL"])) ? (object) (Math.Round(Convert.ToDecimal(dataTable1.Rows[index7]["GIORNI_MESE"]) / 26M) * Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGPER"])) : (object) (Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGPER"]) - Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGDOM"]) - Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGSOS"]) + Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGAZI"]))) : (object) Math.Round(Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGPER"]) * 26M / Convert.ToDecimal(dataTable1.Rows[index7]["GIORNI_MESE"])))) : (object) (Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGPER"]) - Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGDOM"]) - Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGSOS"]) + Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGAZI"]));
        dataTable1.Rows[index7]["IMPRET"] = !(dataTable1.Rows[index7]["TIPSPE"].ToString() == "S") ? (object) (Convert.ToDecimal(dataTable1.Rows[index7]["IMPTRAECO"]) * Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGCONAZI"]) / 26M) : (object) ((Convert.ToDecimal(dataTable1.Rows[index7]["IMPMIN"]) + Convert.ToDecimal(dataTable1.Rows[index7]["IMPSCA"])) * Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGCONAZI"]) / 26M);
        string str24 = dataTable1.Rows[index7]["ANNDEN"]?.ToString() + "-" + dataTable1.Rows[index7]["MESDEN"]?.ToString() + "-25";
        dateTime = Convert.ToDateTime(dataTable1.Rows[index7]["DATDEC"]);
        int num19 = dateTime.Month - Convert.ToInt32(str24);
        Decimal num20 = 0M;
        int num21 = num19 + 1;
        if (num21 > 12)
          num21 = 12;
        if (dataTable1.Rows[index7]["NUMMEN"] is int num22)
        {
          switch (num22)
          {
            case 13:
              if (Convert.ToInt32(dataTable1.Rows[index7]["MESDEN"]) == 12)
              {
                num20 = (Decimal) (Convert.ToInt32(dataTable1.Rows[index7]["IMPRET"]) / 12 * num21);
                break;
              }
              break;
            case 14:
              if (Convert.ToInt32(dataTable1.Rows[index7]["MESDEN"]) == 12)
                num20 = (Decimal) (Convert.ToInt32(dataTable1.Rows[index7]["IMPRET"]) / 12 * num21);
              if (Convert.ToInt32(dataTable1.Rows[index7]["MESMEN14"]) == Convert.ToInt32(dataTable1.Rows[index7]["MESDEN"]))
              {
                num20 += (Decimal) (Convert.ToInt32(dataTable1.Rows[index7]["IMPRET"]) / 12 * num21);
                break;
              }
              break;
            case 15:
              if (Convert.ToInt32(dataTable1.Rows[index7]["MESDEN"]) == 12)
                num20 = (Decimal) (Convert.ToInt32(dataTable1.Rows[index7]["IMPRET"]) / 12 * num21);
              if (dataTable1.Rows[index7]["MESMEN14"] == dataTable1.Rows[index7]["MESDEN"])
                num20 += (Decimal) (Convert.ToInt32(dataTable1.Rows[index7]["IMPRET"]) / 12 * num21);
              if (dataTable1.Rows[index7]["MESMEN15"] == dataTable1.Rows[index7]["MESDEN"])
              {
                num20 += (Decimal) (Convert.ToInt32(dataTable1.Rows[index7]["IMPRET"]) / 12 * num21);
                break;
              }
              break;
            case 16:
              if (Convert.ToInt32(dataTable1.Rows[index7]["MESDEN"]) == 12)
                num20 = (Decimal) (Convert.ToInt32(dataTable1.Rows[index7]["IMPRET"]) / 12 * num21);
              if (dataTable1.Rows[index7]["MESMEN14"] == dataTable1.Rows[index7]["MESDEN"])
                num20 += (Decimal) (Convert.ToInt32(dataTable1.Rows[index7]["IMPRET"]) / 12 * num21);
              if (dataTable1.Rows[index7]["MESMEN15"] == dataTable1.Rows[index7]["MESDEN"])
                num20 += (Decimal) (Convert.ToInt32(dataTable1.Rows[index7]["IMPRET"]) / 12 * num21);
              if (dataTable1.Rows[index7]["MESMEN16"] == dataTable1.Rows[index7]["MESDEN"])
              {
                num20 += (Decimal) (Convert.ToInt32(dataTable1.Rows[index7]["IMPRET"]) / 12 * num21);
                break;
              }
              break;
          }
        }
        dataTable1.Rows[index7]["IMPRET"] = (object) (Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"]) + num20);
        if (Convert.ToInt32(dataTable1.Rows[index7]["IMPRET"]) > 0)
        {
          int length = dataTable1.Rows[index7]["IMPRET"].ToString().LastIndexOf(",");
          if (length > -1)
            dataTable1.Rows[index7]["IMPRET"] = !(Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"].ToString().Substring(length + 1)) < 50M) ? (object) (Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"].ToString().Substring(0, length)) + 1M) : (object) Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"].ToString().Substring(0, length));
        }
        dataTable1.Rows[index7]["IMPCON"] = (object) (Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"]) * Convert.ToDecimal(dataTable1.Rows[index7]["ALIQUOTA"]) / 100M);
        dataTable1.Rows[index7]["IMPFIG"] = !(dataTable1.Rows[index7]["TIPSPE"].ToString() == "S") ? (object) (Convert.ToDecimal(dataTable1.Rows[index7]["IMPTRAECO"]) * Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGFIG"]) / 26M) : (object) ((Convert.ToDecimal(dataTable1.Rows[index7]["IMPMIN"]) + Convert.ToDecimal(dataTable1.Rows[index7]["IMPSCA"])) * Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGFIG"]) / 26M);
        string strSQL8 = "SELECT VALORE FROM PARGENDET WHERE CODPAR = 5" + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable1.Rows[index7]["DAL"].ToString())) + " BETWEEN DATINI AND DATFIN";
        dataTable1.Rows[index7]["IMPADDREC"] = (object) (Convert.ToDecimal(dataTable1.Rows[index7]["IMPCON"]) * Convert.ToDecimal(dataLayer.Get1ValueFromSQL(strSQL8, CommandType.Text)) / 100M);
        if (!PREV && Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGPER"]) - Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGDOM"]) == Convert.ToDecimal(dataTable1.Rows[index7]["NUMGGSOS"]) & Convert.ToDecimal(dataTable1.Rows[index7]["IMPRET"]) == 0M & Convert.ToDecimal(dataTable1.Rows[index7]["IMPCON"]) == 0M & Convert.ToDecimal(dataTable1.Rows[index7]["IMPFIG"]) == 0M)
          dataTable1.Rows[index7]["RIMUOVI"] = (object) "SI";
      }
      for (int index8 = dataTable1.Rows.Count - 1; index8 >= 0; --index8)
      {
        if (dataTable1.Rows[index8]["RIMUOVI"].ToString() == "SI")
          dataTable1.Rows.RemoveAt(index8);
      }
      if (!string.IsNullOrEmpty(STRRICPREV))
      {
        DataTable dataTable11 = new DataTable();
        string strSQL9 = "  SELECT DISTINCT MODPREDET.MAT, MODPREDET.CODPOS, MODPREDET.DAL, MODPREDET.AL," + " MODPREDET.ANNDEN, MODPREDET.MESDEN, '' AS FLAGMOD " + " FROM MODPREDET, MODPRE " + " WHERE MODPREDET.CODPOS = MODPRE.CODPOS " + " AND MODPREDET.MAT = MODPRE.MAT " + " AND MODPREDET.PRORAP = MODPRE.PRORAP " + " AND MODPREDET.PROMOD = MODPRE.PROMOD " + " AND MODPRE.DATANN IS NULL " + STRRICPREV;
        DataTable dataTable12 = dataLayer.GetDataTable(strSQL9);
        if (dataTable12.Rows.Count > 0)
        {
          int num23 = dataTable12.Rows.Count - 1;
          for (int index9 = 0; index9 <= num23; ++index9)
          {
            for (int index10 = dataTable1.Rows.Count - 1; index10 >= 0; --index10)
            {
              if (dataTable12.Rows[index9]["CODPOS"] == dataTable1.Rows[index10]["CODPOS"] & dataTable12.Rows[index9]["MAT"] == dataTable1.Rows[index10]["MAT"] & dataTable12.Rows[index9]["ANNDEN"] == dataTable1.Rows[index10]["ANNDEN"] & dataTable12.Rows[index9]["MESDEN"] == dataTable1.Rows[index10]["MESDEN"] & dataTable12.Rows[index9]["DAL"] == dataTable1.Rows[index10]["DAL"] & dataTable12.Rows[index9]["AL"] == dataTable1.Rows[index10]["AL"])
              {
                dataTable1.Rows.RemoveAt(index10);
                dataTable12.Rows[index9]["FLAGMOD"] = (object) "S";
                break;
              }
            }
          }
        }
        DataTable dataTable13 = dataTable1.Clone();
        int num24 = dataTable12.Rows.Count - 1;
        for (int index11 = 0; index11 <= num24; ++index11)
        {
          if (dataTable12.Rows[index11]["FLAGMOD"].ToString() == "S")
          {
            string strSQL10 = " SELECT MODPREDET.CODPOS, MODPREDET.ANNDEN, MODPREDET.MESDEN," + " MODPREDET.MAT, '' AS NOME, MODPREDET.PRORAP, MODPREDET.DATNAS," + " MODPREDET.DATDEC, MODPREDET.DATCES, MODPREDET.DAL," + " MODPREDET.AL, MODPREDET.ETA65 AS ETAENNE, '' AS QUALIFICA," + " MODPREDET.ALIQUOTA, '' AS DATA_INIZIO_STORDL, '' AS DATA_FINE_STORDL," + " '' AS TIPO_RAPPORTO, MODPREDET.TIPRAP, MODPREDET.CODQUACON," + " MODPREDET.CODCON, MODPREDET.PROCON, MODPREDET.CODLOC," + " MODPREDET.PROLOC, MODPREDET.CODLIV, MODPREDET.FAP," + " MODPREDET.CODGRUASS, '' AS DENLIV, MODPREDET.IMPRET," + " MODPREDET.PERPAR, MODPREDET.PERAPP, MODPREDET.PERFAP," + " MODPREDET.NUMGGAZI, MODPREDET.NUMGGFIG, MODPREDET.NUMGGDOM," + " MODPREDET.NUMGGSOS, MODPREDET.NUMGGCONAZI, MODPREDET.NUMGGPER," + " MODPREDET.IMPFIG, MODPREDET.IMPABB, '' AS ABBPRE, '' AS ASSCON, 0.0 AS IMPADDREC," + " IMPASSCON, IMPCON,IMPMIN,IMPSCA,IMPTRAECO,TIPSPE, 'S' AS PREV, 0 AS GIORNI_MESE, MODPREDET.IMPOCC, " + " '' AS RIMUOVI, MODPRE.CODSTAPRE" + " FROM MODPREDET, MODPRE" + " WHERE MODPREDET.CODPOS = MODPRE.CODPOS" + " AND MODPREDET.MAT = MODPRE.MAT" + " AND MODPREDET.PRORAP = MODPRE.PRORAP" + " AND MODPREDET.PROMOD = MODPRE.PROMOD" + " AND MODPREDET.CODPOS = " + dataTable12.Rows[index11]["CODPOS"]?.ToString() + " AND MODPREDET.MAT = " + dataTable12.Rows[index11]["MAT"]?.ToString() + " AND MODPREDET.ANNDEN = " + dataTable12.Rows[index11]["ANNDEN"]?.ToString() + " AND MODPREDET.MESDEN = " + dataTable12.Rows[index11]["MESDEN"]?.ToString() + " AND MODPREDET.TIPMOV <> 'AR' ";
            dataTable13.Clear();
            dataTable13 = dataLayer.GetDataTable(strSQL10);
            int num25 = dataTable13.Rows.Count - 1;
            for (int index12 = 0; index12 <= num25; ++index12)
            {
              if (Convert.ToInt32(dataTable13.Rows[index12]["CODSTAPRE"]) == 0)
              {
                if (Convert.ToInt32(dataTable13.Rows[index12]["IMPRET"]) == 0)
                {
                  if (dataTable13.Rows[index12]["TIPSPE"].ToString() == "S")
                    dataTable13.Rows[index12]["IMPRET"] = (object) ((Convert.ToDecimal(dataTable13.Rows[index12]["IMPMIN"]) + Convert.ToDecimal(dataTable13.Rows[index12]["IMPSCA"])) * Convert.ToDecimal(dataTable13.Rows[index12]["NUMGGCONAZI"]) / 26M);
                  else
                    dataTable13.Rows[index11]["IMPRET"] = (object) (Convert.ToDecimal(dataTable13.Rows[index12]["IMPTRAECO"]) * Convert.ToDecimal(dataTable13.Rows[index12]["NUMGGCONAZI"]) / 26M);
                  if (Convert.ToInt32(dataTable13.Rows[index12]["IMPRET"]) > 0)
                  {
                    int length = dataTable13.Rows[index12]["IMPRET"].ToString().LastIndexOf(",");
                    if (length > -1)
                      dataTable13.Rows[index12]["IMPRET"] = !(Convert.ToDecimal(dataTable13.Rows[index12]["IMPRET"].ToString().Substring(length + 1)) < 50M) ? (object) (Convert.ToDecimal(dataTable13.Rows[index12]["IMPRET"].ToString().Substring(0, length)) + 1M) : (object) Convert.ToDecimal(dataTable13.Rows[index12]["IMPRET"].ToString().Substring(0, length));
                  }
                  dataTable13.Rows[index12]["IMPCON"] = (object) (Convert.ToDecimal(dataTable13.Rows[index12]["IMPRET"]) * Convert.ToDecimal(dataTable13.Rows[index12]["ALIQUOTA"]) / 100M);
                  string strSQL11 = "SELECT VALORE FROM PARGENDET WHERE CODPAR = 5" + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable13.Rows[index12]["DAL"].ToString())) + " BETWEEN DATINI AND DATFIN";
                  dataTable13.Rows[index12]["IMPADDREC"] = (object) (Convert.ToDecimal(dataTable13.Rows[index12]["IMPCON"]) * Convert.ToDecimal(dataLayer.Get1ValueFromSQL(strSQL11, CommandType.Text)) / 100M);
                }
                if (Convert.ToInt32(dataTable13.Rows[index12]["IMPFIG"]) == 0)
                  dataTable13.Rows[index12]["IMPFIG"] = !(dataTable13.Rows[index12]["TIPSPE"].ToString() == "S") ? (object) (Convert.ToDecimal(dataTable13.Rows[index12]["IMPTRAECO"]) * Convert.ToDecimal(dataTable13.Rows[index12]["NUMGGFIG"]) / 26M) : (object) ((Convert.ToDecimal(dataTable13.Rows[index12]["IMPMIN"]) + Convert.ToDecimal(dataTable13.Rows[index12]["IMPSCA"])) * Convert.ToDecimal(dataTable13.Rows[index12]["NUMGGFIG"]) / 26M);
              }
              dataTable1.ImportRow(dataTable13.Rows[index12]);
            }
          }
        }
      }
      int num26 = dataTable8.Rows.Count - 1;
      for (int index13 = 0; index13 <= num26; ++index13)
      {
        string strSQL12 = "SELECT * FROM PRAINFRDLDET WHERE ANNO = " + dataTable8.Rows[index13]["ANNO"]?.ToString() + " AND MESE = " + dataTable8.Rows[index13]["MESE"]?.ToString();
        DataTable dataTable14 = dataLayer.GetDataTable(strSQL12);
        int num27 = dataTable14.Rows.Count - 1;
        for (int index14 = 0; index14 <= num27; ++index14)
        {
          int num28 = dataTable1.Rows.Count - 1;
          for (int index15 = 0; index15 <= num28; ++index15)
          {
            if (dataTable1.Rows[index15]["CODPOS"] == dataTable14.Rows[index14]["CODPOS"] & dataTable1.Rows[index15]["MAT"] == dataTable14.Rows[index14]["MAT"] & dataTable1.Rows[index15]["PRORAP"] == dataTable14.Rows[index14]["PRORAP"])
            {
              dataTable1.Rows[index15]["IMPFIG"] = dataTable14.Rows[index14]["IMPFIG"];
              break;
            }
          }
        }
      }
      return dataTable1;
    }

    public Decimal GetMinimoContrattuale(
      int CODCON,
      int PROCON,
      int CODLOC,
      int PROLOC,
      int CODLIV,
      string strData,
      Decimal PERAPP,
      Decimal PERPAR)
    {
      DataLayer dataLayer = new DataLayer();
      string strSQL1 = "SELECT VALUE(SUM(IMPVOCRET), 0) AS EMOLUMENTI FROM CONRET " + " WHERE CODCON = " + CODCON.ToString() + " AND PROCON = " + PROCON.ToString() + " AND CODLIV = " + CODLIV.ToString() + " AND CODVOCRET <> 4 " + " AND DATAPPINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " AND DATAPPFIN >= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData));
      Decimal minimoContrattuale = Convert.ToDecimal(dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text));
      if (CODLOC > 0)
      {
        string strSQL2 = "SELECT VALUE(SUM(IMPVOCRET), 0) AS EMOLUMENTI FROM LOCRET " + " WHERE CODLOC = " + CODLOC.ToString() + " AND PROLOC = " + PROLOC.ToString() + " AND CODLIV = " + CODLIV.ToString() + " AND CODVOCRET <> 4 " + " AND DATAPPINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " AND DATAPPFIN >= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData));
        minimoContrattuale = Convert.ToDecimal(minimoContrattuale.ToString() + dataLayer.Get1ValueFromSQL(strSQL2, CommandType.Text));
      }
      if (PERAPP > 0M)
        minimoContrattuale = minimoContrattuale * PERAPP / 100M;
      if (PERPAR > 0M)
        minimoContrattuale = minimoContrattuale * PERPAR / 100M;
      return minimoContrattuale;
    }

    internal Decimal GetImportoParametro(short intCodPar, string strDataDal) => Math.Round(Convert.ToDecimal(new DataLayer().Get1ValueFromSQL("SELECT VALORE FROM PARGENDET WHERE CODPAR = " + intCodPar.ToString() + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strDataDal)) + " BETWEEN DATINI AND DATFIN", CommandType.Text)), 2);

    public void Get_NumGG_Sospensioni(
      ref DataTable dtSos,
      DateTime DataDal,
      DateTime DataAl,
      int CODPOS,
      int MAT,
      int PRORAP,
      ref Decimal NUMGGAZI,
      ref int NUMGGSOS,
      ref Decimal NUMGGFIG)
    {
      clsPrev clsPrev = new clsPrev();
      int num1 = 0;
      Decimal num2 = 0.0M;
      Decimal num3 = 0.0M;
      int num4 = dtSos.Rows.Count - 1;
      DateTime dateTime;
      for (int index = 0; index <= num4; ++index)
      {
        if (CODPOS == Convert.ToInt32(dtSos.Rows[index][nameof (CODPOS)]) && MAT == Convert.ToInt32(dtSos.Rows[index][nameof (MAT)]) && PRORAP == Convert.ToInt32(dtSos.Rows[index][nameof (PRORAP)]) && Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]) >= Convert.ToDateTime(DataDal) && Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"]) <= Convert.ToDateTime(DataAl))
        {
          if (DataDal >= Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"]))
          {
            if (DataAl <= Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]))
            {
              int numGgDomeniche = clsPrev.Get_NumGG_Domeniche(DataDal, DataAl);
              dateTime = Convert.ToDateTime(DataAl);
              num1 = dateTime.Subtract(DataDal).Days + 1 - numGgDomeniche;
              if (Convert.ToBoolean((object) (dtSos.Rows[index]["PERAZI"], 0, false)))
                num2 = Convert.ToDecimal((object) (Convert.ToDecimal(num1), Convert.ToDecimal(dtSos.Rows[index]["PERFIG"]) / 100M));
              if (Convert.ToBoolean((object) (dtSos.Rows[index]["PERFIG"], 0, false)))
              {
                num3 = Convert.ToDecimal((object) (Convert.ToDecimal(num1), Convert.ToDecimal(dtSos.Rows[index]["PERFIG"]) / 100M));
                break;
              }
              break;
            }
            int numGgDomeniche1 = clsPrev.Get_NumGG_Domeniche(DataDal, Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]));
            dateTime = Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]);
            num1 = dateTime.Subtract(DataDal).Days + 1 - numGgDomeniche1;
            if (Convert.ToBoolean((object) (dtSos.Rows[index]["PERAZI"], 0, false)))
              num2 = Convert.ToDecimal((object) (Convert.ToDecimal(num1), Convert.ToDecimal(dtSos.Rows[index]["PERAZI"]) / 100M));
            if (Convert.ToBoolean((object) (dtSos.Rows[index]["PERFIG"], 0, false)))
              num3 = Convert.ToDecimal((object) (Convert.ToDecimal(num1), Convert.ToDecimal(dtSos.Rows[index]["PERFIG"]) / 100M));
          }
          else
          {
            if (DataAl <= Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]))
            {
              int numGgDomeniche = clsPrev.Get_NumGG_Domeniche(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"]), DataAl);
              dateTime = Convert.ToDateTime(DataAl);
              TimeSpan timeSpan = dateTime.Subtract(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"]));
              num1 = timeSpan.Days + 1 - numGgDomeniche;
              if (Convert.ToBoolean((object) (dtSos.Rows[index]["PERAZI"], 0, false)))
              {
                dateTime = Convert.ToDateTime(DataAl);
                timeSpan = dateTime.Subtract(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"]));
                num2 = (Decimal) (timeSpan.Days + 1 - Convert.ToInt32(numGgDomeniche) * Convert.ToInt32(dtSos.Rows[index]["PERAZI"]) / 100);
              }
              if (Convert.ToBoolean((object) (dtSos.Rows[index]["PERFIG"], 0, false)))
              {
                dateTime = Convert.ToDateTime(DataAl);
                timeSpan = dateTime.Subtract(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"]));
                num3 = (Decimal) (timeSpan.Days + 1 - Convert.ToInt32(numGgDomeniche) * Convert.ToInt32(dtSos.Rows[index]["PERFIG"]) / 100);
                break;
              }
              break;
            }
            int numGgDomeniche2 = clsPrev.Get_NumGG_Domeniche(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"]), Convert.ToDateTime(dtSos.Rows[index]["DATFINSOS"]));
            dateTime = Convert.ToDateTime(DataAl);
            num1 = dateTime.Subtract(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"])).Days + 1 - numGgDomeniche2;
            if (Convert.ToBoolean((object) (dtSos.Rows[index]["PERAZI"], 0, false)))
            {
              dateTime = Convert.ToDateTime(DataAl);
              num2 = (Decimal) (dateTime.Subtract(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"])).Days + 1 - Convert.ToInt32(numGgDomeniche2) * Convert.ToInt32(dtSos.Rows[index]["PERAZI"]) / 100);
            }
            if (Convert.ToBoolean((object) (dtSos.Rows[index]["PERFIG"], 0, false)))
            {
              dateTime = Convert.ToDateTime(DataAl);
              num3 = (Decimal) (dateTime.Subtract(Convert.ToDateTime(dtSos.Rows[index]["DATINISOS"])).Days + 1 - Convert.ToInt32(numGgDomeniche2) * Convert.ToInt32(dtSos.Rows[index]["PERFIG"]) / 100);
            }
          }
        }
      }
      NUMGGSOS = num1;
      NUMGGFIG = num3;
      NUMGGAZI = num2;
    }

    public Decimal Get_Aliquota(
      string strData,
      string CODPOS,
      int PRORAP,
      long MAT,
      string STR65,
      string FAP,
      int CODQUACON,
      int CODGRUASS)
    {
      DataLayer dataLayer = new DataLayer();
      DataTable dataTable1 = new DataTable();
      string str = "Select VALUE(SUM(ALIQUOTA), 0.00) AS ALIQUOTA " + " FROM ALIFORASS " + " WHERE ALIFORASS.CODGRUASS = " + CODGRUASS.ToString() + " AND ALIFORASS.CODQUACON=" + CODQUACON.ToString() + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + "  BETWEEN ALIFORASS.DATINI AND VALUE(ALIFORASS.DATFIN,'9999-12-31') ";
      if (STR65 == "S")
        str += " AND ALIFORASS.CODFORASS IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS <> 'PREV') ";
      string strSQL1 = str + " AND ALIFORASS.CODFORASS NOT IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS = 'FAP') ";
      dataTable1.Clear();
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
      Decimal aliquota = Convert.ToDecimal(dataTable2.Rows[0]["ALIQUOTA"]);
      if (FAP == "S")
      {
        string strSQL2 = "SELECT VALFAP FROM CODFAP WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')";
        dataTable2.Clear();
        DataTable dataTable3 = dataLayer.GetDataTable(strSQL2);
        if (dataTable3.Rows.Count > 0)
          aliquota = Convert.ToDecimal(aliquota) + Convert.ToDecimal(dataTable3.Rows[0]["VALFAP"]);
      }
      return aliquota;
    }

    public DataTable GetDatiContratto_Locale(int CODLOC, string strData)
    {
      DataLayer dataLayer = new DataLayer();
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      string strSQL1 = "SELECT CONLOC.*, 0 AS CODQUACON, '' as DENQUA FROM CONLOC " + " WHERE CODLOC = " + CODLOC.ToString() + " AND DATDEC <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
      DataTable dataTable3 = dataLayer.GetDataTable(strSQL1);
      if (dataTable3.Rows.Count > 0)
      {
        string strSQL2 = "SELECT PROCON FROM CONRIF WHERE CODCON = " + dataTable3.Rows[0]["CODCON"]?.ToString() + " AND DATDEC <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
        DataTable dataTable4 = dataLayer.GetDataTable(strSQL2);
        if (dataTable4.Rows.Count > 0)
          dataTable3.Rows[0]["PROCON"] = dataTable4.Rows[0]["PROCON"];
        string strSQL3 = "SELECT CODQUACON, DENQUA FROM QUACON WHERE CODQUACON IN (SELECT CODQUACON FROM CONRIF WHERE CODCON = " + dataTable3.Rows[0]["CODCON"]?.ToString() + ")";
        dataTable4.Clear();
        DataTable dataTable5 = dataLayer.GetDataTable(strSQL3);
        if (dataTable5.Rows.Count > 0)
        {
          dataTable3.Rows[0]["CODQUACON"] = dataTable5.Rows[0]["CODQUACON"];
          dataTable3.Rows[0]["DENQUA"] = dataTable5.Rows[0]["DENQUA"];
        }
      }
      return dataTable3;
    }

    public DataTable GetDatiContratto_Riferimento(int CODCON, string strData)
    {
      DataLayer dataLayer = new DataLayer();
      DataTable dataTable = new DataTable();
      string strSQL = "SELECT CONRIF.*, (SELECT DENQUA FROM QUACON WHERE CODQUACON = CONRIF.CODQUACON) AS DENQUA, 0 AS CODLOC, 0 AS PROLOC FROM CONRIF " + " WHERE CODCON = " + CODCON.ToString() + " AND DATDEC <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
      return dataLayer.GetDataTable(strSQL);
    }

    public bool WRITE_INSERT_DENDET(
      TFI.OCM.Utente.Utente u,
      string CODPOS,
      string ANNDEN,
      string MESDEN,
      string PRODEN,
      string MAT,
      string DAL,
      string AL,
      string FAP,
      string PERFAP,
      string IMPRET,
      string IMPOCC,
      string IMPFIG,
      string IMPABB,
      string IMPASSCON,
      string IMPCON,
      string IMPMIN,
      string PREV,
      string PRORAP,
      string CODCON,
      string PROCON,
      string CODLOC,
      string PROLOC,
      string CODLIV,
      string CODQUACON,
      string DATNAS,
      string ETA65,
      string TIPMOV,
      string DATDEC,
      string DATCES,
      string NUMGGAZI,
      string NUMGGFIG,
      string NUMGGPER,
      string NUMGGDOM,
      string NUMGGSOS,
      string NUMGGCONAZI,
      string IMPSCA,
      string IMPTRAECO,
      string TIPRAP,
      string PERPAR,
      string PERAPP,
      string TIPSPE,
      string CODGRUASS,
      string ALIQUOTA,
      string TIPDEN,
      string DATINISAN,
      string DATFINSAN,
      Decimal TASSO,
      Decimal PRIORITA = 0M,
      string IMPRETDEL = "0m",
      string IMPOCCDEL = "0m",
      string IMPFIGDEL = "0m",
      string IMPCONDEL = "0m",
      Decimal IMPRETPRE = 0M,
      Decimal IMPOCCPRE = 0M,
      Decimal IMPFIGPRE = 0M,
      Decimal IMPSANDETPRE = 0M,
      Decimal IMPCONPRE = 0M,
      string IMPSANDET = "0m",
      string CODCAUSAN = "",
      Decimal IMPABBPRE = 0M,
      Decimal IMPASSCONPRE = 0M,
      string IMPABBDEL = "0m",
      string IMPASSCONDEL = "0m",
      string ANNCOM = "",
      string NUMMOV = "")
    {
      DataLayer dataLayer = new DataLayer();
      string strSQL1 = " SELECT VALUE(MAX(PRODENDET), 0) + 1  FROM DENDET " + " WHERE CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN + " AND MESDEN = " + MESDEN + " AND PRODEN = " + PRODEN + " AND MAT = " + MAT;
      int int32 = Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text));
      string str1 = "INSERT INTO DENDET (" + "CODPOS ," + "ANNDEN ," + "MESDEN ," + "PRODEN ," + "MAT , " + "PRODENDET ," + "DAL ," + "AL ," + "FAP ," + "PERFAP ," + "IMPRET ," + "IMPOCC ," + "IMPFIG ," + "IMPABB ," + "IMPASSCON ," + "IMPCON ," + "IMPMIN ," + "PREV ," + "PRORAP ," + "CODCON ," + "PROCON ," + "CODLOC ," + "PROLOC ," + "CODLIV ," + "CODQUACON ," + "DATNAS ," + "ETA65 ," + " TIPMOV, " + " DATDEC, " + " DATCES, " + " NUMGGAZI, " + " NUMGGFIG, " + " NUMGGPER, " + " NUMGGDOM, " + " NUMGGSOS, " + " NUMGGCONAZI, " + " IMPSCA, " + " IMPTRAECO, " + " TIPRAP, " + " PERPAR, " + " PERAPP, " + " TIPSPE, " + " CODGRUASS, " + " ALIQUOTA, " + " TIPDEN, " + " DATINISAN, " + " DATFINSAN, " + " TASSAN, " + " PRIORITA, " + " IMPRETDEL, " + " IMPOCCDEL, " + " IMPFIGDEL, " + " IMPCONDEL, " + " IMPRETPRE, " + " IMPOCCPRE, " + " IMPFIGPRE, " + " IMPSANDETPRE, " + " IMPCONPRE, " + " IMPSANDET, " + " CODCAUSAN, " + " IMPABBPRE, " + " IMPASSCONPRE, " + " IMPABBDEL, " + " IMPASSCONDEL, " + " ANNCOM, " + "ULTAGG, " + "UTEAGG) " + " VALUES (" + CODPOS + ", " + ANNDEN + ", " + MESDEN + ", " + PRODEN + ", " + MAT + ", " + int32.ToString() + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DAL.Replace("'", ""))) + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(AL.Replace("'", ""))) + ", " + DBMethods.DoublePeakForSql(FAP) + ", ";
      string str2 = (string.IsNullOrEmpty(PERFAP.ToString().Trim()) ? str1 + " NULL, " : str1 + PERFAP.Replace(",", ".") + ", ") + IMPRET.ToString().Replace(",", ".") + ", " + IMPOCC.ToString().Replace(",", ".") + ", " + IMPFIG.ToString().Replace(",", ".") + ", " + IMPABB.ToString().Replace(",", ".") + ", " + IMPASSCON.ToString().Replace(",", ".") + ", " + IMPCON.ToString().Replace(",", ".") + ", " + IMPMIN.ToString().Replace(",", ".") + ", ";
      string str3 = (!(PREV == "X") ? str2 + DBMethods.DoublePeakForSql(PREV) + ", " : str2 + " 'N', ") + PRORAP + ", " + CODCON + ", " + PROCON + ", " + CODLOC + ", " + PROLOC + ", " + CODLIV + ", " + CODQUACON + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATNAS.Replace("'", ""))) + ", " + DBMethods.DoublePeakForSql(ETA65.Substring(0, 1)) + ", " + DBMethods.DoublePeakForSql(TIPMOV.Trim()) + ", ";
      string str4 = string.IsNullOrEmpty(DATDEC) ? str3 + "NULL, " : str3 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATDEC)) + ", ";
      string str5 = (string.IsNullOrEmpty(DATCES) ? str4 + "NULL, " : str4 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCES)) + ", ") + NUMGGAZI.ToString().Replace(",", ".") + ", " + NUMGGFIG.ToString().Replace(",", ".") + ", " + NUMGGPER.ToString().Replace(",", ".") + ", " + NUMGGDOM.ToString().Replace(",", ".") + ", " + NUMGGSOS.ToString().Replace(",", ".") + ", " + NUMGGCONAZI.ToString().Replace(",", ".") + ", " + IMPSCA.ToString().Replace(",", ".") + ", " + IMPTRAECO.ToString().Replace(",", ".") + ", ";
      string str6 = (string.IsNullOrEmpty(TIPRAP.ToString().Trim()) ? str5 + " NULL, " : str5 + TIPRAP + ", ") + PERPAR.ToString().Replace(",", ".") + ", " + PERAPP.ToString().Replace(",", ".") + ", ";
      string str7 = (string.IsNullOrEmpty(TIPSPE.ToString().Trim()) ? str6 + " NULL, " : str6 + DBMethods.DoublePeakForSql(TIPSPE) + ", ") + CODGRUASS + ", " + ALIQUOTA.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(TIPDEN) + ", ";
      string str8 = string.IsNullOrEmpty(DATINISAN) ? str7 + "NULL, " : str7 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATINISAN)) + ", ";
      string str9 = (string.IsNullOrEmpty(DATFINSAN) ? str8 + "NULL, " : str8 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATFINSAN)) + ", ") + TASSO.ToString().Replace(",", ".") + ", " + PRIORITA.ToString().Replace(",", ".") + ", " + IMPRETDEL.ToString().Replace(",", ".") + ", " + IMPOCCDEL.ToString().Replace(",", ".") + ", " + IMPFIGDEL.ToString().Replace(",", ".") + ", " + IMPCONDEL.ToString().Replace(",", ".") + ", " + IMPRETPRE.ToString().Replace(",", ".") + ", " + IMPOCCPRE.ToString().Replace(",", ".") + ", " + IMPFIGPRE.ToString().Replace(",", ".") + ", " + IMPSANDETPRE.ToString().Replace(",", ".") + ", " + IMPCONPRE.ToString().Replace(",", ".") + ", " + IMPSANDET.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(CODCAUSAN) + ", " + IMPABBPRE.ToString().Replace(",", ".") + ", " + IMPASSCONPRE.ToString().Replace(",", ".") + ", " + IMPABBDEL.ToString().Replace(",", ".") + ", " + IMPASSCONDEL.ToString().Replace(",", ".") + ", ";
      string strSQL2 = (string.IsNullOrEmpty(ANNCOM) ? str9 + "NULL, " : str9 + Convert.ToInt32(ANNCOM).ToString() + ", ") + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")";
      return dataLayer.WriteTransactionData(strSQL2, CommandType.Text);
    }

    public bool MODULE_GENERA_SANZIONE(
      ref Decimal PERMAXSOGLIA,
      ref Decimal IMPSAN,
      Decimal IMPORTO_RETRIBUZIONE,
      ref Decimal TASSO,
      Decimal ALIQUOTA,
      string TIPMOVSAN,
      string DATINISAN,
      string DATFINSAN,
      ref string CODCAUSAN,
      string DATRETT = "",
      int ANNO = 0,
      int ANNDENN = 0)
    {
      DataLayer dataLayer = new DataLayer();
      DataTable dataTable1 = new DataTable();
      Decimal num1 = 0.0M;
      try
      {
        if (!string.IsNullOrEmpty(TIPMOVSAN))
        {
          string strSQL1 = "SELECT CODCAU FROM TIPMOVCAU WHERE TIPMOV = " + DBMethods.DoublePeakForSql(TIPMOVSAN) + " AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
          CODCAUSAN = dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text);
          Decimal num2 = (Decimal) Convert.ToDateTime(DATFINSAN).Subtract(Convert.ToDateTime(DATINISAN)).Days;
          string str = "SELECT VALUE(GIORNI, 0) AS GIORNI, VALUE(TASSO, 0.0) AS TASSO, VALUE(PERMAXSOGLIA, 0.0) AS PERMAXSOGLIA ";
          string strSQL2;
          if (string.IsNullOrEmpty(DATRETT))
            strSQL2 = str + " FROM TIPMOVCAU WHERE TIPMOV = '" + TIPMOVSAN + "' AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATINISAN)) + " BETWEEN DATINI AND DATFIN ";
          else
            strSQL2 = str + " FROM TIPMOVCAU WHERE TIPMOV = '" + TIPMOVSAN + "' AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATRETT)) + " BETWEEN DATINI AND DATFIN ";
          dataTable1.Clear();
          DataTable dataTable2 = dataLayer.GetDataTable(strSQL2);
          if (dataTable2.Rows.Count > 0)
          {
            if (TIPMOVSAN == "SAN_RT_MD")
              num2 = Convert.ToDecimal(num2) + Convert.ToDecimal(dataTable2.Rows[0]["GIORNI"]);
            TASSO = Convert.ToDecimal(dataTable2.Rows[0][nameof (TASSO)]);
            PERMAXSOGLIA = Convert.ToDecimal(dataTable2.Rows[0][nameof (PERMAXSOGLIA)]);
          }
          if (ANNO == 0)
          {
            num1 = IMPORTO_RETRIBUZIONE * ALIQUOTA / 100M * num2 * TASSO / 36500M;
            num1 = Convert.ToDecimal(IMPORTO_RETRIBUZIONE) * ALIQUOTA / 100M * num2 * TASSO / 36500M;
          }
          else if (ANNO < 2003)
          {
            num1 = IMPORTO_RETRIBUZIONE * ALIQUOTA / 100M * TASSO / 36500M;
            num1 = Convert.ToDecimal(IMPORTO_RETRIBUZIONE) * ALIQUOTA / 100M * TASSO / 36500M;
          }
          else
          {
            num1 = IMPORTO_RETRIBUZIONE * ALIQUOTA / 100M * num2 * TASSO / 36500M;
            num1 = Convert.ToDecimal(IMPORTO_RETRIBUZIONE) * ALIQUOTA / 100M * num2 * TASSO / 36500M;
          }
        }
        else
          num1 = 0.0M;
        num1 = IMPSAN;
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public bool Module_Rettifiche_01(
      TFI.OCM.Utente.Utente u,
      ref DataTable dtNot,
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int MAT,
      string TIPMOVSAN,
      string DATA_ORA_SISTEMA,
      string TIPPRI,
      string TIPISC,
      string DATINISAN,
      string DATFINSAN,
      string PREV = "N")
    {
      Decimal IMPSAN = 0.0M;
      Decimal TASSO = 0.0M;
      string CODCAUSAN = "";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      bool flag = false;
      try
      {
        int num1 = dtNot.Rows.Count - 1;
        for (int index = 0; index <= num1; ++index)
        {
          Decimal IMPORTO_RETRIBUZIONE = Convert.ToDecimal(dtNot.Rows[index]["IMPRET"]);
          Decimal num2 = Convert.ToDecimal(dtNot.Rows[index]["IMPFIG"]);
          Decimal num3 = Convert.ToDecimal(dtNot.Rows[index]["IMPCON"]);
          Decimal num4 = Convert.ToDecimal(dtNot.Rows[index]["IMPABB"]);
          Decimal num5 = Convert.ToDecimal(dtNot.Rows[index]["IMPASSCON"]);
          string DATRETT = "";
          Decimal PERMAXSOGLIA = 0M;
          flag = this.MODULE_GENERA_SANZIONE(ref PERMAXSOGLIA, ref IMPSAN, IMPORTO_RETRIBUZIONE, ref TASSO, Convert.ToDecimal(dtNot.Rows[index]["ALIQUOTA"]), TIPMOVSAN, DATINISAN, DATFINSAN, ref CODCAUSAN, DATRETT, ANNDEN);
          if (flag)
          {
            flag = this.WRITE_INSERT_DENDET(u, CODPOS.ToString(), ANNDEN.ToString(), MESDEN.ToString(), PRODEN.ToString(), MAT.ToString(), dtNot.Rows[index]["DAL"].ToString(), dtNot.Rows[index]["AL"].ToString(), dtNot.Rows[index]["FAP"].ToString(), dtNot.Rows[index]["PERFAP"].ToString(), IMPORTO_RETRIBUZIONE.ToString(), "0.0d", num2.ToString(), num4.ToString(), num5.ToString(), num3.ToString(), dtNot.Rows[index]["IMPMIN"].ToString(), PREV, dtNot.Rows[index]["PRORAP"].ToString(), dtNot.Rows[index]["CODCON"].ToString(), dtNot.Rows[index]["PROCON"].ToString(), dtNot.Rows[index]["CODLOC"].ToString(), dtNot.Rows[index]["PROLOC"].ToString(), dtNot.Rows[index]["CODLIV"].ToString(), dtNot.Rows[index]["CODQUACON"].ToString(), dtNot.Rows[index]["DATNAS"].ToString(), dtNot.Rows[index]["ETAENNE"].ToString(), "RT", dtNot.Rows[index]["DATDEC"].ToString(), dtNot.Rows[index]["DATCES"].ToString(), dtNot.Rows[index]["NUMGGAZI"].ToString(), dtNot.Rows[index]["NUMGGFIG"].ToString(), dtNot.Rows[index]["NUMGGPER"].ToString(), dtNot.Rows[index]["NUMGGDOM"].ToString(), dtNot.Rows[index]["NUMGGSOS"].ToString(), dtNot.Rows[index]["NUMGGCONAZI"].ToString(), dtNot.Rows[index]["IMPSCA"].ToString(), dtNot.Rows[index]["IMPTRAECO"].ToString(), dtNot.Rows[index]["TIPRAP"].ToString(), dtNot.Rows[index]["PERPAR"].ToString(), dtNot.Rows[index]["PERAPP"].ToString(), dtNot.Rows[index]["TIPSPE"].ToString(), dtNot.Rows[index]["CODGRUASS"].ToString(), dtNot.Rows[index]["ALIQUOTA"].ToString(), TIPISC, DATINISAN, DATFINSAN, TASSO, Convert.ToDecimal(TIPPRI), IMPORTO_RETRIBUZIONE.ToString(), "0.0d", num2.ToString(), num3.ToString(), IMPSANDET: IMPSAN.ToString(), CODCAUSAN: CODCAUSAN, IMPABBDEL: num4.ToString(), IMPASSCONDEL: num5.ToString());
            if (!flag)
              break;
          }
          else
            break;
        }
        return flag;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public bool CreaStampaRicevutaProtocolloCessazioneRDL(
      string DESC_AZIENDA,
      int MAT,
      string COGNOME,
      string NOME,
      string DATACESSAZIONE,
      string DATPRO,
      string CAUSALE,
      string STRPROTOCOLLO,
      string STRFILEPDF)
    {
      try
      {
        DateTime.Now.ToString("t");
        string[] strArray = STRPROTOCOLLO.Split(';');
        string str1 = strArray[strArray.Length - 1].Split(' ')[1];
        string path = STRFILEPDF;
        string str2 = DateTime.Parse(DATACESSAZIONE.ToString()).ToString("dd-MM-yyyy");
        PdfReader reader = new PdfReader(AppContext.BaseDirectory + "bin\\" + "StampePDF\\PdfModuli\\TEMPLATE_RICEVUTA_CESS.pdf");
        PdfStamper pdfStamper = new PdfStamper(reader, (Stream) new FileStream(path, FileMode.Create));
        AcroFields acroFields = pdfStamper.AcroFields;
        acroFields.SetField("protocollo", STRPROTOCOLLO);
        acroFields.SetField("PR", "Protocollo ENPAIA");
        acroFields.SetField("protnum", "Num. " + strArray.ToString());
        acroFields.SetField("protdat", "Data " + DATPRO.Substring(0, 10));
        acroFields.SetField("NOMINATIVO", DESC_AZIENDA);
        acroFields.SetField("DATA", DATPRO.Substring(0, 10));
        acroFields.SetField("ORA", str1);
        acroFields.SetField("NUMPROT", strArray.ToString());
        acroFields.SetField(nameof (MAT), MAT.ToString() + " - " + COGNOME + " " + NOME);
        acroFields.SetField("DATCES", str2.Substring(0, 10));
        acroFields.SetField("CAUCES", CAUSALE);
        pdfStamper.FormFlattening = false;
        pdfStamper.Close();
        reader.Close();
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public string FTP_Download(string pathFile)
    {
      WebClient webClient = new WebClient();
      string fileName = Path.GetFileName(pathFile);
      string str = (this.PROT_PRES_GET_TEMP_PATH_APPLICATION() + "\\" + fileName).Replace("\\\\", "\\");
      if (System.IO.File.Exists(str))
        System.IO.File.Delete(str);
      webClient.Credentials = (ICredentials) new NetworkCredential(Cypher.DeCryptPassword(ConfigurationManager.AppSettings["strUserFTP"]), Cypher.DeCryptPassword(ConfigurationManager.AppSettings["strPwdFTP"]));
      webClient.DownloadFile(pathFile, str);
      return str;
    }
  }
}
