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

        static WpfHotKeys()
        {
            StartSearchCommand.InputGestures.Add(new KeyGesture(Key.Enter));
            PausePlayerCommand.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control));
            PlayPlayerCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            HideOverlayCommand.InputGestures.Add(new KeyGesture(Key.H, ModifierKeys.Control));
            ShowOverlayCommand.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
            FullScreenModeCommand.InputGestures.Add(new KeyGesture(Key.F11, ModifierKeys.Control));
            ScrollDownCommand.InputGestures.Add(new KeyGesture(Key.Down));
        }
    }
}