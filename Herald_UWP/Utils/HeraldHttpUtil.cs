using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using System.IO;

namespace Herald_UWP.Utils
{
    class HeraldHttpUtil
    {
        private HttpClient httpClient = new HttpClient();
        private CancellationTokenSource cts = new CancellationTokenSource();

        public HeraldHttpUtil() { }

        // 处理Post请求的错误信息
        private async Task<string> PostHandler(Func<Task<string>> httpRequestFunc)
        {
            string responseBody = null;
            string exceptionBody = null;
            try
            {
                responseBody = await httpRequestFunc();
                cts.Token.ThrowIfCancellationRequested();
            }
            catch (TaskCanceledException)
            {
                exceptionBody = "请求被取消";
                throw new Exception();
            }
            catch (Exception e)
            {
                exceptionBody += e.Message;
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "接收到了异常",
                    Content = exceptionBody,
                    FullSizeDesired = false,
                    PrimaryButtonText = "好吧",
                };
                await dialog.ShowAsync();
            }
            return responseBody;
        }

        //  发起Post请求，获取string类型的数据
        public async Task<string> Post(string resourceAddress, HttpFormUrlEncodedContent requestContent)
        {
            Task<string> responseContent = PostHandler(async () =>
            {
                string responseBody;
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(resourceAddress), requestContent).AsTask(cts.Token);
                responseBody = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                return responseBody;
            });

            return await responseContent;
        }
    }
}