using Microsoft.AspNetCore.Mvc;
using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using OfferVerse.ViewModels;
using System.Reflection.Metadata.Ecma335;
using AppUser = OfferVerse.Models.User;

namespace OfferVerse.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserDAL _userDAL;
        private readonly ICommentaryDAL _commentaryDAL;
        private readonly IServiceDemandedDAL _serviceDemandedDAL;

        public ProfileController(IUserDAL userDAL, ICommentaryDAL commentaryDAL, IServiceDemandedDAL serviceDemandedDAL)
        {
            _userDAL = userDAL;
            _commentaryDAL = commentaryDAL;
            _serviceDemandedDAL = serviceDemandedDAL;
        }

        public IActionResult ShowProfile()
        {
            return View(AppUser.GetUserInfo(_userDAL, 1)); //TODO: replace 1 with the id of the authenticated user in the session
        }

        public IActionResult EditProfile()
        {
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


        public IActionResult ReviewService(int ServiceDId)
        {
            ReviewServiceViewModel viewModel = new()
            {
                ServiceDemanded = AppUser.GetInProgressTransaction(_userDAL, ServiceDId),
                Commentary = new()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReviewService(ReviewServiceViewModel viewModel, int servicePId, int serviceDId)
        {
            if (!String.IsNullOrEmpty(viewModel.Commentary.Content) &&
                viewModel.Commentary.Content.Length >= 15 && 
                viewModel.Commentary.Content.Length <= 200 && 
                viewModel.Commentary.Rating >= 1 &&
                viewModel.Commentary.Rating <= 5 &&
                viewModel.ServiceDemanded.NbHours >= 1 &&
                viewModel.ServiceDemanded.NbHours <= 200 &&
                serviceDId > 0 &&
                servicePId > 0 &&
                Commentary.InsertCommentary(_commentaryDAL, viewModel.Commentary.Content, viewModel.Commentary.Rating, servicePId) &&
                ServiceDemanded.FinalizeService(_serviceDemandedDAL, serviceDId, viewModel.ServiceDemanded.NbHours) &&
                ServiceDemanded.DebitDemander(_serviceDemandedDAL, 4, viewModel.ServiceDemanded.NbHours) &&
                ServiceDemanded.CreditProvider(_serviceDemandedDAL, servicePId, viewModel.ServiceDemanded.NbHours))
            {
                TempData["message"] = "Service well finalized";

                AppUser user = AppUser.GetUserInfo(_userDAL, 4); //TODO: replace 4 with the id of the authenticated user in the session
                List<ServiceDemanded> servicesDemanded = AppUser.GetTransactions(_userDAL, 4, true); //TODO: replace 4 with the id of the authenticated user in the session

                UserTransactionsViewModel viewModel2 = new()
                {
                    ServicesDemanded = servicesDemanded,
                    User = new(user.MemberId, user.FirstName, user.LastName)
                };

                return RedirectToAction(nameof(ShowInProgressServices), viewModel2);
            }

            ReviewServiceViewModel viewModel3 = new()
            {
                ServiceDemanded = AppUser.GetInProgressTransaction(_userDAL, serviceDId),
                Commentary = new()
            };
            return View(viewModel3);
        }
    }
}
