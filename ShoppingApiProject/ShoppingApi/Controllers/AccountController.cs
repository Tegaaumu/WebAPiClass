using ShoppingApi.BusinessLogic;
using ShoppingApi.InputData;
using ShoppingApi.OutputModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.SMTP;
//using LogLevel = Microsoft.Extensions.Logging.LogLevel;


namespace ShoppingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private SignInManager<ApplicationUser>? _signInManager;
        private UserManager<ApplicationUser>? _userManager;
        public ApplicationDBContext? _applicationDBContext;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ApplicationDBContext? applicationDBContext, ILogger<AccountController> logger, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _applicationDBContext = applicationDBContext;
            _logger = logger;
            _emailSender = emailSender;
        }


        [HttpGet("GetRegisterDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetLogInDetails()
        {
            var RegisteredUsers = _applicationDBContext!.Users;

            var outputLoginDetailsList = new List<OutputLoginDetails>();
            if (RegisteredUsers.Any())
            {
                foreach (var user in RegisteredUsers)
                {
                    var outputLoginDetails = new OutputLoginDetails
                    {
                        UserName = user.UserName,
                        Email = user.Email
                    };
                    outputLoginDetailsList.Add(outputLoginDetails);
                }
            }
            else
            {
                outputLoginDetailsList.Add(new OutputLoginDetails
                {
                    UserName = "You have not yet entered any user details",
                    Email = "sampleemail@gmail.com"
                });
            }

            return Ok(outputLoginDetailsList);
        }

        [HttpPost("RegisterUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Register(LoginDetails loginDetails)
        {
            if (ModelState.IsValid)
            {
                var valueOfRandomValue = GenerateRandmVAlues.RAndomNumber();
                var users = new ApplicationUser()
                {
                    Email = loginDetails.Email,
                    UserName = loginDetails.UserName,
                    UniqueCode = valueOfRandomValue
                };

                var RegisterUsers = await _userManager.CreateAsync(users, loginDetails.Password);
                if (RegisterUsers.Succeeded)
                {
                    var token = _userManager.GenerateEmailConfirmationTokenAsync(users);
                    var confirmationLink = Url.Action("TokenConfirmation", "Account", new { userId = users.Id, token = token }, Request.Scheme);

                    //await _emailSender.SendEmailAsync(users.Email, "Confirm Your Code", $"Here is code please enter it, for authentication purpose {valueOfRandomValue}. Thank you");
                    //Console.WriteLine(confirmationLink);
                    //_logger.Log(LogLevel.Warning, confirmationLink);
                    ValidaionTokenMesage validaionTokenMesage = new ValidaionTokenMesage()
                    {
                        Message = confirmationLink,
                        UserID = users.Id,
                        Token = token,
                        UniqueCode = valueOfRandomValue
                    };

                    return Ok(validaionTokenMesage);
                }
                foreach (var error in RegisterUsers.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);

                }
            }
            var modelStateErrors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).ToList();

            return BadRequest($"PLease place in the proper value at {string.Join(", ", modelStateErrors)}");
        }


        [HttpPost("Confirmregistration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmRegistration(InputValidaionTokenMesage validaionTokenMesage)
        {
            var UserDetails = await _userManager.FindByIdAsync(validaionTokenMesage.UserID);
            if (UserDetails != null)
            {
                var UserDetails2 = _userManager.Users.FirstOrDefault(v => v.Id == validaionTokenMesage.UserID);
                if (UserDetails2!.UniqueCode == validaionTokenMesage.UniqueCode)
                {
                    var result = await _userManager.ConfirmEmailAsync(UserDetails, Convert.ToString(validaionTokenMesage.Token)!);
                    if (result.Succeeded)
                    {
                        return Ok("Confirmation was successfully");
                    }
                }
                return NoContent();
            }
            return BadRequest("User is not valid");
        }
    }
}
