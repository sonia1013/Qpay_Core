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
using System.Web;
using Microsoft.AspNetCore.Http;

namespace Qpay_Core.Repository
{
    public class QpayRepository : IQpayRepository
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


        public async Task<T> CreateApiAsync<T>(string route, BaseRequestModel request) where T:new()
        {
            T result;
            HttpClient httpClient = _clientFactory.CreateClient("funBizApi");
            httpClient.BaseAddress = new Uri("https://apisbx.sinopac.com/funBIZ/QPay.WebAPI/api/");
            var contentPost = new StringContent(
                JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            try
            {
                //HttpResponseMessage response = await _clientFactory.CreateClient("shortUrls").PostAsync();
                using (response = await httpClient.PostAsync(route, contentPost))
                {
                    _logger.LogWarning(response.ToString());
                    if (response.IsSuccessStatusCode)
                    {
                        string resultStr = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<T>(resultStr);
                        return result;
                    }
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        _logger.LogError("Get Qpay {0} service failed. StatusCode , HttpStatusCode:{1}, result:{2}", request.APIService.ToString(), response.StatusCode, response.ToString());
                        throw new WebException("呼叫API錯誤"+response.ToString());
                    }
                }
            }
            catch (Exception ex)
            { 
                throw (ex);
            }
            // Message += "呼叫API錯誤" + response.Content.ReadAsStringAsync().Result.ToString();

        }
    }

}
