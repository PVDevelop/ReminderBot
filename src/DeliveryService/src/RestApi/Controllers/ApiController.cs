using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeliveryLib;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> SendMessage(string clientId, string message)
        {
            var deliveryservice = new DeliveryMessage(clientId, message);
            await deliveryservice.SendMessageAsync(clientId, message, default(CancellationToken));
            return Ok();
        }
    }
}