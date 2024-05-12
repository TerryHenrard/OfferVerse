using OfferVerse.Models;

namespace OfferVerse.DAL.Interfaces
{
    public interface ICommentaryDAL
    {
        public bool InsertCommentary(string content, int rating, int servicePId);
    }
}
