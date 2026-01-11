using Game_Launcher.Helpers;
using Game_Launcher.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Game_Launcher.ModelView
{
    public class MainModelView : INotifyPropertyChanged
    {
        private Settings _appSettings = new Settings();

        public MainModelView()
        {
            _appSettings = SettingsHelper.GetSettings();
        }

        public string BackgroundImage
        {
            get { return _appSettings.Background; }
            set
            {
                _appSettings.Background = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
