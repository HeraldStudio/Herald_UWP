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
        private HeraldHttpUtil client = new HeraldHttpUtil();   // 处理Http请求
        public string UUID { get; set; } = null;

        /// <summary>
        /// 获取授权，保存UUID
        /// </summary>
        /// <param name="userID">用户一卡通号</param>
        /// <param name="password">用户密码</param>
        /// <returns>授权是否成功</returns>
        public async Task<bool> Auth(string userID, string password)
        {
            // 请求所需的参数
            var requestContent = new HttpFormUrlEncodedContent(
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user", userID),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("appid", APIBasicInfo.APP_ID),
                });

            // 发起请求，结果即为UUID，同时要接收异常
            try
            {
                Task<string> responseContent = client.Post(APIBasicInfo.AUTH, requestContent);
                UUID = await responseContent;
            }
            catch (Exception)
            {
                return false;
            }

            // 把UUID也存到localSettings里面，供以后用户直接使用
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["UUID"] = UUID;

            return true;
        }

        /// <summary>
        /// 获得最终可以直接用的数据对象
        /// </summary>
        /// <typeparam name="T">查询的项目类型，如一卡通Card、成绩GPA</typeparam>
        /// <param name="param">查询时可能会需要的参数</param>
        /// <param name="isRefresh">是不是刷新，刷新的话直接从网络获取数据</param>
        /// <returns>同T</returns>
        public async Task<T> QueryForData<T>(List<KeyValuePair<string, string>> param = null, bool isRefresh = false, bool enableCache = true)
        {
            // 根据查询的信息的类名字，获取API中对应的地址,这个类名字到后面还可以用来调用函数
            string APIName = typeof(T).Name;
            string address = (string)typeof(APIBasicInfo).GetField(APIName).GetValue(null);

            // 先从本地获取数据，如果没有本地数据则从服务器获取
            string resultStr = await FileSystem.Read(APIName + ".data");
            if (resultStr != null && resultStr != "" && !isRefresh && enableCache)
            {
                return JsonConvert.DeserializeObject<T>(resultStr);
            }
            else
            {
                // 通过类名字获取对应的Json处理函数
                MethodInfo method = typeof(HeraldClient).GetMethod("Get" + APIName);

                // 可能会出现异常，先不处理，抛给上一层
                try
                {
                    // 先获得Json对象，然后用Json处理函数处理为对应数据类的对象
                    JObject jObject = await QueryForJson(address, param);
                    T resultObj = (T)method.Invoke(this, new object[] { jObject });

                    // enableCache为true，则把数据也写入本地数据
                    if (enableCache)
                    {
                        string jsonStr = JsonConvert.SerializeObject(resultObj);
                        FileSystem.Write(APIName + ".data", jsonStr);
                    }
                    return resultObj;
                }
                catch (HeraldRequestException e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    throw e;
                }
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

            param.Add(new KeyValuePair<string, string>("uuid", UUID));
            var requestContent = new HttpFormUrlEncodedContent(param);

            // 先将获取到的Json直接转换为JObject用于预处理
            JObject resultObj = null;
            bool isWrongResult;

            // 因为服务器的问题，偶尔会返回错误信息而不是Json数据，多试几次就好了
            do
            {
                string resultStr = await client.Post(url, requestContent);
                try
                {
                    isWrongResult = false;
                    resultObj = JObject.Parse(resultStr);
                }
                catch (JsonReaderException)
                {
                    isWrongResult = true;
                }
            } while (isWrongResult);

            // 处理返回的code，对于错误的进一步处理还没写，暂时作为异常抛出
            int code = resultObj["code"].Value<int>();
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

        public GPA GetGPA(JObject json)
        {
            JArray gradeArray = (JArray)json["content"];    // 取出content，这是一个数组
            GPA gpaInfo = gradeArray[0].ToObject<GPA>();    // 将content的第一个项取出来，里边含有平均绩点等信息

            // 移去content的第一个项，对剩余的项用Linq分组
            gradeArray.RemoveAt(0);
            var query = from item in gradeArray
                        group item by item["semester"].Value<string>() into g
                        orderby g.Key descending
                        select new { GroupName = g.Key, Items = g };

            // 分组之后的信息放到GPA对应的Semesters里
            foreach (var g in query)
            {
                var info = new GPASemester();
                info.Semester = g.GroupName;                
                foreach (var item in g.Items)
                {
                    info.Grades.Add(item.ToObject<GPAGrade>());
                }
                gpaInfo.Semesters.Add(info);
            }

            return gpaInfo;
        }

        public Card GetCard(JObject json)
        {
            Card cardInfo = json["content"].ToObject<Card>();
            JArray detailArray = (JArray)json["content"]["detial"];

            // 分割date为日期和时间，并处理type为“银行转帐”时system的空白
            foreach (JObject detail in detailArray)
            {
                string[] tempData;
                tempData = detail["date"].Value<string>().Split(' ');

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
                                    float result = cardDetail["price"].Value<float>();
                                    return result > 0 ? result : 0;
                                }),
                                // 将price中的负数取相反数相加作为支出
                                Outcome = g.Sum(cardDetail =>
                                {
                                    float result = cardDetail["price"].Value<float>();
                                    return result < 0 ? -result : 0;
                                })
                            },
                            Items = g
                        };

            // 将分组后的信息放到Card对应的CardDailys里
            foreach (var g in query)
            {
                var info = new CardDaily();
                info.Date = g.GroupInfo.Date;
                info.Income = g.GroupInfo.Income;
                info.Outcome = g.GroupInfo.Outcome;
                
                foreach (var item in g.Items)
                {
                    info.CardDailyDetails.Add(item.ToObject<CardDailyDetail>());
                }
                cardInfo.CardDailys.Add(info);
            }

            return cardInfo;
        }
    }
}