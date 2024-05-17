using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.Shipment
{
    public class ShippingInput
    {
        public string User_ID_Email { get; set; }
        public string Address { get; set; }
        public string City_State_Country { get; set; }
    }
}
