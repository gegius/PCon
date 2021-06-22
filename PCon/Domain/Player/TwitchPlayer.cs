namespace PCon.Domain.Player
{
    public class TwitchPlayer : IPlayerSettings
    {
        public System.Windows.Visibility GetSliderVisibility()
        {
            return System.Windows.Visibility.Hidden;
        }
    }
}