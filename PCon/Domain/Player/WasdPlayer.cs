namespace PCon.Domain.Player
{
    // ReSharper disable once IdentifierTypo
    public class WasdPlayer : IPlayerSettings
    {
        public System.Windows.Visibility GetSliderVisibility()
        {
            return System.Windows.Visibility.Hidden;
        }
    }
}