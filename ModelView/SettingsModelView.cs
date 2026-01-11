using Game_Launcher.Helpers;
using Game_Launcher.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Game_Launcher.ModelView
{
    public class SettingsModelView : INotifyPropertyChanged
    {
        private Settings _appSettings = SettingsHelper._defaultSettings;
        private string _fullYear;
        public bool[] DarkModeOptions => new bool[] { true, false };
        public int[] IconSizeOptions => new int[] { 1, 2, 3 };
        public string[] BackgroundOptions => new string[]
        {
            "background-1",
            "background-2",
            "background-3",
            "background-4",
            "background-5",
            "background-6",
            "background-7",
            "background-dark-1",
            "background-dark-2",
            "background-dark-3",
            "background-dark-4",
            "background-dark-5",
            "background-dark-6",
            "background-dark-7",
        };

        public SettingsModelView()
        {
            _appSettings = SettingsHelper.GetSettings();
            FullYear = DateTime.Now.Year.ToString();
        }

        public string FullYear
        {
            get { return _fullYear; }
            set
            {
                _fullYear = value;
                OnPropertyChanged();
            }
        }

        public string Background
        {
            get { return Path.GetFileNameWithoutExtension(_appSettings.Background); }
            set
            {
                _appSettings.Background = value;
                SettingsHelper.SetBackground(value);
                ShowRestartRequest();
                OnPropertyChanged();
            }
        }

        public int IconSize
        {
            get { return _appSettings.IconSize; }
            set
            {
                _appSettings.IconSize = value;
                SettingsHelper.SetIconSize(_appSettings.IconSize);
                ShowRestartRequest();
                OnPropertyChanged();
            }
        }

        public bool DarkMode
        {
            get { return _appSettings.DarkMode; }
            set
            {
                _appSettings.DarkMode = value;
                SettingsHelper.SetTheme(_appSettings.DarkMode);
                ShowRestartRequest();
                OnPropertyChanged();
            }
        }

        private void ShowRestartRequest()
        {
            if (MessageBox.Show("A restart is required to apply changes.", "Restart",
                MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
                Process.Start(currentExecutablePath);
                Application.Current.Shutdown();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
