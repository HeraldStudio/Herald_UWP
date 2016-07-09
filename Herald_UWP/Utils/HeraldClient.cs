using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            bool isWrongResult = false;

            // 因为服务器的问题，偶尔会返回错误信息而不是Json数据，多试几次就好了
            do
            {
                string resultStr = await client.Post(url, requestContent);
                try
                {
                    resultObj = JObject.Parse(resultStr);
                }
                catch (JsonReaderException)
                {
                    isWrongResult = true;
                }
            } while (isWrongResult);

            // 处理返回的code，对于错误的进一步处理还没写，暂时作为异常抛出
            int code = resultObj["code"].Value<int>();
            switch(code)
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
            GPA gpaInfo = new GPA();

            return gpaInfo;
        }
    }
}