using System;


namespace Qpay_Core.Models
{
	public class OrderCreateRequestModel
    {
        public string ShopNo { get; set; }  //"NA0249_001"
        public string OrderNo { get; set; } //"A202109170001"
        public decimal Amount { get; set; } //1314
        public string CurrencyID { get; set; }  //TWD
        public string PayType { get; set; } //A:�����b��(AtmPayNo.WebAtmURL.OtpURL����) //C:�H�Υd(CardPayURL����)
        public ATMParam ATMParam { get; set; }      //PayType==A Required
        public CardParam CardParam { get; set; }    //PayType==C Required
        public ConvStoreParam ConvStoreParam { get; set; }
        public string PrdtName { get; set; }    //�����b���q�� or �H�Υd�q��
        public string ReturnURL { get; set; }   //http://10.11.22.113:8803/QPay.ApiClient/Store/Return
        public string BackendURL { get; set; }  //http://10.11.22.113:8803/QPay.ApiClient/AutoPush/PushSuccess
    }

    public class ATMParam
    {
        public string AtmPayNo { get; set; }
        public string WebAtmURL { get; set; }
        public string OtpURL { get; set; }

       // public DateTime ExpireDate { get; set; }  //���ӯ���short datetime
    }

    public class CardParam
    {
        public string CardPayURL { get; set; }

        //public string AutoBilling { get; set; }
    }

    public class ConvStoreParam
    {
        //����
    }

}