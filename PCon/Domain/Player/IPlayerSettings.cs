namespace PCon.Domain.Player
{
    public interface IPlayerSettings
    {
        System.Windows.Visibility SliderVisibility { get; }
        System.Windows.Visibility PlayButtonVisibility { get; }
        System.Windows.Visibility PauseButtonVisibility { get; }
    }
}