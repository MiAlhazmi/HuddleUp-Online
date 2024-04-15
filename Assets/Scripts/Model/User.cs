namespace Model
{
    public class User
    {
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string dob { get; set; } // Assuming you store the date of birth as a string
        public string registeredDate { get; set; } // You might want to convert dob to LocalDate
        public int xp { get; set; }
        public bool authenticated { get; set; }
        public int id { get; set; } // Assuming user ID is an integer
    }
}