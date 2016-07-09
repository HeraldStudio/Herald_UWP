using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Herald_UWP.Utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Herald_UWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private BaseException currentApp = Application.Current as BaseException;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove("UUID");

            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(UserLogin));
            }
        }

        private void NaviToGPA(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(GPAPage));
            }
        }

        private void NaviToCard(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(CardPage));
            }
        }
    }
}
