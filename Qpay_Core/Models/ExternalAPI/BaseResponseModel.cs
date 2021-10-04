namespace Qpay_Core.Models.ExternalAPI
{
    public class BaseResponseModel
    {
        //public string Status { get; set; }  //S:成功 F:失敗
        /// <summary>
        /// API版本
        /// </summary>
        public string Version { get; } = "1.0.0";
        /// <summary>
        /// 商家註冊編號，例如AA0001_001
        /// </summary>
        public string ShopNo { get; }= "NA0249_001";
        public APIService APIService { get; set; }
        /// <summary>
        /// 一次性數值
        /// </summary>
        public string Nonce { get; set; }
        /// <summary>
        /// 簽章值
        /// </summary>
        public string Sign { get; set; }

        public string Message { get; set; } //(交易訊息內文+HashID+IV)=>AES-CBC
    }
}
