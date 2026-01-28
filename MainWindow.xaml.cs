using Game_Launcher.Helpers;
using Game_Launcher.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Game_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isSidebarOpen = true;
        private Button _currentActiveButton;
        private Settings _appSettings = SettingsHelper._defaultSettings;

        public MainWindow()
        {
            InitializeComponent();
        }

        private List<AppItem> _apps = new List<AppItem>();
        private List<AppItem> _offlineGames = new List<AppItem>();
        private List<AppItem> _onlineGames = new List<AppItem>();
        private List<AppItem> _otherApps = new List<AppItem>();

        private string appsDir = "./Apps";
        private string offlineGamesDir = "./OfflineGames";
        private string onlineGamesDir = "./OnlineGames";
        private string otherAppsDir = "./Others";

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            _appSettings = SettingsHelper.GetSettings();
            ApplyTheme(_appSettings.DarkMode);

            _currentActiveButton = AppsButton;
            _currentActiveButton.Style = (Style)FindResource("SidebarButtonActive");

            DisplayAppItems(AppsPanel, _apps);
            DisplayAppItems(OfflineGamesPanel, _offlineGames);
            DisplayAppItems(OnlineGamesPanel, _onlineGames);
            DisplayAppItems(OthersPanel, _otherApps);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReadDirs();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void ReadDirs()
        {
            _apps = DirectoryHelper.DirFilesToAppItems(appsDir);
            _offlineGames = DirectoryHelper.DirFilesToAppItems(offlineGamesDir);
            _onlineGames = DirectoryHelper.DirFilesToAppItems(onlineGamesDir);
            _otherApps = DirectoryHelper.DirFilesToAppItems(otherAppsDir);
        }

        private void DisplayAppItems(WrapPanel wrapPanel, List<AppItem> appItems)
        {
            foreach (AppItem appItem in appItems)
            {
                Button button = new Button();

                if (appItem.Icon == null) continue;

                Image image = new Image() { Source = IconExtractor.ConvertBitmapToBitmapSource(appItem.Icon.ToBitmap()) };

                TextBlock textBlock = new TextBlock() { Text = appItem.Name };

                switch (_appSettings.IconSize)
                {
                    case 1:
                        button.Style = (Style)FindResource("Win11IconButtonSmall");
                        image.Style = (Style)FindResource("IconImageSmall");
                        textBlock.Style = (Style)FindResource("IconTextSmall");
                        break;
                    case 2:
                        button.Style = (Style)FindResource("Win11IconButtonMedium");
                        image.Style = (Style)FindResource("IconImageMedium");
                        textBlock.Style = (Style)FindResource("IconTextMedium");
                        break;
                    case 3:
                        button.Style = (Style)FindResource("Win11IconButtonLarge");
                        image.Style = (Style)FindResource("IconImageLarge");
                        textBlock.Style = (Style)FindResource("IconTextLarge");
                        break;
                }

                StackPanel stackPanel = new StackPanel()
                {
                    Style = (Style)FindResource("IconContent")
                };

                stackPanel.Children.Add(image);
                stackPanel.Children.Add(textBlock);

                button.Content = stackPanel;
                button.GotFocus += (object sender, RoutedEventArgs e) =>
                {
                    textBlock.TextWrapping = TextWrapping.Wrap;
                    textBlock.TextTrimming = TextTrimming.None;
                };
                button.LostFocus += (object sender, RoutedEventArgs e) =>
                {
                    textBlock.TextWrapping = TextWrapping.NoWrap;
                    textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
                };
                button.MouseDoubleClick += (object sender, MouseButtonEventArgs e) =>
                {
                    ShortcutHelper.RunShortcutFromPath(appItem.ShortcutPath);
                };

                wrapPanel.Children.Add(button);
            }
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null) return;

            string category = clickedButton.Tag.ToString();
            UpdateActiveButton(clickedButton);
            ShowPanel(category);
            CategoryTitle.Text = clickedButton.Content.ToString();
        }

        private void UpdateActiveButton(Button newActiveButton)
        {
            if (_currentActiveButton != null)
                _currentActiveButton.Style = (Style)FindResource("SidebarButton");
            _currentActiveButton = newActiveButton;
            _currentActiveButton.Style = (Style)FindResource("SidebarButtonActive");
        }

        private void ShowPanel(string panelName)
        {
            AppsPanel.Visibility = Visibility.Collapsed;
            OfflineGamesPanel.Visibility = Visibility.Collapsed;
            OnlineGamesPanel.Visibility = Visibility.Collapsed;
            OthersPanel.Visibility = Visibility.Collapsed;

            switch (panelName)
            {
                case "Apps":
                    AppsPanel.Visibility = Visibility.Visible;
                    break;
                case "OfflineGames":
                    OfflineGamesPanel.Visibility = Visibility.Visible;
                    break;
                case "OnlineGames":
                    OnlineGamesPanel.Visibility = Visibility.Visible;
                    break;
                case "Others":
                    OthersPanel.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            var widthAnimation = new GridLengthAnimation
            {
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            var opacityAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            if (_isSidebarOpen)
            {
                widthAnimation.From = new GridLength(210);
                widthAnimation.To = new GridLength(0);
                opacityAnimation.To = 0;
                _isSidebarOpen = false;
            }
            else
            {
                widthAnimation.From = new GridLength(0);
                widthAnimation.To = new GridLength(210);
                opacityAnimation.To = 1;
                _isSidebarOpen = true;
            }

            SidebarColumn.BeginAnimation(ColumnDefinition.WidthProperty, widthAnimation);
            Sidebar.BeginAnimation(OpacityProperty, opacityAnimation);
        }

        private void ApplyTheme(bool darkMode)
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;

            if (_appSettings.DarkMode)
            {
                mergedDictionaries.Clear();
                mergedDictionaries.Add(
                    new ResourceDictionary { Source = new Uri("Themes/Dark.xaml", UriKind.Relative) }
                );
            }
            else
            {
                mergedDictionaries.Clear();
                mergedDictionaries.Add(
                    new ResourceDictionary { Source = new Uri("Themes/Light.xaml", UriKind.Relative) }
                );
            }
        }
    }
}
