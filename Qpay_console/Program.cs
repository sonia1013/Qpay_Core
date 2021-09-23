using System;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.Numerics;
using Qpay_Core.Models;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace Qpay_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("測試算出sign");

            string nonce = "NjM2NjA0MzI4ODIyODguMzo3NzI0ZDg4ZmI5Nzc2YzQ1MTNhYzg2MTk3NDBlYTRhNGU0N2IxM2Q2M2JkMTIwOGU5YzZhMGFmNGY5MjA5YzVm";

            string A1 = "4D9709D699CA40EE"; //NEQ5NzA5RDY5OUNBNDBFRQ==
            string A2 = "5A4FEF83140C4E9E";
            string B1 = "BC74301945134CB4";
            string B2 = "961F67F8FCA44AB9";

            string temp1 = GetXORencrypt(A1, A2);
            string temp2 = GetXORencrypt(B1, B2);

            string HashID = GetHashID(temp1, temp2);
            string hash_str = GetHashParams();

            string sign = hash_str + nonce + HashID;
            string result = string.Empty;

            using(SHA256 sha256Hash = SHA256.Create())
            {
                string hash = GetHash(sha256Hash, sign);
                result = hash;
            }


            Console.WriteLine(result.ToUpper());

            string correct_Sign = "A3EAEE3B361B7E7E9B0F6422B954ECA5D54CEC6EAB0880CB484AA6FDA4154331";
            if (result.ToUpper() == correct_Sign)
                Console.WriteLine(true);
            else
                Console.WriteLine(false);

            Console.ReadKey();
        }

        private static string GetXORencrypt(string hex1, string hex2)
        {
            BigInteger dec1 = BigInteger.Parse(hex1, NumberStyles.HexNumber);
            BigInteger dec2 = BigInteger.Parse(hex2, NumberStyles.HexNumber);
            BigInteger result = dec1 ^ dec2;
            string hexResult = result.ToString("X");
            return hexResult;
        }

        private static string GetHashID(string value1, string value2)
        {
            return (value1 + value2).ToUpper();
        }

        //private static string Base64Encode(string str)
        //{
        //    return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        //}
        //private static string Base64Decode(string str)
        //{
        //    return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        //}

        public static string GetHashParams()
        {
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(model);

            string result = "Amount=50000&BackendURL=http://10.11.22.113:8803/QPay.ApiClient/AutoPush/PushSuccess&CurrencyID=TWD&OrderNo=A201804270001&PayType=A&PrdtName=虛擬帳號訂單&ReturnURL=http://10.11.22.113:8803/QPay.ApiClient/Store/Return&ShopNo=BA0026_001";

            return result;
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

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

    }
}
