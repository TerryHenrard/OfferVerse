using Microsoft.AspNetCore.Mvc;
using OfferVerse.DAL.Interfaces;
using AppUser = OfferVerse.Models.User;

namespace OfferVerse.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserDAL _userDAL;

        public ProfileController(IUserDAL userDAL)
        {
            _userDAL = userDAL;
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
            int test = user.MemberId;
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
            return View();
        }
    }
}
