using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Reflection;
using Windows.Storage;

namespace Herald_UWP.Utils
{
    // API的URL和APPID
    internal static class APIBasicInfo
    {
        public const string APP_ID = "5b45c345a764a6d35c905e1a70a590ba";

        // 授权相关的URL
        private const string AUTH_URL = "http://www.heraldstudio.com/uc/";
        public const string AUTH = AUTH_URL + "auth";       // 授权
        public const string CHECK = AUTH_URL + "check";     // 检查UUID有效性
        public const string UPDATE = AUTH_URL + "update";   // 更新用户信息
        public const string DEAUTH = AUTH_URL + "deauth";   // 取消授权

        // 查询相关的URL
        private const string API_URL = "http://www.heraldstudio.com/api/";
        public const string SRTP = API_URL + "srtp";                // 查询SRTP
        public const string Curriculum = API_URL + "curriculum";    // 查询课程表
        public const string GPA = API_URL + "gpa";                  // 查询绩点
        public const string PE = API_URL + "pe";                    // 查询跑操
        public const string PEDetail = API_URL + "pedetail";        // 查询跑操记录
        public const string NIC = API_URL + "nic";                  // 查询校园网信息
        public const string Card = API_URL + "card";                // 查询一卡通
        public const string Lecture = API_URL + "lecture";          // 查询人文讲座
        public const string Library = API_URL + "library";          // 查询图书馆借阅
        public const string Search = API_URL + "search";            // 图书搜索

        public const string TERM = API_URL + "term";                // 查询学期列表
        public const string SIDEBAR = API_URL + "sidebar";          // 查询课程列表
        public const string SIMSIMI = API_URL + "simsimi";          // 调戏小猴
        public const string RENEW = API_URL + "renew";              // 图书馆续借
    }

    public class HeraldClient
    {
        // 用来向小猴的服务器发起请求
        private HeraldHttpUtil client = new HeraldHttpUtil();

        // 登陆之后只会用到UUID所以只有这个变量
        public string UUID { get; set; } = null;

        // 授权函数
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

        // 查询函数
        public async Task<T> Query<T>(List<KeyValuePair<string, string>> param = null, bool isRefresh = false) where T:BaseType
        {
            // 根据查询的信息的封装类名称，获取API中对应的地址
            string APIName = typeof(T).Name;
            string address = (string)typeof(APIBasicInfo).GetField(APIName).GetValue(null);
            
            // 没有本地数据则从服务器获取
            string resultStr = await FileSystem.Read(APIName + ".data");

            if (resultStr != null && resultStr != "" && !isRefresh)
            {
                return FileSystem.ParseJson<T>(resultStr);
            }
            else
            {
                if(param == null)
                {
                    param = new List<KeyValuePair<string, string>>();
                }

                param.Add(new KeyValuePair<string, string>("uuid", UUID));
                var requestContent = new HttpFormUrlEncodedContent(param);

                // 返回不正确的信息就无脑重试
                T resultObj = null;
                while (resultObj == null || resultObj.code != 200)
                {
                    resultStr = await client.Post(address, requestContent);
                    resultObj = FileSystem.ParseJson<T>(resultStr);
                }

                // 写入本地数据，返回查询对象
                FileSystem.Write(APIName + ".data", resultStr);
                return resultObj;
            }
        }
    }

    public static class FileSystem
    {
        // 将Json格式的string转换为相应的封装类
        public static T ParseJson<T>(string jsonString)
        {
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                {
                    return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
                }
            }
            catch(Exception)
            {
                return default(T);
            }
        }

        // 写入本地数据
        public static async void Write(string fileName, string data)
        {
            // 获取应用的本地文件夹并创建文件，如果文件已存在则替换旧的
            var localFolder = ApplicationData.Current.LocalFolder;
            var localFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            // 写入文本
            using (var writeStream = await localFile.OpenStreamForWriteAsync())
            {
                using (var writer = new StreamWriter(writeStream))
                {
                    await writer.WriteAsync(data);
                }
            }
        }

        // 读取本地数据
        public static async Task<string> Read(string fileName)
        {
            try
            {
                // 获取应用的本地文件夹并获取相应文件
                var localFolder = ApplicationData.Current.LocalFolder;
                var localFile = await localFolder.GetFileAsync(fileName);

                // 读取文本
                using (var readStream = await localFile.OpenStreamForReadAsync())
                {
                    using (var reader = new StreamReader(readStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // 没找到指定的文件就返回null
                return null;
            }
        }

        // 删除本地数据
        public static async void Delete(String fileName)
        {
            // 获取应用的本地文件夹，并获取相应文件
            var localFolder = ApplicationData.Current.LocalFolder;
            var localFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        }
    }
}