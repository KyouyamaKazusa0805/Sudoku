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
	/// Indicates the cached data that stores the messages.
	/// The dictionary stores key-value pairs. The key value is the user ID, and the value is the messages
	/// that emitted from the current user.
	/// </summary>
	private readonly ConcurrentDictionary<string, List<Message>> _messageStack = new();

	/// <summary>
	/// Indicates the timer that records the heartbeats.
	/// </summary>
	private readonly Timer _heartBeatTimer = new();


	/// <summary>
	/// Indicates the boolean value that describes whether the current status should be resumed.
	/// The default value is <see langword="false"/>.
	/// </summary>
	private bool _shouldBeResumed = false;

	/// <summary>
	/// Indicates the sequence label of the last message. The default value is 1.
	/// </summary>
	private int _webSocketLastSequence = 1;

	/// <summary>
	/// Indicates the session ID of the current web socket client.
	/// </summary>
	private string? _webSoketSessionId;

	/// <summary>
	/// Indicates the inner web socket client.
	/// </summary>
	private ClientWebSocket _webSocketClient = new();

	/// <summary>
	/// Indicates the data of the shards.
	/// </summary>
	private WebSocketLimit? _gateLimit;

	
	/// <summary>
	/// Initializes a <see cref="BotClient"/> instance via the identity instance and two <see cref="bool"/>
	/// values indicating whether the API is based on sandbox, and whether the client will report errors on APIs
	/// during running respectively.
	/// </summary>
	/// <param name="identity">The identity instance.</param>
	/// <param name="sandBoxApi">Indicates whether the API is based on sandbox.</param>
	/// <param name="reportApiError">Indicates whether the client will report errors to foreground.</param>
	public BotClient(BotIdentity identity, bool sandBoxApi = false, bool reportApiError = false)
	{
		(BotAccessInfo, ApiOrigin, ReportApiError) = (identity, sandBoxApi ? SandboxApi : ReleaseApi, reportApiError);

		_heartBeatTimer.Elapsed += async (_, _) =>
		{
			var rootElement = JsonDocument.Parse($$"""{"op":{{(int)Opcode.Heartbeat}}}""").RootElement;
			await ExecuteCommandAsync(rootElement);
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
	/// Indicates the God ID value. The ID is only used for testing the robustness of the bot functions.
	/// </summary>
	public string GodId { get; set; } = "15640902785096785230";

	/// <summary>
	/// Indicates the identity of the bot access information, used for authorization.
	/// </summary>
	/// <remarks>
	/// You can found those information from
	/// <see href="https://bot.q.qq.com/#/developer/developer-setting">this link</see>.
	/// </remarks>
	public BotIdentity BotAccessInfo { get; set; }

	/// <summary>
	/// The events that the current connection will be received.
	/// You can use <see cref="CommunicationIntents"/> enumeration to get the default settings
	/// of event intents. The default value is <see cref="CommunicationIntents.PublicDomain"/>.
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
	/// returns <see langword="false"/>.
	/// </summary>
	public Predicate<Sender>? MessageFilter { get; set; }


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
	{
		get
		{
			string a = SdkData.SdkName!;
			var b = SdkData.SdkVersion!;
			string c = StringResource.Get("OriginalAuthor")!;
			string d = SdkData.RepoLink_Github;
			string e = SdkData.Copyright;
			return $"{a}_{b}\n{c}{d}\n{e}";
		}
	}

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
	/// Indicates the event triggered when an event related to GUILD-leveled member is triggered.
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
	/// The core method to handle the HTTP requests about a bot.
	/// </summary>
	/// <param name="url">The URL link for the API for requesting.</param>
	/// <param name="method">
	/// <para>The HTTP method. The default value is <see cref="HttpMethod.Get"/>.</para>
	/// <para>
	/// Due to the limitation of the C# syntax (cannot assign non-constant expression
	/// to the argument as the default value), the default value will be created inside the method.
	/// </para>
	/// </param>
	/// <param name="content">Indicates the content data to request.</param>
	/// <param name="sender">Indicates the sender who emits the request.</param>
	/// <returns>A task that encapsulates a result of the HTTP response message.</returns>
	public async Task<HttpResponseMessage?> HttpSendAsync(
		string url, HttpMethod? method = null, HttpContent? content = null, Sender? sender = null)
	{
		method ??= HttpMethod.Get;

		var request = new HttpRequestMessage
		{
			RequestUri = new(new(ApiOrigin), url),
			Content = content,
			Method = method
		};

		request.Headers.Authorization = new("Bot", $"{BotAccessInfo.AppId}.{BotAccessInfo.Token}");

		return await BotHttpClient.SendAsync(request, f);


		async void f(HttpResponseMessage response, FreezeTime freezeTime)
		{
			var errInfo = new ApiErrorInfo(
				url.ReplaceStart(ApiOrigin),
				method.Method,
				response.StatusCode.GetHashCode(),
				StringResource.Get("WebSocketError_SuchApiErrorNotFound")!,
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
					?? sender?.MessageCreator.UserName
					?? string.Empty;

			string accessFailedHeader = StringResource.Get("LogHeader_InterfaceAccessFailed")!;
			string errorCodeText = StringResource.Get("LogContent_ErrorCode")!;
			string detailsText = StringResource.Get("LogContent_Details")!;
			Logging.Error(
				$"""
				[{accessFailedHeader}]{senderAuthorName} {errorCodeText}{errInfo.Code}{detailsText}{errInfo.Detail}
				"""
			);

			ApiErrorEncountered?.Invoke(this, new(sender, errInfo));
			switch (sender)
			{
				case { ReportError: false }:
				{
					string seg1 = StringResource.Get("LogContent_ReportFalseSegment1")!;
					string seg2 = StringResource.Get("LogContent_ReportFalseSegment2")!;
					Logging.Info($"[{accessFailedHeader}] {seg1} {nameof(Sender.ReportError)} {seg2}");

					break;
				}
				case { Message.CreatedTime: var createdTime } when createdTime.AddMinutes(5) < DateTime.Now:
				{
					string messageTimeout = StringResource.Get("LogContent_MessageTimeout")!;
					Logging.Info($"[{accessFailedHeader}] {messageTimeout}");

					break;
				}
				case null:
				{
					string seg1 = StringResource.Get("LogContent_InterfaceMainBodyNotSenderSegment1")!;
					string seg2 = StringResource.Get("LogContent_InterfaceMainBodyNotSenderSegment2")!;
					Logging.Info($"[{accessFailedHeader}] {seg1} {nameof(Sender)}{seg2}");

					break;
				}
				default:
				{
					if (
						errInfo is
						{
							FreezeTime:
							{
								AddTime: { Minutes: var extraMinutes, Seconds: var extraSeconds },
								EndTime: var endTime
							},
							Path: var path,
							Method: var method,
							Code: var errorCode,
							Detail: var details
						}
					)
					{
						_ = Task.Run(f);
					}

					break;


					async void f()
					{
						sender.ReportError = false;

						string windowFrozen = StringResource.Get("LogContent_WindowIsFrozen")!;
						string minutes = StringResource.Get("DateTimeUnit_Minute2")!;
						string seconds = StringResource.Get("DateTimeUnit_Second2")!;
						string unfrozenLast = StringResource.Get("LogContent_UnfrozenTimeLast")!;
						await sender.ReplyAsync(
							string.Join(
								'\n',
								$"{StringResource.Get("Emoji_Cross")}{accessFailedHeader}",
								$"{StringResource.Get("LogContent_RequestUrl")}{path}",
								$"{StringResource.Get("LogContent_RequestMethod")}{method}",
								$"{StringResource.Get("LogContent_ErrorCodeText")}{errorCode}",
								$"{StringResource.Get("LogContent_ErrorDetails")}{details}",
								endTime > DateTime.Now
									? $"{windowFrozen} {extraMinutes} {minutes} {extraSeconds} {seconds}\n{unfrozenLast}{endTime:yyyy-MM-dd HH:mm:ss}"
									: null
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Push or pop the last message sent. The method is used for the recalling.
	/// </summary>
	/// <param name="message">The message need pushing or popping.</param>
	/// <param name="isPushing">
	/// Indicates whether the operation is pushing. <see langword="true"/> is for pushing,
	/// and <see langword="false"/> is for popping.
	/// </param>
	[return: NotNullIfNotNull("message")]
	private Message? LastMessage(Message? message, bool isPushing = false)
	{
		if (message is null)
		{
			return null;
		}

		var timeout = DateTime.Now.AddMinutes(-10);
		string userId = message.MessageCreator.Id;
		_messageStack.TryGetValue(userId, out var messages);

		if (isPushing)
		{
			bool predicate(Message m) => m.CreatedTime < timeout;
			messages?.RemoveAll(predicate);
			(messages ??= new()).Add(message);

			_messageStack[userId] = messages;
		}
		else
		{
			bool predicate(Message m) => m.GuildId == message.GuildId && m.ChannelId == message.ChannelId;
			if (messages?.FindLastIndex(predicate) is { } stackIndex and not -1)
			{
				message = messages[stackIndex];
				messages.RemoveAt(stackIndex);
			}
			else
			{
				message = null;
			}
		}

		return message;
	}

	/// <summary>
	/// Closes the bot and release the memory used or allocated in the bot.
	/// </summary>
	public void Close() => _webSocketClient.Abort();

	/// <summary>
	/// Starts the bot, with the specified number of retrying times.
	/// </summary>
	/// <param name="retryCount">The specified number of retrying times. The default value is 3.</param>
	/// <param name="connectionSucceedCallback">
	/// Indicates the callback method that will be invoked when succeeded to connect to server.
	/// The value can be <see langword="null"/> if you don't want to define the customized callback method.
	/// </param>
	/// <param name="connectionFailCallback">
	/// Indicates the callback method that will be invoked when failed to connect to server,
	/// run out of the specified times of retrying.
	/// The value can be <see langword="null"/> if you don't want to define the customized callback method.
	/// </param>
	public async void StartAsync(int retryCount, Action? connectionSucceedCallback, Action? connectionFailCallback)
	{
		StartTime = DateTime.Now;

		(await ConnectAsync(retryCount) ? connectionSucceedCallback : connectionFailCallback)?.Invoke();
	}
}
