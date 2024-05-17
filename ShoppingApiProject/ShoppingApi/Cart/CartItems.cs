using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.Cart
{
    public class CartItems
    {
        [Key]
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
