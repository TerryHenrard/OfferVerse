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
        private readonly IReportDAL _reportDAL;

        public ProfileController(IUserDAL userDAL, ICommentaryDAL commentaryDAL, IServiceDemandedDAL serviceDemandedDAL, IReportDAL reportDAL)
        {
            _userDAL = userDAL;
            _commentaryDAL = commentaryDAL;
            _serviceDemandedDAL = serviceDemandedDAL;
            _reportDAL = reportDAL;
        }

        private int GetUserIdFromSession()
        {
            return HttpContext.Session.GetInt32("userId") ?? 0;
        }

        public IActionResult ShowProfile()
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }

            int userId = GetUserIdFromSession();
            if (userId == 0)
            {
                return RedirectToAction("login");
            }

            return View(AppUser.GetUserInfo(_userDAL, userId));
        }


        public IActionResult EditProfile()
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }

            return View(AppUser.GetUserInfo(_userDAL, GetUserIdFromSession())); //TODO: replace 1 with the id of the authenticated user in the session
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(AppUser user) 
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }

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
            
            return View(AppUser.GetUserInfo(_userDAL, GetUserIdFromSession())); //TODO: replace 1 with the id of the authenticated user in the session
        }

        public IActionResult ShowCreditsTransactions()
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }

            AppUser user = AppUser.GetUserInfo(_userDAL, GetUserIdFromSession()); //TODO: replace 4 with the id of the authenticated user in the session
            List<ServiceDemanded> servicesDemanded = AppUser.GetTransactions(_userDAL, GetUserIdFromSession()); //TODO: replace 4 with the id of the authenticated user in the session

            UserTransactionsViewModel viewModel = new()
            {
                ServicesDemanded = servicesDemanded,
                User = new(user.MemberId, user.FirstName, user.LastName)
            };

            return View(viewModel);
        }

        public IActionResult ShowInProgressServices()
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }

            AppUser user = AppUser.GetUserInfo(_userDAL, GetUserIdFromSession()); //TODO: replace 4 with the id of the authenticated user in the session
            List<ServiceDemanded> servicesDemanded = AppUser.GetTransactions(_userDAL, GetUserIdFromSession(), true); //TODO: replace 4 with the id of the authenticated user in the session

            UserTransactionsViewModel viewModel = new()
            {
                ServicesDemanded = servicesDemanded,
                User = new(user.MemberId, user.FirstName, user.LastName)
            };

            return View(viewModel);
        }


        public IActionResult ReviewService(int ServiceDId)
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }

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
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }

            if (!string.IsNullOrEmpty(viewModel.Commentary.Content) &&
                viewModel.Commentary.Content.Length >= 15 && 
                viewModel.Commentary.Content.Length <= 200 && 
                viewModel.Commentary.Rating >= 0 &&
                viewModel.Commentary.Rating <= 5 &&
                viewModel.ServiceDemanded.NbHours >= 1 &&
                viewModel.ServiceDemanded.NbHours <= 200 &&
                serviceDId > 0 &&
                servicePId > 0 &&
                Commentary.InsertCommentary(_commentaryDAL, viewModel.Commentary.Content, viewModel.Commentary.Rating, servicePId) &&
                ServiceDemanded.FinalizeService(_serviceDemandedDAL, serviceDId, viewModel.ServiceDemanded.NbHours) &&
                AppUser.DebitUser(_userDAL, GetUserIdFromSession(), viewModel.ServiceDemanded.NbHours) && //TODO: remplacer 4 par l'utilisateur connecté
                AppUser.CreditUser(_userDAL, servicePId, viewModel.ServiceDemanded.NbHours))
            {
                TempData["timeCredits"] = AppUser.GetUserInfo(_userDAL, GetUserIdFromSession()).TimeCredits;
                TempData["message"] = "Service well finalized";

                AppUser user = AppUser.GetUserInfo(_userDAL, GetUserIdFromSession()); //TODO: replace 4 with the id of the authenticated user in the session
                List<ServiceDemanded> servicesDemanded = AppUser.GetTransactions(_userDAL, GetUserIdFromSession(), true); //TODO: replace 4 with the id of the authenticated user in the session

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

        public IActionResult ReportUser(int reportedId)
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }

            ViewData["reportedId"] = reportedId;
            return View(new Report());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReportUser(Report report, int reportedId)
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }

            ModelState.Remove("ReportId");
            ModelState.Remove("Reporter");
            ModelState.Remove("Reported");

            if (ModelState.IsValid)
            {
                Report report2 = new()
                {
                    Title = report.Title,
                    Description = report.Description,
                    Reported = new(reportedId),
                    Reporter = new(GetUserIdFromSession()),
                };

                if (Report.InsertReport(_reportDAL, report2))
                {
                    TempData["message"] = "User well reported";
                    return RedirectToAction(nameof(ShowInProgressServices));
                }
            }
            return View(report);
        }

        public IActionResult ShowFavorites()
        {
            return View(AppUser.GetFavorites(_userDAL, GetUserIdFromSession()));
        }
    }
}
