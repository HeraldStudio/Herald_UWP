﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Herald_UWP.Utils;

namespace Herald_UWP.View
{
    /// <summary>
    /// 成绩信息的页面
    /// </summary>
    public sealed partial class GpaPage
    {
        private readonly App _currentApp = Application.Current as App;
        private Gpa _gpaData;

        public GpaPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeContent(false);
        }

        private async void InitializeContent(bool enableCache)
        {
            // 获取GPA数据
            _gpaData = await _currentApp.Client.QueryForData<Gpa>(enableCache : enableCache);

            // 绑定数据
            DataContext = _gpaData;
            GradesCVS.Source = _gpaData.Semesters;
        }

        private void PullToRefreshInvoked(DependencyObject sender, object args)
        {
            InitializeContent(true);
        }
    }
}