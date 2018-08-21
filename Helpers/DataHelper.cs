using System;
using System.Security.Cryptography;
using System.Text;

namespace Helpers
{
    public static class DataHelper
    {
        public static String EncryptData(String data)
        {
            String encrypted_string = null;
            byte[] data_bytes = Encoding.UTF8.GetBytes(data);

            using (MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5Provider.ComputeHash(Encoding.UTF8.GetBytes(Properties.Settings.Default.hash));

                using (TripleDESCryptoServiceProvider desProvider = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform cryptoTransform = desProvider.CreateEncryptor();
                    byte[] results = cryptoTransform.TransformFinalBlock(data_bytes,0,data_bytes.Length);
                    encrypted_string = Convert.ToBase64String(results,0,results.Length);
                }
            }

            return encrypted_string;
        }

        public static String DecryptData(String data)
        {
            String decrypted_string = null;
            byte[] data_bytes = Convert.FromBase64String(data);

            using (MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5Provider.ComputeHash(Encoding.UTF8.GetBytes(Properties.Settings.Default.hash));

                using (TripleDESCryptoServiceProvider desProvider = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform cryptoTransform = desProvider.CreateDecryptor();
                    byte[] results = cryptoTransform.TransformFinalBlock(data_bytes, 0, data_bytes.Length);
                    decrypted_string =Encoding.UTF8.GetString(results);
                }
            }

            return decrypted_string; 
        }
    }
}
