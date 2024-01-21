﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductWeb.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProductWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductContext _productContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ProductContext productContext, IWebHostEnvironment webHostEnvironment)
        {
            _productContext = productContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var products = _productContext.Products.Include(p => p.Category).ToList();

            foreach (var item in products)
            {
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    item.ImageUrl = SD.ProductPath + "\\" + item.ImageUrl;
                }
            }
            return View(products);
        }

        public IActionResult UpCreate(int? id)
        {
            var productVM = new ProductVM()
            {
                Product = new()
                {
                    Name = "test",
                    Description = "test",
                    Price = 1,
                },
                CategoryList = _productContext.Categories.Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString(),
                })
            };

            if (id != null && id != 0)
            {
                //update
                productVM.Product = _productContext.Products.Find(id);

                if (productVM.Product == null)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(productVM);
        }

        [HttpPost]
        public IActionResult UpCreate(ProductVM productVM)
        {
            if (!ModelState.IsValid) return View(productVM);

            #region การจัดรูปภาพ
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            var file = productVM.file;

            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(file.FileName);
                var uploads = wwwRootPath + SD.ProductPath; // wwwroot\images\product

                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);


                //กรณีมีรูปภาพเดิมต้องลบทิ้งก่อน
                if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                {
                    // wwwroot\images\product\test.jpg
                    var oldImagePath = Path.Combine(uploads, productVM.Product.ImageUrl);

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                //บันทึกรุปภาพใหม่
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                productVM.Product.ImageUrl = fileName + extension;
            }

            #endregion การจัดการูปภาพ

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

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                // wwwroot\images\products\test.jpg
                var oldImagePath = _webHostEnvironment.WebRootPath + SD.ProductPath + "\\" + product.ImageUrl;

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }


            _productContext.Remove(product);
            _productContext.SaveChanges();
            TempData["message"] = "ลบสำเร็จ";
            return RedirectToAction(nameof(Index));
        }
    }
}
