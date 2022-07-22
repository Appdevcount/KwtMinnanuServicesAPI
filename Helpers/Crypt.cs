using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace ETradeAPI.Helpers
{
    public static class Crypt
    {
        private const string HASH_KEY = "1B96EBCA-412F-44b9-8079-B204E1253DA6";
        private static byte[] _IV = new byte[8];
        private static byte[] _key = new byte[8];

        public static string DecryptData(string data)
        {
            InitializeKey(HASH_KEY);
            DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            ICryptoTransform transform = dESCryptoServiceProvider.CreateDecryptor(_key, _IV);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
            byte[] array = new byte[data.Length];
            try
            {
                array = Convert.FromBase64CharArray(data.ToCharArray(), 0, data.Length);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                String error = ex.ToString();
                return "";
            }
            long num = 0L;
            long num2 = (long)data.Length;
            string result;
            try
            {
                while (num2 >= num)
                {
                    cryptoStream.Write(array, 0, array.Length);
                    num = memoryStream.Length + (long)((ulong)Convert.ToUInt32(array.Length / dESCryptoServiceProvider.BlockSize * dESCryptoServiceProvider.BlockSize));
                }
                string text = new ASCIIEncoding().GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                int length = Convert.ToInt32(text.Substring(0, 5));
                text = text.Substring(5, length);
                long arg_F0_0 = memoryStream.Length;
                result = text;
            }
            catch (Exception ex)
            {
                String error = ex.ToString();
                //MessageBox.Show(ex.Message);
                return "";
            }
            return result;
        }
        public static string EncryptData(string data)
        {
            if (data.Length > 92160)
            {
                //MessageBox.Show("Data String too large. Keep within 90Kb.");
                return "";
            }
            if (!InitializeKey("1B96EBCA-412F-44b9-8079-B204E1253DA6"))
            {
                //MessageBox.Show("Fail to generate key for encryption");
                return "";
            }
            data = string.Format("{0,5:00000}" + data, data.Length);
            byte[] array = new byte[data.Length];
            new ASCIIEncoding().GetBytes(data, 0, data.Length, array, 0);
            ICryptoTransform transform = new DESCryptoServiceProvider().CreateEncryptor(_key, _IV);
            MemoryStream stream = new MemoryStream(array);
            CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
            MemoryStream memoryStream = new MemoryStream();
            byte[] buffer = new byte[1024];
            int num;
            do
            {
                num = cryptoStream.Read(buffer, 0, 1024);
                if (num != 0)
                {
                    memoryStream.Write(buffer, 0, num);
                }
            }
            while (num > 0);
            if (memoryStream.Length == 0L)
            {
                return "";
            }
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }
        private static bool InitializeKey(string key)
        {
            bool result;
            try
            {
                byte[] array = new byte[key.Length];
                new ASCIIEncoding().GetBytes(key, 0, key.Length, array, 0);
                byte[] array2 = new SHA1CryptoServiceProvider().ComputeHash(array);
                for (int i = 0; i < 8; i++)
                {
                    _key[i] = array2[i];
                }
                for (int i = 8; i < 16; i++)
                {
                    _IV[i - 8] = array2[i];
                }
                result = true;
            }
            catch (Exception ex)
            {
                String error = ex.ToString();
                result = false;
            }
            return result;
        }

       

    }
}