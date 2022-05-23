namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Gets the common WSS URL.
	/// </summary>
	/// <returns>An address that is used for the web socket.</returns>
	public async Task<string?> GetWssUrlAsync()
		=> (
			await HttpSendAsync($"{ApiOrigin}/gateway/") switch
			{
				{ Content: var responseContent } => await responseContent.ReadFromJsonAsync<JsonElement?>(),
				_ => null
			}
		)?.GetProperty("url").GetString();

	/// <summary>
	/// Gets the WSS URL with shards.
	/// For more information please visit
	/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/wss/shard_url_get.html">this link</see>.
	/// </summary>
	/// <returns>
	/// An address that is used for the web socket.
	/// In addition, the method will also return the recommended number of shards, and the usage data on the connection.
	/// </returns>
	public async Task<WebSocketLimit?> GetWssUrlWithSharedAsync()
		=> await HttpSendAsync($"{ApiOrigin}/gateway/bot") is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<WebSocketLimit?>()
			: null;

	/// <summary>
	/// Try to connect to server.
	/// </summary>
	/// <param name="retryCount">Indicates the number of the trial times if failed to connect.</param>
	/// <returns>
	/// A task instance that encapsulates the <see cref="bool"/> value indicating whether the operation is successful,
	/// as the real result value. If <see langword="false"/>, the maximum trial times
	/// (i.e. <paramref name="retryCount"/>) has been reached, but still not be able to connect server.
	/// </returns>
	public async Task<bool> ConnectAsync(int retryCount)
	{
		int retryEndTime = 10, retryAddTime = 10;
		while (retryCount-- > 0)
		{
			if (_webSocketClient is not { State: var state, CloseStatus: var s, CloseStatusDescription: var d })
			{
				throw new("Property-styled deconstruction failed.");
			}

			try
			{
				_gateLimit = await GetWssUrlWithSharedAsync();
				if (Uri.TryCreate(_gateLimit?.Url, UriKind.Absolute, out var webSocketUri))
				{
					if (state is not (WebSocketState.Open or WebSocketState.Connecting))
					{
						await _webSocketClient.ConnectAsync(webSocketUri, CancellationToken.None);
						WebSocketConnected?.Invoke(this);
						_ = ReceiveAsync();
					}

					break;
				}

				string useGatewayAddress = StringResource.Get("UseGatewayAddress")!;
				string failedToConnect = StringResource.Get("ConnectToServerFailed")!;
				Logging.Error($"[WebSocket][Connect] {useGatewayAddress}<{_gateLimit?.Url}> {failedToConnect}");
			}
			catch (Exception ex)
			{
				string statusText = StringResource.Get("Status")!;
				string descriptionText = StringResource.Get("Description")!;
				Logging.Error($"[WebSocket][Connect] {ex.Message} | {statusText}：{s} | {descriptionText}{d}");
			}

			if (retryCount <= 0)
			{
				string reason = StringResource.Get("ConnectFailed_MaxRetryCountReached")!;
				Logging.Error($"[WebSocket] {reason}");

				return false;
			}

			for (int i = retryEndTime; 0 < i; --i)
			{
				string retryText = StringResource.Get("SecondsLastToRetry")!;
				string closeBrace = StringResource.Get("CloseBrace")!;
				Logging.Info($"[WebSocket] {i} {retryText}${retryCount}{closeBrace} ...");
				await Task.Delay(TimeSpan.FromSeconds(1));
			}

			retryEndTime += retryAddTime;
		}

		return true;
	}

	/// <summary>
	/// Sends the identity information and try to authorize the bot.
	/// </summary>
	/// <returns>A task will be returned, which contains the <see langword="await"/>ing information.</returns>
	private async Task SendIdentifyAsync()
	{
		var data = new
		{
			op = Opcode.Identify,
			d = new
			{
				token = $"Bot {BotAccessInfo.AppId}.{BotAccessInfo.Token}",
				intents = Intents.GetHashCode(),
				shared = new[] { ShardId % (_gateLimit?.Shards ?? 1), _gateLimit?.Shards ?? 1 }
			}
		};

		static string f(Match m) => Regex.Replace(m.Groups[0].Value, """[^\.]""", "*");
		string sendMsg = JsonSerializer.Serialize(data);
		string unsensi = Regex.Replace(sendMsg, """(?<=Bot\s+)[^"]+""", f);
		Logging.Debug($"[WebSocket][SendIdentify] {unsensi}");
		await WebSocketSendAsync(sendMsg, WebSocketMessageType.Text, true);
	}

	/// <summary>
	/// Sends the heartbeat.
	/// </summary>
	/// <returns>A task will be returned, which contains the <see langword="await"/>ing information.</returns>
	private async Task SendHeartBeatAsync()
	{
		if (_webSocketClient.State == WebSocketState.Open)
		{
			string sendMsg = $$"""{"p": 1, "d": {{_webSocketLastSequence}}}""";
			Logging.Debug($"[WebSocket][SendHeartbeat] {sendMsg}");
			await WebSocketSendAsync(sendMsg, WebSocketMessageType.Text, true);
		}
		else
		{
			string connectionNotFound = StringResource.Get("ConnectionNotHavingBeenCreated")!;
			Logging.Error($"[WebSocket][Heartbeat] {connectionNotFound}");
		}
	}

	/// <summary>
	/// To resume the connection.
	/// </summary>
	/// <returns>A task will be returned, which contains the <see langword="await"/>ing information.</returns>
	private async Task SendResumeAsync()
	{
		try
		{
			var data = new
			{
				op = Opcode.Resume,
				d = new
				{
					token = $"Bot {BotAccessInfo.AppId}.{BotAccessInfo.Token}",
					session_id = _webSoketSessionId,
					seq = _webSocketLastSequence
				}
			};

			string sendMsg = JsonSerializer.Serialize(data);
			Logging.Debug($"[WebSocket][SendResume] {sendMsg}");
			await WebSocketSendAsync(sendMsg, WebSocketMessageType.Text, true);
		}
		catch (Exception e)
		{
			string resumeErrorText = StringResource.Get("ResumeError")!;
			Logging.Error($"[WebSocket] {resumeErrorText}{e.Message}");
		}
	}

	/// <summary>
	/// Sends the WebSocket data to server.
	/// </summary>
	/// <param name="data">The desired data to be sent.</param>
	/// <param name="messageType">
	/// The message type of the web socket. The default value is <see cref="WebSocketMessageType.Text"/>.
	/// </param>
	/// <param name="endOfMessaging">
	/// Indicates whether the message ends the messaging. The default value is <see langword="true"/>.
	/// </param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>A task will be returned, which contains the <see langword="await"/>ing information.</returns>
	private async Task WebSocketSendAsync(
		string data, WebSocketMessageType messageType = WebSocketMessageType.Text,
		bool endOfMessaging = true, CancellationToken cancellationToken = default)
	{
		WebSocketSending?.Invoke(this, new(data));

		await _webSocketClient.SendAsync(Encoding.UTF8.GetBytes(data), messageType, endOfMessaging, cancellationToken);
	}

	/// <summary>
	/// Receives the data from server.
	/// </summary>
	/// <returns>A task will be returned, which contains the <see langword="await"/>ing information.</returns>
	private async Task ReceiveAsync()
	{
		while (_webSocketClient.State == WebSocketState.Open)
		{
			try
			{
				using var memory = MemoryPool<byte>.Shared.Rent(1024 * 64);
				var result = await _webSocketClient.ReceiveAsync(memory.Memory, default);

				_ = result is { MessageType: var messageType, Count: var count };
				if (messageType == WebSocketMessageType.Text)
				{
					var json = JsonDocument.Parse(memory.Memory[..count]).RootElement;
					WebSocketReceived?.Invoke(this, new(json.GetRawText()));
					await ExecuteCommandAsync(json);

					continue;
				}

				string webSocketMessageTypeText = StringResource.Get("WebSocketMessageTypeIs")!;
				string statusText = StringResource.Get("Status")!;
				Logging.Info($"[WebSocket][Receive] {webSocketMessageTypeText}{messageType} | {statusText}{_webSocketClient.CloseStatus}");
			}
			catch (Exception ex)
			{
				string statusText = StringResource.Get("Status")!;
				string newLine = Environment.NewLine;
				Logging.Error($"[WebSocket][Receive] {ex.Message} | {statusText}{_webSocketClient.CloseStatus}{newLine}");
			}

			// If the value is 4009, the server will require a re-connection.
			// Now we should set the property 'IsResume' to true.
			if ((int?)_webSocketClient.CloseStatus is 4009)
			{
				_shouldBeResumed = true;
			}

			_webSocketClient.Abort();

			break;
		}

		if (_heartBeatTimer.Enabled)
		{
			_heartBeatTimer.Enabled = false;
		}

		WebSocketClosed?.Invoke(this);

		string tryReconnectToServerText = StringResource.Get("TryReconnectToServer")!;
		Logging.Warn($"[WebSocket] {tryReconnectToServerText}");

		await Task.Delay(TimeSpan.FromSeconds(1));
		_webSocketClient = new();
		await ConnectAsync(30);
	}

	/// <summary>
	/// Parses the message received.
	/// </summary>
	/// <param name="wssJson">The JSON element received via WSS.</param>
	/// <returns>A task will be returned, which contains the <see langword="await"/>ing information.</returns>
	private async Task ExecuteCommandAsync(JsonElement wssJson)
	{
		switch ((Opcode)wssJson.GetProperty("op").GetInt32())
		{
			case Opcode.Dispatch: // Do the message dispatching.
			{
				MessageDispatched?.Invoke(this, wssJson);

				_webSocketLastSequence = wssJson.GetProperty("s").GetInt32();
				if (!wssJson.TryGetProperty("t", out var t) || !wssJson.TryGetProperty("d", out var d))
				{
					Logging.Warn($"[WebSocket][Op00][Dispatch] {wssJson.GetRawText()}");
					break;
				}

				string? data = d.GetRawText(), type = t.GetString();
				switch (type)
				{
					case RawMessageTypes.DirectMessageCreated:
					case RawMessageTypes.MentionMessageCreated:
					case RawMessageTypes.NormalMessageCreated:
					{
						if (d.Deserialize<Message>() is not { } message)
						{
							Logging.Warn($"[WebSocket][{type}] {data}");
							return;
						}

						Logging.Debug($"[WebSocket][{type}] {data}");
						_ = MessageCenterAsync(message, type);

						break;
					}

					case RawMessageTypes.GuildCreated:
					case RawMessageTypes.GuildUpdated:
					case RawMessageTypes.GuildDeleted:
					{
						Logging.Debug($"[WebSocket][{type}] {data}");
						var guild = d.Deserialize<Guild>()!;
						switch (type)
						{
							case RawMessageTypes.GuildCreated:
							case RawMessageTypes.GuildUpdated:
							{
								guild.APIPermissions = await GetGuildPermissionsAsync(guild.Id, null);
								Guilds[guild.Id] = guild;

								break;
							}
							case RawMessageTypes.GuildDeleted:
							{
								Guilds.Remove(guild.Id, out _);

								break;
							}
						}

						GuildEventSetTriggered?.Invoke(this, new(guild, type));

						break;
					}

					case RawMessageTypes.ChannelCreated:
					case RawMessageTypes.ChannelUpdated:
					case RawMessageTypes.ChannelDeleted:
					{
						Logging.Debug($"[WebSocket][{type}] {data}");
						var channel = d.Deserialize<Channel>()!;
						ChannelEventSetTriggered?.Invoke(this, new(channel, type));

						break;
					}

					case RawMessageTypes.GuildMemberAdded:
					case RawMessageTypes.GuildMemberUpdated:
					case RawMessageTypes.GuildMemberRemoved:
					{
						Logging.Debug($"[WebSocket][{type}] {data}");
						var memberWithGuild = d.Deserialize<MemberWithGuildId>()!;
						GuildMemberEventSetTriggered?.Invoke(this, new(memberWithGuild, type));

						break;
					}

					case RawMessageTypes.MessageReactionAdded:
					case RawMessageTypes.MessageReactionRemoved:
					{
						Logging.Debug($"[WebSocket][{type}] {data}");
						var messageReaction = d.Deserialize<MessageReaction>()!;
						MessageReactionEventSetTriggered?.Invoke(this, new(messageReaction, type));

						break;
					}

					case RawMessageTypes.MessageAuditPassed:
					case RawMessageTypes.MessageAuditRejected:
					{
						Logging.Info($"[WebSocket][{type}] {data}");
						var messageAudited = d.Deserialize<MessageAudited>()!;
						messageAudited.IsPassed = type == RawMessageTypes.MessageAuditPassed;
						MessageAudited?.Invoke(this, new(messageAudited));

						break;
					}

					case RawMessageTypes.AudioStarted:
					case RawMessageTypes.AudioFinished:
					case RawMessageTypes.AudioOnMic:
					case RawMessageTypes.AudioOffMic:
					{
						Logging.Info($"[WebSocket][{type}] {data}");
						AudioEventSetTriggered?.Invoke(this, wssJson);

						break;
					}

					case RawMessageTypes.Resumed:
					{
						string connectionIsResumedText = StringResource.Get("ConnectionIsResumed")!;
						Logging.Info($"[WebSocket][Op00][RESUMED] {connectionIsResumedText}");
						await ExecuteCommandAsync(JsonDocument.Parse($$"""{"op":{{(int)Opcode.Heartbeat}}}""").RootElement);

						ConnectionResumed?.Invoke(this, d);

						break;
					}

					case RawMessageTypes.Ready:
					{
						Logging.Debug($"[WebSocket][READY] {data}");

						string authorizationSuccessfulText = StringResource.Get("AuthorizationSuccessful")!;
						Logging.Info($"[WebSocket][Op00] {authorizationSuccessfulText}");
						await ExecuteCommandAsync(JsonDocument.Parse($$"""{"op":{{(int)Opcode.Heartbeat}}}""").RootElement);
						_webSoketSessionId = d.GetProperty("session_id").GetString();

						string getGuildListWithLimit100Text = StringResource.Get("GetGuildListWithLimit100")!;
						Logging.Info($"[WebSocket][GetGuilds] {getGuildListWithLimit100Text}");
						string? guildNext = null;
						for (int page = 1; ; ++page)
						{
							string getGuildJoinedText = StringResource.Get("GetGuildJoined")!;

							var guilds = await GetMeGuildsAsync(guildNext, false, 100, null);
							if (guilds is null)
							{
								string getGuildJoinedFailedText = StringResource.Get("GetGuildJoinedFailed")!;

								Logging.Info($"[WebSocket][GetGuilds] {getGuildJoinedText} {page:00} {getGuildJoinedFailedText}");
								break;
							}

							if (guilds.Count == 0)
							{
								string getGuildJoinedEmptyText = StringResource.Get("GetGuildJoinedEmpty")!;

								Logging.Info($"[WebSocket][GetGuilds] {getGuildJoinedText} {page:00} {getGuildJoinedEmptyText}");
								break;
							}

							string getGuildJoinedSuccessfulText = StringResource.Get("GetGuildJoinedSuccessful")!;
							Logging.Info($"[WebSocket][GetGuilds] {getGuildJoinedText} {page:00} {getGuildJoinedSuccessfulText} {guilds.Count}");

							Parallel.ForEach(guilds, loop);
							guildNext = guilds.Last().Id;


							void loop(Guild guild, ParallelLoopState state, long i)
							{
								guild.APIPermissions = GetGuildPermissionsAsync(guild.Id, null).Result;
								Guilds[guild.Id] = guild;
							}
						}

						string botHasJoinedText = StringResource.Get("BotHasJoined")!;
						string guildCountText = StringResource.Get("GuildCountText")!;
						Logging.Info($"[WebSocket][GetGuilds] {botHasJoinedText} {Guilds.Count} {guildCountText}");

						Info = d.GetProperty("user").Deserialize<User>()!;
						Info.Avatar = (await GetInfoAsync(null))?.Avatar;

						// Triggers the event.
						// Here 'this' can be replaced with 'null' because the first argument is useless.
						AuthorizationPassed?.Invoke(this, new(Info));

						break;
					}

					default:
					{
						string unknownEventText = StringResource.Get("UnknownEvent")!;
						Logging.Warn($"[WebSocket][{type}] {unknownEventText}");
						break;
					}
				}

				break;
			}
			case Opcode.Heartbeat: // Send or receive heartbeat.
			{
				string endpoint = StringResource.Get(wssJson.Get("d") == null ? "Client" : "Server")!;
				string sendHeartbeatText = StringResource.Get("SendHeartbeat")!;
				Logging.Debug($"[WebSocket][Op01] {endpoint}{sendHeartbeatText}");

				HeartbeatMessageReceived?.Invoke(this, wssJson);

				await SendHeartBeatAsync();
				_heartBeatTimer.Enabled = true;

				break;
			}
			case Opcode.Identify: // Send authorization.
			{
				string sendAuthorizationText = StringResource.Get("SendAuthorization")!;
				Logging.Info($"[WebSocket][Op02] {sendAuthorizationText}");

				Identifying?.Invoke(this, wssJson);

				await SendIdentifyAsync();

				break;
			}
			case Opcode.Resume: // Resume the connection.
			{
				string sendReconnect = StringResource.Get("SendResumeMessage")!;
				Logging.Info($"[WebSocket][Op06] {sendReconnect}");
				_shouldBeResumed = false;

				ConnectionResuming?.Invoke(this, wssJson);

				await SendResumeAsync();

				break;
			}
			case Opcode.Reconnect: // Receive the message that requires client reconnecting.
			{
				string receiveReconnect = StringResource.Get("ReceiveReconnectMessage")!;
				Logging.Info($"[WebSocket][Op07] {receiveReconnect}");

				ConnectionReconnected?.Invoke(this, wssJson);

				break;
			}
			case Opcode.InvalidSession: // Receive error message when identifying or resuming.
			{
				string receiveAuthorizationErrorMessage = StringResource.Get("ReceiveAuthorizationErrorMessage")!;
				Logging.Warn($"[WebSocket][Op09] {receiveAuthorizationErrorMessage}");

				SessionInvalid?.Invoke(this, wssJson);

				break;
			}
			case Opcode.Hello: // Receive helloing message.
			{
				const int defaultHeartbeatInterval = 30000;

				string receiveHelloMessage = StringResource.Get("ReceiveHelloMessage")!;
				Logging.Info($"[WebSocket][Op10][{receiveHelloMessage}] {wssJson.GetRawText()}");

				Helloing?.Invoke(this, wssJson);

				int heartbeatIntervalValue = wssJson.Get("d")?.Get("heartbeat_interval")?.GetInt32() ?? defaultHeartbeatInterval;
				_heartBeatTimer.Interval = Math.Min(heartbeatIntervalValue, defaultHeartbeatInterval);
				await ExecuteCommandAsync(
					JsonDocument.Parse(
						$$"""
						{"op":{{(int)(_shouldBeResumed ? Opcode.Resume : Opcode.Identify)}}}
						"""
					).RootElement
				);

				break;
			}
			case Opcode.HeartbeatACK: // Receive acknowledge of heartbeat.
			{
				string receiveHeartbeatAck = StringResource.Get("ReceiveHeartbeatAcknowledgementMessage")!;
				Logging.Debug($"[WebSocket][Op11] {receiveHeartbeatAck}");

				HeartbeatMessageAcknowledged?.Invoke(this, wssJson);

				break;
			}
			case var opCode:
			{
				string unknownOpCode = StringResource.Get("UnkownOperationCode")!;
				Logging.Warn($"[WebSocket][OpNC] {unknownOpCode}{opCode}");

				break;
			}
		}
	}
}
