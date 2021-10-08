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
    public class OrderController : Controller
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
            try
            {
                BaseResponseModel result = await _orderService.OrderCreate(orderCreate);
                return result;
            }
            catch(Exception e)
            {
                //throw e;
                _logger.LogError(e, $"建立訂單失敗{e.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("QueryPayOrder")]
        public async Task<ActionResult<OrderPayQueryRes>> QueryPayStatusAsync(OrderPayQueryReq orderInfo)
        {
            ////--123456789987654321
            //orderInfo = new OrderPayQueryReq()
            //{
            //    //ShopNo="",
            //    APIService = APIService.OrderPayQuery,
            //    Sign = "",
            //    Message = "",
            //    Nonce = "",
            //    PayToken = "",
            //};
            try
            {
                OrderPayQueryRes result = await _orderService.OrderPayQuery(orderInfo);
                return result;
            }
            catch (Exception e)
            {
                //throw e;
                return StatusCode(500);
            }
        }


    }
}
