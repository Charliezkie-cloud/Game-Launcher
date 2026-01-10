using Game_Launcher.Helpers;
using Game_Launcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Game_Launcher
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Settings _appSettings = new Settings()
        {
            DarkMode = false,
            IconSize = 3
        };

        public SettingsWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            _appSettings = SettingsHelper.GetSettings();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
