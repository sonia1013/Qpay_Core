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
            Console.WriteLine("測試");
            string msg = "8C3CFD579B58FDAC6C1DF8C8EB8B79F49DD533F8D8C5DC181074397D21E7364E26D347DF264C76A59886DEA58F742C068BB66D6918791797B4DC31245E8E621F3791938F0A716AC20BFDC50A268CF9FDAF01149B73F8E5A2D61928AB79E30CA35C7DD55698010FEC071C2628444348C7B628ED4DCEE281234C31617B5441A3C6 4466A824BC8907A2D8571F58C4F780EBBB5D6DD98631A867A807718FDD34833FFB2C72F2731D715A35F1BA145C8D1E656136623FFD60EFDAAB87EF1674EA1BD19868DCBC6552D48D9728AB020E75AA6E7804FB9D7FB4AA7E92F89DFFCFA9D263EE9B043AA7A8DB22ECA894F8D5621BF8E6DF5B250CCA9D1C499EF8896B64617F D9FCF142665EA3A660DC6B7296CFE03C80B6FC96CA5B805ADEE4AF9784FF1A6886FCE42C7FA4575FFF4D3AD302B583D08346DD4F876A7BF2B60D514CA021F62293E960458BF886C6CB746A6EEE0EBB3916EE6B861B32D75CE93F8988215D76DEFEE72F4429B2ED48A5AB8FF04683B409FF8072EB1C5E5162EDC625557328F506 520AE3DBD2C19F6327AA97911BF2789373CE4DBBB6EC52A2EB9263ADAB1767B4FC7646440BA34E3716BC25E81A86FE5EF432E0C8D40A5BC25EDE8E2D1CC56A2C";
            string nonce = "NjM2NjA0MzI4ODU1MDcuMzo1MzE5ZWIwNGZjNzZlZGJhOGM5M2U1YTM0Nzk2MGM5NThhZjJiMTFiYjNiYmZmNjk1ZGMzYTFlMTEyMDA0MGU2";
            //string nonce = "";
            string Hash = "AF9F1EB8E157870C74928EE44A6B89F3D64AA9A584FF3373DB4C4B2A7DA46476";
            string IV = "DB4C4B2A7DA46476";

            string A1 = "65960834240E44B7"; //NEQ5NzA5RDY5OUNBNDBFRQ==
            string A2 = "2831076A098E49E7";
            string B1 = "CB1AFFBF915A492B";
            string B2 = "7F242C0AA612454F";

            //string output = AesCBC_Encrypt.AESDecrypt(msg, Hash, IV);
            
            //string A1 = "4D9709D699CA40EE"; //NEQ5NzA5RDY5OUNBNDBFRQ==
            //string A2 = "5A4FEF83140C4E9E";
            //string B1 = "BC74301945134CB4";
            //string B2 = "961F67F8FCA44AB9";

            string temp1 = GetXORencrypt(A1, A2);
            string temp2 = GetXORencrypt(B1, B2);

            string HashID = GetHashID(temp1, temp2);
            //string hash_str = GetHashParams();


            Console.WriteLine(HashID);
            Console.ReadLine();
            //string sign = hash_str + nonce + HashID;
            //string result = SHA256_Hash.GetSHA256Hash(sign).ToUpper();

            //string hashed_nonce = SHA256_Hash.GetSHA256Hash(nonce).ToUpper().PadRight(16);
            //string IV = hashed_nonce.Remove(0, 48);
            ////var atmParam = new ATMParam() { ExpireDate = "20180502"};
            //var model = new OrderCreateReqModel() { ShopNo = "BA0026_001", OrderNo = "A201804270001", Amount = 50000, CurrencyID = "TWD", PayType = "A", PrdtName = "虛擬帳號訂單"};

            //string jsonStr = JsonConvert.SerializeObject(model);

            //string msg = GetMessage(HashID, jsonStr, IV);
            //string test2 = AesCBC_Encrypt.EncryptAesCBC(jsonStr, HashID, IV);

            ////Console.WriteLine(msg);
            ////Console.WriteLine(IV);

            //string correct_Sign = "A3EAEE3B361B7E7E9B0F6422B954ECA5D54CEC6EAB0880CB484AA6FDA4154331";
            ////if (result.ToUpper() == correct_Sign)
            //if(msg==test2)
            //    Console.WriteLine(true);
            //else
            //    Console.WriteLine(false);

            //Console.ReadKey();
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

        //public static string GetMessage(string hashId, string data, string iv)
        //{
        //    return AesCBC_Encrypt.AESEncrypt(data, hashId, iv);
        //}
    }
}
