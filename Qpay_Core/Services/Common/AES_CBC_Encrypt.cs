using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Qpay_Core.Services.Common
{
    public class AES_CBC_Encrypt
    {
        //string decryptData = AESDecrypt("2fbwW9 8vPId2/foafZq6Q==", "1234567812345678", "1234567812345678");

        public static string AESEncrypt(string toEncrypt, string key, string iv)
        {
            byte[] input_Key = UTF8Encoding.UTF8.GetBytes(key);
            byte[] input_IV = UTF8Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = input_Key;
            rDel.IV = input_IV;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.Zeros;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
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


    }
}
