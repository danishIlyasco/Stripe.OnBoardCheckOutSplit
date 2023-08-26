using Azure;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe.OnBoardCheckOutSplit.Models;

namespace Stripe.OnBoardCheckOutSplit.Services.Stripe
{
    public class StripeAccountService : IStripeAccountService
    {
        public string CreateStripeAccount(string apiKey)
        {
            StripeConfiguration.ApiKey = apiKey;
            var accountOptions = new AccountCreateOptions
            {
                Type = "express",
                Capabilities = new AccountCapabilitiesOptions
                {
                    CardPayments = new AccountCapabilitiesCardPaymentsOptions
                    {
                        Requested = true
                    },
                    Transfers = new AccountCapabilitiesTransfersOptions
                    {
                        Requested = true
                    }
                }
            };

            try
            {
                var accountService = new AccountService();
                return accountService.Create(accountOptions).Id;
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool IsComplete(string connectedAccountId)
        {
            try
            {
                var service = new AccountService();
                var response = service.Get(connectedAccountId);

                if (response.Capabilities.Transfers == "active")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public string GetPaymentIntentStatus(string paymentIntentId, string dbStatus)
        {
            try
            {
                var service = new PaymentIntentService();
                var response = service.Get(paymentIntentId);
                if (response != null)
                {
                    if (response.Status == "processing")
                    {
                        return "processing";
                    }
                    else if (response.Status == "succeeded")
                    {
                        return "succeeded";
                    }
                    else if (response.Status == "canceled")
                    {
                        return "canceled";
                    }
                    else
                    {
                        return dbStatus;
                    }
                }
                else
                {
                    return dbStatus;
                }
            }
            catch
            {
                return dbStatus;
            }
        }

        public Session GetCheckOutSesssion(string sessionId)
        {
            try
            {
                var service = new SessionService();
                return service.Get(sessionId);
            }
            catch
            {
                return null;
            }
        }

        public Contract.SessionStatuses GetSesssionStatus(Session session)
        {
            switch (session.Status)
            {
                case "complete":
                    return Contract.SessionStatuses.Complete;
                case "expired":
                    return Contract.SessionStatuses.Expired;
                case "open":
                    return Contract.SessionStatuses.Open;
                default:
                    return Contract.SessionStatuses.NotCreated;
            }
        }

        public Contract.PaymentStatuses GetPaymentStatus(Session session)
        {
            switch (session.PaymentStatus)
            {
                case "paid":
                    return Contract.PaymentStatuses.Paid;
                case "unpaid":
                    return Contract.PaymentStatuses.UnPaid;
                case "no_payment_required":
                    return Contract.PaymentStatuses.NoPaymentRequired;
                default:
                    return Contract.PaymentStatuses.ContractCreated;
            }
        }

        public PaymentIntent GetPaymentIntent(string paymentIntentId)
        {
            try
            {
                var paymentIntentService = new PaymentIntentService();
                return paymentIntentService.Get(paymentIntentId);
            }
            catch
            {
                return null;
            }
        }

        public Session CreateCheckoutSession(Milestone mileStone, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },
                LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = Convert.ToInt64(mileStone.Price * 100), // Amount in cents ($100)
                                Currency = mileStone.Currency,
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = mileStone.Title,
                                }

                            },
                            Quantity = 1,
                        },
                    },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                // This is meta data which can be used to store any information which will be available on payment success/Cancel.
                Metadata = new Dictionary<string, string>
                    {
                        { "contractId", "contractId" },
                        { "freelancerId1", "123" },
                        { "freelancerId2", "123" },
                        { "Architect", "123" }
                    },
                AutomaticTax = new SessionAutomaticTaxOptions
                {
                    Enabled = true
                }
            };

            try
            {
                var service = new SessionService();
                return service.Create(options);
            }
            catch
            {
                return null;
            }
        }

        public string CreateTransferonCharge(long amount, string currency, string destination, string sourceTransaction, string transferGroup)
        {
            var options = new TransferCreateOptions
            {
                Amount = amount,
                Currency = currency,
                Destination = destination,
                SourceTransaction = sourceTransaction,
                TransferGroup = transferGroup
            };

            try
            {
                var service = new TransferService();
                var transfer = service.Create(options);

                return transfer.Id;
            }
            catch
            {
                return null;
            }
           
        }

        public AccountLink CreateOnBoardLink(string stripeConnectId, string refreshUrl, string returnUrl)
        {
            var accountLinkOptions = new AccountLinkCreateOptions
            {
                Account = stripeConnectId,
                RefreshUrl = refreshUrl,
                ReturnUrl = returnUrl,
                Type = "account_onboarding"
            };

            try
            {
                var accountLinkService = new AccountLinkService();
                return accountLinkService.Create(accountLinkOptions);
            }
            catch
            {
                return null;
            }
        }
    }
}
