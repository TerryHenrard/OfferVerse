using OfferVerse.DAL.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OfferVerse.Models
{
    public class ServiceProvided
    {
        private int serviceProvidedId;
        private string title;
        private string description;
        private bool priority;
        private DateTime? datePriority;

        //References
        private List<User>? favorites;
        private User own;
        private Category category;

        //Attributes
        public int ServiceProvidedId 
        {  
            get { return serviceProvidedId; } 
        }

        [Display(Name = "Title")]
        [MinLength(4, ErrorMessage = "4 characters minimum")]
        [MaxLength(50, ErrorMessage = "50 characters maximum")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title field required")]
        public string Title 
        { 
            get { return title; } 
            set { title = value; } 
        }

        [Display(Name = "Description")]
        [MinLength(4, ErrorMessage = "4 characters minimum")]
        [MaxLength(200, ErrorMessage = "200 characters maximum")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description required")]
        public string Description 
        { 
            get { return description; } 
            set { description = value; } 
        }
        public bool Priority 
        { 
            get { return priority; } 
            set { priority = value; } 
        }
        public DateTime? DatePriority 
        { 
            get {  return datePriority; } 
            set {  datePriority = value; } 
        }

        public User Own
        {
            get { return own; }
            set { own = value; }
        }

        public List<User> Favorites
        {
            get { return  favorites; }
            set { favorites = value; }
        }

        public Category Category 
        { 
            get { return category; }
            set { category = value; }
        }

        //Constructors
        public ServiceProvided()
        {

        }

        public ServiceProvided(int serviceProvidedId, string title, 
            string description, bool priority, DateTime? datePriority, int userId)
            :this(serviceProvidedId, title, description)
        {
            Priority = priority;
            DatePriority = datePriority;
            Own = new User(userId);
        }
        public ServiceProvided(string title, string description, int userId, int categoryId)
        {
            Title = title;
            Description = description;
            Own = new User(userId);
            Priority = false;
            Category = new Category(categoryId);
        }

        public ServiceProvided(int serviceProvidedId, string title, string description)
            :this(serviceProvidedId)
        {
            Title = title;
            Description = description;
        }

        public ServiceProvided(int serviceProvidedId, string title, string description, int catId, int uId)
            : this(serviceProvidedId, title, description)
        {
            Category = new Category(catId);
            Own = new User(uId);
        }

        public ServiceProvided(int serviceProvidedId)
        {
            this.serviceProvidedId = serviceProvidedId;
        }

        //Methods
        public static ServiceProvided GetServiceProvidedInfo(IServicesProvidedDAL dal, int sId)
        {
            return dal.GetServiceProvidedInfo(sId);
        }

        public bool ApplyServiceProvidedChanges(IServicesProvidedDAL dal, ServiceProvided sp, int id)
        {
            return dal.ApplyServiceProvidedChanges(sp, id);
        }


        //Overrided methods
        public override string ToString()
        {
            return $" ,{ServiceProvidedId}, {Title}, {Description}, {Priority}, {DatePriority}, {Favorites}, " +
                $"{Own}, {Category}";
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