using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Herald_UWP.Utils;

namespace Herald_UWP.View
{
    public sealed partial class CurriculumPage
    {
        private readonly App _currentApp = Application.Current as App;
        private static Curriculum _curriculumData;
        private static Sidebar _sidebarData;
        private static Dictionary<Course, Grid> _courseGrids;

        // 预设了几种背景颜色
        private readonly Color[] _courseBackgroundColor = {
            Colors.Tomato,
            Colors.CadetBlue,
            Colors.DeepSkyBlue,
            Colors.SteelBlue,
            Colors.Coral,
            Colors.OliveDrab,
            Colors.SeaGreen,
            Colors.LightGreen
        };

        public CurriculumPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeContent();
        }

        private async void InitializeContent(bool isRefresh = false)
        {
            WeekNumList.ItemsSource = new[] {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", };

            _curriculumData = await _currentApp.Client.QueryForData<Curriculum>(isRefresh: isRefresh);
            _sidebarData = await _currentApp.Client.QueryForData<Sidebar>(isRefresh: isRefresh);
            _courseGrids = new Dictionary<Course, Grid>();

            // 给每个课都创建一个Grid
            foreach (var course in _curriculumData.Courses)
            {
                var courseGrid = CreateCourseGrid(course);
                _courseGrids.Add(course, courseGrid);

                // 把上面的Grid放到作为背景的Grid里面
                BackGrid.Children.Add(courseGrid);
                Grid.SetRow(courseGrid, course.TimeRange[0] - 1);
                Grid.SetRowSpan(courseGrid, course.TimeRange[1]);
                Grid.SetColumn(courseGrid, course.Day - 1);
            }

            // 为了测试假装现在还没放假
            // var nowDate = DateTime.Now;
            var nowDate = new DateTime(2016, 4, 20);
            var startDate = _curriculumData.StartDate;
            var nowWeek = ((nowDate - startDate).Days / 7);

            // 根据当前周数设定这个课要不要显示
            SetCourseGridVisibility(nowWeek);
        }

        // 绘制一个课程的Grid在界面上
        private Grid CreateCourseGrid(Course course)
        {
            CourseInfo courseInfo;
            _sidebarData.CourseInfos.TryGetValue(course.Name, out courseInfo);
            Debug.Assert(courseInfo != null, "courseInfo != null");

            // 课程名字
            var courseNameText = new TextBlock
            {
                Text = course.Name,
                FontSize = 12,
                Margin = new Thickness(2, 2, 2, 4),
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            // 教室
            var classroomText = new TextBlock
            {
                Text = course.Classroom,
                FontSize = 12,
                Margin = new Thickness(2,4,2,2),
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            // 放Text的Panel，正面信息
            var coursePanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            coursePanel.Children.Add(courseNameText);
            coursePanel.Children.Add(classroomText);


            // 教师
            var teacherText = new TextBlock
            {
                Text = courseInfo.Teacher,
                FontSize = 12,
                Margin = new Thickness(2, 4, 2, 2),
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            // 学分
            var creditText = new TextBlock
            {
                Text = "学分：" + courseInfo.Credit,
                FontSize = 12,
                Margin = new Thickness(2, 4, 2, 2),
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            // 周数
            var weekText = new TextBlock
            {
                Text = courseInfo.Week + "周",
                FontSize = 12,
                Margin = new Thickness(2, 4, 2, 2),
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            // 放Text的Panel，背面信息
            var courseBackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            courseBackPanel.Children.Add(teacherText);
            courseBackPanel.Children.Add(creditText);
            courseBackPanel.Children.Add(weekText);
            courseBackPanel.Visibility = Visibility.Collapsed;

            // 作为背景的Grid，一个课程对应一种颜色（可能重复）
            var colorNum = Math.Abs(GetStringHash(course.Name))%_courseBackgroundColor.Length;
            var courseGrid = new Grid
            {
                Background = new SolidColorBrush(_courseBackgroundColor[colorNum]),
                CornerRadius = new CornerRadius(5),
                BorderBrush = new SolidColorBrush(Color.FromArgb(21, 0, 0, 0)),
                BorderThickness = new Thickness(1)
            };
            courseGrid.Tapped += TurnCourseGrid;
            courseGrid.Children.Add(coursePanel);
            courseGrid.Children.Add(courseBackPanel);

            return courseGrid;
        }

        private static void TurnCourseGrid(object sender, TappedRoutedEventArgs e)
        {
            var courseGrid = sender as Grid;
            Debug.Assert(courseGrid != null, "courseGrid != null");

            foreach (var child in courseGrid.Children)
            {
                child.Visibility = child.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // 设置这门课是否显示
        private void SetCourseGridVisibility(int nowWeek)
        {
            WeekNum.Text = nowWeek.ToString();
            foreach (var courseGrid in _courseGrids)
            {
                if (courseGrid.Key.WeekRange[0] > nowWeek || nowWeek > courseGrid.Key.WeekRange[1])
                {
                    courseGrid.Value.Visibility = Visibility.Collapsed;
                }
                else
                {
                    courseGrid.Value.Visibility = Visibility.Visible;
                }
            }
        }

        private void SelectWeek(object sender, TappedRoutedEventArgs e)
        {
            var weekNum = int.Parse(((TextBlock) VisualTreeHelper.GetChild((UIElement) sender, 1)).Text);
            SetCourseGridVisibility(weekNum);
            SelectWeekFlyout.Hide();
        }

        // 因为C#里string的GetHashCode函数不保证每次运行都返回相同的值，所以自己写了一个
        private static int GetStringHash(string str)
        {
            unchecked
            {
                return str.Aggregate(23, (current, c) => current * 31 + c);
            }
        }

        // 下拉刷新的操作
        private void PullToRefreshInvoked(DependencyObject sender, object args)
        {
            BackGrid.Children.Clear();
            InitializeContent(true);
        }
    }
}
