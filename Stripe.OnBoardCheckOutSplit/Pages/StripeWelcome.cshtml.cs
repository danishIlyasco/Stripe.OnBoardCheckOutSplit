using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Stripe.OnBoardCheckOutSplit.Data;
using Stripe.OnBoardCheckOutSplit.Models;
using Stripe.OnBoardCheckOutSplit.Services.Stripe;

namespace Stripe.OnBoardCheckOutSplit.Pages
{
    [Authorize]
    public class StripeWelcome : PageModel
    {
        private readonly ILogger<StripeWelcome> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IStripeAccountService _stripeAccountService;

        public StripeWelcome(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<StripeWelcome> logger, IStripeAccountService stripeAccountService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _stripeAccountService = stripeAccountService;
        }

        public async Task OnGetAsync()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser != null)
            {
                // checking Status of the account
                if (_stripeAccountService.IsComplete(appUser.StripeConnectedId))
                {
                    appUser.StripeAccountStatus = ApplicationUser.StripeAccountStatuses.Complete;
                    _context.ApplicationUsers.Update(appUser);
                    _context.SaveChanges();
                    return;
                }
                // incase account is not complete 

                // check if accidently landed user has not created the account.
                if (appUser.StripeAccountStatus == ApplicationUser.StripeAccountStatuses.NotCreated)
                {
                    appUser.StripeConnectedId = _stripeAccountService.CreateStripeAccount(StripeConfiguration.ApiKey);
                    appUser.StripeAccountStatus = ApplicationUser.StripeAccountStatuses.Initiated;
                    _context.ApplicationUsers.Update(appUser);
                    _context.SaveChanges();
                }

                // check if account status is initiated(Created but not complete) redirect user to stripe to complete that.
                if (appUser.StripeAccountStatus == ApplicationUser.StripeAccountStatuses.Initiated)
                {
                    var accountLinkOptions = new AccountLinkCreateOptions
                    {
                        Account = appUser.StripeConnectedId,
                        RefreshUrl = "https://localhost:7122/",
                        ReturnUrl = "https://localhost:7122/StripeWelcome",
                        Type = "account_onboarding"
                    };

                    var accountLinkService = new AccountLinkService();
                    var accountLinks = accountLinkService.Create(accountLinkOptions);
                    Response.Redirect(accountLinks.Url);
                }
            }
            else
            {
                Response.Redirect("Index");
            }
        }
    }
}