using System;

namespace Sudoku.Bilibili.Live.Plugin
{
	/// <summary>
	/// Provides event arguments for <see cref="DisconnectEventHandler"/>.
	/// </summary>
	/// <seealso cref="DisconnectEventHandler"/>
	public readonly struct DisconnectEventArgs
	{
		/// <summary>
		/// Indicates the empty instance.
		/// </summary>
		public static readonly DisconnectEventArgs Empty = default;


		/// <summary>
		/// Indicates the inner error.
		/// </summary>
		public Exception? Error { get; init; }


		/// <summary>
		/// Implicit cast from <see cref="Exception"/> to <see cref="DisconnectEventArgs"/>.
		/// </summary>
		/// <param name="error">The inner error.</param>
		public static implicit operator DisconnectEventArgs(Exception? error) => new() { Error = error };
	}
}
