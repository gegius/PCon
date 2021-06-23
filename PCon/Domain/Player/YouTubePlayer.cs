using System.Windows;

namespace PCon.Domain.Player
{
    public class YouTubePlayer : IPlayerSettings
    {
        public Visibility SliderVisibility { get; } = Visibility.Visible;
        public Visibility PlayButtonVisibility { get; } = Visibility.Visible;
        public Visibility PauseButtonVisibility { get; } = Visibility.Visible;
    }
}