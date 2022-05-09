using CommunicationIntents = Sudoku.Bot.Communication.Intents;

namespace Sudoku.Bot.Communication;

/// <summary>
/// <para>Defines a bot client instance.</para>
/// <para>
/// The optional properties are:
/// <list type="bullet">
/// <item>BotAccessInfo - 机器人鉴权登陆信息,见<see cref="Identity"/></item>
/// <item>SadboxGuildId - 指定用于调试机器人的沙箱频道(DebugBot=true时有效)</item>
/// <item>DebugBot - 指定机器人运行的模式[true:测试; false:正式]；默认值=false</item>
/// <item>Info - 机器人的 <see cref="User"/> 信息(在机器人鉴权通过后更新)；默认值=null</item>
/// <item>Members - 自动记录机器人在各频道内的身份组信息</item>
/// <item>ReportApiError - 向前端消息发出者报告API错误[true:报告;false:静默]；默认值=false</item>
/// <item>SandBox - 机器人调用API的模式[true:沙箱;false:正式]；默认值=false</item>
/// <item>ApiOrigin - (只读) 获取机器人当前使用的ApiUrl</item>
/// <item>
/// Intents - 订阅频道事件,详见:<see cref="Intent"/>；默认值=(GUILDS|GUILD_MEMBERS|AT_MESSAGES|GUILD_MESSAGE_REACTIONS)
/// </item>
/// <item>Guilds - 机器人已加入的频道列表</item>
/// </list>
/// </para>
/// </summary>
public partial class BotClient
{
	/// <summary>
	/// Initializes a <see cref="BotClient"/> instance via the identity instance and two <see cref="bool"/>
	/// values indicating whether the API is based on sandbox, and whether the client will report errors on APIs
	/// during running respectively.
	/// </summary>
	/// <param name="identity">The identity instance.</param>
	/// <param name="sandBoxApi">Indicates whetehr the API is based on sandbox.</param>
	/// <param name="reportApiError">Indicates whether the client will report errors to foreground.</param>
	public BotClient(Identity identity, bool sandBoxApi = false, bool reportApiError = false)
	{
		(BotAccessInfo, ApiOrigin, ReportApiError) = (identity, sandBoxApi ? SandboxApi : ReleaseApi, reportApiError);

		HeartBeatTimer.Elapsed += async (_, _) =>
		{
			var rootElement = JsonDocument.Parse($$"""{"op":{{(int)Opcode.Heartbeat}}}""").RootElement;
			await ExcuteCommandAsync(rootElement);
		};
	}


	/// <summary>
	/// 向前端指令发出者报告API错误
	/// <para>注：该属性作为 <see cref="Sender.ReportError"/> 的默认值使用</para>
	/// </summary>
	public bool ReportApiError { get; set; }

	/// <summary>
	/// 会话分片id
	/// <para>
	/// 分片是按照频道id进行哈希的，同一个频道的信息会固定从同一个链接推送。<br/>
	/// 详见 <see href="https://bot.q.qq.com/wiki/develop/api/gateway/shard.html">Shard机制</see>
	/// </para>
	/// </summary>
	public int ShardId { get; set; } = 0;

	/// <summary>
	/// 机器人接口域名
	/// </summary>
	public string ApiOrigin { get; init; }

	/// <summary>
	/// 鉴权信息
	/// <para>可在这里查询 <see href="https://bot.q.qq.com/#/developer/developer-setting">QQ机器人开发设置</see></para>
	/// </summary>
	public Identity BotAccessInfo { get; set; }

	/// <summary>
	/// 此次连接所需要接收的事件
	/// <para>具体可参考 <see href="https://bot.q.qq.com/wiki/develop/api/gateway/intents.html">事件订阅</see></para>
	/// </summary>
	public Intent Intents { get; set; } = CommunicationIntents.PublicDomain;

	/// <summary>
	/// 机器人用户信息
	/// </summary>
	public User Info { get; private set; } = new();

	/// <summary>
	/// 缓存机器人已加入的频道
	/// </summary>
	public ConcurrentDictionary<string, Guild> Guilds { get; private set; } = new();

	/// <summary>
	/// 保存机器人在各频道内的角色信息
	/// <para>
	/// string - GUILD_ID<br/>
	/// Member - 角色信息
	/// </para>
	/// </summary>
	public ConcurrentDictionary<string, Member?> Members { get; private set; } = new();

