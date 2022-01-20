using System.Windows.Input;

namespace PCon.Domain.HotKeys
{
    public static class WpfHotKeys
    {
        public static readonly RoutedCommand StartSearchCommand = new RoutedCommand();
        public static readonly RoutedCommand PausePlayerCommand = new RoutedCommand();
        public static readonly RoutedCommand PlayPlayerCommand = new RoutedCommand();
        public static readonly RoutedCommand HideOverlayCommand = new RoutedCommand();
        public static readonly RoutedCommand ShowOverlayCommand = new RoutedCommand();
        public static readonly RoutedCommand ScrollDownCommand = new RoutedCommand();
        public static readonly RoutedCommand FullScreenModeCommand = new RoutedCommand();
        public static readonly RoutedCommand IncreaseVideoSpeedCommand = new RoutedCommand();
        public static readonly RoutedCommand SlowVideoSpeedCommand = new RoutedCommand();

        static WpfHotKeys()
        {
            StartSearchCommand.InputGestures.Add(new KeyGesture(Key.Enter));
            PausePlayerCommand.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Alt));
            PlayPlayerCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Alt));
            HideOverlayCommand.InputGestures.Add(new KeyGesture(Key.H, ModifierKeys.Alt));
            ShowOverlayCommand.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Alt));
            FullScreenModeCommand.InputGestures.Add(new KeyGesture(Key.F11, ModifierKeys.Alt));
            ScrollDownCommand.InputGestures.Add(new KeyGesture(Key.Down));
            IncreaseVideoSpeedCommand.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Alt));
            SlowVideoSpeedCommand.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Alt));
        }
    }
}