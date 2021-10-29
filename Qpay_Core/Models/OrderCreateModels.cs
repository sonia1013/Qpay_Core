using Qpay_Core.Models.ExternalAPI;
using System;
using System.Runtime.Serialization;

namespace Qpay_Core.Models
{
	public class OrderCreateReq:BaseReqModel
    {
        //public string ShopNo { get; set; }  //"NA0249_001"
        [DataMember]
        public string OrderNo { get; } = "A" + DateTime.Now.ToString("yyyymmddhhmm");
        [DataMember]
        public int Amount { get; set; } //1314
        [DataMember]
        public string CurrencyID { get; set; }  //TWD
        [DataMember]
        public string PayType { get; set; } //A:�����b��(AtmPayNo.WebAtmURL.OtpURL����) //C:�H�Υd(CardPayURL����)
        [DataMember]
        public ATMParam ATMParam { get; set; }      //PayType==A Required
        [DataMember]
        public CardParam CardParam { get; set; }    //PayType==C Required
        //[DataMember]
        //public ConvStoreParam ConvStoreParam { get; set; }
        [DataMember]
        public string PrdtName { get; set; }    //�����b���q�� or �H�Υd�q��
        [DataMember]
        public string Memo { get; set; }
        /// <summary>
        /// 付款完成轉入URL
        /// </summary>
        /// <remarks>
        /// 不可有單引號、雙引號、百分比, 最大長度255
        /// </remarks>
        [DataMember]
        public string ReturnURL { get; } = "https://localhost:5001/checkout/return";
        /// <summary>
        /// 付款完成背端通知URL
        /// </summary>
        /// <remarks>
        /// 不可有單引號、雙引號、百分比, 最大長度255
        /// </remarks>
        [DataMember]
        public string BackendURL { get; } = "https://localhost:5001/home/success";


    }

    public class ATMParam
    {
        public string AtmPayNo { get; set; }
        public string WebAtmURL { get; set; }
        public string OtpURL { get; set; }
        public string BankNo { get; set; }
        public string AcctNo { get; set; }

        public string ExpireDate { get; set; }  //���ӯ���short datetime
    }

    public class CardParam
    {
        //public string CardPayURL { get; set; }

        /// <summary>
        /// 自動請款(信用卡)
        /// </summary>
        public string AutoBilling { get; set; }

        /// <summary>
        /// 預計自動請款天數
        /// </summary>
        public int? ExpBillingDays { get; set; }

        /// <summary>
        /// 訂單有效時間(分鐘)
        /// </summary>
        public int? ExpMinutes { get; set; }

        /// <summary>
        /// ���ڤ覡-�l��
        /// </summary>
        public string PayTypeSub { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string Staging { get; set; }

        /// <summary>
        /// �ֳt�I�� Token
        /// </summary>
        public string CCToken { get; set; }
    }

    public class ConvStoreParam
    {
        //����
    }

    public class OrderCreateRes:OrderCreateReq
    {
        public string TSNo { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}