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
            return View(AppUser.GetUserInfo(_userDAL, 1)); //TODO: replace 1 with the id of the authenticated user
        }

        public IActionResult EditProfile()
        {
            //I get the information of the current user to pre-fill the fields
            return View(AppUser.GetUserInfo(_userDAL, 1)); //TODO: replace 1 with the id of the authenticated user
        }

        [HttpPost]
        public IActionResult EditProfile(AppUser user) 
        {
            if (ModelState.IsValid /* && methode static d'ajout en db */)
            {
                TempData["message"] = "Profil edited with success";
                return RedirectToAction(nameof(ShowProfile));
            }
            else
            {
                TempData["message"] = "Error while editing profile please try again and if the problem persist contact an administrator";
            }
            return View(AppUser.GetUserInfo(_userDAL, 1)); //TODO: replace 1 with the id of the authenticated user
        }
    }
}
