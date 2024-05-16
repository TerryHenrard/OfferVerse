using Microsoft.AspNetCore.Mvc;
using OfferVerse.DAL;
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
        private readonly IServiceProvidedDAL _serviceProvidedDAL;
        private const int servicesPerPage = 12;

        public HomeController(ILogger<HomeController> logger, IUserDAL userDAL, IServiceProvidedDAL serviceProvidedDAL)
        {
            _logger = logger;
            _userDAL = userDAL;
            _serviceProvidedDAL = serviceProvidedDAL;
        }

        private int GetUserIdFromSession()
        {
            return HttpContext.Session.GetInt32("userId") ?? 0;
        }

        public IActionResult Index(int pageNumber = 1)
        {
            AppUser user = AppUser.GetUserInfo(_userDAL, GetUserIdFromSession()); 
            HttpContext.Session.SetInt32("userId", user.MemberId); // TODO: replace with the id of the authenticated user
            TempData["timeCredits"] = user.TimeCredits;

            int totalPages = ServiceProvided.GetNumberOfPages(_serviceProvidedDAL, servicesPerPage);
            ViewData["currentPage"] = pageNumber;
            ViewData["totalPages"] = totalPages;
            List<ServiceProvided> sp = ServiceProvided.GetServicesProvided(_serviceProvidedDAL, pageNumber, servicesPerPage);

            return View(sp);
        }

        public IActionResult ViewService(int servicePId)
        {
            return View(ServiceProvided.GetServiceProvided(_serviceProvidedDAL, servicePId));  
        }

        public IActionResult AskForAService()
        {
            return RedirectToAction(nameof(Index));
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

        public IActionResult Connect()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Connect(User user)
        {
            ModelState.Remove("City");
            ModelState.Remove("LastName");
            ModelState.Remove("PostCode");
            ModelState.Remove("FirstName");
            ModelState.Remove("StreetName");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("StreetNumber");
            ModelState.Remove("ConfirmPassword");

            if (ModelState.IsValid)
            {
                int userId = _userDAL.CheckLogin(user.Email, user.Password);

                if(userId != 0)
                {
                    TempData["message"] = "Successfully connected.";
                    HttpContext.Session.SetInt32("userId", userId);
                    return RedirectToAction("ResultConnection");
                }
                else
                {
                    TempData["message"] = "Wrong informations. Please, try again.";
                    return RedirectToAction("ResultConnection");
                }
            }
            else
            {
                TempData["message"] = "Please, enter some validate inputs.";
            }
            return View(user);
        }

        public IActionResult ResultConnection()
        {
            return View();
        }

    }
}
