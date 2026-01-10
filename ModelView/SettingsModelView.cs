using Game_Launcher.Helpers;
using Game_Launcher.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Game_Launcher.ModelView
{
    public class SettingsModelView : INotifyPropertyChanged
    {
        private Settings _appSettings = SettingsHelper._defaultSettings;
        public bool[] DarkModeOptions => new bool[] { true, false };
        public int[] IconSizeOptions => new int[] { 1, 2, 3 };

        public SettingsModelView()
        {
            _appSettings = SettingsHelper.GetSettings();
        }

        public int IconSize
        {
            get { return _appSettings.IconSize; }
            set
            {
                _appSettings.IconSize = value;
                SettingsHelper.SetIconSize(_appSettings.IconSize);
                OnPropertyChanged();
            }
        }

        public bool DarkMode
        {
            get { return _appSettings.DarkMode; }
            set
            {
                if (_appSettings.DarkMode != value)
                {
                    _appSettings.DarkMode = value;
                    SettingsHelper.SetTheme(_appSettings.DarkMode);
                    ShowRestartRequest();
                    OnPropertyChanged();
                }
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
