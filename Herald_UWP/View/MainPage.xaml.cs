using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Herald_UWP.View
{
    public sealed partial class MainPage
    {
        private int _preIndex;

        public MainPage()
        {
            InitializeComponent();
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove("UUID");

            Frame?.Navigate(typeof(UserLogin));
        }

        private void NaviToGpa(object sender, RoutedEventArgs e)
        {
            Frame?.Navigate(typeof(GpaPage));
        }

        private void NaviToCard(object sender, RoutedEventArgs e)
        {
            Frame?.Navigate(typeof(CardPage));
        }

        private void NaviToCurriculum(object sender, RoutedEventArgs e)
        {
            Frame?.Navigate(typeof(CurriculumPage));
        }

        private void NaviToPe(object sender, RoutedEventArgs e)
        {
            Frame?.Navigate(typeof(PePage));
        }

        private void NaviToNic(object sender, RoutedEventArgs e)
        {
            Frame?.Navigate(typeof(NicPage));
        }

        // 自定义的Pivot头部在切换时的外观变化
        private void Pivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var preGrid = (Grid)VisualTreeHelper.GetChild(PivotHeaderGrid, _preIndex);
            foreach (var childPnl in preGrid.Children)
            {
                childPnl.Visibility = childPnl.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }

            var selectedIndex = ((Pivot) sender).SelectedIndex;
            _preIndex = selectedIndex;

            var nowGrid = (Grid)VisualTreeHelper.GetChild(PivotHeaderGrid, _preIndex);
            foreach (var childPnl in nowGrid.Children)
            {
                childPnl.Visibility = childPnl.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // 点击自定义的Pivot头部事件
        private void PivotHeader_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            MainPagePivot.SelectedIndex = PivotHeaderGrid.Children.IndexOf((Grid) sender);
        }
    }
}
