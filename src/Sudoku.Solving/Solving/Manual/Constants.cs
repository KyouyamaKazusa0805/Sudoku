namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides with the constants.
/// </summary>
internal static class Constants
{
	/// <summary>
	/// Indicates the invalid first set value
	/// after called <see cref="TrailingZeroCount(int)"/> and <see cref="TrailingZeroCount(uint)"/>.
	/// </summary>
	/// <seealso cref="TrailingZeroCount(int)"/>
	/// <seealso cref="TrailingZeroCount(uint)"/>
	public const int InvalidFirstSet = 32;
}
