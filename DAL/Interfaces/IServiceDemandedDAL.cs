using OfferVerse.Models;

namespace OfferVerse.DAL.Interfaces
{
    public interface IServiceDemandedDAL
    {
        public bool FinalizeService(int serviceDId, int? nbHours);

        public List<ServiceDemanded> GetServicesDemanded(int uId);
    }
}
