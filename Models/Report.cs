using System.ComponentModel.DataAnnotations;

namespace OfferVerse.Models
{
    public class Report
    {
        private readonly int reportId;
        private string title;
        private string description;

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

        [Display(Name = "Please write as much as possible details about why you're reporting the service provider")]
        [DataType(DataType.MultilineText)]
        [MaxLength(800, ErrorMessage = "800 characters maximum")]
        [MinLength(100, ErrorMessage = "100 characters minimum")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description field required")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public Report (int reportId, string title, string description)
        {
            this.reportId = reportId;
            Title = title;
            Description = description;
        }

        public Report()
        {

        }
    }
}
