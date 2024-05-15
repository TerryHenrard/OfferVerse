using OfferVerse.Models;

namespace OfferVerse.ViewModels
{
    public class UserTransactionsViewModel
    {
        public User User { get; set; }
        public List<ServiceDemanded> ServicesDemanded { get; set; }
    }
}