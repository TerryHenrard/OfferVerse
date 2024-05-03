using OfferVerse.DAL.Interfaces;

namespace OfferVerse.Models
{
    public class User : Member
    {
        private string phoneNumber;
        private string postCode;
        private string streetNumber;
        private string streetName;
        private string city;

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
        public string PostCode
        {
            get { return postCode; }
            set { postCode = value; }
        }
        public string StreetNumber
        {
            get { return streetNumber; }
            set { streetNumber = value; }
        }
        public string StreetName
        {
            get { return streetName; }
            set { streetName = value; }
        }
        public string City
        {
            get { return city; }
            set { city = value; }
        }

        public User()
        {

        }

        public User(int memberId, 
                    string email, 
                    string password, 
                    string firstName, 
                    string lastName, 
                    string phoneNumber, 
                    string postCode, 
                    string streetNumber, 
                    string streetName, 
                    string city)
            :base(memberId, email, password, firstName, lastName)
        {
            PhoneNumber = phoneNumber;
            PostCode = postCode;
            StreetNumber = streetNumber;
            StreetName = streetName;
            City = city;
        }

        /******statics methods******/

        public static User GetUserInfo(IUserDAL dal, int memberId)
        {
            return dal.GetUserInfo(memberId);
        }
    }
}
