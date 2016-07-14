using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;

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
            else
            {
                var dialog = new MessageDialog("用户名或密码错误");
                dialog.Commands.Add(new UICommand("确定"));
                await dialog.ShowAsync();
            }
        }
    }
}
