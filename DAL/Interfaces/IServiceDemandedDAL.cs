namespace OfferVerse.DAL.Interfaces
{
    public interface IServiceDemandedDAL
    {
        public bool FinalizeService(int serviceDId, int? nbHours);
    }
}
