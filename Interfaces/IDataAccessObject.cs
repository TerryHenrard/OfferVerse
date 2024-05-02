using Microsoft.AspNetCore.SignalR;

namespace OfferVerse.Interfaces
{
    public interface IDataAccessObject
    {
        public void Get();
        public void GetAll();
        public void Insert();
        public void Update();
        public void Delete();
    }
}
