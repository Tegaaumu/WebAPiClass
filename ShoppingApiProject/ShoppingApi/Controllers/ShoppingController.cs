using ShoppingApi.BusinessLogic;
using ShoppingApi.InputData;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ShoppingApi.InputData;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShoppingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        public IShoppingRepository _IShoppingRepository;
        public ApplicationDBContext? _applicationDBContext;

        public ShoppingController(IShoppingRepository iShoppingRepository, ApplicationDBContext? applicationDBContext)
        {
            _IShoppingRepository = iShoppingRepository;
            _applicationDBContext = applicationDBContext;
        }
        // GET: api/<Shopping>
        [HttpGet("all")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _IShoppingRepository.GetAllItems());
        }

        // GET api/<Shopping>/5
        [HttpGet]
        [Route("{id:int}", Name = "CreatedItems")]
        public async Task<IActionResult> Get(int id)
        {
            if (id is 0)
            {
                return BadRequest("Id can't be zero 0");
            }
            return Ok(await _IShoppingRepository.GetItem(id));
        }


        // GET api/<Shopping>/5
        [HttpGet]
        [Route("{name}/GetByBrandName", Name = "GetByBrandName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByBrandName(string name)
        {
            var Items = _applicationDBContext!.ShoppingInput.Where(s => s.Categpories == name);
            if (Items.Any() == false) return NotFound();

            return Ok(Items);

        }

        // POST api/<Shopping>
        [HttpPost("PlaceItems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        //public IActionResult Post([FromBody] ShoppingInput value)
        public async Task<IActionResult> Post([FromBody] ShoppingInput value)
        {
            if (ModelState.IsValid)
            {
                await _IShoppingRepository.Add(value);
                return CreatedAtRoute("CreatedItems", new { id = value.Id }, value);
            }
            return BadRequest("The form field was not completed");
        }

        // PUT api/<Shopping>/5
        [HttpPatch("{id}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStudentPartial(int id, [FromBody] JsonPatchDocument<ShoppingInput> shoppingItems)
        {
            if (shoppingItems == null || id == 0) BadRequest();

            var existingItems = _applicationDBContext!.ShoppingInput.Where(s => s.Id == id).FirstOrDefault();

            if (existingItems == null) return NotFound();

            //var endMe = _IShoppingRepository.ModifyProduct(existingItems, shoppingItems);

            var shoppingInputCreated = new ShoppingInput()
            {
                Id = existingItems.Id,
                ProductImage = existingItems.ProductImage,
                StarRatings = existingItems.StarRatings,
                ProductPriceM = existingItems.ProductPriceM,
                ProductPriceF = existingItems.ProductPriceF,
                BrandName = existingItems.BrandName,
                Categpories = existingItems.Categpories,
                CompanyImage = existingItems.CompanyImage,
                ItemsRemaining = existingItems.ItemsRemaining,
            };

            shoppingItems!.ApplyTo(shoppingInputCreated, ModelState);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            existingItems.ProductPriceM = shoppingInputCreated.ProductPriceM;
            existingItems.ProductPriceF = shoppingInputCreated.ProductPriceF;
            existingItems.ProductImage = shoppingInputCreated.ProductImage;
            existingItems.ItemsRemaining = shoppingInputCreated.ItemsRemaining;
            //Without writing this code you will note be able to see the change/modification applied.
            _applicationDBContext.SaveChanges();

            return NoContent();

        }

        //// DELETE api/<Shopping>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}