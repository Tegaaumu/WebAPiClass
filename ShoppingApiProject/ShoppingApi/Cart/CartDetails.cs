using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.Cart
{
    public class CartDetails
    {
        [Key]
        public string CartId { get; set; }
        public string UserId  { get; set; }
        public int CartItemId { get; set; } 

        [Required]
        public string ProductImage { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public double TotalPrice
        {
            get
            {
                double total = 0;
                foreach (var item in Items)
                {
                    total += item.Price * item.Quantity;
                }
                return total;
            }
        }

        public DateTime CurrentTime = DateTime.Now;
    }

    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
