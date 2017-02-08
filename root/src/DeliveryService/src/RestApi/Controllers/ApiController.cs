using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliveryLib;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        [HttpPost]
        public IActionResult SendMessage(string clientId, string message)
        {
            var deliveryservice = new DeliveryMessage(clientId, message);
            deliveryservice.SendMessageAsync(clientId, mesage)
        }
    }
}