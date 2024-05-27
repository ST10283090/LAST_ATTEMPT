namespace LAST_ATTEMPT.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Prod_Name { get; set; }
        public string? Prod_Description { get; set; }
        public decimal Price { get; set; }
        public int Prod_Availability { get; set; }
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
