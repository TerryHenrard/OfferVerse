using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using OfferVerse.ViewModels;
using System.Reflection.Metadata.Ecma335;
using AppUser = OfferVerse.Models.User;

namespace OfferVerse.Controllers
{
    public class MyServicesController : Controller
    {
        private readonly IUserDAL _UserDal;
        private readonly ICategoryDAL _CategoryDal;
        private readonly IServiceProvidedDAL _SpDal;
        public MyServicesController(IUserDAL userDal, ICategoryDAL catDal, IServiceProvidedDAL spDal)
        {
            _UserDal = userDal;
            _CategoryDal = catDal;
            _SpDal = spDal;
        }
        public IActionResult MyServices()
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }

            return View(AppUser.GetAllServicesProvided(_UserDal, GetUserIdFromSession()));
        }

        private int GetUserIdFromSession()
        {
            return HttpContext.Session.GetInt32("userId") ?? 0;
        }

        public IActionResult Confirm()
        {
            ViewData["sId"] = Request.Query["sId"];

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirm(AppUser user, int sId)
        {
            if (user.DeleteServiceProvided(_UserDal, sId))
            {
                TempData["Deletion"] = "Service deleted";
            }

            return RedirectToAction(nameof(MyServices));
        }

        public IActionResult Create()
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }

            CreateModifySPViewModel viewModel = new CreateModifySPViewModel()
            {
                Sp = new(),
                Categories = Category.GetCategories(_CategoryDal)
            };
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateModifySPViewModel viewModel)
        {   
            ModelState.Remove("Sp.Favorites");
            ModelState.Remove("Sp.Own");
            ModelState.Remove("Sp.Category.Name");
            ModelState.Remove("Sp.Category.Sp");
            ModelState.Remove("Categories");

            if (ModelState.IsValid &&
                AppUser.AddServiceProvided(_UserDal, viewModel.Sp, GetUserIdFromSession())
                )
            {
                TempData["success"] = "Service created successfully";
                return RedirectToAction(nameof(MyServices));
            }
            else
            {
                TempData["success"] = "Service not created successfully";
            }

            return View();
        }

        public IActionResult Promote(string sId)
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }

            if (AppUser.CheckCredits(_UserDal, GetUserIdFromSession()) && 
                Convert.ToInt32(sId) != 1 && 
                AppUser.PromoteServiceProvided(_UserDal, GetUserIdFromSession()))
            {
                TempData["success"] = "Service promoted successfully";
                TempData["timeCredits"] = AppUser.GetUserInfo(_UserDal, GetUserIdFromSession()).TimeCredits;
            }
            else
            {
                TempData["success"] = "Service not promoted";
            }
            
            return RedirectToAction(nameof(MyServices));
        }

        public IActionResult Modify(string sId)
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }

            CreateModifySPViewModel viewModel = new()
            {
                Sp = ServiceProvided.GetServiceProvidedInfo(_SpDal, Convert.ToInt32(sId)),
                Categories = Category.GetCategories(_CategoryDal)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modify(CreateModifySPViewModel viewModel, string sId)
        {
            ModelState.Remove("Sp.Own");
            ModelState.Remove("Sp.Favorites");
            ModelState.Remove("Sp.Category.Sp");
            ModelState.Remove("Sp.Category.Name");
            ModelState.Remove("Categories");

            if (ModelState.IsValid && viewModel.Sp.ApplyServiceProvidedChanges(_SpDal, viewModel.Sp, Convert.ToInt32(sId)))
            {
                TempData["success"] = "Service modified successfully";
                return RedirectToAction(nameof(MyServices));
            }
            else
            {
                TempData["success"] = "Service not modified";
            }

            return RedirectToAction(nameof(MyServices));
        }
    }
}