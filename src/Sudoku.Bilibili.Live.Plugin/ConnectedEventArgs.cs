namespace Sudoku.Bilibili.Live.Plugin
{
	/// <summary>
	/// Provides event arguments for <see cref="ConnectedEventHandler"/>.
	/// </summary>
	/// <seealso cref="ConnectedEventHandler"/>
	public readonly struct ConnectedEventArgs
	{
		/// <summary>
		/// Indicates the room ID.
		/// </summary>
		public int RoomId { get; init; }


		/// <summary>
		/// Implicit cast from <see cref="int"/> to <see cref="ConnectedEventArgs"/>.
		/// </summary>
		/// <param name="roomId">The room ID.</param>
		public static implicit operator ConnectedEventArgs(int roomId) => new() { RoomId = roomId };
	}
}
