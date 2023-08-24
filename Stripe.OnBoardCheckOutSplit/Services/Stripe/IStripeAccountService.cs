﻿namespace Stripe.OnBoardCheckOutSplit.Services.Stripe
{
    public interface IStripeAccountService
    {
        public string CreateStripeAccount(string apiKey);
        public bool IsComplete(string connectedAccountId);
        public string GetPaymentIntentStatus(string paymentIntentId, string dbStatus);
    }
}
