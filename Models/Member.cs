namespace OfferVerse.Models
{
    public abstract class Member
    {
        private int memberId;
        private string email;
        private string password;
        private string firstName;
        private string lastName;

        public int MemberId
        {
            get { return memberId; }
            set { memberId = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public Member()
        {

        }

        public Member(int memberId, string email, string password, string firstName, string lastName)
        {
            MemberId = memberId;
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
