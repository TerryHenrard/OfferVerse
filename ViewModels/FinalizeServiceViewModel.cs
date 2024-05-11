using OfferVerse.Models;

namespace OfferVerse.ViewModels
{
    public class FinalizeServiceViewModel
    {
        public ServiceDemanded ServiceDemanded { get; set; }
        public Report Report { get; set; }
        public Commentary Commentary { get; set; }

        public FinalizeServiceViewModel() { }  
    }
}
