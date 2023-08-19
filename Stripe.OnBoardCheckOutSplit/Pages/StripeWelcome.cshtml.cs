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
                StripeConfiguration.ApiKey = "sk_test_51NaxGxLHv0zYK8g4ZEh9KncjP5T6hbERI8VIn5bKUZvuY36xCSfp99bdrH5Td65cXkJ5FgDdMFVbmAao6xfm8Wje00pAJrWOjf";

                if (_stripeAccountService.IsComplete(appUser.StripeConnectedId))
                {
                    appUser.StripeAccountStatus = ApplicationUser.StripeAccountStatuses.Complete;
                    _context.ApplicationUsers.Update(appUser);
                    _context.SaveChanges();
                    return;
                }

                if (appUser.StripeAccountStatus == ApplicationUser.StripeAccountStatuses.NotCreated)
                {
                    appUser.StripeConnectedId = _stripeAccountService.CreateStripeAccount(StripeConfiguration.ApiKey);
                    appUser.StripeAccountStatus = ApplicationUser.StripeAccountStatuses.Initiated;
                    _context.ApplicationUsers.Update(appUser);
                    _context.SaveChanges();
                }

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