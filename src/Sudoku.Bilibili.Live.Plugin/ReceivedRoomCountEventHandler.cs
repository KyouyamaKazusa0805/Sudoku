namespace Sudoku.Bilibili.Live.Plugin
{
	/// <summary>
	/// Indicates the methods that triggered while receiving the room count.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event arguments.</param>
	public delegate void ReceivedRoomCountEventHandler(object? sender, ReceivedRoomCountEventArgs e);
}
