using System.Windows;

namespace PCon.Domain.Player
{
    public class TwitchPlayer : IPlayerSettings
    {
        public Visibility SliderVisibility { get; } = Visibility.Hidden;
        public Visibility PlayButtonVisibility { get; } = Visibility.Hidden;
        public Visibility PauseButtonVisibility { get; } = Visibility.Hidden;
    }
}