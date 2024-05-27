using Microsoft.AspNetCore.Identity;

namespace LAST_ATTEMPT.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public string? Processed { get; set; }
    }
}
