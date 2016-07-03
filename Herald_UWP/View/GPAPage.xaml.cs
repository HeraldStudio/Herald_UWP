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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Herald_UWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GPAPage : Page
    {
        private App currentApp = Application.Current as App;
        private GPA GPAData;

        public GPAPage()
        {
            this.InitializeComponent();
            InitializeContent();
        }

        private async void InitializeContent()
        {
            // 获取GPA数据
            GPAData = await currentApp.client.Query<GPA>();

            // GPA中content的第一项（绩点信息）绑定到DataContext，其它的绑定到ListView
            DataContext = GPAData.content[0];
            GradesCVS.Source = GetGradesGrouped();
        }

        // 从GPAData中取出各科成绩
        private ObservableCollection<GPAContent> GetGrades()
        {
            ObservableCollection<GPAContent> grades = new ObservableCollection<GPAContent>();
            for (int i = 1; i < GPAData.content.Length; i++)
            {
                grades.Add(GPAData.content[i]);
            }
            return grades;
        }

        // 获得按照学期分好组的成绩
        private ObservableCollection<GroupInfoList> GetGradesGrouped()
        {
            ObservableCollection<GroupInfoList> groups = new ObservableCollection<GroupInfoList>();

            var query = from item in GetGrades()
                        group item by item.semester into g
                        orderby g.Key descending
                        select new { GroupName = g.Key, Items = g };

            foreach (var g in query)
            {
                GroupInfoList info = new GroupInfoList();
                info.key = g.GroupName;
                foreach (var item in g.Items)
                {
                    info.Add(item);
                }
                groups.Add(info);
            }

            return groups;
        }
    }
}
