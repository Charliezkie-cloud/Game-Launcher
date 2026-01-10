using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Game_Launcher.Helpers
{
    public static class ShortcutHelper
    {
        public static void RunShortcutFromPath(string shortcutPath)
        {
            if (File.Exists(shortcutPath))
            {
                try
                {
                    Process process = new Process();
                    process.StartInfo.FileName = shortcutPath;
                    process.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show($"An error occurred: {shortcutPath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
