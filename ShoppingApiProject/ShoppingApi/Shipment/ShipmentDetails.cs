using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.Shipment
{
    public class ShipmentDetails
    {
        [Key]
        public string Shipping_Id { get; set; } = Guid.NewGuid().ToString();
        public string User_ID { get; set; }
        public double Total { get; set; }

        [Required(ErrorMessage = "You should place your address in other to recieve your product")]
        public string Address { get; set; }
        public string? City_State_Country { get; set; }
        public DateTime Delivery_Date { get; set; } = DateTime.Now.AddDays(14);
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Modified_at { get; set; } = DateTime.Now;
        public string Status { get; set; } = (Shipment_Status.Saved_To_Cart.ToString());
    }
}
