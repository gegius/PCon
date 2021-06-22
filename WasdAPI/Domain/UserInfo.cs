namespace WasdAPI.Domain
{
    public class UserInfo
    {
        public UserInfo(string name,int id, string profileImageUrl, string userDescription, int followersCount, bool isLive)
        {
            Name = name;
            ProfileImageUrl = profileImageUrl;
            UserDescription = userDescription;
            FollowersCount = followersCount;
            IsLive = isLive;
            Id = id;
        }

        public string Name { get; }
        public int Id { get; }
        public string ProfileImageUrl { get; }
        public string UserDescription { get; }
        public int FollowersCount { get; }
        public bool IsLive { get; }
       
    }
}