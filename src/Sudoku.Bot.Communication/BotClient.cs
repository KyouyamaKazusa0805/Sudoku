using CommunicationIntents = Sudoku.Bot.Communication.Intents;

namespace Sudoku.Bot.Communication;

/// <summary>
/// Defines a bot client instance.
/// </summary>
public partial class BotClient
{
	/// <summary>
	/// Indicates the API link that corresponds to the release APIs.
	/// </summary>
	private const string ReleaseApi = "https://api.sgroup.qq.com";

	/// <summary>
	/// Indicates the API link that corresponds to the sandbox APIs.
	/// </summary>
	private const string SandboxApi = "https://sandbox.api.sgroup.qq.com";


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
	/// Indicates whether the client raises the API errors to the foreground commands.
	/// The default value is <see langword="false"/>.
	/// </summary>
	public bool ReportApiError { get; set; }

	/// <summary>
	/// Indicates the shard ID. The default value is 0.
	/// </summary>
	/// <remarks>
	/// For more information please visit
	/// <see href="https://bot.q.qq.com/wiki/develop/api/gateway/shard.html">this link</see>.
	/// </remarks>
	public int ShardId { get; set; } = 0;

	/// <summary>
	/// Indicates the URL link corresponding to the bot API.
	/// </summary>
	public string ApiOrigin { get; init; }

	/// <summary>
	/// Indicates the identity of the bot access information, used for authorization.
	/// </summary>
	/// <remarks>
	/// You can found those information from
	/// <see href="https://bot.q.qq.com/#/developer/developer-setting">this link</see>.
	/// </remarks>
	public Identity BotAccessInfo { get; set; }

	/// <summary>
	/// The events that the current connection will be received.
	/// You can use <see cref="CommunicationIntents"/> enumeration to get the default settings
	/// of event intents.
	/// </summary>
	/// <remarks>
	/// For more information please visit the description about
	/// <see href="https://bot.q.qq.com/wiki/develop/api/gateway/intents.html">intents for the events</see>.
	/// </remarks>
	/// <completionlist cref="CommunicationIntents"/>
	public Intent Intents { get; set; } = CommunicationIntents.PublicDomain;

	/// <summary>
	/// The bot information.
	/// </summary>
	public User Info { get; private set; } = new();

	/// <summary>
	/// Indicates the cached data that describes all GUILDs that the current bot has joined or has been joined.
	/// </summary>
	public ConcurrentDictionary<string, Guild> Guilds { get; private set; } = new();

	/// <summary>
	/// Indicates the cached data that describes all possible members in each GUILD that the bot has joined
	/// or has been joined.
	/// The dictionary stores key-value pairs to store the data. The key is the GUILD ID value, and the value
	/// is the member data that corresponds the current GUILD.
	/// </summary>
	public ConcurrentDictionary<string, Member?> Members { get; private set; } = new();

	/// <summary>
	/// Indicates the filtering predicate instance that can discard messages if the predicate
	/// returns <see langword="true"/>.
	/// </summary>
	public Predicate<Sender>? MessageFilter { get; set; }

	/// <summary>
	/// Indicates the boolean value that describes whether the current status is resumed.
	/// The default value is <see langword="false"/>.
	/// </summary>
	private bool IsResume { get; set; } = false;

	/// <summary>
	/// Indicates the sequence label of the last message. The default value is 1.
	/// </summary>
	private int WebSocketLastSeq { get; set; } = 1;

	/// <summary>
	/// Indicates the session ID of the current web socket client.
	/// </summary>
	private string? WebSoketSessionId { get; set; }

	/// <summary>
	/// Indicates the inner web socket client.
	/// </summary>
	private ClientWebSocket WebSocketClient { get; set; } = new();

	/// <summary>
	/// Indicates the timer that records the heartbeats.
	/// </summary>
	private Timer HeartBeatTimer { get; set; } = new();

	/// <summary>
	/// Indicates the data of the shards.
	/// </summary>
	private WebSocketLimit? GateLimit { get; set; }

	/// <summary>
	/// Indiactes the cached data that stores the messages.
	/// The dictionary stores key-value pairs. The key value is the user ID, and the value is the messages
	/// that emitted from the current user.
	/// </summary>
	private ConcurrentDictionary<string, List<Message>> StackMessage { get; set; } = new();


	/// <summary>
	/// Indicates the SDK information. The return value is a <see cref="string"/>
	/// that is combined by the name, version, GitHub page and the copyright of the author.
	/// </summary>
	/// <remarks><b>
	/// The SDK idea and the framework is copied from
	/// <see href="https://github.com/Antecer/QQChannelBot">this repository</see> (Antecer/QQChannelBot),
	/// so the original value corresponds to the original author instead of me.
	/// </b></remarks>
	public static string SdkIdentifier
		=> $"{SdkData.SdkName}_{SdkData.SdkVersion}\n{SdkData.RepoLink_Github}\n{SdkData.Copyright}";

