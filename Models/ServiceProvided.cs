﻿using System.ComponentModel.DataAnnotations;

namespace OfferVerse.Models
{
    public class ServiceProvided
    {
        private int serviceProvidedId;
        private string title;
        private string description;
        private bool priority;
        private DateTime datePriority;
        private int userId;
        private int categoryId;

        public int ServiceProvidedId 
        {  
            get { return serviceProvidedId; } 
        }

        [Display(Name = "Title")]
        [MinLength(4, ErrorMessage = "4 characters minimum")]
        [MaxLength(50, ErrorMessage = "50 characters maximum")]
        public string Title 
        { 
            get { return title; } 
            set { title = value; } 
        }

        [Display(Name = "Description")]
        [MinLength(4, ErrorMessage = "4 characters minimum")]
        [MaxLength(200, ErrorMessage = "200 characters maximum")]
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
        public DateTime DatePriority 
        { 
            get {  return datePriority; } 
            set {  datePriority = value; } 
        }

        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public int CategoryId
        {
            get { return  categoryId; }
            set { categoryId = value; }
        }
        public ServiceProvided()
        {

        }

        public ServiceProvided(int serviceProvidedId, string title, 
            string description, bool priority, DateTime datePriority, int userId)
        {
            this.serviceProvidedId = serviceProvidedId;
            Title = title;
            Description = description;
            Priority = priority;
            DatePriority = datePriority;
            DatePriority = new DateTime(2002, 1, 1);
            UserId = userId;
        }
        public ServiceProvided(string title, string description, int userId, int categoryId)
        {
            Title = title;
            Description = description;
            UserId = userId;
            Priority = false;
            DatePriority = new DateTime(2002, 2, 3);
            CategoryId = categoryId;
        }

        public ServiceProvided(int serviceProvidedId, string title, string description)
        {
            this.serviceProvidedId = serviceProvidedId;
            Title = title;
            Description = description;
            new DateTime(2002, 2, 3);
        }

        public ServiceProvided(int serviceProvidedId)
        {
            this.serviceProvidedId = serviceProvidedId;
        }

        public override string ToString()
        {
            return Title + " - " + Description + "-" + Priority + "-" + DatePriority;
        }
    }
}