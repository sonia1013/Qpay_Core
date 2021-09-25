using System;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.Numerics;
using Qpay_Core.Models;
using Qpay_Core.Services.Common;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using Sinopac.Shioaji;
using System.Diagnostics.Eventing.Reader;

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
            string result = SHA256_Hash.GetSHA256Hash(sign).ToUpper();

            string hashed_nonce = SHA256_Hash.GetSHA256Hash(nonce).ToUpper().PadRight(16);
            string IV = hashed_nonce.Remove(0, 48);
            //var atmParam = new ATMParam() { ExpireDate = "20180502"};
            var model = new OrderCreateRequestModel() { ShopNo = "BA0026_001", OrderNo = "A201804270001", Amount = 50000, CurrencyID = "TWD", PayType = "A", PrdtName = "虛擬帳號訂單"};

            string jsonStr = JsonConvert.SerializeObject(model);

            string msg = GetMessage(HashID, jsonStr, IV);
            string test2 = AesCBC_Encrypt.EncryptAesCBC(jsonStr, HashID, IV);

            //Console.WriteLine(msg);
            //Console.WriteLine(IV);

            string correct_Sign = "A3EAEE3B361B7E7E9B0F6422B954ECA5D54CEC6EAB0880CB484AA6FDA4154331";
            //if (result.ToUpper() == correct_Sign)
            if(msg==test2)
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

        public static string GetHashParams()
        {
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(model);

            string result = "Amount=50000&BackendURL=http://10.11.22.113:8803/QPay.ApiClient/AutoPush/PushSuccess&CurrencyID=TWD&OrderNo=A201804270001&PayType=A&PrdtName=虛擬帳號訂單&ReturnURL=http://10.11.22.113:8803/QPay.ApiClient/Store/Return&ShopNo=BA0026_001";

            return result;
        }

        public static string GetMessage(string hashId,string data,string iv)
        {
            return AesCBC_Encrypt.AESEncrypt(data, hashId, iv);
        }
    }
}
