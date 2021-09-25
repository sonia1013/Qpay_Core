using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Qpay_Core.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using System.Text;
using Qpay_Core.Models.ExternalAPI;
using Qpay_Core.Repository.Interfaces;
using Qpay_Core.Models;

namespace Qpay_Core.Repository
{
    public class QpayRepository:IQpayRepository
    {
        private readonly ILogger<QpayRepository> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public QpayRepository(ILogger<QpayRepository> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public async Task<string> CreateNonceAsync(NonceRequestModel nonceRequest)
        {
            string result = string.Empty;
            // 建立 HttpClient 實例
            var httpClient = _clientFactory.CreateClient("funBizApi");
            httpClient.BaseAddress = new Uri("https://apisbx.sinopac.com/funBIZ/QPay.WebAPI/api/");
            var contentPost = new StringContent(
                JsonConvert.SerializeObject(nonceRequest), Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await httpClient.PostAsync("Nonce", contentPost);

            if (response.IsSuccessStatusCode)
            {
                string jsonRes = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<NonceResModel>(jsonRes).Nonce;
            }
            else
            {
                _logger.LogError("Get nonce failed. StatusCode : " + response.StatusCode);
            }
            return result;
        }


        public async Task<BaseResponseModel> CreateApiAsync(string route, BaseRequestModel request)
        {
            var result = new BaseResponseModel();
            HttpClient httpClient = _clientFactory.CreateClient("funBizApi");
            httpClient.BaseAddress = new Uri("https://apisbx.sinopac.com/funBIZ/QPay.WebAPI/api/");
            var contentPost = new StringContent(
                JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            try
            {
                //HttpResponseMessage response = await _clientFactory.CreateClient("shortUrls").PostAsync();
                using (HttpResponseMessage response = await httpClient.PostAsync(route, contentPost))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string resultStr = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<BaseResponseModel>(resultStr);
                    }
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        //logger.Error($"Get nonce failed. StatusCode , HttpStatusCode:{response.StatusCode}, result:{result}");
                        result = new BaseResponseModel();
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return result;
        }
    }

    //public class JsonContent : StringContent
    //{
    //    public JsonContent(Object obj) : base(JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8, "application/json")
    //    {

    //    }
    //}

}
