using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ShoppingApi.BusinessLogic;
using ShoppingApi.Cart;
using ShoppingApi.OutputModel;

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
        public CartController(ApplicationDBContext? applicationDBContext, UserManager<ApplicationUser> userManager)
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
            if (_applicationDBContext!.CartDetails.Any() == false)
            {
                return BadRequest("There is no item in our cart");
            }
            return Ok(_applicationDBContext.CartDetails);
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
        public async Task<IActionResult> PostToCart(CartItem productId)
        {
            var productDetails = _applicationDBContext!.ShoppingInput.FirstOrDefault(w => w.Id == productId.ProductId);
            var UserDetails2 = _userManager.Users.FirstOrDefault().Id;
            if (productDetails == null) return BadRequest("this product is not yet available");
            if (UserDetails2 == null) return BadRequest("LogIn before you can add item to cart");
            productId.Price = productDetails.ProductPriceM;
            productId.Name = productDetails.Name;
            List<CartItem> _cartItem = new List<CartItem>();

            _cartItem.Add(productId);
            //conflict in datatype
            var CartDetails = new CartDetails()
            {
                UserId = UserDetails2,
                ProductImage = productDetails.ProductImage,
                CartItemId = productId.CartItemId,
                Items = _cartItem
            };
            //Implement substracting the remaining product from the shopping items
            var AddToCart = _applicationDBContext.CartDetails.Add(CartDetails);
            return Ok("Item added sucessfully");
        }

        [HttpPatch]
        public async Task<IActionResult> PatchToCart(int CartId, [FromBody] JsonPatchDocument<CartItem> cartItem)
        {
            var checkToCart = _applicationDBContext.CartDetails.FirstOrDefault(e => e.CartItemId == CartId);
            if (checkToCart == null) return BadRequest("this product is not yet available");

            var cartItems = new CartItem()
            {
                CartItemId = checkToCart.CartItemId,
                ProductId = checkToCart.Items.FirstOrDefault().ProductId,
                Name = checkToCart.Items.FirstOrDefault().Name,
                Quantity = checkToCart.Items.FirstOrDefault().Quantity,
            };
            cartItem!.ApplyTo(cartItems, ModelState);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            checkToCart.Items.FirstOrDefault().ProductId = cartItems.ProductId;
            checkToCart.Items.FirstOrDefault().Name = cartItems.Name;
            checkToCart.Items.FirstOrDefault().CartItemId = cartItems.CartItemId;
            checkToCart.Items.FirstOrDefault().Quantity = cartItems.Quantity;

            _applicationDBContext.SaveChanges();

            return NoContent();


        }

    }
}
