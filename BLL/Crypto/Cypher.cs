// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Crypto.Cypher
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System;
using System.IO;
using System.Security.Cryptography;

namespace TFI.BLL.Crypto
{
  public class Cypher
  {
    public static string CryptPassword(string strPassword, string strKey = "", string strIV = "")
    {
      try
      {
        SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
        symmetricAlgorithm.BlockSize = 256;
        symmetricAlgorithm.KeySize = 256;
        if (string.IsNullOrEmpty(strKey.Trim()))
          strKey = "akjf78ytHr3z5RoEpm0pw7";
        if (string.IsNullOrEmpty(strIV.Trim()))
          strIV = "BjdCH23973MbnjkcAsy82v";
        MemoryStream memoryStream = new MemoryStream();
        ICryptoTransform encryptor = symmetricAlgorithm.CreateEncryptor(Cypher.GetKey(strKey), Cypher.GetIV(strIV));
        CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, encryptor, CryptoStreamMode.Write);
        StreamWriter streamWriter = new StreamWriter((Stream) cryptoStream);
        streamWriter.Write(strPassword);
        streamWriter.Flush();
        cryptoStream.FlushFinalBlock();
        byte[] numArray = new byte[(int) (memoryStream.Length - 1L + 1L)];
        memoryStream.Position = 0L;
        memoryStream.Read(numArray, 0, (int) memoryStream.Length);
        cryptoStream.Close();
        memoryStream.Close();
        streamWriter.Close();
        return Convert.ToBase64String(numArray);
      }
      catch
      {
        return "";
      }
    }

    public static string DeCryptPassword(string strPassword, string strKey = "", string strIV = "")
    {
      try
      {
        SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
        symmetricAlgorithm.BlockSize = 256;
        symmetricAlgorithm.KeySize = 256;
        if (string.IsNullOrEmpty(strKey.Trim()))
          strKey = "akjf78ytHr3z5RoEpm0pw7";
        if (string.IsNullOrEmpty(strIV.Trim()))
          strIV = "BjdCH23973MbnjkcAsy82v";
        MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(strPassword));
        memoryStream.Position = 0L;
        ICryptoTransform decryptor = symmetricAlgorithm.CreateDecryptor(Cypher.GetKey(strKey), Cypher.GetIV(strIV));
        CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, decryptor, CryptoStreamMode.Read);
        StreamReader streamReader = new StreamReader((Stream) cryptoStream);
        string end = streamReader.ReadToEnd();
        cryptoStream.Close();
        streamReader.Close();
        memoryStream.Close();
        return end;
      }
      catch (Exception ex)
      {
        return "";
      }
    }

    private static byte[] GetIV(string strIV)
    {
      int num1 = (int) Math.Round(31.0);
      byte[] iv = new byte[num1 + 1];
      if (strIV.Length < 1)
        return iv;
      int num2 = strIV.Length;
      if (num2 > num1)
        num2 = num1;
      int num3 = num2 - 1;
      for (int index = 0; index <= num3; ++index)
        iv[index] = Convert.ToByte(strIV[index]);
      return iv;
    }

    private static byte[] GetKey(string strKey)
    {
      int num1 = (int) Math.Round(31.0);
      byte[] key = new byte[num1 + 1];
      if (strKey.Length < 1)
        return key;
      int num2 = strKey.Length;
      if (num2 > num1)
        num2 = num1;
      int num3 = num2 - 1;
      for (int index = 0; index <= num3; ++index)
        key[index] = Convert.ToByte(strKey[index]);
      return key;
    }
  }
}
