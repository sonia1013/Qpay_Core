using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qpay_Core.Models.ExternalAPI
{
    public class BaseRequestModel
    {
        public string ShopNo { get; set; }  //NA0249_001
    }

    public class BaseResponseModel
    {
        public string Status { get; set; }  //S:成功 F:失敗
    }

    public class OrderPayQueryRequest:BaseRequestModel
    {
        public string PayToken { get; set; }  //
    }

    public class OrderPayQueryResponse:BaseResponseModel
    {
        public string ShopNo { get; set; }
        public string PayToken { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        //-......................~~~~~共有22個
    }
}
