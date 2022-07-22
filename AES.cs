using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace ETradeAPI
{
	public class AES
	{
		private static byte[] key = Encoding.UTF8.GetBytes("$1!b@3#d$e%0$1!2");
		public static string EncryptStringAES(string data)
		{
			byte[] array = AES.key;
			byte[] iv = AES.key;
			return AES.EncryptStringToBytes(data, array, iv);
		}

        public static string DecryptToken(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


        public static string EncryptToken(string Token)
        {
            //return Token;//AES.EncryptStringAES(Token + "~" + dt.ToString());
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(Token);
            using (Rijndael encryptor = Rijndael.Create())
            {
                //encryptor.Padding = PaddingMode.;
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    Token = Convert.ToBase64String(ms.ToArray());
                }
            }
            return Token;

        }
        public static string EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
		{
			if (plainText == null || plainText.Length <= 0)
			{
				throw new ArgumentNullException("plainText");
			}
			if (key == null || key.Length <= 0)
			{
				throw new ArgumentNullException("key");
			}
			if (iv == null || iv.Length <= 0)
			{
				throw new ArgumentNullException("key");
			}
			byte[] inArray;
			using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
			{
				rijndaelManaged.Mode = CipherMode.CBC;
				rijndaelManaged.Padding = PaddingMode.PKCS7;
				rijndaelManaged.FeedbackSize = 128;
				rijndaelManaged.Key = key;
				rijndaelManaged.IV = iv;
				ICryptoTransform transform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
						{
							streamWriter.Write(plainText);
						}
						inArray = memoryStream.ToArray();
					}
				}
			}
			return Convert.ToBase64String(inArray);
		}
		public static string DecryptStringAES(string cipherText)
		{
			byte[] array = AES.key;
			byte[] iv = AES.key;
			byte[] cipherText2 = Convert.FromBase64String(cipherText);
			return AES.DecryptStringFromBytes(cipherText2, array, iv);
		}
		public static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
		{
			if (cipherText == null || cipherText.Length <= 0)
			{
				throw new ArgumentNullException("cipherText");
			}
			if (key == null || key.Length <= 0)
			{
				throw new ArgumentNullException("key");
			}
			if (iv == null || iv.Length <= 0)
			{
				throw new ArgumentNullException("key");
			}
			string result = null;
			using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
			{
				rijndaelManaged.Mode = CipherMode.CBC;
				rijndaelManaged.Padding = PaddingMode.PKCS7;
				rijndaelManaged.FeedbackSize = 128;
				rijndaelManaged.Key = key;
				rijndaelManaged.IV = iv;
				ICryptoTransform transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
				try
				{
					using (MemoryStream memoryStream = new MemoryStream(cipherText))
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
						{
							using (StreamReader streamReader = new StreamReader(cryptoStream))
							{
								result = streamReader.ReadToEnd();
							}
						}
					}
				}
				catch
				{
					result = "keyError";
				}
			}
			return result;
		}
	}
}
