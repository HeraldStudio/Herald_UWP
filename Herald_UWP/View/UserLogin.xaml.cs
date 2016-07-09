using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Herald_UWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserLogin : Page
    {
        private BaseException currentApp = Application.Current as BaseException;

        public UserLogin()
        {
            this.InitializeComponent();
        }

        private async void TestAuth(object sender, RoutedEventArgs e)
        {
            string userID = tBox_UserID.Text;
            string password = pBox_Password.Password;

            Task<bool> authStatus = currentApp.client.Auth(userID, password);

            if (await authStatus)
            {
                this.Frame.Navigate(typeof(MainPage));
            }
        }
    }
}
