using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using PayStack.Net;
using ShoppingApi.BusinessLogic;
using ShoppingApi.OutputModel;
using ShoppingApi.Shipment;
using System.Net.NetworkInformation;
using System.Net;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShoppingApi.Controllers
{
    [Authorize(Roles = "user")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        private ApplicationDBContext? _applicationDBContext;
        private UserManager<ApplicationUser>? _userManager;
        private readonly IMapper _mapper;

        public ShipmentController(ApplicationDBContext? applicationDBContext, UserManager<ApplicationUser>? userManager, IMapper mapper)
        {
            _applicationDBContext = applicationDBContext;
            _userManager = userManager;
            _mapper = mapper;
        }
        // GET: api/<ShipmentController>

        //Implement Mapper Here
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var AllPaymentMade = _applicationDBContext!.ShipmentDetails;
            if (AllPaymentMade.Any() == false) return BadRequest("No payment have been made");
            //return Ok(AllPaymentMade);
            return Ok(_mapper.Map<List<ShippingOut>>(AllPaymentMade));
        }

        [HttpPost]
        public async Task<IActionResult> ShipItem(ShippingInput UserDetails)
        {
            var AllPaymentMade = _applicationDBContext!.PaymentDetails.FirstOrDefault(x => x.Status == true && (x.UserId_From_Cart == UserDetails.User_ID_Email || x.Email == UserDetails.User_ID_Email));
            if (AllPaymentMade == null) return BadRequest("This user have not yet made any Payment");
            ShipmentDetails shipmentDetails = new ShipmentDetails()
            {
                User_ID = AllPaymentMade.UserId_From_Cart,
                Total = AllPaymentMade.Amount,
                Address = UserDetails.Address,
                City_State_Country = UserDetails.City_State_Country                
            };
            await _applicationDBContext.ShipmentDetails.AddAsync(shipmentDetails);
            await _applicationDBContext.SaveChangesAsync();

            return Ok(shipmentDetails);

        }

        [HttpPatch("UpdateShipment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateShipment(string shipment_Id, [FromBody] JsonPatchDocument<ShipmentDetails> patchDocument)
        {
            if (patchDocument == null || shipment_Id == null) BadRequest("Place the wright values");
            var shipmentDetails = _applicationDBContext.ShipmentDetails.Where(x => x.Shipping_Id == shipment_Id).FirstOrDefault();
            if (shipmentDetails == null) return BadRequest("Item is not in Shipment");
            var shipmentItems = new ShipmentDetails()
            {
                User_ID = shipmentDetails.User_ID,
                Total = shipmentDetails.Total,
                Address = shipmentDetails.Address,
                City_State_Country = shipmentDetails.City_State_Country,
                Created_at = shipmentDetails.Created_at,
                Modified_at = DateTime.Now,
                Status = shipmentDetails.Status
            };
            patchDocument.ApplyTo(shipmentItems, ModelState);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            shipmentDetails.User_ID = shipmentItems.User_ID;
            shipmentDetails.Total = shipmentItems.Total;
            shipmentDetails.Address = shipmentItems.Address;
            shipmentDetails.City_State_Country = shipmentItems.City_State_Country;
            shipmentDetails.Created_at = shipmentItems.Created_at;
            shipmentDetails.Modified_at = shipmentItems.Modified_at;
            shipmentDetails.Status = shipmentItems.Status;
            await _applicationDBContext.SaveChangesAsync();
            return NoContent();


        }

    }
}
