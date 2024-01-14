using BepInEx;
using BepInEx.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Controllers;
using HttpHandler;
using System.Net.Sockets;
using System.Collections.Specialized;

//插件描述特性 分别为 插件ID 插件名字 插件版本(必须为数字)
[BepInPlugin("vm.mba.plugin.showoffwebapi", "ShowOff_WebAPI", "1.2")]
public class ShowOffWebAPI : BaseUnityPlugin //继承BaseUnityPlugin
{
    // 创建配置文件
    ConfigEntry<int> webApiPort;
    ConfigEntry<string> accessToken;
    private HttpListener listener = new HttpListener();
    private Thread httpThread = null;


    void Awake()
    {
        Logger.LogInfo("WebAPI has been successfully loaded!");
    }
    void Start()
    {
        webApiPort = Config.Bind<int>("WebAPI", "Port", 42062, "The port used by WebAPI."); // 默认端口号为某人直播间
        accessToken = Config.Bind<string>("WebAPI", "AccessToken", "", "WebAPI authentication requires AccessToken for each call to private APIs, which must match the configuration file."); // 简单的鉴权方式

        if (webApiPort.Value > 65535 || webApiPort.Value < 0) // 如果配置文件端口号不合法
        {
            Logger.LogError("The port number in the configuration file is invalid. The WebAPI will not be started. Please check the configuration file.");
            Stop();
            return;
        }
        listener.Prefixes.Add(string.Format("http://+:{0}/", webApiPort.Value)); // 设置Web(HTTP)服务器地址 端口号
        try
        {
            listener.Start(); // 服务器！启动！
        }
        catch (SocketException exception)
        {
            Logger.LogError("Port already in use. Please choose another port for the plugin.");
            Stop();
            return;
        }
        Logger.LogInfo(string.Format("Listening on port {0}.", webApiPort.Value));
        httpThread = new Thread(() => // 创建新线程避免阻塞游戏主线程运行
        {
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                ThreadPool.QueueUserWorkItem((state) => ListenerCallback(context));
            }
        });
        httpThread.IsBackground = true;
        httpThread.Start();
    }

    void Stop() // 关闭事件，可以不需要，但我想文明一些
    {
        httpThread.Abort();
        listener.Stop();
    }


    private void ListenerCallback(HttpListenerContext context)
    {
        JsonResult result = null;
        HttpListenerRequest request = context.Request;
        Logger.LogInfo(string.Format("Received request from: {0}", request.RemoteEndPoint.Address));
        HttpListenerResponse response = context.Response;
        context.Response.Headers["Server"] = string.Format("{0}/{1}", Info.Metadata.Name, Info.Metadata.Version);
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*"); // 允许Javascript跨域访问
        context.Response.Headers.Add("ContentType", "application/json");
        response.ContentEncoding = Encoding.UTF8; // 防止浏览器认为页面为GBK
        string rawURL = context.Request.RawUrl;
        string path = rawURL.Split('?')[0];

        if (rawURL == "/favicon.ico")
        {
            response.StatusCode = 204;
            response.Close();
            return;
        }

        Controller[] controllers = {
                new GetStatus(),
                new SetContent(),
                new SetLevel()
            };


        foreach (var controller in controllers) // 公开接口的处理
        {
            if (controller.requestPath == path)
            {
                switch (controller.type)
                {
                    case APIType.Public:
                        result = handlePublicRequest(controller, request.QueryString);
                        break;
                    default: // 默认类型为私有，十分霸道
                        result = handlePrivateRequest(controller, request.QueryString);
                        break;
                }
            }
        }

        // Logger.LogInfo(responseString);
        string responseString = result == null ? JsonResult.NotFound("Not Found").ToJson() : result.ToJson();  // 结束返回文本
        responseString += "\r\n";
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentType = "text/html";
        response.ContentLength64 = buffer.Length;
        Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
    }

    BepInPlugin GetMetaData()
    {
        return Info.Metadata;
    }

    JsonResult handlePublicRequest(Controller controller, NameValueCollection parameters)
    {
        return controller.Get(parameters, GetMetaData());
    }

    JsonResult handlePrivateRequest(Controller controller, NameValueCollection parameters)
    {
        if (string.IsNullOrEmpty(accessToken.Value) || (parameters["AccessToken"] == accessToken.Value))
        {
            // 私有接口的处理逻辑
            return controller.Get(parameters, GetMetaData());
        }
        else
        {
            return JsonResult.Failed("invalid_token");
        }
    }
}