namespace Stripe.OnBoardCheckOutSplit.Models
{
    public class Milestone : BaseEntity
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
    }
}
