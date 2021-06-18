可以看到项目里已经完成了一个聊天平台机器人（QQ）的环境，如果你需要使用该机器人的话，请先尝试编译后，得到了控制台项目的 EXE 文件和必需的 DLL 文件，然后使用。下面介绍一下具体的使用方式。

## 第一步：编译项目 `Sudoku.Bot` 和 `Sudoku.Bot.Console`

首先找到项目里的 `Sudoku.Bot` 和 `Sudoku.Bot.Console`，然后开始编译程序。

![](https://images.gitee.com/uploads/images/2021/0302/095926_a4883373_1449374.png "1.png")


编译完成后，还不能启动程序。因为我们需要对接必需的 DLL。

## 第二步：安装 Mirai 相关的 JAR 包

显然，我们为了对接我们的项目到底层的处理，是需要辅助工具依托的，这个框架叫 Mirai，可以通过这个方式来进行聊天平台的对接。

这三个文件是你需要下载的，请前往[这里](https://github.com/mamoe/mirai/blob/dev/docs/README.md)查看相关文档。

这里你需要下载的文件有：

* `mirai-console-1.0-M4-all.jar`
* `mirai-console-pure-1.0-M4-all.jar`
* `mirai-core-qqandroid-1.2.3-all.jar`
* `mirai-api-http-v1.8.4.jar`（这个要放在 `plugins` 文件夹下）
* `mirai-native-1.9.2.jar`（同上，也要放在 `plugins` 文件夹下）

![](https://images.gitee.com/uploads/images/2021/0302/101142_919bb368_1449374.png "2-1.png")

![](https://images.gitee.com/uploads/images/2021/0302/101148_d5305c30_1449374.png "2-2.png")

## 第三步：创建 `settings.yml` 文件

在你编译后的文件夹（`bin/Debug/net5.0`）下面，创建 `config` 文件夹，并在此文件夹里创建 `MiraiApiHttp` 文件夹，并再次在此文件夹里创建 `settings.yml` 文件。

创建的模板如下：

```yml
## 该配置为全局配置，对所有Session有效

# 可选，默认值为0.0.0.0
host: '0.0.0.0'
port: 8080
authKey: 1234567890

# 可选，缓存大小，默认4096.缓存过小会导致引用回复与撤回消息失败
cacheSize: 4096

# 可选，是否开启websocket，默认关闭，建议通过Session范围的配置设置
enableWebsocket: true

# 可选，配置CORS跨域，默认为*，即允许所有域名
cors:
  - '*'

## 消息上报
report:
# 功能总开关
  enable: false
  # 群消息上报
  groupMessage:
    report: false
  # 好友消息上报
  friendMessage:
    report: false
  # 临时消息上报
  tempMessage:
    report: false
  # 事件上报
  eventMessage:
    report: false
  # 上报URL
  destinations: []
  # 上报时的额外Header
  extraHeaders: {}

## 心跳
heartbeat:
  # 功能总开关
  enable: false
  # 启动延迟
  delay: 1000
  # 心跳间隔
  period: 15000
  # 心跳上报URL
  destinations: []
  # 上报时的额外信息
  extraBody: {}
  # 上报时的额外头
  extraHeaders: {}
```

截图：

![](https://images.gitee.com/uploads/images/2021/0302/100745_0a4a5867_1449374.png "3.png")

特此说明，IP 地址保持不动，不需要修改。认证代码可以修改成一个固定的序列，比如参考示例里的 `1234567890` 就是一个可以使用的序列。不需要考虑太多，写就完事了。

## 最后一步：打开 `start.bat`

在成功安装 Mirai 之后，会带有一个 BAT 文件提供给我们启动项目。我们先需要启动该项目后，然后才能启动自己的插件项目。

在编译目录下，有一个 `start.bat` 文件，打开它。

![](https://images.gitee.com/uploads/images/2021/0302/101337_b6eead8b_1449374.png "4.png")

接着，我们会看到载入的信息：

![](https://images.gitee.com/uploads/images/2021/0302/101717_94723dd4_1449374.png "5.png")

然后根据图上给出的提示文字，在 `>` 字符处输入 `login 账号 密码`。然后就会登录成功。

然后，启动我们的插件项目。

![](https://images.gitee.com/uploads/images/2021/0302/101818_f4dc6418_1449374.png "6.png")

如看到此提示，那么就说明启动成功了。此时，你登录的账号就是你机器人的账号了，那么它就会自动通过你的电脑，为同学们服务了。

![](https://images.gitee.com/uploads/images/2021/0302/101930_5963b029_1449374.png "7.png")

启用帮助命令请使用 `！帮助`，即一个中文感叹号，和一个帮助文字。

![](https://images.gitee.com/uploads/images/2021/0302/102047_7fe7c4bf_1449374.png "8.png")

> 禁言和解除禁言功能已经完成，但因为是数独项目，因此我删掉了。