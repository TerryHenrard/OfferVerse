using OfferVerse.DAL.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace OfferVerse.Models
{
    public class ServiceDemanded
    {
        private readonly int serviceId;
        private DateTime? startService;
        private DateTime? endService;
        private int? nbHours;

        /******References******/
        private User serviceProvider;
        private User serviceDemander;
        private ServiceProvided serviceProvided;

        public int ServiceId
        {
            get { return serviceId; }
        }

        public DateTime? StartService
        {
            get { return startService; }
            set { startService = value; }
        }

        public DateTime? EndService
        {
            get { return endService; }
            set 
            {
                if (value >= StartService || value == null)
                {
                    endService = value;
                }
                else
                {
                    throw new Exception("the service end date cannot be less than the service start date");
                }
            }
        }

        [Display(Name = "Number of hours spent")]
        [Range(1, 200, ErrorMessage = "The number of hours spent cannot be less of 1 or greater than 200")]
        [Required(ErrorMessage = "A number of hours is required")]
        public int? NbHours
        {
            get { return nbHours; }
            set 
            {
                if ((value >= 1 && value <= 200) || value == null)
                {
                    nbHours = value;
                }
                else
                {
                    throw new Exception("The number of hours spent cannot be less of 1 or greater than 200");
                }
            }
        }

        public User ServiceProvider
        {
            get { return serviceProvider; }
            set {  serviceProvider = value; }
        }
        
        public User ServiceDemander
        {
            get { return serviceDemander; }
            set { serviceDemander = value; }
        }

        public ServiceProvided ServiceProvided
        {
            get { return serviceProvided; }
            set { serviceProvided = value; }
        }

        public ServiceDemanded(int serviceId, DateTime? startService, DateTime? endService, int? nbHours)
        {
            this.serviceId = serviceId;
            StartService = startService;
            EndService = endService;
            NbHours = nbHours;
        }

        public ServiceDemanded(int serviceId, DateTime? startService, int PId, string PFirstName, string PLastName, int SPId, string title, string descrption)
        {
            this.serviceId= serviceId;
            StartService = startService;
            ServiceProvider = new User(PId, PFirstName, PLastName);
            ServiceProvided = new ServiceProvided(SPId, title, descrption);
        }

        public ServiceDemanded(int serviceId, 
                               DateTime? startService, 
                               DateTime? endService, 
                               int? nbHours, 
                               int PId, //Provider
                               string PFirstName, 
                               string PLastName,
                               int DId, //Demander
                               string DFirstName, 
                               string DLastName, 
                               int SPId, //Service provided
                               string title, 
                               string description)
        {
            this.serviceId = serviceId;
            StartService = startService;
            EndService = endService;
            NbHours = nbHours;
            ServiceProvider = new User(PId, PFirstName, PLastName);
            ServiceDemander = new User(DId, DFirstName, DLastName);
            ServiceProvided = new ServiceProvided(SPId, title, description);
        }

        
        public ServiceDemanded()
        {

        }

        /******static methods******/
        public static bool FinalizeService(IServiceDemandedDAL dal, int serviceDId, int? nbHours)
        {
            return dal.FinalizeService(serviceDId, nbHours);
        }

        public static List<ServiceDemanded> GetServicesDemanded(IServiceDemandedDAL dal, int uId)
        {
            return dal.GetServicesDemanded(uId);
        }
    }
}
