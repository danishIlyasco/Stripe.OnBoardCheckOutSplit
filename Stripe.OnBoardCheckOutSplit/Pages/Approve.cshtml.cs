using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stripe.OnBoardCheckOutSplit.Data;
using Stripe.OnBoardCheckOutSplit.Models;
using Stripe.OnBoardCheckOutSplit.Services.Stripe;

namespace Stripe.OnBoardCheckOutSplit.Pages
{
    public class ApproveModel : PageModel
    {
        private readonly ILogger<ApproveModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IStripeAccountService _stripeAccountService;

        public ApproveModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<ApproveModel> logger, IStripeAccountService stripeAccountService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _stripeAccountService = stripeAccountService;
        }

        public void OnGet()
        {

        }

        public void OnPost()
        {
            // Receive the contract which we want to approve.
            var contract = _context.Contracts.FirstOrDefault();

            if (contract != null)
            {
                // Receive payment intent from Id
                var service = new PaymentIntentService();
                var response = service.Get(contract.PaymentIntentId);
                if (response != null)
                { 
                    
                }
            }
        }
    }
}