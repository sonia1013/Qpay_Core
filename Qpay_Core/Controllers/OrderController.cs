using Microsoft.AspNetCore.Mvc;
using Qpay_Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qpay_Core.Controllers
{
    public class OrderController : Controller
    {
        private readonly IQpayRepository _qPayRepository;
        public OrderController(IQpayRepository nonceRepository)
        {
            _qPayRepository = nonceRepository;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        //public async ActionResult CreateOrderAsync()
        //{
        //    QpayRepository;
        //    return ;
        //}
    }
}
