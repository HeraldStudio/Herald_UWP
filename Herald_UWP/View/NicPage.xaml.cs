using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Herald_UWP.Utils;

namespace Herald_UWP.View
{
    public sealed partial class NicPage
    {
        private readonly App _currentApp = Application.Current as App;
        private const float TenGb = 10 * 1024;
        private static Nic _nicData; 

        public NicPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeContent();
        }

        private async void InitializeContent(bool isRefresh = false)
        {
            _nicData = await _currentApp.Client.QueryForData<Nic>(isRefresh: isRefresh);
            DataContext = _nicData;

            if (NicGraphGrid.Children.Count == 2) NicGraphGrid.Children.RemoveAt(1);
            DrawSector();
        }

        private void DrawSector()
        { 
            // 如果单位过小就不绘制了，根本看不出来
            if (_nicData.Unit == "B" || _nicData.Unit == "KB") return;

            // 计算圆弧结束点的坐标
            double usedPercent = 0;
            if (_nicData.Unit == "MB") usedPercent = _nicData.Used / TenGb;
            if (_nicData.Unit == "GB") usedPercent = _nicData.Used / ((Math.Floor(_nicData.Used / 10) + 1) * 10);
            var radius = NicCircle.Width / 2;
            var endX = radius + radius * Math.Sin(usedPercent * 2 * Math.PI);
            var endY = radius - radius * Math.Cos(usedPercent * 2 * Math.PI);

            // 开始绘制，先描绘路径
            var isLargeArc = usedPercent >= 0.5 ? "1" : "0";
            string pathDataStr = $"M{radius},0 V{radius} L{endX},{endY} A {radius},{radius} {usedPercent * 360} {isLargeArc} 0 {radius},0";
            // 根据路径获得对应的控件
            var sector = XamlReader.Load($"<Path xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><Path.Data>{pathDataStr}</Path.Data></Path>") as Path;
            if (sector == null) return;
            sector.Fill = Resources["NicUsedColor"] as SolidColorBrush;
            NicGraphGrid.Children.Add(sector);
        }

        private void PullToRefreshInvoked(DependencyObject sender, object args)
        {
            InitializeContent(true);
        }
    }
}
