using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using xrabbit.consumer.Publisher;

namespace xrabbit.consumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessagePublisher publisher;

        public MessageController(MessagePublisher messagePublisher)
        {
            this.publisher = messagePublisher;
        }

        [HttpPost]
        [Route("publish")]
        public string SendMessage([FromBody] string message)
        {
            this.publisher?.PublishMessage(message);

            return string.Empty;
        }


    }
}