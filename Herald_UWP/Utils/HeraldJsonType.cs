using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

// 这里的数据类型名称和APIBasicInfo中各字段的名称保持一致，实现动态调用
namespace Herald_UWP.Utils
{
    // 基类
    [DataContract]
    public class BaseType
    {
        [DataMember(Order = 0)]
        public int code { get; set; }
    }

    // SRTP
    [DataContract]
    public class SRTP : BaseType
    {
        [DataMember(Order = 0)]
        public SRTPContent[] content { get; set; }
    }

    [DataContract]
    public class SRTPContent
    {
        [DataMember(Order = 0)]
        public string score { get; set; }
        [DataMember(Order = 1)]
        public string total { get; set; }
        [DataMember(Order = 2)]
        public string name { get; set; }
        [DataMember(Order = 3, Name = "card number")]
        public string cardNumber { get; set; }
        [DataMember(Order = 4)]
        public string credit { get; set; }
        [DataMember(Order = 5)]
        public string proportion { get; set; }
        [DataMember(Order = 6)]
        public string project { get; set; }
        [DataMember(Order = 7)]
        public string department { get; set; }
        [DataMember(Order = 8)]
        public string date { get; set; }
        [DataMember(Order = 9)]
        public string type { get; set; }
        [DataMember(Order = 10, Name = "total credit")]
        public string totalCredit { get; set; }
    }


    // 课程表
    [DataContract]
    public class Curriculum : BaseType
    {
        [DataMember(Order = 0)]
        public CurriculumContent content { get; set; }
    }

    // 单个课程信息是有三个元素的字符串数组，结构为：
    // ["课程名称","[上课周数]上课时间","上课地点"]
    [DataContract]
    public class CurriculumContent
    {
        [DataMember(Order = 0)]
        public string[][] Mon { get; set; }
        [DataMember(Order = 1)]
        public string[][] Tue { get; set; }
        [DataMember(Order = 2)]
        public string[][] Wed { get; set; }
        [DataMember(Order = 3)]
        public string[][] Thu { get; set; }
        [DataMember(Order = 4)]
        public string[][] Fri { get; set; }
        [DataMember(Order = 5)]
        public string[][] Sat { get; set; }
        [DataMember(Order = 6)]
        public string[][] Sun { get; set; }
    }


    // GPA
    [DataContract]
    public class GPA : BaseType
    {
        [DataMember(Order = 0)]
        public GPAContent[] content { get; set; }
    }
    
    [DataContract]
    public class GPAContent
    {
        [DataMember(Order = 0)]
        public string gpa { get; set; }
        [DataMember(Order = 1, Name = "gpa without revamp")]
        public string gpaWithoutRevamp { get; set; }
        [DataMember(Order = 2, Name = "calculate time")]
        public string calculateTime { get; set; }
        [DataMember(Order = 3)]
        public string semester { get; set; }
        [DataMember(Order = 4)]
        public string name { get; set; }
        [DataMember(Order = 5)]
        public string credit { get; set; }
        [DataMember(Order = 6)]
        public string score { get; set; }
        [DataMember(Order = 7)]
        public string type { get; set; }
        [DataMember(Order = 8)]
        public string extra { get; set; }
    }


    // 跑操信息概要
    [DataContract]
    public class PE : BaseType
    {
        [DataMember(Order = 0)]
        public string content { get; set; }
        [DataMember(Order = 1)]
        public string remain { get; set; }
    }


    // 跑操记录
    [DataContract]
    public class PEDetail : BaseType
    {
        [DataMember(Order = 0)]
        public PEDetailContent[] content { get; set; }
    }

    [DataContract]
    public class PEDetailContent
    {
        [DataMember(Order = 0, Name = "sign_time")]
        public string signTime { get; set; }
        [DataMember(Order = 1, Name = "sign_date")]
        public string signDate { get; set; }
        [DataMember(Order = 2, Name = "sign_effect")]
        public string signEffect { get; set; }
    }


    // 校园网信息
    [DataContract]
    public class NIC : BaseType
    {
        [DataMember(Order = 0)]
        public NICContent content { get; set; }
    }

    [DataContract]
    public class NICContent
    {
        [DataMember(Order = 0)]
        public NICInfo web { get; set; }
        [DataMember(Order = 1)]
        public string left { get; set; }
    }

    [DataContract]
    public class NICInfo
    {
        [DataMember(Order = 0)]
        public string state { get; set; }
        [DataMember(Order = 1)]
        public string used { get; set; }
    }


    // 一卡通
    [DataContract]
    public class Card : BaseType
    {
        [DataMember(Order = 0)]
        public CardContent content { set; get; }
    }

    [DataContract]
    public class CardContent
    {
        [DataMember(Order = 0)]
        public string state { set; get; }
        [DataMember(Order = 1)]
        public CardItem[] detial { set; get; }
        [DataMember(Order = 2)]
        public string left { set; get; }

    }

    [DataContract]
    public class CardItem
    {
        [DataMember(Order = 0)]
        public string date { set; get; }
        [DataMember(Order = 1)]
        public string price { set; get; }
        [DataMember(Order = 2)]
        public string type { set; get; }
        [DataMember(Order = 3)]
        public string system { set; get; }
        [DataMember(Order = 4)]
        public string left { set; get; }
    }


    // 人文讲座
    [DataContract]
    public class Lecture : BaseType
    {
        [DataMember(Order = 0)]
        public LectureContent content { get; set; }
    }

    [DataContract]
    public class LectureContent
    {
        [DataMember(Order = 0)]
        public int count { get; set; }
        [DataMember(Order = 1)]
        public LectureItem[] detial { get; set; }
    }

    [DataContract]
    public class LectureItem
    {
        [DataMember(Order = 0)]
        public string date { get; set; }
        [DataMember(Order = 1)]
        public string place { get; set; }
    }


    // 图书馆借阅信息
    [DataContract]
    public class Library : BaseType
    {
        [DataMember(Order = 0)]
        public LibraryItem[] content { get; set; }
    }

    [DataContract]
    public class LibraryItem
    {
        [DataMember(Order = 0, Name = "due_date")]
        public string dueDate { get; set; }
        [DataMember(Order = 1)]
        public string author { get; set; }
        [DataMember(Order = 2)]
        public string barcode { get; set; }
        [DataMember(Order = 3, Name = "render_date")]
        public string renderDate { get; set; }
        [DataMember(Order = 4)]
        public string place { get; set; }
        [DataMember(Order = 5)]
        public string title { get; set; }
        [DataMember(Order = 6, Name = "renew_time")]
        public string renewTime { get; set; }
    }


    // 图书馆图书搜索（前20条）
    [DataContract]
    public class Search : BaseType
    {
        [DataMember(Order = 0)]
        public BookItem[] content { get; set; }
    }

    [DataContract]
    public class BookItem
    {
        [DataMember(Order = 0)]
        public string index { get; set; }
        [DataMember(Order = 1)]
        public string all { get; set; }
        [DataMember(Order = 2)]
        public string name { get; set; }
        [DataMember(Order = 3)]
        public string author { get; set; }
        [DataMember(Order = 4)]
        public string publish { get; set; }
        [DataMember(Order = 5)]
        public string type { get; set; }
        [DataMember(Order = 6)]
        public string left { get; set; }
    }
}
