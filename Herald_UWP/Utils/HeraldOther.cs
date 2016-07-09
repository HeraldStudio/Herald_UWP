using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

/// <summary>
/// API地址、文件访问、自定义的异常类型
/// </summary>
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
        private const string BASE_URL = "http://www.heraldstudio.com/api/";
        public const string SRTP = BASE_URL + "srtp";                // 查询SRTP
        public const string CURRICULUM = BASE_URL + "curriculum";    // 查询课程表
        public const string GPA = BASE_URL + "gpa";                  // 查询绩点
        public const string PE = BASE_URL + "pe";                    // 查询跑操
        public const string PE_DETAIL = BASE_URL + "pedetail";        // 查询跑操记录
        public const string NIC = BASE_URL + "nic";                  // 查询校园网信息
        public const string CARD = BASE_URL + "card";                // 查询一卡通
        public const string LECTURE = BASE_URL + "lecture";          // 查询人文讲座
        public const string LIBRARY = BASE_URL + "library";          // 查询图书馆借阅
        public const string SEARCH = BASE_URL + "search";            // 图书搜索

        public const string TERM = BASE_URL + "term";                // 查询学期列表
        public const string SIDEBAR = BASE_URL + "sidebar";          // 查询课程列表
        public const string SIMSIMI = BASE_URL + "simsimi";          // 调戏小猴
        public const string RENEW = BASE_URL + "renew";              // 图书馆续借
    }

    // 文件访问
    public static class FileSystem
    {
        // 将Json格式的string转换为相应的封装类
        public static T ParseJson<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        // 将对象转换为Json格式的string
        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
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
            catch (UnauthorizedAccessException)
            {
                // 还有一定概率碰到这个异常，没找到原因，有点迷
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

    // 异常类型
    public class HeraldRequestException : Exception
    {
        public HeraldRequestException() : base() { }
        public HeraldRequestException(string message) : base(message) { }
        public HeraldRequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}