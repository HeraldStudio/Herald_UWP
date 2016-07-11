using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Herald_UWP.Utils;
using Windows.UI.Xaml.Navigation;

namespace Herald_UWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CurriculumPage
    {
        private readonly App _currentApp = Application.Current as App;
        private Curriculum _curriculumData;

        // 预设了10种背景颜色
        private readonly Color[] _courseBackgroundColor = new Color[]
        {
            Colors.Tomato,
            Colors.CadetBlue,
            Colors.DeepSkyBlue,
            Colors.SteelBlue,
            Colors.Coral,
            Colors.OliveDrab,
            Colors.SeaGreen,
        };

        public CurriculumPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeContent();
        }

        private async void InitializeContent()
        {
            _curriculumData = await _currentApp.Client.QueryForData<Curriculum>();

            // var nowDate = DateTime.Now;
            // 为了测试假装现在还没放假
            var nowDate = new DateTime(2016, 4, 20);
            var startDate = _curriculumData.StartDate;
            var nowWeek = ((nowDate - startDate).Days / 7);

            WeekNum.Text = nowWeek.ToString();

            for (var d = 0; d < 7; d++)
            {
                foreach (var course in _curriculumData.Courses[d])
                {
                    if (course.WeekRange[0] > nowWeek || nowWeek > course.WeekRange[1]) continue;
                    CreateCoursePanel(course, d);
                }
            }
        }

        private void CreateCoursePanel(Course course, int day)
        {
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
            var classroomText = new TextBlock()
            {
                Text = course.Classroom,
                FontSize = 12,
                Margin = new Thickness(2,4,2,2),
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            var coursePanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            coursePanel.Children.Add(courseNameText);
            coursePanel.Children.Add(classroomText);

            var colorNum = Math.Abs(course.Name.GetHashCode()) % _courseBackgroundColor.Length;
            var courseGrid = new Grid()
            {
                Background = new SolidColorBrush(_courseBackgroundColor[colorNum]),
                CornerRadius = new CornerRadius(5),
                BorderBrush = new SolidColorBrush(Color.FromArgb(32, 0, 0, 0)),
                BorderThickness = new Thickness(1),
            };

            courseGrid.Children.Add(coursePanel);

            BackGrid.Children.Add(courseGrid);
            Grid.SetRow(courseGrid, course.TimeRange[0]);
            Grid.SetRowSpan(courseGrid, course.TimeRange[1]);
            Grid.SetColumn(courseGrid, day + 1);
        }
    }
}
