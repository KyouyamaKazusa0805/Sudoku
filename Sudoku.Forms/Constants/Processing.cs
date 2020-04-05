namespace Sudoku.Forms.Constants
{
	/// <summary>
	/// Provides all <see langword="const"/> or <see langword="readonly"/> values
	/// for internal processing.
	/// </summary>
	internal static class Processing
	{
		/// <summary>
		/// The splitter in using <see cref="string.Split(char[]?)"/> or other split methods
		/// where the method contain <see cref="char"/>[]? type parameter.
		/// </summary>
		/// <seealso cref="string.Split(char[]?)"/>
		public static readonly char[] Splitter = new[] { '\r', '\n' };
	}
}
