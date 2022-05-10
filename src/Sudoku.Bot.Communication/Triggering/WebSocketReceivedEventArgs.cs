namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Provides with the event data for <see cref="WebSocketReceivedEventHandler"/>.
/// </summary>
/// <seealso cref="WebSocketReceivedEventHandler"/>
public sealed class WebSocketReceivedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="WebSocketReceivedEventArgs"/> instance via the specified data.
	/// </summary>
	/// <param name="data">The data.</param>
	public WebSocketReceivedEventArgs(string data) => Data = data;


	/// <summary>
	/// Indicates the data received.
	/// </summary>
	public string Data { get; }
}
