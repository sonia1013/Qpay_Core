using Newtonsoft.Json;
using QPay.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Hex = QPay.SampleCode.QPayCommon.HexEncoding;

namespace QPay.SampleCode
{
    public static class QPayToolkit
    {
        private static string _currentVersion = "1.0.0";
        private static string _site = ConfigurationManager.AppSettings["QPayWebAPIUrl"];

        #region Public method

        #region 訂單建立 (虛擬帳號、信用卡)
        /// <summary>
        /// 訂單建立 (虛擬帳號、信用卡)
        /// </summary>
        /// <param name="req"></param>
        /// <example>
        /// 串接範例如下:
        /// CreOrder retObj = QPayToolkit.OrderCreate(new CreOrderReq() { ... });
        /// </example>
        public static CreOrder OrderCreate(CreOrderReq req)
        {
            return GetQPayResponse<CreOrderReq, CreOrder>(req, APIService.OrderCreate);
        }
        #endregion

        #region 訂單交易查詢
        /// <summary>
        /// 訂單交易查詢
        /// </summary>
        /// <param name="req"></param>
        /// <example>
        /// 串接範例如下:
        /// QryOrder retObj = QPayToolkit.OrderQuery(new QryOrderReq() { ... });
        /// </example>
        public static QryOrder OrderQuery(QryOrderReq req)
        {
            return GetQPayResponse<QryOrderReq, QryOrder>(req, APIService.OrderQuery);
        }
        #endregion

        #region 訊息查詢服務
        /// <summary>
        /// 訊息查詢服務
        /// </summary>
        /// <param name="req"></param>
        /// <example>
        /// 串接範例如下:
        /// QryOrderPay retObj = QPayToolkit.OrderPayQuery(new QryOrderPayReq() { ... });
        /// </example>
        public static QryOrderPay OrderPayQuery(QryOrderPayReq req)
        {
            return GetQPayResponse<QryOrderPayReq, QryOrderPay>(req, APIService.OrderPayQuery);
        }
        #endregion

        #endregion

        #region Private method

