﻿using Microsoft.AspNetCore.Mvc;
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

        public HomeController(ILogger<HomeController> logger, IUserDAL userDAL, IServiceProvidedDAL serviceProvidedDAL)
        {
            _logger = logger;
            _userDAL = userDAL;
            _serviceProvidedDAL = serviceProvidedDAL;
        }

        public IActionResult Index()
        {
            AppUser user = AppUser.GetUserInfo(_userDAL, 1); //TODO: replace 1 with the id of the authenticated user in the session

            HttpContext.Session.SetInt32("userId", user.MemberId); //TODO: remplacer par l'id de l'utilisateur connecté
            TempData["timeCredits"] = user.TimeCredits;

            int servicesPerPage = 12;

            ViewData["servicesPerPage"] = ServiceProvided.GetNumberOfPages(_serviceProvidedDAL, servicesPerPage);
            return View(ServiceProvided.GetServicesProvided(_serviceProvidedDAL, 1, servicesPerPage)); 
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
    }
}
