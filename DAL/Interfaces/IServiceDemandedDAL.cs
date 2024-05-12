namespace OfferVerse.DAL.Interfaces
{
    public interface IServiceDemandedDAL
    {
        public bool FinalizeService(int serviceDId, int? nbHours);
        public bool DebitDemander(int serviceDId, int? nbHours);
        public bool CreditProvider(int servicePId, int? nbHours);
    }
}
