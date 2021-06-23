using System.Windows;

namespace PCon.View
{
    public static class ErrorHandler
    {
        public static void ThrowErrorConnection()
        {
            MessageBox.Show("Упс... Что-то пошло не так.\n\nПроверьте соединение с интернетом.", "PCon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        
        public static void ThrowErrorNotSelectedProcess()
        {
            MessageBox.Show("Выбери, пожалуйста, процесс...", "PCon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ThrowErrorNoVlcPlayer()
        {
            MessageBox.Show("Vlc player не найден в базовой комплектации.\n\nПопробуйте переустановить программу.", "PCon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}