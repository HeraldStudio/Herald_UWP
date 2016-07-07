using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Herald_UWP.Utils;
using System.Collections.ObjectModel;

namespace Herald_UWP.View
{
    /// <summary>
    /// 一卡通的页面
    /// </summary>
    public sealed partial class CardPage : Page
    {
        private App currentApp = Application.Current as App;
        private Card cardData;

        public CardPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;  // 回到页面之后还是之前访问的状态
            InitializeContent();
        }

        private async void InitializeContent()
        {
            cardData = await currentApp.client.Query<Card>(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("timedelta", "7"),
                });

            CardItemsCVS.Source = GetCardItemsGrouped();
        }

        private ObservableCollection<CardItem> GetCardItems()
        {
            var cardItems = new ObservableCollection<CardItem>();
            string[] tempData;
            foreach (CardItem item in cardData.content.detial)
            {
                tempData = item.date.Split(' ');
                item.date = tempData[0];
                item.time = tempData[1];
                if (item.system == "" && item.type == "银行转账")
                    item.system = "银行转账";
                cardItems.Add(item);
            }
            return cardItems;
        }

        private ObservableCollection<GroupInfoList> GetCardItemsGrouped()
        {
            var groups = new ObservableCollection<GroupInfoList>();
            var query = from item in GetCardItems()
                        group item by item.date into g
                        orderby g.Key descending
                        // GroupInfo包括日期、总收入、总支出，作为GroupInfo的key
                        select new { GroupInfo = new { Date = g.Key, Income = g.Sum(o => CalculateIncome(o)), Outcome = g.Sum(o => CalculateOutcome(o)) }, Items = g };

            foreach (var g in query)
            {
                var info = new GroupInfoList();
                info.key = g.GroupInfo;
                foreach(var item in g.Items)
                {
                    info.Add(item);
                }
                groups.Add(info);
            }

            return groups;
        }

        private decimal CalculateIncome(CardItem cardItem)
        {
            decimal result;
            if (decimal.TryParse(cardItem.price, out result))
                return result > 0 ? result : 0;
            else
                return -9999;
        }

        private decimal CalculateOutcome(CardItem cardItem)
        {
            decimal result;
            if (decimal.TryParse(cardItem.price, out result))
                return result < 0 ? -result : 0;
            else
                return -9999;
        }

        private void StackPanel_Loading(FrameworkElement sender, object args)
        {
            if (decimal.Parse(((TextBlock)VisualTreeHelper.GetChild(sender, 1)).Text) <= 0)
                sender.Visibility = Visibility.Collapsed;
        }
    }
}
