using Game_Launcher.Helpers;
using Game_Launcher.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Game_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isSidebarOpen = true;
        private Button currentActiveButton;

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

            currentActiveButton = AppsButton;
            currentActiveButton.Style = (Style)FindResource("SidebarButtonActive");
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
                Button button = new Button()
                {
                    Style = (Style)FindResource("Win11IconButton")
                };

                StackPanel stackPanel = new StackPanel()
                {
                    Style = (Style)FindResource("IconContent")
                };

                System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                {
                    Style = (Style)FindResource("IconImage"),
                    Source = IconExtractor.ConvertBitmapToBitmapSource(appItem.Icon.ToBitmap())
                };

                TextBlock textBlock = new TextBlock()
                {
                    Style = (Style)FindResource("IconText"),
                    Text = appItem.Name
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReadDirs();
            DisplayAppItems(AppsPanel, _apps);
            DisplayAppItems(OfflineGamesPanel, _offlineGames);
            DisplayAppItems(OnlineGamesPanel, _onlineGames);
            DisplayAppItems(OthersPanel, _otherApps);
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
            if (currentActiveButton != null)
            {
                currentActiveButton.Style = (Style)FindResource("SidebarButton");
            }
            currentActiveButton = newActiveButton;
            currentActiveButton.Style = (Style)FindResource("SidebarButtonActive");
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

            if (isSidebarOpen)
            {
                widthAnimation.From = new GridLength(210);
                widthAnimation.To = new GridLength(0);
                opacityAnimation.To = 0;
                isSidebarOpen = false;
            }
            else
            {
                widthAnimation.From = new GridLength(0);
                widthAnimation.To = new GridLength(210);
                opacityAnimation.To = 1;
                isSidebarOpen = true;
            }

            SidebarColumn.BeginAnimation(ColumnDefinition.WidthProperty, widthAnimation);
            Sidebar.BeginAnimation(OpacityProperty, opacityAnimation);
        }
    }
}
