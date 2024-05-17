using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingApi.BusinessLogic;
using ShoppingApi.InputData;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShoppingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ApplicationDBContext? _applicationDBContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _RoleManager;

        public RoleController(ApplicationDBContext? applicationDBContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _applicationDBContext = applicationDBContext;
            _userManager = userManager;
            _RoleManager = roleManager;
        }
        // GET: api/<RoleController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
        [Authorize(Policy = "Admintration")]
        public async Task<IActionResult> AddRole(string addRoleName)
        {
            if (addRoleName == null || int.TryParse(addRoleName, out int outbox)) return BadRequest($"Enter a proper string or numbers are not allowed {addRoleName}");
            var checkRoleDB = _RoleManager.Roles.FirstOrDefault(x => x.Id == addRoleName || x.Name == addRoleName);
            if (checkRoleDB != null) return BadRequest("This user is already on our database");
            IdentityRole identityRole = new IdentityRole()
            {
                Name = (addRoleName).ToLower()
            };
             IdentityResult result = await _RoleManager.CreateAsync(identityRole);
            if (result.Succeeded)
            {
                return Ok("Successfully added a new role");
            }
            else
            {
                foreach(IdentityError items in result.Errors)
                {
                    ModelState.AddModelError("", items.Description);
                }

                var modelStateErrors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).ToList();

                return BadRequest($"PLease place in the proper value at {string.Join(", ", modelStateErrors)}");
            }
        }
        [HttpPost("AddUserRole")]
        //[Authorize(Policy = "Admintration")]
        [Authorize(Roles = "TegaUmu")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddUserRole(string User_Name_Id, string roleName)
        {
            var checkUerDetails = _userManager.Users.FirstOrDefault(x => x.Id == User_Name_Id || x.UserName == User_Name_Id);
            var checkRoleDB = _RoleManager.Roles.FirstOrDefault(x => x.Id == roleName || x.Name == roleName);
            if (checkRoleDB == null) return BadRequest("This user is not in our database");
            if (checkUerDetails == null) return BadRequest("Input provided is wrong");

            var result = await _userManager.IsInRoleAsync(checkUerDetails, roleName);

            if (result == true) return BadRequest("User is already in role");
            Claim[] claims = [
                new Claim(ClaimTypes.Email, checkUerDetails.Email!),
                    new Claim(ClaimTypes.Role, checkRoleDB.Name),
                    ];
            var tega = await _userManager.AddClaimsAsync(checkUerDetails!, claims);
            if (!tega.Succeeded) return BadRequest("Unable to save new claim value to database");

            await _userManager.AddToRoleAsync(checkUerDetails, checkRoleDB.Name!);
            return Ok("Successfully added user to a role");
            
        }
    }
}
