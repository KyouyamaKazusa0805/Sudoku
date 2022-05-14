namespace Sudoku.Bot.Communication;

/// <summary>
/// Defines an enumeration type that holds the basic operations for the whole messaging.
/// </summary>
public enum Opcode
{
	/// <summary>
	/// Indicates the operation that the server dispatches the message.
	/// </summary>
	Dispatch = 0,

	/// <summary>
	/// Indicates the operation that the server/client sends/receives the heartbeat.
	/// </summary>
	Heartbeat = 1,

	/// <summary>
	/// Indicates the operation that the client sends the authorization message to the server.
	/// </summary>
	Identify = 2,

	/// <summary>
	/// Indicates the operation that the client resumes the connection.
	/// </summary>
	Resume = 6,

	/// <summary>
	/// Indicates the operation that the server requires the client reconnecting.
	/// </summary>
	Reconnect = 7,

	/// <summary>
	/// Indicates the operation that the session is invalid when <see cref="Identify"/>ing or <see cref="Resume"/>ing.
	/// </summary>
	InvalidSession = 9,

	/// <summary>
	/// Indicates the operation that the helloing message sending by gateway.
	/// </summary>
	/// <remarks>
	/// A "hello" message is the first message when connection is successfully created.
	/// </remarks>
	Hello = 10,

	/// <summary>
	/// Indicates the operation that report the acknowledgement message the client successfully sends the heartbeat.
	/// </summary>
	HeartbeatACK = 11
}
