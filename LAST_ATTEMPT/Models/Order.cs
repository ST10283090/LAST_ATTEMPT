using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LAST_ATTEMPT.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }

}



