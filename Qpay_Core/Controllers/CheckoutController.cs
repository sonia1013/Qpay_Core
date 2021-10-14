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
    //[Route("[controller]")]
    public class CheckoutController : Controller
    {
        private readonly IQpayRepository _qPayRepository;
        private readonly IOrderService _orderService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(IQpayRepository nonceRepository, IOrderService orderService,ILogger<CheckoutController> logger)
        {
            _qPayRepository = nonceRepository;
            _orderService = orderService;
            _logger = logger;

        }
        
        public IActionResult Index()
        {
            return View();
        } 

        public IActionResult Pay()
        {
            return View();
        } 


    }
}
