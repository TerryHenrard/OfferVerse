using System.ComponentModel.DataAnnotations;

namespace OfferVerse.Models
{
    public abstract class Member
    {
        private readonly int memberId;
        private string email;
        private string password;
        private string firstName;
        private string lastName;

        public int MemberId
        {
            get { return memberId; }
        }

        [Display(Name = "Email")]
        [MaxLength(50, ErrorMessage = "50 characters maximum")]
        [MinLength(4, ErrorMessage = "4 characters minimum")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email field required")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [Display(Name = "New password")]
        [MinLength(8, ErrorMessage = "8 characters minimum")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^(?=.* [0 - 9])(?=.* [a - z])(?=.* [A - Z])(?=.*\W)(?!.* ).{8,}$", ErrorMessage = "Must contains at least 1 uppercase and 1 lowercase letter and a number")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [Display(Name = "First name")]
        [MaxLength(50, ErrorMessage = "50 characters maximum")]
        [MinLength(2, ErrorMessage = "2 characters minimum")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name ield required")]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        [Display(Name = "Last name")]
        [MaxLength(50, ErrorMessage = "50 characters maximum")]
        [MinLength(2, ErrorMessage = "2 characters minimum")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name field required")]
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } //only for the validation of the password


        public Member()
        {

        }

        public Member(int memberId, string email, string password, string firstName, string lastName)
        {
            this.memberId = memberId;
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