        #region 取得QPay Web API response
        private static TResult GetQPayResponse<TReq, TResult>(TReq request, APIService apiService) where TReq : IQPayReq
        {
            string shopNo = request.ShopNo;
            //由appSettings取得指定商店雜湊值  ex <add key="AA0001" value="...,...,...,..."/>
            string apiKeyData = ConfigurationManager.AppSettings.Get(shopNo);
            if (string.IsNullOrEmpty(apiKeyData))
                throw new Exception("AppSettings.config 中不存在指定商店API Keys");

            //將取得雜湊值以逗號(,)分隔並轉小寫，產生string陣列
            string[] apiKeys = apiKeyData.ToLower().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //產生取Nonce Request
            NonceReq nonceReq = new NonceReq(shopNo);

            //發送Request並取得Nonce Responce
            NonceRes nonceRes = GetNonce(nonceReq).Result;

            if (string.IsNullOrEmpty(nonceRes.Nonce))
                throw new Exception("Nonce值為null或空值");

            int i;
            //1.移除雜湊中的"-"
            //2.取得雜湊的前16碼
            //3.將步驟2結果轉為16進制byte陣列
            List<byte[]> keyList = apiKeys.ToList().Select(x => Hex.GetBytes(x.Replace("-", "").Substring(0, 16), out i)).ToList();

            string
                sha256,
                iv,
                //1.分別將 雜湊A1 XOR 雜湊A2, 雜湊B1 XOR 雜湊B2
                //2.將步驟1的兩個結果各自轉為16進制字串 S1, S2
                //3.AESKey = S1 + S2
                aesKey = Hex.ToString(QPayCommon.XOR(keyList[0], keyList[1])) + Hex.ToString(QPayCommon.XOR(keyList[2], keyList[3])),
                //之前取得之Nonce
                nonce = nonceRes.Nonce,
                //序列化之Request物件
                innerJson = QPayCommon.SerializeToJson(request),
                //利用 AESKey, Nonce進行AESCBC加密，加密內文(提供out SHA256及 out iv可供後續驗證)
                msg = QPayCommon.EncryptAesData(aesKey, innerJson, nonce, out sha256, out iv);

            //產生WebAPIMessage
            WebAPIMessage req = new WebAPIMessage()
            {
                Version = _currentVersion,
                ShopNo = shopNo,
                APIService = apiService.ToString(),
                Nonce = nonce,
                Message = msg,
                //利用Request物件, AESKey及Nonce組成Sign值
                Sign = request.GenerateSign(aesKey, nonce)
            };

            try
            {
                QPayCommon.InfoLog(string.Format("呼叫商業收付API Order/{0} , Request:{1}", req.APIService, QPayCommon.SerializeToJson(req)));

                //呼叫商業收付Web API
                WebAPIMessage result = NewAPI<WebAPIMessage>("Order", req).Result;

                QPayCommon.InfoLog(string.Format("呼叫商業收付API Order/{0} , Response:{1}", req.APIService, QPayCommon.SerializeToJson(result)));

                //利用 AESKey, Nonce進行AESCBC解密，解密內文(提供out SHA256及 out iv可供後續驗證)
                string decodedMsg = QPayCommon.DecryptAesData(aesKey, result.Message, result.Nonce, out sha256, out iv);

                QPayCommon.InfoLog("Response Message:" + decodedMsg);

                //反序列化取得Response物件
                TResult innerResult = JsonConvert.DeserializeObject<TResult>(decodedMsg);

                //Sign值驗證
                string responseSign = innerResult.GenerateSign(aesKey, result.Nonce);
                if (responseSign != result.Sign)
                {
                    string validateFailMsg = "sign value validate fail!! response sign value:" + result.Sign + ", calculate sign value:" + responseSign;

                    QPayCommon.ExceptionLog(validateFailMsg);
                    throw new Exception(validateFailMsg);
                }

                return innerResult;
            }
            catch(Exception ex)
            {
                QPayCommon.ExceptionLog(null, ex);
                throw ex;
            }
        }
        #endregion

        #region APIClient
        #region 呼叫Nonce API(一次性數值)
        private static async Task<NonceRes> GetNonce(NonceReq req)
        {
            string url = _site + "Nonce";

            HttpResponseMessage responce;

            using (var client = new HttpClient())
            {
                responce = client.PostAsJsonAsync(url, req).Result;
            }

            NonceRes res = new NonceRes();

            if (responce.IsSuccessStatusCode)
            {
                res = await responce.Content.ReadAsAsync<NonceRes>();
            }
            else
            {
                QPayCommon.ExceptionLog("Get nonce failed. StatusCode : " + responce.StatusCode);
                res = new NonceRes();
            }

            return res;
        }
        #endregion

        #region 呼叫商店API
        private static async Task<T> NewAPI<T>(string route, WebAPIMessage req) where T : new()
        {
            string url = _site + route;

            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                response = client.PostAsJsonAsync(url, req).Result;
            }

            T res;
            if (response.IsSuccessStatusCode)
            {
                res = await response.Content.ReadAsAsync<T>();
            }
            else
            {
                QPayCommon.ExceptionLog(string.Format("Call API {0} failed. StatusCode : {1}", req.APIService, response.StatusCode));
                throw new Exception(response.Content.ReadAsStringAsync().Result);
            }

            return res;
        }
        #endregion
        #endregion

