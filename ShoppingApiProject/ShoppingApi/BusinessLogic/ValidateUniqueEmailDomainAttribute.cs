//using Microsoft.AspNetCore.Identity;
//using System.ComponentModel.DataAnnotations;

//namespace ShoppingApi.BusinessLogic
//{
//    public class ValidateUniqueEmailDomainAttribute : ValidationAttribute
//    {
//        private readonly string _allowedDomain;
//        private UserManager<ApplicationUser>? _userManager;

//        public ValidateUniqueEmailDomainAttribute(string allowedDomain)
//        {
//            _allowedDomain = allowedDomain;
//        }
//        public override bool IsValid(object? value)
//        {
//            HttpCompletionOption use = value.ToString();
//            var user = _userManager.FindByEmailAsync(value);
//            return strings[1].ToUpper() == _allowedDomain.ToUpper();
//        }
//    }
//}
