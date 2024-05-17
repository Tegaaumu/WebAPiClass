using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.OutputModel
{
    public class ShippingOut
    {
        public string Shipping_Id { get; set; } = Guid.NewGuid().ToString();
        public string User_ID { get; set; }
        public string Total { get; set; }
        public string Address { get; set; }
    }
}
