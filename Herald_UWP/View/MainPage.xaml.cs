using Windows.UI.Xaml;

namespace Herald_UWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
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
    }
}
