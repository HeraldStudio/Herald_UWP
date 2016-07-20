using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Herald_UWP.Utils;

namespace Herald_UWP.View
{
    public sealed partial class PePage
    {
        private static readonly App CuurrentApp = Application.Current as App;
        private static PeDetail _peDetailData;
        private static Pe _peData;
        private static Pc _pcData;
        
        public PePage()
        {
            InitializeComponent();
            InitializeView();
        }

        // 在首页进行刷新数据时用到的方法
        public static async Task InitializeData(bool isRefresh = false)
        {
            _peDetailData = await CuurrentApp.Client.QueryForData<PeDetail>(isRefresh: isRefresh);
            _peData = await CuurrentApp.Client.QueryForData<Pe>(isRefresh: isRefresh);
            _pcData = await CuurrentApp.Client.QueryForData<Pc>(isRefresh: isRefresh);
        }

        // 首页会用到的两个对象
        public static Pc GetPcData() { return _pcData; }
        public static Pe GetPeData() { return _peData; }

        public static object GetPePageData()
        {
            return new {State = _pcData.State, Pe = _peData};
        }

        // 绘制日历视图
        private async void InitializeView(bool isRefresh = false)
        {
            if (_peData == null || _peDetailData == null || _pcData == null) await InitializeData(isRefresh);
            DataContext = _peData;

            if (PeDetailCalendarGrid.Children.Count != 0) PeDetailCalendarGrid.Children.RemoveAt(0);
            var peCalendarView = new CalendarView()
            {
                Language = "zh",
                SelectionMode = CalendarViewSelectionMode.None,
                PressedBorderBrush = new SolidColorBrush(Colors.White),
                PressedForeground = new SolidColorBrush(Colors.Black),
                OutOfScopeForeground = new SolidColorBrush(Colors.LightGray),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                MaxDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, DateTime.Now.Day),
                MinDate = new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, DateTime.Now.Day)
            };
            peCalendarView.CalendarViewDayItemChanging += SetDayStateOnLoading;
            PeDetailCalendarGrid.Children.Add(peCalendarView);
        }

        // 刷新
        private void PullToRefreshInvoked(DependencyObject sender, object args)
        {
            InitializeView(true);
        }

        // 在加载日历的时候对每个日期判断是否跑过
        private void SetDayStateOnLoading(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            var dayItem = args.Item;
            Debug.Assert(dayItem != null, "当前DayItem不存在");

            PeDetailItem peItem;
            if (!_peDetailData.Details.TryGetValue(dayItem.Date, out peItem)) return;
            if (peItem.SignEffect != "有效") return;
            dayItem.Background = Resources["PeThemeColor"] as SolidColorBrush;
        }
    }
}
