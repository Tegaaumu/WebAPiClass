using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.Payment
{
    public class PaymentDetails
    {
        [Key]
        public string Payment_ID { get; set; } = Guid.NewGuid().ToString();
        public string UserId_From_Cart { get;set; }
        public string Name { get; set;}
        public string Email { get; set; }
        public string Payment_Type { get; set; } = "PayStack";
        public double Amount { get; set;}
        public string TrxRef { get; set; }
        public bool Status{ get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
