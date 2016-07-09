using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Herald_UWP.Utils;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Herald_UWP.View
{
    /// <summary>
    /// 成绩信息的页面
    /// </summary>
    public sealed partial class GPAPage : Page
    {
        private BaseException currentApp = Application.Current as BaseException;
        private GPA GPAData;

        public GPAPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeContent(false);
        }

        private async void InitializeContent(bool isRefresh)
        {
            // 获取GPA数据
            GPAData = await currentApp.client.QueryForData<GPA>(isRefresh : isRefresh);

            // 绑定数据
            DataContext = GPAData;
            GradesCVS.Source = GPAData.Semesters;
        }

        private void PullToRefreshInvoked(DependencyObject sender, object args)
        {
            InitializeContent(true);
        }
    }
}