using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.WishList
{
    public class WishListItems
    {
        [Key]
        public string WishListId { get; set; }
        public int ProductId { get; set; }
        public string User { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Occasion { get; set; }
        public DateTime Reminder { get; set; }
    }
}
