using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductWeb.ViewModels;

namespace ProductWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductContext _productContext;

        public ProductController(ProductContext productContext)
        {
            _productContext = productContext;
        }

        public IActionResult Index()
        {
            var products = _productContext.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        public IActionResult UpCreate(int? id)
        {
            var productVM = new ProductVM()
            {
                Product = new(),
                CategoryList = _productContext.Categories.Select(item=>new SelectListItem 
                {
                    Text = item.Name,
                    Value= item.Id.ToString(),
                })
            };

            if (id != null && id != 0)
            {
                //update
                productVM.Product = _productContext.Products.Find(id);

                if(productVM.Product == null)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(productVM);
        }

        [HttpPost]
        public IActionResult UpCreate(ProductVM productVM)
        {
            var id = productVM.Product.Id;

            if (id != 0)
            {
                //update
                _productContext.Update(productVM.Product);
            }
            else
            {
                //create
                _productContext.Add(productVM.Product);
            }

            _productContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id) 
        {
            var product = _productContext.Products.Find(id);

            if (product == null) 
            {
                TempData["message"] = "ไม่พบข้อมูล";
                return RedirectToAction(nameof(Index));
            }

            _productContext.Remove(product);
            _productContext.SaveChanges();
            TempData["message"] = "ลบสำเร็จ";
            return RedirectToAction(nameof(Index));
        }
    }
}
