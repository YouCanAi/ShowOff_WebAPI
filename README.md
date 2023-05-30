# ShowOff_WebAPI
一个为[《闲置显摆器》](https://store.steampowered.com/app/2392060)添加WebAPI的 [BeplnEx](https://github.com/BepInEx/BepInEx) 插件


![demo](https://raw.githubusercontent.com/VictorModi/ShowOff_WebAPI/main/demo.png)
## 如何安装
 1. 在 [Steam](https://store.steampowered.com/) 购买下载[《闲置显摆器》](https://store.steampowered.com/app/2392060)。
 2. 在 [BeplnEx](https://github.com/BepInEx/BepInEx) 的 [releases页面](https://github.com/BepInEx/BepInEx/releases) 下载 [最新稳定版](https://github.com/BepInEx/BepInEx/releases/latest) 本的 [BeplnEx](https://github.com/BepInEx/BepInEx) 压缩包。
 3. 在 [本项目的releases页面](https://github.com/VictorModi/releases)下载 `ShowOff_WebAPI.dll`。
 4. 将 [BeplnEx](https://github.com/BepInEx/BepInEx) 的压缩包解压至 [《闲置显摆器》](https://store.steampowered.com/app/2392060) 的游戏根目录，然后在 `BepInEx` 文件夹里创建 `plugins` 文件夹(如果已经有该文件夹则不用管这条)。
 5. 将 `ShowOff_WebAPI.dll` 放入 `plugins` 文件夹。
 6. 在 [Steam](https://store.steampowered.com/) 启动游戏，启动完成后打开浏览器输入 http://127.0.0.1:42062 出现游戏版本号及插件版本号即代表安装成功。
## 如何使用
### 显示相关信息
- 终结点: `/`
- 请求方式: `GET`

*提示 : 该 API 无需参数*

`cURL -L http://127.0.0.1:42062/`

```Json
{
    "success":0,
    "message":"",
    "data":{
        "plugName":"ShowOff_WebAPI",
        "plugVer":"1.0",
        "gameName":"Idle ShowOff",
        "gameVer":"0.1",
        "curContent":"😭"
    }
}
```
<table>
  <tr>
    <td>字段名</td>
    <td>数据类型</td>
    <td>说明</td>
  </tr>
  <tr>
    <td>success</td>
    <td>int</td>
    <td>为 `0` 时表示请求成功，`-1`则表示请求失败。</td>
  </tr>
  <tr>
    <td>message</td>
    <td>string</td>
    <td>当请求失败时将会在此字段输出原因。</td>
  </tr>
  <tr>
    <td>plugName</td>
    <td>string</td>
    <td>插件名称。</td>
  </tr>
  <tr>
    <td>plugVer</td>
    <td>string</td>
    <td>插件版本号。</td>
  </tr>
  <tr>
    <td>gameName</td>
    <td>string</td>
    <td>游戏名称。</td>
  </tr>
  <tr>
    <td>gameVer</td>
    <td>string</td>
    <td>游戏版本号。</td>
  </tr>
  <tr>
    <td>curContent</td>
    <td>string</td>
    <td>显示当前推送文本。</td>
  </tr>
</table>

### 推送
- 请求方式: `GET`
- 终结点: `/push`
<table>
  <tr>
    <td>字段名</td>
    <td>数据类型</td>
    <td>说明</td>
  </tr>
  <tr>
    <td>Content</td>
    <td>string</td>
    <td>传递你需要的字符串至显摆器将其显摆。需要注意的是请求WebAPI时填写该项需要注意大小写，因为我喜欢搞特殊ο(=•ω＜=)ρ⌒☆。</td>
  </tr>
</table>

`cURL -L http://127.0.0.1:42062/push?Content=😭`
```Json
{
    "success":0,
    "message":"",
    "data":{
    }
}
```
<table>
  <tr>
    <td>字段名</td>
    <td>数据类型</td>
    <td>说明</td>
  </tr>
  <tr>
    <td>success</td>
    <td>int</td>
    <td>为 `0` 时表示请求成功，`-1`则表示请求失败。</td>
  </tr>
  <tr>
    <td>message</td>
    <td>string</td>
    <td>当请求失败时将会在此字段输出原因。</td>
  </tr>
</table>

## 配置
 - *配置文件放在 `游戏根目录\BepInEx\config\vm.mba.plugin.showoffwebapi.cfg`*
 <!-- 这里我填yaml是瞎写的 不用管 这个应该不是yaml吧 -->
```yaml
## Settings file was created by plugin ShowOff_WebAPI v1.0
## Plugin GUID: vm.mba.plugin.showoffwebapi

[WebAPI]

## WebAPI所使用端口
# Setting type: Int32
# Default value: 42062
Port = 42062

## WebAPI鉴权，每次调用API都需要AccessToken与配置文件一致
# Setting type: String
# Default value: 
AccessToken = 
```
<table>
  <tr>
    <td>配置</td>
    <td>数据类型</td>
    <td>说明</td>
  </tr>
  <tr>
    <td>Port</td>
    <td>int</td>
    <td>WebAPI的端口号，默认42062，按需更改。</td>
  </tr>
  <tr>
    <td>AccessToken</td>
    <td>string</td>
    <td>当填写这项配置后，每次请求WebAPI都会核对客户端输入的AccessToken与这项配置是否相符，若不相符会被直接作为非法请求。若你逆天程度高到需要公网部署，务必在里边填点能记住的东西。需要注意的是请求WebAPI时填写该项需要注意大小写，因为我喜欢搞特殊ο(=•ω＜=)ρ⌒☆。</td>
  </tr>
</table>