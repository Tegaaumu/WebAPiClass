using Microsoft.AspNetCore.Mvc;
using ShoppingApi.BusinessLogic;
using Microsoft.AspNetCore.Identity;
using PayStack.Net;
using ShoppingApi.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShoppingApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        public ApplicationDBContext? _applicationDBContext;
        private UserManager<ApplicationUser>? _userManager;
        private readonly IConfiguration _configuration;
        private readonly string token;

        private PayStackApi PayStack { get; set; }
        public PaymentController(ApplicationDBContext? applicationDBContext, UserManager<ApplicationUser>? userManager, IConfiguration configuration)
        {
            _applicationDBContext = applicationDBContext;
            _userManager = userManager;
            _configuration = configuration;
            token = _configuration["Payment:PayStackSK"];
            PayStack = new PayStackApi(token);
        }
        // GET: api/<PaymentController>
        [HttpGet("GetPayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get()
        {
            var AllPaymentMade = _applicationDBContext!.PaymentDetails.Where(x => x.Status == true);
            if (AllPaymentMade.Any() == false) return BadRequest("No payment have been made");
            return Ok(AllPaymentMade);
        }

        [HttpPost("MakePayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        public async Task<IActionResult> MakePayment(string UserNameOrId)
        {
            var UserDetails = _applicationDBContext.Users.FirstOrDefault(x => x.Id == UserNameOrId || x.Email == UserNameOrId);
            if (UserNameOrId == null) return BadRequest("This User can be found in our database");
            if (User.Identity.IsAuthenticated)
            {
                var itemsFromCart = _applicationDBContext.CartDetails.Where(x =>x.UserId == UserDetails!.Id);
                if (itemsFromCart == null) return BadRequest("There is no item in the cart for this user");
                double TotalPrice = 0;
                foreach(var items in itemsFromCart)
                {
                    TotalPrice += items.TotalPrice;
                }
                var amount = Convert.ToInt32(TotalPrice);
                TransactionInitializeRequest request = new()
                {
                    AmountInKobo = amount * 100,
                    Email = UserDetails.Email,
                    Reference = Generate().ToString(),
                    Currency = "NGN"
                };
                TransactionInitializeResponse response = PayStack.Transactions.Initialize(request);
                if (response.Status)
                {
                    PaymentDetails payment = new PaymentDetails()
                    {
                        Amount = request.AmountInKobo / 100,
                        Name = UserDetails.UserName!,
                        Email = request.Email!,
                        TrxRef = request.Reference,
                        UserId_From_Cart = UserDetails.Id,
                    };
                    //New state added
                    List<Items_Payed_For> general = new List<Items_Payed_For>();
                    foreach (var items in itemsFromCart)
                    {
                        Items_Payed_For items_Payed_For = new Items_Payed_For()
                        {
                            Items_Payed = items.CartId,
                            User_Id = items.UserId,
                        };
                        general.Add(items_Payed_For);
                    }
                    payment.items_payed_for = general;
                        await _applicationDBContext.PaymentDetails.AddAsync(payment);
                    await _applicationDBContext.SaveChangesAsync();
                    return Ok($"Hereis the authentication Url {response.Data.AuthorizationUrl} and here is your code {request.Reference} with a message {response.Message}");
                }
                else
                {
                    return BadRequest(response.Message);
                }

            }
            return BadRequest("User is not yet Autheticated");
        }
        [HttpGet("VerifyPayment")]
        public async Task<IActionResult> Verify(string references)
        {
            TransactionVerifyResponse response = PayStack.Transactions.Verify(references);
            if (response.Data.Status == "success")
            {
                var transaction = _applicationDBContext!.PaymentDetails.Include(x => x.items_payed_for).Where(x => x.TrxRef == references).FirstOrDefault();
                //Newly added.
                var itemsFromCart = _applicationDBContext.CartDetails.Where(x => x.UserId == transaction.UserId_From_Cart);
                //Stop
                if (transaction == null) return BadRequest("The refernces inputed is invalid.");
                if (itemsFromCart == null) return BadRequest("The user is not in the cart");
                transaction.Status = true;
                //Newly added.
                var value = transaction.items_payed_for.Where(x => x.User_Id == transaction.UserId_From_Cart);
                foreach (var items in itemsFromCart)
                {
                    items.Paymemt_Status = true;
                }
                foreach (var items in value)
                {
                    items.Status = true;
                }
                _applicationDBContext.PaymentDetails.Update(transaction);
                await _applicationDBContext.SaveChangesAsync();
                return Ok($"{transaction.Email} you have successfully verified your payment. {response.Data.GatewayResponse}");
            }
            return BadRequest("Ths transaction was not successfull");
        }
        public static int Generate()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            return rand.Next(100000000, 999999999);
        }
 
    }
}
