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

        public static string GetSign<T>(this T inputObj,string nonce) where T : OrderCreateReqModel
        {
            //計算Sign
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(inputObj);
            //先移除所有空值的參數，參數值前後不可有空白。
            //將剩餘所有參數值依照「參數名稱」由小至大排序 不分大小寫即 A < B and a<
            //B ) )，組成如 param1 = value1 & param2 = value2 的字串 。
            //如為多節點參數則不參與 sign 值演算 。
            //最後使用 SHA256 進行計算 。
            //string data = $"Amount={inputObj.Amount}&BackendURL={inputObj.BackendURL}&CurrencyID={inputObj.CurrencyID}&OrderNo={inputObj.OrderNo}&PayType={inputObj.PayType}&PrdtName={inputObj.PrdtName}&ReturnURL={inputObj.BackendURL}&ShopNo={inputObj.ShopNo}"; 
            string data = GetSigningString<T>(inputObj);
            string hashId = QPayCommon.GetHashID();
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
    }
}
