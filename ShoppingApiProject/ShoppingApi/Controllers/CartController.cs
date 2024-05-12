using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ShoppingApi.BusinessLogic;
using ShoppingApi.Cart;
using ShoppingApi.OutputModel;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShoppingApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        public ApplicationDBContext? _applicationDBContext;
        private UserManager<ApplicationUser>? _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private SignInManager<ApplicationUser>? _signInManager;
        public CartController(ApplicationDBContext? applicationDBContext, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _applicationDBContext = applicationDBContext;
            _userManager = userManager;
        }
        // GET: api/<CartController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            if (_applicationDBContext!.CartDetails.Any() == false)
            {
                return BadRequest("There is no item in this cart");
            }
            return Ok(_applicationDBContext.CartDetails);
        }

        [HttpGet("ByUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ByUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = HttpContext.User;
                var userDetails = user.Claims.FirstOrDefault().Value;

                var userM = await _userManager.FindByEmailAsync(userDetails);
                var userId = userM.Id;

                var UserDetails = _applicationDBContext!.CartDetails.Where(x => x.UserId == userId);
                if (userId == null) return BadRequest("You have not logged In");
                if (UserDetails == null) return BadRequest("This user has no item in our database");
                return Ok(UserDetails);
            }
            return Ok("This user is not yet authenticated.");
        }

        // GET api/<CartController>/5
        [HttpGet("{user}")]
        public async Task<IActionResult> Get(string user)
        {
            var UserDetails = _applicationDBContext!.CartDetails.Where(x => x.UserId == user);
            List<CartOutput> _cartOutput = new();
            if (UserDetails == null) return BadRequest("This user has no item in our database");
            foreach (var items in UserDetails)
            {
                var cartField = new CartOutput()
                {
                    Id = items.CartId,
                    UserId = items.UserId,
                    ProductImage = items.ProductImage,
                    TotalPrice = items.TotalPrice,
                };
                _cartOutput.Add(cartField);
            }
            return Ok(_cartOutput);
        }

        [HttpPost("Cart")]
        public async Task<IActionResult> PostToCart(CartItems productId)
        {
            var productDetails = _applicationDBContext!.ShoppingInput.FirstOrDefault(w => w.Id == productId.ProductId);

            var user = HttpContext.User;
            var userDetails = user.Claims.FirstOrDefault().Value;

            var userM = await _userManager.FindByEmailAsync(userDetails);
            var UserDetails2 = userM.Id;

            if (productDetails == null) return BadRequest("this product is not yet available");
            if (UserDetails2 == null) return BadRequest("LogIn before you can add item to cart");
            productId.Name = productDetails.Name;
            List<CartItems> _cartItem = new List<CartItems>();
            _cartItem.Add(productId);
            var cartTotal = productId.Quantity * productDetails.ProductPriceM;
            //conflict in datatype
            var CartDetails = new CartDetails()
            {
                UserId = UserDetails2,
                ProductImage = productDetails.ProductImage,
                Price = productDetails.ProductPriceM,
                Items = _cartItem,
                TotalPrice = cartTotal,
            };
            //Implement substracting the remaining product from the shopping items
            _applicationDBContext.CartDetails.Add(CartDetails);
            productDetails.ItemsRemaining = productDetails.ItemsRemaining - productId.Quantity; 
            await _applicationDBContext.SaveChangesAsync();
            return Ok("Item added sucessfully");
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> PatchToCart(int CartId, [FromBody] CartItems cartItems)
        {
            var checkToCart = _applicationDBContext.CartDetails.FirstOrDefault(e => e.CartId == CartId);
            var productDetails = _applicationDBContext!.ShoppingInput.FirstOrDefault(w => w.Id == cartItems.ProductId);
            if (checkToCart == null) return BadRequest("this product is not yet available");
            if (productDetails == null) return BadRequest("input the right value");
            checkToCart.TotalPrice = checkToCart.Price * cartItems.Quantity;
            checkToCart.ProductImage = productDetails.ProductImage;
            checkToCart.Price = productDetails.ProductPriceM;

            _applicationDBContext.Update(checkToCart);
            _applicationDBContext.SaveChangesAsync();
            return Ok(checkToCart);
        }
            //Change to post.
            //[HttpPatch]
            //public async Task<IActionResult> PatchToCart(int CartId, [FromBody] JsonPatchDocument<CartItems> cartItem)
            //{
            //    var checkToCart = _applicationDBContext.CartDetails.FirstOrDefault(e => e.CartId == CartId);
            //    if (checkToCart == null) return BadRequest("this product is not yet available");
            //    if (checkToCart == null && checkToCart.Items == null)
            //    {
            //        return BadRequest("Man go back.");
            //    }

            //    var firstCartItem = checkToCart.Items.FirstOrDefault(x => x.CartItemId == checkToCart.CartId);
            //    var val = checkToCart.Items;
            //    int quantity = 0;
            //    var quantity1 = "";
            //    int quantity2 = 0;

            //    foreach (var item in val)
            //    {
            //        quantity = item.ProductId;
            //        quantity1 = item.Name;
            //        quantity2 = item.Quantity;
            //    }
            //    var cartItems = new CartItems()
            //    {
            //        CartItemId = checkToCart.CartId,
            //        ProductId = quantity,
            //        Name = quantity1,
            //        Quantity = quantity2,
            //    };
            //    cartItem!.ApplyTo(cartItems, ModelState);
            //    if (!ModelState.IsValid) return BadRequest(ModelState);

            //    if (firstCartItem != null)
            //    {
            //        firstCartItem.ProductId = cartItems.ProductId;
            //        firstCartItem.Name = cartItems.Name;
            //        firstCartItem.CartItemId = cartItems.CartItemId;
            //        firstCartItem.Quantity = cartItems.Quantity;
            //    }

            //    checkToCart.TotalPrice = cartItems.Quantity * checkToCart.Price;

            //    //Added this code to check if the value of the previous amount is same with the current one.
            //    var productDetails = _applicationDBContext!.ShoppingInput.FirstOrDefault(w => w.Id == checkToCart.Items.FirstOrDefault().ProductId);

            //    productDetails.ItemsRemaining = productDetails.ItemsRemaining - cartItems.Quantity;

            //    _applicationDBContext.SaveChanges();

            //    return NoContent();

            //}
            [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string user, int number)
        {
            //var UserDetails = _applicationDBContext!.CartDetails.Where(x => x.UserId == user);
            var UserDetails = _applicationDBContext!.CartDetails.FirstOrDefault(x => x.UserId == user && x.CartId == number);
            if (UserDetails == null) return BadRequest("You entered a wrong user or this cardId id not current");
             var rex = _applicationDBContext.CartDetails.Remove(UserDetails);
            _applicationDBContext.SaveChangesAsync();
            return Ok("Itm was deleted successfully");
        }

    }
}
