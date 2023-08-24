using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe.OnBoardCheckOutSplit.Data;
using Stripe.OnBoardCheckOutSplit.Models;
using Stripe.OnBoardCheckOutSplit.Services.Stripe;

namespace Stripe.OnBoardCheckOutSplit.Pages
{
    [Authorize]
    public class CheckoutSuccessModel : PageModel
    {
        private readonly ILogger<CheckoutModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IStripeAccountService _stripeAccountService;

        public CheckoutSuccessModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<CheckoutModel> logger, IStripeAccountService stripeAccountService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _stripeAccountService = stripeAccountService;
        }

        public async Task OnGetAsync(string cntId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // Please note that this logic is verification of the encrypted string that is set for specific contract and logged in user is owner of the contract.
                var contract = _context.Contracts.FirstOrDefault(x => x.Id.ToString() == cntId && x.ClientUserId == user.Id);

                if (contract != null)
                {
                    var checkoutSession = _stripeAccountService.GetCheckOutSesssion(contract.SessionId);

                    if(checkoutSession != null)
                    {
                        contract.SessionStatus = _stripeAccountService.GetSesssionStatus(checkoutSession);
                        contract.PaymentStatus = _stripeAccountService.GetPaymentStatus(checkoutSession);

                        if (contract.PaymentStatus == Contract.PaymentStatuses.Paid && contract.SessionStatus == Contract.SessionStatuses.Complete)
                        {
                            contract.PaymentIntentId = checkoutSession.PaymentIntentId;

                            var paymentIntent = _stripeAccountService.GetPaymentIntent(contract.PaymentIntentId);
                            
                            if (paymentIntent != null && !string.IsNullOrEmpty(paymentIntent.LatestChargeId))
                            {
                                contract.LatestCahrgeId = paymentIntent.LatestChargeId;
                            }

                            ViewData["Message"] = "Your payment is compeleted successfully and in escrow. Incase of milestone approved successfully it will be transfered to all stakeholders(Freelances, Architects and Platfom)";
                        }
                        else if (contract.PaymentStatus == Contract.PaymentStatuses.NoPaymentRequired)
                        {
                            ViewData["Message"] = "Your payment is in no payment required state. The payment is delayed to a future date, or the Checkout Session is in setup mode and doesn’t require a payment at this time.";
                        }
                        else if (contract.PaymentStatus == Contract.PaymentStatuses.UnPaid) 
                        {
                            ViewData["Message"] = "Your payment is in upaid state state yet.We have not received the payment.";
                        }
                    }

                    _context.Contracts.Update(contract);
                    _context.SaveChanges();
                }
            }
        }
    }
}
