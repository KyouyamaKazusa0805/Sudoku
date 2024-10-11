namespace Sudoku.Concepts.Coordinates.Formatting;

/// <summary>
/// Provides with extension methods on <see cref="NotationBracket"/>.
/// </summary>
/// <seealso cref="NotationBracket"/>
public static class NotationBracketExtensions
{
	/// <summary>
	/// Try to get open bracket token.
	/// </summary>
	/// <param name="this">The bracket.</param>
	/// <returns>The open bracket token.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetOpenBracket(this NotationBracket @this)
		=> @this switch
		{
			NotationBracket.None => null,
			NotationBracket.Round => "(",
			NotationBracket.Square => "[",
			NotationBracket.Curly => "{",
			NotationBracket.Angle => "<",
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};

	/// <summary>
	/// Try to get closed bracket token.
	/// </summary>
	/// <param name="this">The bracket.</param>
	/// <returns>The closed bracket token.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument is not defined.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetClosedBracket(this NotationBracket @this)
		=> @this switch
		{
			NotationBracket.None => null,
			NotationBracket.Round => ")",
			NotationBracket.Square => "]",
			NotationBracket.Curly => "}",
			NotationBracket.Angle => ">",
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
