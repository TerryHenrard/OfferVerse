using OfferVerse.DAL.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OfferVerse.Models
{
    public class User : Member
    {
        private string phoneNumber;
        private string postCode;
        private string streetNumber;
        private string streetName;
        private string city;

        [Display(Name = "Phone number")]
        [MaxLength(50, ErrorMessage = "50 characters maximum")]
        [MinLength(2, ErrorMessage = "8 characters minimum")]
        [Phone]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone number field required")]
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }

        [Display(Name = "Post code")]
        [StringLength(4, ErrorMessage = "Post code must contain 4 numbers")]
        [DataType(DataType.PostalCode)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Post code field required")]
        public string PostCode
        {
            get { return postCode; }
            set { postCode = value; }
        }

        [Display(Name = "Street number")]
        [MaxLength(5, ErrorMessage = "5 characters maximum")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Street number field required")]
        public string StreetNumber
        {
            get { return streetNumber; }
            set { streetNumber = value; }
        }

        [Display(Name = "Street name")]
        [MaxLength(50, ErrorMessage = "50 characters maximum")]
        [MinLength(2, ErrorMessage = "2 characters minimum")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Street name field required")]
        public string StreetName
        {
            get { return streetName; }
            set { streetName = value; }
        }

        [Display(Name = "City")]
        [MaxLength(50, ErrorMessage = "50 characters maximum")]
        [MinLength(4, ErrorMessage = "4 characters minimum")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City field required")]
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
            : base(memberId, email, password, firstName, lastName)
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

        public static List<ServiceProvided> GetAllServicesProvided(IUserDAL dal, int memberId)
        {
            return dal.GetAllServicesProvided(memberId);
        }
    }
}
