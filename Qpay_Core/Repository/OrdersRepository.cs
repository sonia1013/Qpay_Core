using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qpay_Core.Repository
{
    public class OrdersRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrdersRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        //[HttpPost]
        //[Route("create")]
        //public async Task<IActionResult> Post(Todos newTodos)
        //{
        //    var newTodosJson = JsonSerializer.Serialize<Todos>(newTodos);

        //    var request = new HttpRequestMessage(HttpMethod.Post, "https://jsonplaceholder.typicode.com/users/1/todos");

        //    request.Content = new StringContent(newTodosJson, Encoding.UTF8, "application/json");

        //    var httpClient = _httpClientFactory.CreateClient();

        //    var response = await httpClient.SendAsync(request);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return Ok(new { Message = "Failed" });
        //    }

        //    return Ok(new { Message = "Success" });
        //}
    }
}
