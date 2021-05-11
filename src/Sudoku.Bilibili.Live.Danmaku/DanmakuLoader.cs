using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sudoku.Bilibili.Live.Danmaku.EndianBitConverters;
using Sudoku.Bilibili.Live.Danmaku.Extensions;
using Sudoku.Bilibili.Live.Plugin;

namespace Sudoku.Bilibili.Live.Danmaku
{
	/// <summary>
	/// Encapsulates a danmaku loader.
	/// </summary>
	public class DanmakuLoader
	{
		/// <summary>
		/// Encapsulates a <see cref="HttpClient"/>.
		/// </summary>
		private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(5) };

		/// <summary>
		/// Indicates the random number generator.
		/// </summary>
		private static readonly Random Rng = new();


		/// <summary>
		/// Indicates the last live room ID.
		/// </summary>
		private static int _lastRoomId;

		/// <summary>
		/// Indicates the last server that connected.
		/// </summary>
		private static string _lastServer = string.Empty;


		/// <summary>
		/// Indicates whether the instance will save the log in the debug mode.
		/// </summary>
		private readonly bool _debuglog = true;

		/// <summary>
		/// Indicates the protocol version.
		/// </summary>
		private readonly short _protocolversion = 2;

		/// <summary>
		/// Indicates the URI that gets the inner value.
		/// </summary>
		private readonly string _cidInfoUri = "https://api.live.bilibili.com/room/v1/Danmu/getConf?room_id=";

		/// <summary>
		/// Indicates the default hosts.
		/// </summary>
		private readonly string[] _defaultHosts = new[] { "livecmt-2.bilibili.com", "livecmt-1.bilibili.com" };


		/// <summary>
		/// Indicates whether the current client has connected to the server.
		/// </summary>
		private bool _connected = false;

		/// <summary>
		/// Indicates the chat port.
		/// </summary>
		private int _chatPort = 2243;

		/// <summary>
		/// Indicates the chat host.
		/// </summary>
		private string _chatHost = "chat.bilibili.com";

		/// <summary>
		/// Indicates the client.
		/// </summary>
		private TcpClient? _client;

		/// <summary>
		/// Indicates the net stream.
		/// </summary>
		private Stream? _netStream;

		/// <summary>
		/// Indicates the inner error.
		/// </summary>
		public Exception? _error;


		/// <summary>
		/// Indicates the event to be triggered while receiving a danmaku.
		/// </summary>
		public event ReceivedDanmakuEventHandler? ReceivedDanmaku;

		/// <summary>
		/// Indicates the event to be triggered while disconnecting.
		/// </summary>
		public event DisconnectEventHandler? Disconnected;

		/// <summary>
		/// Indicates the event to be triggered while receiving the room count.
		/// </summary>
		public event ReceivedRoomCountEventHandler? ReceivedRoomCount;

		/// <summary>
		/// Indicates the event to be triggered while logging the message.
		/// </summary>
		public event LogMessageEventHandler? LogMessage;


