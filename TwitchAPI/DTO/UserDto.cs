namespace TwitchAPI.DTO
{
    public class UserDto
    {
        public string Name { get; }
        public string ProfileImageUrl { get; }
        public string UserDescription { get; }
        public int FollowersCount { get; }
        public StreamDto StreamInfo { get; }
        
        public UserDto(string name, string profileImageUrl, string userDescription, int followersCount,
            StreamDto streamDto)
        {
            Name = name;
            ProfileImageUrl = profileImageUrl;
            UserDescription = userDescription;
            StreamInfo = streamDto;
            FollowersCount = followersCount;
        }
        
        public UserDto((string name, string profileImageUrl, string userDescription, int followersCount) args, StreamDto streamDto)
        {
            Name = args.name;
            ProfileImageUrl = args.profileImageUrl;
            UserDescription = args.userDescription;
            StreamInfo = streamDto;
            FollowersCount = args.followersCount;
        }
    }
}