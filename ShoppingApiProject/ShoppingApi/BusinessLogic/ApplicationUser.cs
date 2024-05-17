using Microsoft.AspNetCore.Identity;

namespace ShoppingApi.BusinessLogic
{
    public class ApplicationUser : IdentityUser
    {
        public string UniqueCode { get; set; }
        public string? Address { get; set; }
    }
}
