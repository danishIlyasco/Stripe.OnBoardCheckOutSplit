using Microsoft.AspNetCore.Identity;

namespace Stripe.OnBoardCheckOutSplit.Models
{
    public class ApplicationUser : IdentityUser<string>
    {

        /// <summary>
        /// StripeAccountStatus
        /// </summary>
        public enum StripeAccountStatuses
        {
            NotCreated,
            Initiated,
            Incomplete,
            Complete
        }

        /// <summary>
        /// Stripe connect account Id
        /// </summary>
        public string StripeConnectedId { get; set; }

        public StripeAccountStatuses StripeAccountStatus { get; set; }
    }
}
