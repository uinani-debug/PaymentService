using AutoMapper;
using AccountLibrary.API.Models;
using AccountLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using PaymentService.API.Models;
using Newtonsoft.Json;
using System.Threading;

namespace AccountLibrary.API.Controllers
{
    [ApiController]

    public class PaymentController : ControllerBase
    {
        private readonly IPaymentLibraryRepository _PaymentLibraryRepository;
        private readonly IMapper _mapper;
        private readonly ConsumerConfig _config;
        public PaymentController(IPaymentLibraryRepository PaymentLibraryRepository,
            IMapper mapper)
        {



            _PaymentLibraryRepository = PaymentLibraryRepository ??
                throw new ArgumentNullException(nameof(PaymentLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        [Route("Payments")]
        [HttpPost]
        public ActionResult<string> AccountTransaction(PaymentRequest Request)
        {

            if (Request == null)
            {
                return NotFound();
            }
            publishEvent(Request);

            return Accepted();
        }

        private async void publishEvent(PaymentRequest Request)
        {
            var config = new ProducerConfig { BootstrapServers = "3.129.43.25:9092" };

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    string jsonRequest = JsonConvert.SerializeObject(Request);

                    var dr = await p.ProduceAsync("CreditAccount", new Message<Null, string> { Value = jsonRequest });
                    var dr1 = await p.ProduceAsync("DebitAccount", new Message<Null, string> { Value = jsonRequest });
                    //  Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    // Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }

        [Route("health")]
        [HttpGet]
        public ActionResult health()
        {
            return Ok();
        }

        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}