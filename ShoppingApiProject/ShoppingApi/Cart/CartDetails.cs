using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.Cart
{
    public class CartDetails
    {
        
        //Here is where the problem is is coming
        [Key]
        public int CartId { get; set; }
        public string UserId  { get; set; }

        [Required]
        public string ProductImage { get; set; }
        public double Price { get; set; }
        public List<CartItems> Items { get; set; } = new List<CartItems>();
        public double TotalPrice { get; set; } 
        public DateTime CurrentTime { get; set; } = DateTime.Now;

        //private double CalculateTotalPrice()
        //{
        //    double total = 0;
        //    foreach (var item in Items)
        //    {
        //        total += Price * item.Quantity;
        //    }
        //    return total;
        //}
    }
}
