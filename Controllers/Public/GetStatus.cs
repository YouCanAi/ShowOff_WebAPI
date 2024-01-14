using HttpHandler;
using System.Collections.Specialized;
using BepInEx;
using UnityEngine;

namespace Controllers
{
    class GetStatus : Controller
    {
        public override string requestPath => "/";

        public override APIType type => APIType.Public;

        public override JsonResult Get(NameValueCollection parameters, BepInPlugin bepInPlugin)
        {
            return JsonResult.OK("", StatusData.GetStatusData(bepInPlugin));
        }

        private class StatusData
        {
            public string gameName;
            public string gameVer;
            public string pluName;
            public string pluVer;
            public string curContent;
            public int curLevel;
            public int curExp;

            public static StatusData GetStatusData(BepInPlugin bepInPlugin)
            {
                return new StatusData
                {
                    gameName = Application.productName,
                    gameVer = Application.version,
                    pluName = bepInPlugin.Name,
                    pluVer = bepInPlugin.Version.ToString(),
                    curContent = 推送功能.单例.上次输入,
                    curLevel = 挂机功能.单例.等级,
                    curExp = 挂机功能.单例.经验
                };
            }
        }
    }
}
