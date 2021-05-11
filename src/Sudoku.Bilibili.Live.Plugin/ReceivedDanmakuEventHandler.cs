namespace Sudoku.Bilibili.Live.Plugin
{
	/// <summary>
	/// Indicates the methods that triggered while receiving a danmaku.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event arguments.</param>
	public delegate void ReceivedDanmakuEventHandler(object? sender, ReceivedDanmakuEventArgs e);
}
