using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using PCon.Domain;
using PCon.Services;
using PCon.Services.ProcessServices;

namespace PCon.View
{
    public partial class DesktopSettings : Window
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
  
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
  
        private const int HOTKEY_ID = 9000;
        private const uint MOD_SHIFT = 0x0004;
        private const uint VK_TAB = 0x09;
        private const uint VK_CAPITAL= 0x14;
        private const uint VK_CONTROL= 0x11;
        private const uint VK_ESCAPE= 0x18;
        private const uint MOD_CONTROL = 0x0002;


        private ServiceCollection _serviceCollection;
        private IntPtr _windowHandle;
        private HwndSource _source;
        
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
  
            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);
  
            RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL, VK_TAB);
        }
        
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            var vkey = ((int)lParam >> 16) & 0xFFFF;
                            Console.WriteLine(vkey);
                            if (vkey == VK_TAB && overlaySettingsStarted)
                            {
                                if (overlaySettings.Visibility == Visibility.Visible &&
                                    (processChecker.IsWindowShowed("OverlaySettings") || processChecker.IsWindowShowed(mainProcess)))
                                {
                                    StopOverlaySettingsAttach();
                                    Overlay?.StartOverlayAttach();
                                    Console.WriteLine("опусти");
                                }
                                else if (overlaySettings.Visibility == Visibility.Hidden &&
                                         (processChecker.IsWindowShowed(mainProcess) || processChecker.IsWindowShowed("Overlay")))
                                {
                                    Overlay?.StopOverlayAttach();
                                    StartOverlaySettingsAttach();
                                    Console.WriteLine("запусти");
                                }
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        public void StopOverlaySettingsAttach()
        {
            cancellationTokenSource.Cancel();
            overlaySettings.Visibility = Visibility.Hidden;
        }

        private void StartOverlaySettingsAttach()
        {
            cancellationTokenSource = new CancellationTokenSource();
            overlaySettings.Visibility = Visibility.Visible;
            Console.WriteLine("ВОТИЯ");
            var size = snapper.GetMainProcessWindowSize();
            overlaySettings.Width = size.Width;
            overlaySettings.Height = size.Height;
            CheckMainProcess();
        }
  
        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
            base.OnClosed(e);
        }
        
        private OverlaySettings overlaySettings;
        private IProcessChecker processChecker;
        private string mainProcess;
        private WindowSnapper snapper;
        private bool overlaySettingsStarted;
        public Overlay Overlay { get; set; }
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public DesktopSettings(ServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            InitializeComponent();
        }
        

        private void Button_Click_Show(object sender, RoutedEventArgs e)
        {
            processChecker = new ProcessChecker(mainProcess);
            overlaySettings = new OverlaySettings(mainProcess, _serviceCollection) {DesktopSettings = this};
            InitSnapper();
            overlaySettingsStarted = true;
            
            overlaySettings.Visibility = Visibility.Hidden;
            Hide();
            
        }
        
        private async void CheckMainProcess()
        {
            Console.WriteLine("зашел оверлейнастройки");
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                await processChecker.WaitHideAsync("OverlaySettings", cancellationTokenSource.Token);
                Console.WriteLine("прошел чекпоинт1 оверлейнастройки");
                if (cancellationTokenSource.Token.IsCancellationRequested) break;
                overlaySettings.Visibility = Visibility.Hidden;
                await processChecker.WaitShowAsync(cancellationTokenSource.Token);
                if (cancellationTokenSource.Token.IsCancellationRequested) break;
                Console.WriteLine("прошел чекпоинт2 оверлейнастройки");
                overlaySettings.Visibility = Visibility.Visible;
            }
            Console.WriteLine("вышел оверлейнастройки");
        }
        
        private void InitSnapper()
        {
            snapper = new WindowSnapper(overlaySettings, mainProcess);
            snapper.Attach();
        }

        private void Label_OnClick(object sender, RoutedEventArgs e)
        {
            var label = (Label) sender;
            label.Background = FindResource("AwesomeGreenColor") as Brush;
            if (PanelInsideProcessPrograms.Children.Contains(label))
            {
                PanelInsideBoxPrograms.Children.Clear();
                PanelInsideBoxPrograms.Children.Add(CreateLabel(label.Content));
                mainProcess = label.Content.ToString();
            }

            if (PanelInsideBoxPrograms.Children.Contains(label))
            {
                PanelInsideBoxPrograms.Children.Remove(label);
            }
        }
        
        private void Label_MouseEnter(object sender, RoutedEventArgs e)
        {
            ((Label)sender).Background = FindResource("AwesomeAquamarineColor") as Brush;
        }
        
        private void Label_MouseLeave(object sender, RoutedEventArgs e)
        {
            ((Label)sender).Background = FindResource("Empty") as Brush;
        }
        
        private void Label_MouseUp(object sender, RoutedEventArgs e)
        {
            ((Label)sender).Background = FindResource("AwesomeAquamarineColor") as Brush;
        }

        private void Update_OnClick(object sender, RoutedEventArgs e)
        {
            PanelInsideProcessPrograms.Children.Clear();
            var processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                var name = process.MainWindowTitle;
                if (!String.IsNullOrEmpty(name) && name != "DesktopSettings")
                {
                    var label = CreateLabel(name);
                    PanelInsideProcessPrograms.Children.Add(label);
                }
            }
        }

        public static T DeepCopy<T>(T element)
        {
            var xaml = XamlWriter.Save(element);
            var xamlString = new StringReader(xaml);
            var xmlTextReader = new XmlTextReader(xamlString);
            var deepCopyObject = (T)XamlReader.Load(xmlTextReader);
            return deepCopyObject;
        }

        private Label CreateLabel(object content)
        {
            var label = new Label { Foreground = Brushes.Red, Content = content};
            label.MouseDown += Label_OnClick;
            label.MouseEnter += Label_MouseEnter;
            label.MouseLeave += Label_MouseLeave;
            label.MouseUp += Label_MouseUp;
            return label;
        }
    }
}