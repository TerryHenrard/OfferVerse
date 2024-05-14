using Microsoft.AspNetCore.Mvc;
using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using System.Reflection.Metadata.Ecma335;
using AppUser = OfferVerse.Models.User;

namespace OfferVerse.Controllers
{
    public class MyServicesController : Controller
    {
        private readonly IUserDAL _UserDal;
        private readonly ICategoryDAL _CategoryDal;
        public MyServicesController(IUserDAL userDal, ICategoryDAL catDal)
        {
            _UserDal = userDal;
            _CategoryDal = catDal;
        }
        public IActionResult MyServices()
        {
            return View(AppUser.GetAllServicesProvided(_UserDal, 4));
        }

        public IActionResult Confirm()
        {
            ViewData["sId"] = Request.Query["sId"];

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirm(AppUser user, int sId)
        {
            if (user.DeleteServiceProvided(_UserDal, sId))
            {
                TempData["Deletion"] = "Service deleted";
            }

            return RedirectToAction(nameof(MyServices));
        }

        public IActionResult Create()
        {
            ViewBag.categories = _CategoryDal.GetCategories();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServiceProvided service)
        {
            service.DatePriority = new DateTime(2000, 1, 1);
            bool test = AppUser.AddServiceProvided(_UserDal, service, 4);
            if (ModelState.IsValid)
            {
                TempData["success"] = "Service created successfully";
                return RedirectToAction(nameof(MyServices));
            }
            else
            {
                TempData["success"] = "Service not created successfully";
            }

            return RedirectToAction(nameof(MyServices));
        }

        public IActionResult Promote()
        {
            int sId = Convert.ToInt32(Request.Query["sId"].ToString());

            if(sId != 1 && AppUser.PromoteServiceProvided(_UserDal, sId))
            {
                TempData["success"] = "Service promoted successfully";
            }
            else
            {
                TempData["success"] = "Service not promoted";
            }
            
            return RedirectToAction(nameof(MyServices));
        }
    }
}