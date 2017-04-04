using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace PVDevelop.ReminderBot.Microservice.Tests.UseCases.UnknownCommand
{
    public class TelegramTestController : Controller
    {
        [HttpPost("bot{token}/getUpdates")]
        public IActionResult GetUpdates(string token, [FromBody] GetUpdatesDto getUpdatesDto)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (getUpdatesDto == null) throw new ArgumentNullException(nameof(getUpdatesDto));

            var content = GetContent();

            return new ContentResult
            {
                ContentType = "application/json",
                Content = content
            };
        }

        private string GetContent()
        {
            var jsonContentPath = GetJsonContentPath();
            return System.IO.File.ReadAllText(jsonContentPath);
        }

        private string GetJsonContentPath()
        {
            return Path.Combine(
                AppContext.BaseDirectory,
                "UseCases/UnknownCommand/UnknownCommandUpdate.json");
        }
    }
}
