using Microsoft.AspNetCore.Mvc;
using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using OfferVerse.ViewModels;
using System.Reflection.Metadata.Ecma335;
using AppUser = OfferVerse.Models.User;

namespace OfferVerse.Controllers
{
    public class AdminController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IUserDAL _userDAL;
        private readonly IReportDAL _reportDAL;
        private readonly IServiceProvidedDAL _serviceProvidedDAL;
        private const int servicesPerPage = 12;

        public AdminController(ILogger<HomeController> logger, IUserDAL userDAL, IServiceProvidedDAL serviceProvidedDAL, IReportDAL reportDal)
        {
            _logger = logger;
            _userDAL = userDAL;
            _serviceProvidedDAL = serviceProvidedDAL;
            _reportDAL = reportDal;
        }

        private int GetUserIdFromSession()
        {
            return HttpContext.Session.GetInt32("userId") ?? 0;
        }


        public IActionResult Admin()
        {
            bool isAdmin = _userDAL.IsAdmin(GetUserIdFromSession());
            if (isAdmin)
            {
                TempData["displayLogout"] = GetUserIdFromSession() != 0;
                List<Report> reports = _reportDAL.GetReports();
                return View(reports);
            }
            else
            {
                TempData["message"] = "You are not an admin.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult Sanction(int id)
        {
            bool success = _reportDAL.SanctionUser(id);
            if (success)
                TempData["message"] = "The user has been sanctionned.";
            else
                TempData["message"] = "Dear admin, an error has occured.";

            return RedirectToAction(nameof(Admin));
        }

        [HttpPost]
        public IActionResult DeleteReport(int reportId)
        {
            bool success = _reportDAL.DeleteReport(reportId);
            if (success)
                TempData["message"] = "The report has been deleted.";
            else
                TempData["message"] = "Dear admin, an error has occured.";

            return RedirectToAction(nameof(Admin));
        }
    }
}
