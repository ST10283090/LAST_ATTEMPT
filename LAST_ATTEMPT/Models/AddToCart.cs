using System.ComponentModel.DataAnnotations.Schema;

namespace LAST_ATTEMPT.Models
{
    public class AddToCart
    {
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? UserId { get; set; }
        public int Quantity { get; set; }
    }
}
