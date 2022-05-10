namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Provides with data used by <see cref="WebSocketSendingEventHandler"/>.
/// </summary>
/// <seealso cref="WebSocketSendingEventHandler"/>
public sealed class WebSocketSendingEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="WebSocketSendingEventArgs"/> instance via the data.
	/// </summary>
	/// <param name="data">The data.</param>
	public WebSocketSendingEventArgs(string data) => Data = data;


	/// <summary>
	/// Indicates the data sending.
	/// </summary>
	public string Data { get; }
}
