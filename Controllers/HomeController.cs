﻿using Microsoft.AspNetCore.Mvc;
using OfferVerse.DAL;
using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using OfferVerse.ViewModels;
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
            ViewServiceViewModel ViewModel = new()
            {
                ServiceProvided = ServiceProvided.GetServiceProvided(_serviceProvidedDAL, servicePId),
                Favorites = AppUser.GetFavorites(_userDAL, GetUserIdFromSession()),
            };
            return View(ViewModel);  
        }

        public IActionResult AskForAService(int sProvidedId, int sProviderId)
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction(nameof(Connect));
            }

            if (GetUserIdFromSession() != sProviderId && AppUser.AskForAService(_userDAL, GetUserIdFromSession(), sProvidedId, sProviderId))
            {
                TempData["message"] = "Service Asked";
            }
            else
            {
                TempData["message"] = "Unable to Ask a service";
            }

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
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["message"] = "Wrong informations. Please, try again.";
                }
            }
            else
            {
                TempData["message"] = "Please, enter some validate inputs.";
            }
            return View(user);
        }

        public IActionResult PutInFavorite(int servicePId)
        {
            if (GetUserIdFromSession() == 0)
            {
                TempData["message"] = "must be logged in to put in favorite";
                return RedirectToAction(nameof(Connect));
            }

            if (ServiceProvided.PutInFavorite(_serviceProvidedDAL, servicePId, GetUserIdFromSession()))
            {
                TempData["message"] = "Added to your favorite";
            }
            else
            {
                TempData["message"] = "Not added to your favorite";
            }
            return RedirectToAction(nameof(ViewService), new { servicePId });
        }

        public IActionResult DeleteFavorite(int servicePId)
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction(nameof(Connect));
            }

            if (ServiceProvided.DeleteInFavorite(_serviceProvidedDAL, servicePId, GetUserIdFromSession()))
            {
                TempData["message"] = "Deleted from your favorites";
            }
            else
            {
                TempData["message"] = "Not deleted from your favorites";
            }
            return RedirectToAction(nameof(ViewService), new { servicePId });
        }

        public IActionResult Register()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                int result = _userDAL.Register(user.FirstName, user.LastName, user.Email, user.City, user.PostCode, user.StreetName, user.StreetNumber, user.Password, user.ConfirmPassword, user.PhoneNumber);
                if(result == -2) // password don't match
                {
                    TempData["message"] = "The password don't match.";
                }
                else if(result == -1)
                {
                    TempData["message"] = "An error has occured.";
                }
                else
                {
                    HttpContext.Session.SetInt32("userId", result);
                    TempData["message"] = "The account has been created";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                TempData["message"] = "Wrong informations. Please, try again.";
            }
            return View(user);
        }

        public IActionResult Logout()
        {
            if(GetUserIdFromSession() != 0)
            {
                HttpContext.Session.SetInt32("userId", 0);
                TempData["message"] = "Successfully disconnected.";
            }
            else
            {
                TempData["message"] = "You are not logged in.";
            }
            return RedirectToAction("Index");
        }
    }
}