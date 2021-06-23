using System.Windows;

namespace PCon.Domain.Player
{
    public class YouTubePlayer : IPlayerSettings
    {
        public System.Windows.Visibility SliderVisibility { get; } = System.Windows.Visibility.Visible;
        public System.Windows.Visibility PlayButtonVisibility { get; } = System.Windows.Visibility.Visible;
        public System.Windows.Visibility PauseButtonVisibility { get; } = System.Windows.Visibility.Visible;
    }
}