		/// <summary>
		/// Connects to the server, with the specified live room ID.
		/// </summary>
		/// <param name="roomId">The room ID.</param>
		/// <returns>A task that handles that operation.</returns>
		/// <exception cref="InvalidOperationException">
		/// Throws when the room has already connected, or the chat host <see cref="_chatHost"/>
		/// keeps <see langword="null"/> or empty value.
		/// </exception>
		public async Task<bool> ConnectAsync(int roomId)
		{
			try
			{
				if (_connected)
				{
					throw new InvalidOperationException("The room has already connected.");
				}

				int channelId = roomId;
				string token = "";
				if (channelId != _lastRoomId)
				{
					try
					{
						string req = await HttpClient.GetStringAsync(_cidInfoUri + channelId);
						var roomobj = JObject.Parse(req);
						token = roomobj["data"]!["token"]!.ToString();
						_chatHost = roomobj["data"]!["host"]!.ToString();
						_chatPort = roomobj["data"]!["port"]!.Value<int>();
						if (string.IsNullOrEmpty(_chatHost))
						{
							throw new InvalidOperationException("The chat host keeps null or empty value.");
						}
					}
					catch (WebException ex)
					{
						_chatHost = _defaultHosts[Rng.Next(_defaultHosts.Length)];

						LogMessage?.Invoke(
							this,
							ex.Response is HttpWebResponse { StatusCode: HttpStatusCode.NotFound }
							? "It seems that the ID of that live room doesn't exist."
							: "It seems that the BiliBili server raises an error."
						);
					}
					catch // Other exceptions, such as XML parser error.
					{
						_chatHost = _defaultHosts[new Random().Next(_defaultHosts.Length)];

						LogMessage?.Invoke(this, "Other exception raised.");
					}
				}
				else
				{
					_chatHost = _lastServer;
				}

				_client = new();

				var ipAddress = await Dns.GetHostAddressesAsync(_chatHost);
				var random = new Random();
				int idx = random.Next(ipAddress.Length);
				await _client.ConnectAsync(ipAddress[idx], _chatPort);

				_netStream = Stream.Synchronized(_client.GetStream());

				if (await SendJoinChannelAsync(channelId, token))
				{
					_connected = true;
					_ = HeartbeatLoopAsync();
					_ = ReceiveMessageLoopAsync();
					_lastServer = _chatHost;
					_lastRoomId = roomId;
					return true;
				}

				return false;
			}
			catch (Exception ex)
			{
				_error = ex;
				return false;
			}
		}

		/// <summary>
		/// Receive the message loop.
		/// </summary>
		/// <returns>The task of this operation.</returns>
		private async Task ReceiveMessageLoopAsync()
		{
			if (_netStream is null)
			{
				return;
			}

			try
			{
				byte[] stableBuffer = new byte[16], buffer = new byte[4096];
				while (_connected)
				{
					await _netStream.ReadBAsync(stableBuffer, 0, 16);
					var protocol = DanmakuProtocol.FromBuffer(stableBuffer);
					if (protocol.PacketLength < 16)
					{
						throw new NotSupportedException(
							$"Failed to execute on this protocol, of length {protocol.PacketLength}."
						);
					}

					int payloadlength = protocol.PacketLength - 16;
					if (payloadlength == 0)
					{
						continue;
					}

					buffer = new byte[payloadlength];

					await _netStream.ReadBAsync(buffer, 0, payloadlength);
					if (protocol is { Version: 2, Action: 5 })
					{
						// Handles deflate message.
						using var ms = new MemoryStream(buffer, 2, payloadlength - 2); // Skip 0x78 and 0xDA.
						using var deflate = new DeflateStream(ms, CompressionMode.Decompress);
						byte[] headerbuffer = new byte[16];

						try
						{
							while (true)
							{
								await deflate.ReadBAsync(headerbuffer, 0, 16);
								var protocolIn = DanmakuProtocol.FromBuffer(headerbuffer);
								payloadlength = protocolIn.PacketLength - 16;
								byte[] danmakubuffer = new byte[payloadlength];
								await deflate.ReadBAsync(danmakubuffer, 0, payloadlength);
								ProcessDanmaku(protocol.Action, danmakubuffer);
							}
						}
						catch
						{
						}
					}
					else
					{
						ProcessDanmaku(protocol.Action, buffer);
					}
				}
			}
			catch (Exception ex)
			{
				_error = ex;
				InnerDisconnect();
			}
		}

		private void ProcessDanmaku(int action, byte[] buffer)
		{
			switch (action)
			{
				case 3: // (OpHeartbeatReply)
				{
					uint viewerCount = EndianBitConverter.BigEndian.ToUInt32(buffer, 0);
					ReceivedRoomCount?.Invoke(this, viewerCount);

					break;
				}
				case 5: //playerCommand (OpSendMsgReply)
				{
					string json = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
					if (_debuglog)
					{
						Console.WriteLine(json);
					}

					try
					{
						var danmakuModel = new DanmakuInfo(json, 2);
						ReceivedDanmaku?.Invoke(this, danmakuModel);
					}
					catch
					{
					}

					break;
				}
				case 8: // (OpAuthReply)
				default:
				{
					break;
				}
			}
		}

