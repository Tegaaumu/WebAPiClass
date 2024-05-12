namespace ShoppingApi.Cart
{
    public class CartOutput
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ProductImage { get; set; }
        public string StatusFlags { get; set; }
        public double TotalPrice { get; set; }
    }
}
