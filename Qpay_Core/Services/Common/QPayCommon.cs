using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Qpay_Core.Models;
using Qpay_Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Qpay_Core.Services.Common
{
    public static class QPayCommon
    {

        public static string GetHashID()
        {
            string A1 = "4D9709D699CA40EE";
            string A2 = "5A4FEF83140C4E9E";
            string B1 = "BC74301945134CB4";
            string B2 = "961F67F8FCA44AB9";

            string value1 = GetXORencrypt(A1, A2);
            string value2 = GetXORencrypt(B1, B2);
            return (value1 + value2).ToUpper();
        }

        public static string GetXORencrypt(string hex1, string hex2)
        {
            BigInteger dec1 = BigInteger.Parse(hex1, NumberStyles.HexNumber);
            BigInteger dec2 = BigInteger.Parse(hex2, NumberStyles.HexNumber);
            BigInteger result = dec1 ^ dec2;
            string hexResult = result.ToString("X");
            return hexResult;
        }

        public static string SerializeToJson<T>(this T data)
        {
            //value為null或空值則不序列化(空值則需在property上加attribute)
            string result = JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });
            return result;
        }

        /// <summary>
        /// 計算IV值-
        /// </summary>
        /// <param name="nonce"></param>
        /// <returns></returns>
        public static string CalculateIVbyNonce(string nonce)
        {
            string hashed_nonce = SHA256_Hash.GetSHA256Hash(nonce).ToUpper();
            return hashed_nonce.Remove(0, 48);
        }

    }

    //public class JsonContent : StringContent
    //{
    //    public JsonContent(Object obj) : base(JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8, "application/json")
    //    {

    //    }
    //}

}
