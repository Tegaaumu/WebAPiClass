using Microsoft.AspNetCore.Mvc;
using ShoppingApi.BusinessLogic;
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
        [Remote(action: "IsEmailUsed", controller: "Account")]
        [ValidateEmailDomain(allowedDomain: "gmail.com", ErrorMessage = "Email address must be gmail.com")]
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
