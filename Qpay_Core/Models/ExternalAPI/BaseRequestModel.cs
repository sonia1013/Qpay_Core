using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Qpay_Core.Models.ExternalAPI
{
    public class BaseRequestModel
    {
        /// <summary>
        /// API版本
        /// </summary>
        [DataMember]
        public string Version { get; } = "1.0.0";
        /// <summary>
        /// 商家註冊編號，例如AA0001_001
        /// </summary>
        [DataMember]
        public string ShopNo { get; set; } //= "NA0249_001";
        public APIService APIService { get; set; }
        /// <summary>
        /// 一次性數值
        /// </summary>
        [DataMember]
        public string Nonce { get; set; }
        /// <summary>
        /// 簽章值
        /// </summary>
        [DataMember]
        public string Sign { get; set; }
        [DataMember]
        public string Message { get; set; } //(交易訊息內文+HashID+IV)=>AES-CBC
    }

    public class OrderPayQueryReq:BaseRequestModel
    {
        //public string ShopNo { get; set; }
        public string PayToken { get; set; }
    }

    public class OrderPayQueryRes:BaseResponseModel
    {
        //public string ShopNo { get; set; }
        public string PayToken { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }  // S or F
        public string Description { get; set; }
    }

    public class OrderCreateReq : BaseRequestModel
    {
        public string OrderNo { get; set; } //"A202109170001"
        public decimal Amount { get; set; } //1314
        public string CurrencyID { get; set; }  //TWD
        public string PayType { get; set; } //A:�����b��(AtmPayNo.WebAtmURL.OtpURL����) //C:�H�Υd(CardPayURL����)
        public ATMParam ATMParam { get; set; }      //PayType==A Required
        public CardParam CardParam { get; set; }    //PayType==C Required
        public ConvStoreParam ConvStoreParam { get; set; }
        public string PrdtName { get; set; }    //�����b���q�� or �H�Υd�q��
        public string Memo { get; set; }
        public string ReturnURL { get; set; } //= "http://10.11.22.113:8803/QPay.ApiClient/Store/Return";
        public string BackendURL { get; set; } //= "http://10.11.22.113:8803/QPay.ApiClient/AutoPush/PushSuccess";

    }

    public class OrderCreateRes : BaseResponseModel
    {
        public string TSNo { get; set; }
        public string Status { get; set; }
    }

    public class OrderQueryReq : BaseRequestModel
    {
        public string PayToken { get; set; }
    }

    public class OrderQueryRes : BaseResponseModel
    {
        public string PayToken { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// 商業收付API列舉
    /// </summary>
    public enum APIService
    {
        OrderCreate = 1,
        OrderQuery = 2,
        OrderPayQuery = 3,
    }
}
