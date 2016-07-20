using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

// API地址、文件访问、自定义的异常类型
namespace Herald_UWP.Utils
{
    // API的URL和APPID
    internal static class ApiBasicInfo
    {
        public const string AppId = "5b45c345a764a6d35c905e1a70a590ba";

        // 授权相关的URL
        private const string AuthUrl = "http://www.heraldstudio.com/uc/";
        public const string Auth = AuthUrl + "auth";       // 授权
        public const string Check = AuthUrl + "check";     // 检查UUID有效性
        public const string Update = AuthUrl + "update";   // 更新用户信息
        public const string Deauth = AuthUrl + "deauth";   // 取消授权

        // 查询相关的URL
        private const string ApiUrl = "http://www.heraldstudio.com/api/";
        public const string Srtp = ApiUrl + "srtp";                // 查询SRTP
        public const string Curriculum = ApiUrl + "curriculum";    // 查询课程表
        public const string Gpa = ApiUrl + "gpa";                  // 查询绩点
        public const string Pe = ApiUrl + "pe";                    // 查询跑操
        public const string Pc = ApiUrl + "pc";                    // 查询跑操
        public const string PeDetail = ApiUrl + "pedetail";        // 查询跑操记录
        public const string Nic = ApiUrl + "nic";                  // 查询校园网信息
        public const string Card = ApiUrl + "card";                // 查询一卡通
        public const string Lecture = ApiUrl + "lecture";          // 查询人文讲座
        public const string Library = ApiUrl + "library";          // 查询图书馆借阅
        public const string Search = ApiUrl + "search";            // 图书搜索
        public const string Sidebar = ApiUrl + "sidebar";          // 查询课程列表

        public const string Term = ApiUrl + "term";                // 查询学期列表
        public const string Simsimi = ApiUrl + "simsimi";          // 调戏小猴
        public const string Renew = ApiUrl + "renew";              // 图书馆续借
    }

    // 文件访问
    public static class FileSystem
    {
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
        public static async void Delete(string fileName)
        {
            // 获取应用的本地文件夹，并获取相应文件
            var localFolder = ApplicationData.Current.LocalFolder;
            await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
        }
    }

    // 异常类型
    public class HeraldRequestException : Exception
    {
        public HeraldRequestException() { }
        public HeraldRequestException(string message) : base(message) { }
        public HeraldRequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}