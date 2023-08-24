namespace Stripe.OnBoardCheckOutSplit.Models
{
    public class ContractUser : BaseEntity
    {
        public ApplicationUser ApplicationUser { get; set; }
        public int Percentage { get; set; }
    }
}
