namespace TwitchAPI.DTO
{
    public class UserInfo
    {
        public string Name { get; }
        public string ProfileImageUrl { get; }
        public string UserDescription { get; }
        public int FollowersCount { get; }
        public StreamInfo StreamInfo { get; }

        public UserInfo(string name, string profileImageUrl, string userDescription, int followersCount,
            StreamInfo streamInfo)
        {
            Name = name;
            ProfileImageUrl = profileImageUrl;
            UserDescription = userDescription;
            StreamInfo = streamInfo;
            FollowersCount = followersCount;
        }

        public UserInfo((string name, string profileImageUrl, string userDescription, int followersCount) args,
            StreamInfo streamInfo)
        {
            Name = args.name;
            ProfileImageUrl = args.profileImageUrl;
            UserDescription = args.userDescription;
            StreamInfo = streamInfo;
            FollowersCount = args.followersCount;
        }
    }
}