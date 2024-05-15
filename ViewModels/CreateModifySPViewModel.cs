using OfferVerse.Models;

namespace OfferVerse.ViewModels
{
    public class CreateModifySPViewModel
    {
        public ServiceProvided Sp { get; set; }

        public List<Category> Categories { get; set; }
    }
}
