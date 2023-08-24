namespace Stripe.OnBoardCheckOutSplit.Models
{
    public class Project : BaseEntity
    {
        public string ProjectTitle { get; set; }
        public List<Milestone> Milestones { get; set; }
    }
}
