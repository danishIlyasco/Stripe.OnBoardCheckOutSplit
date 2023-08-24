namespace Stripe.OnBoardCheckOutSplit.Models
{
    public class ContractUser : BaseEntity
    {
        public ApplicationUser ApplicationUser { get; set; }
        public int Percentage { get; set; }
        public string? StripeTranferId { get; set; }
        public bool IsTransfered { get; set; } = false;
        public string ApplicationUserId { get; set; }
    }
}
