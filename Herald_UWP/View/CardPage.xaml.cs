using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Herald_UWP.Utils;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media;

namespace Herald_UWP.View
{
    /// <summary>
    /// 一卡通的页面
    /// </summary>
    public sealed partial class CardPage
    {
        private readonly App _currentApp = Application.Current as App;
        private Card _cardData;

        public CardPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeContent();
        }

        private async void InitializeContent()
        {
            _cardData = await _currentApp.Client.QueryForData<Card>(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("timedelta", "7"),
                });

            var todayData = await _currentApp.Client.QueryForData<Card>(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("timedelta", "1"),
                }, false);

            _cardData.CardDailys.InsertRange(0, todayData.CardDailys);

            CardItemsCVS.Source = _cardData.CardDailys;
        }

        private async void PullToRefreshInvoked(DependencyObject sender, object args)
        {
            // 计算有多少天没更新数据了
            var latestDate = _cardData.CardDailys.First().Date.Split('/');
            var oldDate = new DateTime(int.Parse(latestDate[0]), int.Parse(latestDate[1]), int.Parse(latestDate[2]));
            var newDate = DateTime.Now.ToLocalTime();
            var gapDay = (newDate - oldDate).Days;

            // 可能上一次跟新的时候当天数据没有获取完，所以删掉重新获取
            _cardData.CardDailys.RemoveAt(0);

            // gapDay不为零说明上次更新不是今天，要获取今天之前的内容
            if (gapDay != 0)
            {
                var newCardData = await _currentApp.Client.QueryForData<Card>(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("timedelta", gapDay.ToString()),
                    }, enableCache : false);
                _cardData.CardDailys.InsertRange(0, newCardData.CardDailys);
            }

            // 但不管上次更新是在什么时候，当天的信息都要重新获取
            var todayData = await _currentApp.Client.QueryForData<Card>(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("timedelta", "1"),
                }, false);
            _cardData.CardDailys.InsertRange(0, todayData.CardDailys);

            // 最后更新本地文件的内容
            FileSystem.Write("Card.data", JsonConvert.SerializeObject(_cardData));
            // 重新绑定数据
            CardItemsCVS.Source = _cardData.CardDailys;
        }

        private void DecideIncomeShowOnLoading(FrameworkElement sender, object args)
        {
            if (float.Parse(((TextBlock)VisualTreeHelper.GetChild(sender, 1)).Text) <= 0)
                sender.Visibility = Visibility.Collapsed;
        }
    }
}
