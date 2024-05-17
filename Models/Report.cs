using OfferVerse.DAL.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OfferVerse.Models
{
    public class Report
    {
        /******Attributs******/
        private readonly int reportId;
        private string title;
        private string description;

        /******References******/
        private User reported;
        private User reporter;

        /******Properties******/
        public int ReportId
        { 
            get { return reportId; } 
        }

        [Display(Name = "Title of the report")]
        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessage = "50 characters maximum")]
        [MinLength(5, ErrorMessage = "5 characters minimum")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title field required")]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        [Display(Name = "Please write as much as possible details about why you're reporting the service provider. (min 100 chararacters)")]
        [DataType(DataType.MultilineText)]
        [MaxLength(800, ErrorMessage = "800 characters maximum")]
        [MinLength(100, ErrorMessage = "100 characters minimum")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description field required")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public User Reporter
        {
            get { return reporter; }
            set { reporter = value; }
        }

        public User Reported
        {
            get { return reported; }
            set { reported = value; }
        }

        public Report (int reportId, 
                       string title, 
                       string description, 
                       int reporterId, 
                       string reporterFN, 
                       string reporterLN, 
                       int reportedId, 
                       string reportedFN, 
                       string reportedLN)
            :this(reportId, title, description)
        {
            Reporter = new(reporterId, reporterFN, reporterLN);
            Reported = new(reportedId, reportedFN, reportedLN);
        }

        public Report (int reportId, string title, string description)
        {
            this.reportId = reportId;
            Title = title;
            Description = description;
        }

        public Report(int reportId, string title, string description, User reporter, User reported)
        {
            this.reportId = reportId;
            Title = title;
            Description = description;
            Reporter = reporter;
            Reported = reported;
        }

        public Report(int reportId)
        {
            this.reportId = reportId;
        }

        public Report()
        {

        }

        /******Static methods******/
        public static bool InsertReport(IReportDAL dal, Report report)
        {
            return dal.InsertReport(report);
        }
    }
}
