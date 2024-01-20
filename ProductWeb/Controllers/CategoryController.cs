using Microsoft.AspNetCore.Mvc;

namespace ProductWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ProductContext _productContext;

        public CategoryController(ProductContext productContext)
        {
            _productContext = productContext;
        }
        public IActionResult Index()
        {
            var result = _productContext.Categories.ToList();
            return View(result);
        }

        public IActionResult UpCreate(int? id) 
        {
            var category = new Category();

            if (id == null || id == 0)
            {
                //create
            }
            else
            {
                //update
                category = _productContext.Categories.Find(id);
                if(category == null) 
                {
                    TempData["message"] = "ไม่พบข้อมูล";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult UpCreate(Category category)
        {
            var id = category.Id;

            if (id == 0)
            {
                //create
                _productContext.Categories.Add(category);
            }
            else
            {
                //update
                _productContext.Update(category);
            }

            _productContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id) 
        {
            var category = _productContext.Categories.Find(id);

            if (category != null)
            {
                _productContext.Categories.Remove(category);
                _productContext.SaveChanges();
                TempData["message"] = "ลบสำเร็จเรียบร้อย";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
