namespace Sudoku.Data;

/// <summary>
/// Provides extension methods on <see cref="ConclusionType"/>.
/// </summary>
/// <seealso cref="ConclusionType"/>
public static class ConclusionTypeExtensions
{
	/// <summary>
	/// Gets the notation of the conclusion type.
	/// </summary>
	/// <param name="this">The conclusion type.</param>
	/// <returns>The notation of the conclusion type.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument is not <see cref="ConclusionType.Assignment"/>
	/// or <see cref="ConclusionType.Elimination"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetNotation(this ConclusionType @this) =>
		@this switch
		{
			ConclusionType.Assignment => " = ",
			ConclusionType.Elimination => " <> ",
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
