using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.InputData
{
    public class LoginDetails
    {

        [Required]
        public string UserName { get; set; }


        [Required]
        //[Remote(action: "IsEmailInUse", controller: "Account")]
        [DataType(DataType.EmailAddress)]
        //[EmailAddress]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "PAssword must correcpond")]
        public string ConfirmPassword { get; set; }

        public bool IsPersistent = true;
    }
}
