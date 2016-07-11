using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Herald_UWP.View
{
    public sealed partial class UserLogin
    {
        private readonly App _currentApp = Application.Current as App;

        public UserLogin()
        {
            InitializeComponent();
        }

        private async void TestAuth(object sender, RoutedEventArgs e)
        {
            var userId = TBoxUserId.Text;
            var password = PBoxPassword.Password;

            var authStatus = _currentApp.Client.Auth(userId, password);

            if (await authStatus)
            {
                Frame?.Navigate(typeof(MainPage));
            }
        }
    }
}
