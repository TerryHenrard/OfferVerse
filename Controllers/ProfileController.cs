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

        public IActionResult Profile()
        {
            return View(AppUser.GetUserInfo(_userDAL, 1));
        }
    }
}
