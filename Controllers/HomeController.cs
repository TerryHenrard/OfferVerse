using Microsoft.AspNetCore.Mvc;
using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using System.Diagnostics;
using AppUser = OfferVerse.Models.User;

namespace OfferVerse.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserDAL _userDAL;

        public HomeController(ILogger<HomeController> logger, IUserDAL userDAL)
        {
            _logger = logger;
            _userDAL = userDAL;
        }

        public IActionResult Index()
        {
            TempData["timeCredits"] = AppUser.GetUserInfo(_userDAL, 1).TimeCredits;
            return View(); //TODO: replace 1 with the id of the authenticated user in the session
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] 
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
