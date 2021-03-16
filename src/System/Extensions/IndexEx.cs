using Sudoku.DocComments;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Index"/>.
	/// </summary>
	/// <seealso cref="Index"/>
	public static class IndexEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">The index.</param>
		/// <param name="isFromEnd">
		/// Indicates whether the current index is from end.
		/// </param>
		/// <param name="value">Indicates the value.</param>
		public static void Deconstruct(this in Index @this, out bool isFromEnd, out int value)
		{
			isFromEnd = @this.IsFromEnd;
			value = @this.Value;
		}
	}
}
