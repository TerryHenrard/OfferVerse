using Microsoft.AspNetCore.Mvc;
using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using OfferVerse.ViewModels;
using AppUser = OfferVerse.Models.User;

namespace OfferVerse.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserDAL _userDAL;
        private readonly ICommentaryDAL _commentaryDAL;

        public ProfileController(IUserDAL userDAL, ICommentaryDAL commentaryDAL)
        {
            _userDAL = userDAL;
            _commentaryDAL = commentaryDAL;
        }

        public IActionResult ShowProfile()
        {
            return View(AppUser.GetUserInfo(_userDAL, 1)); //TODO: replace 1 with the id of the authenticated user in the session
        }

        public IActionResult EditProfile()
        {
            //I get the information of the current user to pre-fill the fields
            return View(AppUser.GetUserInfo(_userDAL, 1)); //TODO: replace 1 with the id of the authenticated user in the session
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(AppUser user) 
        {
            if (!user.EditPassword)
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }
            if (ModelState.IsValid && user.ApplyProfileChanges(_userDAL))
            {
                TempData["message"] = "Profil edited with success";
                return RedirectToAction(nameof(ShowProfile));
            }
            
            return View(AppUser.GetUserInfo(_userDAL, 1)); //TODO: replace 1 with the id of the authenticated user in the session
        }

        public IActionResult ShowCreditsTransactions()
        {
            AppUser user = AppUser.GetUserInfo(_userDAL, 4); //TODO: replace 4 with the id of the authenticated user in the session
            List<ServiceDemanded> servicesDemanded = AppUser.GetTransactions(_userDAL, 4); //TODO: replace 4 with the id of the authenticated user in the session

            UserTransactionsViewModel viewModel = new()
            {
                ServicesDemanded = servicesDemanded,
                User = new(user.MemberId, user.FirstName, user.LastName)
            };

            return View(viewModel);
        }
        
        public IActionResult ShowInProgressServices()
        {
            AppUser user = AppUser.GetUserInfo(_userDAL, 4); //TODO: replace 4 with the id of the authenticated user in the session
            List<ServiceDemanded> servicesDemanded = AppUser.GetTransactions(_userDAL, 4, true); //TODO: replace 4 with the id of the authenticated user in the session

            UserTransactionsViewModel viewModel = new()
            {
                ServicesDemanded = servicesDemanded,
                User = new(user.MemberId, user.FirstName, user.LastName)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("FinalizeService")]
        public IActionResult FinalizeService(string serviceId)
        {
            return View(new FinalizeServiceViewModel()
            {
                ServiceDemanded = AppUser.GetInProgressTransaction(_userDAL, Convert.ToInt32(serviceId)),
                Commentary = new(),
                Report = new()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("FinalizeService-Commentary")]
        public IActionResult FinalizeService(Commentary commentary, int nbHours, string serviceId)
        {
            //TODO: terminer l'insertion en db du commentaire et compléter avec le bon nombre d'heure et la date de fin de service !!!
            if (ModelState.IsValid && Commentary.InsertCommentary(_commentaryDAL, commentary))
            {
                return RedirectToAction(nameof(ShowInProgressServices));
            }
            else
            {
                return View("FinalizeService", new FinalizeServiceViewModel()
                {
                    ServiceDemanded = AppUser.GetInProgressTransaction(_userDAL, Convert.ToInt32(serviceId)),
                    Commentary = new(),
                    Report = new()
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("FinalizeService-Report")]
        public IActionResult FinalizeService(Report report, string serviceId)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(ShowInProgressServices));
            }
            else
            {
                ViewData["ReportFormSubmittedWithErrors"] = true;
                return View("FinalizeService", new FinalizeServiceViewModel()
                {
                    ServiceDemanded = AppUser.GetInProgressTransaction(_userDAL, Convert.ToInt32(serviceId)),
                    Commentary = new(),
                    Report = new()
                });
            }
        }
    }
}
