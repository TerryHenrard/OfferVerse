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
            AppUser user = AppUser.GetUserInfo(_userDAL, 1); //TODO: replace 1 with the id of the authenticated user in the session

            HttpContext.Session.SetInt32("userId", user.MemberId); //TODO: remplacer par l'id de l'utilisateur connecté
            TempData["timeCredits"] = user.TimeCredits;

            return View(); 
        }

        public IActionResult Connect()
        {
            return View();
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
