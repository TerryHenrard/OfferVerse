using Microsoft.AspNetCore.Mvc;
using OfferVerse.DAL.Interfaces;
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
    }
}
