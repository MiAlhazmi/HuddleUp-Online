namespace Model
{
    public class UserInfo
    {
        public long ID { get; set; }
        public string Username { get; set; }
        public int Xp { get; set; }

        public UserInfo(long id, string username, int xp)
        {
            ID = id;
            Username = username;
            Xp = xp;
        }
    }
}