	/// <summary>
	/// 消息过滤器
	/// <para>当返回值为True时，该消息将被拦截并丢弃</para>
	/// </summary>
	public Func<Sender, bool>? MessageFilter { get; set; }

	/// <summary>
	/// 断线重连状态标志
	/// </summary>
	private bool IsResume { get; set; } = false;

	/// <summary>
	/// Socket客户端收到的最新的消息的s，如果是第一次连接，传null
	/// </summary>
	private int WebSocketLastSeq { get; set; } = 1;

	/// <summary>
	/// Socket客户端存储的SessionId
	/// </summary>
	private string? WebSoketSessionId { get; set; }

	/// <summary>
	/// Socket客户端
	/// </summary>
	private ClientWebSocket WebSocketClient { get; set; } = new();

	/// <summary>
	/// Socket心跳定时器
	/// </summary>
	private Timer HeartBeatTimer { get; set; } = new();

	/// <summary>
	/// 会话分片信息
	/// </summary>
	private WebSocketLimit? GateLimit { get; set; }

	/// <summary>
	/// 缓存消息
	/// <para>
	/// string - UserId<br/>
	/// List&lt;Message&gt; - 用户消息列表
	/// </para>
	/// </summary>
	private ConcurrentDictionary<string, List<Message>> StackMessage { get; set; } = new();


	/// <summary>
	/// 返回SDK相关信息
	/// <para>
	/// 框架名称_版本号<br/>
	/// 代码仓库地址<br/>
	/// 版权信息<br/>
	/// <em>作者夹带的一点私货</em>
	/// </para>
	/// </summary>
	public static string SDK => $"{InfoSdk.Name}_{InfoSdk.Version}\n{InfoSdk.GitHTTPS}\n{InfoSdk.Copyright}";

	/// <summary>
	/// Indicates the time that records the bot being running.
	/// </summary>
	public static DateTime StartTime { get; private set; } = DateTime.Now;

	/// <summary>
	/// 正式环境
	/// </summary>
	private static string ReleaseApi => "https://api.sgroup.qq.com";

	/// <summary>
	/// 沙箱环境
	/// <para>
	/// 沙箱环境只会收到测试频道的事件，且调用openapi仅能操作测试频道
	/// </para>
	/// </summary>
	private static string SandboxApi => "https://sandbox.api.sgroup.qq.com";


	/// <summary>
	/// WebSocketClient连接后触发
	/// <para>等同于 WebSocket.OnOpen 事件</para>
	/// </summary>
	public event Action<BotClient>? OnWebSocketConnected;

	/// <summary>
	/// WebSocketClient关闭后触发
	/// </summary>
	public event Action<BotClient>? OnWebSocketClosed;

	/// <summary>
	/// WebSocketClient发送数据前触发
	/// </summary>
	public event Action<BotClient, string>? OnWebSoketSending;

	/// <summary>
	/// WebSocketClient收到数据后触发
	/// </summary>
	public event Action<BotClient, string>? OnWebSocketReceived;

	/// <summary>
	/// 收到服务端推送的消息时触发
	/// </summary>
	public event Action<BotClient, JsonElement>? OnDispatch;

	/// <summary>
	/// 客户端发送心跳或收到服务端推送心跳时触发
	/// </summary>
	public event Action<BotClient, JsonElement>? OnHeartbeat;

	/// <summary>
	/// 客户端发送鉴权时触发
	/// </summary>
	public event Action<BotClient, JsonElement>? OnIdentify;

	/// <summary>
	/// 客户端恢复连接时触发
	/// </summary>
	public event Action<BotClient, JsonElement>? OnResume;

	/// <summary>
	/// 服务端通知客户端重新连接时触发
	/// </summary>
	public event Action<BotClient, JsonElement>? OnReconnect;

	/// <summary>
	/// 当identify或resume的时候，参数错误的时候触发
	/// </summary>
	public event Action<BotClient, JsonElement>? OnInvalidSession;

	/// <summary>
	/// 当客户端与网关建立ws连接的时候触发
	/// </summary>
	public event Action<BotClient, JsonElement>? OnHello;

	/// <summary>
	/// 客户端发送心跳被服务端接收后触发
	/// </summary>
	public event Action<BotClient, JsonElement>? OnHeartbeatACK;

	/// <summary>
	/// 鉴权连接成功后触发
	/// <para>注:此时获取的User对象只有3个属性 {id,username,bot}</para>
	/// </summary>
	public event Action<User>? OnReady;

