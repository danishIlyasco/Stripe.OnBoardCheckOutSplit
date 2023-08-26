using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
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

        public async Task OnGet(string unpaidContractId = null)
        {
            //Case when contract is created and payment is in unpaid state due to any reason like user failed to pay or left the checkout incomplete on stripe.
            if (!string.IsNullOrWhiteSpace(unpaidContractId))
            {
                var user = await _userManager.GetUserAsync(User);

                var unPaidContract = _context.Contracts.FirstOrDefault(x => x.ClientUserId == user.Id && unpaidContractId == x.Id.ToString());

                if (unPaidContract != null && unPaidContract.PaymentStatus == Contract.PaymentStatuses.UnPaid)
                {
                    var checkoutSession = _stripeAccountService.GetCheckOutSesssion(unPaidContract.SessionId);

                    //Preparing url for redirect from stripe based on success or cancel.
                    var domain = "https://localhost:7122";
                    var successUrl = string.Format("{0}/CheckoutSuccess?cntId={1}", domain, unPaidContract.Id);
                    var cancelUrl = string.Format("{0}/CheckoutCancel?cntId={1}", domain, unPaidContract.Id);

                    if (checkoutSession != null && _stripeAccountService.GetSesssionStatus(checkoutSession) == Contract.SessionStatuses.Open)
                    {
                        Response.Redirect(checkoutSession.Url);
                    }
                    else if (checkoutSession == null || _stripeAccountService.GetSesssionStatus(checkoutSession) == Contract.SessionStatuses.Expired)
                    {
                        var mileStone = _context.Milestones.FirstOrDefault(x => x.Id == unPaidContract.MileStoneId);

                        if (mileStone != null)
                        {
                            Session newSession = _stripeAccountService.CreateCheckoutSession(mileStone, successUrl, cancelUrl);

                            if (newSession == null || string.IsNullOrEmpty(newSession.Id))
                            {
                                Response.Redirect(domain + "/Checkout");
                            }

                            //checkout initiated successful
                            unPaidContract.SessionId = newSession.Id;
                            unPaidContract.SessionExpiry = newSession.ExpiresAt;
                            unPaidContract.PaymentStatus = _stripeAccountService.GetPaymentStatus(newSession);

                            _context.Update(unPaidContract);
                            _context.SaveChanges();

                            Response.Redirect(newSession.Url);
                        }
                    }
                }
            }
            else
            {
                // Here on checkout page landing a contract will be created.
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
                    _context.Contracts.Add(new Contract { ContractUsers = new List<ContractUser> { new ContractUser { ApplicationUser = await _userManager.GetUserAsync(User), Percentage = 80 } }, MileStoneId = milestone.Id, MileStone = milestone, PaymentStatus = Contract.PaymentStatuses.ContractCreated, PaymentIntentId = string.Empty });
                    _context.SaveChanges();
                }
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
                Session session = _stripeAccountService.CreateCheckoutSession(mileStone, successUrl, cancelUrl);

                if (session == null || string.IsNullOrEmpty(session.Id))
                {
                    Response.Headers.Add("Location", domain + "/Checkout");
                    return new StatusCodeResult(303);
                }

                //checkout initiated successful
                contract.SessionId = session.Id;
                contract.SessionExpiry = session.ExpiresAt;
                contract.SessionStatus = _stripeAccountService.GetSesssionStatus(session);
                contract.PaymentStatus = _stripeAccountService.GetPaymentStatus(session);
                
                _context.Update(contract);
                _context.SaveChanges();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            else
            {
                Response.Headers.Add("Location", successUrl);
                return new StatusCodeResult(303);
            }
        }
    }
}