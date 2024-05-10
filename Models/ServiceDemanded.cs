using System.Globalization;

namespace OfferVerse.Models
{
    public class ServiceDemanded
    {
        private int serviceId;
        private DateTime startService;
        private DateTime endService;
        private int nbHours;

        /******References******/
        private User serviceProvider;
        private User serviceDemander;
        private ServiceProvided serviceProvided;

        public int ServiceId
        {
            get { return serviceId; }
        }

        public DateTime StartService
        {
            get { return startService; }
            set { startService = value; }
        }

        public DateTime EndService
        {
            get { return endService; }
            set 
            {
                if (value >= StartService)
                {
                    endService = value;
                }
                else
                {
                    throw new Exception("the service end date cannot be less than the service start date");
                }
            }
        }
        public int NbHours
        {
            get { return nbHours; }
            set { nbHours = value; }
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

        public ServiceDemanded(int serviceId, DateTime startService, DateTime endService, int nbHours)
        {
            this.serviceId = serviceId;
            StartService = startService;
            EndService = endService;
            NbHours = nbHours;
        }

        public ServiceDemanded(int serviceId, 
                               DateTime startService, 
                               DateTime endService, 
                               int nbHours, 
                               int PId, 
                               string PFirstName, 
                               string PLastName,
                               int DId, 
                               string DFirstName, 
                               string DLastName, 
                               int SPId, 
                               string title, 
                               string descrption)
        {
            this.serviceId = serviceId;
            StartService = startService;
            EndService = endService;
            NbHours = nbHours;
            ServiceProvider = new User(PId, PFirstName, PLastName);
            ServiceDemander = new User(DId, DFirstName, DLastName);
            ServiceProvided = new ServiceProvided(SPId, title, descrption);
        }

        public ServiceDemanded()
        {

        }
    }
}
