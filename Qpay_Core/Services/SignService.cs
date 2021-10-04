using Newtonsoft.Json.Linq;
using Qpay_Core.Services.Common;
using Qpay_Core.Models.ExternalAPI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Threading.Tasks;
using Qpay_Core.Models;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Qpay_Core.Services
{
    public static class SignService
    {
        public static string GetSign<T>(this T inputObj,string nonce) where T : OrderCreateRequestModel
        {
            //計算Sign
            //string data = SignService.SerializeToJson(request);
            //先移除所有空值的參數，參數值前後不可有空白。
            //將剩餘所有參數值依照「參數名稱」由小至大排序 不分大小寫即 A < B and a<
            //B ) )，組成如 param1 = value1 & param2 = value2 的字串 。
            //如為多節點參數則不參與 sign 值演算 。
            //最後使用 SHA256 進行計算 。
            string data = $"Amount={inputObj.Amount}&BackendURL={inputObj.BackendURL}&CurrencyID={inputObj.CurrencyID}&OrderNo={inputObj.OrderNo}&PayType={inputObj.PayType}&PrdtName={inputObj.PrdtName}&ReturnURL={inputObj.BackendURL}&ShopNo={inputObj.ShopNo}"; 

            string hashId = GetHashID();
            string sign = data + nonce + hashId;
            return SHA256_Hash.GetSHA256Hash(sign).ToUpper();
        }

        //internal static string GetSign<TReq>(string nonce, TReq request) where TReq : BaseRequestModel
        //{
        //    string data = SignService.SerializeToJson(request);
        //    string hashId = GetHashID();
        //    string sign = data + nonce + hashId;
        //    return SHA256_Hash.GetSHA256Hash(sign).ToUpper();
        //}

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
                //if (m.GetCustomAttribute<SignExcludeAttribute>() == null)
                //{
                    var x = type.GetProperty(m.Name).GetValue(obj);
                    if (x != null)
                    {
                        if (m.PropertyType.Assembly != type.Assembly && x.GetType().Name != "List`1" && x.GetType().Name != "Dictionary`2")
                        {
                            dic[m.Name] = x.ToString();
                        }
                    }
                //}
            }

            string signString = string.Join("&", dic.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(x => string.Format("{0}={1}", x.Key, x.Value)));   //value為null或空值則不加入sign值計算
            return signString;
        }

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


    }
}
