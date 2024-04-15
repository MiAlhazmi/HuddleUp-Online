namespace Model
{
    public class User
    {
        private string username;
        private string password;
        private string email;
        private string dob; // Assuming you store the date of birth as a string
        private string registeredDate; // You might want to convert dob to LocalDate
        private int xp;
        private bool authenticated;
        private int id; // Assuming user ID is an integer
    }
}