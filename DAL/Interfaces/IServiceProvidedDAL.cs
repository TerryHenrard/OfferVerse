using OfferVerse.Models;

namespace OfferVerse.DAL.Interfaces
{
    public interface IServiceProvidedDAL
    {
        public ServiceProvided GetServiceProvidedInfo(int sId);
        public bool ApplyServiceProvidedChanges(ServiceProvided sp, int id);
        public List<ServiceProvided> GetServicesProvided(int pageNb, int servicesPerPage);
        public int GetNumberOfPages(int servicesPerPage);
        public ServiceProvided GetServiceProvided(int servicePId);
        public bool PutInFavorite(int servicePId, int userId);
        public bool DeleteInFavorite(int servicePId, int userId);
    }
}
