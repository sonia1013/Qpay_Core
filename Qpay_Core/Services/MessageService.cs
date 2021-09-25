using Microsoft.Extensions.Logging;
using Qpay_Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Qpay_Core.Services
{
    public class MessageService
    {
        private readonly ILogger<QpayRepository> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string HashID;
        private readonly string data;
        private readonly string nonce;

        public MessageService(ILogger<QpayRepository> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        //JSONmsg = 
    }
}
