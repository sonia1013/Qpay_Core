using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Qpay_Core.Models;
using Qpay_Core.Models.ExternalAPI;
using Qpay_Core.Repository.Interfaces;
using Qpay_Core.Services;
using Qpay_Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qpay_Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IQpayRepository _qPayRepository;
        private readonly IOrderService _orderService;
        public OrderController(IQpayRepository nonceRepository, IOrderService orderService,ILogger<OrderController> logger)
        {
            _qPayRepository = nonceRepository;
            _orderService = orderService;
            _logger = logger;
        }
        
        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpPost("CreateOrder")]
        public async Task<ActionResult<BaseResponseModel>> CreateOrderAsync(OrderCreateReq orderCreate)
        {
            _logger.LogInformation($"嘗試建立訂單:{orderCreate.OrderNo}");
            try
            {
                BaseResponseModel result = await _orderService.OrderCreate(orderCreate);
                return result;
            }
            catch(Exception e)
            {
                _logger.LogError($"建立訂單失敗{e.Message}");
                throw e;
            }
        }


        //[HttpPost("QueryOrder")]
        //public async Task<ActionResult<BaseResponseModel>> QueryOrderAsync(OrderCreateReq orderCreate)
        //{
        //    _logger.LogInformation("Log message in the Index() method");
        //    try
        //    {
        //        BaseResponseModel result = await _orderService.OrderCreate(orderCreate);
        //        return result;
        //    }
        //    catch(Exception e)
        //    {
        //        //throw e;
        //        _logger.LogError($"建立訂單失敗{e.Message}");
        //        return StatusCode(500);
        //    }
        //}

        ///// <summary>
        ///// 確認訂單付款是否成功
        ///// </summary>
        ///// <param name="orderInfo"></param>
        ///// <returns></returns>
        //[HttpPost("QueryPayOrder")]
        //public async Task<ActionResult<OrderPayQueryRes>> QueryPayStatusAsync(OrderPayQueryReq orderInfo)
        //{
        //    /*
        //        在 「 7 .1 建立 訂單交易 」 有 傳送 BackendURL 且該訂單 「 付款完成 」
        //        或 「 指定預計自動請款 」完成請款 時則會 透過此通知 傳送 訊息 Token 值 給
        //        您 您 收到後需回覆接收成功 ，如未回覆系統 會每隔 10 分鐘重新發送一次，
        //        直到失敗 5 次後才會停止發送 。
        //     */
        //    var request = new ReturnUrlModel
        //    {
        //        ShopNo = "",
        //        PayToken = ""
        //    };
        //    try
        //    {
        //        OrderPayQueryRes result = await _orderService.OrderPayQuery(orderInfo); //透過本 API 介面，傳遞 PayToken 參數查詢 訂單內容 相關訊息 。
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        //throw e;
        //        return StatusCode(500);
        //    }
        //}


    }
}
