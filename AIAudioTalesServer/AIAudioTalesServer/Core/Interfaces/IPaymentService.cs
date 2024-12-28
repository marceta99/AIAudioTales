using AIAudioTalesServer.Domain.Entities;
using AIAudioTalesServer.Web.DTOS;

namespace AIAudioTalesServer.Core.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// Places an order for the given basket and user, returning a Stripe sessionId if successful.
        /// </summary>
        Task<string?> PlaceOrderAsync(DTOBasket basket, User user);

        /// <summary>
        /// Handles success callback from Stripe checkout session.
        /// Updates purchase status in the DB, etc.
        /// Returns true if success, false if not found or error.
        /// </summary>
        Task<bool> CheckoutSuccessAsync(string sessionId);

        /// <summary>
        /// Handles canceled checkout.
        /// Removes canceled purchase from DB, returns true if successful.
        /// </summary>
        Task<bool> CheckoutCanceledAsync(string sessionId);

        /// <summary>
        /// Processes a Stripe Webhook event. (Subscription updates, etc.)
        /// Usually you parse the JSON request body and signature here.
        /// </summary>
        Task ListenWebhookAsync(Stream requestBody, string stripeSignature);

        /// <summary>
        /// Creates a subscription session for the current user
        /// and returns the sessionId if successful.
        /// </summary>
        Task<string?> CreateSubscribeSessionAsync(int userId);
    }
}
