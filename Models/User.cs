namespace tar1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; } = true;
        private static int _nextId = 1;
        private static List<User> UsersList = new List<User>();

        public bool Insert()
        {
            if (UsersList.Any(u => u.Email == this.Email))
                return false;
            this.Id = _nextId++;
            UsersList.Add(this);
            return true;
        }

        public List<User> Read()
        {
            return UsersList;
        }


        public static User Login(string email, string password)
        {
            return UsersList.FirstOrDefault(u => u.Email == email && u.Password == password);
        }
    }
}
