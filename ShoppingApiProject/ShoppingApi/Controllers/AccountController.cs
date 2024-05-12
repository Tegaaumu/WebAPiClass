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
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Microsoft.AspNetCore.Hosting.Server;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
//using LogLevel = Microsoft.Extensions.Logging.LogLevel;


namespace ShoppingApi.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private SignInManager<ApplicationUser>? _signInManager;
        private UserManager<ApplicationUser>? _userManager;
        public ApplicationDBContext? _applicationDBContext;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;
        private IConfiguration _configuration {get; set;}

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ApplicationDBContext? applicationDBContext, ILogger<AccountController> logger, IEmailSender emailSender, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _applicationDBContext = applicationDBContext;
            _logger = logger;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        //[AcceptVerbs("Get", "Post")]
        //public async Task<IActionResult> IsEmailUsed(string email)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user == null)
        //    {
        //        return Ok(true);
        //    }
        //    else
        //    {
        //        return Ok($"This user email{user}exist");
        //    }
        //}


        [HttpGet("GetRegisterDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                        Id = user.Id,
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
        //added new code [FromBody]
        public async Task<IActionResult> Register([FromBody]LoginDetails loginDetails)
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
                    Claim[] claims = [
                    new Claim(ClaimTypes.Email, loginDetails.Email),
                    new Claim(ClaimTypes.Role, loginDetails.UserName),
                    ];
                    var tega = await _userManager.AddClaimsAsync(users!,claims);

                    var token = _userManager.GenerateEmailConfirmationTokenAsync(users);
                    var confirmationLink = Url.Action("TokenConfirmation", "Account", new { userId = users.Id, token = token }, Request.Scheme);
                    //For token
                    _emailSender.SendEmailAsync(users.Email, "Confirm Your Code", $" This is your code {valueOfRandomValue}");
                    Console.WriteLine(confirmationLink);

                    _logger.Log(LogLevel.Warning, confirmationLink);
                    ValidaionTokenMesage validaionTokenMesage = new ValidaionTokenMesage()
                    {
                        Message = $"Go to {confirmationLink} and paste your information to Confirm your token.",
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
                    var text = Convert.ToString(validaionTokenMesage.Token)!;
                    var result = await _userManager.ConfirmEmailAsync(UserDetails, text);
                    if (result.Succeeded)
                    {
                        return Ok("Confirmation was successfully");
                    }
                }
                return NoContent();
            }
            return BadRequest("User is not valid");
        }

        [AllowAnonymous]
        [HttpPost("LogIn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(LoginViewModel signIn)
        {
            if (ModelState.IsValid)
            {

            //var user1 = await _userManager!.Users.FirstOrDefaultAsync(u => u.Email == signIn.UserName || u.UserName == signIn.UserName);
            var user = await _userManager.FindByEmailAsync(signIn.UserName);
            if (user == null)
            {
                return BadRequest("You entered a wrong username or email");
            }


                //if (user != null && !user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, signIn.Password)))
                //{
                //    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                //    var modelStateErrors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).ToList();
                //    return BadRequest($"Please place in the proper value at {string.Join(", ", modelStateErrors)}");
                //}
                 
                var test = await _userManager.CheckPasswordAsync(user, signIn.Password);
                if (!test) return BadRequest("The password is worng");

                var result = await _signInManager.CheckPasswordSignInAsync(user, signIn.Password, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var key = _configuration["AuthSettings:Key"];
                    var key1 = _configuration["AuthSettings:Key"];
                    var key2 = _configuration["AuthSettings:Audince"];
                    //New code added for JWT
                    var TokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
                    //var tokenDescriptor = new JwtSecurityToken(
                    var token = new JwtSecurityToken(
                        issuer: _configuration["AuthSettings:Issuer"],
                        audience: _configuration["AuthSettings:Audince"],
                        claims: await _userManager.GetClaimsAsync(user),
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: new SigningCredentials(TokenKey, SecurityAlgorithms.HmacSha256));
                    //var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
                    var TokenString = new JwtSecurityTokenHandler().WriteToken(token);
                    Console.WriteLine(TokenString);
                    return Ok("You have signed in successfully " + TokenString);
                    //return Ok("You have signed in successfully " + TokenString);
                }
                else
                {
                    return BadRequest(null + " " + result.GetType);
                }

                //if (result.IsLockedOut)
                //{
                //    return Ok("Account Locked");
                //}
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("IfUserIsSignedIn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSignIn()
        {
            //check here to print out claims for user.
            var te = ClaimsPrincipal.Current;
            //var User = HttpContext.User;
            var xser = _signInManager!.IsSignedIn(User);
            var user = HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                var userDetails = user.Claims.FirstOrDefault().Value;
                var userDetails1 = user.Identities.FirstOrDefault().Claims.FirstOrDefault().Value;
                return Ok($"{userDetails} is currently signed in");
            }


            return BadRequest("You are not signed in yet");
        }

        [HttpPost("LogOut")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok("Users has been logged out sucessfully");
        }

        [HttpPost("ForgetPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = await _userManager.FindByEmailAsync(model.Email);

                var user = _userManager.Users.FirstOrDefault(u => u.UserName == model.Email || u.Email == model.Email);
                var te = await _userManager.IsEmailConfirmedAsync(user);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {

                    //var valueOfRandomValue = GenerateRandmVAlues.RAndomNumber();

                    Task<string> token = _userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = user.Email, token = token }, Request.Scheme);
                    var passwordResetLink1 = Url.Action("ResetPassword", "Account");


                    ValidaionTokenMesage validaionTokenMesage = new ValidaionTokenMesage()
                    {
                        Message = $"Go to {passwordResetLink1} and paste your information to reset your token",
                        UserID = user.UserName,
                        Token = token,
                        UniqueCode = user.UniqueCode
                    };

                    return Ok(validaionTokenMesage);
                }
                
                return Ok("Invalid parameters");
            }
            return Ok(model);

        }

        [HttpPost("ResetPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var users = _userManager.Users.FirstOrDefault(u=> u.UserName == resetPassword.Email || u.Email == resetPassword.Email);
            if (users == null) return BadRequest("This user is not in our database");

            var result = await _userManager.ResetPasswordAsync(users, resetPassword.Token, resetPassword.Password);
            if (result.Succeeded)
            {
                if (await _userManager.IsLockedOutAsync(users))
                {
                    await _userManager.SetLockoutEndDateAsync(users, DateTimeOffset.UtcNow);
                }
                return Ok("Your password has been reset");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return Ok(resetPassword);
        }

        [HttpPost("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
        {
            var user = _signInManager.IsSignedIn(User);
            var Username = User.Identity.Name;
            //var userDetails = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == Username);
            var userDetails = await _userManager.GetUserAsync(User);
            if (user == false)
            {
                return BadRequest("This user must be signed in before he can change his password");
            }
            var result = await _userManager.ChangePasswordAsync(userDetails, changePassword.OldPassword, changePassword.NewPassword);
            if (result.Succeeded)
            {

                await _signInManager.RefreshSignInAsync(userDetails);
                return Ok("You have changed your password");
            }
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }

            var modelStateErrors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).ToList();
            return BadRequest($"PLease place in the proper value at {string.Join(", ", modelStateErrors)}");
        }

    }

}
