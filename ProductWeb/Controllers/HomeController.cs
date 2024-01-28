using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductWeb.Services;
using System.Security.Claims;

namespace ProductWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductContext _productContext;
        private readonly ShoppingCartService _shoppingCartService;

        public HomeController(ProductContext productContext,ShoppingCartService shoppingCartService)
        {
            _productContext = productContext;
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            var products = _productContext.Products.ToList();

            foreach (var item in products)
            {
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    item.ImageUrl = SD.ProductPath + "\\" + item.ImageUrl;
                }
            }

            return View(products);
        }

       public IActionResult Details(int productId)
        {
            var product = _productContext.Products.Include(p=>p.Category)
                .FirstOrDefault(x => x.Id.Equals(productId));

            if (product == null) 
            {
                TempData["message"] = "ไม่พบข้อมูล";
                return RedirectToAction(nameof(Index));
            }

            ShoppingCart shoppingCart = new()
            {
                Product = product,
                Count = 1,
            };

            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize] //ตรวจสอบสิทธิ์ตาม role
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity; //Id
            //var user = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //shoppingCart.UserId = user.Value;
            shoppingCart.UserId = userId;

            var cartFromDb = _productContext.ShoppingCarts.
                FirstOrDefault(x => x.UserId == shoppingCart.UserId && x.ProductId==shoppingCart.ProductId);

            if(cartFromDb == null) 
            { 
                //ยังไม่เคยหยิบใส่ตะกร้า
                _shoppingCartService.Add(shoppingCart);
            }
            else 
            {
                //แสดงว่ามีในตะกร้าแล้ว
                _shoppingCartService.IncrementCount(cartFromDb, shoppingCart.Count);
            }
           
            _shoppingCartService.Save();

            return RedirectToAction(nameof(Index));
        }


    }
}
