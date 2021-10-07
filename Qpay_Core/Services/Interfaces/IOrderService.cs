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
        //public Task<BaseResponseModel> GetQPayResponse(OrderCreateReqModel orderCreateReq);
        //public Task<TReq> GetQPayResponse<TReq, TResult>(TReq request, APIService apiService) where TReq : BaseRequestModel;

        public Task<OrderCreateRes> OrderCreate(OrderCreateReq req);
        public Task<OrderPayQueryRes> OrderPayQuery(OrderPayQueryReq req);
        public Task<OrderQueryRes> OrderQuery(OrderQueryReq req);


    }
}
