namespace PCon.Domain.Session
{
    public class User
    {
        public User(string nickname, string password)
        {
            Nickname = nickname;
            Password = password;
        }

        private string Nickname { get; set; }
        private string Password { get; set; }
    }
}