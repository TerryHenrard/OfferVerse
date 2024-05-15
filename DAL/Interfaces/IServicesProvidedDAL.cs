using OfferVerse.Models;

namespace OfferVerse.DAL.Interfaces
{
    public interface IServicesProvidedDAL
    {
        public ServiceProvided GetServiceProvidedInfo(int sId);

        public bool ApplyServiceProvidedChanges(ServiceProvided sp, int id);
    }
}
