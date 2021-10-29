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
        //public Task<T> CreateApiAsync(string route, BaseApiMessage request);

        public Task<T> CreateApiAsync<T>(string route, BaseApiMessage request) where T : new();

        public Task<string> CreateNonceAsync(NonceRequestModel nonceRequest);
    }
}
