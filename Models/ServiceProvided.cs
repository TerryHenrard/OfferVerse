﻿namespace OfferVerse.Models
{
    public class ServiceProvided
    {
        private int serviceProvidedId;
        private string title;
        private string description;
        private bool priority;
        private DateTime datePriority;

        public int ServiceProvidedId 
        {  
            get { return serviceProvidedId; } 
            set { serviceProvidedId = value; }
        }
        public string Title 
        { 
            get { return title; } 
            set { title = value; } 
        }
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

        public ServiceProvided(int serviceProvidedId, string title, string description, bool priority, DateTime datePriority)
        {
            ServiceProvidedId = serviceProvidedId;
            Title = title;
            Description = description;
            Priority = priority;
            DatePriority = datePriority;
        }

        public override string ToString()
        {
            return Title + " - " + Description + "-" + Priority + "-" + DatePriority;
        }
    }
}