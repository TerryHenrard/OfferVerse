using OfferVerse.DAL.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OfferVerse.Models
{
    public class User : Member
    {
        /******Attributs******/
        private string phoneNumber;
        private string postCode;
        private string streetNumber;
        private string streetName;
        private string city;
        private int timeCredits;

        /******References******/
        List<ServiceDemanded>? servicesDemanded;
        List<ServiceProvided>? servicesProvided;


        /******Properties******/
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

        public int TimeCredits
        {
            get { return timeCredits; }
            set { timeCredits = value; }
        }

        public List<ServiceDemanded>? ServicesDemanded
        {
            get { return  servicesDemanded; }
            set { servicesDemanded = value; }
        }

        public List<ServiceProvided>? ServicesProvided
        {
            get { return servicesProvided; }
            set { servicesProvided = value; }
        }

        /******Constructors******/
        public User()
        {

        }
        
        public User(int userId)
            :base(userId)
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
                    string city,
                    int timeCredits)
            : base(memberId, email, password, firstName, lastName)
        {
            PhoneNumber = phoneNumber;
            PostCode = postCode;
            StreetNumber = streetNumber;
            StreetName = streetName;
            City = city;
            TimeCredits = timeCredits;
            ServicesDemanded = null;
            ServicesProvided = null;
        }

        public User(int id, string firstName, string lastName)
            :base(id, firstName, lastName)
        {
            
        }

        /******Traditionnals methods******/
        public bool AddServiceDemanded(ServiceDemanded sd)
        {
            bool result = false;
            if (sd != null && 
                ServicesDemanded != null && 
                !ServicesDemanded.Contains(sd))
            {
                ServicesDemanded.Add(sd);
                result = true;
            }
            return result;
        }

        public bool ApplyProfileChanges(IUserDAL dal)
        {
            return dal.ApplyProfileChanges(this);
        }

        /******static methods******/
        public static User GetUserInfo(IUserDAL dal, int memberId)
        {
            return dal.GetUserInfo(memberId);
        }

        public static List<ServiceProvided> GetAllServicesProvided(IUserDAL dal, int memberId)
        {
            return dal.GetAllServicesProvided(memberId);
        }
        public static List<ServiceDemanded> GetTransactions(IUserDAL dal, int memberId, bool inProgess = false)
        {
            return dal.GetTransactions(memberId, inProgess);
        }
        public static ServiceDemanded GetInProgressTransaction(IUserDAL dal, int serviceDId)
        {
            return dal.GetInProgressTransaction(serviceDId);
        }

        /******Existant methods overrided*******/
        public override string ToString()
        {
            return base.ToString() +  $", {PhoneNumber}, {PostCode}, {StreetNumber}, " +
                $"{StreetName}, {City}, {TimeCredits}, {ServicesDemanded}, {ServicesProvided}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            return ToString() == obj.ToString();
        }
    }
}