        #endregion
    }

    public static class QPayCommon
    {
        #region 共用方法
        /// <summary>
        /// 寫入Info log
        /// </summary>
        /// <remarks>可自行實作</remarks>
        public static void InfoLog(string message)
        {
            //Info Log
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 寫入Exception log
        /// </summary>
        /// <remarks>可自行實作</remarks>
        public static void ExceptionLog(string exMessage, Exception ex = null)
        {
            //Excetpion Log
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 序列化物件(忽略null或default值之properties)
        /// </summary>
        public static string SerializeToJson<T>(this T data)
        {
            //value為null或空值則不序列化(空值則需在property上加attribute)
            string result = JsonConvert.SerializeObject(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });
            return result;
        }

        /// <summary>
        /// XOR
        /// </summary>
        /// <remarks>陣列需同長度</remarks>
        public static byte[] XOR(byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
                throw new ArgumentException("arr1 and arr2 are not the same length");

            byte[] result = new byte[arr1.Length];

            for (int i = 0; i < arr1.Length; ++i)
                result[i] = (byte)(arr1[i] ^ arr2[i]);

            return result;
        }

        /// <summary>
        /// byte[] 轉為 16進制字串
        /// </summary>
        public static string ToHex(this byte[] bytes, bool uppercase = false)
        {
            return string.Concat(from x in bytes
                                 select x.ToString(uppercase ? "X2" : "x2"));
        }
        #endregion

        #region Hash, Sign

        /// <summary>
        /// 加密演算法列舉
        /// </summary>
        public enum HashAlgorithmName
        {
            MD5,
            SHA1,
            SHA256,
            SHA512
        }
        /// <summary>
        /// 產生Sign值
        /// </summary>
        /// <remarks>Hash(Sign字串 + Nonce + AESKey), 再將結果轉為16進制字串(小寫)</remarks>
        public static string GenerateSign<T>(this T o, string key, string nonce, [CallerMemberName] string callerMethod = "")
        {
            string sign = string.Concat(GetSigningString(o, callerMethod), nonce, key).Hash(HashAlgorithmName.SHA256, Encoding.UTF8).ToHex(false);
            return sign.ToUpper();
        }

        /// <summary>
        /// 物件產生Sign字串
        /// </summary>
        /// <remarks>忽略有SignExcludeAttribute之properties</remarks>
        public static string GetSigningString<T>(this T obj, [CallerMemberName] string callerMethod = "")
        {
            if (obj == null)
                throw new ArgumentNullException("Request Object is null");
            var dic = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var type = obj.GetType();
            foreach (var p in type.GetMembers().Where(x => x.MemberType == MemberTypes.Property))
            {
                var m = p as PropertyInfo;
                //排除SignExcludeAttribute
                if (m.GetCustomAttribute<SignExcludeAttribute>() == null)
                {
                    var x = type.GetProperty(m.Name).GetValue(obj);
                    if (x != null)
                    {
                        if (m.PropertyType.Assembly != type.Assembly && x.GetType().Name != "List`1" && x.GetType().Name != "Dictionary`2")
                        {
                            dic[m.Name] = x.ToString();
                        }
                    }
                }
            }

            string signString = string.Join("&", dic.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(x => string.Format("{0}={1}", x.Key, x.Value)));   //value為null或空值則不加入sign值計算
            return signString;
        }

        public static byte[] Hash(this string s, HashAlgorithmName name, Encoding encoding = null)
        {
            bool flag = string.IsNullOrWhiteSpace(s);
            if (flag)
            {
                throw new ArgumentNullException();
            }
            bool flag2 = encoding == null;
            if (flag2)
            {
                encoding = Encoding.UTF8;
            }
            return Hash(encoding.GetBytes(s), name);
        }

        public static byte[] Hash(byte[] bytes, HashAlgorithmName name)
        {
            bool flag = bytes == null || bytes.Length == 0;
            if (flag)
            {
                throw new ArgumentNullException();
            }
            byte[] result;
            using (HashAlgorithm algorithm = GetAlgorithm(name))
            {
                result = algorithm.ComputeHash(bytes);
            }
            return result;
        }

        /// <summary>
        /// 取得加密演算法
        /// </summary>
        public static HashAlgorithm GetAlgorithm(HashAlgorithmName algoName)
        {
            HashAlgorithm result;
            switch (algoName)
            {
                case HashAlgorithmName.MD5:
                    result = new MD5CryptoServiceProvider();
                    break;
                case HashAlgorithmName.SHA1:
                    result = new SHA1Managed();
                    break;
                case HashAlgorithmName.SHA256:
                    result = new SHA256Managed();
                    break;
                case HashAlgorithmName.SHA512:
                    result = new SHA512Managed();
                    break;
                default:
                    result = null;
                    break;
            }
            return result;
        }
        #endregion
        
        #region Encrypt
        /// <summary>
        /// AES CBC加密
        /// </summary>
        /// <param name="aesKey">AESKey</param>
        /// <param name="data">欲加密內文</param>
        /// <param name="nonce">Nonce</param>
        /// <param name="sha256O">SHA值, 由Nonce 進行SHA256加密後取得</param>
        /// <param name="ivO">iv值, SHA值末16碼</param>
        public static string EncryptAesData(string aesKey, string data, string nonce, out string sha256O, out string ivO)
        {
            string sha256 = sha256O = GetSHA256Hash(nonce), iv = ivO = sha256.Substring(sha256.Length - 16, 16), msg = EncryptAesCBC(data, aesKey, iv);
            return msg;
        }

        /// <summary>
        /// AES CBC解密
        /// </summary>
        /// <param name="aesKey">AESKey</param>
        /// <param name="cipherText">已加密內容</param>
        /// <param name="nonce">Nonce</param>
        /// <param name="sha256O">SHA值, 由Nonce 進行SHA256加密後取得</param>
        /// <param name="ivO">iv值, SHA值末16碼</param>
        public static string DecryptAesData(string aesKey, string cipherText, string nonce, out string sha256O, out string ivO)
        {
            string sha256 = sha256O = GetSHA256Hash(nonce), iv = ivO = sha256.Substring(sha256.Length - 16, 16), data = DecryptAesCBC(cipherText, aesKey, iv);
            return data;
        }

        /// <summary>
        /// 字串進行SHA256加密
        /// </summary>
        public static string GetSHA256Hash(string input)
        {
            SHA256 sha256Hasher = SHA256.Create();
            
            byte[] data = sha256Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            return EncodingByteArraytoHexString(data);
        }

        /// <summary>
        /// Byte陣列轉為16進制字串
        /// </summary>
        private static string EncodingByteArraytoHexString(byte[] data)
        {
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            { 
                sBuilder.Append(data[i].ToString("X2"));
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// AES CBC加密
        /// </summary>
        private static string EncryptAesCBC(string source, string key, string iv)
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

        /// <summary>
        /// AES CBC解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        private static string DecryptAesCBC(string cipherText, string key, string iv)
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

        //16進制相關方法
        #region HexEncoding
        public class HexEncoding
        {
            public HexEncoding()
            {
            }
            
            public static int GetByteCount(string hexString)
            {
                int numHexChars = 0;
                char c;

                for (int i = 0; i < hexString.Length; i++)
                {
                    c = hexString[i];
                    if (IsHexDigit(c))
                        numHexChars++;
                }
                if (numHexChars % 2 != 0)
                {
                    numHexChars--;
                }
                return numHexChars / 2;
            }

            public static byte[] GetBytes(string hexString, out int discarded)
            {
                discarded = 0;
                string newString = "";
                char c;
                for (int i = 0; i < hexString.Length; i++)
                {
                    c = hexString[i];
                    if (IsHexDigit(c))
                        newString += c;
                    else
                        discarded++;
                }
                if (newString.Length % 2 != 0)
                {
                    discarded++;
                    newString = newString.Substring(0, newString.Length - 1);
                }

                int byteLength = newString.Length / 2;
                byte[] bytes = new byte[byteLength];
                string hex;
                int j = 0;
                for (int i = 0; i < bytes.Length; i++)
                {
                    hex = new string(new char[] { newString[j], newString[j + 1] });
                    bytes[i] = HexToByte(hex);
                    j = j + 2;
                }
                return bytes;
            }
            public static string ToString(byte[] bytes)
            {
                string hexString = "";
                for (int i = 0; i < bytes.Length; i++)
                {
                    hexString += bytes[i].ToString("X2");
                }
                return hexString;
            }

            public static bool InHexFormat(string hexString)
            {
                bool hexFormat = true;

                foreach (char digit in hexString)
                {
                    if (!IsHexDigit(digit))
                    {
                        hexFormat = false;
                        break;
                    }
                }
                return hexFormat;
            }
            
            public static bool IsHexDigit(char c)
            {
                int numChar;
                int numA = Convert.ToInt32('A');
                int num1 = Convert.ToInt32('0');
                c = char.ToUpper(c);
                numChar = Convert.ToInt32(c);
                if (numChar >= numA && numChar < (numA + 6))
                    return true;
                if (numChar >= num1 && numChar < (num1 + 10))
                    return true;
                return false;
            }
            
            private static byte HexToByte(string hex)
            {
                if (hex.Length > 2 || hex.Length <= 0)
                    throw new ArgumentException("hex must be 1 or 2 characters in length");
                byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                return newByte;
            }
        }
        #endregion
        #endregion        
    }
}

namespace QPay.Domain
{
    #region SignExcludeAttribute
    /// <summary>
    /// SignExclude
    /// </summary>
    /// <remarks>有此Attribute的properties不加入Sign計算</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class SignExcludeAttribute : Attribute
    {
    }
    #endregion

    #region APIService
    /// <summary>
    /// 商業收付API列舉
    /// </summary>
    public enum APIService
    {
        OrderCreate = 1,
        OrderQuery = 2,
        OrderPayQuery = 3,
    }
    #endregion

    #region Request物件

    #region BaseReq
    public interface IQPayReq
    {
        string ShopNo { get; set; }
    }
    #endregion

    #region NonceReq
    public class NonceReq
    {
        [DataMember]
        public string ShopNo { get; set; }

        public NonceReq(string shopNo)
        {
            ShopNo = shopNo;
        }
    }
    #endregion

    #region CreOrderReq
    [DataContract]
    public class CreOrderReq : IQPayReq
    {
        /// <summary>
        /// 會員編號，例如AA0001_001
        /// </summary>
        [DataMember]
        public string ShopNo { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        /// <remarks>
        /// 不可有單引號、雙引號、百分比, 最大長度50
        /// </remarks>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>
        /// 訂單金額
        /// </summary>
        [DataMember]
        public int Amount { get; set; }

        /// <summary>
        /// 幣別
        /// </summary>
        [DataMember]
        public string CurrencyID { get; set; }
        
        /// <summary>
        /// 收款名稱
        /// </summary>
        /// <remarks>
        /// 不可有單引號、雙引號、百分比, 最大長度60
        /// </remarks>
        [DataMember]
        public string PrdtName { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        /// <remarks>
        /// 不可有單引號、雙引號、百分比, 最大長度30
        /// </remarks>
        [DataMember]
        public string Memo { get; set; }

        /// <summary>
        /// 自訂參數一
        /// </summary>
        /// <remarks>
        /// 不可有單引號、雙引號、百分比, 最大長度255
        /// </remarks>
        [DataMember]
        public string Param1 { get; set; }

        /// <summary>
        /// 自訂參數二
        /// </summary>
        /// <remarks>
        /// 不可有單引號、雙引號、百分比, 最大長度255
        /// </remarks>
        [DataMember]
        public string Param2 { get; set; }

        /// <summary>
        /// 自訂參數三
        /// </summary>
        /// <remarks>
        /// 不可有單引號、雙引號、百分比, 最大長度255
        /// </remarks>
        [DataMember]
        public string Param3 { get; set; }

        /// <summary>
        /// 付款完成轉入URL
        /// </summary>
        /// <remarks>
        /// 不可有單引號、雙引號、百分比, 最大長度255
        /// </remarks>
        [DataMember]
        public string ReturnURL { get; set; }

        /// <summary>
        /// 付款完成背端通知URL
        /// </summary>
        /// <remarks>
        /// 不可有單引號、雙引號、百分比, 最大長度255
        /// </remarks>
        [DataMember]
        public string BackendURL { get; set; }

        /// <summary>
        /// 收款方式(A:ATM轉帳,C:信用卡)
        /// </summary>
        [DataMember]
        public string PayType { get; set; }

        /// <summary>
        /// ATM參數
        /// </summary>
        [DataMember]
        public ATMParam ATMParam { get; set; }

        /// <summary>
        /// 信用卡參數
        /// </summary>
        [DataMember]
        public CardParam CardParam { get; set; }
    }

    #region ATMParam - 虛擬帳號參數
    /// <summary>
    /// 訂單建立ATM參數
    /// </summary>
    [DataContract]
    public class ATMParam
    {
        /// <summary>
        /// 付款截止日期
        /// </summary>
        [DataMember]
        public string ExpireDate { get; set; }
    }
    #endregion

    #region CardParam - 信用卡參數
    /// <summary>
    /// 訂單建立信用卡參數
    /// </summary>
    [DataContract]
    public class CardParam
    {
        /// <summary>
        /// 自動請款(信用卡)
        /// </summary>
        [DataMember]
        public string AutoBilling { get; set; }

        /// <summary>
        /// 預計自動請款天數
        /// </summary>
        [DataMember]
        public int? ExpBillingDays { get; set; }

        /// <summary>
        /// 訂單有效時間(分鐘)
        /// </summary>
        [DataMember]
        public int? ExpMinutes { get; set; }

        /// <summary>
        /// 收款方式-子項
        /// </summary>
        [DataMember]
        public string PayTypeSub { get; set; }

        /// <summary>
        /// 期數
        /// </summary>
        [DataMember]
        public string Staging { get; set; }

        /// <summary>
        /// 快速付款 Token
        /// </summary>
        [DataMember]
        public string CCToken { get; set; }
    }
    #endregion

    #endregion

    #region QryOrderReq
    [DataContract]
    public class QryOrderReq : IQPayReq
    {
        /// <summary>
        /// 會員編號，例如AA0001
        /// </summary>
        [DataMember]
        public string ShopNo { get; set; }

        /// <summary>
        /// 用戶訂單編號(不可有單引號、雙引號、百分比)
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>
        /// 收款方式
        /// </summary>
        /// <remarks>
        /// A: ATM轉帳
        /// C: 信用卡
        /// </remarks>
        [DataMember]
        public string PayType { get; set; }

        /// <summary>
        /// 交易日期(起)，例如2017/5/3 00:00則帶201705030000
        /// </summary>
        [DataMember]
        public string OrderDateTimeS { get; set; }

        /// <summary>
        /// 交易日期(迄)，例如2017/5/3 23:59則帶201705032359
        /// </summary>
        [DataMember]
        public string OrderDateTimeE { get; set; }

        /// <summary>
        /// 付款日期(起)，例如2017/5/3 00:00則帶201705030000
        /// </summary>
        [DataMember]
        public string PayDateTimeS { get; set; }

        /// <summary>
        /// 付款日期(迄)，例如2017/5/3 23:59則帶201705032359
        /// </summary>
        [DataMember]
        public string PayDateTimeE { get; set; }

        /// <summary>
        /// 依付款狀態為條件查詢
        /// </summary>
        /// <remarks>
        /// 1.若 PayType 為“A” (ATM 轉帳)
        ///     Y：完成付款 (已轉帳)
        ///     N：未完成付款（未轉帳、逾期未轉）
        /// 2.若 Paytype 為“C” (信用卡)
        ///     Y：已請款的訂單
        ///     N：未請款的訂單（含：未付款、待請款、取消授權、授權逾期）
        /// </remarks>
        [DataMember]
        public string PayFlag { get; set; }
    }
    #endregion

    #region QryOrderPayReq
    [DataContract]
    public class QryOrderPayReq : IQPayReq
    {
        /// <summary>
        /// 會員編號，例如AA0001
        /// </summary>
        [DataMember]
        public string ShopNo { get; set; }

        /// <summary>
        /// Token值
        /// </summary>
        [DataMember]
        public string PayToken { get; set; }
    }
    #endregion

    #endregion

    #region Response, Domain物件

    #region NonceRes
    public class NonceRes
    {
        [DataMember]
        public string Nonce { get; set; }
    }
    #endregion

    #region WebAPIMessage
    [DataContract]
    public class WebAPIMessage
    {
        /// <summary>
        /// API版本
        /// </summary>
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        /// 會員編號，例如AA0001_001
        /// </summary>
        [DataMember]
        public string ShopNo { get; set; }

        [DataMember]
        public string APIService { get; set; }

        /// <summary>
        /// 簽章值
        /// </summary>
        [DataMember]
        [SignExclude]
        public string Sign { get; set; }

        /// <summary>
        /// 一次性數值
        /// </summary>
        [DataMember]
        public string Nonce { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
    #endregion

    #region CreOrder
    [DataContract]
    public class CreOrder
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 會員編號，例如AA0001
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string ShopNo { get; set; }

        /// <summary>
        /// 網易收交易編號，例如AA00010000001
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string TSNo { get; set; }
        
        /// <summary>
        /// 訂單金額
        /// </summary>
        [DataMember]
        public int Amount { get; set; }

        /// <summary>
        /// 處理狀態(S:處理成功(正常), F:處理失敗(錯誤))
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string Status { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string Description { get; set; }

        /// <summary>
        /// 自訂參數一
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string Param1 { get; set; }

        /// <summary>
        /// 自訂參數二
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string Param2 { get; set; }

        /// <summary>
        /// 自訂參數三
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string Param3 { get; set; }

        /// <summary>
        /// 收款方式
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string PayType { get; set; }

        /// <summary>
        /// 虛擬帳號回傳參數
        /// </summary>
        [DataMember]
        public QPay.Domain.Response.ATMParam ATMParam { get; set; }

        /// <summary>
        /// 信用卡回傳參數
        /// </summary>
        [DataMember]
        public QPay.Domain.Response.CardParam CardParam { get; set; }
    }

    namespace QPay.Domain.Response
    {
        #region 虛擬帳號回傳參數
        [DataContract]
        public class ATMParam
        {
            /// <summary>
            /// 虛擬帳號，長度為14碼
            /// </summary>
            [DataMember]
            [DefaultValue("")]
            public string AtmPayNo { get; set; }

            /// <summary>
            /// 使用WebATM付款的URL
            /// </summary>
            [DataMember]
            [DefaultValue("")]
            public string WebAtmURL { get; set; }

            /// <summary>
            /// 使用OTP付款的URL
            /// </summary>
            [DataMember]
            [DefaultValue("")]
            public string OtpURL { get; set; }

            /// <summary>
            /// 金融機構代碼
            /// </summary>
            [DataMember]
            [DefaultValue("")]
            public string BankNo { get; set; }


            /// <summary>
            /// 轉帳帳號
            /// </summary>
            [DataMember]
            [DefaultValue("")]
            public string AcctNo { get; set; }
        }
        #endregion

        #region 信用卡回傳參數
        [DataContract]
        public class CardParam
        {
            /// <summary>
            /// 使用信用卡付款URL
            /// </summary>
            [DataMember]
            [DefaultValue("")]
            public string CardPayURL { get; set; }

            /// <summary>
            /// 快速付款 Token
            /// </summary>
            [DataMember]
            [DefaultValue("")]
            public string CCToken { get; set; }
        }
        #endregion
    }
    #endregion

    #region QryOrder
    [DataContract]
    public class QryOrder
    {
        /// <summary>
        /// 會員編號，例如AA0001_001
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string ShopNo { get; set; }

        /// <summary>
        /// 訊息時間, yyyyMMddHHmm
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string Date { get; set; }

        /// <summary>
        /// 處理狀態, S:處理成功(正常) / F:處理失敗(錯誤)
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string Status { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <remarks>
        /// 若Status為F時，則帶入錯誤訊息，為s時，則為S00000或S00001
        /// </remarks>
        [DataMember]
        [DefaultValue("")]
        public string Description { get; set; }

        /// <summary>
        /// 訂單資訊
        /// </summary>
        [DataMember]
        public IList<OrderInfo> OrderList { get; set; }
    }
    #endregion

    #region OrderInfo
    [DataContract]
    public class OrderInfo
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 網易收交易編號
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string TSNo { get; set; }

        /// <summary>
        /// 交易時間, yyyyMMddHHmm
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string TSDate { get; set; }

        /// <summary>
        /// 信用卡授權時間, yyyyMMddHHmm
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string ApprovedDate { get; set; }

        /// <summary>
        /// 付款時間/請款時間, yyyyMMddHHmm
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string PayDate { get; set; }

        /// <summary>
        /// 訂單金額
        /// </summary>
        /// <remarks>
        /// 包含小數二位，例如180元則會回傳18000
        /// </remarks>
        [DataMember]
        public int Amount { get; set; }

        /// <summary>
        /// 收款方式
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string PayType { get; set; }

        /// <summary>
        /// 訂單付款狀態
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string PayStatus { get; set; }

        /// <summary>
        /// 訂單有效期限, yyyyMMddHHmm
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string ExpireDate { get; set; }

        /// <summary>
        /// 退款註記, Y：有退款/N：無退款
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string RefundFlag { get; set; }

        /// <summary>
        /// 退款狀態
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string RefundStatus { get; set; }

        /// <summary>
        /// 申請退款日期
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string RefundDate { get; set; }

        /// <summary>
        /// 收款名稱
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string PrdtName { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string Memo { get; set; }

        /// <summary>
        /// 自訂參數一
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string Param1 { get; set; }

        /// <summary>
        /// 自訂參數二
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string Param2 { get; set; }

        /// <summary>
        /// 自訂參數三
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string Param3 { get; set; }

        /// <summary>
        /// 虛擬帳號回傳參數
        /// </summary>
        [DataMember]
        public QPay.Domain.Response.ATMParam ATMParam { get; set; }

        /// <summary>
        /// 信用卡回傳參數
        /// </summary>
        [DataMember]
        public QPay.Domain.Response.CardParam CardParam { get; set; }
    }
    #endregion

    #region QryOrderPay
    [DataContract]
    public class QryOrderPay
    {
        [DataMember]
        [DefaultValue("")]
        public string ShopNo { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string PayToken { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string Date { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string Status { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string Description { get; set; }

        [DataMember]
        public TSResult TSResultContent { get; set; }
    }
    #endregion

    #region TSResult
    [DataContract]
    public class TSResult
    {
        [DataMember]
        [DefaultValue("")]
        public string APType { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string TSNo { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string OrderNo { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string ShopNo { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string PayType { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string Amount { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string Status { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string Description { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string Param1 { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string Param2 { get; set; }

        [DataMember]
        [DefaultValue("")]
        public string Param3 { get; set; }

        /// <summary>
        /// 快速付款 Token
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string CCToken { get; set; }
    }
    #endregion

    #endregion
}
