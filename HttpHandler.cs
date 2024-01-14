using System.Collections.Specialized;
using BepInEx;
using UnityEngine;

namespace HttpHandler
{
    internal abstract class Controller
    {

        public abstract string requestPath { get; }
        public abstract APIType type { get; }
        public abstract JsonResult Get(NameValueCollection parameters, BepInPlugin bepInPlugin);

    }

    class JsonResult
    {
        public int status_code;
        public string message;
        public object data;

        public static JsonResult OK(string message, object data = null)
        {
            return CreateResult(200, message, data);
        }

        public static JsonResult Failed(string message, object data = null)
        {
            return CreateResult(403, message, data);
        }

        public static JsonResult NotFound(string message, object data = null)
        {
            return CreateResult(404, message, data);
        }

        private static JsonResult CreateResult(int statusCode, string message, object data = null)
        {
            JsonResult result = new JsonResult
            {
                status_code = statusCode,
                message = message,
                data = data
            };
            return result;
        }

        public string ToJson()
        {
            string noDataJson = JsonUtility.ToJson(this);
            string dataJson = (data != null) ? JsonUtility.ToJson(data) : "\"\"";
            return $"{noDataJson.Substring(0, noDataJson.Length - 1)},\"data\":{dataJson}}}";
        }
    }
    enum APIType
    {
        Public,
        Private
    }
}
