﻿using Microsoft.AspNetCore.Identity;
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

        public async void OnPost()
        {
            // Receive the contract which we want to approve.
            var contract = _context.Contracts.Include("ContractUsers").FirstOrDefault();

            if (contract != null)
            {
                var mileStone = _context.Milestones.FirstOrDefault(x => x.Id == contract.MileStoneId);
                var checkoutSession = _stripeAccountService.GetCheckOutSesssion(contract.SessionId);

                if (checkoutSession != null)
                {
                    contract.SessionStatus = _stripeAccountService.GetSesssionStatus(checkoutSession);
                    
                    if (_stripeAccountService.GetPaymentStatus(checkoutSession) == Contract.PaymentStatuses.Paid)
                    {
                        foreach (var contractUser in contract.ContractUsers.Where(x => !x.IsTransfered))
                        {
                            var user = _context.Users.FirstOrDefault(x => x.Id == contractUser.ApplicationUserId);

                            if (user != null && user.StripeAccountStatus == ApplicationUser.StripeAccountStatuses.Complete)
                            {
                                var paymentIntent = _stripeAccountService.GetPaymentIntent(contract.PaymentIntentId);

                                var priceToTransfer = (long)(Convert.ToDecimal(contractUser.Percentage) / 100 * mileStone.Price * 100);
                                var transferId = _stripeAccountService.CreateTransferonCharge(priceToTransfer, mileStone.Currency, user.StripeConnectedId, contract.LatestCahrgeId, contract.Id.ToString());
                                contractUser.StripeTranferId = transferId;
                                contractUser.IsTransfered = true;
                                _context.ContractUsers.Update(contractUser);
                            }
                            else
                            {
                                // this User(freelabncer or architect) has not completed his Stripe account please 
                            }
                        }

                        if (contract.ContractUsers.Where(x => x.IsTransfered).Count() == contract.ContractUsers.Count())
                        {
                            contract.PaymentStatus = Contract.PaymentStatuses.Splitted;
                        }
                        else
                        {
                            contract.PaymentStatus = Contract.PaymentStatuses.PartiallySplitted;
                        }

                        _context.Contracts.Update(contract);
                        _context.SaveChanges();
                    }
                }

            }
        }
    }
}