namespace Sudoku.Bot.Communication;

/// <summary>
/// 枚举操作码
/// </summary>
public enum Opcode
{
	/// <summary>
	/// 服务端进行消息推送
	/// <para>客户端操作：Receive</para>
	/// </summary>
	Dispatch = 0,

	/// <summary>
	/// 客户端或服务端发送心跳
	/// <para>客户端操作：Send/Receive</para>
	/// </summary>
	Heartbeat = 1,

	/// <summary>
	/// 客户端发送鉴权
	/// <para>客户端操作：Send</para>
	/// </summary>
	Identify = 2,

	/// <summary>
	/// 客户端恢复连接
	/// <para>客户端操作：Send</para>
	/// </summary>
	Resume = 6,

	/// <summary>
	/// 服务端通知客户端重新连接
	/// <para>客户端操作：Receive</para>
	/// </summary>
	Reconnect = 7,

	/// <summary>
	/// 当identify或resume的时候，如果参数有错，服务端会返回该消息
	/// <para>客户端操作：Receive</para>
	/// </summary>
	InvalidSession = 9,

	/// <summary>
	/// 当客户端与网关建立ws连接之后，网关下发的第一条消息
	/// <para>客户端操作：Receive</para>
	/// </summary>
	Hello = 10,

	/// <summary>
	/// 当发送心跳成功之后，就会收到该消息
	/// <para>客户端操作：Receive</para>
	/// </summary>
	HeartbeatACK = 11,
}
