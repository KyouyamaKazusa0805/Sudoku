namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Gets the common WSS URL.
	/// </summary>
	/// <returns>An address that is used for the web socket.</returns>
	public async Task<string?> GetWssUrlAsync()
	{
		var response = await HttpSendAsync($"{ApiOrigin}/gateway/");
		var res = response is null ? null : await response.Content.ReadFromJsonAsync<JsonElement?>();
		return res?.GetProperty("url").GetString();
	}

	/// <summary>
	/// 获取带分片 WSS 接入点
	/// <para>
	/// 详情查阅: <see href="https://bot.q.qq.com/wiki/develop/api/openapi/wss/shard_url_get.html">QQ机器人文档</see>
	/// </para>
	/// </summary>
	/// <returns>一个用于连接 websocket 的地址。<br/>同时返回建议的分片数，以及目前连接数使用情况。</returns>
	public async Task<WebSocketLimit?> GetWssUrlWithSharedAsync()
	{
		var response = await HttpSendAsync($"{ApiOrigin}/gateway/bot");
		return response is null ? null : await response.Content.ReadFromJsonAsync<WebSocketLimit?>();
	}

	/// <summary>
	/// 建立到服务器的连接
	/// <para><em>RetryCount</em> 连接失败后允许的重试次数</para>
	/// </summary>
	/// <param name="retryCount">连接失败后允许的重试次数</param>
	/// <returns>表示是否连接成功。如果提前退出，返回值为 false。</returns>
	public async Task<bool> ConnectAsync(int retryCount)
	{
		int retryEndTime = 10, retryAddTime = 10;
		while (retryCount-- > 0)
		{
			try
			{
				GateLimit = await GetWssUrlWithSharedAsync();
				if (Uri.TryCreate(GateLimit?.Url, UriKind.Absolute, out var webSocketUri))
				{
					if (WebSocketClient.State is not (WebSocketState.Open or WebSocketState.Connecting))
					{
						await WebSocketClient.ConnectAsync(webSocketUri, CancellationToken.None);
						WebSocketConnected?.Invoke(this);
						_ = ReceiveAsync();
					}

					break;
				}

				Log.Error($"[WebSocket][Connect] 使用网关地址<{GateLimit?.Url}> 建立连接失败！");
			}
			catch (Exception ex)
			{
				Log.Error($"[WebSocket][Connect] {ex.Message} | Status：{WebSocketClient.CloseStatus} | Description：{WebSocketClient.CloseStatusDescription}");
			}

			if (retryCount <= 0)
			{
				Log.Error($"[WebSocket] 重连次数已耗尽，无法与频道服务器建立连接！");

				return false;
			}

			for (int i = retryEndTime; 0 < i; --i)
			{
				Log.Info($"[WebSocket] {i} 秒后再次尝试连接（剩余重试次数：${retryCount}）...");
				await Task.Delay(TimeSpan.FromSeconds(1));
			}

			retryEndTime += retryAddTime;
		}

		return true;
	}

	/// <summary>
	/// 鉴权连接
	/// </summary>
	/// <returns></returns>
	private async Task SendIdentifyAsync()
	{
		var data = new
		{
			op = Opcode.Identify,
			d = new
			{
				token = $"Bot {BotAccessInfo.AppId}.{BotAccessInfo.Token}",
				intents = Intents.GetHashCode(),
				shared = new[] { ShardId % (GateLimit?.Shards ?? 1), GateLimit?.Shards ?? 1 }
			}
		};

		string sendMsg = JsonSerializer.Serialize(data);
		Log.Debug("[WebSocket][SendIdentify] " + Regex.Replace(sendMsg, @"(?<=Bot\s+)[^""]+", static m => Regex.Replace(m.Groups[0].Value, @"[^\.]", "*"))); // 敏感信息脱敏处理
		await WebSocketSendAsync(sendMsg, WebSocketMessageType.Text, true);
	}

	/// <summary>
	/// 发送心跳
	/// </summary>
	/// <returns></returns>
	private async Task SendHeartBeatAsync()
	{
		if (WebSocketClient.State == WebSocketState.Open)
		{
			string sendMsg = "{\"op\": 1, \"d\":" + WebSocketLastSeq + "}";
			Log.Debug($"[WebSocket][SendHeartbeat] {sendMsg}");
			await WebSocketSendAsync(sendMsg, WebSocketMessageType.Text, true);
		}
		else
		{
			Log.Error($"[WebSocket][Heartbeat] 未建立连接！");
		}
	}

	/// <summary>
	/// 恢复连接
	/// </summary>
	/// <returns></returns>
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
					session_id = WebSoketSessionId,
					seq = WebSocketLastSeq
				}
			};
			string sendMsg = JsonSerializer.Serialize(data);
			Log.Debug($"[WebSocket][SendResume] {sendMsg}");
			await WebSocketSendAsync(sendMsg, WebSocketMessageType.Text, true);
		}
		catch (Exception e)
		{
			Log.Error($"[WebSocket] Resume Error: {e.Message}");
		}
	}

	/// <summary>
	/// WebScoket发送数据到服务端
	/// </summary>
	/// <param name="data">要发送的数据</param>
	/// <param name="msgType">WebSocket消息类型</param>
	/// <param name="endOfMsg">表示数据已发送结束</param>
	/// <param name="cancelToken">用于传播应取消此操作的通知的取消令牌。</param>
	/// <returns></returns>
	private async Task WebSocketSendAsync(
		string data, WebSocketMessageType msgType = WebSocketMessageType.Text,
		bool endOfMsg = true, CancellationToken? cancelToken = null)
	{
		WebSocketSending?.Invoke(this, new(data));

		await WebSocketClient.SendAsync(Encoding.UTF8.GetBytes(data), msgType, endOfMsg, cancelToken ?? CancellationToken.None);
	}

	/// <summary>
	/// WebSocket接收服务端数据
	/// </summary>
	/// <returns></returns>
	private async Task ReceiveAsync()
	{
		while (WebSocketClient.State == WebSocketState.Open)
		{
			try
			{
				using var memory = MemoryPool<byte>.Shared.Rent(1024 * 64);
				var result = await WebSocketClient.ReceiveAsync(memory.Memory, CancellationToken.None);
				if (result.MessageType == WebSocketMessageType.Text)
				{
					var json = JsonDocument.Parse(memory.Memory[..result.Count]).RootElement;
					WebSocketReceived?.Invoke(this, new(json.GetRawText()));
					await ExcuteCommandAsync(json);
					continue;
				}

				Log.Info($"[WebSocket][Receive] SocketType：{result.MessageType} | Status：{WebSocketClient.CloseStatus}");
			}
			catch (Exception e)
			{
				Log.Error($"[WebSocket][Receive] {e.Message} | Status：{WebSocketClient.CloseStatus}{Environment.NewLine}");
			}

			// 关闭代码4009表示频道服务器要求的重连，可以进行Resume
			if (WebSocketClient.CloseStatus.GetHashCode() == 4009)
			{
				IsResume = true;
			}

			WebSocketClient.Abort();
			break;
		}
		if (HeartBeatTimer.Enabled)
		{
			HeartBeatTimer.Enabled = false;
		}
		WebSocketClosed?.Invoke(this);
		Log.Warn($"[WebSocket] 重新建立到服务器的连接...");
		await Task.Delay(TimeSpan.FromSeconds(1));
		WebSocketClient = new();
		await ConnectAsync(30);
	}

	/// <summary>
	/// 根据收到的数据分析用途
	/// </summary>
	/// <param name="wssJson">Wss接收的数据</param>
	/// <returns></returns>
	private async Task ExcuteCommandAsync(JsonElement wssJson)
	{
		switch ((Opcode)wssJson.GetProperty("op").GetInt32())
		{
			case Opcode.Dispatch: // Do the message pushing.
			{
				MessageDispatched?.Invoke(this, wssJson);

				WebSocketLastSeq = wssJson.GetProperty("s").GetInt32();
				if (!wssJson.TryGetProperty("t", out var t) || !wssJson.TryGetProperty("d", out var d))
				{
					Log.Warn($"[WebSocket][Op00][Dispatch] {wssJson.GetRawText()}");
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
							Log.Warn($"[WebSocket][{type}] {data}");
							return;
						}

						Log.Debug($"[WebSocket][{type}] {data}");
						_ = MessageCenterAsync(message, type);

						break;
					}

					case RawMessageTypes.GuildCreated:
					case RawMessageTypes.GuildUpdated:
					case RawMessageTypes.GuildDeleted:
					{
						Log.Debug($"[WebSocket][{type}] {data}");
						var guild = d.Deserialize<Guild>()!;
						switch (type)
						{
							case RawMessageTypes.GuildCreated:
							case RawMessageTypes.GuildUpdated:
							{
								guild.APIPermissions = await GetGuildPermissionsAsync(guild.Id);
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
						Log.Debug($"[WebSocket][{type}] {data}");
						var channel = d.Deserialize<Channel>()!;
						ChannelEventSetTriggered?.Invoke(this, new(channel, type));

						break;
					}

					case RawMessageTypes.GuildMemberAdded:
					case RawMessageTypes.GuildMemberUpdated:
					case RawMessageTypes.GuildMemberRemoved:
					{
						Log.Debug($"[WebSocket][{type}] {data}");
						var memberWithGuild = d.Deserialize<MemberWithGuildId>()!;
						GuildMemberEventSetTriggered?.Invoke(this, new(memberWithGuild, type));

						break;
					}

					case RawMessageTypes.MessageReactionAdded:
					case RawMessageTypes.MessageReactionRemoved:
					{
						Log.Debug($"[WebSocket][{type}] {data}");
						var messageReaction = d.Deserialize<MessageReaction>()!;
						MessageReactionEventSetTriggered?.Invoke(this, new(messageReaction, type));

						break;
					}

					case RawMessageTypes.MessageAuditPassed:
					case RawMessageTypes.MessageAuditRejected:
					{
						Log.Info($"[WebSocket][{type}] {data}");
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
						Log.Info($"[WebSocket][{type}] {data}");
						AudioEventSetTriggered?.Invoke(this, wssJson);

						break;
					}

					case RawMessageTypes.Resumed:
					{
						Log.Info($"[WebSocket][Op00][RESUMED] 已恢复与服务器的连接");
						await ExcuteCommandAsync(JsonDocument.Parse($$"""{"op":{{(int)Opcode.Heartbeat}}}""").RootElement);
						ConnectionResumed?.Invoke(this, d);

						break;
					}

					case RawMessageTypes.Ready:
					{
						Log.Debug($"[WebSocket][READY] {data}");
						Log.Info($"[WebSocket][Op00] 服务端 鉴权成功");
						await ExcuteCommandAsync(JsonDocument.Parse($$"""{"op":{{(int)Opcode.Heartbeat}}}""").RootElement);
						WebSoketSessionId = d.GetProperty("session_id").GetString();

						Log.Info($"[WebSocket][GetGuilds] 获取已加入的频道列表，分页大小：100");
						string? guildNext = null;
						for (int page = 1; ; ++page)
						{
							var guilds = await GetMeGuildsAsync(guildNext, false, 100, null);
							if (guilds is null)
							{
								Log.Info($"[WebSocket][GetGuilds] 获取已加入的频道列表，第 {page:00} 页失败");
								break;
							}

							if (guilds.Count == 0)
							{
								Log.Info($"[WebSocket][GetGuilds] 获取已加入的频道列表，第 {page:00} 页为空，操作结束");
								break;
							}

							Log.Info($"[WebSocket][GetGuilds] 获取已加入的频道列表，第 {page:00} 页成功，数量：{guilds.Count}");
							Parallel.ForEach(guilds, (guild, state, i) =>
							{
								guild.APIPermissions = GetGuildPermissionsAsync(guild.Id).Result;
								Guilds[guild.Id] = guild;
							});
							guildNext = guilds.Last().Id;
						}
						Log.Info($"[WebSocket][GetGuilds] 机器人已加入 {Guilds.Count} 个频道");

						Info = d.GetProperty("user").Deserialize<User>()!;
						Info.Avatar = (await GetMeAsync(null))?.Avatar;

						// Triggers the event.
						// Here 'this' can be replaced with 'null' because the first argument is useless.
						AuthorizationPassed?.Invoke(this, new(Info));

						break;
					}

					default:
					{
						Log.Warn($"[WebSocket][{type}] 未知事件");
						break;
					}
				}

				break;
			}
			case Opcode.Heartbeat: // Send & Receive 客户端或服务端发送心跳
			{
				Log.Debug($"[WebSocket][Op01] {(wssJson.Get("d") == null ? "客户端" : "服务器")} 发送心跳包");
				HeartbeatMessageReceived?.Invoke(this, wssJson);
				await SendHeartBeatAsync();
				HeartBeatTimer.Enabled = true;

				break;
			}
			case Opcode.Identify: // Send 客户端发送鉴权
			{
				Log.Info($"[WebSocket][Op02] 客户端 发起鉴权");
				Identifying?.Invoke(this, wssJson);
				await SendIdentifyAsync();

				break;
			}
			case Opcode.Resume: // Send 客户端恢复连接
			{
				Log.Info($"[WebSocket][Op06] 客户端 尝试恢复连接..");
				IsResume = false;
				ConnectionResuming?.Invoke(this, wssJson);
				await SendResumeAsync();

				break;
			}
			case Opcode.Reconnect: // Receive 服务端通知客户端重新连接
			{
				Log.Info($"[WebSocket][Op07] 服务器 要求客户端重连");
				ConnectionReconnected?.Invoke(this, wssJson);

				break;
			}
			case Opcode.InvalidSession: // Receive 当identify或resume的时候，如果参数有错，服务端会返回该消息
			{
				Log.Warn($"[WebSocket][Op09] 客户端鉴权信息错误");
				SessionInvalid?.Invoke(this, wssJson);

				break;
			}
			case Opcode.Hello: // Receive 当客户端与网关建立ws连接之后，网关下发的第一条消息
			{
				Log.Info($"[WebSocket][Op10][成功与网关建立连接] {wssJson.GetRawText()}");
				Helloing?.Invoke(this, wssJson);
				int heartbeat_interval = wssJson.Get("d")?.Get("heartbeat_interval")?.GetInt32() ?? 30000;
				HeartBeatTimer.Interval = heartbeat_interval < 30000 ? heartbeat_interval : 30000;  // 设置心跳时间为30s
				await ExcuteCommandAsync(JsonDocument.Parse("{\"op\":" + (int)(IsResume ? Opcode.Resume : Opcode.Identify) + "}").RootElement);

				break;
			}
			case Opcode.HeartbeatACK: // Receive 当发送心跳成功之后，就会收到该消息
			{
				Log.Debug($"[WebSocket][Op11] 服务器 收到心跳包");
				HeartbeatMessageAcknowledged?.Invoke(this, wssJson);

				break;
			}
			case var opCode:
			{
				Log.Warn($"[WebSocket][OpNC] 未知操作码: {opCode}");

				break;
			}
		}
	}
}
