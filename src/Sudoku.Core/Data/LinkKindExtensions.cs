namespace Sudoku.Data;

/// <summary>
/// Provides extension methods on <see cref="LinkKind"/>.
/// </summary>
/// <seealso cref="LinkKind"/>
public static class LinkKindExtensions
{
	/// <summary>
	/// Gets the notation of the link kind.
	/// </summary>
	/// <param name="this">The link kind.</param>
	/// <returns>The notation.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the link kind is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetNotation(this LinkKind @this) =>
		@this switch
		{
			LinkKind.Default => " -> ",
			LinkKind.Weak or LinkKind.Line => " -- ",
			LinkKind.Strong or LinkKind.ConjugatePair => " == ",
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
