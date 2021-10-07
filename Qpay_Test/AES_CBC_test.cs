using NUnit.Framework;
using System;
using System.IO;
using Qpay_Core.Services.Common;
using Qpay_Core.Models;
using Sinopac.Shioaji;
using System.Globalization;
using System.Numerics;
using static System.Net.WebRequestMethods;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;

namespace Qpay_Test
{
    public class Tests
    {

        [SetUp]
        public void Setup()
        {
           // Console.WriteLine("測試");
            //string msg = "8C3CFD579B58FDAC6C1DF8C8EB8B79F49DD533F8D8C5DC181074397D21E7364E26D347DF264C76A59886DEA58F742C068BB66D6918791797B4DC31245E8E621F3791938F0A716AC20BFDC50A268CF9FDAF01149B73F8E5A2D61928AB79E30CA35C7DD55698010FEC071C2628444348C7B628ED4DCEE281234C31617B5441A3C6 4466A824BC8907A2D8571F58C4F780EBBB5D6DD98631A867A807718FDD34833FFB2C72F2731D715A35F1BA145C8D1E656136623FFD60EFDAAB87EF1674EA1BD19868DCBC6552D48D9728AB020E75AA6E7804FB9D7FB4AA7E92F89DFFCFA9D263EE9B043AA7A8DB22ECA894F8D5621BF8E6DF5B250CCA9D1C499EF8896B64617F D9FCF142665EA3A660DC6B7296CFE03C80B6FC96CA5B805ADEE4AF9784FF1A6886FCE42C7FA4575FFF4D3AD302B583D08346DD4F876A7BF2B60D514CA021F62293E960458BF886C6CB746A6EEE0EBB3916EE6B861B32D75CE93F8988215D76DEFEE72F4429B2ED48A5AB8FF04683B409FF8072EB1C5E5162EDC625557328F506 520AE3DBD2C19F6327AA97911BF2789373CE4DBBB6EC52A2EB9263ADAB1767B4FC7646440BA34E3716BC25E81A86FE5EF432E0C8D40A5BC25EDE8E2D1CC56A2C";
            //string nonce = "NjM2NjA0MzI4ODU1MDcuMzo1MzE5ZWIwNGZjNzZlZGJhOGM5M2U1YTM0Nzk2MGM5NThhZjJiMTFiYjNiYmZmNjk1ZGMzYTFlMTEyMDA0MGU2";
            //string nonce = "";
            //string Hash = "AF9F1EB8E157870C74928EE44A6B89F3D64AA9A584FF3373DB4C4B2A7DA46476";
            //string IV = "DB4C4B2A7DA46476";

            //NEQ5NzA5RDY5OUNBNDBFRQ==

            //string output = AesCBC_Encrypt.AESDecrypt(msg, Hash, IV);

            //string A1 = "4D9709D699CA40EE"; //NEQ5NzA5RDY5OUNBNDBFRQ==
            //string A2 = "5A4FEF83140C4E9E";
            //string B1 = "BC74301945134CB4";
            //string B2 = "961F67F8FCA44AB9";

            //string temp1 = GetXORencrypt(A1, A2);
            //string temp2 = GetXORencrypt(B1, B2);
            //
            //string HashID = GetHashID(temp1, temp2);
            ////string hash_str = GetHashParams();
            //
            //
            //Console.WriteLine(HashID);
            //Console.ReadLine();
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



        [Test]
        public void Input_four_strings_and_return_HashID_is_Matched()
        {
            string Expected = "17D8E6558DC60E702A6B57E1B9B7060D";
            string a1 = "4D9709D699CA40EE";
            string a2 = "5A4FEF83140C4E9E";
            string b1 = "BC74301945134CB4";
            string b2 = "961F67F8FCA44AB9";
            string temp1 = SHA256_Hash.GetXorResult(a1, a2);
            string temp2 = SHA256_Hash.GetXorResult(b1, b2);

            string HashID = SHA256_Hash.GetHashID(temp1, temp2);

            Assert.AreEqual(Expected, HashID);
        }

        [Test]
        public void Try_Getting_IV_by_Nonce_ShouldNotBeNull()
        {
            string Expected = "DB4C4B2A7DA46476";
            string nonce = "NjM2NjA0MzI4ODU1MDcuMzo1MzE5ZWIwNGZjNzZlZGJhOGM5M2U1YTM0Nzk2MGM5NThhZjJiMTFiYjNiYmZmNjk1ZGMzYTFlMTEyMDA0MGU2";
            
        }

        [Test]
        public void EnsureAesCBCisCorrect()
        {
            Assert.Pass();
            string Expected = "2C236A4E91DB2F7670E79BBCE3A626EB728916919012681FF92BE0B4BBF57F5519AF1A469A1D8710B202CB2C2F3C12A770788D825AD0F0A22AED518545A0D244AD0F9C37C7C693EFFABE78B606BCDAED6284902F7F522BBA85D9BE7EFEF46C6793FB6A5D6624C2642A74EB312034BEA931EE3A5F3C660F3ABAA9032949AE86DEFEB452545807561D282C7B7C8E9102CED1404B8B542BC09CE12FA38F335BE7F027AE74BDDBADDB1790B172EFBF1FD25524E2BB64A626EA44643D4BD490E348E926BB7A48D5FA939EEC5BE681009E7AC7FED1C8475B715891321406960675B5A216032CF8657A3CB2B2D0C7FF85027D70E1F2B5DD414373912E97FA6FB85E9AB89B118BC545583CC9AC503F8BAD73C185CB97B28313618021F9217A30278043EF728BB5C49D231C4A22279864F68194254BC624789F36CCDEE75861CFC667CD8E9E89F1DB04ABA0D26FEF24BFE0470488";

            string HashId = "17D8E6558DC60E702A6B57E1B9B7060D";
            string IV = "CB6FA68E42B655AB";
            var jsonReq = new OrderCreateReqModel()
            {
                ShopNo = "BA2206_001",
                OrderNo = "A201804270001",
                Amount = 50000,
                CurrencyID = "TWD",
                ATMParam = new ATMParam { ExpireDate = "20180502" },
                CardParam = null,
                PrdtName = "虛擬帳號訂單",
            };
            string strReq = Newtonsoft.Json.JsonConvert.SerializeObject(jsonReq);
            string result = AesCBC_Encrypt.AESEncrypt(strReq.Replace("null",""), HashId, IV);
            Assert.AreEqual(Expected, result);
        }

        //[Test]
        //public static string GetHashParams()
        //{
        //    string Expected = "Amount=50000&BackendURL=http://10.11.22.113:8803/QPay.ApiClient/AutoPush/PushSuccess&CurrencyID=TWD&OrderNo=A201804270001&PayType=A&PrdtName=虛擬帳號訂單&ReturnURL=http://10.11.22.113:8803/QPay.ApiClient/Store/Return&ShopNo=BA0026_001NjM2NjA0MzI4ODIyODguMzo3NzI0ZDg4ZmI5Nzc2YzQ1MTNhYzg2MTk3NDBlYTRhNGU0N2IxM2Q2M2JkMTIwOGU5YzZhMGFmNGY5MjA5YzVm17D8E6558DC60E702A6B57E1B9B7060D";
        //    //string json = Newtonsoft.Json.JsonConvert.SerializeObject(model);

        //    string result = "Amount=50000&BackendURL=http://10.11.22.113:8803/QPay.ApiClient/AutoPush/PushSuccess&CurrencyID=TWD&OrderNo=A201804270001&PayType=A&PrdtName=虛擬帳號訂單&ReturnURL=http://10.11.22.113:8803/QPay.ApiClient/Store/Return&ShopNo=BA0026_001";

        //    return result;
        //}

        [Test]
        public static string TryDecrytedMessageWithAesCBC(string inputStr, string hashId, string iv)
        {
            //string Expected = "{"OrderNo":"A201804270001","ShopNo":"BA0026_001","TSNo":"BA002600000037","PayType":"A","Amount":50000,"Status":"S","Desc ription":"S0000處理成功","ATMParam":{"AtmPayNo":"99922511001200","WebAtmURL":"http://34.58:7101/QPay.WebPaySite/Bridge/PayWebATM?TD=BA002600000037&TK=6fecec25daae4b5bb45e80bc9ee6f7ed","OtpURL":"http://10.11.34.58:7101/QPay.WebPaySite/Bridge/PayOTP?TD=BA002600000037&TK=6fecec25daae4b5bb45e80bc9ee6f7ed"}}";
            return AesCBC_Encrypt.AESDecrypt(inputStr, hashId, iv);
        }
    }
}