	/// <summary>
	/// 恢复连接成功后触发
	/// </summary>
	public event Action<BotClient, JsonElement>? OnResumed;

	/// <summary>
	/// 频道信息变更后触发
	/// <para>
	/// 机器人加入频道, 频道资料变更, 机器人退出频道<br/>
	/// BotClient - 机器人对象<br/>
	/// Guild - 频道对象<br/>
	/// string - 事件类型（GUILD_ADD | GUILD_UPDATE | GUILD_REMOVE）
	/// </para>
	/// </summary>
	public event Action<BotClient, Guild, string>? OnGuildMsg;

	/// <summary>
	/// 子频道被修改后触发
	/// <para>
	/// 子频道被创建, 子频道资料变更, 子频道被删除<br/>
	/// BotClient - 机器人对象<br/>
	/// Channel - 频道对象（没有分组Id和排序位置属性）<br/>
	/// string - 事件类型（CHANNEL_CREATE|CHANNEL_UPDATE|CHANNEL_DELETE）
	/// </para>
	/// </summary>
	public event Action<BotClient, Channel, string>? OnChannelMsg;

	/// <summary>
	/// 成员信息变更后触发
	/// <para>
	/// 成员加入, 资料变更, 移除成员<br/>
	/// BotClient - 机器人对象<br/>
	/// MemberWithGuildID - 成员对象<br/>
	/// string - 事件类型（GUILD_MEMBER_ADD | GUILD_MEMBER_UPDATE | GUILD_MEMBER_REMOVE）
	/// </para>
	/// </summary>
	public event Action<BotClient, MemberWithGuildId, string>? OnGuildMemberMsg;

	/// <summary>
	/// 修改表情表态后触发
	/// <para>
	/// 添加表情表态, 删除表情表态<br/>
	/// BotClient - 机器人对象<br/>
	/// MessageReaction - 表情表态对象<br/>
	/// string - 事件类型（MESSAGE_REACTION_ADD|MESSAGE_REACTION_REMOVE）
	/// </para>
	/// </summary>
	public event Action<BotClient, MessageReaction, string>? OnMessageReaction;

	/// <summary>
	/// 消息审核出结果后触发
	/// <para>
	/// 消息审核通过才有MessageId属性<br/>
	/// BotClient - 机器人对象<br/>
	/// MessageAudited - 消息审核对象<br/>
	/// MessageAudited.IsPassed - 消息审核是否通过
	/// </para>
	/// </summary>
	public event Action<BotClient, MessageAudited?>? OnMessageAudit;

	/// <summary>
	/// 音频状态变更后触发
	/// </summary>
	public event Action<BotClient, JsonElement?>? OnAudioMsg;

	/// <summary>
	/// 频道内有人发消息就触发 (包含 @机器人 消息)
	/// <para>
	/// Sender - 发件人对象<br/>
	/// <see cref="Sender.MessageType"/> 包含消息类别（公开，AT机器人，AT全员，私聊）
	/// </para>
	/// </summary>
	public event Action<Sender>? OnMsgCreate;

	/// <summary>
	/// API调用出错时触发
	/// <para>
	/// Sender - 发件人对象（仅当调用API的主体为Sender时才有）<br/>
	/// List&lt;string&gt; - API错误信息{接口地址，请求方式，异常代码，异常原因}<br/>
	/// FreezeTime - 对访问出错的接口，暂停使用的时间
	/// </para>
	/// </summary>
	public event Action<Sender?, ApiErrorInfo>? OnApiError;


