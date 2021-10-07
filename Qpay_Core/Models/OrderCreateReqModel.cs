using System;
namespace Qpay_Core.Models
{
	public class OrderCreateReqModel
    {
        public string ShopNo { get; set; }  //"NA0249_001"
        public string OrderNo { get; set; } //"A202109170001"
        public int Amount { get; set; } //1314
        public string CurrencyID { get; set; }  //TWD
        public string PayType { get; set; } //A:�����b��(AtmPayNo.WebAtmURL.OtpURL����) //C:�H�Υd(CardPayURL����)
        public ATMParam ATMParam { get; set; }      //PayType==A Required
        public CardParam CardParam { get; set; }    //PayType==C Required
        public ConvStoreParam ConvStoreParam { get; set; }
        public string PrdtName { get; set; }    //�����b���q�� or �H�Υd�q��
        public string Memo { get; set; }    
        public string ReturnURL { get; } ="http://10.11.22.113:8803/QPay.ApiClient/Store/Return";
        public string BackendURL { get; } = "http://10.11.22.113:8803/QPay.ApiClient/AutoPush/PushSuccess";


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
        /// �۰ʽд�(�H�Υd)
        /// </summary>
        public string AutoBilling { get; set; }

        /// <summary>
        /// �w�p�۰ʽдڤѼ�
        /// </summary>
        public int? ExpBillingDays { get; set; }

        /// <summary>
        /// �q�榳�Įɶ�(����)
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

    public class OrderCreateResponseModel:OrderCreateReqModel
    {
        public string TSNo { get; set; }
        public string Status { get; set; }
    }
}