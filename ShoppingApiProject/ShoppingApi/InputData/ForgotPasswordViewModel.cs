using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.InputData
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string Email { get; set; }
    }
}
