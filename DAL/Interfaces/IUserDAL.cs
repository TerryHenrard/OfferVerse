using OfferVerse.Models;

namespace OfferVerse.DAL.Interfaces
{
    public interface IUserDAL
    {
        public User GetUserInfo(int id);
        public bool ApplyProfileChanges(User userProfil);
        public List<ServiceProvided> GetAllServicesProvided(int memberId);
        public List<ServiceDemanded> GetTransactions(int memberId, bool inProgress);
        public ServiceDemanded GetInProgressTransaction(int serviceDId);
        public bool DeleteServiceProvided(int sId);
        public bool AddServiceProvided(ServiceProvided service, int uId);
        public bool PromoteServiceProvided(int sId);
        public bool CheckCredits(int uId);
        public bool DebitUser(int uId);
        public bool DebitUser(int memberId, int? nbHours);
        public bool CreditUser(int memberId, int? nbHours);
        public int CheckLogin(string mail, string password);

        public bool AskForAService(int sProvidedId, int sDemanderId, int sProviderId);

        public bool CheckIfServiceDemanded(int uId, int spId);
        public List<ServiceProvided> GetFavorites(int userId);
        public int Register(string firstName, string lastName, string mail, string city, string postCode, string streetName, string streetNumber, string password, string confirmPassword, string phoneNumber);
        public bool IsAdmin(int id);

    }
}
