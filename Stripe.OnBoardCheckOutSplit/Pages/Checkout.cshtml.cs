using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stripe.OnBoardCheckOutSplit.Data;
using Stripe.OnBoardCheckOutSplit.Models;
using Stripe.OnBoardCheckOutSplit.Services.Stripe;

namespace Stripe.OnBoardCheckOutSplit.Pages
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        private readonly ILogger<CheckoutModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IStripeAccountService _stripeAccountService;

        public CheckoutModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<CheckoutModel> logger, IStripeAccountService stripeAccountService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _stripeAccountService = stripeAccountService;
        }

        public async Task OnGet()
        {
            var project = _context.Projects.FirstOrDefault();

            if (project == null)
            {
                _context.Projects.Add(new Project { ProjectTitle = "My Project", Milestones = new List<Milestone> { new Milestone { Price = 100, Title = "First Milestone", Currency = "usd" } } });
                _context.SaveChanges();
            }
            var contract = _context.Contracts.FirstOrDefault();
            if (contract == null)
            {
                var milestone = _context.Milestones.FirstOrDefault();
                _context.Contracts.Add(new Contract { ContractUsers = new List<ContractUser> { new ContractUser { ApplicationUser = await _userManager.GetUserAsync(User), Percentage = 80 } }, MileStone = milestone, PaymentStatus = Contract.PaymentStatuses.ContractCreated, PaymentIntentId = string.Empty });
                _context.SaveChanges();
            }
        }

        public IActionResult OnPost()
        {
            // getting DataProject milestone and contract details from database.
            var project = _context.Projects.Include("Milestones").FirstOrDefault();
            var mileStone = project.Milestones.FirstOrDefault();
            var contract = _context.Contracts.Include("ContractUsers").FirstOrDefault(x => x.MileStone.Id == mileStone.Id);

            //Preparing url for redirect from stripe based on success or cancel.
            var domain = "https://localhost:7122";
            var successUrl = string.Format("{0}/CheckoutSuccess?cntId={1}", domain, contract.Id);
            var cancelUrl = string.Format("{0}/CheckoutCancel?cntId={1}", domain, contract.Id);

            if (contract.PaymentStatus == Contract.PaymentStatuses.ContractCreated)
            {

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
                {
                    new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            UnitAmount = Convert.ToInt64(mileStone.Price * 100), // Amount in cents ($100)
                            Currency = mileStone.Currency,
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
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
                    {"contractId", contract.Id.ToString()},
                    {"freelancerId1", "123"},
                    {"freelancerId2", "123"},
                    {"Architect", "123"}
                }
                };


                Stripe.Checkout.Session session;

                try
                {
                    var service = new Stripe.Checkout.SessionService();
                    session = service.Create(options);
                }
                catch (Exception ex)
                {
                    Response.Headers.Add("Location", domain + "/Checkout");
                    return new StatusCodeResult(303);
                }

                if (session != null && !string.IsNullOrEmpty(session.Id))
                {
                    //checkout initiated successful
                    contract.SessionId = session.Id;
                    contract.SessionExpiry = session.ExpiresAt;

                    if (session.PaymentStatus == "unpaid")
                    {
                        contract.PaymentStatus = Contract.PaymentStatuses.UnPaid;
                    }

                    if (session.Status == "open")
                    {
                        contract.SessionStatus = Contract.SessionStatuses.Open;
                    }

                    _context.Update(contract);
                    _context.SaveChanges();

                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);
                }
                else
                {
                    // try checkout again
                    Response.Headers.Add("Location", domain + "/Checkout");
                    return new StatusCodeResult(303);
                }
            }
            else
            {
                Response.Headers.Add("Location", successUrl);
                return new StatusCodeResult(303);
            }
        }
    }
}