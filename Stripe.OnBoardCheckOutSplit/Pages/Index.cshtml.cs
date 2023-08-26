using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using Stripe.OnBoardCheckOutSplit.Data;
using Stripe.OnBoardCheckOutSplit.Models;
using Stripe.OnBoardCheckOutSplit.Services.Stripe;

namespace Stripe.OnBoardCheckOutSplit.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IStripeAccountService _stripeAccountService;
        
        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<IndexModel> logger, IStripeAccountService stripeAccountService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _stripeAccountService = stripeAccountService;
        }

        public async Task OnGet()
        {
            var appUser = await _userManager.GetUserAsync(User);

            if(appUser != null)
            {
                ViewData["isStripeConnected"] = appUser.StripeAccountStatus == ApplicationUser.StripeAccountStatuses.Complete;
            }
        }

        public async Task OnPostAsync()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser != null)
            {
                
                // Connected Account creation.
                if (appUser.StripeAccountStatus == ApplicationUser.StripeAccountStatuses.NotCreated)
                {
                    appUser.StripeConnectedId = _stripeAccountService.CreateStripeAccount(StripeConfiguration.ApiKey);
                    
                    if(!string.IsNullOrEmpty(appUser.StripeConnectedId))
                    {
                        appUser.StripeAccountStatus = ApplicationUser.StripeAccountStatuses.Initiated;
                        _context.ApplicationUsers.Update(appUser);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return;
                    }
                }

                // Redirect user to stripe for account completion. A stripe link is generated with return Url and refresh url.
                if (appUser.StripeAccountStatus == ApplicationUser.StripeAccountStatuses.Initiated)
                {
                    var accountConnectingLink = _stripeAccountService.CreateOnBoardLink(appUser.StripeConnectedId, "https://localhost:7122", "https://localhost:7122/StripeWelcome");

                    if (accountConnectingLink != null)
                    {
                        Response.Redirect(accountConnectingLink.Url);
                    }
                    else
                    {
                        Response.Redirect("https://localhost:7122");
                    }
                }
            }
        }
    }
}