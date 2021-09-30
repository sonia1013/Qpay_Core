using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qpay_Core.Repository.Interfaces;
using Qpay_Core.Services.Interfaces;
using Qpay_Core.Services;
using Qpay_Core.Models;
using Qpay_Core.Models.ExternalAPI;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Qpay_Core.Repository;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Qpay_Core.Services.Common;
using System.Configuration;
using System.Security.Policy;

namespace Qpay_Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IQpayRepository _qPayRepository;
        

        public OrderService(ILogger<OrderService> logger, IQpayRepository qPayRepository)
        {
            _qPayRepository = qPayRepository;
            
        }

        //public async Task<BaseResponseModel> OrderCreateAsync(OrderCreateRequestModel request)
        public async Task<BaseResponseModel> OrderCreateAsync<TReq, TResult>(TReq request, APIService apiService) where TReq : BaseRequestModel
        {
            DateTime dateTime = DateTime.Now;
            string timeStr = dateTime.ToString("yyyymmddhhmm");
            //var request = new OrderCreateRequestModel()
            //{
            //    ShopNo = "NA0249_001",
            //    OrderNo = "A"+timeStr,
            //    Amount = 50000,
            //    CurrencyID = "TWD",
            //    PayType = "A",
            //    ATMParam = new ATMParam() { ExpireDate = "20210930" },
            //    PrdtName = "baby kimchi",
            //};

            string shopNo = request.ShopNo;
            NonceRequestModel nonceReq = new NonceRequestModel() { ShopNo = shopNo };
            string nonce = await _qPayRepository.CreateNonceAsync(nonceReq);

            if (string.IsNullOrEmpty(nonce))
                throw new Exception("Nonce值為null或空值");
            //取得HashID
            string hashId = SignService.GetHashID();
            string jsonData = JsonConvert.SerializeObject(request);

            //計算IV
            string hashed_nonce = SHA256_Hash.GetSHA256Hash(nonce).ToUpper().PadRight(16);
            string iv = hashed_nonce.Remove(0, 48);
            string message = AesCBC_Encrypt.EncryptAesCBC(jsonData, hashId,iv);


            //產生WebAPIMessage
            BaseRequestModel req = new BaseRequestModel()
            {
                //Version = _currentVersion,
                //ShopNo = shopNo,
                APIService = APIService.OrderCreate,
                Nonce = nonce,
                Message = message,
                //利用Request物件, AESKey及Nonce組成Sign值
                Sign = SignService.GetSign(nonce, request)
            };
            try
            {
                _logger.LogWarning(string.Format("呼叫商業收付API Order/{0} , Request:{1}", req.APIService, req.Message));

                //呼叫商業收付Web API
                var result = await _qPayRepository.CreateApiAsync("Order", req);

                _logger.LogWarning(string.Format("呼叫商業收付API Order/{0} , Response:{1}", req.APIService, result.Message));
                string decodedMsg = AesCBC_Encrypt.DecryptAesCBC(result.Message, hashId, result.Nonce);

                _logger.LogInformation("Response Message:" + decodedMsg);

                var innerResult = JsonConvert.DeserializeObject<BaseResponseModel>(decodedMsg);
          
                return innerResult;
            }
            catch (Exception ex)
            {
                //_logger.LogWarning(null, ex);
                throw ex;
            }
        }

        private static void GetHashKeys(string shopNo)
        {
            //由appSettings取得指定商店雜湊值  ex <add key="AA0001" value="...,...,...,..."/>
            string apiKeyData = ConfigurationManager.AppSettings.Get(shopNo);
            if (string.IsNullOrEmpty(apiKeyData))
                throw new Exception("AppSettings.config 中不存在指定商店API Keys");

            //將取得雜湊值以逗號(,)分隔並轉小寫，產生string陣列
            string[] apiKeys = apiKeyData.ToLower().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            int i;
            //1.移除雜湊中的"-"
            //2.取得雜湊的前16碼
            //3.將步驟2結果轉為16進制byte陣列
            //List<byte[]> keyList = apiKeys.ToList().Select(x => Hex.GetBytes(x.Replace("-", "").Substring(0, 16), out i)).ToList();
        }
    }
}
