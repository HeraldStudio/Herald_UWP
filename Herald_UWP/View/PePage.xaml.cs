using System;
using System.Diagnostics;
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
        private readonly App _cuurrentApp = Application.Current as App;
        private static PeDetail _peDetailData;
        private static Pe _peData;

        public PePage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeContent();
        }

        private async void InitializeContent(bool isRefresh = false)
        {
            _peDetailData = await _cuurrentApp.Client.QueryForData<PeDetail>(isRefresh: isRefresh);
            _peData = await _cuurrentApp.Client.QueryForData<Pe>(isRefresh: isRefresh);
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

        private void PullToRefreshInvoked(DependencyObject sender, object args)
        {
            InitializeContent(true);
        }

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
