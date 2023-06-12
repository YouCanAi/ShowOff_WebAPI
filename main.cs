using BepInEx;
using BepInEx.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ShowOffWebAPI
{
    //插件描述特性 分别为 插件ID 插件名字 插件版本(必须为数字)
    [BepInPlugin("vm.mba.plugin.showoffwebapi", "ShowOff_WebAPI", "1.0")]
    public class ShowOffWebAPI : BaseUnityPlugin //继承BaseUnityPlugin
    {
        // 创建配置文件
        ConfigEntry<int> webApiPort;
        ConfigEntry<string> accessToken;
        ConfigEntry<string> favicon_ico;
        private HttpListener listener = new HttpListener();
        private string repContent = 推送功能.单例.上次输入;

        void Awake()
        {
            Logger.LogInfo("WebAPI 成功被加载!");
        }
        void Start()
        {
            webApiPort = Config.Bind<int>("WebAPI", "Port", 42062, "WebAPI所使用端口"); // 默认端口号为某人直播间
            accessToken = Config.Bind<string>("WebAPI", "AccessToken", "", "WebAPI鉴权，每次调用API都需要AccessToken与配置文件一致"); // 简单的鉴权方式
            favicon_ico = Config.Bind<string>("WebAPI", "favicon_ico", "https://raw.githubusercontent.com/VictorModi/ShowOff_WebAPI/master/favicon.ico", "设定favicon，实现浏览器调用API时在标签栏显示图标，填入图标的URL");
            Logger.LogInfo(JsonUtility.ToJson(getStatus()));
            if (webApiPort.Value > 65535 || webApiPort.Value < 0) // 如果配置文件端口号不合法
            {
                Logger.LogInfo("配置文件的端口号不合法，WebAPI将不会被启动。请检查配置文件");
            }
            else
            {
                // Logger.LogInfo(pluName);
                // Logger.LogInfo(pluVer);
                // Logger.LogInfo(gameName);
                // Logger.LogInfo(gameVer);
                // Logger.LogInfo(推送功能.单例.上次输入);
                
                listener.Prefixes.Add(string.Format("http://+:{0}/", webApiPort.Value)); // 设置Web(HTTP)服务器地址 端口号

                listener.Start(); // 服务器！启动！

                Logger.LogInfo(string.Format("正在监听: http://+:{0}/", webApiPort.Value));
                Thread thread = new Thread(() => // 创建新线程避免阻塞主游戏运行
                {
                    HttpListenerContext context;
                    while (true)
                    {
                        context = listener.GetContext();
                        ThreadPool.QueueUserWorkItem((state) => ListenerCallback(context));
                    }
                });
                thread.IsBackground = true;
                thread.Start();
            }
        }

        void Stop() // 关闭事件，可以不需要，但我想文明一些
        {
            listener.Stop();
            Logger.LogInfo("WebAPI 被正常关闭!");
        }

        public StatusData getStatus() 
        {
            StatusData statusData = new StatusData();
            statusData.pluName = Info.Metadata.Name;
            statusData.pluVer = Info.Metadata.Version.ToString();
            statusData.gameName = Application.productName;
            statusData.gameVer = Application.version;
            statusData.curContent = 推送功能.单例.上次输入;
            return statusData;
        }

        private object ListenerCallback(HttpListenerContext context)
        {
            ResponseContent responseClass;
            // Logger.LogInfo("收到请求");
            // HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            Logger.LogInfo(string.Format("收到请求，来自: {0}", request.RemoteEndPoint.Address));
            string repAccessToken = request.QueryString["AccessToken"]; // 获取AccessToken字段
            HttpListenerResponse response = context.Response;
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*"); // 允许Javascript跨域访问
            context.Response.Headers.Add("ContentType", "application/json");
            response.ContentEncoding = Encoding.UTF8; // 防止浏览器认为页面为GBK
            string rawURL = context.Request.RawUrl;
            string path = rawURL.Split('?')[0];
            // Logger.LogInfo(string.Format("请求地址: {0}", rawURL));
            Logger.LogInfo(string.Format("请求路径: {0}", path));
            if (rawURL == "/favicon.ico") {
                response.StatusCode = 302;
                response.AddHeader("Location", favicon_ico.Value);
                response.Close();
                return null;
            }
            if (string.IsNullOrEmpty(accessToken.Value) || (accessToken.Value == repAccessToken)) // 如果设置过accessToken则验证，没设置过就当认证通过
            { // 鉴权成功

                // 2023-05-28
                // 妈的，懒得调用任何Json库
                // 我他妈直接手拼Json

                // 2023-06-09
                // 我操，原来Unity自带一个Json库

                Logger.LogInfo("鉴权成功");
                if (path == "/")
                {
                    // 处理根路径请求
                    response.StatusCode = 200;
                    // responseString = string.Format(/*lang=json*/ "{{\"success\":0,\"message\":\"\",\"data\":{{\"plugName\":\"{0}\",\"plugVer\":\"{1}\",\"gameName\":\"{2}\",\"gameVer\":\"{3}\",\"curContent\":\"{4}\"}}}}", pluName, pluVer.ToString(), gameName, gameVer, 推送功能.单例.上次输入);
                    responseClass = new ResponseContent(0, "", getStatus());
                    Logger.LogInfo("信息获取完成: " + JsonUtility.ToJson(getStatus()));
                    // 显示插件及游戏信息                
                }
                else if (path == "/push") // 路径push(推送)
                {
                    // 处理推送路径请求
                    repContent = request.QueryString["Content"];
                    if (string.IsNullOrEmpty(repContent)) // push路径，无Content字段
                    {
                        response.StatusCode = 403;
                        // responseString = /*lang=json*/ "{\"success\":-1,\"message\":\"非法请求，'Content'字段不存在。\",\"data\":{}}";
                        responseClass = new ResponseContent(-1, "非法请求，'Content'字段不存在。");
                    }
                    else // push路径，请求合法
                    {
                        response.StatusCode = 200;
                        RichPresence.单例.显示内容 = repContent;
                        RichPresence.单例.刷新Steam富状态();
                        推送功能.单例.上次输入 = repContent;
                        Logger.LogInfo("修改内容: " + 推送功能.单例.上次输入);
                        // responseString = /*lang=json*/ "{\"success\":0,\"message\":\"\",\"data\":{}}";
                        responseClass = new ResponseContent(0, "修改成功");
                    }
                }
                else
                {
                    // 处理其他不存在的路径请求
                    response.StatusCode = 403;
                    // responseString = /*lang=json*/ "{\"success\":-1,\"message\":\"非法请求，不存在的路径。\",\"data\":{}}";
                    responseClass = new ResponseContent(0, "非法请求，不存在的路径。");
                }
            }
            else // 鉴权失败
            {
                response.StatusCode = 403;
                // responseString = /*lang=json*/ "{\"success\":-1,\"message\":\"非法请求，鉴权失败，请检查accessToken。\",\"data\":{}}";
                responseClass = new ResponseContent(0, "非法请求，鉴权失败，请检查accessToken。");
            }
            // Logger.LogInfo(responseString);
            string responseString = responseClass._2json() + "\r\n"; // 结束返回文本
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            return null;
        }
    }
    public class ResponseContent // 响应数据 应转化为json
    {
        public int success;
        public string message;
        public object data;

        public ResponseContent(int success, string message, object data = null)
        {
            this.success = success;
            this.message = message;
            if (data == null)
            {
                this.data = new {};
            }
            else
            {
                this.data = data;
            }
        }
        public string _2json()
        {
            
            string noDataJson = JsonUtility.ToJson(this);
            //return noDataJson;
            return  noDataJson.Substring(0, noDataJson.Length - 1) + ",\"data\":" +  JsonUtility.ToJson(this.data) + "}";

        }
    }
    public class StatusData
    {
        public string gameName;
        public string gameVer;
        public string pluName;
        public string pluVer;
        public string curContent;

    }
}
