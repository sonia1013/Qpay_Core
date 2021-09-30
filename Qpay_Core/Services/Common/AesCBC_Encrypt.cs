using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Qpay_Core.Services.Common
{
    public class AesCBC_Encrypt
    {
        //string decryptData = AESDecrypt("2fbwW9 8vPId2/foafZq6Q==", "1234567812345678", "1234567812345678");

        public static string AESEncrypt(string toEncrypt, string key, string iv)
        {
            StringBuilder sb = new StringBuilder();
            byte[] input_Key = Encoding.ASCII.GetBytes(key);
            byte[] input_IV = Encoding.ASCII.GetBytes(iv);
            byte[] dataByteArray = Encoding.UTF8.GetBytes(toEncrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = input_Key;
            rDel.IV = input_IV;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultByteArray = cTransform.TransformFinalBlock(dataByteArray, 0, dataByteArray.Length);

            foreach (byte b in resultByteArray)
            {
                sb.Append(string.Format("{0:x2}", b));
            }

            return sb.ToString();
            //return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            //AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            //using (AesManaged aesAlg = new AesManaged())
            //{

            //}
        }

        public static string EncryptAesCBC(string source, string key, string iv)
        {
            StringBuilder sb = new StringBuilder();
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            byte[] keyB = Encoding.ASCII.GetBytes(key);
            byte[] ivB = Encoding.ASCII.GetBytes(iv);
            byte[] dataByteArray = Encoding.UTF8.GetBytes(source);

            aes.Key = keyB;
            aes.IV = ivB;

            string encrypt = "";
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(dataByteArray, 0, dataByteArray.Length);
                cs.FlushFinalBlock();
                //輸出資料
                foreach (byte b in ms.ToArray())
                {
                    sb.AppendFormat("{0:X2}", b);
                }
                encrypt = sb.ToString();
            }
            return encrypt;
        }


        public static string AESDecrypt(string toDecrypt, string key, string iv)
        {
            byte[] input_Key = UTF8Encoding.UTF8.GetBytes(key);
            byte[] input_IV = UTF8Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = input_Key;
            rDel.IV = input_IV;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// AES CBC解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DecryptAesCBC(string cipherText, string key, string iv)
        {
            byte[] dataByteArray = new byte[cipherText.Length / 2];
            for (int x = 0; x < cipherText.Length / 2; x++)
            {
                int i = (Convert.ToInt32(cipherText.Substring(x * 2, 2), 16));
                dataByteArray[x] = (byte)i;
            }

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            byte[] keyB = Encoding.ASCII.GetBytes(key);
            byte[] ivB = Encoding.ASCII.GetBytes(iv);
            aes.Key = keyB;
            aes.IV = ivB;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

    }
}
