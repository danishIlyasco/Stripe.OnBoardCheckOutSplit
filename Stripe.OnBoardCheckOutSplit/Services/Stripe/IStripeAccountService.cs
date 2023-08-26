using Stripe.Checkout;
using Stripe.OnBoardCheckOutSplit.Models;

namespace Stripe.OnBoardCheckOutSplit.Services.Stripe
{
    public interface IStripeAccountService
    {
        public string CreateStripeAccount(string apiKey);
        public bool IsComplete(string connectedAccountId);
        public string GetPaymentIntentStatus(string paymentIntentId, string dbStatus);
        public Session GetCheckOutSesssion(string sessionId);
        public Contract.SessionStatuses GetSesssionStatus(Session session);
        public Contract.PaymentStatuses GetPaymentStatus(Session session);
        public PaymentIntent GetPaymentIntent(string paymentIntentId);
        public Session CreateCheckoutSession(Milestone milestone, string successUrl, string cancelUrl);
        public string CreateTransferonCharge(long amount, string currency, string destination, string sourceTransaction, string transferGroup);
        public AccountLink CreateOnBoardLink(string stripeConnectId, string refreshUrl, string returnUrl);
    }
}
