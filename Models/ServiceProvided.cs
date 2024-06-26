﻿using OfferVerse.DAL.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

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
        private User own;
        private Category category;
        private List<Commentary> commentaries;
        private List<ServiceDemanded>? servicesDemanded;

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

        public Category Category 
        { 
            get { return category; }
            set { category = value; }
        }
        
        public List<Commentary> Commentaries
        {
            get { return commentaries; }
            set {  commentaries = value; }
        }

        public List<ServiceDemanded> ServicesDemanded
        {
            get { return servicesDemanded; }
            set { servicesDemanded = value; }
        }

        //constructors
        public ServiceProvided(int servicePId, string title, string description, bool priority, DateTime? datePriority)
        {
            this.serviceProvidedId = servicePId;
            Title = title;
            Description = description;
            Priority = priority;
            DatePriority = datePriority;
        }
        public ServiceProvided(int servicePId, string title, string description, bool priority, DateTime? datePriority, int userId, string userFN, string userLN)
            :this(servicePId, title, description, priority, datePriority)
        {
            Own = new(userId, userFN, userLN);
        }
        public ServiceProvided(int servicePId, string title, string description, bool priority, DateTime? datePriority, int userId, int categoryId, string categoryName, string imagePath)
            :this(servicePId, title, description, priority, datePriority)
        {
            Own = new(userId);
            Category = new(categoryId, categoryName, imagePath);
            Commentaries = new List<Commentary>();
        }

        public ServiceProvided(
            int servicePId, 
            string title, 
            string description, 
            bool priority, 
            DateTime? datePriority, 
            int userId, 
            string phoneNumber,
            string postCode,
            string streetName,
            string streetNumber,
            string city,
            string email,
            string firstName,
            string lastName, 
            int categoryId, 
            string categoryName, 
            string imagePath)
            : this(servicePId, title, description, priority, datePriority)
        {
            Own = new(userId, email, "", firstName, lastName, phoneNumber, postCode, streetNumber, streetName, city, 0);
            Category = new(categoryId, categoryName, imagePath);
            Commentaries = new List<Commentary>();
        }

        /********************************************************************************/

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

        public ServiceProvided()
        {

        }

        /******Methods******/
        public double GlobalRating()
        {
            return Commentaries.Count > 0
                ? Math.Round(Commentaries.Average(com => com.Rating), 1)
                : -1;
        }


        public bool AddCommentary(Commentary commentary)
        {
            bool success = false;
            if (commentary != null && !Commentaries.Contains(commentary))
            {
                Commentaries.Add(commentary);
                success = true;
            }
            return success;
        }

        public static ServiceProvided GetServiceProvidedInfo(IServiceProvidedDAL dal, int sId)
        {
            return dal.GetServiceProvidedInfo(sId);
        }

        public bool ApplyServiceProvidedChanges(IServiceProvidedDAL dal, ServiceProvided sp, int id)
        {
            return dal.ApplyServiceProvidedChanges(sp, id);
        }

        public static List<ServiceProvided> GetServicesProvided(IServiceProvidedDAL dal, int pageNb, int servicesPerPage)
        {
            return dal.GetServicesProvided(pageNb, servicesPerPage);
        }

        public static int GetNumberOfPages(IServiceProvidedDAL dal, int servicesPerPage)
        {
            return dal.GetNumberOfPages(servicesPerPage);
        }

        public static ServiceProvided GetServiceProvided(IServiceProvidedDAL dal, int servicePId)
        {
            return dal.GetServiceProvided(servicePId);
        }

        public static bool PutInFavorite(IServiceProvidedDAL dal, int servicePId, int userId)
        {
            return dal.PutInFavorite(servicePId, userId);
        }

        public static bool DeleteInFavorite(IServiceProvidedDAL dal, int servicePId, int userId)
        {
            return dal.DeleteInFavorite(servicePId, userId);
        }

        //Overrided methods
        public override string ToString()
        {
            return $" ,{ServiceProvidedId}, {Title}, {Description}, {Priority}, {DatePriority}";
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