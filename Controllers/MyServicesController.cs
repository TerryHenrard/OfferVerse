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
        private readonly IServicesProvidedDAL _SpDal;
        public MyServicesController(IUserDAL userDal, ICategoryDAL catDal, IServicesProvidedDAL spDal)
        {
            _UserDal = userDal;
            _CategoryDal = catDal;
            _SpDal = spDal;
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
            ViewBag.categories = _CategoryDal.GetCategories();
            //ModelState.IsValid is false because of the data annotations
            //So i'm verifying manually && ModelState.Remove() is not working
            if (!String.IsNullOrEmpty(service.Title) &&
                !String.IsNullOrEmpty(service.Description) &&
                service.Title.Length <= 50 &&
                service.Title.Length >= 5
                && service.Description.Length >= 5 &&
                service.Description.Length <= 200 &&
                AppUser.AddServiceProvided(_UserDal, service, 4)
                )
            {
                TempData["success"] = "Service created successfully";
                return RedirectToAction(nameof(MyServices));
            }
            else
            {
                TempData["success"] = "Service not created successfully";
            }

            return View();
        }

        public IActionResult Promote(string sId)
        {
            if(Convert.ToInt32(sId) != 1 && AppUser.PromoteServiceProvided(_UserDal, Convert.ToInt32(sId)))
            {
                TempData["success"] = "Service promoted successfully";
            }
            else
            {
                TempData["success"] = "Service not promoted";
            }
            
            return RedirectToAction(nameof(MyServices));
        }

        public IActionResult Modify(string sId)
        { 
            ViewBag.categories = _CategoryDal.GetCategories();//Put this method into the model + ViewModel for categories   
            return View(ServiceProvided.GetServiceProvidedInfo(_SpDal, Convert.ToInt32(sId)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modify(ServiceProvided sp, string sId)
        {
            ModelState.Remove("Own");
            ModelState.Remove("Favorites");
            bool test = sp.ApplyServiceProvidedChanges(_SpDal, sp, Convert.ToInt32(sId));
            if (ModelState.IsValid)
            {
                TempData["success"] = "Service modified successfully";
                return RedirectToAction(nameof(MyServices));
            }
            else
            {
                TempData["success"] = "Service not modified";
            }

            return RedirectToAction(nameof(MyServices));
        }
    }
}