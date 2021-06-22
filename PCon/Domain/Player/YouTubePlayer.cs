namespace PCon.Domain.Player
{
    public class YouTubePlayer : IPlayerSettings
    {
        public System.Windows.Visibility GetSliderVisibility()
        {
            return System.Windows.Visibility.Visible;
        }
    }
}