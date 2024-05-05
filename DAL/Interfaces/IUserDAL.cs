using OfferVerse.Models;

namespace OfferVerse.DAL.Interfaces
{
    public interface IUserDAL
    {
        public User GetUserInfo(int id);
        public bool ApplyProfileChanges(User userProfil);
        public List<ServiceProvided> GetAllServicesProvided(int memberId);
    }
}
