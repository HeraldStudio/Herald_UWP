﻿using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Herald_UWP.Utils;
using System.Collections.ObjectModel;

namespace Herald_UWP.View
{
    /// <summary>
    /// 成绩信息的页面
    /// </summary>
    public sealed partial class GPAPage : Page
    {
        private App currentApp = Application.Current as App;
        private GPA GPAData;

        public GPAPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;  // 回到页面之后还是之前访问的状态
            InitializeContent();
        }

        private async void InitializeContent(bool isRefresh = false)
        {
            // 获取GPA数据
            GPAData = await currentApp.client.Query<GPA>(isRefresh : isRefresh);

            // GPA中content的第一项（绩点信息）绑定到DataContext，其它的绑定到ListView
            DataContext = GPAData.content[0];
            GradesCVS.Source = GetGradesGrouped();
        }

        // 从GPAData中取出各科成绩
        private ObservableCollection<GPAContent> GetGrades()
        {
            var grades = new ObservableCollection<GPAContent>();
            for (int i = 1; i < GPAData.content.Length; i++)
            {
                grades.Add(GPAData.content[i]);
            }
            return grades;
        }

        // 获得按照学期分好组的成绩
        private ObservableCollection<GroupInfoList> GetGradesGrouped()
        {
            var groups = new ObservableCollection<GroupInfoList>();
            var query = from item in GetGrades()
                        group item by item.semester into g
                        orderby g.Key descending
                        select new { GroupName = g.Key, Items = g };

            foreach (var g in query)
            {
                var info = new GroupInfoList();
                info.key = g.GroupName;
                foreach (var item in g.Items)
                {
                    info.Add(item);
                }
                groups.Add(info);
            }

            return groups;
        }

        private void PullToRefreshInvoked(DependencyObject sender, object args)
        {
            InitializeContent(true);
        }
    }
}