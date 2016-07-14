using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Web.Http;

namespace Herald_UWP.Utils
{
    public class HeraldHttpUtil
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        // 处理Post请求的错误信息
        private async Task<string> PostHandler(Func<Task<string>> httpRequestFunc)
        {
            string responseBody = null;
            string exceptionBody = null;
            try
            {
                responseBody = await httpRequestFunc();
                _cts.Token.ThrowIfCancellationRequested();
            }
            catch (TaskCanceledException)
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                exceptionBody += e.Message;
                var dialog = new MessageDialog(exceptionBody, "接收到了异常");
                dialog.Commands.Add(new UICommand("确定"));
                await dialog.ShowAsync();
            }
            return responseBody;
        }

        //  发起Post请求，获取string类型的数据
        public async Task<string> Post(string resourceAddress, HttpFormUrlEncodedContent requestContent)
        {
            var responseContent = PostHandler(async () =>
            {
                var response = await _httpClient.PostAsync(new Uri(resourceAddress), requestContent).AsTask(_cts.Token);
                var responseBody = await response.Content.ReadAsStringAsync().AsTask(_cts.Token);
                return responseBody;
            });

            return await responseContent;
        }
    }
}