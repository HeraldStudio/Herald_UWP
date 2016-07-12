﻿using System;
using System.Collections.Generic;
using Windows.Media.Devices;
using Newtonsoft.Json;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

// 这里的数据类型名称和APIBasicInfo中各字段的名称保持一致，实现动态调用
namespace Herald_UWP.Utils
{
    //// 分组用的数据类型
    //public class GroupInfoList
    //{
    //    public JObject Key { get; set; }
    //    public List<object> Items { get; set; } = new List<object>();
    //}

    //// 基类
    //[DataContract]
    //public class BaseHeraldType
    //{
    //    [DataMember(Order = 0)]
    //    public int code { get; set; }
    //}

    //// SRTP
    //[DataContract]
    //public class SRTP : BaseHeraldType
    //{
    //    [DataMember(Order = 0)]
    //    public SRTPContent[] content { get; set; }
    //}

    //[DataContract]
    //public class SRTPContent
    //{
    //    [DataMember(Order = 0)]
    //    public string score { get; set; }
    //    [DataMember(Order = 1)]
    //    public string total { get; set; }
    //    [DataMember(Order = 2)]
    //    public string name { get; set; }
    //    [DataMember(Order = 3, Name = "card number")]
    //    public string cardNumber { get; set; }
    //    [DataMember(Order = 4)]
    //    public string credit { get; set; }
    //    [DataMember(Order = 5)]
    //    public string proportion { get; set; }
    //    [DataMember(Order = 6)]
    //    public string project { get; set; }
    //    [DataMember(Order = 7)]
    //    public string department { get; set; }
    //    [DataMember(Order = 8)]
    //    public string date { get; set; }
    //    [DataMember(Order = 9)]
    //    public string type { get; set; }
    //    [DataMember(Order = 10, Name = "total credit")]
    //    public string totalCredit { get; set; }
    //}

    [JsonObject]
    public class Curriculum
    {
        public DateTime StartDate { set; get; }
        public List<Course> Courses { set; get; } = new List<Course>();
    }

    [JsonObject]
    public class Course
    {
        public string Name { set; get; }
        public string Classroom { set; get; }
        public int Day { set; get; }
        public int[] WeekRange { set; get; } = new int[2];
        public int[] TimeRange { set; get; } = new int[2];
    }

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

    [JsonObject]
    public class GpaSemester
    {
        public string Semester{set; get;}
        public List<GpaGrade> Grades { set; get; } = new List<GpaGrade>();
    }

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

    //// 跑操信息概要
    //[DataContract]
    //public class PE : BaseHeraldType
    //{
    //    [DataMember(Order = 0)]
    //    public string content { get; set; }
    //    [DataMember(Order = 1)]
    //    public string remain { get; set; }
    //}


    //// 跑操记录
    //[DataContract]
    //public class PEDetail : BaseHeraldType
    //{
    //    [DataMember(Order = 0)]
    //    public PEDetailContent[] content { get; set; }
    //}

    //[DataContract]
    //public class PEDetailContent
    //{
    //    [DataMember(Order = 0, Name = "sign_time")]
    //    public string signTime { get; set; }
    //    [DataMember(Order = 1, Name = "sign_date")]
    //    public string signDate { get; set; }
    //    [DataMember(Order = 2, Name = "sign_effect")]
    //    public string signEffect { get; set; }
    //}


    //// 校园网信息
    //[DataContract]
    //public class NIC : BaseHeraldType
    //{
    //    [DataMember(Order = 0)]
    //    public NICContent content { get; set; }
    //}

    //[DataContract]
    //public class NICContent
    //{
    //    [DataMember(Order = 0)]
    //    public NICInfo web { get; set; }
    //    [DataMember(Order = 1)]
    //    public string left { get; set; }
    //}

    //[DataContract]
    //public class NICInfo
    //{
    //    [DataMember(Order = 0)]
    //    public string state { get; set; }
    //    [DataMember(Order = 1)]
    //    public string used { get; set; }
    //}


    //// 一卡通
    //[DataContract]
    //public class Card : BaseHeraldType
    //{
    //    [DataMember(Order = 0)]
    //    public CardContent content { set; get; }
    //}

    //[DataContract]
    //public class CardContent
    //{
    //    [DataMember(Order = 0)]
    //    public string state { set; get; }
    //    [DataMember(Order = 1, Name = "detial")]
    //    public CardItem[] detail { set; get; }
    //    [DataMember(Order = 2)]
    //    public string left { set; get; }
    //}

    //[DataContract]
    //public class CardItem
    //{
    //    [DataMember(Order = 0)]
    //    public string date { set; get; }
    //    [DataMember(Order = 1)]
    //    public string price { set; get; }
    //    [DataMember(Order = 2)]
    //    public string type { set; get; }
    //    [DataMember(Order = 3)]
    //    public string system { set; get; }
    //    [DataMember(Order = 4)]
    //    public string left { set; get; }
    //    [DataMember(Order = 5)]
    //    public string time { set; get; }
    //}

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

    [JsonObject]
    public class CardDaily
    {
        [JsonProperty("date")]
        public string Date{set; get;}
        public float Income{set; get;}
        public float Outcome{set; get;}
        public List<CardDailyDetail> CardDailyDetails { set; get; } = new List<CardDailyDetail>();
    }

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

    //// 人文讲座
    //[DataContract]
    //public class Lecture : BaseHeraldType
    //{
    //    [DataMember(Order = 0)]
    //    public LectureContent content { get; set; }
    //}

    //[DataContract]
    //public class LectureContent
    //{
    //    [DataMember(Order = 0)]
    //    public int count { get; set; }
    //    [DataMember(Order = 1)]
    //    public LectureItem[] detial { get; set; }
    //}

    //[DataContract]
    //public class LectureItem
    //{
    //    [DataMember(Order = 0)]
    //    public string date { get; set; }
    //    [DataMember(Order = 1)]
    //    public string place { get; set; }
    //}


    //// 图书馆借阅信息
    //[DataContract]
    //public class Library : BaseHeraldType
    //{
    //    [DataMember(Order = 0)]
    //    public LibraryItem[] content { get; set; }
    //}

    //[DataContract]
    //public class LibraryItem
    //{
    //    [DataMember(Order = 0, Name = "due_date")]
    //    public string dueDate { get; set; }
    //    [DataMember(Order = 1)]
    //    public string author { get; set; }
    //    [DataMember(Order = 2)]
    //    public string barcode { get; set; }
    //    [DataMember(Order = 3, Name = "render_date")]
    //    public string renderDate { get; set; }
    //    [DataMember(Order = 4)]
    //    public string place { get; set; }
    //    [DataMember(Order = 5)]
    //    public string title { get; set; }
    //    [DataMember(Order = 6, Name = "renew_time")]
    //    public string renewTime { get; set; }
    //}


    //// 图书馆图书搜索（前20条）
    //[DataContract]
    //public class Search : BaseHeraldType
    //{
    //    [DataMember(Order = 0)]
    //    public BookItem[] content { get; set; }
    //}

    //[DataContract]
    //public class BookItem
    //{
    //    [DataMember(Order = 0)]
    //    public string index { get; set; }
    //    [DataMember(Order = 1)]
    //    public string all { get; set; }
    //    [DataMember(Order = 2)]
    //    public string name { get; set; }
    //    [DataMember(Order = 3)]
    //    public string author { get; set; }
    //    [DataMember(Order = 4)]
    //    public string publish { get; set; }
    //    [DataMember(Order = 5)]
    //    public string type { get; set; }
    //    [DataMember(Order = 6)]
    //    public string left { get; set; }
    //}
}
