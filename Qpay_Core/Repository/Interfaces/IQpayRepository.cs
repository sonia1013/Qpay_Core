using Qpay_Core.Models.ExternalAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qpay_Core.Repository;
using Qpay_Core.Models;

namespace Qpay_Core.Repository.Interfaces
{
    public interface IQpayRepository
    {
        public Task<BaseResponseModel> CreateApiAsync(string route, BaseRequestModel request);

        public Task<string> CreateNonceAsync(NonceRequestModel nonceRequest);
    }
}
