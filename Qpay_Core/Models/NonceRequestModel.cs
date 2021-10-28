using Qpay_Core.Models.ExternalAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Qpay_Core.Models
{
    public class NonceRequestModel
    {
        [DataMember]
        public string ShopNo { get; set; }

        //public NonceReq(string shopNo)
        //{
        //    ShopNo = shopNo;
        //}
    }

    public class NonceResModel
    {
        public string Nonce { get; set; }  //NA0249_001
    }

    public class HashInput
    {
        public string A1 { get; set; }  //86D50DEF3EB7400E
        public string A2 { get; set; }  //01FD27C09E5549E5
        public string B1 { get; set; }  //9E004965F4244953
        public string B2 { get; set; }  //7FB3385F414E4F91
    }

    public class ServiceType
    {
        public string ApiServiceName { get; set; }
    }
}
