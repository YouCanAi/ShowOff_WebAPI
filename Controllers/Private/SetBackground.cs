using HttpHandler;
using System.Collections.Specialized;

namespace Controllers
{
    class SetBackground : Controller
    {
        // TODO: 可直接设置背景的API
        public override string requestPath => "/set_background";

        public override APIType type => APIType.Private;

        public override JsonResult Get(NameValueCollection parameters, ShowOffWebAPI showOffWebApi)
        {
            return JsonResult.Failed("Incomplete Controller.");
        }
    }
}
