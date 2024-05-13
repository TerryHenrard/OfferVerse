using Microsoft.AspNetCore.Mvc;
using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using System.Reflection.Metadata.Ecma335;
using AppUser = OfferVerse.Models.User;

namespace OfferVerse.Controllers
{
    public class MyServicesController : Controller
    {
        private readonly IUserDAL dal;
        public MyServicesController(IUserDAL dal)
        {
            this.dal = dal;
        }
        public IActionResult MyServices()
        {
            return View(AppUser.GetAllServicesProvided(dal, 4));
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
            if (user.DeleteServiceProvided(dal, sId))
            {
                TempData["Deletion"] = "Service deleted";
            }

            return RedirectToAction(nameof(MyServices));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServiceProvided service)
        {
            bool test = AppUser.AddServiceProvided(dal, service, 4);
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
    }
}
