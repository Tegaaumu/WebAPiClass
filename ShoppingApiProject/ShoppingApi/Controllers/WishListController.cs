using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingApi.BusinessLogic;
using ShoppingApi.Cart;
using ShoppingApi.WishList;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShoppingApi.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class WishListController : ControllerBase
    {
        public ApplicationDBContext? _applicationDBContext;
        private UserManager<ApplicationUser>? _userManager;
        public WishListController(ApplicationDBContext? applicationDBContext, UserManager<ApplicationUser> userManager)
        {
            _applicationDBContext = applicationDBContext;
            _userManager = userManager;
        }
        // GET: api/<CartController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            if (_applicationDBContext!.WishList.Any() == false)
            {
                return BadRequest("There is no item in this wishList");
            }
            return Ok(_applicationDBContext.CartDetails);
        }


        // GET api/<CartController>/5
        [HttpGet("{user}")]
        public async Task<IActionResult> Get(string user)
        {
            var UserDetails = _applicationDBContext!.WishList.Where(x => x.User == user);

            if (UserDetails == null) return BadRequest("This user has no item in our database");

            return Ok(UserDetails);
        }

        [HttpPost("WishList")]
        public async Task<IActionResult> PostToCart(WishListItems wishListItems)
        {
            var productDetails = _applicationDBContext!.ShoppingInput.FirstOrDefault(w => w.Id == wishListItems.ProductId);
            var UserDetails2 = _userManager.Users.FirstOrDefault().Id;
            if (productDetails == null) return BadRequest("this product is not yet available");
            if (UserDetails2 == null) return BadRequest("LogIn before you can add item to cart");

            //Implement substracting the remaining product from the shopping items
            var AddToCart = _applicationDBContext.WishList.Add(wishListItems);
            productDetails.ItemsRemaining = productDetails.ItemsRemaining - wishListItems.Quantity;

            await _applicationDBContext.SaveChangesAsync();

            return Ok("Item added sucessfully");
        }


    }
}
