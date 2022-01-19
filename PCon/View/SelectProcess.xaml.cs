using System.Windows;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using PCon.Domain.HotKeys;
using PCon.Infrastructure;
namespace PCon.View
{
    public partial class SelectProcess : Window
    {
        public DesktopSettings DesktopSettings { get; set; }
        public Overlay Overlay { get; set; }
        private readonly ServiceCollection _serviceCollection;
        private string mainProcess;
        
        public SelectProcess(ServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            InitializeComponent();
        }
        
        private void Button_Click_Start(object sender, RoutedEventArgs e)
        {
            DesktopSettings.mainProcess = mainProcess;
            DesktopSettings.Start();
            Close();
        }

        private void ProcessLabel_OnClick(object sender, RoutedEventArgs e)
        {
            var label = (Label) sender;
            label.Background = FindResource("AwesomeGreenColor") as Brush;
            if (!PanelInsideProcessPrograms.Children.Contains(label)) return;
            ChangeColor(sender);
            mainProcess = label.Content.ToString();
        }

        private void Update_OnClick(object sender, RoutedEventArgs e)
        {
            mainProcess = null;
            PanelInsideProcessPrograms.Children.Clear();
            var processlist = Process.GetProcesses()
                .Where(p => (long)p.MainWindowHandle != 0)
                .ToArray();
            foreach (var process in processlist)
            {
                var name = process.MainWindowTitle;
                if (IsCorrectProcess(name))
                    PanelInsideProcessPrograms.Children.Add(CreateProcessLabel(name));
            }
        }

        private static bool IsCorrectProcess(string name)
        {
            return !string.IsNullOrEmpty(name) && name != "DesktopSettings";
        }

        private Label CreateProcessLabel(object content)
        {
            var label = new Label {Foreground = Brushes.Black, Content = content};
            label.MouseDown += ProcessLabel_OnClick;
            label.MouseEnter += ProcessLabel_MouseEnter;
            label.MouseLeave += ProcessLabel_MouseLeave;
            label.MouseUp += ProcessLabel_MouseUp;
            return label;
        }

        private void ChangeColor(object sender)
        {
            foreach (var child in PanelInsideProcessPrograms.Children)
            {
                var label = (Label) child;
                label.Background = label == (Label) sender
                    ? FindResource("AwesomeGreenColor") as Brush
                    : FindResource("Empty") as Brush;
            }
        }

        private void ProcessLabel_MouseEnter(object sender, RoutedEventArgs e)
        {
            if (((Label) sender).Background != FindResource("AwesomeGreenColor") as Brush)
                ((Label) sender).Background = FindResource("AwesomeAquamarineColor") as Brush;
        }

        private void ProcessLabel_MouseLeave(object sender, RoutedEventArgs e)
        {
            if (((Label) sender).Background != FindResource("AwesomeGreenColor") as Brush)
                ((Label) sender).Background = FindResource("Empty") as Brush;
        }

        private void ProcessLabel_MouseUp(object sender, RoutedEventArgs e)
        {
            if (((Label) sender).Background != FindResource("AwesomeGreenColor") as Brush)
                ((Label) sender).Background = FindResource("AwesomeAquamarineColor") as Brush;
        }
    }
}