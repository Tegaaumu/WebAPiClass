using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.InputData
{
    public class ChangePassword
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "Password and confirm password must match")]
        public string ConfirmNewPassword { get; set; }
    }
}
