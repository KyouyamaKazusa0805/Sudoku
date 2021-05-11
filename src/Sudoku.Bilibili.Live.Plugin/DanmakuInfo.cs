using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Sudoku.Bilibili.Live.Plugin
{
	/// <summary>
	/// Encapsulates a danmaku information instance.
	/// </summary>
	/// <param name="RawData">Indicates the raw data (i.e. JSON string).</param>
	public sealed record DanmakuInfo(string RawData)
	{
		/// <summary>
		/// Indicates the regular expression instance that matches the entry effect.
		/// </summary>
		public static readonly Regex EntryEffectRegex = new(@"\<%(.+?)%\>");


		/// <summary>
		/// Indicates the message type.
		/// </summary>
		public MessageType MsgType { get; init; }

		/// <summary>
		/// Indicates the interact type.
		/// </summary>
		public InteractType InteractType { get; init; }

		/// <summary>
		/// Indicates the guard level of that user.
		/// </summary>
		/// <remarks>
		/// This property contains the value when <see cref="MsgType"/> holds the below cases:
		/// <list type="bullet">
		/// <item><see cref="MessageType.Comment"/></item>
		/// <item><see cref="MessageType.WelcomeGuard"/></item>
		/// <item><see cref="MessageType.JoinGuarding"/></item>
		/// </list>
		/// </remarks>
		/// <seealso cref="MsgType"/>
		public GuardLevel GuardLevel { get; init; }

		/// <summary>
		/// Indicates the user ID that corresponding user raises a danmaku.
		/// </summary>
		/// <remarks>
		/// This property contains the value when <see cref="MsgType"/> holds the below cases:
		/// <list type="bullet">
		/// <item><see cref="MessageType.Comment"/></item>
		/// <item><see cref="MessageType.GiftSend"/></item>
		/// <item><see cref="MessageType.Welcome"/></item>
		/// <item><see cref="MessageType.WelcomeGuard"/></item>
		/// <item><see cref="MessageType.JoinGuarding"/></item>
		/// <item><see cref="MessageType.Interact"/></item>
		/// </list>
		/// </remarks>
		/// <seealso cref="MsgType"/>
		public int UserID { get; init; }

		/// <summary>
		/// Indicates the number of the gift sent or the number of the members that
		/// join the team (i.e. pinyin: jian dui).
		/// </summary>
		/// <remarks>
		/// This property contains the value when <see cref="MsgType"/> holds the below cases:
		/// <list type="bullet">
		/// <item><see cref="MessageType.GiftSend"/></item>
		/// <item><see cref="MessageType.JoinGuarding"/></item>
		/// </list>
		/// </remarks>
		/// <seealso cref="MsgType"/>
		public int GiftCount { get; init; }

		/// <summary>
		/// Indicates whether the user is the room manager (administrator).
		/// </summary>
		/// <remarks>
		/// This property contains the value when <see cref="MsgType"/> holds the below cases:
		/// <list type="bullet">
		/// <item><see cref="MessageType.Comment"/></item>
		/// <item><see cref="MessageType.GiftSend"/></item>
		/// </list>
		/// </remarks>
		/// <seealso cref="MsgType"/>
		public bool IsAdmin { get; init; }

		/// <summary>
		/// Indicates whether the user is VIP (i.e. pinyin: lao ye).
		/// </summary>
		/// <remarks>
		/// This property contains the value when <see cref="MsgType"/> holds the below cases:
		/// <list type="bullet">
		/// <item><see cref="MessageType.Comment"/></item>
		/// <item><see cref="MessageType.Welcome"/></item>
		/// </list>
		/// </remarks>
		/// <seealso cref="MsgType"/>
		public bool IsVip { get; init; }

		/// <summary>
		/// Indicates the name of the gift sent.
		/// </summary>
		public string? GiftName { get; init; }

		/// <summary>
		/// Indicates the room ID.
		/// </summary>
		public string? RoomId { get; init; }

		/// <summary>
		/// Indicates the content text.
		/// </summary>
		/// <remarks>
		/// This property contains the value when <see cref="MsgType"/> holds the below cases:
		/// <list type="bullet">
		/// <item><see cref="MessageType.Comment"/></item>
		/// </list>
		/// </remarks>
		/// <seealso cref="MsgType"/>
		public string? CommentText { get; init; }

		/// <summary>
		/// Indicates the user that raises a danmaku.
		/// </summary>
		/// <remarks>
		/// This property contains the value when <see cref="MsgType"/> holds the below cases:
		/// <list type="bullet">
		/// <item><see cref="MessageType.Comment"/></item>
		/// <item><see cref="MessageType.GiftSend"/></item>
		/// <item><see cref="MessageType.Welcome"/></item>
		/// <item><see cref="MessageType.WelcomeGuard"/></item>
		/// <item><see cref="MessageType.JoinGuarding"/></item>
		/// <item><see cref="MessageType.Interact"/></item>
		/// </list>
		/// </remarks>
		/// <seealso cref="MsgType"/>
		public string? UserName { get; init; }

		/// <summary>
		/// Indicates the inner JSON version. The value is always <c>2</c>.
		/// </summary>
		public int JsonVersion { get; init; }

		/// <summary>
		/// Indicates the SC keep time.
		/// </summary>
		public int ScKeepTime { get; init; }

		/// <summary>
		/// Indicates the JSON object about the danmaku receiving operation.
		/// </summary>
		public JToken? RawDataJToken { get; init; }

		/// <summary>
		/// Indicates the rank of the gift sent.
		/// </summary>
		/// <remarks>
		/// This property contains the value when <see cref="MsgType"/> holds the below cases:
		/// <list type="bullet">
		/// <item><see cref="MessageType.GiftTop"/></item>
		/// </list>
		/// </remarks>
		/// <seealso cref="MsgType"/>
		public IList<GiftRank>? GiftRanking { get; init; }


		/// <summary>
		/// Initializes a <see cref="DanmakuInfo"/> instance with the JSON and its version.
		/// </summary>
		/// <param name="json">The JSON received.</param>
		/// <param name="version">The version of the JSON.</param>
		/// <exception cref="NotSupportedException">
		/// Throws when the specified JSON version doesn't support. In fact,
		/// the argument <paramref name="version"/> must hold the value <c>1</c> or <c>2</c>.
		/// </exception>
		public DanmakuInfo(string json, int version = 1) : this(json)
		{
			JsonVersion = version;

			switch (version)
			{
				case 1:
				{
					var obj = JArray.Parse(json);

					CommentText = obj[1].ToString();
					UserName = obj[2]![1]!.ToString();
					MsgType = MessageType.Comment;
					RawDataJToken = obj;

					break;
				}
				case 2:
				{
					var obj = JObject.Parse(json);

					RawDataJToken = obj;
					switch (obj["cmd"]!.ToString())
					{
						case DanmakuJsonCommands.Live:
						{
							MsgType = MessageType.LiveStart;
							RoomId = obj["roomid"]!.ToString();

							break;
						}
						case DanmakuJsonCommands.Preparing:
						{
							MsgType = MessageType.LiveEnd;
							RoomId = obj["roomid"]!.ToString();

							break;
						}
						case DanmakuJsonCommands.DanmakuMessage:
						{
							MsgType = MessageType.Comment;
							CommentText = obj["info"]![1]!.ToString();
							UserID = obj["info"]![2]![0]!.ToObject<int>();
							UserName = obj["info"]![2]![1]!.ToString();
							IsAdmin = obj["info"]![2]![2]!.ToString() == "1";
							IsVip = obj["info"]![2]![3]!.ToString() == "1";
							GuardLevel = (GuardLevel)obj["info"]![7]!.ToObject<int>();

							break;
						}
						case DanmakuJsonCommands.SendingGift:
						{
							MsgType = MessageType.GiftSend;
							GiftName = obj["data"]!["giftName"]!.ToString();
							UserName = obj["data"]!["uname"]!.ToString();
							UserID = obj["data"]!["uid"]!.ToObject<int>();
							GiftCount = obj["data"]!["num"]!.ToObject<int>();

							break;
						}
						case DanmakuJsonCommands.ComboSending:
						{
							MsgType = MessageType.Unknown;

							break;
						}
						case DanmakuJsonCommands.GiftRanking:
						{
							MsgType = MessageType.GiftTop;

							var allTops = obj["data"]!.ToList();

							GiftRanking = new List<GiftRank>();
							foreach (var top in allTops)
							{
								GiftRanking.Add(new()
								{
									Uid = top.Value<int>("uid"),
									UserName = top.Value<string>("uname")!,
									Cost = top.Value<decimal>("coin")
								});
							}

							break;
						}
						case DanmakuJsonCommands.Welcome:
						{
							MsgType = MessageType.Welcome;

							UserName = obj["data"]!["uname"]!.ToString();
							UserID = obj["data"]!["uid"]!.ToObject<int>();
							IsVip = true;
							IsAdmin = obj["data"]!["isadmin"]?.ToString() is "1";

							break;
						}
						case DanmakuJsonCommands.WelcomeGuard:
						{
							MsgType = MessageType.WelcomeGuard;

							UserName = obj["data"]!["username"]!.ToString();
							UserID = obj["data"]!["uid"]!.ToObject<int>();
							GuardLevel = (GuardLevel)obj["data"]!["guard_level"]!.ToObject<int>();

							break;
						}
						case DanmakuJsonCommands.EntryEffect:
						{
							string msg = obj["data"]!["copy_writing"]!.ToString();
							switch (EntryEffectRegex.Match(msg))
							{
								/*indexer-pattern*/
								case { Success: true, Groups: { Count: >= 2 } groups }
								when groups[1] is { Value: var value }:
								{
									MsgType = MessageType.WelcomeGuard;
									UserName = value;
									UserID = obj["data"]!["uid"]!.ToObject<int>();
									GuardLevel = (GuardLevel)obj["data"]!["privilege_type"]!.ToObject<int>();

									break;
								}
								default:
								{
									MsgType = MessageType.Unknown;

									break;
								}
							}

							break;
						}
						case DanmakuJsonCommands.GuardBuy:
						{
							MsgType = MessageType.JoinGuarding;

							UserID = obj["data"]!["uid"]!.ToObject<int>();
							UserName = obj["data"]!["username"]!.ToString();
							GuardLevel = (GuardLevel)obj["data"]!["guard_level"]!.ToObject<int>();
							GiftName = GuardLevel switch
							{
								GuardLevel.Captain => "舰长",
								GuardLevel.Prefector => "提督",
								GuardLevel.Governer => "总督",
								_ => string.Empty
							};
							GiftCount = obj["data"]!["num"]!.ToObject<int>();
							break;
						}
						case DanmakuJsonCommands.SuperChatMessage:
						case DanmakuJsonCommands.SuperChatMessageJp:
						{
							MsgType = MessageType.SuperChat;
							CommentText = obj["data"]!["message"]?.ToString();
							UserID = obj["data"]!["uid"]!.ToObject<int>();
							UserName = obj["data"]!["user_info"]!["uname"]!.ToString();
							ScKeepTime = obj["data"]!["time"]!.ToObject<int>();

							break;
						}
						case DanmakuJsonCommands.Interact:
						{
							MsgType = MessageType.Interact;
							UserName = obj["data"]!["uname"]!.ToString();
							UserID = obj["data"]!["uid"]!.ToObject<int>();
							InteractType = (InteractType)obj["data"]!["msg_type"]!.ToObject<int>();

							break;
						}
						case DanmakuJsonCommands.WarningFromSuperRoomManager:
						{
							MsgType = MessageType.WarningFromSuperRoomManager;
							CommentText = obj["msg"]?.ToString();

							break;
						}
						case DanmakuJsonCommands.LiveEnd:
						{
							MsgType = MessageType.LiveEnd;
							CommentText = obj["msg"]?.ToString();

							break;
						}
						/*slice-pattern*/
						case var c when c.StartsWith(DanmakuJsonCommands.DanmakuMessage):
						{
							MsgType = MessageType.Comment;
							CommentText = obj["info"]![1]!.ToString();
							UserID = obj["info"]![2]![0]!.ToObject<int>();
							UserName = obj["info"]![2]![1]!.ToString();
							IsAdmin = obj["info"]![2]![2]!.ToString() == "1";
							IsVip = obj["info"]![2]![3]!.ToString() == "1";
							GuardLevel = (GuardLevel)obj["info"]![7]!.ToObject<int>();

							break;
						}
						default:
						{
							MsgType = MessageType.Unknown;

							break;
						}
					}

					break;
				}
				default:
				{
					throw new NotSupportedException("The specified version JSON doesn't support.");
				}
			}
		}
	}
}
