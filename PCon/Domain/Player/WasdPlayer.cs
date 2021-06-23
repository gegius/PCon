using System.Windows;

namespace PCon.Domain.Player
{
    // ReSharper disable once IdentifierTypo
    public class WasdPlayer : IPlayerSettings
    {
        public System.Windows.Visibility SliderVisibility { get; } = System.Windows.Visibility.Hidden;
        public System.Windows.Visibility PlayButtonVisibility { get; } = System.Windows.Visibility.Hidden;
        public System.Windows.Visibility PauseButtonVisibility { get; } = System.Windows.Visibility.Hidden;
    }
}