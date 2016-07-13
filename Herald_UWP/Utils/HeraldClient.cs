using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Herald_UWP.Utils
{
    public class HeraldClient
    {
        private readonly HeraldHttpUtil _client = new HeraldHttpUtil();
        public string Uuid { get; set; }

        /// <summary>
        /// 获取授权，保存UUID
        /// </summary>
        /// <param name="userId">用户一卡通号</param>
        /// <param name="password">用户密码</param>
        /// <returns>授权是否成功</returns>
        public async Task<bool> Auth(string userId, string password)
        {
            // 请求所需的参数
            var requestContent = new HttpFormUrlEncodedContent(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user", userId),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("appid", ApiBasicInfo.AppId),
                });

            // 发起请求，结果即为UUID，同时要接收异常
            try
            {
                var responseContent = _client.Post(ApiBasicInfo.Auth, requestContent);
                Uuid = await responseContent;
            }
            catch (Exception)
            {
                return false;
            }

            // 把UUID也存到localSettings里面，供以后用户直接使用
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["UUID"] = Uuid;

            return true;
        }

        /// <summary>
        /// 获得最终可以直接用的数据对象
        /// </summary>
        /// <typeparam name="T">查询的项目类型，如一卡通Card、成绩GPA</typeparam>
        /// <param name="param">查询时可能会需要的参数</param>
        /// <param name="isRefresh">若是刷新，则直接从网络获取数据，并缓存</param>
        /// <param name="enableCache">是否使用缓存</param>
        /// <returns>同T</returns>
        public async Task<T> QueryForData<T>(List<KeyValuePair<string, string>> param = null, bool isRefresh = false, bool enableCache = true)
        {
            // 根据查询的信息的类名字，获取API中对应的地址,这个类名字到后面还可以用来调用函数
            var apiName = typeof(T).Name;
            var address = (string)typeof(ApiBasicInfo).GetField(apiName).GetValue(null);

            // 先从本地获取数据，如果没有本地数据则从服务器获取
            if (enableCache && !isRefresh)
            {
                var resultStr = await FileSystem.Read(apiName + ".data");
                if (!string.IsNullOrEmpty(resultStr)) return JsonConvert.DeserializeObject<T>(resultStr);
            }

            // 通过类名字获取对应的Json处理函数
            var method = typeof(HeraldClient).GetMethod("Get" + apiName);

            // 可能会出现异常，先不处理，抛给上一层
            try
            {
                // 先获得Json对象，然后用Json处理函数处理为对应数据类的对象
                var jObject = await QueryForJson(address, param);
                var resultObj = (T)method.Invoke(this, new object[] { jObject });

                // enableCache为true，则把数据也写入本地数据
                if (!enableCache) return resultObj;

                var jsonStr = JsonConvert.SerializeObject(resultObj);
                FileSystem.Write(apiName + ".data", jsonStr);

                return resultObj;
            }
            catch (HeraldRequestException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 向指定的API获取Json数据并转换为JObject格式返回，遇到错误会抛出异常
        /// </summary>
        /// <param name="url">API地址</param>
        /// <param name="param">额外的参数</param>
        /// <returns>JObeject格式的结果</returns>
        private async Task<JObject> QueryForJson(string url, List<KeyValuePair<string, string>> param = null)
        {
            if (param == null)
                param = new List<KeyValuePair<string, string>>();

            param.Add(new KeyValuePair<string, string>("uuid", Uuid));
            var requestContent = new HttpFormUrlEncodedContent(param);

            // 先将获取到的Json直接转换为JObject用于预处理
            var resultObj = new JObject();
            bool isWrongResult;

            // 因为服务器的问题，偶尔会返回错误信息而不是Json数据，多试几次就好了,最多试10次
            var loopCount = 10;
            do
            {
                if (loopCount-- <= 0) break;
                var resultStr = await _client.Post(url, requestContent);
                try
                {
                    isWrongResult = false;
                    resultObj = JObject.Parse(resultStr);
                    if (resultObj["code"].Value<int>() == 500) isWrongResult = true;
                    if (url == ApiBasicInfo.Sidebar && !resultObj["content"].HasValues) isWrongResult = true;
                }
                catch (JsonReaderException)
                {
                    isWrongResult = true;
                }
            } while (isWrongResult);

            // 处理返回的code，对于错误的进一步处理还没写，暂时作为异常抛出
            var code = resultObj["code"].Value<int>();
            switch (code)
            {
                case 200:
                    return resultObj;
                case 400:
                    throw new HeraldRequestException("Error " + code + "：缺少参数");
                case 401:
                    throw new HeraldRequestException("Error" + code + "：身份认证失败");
                case 408:
                    throw new HeraldRequestException("Error" + code + "：访问超时");
                case 500:
                    throw new HeraldRequestException("Error" + code + "：系统错误");
                default:
                    throw new HeraldRequestException("Error" + code + "其它未知错误");
            }
        }

        public Gpa GetGpa(JObject json)
        {
            var gradeArray = (JArray)json["content"];    // 取出content，这是一个数组
            var gpaInfo = gradeArray[0].ToObject<Gpa>();    // 将content的第一个项取出来，里边含有平均绩点等信息

            // 移去content的第一个项，对剩余的项用Linq分组
            gradeArray.RemoveAt(0);
            var query = from item in gradeArray
                        group item by item["semester"].Value<string>() into g
                        orderby g.Key descending
                        select new { GroupName = g.Key, Items = g };

            // 分组之后的信息放到GPA对应的Semesters里
            foreach (var g in query)
            {
                var info = new GpaSemester {Semester = g.GroupName};

                foreach (var item in g.Items)
                {
                    info.Grades.Add(item.ToObject<GpaGrade>());
                }
                gpaInfo.Semesters.Add(info);
            }

            return gpaInfo;
        }

        public Card GetCard(JObject json)
        {
            var cardInfo = json["content"].ToObject<Card>();
            var detailArray = (JArray)json["content"]["detial"];

            // 分割date为日期和时间，并处理type为“银行转帐”时system的空白
            foreach (var jToken in detailArray)
            {
                var detail = (JObject) jToken;
                var tempData = detail["date"].Value<string>().Split(' ');

                detail["date"] = tempData[0];
                detail.Property("date").AddAfterSelf(new JProperty("time", tempData[1]));

                if (detail["system"].Value<string>() == "" && detail["type"].Value<string>() == "银行转帐")
                    detail["system"] = "银行转帐";
            }

            // 用Linq分组
            var query = from item in detailArray
                        group item by item["date"].Value<string>() into g
                        orderby g.Key descending
                        // GroupInfo包括日期、总收入、总支出，作为GroupInfo的key
                        select new
                        {
                            GroupInfo = new
                            {
                                Date = g.Key,
                                // 将price中的正数相加作为收入
                                Income = g.Sum(cardDetail =>
                                {
                                    var result = cardDetail["price"].Value<float>();
                                    return result > 0 ? result : 0;
                                }),
                                // 将price中的负数取相反数相加作为支出
                                Outcome = g.Sum(cardDetail =>
                                {
                                    var result = cardDetail["price"].Value<float>();
                                    return result < 0 ? -result : 0;
                                })
                            },
                            Items = g
                        };

            // 将分组后的信息放到Card对应的CardDailys里
            foreach (var g in query)
            {
                var info = new CardDaily
                {
                    Date = g.GroupInfo.Date,
                    Income = g.GroupInfo.Income,
                    Outcome = g.GroupInfo.Outcome
                };

                foreach (var item in g.Items)
                {
                    info.CardDailyDetails.Add(item.ToObject<CardDailyDetail>());
                }
                cardInfo.CardDailys.Add(info);
            }

            return cardInfo;
        }

        public Sidebar GetSidebar(JObject json)
        {
            var sidebarInfo = new Sidebar();
            foreach (var jCourse in json["content"])
            {
                var courseInfo = jCourse.ToObject<CourseInfo>();
                sidebarInfo.CourseInfos.Add(jCourse["course"].Value<string>(), courseInfo);
            }
            return sidebarInfo;
        }

        public Pe GetPe(JObject json)
        {
            return json.ToObject<Pe>();
        }

        public PeDetail GetPeDetail(JObject json)
        {
            var peDetailInfo = new PeDetail();
            foreach (var jDetail in json["content"])
            {
                var detail = jDetail.ToObject<PeDetailItem>();

                var signDate = jDetail["sign_date"].Value<string>();
                var signTime = jDetail["sign_time"].Value<string>();
                var splitedDate = signDate.Split('-');
                var splitedTime = signTime.Split('.');

                detail.SginTime = new DateTime(int.Parse(splitedDate[0]), int.Parse(splitedDate[1]), int.Parse(splitedDate[2]), int.Parse(splitedTime[0]), int.Parse(splitedTime[1]), 0);
                peDetailInfo.Details.Add(new DateTime(detail.SginTime.Year, detail.SginTime.Month, detail.SginTime.Day), detail);
            }
            return peDetailInfo;
        }

        public Curriculum GetCurriculum(JObject json)
        {
            var curriculumInfo = new Curriculum();
            var dayDictionary = new Dictionary<string, int>()
            {
                {"Mon",1 },
                {"Tue",2 },
                {"Wed",3 },
                {"Thu",4 },
                {"Fri",5 },
                {"Sat",6 },
                {"Sun",7 }
            };

            // 从服务器获得的开始日期没有年份，这就很尴尬了，先假设在当前年份
            var nowDate = DateTime.Now;
            var startMonth = json["content"]["startdate"]["month"].Value<int>();
            var startDay = json["content"]["startdate"]["day"].Value<int>();
            var startDate = new DateTime(nowDate.Year,startMonth,startDay);

            // 如果这个假设的日期比现在还靠后，那么就认为开始日期再前一年
            var gap = (nowDate - startDate).Days;
            curriculumInfo.StartDate = gap > 0 ? startDate : new DateTime(nowDate.Year - 1, startMonth, startDay);

            foreach (var jCourses in json["content"])
            {
                foreach (var jCourse in jCourses.Values())
                {
                    int tempDay;
                    if (!dayDictionary.TryGetValue(((JProperty)jCourses).Name, out tempDay)) break;

                    var course = new Course()
                    {
                        Name = jCourse[0].Value<string>(),
                        Classroom = jCourse[2].Value<string>(),
                        Day = tempDay
                    };

                    var temp = jCourse[1].Value<string>();
                    var index1 = temp.IndexOf("-", StringComparison.Ordinal);
                    var index2 = temp.IndexOf("周", StringComparison.Ordinal);
                    var index3 = temp.IndexOf("节", StringComparison.Ordinal);
                    var index4 = temp.LastIndexOf("-", StringComparison.Ordinal);

                    course.WeekRange[0] = int.Parse(temp.Substring(1, index1 - 1));
                    course.WeekRange[1] = int.Parse(temp.Substring(index1 + 1, index2 - index1 - 1));
                    course.TimeRange[0] = int.Parse(temp.Substring(index2 + 2, index4 - index2 - 2));
                    course.TimeRange[1] = int.Parse(temp.Substring(index4 + 1, index3 - index4 - 1));
                    course.TimeRange[1] = course.TimeRange[1] - course.TimeRange[0] + 1;

                    curriculumInfo.Courses.Add(course);
                }
            }

            return curriculumInfo;
        }
    }
}