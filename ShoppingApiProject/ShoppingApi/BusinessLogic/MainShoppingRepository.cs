using ShoppingApi.InputData;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ShoppingApi.BusinessLogic
{
    public class MainShoppingRepository : IShoppingRepository
    {
        public ApplicationDBContext? _applicationDBContext;
        public MainShoppingRepository(ApplicationDBContext? applicationDBContext)
        {
         _applicationDBContext = applicationDBContext;
         //_applicationDBContext = applicationDBContext ?? throw new ArgumentNullException(nameof(_applicationDBContext));
        }
        public  async Task<ShoppingInput> Add(ShoppingInput shoppingInput)
        {
            _applicationDBContext!.Add(shoppingInput);
            await _applicationDBContext.SaveChangesAsync();
            return shoppingInput;
        }

        public async Task<IEnumerable<ShoppingInput>> GetAllItems()
        {
            if (_applicationDBContext!.ShoppingInput.Count<ShoppingInput>() == 0 || _applicationDBContext!.ShoppingInput == null)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.WriteLine("This Current database is an empty one please input values to the database");
                Console.ResetColor();
                return Enumerable.Empty<ShoppingInput>();
            }
            return _applicationDBContext.ShoppingInput;
        }

        //public ActionResult GetByBrandNAme(string name)
        //{
        //    var Items = _applicationDBContext!.ShoppingInput.All(s => s.BrandName == name);
        //    if (Items == null) return NotFound();
        //}

        public async Task<ShoppingInput> GetItem(int number)
        {
            number = (number == 0) ? 1 : number;
            var Value = await _applicationDBContext!.ShoppingInput.FindAsync(number);
            if (Value == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Value does not exist");
                Console.ResetColor();
                return new ShoppingInput();
            }
            return Value;
        }

        //public async Task<ShoppingInput> ModifyProduct(ShoppingInput existingItems, JsonPatchDocument<ShoppingInput> shoppingInput)
        //{
        //    var shoppingInputCreated = new ShoppingInput()
        //    {
        //        Id = existingItems.Id,
        //        ProductImage = existingItems.ProductImage,
        //        StarRatings = existingItems.StarRatings,
        //        ProductPriceM = existingItems.ProductPriceM,
        //        ProductPriceF = existingItems.ProductPriceF,
        //        BrandName = existingItems.BrandName,
        //        Categpories = existingItems.Categpories,
        //        CompanyImage = existingItems.CompanyImage,
        //        ItemsRemaining = existingItems.ItemsRemaining,
        //    };

        //    shoppingInput.ApplyTo(shoppingInputCreated, ModelState);
        //}
    }
}
