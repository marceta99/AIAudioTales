using AIAudioTalesServer.Data.Interfaces;
using AIAudioTalesServer.Models.DTOS;
using Stripe.Checkout;

namespace AIAudioTalesServer.Data.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentRepository(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> CheckOut(BasketDTO basket)
        {
            try
            {
                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";

                var lineItems = new List<SessionLineItemOptions>();

                foreach (var basketItem in basket.BasketItems)
                {
                    lineItems.Add(
                        new SessionLineItemOptions()
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long?)basketItem.ItemPrice*100, // Price Defined in  cents
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = basketItem.Book.Title,
                                    Description = basketItem.Book.Description,
                                    Images = new List<string> { basketItem.Book.ImageURL }
                                }
                            },
                            Quantity = 1
                        });
                }

                var options = new SessionCreateOptions
                {
                    SuccessUrl = $"{baseUrl}/api/payment/success?sessionId=" + "{CHECKOUT_SESSION_ID}",
                    CancelUrl = $"{baseUrl}/api/payment/canceled?sessionId=" + "{CHECKOUT_SESSION_ID}",
                    PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                    LineItems = lineItems,
                    Mode = "payment", // one time payment
                    InvoiceCreation = new SessionInvoiceCreationOptions { Enabled = true }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                return session.Id;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
