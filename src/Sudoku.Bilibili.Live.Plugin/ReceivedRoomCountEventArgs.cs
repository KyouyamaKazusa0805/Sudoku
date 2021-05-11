namespace Sudoku.Bilibili.Live.Plugin
{
	/// <summary>
	/// Provides event arguments for <see cref="ReceivedDanmakuEventHandler"/>.
	/// </summary>
	/// <seealso cref="ReceivedDanmakuEventHandler"/>
	public readonly struct ReceivedRoomCountEventArgs
	{
		/// <summary>
		/// Indicates the user count.
		/// </summary>
		public uint UserCount { get; init; }


		/// <summary>
		/// Implicit cast from <see cref="uint"/> to <see cref="ReceivedRoomCountEventArgs"/>.
		/// </summary>
		/// <param name="userCount">The user count.</param>
		public static implicit operator ReceivedRoomCountEventArgs(uint userCount) => new()
		{
			UserCount = userCount
		};
	}
}