		private async Task HeartbeatLoopAsync()
		{
			try
			{
				while (_connected)
				{
					await SendHeartbeatAsync();

					await Task.Delay(30000);
				}
			}
			catch (Exception ex)
			{
				_error = ex;

				InnerDisconnect();
			}
		}

		/// <summary>
		/// To disconnect the client.
		/// </summary>
		public void Disconnect()
		{
			try
			{
			}
			finally
			{
				_connected = false;
				_client?.Close();
				_netStream = null;
			}
		}

		/// <summary>
		/// The inner method to disconnect the client.
		/// </summary>
		private void InnerDisconnect()
		{
			if (_connected)
			{
#if DEBUG
				Debug.WriteLine("Disconnected");
#endif

				_connected = false;
				_client?.Close();
				_netStream = null;

				Disconnected?.Invoke(this, _error);
			}
		}

		/// <summary>
		/// Send the heartbeat asynchronously.
		/// </summary>
		/// <returns>The task of this operation.</returns>
		private async Task SendHeartbeatAsync()
		{
			await SendSocketDataAsync(2);

#if DEBUG
			Debug.WriteLine("Message Sent: Heartbeat");
#endif
		}

		/// <summary>
		/// Send the socket data asynchronously.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="body">The body.</param>
		/// <returns>The task of this operation.</returns>
		private Task SendSocketDataAsync(int action, string body = "") =>
			SendSocketDataAsync(0, 16, _protocolversion, action, 1, body);

		/// <summary>
		/// Send the socket data asynchronously.
		/// </summary>
		/// <param name="packetlength">The packet length.</param>
		/// <param name="magic">The magic number.</param>
		/// <param name="version">The version.</param>
		/// <param name="action">The action.</param>
		/// <param name="param">The parameter.</param>
		/// <param name="body">The real body.</param>
		/// <returns>The task of this operation.</returns>
		private async Task SendSocketDataAsync(
			int packetlength, short magic, short version, int action, int param = 1, string body = "")
		{
			if (_netStream is null)
			{
				return;
			}

			byte[] playload = Encoding.UTF8.GetBytes(body);
			if (packetlength == 0)
			{
				packetlength = playload.Length + 16;
			}

			byte[] buffer = new byte[packetlength];
			using var ms = new MemoryStream(buffer);
			byte[] b = EndianBitConverter.BigEndian.GetBytes(buffer.Length);

			await ms.WriteAsync(b.AsMemory(0, 4));
			b = EndianBitConverter.BigEndian.GetBytes(magic);
			await ms.WriteAsync(b.AsMemory(0, 2));
			b = EndianBitConverter.BigEndian.GetBytes(version);
			await ms.WriteAsync(b.AsMemory(0, 2));
			b = EndianBitConverter.BigEndian.GetBytes(action);
			await ms.WriteAsync(b.AsMemory(0, 4));
			b = EndianBitConverter.BigEndian.GetBytes(param);
			await ms.WriteAsync(b.AsMemory(0, 4));

			if (playload.Length > 0)
			{
				await ms.WriteAsync(playload);
			}

			await _netStream.WriteAsync(buffer);
		}

		/// <summary>
		/// Send join channel.
		/// </summary>
		/// <param name="channelId">The channel ID.</param>
		/// <param name="token">The token.</param>
		/// <returns>The task of this operation.</returns>
		private async Task<bool> SendJoinChannelAsync(int channelId, string token)
		{
			await SendSocketDataAsync(7, JsonConvert.SerializeObject(new
			{
				roomid = channelId,
				uid = 0,
				protover = 2,
				token,
				platform = "danmuji"
			}));

			return true;
		}
	}
}
