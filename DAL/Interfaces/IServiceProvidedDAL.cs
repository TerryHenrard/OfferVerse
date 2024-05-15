using OfferVerse.Models;

namespace OfferVerse.DAL.Interfaces
{
    public interface IServiceProvidedDAL
    {
        public ServiceProvided GetServiceProvidedInfo(int sId);

        public bool ApplyServiceProvidedChanges(ServiceProvided sp, int id);

        public List<ServiceProvided> GetServicesProvided(int pageNb, int servicePerPage);
    }
}
