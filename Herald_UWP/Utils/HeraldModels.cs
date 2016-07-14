using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

// 这里的数据类型名称和APIBasicInfo中各字段的名称保持一致，实现动态调用
namespace Herald_UWP.Utils
{
    /// <summary>
    /// 课表的两个数据类型
    /// </summary>
    [JsonObject]
    public class Curriculum
    {
        public DateTime StartDate { set; get; }
        public List<Course> Courses { set; get; } = new List<Course>();
    }

    // 单门课程的上课信息
    [JsonObject]
    public class Course
    {
        public string Name { set; get; }
        public string Classroom { set; get; }
        public int Day { set; get; }
        public int[] WeekRange { set; get; } = new int[2];
        public int[] TimeRange { set; get; } = new int[2];
    }

    /// <summary>
    /// 课程信息的两个数据类型
    /// </summary>
    [JsonObject]
    public class Sidebar
    {
        public Dictionary<string, CourseInfo> CourseInfos { set; get; } = new Dictionary<string, CourseInfo>();
    }

    [JsonObject]
    public class CourseInfo
    { 
        [JsonProperty("lecturer")]
        public string Teacher { set; get; }
        [JsonProperty("week")]
        public string Week { set; get; }
        [JsonProperty("credit")]
        public string Credit { set; get; }
    }

    /// <summary>
    /// 成绩的三个数据类型
    /// </summary>
    [JsonObject]
    public class Gpa
    {
        [JsonProperty("gpa")]
        public string AverageGpa { set; get; }
        [JsonProperty("gpa without revamp")]
        public string FirstGpa { set; get; }
        [JsonProperty("calculate time")]
        public string CalTime { set; get; }
        public List<GpaSemester> Semesters { set; get; } = new List<GpaSemester>();
    }

    // 每个学期的成绩信息
    [JsonObject]
    public class GpaSemester
    {
        public string Semester{set; get;}
        public List<GpaGrade> Grades { set; get; } = new List<GpaGrade>();
    }

    // 没门课的成绩信息
    [JsonObject]
    public class GpaGrade
    {
        [JsonProperty("name")]
        public string Name{set; get;}
        [JsonProperty("credit")]
        public string Credit{set; get;}
        [JsonProperty("score")]
        public string Score{set; get;}
        [JsonProperty("type")]
        public string Type{set; get;}
    }

    /// <summary>
    /// 三个一卡通相关的类
    /// </summary>
    [JsonObject]
    public class Card
    {
        [JsonProperty("state")]
        public string State{set; get;}
        [JsonProperty("left")]
        public float Left{set; get;}
        public List<CardDaily> CardDailys { set; get; } = new List<CardDaily>();
    }

    // 每日的消费
    [JsonObject]
    public class CardDaily
    {
        [JsonProperty("date")]
        public string Date{set; get;}
        public float Income{set; get;}
        public float Outcome{set; get;}
        public List<CardDailyDetail> CardDailyDetails { set; get; } = new List<CardDailyDetail>();
    }

    // 每项消费的详情
    [JsonObject]
    public class CardDailyDetail
    {
        [JsonProperty("system")]
        public string System{set; get;}
        [JsonProperty("type")]
        public string Type{set; get;}
        public string Time{set; get;}
        [JsonProperty("price")]
        public string Price{set; get;}
        [JsonProperty("left")]
        public string Left{set; get;}
    }

    /// <summary>
    /// 跑操信息
    /// </summary>
    [JsonObject]
    public class Pe
    {
        [JsonProperty("content")]
        public int DoneCount { set; get; }
        [JsonProperty("remain")]
        public int RemainDay { set; get; }
    }

    /// <summary>
    /// 跑操详细信息
    /// </summary>
    [JsonObject]
    public class PeDetail
    {
        public Dictionary<DateTimeOffset, PeDetailItem> Details { set; get; } = new Dictionary<DateTimeOffset, PeDetailItem>();
    }

    [JsonObject]
    public class PeDetailItem
    {
        [JsonProperty("sign_effect")]
        public string SignEffect { set; get; }
        public DateTime SginTime { set; get; }
    }

    [JsonObject]
    public class Nic
    {
        [JsonProperty("left")]
        public float Left { set; get; }
        public float Used { set; get; }
        public string Unit { set; get; }
        public string State { set; get; }
    }
}
