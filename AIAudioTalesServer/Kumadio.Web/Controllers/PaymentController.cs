using Kumadio.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIAudioTalesServer.Web.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
    }
}
