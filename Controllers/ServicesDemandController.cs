using Microsoft.AspNetCore.Mvc;
using OfferVerse.DAL.Interfaces;
using OfferVerse.Models;
using AppUser = OfferVerse.Models.User;

namespace OfferVerse.Controllers
{
    public class ServicesDemandController : Controller
    {
        private readonly IServiceDemandedDAL _SdDAL;
        private readonly IUserDAL _UserDAL;

        public ServicesDemandController(IServiceDemandedDAL SdDal, IUserDAL userDAL)
        {
            _SdDAL = SdDal;
            _UserDAL = userDAL;
        }

        private int GetUserIdFromSession()
        {
            return HttpContext.Session.GetInt32("userId") ?? 0;
        }

        public IActionResult ViewServicesDemand()
        {
            if (GetUserIdFromSession() == 0)
            {
                return RedirectToAction("Connect", "Home");
            }
            return View(ServiceDemanded.GetServicesDemanded(_SdDAL, GetUserIdFromSession()));
        }

        public IActionResult Accept(int demandId)
        {
            if(AppUser.AcceptDemand(_UserDAL, demandId))
            {
                TempData["message"] = "Demand accepted";
            }
            else
            {
                TempData["message"] = "Demand not accepted";
            }

            return RedirectToAction(nameof(ViewServicesDemand));
        }

        public IActionResult Refuse(int demandId) 
        {
            if(AppUser.RefuseDemand(_UserDAL, demandId))
            {
                TempData["message"] = "Demand refused";
            }
            else
            {
                TempData["message"] = "Demand not refused";
            }

            return RedirectToAction(nameof(ViewServicesDemand));
        }
    }
}
