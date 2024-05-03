using OfferVerse.Models;

namespace OfferVerse.DAL.Interfaces
{
    public interface IUserDAL
    {
        public User GetUserInfo(int id);
    }
}
