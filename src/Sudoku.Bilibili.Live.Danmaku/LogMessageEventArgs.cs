namespace Sudoku.Bilibili.Live.Danmaku
{
	/// <summary>
	/// Provides event arguments for <see cref="LogMessageEventHandler"/>.
	/// </summary>
	/// <seealso cref="LogMessageEventHandler"/>
	public readonly struct LogMessageEventArgs
	{
		/// <summary>
		/// Indicates the inner message.
		/// </summary>
		public string? Message { get; init; }


		/// <summary>
		/// Implicit cast from <see cref="string"/>? to <see cref="LogMessageEventArgs"/>.
		/// </summary>
		/// <param name="message">The message.</param>
		public static implicit operator LogMessageEventArgs(string? message) => new() { Message = message };
	}
}