	/// <summary>
	/// 集中处理机器人的HTTP请求
	/// </summary>
	/// <param name="url">请求网址</param>
	/// <param name="method">请求类型(默认GET)</param>
	/// <param name="content">请求数据</param>
	/// <param name="sender">指示本次请求由谁发起的</param>
	/// <returns></returns>
	public async Task<HttpResponseMessage?> HttpSendAsync(
		string url, HttpMethod? method = null, HttpContent? content = null, Sender? sender = null)
	{
		method ??= HttpMethod.Get;
		var request = new HttpRequestMessage { RequestUri = new(new(ApiOrigin), url), Content = content, Method = method };
		request.Headers.Authorization = new("Bot", $"{BotAccessInfo.BotAppId}.{BotAccessInfo.BotToken}");

		return await BotHttpClient.SendAsync(request, f);


		async void f(HttpResponseMessage response, FreezeTime freezeTime)
		{
			var errInfo = new ApiErrorInfo(
				url.ReplaceStart(ApiOrigin),
				method.Method,
				response.StatusCode.GetHashCode(),
				"此错误类型未收录!",
				freezeTime
			);

			if (response.Content.Headers.ContentType?.MediaType == "application/json")
			{
				var err = JsonSerializer.Deserialize<ApiStatus>(await response.Content.ReadAsStringAsync());
				if (err is { Code: { } c })
				{
					errInfo.Code = c;
				}
				if (err is { Message: { } m })
				{
					errInfo.Detail = m;
				}
			}
			if (StatusCodes.OpenapiCode.TryGetValue(errInfo.Code, out string? value))
			{
				errInfo.Detail = value;
			}

			string senderAuthorName = (sender?.Bot.Guilds.TryGetValue(sender.GuildId, out var guild) is true ? guild.Name : null)
				?? sender?.Author.UserName
				?? string.Empty;

			Log.Error($"[接口访问失败]{senderAuthorName} 代码：{errInfo.Code}，详情：{errInfo.Detail}");

			OnApiError?.Invoke(sender, errInfo);
			if (sender is not null)
			{
				if (!sender.ReportError)
				{
					Log.Info($"[接口访问失败] 机器人配置ReportError!=true，取消推送给发件人");
				}
				else if (sender.Message.CreatedTime.AddMinutes(5) < DateTime.Now)
				{
					Log.Info($"[接口访问失败] 被动可回复时间已超时，取消推送给发件人");
				}
				else
				{
					_ = Task.Run(f);


					async void f()
					{
						sender.ReportError = false;
						await sender.ReplyAsync(
							string.Join(
								'\n',
								"❌接口访问失败",
								$"接口地址：{errInfo.Path}",
								$"请求方式：{errInfo.Method}",
								$"异常代码：{errInfo.Code}",
								$"异常详情：{errInfo.Detail}",
								errInfo.FreezeTime.EndTime > DateTime.Now
									? $"接口冻结：暂停使用此接口 {errInfo.FreezeTime.AddTime.Minutes}分{errInfo.FreezeTime.AddTime.Seconds}秒\n解冻时间：{errInfo.FreezeTime.EndTime:yyyy-MM-dd HH:mm:ss}"
									: null
							)
						);
					}
				}
			}
			else
			{
				Log.Info($"[接口访问失败] 访问接口的主体不是Sender，取消推送给发件人");
			}
		}
	}

	/// <summary>
	/// 存取最后一次发送的消息
	/// <para>注：目的是为了用于撤回消息（自动删除已过5分钟的记录）</para>
	/// </summary>
	/// <param name="msg">需要存储的msg，或者用于检索同频道的msg</param>
	/// <param name="push">fase-出栈；true-入栈</param>
	private Message? LastMessage(Message? msg, bool push = false)
	{
		if (msg is null)
		{
			return null;
		}

		var outTime = DateTime.Now.AddMinutes(-10);
		string userId = msg.MessageCreator.Id;
		StackMessage.TryGetValue(userId, out var messages);

		if (push)
		{
			messages?.RemoveAll(m => m.CreatedTime < outTime);
			messages ??= new();
			messages.Add(msg);
			StackMessage[userId] = messages;
		}
		else
		{
			bool predicate(Message m) => m.GuildId.Equals(msg.GuildId) && m.ChannelId.Equals(msg.ChannelId);
			int stackIndex = messages?.FindLastIndex(predicate) ?? -1;
			if (stackIndex != -1)
			{
				msg = messages![stackIndex];
				messages.RemoveAt(stackIndex);
			}
			else
			{
				msg = null;
			}
		}

		return msg;
	}

	/// <summary>
	/// 关闭机器人并释放所有占用的资源
	/// </summary>
	public void Close() => WebSocketClient.Abort();


	/// <summary>
	/// Starts the bot, with the specified number of retrying times.
	/// </summary>
	/// <param name="retryCount">The specified number of retrying times. The default value is 3.</param>
	[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	public async void Start(int retryCount = 3)
	{
		StartTime = DateTime.Now;

		await ConnectAsync(retryCount);
	}

	/// <summary>
	/// 上帝ID
	/// <para>仅用于测试,方便机器人开发者验证功能</para>
	/// </summary>
	public string GodId { get; set; } = "15524401336961673551";
}