	/// <summary>
	/// Indicates the time that records the bot being running.
	/// </summary>
	public static DateTime StartTime { get; private set; } = DateTime.Now;


	/// <summary>
	/// Indicates the event triggered when the web socket has been connected.
	/// </summary>
	public event WebSocketConnectedEventHandler? WebSocketConnected;

	/// <summary>
	/// Indicates the event triggered when the web socket client has closed the connection.
	/// </summary>
	public event WebSocketClosedEventHandler? WebSocketClosed;

	/// <summary>
	/// Indicates the event triggered before sending the message.
	/// </summary>
	public event WebSocketSendingEventHandler? WebSocketSending;

	/// <summary>
	/// Indicates the event triggered after being received the message.
	/// </summary>
	public event WebSocketReceivedEventHandler? WebSocketReceived;

	/// <summary>
	/// Indicates the event triggered when a message is dispatched from the server.
	/// </summary>
	public event JsonElementRelatedEventHandler? MessageDispatched;

	/// <summary>
	/// Indicates the event triggered when a heartbeat message is received from the server,
	/// or sent from the current client.
	/// </summary>
	public event JsonElementRelatedEventHandler? HeartbeatMessageReceived;

	/// <summary>
	/// Indicates the event triggered when an authorization message is sent from the server.
	/// </summary>
	public event JsonElementRelatedEventHandler? Identifying;

	/// <summary>
	/// Indicates the event triggered when the connection is being resumed.
	/// </summary>
	public event JsonElementRelatedEventHandler? ConnectionResuming;

	/// <summary>
	/// Indicates the event triggered when the connection is resumed.
	/// </summary>
	public event JsonElementRelatedEventHandler? ConnectionResumed;

	/// <summary>
	/// Indicates the event triggered when the server noticed the user that connection is reconnected.
	/// </summary>
	public event JsonElementRelatedEventHandler? ConnectionReconnected;

	/// <summary>
	/// Indicates the event triggered when the invalid cast has been encountered while triggering
	/// <see cref="Identifying"/> or <see cref="ConnectionResuming"/>.
	/// </summary>
	/// <seealso cref="Identifying"/>
	/// <seealso cref="ConnectionResuming"/>
	public event JsonElementRelatedEventHandler? SessionInvalid;

	/// <summary>
	/// Indicates the event triggered when created the connection between client and the gateway.
	/// </summary>
	public event JsonElementRelatedEventHandler? Helloing;

	/// <summary>
	/// Indicates the event triggered when the server has received the heartbeat message.
	/// </summary>
	public event JsonElementRelatedEventHandler? HeartbeatMessageAcknowledged;

	/// <summary>
	/// Indicates the event triggered when an audio instance has been changed its status.
	/// </summary>
	public event NullableJsonElementRelatedEventHandler? AudioEventSetTriggered;

	/// <summary>
	/// Indicates the event triggered when the authorization on bot data is passed.
	/// </summary>
	public event AuthoizationPassedEventHandler? AuthorizationPassed;

	/// <summary>
	/// Indicates the event triggered when a message is created.
	/// This event will also contain the case that the bot is mentioned.
	/// </summary>
	public event MessageCreatedEventHandler? MessageCreated;

	/// <summary>
	/// Indicates the event triggered when an event related to the specified GUILD is triggered.
	/// </summary>
	public event GuildRelatedEventHandler? GuildEventSetTriggered;

	/// <summary>
	/// Indicates the event triggered when an event related to GUILD-levelled member is triggered.
	/// </summary>
	public event GuildMemberRelatedEventHandler? GuildMemberEventSetTriggered;

	/// <summary>
	/// Indicates the event triggered when an event related to the specified channel is triggered.
	/// </summary>
	public event ChannelRelatedEventHandler? ChannelEventSetTriggered;

	/// <summary>
	/// Indicates the event triggered when an event related to a message reaction is triggered.
	/// </summary>
	public event MessageReactionRelatedEventHandler? MessageReactionEventSetTriggered;

	/// <summary>
	/// Indicates the event triggered when a message is audited.
	/// </summary>
	public event MessageAuditedEventHandler? MessageAudited;

	/// <summary>
	/// Indicates the event triggered when an error has been encountered when called an API.
	/// </summary>
	public event ApiEncounteredErrorEventHandler? ApiErrorEncountered;


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

			string senderAuthorName =
				(sender?.Bot.Guilds.TryGetValue(sender.GuildId, out var guild) is true ? guild.Name : null)
					?? sender?.Author.UserName
					?? string.Empty;

			Log.Error($"[接口访问失败]{senderAuthorName} 代码：{errInfo.Code}，详情：{errInfo.Detail}");

			ApiErrorEncountered?.Invoke(this, new(sender, errInfo));
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
