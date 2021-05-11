namespace Sudoku.Bilibili.Live.Plugin
{
	/// <summary>
	/// Provides event arguments for <see cref="ReceivedDanmakuEventHandler"/>.
	/// </summary>
	/// <seealso cref="ReceivedDanmakuEventHandler"/>
	public readonly struct ReceivedDanmakuEventArgs
	{
		/// <summary>
		/// Indicates the danmaku information instance.
		/// </summary>
		public DanmakuInfo Info { get; init; }


		/// <summary>
		/// Implicit cast from <see cref="DanmakuInfo"/> to <see cref="ReceivedDanmakuEventArgs"/>.
		/// </summary>
		/// <param name="info">The information instance.</param>
		public static implicit operator ReceivedDanmakuEventArgs(DanmakuInfo info) => new() { Info = info };
	}
}
