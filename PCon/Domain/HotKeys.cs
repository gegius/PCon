using System.Windows.Input;

namespace PCon.Domain
{
    public static class HotKeys
    {
        public static readonly RoutedCommand StartSearchCommand = new RoutedCommand();
        public static readonly RoutedCommand PausePlayerCommand = new RoutedCommand();
        public static readonly RoutedCommand PlayPlayerCommand = new RoutedCommand();
        public static readonly RoutedCommand HideOverlayCommand = new RoutedCommand();
        public static readonly RoutedCommand ShowOverlayCommand = new RoutedCommand();
        public static readonly RoutedCommand ScrollDownCommand = new RoutedCommand();

        static HotKeys()
        {
            StartSearchCommand.InputGestures.Add(new KeyGesture(Key.Enter));
            PausePlayerCommand.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control));
            PlayPlayerCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            HideOverlayCommand.InputGestures.Add(new KeyGesture(Key.H, ModifierKeys.Control));
            ShowOverlayCommand.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
            ScrollDownCommand.InputGestures.Add(new KeyGesture(Key.Down));
        }
    }
}