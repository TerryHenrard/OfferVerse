using Microsoft.Extensions.Primitives;
using OfferVerse.DAL.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OfferVerse.Models
{
    public class Commentary
    {
        private readonly int commentaryId;
        private string content;
        private int rating;
        private DateTime commentaryDate;

        /*References*/
        private User user;
        private ServiceProvided serviceProvided;

        public int CommentaryId 
        { 
            get {  return commentaryId; } 
        }

        [Display(Name = "Leave a positive and constructive commentary")]
        [DataType(DataType.MultilineText)]
        [MaxLength(300, ErrorMessage = "300 characters maximum")]
        [MinLength(15, ErrorMessage = "15 characters minimum")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Commentary field required")]
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        [Display(Name = "Rating out of 5")]
        [Range(0, 5, ErrorMessage = "The rating cannot be less of 0 or greater than 5")]
        [Required(ErrorMessage = "A rating is required")]
        public int Rating
        {
            get { return rating; }
            set 
            {
                if (value >= 0 && value <= 5)
                {
                    rating = value;
                }
                else
                {
                    throw new Exception("The value of the rating can't be less than 0 or greater than 5");
                }
            }
        }

        public DateTime CommentaryDate
        {
            get { return commentaryDate; }
            set { commentaryDate = value; }
        }

        public User User
        {
            get { return user; }
            set { user = value; }
        }

        public ServiceProvided ServiceProvided
        {
            get { return serviceProvided; }
            set { serviceProvided = value; }
        }

        public Commentary(int commentaryId, string content, int rating, DateTime commentaryDate)
        {
            this.commentaryId = commentaryId;
            Content = content;
            Rating = rating;
            CommentaryDate = commentaryDate;
        }

        public Commentary(int commentaryId, string content, int rating, DateTime commentaryDate, int userId, string firstName, string lastName)
            : this(commentaryId, content, rating, commentaryDate)
        {
            User = new(userId, firstName, lastName);
        }

        public Commentary(int commentaryId, string content, int rating, DateTime commentaryDate, int userId, int servicePId)
            :this(commentaryId, content, rating, commentaryDate)
        {
            User = new(userId);
            ServiceProvided = new(servicePId);
        }

        public Commentary()
        {

        }

        /******static methods******/
        public static bool InsertCommentary(ICommentaryDAL dal, string content, int rating, int servicePId)
        {
            return dal.InsertCommentary(content, rating, servicePId);
        }
    }
}
