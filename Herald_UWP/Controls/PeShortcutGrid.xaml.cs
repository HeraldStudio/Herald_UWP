using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Herald_UWP.Controls
{
    public sealed partial class PeShortcutGrid
    {
        public static readonly DependencyProperty ForecastProperty = DependencyProperty.Register("Forecast",
            typeof(string), typeof(PeShortcutGrid), null);

        public string Forecast
        {
            get { return GetValue(ForecastProperty) as string; }
            set { SetValue(ForecastProperty, value);}
        }

        public static readonly DependencyProperty TodayStateProperty = DependencyProperty.Register("TodayState",
            typeof(string), typeof(PeShortcutGrid), null);

        public string TodayState
        {
            get { return GetValue(TodayStateProperty) as string; }
            set { SetValue(TodayStateProperty, value); }
        }

        public static readonly DependencyProperty DoneCountProperty = DependencyProperty.Register("DoneCount",
            typeof(string), typeof(PeShortcutGrid), null);

        public string DoneCount
        {
            get { return GetValue(DoneCountProperty) as string; }
            set { SetValue(DoneCountProperty, value); }
        }

        public static readonly DependencyProperty RemainCountProperty = DependencyProperty.Register("RemainCount",
            typeof(string), typeof(PeShortcutGrid), null);

        public string RemainCount
        {
            get { return GetValue(RemainCountProperty) as string; }
            set { SetValue(RemainCountProperty, value); }
        }

        public static readonly DependencyProperty RemainDayProperty = DependencyProperty.Register("RemainDay",
            typeof(string), typeof(PeShortcutGrid), null);

        public string RemainDay
        {
            get { return GetValue(RemainDayProperty) as string; }
            set { SetValue(RemainDayProperty, value); }
        }
        
        public PeShortcutGrid()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
