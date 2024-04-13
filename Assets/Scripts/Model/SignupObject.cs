namespace Model
{
    public class SignupObject
    {
        private string username;
        private string password;
        private string email;
        private string dob;

        public string Username
        {
            get { return username; }
            set { username = value; } // You might add validation logic here
        }

        public string Password
        {
            get { return password; }
            set { password = value; } // Example: Hash password before setting
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Dob
        {
            get { return dob; }
            set { dob = value; } // You might add validation logic here
        }

        public SignupObject(string username, string password, string email, string dob)
        {
            Username = username;
            Password = password;
            Email = email;
            Dob = dob;
        }
    }
}