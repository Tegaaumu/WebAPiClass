using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.Payment
{
    public class Items_Payed_For
    {
        [Key]
        public string Payment_Id { get; set; } = Guid.NewGuid().ToString();
        public int Items_Payed { get; set;}
        public string User_Id { get; set;}
        public bool Status { get; set; } = false;

    }
}
