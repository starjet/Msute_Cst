using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MsuteClasses
{
    public static class HpCrypt
    {
        private static readonly byte[] mbrSalt = new byte[]
	{
		67,
		82,
		72,
		39,
		136,
		154,
		205,
		161
	};

        public static byte[] EncryptBytes(byte[] pBytes, string pPassword)
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.BlockSize = 128;
            rijndaelManaged.KeySize = 256;
            rijndaelManaged.Mode = CipherMode.CBC;
            rijndaelManaged.Padding = PaddingMode.PKCS7;
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(pPassword, HpCrypt.mbrSalt);
            rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(32);
            rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(16);
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ICryptoTransform transform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                {
                    memoryStream.Write(rfc2898DeriveBytes.Salt, 0, 8);
                    cryptoStream.Write(pBytes, 0, pBytes.Length);
                    cryptoStream.FlushFinalBlock();
                }
                result = memoryStream.ToArray();
            }
            return result;
        }

        public static byte[] EncryptString(string pText, string pPassword)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(pText);
            return HpCrypt.EncryptBytes(bytes, pPassword);
        }

        public static byte[] DecryptBytes(byte[] pEncrypted, string pPassword)
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.BlockSize = 128;
            rijndaelManaged.KeySize = 256;
            rijndaelManaged.Mode = CipherMode.CBC;
            rijndaelManaged.Padding = PaddingMode.PKCS7;
            byte[] array = new byte[pEncrypted.Length];
            using (MemoryStream memoryStream = new MemoryStream(pEncrypted))
            {
                byte[] array2 = new byte[8];
                memoryStream.Read(array2, 0, 8);
                Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(pPassword, array2);
                rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(32);
                rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(16);
                ICryptoTransform transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
                {
                    cryptoStream.Read(array, 0, array.Length);
                }
            }
            return array;
        }

        public static string DecryptString(byte[] pEncrypted, string pPassword)
        {
            byte[] bytes = HpCrypt.DecryptBytes(pEncrypted, pPassword);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}