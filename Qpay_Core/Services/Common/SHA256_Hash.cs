using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Qpay_Core.Services.Common
{
    public class SHA256_Hash
    {
        /// <summary>
        /// SHA256雜湊
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetSHA256Hash(string input)
        {
            //    using var hash = SHA256.Create();
            //    var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            //    return Convert.ToHexString(byteArray).ToLower();
            using (SHA256 sha256Hash = SHA256.Create())
            {
                string hash = GetHash(sha256Hash, input);
                return hash;
            }
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)    //參考微軟官方method
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

        public static string GetXorResult(string hex1, string hex2)
        {
            BigInteger dec1 = BigInteger.Parse(hex1, NumberStyles.HexNumber);
            BigInteger dec2 = BigInteger.Parse(hex2, NumberStyles.HexNumber);
            BigInteger result = dec1 ^ dec2;
            string hexResult = result.ToString("X");
            return hexResult;
        }

        public static string GetHashID(string value1, string value2)
        {
            return (value1 + value2).ToUpper();
        }

    }
}
