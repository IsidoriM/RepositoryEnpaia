using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TFI.CRYPTO.Crypto
{
    public static class Cypher
    {
        private const string KeyFile = "SXeHWTQfQOfSPUZqDBVFDS4z6XBXMrhzsQbdG3KmWUtg9DGkTnYCXKU04OL0r4Dy";

        private const int BufferSize = 8192;

        private const string salt = "ph2DoYmgABfXlyD8JExHjdgfcUBaspNooUoCwtMolIi9jS8eWFedWTW77Sr3WIV";

        private static readonly string STATIC_PATH_TMP = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings.Get("TMPCryptoFile").ToString());

        private static string _GetKey()
        {
            return "akjf78ytHr3z5RoEpm0pw7";
        }

        private static string _GetIV()
        {
            return "BjdCH23973MbnjkcAsy82v";
        }

        public static string CryptPassword(string strPassword, string strKey = "", string strIV = "")
        {
            try
            {
                SymmetricAlgorithm mCryptProv = SymmetricAlgorithm.Create("Rijndael");
                mCryptProv.BlockSize = 256;
                mCryptProv.KeySize = 256;
                if (string.IsNullOrEmpty(strKey.Trim()))
                {
                    strKey = _GetKey();
                }
                if (string.IsNullOrEmpty(strIV.Trim()))
                {
                    strIV = _GetIV();
                }
                MemoryStream mMemStr = new MemoryStream();
                ICryptoTransform mEncryptor = mCryptProv.CreateEncryptor(GetKey(strKey), GetIV(strIV));
                CryptoStream mCryptStr = new CryptoStream(mMemStr, mEncryptor, CryptoStreamMode.Write);
                StreamWriter mStrWri = new StreamWriter(mCryptStr);
                mStrWri.Write(strPassword);
                mStrWri.Flush();
                mCryptStr.FlushFinalBlock();
                byte[] mBytes = new byte[(int)(mMemStr.Length - 1 + 1)];
                mMemStr.Position = 0L;
                mMemStr.Read(mBytes, 0, (int)mMemStr.Length);
                mCryptStr.Close();
                mMemStr.Close();
                mStrWri.Close();
                return Convert.ToBase64String(mBytes);
            }
            catch
            {
                return "";
            }
        }

        public static string DeCryptPassword(string strPassword, string strKey = "", string strIV = "")
        {
            string strDecryptedPwd = "";
            try
            {
                SymmetricAlgorithm mCryptProv = SymmetricAlgorithm.Create("Rijndael");
                mCryptProv.BlockSize = 256;
                mCryptProv.KeySize = 256;
                if (string.IsNullOrEmpty(strKey.Trim()))
                {
                    strKey = _GetKey();
                }
                if (string.IsNullOrEmpty(strIV.Trim()))
                {
                    strIV = _GetIV();
                }
                byte[] mBytes = Convert.FromBase64String(strPassword);
                MemoryStream mMemStr = new MemoryStream(mBytes);
                mMemStr.Position = 0L;
                ICryptoTransform mDecrypt = mCryptProv.CreateDecryptor(GetKey(strKey), GetIV(strIV));
                CryptoStream mCSReader = new CryptoStream(mMemStr, mDecrypt, CryptoStreamMode.Read);
                StreamReader mStrRead = new StreamReader(mCSReader);
                strDecryptedPwd = mStrRead.ReadToEnd();
                mCSReader.Close();
                mStrRead.Close();
                mMemStr.Close();
                return strDecryptedPwd;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static byte[] GetIV(string strIV)
        {
            int arrSize = (int)Math.Round(31.0);
            byte[] arrIV = new byte[arrSize + 1];
            if (strIV.Length < 1)
            {
                return arrIV;
            }
            int lastBound = strIV.Length;
            if (lastBound > arrSize)
            {
                lastBound = arrSize;
            }
            int loopTo = lastBound - 1;
            for (int temp = 0; temp <= loopTo; temp++)
            {
                arrIV[temp] = Convert.ToByte(strIV[temp]);
            }
            return arrIV;
        }

        private static byte[] GetKey(string strKey)
        {
            int arrSize = (int)Math.Round(31.0);
            byte[] arrKey = new byte[arrSize + 1];
            if (strKey.Length < 1)
            {
                return arrKey;
            }
            int lastBound = strKey.Length;
            if (lastBound > arrSize)
            {
                lastBound = arrSize;
            }
            int loopTo = lastBound - 1;
            for (int temp = 0; temp <= loopTo; temp++)
            {
                arrKey[temp] = Convert.ToByte(strKey[temp]);
            }
            return arrKey;
        }

        public static void CriptaFile(string FileWithDestinationPath)
        {
            string[] arFileWithPath = FileWithDestinationPath.Split('\\');
            string NomeFile = arFileWithPath[arFileWithPath.Length - 1];
            string PathOutput = FileWithDestinationPath.Replace(NomeFile, "");
            try
            {
                if (!Directory.Exists(PathOutput))
                {
                    Directory.CreateDirectory(PathOutput);
                }
                if (!Directory.Exists(STATIC_PATH_TMP))
                {
                    Directory.CreateDirectory(STATIC_PATH_TMP);
                }
                FileStream input = new FileStream(FileWithDestinationPath, FileMode.Open, FileAccess.Read);
                FileStream output = new FileStream(Path.Combine(STATIC_PATH_TMP, NomeFile), FileMode.OpenOrCreate, FileAccess.Write);
                RijndaelManaged algorithm = new RijndaelManaged
                {
                    KeySize = 256,
                    BlockSize = 128
                };
                Rfc2898DeriveBytes hashedKey = new Rfc2898DeriveBytes("SXeHWTQfQOfSPUZqDBVFDS4z6XBXMrhzsQbdG3KmWUtg9DGkTnYCXKU04OL0r4Dy", Encoding.ASCII.GetBytes("ph2DoYmgABfXlyD8JExHjdgfcUBaspNooUoCwtMolIi9jS8eWFedWTW77Sr3WIV"));
                algorithm.Key = hashedKey.GetBytes(algorithm.KeySize / 8);
                algorithm.IV = hashedKey.GetBytes(algorithm.BlockSize / 8);
                using (CryptoStream encryptedStream = new CryptoStream(output, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    CopyStream(input, encryptedStream);
                }
                File.Delete(FileWithDestinationPath);
                File.Move(Path.Combine(STATIC_PATH_TMP, NomeFile), FileWithDestinationPath);
                File.Delete(Path.Combine(STATIC_PATH_TMP, NomeFile));
            }
            catch (Exception)
            {
            }
        }

        public static void DeCriptaFile(string FileWithDestinationPath)
        {
            string[] arFileWithPath = FileWithDestinationPath.Split('\\');
            string NomeFile = arFileWithPath[arFileWithPath.Length - 1];
            FileStream input = new FileStream(FileWithDestinationPath, FileMode.Open, FileAccess.Read);
            FileStream output = new FileStream(Path.Combine(STATIC_PATH_TMP, NomeFile), FileMode.OpenOrCreate, FileAccess.Write);
            RijndaelManaged algorithm = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128
            };
            Rfc2898DeriveBytes hashedKey = new Rfc2898DeriveBytes("SXeHWTQfQOfSPUZqDBVFDS4z6XBXMrhzsQbdG3KmWUtg9DGkTnYCXKU04OL0r4Dy", Encoding.ASCII.GetBytes("ph2DoYmgABfXlyD8JExHjdgfcUBaspNooUoCwtMolIi9jS8eWFedWTW77Sr3WIV"));
            algorithm.Key = hashedKey.GetBytes(algorithm.KeySize / 8);
            algorithm.IV = hashedKey.GetBytes(algorithm.BlockSize / 8);
            try
            {
                using CryptoStream decryptedStream = new CryptoStream(output, algorithm.CreateDecryptor(), CryptoStreamMode.Write);
                CopyStream(input, decryptedStream);
            }
            catch
            {
            }
        }

        private static void CopyStream(Stream input, Stream output)
        {
            using (output)
            {
                using (input)
                {
                    byte[] buffer = new byte[8192];
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                    }
                }
            }
        }
    }
}
