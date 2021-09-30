using Qpay_Core.Models.ExternalAPI;
using Qpay_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qpay_Core.Services.Interfaces
{
    public interface IOrderService
    {
        //public Task<BaseResponseModel> OrderCreateAsync(OrderCreateRequestModel orderCreateReq);
        public Task<BaseResponseModel> OrderCreateAsync<TReq, TResult>(TReq request, APIService apiService) where TReq : BaseRequestModel;
    }
}
