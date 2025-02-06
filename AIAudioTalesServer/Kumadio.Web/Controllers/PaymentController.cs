using Kumadio.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIAudioTalesServer.Web.Controllers
{
    [Route("api/[controller]")]
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
