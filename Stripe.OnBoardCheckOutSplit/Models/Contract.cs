using System.ComponentModel.DataAnnotations;

namespace Stripe.OnBoardCheckOutSplit.Models
{
    public class Contract : BaseEntity
    {
        /// <summary>
        /// StripeAccountStatus
        /// </summary>
        public enum PaymentStatuses
        {   
            ContractCreated,
            UnPaid,
            Paid,
            NoPaymentRequired,
            Splitted,
            Cancelled
        }

        public enum SessionStatuses
        {
            Open,
            Complete,
            Expired
        }
        public Milestone MileStone { get; set; }
        public List<ContractUser> ContractUsers { get; set; }
        public PaymentStatuses PaymentStatus { get; set; }
        public string PaymentIntentId { get; set; }
        public ApplicationUser ClientUser { get; set; }        
        public string? ClientUserId { get; set; }
        public string? TransactionId { get; set; }
        public string? SessionId { get; set; }
        public SessionStatuses SessionStatus { get; set; }
    }
}
