using System;
using System.Text;
using System.Security.Cryptography;

namespace Qpay_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("請輸入要做Base64的字串:");

            string nonce = "NjM2NjA0MzI4ODIyODguMzo3NzI0ZDg4ZmI5Nzc2YzQ1MTNhYzg2MTk3NDBlYTRhNGU0N2IxM2Q2M2JkMTIwOGU5YzZhMGFmNGY5MjA5YzVm";

            //string A1 = "4D9709D699CA40EE"; //NEQ5NzA5RDY5OUNBNDBFRQ==
            //string A2 = "5A4FEF83140C4E9E";
            //string B1 = "BC74301945134CB4";
            //string B2 = "961F67F8FCA44AB9";

            string hex1 = "4D9709D699CA40EE";
            string hex2 = "5A4FEF83140C4E9E";
            long dec1 = Convert.ToInt64(hex1, 16);
            long dec2 = Convert.ToInt64(hex2, 16);
            long result = dec1 ^ dec2;
            string hexResult = result.ToString("X");


            string temp1 = "17d8e6558dc60e70";
            // A1 XOR A2 = "17d8e6558dc60e70"
            string temp2 = "2a6b57e1b9b7060d";
            // B1 XOR B2 = "2a6b57e1b9b7060d"

            string HashID = (temp1 + temp2).ToUpper();


            //加入訊息內文:123


            string Sign = "" + nonce + HashID;
            //string result = Base64Encode(input);
            //Console.WriteLine(result);
        }

        private static string Base64Encode(string str)
        {
            return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }
        private static string Base64Decode(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }

        /// <summary>
        /// XOR  解密
        /// </summary>
        /// <param name="data">欲解密資料</param>
        /// <param name="salt">salt</param>
        /// <returns></returns>
        private static string XorDecrypt(string data, string salt)
        {
            data = Base64Decode(data);
            var salts = salt.ToCharArray();
            char[] output = new char[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                output[i] = (char)(data[i] ^ salts[i % salts.Length]);
            }
            var res = new string(output);
            return Base64Decode(res);

        }

        /// <summary>
        /// XOR 加密
        /// </summary>
        /// <param name="data">欲加密資料</param>
        /// <param name="salt">salt</param>
        /// <returns></returns>
        private static string XorEncrypt(string data, string salt)
        {
            data = Base64Encode(data);
            var salts = salt.ToCharArray();
            char[] output = new char[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                output[i] = (char)(data[i] ^ salts[i % salts.Length]);
            }
            return Base64Encode(new string(output));
        }

        /// <summary>
        /// SHA256雜湊
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //public static string Get_SHA256_Hash(string input)
        //{
        //    using var hash = SHA256.Create();
        //    var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        //    return Convert.ToHexString(byteArray).ToLower();
        //}
    }
}
