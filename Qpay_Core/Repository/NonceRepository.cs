using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Qpay_Core.Repository;
using Qpay_Core.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;

namespace Qpay_Core.Repository
{
    public class NonceRepository
    {
        private readonly ILogger<NonceRepository> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public NonceRepository(ILogger<NonceRepository> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public async Task<string> GetNonce(string shopNo)
        {
            string nonce = string.Empty;
            //HttpClient client = _clientFactory.CreateClient("shortUrls");
            string Url = "https://apisbx.sinopac.com/funBIZ/QPay.WebAPI/api/Nonce";
            try
            {
                //HttpResponseMessage response = await _clientFactory.CreateClient("shortUrls").PostAsync();
                using (HttpResponseMessage response = await _clientFactory.CreateClient("shortUrls").PostAsync(Url, new JsonContent(new { ShopNo = shopNo })))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        nonce += result;
                    }
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        //logger.Error($"GetAsync Failed, url:{Url}, HttpStatusCode:{response.StatusCode}, result:{result}");
                        return "404 error";
                    }
                }
                return nonce;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
    }

    public class JsonContent : StringContent
    {
        public JsonContent(Object obj) : base(JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8, "application/json")
        {

        }
    }

}
