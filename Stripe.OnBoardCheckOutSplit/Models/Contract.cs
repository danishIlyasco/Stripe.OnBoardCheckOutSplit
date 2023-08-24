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
            PartiallySplitted,
            Splitted,
            Cancelled
        }

        public enum SessionStatuses
        {
            NotCreated,
            Open,
            Complete,
            Expired
        }
        public Milestone MileStone { get; set; }
        public List<ContractUser> ContractUsers { get; set; }
        public PaymentStatuses PaymentStatus { get; set; }
        public string? PaymentIntentId { get; set; }
        public ApplicationUser ClientUser { get; set; }        
        public string? ClientUserId { get; set; }
        public string? TransactionId { get; set; }
        public string? SessionId { get; set; }
        public SessionStatuses SessionStatus { get; set; }
        public DateTime? SessionExpiry { get; set; }
        public string? LatestCahrgeId { get; set; }

        public Guid MileStoneId { get; set; }
    }
}
