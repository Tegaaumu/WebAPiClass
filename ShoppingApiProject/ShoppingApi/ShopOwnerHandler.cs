using Microsoft.AspNetCore.Authorization;
using ShoppingApi.BusinessLogic;

namespace ShoppingApi
{
    public class ShopOwnerHandler : AuthorizationHandler<ShopOwnerRequirement>
    {
        private ApplicationDBContext? _applicationDBContext;
        public ShopOwnerHandler(ApplicationDBContext? applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ShopOwnerRequirement requirement)
        {
            var httpContext = (HttpContext) context.Resource!;
            var shopExists = httpContext.Request.RouteValues.TryGetValue("user", out object? shopId);
            if (shopExists)
            {
                var user1 = httpContext.User.Claims.FirstOrDefault(x => x.Type == "email" || x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
                //var user = httpContext.User.Claims.FirstOrDefault(x => x.Value == "emailaddress");
                var pasedShopId = shopId.ToString();
                var pasedUserId = user1.ToString();
                var shop = _applicationDBContext!.CartDetails.FirstOrDefault(x => x.UserId == pasedShopId);
                if (shop != null) context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
