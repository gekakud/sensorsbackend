using System;
using System.Collections.Generic;
using Core.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DataAnalyticsService.Controllers
{
    [Route("api/Analytics")]
    [ApiController]
    public class DataAnalyticsController : ControllerBase
    {
        private IMessageBus _bus;

        public DataAnalyticsController(IMessageBus bus)
        {
            _bus = bus;

            _bus.Subscribe("somepipe", m =>
            {
                Console.WriteLine(m.MessageText);
            });
        }
    }
}
