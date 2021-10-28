﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Qpay_Core.Services.Common
{
    public static class AesCBC_Encrypt
    {
        /// <summary>
        /// AES CBC加密
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        public static string AESEncrypt(string toEncrypt, string key, string iv)
        {
            StringBuilder sb = new StringBuilder();
            byte[] input_Key = Encoding.ASCII.GetBytes(key);
            byte[] input_IV = Encoding.ASCII.GetBytes(iv);
            byte[] dataByteArray = Encoding.UTF8.GetBytes(toEncrypt);
            //byte[] resultByteArray = cTransform.TransformFinalBlock(dataByteArray, 0, dataByteArray.Length);
            //foreach (byte b in resultByteArray)
            //{
            //    sb.Append(string.Format("{0:X2}", b));
            //}
            //return sb.ToString();
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.Key = input_Key;
            aes.IV = input_IV;
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

        public static string EncryptAesCBC(string source, string key, string iv)
        {
            StringBuilder sb = new StringBuilder();
            byte[] keyB = Encoding.ASCII.GetBytes(key);
            byte[] ivB = Encoding.ASCII.GetBytes(iv);
            byte[] dataByteArray = Encoding.UTF8.GetBytes(source);
            string encrypt = "";

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = keyB;
                aes.IV = ivB;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    //using (StreamWriter sw = new StreamWriter(cs))
                    //{
                    //    sw.Write(dataByteArray, 0, dataByteArray.Length);
                    //    sw.Flush();

                    //}
                    //encrypt = ms.ToArray();
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
        }


        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
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
            //byte[] dataByteArray = new byte[cipherText.Length / 2];
            //for (int x = 0; x < cipherText.Length / 2; x++)
            //{
            //    int i = (Convert.ToInt32(cipherText.Substring(x * 2, 2), 16));
            //    dataByteArray[x] = (byte)i;
            //}
            byte[] dataByteArray = HexToByte(cipherText);

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

        public static byte[] HexToByte(string hex)  //StringToByteArray
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        //public static byte[] HexToByte(this string hexString)
        //{
        //    byte[] output = new byte[hexString.Length / 2];
        //    for (int x = 0; x < hexString.Length; x=x+2)
        //    {
        //        string value = hexString.Substring(x * 2, 2);
        //        //dataByteArray[x / 2] = Convert.ToByte(hexString.Substring(x, 2), 16);
        //        uint i = Convert.ToByte(value, 16);
        //        output[x] = (byte)i;
        //    }
        //    //for (int i = 0; i < hexString.Length; i = i + 2)
        //    //{
        //    //    output[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
        //    //}
        //    return output;

        //}
    }
}
