using HttpHandler;
using System.Collections.Specialized;
using BepInEx.Logging;

namespace Controllers
{
    class SetContent : Controller
    {
        public override string requestPath => "/set_content";

        public override APIType type => APIType.Private;

        public override JsonResult Get(NameValueCollection parameters, ShowOffWebAPI showOffWebApi)
        {
            if (string.IsNullOrEmpty(parameters["Content"]))
            {
                return JsonResult.Failed("Invalid request, 'Content' field is missing.");
            }
            showOffWebApi.Log(LogLevel.Info, string.Format("Content set to {0}", parameters["Content"]));
            RichPresence.单例.显示内容 = parameters["Content"];
            RichPresence.单例.刷新Steam富状态();
            推送功能.单例.上次输入 = parameters["Content"];
            return JsonResult.OK("Modification successful!");
        }
    }
}