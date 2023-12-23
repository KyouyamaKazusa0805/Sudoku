namespace Sudoku.Analytics;

/// <summary>
/// Provides with extension methods on <see cref="ConclusionType"/>.
/// </summary>
/// <seealso cref="ConclusionType"/>
public static class ConclusionTypeExtensions
{
	/// <summary>
	/// Gets the notation of the conclusion type.
	/// </summary>
	/// <param name="this">The conclusion type kind.</param>
	/// <returns>The string representation of the conclusion kind.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="this"/> is not defined in enumeration type.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Notation(this ConclusionType @this)
		=> @this switch
		{
			Assignment => " = ",
			Elimination => " <> ",
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
