using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Herald_UWP.Utils;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Herald_UWP.View
{
    /// <summary>
    /// 一卡通的页面
    /// </summary>
    public sealed partial class CardPage : Page
    {
        private BaseException currentApp = Application.Current as BaseException;
        private Card cardData;
        private Card todayData;

        public CardPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;  // 回到页面之后还是之前访问的状态
            InitializeContent();
        }

        private async void InitializeContent(bool isRefresh = false)
        {
            cardData = await currentApp.client.QueryRawData<Card>(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("timedelta", "7"),
                }, isRefresh);

            todayData = await currentApp.client.QueryRawData<Card>(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("timedelta", "1"),
                }, true, true);

            CardItemsCVS.Source = await GetCardItemsGrouped();
        }

        private ObservableCollection<CardItem> GetCardItems()
        {
            var cardItems = new ObservableCollection<CardItem>();

            if(todayData.content.detail.Length != 0)
            {
                foreach(CardItem item in todayData.content.detail)
                {
                    PreprocessCardItem(item);
                    cardItems.Add(item);
                }
            }

            foreach (CardItem item in cardData.content.detail)
            {
                PreprocessCardItem(item);
                cardItems.Add(item);
            }

            return cardItems;
        }

        // 对消费信息的预处理，分割日期以及修正银行转账
        private void PreprocessCardItem(CardItem item)
        {
            string[] tempData;
            tempData = item.date.Split(' ');
            item.date = tempData[0];
            item.time = tempData[1];
            if (item.system == "" && item.type == "银行转帐")
                item.system = "银行转帐";
        }

        private async Task<ObservableCollection<GroupInfoList>> GetCardItemsGrouped(bool isRefresh = true)
        {
            var groups = new ObservableCollection<GroupInfoList>();

            if (!isRefresh)
            {
                groups = await currentApp.client.QueryGroupedData("CardItems");
                if (groups != null)
                    return groups;
                else
                    groups = new ObservableCollection<GroupInfoList>();
            }

            var query = from item in GetCardItems()
                        group item by item.date into g
                        orderby g.Key descending
                        // GroupInfo包括日期、总收入、总支出，作为GroupInfo的key
                        select new { GroupInfo = new JObject(new JProperty("Date", g.Key), new JProperty("Income", g.Sum(o => FindIncome(o))),new JProperty("Outcome", g.Sum(o => FindOutcome(o)))), Items = g };

            foreach (var g in query)
            {
                var info = new GroupInfoList();
                info.Key = g.GroupInfo;
                foreach(var item in g.Items)
                {
                    info.Items.Add(item);
                }
                groups.Add(info);
            }
                        
            currentApp.client.StoreGroupedData("CardItems", groups);
            return groups;
        }

        private float FindIncome(CardItem cardItem)
        {
            float result;
            if (float.TryParse(cardItem.price, out result))
                return result > 0 ? result : 0;
            else
                return -9999;
        }

        private float FindOutcome(CardItem cardItem)
        {
            float result;
            if (float.TryParse(cardItem.price, out result))
                return result < 0 ? -result : 0;
            else
                return -9999;
        }

        private void DecideDisplayIncome_Loading(FrameworkElement sender, object args)
        {
            VisualTreeHelper.GetChild
            if (decimal.Parse(((TextBlock)VisualTreeHelper.GetChild(sender, 1)).Text) <= 0)
                sender.Visibility = Visibility.Collapsed;
        }

        private void PullToRefreshInvoked(DependencyObject sender, object args)
        {
            InitializeContent(true);
        }
    }
}
