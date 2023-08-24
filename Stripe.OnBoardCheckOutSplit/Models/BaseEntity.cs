using System.ComponentModel.DataAnnotations;

namespace Stripe.OnBoardCheckOutSplit.Models
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// unique id of the entity
        /// </summary>
        [Key]
        public Guid Id { get; set; }
    }
}
