﻿using System;
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
            _logger = logger;
        }

        //public async Task<BaseResponseModel> GetQPayResponse(OrderCreateReqModel request)
        private async Task<TResult> GetQPayResponse<TReq, TResult>(TReq apirequest, APIService apiService) where TReq : BaseReqModel    //其實主要是傳入ShopNo
        {
            string shopNo = apirequest.ShopNo;
            //DateTime dateTime = DateTime.Now;
            //string timeStr = dateTime.ToString("yyyymmddhhmm");
            string decodedMsg = "";

            //var qPayRequest = new OrderCreateReqModel()
            //{
            //    ShopNo = shopNo,
            //    OrderNo = "A" + timeStr, //Get Set 其實改成為唯讀欄位+時間戳記就好了
            //    Amount = apirequest.,
            //    CurrencyID = "TWD",
            //    PayType = "A",
            //    //ATMParam = new ATMParam() { ExpireDate = "20211108" },
            //    CardParam=new CardParam() {AutoBilling="Y" },
            //    PrdtName = "baby kimchi",
            //    ReturnURL="https://localhost:5001/checkout/return",
            //    BackendURL="https://localhost:5001/home/success"
            //};

            //取得nonce值
            NonceRequestModel nonceReq = new NonceRequestModel() { ShopNo = shopNo };
            string nonce;
            try
            {
                nonce = _qPayRepository.CreateNonceAsync(nonceReq).Result;
                
                if (string.IsNullOrEmpty(nonce))
                    throw new Exception("Nonce值為null或空值");
            }
            catch (Exception ex)
            {
                throw ex ;
            } 
            
            //取得HashID
            string hashId = QPayCommon.GetHashID();

            //計算IV
            string iv = QPayCommon.CalculateIVbyNonce(nonce);

            string jsonData = QPayCommon.SerializeToJson(apirequest);
            _logger.LogInformation("加密內文="+JsonConvert.SerializeObject(jsonData));
            
            //取得加密內文
            string message = AesCBC_Encrypt.AESEncrypt(jsonData, hashId, iv);

            //產生WebAPIMessage
            BaseApiMessage req = new BaseApiMessage()
            {
                //Version = _currentVersion,
                ShopNo = shopNo,
                APIService = apiService,
                Nonce = nonce,
                Message = message,
                //利用Request物件, AESKey及Nonce組成Sign值
                Sign = SignService.GetSign(apirequest, nonce)
            };
            
            try
            {
                _logger.LogWarning(string.Format("呼叫商業收付API Order/{0} , Request:{1}", apiService, QPayCommon.SerializeToJson(req)));

                //呼叫商業收付Web API
                BaseResponseModel result = await _qPayRepository.CreateApiAsync<BaseResponseModel>("Order", req);
                
                
                while (result.Message != "")
                {
                    _logger.LogWarning(string.Format("呼叫商業收付API Order/{0} , Response:{1}", apiService, result.Message));
                    decodedMsg += AesCBC_Encrypt.DecryptAesCBC(result.Message, hashId, result.Nonce);
                    break;
                }
                
                _logger.LogInformation("Response Message:" + decodedMsg);

                TResult innerResult = JsonConvert.DeserializeObject<TResult>(decodedMsg);

                return innerResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                _logger.LogError("串接API時發生錯誤", ex);
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


        public async Task<OrderCreateRes> OrderCreate(OrderCreateReq req)
        {
            var result = await GetQPayResponse<OrderCreateReq, OrderCreateRes>(req, APIService.OrderCreate);
            return result;
        }

        public async Task<OrderPayQueryRes> OrderPayQuery(OrderPayQueryReq req)
        {
            var result = await GetQPayResponse<OrderPayQueryReq, OrderPayQueryRes>(req, APIService.OrderCreate);
            return result;
        }

        public async Task<OrderQueryRes> OrderQuery(OrderQueryReq req)
        {
            var result = await GetQPayResponse<OrderQueryReq, OrderQueryRes>(req, APIService.OrderCreate);
            return result;
        }
    }
}
