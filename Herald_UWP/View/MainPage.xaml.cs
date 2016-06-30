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
        private App currentApp = Application.Current as App;

        public MainPage()
        {
            this.InitializeComponent();
            InitialContent();
        }

        private void InitialContent()
        {
            QueryGPA();
        }

        private async void QueryGPA()
        {
            GPA info = await currentApp.client.Query<GPA>();
            if (info != null)
            {
                string str = "\n平均绩点：" + info.content[0].gpa + "\n计算时间：" + info.content[0].calculateTime;
                GPA.Text = str;
            }
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

        private void DeleteFile(object sender, RoutedEventArgs e)
        {
            FileSystem.Delete("GPA.data");
            QueryGPA();
        }
    }
}
