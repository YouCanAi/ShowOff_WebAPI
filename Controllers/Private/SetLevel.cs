using HttpHandler;
using System.Collections.Specialized;
using System;

namespace Controllers
{
    class SetLevel : Controller
    {
        public override string requestPath => "/set_xp";

        public override APIType type => APIType.Private;

        public override JsonResult Get(NameValueCollection parameters, ShowOffWebAPI showOffWebApi)
        {
            if (string.IsNullOrEmpty(parameters["value"]))
            {
                return JsonResult.Failed("Invalid request, 'value' field is missing.");
            }

            if (!int.TryParse(parameters["value"], out int xpValue))
            {
                return JsonResult.Failed("'value' should be a valid integer.");
            }

            if (!Enum.TryParse(parameters["type"], true, out XpRequestType xpType))
            {
                xpType = XpRequestType.Exp;
            }

            if (!Enum.TryParse(parameters["way"], true, out XpRequestWay xpWay))
            {
                xpWay = XpRequestWay.Add;
            }

            if (!bool.TryParse(parameters["toInput"], out bool refresh))
            {
                refresh = true;
            }

            SetXP(xpValue, xpType, xpWay, refresh);
            return JsonResult.OK(string.Format("{0} {1} completed.", xpType.ToString(), xpWay == XpRequestWay.Add ? "increase" : "setting"));
        }

        private void SetXP(int value, XpRequestType type = XpRequestType.Exp, XpRequestWay way = XpRequestWay.Add, bool refresh = true)
        {
            switch (type)
            {
                case XpRequestType.Exp:
                    SetXPExp(way, value, refresh);
                    break;
                case XpRequestType.Level:
                    SetXPLevel(way, value);
                    break;
            }
        }

        private void SetXPExp(XpRequestWay way, int value, bool refresh = true)
        {
            switch (way)
            {
                case XpRequestWay.Add:
                    挂机功能.单例.经验 += value;
                    break;
                case XpRequestWay.Set:
                    挂机功能.单例.经验 = value;
                    break;
            }

            if (refresh)
            {
                挂机功能.单例.刷新();
            }
        }

        private void SetXPLevel(XpRequestWay way, int value, bool refresh = true)
        {
            switch (way)
            {
                case XpRequestWay.Add:
                    挂机功能.单例.等级 += value;
                    break;
                case XpRequestWay.Set:
                    挂机功能.单例.等级 = value;
                    break;
            }

            if (refresh)
            {
                挂机功能.单例.刷新();
            }
        }
        enum XpRequestType
        {
            Level,
            Exp
        }

        enum XpRequestWay
        {
            Add,
            Set
        }
    }
}