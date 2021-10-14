using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qpay_Core.Models.ViewModels
{
    public class PaymentDetail
    {
        public DateTime OrderTime { get; set; }
        public string Name { get; set; }
        public int TotalAmount { get; set; }
        public string CardNumbers { get; set; }
        public string CardExpireDate { get; set; }
        public string CardCSV { get; set; }
    }


    public enum PayType
    {

    }
}
