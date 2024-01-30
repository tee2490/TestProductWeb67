using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductWeb.Data;
using ProductWeb.ViewModels;

namespace ProductWeb.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]
    public class OrderController : Controller
	{
		private readonly ProductContext _productContext;
		private readonly IWebHostEnvironment _webHostEnvironment;


        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(ProductContext productContext, IWebHostEnvironment webHostEnvironment)
        {
			_productContext = productContext;
			_webHostEnvironment = webHostEnvironment;
		}

        public IActionResult Index()
		{
			var orderHeaders = _productContext.OrderHeaders.ToList();

			return View(orderHeaders);
		}

		public IActionResult Detail(int id) 
		{
			OrderVM orderVM = new()
			{
				OrderHeader = _productContext.OrderHeaders.Include(x=>x.User).FirstOrDefault(x => x.Id == id),
				OrderDetail = _productContext.OrderDetails.Include(x=>x.Product).Where(x=>x.OrderId == id).ToList(),
			};

			orderVM.OrderHeader.PaymentImage = SD.PaymentPath+"\\"+orderVM.OrderHeader.PaymentImage;


            return View(orderVM);
		}

        [HttpPost]
        //[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderHeader()
        {
            var orderHeaderFromDb = _productContext.OrderHeaders.Find(OrderVM.OrderHeader.Id);

            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

            _productContext.OrderHeaders.Update(orderHeaderFromDb);
            _productContext.SaveChanges();

            TempData["message"] = "Order Header Updated Successfully.";

            return RedirectToAction("Detail", "Order", new { id = orderHeaderFromDb.Id });
        }

        [HttpPost]
        public IActionResult StatusOrder(string status)
        {
            var orderHeaderFromDb = _productContext.OrderHeaders.Find(OrderVM.OrderHeader.Id);

            if (orderHeaderFromDb.OrderStatus == SD.StatusPending)
            {
                orderHeaderFromDb.OrderStatus = status;
                TempData["message"] = "Status has been updated Succesfully.";
                _productContext.SaveChanges();
            }
            else
            {
                TempData["message"] = "Can't update because status has ended.";
            }

            return RedirectToAction("Detail", "Order", new { id = OrderVM.OrderHeader.Id });
        }



    }
}
