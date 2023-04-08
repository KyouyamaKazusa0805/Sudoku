namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with extension methods on <see cref="Technique"/>.
/// </summary>
/// <seealso cref="Technique"/>
public static class TechniqueExtensions
{
	/// <summary>
	/// Try to get the name of the current <see cref="Technique"/>.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>
	/// The name of the current technique.
	/// Return <see langword="null"/> if the current technique does not contain a valid name in resource dictionary.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetName(this Technique @this) => R[@this.ToString()];

	/// <summary>
	/// Try to get the abbreviation of the current <see cref="Technique"/>.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>The abbreviation of the current technique.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetAbbreviation(this Technique @this) => R[$"TechniqueAbbr_{@this}"];

	/// <summary>
	/// Try to get all aliases of the current <see cref="Technique"/>.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>
	/// All possible aliases of the current technique.
	/// If the technique does not contain any aliases, the return value will be <see langword="null"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string[]? GetAliases(this Technique @this)
		=> R[$"TechniqueAlias_{@this}"]?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
