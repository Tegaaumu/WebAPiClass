using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.Payment
{
    public class PaymentDetails
    {
        [Key]
        public string Payment_ID { get; set; } = Guid.NewGuid().ToString();
        public int userId_from_cart { get;set; }
        public string Payment_Type { get; set;}
        public double Amount { get; set;}

    }
}
